/*
 * JpegWriter.cs - Implementation of the "DotGNU.Images.JpegWriter" class.
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
using System.Runtime.InteropServices;
using OpenSystem.Platform;

internal unsafe sealed class JpegWriter
{
	// Save a JPEG image to the specified stream.
	public static void Save(Stream stream, Image image)
			{
				// Determine if we actually have the JPEG library.
				if(!JpegLib.JpegLibraryPresent())
				{
					throw new FormatException("libjpeg is not available");
				}

				// Create the compression object.
				JpegLib.jpeg_compress_struct cinfo;
				cinfo = new JpegLib.jpeg_compress_struct();
				cinfo.err = JpegLib.CreateErrorHandler();
				JpegLib.jpeg_create_compress(ref cinfo);

				// Initialize the destination manager.
				JpegLib.StreamToDestinationManager(ref cinfo, stream);

				// Get the frame to be written.
				Frame frame = image.GetFrame(0);

				// Set the JPEG compression parameters.
				cinfo.image_width = (UInt)(frame.Width);
				cinfo.image_height = (UInt)(frame.Height);
				cinfo.input_components = (Int)3;
				cinfo.in_color_space = JpegLib.J_COLOR_SPACE.JCS_RGB;
				JpegLib.jpeg_set_defaults(ref cinfo);

				// Start the compression process.
				JpegLib.jpeg_start_compress(ref cinfo, (Int)1);

				// Write the scanlines to the image.
				int posn, width, offset, stride;
				width = frame.Width;
				stride = frame.Stride;
				byte[] data = frame.Data;
				PixelFormat format = frame.PixelFormat;
				IntPtr buf = Marshal.AllocHGlobal(width * 3);
				byte *pbuf = (byte *)buf;
				while(((int)(cinfo.next_scanline)) <
							((int)(cinfo.image_height)))
				{
					// Convert the source scanline into the JCS_RGB format.
					offset = ((int)(cinfo.next_scanline)) * stride;
					switch(format)
					{
						case PixelFormat.Format16bppRgb555:
						case PixelFormat.Format16bppArgb1555:
						{
							Rgb555(data, offset, width, pbuf);
						}
						break;

						case PixelFormat.Format16bppRgb565:
						{
							Rgb565(data, offset, width, pbuf);
						}
						break;

						case PixelFormat.Format24bppRgb:
						{
							Rgb24bpp(data, offset, width, pbuf);
						}
						break;

						case PixelFormat.Format32bppRgb:
						case PixelFormat.Format32bppArgb:
						case PixelFormat.Format32bppPArgb:
						{
							Rgb32bpp(data, offset, width, pbuf);
						}
						break;

						case PixelFormat.Format1bppIndexed:
						{
							Rgb1bpp(data, offset, width, pbuf, frame.Palette);
						}
						break;

						case PixelFormat.Format4bppIndexed:
						{
							Rgb4bpp(data, offset, width, pbuf, frame.Palette);
						}
						break;

						case PixelFormat.Format8bppIndexed:
						{
							Rgb8bpp(data, offset, width, pbuf, frame.Palette);
						}
						break;

						case PixelFormat.Format16bppGrayScale:
						{
							GrayScale16bpp(data, offset, width, pbuf);
						}
						break;

						case PixelFormat.Format48bppRgb:
						{
							Rgb48bpp(data, offset, width, pbuf);
						}
						break;

						case PixelFormat.Format64bppPArgb:
						case PixelFormat.Format64bppArgb:
						{
							Rgb64bpp(data, offset, width, pbuf);
						}
						break;
					}

					// Write the scanline to the buffer.
					JpegLib.jpeg_write_scanlines
						(ref cinfo, ref buf, (UInt)1);
				}
				Marshal.FreeHGlobal(buf);

				// Finish the compression process.
				JpegLib.jpeg_finish_compress(ref cinfo);

				// Clean everything up.
				JpegLib.FreeDestinationManager(ref cinfo);
				JpegLib.jpeg_destroy_compress(ref cinfo);
				JpegLib.FreeErrorHandler(cinfo.err);
			}

	// Convert 15-bit RGB data into the JPEG scanline format.
	private static void Rgb555(byte[] data, int offset, int width, byte *buf)
			{
				int posn = 0;
				int value;
				while(width > 0)
				{
					value = data[offset] | (data[offset + 1] << 8);
					buf[posn]     = (byte)(((value >> 7) & 0xF8) |
									       ((value >> 12) & 0x07));
					buf[posn + 1] = (byte)(((value >> 2) & 0xF8) |
									       ((value >> 7) & 0x07));
					buf[posn + 2] = (byte)(((value << 3) & 0xF8) |
									       ((value >> 2) & 0x07));
					offset += 2;
					posn += 3;
					--width;
				}
			}

	// Convert 16-bit RGB data into the JPEG scanline format.
	private static void Rgb565(byte[] data, int offset, int width, byte *buf)
			{
				int posn = 0;
				int value;
				while(width > 0)
				{
					value = data[offset] | (data[offset + 1] << 8);
					buf[posn]     = (byte)(((value >> 8) & 0xF8) |
									       ((value >> 13) & 0x07));
					buf[posn + 1] = (byte)(((value >> 3) & 0xFC) |
									       ((value >> 9) & 0x03));
					buf[posn + 2] = (byte)(((value << 3) & 0xF8) |
									       ((value >> 2) & 0x07));
					offset += 2;
					posn += 3;
					--width;
				}
			}

	// Convert 24-bit RGB data into the JPEG scanline format.
	private static void Rgb24bpp(byte[] data, int offset, int width, byte *buf)
			{
				int posn = 0;
				while(width > 0)
				{
					buf[posn]     = data[offset + 2];
					buf[posn + 1] = data[offset + 1];
					buf[posn + 2] = data[offset];
					offset += 3;
					posn += 3;
					--width;
				}
			}

	// Convert 32-bit RGB data into the JPEG scanline format.
	private static void Rgb32bpp(byte[] data, int offset, int width, byte *buf)
			{
				int posn = 0;
				while(width > 0)
				{
					buf[posn]     = data[offset + 2];
					buf[posn + 1] = data[offset + 1];
					buf[posn + 2] = data[offset];
					offset += 4;
					posn += 3;
					--width;
				}
			}

	// Convert 1-bit indexed data into the JPEG scanline format.
	private static void Rgb1bpp
				(byte[] data, int offset, int width, byte *buf, int[] palette)
			{
				int posn = 0;
				int bit = 0x80;
				int value;
				while(width > 0)
				{
					if((data[offset] & bit) != 0)
					{
						value = palette[1];
					}
					else
					{
						value = palette[0];
					}
					buf[posn]     = (byte)(value >> 16);
					buf[posn + 1] = (byte)(value >> 8);
					buf[posn + 2] = (byte)value;
					bit >>= 1;
					if(bit == 0)
					{
						bit = 0x80;
						++offset;
					}
					posn += 3;
					--width;
				}
			}

	// Convert 4-bit indexed data into the JPEG scanline format.
	private static void Rgb4bpp
				(byte[] data, int offset, int width, byte *buf, int[] palette)
			{
				int posn = 0;
				int bit = 4;
				int value;
				while(width > 0)
				{
					value = palette[(data[offset] >> bit) & 0x0F];
					buf[posn]     = (byte)(value >> 16);
					buf[posn + 1] = (byte)(value >> 8);
					buf[posn + 2] = (byte)value;
					bit -= 4;
					if(bit < 0)
					{
						bit = 4;
						++offset;
					}
					posn += 3;
					--width;
				}
			}

	// Convert 8-bit indexed data into the JPEG scanline format.
	private static void Rgb8bpp
				(byte[] data, int offset, int width, byte *buf, int[] palette)
			{
				int posn = 0;
				int value;
				while(width > 0)
				{
					value = palette[data[offset]];
					buf[posn]     = (byte)(value >> 16);
					buf[posn + 1] = (byte)(value >> 8);
					buf[posn + 2] = (byte)value;
					++offset;
					posn += 3;
					--width;
				}
			}

	// Convert 16-bit grayscale data into the JPEG scanline format.
	private static void GrayScale16bpp
				(byte[] data, int offset, int width, byte *buf)
			{
				int posn = 0;
				byte value;
				while(width > 0)
				{
					value = data[offset + 1];
					buf[posn]     = value;
					buf[posn + 1] = value;
					buf[posn + 2] = value;
					offset += 2;
					posn += 3;
					--width;
				}
			}

	// Convert 48-bit RGB data into the JPEG scanline format.
	private static void Rgb48bpp(byte[] data, int offset, int width, byte *buf)
			{
				int posn = 0;
				while(width > 0)
				{
					buf[posn]     = data[offset + 5];
					buf[posn + 1] = data[offset + 3];
					buf[posn + 2] = data[offset + 1];
					offset += 6;
					posn += 3;
					--width;
				}
			}

	// Convert 64-bit RGB data into the JPEG scanline format.
	private static void Rgb64bpp(byte[] data, int offset, int width, byte *buf)
			{
				int posn = 0;
				while(width > 0)
				{
					buf[posn]     = data[offset + 5];
					buf[posn + 1] = data[offset + 3];
					buf[posn + 2] = data[offset + 1];
					offset += 8;
					posn += 3;
					--width;
				}
			}

}; // class JpegWriter

}; // namespace DotGNU.Images
