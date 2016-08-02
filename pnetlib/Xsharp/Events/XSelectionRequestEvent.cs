/*
 * XSelectionRequestEvent.cs - Definitions for X event structures.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace Xsharp.Events
{

using System;
using System.Runtime.InteropServices;
using OpenSystem.Platform;
using OpenSystem.Platform.X11;

// Selection request event.
[StructLayout(LayoutKind.Sequential)]
internal struct XSelectionRequestEvent
{
	// Structure fields.
	XAnyEvent			common__;
	public XWindow    	requestor;
	public XAtom    	selection;
	public XAtom    	target;
	public XAtom    	property;
	public XTime		time;

	// Access parent class fields.
	public int type           { get { return common__.type; } }
	public uint serial        { get { return common__.serial; } }
	public bool send_event    { get { return common__.send_event; } }
	public IntPtr display     { get { return common__.display; } }
	public XWindow     owner  { get { return common__.window; } }

	// Convert this object into a string.
	public override String ToString()
			{
				return ((EventType)type).ToString() + ": owner=" +
					   ((ulong)owner).ToString() +
					   " requestor=" + ((ulong)requestor).ToString() +
					   " selection=" + ((ulong)selection).ToString() +
					   " target=" + ((ulong)target).ToString() +
					   " property=" + ((ulong)property).ToString() +
					   " time=" + ((ulong)time).ToString();
			}

} // struct XSelectionRequestEvent

} // namespace Xsharp.Events
