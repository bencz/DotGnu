/*
 * XAnyEvent.cs - Definitions for X event structures.
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

// Basic event structure that allows us to access common fields.
[StructLayout(LayoutKind.Sequential)]
internal struct XAnyEvent
{
	// Structure fields.
	public Xlib.Xint	type__;
	public Xlib.Xulong	serial__;
	public XBool		send_event__;
	public IntPtr		display;
	public XWindow    	window;

	// Convert odd fields into types that are useful.
	public int type    { get { return (int)type__; }
						 set { type__ = (Xlib.Xint)value; } }
	public uint serial { get { return (uint)serial__; } }
	public bool send_event
		{ get { return (send_event__ != XBool.False); } }

	// Convert this object into a string.
	public override String ToString()
			{
				return ((EventType)type).ToString() + ": window=" +
					   ((ulong)window).ToString();
			}

} // struct XAnyEvent

} // namespace Xsharp.Events
