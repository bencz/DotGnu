/*
 * FileKeyProvider.cs - Implementation of the
 *			"Microsoft.Win32.FileKeyProvider" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace Microsoft.Win32
{

#if CONFIG_WIN32_SPECIFICS

using System;
using System.Collections;
using System.IO;
using System.Security;
using Platform;

// This class implements a file-based registry on disk.  The registry
// is stored within the "$HOME/.cli" directory.  This can be overridden
// with the "CLI_STORAGE_ROOT" environment variable.
//
// Each registry key is stored as a sub-directory within the filesystem.
// Sub-keys are represented as sub-directories underneath their parent.
//
// Values are stored in a file called "values.reg" within the sub-directory
// that corresponds to the registry key.  The format is XML-based, with
// the following structure:
//
//    <values>
//      <value name="foo" type="bar">...</value>
//      ...
//    </values>
//
// The "type" attribute may be set to one of "string", "int", "uint",
// "long", "ulong", "binary", or "strings".  In the first 5 cases,
// the text within the "value" tag is the string form of the value.
// If the type is not recognized, it is treated as "string".
//
// In the case of "binary", the content is the base64-encoded binary
// value.  In the case of "strings", the content is a list of "string"
// tags, each one defining an individual list element.  e.g.
//
//    <values>
//      <value name="foo" type="strings">
//        <string>abc</string>
//        <string>def</string>
//        ...
//      </value>
//      ...
//    </values>

internal sealed class FileKeyProvider : IRegistryKeyProvider
{
	// Internal state.
	private String directory;
	private String fullName;
	private SecurityElement values;
	private bool modified;

	// Directory names under "$HOME/.cli/registry" that correspond
	// to the various registry hives.
	private static readonly String[] hiveDirectories = {
			"ClassesRoot",
			"CurrentUser",
			"LocalMachine",
			"Users",
			"PerformanceData",
			"CurrentConfig",
			"DynData"
		};

	// Create a directory.
	private static void CreateDirectory(String name)
			{
				if(Directory.Exists(name))
				{
					return;
				}
				try
				{
					Directory.CreateDirectory(name);
				}
				catch(Exception)
				{
					throw new NotSupportedException();
				}
			}

	// Constructors.
	private FileKeyProvider(FileKeyProvider parent, String name)
			{
				// Record the name information for later.
				this.fullName = parent.fullName + "\\" + name;

				// Build the full pathname for the directory.
				this.directory =
					parent.directory + Path.DirectorySeparatorChar + name;
			}
	public FileKeyProvider(RegistryHive hive, String name)
			{
				// Record the name information for later.
				this.fullName = name;

				// Find the root position of the user's storage area.
				String root = InfoMethods.GetUserStorageDir();

				// Build the full pathname for the hive directory.
				directory = root + Path.DirectorySeparatorChar + "registry";
				CreateDirectory(directory);
				directory = directory + Path.DirectorySeparatorChar +
						hiveDirectories
							[((int)hive) - ((int)RegistryHive.ClassesRoot)];
				CreateDirectory(directory);
			}

	// Close a reference to this key and flush any modifications to disk.
	public void Close(bool writable)
			{
				Flush();
			}

	// Create a subkey underneath this particular registry key.
	public IRegistryKeyProvider CreateSubKey(String subkey)
			{
				String dir = directory + Path.DirectorySeparatorChar + subkey;
				if(Directory.Exists(dir))
				{
					return new FileKeyProvider(this, subkey);
				}
				Errno error = DirMethods.CreateDirectory(dir);
				if(error == Errno.Success || error == Errno.EEXIST)
				{
					return new FileKeyProvider(this, subkey);
				}
				throw new ArgumentException(_("IO_RegistryKeyNotExist"));
			}

	// Returns true if we should delete subkeys from their parents.
	public bool DeleteFromParents
			{
				get
				{
					// Use "Delete" and "DeleteTree".
					return false;
				}
			}

	// Delete a subkey of this key entry.  Returns false if not present.
	public bool DeleteSubKey(String name)
			{
				// Not used by this registry key provider.
				return false;
			}

	// Delete this key entry.
	public void Delete()
			{
				try
				{
					if((Directory.GetDirectories(directory)).Length != 0)
					{
						throw new InvalidOperationException
							(_("IO_RegistryHasSubKeys"));
					}
					File.Delete(directory + Path.DirectorySeparatorChar +
								"values.reg");
					Directory.Delete(directory);
				}
				catch(IOException)
				{
					// We probably couldn't delete because it doesn't exist.
				}
			}

	// Delete a subkey entry and all of its descendents.
	// Returns false if not present.
	public bool DeleteSubKeyTree(String name)
			{
				// Not used by this registry key provider.
				return false;
			}

	// Delete this key entry and all of its descendents.
	public void DeleteTree()
			{
				try
				{
					Directory.Delete(directory, true);
				}
				catch(IOException)
				{
					// We probably couldn't delete because it doesn't exist.
				}
			}

	// Fetch the contents of the "values.reg" file into the internal cache.
	private void ReadToCache()
			{
				try
				{
					StreamReader reader = new StreamReader
						(directory + Path.DirectorySeparatorChar +
						 "values.reg");
					String xml = reader.ReadToEnd();
					reader.Close();
					values = SecurityElement.Parse(xml);
				}
				catch(IOException)
				{
					values = new SecurityElement("values");
				}
			}

	// Find the security element corresponding to a particular value.
	private SecurityElement FindValue(String name)
			{
				if(values == null)
				{
					ReadToCache();
				}
				ArrayList children = values.Children;
				if(children != null)
				{
					foreach(SecurityElement e in children)
					{
						if(e.Tag == "value" &&
						   e.Attribute("name") == name)
						{
							return e;
						}
					}
				}
				return null;
			}

	// Delete a particular value underneath this registry key.
	// Returns false if the value is missing.
	public bool DeleteValue(String name)
			{
				lock(this)
				{
					SecurityElement e = FindValue(name);
					if(e != null)
					{
						values.Children.Remove(e);
						modified = true;
						return true;
					}
					else
					{
						return false;
					}
				}
			}

	// Flush all modifications to this registry key.
	public void Flush()
			{
				lock(this)
				{
					if(values != null && modified)
					{
						try
						{
							StreamWriter writer = new StreamWriter
								(directory + Path.DirectorySeparatorChar +
								 "values.reg");
							writer.Write(values.ToString());
							writer.Close();
						}
						catch(IOException)
						{
							// Could not create the file for some reason.
						}
					}
					values = null;
					modified = false;
				}
			}

	// Get the names of all subkeys underneath this registry key.
	public String[] GetSubKeyNames()
			{
				return Directory.GetDirectories(directory);
			}

	// Get a value from underneath this registry key.
	public Object GetValue(String name, Object defaultValue)
			{
				lock(this)
				{
					SecurityElement e = FindValue(name);
					if(e != null)
					{
						String type = e.Attribute("type");
						if(type == null)
						{
							type = "string";
						}
						Object value;
						String text = e.Text;
						if(text == null)
						{
							text = String.Empty;
						}
						switch(type)
						{
							case "string": default:
							{
								value = text;
							}
							break;

							case "int":
							{
								value = Int32.Parse(text);
							}
							break;

							case "uint":
							{
								value = UInt32.Parse(text);
							}
							break;

							case "long":
							{
								value = Int64.Parse(text);
							}
							break;

							case "ulong":
							{
								value = UInt64.Parse(text);
							}
							break;

							case "binary":
							{
								value = Convert.FromBase64String(text);
							}
							break;

							case "strings":
							{
								ArrayList children = e.Children;
								ArrayList list = new ArrayList();
								if(children != null)
								{
									foreach(SecurityElement elem in children)
									{
										if(elem.Tag == "string")
										{
											text = elem.Text;
											if(text == null)
											{
												text = String.Empty;
											}
											list.Add(text);
										}
									}
								}
								value = list.ToArray(typeof(String));
							}
							break;
						}
						return value;
					}
					else
					{
						return defaultValue;
					}
				}
			}

	// Get the names of all values underneath this registry key.
	public String[] GetValueNames()
			{
				lock(this)
				{
					if(values == null)
					{
						ReadToCache();
					}
					ArrayList children = values.Children;
					ArrayList list = new ArrayList();
					if(children != null)
					{
						foreach(SecurityElement elem in children)
						{
							if(elem.Tag == "value")
							{
								list.Add(elem.Attribute("name"));
							}
						}
					}
					return (String[])(list.ToArray(typeof(String)));
				}
			}

	// Open a subkey.
	public IRegistryKeyProvider OpenSubKey(String name, bool writable)
			{
				String dir = directory + Path.DirectorySeparatorChar + name;
				if(Directory.Exists(dir))
				{
					return new FileKeyProvider(this, name);
				}
				else
				{
					return null;
				}
			}

	// Set a value under this registry key.
	public void SetValue(String name, Object value)
			{
				lock(this)
				{
					// Find or create a tag for this name.
					SecurityElement e = FindValue(name);
					if(e == null)
					{
						e = new SecurityElement("value");
						e.AddAttribute("name", SecurityElement.Escape(name));
						values.AddChild(e);
					}

					// Modify the value associated with the tag.
					e.Children = null;
					if(value is String)
					{
						e.SetAttribute("type", "string");
						e.Text = SecurityElement.Escape((String)value);
					}
					else if(value is int)
					{
						e.SetAttribute("type", "int");
						e.Text = ((int)value).ToString();
					}
					else if(value is uint)
					{
						e.SetAttribute("type", "uint");
						e.Text = ((uint)value).ToString();
					}
					else if(value is long)
					{
						e.SetAttribute("type", "long");
						e.Text = ((long)value).ToString();
					}
					else if(value is ulong)
					{
						e.SetAttribute("type", "ulong");
						e.Text = ((ulong)value).ToString();
					}
					else if(value is byte[])
					{
						e.SetAttribute("type", "binary");
						e.Text = Convert.ToBase64String((byte[])value);
					}
					else if(value is String[])
					{
						e.SetAttribute("type", "strings");
						String[] list = (String[])value;
						SecurityElement child;
						for(int i = 0; i < list.Length; i++)
						{
							child = new SecurityElement("string");
							String str = list[i];
							if(str == null)
							{
								str = String.Empty;
							}
							child.Text = SecurityElement.Escape(str);
							e.AddChild(child);
						}
					}
					else
					{
						// Treat everything else as a string.
						e.SetAttribute("type", "string");
						e.Text = SecurityElement.Escape(value.ToString());
					}

					// The value has been modified.
					modified = true;
				}
			}

	// Get the name of this registry key provider.
	public String Name
			{
				get
				{
					return fullName;
				}
			}

	// Get the number of subkeys underneath this key.
	public int SubKeyCount
			{
				get
				{
					return (Directory.GetDirectories(directory)).Length;
				}
			}

	// Get the number of values that are associated with this key.
	public int ValueCount
			{
				get
				{
					lock(this)
					{
						if(values == null)
						{
							return 0;
						}
						ArrayList children = values.Children;
						int count = 0;
						if(children != null)
						{
							foreach(SecurityElement elem in children)
							{
								if(elem.Tag == "value")
								{
									++count;
								}
							}
						}
						return count;
					}
				}
			}

}; // class FileKeyProvider

#endif // CONFIG_WIN32_SPECIFICS

}; // namespace Microsoft.Win32
