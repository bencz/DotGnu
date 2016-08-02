/*
 * IceReplyWaitInfo.cs - Definition of the ICE reply wait info structure.
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
internal struct IceReplyWaitInfo
{
	public Xlib.Xulong	sequence_of_request__;
	public Xlib.Xint	major_opcode_of_request__;
	public Xlib.Xint	minor_opcode_of_request__;
	[NonSerializedAttribute]
	public Object		reply;

	// Convert odd fields into types that are useful.
	public uint sequence_of_request
			{ get { return (uint)sequence_of_request__; }
			  set { sequence_of_request__ = (Xlib.Xulong)value; } }
	public int major_opcode_of_request
			{ get { return (int)major_opcode_of_request__; }
			  set { major_opcode_of_request__ = (Xlib.Xint)value; } }
	public int minor_opcode_of_request
			{ get { return (int)minor_opcode_of_request__; }
			  set { minor_opcode_of_request__ = (Xlib.Xint)value; } }

} // struct IceReplyWaitInfo

} // namespace Xsharp.Ice
