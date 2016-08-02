/*
 * IconWriter.cs - Implementation of the "DotGNU.Images.IconWriter" class.
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

internal sealed class IconWriter
{
	// Save a Windows icon image to the specified stream.  If "hotspots"
	// is "true", then the image is actually a Windows cursor with hotspots.
	public static void Save(Stream stream, Image image, bool hotspots)
			{
				byte[] buffer = new byte [1024];
				int numImages = image.NumFrames;
				int offset, index, size, bitCount;
				int[] offsetList;
				int[] sizeList;
				Frame frame;

				// Write the image header.
				buffer[0] = 0;
				buffer[1] = 0;
				buffer[2] = (byte)(hotspots ? 2 : 1);
				buffer[3] = 0;
				Utils.WriteUInt16(buffer, 4, numImages);
				stream.Write(buffer, 0, 6);

				// Infer the starting offsets and sizes for each of the images.
				offset = 6 + numImages * 16;
				offsetList = new int [numImages];
				sizeList = new int [numImages];
				for(index = 0; index < numImages; ++index)
				{
					frame = image.GetFrame(index);
					size = 40;
					if((frame.pixelFormat & PixelFormat.Indexed) != 0)
					{
						size +=
						  4 * (1 << Utils.FormatToBitCount(frame.pixelFormat));
					}
					size += frame.Height * (frame.Stride + frame.MaskStride);
					offsetList[index] = offset;
					sizeList[index] = size;
					offset += size;
				}

				// Write the contents of the resource directory.
				for(index = 0; index < image.NumFrames; ++index)
				{
					frame = image.GetFrame(index);
					bitCount = Utils.FormatToBitCount(frame.pixelFormat);
					buffer[0] = (byte)(frame.Width);
					buffer[1] = (byte)(frame.Height);
					buffer[2] = (byte)(1 << bitCount);
					buffer[3] = 0;
					if(hotspots)
					{
						Utils.WriteUInt16(buffer, 4, frame.HotspotX);
						Utils.WriteUInt16(buffer, 6, frame.HotspotY);
					}
					else
					{
						Utils.WriteUInt16(buffer, 4, 0);
						Utils.WriteUInt16(buffer, 6, 0);
					}
					Utils.WriteInt32(buffer, 8, sizeList[index]);
					Utils.WriteInt32(buffer, 12, offsetList[index]);
					stream.Write(buffer, 0, 16);
				}

				// Write each of the images.
				for(index = 0; index < image.NumFrames; ++index)
				{
					frame = image.GetFrame(index);
					bitCount = Utils.FormatToBitCount(frame.pixelFormat);
					BmpWriter.SaveBitmapInfo
						(stream, frame, bitCount,
						 (frame.Stride + frame.MaskStride) * frame.Height,
						  buffer, frame.Height * 2);
					BmpWriter.SaveBitmapData(stream, frame, false, false);
					BmpWriter.SaveBitmapData(stream, frame, true, true);
				}
			}

}; // class IconWriter

}; // namespace DotGNU.Images
