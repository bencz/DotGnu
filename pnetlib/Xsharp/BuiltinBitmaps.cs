/*
 * BuiltinBitmaps.cs - Builtin bitmaps for drawing special decorations.
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

namespace Xsharp
{

using System;
using OpenSystem.Platform.X11;

internal sealed class BuiltinBitmaps
{

	// Size of special bitmaps.
	public const int RadioWidth = 12;
	public const int RadioHeight = 12;

	// Radio button - bottom shadow color.
	private static readonly byte[] radio_b_bits = {
   		0xf0, 0x00, 0x0c, 0x03, 0x02, 0x00, 0x02, 0x02, 0x01, 0x00, 0x01, 0x00,
   		0x01, 0x00, 0x01, 0x00, 0x02, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00
	};

	// Radio button - enhanced bottom shadow color.
	private static readonly byte[] radio_B_bits = {
   		0x00, 0x00, 0xf0, 0x00, 0x0c, 0x03, 0x04, 0x00, 0x02, 0x00, 0x02, 0x00,
   		0x02, 0x00, 0x02, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	};

	// Radio button - top shadow color.
	private static readonly byte[] radio_t_bits = {
   		0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x04, 0x00, 0x08, 0x00, 0x08,
   		0x00, 0x08, 0x00, 0x08, 0x00, 0x04, 0x00, 0x04, 0x0c, 0x03, 0xf0, 0x00
	};

	// Radio button - enhanced top shadow color.
	private static readonly byte[] radio_T_bits = {
   		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x04,
   		0x00, 0x04, 0x00, 0x04, 0x00, 0x02, 0x0c, 0x03, 0xf0, 0x00, 0x00, 0x00
	};

	// Radio button - "white" background.
	private static readonly byte[] radio_w_bits = {
   		0x00, 0x00, 0x00, 0x00, 0xf0, 0x00, 0xf8, 0x01, 0x9c, 0x03, 0x0c, 0x03,
   		0x0c, 0x03, 0x9c, 0x03, 0xf8, 0x01, 0xf0, 0x00, 0x00, 0x00, 0x00, 0x00
	};

	// Radio button - "black" foreground.
	private static readonly byte[] radio_f_bits = {
   		0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0x00, 0xf0, 0x00,
   		0xf0, 0x00, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	};

	// Close caption button.
	private static readonly byte[] close_button_bits =
		{0x00, 0x00, 0x86, 0x01, 0xcc, 0x00, 0x78, 0x00, 0x30,
		 0x00, 0x78, 0x00, 0xcc, 0x00, 0x86, 0x01, 0x00, 0x00};

	// Minimize caption button.
	private static readonly byte[] minimize_button_bits =
		{0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
		 0x00, 0x00, 0x00, 0x00, 0x00, 0x7e, 0x00, 0x7e, 0x00};

	// Maximize caption button.
	private static readonly byte[] maximize_button_bits =
		{0xff, 0x01, 0xff, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
		 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0xff, 0x01};

	// Restore caption button.
	private static readonly byte[] restore_button_bits =
		{0xfc, 0x00, 0xfc, 0x00, 0x84, 0x00, 0xbf, 0x00, 0xbf,
		 0x00, 0xe1, 0x00, 0x21, 0x00, 0x21, 0x00, 0x3f, 0x00};

	// Help caption button.
	private static readonly byte[] help_button_bits =
		{0x78, 0x00, 0xcc, 0x00, 0xcc, 0x00, 0x60, 0x00, 0x30,
		 0x00, 0x30, 0x00, 0x00, 0x00, 0x30, 0x00, 0x30, 0x00};

	// Pre-loaded bitmaps for the current object's display.
	public XPixmap RadioBottom;
	public XPixmap RadioBottomEnhanced;
	public XPixmap RadioTop;
	public XPixmap RadioTopEnhanced;
	public XPixmap RadioBackground;
	public XPixmap RadioForeground;
	public XPixmap Close;
	public XPixmap Minimize;
	public XPixmap Maximize;
	public XPixmap Restore;
	public XPixmap Help;

	// Load builtin bitmaps for a particular display.
	public BuiltinBitmaps(Display display)
			{
				IntPtr dpy = display.dpy;
				XDrawable drawable = display.DefaultRootWindow.handle;
				RadioBottom = Xlib.XCreateBitmapFromData
					(dpy, drawable, radio_b_bits, (uint)12, (uint)12);
				RadioBottomEnhanced = Xlib.XCreateBitmapFromData
					(dpy, drawable, radio_B_bits, (uint)12, (uint)12);
				RadioTop = Xlib.XCreateBitmapFromData
					(dpy, drawable, radio_t_bits, (uint)12, (uint)12);
				RadioTopEnhanced = Xlib.XCreateBitmapFromData
					(dpy, drawable, radio_T_bits, (uint)12, (uint)12);
				RadioBackground = Xlib.XCreateBitmapFromData
					(dpy, drawable, radio_w_bits, (uint)12, (uint)12);
				RadioForeground = Xlib.XCreateBitmapFromData
					(dpy, drawable, radio_f_bits, (uint)12, (uint)12);
				Close = Xlib.XCreateBitmapFromData
					(dpy, drawable, close_button_bits, (uint)9, (uint)9);
				Minimize = Xlib.XCreateBitmapFromData
					(dpy, drawable, minimize_button_bits, (uint)9, (uint)9);
				Maximize = Xlib.XCreateBitmapFromData
					(dpy, drawable, maximize_button_bits, (uint)9, (uint)9);
				Restore = Xlib.XCreateBitmapFromData
					(dpy, drawable, restore_button_bits, (uint)9, (uint)9);
				Help = Xlib.XCreateBitmapFromData
					(dpy, drawable, help_button_bits, (uint)9, (uint)9);
			}

} // class BuiltinBitmaps

} // namespace Xsharp
