/*
 * Xauth.cs - Definition of authentication tokens.
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

[StructLayout(LayoutKind.Sequential)]
internal struct Xauth
{

	public ushort family;
	public ushort address_length;
	public IntPtr address;
	public ushort number_length;
	public IntPtr number;
	public ushort name_length;
	public IntPtr name;
	public ushort data_length;
	public IntPtr data;

} // struct Xauth

} // namespace Xsharp.Types
