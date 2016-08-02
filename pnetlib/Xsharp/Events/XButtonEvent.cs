/*
 * XButtonEvent.cs - Definitions for X event structures.
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

// Button event.
[StructLayout(LayoutKind.Sequential)]
internal struct XButtonEvent
{
	// Structure fields.
	XAnyEvent			common__;
	public XWindow    	root;
	public XWindow    	subwindow;
	public XTime		time;
	public Xlib.Xint	x__;
	public Xlib.Xint	y__;
	public Xlib.Xint	x_root__;
	public Xlib.Xint	y_root__;
	public Xlib.Xuint	state__;
	public Xlib.Xuint	button__;
	public XBool		same_screen__;

	// Access parent class fields.
	public int type           { get { return common__.type; } }
	public uint serial        { get { return common__.serial; } }
	public bool send_event    { get { return common__.send_event; } }
	public IntPtr display     { get { return common__.display; } }
	public XWindow     window { get { return common__.window; } }

	// Convert odd fields into types that are useful.
	public int x              { get { return (int)x__; } }
	public int y              { get { return (int)y__; } }
	public int x_root         { get { return (int)x_root__; } }
	public int y_root         { get { return (int)y_root__; } }
	public ModifierMask state { get { return (ModifierMask)(uint)state__; } }
	public ButtonName button  { get { return (ButtonName)(uint)button__; } }
	public bool same_screen
		{ get { return (same_screen__ != XBool.False); } }

	// Convert this object into a string.
	public override String ToString()
			{
				return common__.ToString() +
					   " x=" + x.ToString() +
					   " y=" + y.ToString() +
					   " x_root=" + x_root.ToString() +
					   " y_root=" + y_root.ToString() +
					   " state=" + state.ToString() +
					   " button=" + button.ToString() +
					   " time=" + ((ulong)time).ToString() +
					   " root=" + ((ulong)root).ToString() +
					   " subwindow=" + ((ulong)subwindow).ToString() +
					   " same_screen=" + same_screen.ToString();
			}

} // struct XButtonEvent

} // namespace Xsharp.Events
