/*
 * Registry.cs - Implementation of the
 *			"Microsoft.Win32.Registry" class.
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

public sealed class Registry
{
	// Constructor.
	private Registry() {}

	// Standard registries on the local machine.
	public static readonly RegistryKey ClassesRoot;
	public static readonly RegistryKey CurrentUser;
	public static readonly RegistryKey LocalMachine;
	public static readonly RegistryKey Users;
	public static readonly RegistryKey PerformanceData;
	public static readonly RegistryKey CurrentConfig;
	public static readonly RegistryKey DynData;

	// Initialize the standard registries.
	static Registry()
			{
				ClassesRoot = RegistryKey.OpenRemoteBaseKey
					(RegistryHive.ClassesRoot, String.Empty);
				CurrentUser = RegistryKey.OpenRemoteBaseKey
					(RegistryHive.CurrentUser, String.Empty);
				LocalMachine = RegistryKey.OpenRemoteBaseKey
					(RegistryHive.LocalMachine, String.Empty);
				Users = RegistryKey.OpenRemoteBaseKey
					(RegistryHive.Users, String.Empty);
				PerformanceData = RegistryKey.OpenRemoteBaseKey
					(RegistryHive.PerformanceData, String.Empty);
				CurrentConfig = RegistryKey.OpenRemoteBaseKey
					(RegistryHive.CurrentConfig, String.Empty);
				DynData = RegistryKey.OpenRemoteBaseKey
					(RegistryHive.DynData, String.Empty);
			}

	// Registry providers that have already been allocated.
	private static IRegistryKeyProvider[] providers;

	// Get a registry key provider for a particular hive.
	internal static IRegistryKeyProvider GetProvider
				(RegistryHive hKey, String name)
			{
				int index;

				lock(typeof(Registry))
				{
					// Allocate the "providers" array if necessary.
					if(providers == null)
					{
						providers = new IRegistryKeyProvider[7];
					}

					// See if we already have a provider for this hive.
					index = ((int)hKey) - ((int)(RegistryHive.ClassesRoot));
					if(providers[index] != null)
					{
						return providers[index];
					}

					// Create a Win32 provider if we are on a Windows system.
					if(Win32KeyProvider.IsWin32())
					{
						providers[index] = new Win32KeyProvider
							(name, Win32KeyProvider.HiveToHKey(hKey));
						return providers[index];
					}

					// Try to create a file-based provider for the hive.
					try
					{
						providers[index] = new FileKeyProvider(hKey, name);
						return providers[index];
					}
					catch(NotSupportedException)
					{
						// Could not create the hive directory - fall through.
					}

					// Create a memory-based provider on all other systems.
					providers[index] = new MemoryKeyProvider
						(null, name, name);
					return providers[index];
				}
			}

#if CONFIG_FRAMEWORK_2_0

	// Get a registry key from a full path.
	private static RegistryKey GetKey(String keyName, bool writable)
			{
				String rest;
				RegistryKey key;
				int index, index2;
				index = keyName.IndexOf('\\');
				index2 = keyName.IndexOf('/');
				if(index == -1)
				{
					index = index2;
				}
				else if(index2 != -1 && index2 < index)
				{
					index = index2;
				}
				if(index != 1)
				{
					rest = keyName.Substring(index + 1);
					keyName = keyName.Substring(0, index);
				}
				else
				{
					rest = String.Empty;
				}
				switch(keyName)
				{
					case "HKEY_CLASSES_ROOT":	key = ClassesRoot; break;
					case "HKEY_CURRENT_USER":	key = CurrentUser; break;
					case "HKEY_LOCAL_MACHINE":	key = LocalMachine; break;
					case "HKEY_USERS":			key = Users; break;
					case "HKEY_PERFORMANCE_DATA":
						key = PerformanceData; break;
					case "HKEY_CURRENT_CONFIG":	key = CurrentConfig; break;
					case "HKEY_DYN_DATA":		key = DynData; break;

					default:
					{
						throw new ArgumentException
							(_("IO_RegistryKeyNotExist"));
					}
					// Not reached;
				}
				if(writable)
				{
					return key.CreateSubKey(rest);
				}
				else
				{
					return key.OpenSubKey(rest);
				}
			}

	// Get a particular registry value.
	public static Object GetValue
				(String keyName, String valueName, Object defaultValue)
			{
				RegistryKey key = GetKey(keyName, false);
				try
				{
					return key.GetValue(valueName, defaultValue);
				}
				finally
				{
					key.Close();
				}
			}

	// Set a particular registry value.
	public static void SetValue
				(String keyName, String valueName, Object value,
				 RegistryValueKind valueKind)
			{
				RegistryKey key = GetKey(keyName, true);
				try
				{
					key.SetValue(valueName, value, valueKind);
				}
				finally
				{
					key.Close();
				}
			}
	public static void SetValue
				(String keyName, String valueName, Object value)
			{
				RegistryKey key = GetKey(keyName, true);
				try
				{
					key.SetValue(valueName, value, RegistryValueKind.Unknown);
				}
				finally
				{
					key.Close();
				}
			}

#endif // CONFIG_FRAMEWORK_2_0

}; // class Registry

#endif // CONFIG_WIN32_SPECIFICS

}; // namespace Microsoft.Win32
