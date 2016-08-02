/*
 * RegistryKey.cs - Implementation of the
 *			"Microsoft.Win32.RegistryKey" class.
 *
 * Copyright (C) 2003, 2004  Southern Storm Software, Pty Ltd.
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
using System.IO;
using System.Security;
using System.Security.AccessControl;

public sealed class RegistryKey : MarshalByRefObject, IDisposable
{
	// Internal state.
	private String name;
	private IRegistryKeyProvider provider;
	private bool writable;

	// Standard hive names.
	private static readonly String[] hiveNames = {
			"HKEY_CLASSES_ROOT",
			"HKEY_CURRENT_USER",
			"HKEY_LOCAL_MACHINE",
			"HKEY_USERS",
			"HKEY_PERFORMANCE_DATA",
			"HKEY_CURRENT_CONFIG",
			"HKEY_DYN_DATA"
		};

	// Constructor.
	private RegistryKey(IRegistryKeyProvider provider, bool writable)
			{
				this.name = provider.Name;
				this.provider = provider;
				this.writable = writable;
			}

	// Destructor.
	~RegistryKey()
			{
				Close();
			}

	// Implement the IDisposable interface.
	void IDisposable.Dispose()
			{
				Close();
			}

	// Close this key and flush any modifications to disk.
	public void Close()
			{
				if(provider != null)
				{
					provider.Close(writable);
					provider = null;
				}
			}

	// Resolve a subkey to its parent and last component.
	private static RegistryKey Resolve(RegistryKey start, String subkey,
									   bool create, out String last)
			{
				int index;
				String temp;
				IRegistryKeyProvider key;
				RegistryKey savedStart = start;
				RegistryKey prevStart;

				last = String.Empty;
				while((index = subkey.IndexOf('\\')) != -1 ||
				      (index = subkey.IndexOf('/')) != -1)
				{
					// Extract the name of this component.
					temp = subkey.Substring(0, index);
					subkey = subkey.Substring(index + 1);

					// Bail out if "start" does not have a provider.
					if(start.provider == null)
					{
						throw new IOException(_("IO_RegistryKeyClosed"));
					}

					// Create or open a new component.
					prevStart = start;
					if(create)
					{
						key = start.provider.OpenSubKey(temp, true);
						if(key == null)
						{
							key = start.provider.CreateSubKey(temp);
						}
						start = new RegistryKey(key, true);
					}
					else
					{
						key = start.provider.OpenSubKey(temp, false);
						if(key == null)
						{
							return null;
						}
						start = new RegistryKey(key, false);
					}
					if(prevStart != savedStart)
					{
						// Intermediate registry key that we won't be needing
						// any more, so clean up its resources.
						prevStart.Close();
					}
				}
				last = subkey;
				if(start.provider == null)
				{
					throw new IOException(_("IO_RegistryKeyClosed"));
				}
				return start;
			}

	// Create a subkey underneath this particular registry key.
	public RegistryKey CreateSubKey(String subkey)
			{
				// Validate the parameters.
				if(subkey == null)
				{
					throw new ArgumentNullException("subkey");
				}
				if(provider == null)
				{
					throw new IOException(_("IO_RegistryKeyClosed"));
				}
				if(!writable)
				{
					throw new UnauthorizedAccessException
						(_("IO_RegistryReadOnly"));
				}

				// Resolve the subkey to a parent and last component.
				String last;
				RegistryKey parent;
				parent = Resolve(this, subkey, true, out last);

				// Open or create the subkey and make it writable.
				IRegistryKeyProvider key;
				key = parent.provider.OpenSubKey(last, true);
				if(key == null)
				{
					key = parent.provider.CreateSubKey(last);
				}
				return new RegistryKey(key, true);
			}
#if CONFIG_FRAMEWORK_2_0
	[TODO]
	public RegistryKey CreateSubKey(String subkey,
								  RegistryKeyPermissionCheck permissionCheck)
			{
				return CreateSubKey(subkey);
			}
#if CONFIG_ACCESS_CONTROL
	[TODO]
	public RegistryKey CreateSubKey(String subkey,
								  RegistryKeyPermissionCheck permissionCheck,
								  RegistryRights rights)
			{
				return CreateSubKey(subkey);
			}
#endif // CONFIG_ACCESS_CONTROL
#endif // CONFIG_FRAMEWORK_2_0
#if CONFIG_ACCESS_CONTROL
	public RegistryKey CreateSubKey
				(String subkey, RegistrySecurity registrySecurity)
			{
				return CreateSubKey(subkey);
			}
#endif

	// Delete a particular subkey.
	public void DeleteSubKey(String subkey, bool throwOnMissingSubKey)
			{
				// Validate the parameters.
				if(subkey == null)
				{
					throw new ArgumentNullException("subkey");
				}
				if(provider == null)
				{
					throw new IOException(_("IO_RegistryKeyClosed"));
				}

				// Take a shortcut if we need to "delete from parents".
				if(provider.DeleteFromParents)
				{
					if(!provider.DeleteSubKey(subkey) && throwOnMissingSubKey)
					{
						throw new ArgumentException
							(_("IO_RegistryKeyNotExist"));
					}
					return;
				}

				// Resolve the subkey to a parent and last component.
				String last;
				RegistryKey parent;
				parent = Resolve(this, subkey, false, out last);
				if(parent == null)
				{
					if(throwOnMissingSubKey)
					{
						throw new ArgumentException
							(_("IO_RegistryKeyNotExist"));
					}
					return;
				}

				// Find and delete the subkey.
				IRegistryKeyProvider key;
				key = parent.provider.OpenSubKey(last, false);
				if(key == null)
				{
					if(throwOnMissingSubKey)
					{
						throw new ArgumentException
							(_("IO_RegistryKeyNotExist"));
					}
					return;
				}
				key.Close(false);
				key.Delete();
			}
	public void DeleteSubKey(String subkey)
			{
				DeleteSubKey(subkey, true);
			}

	// Delete a particular subkey and all of its descendents.
	public void DeleteSubKeyTree(String subkey)
			{
				// Validate the parameters.
				if(subkey == null)
				{
					throw new ArgumentNullException("subkey");
				}
				if(provider == null)
				{
					throw new IOException(_("IO_RegistryKeyClosed"));
				}

				// Take a shortcut if we need to "delete from parents".
				if(provider.DeleteFromParents)
				{
					if(!provider.DeleteSubKeyTree(subkey))
					{
						throw new ArgumentException
							(_("IO_RegistryKeyNotExist"));
					}
					return;
				}

				// Resolve the subkey to a parent and last component.
				String last;
				RegistryKey parent;
				parent = Resolve(this, subkey, false, out last);
				if(parent == null)
				{
					throw new ArgumentException(_("IO_RegistryKeyNotExist"));
				}

				// Find and delete the subkey.
				IRegistryKeyProvider key;
				key = parent.provider.OpenSubKey(last, false);
				if(key == null)
				{
					throw new ArgumentException(_("IO_RegistryKeyNotExist"));
				}
				key.Close(false);
				key.DeleteTree();
			}

	// Delete a particular value underneath this registry key.
	public void DeleteValue(String name, bool throwOnMissingValue)
			{
				if(name == null)
				{
					name = String.Empty;
				}
				if(!writable)
				{
					throw new UnauthorizedAccessException
						(_("IO_RegistryReadOnly"));
				}
				if(provider != null)
				{
					if(!provider.DeleteValue(name))
					{
						if(throwOnMissingValue)
						{
							throw new ArgumentException
								(_("IO_RegistryKeyNotExist"));
						}
					}
				}
				else
				{
					throw new IOException(_("IO_RegistryKeyClosed"));
				}
			}
	public void DeleteValue(String name)
			{
				DeleteValue(name, true);
			}

	// Flush all modifications to this registry key.
	public void Flush()
			{
				if(provider != null)
				{
					provider.Flush();
				}
			}

#if CONFIG_ACCESS_CONTROL

	// Get the access control information for this key.
	[TODO]
	public RegistrySecurity GetAccessControl()
			{
				// TODO
				return null;
			}
	[TODO]
	public RegistrySecurity GetAccessControl(AccessControlSections ruleInfo)
			{
				// TODO
				return null;
			}

	// Set the access control information for this key.
	[TODO]
	public void SetAccessControl(RegistrySecurity registrySecurity)
			{
				if(registrySecurity == null)
				{
					throw new ArgumentNullException("registrySecurity");
				}
				// TODO
			}

#endif

	// Get the names of all subkeys underneath this registry key.
	public String[] GetSubKeyNames()
			{
				if(provider != null)
				{
					return provider.GetSubKeyNames();
				}
				else
				{
					throw new IOException(_("IO_RegistryKeyClosed"));
				}
			}

	// Get a value from underneath this registry key.
	public Object GetValue(String name, Object defaultValue)
			{
				if(name == null)
				{
					name = String.Empty;
				}
				if(provider != null)
				{
					return provider.GetValue(name, defaultValue);
				}
				else
				{
					throw new IOException(_("IO_RegistryKeyClosed"));
				}
			}
	public Object GetValue(String name)
			{
				return GetValue(name, null);
			}
#if CONFIG_FRAMEWORK_2_0
	[Obsolete("Use the RegistryValueOptions variant of GetValue instead")]
	public Object GetValue(String name, Object defaultValue, bool doNotExpand)
			{
				if(doNotExpand)
				{
					return GetValue
						(name, defaultValue,
						 RegistryValueOptions.DoNotExpandEnvironmentNames);
				}
				else
				{
					return GetValue
						(name, defaultValue, RegistryValueOptions.None);
				}
			}
	[TODO]
	public Object GetValue(String name, Object defaultValue,
						   RegistryValueOptions options)
			{
				// TODO
				return GetValue(name, defaultValue);
			}

	// Get the kind associated with a value.
	[TODO]
	public RegistryValueKind GetValueKind(String name)
			{
				// TODO
				return RegistryValueKind.String;
			}
#endif // CONFIG_FRAMEWORK_2_0

	// Get the names of all values underneath this registry key.
	public String[] GetValueNames()
			{
				if(provider != null)
				{
					return provider.GetValueNames();
				}
				else
				{
					throw new IOException(_("IO_RegistryKeyClosed"));
				}
			}

	// Open a registry key for a local or remote machine.
	public static RegistryKey OpenRemoteBaseKey(RegistryHive hKey,
												String machineName)
			{
				// Validate the parameters.
				if(hKey < RegistryHive.ClassesRoot ||
				   hKey > RegistryHive.DynData)
				{
					throw new ArgumentException(_("Arg_InvalidHive"));
				}
				if(machineName == null)
				{
					throw new ArgumentNullException("machineName");
				}

				// Get the name of the hive to be accessed.
				String name = hiveNames
					[((int)hKey) - (int)(RegistryHive.ClassesRoot)];

				// Is this a remote hive reference?
				if(machineName != String.Empty)
				{
					if(Win32KeyProvider.IsWin32())
					{
						// Attempt to connect to the remote registry.
						IntPtr newKey;
						if(Win32KeyProvider.RegConnectRegistry
								(machineName,
								 Win32KeyProvider.HiveToHKey(hKey),
								 out newKey) != 0)
						{
							throw new SecurityException
								(_("Invalid_RemoteRegistry"));
						}
						return new RegistryKey
							(new Win32KeyProvider(name, newKey), true);
					}
					else
					{
						// Not Win32 - cannot access remote registries.
						throw new SecurityException
							(_("Invalid_RemoteRegistry"));
					}
				}

				// Open a local hive.
				return new RegistryKey
					(Registry.GetProvider(hKey, name), true);
			}

	// Open a subkey.
	public RegistryKey OpenSubKey(String name, bool writable)
			{
				// Validate the parameters.
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(provider == null)
				{
					throw new IOException(_("IO_RegistryKeyClosed"));
				}

				// Resolve the subkey to a parent and last component.
				String last;
				RegistryKey parent;
				parent = Resolve(this, name, false, out last);
				if(parent == null)
				{
					return null;
				}

				// Open the subkey in the specified mode.
				IRegistryKeyProvider key;
				key = parent.provider.OpenSubKey(last, writable);
				if(key == null)
				{
					return null;
				}
				return new RegistryKey(key, writable);
			}
	public RegistryKey OpenSubKey(String name)
			{
				return OpenSubKey(name, false);
			}
#if CONFIG_FRAMEWORK_2_0
	[TODO]
	public RegistryKey OpenSubKey(String name,
								  RegistryKeyPermissionCheck permissionCheck)
			{
				return OpenSubKey(name, false);
			}
#if CONFIG_ACCESS_CONTROL
	[TODO]
	public RegistryKey OpenSubKey(String name,
								  RegistryKeyPermissionCheck permissionCheck,
								  RegistryRights rights)
			{
				return OpenSubKey(name, false);
			}
#endif // CONFIG_ACCESS_CONTROL
#endif // CONFIG_FRAMEWORK_2_0

	// Set a value under this registry key.
	public void SetValue(String name, Object value)
			{
				if(name == null)
				{
					name = String.Empty;
				}
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(!writable)
				{
					throw new UnauthorizedAccessException
						(_("IO_RegistryReadOnly"));
				}
				if(provider != null)
				{
					provider.SetValue(name, value);
				}
				else
				{
					throw new IOException(_("IO_RegistryKeyClosed"));
				}
			}
#if CONFIG_FRAMEWORK_2_0
	public void SetValue
				(String name, Object value, RegistryValueKind valueKind)
			{
				// TODO
				SetValue(name, value);
			}
#endif

	// Get the string form of this registry key.
	public override String ToString()
			{
				return name;
			}

	// Get the name of the registry key.
	public String Name
			{
				get
				{
					return name;
				}
			}

	// Get the number of subkeys underneath this key.
	public int SubKeyCount
			{
				get
				{
					if(provider != null)
					{
						return provider.SubKeyCount;
					}
					else
					{
						throw new IOException(_("IO_RegistryKeyClosed"));
					}
				}
			}

	// Get the number of values that are associated with this key.
	public int ValueCount
			{
				get
				{
					if(provider != null)
					{
						return provider.ValueCount;
					}
					else
					{
						throw new IOException(_("IO_RegistryKeyClosed"));
					}
				}
			}

}; // class RegistryKey

#endif // CONFIG_WIN32_SPECIFICS

}; // namespace Microsoft.Win32
