/*
 * XWindowChanges.cs - Definition of configuration changes structure.
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
using OpenSystem.Platform;
using OpenSystem.Platform.X11;

// Window configuration change structure.
[StructLayout(LayoutKind.Sequential)]
internal struct XWindowChanges
{

	// Structure fields.
	public Xlib.Xint		x__;
	public Xlib.Xint		y__;
	public Xlib.Xint		width__;
	public Xlib.Xint		height__;
	public Xlib.Xint		border_width__;
	public XWindow			sibling;
	public Xlib.Xint		stack_mode__;

	// Convert odd fields into types that are useful.
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
	public int border_width
			{ get { return (int)border_width__; }
			  set { border_width__ = (Xlib.Xint)value; } }
	public int stack_mode
			{ get { return (int)stack_mode__; }
			  set { stack_mode__ = (Xlib.Xint)value; } }

} // struct XWindowChanges

} // namespace Xsharp.Types
