/*
 * XGCValues.cs - Definition of the X graphic context values structure.
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
using OpenSystem.Platform.X11;

// X graphic context values structure.
[StructLayout(LayoutKind.Sequential)]
internal struct XGCValues
{

	// Structure fields.
	public Xlib.Xint	function__;
	public XPixel    	plane_mask;
	public XPixel    	foreground;
	public XPixel    	background;
	public Xlib.Xint	line_width__;
	public Xlib.Xint	line_style__;
	public Xlib.Xint	cap_style__;
	public Xlib.Xint	join_style__;
	public Xlib.Xint	fill_style__;
	public Xlib.Xint	fill_rule__;
	public Xlib.Xint	arc_mode__;
	public XPixmap    	tile;
	public XPixmap    	stipple;
	public Xlib.Xint	ts_x_origin__;
	public Xlib.Xint	ts_y_origin__;
	public XFont		font;
	public Xlib.Xint	subwindow_mode__;
	public XBool		graphics_exposures__;
	public Xlib.Xint	clip_x_origin__;
	public Xlib.Xint	clip_y_origin__;
	public XPixmap    	clip_mask;
	public Xlib.Xint	dash_offset__;
	public sbyte		dashes;

	// Convert odd fields into types that are useful.
	public GCFunction function
			{ get { return (GCFunction)(int)function__; }
			  set { function__ = (Xlib.Xint)(int)value; } }
	public int line_width
			{ get { return (int)line_width__; }
			  set { line_width__ = (Xlib.Xint)value; } }
	public LineStyle line_style
			{ get { return (LineStyle)(int)line_style__; }
			  set { line_style__ = (Xlib.Xint)(int)value; } }
	public CapStyle cap_style
			{ get { return (CapStyle)(int)cap_style__; }
			  set { cap_style__ = (Xlib.Xint)(int)value; } }
	public JoinStyle join_style
			{ get { return (JoinStyle)(int)join_style__; }
			  set { join_style__ = (Xlib.Xint)(int)value; } }
	public FillStyle fill_style
			{ get { return (FillStyle)(int)fill_style__; }
			  set { fill_style__ = (Xlib.Xint)(int)value; } }
	public FillRule fill_rule
			{ get { return (FillRule)(int)fill_rule__; }
			  set { fill_rule__ = (Xlib.Xint)(int)value; } }
	public ArcMode arc_mode
			{ get { return (ArcMode)(int)arc_mode__; }
			  set { arc_mode__ = (Xlib.Xint)(int)value; } }
	public int ts_x_origin
			{ get { return (int)ts_x_origin__; }
			  set { ts_x_origin__ = (Xlib.Xint)value; } }
	public int ts_y_origin
			{ get { return (int)ts_y_origin__; }
			  set { ts_y_origin__ = (Xlib.Xint)value; } }
	public SubwindowMode subwindow_mode
			{ get { return (SubwindowMode)(int)subwindow_mode__; }
			  set { subwindow_mode__ = (Xlib.Xint)(int)value; } }
	public bool graphics_exposures
			{ get { return (graphics_exposures__ != XBool.False); }
			  set { graphics_exposures__ = (value ? XBool.True
			  									  : XBool.False); } }
	public int clip_x_origin
			{ get { return (int)clip_x_origin__; }
			  set { clip_x_origin__ = (Xlib.Xint)value; } }
	public int clip_y_origin
			{ get { return (int)clip_y_origin__; }
			  set { clip_y_origin__ = (Xlib.Xint)value; } }
	public int dash_offset
			{ get { return (int)dash_offset__; }
			  set { dash_offset__ = (Xlib.Xint)value; } }

} // struct XGCValues

} // namespace Xsharp.Types
