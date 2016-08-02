/*
 * HandleRef.cs - Implementation of the
 *			"System.Runtime.InteropServices.HandleRef" class.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_COM_INTEROP

public struct HandleRef
{
	// Internal state.
	private Object wrapper;
	private IntPtr handle;

	// Constructor.
	public HandleRef(Object wrapper, IntPtr handle)
			{
				this.wrapper = wrapper;
				this.handle = handle;
			}

	// Get this instance's fields.
	public IntPtr Handle
			{
				get
				{
					return handle;
				}
			}
	public Object Wrapper
			{
				get
				{
					return wrapper;
				}
			}

	// Convert a handle reference into an IntPtr.
	public static explicit operator IntPtr(HandleRef value)
			{
				return value.handle;
			}

}; // struct HandleRef

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
