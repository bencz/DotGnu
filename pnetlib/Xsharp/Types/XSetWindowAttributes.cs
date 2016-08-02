/*
 * XSetWindowAttributes.cs - Definition of window attributes that can be set.
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

namespace Xsharp.Types
{

using System;
using System.Runtime.InteropServices;
using Xsharp.Events;
using OpenSystem.Platform.X11;

// Set window attributes structure.
[StructLayout(LayoutKind.Sequential)]
internal struct XSetWindowAttributes
{

	// Structure fields.
	public XPixmap    		background_pixmap;
	public XPixel    		background_pixel;
	public XPixmap    		border_pixmap;
	public XPixel    		border_pixel;
	public Xlib.Xint		bit_gravity__;
	public Xlib.Xint		win_gravity__;
	public Xlib.Xint		backing_store__;
	public XPixel    		backing_planes;
	public XPixel    		backing_pixel;
	public XBool			save_under__;
	public Xlib.Xlong		event_mask__;
	public Xlib.Xlong		do_not_propagate_mask__;
	public XBool			override_redirect__;
	public XColormap    	colormap;
	public XCursor			cursor;

	// Convert odd fields into types that are useful.
	public int bit_gravity
			{ get { return (int)bit_gravity__; }
			  set { bit_gravity__ = (Xlib.Xint)value; } }
	public int win_gravity
			{ get { return (int)win_gravity__; }
			  set { win_gravity__ = (Xlib.Xint)value; } }
	public int backing_store
			{ get { return (int)backing_store__; }
			  set { backing_store__ = (Xlib.Xint)value; } }
	public bool save_under
			{ get { return (save_under__ != XBool.False); }
			  set { save_under__ = (value ? XBool.True
			  							  : XBool.False); } }
	public EventMask event_mask
			{ get { return (EventMask)(int)event_mask__; }
			  set { event_mask__ = (Xlib.Xlong)(int)value; } }
	public EventMask do_not_propagate_mask
			{ get { return (EventMask)(int)do_not_propagate_mask__; }
			  set { do_not_propagate_mask__ = (Xlib.Xlong)(int)value; } }
	public bool override_redirect
			{ get { return (override_redirect__ != XBool.False); }
			  set { override_redirect__ = (value ? XBool.True
			  							  		 : XBool.False); } }

} // struct XSetWindowAttributes

} // namespace Xsharp.Types
