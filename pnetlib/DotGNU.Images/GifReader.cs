/*
 * GifReader.cs - Implementation of the "DotGNU.Images.GifReader" class.
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

internal sealed class GifReader
{
	// Load a GIF image from the specified stream.  The first 4 bytes
	// have already been read and discarded.  We always load GIF's
	// as 8bpp because that makes it easier to handle decompression.
	// GIF's with lower bit depths will be expanded appropriately.
	public static void Load(Stream stream, Image image)
			{
				byte[] buffer = new byte [1024];
				int logicalWidth, logicalHeight;
				int flags, bitCount, numColors, tag;
				int[] palette;
				int transparentPixel;
				int imageWidth, imageHeight;
				Frame frame;

				// Read the rest of the GIF header and validate it.
				if(stream.Read(buffer, 0, 9) != 9)
				{
					throw new FormatException();
				}
				if((buffer[0] != (byte)'7' && buffer[0] != (byte)'9') ||
				   buffer[1] != (byte)'a')
				{
					throw new FormatException();
				}
				logicalWidth = Utils.ReadUInt16(buffer, 2);
				logicalHeight = Utils.ReadUInt16(buffer, 4);
				flags = buffer[6];
				// buffer[7] is the background index, which we ignore.
				// buffer[8] is the aspect ratio, which we ignore.
				if(logicalWidth == 0 || logicalHeight == 0)
				{
					throw new FormatException();
				}

				// Set the global image information.
				bitCount = (flags & 0x07) + 1;
				numColors = (1 << bitCount);
				image.Width = logicalWidth;
				image.Height = logicalHeight;
				image.PixelFormat = PixelFormat.Format8bppIndexed;
				image.LoadFormat = Image.Gif;

				// Read the global color table, if present.
				if((flags & 0x80) != 0)
				{
					image.Palette = ReadGifPalette(stream, buffer, numColors);
				}

				// Process the image and extension blocks in the image.
				transparentPixel = -1;
				while(stream.Read(buffer, 0, 1) == 1)
				{
					tag = buffer[0];
					if(tag == 0x2C)
					{
						// Read the image descriptor.
						if(stream.Read(buffer, 0, 9) != 9)
						{
							throw new FormatException();
						}
						imageWidth = Utils.ReadUInt16(buffer, 4);
						imageHeight = Utils.ReadUInt16(buffer, 6);
						flags = buffer[8];
						if(imageWidth == 0 || imageHeight == 0)
						{
							throw new FormatException();
						}
						frame = image.AddFrame(imageWidth, imageHeight,
											   image.PixelFormat);
						frame.TransparentPixel = transparentPixel;
						frame.OffsetX = Utils.ReadUInt16(buffer, 0);
						frame.OffsetY = Utils.ReadUInt16(buffer, 2);
						transparentPixel = -1;

						// Read the local color table, if any.
						if((flags & 0x80) != 0)
						{
							tag = (1 << ((flags & 0x07) + 1));
							frame.Palette = ReadGifPalette
								(stream, buffer, tag);
						}

						// Decompress the image into the frame.
						Decompress(stream, buffer, frame, (flags & 0x40) != 0);
					}
					else if(tag == 0x21)
					{
						// Process an extension.
						if(stream.Read(buffer, 0, 1) != 1)
						{
							throw new FormatException();
						}
						if(buffer[0] == (byte)0xF9)
						{
							// Graphic control extension sub-block.
							if(stream.Read(buffer, 0, 1) != 1)
							{
								throw new FormatException();
							}
							tag = buffer[0];
							if(stream.Read(buffer, 0, tag) != tag)
							{
								throw new FormatException();
							}
							if(tag >= 4)
							{
								if((buffer[0] & 0x01) != 0)
								{
									transparentPixel = buffer[3];
								}
								else
								{
									transparentPixel = -1;
								}
							}
						}

						// Skip the remaining extension sub-blocks.
						SkipSubBlocks(stream, buffer);
					}
					else if(tag == 0x3B)
					{
						// End of the GIF file.
						break;
					}
					else
					{
						// Invalid GIF file.
						throw new FormatException();
					}
				}
			}

	// Read a palette from a GIF file.
	private static int[] ReadGifPalette(Stream stream, byte[] buffer, int num)
			{
				int index;
				int[] palette = new int [256];
				if(stream.Read(buffer, 0, num * 3) != num * 3)
				{
					throw new FormatException();
				}
				for(index = 0; index < num && index < 256; ++index)
				{
					palette[index] =
						(buffer[index * 3] << 16) |
						(buffer[index * 3 + 1] << 8) |
						(buffer[index * 3 + 2]);
				}
				return palette;
			}

	// Skip data sub blocks at the current position in a GIF file.
	private static void SkipSubBlocks(Stream stream, byte[] buffer)
			{
				int size;
				while(stream.Read(buffer, 0, 1) == 1)
				{
					size = buffer[0];
					if(size == 0)
					{
						// End of the sub-block list.
						return;
					}
					if(stream.Read(buffer, 0, size) != size)
					{
						throw new FormatException();
					}
				}
				throw new FormatException();
			}

	// Helper class for reading codes from an LZW stream.
	private class GifCodeHelper
	{
		// Internal state.
		private Stream stream;
		private byte[] buffer;
		private int origCodeSize;
		internal int codeSize;
		private int codeMask;
		internal int codeMax;
		private int posn, len;
		private int last, numBits;

		// Constructor.
		public GifCodeHelper(Stream stream, byte[] buffer, int codeSize)
				{
					this.stream = stream;
					this.buffer = buffer;
					this.origCodeSize = codeSize;
					this.codeSize = codeSize;
					this.codeMask = (1 << codeSize) - 1;
					this.codeMax = (1 << codeSize);
					this.posn = 0;
					this.len = 0;
					this.last = 0;
					this.numBits = 0;
				}

		// Get the next code from the input stream.
		public int GetCode()
				{
					// Read sufficient bits to make up a code value.
					while(numBits < codeSize)
					{
						if(posn < len)
						{
							last |= (buffer[posn++] << numBits);
							numBits += 8;
						}
						else
						{
							if(stream.Read(buffer, 0, 1) != 1)
							{
								throw new FormatException();
							}
							len = buffer[0];
							if(len == 0)
							{
								// This is the end of the code stream.
								return -1;
							}
							posn = 0;
							if(stream.Read(buffer, 0, len) != len)
							{
								throw new FormatException();
							}
						}
					}

					// Extract the code and return it.
					int code = last & codeMask;
					last >>= codeSize;
					numBits -= codeSize;
					return code;
				}

		// Increase the code size.
		public void IncreaseCodeSize()
				{
					++codeSize;
					codeMask = (1 << codeSize) - 1;
					codeMax = (1 << codeSize);
				}

		// Reset the code size to the original.
		public void ResetCodeSize()
				{
					codeSize = origCodeSize;
					codeMask = (1 << codeSize) - 1;
					codeMax = (1 << codeSize);
				}

	}; // class GifCodeHelper

	// Decompress the bytes corresponding to a GIF image.
	private static void Decompress
				(Stream stream, byte[] buffer, Frame frame, bool interlaced)
			{
				int minCodeSize, code;
				int clearCode, endCode;
				int nextCode, oldCode;
				GifCodeHelper helper;
				ushort[] prefix = new ushort [4096];
				byte[] suffix = new byte [4096];
				ushort[] length = new ushort [4096];
				int offset, x, y, width, height;
				int stride, lineChange, temp;
				byte[] data;
				byte[] stringbuf = new byte [4096];
				int stringLen, tempCode, pass;
				byte lastSuffix;

				// Read the minimum code size and validate it.
				if(stream.Read(buffer, 0, 1) != 1)
				{
					throw new FormatException();
				}
				minCodeSize = buffer[0];
				if(minCodeSize < 2)
				{
					minCodeSize = 2;
				}
				else if(minCodeSize > 11)
				{
					minCodeSize = 11;
				}

				// Create a code helper.
				helper = new GifCodeHelper(stream, buffer, minCodeSize + 1);

				// Set the clear and end codes.
				clearCode = (1 << minCodeSize);
				endCode = clearCode + 1;
				nextCode = endCode;

				// Initialize the table.
				for(code = 0; code < clearCode; ++code)
				{
					prefix[code] = (ushort)49428;
					suffix[code] = (byte)code;
					length[code] = (ushort)1;
				}

				// Initialize the image output parameters.
				x = 0;
				y = 0;
				width = frame.Width;
				height = frame.Height;
				data = frame.Data;
				pass = 0;
				lineChange = (interlaced ? 8 : 1);
				stride = frame.Stride;

				// Process the codes in the input stream.
				code = clearCode;
				for(;;)
				{
					oldCode = code;
					if((code = helper.GetCode()) == -1)
					{
						// We've run out of data blocks.
						break;
					}
					else if(code == clearCode)
					{
						// Clear the code table and restart.
						helper.ResetCodeSize();
						nextCode = endCode;
						continue;
					}
					else if(code == endCode)
					{
						// End of the GIF input stream: skip remaining blocks.
						SkipSubBlocks(stream, buffer);
						break;
					}
					else
					{
						// Sanity check for out of range codes.
						if(code > nextCode && nextCode != 0)
						{
							code = 0;
						}

						// Update the next code in the table.
						prefix[nextCode] = (ushort)oldCode;
						length[nextCode] = (ushort)(length[oldCode] + 1);

						// Form the full string for this code.
						stringLen = length[code];
						tempCode = code;
						do
						{
							lastSuffix = suffix[tempCode];
							--stringLen;
							stringbuf[stringLen] = lastSuffix;
							tempCode = prefix[tempCode];
						}
						while(stringLen > 0);
						suffix[nextCode] = lastSuffix;
						stringLen = length[code];
						if(code == nextCode)
						{
							stringbuf[stringLen - 1] = lastSuffix;
						}

						// Copy the string into the actual image.
						offset = 0;
						while(stringLen > 0)
						{
							temp = width - x;
							if(temp > stringLen)
							{
								temp = stringLen;
							}
							Array.Copy(stringbuf, offset, data,
									   y * stride + x, temp);
							x += temp;
							offset += temp;
							stringLen -= temp;
							if(x >= width)
							{
								x = 0;
								y += lineChange;
								while(y >= height && interlaced)
								{
									// Move on to the next interlace pass.
									++pass;
									if(pass == 1)
									{
										y = 4;
										lineChange = 8;
									}
									else if(pass == 2)
									{
										y = 2;
										lineChange = 4;
									}
									else if(pass == 3)
									{
										y = 1;
										lineChange = 2;
									}
									else
									{
										break;
									}
								}
								if(y >= height)
								{
									// Shouldn't happen - just in case.
									y = 0;
								}
							}
						}

						// Set the suffix for the next code.
						suffix[nextCode] = (byte)lastSuffix;

						// Move on to the next code.
						if(nextCode != clearCode)
						{
							++nextCode;
							if(nextCode == helper.codeMax)
							{
								helper.IncreaseCodeSize();
								if(helper.codeSize > 12)
								{
									helper.ResetCodeSize();
									nextCode = clearCode;
								}
							}
						}
					}
				}
			}

}; // class GifReader

}; // namespace DotGNU.Images
