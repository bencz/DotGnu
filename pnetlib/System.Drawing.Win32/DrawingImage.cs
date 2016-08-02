/*
 * DrawingImage.cs - Implementation of DrawingImage for System.Drawing.Win32.
 * Copyright (C) 2003  Neil Cawse.
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
using System.Runtime.InteropServices;
using DotGNU.Images;

public class DrawingImage : ToolkitImageBase
{
	private Frame imageFrame;

	public DrawingImage(DotGNU.Images.Image image, int frame) : base(image, frame)
	{
		ImageChanged();
	}

	public override void ImageChanged()
	{
		imageFrame = image.GetFrame(frame);
	}

	// Draw this image onto the hdc at position x, y respecting the mask.
	public void Draw(IntPtr hdc, int x, int y)
	{
		// Set up the bitmap hdc.
		IntPtr srcHdc = Win32.Api.CreateCompatibleDC(hdc);
		byte[] dibInfo = GetBitmapInfo(imageFrame.PixelFormat, imageFrame.Width, imageFrame.Height, imageFrame.Palette);
		// Create the device dependant bitmap from the device independant bitmap.
		IntPtr hBitmap = Win32.Api.CreateDIBitmap(hdc, dibInfo, Win32.Api.CBM_INIT, imageFrame.Data, dibInfo, (uint)Win32.Api.DibColorTableType.DIB_RGB_COLORS);
		IntPtr oldBitmap = Win32.Api.SelectObject(srcHdc, hBitmap);

		// Now setup the mask hdc.
		IntPtr srcMaskHdc = Win32.Api.CreateCompatibleDC(hdc);
		int[] maskPalette = new int[] {unchecked((int)0xffffffff), 0};
		byte[] dibMaskInfo = GetBitmapInfo(PixelFormat.Format1bppIndexed, imageFrame.Width, imageFrame.Height, maskPalette);
		// Create the device dependant mask bitmap from the device independant bitmap.
		IntPtr hMaskBitmap = Win32.Api.CreateDIBitmap(hdc, dibMaskInfo, Win32.Api.CBM_INIT, imageFrame.Mask, dibMaskInfo, (uint)Win32.Api.DibColorTableType.DIB_RGB_COLORS);
		IntPtr oldMaskBitmap = Win32.Api.SelectObject(srcMaskHdc, hMaskBitmap);

		// Take into account the mask and write the bitmap to the control hdc.
		// Make all bits in the src black if they are not masked.
		Win32.Api.BitBlt(srcHdc, 0, 0, imageFrame.Width, imageFrame.Height, srcMaskHdc, 0, 0, Win32.Api.RopType.SRCINVERT);
		// And the dest into the mask so the bits in the mask that are not black have the dest bits. 
		Win32.Api.BitBlt(srcMaskHdc, 0, 0, imageFrame.Width, imageFrame.Height, hdc, x, y, Win32.Api.RopType.SRCAND);
		// Combine the src and the mask bitmaps.
		Win32.Api.BitBlt(srcMaskHdc, 0, 0, imageFrame.Width, imageFrame.Height, srcHdc, 0, 0, Win32.Api.RopType.SRCPAINT);
		// Write to the control.
		Win32.Api.BitBlt(hdc, x, y, imageFrame.Width, imageFrame.Height, srcMaskHdc, 0, 0, Win32.Api.RopType.SRCCOPY);

		// Cleanup
		Win32.Api.SelectObject(srcMaskHdc, oldMaskBitmap);
		Win32.Api.DeleteObject(hMaskBitmap);
		Win32.Api.DeleteDC(srcMaskHdc);
		Win32.Api.SelectObject(srcHdc, oldBitmap);
		Win32.Api.DeleteObject(hBitmap);
		Win32.Api.DeleteDC(srcHdc);

	}

	public IntPtr BrushFromBitmap(IntPtr hdc)
	{
		byte[] bitmapInfo = DrawingImage.GetBitmapInfo(imageFrame.PixelFormat, imageFrame.Width, imageFrame.Height, imageFrame.Palette);
		IntPtr hBitmap = Win32.Api.CreateCompatibleBitmap(hdc, imageFrame.Width, imageFrame.Height);
		Win32.Api.SetDIBits(hdc, hBitmap, 0, (uint)imageFrame.Height, ref imageFrame.Data[0], bitmapInfo, Win32.Api.DibColorTableType.DIB_RGB_COLORS);
		IntPtr hBrush = Win32.Api.CreatePatternBrush(hBitmap);
		// We dont need the bitmap anymore.
		Win32.Api.DeleteObject(hBitmap);
		return hBrush;
	}

	public static void DrawGlyph(IntPtr hdc, int x, int y, byte[] xbmBits, int bitsWidth, int bitsHeight, System.Drawing.Color color)
	{
		// Convert the xbm data to the bmp format.
		int[] palette = new int[] {0, color.ToArgb()};
		int xbmBytesPerRow = (bitsWidth + 7) / 8;
		int bmpStride = (xbmBytesPerRow + 3) & ~3;
		// Setup the array to hold the converted data
		byte[] bits = new byte[bmpStride * bitsHeight];

		int bmpStartStride = 0;
		int xbmStartStride = 0;
		for (int y1 = 0; y1 < bitsHeight; y1++)
		{
			byte bmpBit = 0x80;
			byte bmpByte = 0x00;
			int bmpPos = bmpStartStride;
		
			for (int x1 = 0; x1 < bitsWidth; x1++)
			{
				// Get the xbm bit.
				byte xbmByte = xbmBits[xbmStartStride + x1 / 8];
				bool bit = (xbmByte & (1<< (x1 & 0x07))) > 0;
				// Set the bmp byte.
				if (bit)
				{
					bmpByte |= bmpBit;
				}
				bmpBit = (byte)(bmpBit >> 1);
				// Update when we are on a byte boundary.
				if (bmpBit == 0 || x1 == bitsWidth -1)
				{
					bits[bmpPos++] = bmpByte;
					bmpBit = 0x80;
					bmpByte = 0x00;
				}
			}
			bmpStartStride += bmpStride;
			xbmStartStride += xbmBytesPerRow;
		}

		// Set up the bitmap hdc.
		IntPtr srcHdc = Win32.Api.CreateCompatibleDC(hdc);
		byte[] dibInfo = GetBitmapInfo(PixelFormat.Format1bppIndexed, bitsWidth, bitsHeight, palette);
		// Create the device dependant bitmap from the device independant bitmap.
		IntPtr hBitmap = Win32.Api.CreateDIBitmap(hdc, dibInfo, Win32.Api.CBM_INIT, bits, dibInfo, (uint)Win32.Api.DibColorTableType.DIB_RGB_COLORS);
		IntPtr oldBitmap = Win32.Api.SelectObject(srcHdc, hBitmap);

		// Now setup the mask hdc.
		IntPtr srcMaskHdc = Win32.Api.CreateCompatibleDC(hdc);
		int[] maskPalette = new int[] {unchecked((int)0xffffffff), 0};
		byte[] dibMaskInfo = GetBitmapInfo(PixelFormat.Format1bppIndexed, bitsWidth, bitsHeight, maskPalette);
		// Create the device dependant mask bitmap from the device independant bitmap.
		IntPtr hMaskBitmap = Win32.Api.CreateDIBitmap(hdc, dibMaskInfo, Win32.Api.CBM_INIT, bits, dibMaskInfo, (uint)Win32.Api.DibColorTableType.DIB_RGB_COLORS);
		IntPtr oldMaskBitmap = Win32.Api.SelectObject(srcMaskHdc, hMaskBitmap);

		// And the dest into the mask so the bits in the mask that are not black have the dest bits. 
		Win32.Api.BitBlt(srcMaskHdc, 0, 0, bitsWidth, bitsHeight, hdc, x, y, Win32.Api.RopType.SRCAND);
		// Combine the src and the mask bitmaps.
		Win32.Api.BitBlt(srcMaskHdc, 0, 0, bitsWidth, bitsHeight, srcHdc, 0, 0, Win32.Api.RopType.SRCPAINT);
		// Write to the control.
		Win32.Api.BitBlt(hdc, x, y, bitsWidth, bitsHeight, srcMaskHdc, 0, 0, Win32.Api.RopType.SRCCOPY);

		// Cleanup
		Win32.Api.SelectObject(srcMaskHdc, oldMaskBitmap);
		Win32.Api.DeleteObject(hMaskBitmap);
		Win32.Api.DeleteDC(srcMaskHdc);
		Win32.Api.SelectObject(srcHdc, oldBitmap);
		Win32.Api.DeleteObject(hBitmap);
		Win32.Api.DeleteDC(srcHdc);
	}

	// Create a the BITMAPINFO header for a bitmap.
	private static byte[] GetBitmapInfo(PixelFormat format, int width, int height, int[] palette)
	{
		// Set the size of the structure
		int size = 40;
		if (format == PixelFormat.Format16bppRgb565)
			size += 3 * 4;
		else if (format == PixelFormat.Format8bppIndexed)
			size += 256 * 4;
		else if (format == PixelFormat.Format4bppIndexed)
			size += 40 + 16 * 4;
		else if (format == PixelFormat.Format1bppIndexed)
			size += 40 + 2 * 4;
		byte[] bitmapInfo = new byte[size];
		WriteInt32(bitmapInfo, 0, 40); //biSize
		WriteInt32(bitmapInfo, 4, width); //biWidth
		WriteInt32(bitmapInfo, 8, -height); //biHeight
		WriteInt32(bitmapInfo, 12, 1); //biPlanes
		WriteInt32(bitmapInfo, 14, FormatToBitCount(format)); //biBitCount
		if (format == PixelFormat.Format16bppRgb565)
		{
			WriteInt32(bitmapInfo, 16, (int)Win32.Api.BitMapInfoCompressionType.BI_BITFIELDS);
			// Setup the masks for 565
			WriteInt32(bitmapInfo, 40, 0xF800); // R Mask
			WriteInt32(bitmapInfo, 44, 0x07E0); // G Mask
			WriteInt32(bitmapInfo, 48, 0x001F); // B Mask
		}
		else
			WriteInt32(bitmapInfo, 16, (int)Win32.Api.BitMapInfoCompressionType.BI_RGB); // biCompression

		WriteInt32(bitmapInfo, 20, 0); // biSizeImage
		WriteInt32(bitmapInfo, 24, 3780); // biXPelsPerMeter
		WriteInt32(bitmapInfo, 28, 3780); // biYPelsPerMeter
		WriteInt32(bitmapInfo, 32, 0); // biClrUsed
		WriteInt32(bitmapInfo, 36, 0); // biClrImportant
		//Setup palette
		if (palette != null)
		{
			// Write in RGBQUADS
			for (int i = 0; i < palette.Length; i++)
				WriteBGR(bitmapInfo, 40 + i * 4, palette[i]);
		}
		return bitmapInfo;
	}

	// Convert a pixel format into a bit count value.
	private static short FormatToBitCount(PixelFormat pixelFormat)
	{
		switch(pixelFormat)
		{
			case PixelFormat.Format1bppIndexed:
				return 1;

			case PixelFormat.Format4bppIndexed:
				return 4;

			case PixelFormat.Format8bppIndexed:
				return 8;

			case PixelFormat.Format16bppRgb555:
			case PixelFormat.Format16bppRgb565:
			case PixelFormat.Format16bppArgb1555:
			case PixelFormat.Format16bppGrayScale:
				return 16;

			case PixelFormat.Format24bppRgb:
				return 24;

			case PixelFormat.Format32bppRgb:
			case PixelFormat.Format32bppPArgb:
			case PixelFormat.Format32bppArgb:
				return 32;

			case PixelFormat.Format48bppRgb:
				return 48;

			case PixelFormat.Format64bppPArgb:
			case PixelFormat.Format64bppArgb:
				return 64;

			default:
				return 32;
		}
	}

	// Write a BGR value to a buffer as an RGBQUAD.
	private static void WriteBGR(byte[] buffer, int offset, int value)
	{
		buffer[offset] = (byte)value;
		buffer[offset + 1] = (byte)(value >> 8);
		buffer[offset + 2] = (byte)(value >> 16);
		buffer[offset + 3] = (byte)0;
	}

		// Write a little-endian 16-bit integer value to a buffer.
	private static void WriteUInt16(byte[] buffer, int offset, int value)
	{
		buffer[offset] = (byte)value;
		buffer[offset + 1] = (byte)(value >> 8);
	}

	// Write a little-endian 32-bit integer value to a buffer.
	private static void WriteInt32(byte[] buffer, int offset, int value)
	{
		buffer[offset] = (byte)value;
		buffer[offset + 1] = (byte)(value >> 8);
		buffer[offset + 2] = (byte)(value >> 16);
		buffer[offset + 3] = (byte)(value >> 24);
	}

	protected override void Dispose(bool disposing)
	{
	}

}
}
