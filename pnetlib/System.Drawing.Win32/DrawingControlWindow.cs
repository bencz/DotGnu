/*
 * DrawingControlWindow.cs - Implementation of the control, inherits from drawing window.
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
using System.Collections;

internal class DrawingControlWindow : DrawingWindow, IToolkitWindow
{
	//for a unique class name
	protected static uint createCount;

	public DrawingControlWindow(DrawingToolkit toolkit, string name, DrawingWindow parent, int x, int y, int width, int height, IToolkitEventSink sink) : base (toolkit)
	{
		this.sink = sink;
		//Console.WriteLine("DrawingControlWindow");
		this.parent = parent;
		dimensions = new Rectangle( x, y, width, height );
		//At the moment we create a unique class name for EVERY window. SWF does it for each window type
		className = "DrawingControlWindow" + createCount++;
		//Register the windows class
		windowsClass = new Win32.Api.WNDCLASS();
		windowsClass.style = Win32.Api.WindowClassStyle.CS_DBLCLKS;
		windowsClass.lpfnWndProc = new Win32.Api.WNDPROC(toolkit.WindowsLoop);
		windowsClass.hbrBackground = IntPtr.Zero; //(IntPtr)(Win32.Api.COLOR_WINDOW + 1);
		windowsClass.lpszClassName = className ;
		if (Win32.Api.RegisterClassA( ref windowsClass)==0) 
		{
			throw new Exception("Failed to register Windows class " + className);
		}
		//Set default windows settings
		style = Win32.Api.WindowStyle.WS_CHILD | Win32.Api.WindowStyle.WS_TABSTOP | Win32.Api.WindowStyle.WS_CLIPSIBLINGS;
		extendedStyle = 0;
	}

	// Reparent this window to underneath a new parent. If there is no parent then the parent is the desktop and the control is not visible.
	void IToolkitWindow.Reparent(IToolkitWindow parent, int x, int y)
	{
		if (parent != this.parent)
		{
			// window with no parent cant be visible
			if (parent == null || (parent as DrawingWindow).hwnd == IntPtr.Zero)
				IsMapped = false;
			bool parented = (this.parent != null && this.parent.hwnd != IntPtr.Zero);

			// Adjust the heirararchy of parents and top of hierarchy
			ReparentOwn(parent as DrawingWindow);
			
			suspendExternalMoveResizeNotify = true;
			Win32.Api.SetParent( hwnd, parentHwnd);
			suspendExternalMoveResizeNotify = false;
			// Reposition this window on the client
			(this as IToolkitWindow).MoveResize(x, y, dimensions.Width, dimensions.Height);
			
			if (visible && !parented)
				SetVisible();
		}
		//Console.WriteLine("DrawingWindow.Reparent, " + sink);
	}
	
	internal override void CreateWindow()
	{
		hwnd = Win32.Api.CreateWindowExA( extendedStyle, className, string.Empty, style, dimensions.X, dimensions.Y, dimensions.Width, dimensions.Height, parentHwnd, IntPtr.Zero,Win32.Api.GetModuleHandleA(null),IntPtr.Zero );
		//Console.WriteLine("DrawingControlWindow.CreateWindow hwnd="+hwnd + ",parent hwnd="+parentHwnd+",["+dimensions.X+","+dimensions.Y+","+dimensions.Width+","+dimensions.Height+"]");
		if (hwnd==IntPtr.Zero) 
		{
			throw new Exception("Failed to create new Window");
		}
		sink.ToolkitExternalMove( dimensions.X, dimensions.Y );
		sink.ToolkitExternalResize( dimensions.Width, dimensions.Height );

		SetTopOfHierarchy();
	}

	protected IntPtr parentHwnd 
	{
		get 
		{
			if (parent != null && parent.hwnd != IntPtr.Zero)
				return parent.hwnd;
			else
				return Win32.Api.GetDesktopWindow();
		}
	}

	public override bool IsMapped
	{
		get
		{
			return base.IsMapped;
		}
		set
		{
			// cant change the visiblity of a window that doesnt have a parent
			if (parent != null && parent.hwnd != IntPtr.Zero)
				base.IsMapped = value;
			else
				visible = value;
		}
	}

	void IToolkitWindow.SendBeginInvoke(IntPtr i_gch)
	{
		StaticSendBeginInvoke(hwnd,i_gch);
	}


}; // class DrawingControlWindow

}; // namespace System.Drawing.Toolkit
