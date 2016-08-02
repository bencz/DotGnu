/*
 * IRegistryKeyProvider.cs - Implementation of the
 *			"Microsoft.Win32.IRegistryKeyProvider" class.
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

// This interface is used internally to represent a registry key
// provider interface.  There may be different providers for
// different operating systems.

internal interface IRegistryKeyProvider
{
	// Close a reference to this key and flush any modifications to disk.
	void Close(bool writable);

	// Create a subkey underneath this particular registry key.
	IRegistryKeyProvider CreateSubKey(String subkey);

	// Returns true if we should delete subkeys from their parents.
	bool DeleteFromParents { get; }

	// Delete a subkey of this key entry.  Returns false if not present.
	bool DeleteSubKey(String name);

	// Delete this key entry.
	void Delete();

	// Delete a subkey entry and all of its descendents.
	// Returns false if not present.
	bool DeleteSubKeyTree(String name);

	// Delete this key entry and all of its descendents.
	void DeleteTree();

	// Delete a particular value underneath this registry key.
	// Returns false if the value is missing.
	bool DeleteValue(String name);

	// Flush all modifications to this registry key.
	void Flush();

	// Get the names of all subkeys underneath this registry key.
	String[] GetSubKeyNames();

	// Get a value from underneath this registry key.
	Object GetValue(String name, Object defaultValue);

	// Get the names of all values underneath this registry key.
	String[] GetValueNames();

	// Open a subkey.
	IRegistryKeyProvider OpenSubKey(String name, bool writable);

	// Set a value under this registry key.
	void SetValue(String name, Object value);

	// Get the name of this registry key provider.
	String Name { get; }

	// Get the number of subkeys underneath this key.
	int SubKeyCount { get; }

	// Get the number of values that are associated with this key.
	int ValueCount { get; }

}; // interface IRegistryKeyProvider

#endif // CONFIG_WIN32_SPECIFICS

}; // namespace Microsoft.Win32
