/*
 * Frame.cs - Implementation of the "DotGNU.Images.Frame" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2003  Neil Cawse.
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
using System.Runtime.InteropServices;

public class Frame : MarshalByRefObject, IDisposable
{
	// Internal state.
	private Image image;
	internal int width;
	internal int height;
	internal int stride;
	private int maskStride;
	internal PixelFormat pixelFormat;
	private int[] palette;
	private int transparentPixel;
	private int hotspotX;
	private int hotspotY;
	private int offsetX;
	private int offsetY;
	internal byte[] data;
	internal byte[] mask;
	private bool generatedMask;

	// Constructor.
	internal Frame(Image image, int width, int height, PixelFormat pixelFormat)
			{
				this.image = image;
				this.width = width;
				this.height = height;
				this.stride = Utils.FormatToStride(pixelFormat, width);
				this.maskStride = (((width + 7) / 8) + 3) & ~3;
				this.pixelFormat = pixelFormat;
				this.palette = null;
				this.transparentPixel = -1;
				this.hotspotX = 0;
				this.hotspotY = 0;
				this.offsetX = 0;
				this.offsetY = 0;
				this.data = new byte [height * stride];
				this.mask = null;
				this.generatedMask = false;
			}
	private Frame(Image newImage, Frame frame, int newWidth, int newHeight, PixelFormat format, bool cloneData)
			{
				// Clone from the other frame.
				image = newImage;
				width = newWidth;
				height = newHeight;
				pixelFormat = format;
				stride =Utils.FormatToStride(pixelFormat, width);
				maskStride = (((width + 7) / 8) + 3) & ~3;
				generatedMask = false;
				if(frame.palette != null)
				{
					if(newImage != null && frame.palette == frame.image.Palette)
					{
						// The palette is a copy of the image's.
						palette = newImage.Palette;
					}
					else if (cloneData)
					{
						// The palette is specific to this frame.
						palette = (int[])(frame.palette.Clone());
					}
				}
				transparentPixel = frame.transparentPixel;
				hotspotX = frame.hotspotX;
				hotspotY = frame.hotspotY;
				offsetX = frame.offsetX;
				offsetY = frame.offsetY;
				if(cloneData & frame.data != null)
				{
					data = (byte[])(frame.data.Clone());
				}
				if(cloneData & frame.mask != null)
				{
					mask = (byte[])(frame.mask.Clone());
					generatedMask = frame.generatedMask;
				}
			}


	// Destructor.
	~Frame()
			{
				Dispose(false);
			}

	// Get the frame's properties.
	public int Width
			{
				get
				{
					return width;
				}
			}
	public int Height
			{
				get
				{
					return height;
				}
			}
	public int Stride
			{
				get
				{
					return stride;
				}
			}
	public int MaskStride
			{
				get
				{
					return maskStride;
				}
			}
	public PixelFormat PixelFormat
			{
				get
				{
					return pixelFormat;
				}
			}
	public byte[] Data
			{
				get
				{
					return data;
				}
			}
	public byte[] Mask
			{
				get
				{
					if(mask == null && transparentPixel != -1)
					{
						// Generate the mask from the pixel information.
						GenerateTransparencyMask();
					}
					else if(mask == null && (pixelFormat & PixelFormat.Alpha) !=0)
					{
						GenerateAlphaTransparencyMask();
					}
					return mask;
				}
			}
	public int[] Palette
			{
				get
				{
					// The palette for indexed images, null if an RGB image.
					return palette;
				}
				set
				{
					palette = value;
				}
			}
	public int TransparentPixel
			{
				get
				{
					// Index into "Palette" of the transparent pixel value.
					// Returns -1 if there is no transparent pixel specified.
					return transparentPixel;
				}
				set
				{
					transparentPixel = value;
				}
			}
	public int HotspotX
			{
				get
				{
					return hotspotX;
				}
				set
				{
					hotspotX = value;
				}
			}
	public int HotspotY
			{
				get
				{
					return hotspotY;
				}
				set
				{
					hotspotY = value;
				}
			}
	public int OffsetX
			{
				get
				{
					return offsetX;
				}
				set
				{
					offsetX = value;
				}
			}
	public int OffsetY
			{
				get
				{
					return offsetY;
				}
				set
				{
					offsetY = value;
				}
			}

	// Add a mask to this frame.
	public void AddMask()
			{
				if(mask == null)
				{
					mask = new byte[height * maskStride];
				}
			}

	// Clone this frame into a new image.
	internal Frame CloneFrame(Image newImage)
			{
				return new Frame(newImage, this, width, height, pixelFormat, true);
			}
	
	// Clone a frame without the data.
	internal Frame CloneFrameEmpty(int newWidth, int newHeight, PixelFormat format)
			{
				return new Frame(this.image, this, newWidth, newHeight, format, false);
			}

	internal void NewImage(Image newImage)
			{
				image = newImage;
			}

	// Dispose of this object.
	public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
	protected virtual void Dispose(bool disposing)
			{
				data = null;
				mask = null;
			}

	// Get the pixel value at a specific location.
	public int GetPixel(int x, int y)
			{
				int ptr = y * stride;
				switch (pixelFormat)
				{
					case (PixelFormat.Format64bppPArgb):
					{
						ptr += 8 * x;
						byte a = data[ptr+7];
						if(a == 0) { return 0; }
						byte b = (byte)((data[ptr+1] * 255) / a);
						byte g = (byte)((data[ptr+3] * 255) / a);
						byte r = (byte)((data[ptr+5] * 255) / a);
						return b | g << 8 | r << 16;
					}
					case (PixelFormat.Format64bppArgb):
					{
						ptr += 8 * x;
						return data[ptr+1] | data[ptr+3] << 8 | data[ptr+5] << 16;
					}
					case (PixelFormat.Format48bppRgb):
					{
						ptr += 6 * x;
						return data[ptr+1] | data[ptr+3] << 8 | data[ptr+5] << 16;
					}
					case (PixelFormat.Format32bppPArgb):
					{
						ptr += 4 * x;
						byte a = data[ptr+3];
						if(a == 0) { return 0; }
						byte b = (byte)((data[ptr] * 255) / a);
						byte g = (byte)((data[ptr+1] * 255) / a);
						byte r = (byte)((data[ptr+2] * 255) / a);
						return b | g << 8 | r << 16;
					}
					case (PixelFormat.Format32bppArgb):
					{
						ptr += 4 * x;
						return data[ptr++] | data[ptr++] << 8 | data[ptr++] << 16;
					}
					case (PixelFormat.Format32bppRgb):
					{
						ptr += 4 * x;
						return data[ptr] | data[ptr+1] << 8 | data[ptr+2] << 16;
					}
					case (PixelFormat.Format24bppRgb):
					{
						ptr += 3 * x;
						return data[ptr++] | data[ptr++] << 8 | data[ptr++] << 16;
					}
					case (PixelFormat.Format16bppRgb565):
					{
						ptr += 2 * x;
						int g = data[ptr++];
						int r = data[ptr++];
						int b = (g & 0x1F) * 255 / 31;
						g = (r << 3 & 0x38  | g >> 5 & 0x07) * 255 / 63;
						r =  (r >> 3) * 255 / 31;
						return r | g << 8 | b << 16;
					}
					case (PixelFormat.Format16bppRgb555):
					{
						ptr += 2 * x;
						int g = data[ptr++];
						int r = data[ptr++];
						int b = (g & 0x1F) * 255 / 31;
						g =( r << 3 & 0x18  | g >> 5 & 0x07) * 255 / 31;
						r = ( r >> 2 & 0x1F) * 255 / 31;
						return r | g << 8 | b << 16;
					}
					case (PixelFormat.Format16bppGrayScale):
					{
						ptr += 2 * x;
						byte all = data[ptr+1];
						return all | all << 8 | all << 16;
					}
					case (PixelFormat.Format8bppIndexed):
					{
						return palette[data[ptr]];
					}
					case (PixelFormat.Format4bppIndexed):
					{
						ptr += x/2;
						if ((x & 0x01) == 0)
							return palette[data[ptr]>>4];
						else
							return palette[data[ptr] & 0x0F];
					}
					case (PixelFormat.Format1bppIndexed):
					{
						ptr += x/8;
						if ((data[ptr] &(1<<7 - (x & 0x07)))== 0)
							return palette[0];
						else
							return palette[1];
					}
					default:
					{
						throw new NotSupportedException(pixelFormat.ToString());
					}
				}
			}

	// Get the mask value at a specific location.
	public int GetMask(int x, int y)
			{
				int ptr = y * maskStride + x/8;
				if ((mask[ptr] &(1<<7 - (x & 0x07)))== 0)
					return 0;
				else
					return 1;
			}

	// Get the contents of a scan line (the buffer must be
	// at least "Stride" bytes in length).
	public void GetScanLine(int line, byte[] buffer)
			{
				if(line >= 0 && line < height && data != null)
				{
					Array.Copy(data, line * stride, buffer, 0, stride);
				}
			}
	public void GetScanLine(int line, IntPtr buffer)
			{
				if(line >= 0 && line < height && data != null &&
				   buffer != IntPtr.Zero)
				{
					Marshal.Copy(data, line * stride, buffer, stride);
				}
			}

	// Get the contents of a mask line (the buffer must be
	// at least "MaskStride" bytes in length).
	public void GetMaskLine(int line, byte[] buffer)
			{
				if(line >= 0 && line < height && mask != null)
				{
					Array.Copy(mask, line * maskStride, buffer, 0, maskStride);
				}
			}
	public void GetMaskLine(int line, IntPtr buffer)
			{
				if(line >= 0 && line < height && mask != null &&
				   buffer != IntPtr.Zero)
				{
					Marshal.Copy(mask, line * maskStride, buffer, maskStride);
				}
			}

	// Rotate or flip the contents of this frame.
	public void RotateFlip(RotateFlipType rotateFlipType)
			{
				// TODO
			}

	// Get a re-scaled version of this frame.
	public Frame Scale(int newWidth, int newHeight)
			{
				if ((newWidth == width) && (newHeight == height))
					return CloneFrame (image);

				return BmpResizer.Resize(this, 0, 0, Width, Height, newWidth, newHeight);
			}

	// Set the pixel value at a specific location.
	public void SetPixel(int x, int y, int color)
			{
				int ptr = y * stride;
				switch (pixelFormat)
				{
					case (PixelFormat.Format24bppRgb):
						ptr += 3 * x;
						data[ptr++] = (byte)color;
						data[ptr++] = (byte)(color >> 8);
						data[ptr] = (byte)(color >> 16);
						break;
					case (PixelFormat.Format16bppRgb565):
						ptr += 2 * x;
						data[ptr++] = (byte)((color>>5 & 0xE0) | (color >> 3 & 0x1F)) ;
						data[ptr] = (byte)((color>>16 & 0xF8) | (color >>13 & 0x07));
						break;
					case (PixelFormat.Format16bppRgb555):
						ptr += 2 * x;
						data[ptr++] =  (byte)((color>>6 & 0xE0) | (color>>3 & 0x1F));
						data[ptr] =  (byte)((color>>17 & 0x7C) | (color >>14 & 0x03));
						break;
					case (PixelFormat.Format8bppIndexed):
						ptr += x;
						data[ptr] = (byte)Utils.BestPaletteColor(palette, color);
						break;
					case (PixelFormat.Format4bppIndexed):
						ptr += x/2;
						if ((x & 0x01) == 0)
							data[ptr] = (byte)(Utils.BestPaletteColor(palette, color)<< 4 | data[ptr] & 0x0F);
						else
							data[ptr] =(byte)(Utils.BestPaletteColor(palette, color) & 0x0F | data[ptr] & 0xF0);
						break;
					case (PixelFormat.Format1bppIndexed):
						ptr += x/8;
						if (Utils.BestPaletteColor(palette, color) == 0)
							data[ptr] &= (byte)(~(1<<7 - (x & 0x07)));
						else
							data[ptr] |= (byte)(1<<7 - (x & 0x07));
						break;
				}
			}

	// Set the mask value at a specific location.
	public void SetMask(int x, int y, int value)
			{
				int ptr = y * maskStride + x/8;
				if (value == 0)
					mask[ptr] &= (byte)(~(1<<7 - (x & 0x07)));
				else
					mask[ptr] |= (byte)(1<<7 - (x & 0x07));
			}

	// Set the contents of a scan line (the buffer must be
	// at least "Stride" bytes in length).
	public void SetScanLine(int line, byte[] buffer)
			{
				if(line >= 0 && line < height && data != null)
				{
					Array.Copy(buffer, 0, data, line * stride, stride);
				}
			}
	public void SetScanLine(int line, IntPtr buffer)
			{
				if(line >= 0 && line < height && data != null &&
				   buffer != IntPtr.Zero)
				{
					Marshal.Copy(buffer, data, line * stride, stride);
				}
			}

	// Set the contents of a mask line (the buffer must be
	// at least "MaskStride" bytes in length).
	public void SetMaskLine(int line, byte[] buffer)
			{
				if(line >= 0 && line < height && mask != null)
				{
					Array.Copy(buffer, 0, mask, line * maskStride, maskStride);
				}
			}

	public void SetMaskLine(int line, IntPtr buffer)
			{
				if(line >= 0 && line < height && mask != null &&
				   buffer != IntPtr.Zero)
				{
					Marshal.Copy(buffer, mask, line * maskStride, maskStride);
				}
			}

	// Create a new image from the rect (x, y, width, height) of this image.
	// The new image is in a parallelogram defined by 3 points - Top-Left, Top-Right
	// and Bottom-Left. The remaining point is inferred. The top left corner of the
	// bounding rectangle must be (0,0).
	public Frame AdjustImage(int originx1, int originy1, int originx2, int originy2,
		int originx3, int originy3, int destx1, int desty1, int destx2, int desty2,
		int destx3, int desty3)
			{
				// Return if there are no changes.
				if (originx1 == destx1 && originy1 == desty1 && originx2 == destx2 &&
					originy2 == desty2 && originx3 == destx3 && originy3 == desty3)
					return this.CloneFrame(null);
				// TODO
				if (originx1 != originx3 || originy1 != originy2 || desty2 != 0 || destx3 != 0)
					throw new NotSupportedException ("No shearing or rotation yet");
				if (destx1 != 0 || desty1 != 0 || desty2 != 0 || destx3 != 0)
					throw new NotSupportedException ("No shearing or rotation yet");

				// Calculate the inferred points.
				int orginx4 = originx3 - originx1 + originx2;
				int orginy4 = originy2 - originy1 + originy3;
				int destx4 = destx3 - destx1 + destx2;
				int desty4 = desty2 - desty1 + desty3;

				// Calculate the outside bounds.
				int boundsx = destx1;
				int boundsy = desty1;
				int boundsright = destx1;
				int boundsbottom = desty1;

				if (boundsright < destx2)
					boundsright = destx2;
				else
					boundsx = destx2;
				if (boundsbottom < desty2)
					boundsbottom = desty2;
				else
					boundsy = desty2;
				if (boundsright < destx3)
					boundsright = destx3;
				if (boundsx > destx3)
					boundsx = destx3;
				if (boundsbottom < desty3)
					boundsbottom = desty3;
				if (boundsy > desty3)
					boundsy = desty3;
				if (boundsright < destx4)
					boundsright = destx4;
				if (boundsx > destx4)
					boundsx = destx4;
				if (boundsbottom < desty4)
					boundsbottom = desty4;
				if (boundsy > desty4)
					boundsy = desty4;

				if (boundsx != 0 || boundsy != 0)
					throw new ArgumentException();

				Frame f;
				// Can we save time by just copying?
				if (originx2 - originx1 == destx2 - destx1 && originy2 - originy1 == desty2 - desty1)
				{
					f = CloneFrameEmpty(destx2 - destx1, desty3 - desty1, pixelFormat);
					f.data = new byte [f.height * f.stride];
					
					Copy(f, 0, 0, f.width, f.height, this, originx1, originy1);
				}
				else
				{
					f = BmpResizer.Resize(this, originx1, originy1 , originx2 - originx1, originy3 - originy1, destx2 - destx1, desty3 - desty1);
					// TODO: The shearing and rotating.
					//f = new Frame(newImage, boundsright - boundsx, boundsbottom - boundsy, pixelFormat);
				}
				return f;
			}

	// Change the PixelFormat of a particular frame.
	public Frame Reformat(PixelFormat format)
			{
				return BmpReformatter.Reformat(this, format);
			}

	// Copy as much of frame as possible to the x, y position of this frame.
	public void Copy(Frame frame, int x, int y)
			{
				Copy(this, x, y, width, height, frame, 0, 0);
			}

	public static void Copy(Frame dest, int x, int y, int width, int height, Frame source, int sourceX, int sourceY)
			{
				// Developers should be aware of any costly conversions.
				// So we don't automatically match the depths.
				if (source.pixelFormat != dest.pixelFormat)
				{
					throw new InvalidOperationException();
				}

				if (x < 0 || y < 0 || width <= 0 || height <= 0)
				{
					throw new ArgumentOutOfRangeException();
				}

				if (x + width > dest.width)
				{
					width = dest.width - x;
				}

				if (y + height > dest.height)
				{
					height = dest.height - x;
				}

				if (source.width - sourceX < width)
				{
					width = source.width - sourceX;
				}
				if (source.height - sourceY < height)
				{
					height = source.height - sourceY;
				}

				//TODO:
				// If we are copying an index bitmap, we need to find the color
				// in the destination palette that is closest to the color we are
				// copying to. We would also need to add colors to the palette,
				// if there is space and optionally optimize the palette.
				// For now we just overwrite the old palette.

				if (source.palette != null)
				{
					dest.palette = (int[])source.palette.Clone();
				}
								
				int bits = Utils.FormatToBitCount(source.pixelFormat);
				Copy (bits, dest.data, dest.stride, x, y, width, height, source.Data, source.stride, sourceX, sourceY);

				//TODO:
				// The mask is not taken into account when copying. We need to
				// look at adding alpha support.
				// For now, just copy over the mask.
				if (source.Mask != null)
				{
					dest.AddMask();
					Copy(1, dest.mask, dest.maskStride,  x, y, width, height, source.mask, source.maskStride, sourceX, sourceY);
				}

			}

	// Take a dest and source image with “bits” bits per pixel. Copy the source starting at (sourceX, sourceY) into a rectangle in the dest x, y, width, height.
	private static void Copy(int bits, byte[] destData, int destStride, int x, int y, int width, int height, byte[] sourceData, int sourceStride, int sourceX, int sourceY)
			{
				int sourceXOffset = sourceX * bits/8;
				int pSourceRow = sourceY * sourceStride + sourceXOffset;

				int destXOffset = x * bits / 8;
				int pDestinationRow = y * destStride + destXOffset;
				int lineLength = (width * bits + 7) / 8;
				
				if (bits >= 8)
				{
					for (int bottom = y + height; y < bottom ; y++)
					{
						Array.Copy(sourceData, pSourceRow, destData, pDestinationRow, lineLength);
						pSourceRow += sourceStride;
						pDestinationRow += destStride;
					}
				}
				else
				{
					int sourceOffset = (sourceX * bits) & 7;
					int destOffset = (x * bits) & 7;
					int sourceOffsetReverse = 8 - sourceOffset;
					for (int bottom = y + height; y < bottom ; y++)
					{
						if (sourceOffset == 0)
						{
							if (destOffset == 0)
							{
								Array.Copy(sourceData, pSourceRow, destData, pDestinationRow, lineLength);
							}
							else
							{
								int pSource = pSourceRow;
								int pDest = pDestinationRow;
								int prevDest = destData[pDest];
								for (int pSourceEnd = pSource + lineLength; pSource < pSourceEnd;)
								{
									prevDest = ((prevDest << 8) | sourceData[pSource++]);
									destData[pDest++] = (byte)(prevDest >> destOffset);
								}
							}
						}
						else if (destOffset == 0)
						{
							int pSource = pSourceRow;
							int pDest = pDestinationRow;
							int prevSource = sourceData[pSource++];
							for (int pSourceEnd = pSource + lineLength; pSource < pSourceEnd;)
							{
								prevSource = ((prevSource << 8) | sourceData[pSource++]);
								destData[pDest++] = (byte)(prevSource >> sourceOffsetReverse);
							}
						}
						else
						{
							int pSource = pSourceRow;
							int pDest = pDestinationRow;
							int prevSource = sourceData[pSource++];
							int prevDest = destData[pDest];
							for (int pSourceEnd = pSource + lineLength; pSource < pSourceEnd;)
							{
								prevSource = ((prevSource << 8) | sourceData[pSource++]);
								prevDest = ((prevDest << 8) | (byte)(prevSource >> sourceOffsetReverse));
								destData[pDest++] = (byte)(prevDest >> destOffset);
							}
						}
						pSourceRow += sourceStride;
						pDestinationRow += destStride;
					}
				}
			}

	// Generate a mask from a transparency color.
	private void GenerateTransparencyMask()
			{
				byte[] data = this.data;

				// Make sure that the frame is compatible with this operation.
				if(palette == null || transparentPixel < 0 ||
				   transparentPixel >= palette.Length ||
				   (pixelFormat & PixelFormat.Indexed) == 0 ||
				   data == null)
				{
					return;
				}

				// Add the mask bits to the frame.
				AddMask();
				byte[] mask = this.mask;

				// Process the image lines looking for the transparency color.
				int x, y, transparent, offset, maskOffset;
				int width = this.width;
				transparent = transparentPixel;
				for(y = 0; y < height; ++y)
				{
					offset = y * stride;
					maskOffset = y * maskStride;
					if(pixelFormat == PixelFormat.Format8bppIndexed)
					{
						for(x = 0; x < width; ++x)
						{
							if(data[offset + x] != transparent)
							{
								mask[maskOffset + (x >> 3)] |=
									(byte)(0x80 >> (x & 7));
							}
						}
					}
					else if(pixelFormat == PixelFormat.Format4bppIndexed)
					{
						for(x = 0; x < width; ++x)
						{
							if((x & 1) == 0)
							{
								if(((data[offset + (x >> 1)] & 0xF0) >> 4)
										!= transparent)
								{
									mask[maskOffset + (x >> 3)] |=
										(byte)(0x80 >> (x & 7));
								}
							}
							else
							{
								if((data[offset + (x >> 1)] & 0x0F)
										!= transparent)
								{
									mask[maskOffset + (x >> 3)] |=
										(byte)(0x80 >> (x & 7));
								}
							}
						}
					}
					else if(pixelFormat == PixelFormat.Format1bppIndexed)
					{
						if(transparent == 0)
						{
							for(x = 0; x < stride; ++x)
							{
								mask[maskOffset + x] = data[offset + x];
							}
						}
						else
						{
							for(x = 0; x < stride; ++x)
							{
								mask[maskOffset + x] =
									(byte)(~(data[offset + x]));
							}
						}
					}
				}
			}

	// TODO: Remove when we have true alpha support
	private void GenerateAlphaTransparencyMask()
			{
				byte[] data = this.data;
				// Make sure that the frame is compatible with this operation.
				if(palette != null || transparentPixel != -1 ||
				   (pixelFormat & PixelFormat.Alpha) == 0 ||
				   data == null)
				{
					return;
				}

				// Add the mask bits to the frame.
				AddMask();
				byte[] mask = this.mask;
				
				// Process the image lines looking for the alpha < 0xFF.
				int x, y, offset, maskOffset;
				int width = this.width;
				for(y = 0; y < height; ++y)
				{
					offset = y * stride;
					maskOffset = y * maskStride;
					for(x = 0; x < width; ++x)
					{
						int alpha = 0xFF; 
						switch(pixelFormat)
						{
							case PixelFormat.Format32bppArgb:
							{
								alpha = data[offset + 4 * x + 3] ; 
							}
							break;
							case PixelFormat.Format32bppPArgb:
							{
								alpha = data[offset + 4 * x + 3] ; 
							}
							break;
							case PixelFormat.Format64bppPArgb:
							{
								alpha = data[offset + 8 * x + 7];
							}
							break;
							case PixelFormat.Format64bppArgb:
							{
								alpha = data[offset + 8 * x];
							}
							break;
						}
						if(alpha != 0)
						{
							mask[maskOffset + (x >> 3)] |=
										(byte)(0x80 >> (x & 7));
						}
					}
				}
			}

	// Add pixels of a specific color to the mask to make them transparent.
	public void MakeTransparent(int color)
			{
				// If we already have a mask, or transparent pixel, then
				// we need to use that information rather than the color
				// that we were supplied.  This is for icon files where it
				// is highly unlikely that the supplied color will exactly
				// match the real transparency pixel in the icon image.
				if(mask != null && !generatedMask)
				{
					return;
				}
				if(transparentPixel >= 0 && palette != null &&
				   transparentPixel < palette.Length)
				{
					color = palette[transparentPixel];
					if((pixelFormat & PixelFormat.Alpha) == 0)
					{
						color &= 0x00FFFFFF;
					}
				}
				else
				{
					generatedMask = true;
				}

				// Make sure we have a mask.  TODO: make this faster.
				AddMask();
				for (int y = 0; y < height; y++)
				{
					for (int x = 0; x < width; x++)
					{
						int pixel = GetPixel(x, y);
						if (pixel == color)
							SetMask(x, y, 0);
						else 
							SetMask(x, y, 1);
					}
				}
			}

	public int BitsPerPixel
			{
				get
				{
					return Utils.FormatToBitCount(pixelFormat);
				}
			}

	public int BytesPerLine
			{
				get
				{
					return Utils.BytesPerLine(pixelFormat, width);
				}
			}

	public override string ToString()
	{
		return "Frame [" + width + "," + height +"], " + pixelFormat;
	}


}; // class Frame

}; // namespace DotGNU.Images
