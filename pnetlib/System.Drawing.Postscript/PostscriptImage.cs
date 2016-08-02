/*
 * PostscriptImage.cs - Implementation of images for System.Drawing.
 *
 * Copyright (C) 2003  Free Software Foundation, Inc.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace System.Drawing.Toolkit
{

using System;
using DotGNU.Images;

public class PostscriptImage : ToolkitImageBase
{
	// Internal state.
	private String imageDataDict;
	private String imageData;
	private String maskDataDict;
	private String maskDataStream;

	// Constructor.
	public PostscriptImage(DotGNU.Images.Image image, int frame)
			: base(image, frame)
			{
				ImageChanged();
			}

	// Returns the LanguageLevel2 postscript for this image.
	public String LevelTwo
			{
				get
				{
					if(imageDataDict == null) { return String.Empty; }
					return String.Format
						("/DeviceRGB setcolorspace " +
						 "1 dict begin {0} {1} end ",
						 imageDataDict, imageData);
				}
			}

	// Returns the LanguageLevel3 postscript for this image.
	public String LevelThree
			{
				get
				{
					if(imageDataDict == null) { return String.Empty; }
					return String.Format
						("/DeviceRGB setcolorspace " +
						 "4 dict begin " +
						 "{0} {1} {2} " +
						 "/ImageDict 4 dict def " +
						 "ImageDict begin " +
						 "/ImageType 3 def " +
						 "/DataDict ImageDataDict def " +
						 "/MaskDict MaskDataDict def " +
						 "/InterleaveType 3 def " +
						 "end " +
						 "ImageDict image " +
						 "{4} ",
						maskDataStream, maskDataDict, imageDataDict, imageData);
				}
			}

	public override void ImageChanged()
			{
				Frame f = null;
				if(image == null || (f = image.GetFrame(frame)) == null)
				{
					imageDataDict = null;
					imageData = null;
					maskDataDict = null;
					maskDataStream = null;
				}

				imageDataDict = String.Format
					("/ImageDataDict 8 dict def " +
					 "ImageDataDict begin " +
					 "/ImageType 1 def " +
					 "/Width {0} def " +
					 "/Height {1} def " +
					 "/ImageMatrix [1 0 0 -1 0 {1}] def " +
					 "/DataSource currentfile /ASCII85Decode filter def " +
					 "/BitsPerComponent 8 def " +
					 "/Decode [0 1 0 1 0 1] def " +
					 "/Interpolation true def " +
					 "end ",
					 f.Width, f.Height);
				maskDataDict = String.Format
					("/MaskDataDict 8 dict def " +
					 "MaskDataDict begin " +
					 "/ImageType 1 def " +
					 "/Width {0} def " +
					 "/Height {1} def " +
					 "/ImageMatrix [1 0 0 -1 0 {1}] def " +
					 "/DataSource maskstream def " +
					 "/BitsPerComponent 1 def " +
					 "/Decode [0 1] def " +
					 "/Interpolation true def " +
					 "end ",
					 f.Width, f.Height);

				CompileFrameData(f);
			}

	// Dispose of this image.
	protected override void Dispose(bool disposing)
			{
				imageDataDict = null;
				imageData = null;
				maskDataDict = null;
				maskDataStream = null;
			}

	private void CompileFrameData(Frame f)
			{
				int width = f.Width;
				int height = f.Height;
				int stride = f.Stride;
				int fMaskStride = f.MaskStride;
				int transparentPixel = f.TransparentPixel;
				int[] fPalette = f.Palette;
				byte[] fData = f.Data;
				byte[] fMask = f.Mask;
				PixelFormat pixelFormat = f.PixelFormat;

				int maskStride = (width + 7) / 8;
				byte[] mask = new byte[height*maskStride];
				if(fMask != null)
				{
					// the mask in the frame is padded to 4-bytes, while the
					// masks in postscript are padded to 1-byte, so perform a
					// conversion if necessary, but direct copy if possible
					if(maskStride == fMaskStride)
					{
						Array.Copy(fMask, 0, mask, 0, mask.Length);
					}
					else
					{
						int i = 0;
						int j = 0;
						int k = 0;
						while(i < height)
						{
							Array.Copy(fMask, j, mask, k, maskStride);
							++i;
							j += fMaskStride;
							k += maskStride;
						}
					}
				}
				byte[] data = new byte[width*3*height];
				int offset = 0;
				int maskOffset = 0;
				for(int y = 0, i = 0; y < height; ++y)
				{
					for(int x = 0, ptr = offset; x < width; ++x)
					{
						switch(pixelFormat)
						{
							case PixelFormat.Format64bppPArgb:
							{
								byte a = fData[ptr+7];
								if(a != 0)
								{
									data[i+2] = (byte)((fData[ptr+1] * 255) / a);
									data[i+1] = (byte)((fData[ptr+3] * 255) / a);
									data[i] = (byte)((fData[ptr+5] * 255) / a);
								}
								i += 3;
								ptr += 8;
							}
							break;

							case PixelFormat.Format64bppArgb:
							{
								data[i+2] = fData[ptr+1];
								data[i+1] = fData[ptr+3];
								data[i] = fData[ptr+5];
								i += 3;
								ptr += 8;
							}
							break;

							case PixelFormat.Format48bppRgb:
							{
								data[i+2] = fData[ptr+1];
								data[i+1] = fData[ptr+3];
								data[i] = fData[ptr+5];
								i += 3;
								ptr += 6;
							}
							break;

							case PixelFormat.Format32bppPArgb:
							{
								byte a = fData[ptr+3];
								if(a != 0)
								{
									data[i+2] = (byte)((fData[ptr] * 255) / a);
									data[i+1] = (byte)((fData[ptr+1] * 255) / a);
									data[i] = (byte)((fData[ptr+2] * 255) / a);
								}
								i += 3;
								ptr += 4;
							}
							break;

							case PixelFormat.Format32bppArgb:
							{
								data[i+2] = fData[ptr++];
								data[i+1] = fData[ptr++];
								data[i] = fData[ptr++];
								i += 3;
								++ptr;
							}
							break;

							case PixelFormat.Format24bppRgb:
							{
								data[i+2] = fData[ptr++];
								data[i+1] = fData[ptr++];
								data[i] = fData[ptr++];
								i += 3;
							}
							break;

							case PixelFormat.Format16bppRgb565:
							{
								int g = fData[ptr++];
								int r = fData[ptr++];
								int b = (g & 0x1F) * 255 / 31;
								g = (r << 3 & 0x38  | g >> 5 & 0x07) * 255 / 63;
								r =  (r >> 3) * 255 / 31;
								data[i++] = (byte)r;
								data[i++] = (byte)g;
								data[i++] = (byte)b;
							}
							break;

							case PixelFormat.Format16bppRgb555:
							{
								int g = fData[ptr++];
								int r = fData[ptr++];
								int b = (g & 0x1F) * 255 / 31;
								g =( r << 3 & 0x18  | g >> 5 & 0x07) * 255 / 31;
								r = ( r >> 2 & 0x1F) * 255 / 31;
								data[i++] = (byte)r;
								data[i++] = (byte)g;
								data[i++] = (byte)b;
							}
							break;

							case (PixelFormat.Format16bppGrayScale):
							{
								++ptr;
								byte all = data[ptr++];
								data[i++] = (byte)all;
								data[i++] = (byte)all;
								data[i++] = (byte)all;
							}
							break;

							case PixelFormat.Format8bppIndexed:
							{
								int idx = fData[ptr++];
								int color = fPalette[idx];
								data[i++] = (byte)(color >> 16);
								data[i++] = (byte)(color >> 8);
								data[i++] = (byte)(color);
								if(transparentPixel == idx)
								{
									int mptr = maskOffset + x/8;
									mask[mptr] |= (byte)(1 << (7 - (x & 0x07)));
								}
							}
							break;

							case PixelFormat.Format4bppIndexed:
							{
								int idx = fData[ptr++] >> 4;
								int color = fPalette[idx];
								data[i++] = (byte)(color >> 16);
								data[i++] = (byte)(color >> 8);
								data[i++] = (byte)(color);
								if(transparentPixel == idx)
								{
									int mptr = maskOffset + x/8;
									mask[mptr] |= (byte)(1 << (7 - (x & 0x07)));
								}
								++x;
								if(x < width)
								{
									idx = fData[ptr++] & 0x0F;
									color = fPalette[idx];
									data[i++] = (byte)(color >> 16);
									data[i++] = (byte)(color >> 8);
									data[i++] = (byte)(color);
									if(transparentPixel == idx)
									{
										int mptr = maskOffset + x/8;
										mask[mptr] |= (byte)(1 << (7 - (x & 0x07)));
									}
								}
							}
							break;

							case PixelFormat.Format1bppIndexed:
							{
								byte r0 = (byte)(fPalette[0] >> 16);
								byte g0 = (byte)(fPalette[0] >> 8);
								byte b0 = (byte)(fPalette[0]);
								byte r1 = (byte)(fPalette[1] >> 16);
								byte g1 = (byte)(fPalette[1] >> 8);
								byte b1 = (byte)(fPalette[1]);
								byte val = fData[ptr++];
								byte m = 0x80;
								int j = 0;
								int limit = ((width - x) < 8) ? (width - x) : 8;
								int mptr = ptr-1;
								while(j < limit)
								{
									m = (byte)(m >> j);
									if((val & m) == 0)
									{
										data[i++] = r0;
										data[i++] = g0;
										data[i++] = b0;
										if(transparentPixel == 0)
										{
											mask[mptr] |= (byte)(1 << j);
										}
									}
									else
									{
										data[i++] = r1;
										data[i++] = g1;
										data[i++] = b1;
										if(transparentPixel == 1)
										{
											mask[mptr] |= (byte)(1 << j);
										}
									}
									++j;
								}
								x += j;
							}
							break;

							default:
							{
								imageDataDict = null;
								imageData = null;
								maskDataDict = null;
								maskDataStream = null;
								return;
							}
							// Not reached.
						}
						offset += stride;
						maskOffset += maskStride;
					}
				}
				imageData = PostscriptGraphics.ASCII85Encode(data);
				maskDataStream = String.Format
					("currentfile /ASCII85Decode filter " +
					 "/ReusableStreamDecode filter " +
					 "{0} /maskstream exch def ",
					 PostscriptGraphics.ASCII85Encode(mask));
			}

}; // class PostscriptImage

}; // namespace System.Drawing.Toolkit
