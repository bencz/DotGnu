/*
 * DrawingHiddenWindow.cs - Hidden Window class..
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
internal class DrawingHiddenWindow : DrawingWindow
{
	public DrawingHiddenWindow(DrawingToolkit toolkit, String name,
		int width, int height, IToolkitEventSink sink) : base ( toolkit )
	{
		className = "DrawingHiddenWindow";
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
		style = Win32.Api.WindowStyle.WS_OVERLAPPEDWINDOW;
		extendedStyle = 0;
		topOfhierarchy = this;
	}

	//Create the invisible control
	internal override void CreateWindow() 
	{
		hwnd = Win32.Api.CreateWindowExA( extendedStyle, className, "", style, Win32.Api.CW_USEDEFAULT, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, Win32.Api.GetModuleHandleA(null), IntPtr.Zero );
		if (hwnd==IntPtr.Zero) 
		{
			throw new Exception("OS reported failure to create new Window");
		}
		//Console.WriteLine( "DrawingHiddenWindow.CreateWindow, "+sink );
	}
}

}
