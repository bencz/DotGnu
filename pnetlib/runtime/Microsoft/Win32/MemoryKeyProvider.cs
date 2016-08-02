/*
 * MemoryKeyProvider.cs - Implementation of the
 *			"Microsoft.Win32.MemoryKeyProvider" class.
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

// This class implements a registry in memory.  The contents
// are discarded when the program exits.

internal sealed class MemoryKeyProvider : IRegistryKeyProvider
{
	// Internal state.
	private MemoryKeyProvider parent;
	private String fullName;
	private String name;
	private Hashtable subkeys;
	private Hashtable values;
	private bool deleted;

	// Constructor.
	public MemoryKeyProvider(MemoryKeyProvider parent, String fullName,
							 String name)
			{
				this.parent = parent;
				this.fullName = fullName;
				this.name = name;
				this.subkeys = null;
				this.values = null;
				this.deleted = false;
			}

	// Close a reference to this key and flush any modifications to disk.
	public void Close(bool writable)
			{
				// Nothing to do here - there is no disk copy.
			}

	// Create a subkey underneath this particular registry key.
	public IRegistryKeyProvider CreateSubKey(String subkey)
			{
				lock(this)
				{
					if(deleted)
					{
						throw new ArgumentException
							(_("IO_RegistryKeyNotExist"));
					}
					if(subkeys == null)
					{
						subkeys = new Hashtable
							(CaseInsensitiveHashCodeProvider.Default,
							 CaseInsensitiveComparer.Default);
					}
					Object value = subkeys[subkey];
					if(value == null)
					{
						value = new MemoryKeyProvider
							(this, name + "\\" + subkey, subkey);
						subkeys[subkey] = value;
					}
					return (IRegistryKeyProvider)value;
				}
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
				// Not used for memory-based registries.
				return false;
			}

	// Delete this key entry.
	public void Delete()
			{
				MemoryKeyProvider parent;

				// Delete the information in this entry.
				lock(this)
				{
					if(deleted)
					{
						// This key was already deleted.
						throw new ArgumentException
							(_("IO_RegistryKeyNotExist"));
					}
					else if(subkeys != null && subkeys.Count != 0)
					{
						throw new InvalidOperationException
							(_("IO_RegistryHasSubKeys"));
					}
					else
					{
						deleted = true;
						subkeys = null;
						if(values != null && values.Count != 0)
						{
							values.Clear();
						}
						values = null;
					}
					parent = this.parent;
				}

				// Remove the name from the parent provider.
				if(parent != null)
				{
					lock(parent)
					{
						if(parent.subkeys != null)
						{
							parent.subkeys.Remove(name);
						}
					}
				}
			}

	// Delete a subkey entry and all of its descendents.
	// Returns false if not present.
	public bool DeleteSubKeyTree(String name)
			{
				// Not used for memory-based registries.
				return false;
			}

	// Delete this key entry and all of its descendents.
	public void DeleteTree()
			{
				// Collect up all of the children.
				IRegistryKeyProvider[] children;
				lock(this)
				{
					if(deleted)
					{
						// Ignore the operation if we are already deleted.
						return;
					}
					if(subkeys != null)
					{
						children = new IRegistryKeyProvider [subkeys.Count];
						IDictionaryEnumerator e = subkeys.GetEnumerator();
						int index = 0;
						while(e.MoveNext())
						{
							children[index++] =
								(IRegistryKeyProvider)(e.Value);
						}
					}
					else
					{
						children = null;
					}
				}

				// Recursively delete the children.
				if(children != null)
				{
					int posn;
					for(posn = 0; posn < children.Length; ++posn)
					{
						children[posn].DeleteTree();
					}
				}

				// Delete this key entry.  If new subkeys were
				// added in the meantime, then simply ignore them.
				lock(this)
				{
					deleted = true;
					if(subkeys != null && subkeys.Count != 0)
					{
						subkeys.Clear();
					}
					subkeys = null;
					if(values != null && values.Count != 0)
					{
						values.Clear();
					}
					values = null;
				}
			}

	// Delete a particular value underneath this registry key.
	// Returns false if the value is missing.
	public bool DeleteValue(String name)
			{
				lock(this)
				{
					if(deleted)
					{
						throw new ArgumentException
							(_("IO_RegistryKeyNotExist"));
					}
					if(values != null && values.Contains(name))
					{
						values.Remove(name);
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
				// Nothing to do here - there is no disk copy.
			}

	// Create an array of names from a hash table's keys.
	private static String[] CreateNameArray(Hashtable table)
			{
				String[] array = new String [table.Count];
				IDictionaryEnumerator e = table.GetEnumerator();
				int index = 0;
				while(e.MoveNext())
				{
					array[index++] = (String)(e.Key);
				}
				return array;
			}

	// Get the names of all subkeys underneath this registry key.
	public String[] GetSubKeyNames()
			{
				lock(this)
				{
					if(deleted)
					{
						throw new ArgumentException
							(_("IO_RegistryKeyNotExist"));
					}
					if(subkeys != null)
					{
						return CreateNameArray(subkeys);
					}
					else
					{
						return new String [0];
					}
				}
			}

	// Get a value from underneath this registry key.
	public Object GetValue(String name, Object defaultValue)
			{
				lock(this)
				{
					if(deleted)
					{
						throw new ArgumentException
							(_("IO_RegistryKeyNotExist"));
					}
					if(values == null)
					{
						return defaultValue;
					}
					Object value = values[name];
					if(value != null)
					{
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
					if(deleted)
					{
						throw new ArgumentException
							(_("IO_RegistryKeyNotExist"));
					}
					if(values != null)
					{
						return CreateNameArray(values);
					}
					else
					{
						return new String [0];
					}
				}
			}

	// Open a subkey.
	public IRegistryKeyProvider OpenSubKey(String name, bool writable)
			{
				lock(this)
				{
					if(deleted)
					{
						throw new ArgumentException
							(_("IO_RegistryKeyNotExist"));
					}
					if(subkeys != null)
					{
						return (IRegistryKeyProvider)(subkeys[name]);
					}
					else
					{
						return null;
					}
				}
			}

	// Set a value under this registry key.
	public void SetValue(String name, Object value)
			{
				lock(this)
				{
					if(deleted)
					{
						throw new ArgumentException
							(_("IO_RegistryKeyNotExist"));
					}
					if(values == null)
					{
						values = new Hashtable
							(CaseInsensitiveHashCodeProvider.Default,
							 CaseInsensitiveComparer.Default);
					}
					values[name] = value;
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
					lock(this)
					{
						if(deleted)
						{
							throw new ArgumentException
								(_("IO_RegistryKeyNotExist"));
						}
						if(subkeys != null)
						{
							return subkeys.Count;
						}
						else
						{
							return 0;
						}
					}
				}
			}

	// Get the number of values that are associated with this key.
	public int ValueCount
			{
				get
				{
					lock(this)
					{
						if(deleted)
						{
							throw new ArgumentException
								(_("IO_RegistryKeyNotExist"));
						}
						if(values != null)
						{
							return values.Count;
						}
						else
						{
							return 0;
						}
					}
				}
			}

}; // class MemoryKeyProvider

#endif // CONFIG_WIN32_SPECIFICS

}; // namespace Microsoft.Win32
