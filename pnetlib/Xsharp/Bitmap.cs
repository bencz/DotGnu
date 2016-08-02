/*
 * Bitmap.cs - Basic bitmap handling for X applications.
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

/// <summary>
/// <para>The <see cref="T:Xsharp.Bitmap"/> class manages off-screen
/// bitmaps on an X display screen.</para>
/// </summary>
public sealed class Bitmap : Drawable
{
	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.Bitmap"/> instance
	/// that represents an off-screen bitmap.  The bitmap is created
	/// on the default screen of the primary display.</para>
	/// </summary>
	///
	/// <param name="width">
	/// <para>The width of the new bitmap.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the new bitmap.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>The <paramref name="width"/> or <paramref name="height"/>
	/// values are out of range.</para>
	/// </exception>
	public Bitmap(int width, int height)
			: base(Xsharp.Application.Primary.Display,
				   Xsharp.Application.Primary.Display.DefaultScreenOfDisplay,
				   DrawableKind.Bitmap)
			{
				if(width < 1 || width > 32767 ||
				   height < 1 || height > 32767)
				{
					throw new XException(S._("X_InvalidBitmapSize"));
				}
				try
				{
					IntPtr display = dpy.Lock();
					SetPixmapHandle(Xlib.XCreatePixmap
						(display, (XDrawable)
							Xlib.XRootWindowOfScreen(screen.screen),
						 (uint)width, (uint)height, (uint)1));
					this.width = width;
					this.height = height;
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.Bitmap"/> instance
	/// that represents an off-screen bitmap.  The bitmap is created
	/// on the specified <paramref name="screen"/>.</para>
	/// </summary>
	///
	/// <param name="screen">
	/// <para>The screen upon which to create the new bitmap.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the new bitmap.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the new bitmap.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>The <paramref name="screen"/> value is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>The <paramref name="width"/> or <paramref name="height"/>
	/// values are out of range.</para>
	/// </exception>
	public Bitmap(Screen screen, int width, int height)
			: base(GetDisplay(screen), screen, DrawableKind.Bitmap)
			{
				if(width < 1 || width > 32767 ||
				   height < 1 || height > 32767)
				{
					throw new XException(S._("X_InvalidBitmapSize"));
				}
				try
				{
					IntPtr display = dpy.Lock();
					SetPixmapHandle(Xlib.XCreatePixmap
						(display, (XDrawable)
							Xlib.XRootWindowOfScreen(screen.screen),
						 (uint)width, (uint)height, (uint)1));
					this.width = width;
					this.height = height;
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.Bitmap"/> instance
	/// that represents an off-screen bitmap.  The bitmap is created
	/// on the default screen of the primary display, using the
	/// supplied bitmap data.</para>
	/// </summary>
	///
	/// <param name="width">
	/// <para>The width of the new bitmap.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the new bitmap.</para>
	/// </param>
	///
	/// <param name="bits">
	/// <para>The bits that make up the data.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="bits"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>The <paramref name="width"/> or <paramref name="height"/>
	/// values are out of range, or <paramref name="bits"/> is invalid
	/// in some way.</para>
	/// </exception>
	public Bitmap(int width, int height, byte[] bits)
			: base(Xsharp.Application.Primary.Display,
				   Xsharp.Application.Primary.Display.DefaultScreenOfDisplay,
				   DrawableKind.Bitmap)
			{
				if(width < 1 || width > 32767 ||
				   height < 1 || height > 32767)
				{
					throw new XException(S._("X_InvalidBitmapSize"));
				}
				if(bits == null)
				{
					throw new ArgumentNullException("bits");
				}
				int unit = (width <= 8 ? 7 : 15);
				if(((((width + unit) & ~unit) * height) / 8) > bits.Length)
				{
					throw new XException(S._("X_InvalidBitmapBits"));
				}
				try
				{
					IntPtr display = dpy.Lock();
					XDrawable drawable = (XDrawable)
							Xlib.XRootWindowOfScreen(screen.screen);
					SetPixmapHandle(Xlib.XCreateBitmapFromData
						(display, drawable, bits, (uint)width, (uint)height));
					this.width = width;
					this.height = height;
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.Bitmap"/> instance
	/// that represents an off-screen bitmap.  The bitmap is created
	/// on the specified <paramref name="screen"/>, using the
	/// supplied bitmap data.</para>
	/// </summary>
	///
	/// <param name="screen">
	/// <para>The screen upon which to create the new bitmap.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the new bitmap.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the new bitmap.</para>
	/// </param>
	///
	/// <param name="bits">
	/// <para>The bits that make up the data.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="bits"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>The <paramref name="width"/> or <paramref name="height"/>
	/// values are out of range, or <paramref name="bits"/> is invalid
	/// in some way.</para>
	/// </exception>
	public Bitmap(Screen screen, int width, int height, byte[] bits)
			: base(GetDisplay(screen), screen, DrawableKind.Bitmap)
			{
				if(width < 1 || width > 32767 ||
				   height < 1 || height > 32767)
				{
					throw new XException(S._("X_InvalidBitmapSize"));
				}
				if(bits == null)
				{
					throw new ArgumentNullException("bits");
				}
				if(((((width + 15) & ~15) * height) / 8) > bits.Length)
				{
					throw new XException(S._("X_InvalidBitmapBits"));
				}
				try
				{
					IntPtr display = dpy.Lock();
					XDrawable drawable = (XDrawable)
							Xlib.XRootWindowOfScreen(screen.screen);
					SetPixmapHandle(Xlib.XCreateBitmapFromData
						(display, drawable, bits, (uint)width, (uint)height));
					this.width = width;
					this.height = height;
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Internal constructor that wraps a bitmap XID.
	internal Bitmap(Display dpy, Screen screen, XPixmap pixmap)
			: base(dpy, screen, DrawableKind.Bitmap)
			{
				SetPixmapHandle(pixmap);
				try
				{
					// Get the geometry of the pixmap from the X server.
					IntPtr display = dpy.Lock();
					XWindow root_return;
					Xlib.Xint x_return, y_return;
					Xlib.Xuint width_return, height_return;
					Xlib.Xuint border_width_return, depth_return;
					Xlib.XGetGeometry
						(display, handle, out root_return,
						 out x_return, out y_return,
						 out width_return, out height_return,
						 out border_width_return, out depth_return);
					this.width = (int)width_return;
					this.height = (int)height_return;
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Get the display value from a specified screen, and check for null.
	private static Display GetDisplay(Screen screen)
			{
				if(screen == null)
				{
					throw new ArgumentNullException("screen");
				}
				return screen.DisplayOfScreen;
			}

	/// <summary>
	/// <para>Destroy this bitmap if it is currently active.</para>
	/// </summary>
	public override void Destroy()
			{
				try
				{
					IntPtr d = dpy.Lock();
					if(handle != XDrawable.Zero)
					{
						Xlib.XFreePixmap(d, (XPixmap)handle);
						handle = XDrawable.Zero;
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Convert a color into a pixel value, relative to this drawable.
	// Should be called with the display lock.
	internal override XPixel ToPixel(Color color)
			{
				// Expand standard colors before conversion.
				if(color.Index != StandardColor.RGB)
				{
					color = screen.ToColor(color.Index);
				}

				// Convert the color into monochrome, where 1 is black
				// foreground and 0 is white background.
				if(((color.Red * 54 + color.Green * 183 +
				     color.Blue * 19) / 256) < 128)
				{
					return (XPixel)1;
				}
				else
				{
					return (XPixel)0;
				}
			}

} // class Bitmap

} // namespace Xsharp
