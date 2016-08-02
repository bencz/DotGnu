/*
 * Win32KeyProvider.cs - Implementation of the
 *			"Microsoft.Win32.Win32KeyProvider" class.
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
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

// This class implements registry functionality for Win32 systems.

internal sealed class Win32KeyProvider : IRegistryKeyProvider
{
	// Internal state.
	private String name;
	private IntPtr hKey;

	// Constructors.
	public Win32KeyProvider(String name, IntPtr hKey)
			{
				this.name = name;
				this.hKey = hKey;
			}

	// Destructor.
	~Win32KeyProvider()
			{
				Close(false);
			}

	// Determine if we should use the Win32 registry.
	public static bool IsWin32()
			{
				return (Environment.OSVersion.Platform != PlatformID.Unix);
			}

	// Close a reference to this key and flush any modifications to disk.
	public void Close(bool writable)
			{
				lock(this)
				{
					if(hKey != IntPtr.Zero)
					{
						long key = hKey.ToInt64();
						if(key < (long)(int)(RegistryHive.ClassesRoot) ||
						   key > (long)(int)(RegistryHive.DynData))
						{
							RegCloseKey(hKey);
						}
						hKey = IntPtr.Zero;
					}
				}
			}

	// Create a subkey underneath this particular registry key.
	public IRegistryKeyProvider CreateSubKey(String subkey)
			{
				lock(this)
				{
					if(hKey != IntPtr.Zero)
					{
						IntPtr newKey;
						if(RegCreateKey(hKey, subkey, out newKey) == 0)
						{
							return new Win32KeyProvider
								(name + "\\" + subkey, newKey);
						}
					}
				}
				throw new ArgumentException(_("IO_RegistryKeyNotExist"));
			}

	// Returns true if we should delete subkeys from their parents.
	public bool DeleteFromParents
			{
				get
				{
					// Use "DeleteSubKey" and "DeleteSubKeyTree".
					return true;
				}
			}

	// Delete a subkey of this key entry.  Returns false if not present.
	public bool DeleteSubKey(String name)
			{
				lock(this)
				{
					if(hKey != IntPtr.Zero)
					{
						// Make sure that we don't have any subkeys.
						// This is to work around an API discontinuity:
						// Win95 deletes subtrees, but WinNT doesn't.
						uint numSubKeys, numValues;
						RegQueryInfoKey(hKey, null,
										IntPtr.Zero, IntPtr.Zero,
										out numSubKeys, IntPtr.Zero,
										IntPtr.Zero, out numValues,
										IntPtr.Zero, IntPtr.Zero,
										IntPtr.Zero, IntPtr.Zero);
						if(numSubKeys != 0)
						{
							throw new InvalidOperationException
								(_("IO_RegistryHasSubKeys"));
						}
						return (RegDeleteKey(hKey, name) == 0);
					}
					else
					{
						return false;
					}
				}
			}

	// Delete this key entry.
	public void Delete()
			{
				// This version is not used under Win32.
			}

	// Internal recursive version of "DeleteSubKeyTree".
	private static bool DeleteSubKeyTree(IntPtr hKey, String name)
			{
				IntPtr subTree;

				// Open the subkey tree.
				if(RegOpenKeyEx(hKey, name, 0, KEY_ALL_ACCESS,
								out subTree) != 0)
				{
					return false;
				}

				// Collect up the names of all subkeys under "subTree".
				uint numSubKeys, numValues;
				RegQueryInfoKey(subTree, null,
								IntPtr.Zero, IntPtr.Zero,
								out numSubKeys, IntPtr.Zero,
								IntPtr.Zero, out numValues,
								IntPtr.Zero, IntPtr.Zero,
								IntPtr.Zero, IntPtr.Zero);
				String[] names = new String [numSubKeys];
				uint index = 0;
				char[] nameBuf = new char [1024];
				uint nameLen;
				long writeTime;
				while(index < numSubKeys)
				{
					nameBuf.Initialize();
					nameLen = (uint)(name.Length);
					if(RegEnumKeyEx(subTree, index, nameBuf, ref nameLen,
									IntPtr.Zero, IntPtr.Zero,
									IntPtr.Zero, out writeTime) != 0)
					{
						break;
					}
					names[(int)(index++)] = ArrayToString(nameBuf);
				}

				// Recursively delete the subtrees.
				foreach(String n in names)
				{
					DeleteSubKeyTree(subTree, n);
				}

				// Close the subtree.
				RegCloseKey(subTree);

				// Delete the subkey that corresponds to the subtree.
				return (RegDeleteKey(hKey, name) == 0);
			}

	// Delete a subkey entry and all of its descendents.
	// Returns false if not present.
	public bool DeleteSubKeyTree(String name)
			{
				lock(this)
				{
					if(hKey != IntPtr.Zero)
					{
						return DeleteSubKeyTree(hKey, name);
					}
					else
					{
						return false;
					}
				}
			}

	// Delete this key entry and all of its descendents.
	public void DeleteTree()
			{
				// This version is not used under Win32.
			}

	// Delete a particular value underneath this registry key.
	// Returns false if the value is missing.
	public bool DeleteValue(String name)
			{
				lock(this)
				{
					if(hKey != IntPtr.Zero)
					{
						if(RegDeleteValue(hKey, name) == 0)
						{
							return true;
						}
					}
					return false;
				}
			}

	// Flush all modifications to this registry key.
	public void Flush()
			{
				lock(this)
				{
					if(hKey != IntPtr.Zero)
					{
						RegFlushKey(hKey);
					}
				}
			}

	// Convert a null-terminated "char[]" array into a "String".
	private static String ArrayToString(char[] array)
			{
				int index = 0;
				while(index < array.Length && array[index] != '\0')
				{
					++index;
				}
				return new String(array, 0, index);
			}

	// Convert a null-terminated "byte[]" array into a "String",
	// where the array contains 16-bit character values.
	private static String ArrayToString(byte[] array, ref int index)
			{
				int len = 0;
				while((index + len + 1) < array.Length &&
				      (array[index + len] != 0 ||
					   array[index + len + 1] != 0))
				{
					len += 2;
				}
				StringBuilder builder = new StringBuilder(len / 2);
				len = 0;
				while((index + len + 1) < array.Length &&
				      (array[index + len] != 0 ||
					   array[index + len + 1] != 0))
				{
					builder.Append((char)(((int)array[index + len]) |
										  (((int)array[index + len + 1])
										  		<< 8)));
					len += 2;
				}
				index += len + 2;
				return builder.ToString();
			}
	private static String ArrayToString(byte[] array)
			{
				int index = 0;
				return ArrayToString(array, ref index);
			}

	// Convert a "byte[]" array into an array of "String" values.
	private static String[] ArrayToStringArray(byte[] array)
			{
				ArrayList list = new ArrayList();
				String value;
				int index = 0;
				for(;;)
				{
					value = ArrayToString(array, ref index);
					if(value.Length == 0)
					{
						break;
					}
					list.Add(value);
				}
				return (String[])(list.ToArray(typeof(String)));
			}

	// Convert a "String" into a "byte[]" array.
	private static byte[] StringToArray(String value)
			{
				byte[] data = new byte [value.Length * 2];
				int index;
				for(index = 0; index < value.Length; ++index)
				{
					data[index * 2] = (byte)(value[index]);
					data[index * 2 + 1] = (byte)(value[index] >> 8);
				}
				return data;
			}

	// Convert a "String[]" array into a "byte[]" array.
	private static byte[] StringArrayToArray(String[] value)
			{
				// Determine the total length of the "byte[]" array.
				int len = 0;
				int index, posn;
				String str;
				for(index = 0; index < value.Length; ++index)
				{
					str = value[index];
					if(str != null)
					{
						len += str.Length * 2 + 2;
					}
					else
					{
						len += 2;
					}
				}
				len += 2;

				// Create the "byte[]" array.
				byte[] data = new byte [len];

				// Write the strings into the "byte[]" array.
				len = 0;
				for(index = 0; index < value.Length; ++index)
				{
					str = value[index];
					if(str != null)
					{
						for(posn = 0; posn < str.Length; ++posn)
						{
							data[len++] = (byte)(str[posn]);
							data[len++] = (byte)(str[posn] >> 8);
						}
						len += 2;
					}
					else
					{
						len += 2;
					}
				}

				// Return the final encoded array to the caller.
				return data;
			}

	// Get the names of all subkeys underneath this registry key.
	public String[] GetSubKeyNames()
			{
				lock(this)
				{
					if(hKey != IntPtr.Zero)
					{
						// Get the number of subkey names under the key.
						uint numSubKeys, numValues;
						RegQueryInfoKey(hKey, null,
										IntPtr.Zero, IntPtr.Zero,
										out numSubKeys, IntPtr.Zero,
										IntPtr.Zero, out numValues,
										IntPtr.Zero, IntPtr.Zero,
										IntPtr.Zero, IntPtr.Zero);

						// Create an array to hold the names.
						String[] names = new String [numSubKeys];

						// Enumerate the names into the array.
						uint index = 0;
						char[] name = new char [1024];
						uint nameLen;
						long writeTime;
						while(index < numSubKeys)
						{
							name.Initialize();
							nameLen = (uint)(name.Length);
							if(RegEnumKeyEx(hKey, index, name, ref nameLen,
											IntPtr.Zero, IntPtr.Zero,
											IntPtr.Zero, out writeTime) != 0)
							{
								break;
							}
							names[(int)(index++)] = ArrayToString(name);
						}

						// Return the final name array to the caller.
						return names;
					}
					return new String [0];
				}
			}

	// Get a value from underneath this registry key.
	public Object GetValue(String name, Object defaultValue)
			{
				uint type = REG_NONE;
				byte[] data = null;
				uint dataLen;

				// Query the value from the registry.
				lock(this)
				{
					if(hKey != IntPtr.Zero)
					{
						// Determine how big the buffer needs to be first.
						dataLen = 0;
						if(RegQueryValueEx(hKey, name, IntPtr.Zero,
										   out type, IntPtr.Zero, ref dataLen)
								!= 0)
						{
							return defaultValue;
						}

						// Allocate a buffer to hold the data.
						data = new byte [dataLen];

						// Now query the actual value.
						if(RegQueryValueEx(hKey, name, IntPtr.Zero,
										   out type, data, ref dataLen)
								!= 0)
						{
							return defaultValue;
						}
					}
					else
					{
						return defaultValue;
					}
				}

				// Decode the value into something that we can return.
				switch(type)
				{
					case REG_BINARY:		return data;

					case REG_DWORD_LITTLE_ENDIAN:
					{
						if(data.Length == 4)
						{
							return ((int)(data[0])) |
								   (((int)(data[1])) << 8) |
								   (((int)(data[2])) << 16) |
								   (((int)(data[3])) << 24);
						}
					}
					break;

					case REG_DWORD_BIG_ENDIAN:
					{
						if(data.Length == 4)
						{
							return ((int)(data[3])) |
								   (((int)(data[2])) << 8) |
								   (((int)(data[1])) << 16) |
								   (((int)(data[0])) << 24);
						}
					}
					break;

					case REG_SZ:
					case REG_EXPAND_SZ:
					{
						return ArrayToString(data);
					}
					// Not reached.

					case REG_MULTI_SZ:
					{
						return ArrayToStringArray(data);
					}
					// Not reached.

					case REG_QWORD_LITTLE_ENDIAN:
					{
						if(data.Length == 8)
						{
							return ((long)(data[0])) |
								   (((long)(data[1])) << 8) |
								   (((long)(data[2])) << 16) |
								   (((long)(data[3])) << 24) |
								   (((long)(data[4])) << 32) |
								   (((long)(data[5])) << 40) |
								   (((long)(data[6])) << 48) |
								   (((long)(data[7])) << 56);
						}
					}
					break;

					default: break;
				}

				// If we get here, then we don't know how to decode the data.
				return defaultValue;
			}

	// Get the names of all values underneath this registry key.
	public String[] GetValueNames()
			{
				lock(this)
				{
					if(hKey != IntPtr.Zero)
					{
						// Get the number of value names under the key.
						uint numSubKeys, numValues;
						RegQueryInfoKey(hKey, null,
										IntPtr.Zero, IntPtr.Zero,
										out numSubKeys, IntPtr.Zero,
										IntPtr.Zero, out numValues,
										IntPtr.Zero, IntPtr.Zero,
										IntPtr.Zero, IntPtr.Zero);

						// Create an array to hold the names.
						String[] names = new String [numValues];

						// Enumerate the names into the array.
						uint index = 0;
						char[] name = new char [1024];
						uint nameLen;
						while(index < numValues)
						{
							name.Initialize();
							nameLen = (uint)(name.Length);
							if(RegEnumValue(hKey, index, name, ref nameLen,
											IntPtr.Zero, IntPtr.Zero,
											IntPtr.Zero, IntPtr.Zero) != 0)
							{
								break;
							}
							names[(int)(index++)] = ArrayToString(name);
						}

						// Return the final name array to the caller.
						return names;
					}
					return new String [0];
				}
			}

	// Open a subkey.
	public IRegistryKeyProvider OpenSubKey(String name, bool writable)
			{
				lock(this)
				{
					if(hKey != IntPtr.Zero)
					{
						IntPtr newKey;
						if(RegOpenKeyEx
								(hKey, name, 0,
								 (writable ? KEY_ALL_ACCESS : KEY_READ),
								 out newKey) != 0)
						{
							return null;
						}
						return new Win32KeyProvider
							(this.name + "\\" + name, newKey);
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
				uint type;
				byte[] data;

				// Convert the value into a byte array and type.
				if(value is String)
				{
					// Convert a string.
					type = REG_SZ;
					data = StringToArray((String)value);
				}
				else if(value is int)
				{
					// Convert a signed integer.
					int ivalue = (int)value;
					type = REG_DWORD_LITTLE_ENDIAN;
					data = new byte [4];
					data[0] = (byte)ivalue;
					data[1] = (byte)(ivalue >> 8);
					data[2] = (byte)(ivalue >> 16);
					data[3] = (byte)(ivalue >> 24);
				}
				else if(value is uint)
				{
					// Convert an unsigned integer.
					uint uivalue = (uint)value;
					type = REG_DWORD_LITTLE_ENDIAN;
					data = new byte [4];
					data[0] = (byte)uivalue;
					data[1] = (byte)(uivalue >> 8);
					data[2] = (byte)(uivalue >> 16);
					data[3] = (byte)(uivalue >> 24);
				}
				else if(value is long)
				{
					// Convert a signed long integer.
					long lvalue = (long)value;
					type = REG_QWORD_LITTLE_ENDIAN;
					data = new byte [8];
					data[0] = (byte)lvalue;
					data[1] = (byte)(lvalue >> 8);
					data[2] = (byte)(lvalue >> 16);
					data[3] = (byte)(lvalue >> 24);
					data[4] = (byte)(lvalue >> 32);
					data[5] = (byte)(lvalue >> 40);
					data[6] = (byte)(lvalue >> 48);
					data[7] = (byte)(lvalue >> 56);
				}
				else if(value is ulong)
				{
					// Convert an unsigned long integer.
					ulong ulvalue = (ulong)value;
					type = REG_QWORD_LITTLE_ENDIAN;
					data = new byte [8];
					data[0] = (byte)ulvalue;
					data[1] = (byte)(ulvalue >> 8);
					data[2] = (byte)(ulvalue >> 16);
					data[3] = (byte)(ulvalue >> 24);
					data[4] = (byte)(ulvalue >> 32);
					data[5] = (byte)(ulvalue >> 40);
					data[6] = (byte)(ulvalue >> 48);
					data[7] = (byte)(ulvalue >> 56);
				}
				else if(value is byte[])
				{
					// Convert a raw binary byte array.
					type = REG_BINARY;
					data = (byte[])value;
				}
				else if(value is String[])
				{
					// Convert an array of strings.
					type = REG_MULTI_SZ;
					data = StringArrayToArray((String[])value);
				}
				else
				{
					// Last ditch attempt: use the string form of the value.
					type = REG_SZ;
					data = StringToArray(value.ToString());
				}

				// Set the value within the registry.
				lock(this)
				{
					if(hKey != IntPtr.Zero)
					{
						RegSetValueEx(hKey, name, 0, type, data,
									  (uint)(data.Length));
					}
				}
			}

	// Get the name of this registry key provider.
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
					lock(this)
					{
						if(hKey != IntPtr.Zero)
						{
							uint numSubKeys, numValues;
							RegQueryInfoKey(hKey, null,
											IntPtr.Zero, IntPtr.Zero,
											out numSubKeys, IntPtr.Zero,
											IntPtr.Zero, out numValues,
											IntPtr.Zero, IntPtr.Zero,
											IntPtr.Zero, IntPtr.Zero);
							return (int)numSubKeys;
						}
						return 0;
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
						if(hKey != IntPtr.Zero)
						{
							uint numSubKeys, numValues;
							RegQueryInfoKey(hKey, null,
											IntPtr.Zero, IntPtr.Zero,
											out numSubKeys, IntPtr.Zero,
											IntPtr.Zero, out numValues,
											IntPtr.Zero, IntPtr.Zero,
											IntPtr.Zero, IntPtr.Zero);
							return (int)numValues;
						}
						return 0;
					}
				}
			}

	// Convert a hive value into a HKEY value.
	public static IntPtr HiveToHKey(RegistryHive hive)
			{
				return new IntPtr((int)hive);
			}

	// Import the Win32 registry functions from "advapi32.dll".

	[DllImport("advapi32.dll",
			   CallingConvention=CallingConvention.Winapi)]
	[MethodImpl(MethodImplOptions.PreserveSig)]
	extern private static int RegCloseKey(IntPtr hKey);

	[DllImport("advapi32.dll", EntryPoint="RegConnectRegistryW",
			   CallingConvention=CallingConvention.Winapi)]
	[MethodImpl(MethodImplOptions.PreserveSig)]
	extern public static int RegConnectRegistry
				([MarshalAs(UnmanagedType.LPWStr)] String lpMachineName,
				 IntPtr hKey, out IntPtr phkResult);

	[DllImport("advapi32.dll", EntryPoint="RegCreateKeyW",
			   CallingConvention=CallingConvention.Winapi)]
	[MethodImpl(MethodImplOptions.PreserveSig)]
	extern private static int RegCreateKey
				(IntPtr hkey,
				 [MarshalAs(UnmanagedType.LPWStr)] String lpSubKey,
				 out IntPtr phkResult);

	[DllImport("advapi32.dll", EntryPoint="RegDeleteKeyW",
			   CallingConvention=CallingConvention.Winapi)]
	[MethodImpl(MethodImplOptions.PreserveSig)]
	extern private static int RegDeleteKey
				(IntPtr hkey,
				 [MarshalAs(UnmanagedType.LPWStr)] String lpSubKey);

	[DllImport("advapi32.dll", EntryPoint="RegDeleteValueW",
			   CallingConvention=CallingConvention.Winapi)]
	[MethodImpl(MethodImplOptions.PreserveSig)]
	extern private static int RegDeleteValue
				(IntPtr hkey,
				 [MarshalAs(UnmanagedType.LPWStr)] String lpValueName);

	[DllImport("advapi32.dll", EntryPoint="RegEnumKeyExW",
			   CallingConvention=CallingConvention.Winapi)]
	[MethodImpl(MethodImplOptions.PreserveSig)]
	extern private static int RegEnumKeyEx
				(IntPtr hkey, uint index,
				 char[] lpName, ref uint lpcbName,
				 IntPtr reserved, IntPtr lpClass, IntPtr lpcbClass,
				 out long lpftLastWriteTime);

	[DllImport("advapi32.dll", EntryPoint="RegEnumValueW",
			   CallingConvention=CallingConvention.Winapi)]
	[MethodImpl(MethodImplOptions.PreserveSig)]
	extern private static int RegEnumValue
				(IntPtr hkey, uint index,
				 char[] lpValueName, ref uint lpcbValueName,
				 IntPtr reserved, IntPtr lpType,
				 IntPtr lpData, IntPtr lpcbData);

	[DllImport("advapi32.dll",
			   CallingConvention=CallingConvention.Winapi)]
	[MethodImpl(MethodImplOptions.PreserveSig)]
	extern private static int RegFlushKey(IntPtr hKey);

	[DllImport("advapi32.dll", EntryPoint="RegOpenKeyExW",
			   CallingConvention=CallingConvention.Winapi)]
	[MethodImpl(MethodImplOptions.PreserveSig)]
	extern private static int RegOpenKeyEx
				(IntPtr hkey,
				 [MarshalAs(UnmanagedType.LPWStr)] String lpSubKey,
				 uint ulOptions, uint samDesired, out IntPtr phkResult);

	[DllImport("advapi32.dll", EntryPoint="RegQueryInfoKeyW",
			   CallingConvention=CallingConvention.Winapi)]
	[MethodImpl(MethodImplOptions.PreserveSig)]
	extern private static int RegQueryInfoKey
				(IntPtr hkey,
				 byte[] lpClass, IntPtr lpcbClass, IntPtr lpReserved,
				 out uint lpcSubKeys, IntPtr lpcbMaxSubKeyLen,
				 IntPtr lpcbMaxClassLen, out uint lpcValues,
				 IntPtr lpcbMaxValueNameLen, IntPtr lpcbMaxValueLen,
				 IntPtr lpcbSecurityDescriptor, IntPtr lpftLastWriteTime);

	[DllImport("advapi32.dll", EntryPoint="RegQueryValueExW",
			   CallingConvention=CallingConvention.Winapi)]
	[MethodImpl(MethodImplOptions.PreserveSig)]
	extern private static int RegQueryValueEx
				(IntPtr hkey,
				 [MarshalAs(UnmanagedType.LPWStr)] String lpValueName,
				 IntPtr lpReserved, out uint lpType,
				 byte[] lpData, ref uint lpcbData);

	[DllImport("advapi32.dll", EntryPoint="RegQueryValueExW",
			   CallingConvention=CallingConvention.Winapi)]
	[MethodImpl(MethodImplOptions.PreserveSig)]
	extern private static int RegQueryValueEx
				(IntPtr hkey,
				 [MarshalAs(UnmanagedType.LPWStr)] String lpValueName,
				 IntPtr lpReserved, out uint lpType,
				 IntPtr lpData, ref uint lpcbData);

	[DllImport("advapi32.dll", EntryPoint="RegSetValueExW",
			   CallingConvention=CallingConvention.Winapi)]
	[MethodImpl(MethodImplOptions.PreserveSig)]
	extern private static int RegSetValueEx
				(IntPtr hkey,
				 [MarshalAs(UnmanagedType.LPWStr)] String lpValueName,
				 uint Reserved, uint dwType,
				 byte[] lpData, uint cbData);

	// Type codes for values that can be stored in the registry.

	private const uint REG_NONE                       = 0;
	private const uint REG_SZ                         = 1;
	private const uint REG_EXPAND_SZ                  = 2;
	private const uint REG_BINARY                     = 3;
	private const uint REG_DWORD_LITTLE_ENDIAN        = 4;
	private const uint REG_DWORD_BIG_ENDIAN           = 5;
	private const uint REG_LINK                       = 6;
	private const uint REG_MULTI_SZ                   = 7;
	private const uint REG_RESOURCE_LIST              = 8;
	private const uint REG_FULL_RESOURCE_DESCRIPTOR   = 9;
	private const uint REG_RESOURCE_REQUIREMENTS_LIST = 10;
	private const uint REG_QWORD_LITTLE_ENDIAN        = 11;

	// Access types for the "samDesired" parameter of "RegOpenKeyEx".

	private const uint KEY_QUERY_VALUE                = 1;
	private const uint KEY_SET_VALUE                  = 2;
	private const uint KEY_CREATE_SUB_KEY             = 4;
	private const uint KEY_ENUMERATE_SUB_KEYS         = 8;
	private const uint KEY_NOTIFY                     = 16;
	private const uint KEY_CREATE_LINK                = 32;
	private const uint KEY_WRITE                      = 0x20006;
	private const uint KEY_READ                       = 0x20019;
	private const uint KEY_ALL_ACCESS                 = 0xF003F;

}; // class Win32KeyProvider

#endif // CONFIG_WIN32_SPECIFICS

}; // namespace Microsoft.Win32
