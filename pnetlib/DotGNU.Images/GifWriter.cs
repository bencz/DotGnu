/*
 * GifWriter.cs - Implementation of the "DotGNU.Images.GifWriter" class.
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

internal sealed class GifWriter
{
	// Save a GIF image to the specified stream.  This uses the "ungif"
	// approach of writing uncompressed code samples to avoid infringing
	// on the LZW patent.  The patent may have expired already, but we
	// aren't taking any chances.  Use PNG instead - it compresses better.
	public static void Save(Stream stream, Image image)
			{
				byte[] buffer = new byte [1024];
				Frame frame0, frame;
				int bitCount, index;
				int[] palette;

				// Get the first frame in the image and the image's bit depth.
				frame0 = image.GetFrame(0);
				bitCount = Utils.FormatToBitCount(frame0.PixelFormat);

				// Write the GIF file header.
				buffer[0] = (byte)'G';
				buffer[1] = (byte)'I';
				buffer[2] = (byte)'F';
				buffer[3] = (byte)'8';
				buffer[4] = (byte)'9';
				buffer[5] = (byte)'a';
				Utils.WriteUInt16(buffer, 6, image.Width);
				Utils.WriteUInt16(buffer, 8, image.Height);
				buffer[10] = (byte)((bitCount - 1) |
									((bitCount - 1) << 4) | 0x80);
				if(frame0.TransparentPixel != -1)
				{
					// Use the transparent pixel as the background color.
					buffer[11] = (byte)(frame0.TransparentPixel);
				}
				else
				{
					// Assume that index zero is the background color.
					buffer[11] = (byte)0x00;
				}
				buffer[12] = (byte)0x00;	// No aspect ratio specified.
				stream.Write(buffer, 0, 13);

				// Write the global color table, which is the palette
				// for the first frame.  We use local color tables only
				// if the palette changes for subsequent frames.
				palette = frame0.Palette;
				WriteGifPalette(stream, palette, bitCount, buffer);

				// Write out the frames.
				for(index = 0; index < image.NumFrames; ++index)
				{
					// Get the object for this frame.
					frame = image.GetFrame(index);

					// Write a graphics control extension for transparencies.
					if(frame.TransparentPixel != -1)
					{
						buffer[0] = (byte)0x21;
						buffer[1] = (byte)0xF9;
						buffer[2] = (byte)0x04;
						buffer[3] = (byte)0x01;
						buffer[4] = (byte)0x00;
						buffer[5] = (byte)0x00;
						buffer[6] = (byte)(frame.TransparentPixel);
						buffer[7] = (byte)0x00;
						stream.Write(buffer, 0, 8);
					}

					// Write the image descriptor header.
					buffer[0] = (byte)0x2C;
					Utils.WriteUInt16(buffer, 1, frame.OffsetX);
					Utils.WriteUInt16(buffer, 3, frame.OffsetY);
					Utils.WriteUInt16(buffer, 5, frame.Width);
					Utils.WriteUInt16(buffer, 7, frame.Height);
					if(frame.Palette != palette)
					{
						buffer[9] = (byte)((bitCount - 1) | 0x80);
					}
					else
					{
						buffer[9] = (byte)0x00;
					}
					stream.Write(buffer, 0, 10);

					// Write the local color table if necessary.
					if(frame.Palette != palette)
					{
						WriteGifPalette
							(stream, frame.Palette, bitCount, buffer);
					}

					// Compress and output the frame's contents.
					Compress(stream, buffer, bitCount, frame);
				}

				// Write the GIF file terminator.
				buffer[0] = (byte)0x3B;
				stream.Write(buffer, 0, 1);
			}

	// Determine if an image only has indexed frames in it and
	// that all frames have the same pixel format.
	public static bool IsGifEncodable(Image image)
			{
				int index;
				Frame frame;
				PixelFormat format = (PixelFormat)(-1);
				if(image.NumFrames == 0)
				{
					return false;
				}
				for(index = 0; index < image.NumFrames; ++index)
				{
					frame = image.GetFrame(index);
					if((frame.PixelFormat & PixelFormat.Indexed) == 0)
					{
						return false;
					}
					if(format != (PixelFormat)(-1) &&
					   format != frame.PixelFormat)
					{
						return false;
					}
					format = frame.PixelFormat;
				}
				return true;
			}

	// Write a GIF palette to an output stream.
	private static void WriteGifPalette
				(Stream stream, int[] palette, int bitCount, byte[] buffer)
			{
				int numColors = (1 << bitCount);
				int index, value;
				Array.Clear(buffer, 0, numColors * 3);
				for(index = 0; palette != null && index < numColors &&
							   index < palette.Length; ++index)
				{
					value = palette[index];
					buffer[index * 3]     = (byte)(value >> 16);
					buffer[index * 3 + 1] = (byte)(value >> 8);
					buffer[index * 3 + 2] = (byte)value;
				}
				stream.Write(buffer, 0, numColors * 3);
			}

	// Compress GIF data.  Actually, we just encode it using an "ungif" method.
	private static void Compress
				(Stream stream, byte[] buffer, int bitCount, Frame frame)
			{
				int codeSize, offset, clearCode, codeMax;
				int x, y, width, height, output, minCodeSize;
				int accum, accumSize, endCode, nextCode;
				int pixel = 0;
				PixelFormat format;
				byte[] data;

				// Output the minimum code size.
				if(bitCount < 2)
				{
					minCodeSize = 2;
				}
				else
				{
					minCodeSize = bitCount;
				}
				buffer[0] = (byte)minCodeSize;
				stream.Write(buffer, 0, 1);

				// Determine the special codes and the actual code size.
				clearCode = (1 << minCodeSize);
				endCode = clearCode + 1;
				nextCode = endCode + 1;
				codeSize = minCodeSize + 1;
				codeMax = (1 << codeSize);

				// Scan the image and "compress" it.
				width = frame.Width;
				height = frame.Height;
				data = frame.Data;
				format = frame.PixelFormat;
				accum = 0;
				accumSize = 0;
				output = 0;
				for(y = 0; y < height; ++y)
				{
					offset = y * frame.Stride;
					for(x = 0; x < width; ++x)
					{
						if(format == PixelFormat.Format8bppIndexed)
						{
							pixel = data[offset++];
						}
						else if(format == PixelFormat.Format4bppIndexed)
						{
							if((x & 1) == 0)
							{
								pixel = (data[offset] >> 4) & 0x0F;
							}
							else
							{
								pixel = (data[offset++] & 0x0F);
							}
						}
						else if(format == PixelFormat.Format1bppIndexed)
						{
							if((data[offset] & (0x80 >> (x & 7))) != 0)
							{
								pixel = 1;
							}
							else
							{
								pixel = 0;
							}
							if((x & 7) == 7)
							{
								++offset;
							}
						}
						accum |= (pixel << accumSize);
						accumSize += codeSize;
						++nextCode;
						if(nextCode >= codeMax)
						{
							++codeSize;
							if(codeSize > 12)
							{
								// Reset the virtual code table.
								accum |= (clearCode << accumSize);
								accumSize += codeSize;
								codeSize = minCodeSize + 1;
								codeMax = (1 << codeSize);
							}
							else
							{
								// Increase the virtual code table size.
								++codeSize;
								codeMax = (1 << codeSize);
							}
						}
						while(accumSize >= 8)
						{
							// Output bytes from the accumulator to the stream.
							++output;
							buffer[output] = (byte)accum;
							accum >>= 8;
							accumSize -= 8;
							if(output >= 255)
							{
								buffer[0] = (byte)255;
								stream.Write(buffer, 0, 256);
								output = 0;
							}
						}
					}
				}

				// Output the end code.
				accum |= (endCode << codeSize);
				accum += codeSize;
				while(accumSize > 0)
				{
					// Output bytes from the accumulator to the stream.
					++output;
					buffer[output] = (byte)accum;
					accum >>= 8;
					accumSize -= 8;
					if(output >= 255)
					{
						buffer[0] = (byte)255;
						stream.Write(buffer, 0, 256);
						output = 0;
					}
				}
				if(output > 0)
				{
					// Output the short block at the end.
					buffer[0] = (byte)output;
					stream.Write(buffer, 0, output + 1);
				}

				// Output a zero-length block to terminate the data.
				buffer[0] = (byte)0x00;
				stream.Write(buffer, 0, 1);
			}

}; // class GifWriter

}; // namespace DotGNU.Images
