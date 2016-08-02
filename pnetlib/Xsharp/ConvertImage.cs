/*
 * ConvertImage.cs - Convert DIB images into XImage structures.
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

namespace Xsharp
{

using System;
using DotGNU.Images;
using OpenSystem.Platform.X11;

internal sealed class ConvertImage
{
	// Convert an image frame into an XImage.
	public static IntPtr FrameToXImage(Screen screen, Frame frame)
			{
				int[] fpalette;
				XPixel[] palette;
				int index, color;
				Colormap colormap = screen.DefaultColormap;

				// Create a palette to use to render the image.
				fpalette = frame.Palette;
				if(fpalette != null)
				{
					// Convert the palette within the image frame itself.
					palette = new XPixel [256];
					for(index = 0; index < 256 && index < fpalette.Length;
						++index)
					{
						color = fpalette[index];
						palette[index] = colormap.RGBToPixel
							(new Color((color >> 16) & 0xFF,
									   (color >> 8) & 0xFF, color & 0xFF));
					}
				}
				else
				{
					// We have an RGB image: use a standard palette.
					palette = colormap.GetStandardPalette();
				}

				// Convert the frame into an XImage and return it.
				return Xlib.XSharpCreateImageFromDIB
						(screen.screen, frame.Width, frame.Height,
						 frame.Stride, (int)(frame.PixelFormat),
						 frame.Data, 0, palette);
			}

	// Convert an image frame's mask into an XImage.  IntPtr.Zero if no mask.
	public static IntPtr MaskToXImage(Screen screen, Frame frame)
			{
				byte[] mask = frame.Mask;
				if(mask == null)
				{
					return IntPtr.Zero;
				}
				return Xlib.XSharpCreateImageFromDIB
					(screen.screen, frame.Width, frame.Height,
					 frame.MaskStride, (int)(PixelFormat.Format1bppIndexed),
					 mask, 1, null);
			}

	// Convert an image frame into an XImage bitmap.
	public static IntPtr FrameToXImageBitmap(Screen screen, Frame frame)
			{
				byte[] data = frame.Data;
				if(data == null)
				{
					return IntPtr.Zero;
				}
				return Xlib.XSharpCreateImageFromDIB
					(screen.screen, frame.Width, frame.Height,
					 frame.Stride, (int)(PixelFormat.Format1bppIndexed),
					 data, 1, null);
			}

	// Convert an XImage into a Pixmap object.
	public static Pixmap XImageToPixmap(Screen screen, IntPtr ximage)
			{
				int width, height;
				Xlib.XSharpGetImageSize(ximage, out width, out height);
				Pixmap pixmap = new Pixmap(screen, width, height);
				Graphics graphics = new Graphics(pixmap);
				graphics.PutXImage(ximage, 0, 0, 0, 0, width, height);
				graphics.Dispose();
				return pixmap;
			}

	// Convert an XImage mask into a Bitmap object.
	public static Bitmap XImageMaskToBitmap(Screen screen, IntPtr ximage)
			{
				int width, height;
				Xlib.XSharpGetImageSize(ximage, out width, out height);
				Bitmap bitmap = new Bitmap(screen, width, height);
				Graphics graphics = new Graphics(bitmap);
				graphics.PutXImage(ximage, 0, 0, 0, 0, width, height);
				graphics.Dispose();
				return bitmap;
			}

} // class ConvertImage

} // namespace Xsharp
