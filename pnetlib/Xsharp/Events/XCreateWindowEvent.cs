/*
 * XCreateWindowEvent.cs - Definitions for X event structures.
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

// Create window event.
[StructLayout(LayoutKind.Sequential)]
internal struct XCreateWindowEvent
{
	// Structure fields.
	XAnyEvent			common__;
	public XWindow    	window;
	public Xlib.Xint  	x__;
	public Xlib.Xint  	y__;
	public Xlib.Xint  	width__;
	public Xlib.Xint  	height__;
	public Xlib.Xint  	border_width__;
	public XBool  		override_redirect__;

	// Access parent class fields.
	public int type           { get { return common__.type; } }
	public uint serial        { get { return common__.serial; } }
	public bool send_event    { get { return common__.send_event; } }
	public IntPtr display     { get { return common__.display; } }
	public XWindow     parent { get { return common__.window; } }

	// Convert odd fields into types that are useful.
	public int x              { get { return (int)x__; } }
	public int y              { get { return (int)y__; } }
	public int width          { get { return (int)width__; } }
	public int height         { get { return (int)height__; } }
	public int border_width   { get { return (int)border_width__; } }
	public bool override_redirect
			{ get { return (override_redirect__ != XBool.False); } }

	// Convert this object into a string.
	public override String ToString()
			{
				return ((EventType)type).ToString() + ": parent=" +
					   ((ulong)parent).ToString() +
					   " window=" + ((ulong)window).ToString() +
					   " x=" + x.ToString() +
					   " y=" + y.ToString() +
					   " width=" + width.ToString() +
					   " height=" + height.ToString() +
					   " border_width=" + border_width.ToString() +
					   " override_redirect=" + override_redirect.ToString();
			}

} // struct XCreateWindowEvent

} // namespace Xsharp.Events
