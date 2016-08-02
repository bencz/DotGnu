/*
 * Colormap.cs - Colormap handling for X applications.
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
using System.Collections;
using Xsharp.Types;
using OpenSystem.Platform.X11;

/// <summary>
/// <para>The <see cref="T:Xsharp.Colormap"/> class manages colormaps
/// for an X display screen.</para>
/// </summary>
public sealed class Colormap
{
	// Internal state.
	private Display dpy;
	private Screen screen;
	private XColormap colormap;
	private Hashtable cachedPixels;
	private XPixel[] standardPalette;

	// Constructor that is called from the "Screen" class.
	internal Colormap(Display dpy, Screen screen, XColormap colormap)
			{
				this.dpy = dpy;
				this.screen = screen;
				this.colormap = colormap;
				this.cachedPixels = new Hashtable(1024); // create hash with big capacity to avoid expansions of hashtable
			}

	// Convert an RGB color value into a pixel using this colormap.
	// This assumes that standard colors have already been expanded.
	internal XPixel RGBToPixel(Color color)
			{
				try
				{
					// Acquire the display lock.
					IntPtr display = dpy.Lock();

					// Do we already have a cached pixel value for this color?
					Object cached = cachedPixels[color];
					if(cached != null)
					{
						return (XPixel)(cached);
					}

					// Try to allocate the color from the X display server.
					XColor xcolor;
					int red = color.Red;
					int green = color.Green;
					int blue = color.Blue;
					xcolor.pixel = XPixel.Zero;
					xcolor.red = (ushort)(red << 8);
					xcolor.green = (ushort)(green << 8);
					xcolor.blue = (ushort)(blue << 8);
					xcolor.flags =
						(sbyte)(XColor.DoRed | XColor.DoGreen | XColor.DoBlue);
					xcolor.pad = (sbyte)0;
					if(Xlib.XAllocColor(display, colormap, ref xcolor) != 0)
					{
						// Cache the value for next time.
						cachedPixels[color] = xcolor.pixel;
						return xcolor.pixel;
					}

					// TODO: do a closest color match for the color.

					// Last ditch: return black or white depending upon
					// the intensity of the color.
					if(((color.Red * 54 + color.Green * 183 +
					     color.Blue * 19) / 256) < 128)
					{
						return Xlib.XBlackPixelOfScreen(screen.screen);
					}
					else
					{
						return Xlib.XWhitePixelOfScreen(screen.screen);
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Get a standard 216-entry palette for mapping RGB images
	// to indexed display hardware.
	internal XPixel[] GetStandardPalette()
			{
				if(standardPalette != null)
				{
					return standardPalette;
				}
				standardPalette = new XPixel [216];
				int red, green, blue, index;
				index = 0;
				for(red = 0x00; red <= 0xFF; red += 0x33)
				{
					for(green = 0x00; green <= 0xFF; green += 0x33)
					{
						for(blue = 0x00; blue <= 0xFF; blue += 0x33)
						{
							standardPalette[index++] =
								RGBToPixel(new Color(red, green, blue));
						}
					}
				}
				return standardPalette;
			}

} // class Colormap

} // namespace Xsharp
