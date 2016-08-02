/*
 * DrawingTopLevelWindow.cs - This is a windows form
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
using DotGNU.Images;

internal class DrawingTopLevelWindow : DrawingWindow, IToolkitTopLevelWindow
{

	protected static uint createCount;
	internal Size maximumSize;
	internal Size minimumSize;
	public DrawingTopLevelWindow(DrawingToolkit toolkit, String name,
		int width, int height, IToolkitEventSink sink) : base ( toolkit )
	{
		this.sink = sink;
		//Console.WriteLine("DrawingTopLevelWindow");
		dimensions = new Rectangle(0, 0, width, height);

		// At the moment we create a unique class name for EVERY window. SWF does it for each unique window class
		className = "DrawingTopLevelWindow" + createCount++;

		// Register the windows class
		windowsClass = new Win32.Api.WNDCLASS();
		windowsClass.style = Win32.Api.WindowClassStyle.CS_DBLCLKS;
		windowsClass.lpfnWndProc = new Win32.Api.WNDPROC(toolkit.WindowsLoop);
		// We will draw
		windowsClass.hbrBackground = IntPtr.Zero;
		windowsClass.lpszClassName = className ;
		if (Win32.Api.RegisterClassA( ref windowsClass)==0) 
		{
			throw new Exception("Failed to register Windows class " + className);
		}
			
		// Set default window characteristics
		flags = ToolkitWindowFlags.Menu | ToolkitWindowFlags.Close | ToolkitWindowFlags.Minimize | ToolkitWindowFlags.Maximize | ToolkitWindowFlags.Caption | ToolkitWindowFlags.ResizeHandles;
		toolkit.GetWin32StylesFromFlags( flags, out style, out extendedStyle);
		menu = false;
		extendedStyle = 0;
		topOfhierarchy = this;
	}

	void IToolkitTopLevelWindow.Iconify()
	{
		Win32.Api.ShowWindow(hwnd, Win32.Api.ShowWindowCommand.SW_MINIMIZE);
		//Console.WriteLine("DrawingWindow.Iconify, "+sink);
	}

	// Maximize the window.
	void IToolkitTopLevelWindow.Maximize()
	{
		Win32.Api.ShowWindow(hwnd, Win32.Api.ShowWindowCommand.SW_MAXIMIZE);
		//Console.WriteLine("DrawingWindow.Maximize, "+sink);
	}

	// Restore the window from its iconified or maximized state.
	void IToolkitTopLevelWindow.Restore()
	{
		Win32.Api.ShowWindow(hwnd, Win32.Api.ShowWindowCommand.SW_RESTORE);
		//Console.WriteLine("DrawingWindow.Restore, "+sink);
	}

	public void WindowStateChanged(int state)
	{
		sink.ToolkitStateChanged(state);
	}

	// Set the owner for modal and modeless dialog support.
	void IToolkitTopLevelWindow.SetDialogOwner(IToolkitTopLevelWindow owner)
	{
		// TODO
	}

	// Set this window's icon.
	// The small icon is the icon in the taskbar and in the window title.
	// The large icon is used when alt-tabing.
	void IToolkitTopLevelWindow.SetIcon(Icon icon)
	{
		// Get the first icon in the set that is 16x16.
		Icon smallIcon = new Icon(icon, new Size(16, 16));
		IntPtr smallIconHandle = HandleFromIcon(smallIcon);
		// Tell windows it has a new small icon.
		Win32.Api.SendMessageA( hwnd, Win32.Api.WindowsMessages.WM_SETICON, 0/*ICON_SMALL*/, smallIconHandle);
		Win32.Api.DeleteObject(smallIconHandle);

		// Get the large icon.
		Icon largeIcon = new Icon(icon, new Size(32, 32));
		IntPtr largeIconHandle = HandleFromIcon(largeIcon);
		// Tell windows it has a new large icon.
		Win32.Api.SendMessageA( hwnd, Win32.Api.WindowsMessages.WM_SETICON, 1/*ICON_BIG*/, largeIconHandle); 
		Win32.Api.DeleteObject(largeIconHandle);
		//Console.WriteLine("DrawingTopLevelWindow.SetIcon, "+sink);
	}

	// Create an HICON handle from an icon.
	// "DeleteObject" must be called on it when no longer needed.
	IntPtr HandleFromIcon(Icon icon)
	{
		Frame frame = ToolkitManager.GetImageFrame(icon);
		Win32.Api.ICONINFO iconInfo = new System.Drawing.Win32.Api.ICONINFO();
		iconInfo.fIcon = true;
		// Hotspot is always the middle for an icon.
		iconInfo.xHotspot = iconInfo.yHotspot = 0;

		// Create the mono bitmap mask using CreateBitmap rather than CreateDIBitmap, its faster.
		// CreateBitmap expects the mask data to be word aligned, not int aligned and the mask is inverted.
		int frameBytes = (frame.Width + 7) / 8;
		int newStride = (frameBytes + 1) & ~ 1;
		byte[] inverseMask = new byte[frame.Height * newStride];
		int prevOldPos = 0;
		int prevNewPos = 0;
		for (int y = 0; y < frame.Height; y++)
		{
			int oldPos = prevOldPos;
			int newPos = prevNewPos;
			for (int b = 0; b < frameBytes; b++)
				inverseMask[newPos++] = (byte)~frame.Mask[oldPos++];
			prevOldPos += frame.MaskStride;
			prevNewPos += newStride;
		}
		iconInfo.hbmMask = Win32.Api.CreateBitmap(frame.Width, frame.Height, 1, 1, inverseMask);

		iconInfo.hbmColor = HandleFromBitmap(frame, true);

		IntPtr iconHandle = Win32.Api.CreateIconIndirect(ref iconInfo);
		Win32.Api.DeleteObject(iconInfo.hbmMask);
		Win32.Api.DeleteObject(iconInfo.hbmColor);
		return iconHandle;
	}

	// Create a device independant bitmap from a frame. Optionally set all bits that are masked to black.
	// This is required for icons.
	private IntPtr HandleFromBitmap(Frame frame, bool andMask)
	{
		// By default we use the data straight from the frame.
		byte[] data = frame.Data;
		if (andMask)
		{
			//TODO: this could be slow.
			// Create a new image that we will copy the pixels to, leaving the masked pixels black.
			DotGNU.Images.Image newImage  = new DotGNU.Images.Image(frame.Width, frame.Height, frame.PixelFormat);
			Frame newFrame = newImage.AddFrame();
			data = new byte[data.Length];
			for (int y = 0; y < frame.Height; y++)
			{
				for (int x = 0; x < frame.Width; x++)
				{
					if (frame.GetMask(x, y) != 0)
						newFrame.SetPixel(x, y, frame.GetPixel(x, y));
				}
			}
			data = newFrame.Data;
		}

		// Create BITMAPINFO structure.
		int bitmapInfoSize = 40;
		int bitCount = frame.BitsPerPixel;
		// Do we have a palette?
		if(bitCount <= 8)
			bitmapInfoSize += 1 << bitCount * 4;
		byte[] bitmapInfo = new byte[bitmapInfoSize];

		// Build and write the BITMAPINFOHEADER structure.
		WriteInt32(bitmapInfo, 0, 40);// biSize
		WriteInt32(bitmapInfo, 4, frame.Width);
		WriteInt32(bitmapInfo, 8, -frame.Height);// upside down so make the height negative.
		WriteUInt16(bitmapInfo, 12, 1);// biPlanes
		WriteUInt16(bitmapInfo, 14, bitCount);
		WriteInt32(bitmapInfo, 16, 0);// biCompression
		WriteInt32(bitmapInfo, 20, 0);// size of image
		WriteInt32(bitmapInfo, 24, 3780);// biXPelsPerMeter
		WriteInt32(bitmapInfo, 28, 3780);// biYPelsPerMeter
		WriteInt32(bitmapInfo, 32, 0);	// biClrUsed
		WriteInt32(bitmapInfo, 36, 0);	// biClrImportant

		// Write the palette.
		if(bitCount <= 8)
		{
			int count = (1 << bitCount);
			for(int index = 0; index < count; ++index)
			{
				if(frame.Palette != null && index < frame.Palette.Length)
					WriteBGR(bitmapInfo, index * 4 + 40, frame.Palette[index]);
				else
				{
					// Short palette: pad with black pixels.
					WriteBGR(bitmapInfo, index * 4 + 40, 0);
				}
			}
		}

		return Win32.Api.CreateDIBitmap( Win32.Api.GetDC(hwnd), bitmapInfo, 4 /*CBM_INIT*/, data, bitmapInfo, 0 /*DIB_RGB_COLORS*/);
	}

	// Write a little-endian 16-bit integer value to a buffer.
	private void WriteUInt16(byte[] buffer, int offset, int value)
	{
		buffer[offset] = (byte)value;
		buffer[offset + 1] = (byte)(value >> 8);
	}

	// Write a little-endian 32-bit integer value to a buffer.
	private void WriteInt32(byte[] buffer, int offset, int value)
	{
		buffer[offset] = (byte)value;
		buffer[offset + 1] = (byte)(value >> 8);
		buffer[offset + 2] = (byte)(value >> 16);
		buffer[offset + 3] = (byte)(value >> 24);
	}

	// Write a BGR value to a buffer as an RGBQUAD.
	private void WriteBGR(byte[] buffer, int offset, int value)
	{
		buffer[offset] = (byte)value;
		buffer[offset + 1] = (byte)(value >> 8);
		buffer[offset + 2] = (byte)(value >> 16);
		buffer[offset + 3] = (byte)0;
	}

	// Set this window's maximum size.
	void IToolkitTopLevelWindow.SetMaximumSize(Size size)
	{
		maximumSize = size;
		//Console.WriteLine("DrawingTopLevelWindow.SetMaximumSize, "+sink);
	}

	// Set this window's minimum size.
	void IToolkitTopLevelWindow.SetMinimumSize(Size size)
	{
		minimumSize = size;
		//Console.WriteLine("DrawingTopLevelWindow.SetMinimumSize, "+sink);
	}

	// Set the window title.
	void IToolkitTopLevelWindow.SetTitle(String title)
	{
		if (hwnd == IntPtr.Zero)
			throw new ApplicationException("DrawingWindow.SetTitle ERROR:Cant set title. Hwnd not created yet");
		if(title == null)
		{
			title = String.Empty;
		}
		Win32.Api.SetWindowTextA(hwnd, title);
		//Console.WriteLine("DrawingTopLevelWindow.SetTitle, " + sink);
	}

	// Change the set of supported window decorations and functions.
	void IToolkitTopLevelWindow.SetWindowFlags(ToolkitWindowFlags flags)
	{
		if (hwnd == IntPtr.Zero)
			throw new ApplicationException("DrawingTopLevelWindow.SetWindowsFlags ERROR: Cant SetWindowsFlags. Hwnd not created yet");
	
		Win32.Api.WindowStyle style;
		Win32.Api.WindowsExtendedStyle extendedStyle;
			
		toolkit.GetWin32StylesFromFlags( flags, out style, out extendedStyle);
		//Now set the style
		
		Win32.Api.SetLastError(0);
		
		if (Win32.Api.SetWindowLongA(hwnd, Win32.Api.SetWindowLongType.GWL_STYLE,style) == 0
			&& Win32.Api.GetLastError() != 0)
			throw new InvalidOperationException("Unable to change the window style");
			
		Win32.Api.SetLastError(0);
		
		if (Win32.Api.SetWindowLongA(hwnd, Win32.Api.SetWindowLongType.GWL_EXSTYLE,extendedStyle) == 0
			&& Win32.Api.GetLastError() != 0)
			throw new InvalidOperationException("Unable to change the extended window style");
			
		// Redraw the entire window including the non client portion
		Win32.Api.RedrawWindow( hwnd, IntPtr.Zero, IntPtr.Zero, Win32.Api.RedrawWindowFlags.RDW_INVALIDATE | Win32.Api.RedrawWindowFlags.RDW_FRAME );
		//Console.WriteLine( "DrawingTopLevelWindow.SetWindowFlags, " + sink );
			

	}

	void IToolkitTopLevelWindow.SetOpacity(double opacity)
	{
		// Not yet implemented
	}

	internal override void SetFocus()
	{
		if (sink != null)
			sink.ToolkitPrimaryFocusEnter();
		//Console.WriteLine( "DrawingTopLevelWindow.GotFocus "+sink );
	}

	internal override void KillFocus()
	{
		if (sink != null)
			sink.ToolkitPrimaryFocusLeave();
		//Console.WriteLine( "DrawingTopLevelWindow.LostFocus "+sink );
	}

	internal override void Close()
	{
		if(sink != null)
		{
			sink.ToolkitClose();
		}
		//Console.WriteLine( "DrawingTopLevelWindow.Close "+sink );
	}


	//Create the invisible control
	internal override void CreateWindow() 
	{
		int leftAdjust, topAdjust, rightAdjust, bottomAdjust;
		Toolkit.GetWindowAdjust( out leftAdjust, out topAdjust, out rightAdjust, out bottomAdjust, flags);

		Size outside = new Size(dimensions.Width + leftAdjust + rightAdjust, dimensions.Height + topAdjust + bottomAdjust);

		hwnd = Win32.Api.CreateWindowExA( extendedStyle, className, "", style, Win32.Api.CW_USEDEFAULT, 0, outside.Width, outside.Height, IntPtr.Zero, IntPtr.Zero, Win32.Api.GetModuleHandleA(null), IntPtr.Zero );
		if (hwnd==IntPtr.Zero) 
		{
			throw new Exception("OS reported failure to create new Window");
		}
		dimensions = (this as IToolkitWindow).Dimensions;
		outsideDimensions = new Rectangle(dimensions.X - leftAdjust, dimensions.Y - topAdjust, dimensions.Width + leftAdjust + rightAdjust, dimensions.Height + topAdjust + bottomAdjust);
		sink.ToolkitExternalMove( dimensions.X, dimensions.Y );
		sink.ToolkitExternalResize( dimensions.Width, dimensions.Height );
		//Console.WriteLine( "DrawingTopLevelWindow.CreateWindow, "+sink+", [" + dimensions.Size + "]" );
	}

	void IToolkitWindow.SendBeginInvoke(IntPtr i_gch)
	{
		StaticSendBeginInvoke(hwnd,i_gch);
	}

}; // class DrawingTopLevelWindow

}; // namespace System.Drawing.Toolkit
