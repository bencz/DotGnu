/*
 * BmpWriter.cs - Implementation of the "DotGNU.Images.BmpWriter" class.
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

internal sealed class BmpWriter
{
	// Save a BMP image to the specified stream.
	public static void Save(Stream stream, Image image)
			{
				byte[] buffer = new byte [1024];
				int bitCount;
				int offset;
				int size;

				// We can only save the first frame in BMP formats.
				Frame frame = image.GetFrame(0);
				if(frame == null)
				{
					return;
				}

				// Determine the size of the bitmap and the offset to it.
				bitCount = Utils.FormatToBitCount(frame.pixelFormat);
				size = frame.Stride * frame.Height;
				offset = 14 + 40;
				if(bitCount <= 8)
				{
					offset += (1 << bitCount) * 4;
				}

				// Build and write the BITMAPFILEHEADER structure.
				buffer[0] = (byte)'B';
				buffer[1] = (byte)'M';
				Utils.WriteInt32(buffer, 2, offset + size);
				buffer[6] = (byte)0;
				buffer[7] = (byte)0;
				buffer[8] = (byte)0;
				buffer[9] = (byte)0;
				Utils.WriteInt32(buffer, 10, offset);
				stream.Write(buffer, 0, 14);

				// Build and write the BITMAPINFO details.
				SaveBitmapInfo(stream, frame, bitCount, size, buffer,
							   frame.Height);

				// Write the bitmap data in the frame to the stream.
				SaveBitmapData(stream, frame, false, false);
			}

	// Save a BITMAPINFO structure for a frame.
	public static void SaveBitmapInfo
				(Stream stream, Frame frame, int bitCount,
				 int size, byte[] buffer, int height)
			{
				// Build and write the BITMAPINFOHEADER structure.
				Utils.WriteInt32(buffer, 0, 40);			// biSize
				Utils.WriteInt32(buffer, 4, frame.Width);
				Utils.WriteInt32(buffer, 8, height);
				Utils.WriteUInt16(buffer, 12, 1);			// biPlanes
				Utils.WriteUInt16(buffer, 14, bitCount);
				Utils.WriteInt32(buffer, 16, 0);			// biCompression
				Utils.WriteInt32(buffer, 20, size);
				Utils.WriteInt32(buffer, 24, 3780);			// biXPelsPerMeter
				Utils.WriteInt32(buffer, 28, 3780);			// biYPelsPerMeter
				Utils.WriteInt32(buffer, 32, 0);			// biClrUsed
				Utils.WriteInt32(buffer, 36, 0);			// biClrImportant
				stream.Write(buffer, 0, 40);

				// Write the palette.
				if(bitCount <= 8)
				{
					int[] palette = frame.Palette;
					int count = (1 << bitCount);
					int index;
					for(index = 0; index < count; ++index)
					{
						if(palette != null && index < palette.Length)
						{
							Utils.WriteBGR(buffer, index * 4, palette[index]);
						}
						else
						{
							// Short palette: pad with black pixels.
							Utils.WriteBGR(buffer, index * 4, 0);
						}
					}
					stream.Write(buffer, 0, count * 4);
				}
			}

	// Save the bitmap data in a frame.
	public static void SaveBitmapData(Stream stream, Frame frame,
									  bool mask, bool inverted)
			{
				byte[] data;
				int stride;
				int line, column;

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

				// BMP images are stored upside down in the stream.
				if(!inverted)
				{
					for(line = frame.Height - 1; line >= 0; --line)
					{
						stream.Write(data, line * stride, stride);
					}
				}
				else
				{
					for(line = frame.Height - 1; line >= 0; --line)
					{
						for(column = 0; column < stride; ++column)
						{
							stream.WriteByte
								((byte)(data[line * stride + column] ^ 0xFF));
						}
					}
				}
			}

}; // class BmpWriter

}; // namespace DotGNU.Images
