/*
 * PngWriter.cs - Implementation of the "DotGNU.Images.PngWriter" class.
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

internal sealed class PngWriter
{
	// Magic number header for PNG images.
	private static readonly byte[] magic = {137, 80, 78, 71, 13, 10, 26, 10};

	// Save a PNG image to the specified stream.
	public static void Save(Stream stream, Image image)
			{
				Frame frame = image.GetFrame(0);
				byte[] buffer = new byte [1024];
				ChunkWriter writer = new ChunkWriter(stream);
				int colorType, bitDepth;
				int sigRed, sigGreen, sigBlue, sigAlpha;
				int paletteSize, posn;
				int[] palette;
				ZlibCompressor compressor;
				ScanlineWriter scanlineWriter;
				OutputFunc func;
				int y;

				// Determine the color type and bit depth for the image.
				sigRed = -1;
				sigGreen = -1;
				sigBlue = -1;
				sigAlpha = -1;
				paletteSize = 0;
				switch(frame.PixelFormat)
				{
					case PixelFormat.Format16bppRgb555:
					{
						sigRed = 5;
						sigGreen = 5;
						sigBlue = 5;
						colorType = 2;
						bitDepth = 8;
					}
					break;

					case PixelFormat.Format16bppRgb565:
					{
						sigRed = 5;
						sigGreen = 6;
						sigBlue = 5;
						colorType = 2;
						bitDepth = 8;
					}
					break;

					case PixelFormat.Format24bppRgb:
					case PixelFormat.Format32bppRgb:
					{
						colorType = 2;
						bitDepth = 8;
					}
					break;

					case PixelFormat.Format1bppIndexed:
					{
						colorType = 3;
						bitDepth = 1;
						paletteSize = 2;
					}
					break;

					case PixelFormat.Format4bppIndexed:
					{
						colorType = 3;
						bitDepth = 4;
						paletteSize = 16;
					}
					break;

					case PixelFormat.Format8bppIndexed:
					{
						colorType = 3;
						bitDepth = 8;
						paletteSize = 256;
					}
					break;

					case PixelFormat.Format16bppArgb1555:
					{
						sigRed = 5;
						sigGreen = 5;
						sigBlue = 5;
						sigAlpha = 1;
						colorType = 6;
						bitDepth = 8;
					}
					break;

					case PixelFormat.Format32bppPArgb:
					case PixelFormat.Format32bppArgb:
					{
						colorType = 6;
						bitDepth = 8;
					}
					break;

					case PixelFormat.Format16bppGrayScale:
					{
						colorType = 0;
						bitDepth = 16;
					}
					break;

					case PixelFormat.Format48bppRgb:
					{
						colorType = 2;
						bitDepth = 16;
					}
					break;

					case PixelFormat.Format64bppPArgb:
					case PixelFormat.Format64bppArgb:
					{
						colorType = 6;
						bitDepth = 16;
					}
					break;

					default: throw new FormatException("unknown format");
				}

				// Write out the PNG magic number.
				stream.Write(magic, 0, magic.Length);

				// Write the header chunk.
				Utils.WriteInt32B(buffer, 0, frame.Width);
				Utils.WriteInt32B(buffer, 4, frame.Height);
				buffer[8] = (byte)bitDepth;
				buffer[9] = (byte)colorType;
				buffer[10] = (byte)0;			// Compression method.
				buffer[11] = (byte)0;			// Filter method.
				buffer[12] = (byte)0;			// Interlace method.
				writer.Write(PngReader.IHDR, buffer, 0, 13);

				// Write the significant bits chunk if necessary.
				if(sigAlpha != -1)
				{
					buffer[0] = (byte)sigRed;
					buffer[1] = (byte)sigGreen;
					buffer[2] = (byte)sigBlue;
					buffer[3] = (byte)sigAlpha;
					writer.Write(PngReader.sBIT, buffer, 0, 4);
				}
				else if(sigRed != -1)
				{
					buffer[0] = (byte)sigRed;
					buffer[1] = (byte)sigGreen;
					buffer[2] = (byte)sigBlue;
					writer.Write(PngReader.sBIT, buffer, 0, 3);
				}

				// Write the palette and transparency chunks.
				if(paletteSize > 0)
				{
					Array.Clear(buffer, 0, buffer.Length);
					palette = frame.Palette;
					if(palette != null)
					{
						for(posn = 0; posn < palette.Length &&
									  posn < paletteSize; ++posn)
						{
							buffer[posn * 3] = (byte)(palette[posn] >> 16);
							buffer[posn * 2 + 1] = (byte)(palette[posn] >> 8);
							buffer[posn * 2 + 2] = (byte)(palette[posn]);
						}
					}
					writer.Write(PngReader.PLTE, buffer, 0, paletteSize * 3);
					if(frame.TransparentPixel >= 0 &&
					   frame.TransparentPixel < paletteSize)
					{
						for(posn = 0; posn < paletteSize; ++posn)
						{
							buffer[posn] = (byte)0xFF;
						}
						buffer[frame.TransparentPixel] = (byte)0x00;
						writer.Write(PngReader.tRNS, buffer, 0,
									 frame.TransparentPixel + 1);
					}
				}

				// Compress and write the scanlines to the output stream.
				compressor = new ZlibCompressor(writer);
				scanlineWriter = new ScanlineWriter
					(compressor, frame.Width, frame.PixelFormat);
				func = GetOutputFunc(frame.PixelFormat);
				for(y = 0; y < frame.Height; ++y)
				{
					func(frame, y, scanlineWriter.Buffer);
					scanlineWriter.FlushScanline();
				}
				compressor.Finish();

				// Write the end chunk.
				writer.Write(PngReader.IEND, buffer, 0, 0);
			}

	// Delegate type for scanline output functions.
	private delegate void OutputFunc(Frame frame, int y, byte[] scanline);

	// Get the scanline output function for a particular pixel format.
	private static OutputFunc GetOutputFunc(PixelFormat format)
			{
				OutputFunc func = null;
				switch(format)
				{
					case PixelFormat.Format16bppRgb555:
					{
						func = new OutputFunc(Rgb555);
					}
					break;

					case PixelFormat.Format16bppRgb565:
					{
						func = new OutputFunc(Rgb565);
					}
					break;

					case PixelFormat.Format24bppRgb:
					{
						func = new OutputFunc(Rgb24bpp);
					}
					break;

					case PixelFormat.Format32bppRgb:
					{
						func = new OutputFunc(Rgb32bpp);
					}
					break;

					case PixelFormat.Format1bppIndexed:
					{
						func = new OutputFunc(Indexed1bpp);
					}
					break;

					case PixelFormat.Format4bppIndexed:
					{
						func = new OutputFunc(Indexed4bpp);
					}
					break;

					case PixelFormat.Format8bppIndexed:
					{
						func = new OutputFunc(Indexed8bpp);
					}
					break;

					case PixelFormat.Format16bppArgb1555:
					{
						func = new OutputFunc(RgbAlpha555);
					}
					break;

					case PixelFormat.Format32bppPArgb:
					case PixelFormat.Format32bppArgb:
					{
						func = new OutputFunc(RgbAlpha32bpp);
					}
					break;

					case PixelFormat.Format16bppGrayScale:
					{
						func = new OutputFunc(GrayScale16bpp);
					}
					break;

					case PixelFormat.Format48bppRgb:
					{
						func = new OutputFunc(Rgb48bpp);
					}
					break;

					case PixelFormat.Format64bppPArgb:
					case PixelFormat.Format64bppArgb:
					{
						func = new OutputFunc(RgbAlpha64bpp);
					}
					break;
				}
				return func;
			}

	// Output RGB data in 15-bit 555 format.
	private static void Rgb555(Frame frame, int y, byte[] scanline)
			{
				int width = frame.Width;
				byte[] data = frame.Data;
				int posn, offset, value, component;
				offset = y * frame.Stride + width * 2;
				for(posn = (width - 1) * 3; posn >= 0; posn -= 3)
				{
					offset -= 2;
					value = data[offset] | (data[offset + 1] << 8);
					component = ((value >> 7) & 0xF8);
					scanline[posn]     = (byte)(component | (component >> 5));
					component = ((value >> 2) & 0xF8);
					scanline[posn + 1] = (byte)(component | (component >> 5));
					component = ((value << 3) & 0xF8);
					scanline[posn + 2] = (byte)(component | (component >> 5));
				}
			}

	// Output RGB data in 16-bit 565 format.
	private static void Rgb565(Frame frame, int y, byte[] scanline)
			{
				int width = frame.Width;
				byte[] data = frame.Data;
				int posn, offset, value, component;
				offset = y * frame.Stride + width * 2;
				for(posn = (width - 1) * 3; posn >= 0; posn -= 3)
				{
					offset -= 2;
					value = data[offset] | (data[offset + 1] << 8);
					component = ((value >> 8) & 0xF8);
					scanline[posn]     = (byte)(component | (component >> 5));
					component = ((value >> 3) & 0xFC);
					scanline[posn + 1] = (byte)(component | (component >> 6));
					component = ((value << 3) & 0xF8);
					scanline[posn + 2] = (byte)(component | (component >> 5));
				}
			}

	// Output RGB data in 15-bit 555 format with a 1-bit alpha channel.
	private static void RgbAlpha555(Frame frame, int y, byte[] scanline)
			{
				int width = frame.Width;
				byte[] data = frame.Data;
				int posn, offset, value, component;
				offset = y * frame.Stride + width * 2;
				for(posn = (width - 1) * 4; posn >= 0; posn -= 4)
				{
					offset -= 2;
					value = data[offset] | (data[offset + 1] << 8);
					component = ((value >> 7) & 0xF8);
					scanline[posn]     = (byte)(component | (component >> 5));
					component = ((value >> 2) & 0xF8);
					scanline[posn + 1] = (byte)(component | (component >> 5));
					component = ((value << 3) & 0xF8);
					scanline[posn + 2] = (byte)(component | (component >> 5));
					if((value & 0x8000) != 0)
					{
						scanline[posn + 3] = 0xFF;
					}
					else
					{
						scanline[posn + 3] = 0x00;
					}
				}
			}

	// Output 24-bit RGB data.
	private static void Rgb24bpp(Frame frame, int y, byte[] scanline)
			{
				int width = frame.Width;
				byte[] data = frame.Data;
				int posn, offset;
				offset = y * frame.Stride + width * 3;
				for(posn = (width - 1) * 3; posn >= 0; posn -= 3)
				{
					// Convert BGR data into RGB data.
					offset -= 3;
					scanline[posn]     = data[offset + 2];
					scanline[posn + 1] = data[offset + 1];
					scanline[posn + 2] = data[offset];
				}
			}

	// Output 32-bit RGB data.
	private static void Rgb32bpp(Frame frame, int y, byte[] scanline)
			{
				int width = frame.Width;
				byte[] data = frame.Data;
				int posn, offset;
				offset = y * frame.Stride + width * 3;
				for(posn = (width - 1) * 4; posn >= 0; posn -= 4)
				{
					// Convert BGR data into RGB data.
					offset -= 3;
					scanline[posn]     = data[offset + 2];
					scanline[posn + 1] = data[offset + 1];
					scanline[posn + 2] = data[offset];
				}
			}

	// Output 32-bit RGB data with an alpha channel.
	private static void RgbAlpha32bpp(Frame frame, int y, byte[] scanline)
			{
				int width = frame.Width;
				byte[] data = frame.Data;
				int posn, offset;
				offset = y * frame.Stride + width * 4;
				for(posn = (width - 1) * 4; posn >= 0; posn -= 4)
				{
					// Convert BGR data into RGB data.
					offset -= 4;
					scanline[posn]     = data[offset + 2];
					scanline[posn + 1] = data[offset + 1];
					scanline[posn + 2] = data[offset];
					scanline[posn + 3] = data[offset + 3];
				}
			}

	// Output 48-bit RGB data.
	private static void Rgb48bpp(Frame frame, int y, byte[] scanline)
			{
				int width = frame.Width;
				byte[] data = frame.Data;
				int posn, offset;
				offset = y * frame.Stride + width * 6;
				for(posn = (width - 1) * 6; posn >= 0; posn -= 6)
				{
					// Convert BGR data into RGB data and byteswap.
					offset -= 6;
					scanline[posn]     = data[offset + 5];
					scanline[posn + 1] = data[offset + 4];
					scanline[posn + 2] = data[offset + 3];
					scanline[posn + 3] = data[offset + 2];
					scanline[posn + 4] = data[offset + 1];
					scanline[posn + 5] = data[offset];
				}
			}

	// Output 64-bit RGB data with an alpha channel.
	private static void RgbAlpha64bpp(Frame frame, int y, byte[] scanline)
			{
				int width = frame.Width;
				byte[] data = frame.Data;
				int posn, offset;
				offset = y * frame.Stride + width * 8;
				for(posn = (width - 1) * 8; posn >= 0; posn -= 8)
				{
					// Convert BGR data into RGB data and byteswap.
					offset -= 8;
					scanline[posn]     = data[offset + 5];
					scanline[posn + 1] = data[offset + 4];
					scanline[posn + 2] = data[offset + 3];
					scanline[posn + 3] = data[offset + 1];
					scanline[posn + 4] = data[offset + 1];
					scanline[posn + 5] = data[offset];
					scanline[posn + 6] = data[offset + 7];
					scanline[posn + 7] = data[offset + 6];
				}
			}

	// Output 1-bit indexed data.
	private static void Indexed1bpp(Frame frame, int y, byte[] scanline)
			{
				int width = frame.Width;
				byte[] data = frame.Data;
				int posn, offset, bit;
				offset = y * frame.Stride;
				bit = 0x80;
				for(posn = 0; posn < width; ++posn)
				{
					if((data[offset] & bit) != 0)
					{
						scanline[posn] = 1;
					}
					else
					{
						scanline[posn] = 0;
					}
					bit >>= 1;
					if(bit == 0)
					{
						++offset;
						bit = 0x80;
					}
				}
			}

	// Output 4-bit indexed data.
	private static void Indexed4bpp(Frame frame, int y, byte[] scanline)
			{
				int width = frame.Width;
				byte[] data = frame.Data;
				int posn, offset, bit;
				offset = y * frame.Stride;
				bit = 4;
				for(posn = 0; posn < width; ++posn)
				{
					scanline[posn] = (byte)((data[offset] >> bit) & 0x0F);
					bit -= 4;
					if(bit < 0)
					{
						++offset;
						bit = 4;
					}
				}
			}

	// Output 8-bit indexed data.
	private static void Indexed8bpp(Frame frame, int y, byte[] scanline)
			{
				Array.Copy(frame.Data, y * frame.Stride,
						   scanline, 0, frame.Width);
			}

	// Output 16-bit grayscale data.
	private static void GrayScale16bpp(Frame frame, int y, byte[] scanline)
			{
				int width = frame.Width;
				byte[] data = frame.Data;
				int posn, offset;
				offset = y * frame.Stride + width * 2;
				for(posn = (width - 1) * 2; posn >= 0; posn -= 2)
				{
					// Byteswap the data.
					offset -= 2;
					scanline[posn]     = data[offset + 1];
					scanline[posn + 1] = data[offset];
				}
			}

	// Chunk writer class.
	private class ChunkWriter
	{
		// Internal state.
		private Stream stream;
		private Crc32 crc;
		private byte[] header;

		// Constructor.
		public ChunkWriter(Stream stream)
				{
					this.stream = stream;
					this.crc = new Crc32();
					this.header = new byte [8];
				}

		// Start a chunk with a particular type.
		public void StartChunk(int type, int length)
				{
					// Format the header.
					Utils.WriteInt32B(header, 0, length);
					Utils.WriteInt32B(header, 4, type);

					// Write out the header.
					stream.Write(header, 0, 8);

					// Reset the CRC computation and add the chunk type.
					crc.Reset();
					crc.Update(header, 4, 4);
				}

		// Write data to the current chunk.
		public void Write(byte[] buffer, int offset, int count)
				{
					if(count > 0)
					{
						stream.Write(buffer, offset, count);
						crc.Update(buffer, offset, count);
					}
				}

		// Write out the end of the current chunk.
		public void EndChunk()
				{
					// Write out the CRC at the end of the chunk.
					Utils.WriteInt32B(header, 0, (int)(crc.Value));
					stream.Write(header, 0, 4);
				}

		// Write a full chunk in one call.
		public void Write(int type, byte[] buffer, int offset, int count)
				{
					StartChunk(type, count);
					Write(buffer, offset, count);
					EndChunk();
				}

	}; // class ChunkWriter

	// Zlib compression object.
	private class ZlibCompressor
	{
		// Internal state.
		private ChunkWriter writer;
		private Deflater deflater;
		private byte[] outBuffer;
		private int outLen;
		private bool wroteBlock;

		// Constructor.
		public ZlibCompressor(ChunkWriter writer)
				{
					this.writer = writer;
					this.deflater = new Deflater();
					this.outBuffer = new byte [4096];
					this.outLen = 0;
					this.wroteBlock = false;
				}

		// Write data to this compressor.
		public void Write(byte[] buffer, int offset, int count)
				{
					int len;

					// Set the input for the deflater.
					if(count == 0)
					{
						// Nothing to do if no data was supplied.
						return;
					}
					deflater.SetInput(buffer, offset, count);

					// Deflate data until the deflater asks for more input.
					for(;;)
					{
						len = deflater.Deflate
							(outBuffer, outLen, outBuffer.Length - outLen);
						if(len > 0)
						{
							outLen += len;
							if(outLen >= outBuffer.Length)
							{
								writer.Write
									(PngReader.IDAT, outBuffer, 0, outLen);
								outLen = 0;
								wroteBlock = true;
							}
						}
						else
						{
							break;
						}
					}
				}

		// Mark the stream as finished.
		public void Finish()
				{
					int len;

					// Tell the deflater that the input has finished.
					deflater.Finish();

					// Flush the remaining deflated data to the output buffer.
					for(;;)
					{
						len = deflater.Deflate
							(outBuffer, outLen, outBuffer.Length - outLen);
						if(len > 0)
						{
							outLen += len;
							if(outLen >= outBuffer.Length)
							{
								writer.Write
									(PngReader.IDAT, outBuffer, 0, outLen);
								outLen = 0;
								wroteBlock = true;
							}
						}
						else
						{
							break;
						}
					}

					// Flush the final output block if it is short.
					if(outLen > 0)
					{
						writer.Write(PngReader.IDAT, outBuffer, 0, outLen);
						wroteBlock = true;
					}

					// If we didn't write any blocks, then output a
					// zero-length IDAT chunk so that we have something.
					// This shouldn't happen, but let's be paranoid.
					if(!wroteBlock)
					{
						writer.Write(PngReader.IDAT, outBuffer, 0, 0);
					}
				}

	}; // class ZlibCompressor

	// Scanline writing object.
	private class ScanlineWriter
	{
		// Internal state.
		private ZlibCompressor compressor;
		private int bytesPerLine;
		private int bytesPerPixel;
		private bool usePaeth;
		private byte[] scanline;
		private byte[] prevScanline;
		private byte[] paeth;
		private byte[] filter;
		private int y;

		// Constructor.
		public ScanlineWriter(ZlibCompressor compressor,
							  int width, PixelFormat format)
				{
					// Initialize the object.
					this.compressor = compressor;
					this.usePaeth = true;
					this.filter = new byte [1];
					this.y = 0;

					// Get the scanline size parameters.
					switch(format)
					{
						case PixelFormat.Format16bppRgb555:
						case PixelFormat.Format16bppRgb565:
						case PixelFormat.Format24bppRgb:
						case PixelFormat.Format32bppRgb:
						{
							bytesPerLine = width * 3;
							bytesPerPixel = 3;
						}
						break;
	
						case PixelFormat.Format1bppIndexed:
						case PixelFormat.Format4bppIndexed:
						case PixelFormat.Format8bppIndexed:
						{
							bytesPerLine = width;
							bytesPerPixel = 1;
							usePaeth = false;
						}
						break;
	
						case PixelFormat.Format16bppArgb1555:
						case PixelFormat.Format32bppPArgb:
						case PixelFormat.Format32bppArgb:
						{
							bytesPerLine = width * 4;
							bytesPerPixel = 4;
						}
						break;
	
						case PixelFormat.Format16bppGrayScale:
						{
							bytesPerLine = width * 2;
							bytesPerPixel = 2;
						}
						break;
	
						case PixelFormat.Format48bppRgb:
						{
							bytesPerLine = width * 6;
							bytesPerPixel = 6;
						}
						break;
	
						case PixelFormat.Format64bppPArgb:
						case PixelFormat.Format64bppArgb:
						{
							bytesPerLine = width * 8;
							bytesPerPixel = 8;
						}
						break;
					}

					// Allocate space for the scanline buffers.
					scanline = new byte [bytesPerLine];
					prevScanline = new byte [bytesPerLine];
					paeth = (usePaeth ? new byte [bytesPerLine] : null);
				}

		// Get the scanline buffer for the current scanline.
		public byte[] Buffer
				{
					get
					{
						return scanline;
					}
				}

		// Flush the current scanline.
		public void FlushScanline()
				{
					byte[] temp;
					int x, posn, width;
					int bpp = this.bytesPerPixel;
					int bytesPerLine = this.bytesPerLine;

					// Filter the scanline.  We use a fairly simple approach,
					// using no filter for indexed images and the "Paeth"
					// filter for RGB images, as recommended by RFC-2083.
					// It is possible to dynamically adjust the filter to
					// match the scanline, but we don't do that at the moment.
					if(usePaeth && y > 0 && bytesPerLine > 0)
					{
						// Apply the "Paeth" filter to the line.
						temp = paeth;
						for(posn = 0; posn < bpp; ++posn)
						{
							temp[posn] = (byte)(scanline[posn] -
								PngReader.Paeth(0, prevScanline[posn], 0));
						}
						for(posn = bpp; posn < bytesPerLine; ++posn)
						{
							temp[posn] = (byte)(scanline[posn] -
								PngReader.Paeth
									(scanline[posn - bpp],
									 prevScanline[posn],
									 prevScanline[posn - bpp]));
						}
						filter[0] = 4;
					}
					else
					{
						// No filter is needed for this scanline.
						temp = scanline;
						filter[0] = 0;
					}

					// Write the filter type byte and the filtered scanline.
					compressor.Write(filter, 0, 1);
					compressor.Write(temp, 0, bytesPerLine);

					// Swap the buffers and advance to the next scanline.
					temp = scanline;
					scanline = prevScanline;
					prevScanline = temp;
					++y;
				}

	}; // class ScanlineWriter

}; // class PngWriter

}; // namespace DotGNU.Images
