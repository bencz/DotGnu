/*
 * IconReader.cs - Implementation of the "DotGNU.Images.IconReader" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace DotGNU.Images
{

using System;
using System.IO;

internal sealed class IconReader
{
	// Load a Windows icon image from the specified stream.  The first
	// 4 bytes have already been read and discarded.  If "hotspots" is
	// "true", then the image is actually a Windows cursor with hotspots.
	public static void Load(Stream stream, Image image, bool hotspots)
			{
				byte[] buffer = new byte [1024];
				int offset = 4;
				int numImages, index;
				int width, height, bpp;
				PixelFormat format;
				Frame frame;
				int[] palette;
				int paletteCount;
				int paletteIndex;

				// Read the number of images in the file.
				if(stream.Read(buffer, 0, 2) != 2)
				{
					throw new FormatException();
				}
				numImages = Utils.ReadUInt16(buffer, 0);
				offset += 2;

				// Read the resource directory.
				int[] offsetList = new int [numImages];
				int[] hotspotX = null;
				int[] hotspotY = null;
				if(hotspots)
				{
					hotspotX = new int[numImages];
					hotspotY = new int[numImages];
				}
				for(index = 0; index < numImages; ++index)
				{
					if(stream.Read(buffer, 0, 16) != 16)
					{
						throw new FormatException();
					}
					offset += 16;
					if(hotspots)
					{
						hotspotX[index] = Utils.ReadUInt16(buffer, 4);
						hotspotY[index] = Utils.ReadUInt16(buffer, 6);
					}
					offsetList[index] = Utils.ReadInt32(buffer, 12);
				}

				// Read the contents of the images in the stream.
				for(index = 0; index < numImages; ++index)
				{
					
					// Seek to the start of the image.
					Utils.Seek(stream, offset, offsetList[index]);
					offset = offsetList[index];

					// Read the DIB header.
					if(stream.Read(buffer, 0, 40) != 40)
					{
						throw new FormatException();
					}
					offset += 40;
					width = Utils.ReadUInt16(buffer, 4);
					// The DIB height is the mask and the bitmap.
					height = Utils.ReadUInt16(buffer, 8) / 2;
					bpp = Utils.ReadUInt16(buffer, 14);
					if (bpp == 1)
						format = PixelFormat.Format1bppIndexed;
					else if (bpp == 4)
						format = PixelFormat.Format4bppIndexed;
					else if (bpp == 8)
						format = PixelFormat.Format8bppIndexed;
					else if (bpp == 24)
						format = PixelFormat.Format24bppRgb;
					else if (bpp == 32)
						format = PixelFormat.Format32bppArgb;
					else
						throw new FormatException();

					// Create a new frame for this icon.
					frame = new Frame(image, width, height, format);
					image.AddFrame(frame);
					if(hotspots)
					{
						frame.HotspotX = hotspotX[index];
						frame.HotspotY = hotspotY[index];
					}

					// Copy some of the format information up to the image.
					if(frame.Width > image.Width)
					{
						image.Width = frame.Width;
					}
					if(frame.Height > image.Height)
					{
						image.Height = frame.Height;
					}
					if(image.NumFrames == 1)
					{
						image.PixelFormat = format;
					}

					// If indexed, get the palette.
					if((frame.pixelFormat & PixelFormat.Indexed) != 0)
					{
						paletteCount =
							(1 << Utils.FormatToBitCount(frame.pixelFormat));
						if(stream.Read(buffer, 0, paletteCount * 4)
								!= paletteCount * 4)
						{
							throw new FormatException();
						}
						offset += paletteCount * 4;
						palette = new int [paletteCount];
						for(paletteIndex = 0; paletteIndex < paletteCount;
							++paletteIndex)
						{
							palette[paletteIndex] = Utils.ReadBGR
								(buffer, paletteIndex * 4);
						}
						frame.Palette = palette;
					}

					// Read the main part of the icon or cursor.
					BmpReader.LoadBitmapData(stream, frame, false, true);
					offset += frame.Height * frame.Stride;

					// Read the mask.
					BmpReader.LoadBitmapData(stream, frame, true, true);
					offset += frame.Height * frame.MaskStride;

					// Invert the mask, because we want 1 to mean "active".
					InvertMask(frame);
				}

				// Set the appropriate load format.
				if(hotspots)
				{
					image.LoadFormat = Image.Cursor;
				}
				else
				{
					image.LoadFormat = Image.Icon;
				}
			}

	// Invert the mask in a frame.
	private static void InvertMask(Frame frame)
			{
				byte[] mask = frame.Mask;
				int posn;
				for(posn = 0; posn < mask.Length; ++posn)
				{
					mask[posn] = (byte)(mask[posn] ^ 0xFF);
				}
			}

}; // class IconReader

}; // namespace DotGNU.Images
