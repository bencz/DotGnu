/*
 * DrawingPopupWindow.cs - A window that captures the mouse and keyboard.
 *
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

internal class DrawingPopupWindow : DrawingWindow, IToolkitWindow
{
	protected static uint createCount;

	public DrawingPopupWindow(DrawingToolkit toolkit, int x, int y, int width, int height,
		IToolkitEventSink sink) : base ( toolkit )
	{
		//Console.WriteLine("DrawingPopupWindow");
		this.sink = sink;
		dimensions = new Rectangle(x, y, width, height);

		// At the moment we create a unique class name for EVERY window. SWF does it for each unique window class
		className = "DrawingPopupWindow" + createCount++;

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
		style = Win32.Api.WindowStyle.WS_POPUP;
		menu = false;
		extendedStyle = Win32.Api.WindowsExtendedStyle.WS_EX_TOOLWINDOW;
		// We capture the mouse, and we want the client windows to be notified
		topOfhierarchy = this;
	}

	internal override void CreateWindow()
	{
		hwnd = Win32.Api.CreateWindowExA( extendedStyle, className, string.Empty, style, dimensions.X, dimensions.Y, dimensions.Width, dimensions.Height, IntPtr.Zero, IntPtr.Zero,Win32.Api.GetModuleHandleA(null),IntPtr.Zero );
		//Console.WriteLine("DrawingPopupWindow.CreateWindow hwnd="+hwnd + ",["+dimensions.X+","+dimensions.Y+","+dimensions.Width+","+dimensions.Height+"]");
		if (hwnd==IntPtr.Zero) 
		{
			throw new Exception("Failed to create new Window");
		}
		sink.ToolkitExternalMove( dimensions.X, dimensions.Y );
		sink.ToolkitExternalResize( dimensions.Width, dimensions.Height );
	}

	internal override int MouseActivate(DrawingWindow activateWindow)
	{
		return (int)Win32.Api.WM_MOUSEACTIVATEReturn.MA_NOACTIVATE;
	}

	public override bool IsMapped
	{
		get
		{
			return base.IsMapped;
		}
		set
		{
			base.IsMapped = value;
			(this as IToolkitWindow).Capture = value;
		}
	}

	protected internal override void MouseMove(int msg, int wParam, int lParam)
	{
		// Another popup window could grab the capture from this control. When we move back onto this control, we need to make sure that we regain the capture.
		IToolkitWindow window = (this as IToolkitWindow);
		if (window.Capture != visible)
		{
			window.Capture = base.IsMapped;
		}
		base.MouseMove (msg, wParam, lParam);
	}

}
}
