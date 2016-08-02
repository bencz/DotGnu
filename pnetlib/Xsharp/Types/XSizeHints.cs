/*
 * XSizeHints.cs - Definition of window manager size hint information.
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

// Window manager size hint structure.
[StructLayout(LayoutKind.Sequential)]
internal struct XSizeHints
{

	// Structure fields.
	public Xlib.Xlong		flags__;
	public Xlib.Xint		x__;
	public Xlib.Xint		y__;
	public Xlib.Xint		width__;
	public Xlib.Xint		height__;
	public Xlib.Xint		min_width__;
	public Xlib.Xint		min_height__;
	public Xlib.Xint		max_width__;
	public Xlib.Xint		max_height__;
	public Xlib.Xint		width_inc__;
	public Xlib.Xint		height_inc__;
	public Xlib.Xint		min_aspect_x__;
	public Xlib.Xint		min_aspect_y__;
	public Xlib.Xint		max_aspect_x__;
	public Xlib.Xint		max_aspect_y__;
	public Xlib.Xint		base_width__;
	public Xlib.Xint		base_height__;
	public Xlib.Xint		win_gravity__;

	// Convert odd fields into types that are useful.
	public SizeHintsMask flags
			{ get { return (SizeHintsMask)(long)flags__; }
			  set { flags__ = (Xlib.Xlong)(long)value; } }
	public int x
			{ get { return (int)x__; }
			  set { x__ = (Xlib.Xint)value; } }
	public int y
			{ get { return (int)y__; }
			  set { y__ = (Xlib.Xint)value; } }
	public int width
			{ get { return (int)width__; }
			  set { width__ = (Xlib.Xint)value; } }
	public int height
			{ get { return (int)height__; }
			  set { height__ = (Xlib.Xint)value; } }
	public int min_width
			{ get { return (int)min_width__; }
			  set { min_width__ = (Xlib.Xint)value; } }
	public int min_height
			{ get { return (int)min_height__; }
			  set { min_height__ = (Xlib.Xint)value; } }
	public int max_width
			{ get { return (int)max_width__; }
			  set { max_width__ = (Xlib.Xint)value; } }
	public int max_height
			{ get { return (int)max_height__; }
			  set { max_height__ = (Xlib.Xint)value; } }
	public int width_inc
			{ get { return (int)width_inc__; }
			  set { width_inc__ = (Xlib.Xint)value; } }
	public int height_inc
			{ get { return (int)height_inc__; }
			  set { height_inc__ = (Xlib.Xint)value; } }
	public int min_aspect_x
			{ get { return (int)min_aspect_x__; }
			  set { min_aspect_x__ = (Xlib.Xint)value; } }
	public int min_aspect_y
			{ get { return (int)min_aspect_y__; }
			  set { min_aspect_y__ = (Xlib.Xint)value; } }
	public int max_aspect_x
			{ get { return (int)max_aspect_x__; }
			  set { max_aspect_x__ = (Xlib.Xint)value; } }
	public int max_aspect_y
			{ get { return (int)max_aspect_y__; }
			  set { max_aspect_y__ = (Xlib.Xint)value; } }
	public int base_width
			{ get { return (int)base_width__; }
			  set { base_width__ = (Xlib.Xint)value; } }
	public int base_height
			{ get { return (int)base_height__; }
			  set { base_height__ = (Xlib.Xint)value; } }
	public int win_gravity
			{ get { return (int)win_gravity__; }
			  set { win_gravity__ = (Xlib.Xint)value; } }

} // struct XSizeHints

} // namespace Xsharp.Types
