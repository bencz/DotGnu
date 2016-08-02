/*
 * XClassHint.cs - Definition of class hint information.
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

// Window manager hint structure.
[StructLayout(LayoutKind.Sequential)]
internal struct XClassHint
{

	// Structure fields.
	public IntPtr			res_name__;
	public IntPtr			res_class__;

	// Convert odd fields into types that are useful.
	public String res_name
			{
				get
				{
					return Marshal.PtrToStringAnsi(res_name__);
				}
				set
				{
					if(res_name__ != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(res_name__);
					}
					res_name__ = Marshal.StringToHGlobalAnsi(value);
				}
			}
	public String res_class
			{
				get
				{
					return Marshal.PtrToStringAnsi(res_class__);
				}
				set
				{
					if(res_class__ != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(res_class__);
					}
					res_class__ = Marshal.StringToHGlobalAnsi(value);
				}
			}

	// Free the members of this structure.
	public void Free()
			{
				res_name = null;
				res_class = null;
			}

} // struct XClassHint

} // namespace Xsharp.Types
