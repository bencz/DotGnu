/*
 * BmpReader.cs - Implementation of the "DotGNU.Images.BmpReader" class.
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

internal sealed class BmpReader
{
	// Load a BMP image from the specified stream.  The first 4 bytes
	// have already been read and discarded.
	public static void Load(Stream stream, Image image)
			{
				byte[] buffer = new byte [1024];
				int width, height, planes, bitCount;
				int compression;
				bool quads;

				// Read the rest of the BITMAPFILEHEADER.
				if(stream.Read(buffer, 0, 10) != 10)
				{
					throw new FormatException();
				}
				int bfOffBits = Utils.ReadInt32(buffer, 6);

				// The current file offset at the end of the BITMAPFILEHEADER.
				int offset = 14;

				// Get the size of the BITMAPINFOHEADER structure that follows,
				// and then read it into the buffer.
				if(stream.Read(buffer, 0, 4) != 4)
				{
					throw new FormatException();
				}
				int size = Utils.ReadInt32(buffer, 0);
				if(size <= 4 || size > 1024)
				{
					throw new FormatException();
				}
				if(stream.Read(buffer, 4, size - 4) != (size - 4))
				{
					throw new FormatException();
				}
				offset += size;
				if(size >= 40)
				{
					// This is a BITMAPINFOHEADER structure (Windows bitmaps).
					width = Utils.ReadInt32(buffer, 4);
					height = Utils.ReadInt32(buffer, 8);
					planes = Utils.ReadUInt16(buffer, 12);
					bitCount = Utils.ReadUInt16(buffer, 14);
					compression = Utils.ReadInt32(buffer, 16);
					quads = true;
				}
				else if(size == 12)
				{
					// This is a BITMAPCOREHEADER structure (OS/2 bitmaps).
					width = Utils.ReadUInt16(buffer, 4);
					height = Utils.ReadUInt16(buffer, 6);
					planes = Utils.ReadUInt16(buffer, 8);
					bitCount = Utils.ReadUInt16(buffer, 10);
					compression = 0;	// BI_RGB
					quads = false;
				}
				else
				{
					throw new FormatException();
				}

				// Perform a sanity check on the header values.
				if(width <= 0 || planes != 1)
				{
					throw new FormatException();
				}
				if(bitCount != 1 && bitCount != 4 &&  bitCount != 16 &&
				   bitCount != 8 && bitCount != 24)
				{
					// TODO: non-traditional BMP formats.
					throw new FormatException();
				}
				if(compression != 0 && compression != 3/*BI_BITFIELDS*/)
				{
					// TODO: RLE bitmaps
					throw new FormatException();
				}

				// Set the basic image properties.
				image.Width = width;
				image.Height = height < 0 ? -height : height;
				image.PixelFormat = Utils.BitCountToFormat(bitCount);
				image.LoadFormat = Image.Bmp;

				// Do the unusual 16 bit formats.
				if (compression == 3)
				{
					if(stream.Read(buffer, 0, 3 * 4) != (3 * 4))
					{
						throw new FormatException();
					}
					int redMask = Utils.ReadInt32(buffer, 0);
					int greenMask = Utils.ReadInt32(buffer, 4);
					int blueMask = Utils.ReadInt32(buffer, 8);

					if (blueMask == 0x001F && redMask == 0x7C00 && greenMask == 0x03E0)
						image.PixelFormat = PixelFormat.Format16bppRgb555;
					else if (blueMask == 0x001F && redMask == 0xF800 && greenMask == 0x07E0)
						image.PixelFormat = PixelFormat.Format16bppRgb565;
					else
						throw new FormatException();
				}

				// Read the palette into memory and set it.
				if(bitCount <= 8)
				{
					int colors = (1 << bitCount);
					int index;
					int[] palette = new int [colors];
					if(quads)
					{
						// The RGB values are specified as RGBQUAD's.
						if(stream.Read(buffer, 0, colors * 4) != (colors * 4))
						{
							throw new FormatException();
						}
						offset += colors * 4;
						for(index = 0; index < colors; ++index)
						{
							palette[index] = Utils.ReadBGR(buffer, index * 4);
						}
					}
					else
					{
						// The RGB values are specified as RGBTRIPLE's.
						if(stream.Read(buffer, 0, colors * 3) != (colors * 3))
						{
							throw new FormatException();
						}
						offset += colors * 3;
						for(index = 0; index < colors; ++index)
						{
							palette[index] = Utils.ReadBGR(buffer, index * 3);
						}
					}
					image.Palette = palette;
				}

				// Seek to the start of the bitmap data.
				Utils.Seek(stream, offset, bfOffBits);

				// Add a frame to the image object.
				Frame frame = image.AddFrame();

				// Load the bitmap data from the stream into the frame.
				LoadBitmapData(stream, frame, false, height > 0);

			}

	// Load bitmap data into a frame.
	public static void LoadBitmapData(Stream stream, Frame frame, bool mask, bool reverse)
			{
				byte[] data;
				int stride;
				int line;

				// Get the buffer and stride for the frame.
				if(!mask)
				{
					data = frame.Data;
					stride = frame.Stride;
				}
				else
				{
					frame.AddMask();
					data = frame.Mask;
					stride = frame.MaskStride;
				}

				// BMP images are usuallystored upside down in the stream.
				if (reverse)
				{
					for(line = frame.Height - 1; line >= 0; --line)
					{
						stream.Read(data, line * stride, stride);
					}
				}
				else
				{
					for(line = 0; line <  frame.Height; line++)
					{
						stream.Read(data, line * stride, stride);
					}
				}
			}

}; // class BmpReader

}; // namespace DotGNU.Images
