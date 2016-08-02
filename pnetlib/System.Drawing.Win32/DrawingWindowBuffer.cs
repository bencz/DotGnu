/*
 * DrawingWindowBuffer.cs - Window Double Buffer class.
 * 
 * Copyright (C) 2004  Neil Cawse.
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

	public class DrawingWindowBuffer : IToolkitWindowBuffer, IDisposable
	{
		DrawingWindow window;
		IntPtr hBitmap;
		IntPtr hOldBitmap;
		IToolkitGraphics bufferGraphics;
		Size size;

		public DrawingWindowBuffer(IToolkitWindow windowToBuffer)
		{
			window = windowToBuffer as DrawingWindow;
			CreateBuffer(windowToBuffer.Dimensions.Size);
		}

		private void CreateBuffer(Size size)
		{
			this.size = size;
			hBitmap = Win32.Api.CreateCompatibleBitmap(window.hdc, size.Width, size.Height);
		}

		private void DeleteBuffer()
		{
			// Must select it out before deleting it - in case BeginDoubleBuffer is called twice in a row.
			Win32.Api.SelectObject(window.hdc, hOldBitmap);
			hOldBitmap = IntPtr.Zero;
			Win32.Api.DeleteObject(hBitmap);
				
		}

		public IToolkitGraphics BeginDoubleBuffer()
		{
			Size newSize = (window as IToolkitWindow).Dimensions.Size;
			// If the size changes, we need to recreate the buffer.
			if (size != newSize)
			{
				DeleteBuffer();
				CreateBuffer(newSize);
			}
			
			// Setup the hdc.
			IntPtr hBitmapDC = Win32.Api.CreateCompatibleDC(window.hdc);
			hOldBitmap = Win32.Api.SelectObject(hBitmapDC, hBitmap);

			// Create a DrawingGraphics from the hdc.
			bufferGraphics = new DrawingGraphics(window.Toolkit, hBitmapDC);
			return bufferGraphics;
		}

		public void EndDoubleBuffer()
		{
			DrawingGraphics g = bufferGraphics as DrawingGraphics;
			// Copy the buffer hdc to the window hdc.
			Win32.Api.BitBlt(window.hdc, 0, 0, size.Width, size.Height, g.hdc, 0, 0, Win32.Api.RopType.SRCCOPY);
			// Remove out the bitmap from the hdc.
			Win32.Api.SelectObject(g.hdc, hOldBitmap);
			hOldBitmap = IntPtr.Zero;
			bufferGraphics.Dispose();
			bufferGraphics = null;
		}

		public void Dispose()
		{
			Win32.Api.DeleteObject(hBitmap);
			hBitmap = IntPtr.Zero;
			if (bufferGraphics != null)
			{
				bufferGraphics.Dispose();
			}
		}

	}
}