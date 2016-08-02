/*
 * IceConn.cs - Internal structure of the "IceConn" object.
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

namespace Xsharp.Ice
{

using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct IceConn
{
	public Xlib.Xuint		flags;
	public Xlib.Xint		connection_status;
	public byte				my_ice_version_index;
	public IntPtr			trans_conn;
	public Xlib.Xulong		send_sequence;
	public Xlib.Xulong		receive_sequence;
	public IntPtr			connection_string;
	public IntPtr			vendor;
	public IntPtr			release;
	public IntPtr			inbuf;
	public IntPtr			inbufptr;
	public IntPtr			inbufmax;
	public IntPtr			outbuf;
	public IntPtr			outbufptr;
	public IntPtr			outbufmax;
	
	// There are other fields, but "inbuf" and "outbuf" are the
	// ones that we are most interested in.

} // struct IceConn

} // namespace Xsharp.Ice
