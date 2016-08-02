/*
 * XTextProperty.cs - Definition of text property blocks.
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

namespace Xsharp.Types
{

using System;
using System.Runtime.InteropServices;
using OpenSystem.Platform;
using OpenSystem.Platform.X11;

// Window manager hint structure.
[StructLayout(LayoutKind.Sequential)]
internal struct XTextProperty
{

	// Structure fields.
	public IntPtr			value__;
	public XAtom			encoding__;
	public Xlib.Xint		format__;
	public Xlib.Xulong		nitems__;

	// Set this text property to a particular string.
	public bool SetText(String value)
			{
				if(value == null)
				{
					value = String.Empty;
				}
				IntPtr str = Marshal.StringToHGlobalAnsi(value);
				if(str == IntPtr.Zero)
				{
					return false;
				}
				if(Xlib.XStringListToTextProperty(ref str, 1, ref this)
						== XStatus.Zero)
				{
					Marshal.FreeHGlobal(str);
					return false;
				}
				Marshal.FreeHGlobal(str);
				return true;
			}
	public bool SetText(String[] value)
			{
				// Bail out early if we don't have any strings.
				if(value == null || value.Length == 0)
				{
					return SetText("");
				}

				// Convert the strings into an array of pointers.
				IntPtr[] strings = new IntPtr [value.Length];
				int posn;
				String str;
				for(posn = 0; posn < value.Length; ++posn)
				{
					str = value[posn];
					if(str == null)
					{
						str = String.Empty;
					}
					strings[posn] = Marshal.StringToHGlobalAnsi(str);
				}

				// Convert the string list into a text property.
				bool result = (Xlib.XStringListToTextProperty
					(strings, value.Length, ref this) != XStatus.Zero);

				// Free the strings, which we no longer require.
				for(posn = 0; posn < value.Length; ++posn)
				{
					Marshal.FreeHGlobal(strings[posn]);
				}
				return result;
			}

	// Free this text property.
	public void Free()
			{
				if(value__ != IntPtr.Zero)
				{
					Xlib.XFree(value__);
					value__ = IntPtr.Zero;
				}
			}

} // struct XTextProperty

} // namespace Xsharp.Types
