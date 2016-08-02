/*
 * PngReader.cs - Implementation of the "DotGNU.Images.PngReader" class.
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
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip.Compression;

internal sealed class PngReader
{
	// Integer versions of the PNG chunk types.
	public const int IHDR = 0x49484452;
	public const int PLTE = 0x504C5445;
	public const int IDAT = 0x49444154;
	public const int IEND = 0x49454E44;
	public const int tRNS = 0x74524E53;
	public const int sBIT = 0x73424954;

	// Rule table for Adam7 interlacing.  Each line contains
	// the (x, y) offset of the top-left pixel in the interlace,
	// and the (multx, multy) factors for laying out the pixels.
	private static readonly int[] adam7Rules = {
				0, 0, 8, 8,
				4, 0, 8, 8,
				0, 4, 4, 8,
				2, 0, 4, 4,
				0, 2, 2, 4,
				1, 0, 2, 2,
				0, 1, 1, 2,
				0, 0, 1, 1
			};

	// Load a PNG image from the specified stream.  The first 4 bytes
	// have already been read and discarded.
	public static void Load(Stream stream, Image image)
			{
				byte[] buffer = new byte [1024];
				int width = 0;
				int height = 0;
				int bitDepth = 0;
				int colorType = 0;
				int compressionMethod = 0;
				int filterMethod = 0;
				int interlaceMethod = 0;
				Frame frame = null;
				PixelFormat format = 0;
				int index;
				int significant = 0;
				ZlibDecompressor decompress = null;
				ScanlineReader scanlineReader;
				int pass, passWidth, passHeight;
				PassFunc passFunc;

				// Read the rest of the magic number and check it.
				if(stream.Read(buffer, 0, 4) != 4)
				{
					throw new FormatException("could not read magic number");
				}
				if(buffer[0] != (byte)13 ||
				   buffer[1] != (byte)10 ||
				   buffer[2] != (byte)26 ||
				   buffer[3] != (byte)10)
				{
					throw new FormatException("invalid magic number");
				}

				// Create a chunk reader for the stream.
				ChunkReader reader = new ChunkReader(stream, buffer);

				// Read all of the chunks from the stream.
				while(reader.Type != IEND)
				{
					// Process the critical chunk types.
					if(reader.Type == IHDR)
					{
						// We can only have one header per PNG image.
						if(image.NumFrames > 0)
						{
							throw new FormatException("multiple headers");
						}

						// Read the contents of the image header.
						if(reader.Read(buffer, 0, 13) != 13)
						{
							throw new FormatException("truncated header");
						}
						width = Utils.ReadInt32B(buffer, 0);
						height = Utils.ReadInt32B(buffer, 4);
						bitDepth = buffer[8];
						colorType = buffer[9];
						compressionMethod = buffer[10];
						filterMethod = buffer[11];
						interlaceMethod = buffer[12];

						// Sanity-check the values.
						if(width < 1 || height < 1)
						{
							throw new FormatException("invalid size");
						}
						if(colorType == 0)
						{
							if(bitDepth != 1 && bitDepth != 2 &&
							   bitDepth != 4 && bitDepth != 8 &&
							   bitDepth != 16)
							{
								throw new FormatException
									("invalid depth for color type 0");
							}
						}
						else if(colorType == 2 || colorType == 4 ||
								colorType == 6)
						{
							if(bitDepth != 8 && bitDepth != 16)
							{
								throw new FormatException
									("invalid depth for color type " +
									 colorType.ToString());
							}
						}
						else if(colorType == 3)
						{
							if(bitDepth != 1 && bitDepth != 2 &&
							   bitDepth != 4 && bitDepth != 8)
							{
								throw new FormatException
									("invalid depth for color type 3");
							}
						}
						else
						{
							throw new FormatException("invalid color type");
						}
						if(compressionMethod != 0)
						{
							throw new FormatException
								("invalid compression method");
						}
						if(filterMethod != 0)
						{
							throw new FormatException
								("invalid filter method");
						}
						if(interlaceMethod != 0 && interlaceMethod != 1)
						{
							throw new FormatException
								("invalid interlace method");
						}

						// Create the image frame with the requested values.
						if(colorType == 3)
						{
							format = PixelFormat.Format8bppIndexed;
						}
						else if((colorType & 4) != 0)
						{
							if(significant == 0x01050505 && bitDepth == 8)
							{
								format = PixelFormat.Format16bppArgb1555;
							}
							else if(bitDepth == 8)
							{
								format = PixelFormat.Format32bppArgb;
							}
							else
							{
								format = PixelFormat.Format64bppArgb;
							}
						}
						else if(colorType == 0 && bitDepth == 16)
						{
							format = PixelFormat.Format16bppGrayScale;
						}
						else
						{
							if(significant == 0x00050505 && bitDepth == 8)
							{
								format = PixelFormat.Format16bppRgb555;
							}
							else if(significant == 0x00050605 && bitDepth == 8)
							{
								format = PixelFormat.Format16bppRgb565;
							}
							else if(bitDepth == 8)
							{
								format = PixelFormat.Format24bppRgb;
							}
							else
							{
								format = PixelFormat.Format48bppRgb;
							}
						}
						image.Width = width;
						image.Height = height;
						image.PixelFormat = format;
						image.LoadFormat = Image.Png;
						frame = image.AddFrame(width, height, format);
					}
					else if(reader.Type == PLTE)
					{
						// We must have a frame at this point.
						if(frame == null)
						{
							throw new FormatException
								("palette specified before image header");
						}

						// The palette is only required for color type 3.
						// Other color types use it as a hint only.
						if(colorType == 3)
						{
							int[] palette = new int [256];
							frame.Palette = palette;
							Array.Clear(buffer, 0, buffer.Length);
							if(reader.Length > 768)
							{
								reader.Read(buffer, 0, 768);
							}
							else
							{
								reader.Read(buffer, 0, buffer.Length);
							}
							for(index = 0; index < 256; ++index)
							{
								palette[index] =
									Utils.ReadRGB(buffer, index * 3);
							}
						}
					}
					else if(reader.Type == tRNS)
					{
						// We must have a frame at this point.
						if(frame == null)
						{
							throw new FormatException
								("transparency specified before image header");
						}

						// We only support simple transparencies for
						// color type 3 at present.  The transparency
						// information is ignored for other color types.
						if(colorType == 3)
						{
							index = 0;
							while(index < 256 && reader.Length > 0)
							{
								if(reader.Read(buffer, 0, 1) != 1)
								{
									break;
								}
								if(buffer[0] < 0x80)
								{
									frame.TransparentPixel = index;
									break;
								}
								++index;
							}
						}
					}
					else if(reader.Type == sBIT)
					{
						// Read the number of significant bits so that
						// we can detect images that started off life
						// as 15-bit or 16-bit RGB.
						if(reader.Length == 3)
						{
							reader.Read(buffer, 0, 3);
							significant = Utils.ReadRGB(buffer, 0);
						}
						else if(reader.Length == 4)
						{
							reader.Read(buffer, 0, 4);
							significant = Utils.ReadRGB(buffer, 0) |
										  (buffer[3] << 24);
						}
					}
					else if(reader.Type == IDAT)
					{
						// We must have a frame at this point.
						if(frame == null)
						{
							throw new FormatException
								("image data specified before image header");
						}

						// There can be only one set of data chunks.
						if(decompress != null)
						{
							throw new FormatException
								("multiple image data blocks encountered");
						}

						// Create a zlib decompressor.
						decompress = new ZlibDecompressor(reader);

						// Get the pass processing function.
						passFunc = GetPassFunc(colorType, bitDepth, format);

						// Process the data in the image.
						if(interlaceMethod == 0)
						{
							// No interlacing.
							scanlineReader = new ScanlineReader
								(decompress, width, height,
								colorType, bitDepth);
							passFunc(frame, scanlineReader, width, height,
								     0, 0, 1, 1);
						}
						else
						{
							// Use Adam7 interlacing.
							for(pass = 0; pass < 7; ++pass)
							{
								// Calculate the width and height of the pass.
								// Please refer "PNG - The Definitive Guide"
								// for a totally misleading and incompatible
								// description of the following code - Gopal

								passWidth = width + 
											adam7Rules[(pass+1) * 4 + 2] - 1;
								passWidth /= adam7Rules[pass * 4 + 2];
								if(passWidth <= 0)
								{
									continue;
								}
								
								passHeight = height + 
											adam7Rules[(pass+1) * 4 + 3 ] - 1;
								passHeight /= adam7Rules[pass * 4 + 3];
								if(passHeight <= 0)
								{
									continue;
								}

								// Create a scanline reader for the pass.
								scanlineReader = new ScanlineReader
									(decompress, passWidth, passHeight,
									 colorType, bitDepth);

								// Process the Adam7 pass.
								passFunc(frame, scanlineReader,
										 passWidth, passHeight,
										 adam7Rules[pass * 4],
										 adam7Rules[pass * 4 + 1],
										 adam7Rules[pass * 4 + 2],
										 adam7Rules[pass * 4 + 3]);
							}
						}

						// Eat any remaining IDAT data blocks.
						decompress.EatRemaining();

						// Skip the "Reset", because we've already done it.
						continue;
					}

					// Reset the chunk reader and move on to the next chunk.
					reader.Reset(buffer);
				}

				// Skip the contents of the IEND chunk and check its CRC.
				reader.Skip(buffer);

				// If we don't have a frame or decompressor,
				// then the PNG stream was empty.
				if(frame == null || decompress == null)
				{
					throw new FormatException("PNG did not contain an image");
				}
			}

	// Compute the "Paeth" predictor function.  Algorithm from RFC-2083.
	public static byte Paeth(int a, int b, int c)
			{
				int p = a + b - c;
				int pa = p - a;
				if(pa < 0)
				{
					pa = -pa;
				}
				int pb = p - b;
				if(pb < 0)
				{
					pb = -pb;
				}
				int pc = p - c;
				if(pc < 0)
				{
					pc = -pc;
				}
				if(pa <= pb && pa <= pc)
				{
					return (byte)a;
				}
				else if(pb <= pc)
				{
					return (byte)b;
				}
				else
				{
					return (byte)c;
				}
			}

	// Delegate type for a pass processing function.
	private delegate void PassFunc(Frame frame, ScanlineReader reader,
								   int width, int height,
								   int offsetX, int offsetY,
								   int multX, int multY);

	// Get the pass processing function for a particular image type.
	private static PassFunc GetPassFunc
				(int colorType, int bitDepth, PixelFormat format)
			{
				PassFunc func = null;
				switch(colorType)
				{
					case 0:
					{
						switch(bitDepth)
						{
							case 1:
							{
								func = new PassFunc(GrayScale1bpp);
							}
							break;

							case 2:
							{
								func = new PassFunc(GrayScale2bpp);
							}
							break;

							case 4:
							{
								func = new PassFunc(GrayScale4bpp);
							}
							break;

							case 8:
							{
								func = new PassFunc(GrayScale8bpp);
							}
							break;

							case 16:
							{
								func = new PassFunc(GrayScale16bpp);
							}
							break;
						}
					}
					break;

					case 2:
					{
						if(format == PixelFormat.Format16bppRgb555)
						{
							func = new PassFunc(Rgb555);
						}
						else if(format == PixelFormat.Format16bppRgb565)
						{
							func = new PassFunc(Rgb565);
						}
						else if(bitDepth == 8)
						{
							func = new PassFunc(Rgb8bpp);
						}
						else
						{
							func = new PassFunc(Rgb16bpp);
						}
					}
					break;

					case 3:
					{
						func = new PassFunc(Indexed8bpp);
					}
					break;

					case 4:
					{
						if(bitDepth == 8)
						{
							func = new PassFunc(GrayScaleAlpha8bpp);
						}
						else
						{
							func = new PassFunc(GrayScaleAlpha16bpp);
						}
					}
					break;

					case 6:
					{
						if(format == PixelFormat.Format16bppArgb1555)
						{
							func = new PassFunc(RgbAlpha555);
						}
						else if(bitDepth == 8)
						{
							func = new PassFunc(RgbAlpha8bpp);
						}
						else
						{
							func = new PassFunc(RgbAlpha16bpp);
						}
					}
					break;
				}
				return func;
			}

	// Process a 1-bit gray scale image.
	private static void GrayScale1bpp(Frame frame, ScanlineReader reader,
								      int width, int height,
								      int offsetX, int offsetY,
								      int multX, int multY)
			{
				int x, y, offset, posn, bit;
				byte value;
				byte[] scanline;
				byte[] data = frame.Data;
				for(y = 0; y < height; ++y)
				{
					offset = ((y * multY) + offsetY) * frame.Stride +
							 offsetX * 3;
					posn = 0;
					bit = 0x80;
					scanline = reader.Read();
					for(x = 0; x < width; ++x)
					{
						if((scanline[posn] & bit) != 0)
						{
							value = 0xFF;
						}
						else
						{
							value = 0x00;
						}
						data[offset]     = value;
						data[offset + 1] = value;
						data[offset + 2] = value;
						offset += multX * 3;
						bit >>= 1;
						if(bit == 0)
						{
							++posn;
							bit = 0x80;
						}
					}
				}
			}

	// Process a 2-bit gray scale image.
	private static void GrayScale2bpp(Frame frame, ScanlineReader reader,
								      int width, int height,
								      int offsetX, int offsetY,
								      int multX, int multY)
			{
				int x, y, offset, posn, bit;
				byte value;
				byte[] scanline;
				byte[] data = frame.Data;
				for(y = 0; y < height; ++y)
				{
					offset = ((y * multY) + offsetY) * frame.Stride +
							 offsetX * 3;
					posn = 0;
					bit = 6;
					scanline = reader.Read();
					for(x = 0; x < width; ++x)
					{
						value = (byte)(((scanline[posn] >> bit) & 0x03) * 0x55);
						data[offset]     = value;
						data[offset + 1] = value;
						data[offset + 2] = value;
						offset += multX * 3;
						bit -= 2;
						if(bit < 0)
						{
							++posn;
							bit = 6;
						}
					}
				}
			}

	// Process a 4-bit gray scale image.
	private static void GrayScale4bpp(Frame frame, ScanlineReader reader,
								      int width, int height,
								      int offsetX, int offsetY,
								      int multX, int multY)
			{
				int x, y, offset, posn, bit;
				byte value;
				byte[] scanline;
				byte[] data = frame.Data;
				for(y = 0; y < height; ++y)
				{
					offset = ((y * multY) + offsetY) * frame.Stride +
							 offsetX * 3;
					posn = 0;
					bit = 4;
					scanline = reader.Read();
					for(x = 0; x < width; ++x)
					{
						value = (byte)(((scanline[posn] >> bit) & 0x0F) * 0x11);
						data[offset]     = value;
						data[offset + 1] = value;
						data[offset + 2] = value;
						offset += multX * 3;
						bit -= 4;
						if(bit < 0)
						{
							++posn;
							bit = 4;
						}
					}
				}
			}

	// Process a 8-bit gray scale image.
	private static void GrayScale8bpp(Frame frame, ScanlineReader reader,
								      int width, int height,
								      int offsetX, int offsetY,
								      int multX, int multY)
			{
				int x, y, offset, posn;
				byte value;
				byte[] scanline;
				byte[] data = frame.Data;
				for(y = 0; y < height; ++y)
				{
					offset = ((y * multY) + offsetY) * frame.Stride +
							 offsetX * 3;
					posn = 0;
					scanline = reader.Read();
					for(x = 0; x < width; ++x)
					{
						value = scanline[posn];
						data[offset]     = value;
						data[offset + 1] = value;
						data[offset + 2] = value;
						offset += multX * 3;
						++posn;
					}
				}
			}

	// Process a 16-bit gray scale image.
	private static void GrayScale16bpp(Frame frame, ScanlineReader reader,
								       int width, int height,
								       int offsetX, int offsetY,
								       int multX, int multY)
			{
				int x, y, offset, posn;
				byte value;
				byte[] scanline;
				byte[] data = frame.Data;
				for(y = 0; y < height; ++y)
				{
					offset = ((y * multY) + offsetY) * frame.Stride +
							 offsetX * 3;
					posn = 0;
					scanline = reader.Read();
					for(x = 0; x < width; ++x)
					{
						value = scanline[posn];
						data[offset]     = value;
						data[offset + 1] = value;
						data[offset + 2] = value;
						offset += multX * 3;
						posn += 2;
					}
				}
			}

	// Process a 8-bit gray scale image with alpha.
	private static void GrayScaleAlpha8bpp(Frame frame, ScanlineReader reader,
								      	   int width, int height,
								      	   int offsetX, int offsetY,
								      	   int multX, int multY)
			{
				int x, y, offset, posn;
				byte value;
				byte[] scanline;
				byte[] data = frame.Data;
				for(y = 0; y < height; ++y)
				{
					offset = ((y * multY) + offsetY) * frame.Stride +
							 offsetX * 4;
					posn = 0;
					scanline = reader.Read();
					for(x = 0; x < width; ++x)
					{
						value = scanline[posn];
						data[offset]     = value;
						data[offset + 1] = value;
						data[offset + 2] = value;
						data[offset + 3] = scanline[posn + 1];
						offset += multX * 4;
						posn += 2;
					}
				}
			}

	// Process a 16-bit gray scale image with alpha.
	private static void GrayScaleAlpha16bpp(Frame frame, ScanlineReader reader,
								       	    int width, int height,
								       	    int offsetX, int offsetY,
								       	    int multX, int multY)
			{
				int x, y, offset, posn;
				byte value;
				byte[] scanline;
				byte[] data = frame.Data;
				for(y = 0; y < height; ++y)
				{
					offset = ((y * multY) + offsetY) * frame.Stride +
							 offsetX * 4;
					posn = 0;
					scanline = reader.Read();
					for(x = 0; x < width; ++x)
					{
						value = scanline[posn];
						data[offset]     = value;
						data[offset + 1] = value;
						data[offset + 2] = value;
						data[offset + 3] = scanline[posn + 2];
						offset += multX * 4;
						posn += 4;
					}
				}
			}

	// Process a 8-bit indexed image.
	private static void Indexed8bpp(Frame frame, ScanlineReader reader,
								    int width, int height,
								    int offsetX, int offsetY,
								    int multX, int multY)
			{
				int x, y, offset, posn;
				byte[] scanline;
				byte[] data = frame.Data;
				for(y = 0; y < height; ++y)
				{
					offset = ((y * multY) + offsetY) * frame.Stride + offsetX;
					scanline = reader.Read();
					if(multX == 1)
					{
						Array.Copy(scanline, 0, data, offset, width);
					}
					else
					{
						posn = 0;
						for(x = 0; x < width; ++x)
						{
							data[offset] = scanline[posn];
							offset += multX;
							++posn;
						}
					}
				}
			}

	// Process a 8-bit RGB image.
	private static void Rgb8bpp(Frame frame, ScanlineReader reader,
								int width, int height,
								int offsetX, int offsetY,
								int multX, int multY)
			{
				int x, y, offset, posn;
				byte[] scanline;
				byte[] data = frame.Data;
				for(y = 0; y < height; ++y)
				{
					offset = ((y * multY) + offsetY) * frame.Stride +
							 offsetX * 3;
					posn = 0;
					scanline = reader.Read();
					for(x = 0; x < width; ++x)
					{
						// Flip the RGB PNG data into BGR form.
						data[offset]     = scanline[posn + 2];
						data[offset + 1] = scanline[posn + 1];
						data[offset + 2] = scanline[posn];
						offset += multX * 3;
						posn += 3;
					}
				}
			}

	// Process a 16-bit RGB image.
	private static void Rgb16bpp(Frame frame, ScanlineReader reader,
								 int width, int height,
								 int offsetX, int offsetY,
								 int multX, int multY)
			{
				int x, y, offset, posn;
				byte[] scanline;
				byte[] data = frame.Data;
				for(y = 0; y < height; ++y)
				{
					offset = ((y * multY) + offsetY) * frame.Stride +
							 offsetX * 6;
					posn = 0;
					scanline = reader.Read();
					for(x = 0; x < width; ++x)
					{
						// Flip the RGB PNG data into BGR form.
						data[offset]     = scanline[posn + 5];
						data[offset + 1] = scanline[posn + 4];
						data[offset + 2] = scanline[posn + 3];
						data[offset + 3] = scanline[posn + 2];
						data[offset + 4] = scanline[posn + 1];
						data[offset + 5] = scanline[posn];
						offset += multX * 6;
						posn += 6;
					}
				}
			}

	// Process a RGB image with 555 bit fields.
	private static void Rgb555(Frame frame, ScanlineReader reader,
							   int width, int height,
							   int offsetX, int offsetY,
							   int multX, int multY)
			{
				int x, y, offset, posn, value;
				byte[] scanline;
				byte[] data = frame.Data;
				for(y = 0; y < height; ++y)
				{
					offset = ((y * multY) + offsetY) * frame.Stride +
							 offsetX * 2;
					posn = 0;
					scanline = reader.Read();
					for(x = 0; x < width; ++x)
					{
						value = ((scanline[posn] & 0xF8) << 7) |
								((scanline[posn + 1] & 0xF8) << 2) |
								((scanline[posn + 2] & 0xF8) >> 3);
						data[offset]     = (byte)value;
						data[offset + 1] = (byte)(value >> 8);
						offset += multX * 2;
						posn += 3;
					}
				}
			}

	// Process a RGB image with 565 bit fields.
	private static void Rgb565(Frame frame, ScanlineReader reader,
							   int width, int height,
							   int offsetX, int offsetY,
							   int multX, int multY)
			{
				int x, y, offset, posn, value;
				byte[] scanline;
				byte[] data = frame.Data;
				for(y = 0; y < height; ++y)
				{
					offset = ((y * multY) + offsetY) * frame.Stride +
							 offsetX * 2;
					posn = 0;
					scanline = reader.Read();
					for(x = 0; x < width; ++x)
					{
						value = ((scanline[posn] & 0xF8) << 8) |
								((scanline[posn + 1] & 0xFC) << 3) |
								((scanline[posn + 2] & 0xF8) >> 3);
						data[offset]     = (byte)value;
						data[offset + 1] = (byte)(value >> 8);
						offset += multX * 2;
						posn += 3;
					}
				}
			}

	// Process a 8-bit RGB image with alpha.
	private static void RgbAlpha8bpp(Frame frame, ScanlineReader reader,
								     int width, int height,
								     int offsetX, int offsetY,
								     int multX, int multY)
			{
				int x, y, offset, posn;
				byte[] scanline;
				byte[] data = frame.Data;
				for(y = 0; y < height; ++y)
				{
					offset = ((y * multY) + offsetY) * frame.Stride +
							 offsetX * 4;
					posn = 0;
					scanline = reader.Read();
					for(x = 0; x < width; ++x)
					{
						// Flip the RGB PNG data into BGR form.
						data[offset]     = scanline[posn + 2];
						data[offset + 1] = scanline[posn + 1];
						data[offset + 2] = scanline[posn];
						data[offset + 3] = scanline[posn + 3];
						offset += multX * 4;
						posn += 4;
					}
				}
			}

	// Process a 16-bit RGB image with alpha.
	private static void RgbAlpha16bpp(Frame frame, ScanlineReader reader,
								      int width, int height,
								      int offsetX, int offsetY,
								      int multX, int multY)
			{
				int x, y, offset, posn;
				byte[] scanline;
				byte[] data = frame.Data;
				for(y = 0; y < height; ++y)
				{
					offset = ((y * multY) + offsetY) * frame.Stride +
							 offsetX * 8;
					posn = 0;
					scanline = reader.Read();
					for(x = 0; x < width; ++x)
					{
						// Flip the RGB PNG data into BGR form.
						data[offset]     = scanline[posn + 5];
						data[offset + 1] = scanline[posn + 4];
						data[offset + 2] = scanline[posn + 3];
						data[offset + 3] = scanline[posn + 2];
						data[offset + 4] = scanline[posn + 1];
						data[offset + 5] = scanline[posn];
						data[offset + 6] = scanline[posn + 7];
						data[offset + 7] = scanline[posn + 6];
						offset += multX * 8;
						posn += 8;
					}
				}
			}

	// Process a RGB image with alpha and 555 bit fields.
	private static void RgbAlpha555(Frame frame, ScanlineReader reader,
							   		int width, int height,
							   		int offsetX, int offsetY,
							   		int multX, int multY)
			{
				int x, y, offset, posn, value;
				byte[] scanline;
				byte[] data = frame.Data;
				for(y = 0; y < height; ++y)
				{
					offset = ((y * multY) + offsetY) * frame.Stride +
							 offsetX * 2;
					posn = 0;
					scanline = reader.Read();
					for(x = 0; x < width; ++x)
					{
						value = ((scanline[posn] & 0xF8) << 7) |
								((scanline[posn + 1] & 0xF8) << 2) |
								((scanline[posn + 2] & 0xF8) >> 3) |
								((scanline[posn + 3] & 0x80) << 8);
						data[offset]     = (byte)value;
						data[offset + 1] = (byte)(value >> 8);
						offset += multX * 2;
						posn += 4;
					}
				}
			}

	// Class that assists with reading from PNG chunks and
	// calculating CRC-32 values as we go.
	private class ChunkReader
	{
		// Internal state.
		private Stream stream;
		private uint length;
		private int type;
		private Crc32 crc;
		private bool crcChecked;

		// Constructor.
		public ChunkReader(Stream stream, byte[] buffer)
				{
					// Record the stream for later.
					this.stream = stream;

					// Read the contents of the chunk header.
					if(stream.Read(buffer, 0, 8) != 8)
					{
						throw new FormatException("truncated chunk header");
					}
					length = (uint)(Utils.ReadInt32B(buffer, 0));
					type = Utils.ReadInt32B(buffer, 4);

					// Initialize the checksum process.
					crc = new Crc32();
					crc.Update(buffer, 4, 4);
					crcChecked = false;
				}

		// Get the chunk length.
		public int Length
				{
					get
					{
						return (int)length;
					}
				}

		// Get the chunk type.
		public int Type
				{
					get
					{
						return type;
					}
				}

		// Read data from the chunk.
		public int Read(byte[] buffer, int offset, int count)
				{
					if(((uint)count) > length)
					{
						count = (int)length;
					}
					if(count > 0)
					{
						if(stream.Read(buffer, offset, count) != count)
						{
							throw new FormatException("truncated chunk data");
						}
						crc.Update(buffer, offset, count);
						length -= (uint)count;
					}
					if(length == 0 && !crcChecked)
					{
						// Read the CRC and then check it.
						crcChecked = true;
						int b1 = stream.ReadByte();
						int b2 = stream.ReadByte();
						int b3 = stream.ReadByte();
						int b4 = stream.ReadByte();
						if(b1 == -1 || b2 == -1 || b3 == -1 || b4 == -1)
						{
							throw new FormatException("truncated CRC");
						}
						int value = (b1 << 24) | (b2 << 16) | (b3 << 8) | b4;
						if(value != (int)(crc.Value))
						{
							throw new FormatException("invalid CRC");
						}
					}
					return count;
				}

		// Skip the remaining data in the chunk.
		public void Skip(byte[] buffer)
				{
					while(length > 0)
					{
						Read(buffer, 0, buffer.Length);
					}
				}

		// Reset the chunk reader for the next chunk.
		public void Reset(byte[] buffer)
				{
					// Skip the remaining data in this chunk if necessary.
					Skip(buffer);

					// Read the contents of the next chunk header.
					if(stream.Read(buffer, 0, 8) != 8)
					{
						throw new FormatException("truncated chunk header");
					}
					length = (uint)(Utils.ReadInt32B(buffer, 0));
					type = Utils.ReadInt32B(buffer, 4);

					// Reset the checksum computation.
					crc.Reset();
					crc.Update(buffer, 4, 4);
					crcChecked = false;
				}

	}; // class ChunkReader

	// Zlib decompressor.
	private class ZlibDecompressor
	{
		// Internal state.
		private ChunkReader reader;
		private byte[] buffer;
		private Inflater inflater;
		private bool done;

		// Constructor.
		public ZlibDecompressor(ChunkReader reader)
				{
					this.reader = reader;
					this.buffer = new byte [1024];
					this.inflater = new Inflater();
					this.done = false;
				}

		// Read more input from the IDAT chunks.  False if no more.
		private bool ReadInput()
				{
					int len;
					while(reader.Type == IDAT)
					{
						// Read the next block from the current IDAT chunk.
						len = reader.Read(buffer, 0, buffer.Length);
						if(len != 0)
						{
							inflater.SetInput(buffer, 0, len);
							return true;
						}

						// We've reached the end of the current IDAT chunk.
						reader.Reset(buffer);
					}
					return false;
				}

		// Eat the remaining IDAT data in case there are more IDAT data
		// blocks than actual compressed bytes in the zlib stream, or we
		// aborted early for some reason.
		public void EatRemaining()
				{
					while(reader.Type == IDAT)
					{
						reader.Reset(buffer);
					}
				}

		// Read data from the zlib stream in the IDAT chunks.
		public int Read(byte[] buffer, int offset, int count)
				{
					int total = 0;
					int len;
					while(!done && count > 0)
					{
						len = inflater.Inflate(buffer, offset, count);
						if(len != 0)
						{
							// The inflater gave us some data.
							offset += len;
							count -= len;
							total += len;
						}
						else if(inflater.IsFinished)
						{
							// This is the end of the zlib stream.
							done = true;
							break;
						}
						else if(inflater.IsNeedingInput)
						{
							// The inflater requested more input bytes.
							if(!ReadInput())
							{
								throw new FormatException
									("insufficient data for zlib stream");
							}
						}
						else if(inflater.IsNeedingDictionary)
						{
							// PNG files cannot have compression dictionaries.
							throw new FormatException
								("zlib stream requested a dictionary");
						}
						else
						{
							// This shouldn't happen.
							throw new FormatException
								("zlib decompression failed");
						}
					}
					return total;
				}

	}; // class ZlibDecompressor

	// Scanline reader.  This reads scanlines from a zlib stream
	// and applies filtering to each line.
	private class ScanlineReader
	{
		// Internal state.
		private ZlibDecompressor decompressor;
		private int bytesPerPixel;
		private int bytesPerLine;
		private int y;
		private int height;
		private byte[] scanline;
		private byte[] prevScanline;

		// Constructor.
		public ScanlineReader(ZlibDecompressor decompressor,
							  int width, int height,
							  int colorType, int bitDepth)
				{
					// Determine the number of bytes per pixel (rounded up),
					// and the number of bytes per scan line.
					switch(colorType)
					{
						case 0:
						{
							switch(bitDepth)
							{
								case 1:
								{
									bytesPerPixel = 1;
									bytesPerLine = (width + 7) / 8;
								}
								break;

								case 2:
								{
									bytesPerPixel = 1;
									bytesPerLine = (width + 3) / 4;
								}
								break;

								case 4:
								{
									bytesPerPixel = 1;
									bytesPerLine = (width + 1) / 2;
								}
								break;

								case 8:
								{
									bytesPerPixel = 1;
									bytesPerLine = width;
								}
								break;

								case 16:
								{
									bytesPerPixel = 2;
									bytesPerLine = width * 2;
								}
								break;
							}
						}
						break;

						case 2:
						{
							if(bitDepth == 8)
							{
								bytesPerPixel = 3;
								bytesPerLine = width * 3;
							}
							else
							{
								bytesPerPixel = 6;
								bytesPerLine = width * 6;
							}
						}
						break;

						case 3:
						{
							bytesPerPixel = 1;
							bytesPerLine = width;
						}
						break;

						case 4:
						{
							if(bitDepth == 8)
							{
								bytesPerPixel = 2;
								bytesPerLine = width * 2;
							}
							else
							{
								bytesPerPixel = 4;
								bytesPerLine = width * 4;
							}
						}
						break;

						case 6:
						{
							if(bitDepth == 8)
							{
								bytesPerPixel = 4;
								bytesPerLine = width * 4;
							}
							else
							{
								bytesPerPixel = 8;
								bytesPerLine = width * 8;
							}
						}
						break;
					}

					// Allocate the scanline buffers.
					scanline = new byte [bytesPerLine];
					prevScanline = new byte [bytesPerLine];

					// Initialize the other values.
					this.decompressor = decompressor;
					this.y = 0;
					this.height = height;
				}

		// Read the next scanline.  Returns null at the end of the image.
		public byte[] Read()
				{
					int type, posn;
					byte[] temp;
					int bpp = bytesPerPixel;
					int size = bytesPerLine;

					// Bail out if the image is finished.
					if(y >= height)
					{
						return null;
					}

					// Read the filter type byte for the scanline.
					if(decompressor.Read(scanline, 0, 1) != 1)
					{
						throw new FormatException
							("could not read the filter type byte for " +
							 "scanline " + y.ToString());
					}
					type = scanline[0];
					if(type < 0 || type > 4)
					{
						throw new FormatException
							("invalid filter type byte for scanline " +
							 y.ToString() + ": " + type.ToString());
					}

					// Read the scanline data.
					if(decompressor.Read(scanline, 0, size) != size)
					{
						throw new FormatException
							("scanline " + y.ToString() + " is truncated");
					}

					// Apply the specified filter.
					switch(type)
					{
						case 1:
						{
							// Apply the "Sub" filter.
							for(posn = bpp; posn < size; ++posn)
							{
								scanline[posn] += scanline[posn - bpp];
							}
						}
						break;

						case 2:
						{
							// Apply the "Up" filter.
							for(posn = 0; posn < size; ++posn)
							{
								scanline[posn] += prevScanline[posn];
							}
						}
						break;

						case 3:
						{
							// Apply the "Average" filter.
							for(posn = 0; posn < bpp; ++posn)
							{
								scanline[posn] +=
									(byte)(prevScanline[posn] / 2);
							}
							for(posn = bpp; posn < size; ++posn)
							{
								scanline[posn] +=
									(byte)((scanline[posn - bpp] +
									       prevScanline[posn]) / 2);
							}
						}
						break;

						case 4:
						{
							// Apply the "Paeth" filter.
							for(posn = 0; posn < bpp; ++posn)
							{
								scanline[posn] +=
									Paeth(0, prevScanline[posn], 0);
							}
							for(posn = bpp; posn < size; ++posn)
							{
								scanline[posn] +=
									Paeth(scanline[posn - bpp],
										  prevScanline[posn],
										  prevScanline[posn - bpp]);
							}
						}
						break;
					}

					// Swap the scanline buffers and return to the caller.
					temp = scanline;
					scanline = prevScanline;
					prevScanline = temp;
					return temp;
				}

	}; // class ScanlineReader

}; // class PngReader

}; // namespace DotGNU.Images
