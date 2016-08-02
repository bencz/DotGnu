/*
 * JpegReader.cs - Implementation of the "DotGNU.Images.JpegReader" class.
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

internal unsafe sealed class JpegReader
{
	// Load a JPEG image from the specified stream.  The first 4 bytes
	// have already been read and discarded.
	public static void Load(Stream stream, Image image,
							byte[] prime, int primeLen)
			{
				// Determine if we actually have the JPEG library.
				if(!JpegLib.JpegLibraryPresent())
				{
					throw new FormatException("libjpeg is not available");
				}

				// Create the decompression object.
				JpegLib.jpeg_decompress_struct cinfo;
				cinfo = new JpegLib.jpeg_decompress_struct();
				cinfo.err = JpegLib.CreateErrorHandler();
				JpegLib.jpeg_create_decompress(ref cinfo);

				// Initialize the source manager.
				JpegLib.StreamToSourceManager
					(ref cinfo, stream, prime, primeLen);

				// Read the JPEG header.
				JpegLib.jpeg_read_header(ref cinfo, (Int)1);

				// Set the decompression parameters the way we want them.
				cinfo.out_color_space = JpegLib.J_COLOR_SPACE.JCS_RGB;

				// Start the decompression process.
				JpegLib.jpeg_start_decompress(ref cinfo);

				// Initialize the image to 24-bit RGB, to match the JPEG file.
				image.Width = (int)(cinfo.output_width);
				image.Height = (int)(cinfo.output_height);
				image.PixelFormat = PixelFormat.Format24bppRgb;
				if(prime[3] == 0xE1)
				{
					image.LoadFormat = Image.Exif;
				}
				else
				{
					image.LoadFormat = Image.Jpeg;
				}
				Frame frame = image.AddFrame();

				// Read the scanlines from the image.
				int posn, width, offset, stride, y, twidth;
				width = frame.Width;
				twidth = width * 3;
				stride = frame.Stride;
				byte[] data = frame.Data;
				IntPtr buf = Marshal.AllocHGlobal(width * 3);
				byte *pbuf = (byte *)buf;
				y = 0;
				while(((int)(cinfo.output_scanline)) <
							((int)(cinfo.output_height)))
				{
					JpegLib.jpeg_read_scanlines
						(ref cinfo, ref buf, (UInt)1);
					offset = (y++) * stride;
					for(posn = 0; posn < twidth; posn += 3)
					{
						// Convert the JPEG RGB data into BGR for the frame.
						data[offset]     = pbuf[posn + 2];
						data[offset + 1] = pbuf[posn + 1];
						data[offset + 2] = pbuf[posn];
						offset += 3;
					}
				}
				Marshal.FreeHGlobal(buf);

				// Finish the decompression process.
				JpegLib.jpeg_finish_decompress(ref cinfo);

				// Clean everything up.
				JpegLib.FreeSourceManager(ref cinfo);
				JpegLib.jpeg_destroy_decompress(ref cinfo);
				JpegLib.FreeErrorHandler(cinfo.err);
			}

}; // class JpegReader

}; // namespace DotGNU.Images
