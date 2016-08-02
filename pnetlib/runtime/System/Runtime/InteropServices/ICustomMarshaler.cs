/*
 * ICustomMarshaler.cs - Implementation of the
 *			"System.Runtime.InteropServices.ICustomMarshaler" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_RUNTIME_INFRA

public interface ICustomMarshaler
{

	// Clean up the managed data used by NativeToManaged operation.
	void CleanUpManagedData(Object ManagedObj);

	// Clean up the native data used by ManagedToNative operation.
	void CleanUpNativeData(IntPtr pNativeData);

	// Get the native size of the data to be marshaled.
	int GetNativeDataSize();

	// Marshal a managed object to a native pointer.
	IntPtr MarshalManagedToNative(Object ManagedObj);

	// Marshal a native pointer to a managed object.
	Object MarshalNativeToManaged(IntPtr pNativeData);

	// Classes that implement this interface must also supply the
	// following static method to be called whenever the runtime
	// engine needs to get a custom marshaler of that class:
	//
	// static ICustomMarshaler GetInstance(String cookie);

}; // interface ICustomMarshaler

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System.Runtime.InteropServices
