/*
 * IcePoVersionRec.cs - Definition of the ICE version structure.
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
internal struct IcePoVersionRec
{
	public Xlib.Xint			major_version__;
	public Xlib.Xint			minor_version__;
	public IcePoProcessMsgProc	process_msg_proc;

	// Convert odd fields into types that are useful.
	public int major_version
			{ get { return (int)major_version__; }
			  set { major_version__ = (Xlib.Xint)value; } }
	public int minor_version
			{ get { return (int)minor_version__; }
			  set { minor_version__ = (Xlib.Xint)value; } }

} // struct IcePoVersionRec

} // namespace Xsharp.Ice
