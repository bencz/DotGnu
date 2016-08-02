/*
 * GCHandle.cs - Implementation of the
 *			"System.Runtime.InteropServices.GCHandle" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

using System.Runtime.CompilerServices;

public struct GCHandle
{

	// Internal state.
	private int handle;

	// Private constructor that is called from "Alloc".
	private GCHandle(Object value, GCHandleType type)
			{
				handle = GCAlloc(value, type);
			}

	// Private constructor that boxes up a handle value.
	private GCHandle(int value)
			{
				handle = value;
			}

	// Get the address of a pinned object that is referred to by this handle.
	public IntPtr AddrOfPinnedObject()
			{
				if((handle & 3) == 3)
				{
					// The handle is valid and pinned.
					return GCAddrOfPinnedObject(handle);
				}
				else if(handle != 0)
				{
					// The handle is not pinned.
					throw new InvalidOperationException
						(_("Invalid_GCHandleNotPinned"));
				}
				else
				{
					// The handle has not been initialized.
					throw new InvalidOperationException
						(_("Invalid_GCHandleNotInit"));
				}
			}

	// Internal version of "AddrOfPinnedObject".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr GCAddrOfPinnedObject(int handle);

	// Allocate a handle for a particular object.
	public static GCHandle Alloc(Object obj)
			{
				return new GCHandle(obj, GCHandleType.Normal);
			}
	public static GCHandle Alloc(Object obj, GCHandleType type)
			{
				return new GCHandle(obj, type);
			}

	// Internal version of "Alloc".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int GCAlloc(Object value, GCHandleType type);

	// Free this handle.
	public void Free()
			{
				if(handle != 0)
				{
					GCFree(handle);
					handle = 0;
				}
				else
				{
					throw new InvalidOperationException
						(_("Invalid_GCHandleNotInit"));
				}
			}

	// Internal version of "Free".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void GCFree(int handle);

	// Convert to and from the native integer representation.
	public static explicit operator GCHandle(IntPtr value)
			{
				int handle = value.ToInt32();
				if(GCValidate(handle))
				{
					return new GCHandle(handle);
				}
				else
				{
					throw new InvalidOperationException
						(_("Invalid_GCHandleInvalid"));
				}
			}
	public static explicit operator IntPtr(GCHandle handle)
			{
				return new IntPtr(handle.handle);
			}

	// Internal method to validate a handle.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool GCValidate(int handle);

	// Determine if this handle is allocated and valid.
	public bool IsAllocated
			{
				get
				{
					return (handle != 0);
				}
			}

	// Get or set the underlying object target for this handle.
	public Object Target
			{
				get
				{
					if(handle != 0)
					{
						return GCGetTarget(handle);
					}
					else
					{
						throw new InvalidOperationException
							(_("Invalid_GCHandleNotInit"));
					}
				}
				set
				{
					if(handle != 0)
					{
						GCSetTarget(handle, value);
					}
					else
					{
						throw new InvalidOperationException
							(_("Invalid_GCHandleNotInit"));
					}
				}
			}

	// Internal implementation of the "Target" property.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static Object GCGetTarget(int handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void GCSetTarget(int handle, Object value);

	// Get the type of this GC handle.
	internal GCHandleType GetHandleType()
			{
				return (GCHandleType)(handle & 3);
			}

}; // struct GCHandle

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System.Runtime.InteropServices
