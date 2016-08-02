/*
 * XWMHints.cs - Definition of window manager hint information.
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
using OpenSystem.Platform.X11;

// Window manager hint structure.
[StructLayout(LayoutKind.Sequential)]
internal struct XWMHints
{

	// Structure fields.
	public Xlib.Xlong		flags__;
	public XBool			input__;
	public Xlib.Xint		initial_state__;
	public XPixmap			icon_pixmap;
	public XWindow    		icon_window;
	public Xlib.Xint		icon_x__;
	public Xlib.Xint		icon_y__;
	public XPixmap			icon_mask;
	public XID      	 	window_group;

	// Convert odd fields into types that are useful.
	public WMHintsMask flags
			{ get { return (WMHintsMask)(long)flags__; }
			  set { flags__ = (Xlib.Xlong)(long)value; } }
	public bool input
			{ get { return (input__ != XBool.False); }
			  set { input__ = (value ? XBool.True : XBool.False); } }
	public WindowState initial_state
			{ get { return (WindowState)initial_state__; }
			  set { initial_state__ = (Xlib.Xint)value; } }
	public int icon_x
			{ get { return (int)icon_x__; }
			  set { icon_x__ = (Xlib.Xint)value; } }
	public int icon_y
			{ get { return (int)icon_y__; }
			  set { icon_y__ = (Xlib.Xint)value; } }

} // struct XWMHints

} // namespace Xsharp.Types
