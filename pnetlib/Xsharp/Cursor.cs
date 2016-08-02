/*
 * Cursor.cs - User-defined cursor abstraction.
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
using Xsharp.Types;
using DotGNU.Images;
using OpenSystem.Platform.X11;

/// <summary>
/// <para>The <see cref="T:Xsharp.Cursor"/> type encapsulates a
/// user-defined cursor that may be set on a widget.</para>
/// </summary>
public class Cursor
{
	// Internal state.
	private CursorType type;
	private Bitmap source;
	private Bitmap mask;
	private int hotspotX;
	private int hotspotY;
	private XCursor cursor;
	private bool reverse;

	/// <summary>
	/// <para>Create a new cursor, based on a pre-defined cursor type.</para>
	/// </summary>
	///
	/// <param name="type">
	/// <para>The pre-defined cursor type to use.</para>
	/// </param>
	public Cursor(CursorType type)
			{
				this.type = type;
				this.source = null;
				this.mask = null;
				this.cursor = XCursor.Zero;
			}

	/// <summary>
	/// <para>Create a new cursor, based on a user-supplied image
	/// and mask.</para>
	/// </summary>
	///
	/// <param name="source">
	/// <para>The bitmap defining the source image for the cursor.</para>
	/// </param>
	///
	/// <param name="mask">
	/// <para>The bitmap defining the mask for the cursor.</para>
	/// </param>
	///
	/// <param name="hotspotX">
	/// <para>The X position of the cursor hotspot.</para>
	/// </param>
	///
	/// <param name="hotspotY">
	/// <para>The Y position of the cursor hotspot.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException"/>
	/// <para>Raised if <paramref name="source"/> or <paramref name="mask"/>
	/// is <see langword="null"/>.</para>
	/// </exception>
	public Cursor(Bitmap source, Bitmap mask, int hotspotX, int hotspotY)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				if(mask == null)
				{
					throw new ArgumentNullException("mask");
				}
				this.type = CursorType.XC_inherit_parent;
				this.source = source;
				this.mask = mask;
				this.cursor = XCursor.Zero;
			}

	/// <summary>
	/// <para>Create a new cursor, based on a user-supplied image frame.</para>
	/// </summary>
	///
	/// <param name="screen">
	/// <para>The screen to create the cursor for, or
	/// <see langword="null"/> for the default screen on the
	/// default display.</para>
	/// </param>
	///
	/// <param name="frame">
	/// <para>The frame defining the cursor image.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException"/>
	/// <para>Raised if <paramref name="frame"/> is
	/// <see langword="null"/>.</para>
	/// </exception>
	public Cursor(Screen screen, Frame frame)
			{
				Display dpy;
				if(frame == null)
				{
					throw new ArgumentNullException("frame");
				}
				if(screen != null)
				{
					dpy = screen.DisplayOfScreen;
				}
				else
				{
					dpy = Application.Primary.Display;
					screen = dpy.DefaultScreenOfDisplay;
				}
				if( /* irgnore pixel format! frame.PixelFormat != PixelFormat.Format1bppIndexed  || */
				   frame.Mask == null)
				{
					// The frame is not suitable for use as a cursor.
					this.type = CursorType.XC_left_ptr;
					this.source = null;
					this.mask = null;
					this.cursor = XCursor.Zero;
				}
				else
				{
					this.type = CursorType.XC_inherit_parent;
					this.cursor = XCursor.Zero;
					try
					{
						dpy.Lock();
						IntPtr pixmapXImage =
							ConvertImage.FrameToXImageBitmap(screen, frame);
						IntPtr maskXImage = ConvertImage.MaskToXImage
							(screen, frame);
						source = ConvertImage.XImageMaskToBitmap
							(screen, pixmapXImage);
						mask = ConvertImage.XImageMaskToBitmap
							(screen, maskXImage);
						Xlib.XSharpDestroyImage(pixmapXImage);
						Xlib.XSharpDestroyImage(maskXImage);
						hotspotX = frame.HotspotX;
						hotspotY = frame.HotspotY;
						if(frame.Palette != null && frame.Palette[0] == 0)
						{
							reverse = true;
						}
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	// Set this cursor on a widget.
	internal void SetCursor(Widget widget)
			{
				Display dpy = widget.dpy;
				try
				{
					IntPtr display = dpy.Lock();
					XWindow window = widget.GetWidgetHandle();
					if(source != null)
					{
						if(cursor == XCursor.Zero)
						{
							XColor foreground = new XColor();
							foreground.red = (ushort)0;
							foreground.green = (ushort)0;
							foreground.blue = (ushort)0;
							foreground.flags =
								(XColor.DoRed | XColor.DoGreen | XColor.DoBlue);
							XColor background = new XColor();
							background.red = (ushort)0xFFFF;
							background.green = (ushort)0xFFFF;
							background.blue = (ushort)0xFFFF;
							background.flags =
								(XColor.DoRed | XColor.DoGreen | XColor.DoBlue);
							if(reverse)
							{
								cursor = Xlib.XCreatePixmapCursor
									(display,
									 source.GetPixmapHandle(),
									 mask.GetPixmapHandle(),
									 ref background, ref foreground,
									 (uint)hotspotX, (uint)hotspotY);
							}
							else
							{
								cursor = Xlib.XCreatePixmapCursor
									(display,
									 source.GetPixmapHandle(),
									 mask.GetPixmapHandle(),
									 ref foreground, ref background,
									 (uint)hotspotX, (uint)hotspotY);
							}
						}
						Xlib.XDefineCursor(display, window, cursor);
					}
					else if(type == CursorType.XC_inherit_parent)
					{
						Xlib.XUndefineCursor(display, window);
					}
					else
					{
						Xlib.XDefineCursor
							(display, window, dpy.GetCursor(type));
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

} // class Cursor

} // namespace Xsharp
