/*
 * Utils.cs - Implementation of the "DotGNU.Images.Utils" class.
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

internal sealed class Utils
{
	// Read a little-endian 16-bit integer value from a buffer.
	public static int ReadUInt16(byte[] buffer, int offset)
			{
				return ((buffer[offset]) | (buffer[offset + 1] << 8));
			}

	// Read a little-endian 32-bit integer value from a buffer.
	public static int ReadInt32(byte[] buffer, int offset)
			{
				return ((buffer[offset]) |
						(buffer[offset + 1] << 8) |
						(buffer[offset + 2] << 16) |
						(buffer[offset + 3] << 24));
			}

	// Read a big-endian 16-bit integer value from a buffer.
	public static int ReadUInt16B(byte[] buffer, int offset)
			{
				return ((buffer[offset + 1]) | (buffer[offset] << 8));
			}

	// Read a big-endian 32-bit integer value from a buffer.
	public static int ReadInt32B(byte[] buffer, int offset)
			{
				return ((buffer[offset + 3]) |
						(buffer[offset + 2] << 8) |
						(buffer[offset + 1] << 16) |
						(buffer[offset] << 24));
			}

	// Read a BGR value from a buffer.
	public static int ReadBGR(byte[] buffer, int offset)
			{
				return ((buffer[offset]) |
						(buffer[offset + 1] << 8) |
						(buffer[offset + 2] << 16));
			}

	// Read an RGB value from a buffer.
	public static int ReadRGB(byte[] buffer, int offset)
			{
				return ((buffer[offset + 2]) |
						(buffer[offset + 1] << 8) |
						(buffer[offset] << 16));
			}

	// Write a little-endian 16-bit integer value to a buffer.
	public static void WriteUInt16(byte[] buffer, int offset, int value)
			{
				buffer[offset] = (byte)value;
				buffer[offset + 1] = (byte)(value >> 8);
			}

	// Write a little-endian 32-bit integer value to a buffer.
	public static void WriteInt32(byte[] buffer, int offset, int value)
			{
				buffer[offset] = (byte)value;
				buffer[offset + 1] = (byte)(value >> 8);
				buffer[offset + 2] = (byte)(value >> 16);
				buffer[offset + 3] = (byte)(value >> 24);
			}

	// Write a big-endian 16-bit integer value to a buffer.
	public static void WriteUInt16B(byte[] buffer, int offset, int value)
			{
				buffer[offset + 1] = (byte)value;
				buffer[offset] = (byte)(value >> 8);
			}

	// Write a big-endian 32-bit integer value to a buffer.
	public static void WriteInt32B(byte[] buffer, int offset, int value)
			{
				buffer[offset + 3] = (byte)value;
				buffer[offset + 2] = (byte)(value >> 8);
				buffer[offset + 1] = (byte)(value >> 16);
				buffer[offset] = (byte)(value >> 24);
			}

	// Write a BGR value to a buffer as an RGBQUAD.
	public static void WriteBGR(byte[] buffer, int offset, int value)
			{
				buffer[offset] = (byte)value;
				buffer[offset + 1] = (byte)(value >> 8);
				buffer[offset + 2] = (byte)(value >> 16);
				buffer[offset + 3] = (byte)0;
			}

	// Write an RGB value to a buffer as a triple.
	public static void WriteRGB(byte[] buffer, int offset, int value)
			{
				buffer[offset + 2] = (byte)value;
				buffer[offset + 1] = (byte)(value >> 8);
				buffer[offset] = (byte)(value >> 16);
			}

	// Perform a forward-seek on a stream, even on streams that
	// cannot support seeking properly.  "current" is the current
	// position in the stream, and "offset" is the desired offset.
	public static void Seek(Stream stream, long current, long offset)
			{
				if(offset < current)
				{
					throw new FormatException();
				}
				else if(offset == current)
				{
					return;
				}
				if(stream.CanSeek)
				{
					stream.Seek(offset, SeekOrigin.Begin);
				}
				else
				{
					byte[] buffer = new byte [1024];
					int len;
					while(current < offset)
					{
						if((offset - current) < 1024)
						{
							len = (int)(offset - current);
						}
						else
						{
							len = 1024;
						}
						if(stream.Read(buffer, 0, len) != len)
						{
							throw new FormatException();
						}
					}
				}
			}

	// Convert a pixel format into a stride value.
	public static int FormatToStride(PixelFormat pixelFormat, int width)
			{
				return ((BytesPerLine(pixelFormat, width) + 3) & ~3);
			}

	public static int BytesPerLine(PixelFormat pixelFormat, int width)
			{
				int lineBytes;
				switch(pixelFormat)
				{
					case PixelFormat.Format1bppIndexed:
						lineBytes = (width + 7) / 8;
						break;

					case PixelFormat.Format4bppIndexed:
						lineBytes = (width + 1) / 2;
						break;

					case PixelFormat.Format8bppIndexed:
						lineBytes = width;
						break;

					case PixelFormat.Format16bppRgb555:
					case PixelFormat.Format16bppRgb565:
					case PixelFormat.Format16bppArgb1555:
					case PixelFormat.Format16bppGrayScale:
						lineBytes = width * 2;
						break;

					case PixelFormat.Format24bppRgb:
						lineBytes = width * 3;
						break;

					case PixelFormat.Format32bppRgb:
					case PixelFormat.Format32bppPArgb:
					case PixelFormat.Format32bppArgb:
					default:
						lineBytes = width * 4;
						break;

					case PixelFormat.Format48bppRgb:
						lineBytes = width * 6;
						break;

					case PixelFormat.Format64bppPArgb:
					case PixelFormat.Format64bppArgb:
						lineBytes = width * 8;
						break;
				}
			return lineBytes;	
		}

	// Convert a pixel format into a bit count value.
	public static int FormatToBitCount(PixelFormat pixelFormat)
			{
				switch(pixelFormat)
				{
					case PixelFormat.Format1bppIndexed:
						return 1;
					case PixelFormat.Format4bppIndexed:
						return 4;
					case PixelFormat.Format8bppIndexed:
						return 8;

					case PixelFormat.Format16bppRgb555:
					case PixelFormat.Format16bppRgb565:
					case PixelFormat.Format16bppArgb1555:
					case PixelFormat.Format16bppGrayScale:
						return 16;

					case PixelFormat.Format24bppRgb:
						return 24;

					case PixelFormat.Format32bppRgb:
					case PixelFormat.Format32bppPArgb:
					case PixelFormat.Format32bppArgb:
						return 32;

					case PixelFormat.Format48bppRgb:
						return 48;

					case PixelFormat.Format64bppPArgb:
					case PixelFormat.Format64bppArgb:
						return 64;

					default:
						return 32;
				}
			}

	// Convert a bit count into a pixel format.
	public static PixelFormat BitCountToFormat(int bitCount)
			{
				switch(bitCount)
				{
					case 1:
						return PixelFormat.Format1bppIndexed;
					case 4:
						return PixelFormat.Format4bppIndexed;
					case 8:
						return PixelFormat.Format8bppIndexed;
					case 15:
						return PixelFormat.Format16bppRgb555;
					case 16:
						return PixelFormat.Format16bppRgb565;
					case 24:
						return PixelFormat.Format24bppRgb;
					case 32: default:
						return PixelFormat.Format32bppRgb;
					case 48:
						return PixelFormat.Format48bppRgb;
					case 64:
						return PixelFormat.Format64bppArgb;
				}
			}

	// Return the index to a closest matched color in the palette.
	public static int BestPaletteColor(int[] palette, int r, int g, int b)
			{
				int distance = 200000;
				int j = -1;
				for( int i = 0; i < palette.Length; i++)
				{
					int color = palette[i];
					int bDist = (byte)color - b;
					int gDist = (byte)(color >>8) - g;
					int rDist = (byte)(color>>16) - r;
					int d = bDist * bDist + gDist * gDist + rDist * rDist;
					if (d < distance)
					{
						distance = d;
						j = i;
						if (distance == 0)
							break;
					}
				}
				return j;
			}

	// Return the index to a closest matched color in the palette.
	public static int BestPaletteColor(int[] palette, int color)
			{
				return BestPaletteColor(palette, (byte)(color >> 16),
					(byte)(color >> 8), (byte)color);
			}
}; // class Utils

}; // namespace DotGNU.Images
