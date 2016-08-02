/*
 * DrawingToolkit.cs - Implementation of IToolkit for Win32.
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
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Collections;
using System.Threading;
using DotGNU.Images;

public class DrawingToolkit : IToolkit
{
	private static ArrayList timers = new ArrayList();
	internal DrawingWindow[] windows = new DrawingWindow[64];
	internal int windowCount = 0;
	internal Win32.Api.WindowsMessages registeredBeginInvokeMessage;
	// The child window or control that has captured or null for none
	internal DrawingWindow capturedWindow;
	// The top level window that has the capture.
	internal DrawingWindow capturedTopWindow;
	// The current window that has been entered by the mouse.
	internal DrawingWindow enteredWindow;
	// The window used as the parent when we want to hide the taskbar item and for processing the timers
	private DrawingHiddenWindow hiddenWindow;
	private IntPtr nullPenHandle;
	private IntPtr hollowBrushHandle;

	public DrawingToolkit()
	{
		hiddenWindow = new DrawingHiddenWindow(this, "DrawingHiddenWindow",0,0,null);
		AddWindow(hiddenWindow, null);
		hiddenWindow.CreateWindow();
		registeredBeginInvokeMessage = Win32.Api.RegisterWindowMessageA("DOTNET_BEGIN_INVOKE_MESSAGE");
	}

	// Process events in the event queue.  If "waitForEvent" is true,
	// then wait for the next event and return "false" if "Quit" was
	// seen.  If "waitForEvent" is false, then process events in the
	// queue and return "true".  If "waitForEvent" is false and there
	// are no events in the queue, then return "false".
	public bool ProcessEvents(bool waitForEvent)
	{
		Win32.Api.MSG message;
		if (!waitForEvent)
		{
			// return false if there is no message or the quit message
			if (!Win32.Api.PeekMessageA(out message, IntPtr.Zero, 0, 0, Win32.Api.PeekMessageType.PM_NOREMOVE)
				|| message.message==Win32.Api.WindowsMessages.WM_QUIT)
				return false;
		}
		// process the message
		if (Win32.Api.GetMessageA(out message, IntPtr.Zero,0,0)==0)
			return false; //occurs on WM_QUIT
		Win32.Api.TranslateMessage(ref message);
		Win32.Api.DispatchMessageA(ref message);	
		return true;
	}

	// Send a quit message to the toolkit, which should cause
	// it to exit from the "Run" method.
	public void Quit()
	{
		Win32.Api.PostQuitMessage(0);
	}

	// Send a wakeup message to a thread's message queue to cause
	// it to return back from "ProcessEvents".
	public void Wakeup(Thread thread)
	{
		Win32.Api.PostMessageA(hiddenWindow.hwnd,Win32.Api.WindowsMessages.WM_NULL, 0, 0);
	}

	// Resolve a system color to an RGB value.  Returns -1 if the
	// system does not support the color and a default should be used.
	public int ResolveSystemColor(KnownColor color)
	{
		// array out of bounds
		if ((int)color >= Win32.Api.KnownColorWindowsMap.Length)
			return -1;

		// map KnownColor to Windows color type
		int syscolortype = Win32.Api.KnownColorWindowsMap[(int)color];
			
		// get BGR value of system color
		int colorbgr = Win32.Api.GetSysColor(syscolortype);

		// convert the BGR value to RGB
		int colorrgb = Win32.Api.SwapRGB(colorbgr);

		return colorrgb;
	}

	[TODO]
	// Create an IToolkitGraphics object from a HDC.
	public IToolkitGraphics CreateFromHdc(IntPtr hdc, IntPtr hdevice)
	{
		// This is tricky - maybe we have to keep track of which hdc's we create?
		return null;
	}

	[TODO]
	// Create an IToolkitGraphics object from a HWND.
	public IToolkitGraphics CreateFromHwnd(IntPtr hwnd)
	{
		return null;
	}

	public IToolkitGraphics CreateFromImage(IToolkitImage image)
	{
		return new DrawingGraphicsImage(this, image);
	}

	// Create a solid toolkit brush.
	public IToolkitBrush CreateSolidBrush(System.Drawing.Color color)
	{
		return new DrawingSolidBrush(this, color);
	}

	// Create a hatched toolkit brush.
	public IToolkitBrush CreateHatchBrush
		(HatchStyle style, System.Drawing.Color foreColor,
		System.Drawing.Color backColor)
	{
		return new DrawingHatchBrush(this, style, foreColor, backColor);
	}

	// Create an XOR brush.
	public IToolkitBrush CreateXorBrush(IToolkitBrush innerBrush)
	{
		return new DrawingXorBrush(this, innerBrush);
	}

	[TODO]
	// Create a linear gradient brush.
	public IToolkitBrush CreateLinearGradientBrush
		(RectangleF rect, System.Drawing.Color color1,
		System.Drawing.Color color2,
		LinearGradientMode mode)
	{
		return null;
	}

	[TODO]
	public IToolkitBrush CreateLinearGradientBrush
		(RectangleF rect, System.Drawing.Color color1,
		System.Drawing.Color color2, float angle,
		bool isAngleScaleable)
	{
		return null;
	}

	// Create a texture brush.
	public IToolkitBrush CreateTextureBrush
		(TextureBrush properties, IToolkitImage image,
		RectangleF dstRect, ImageAttributes imageAttr)
	{
		return new DrawingTextureBrush
			(this, properties, image, dstRect, imageAttr);
	}

	// Create a toolkit pen from pen properties.
	// If the toolkit does not support the precise combination of pen
	// properties, it will return the closest matching pen.
	public IToolkitPen CreatePen(Pen pen)
	{
		return new DrawingPen(this, pen);
	}

	// Create a toolkit font from the properties in the specified object.
	public IToolkitFont CreateFont(System.Drawing.Font font, float dpi)
	{
		return new DrawingFont(this, font, dpi);
	}

	// Create the default system font on this platform.
	public System.Drawing.Font CreateDefaultFont()
	{
		// Default is "Microsoft Sans Serif, 9".
		return new System.Drawing.Font
			(new FontFamily(GenericFontFamilies.SansSerif), 9.0f);
	}

	[TODO]
	// Get the handle for the halftone palette.  IntPtr.Zero if not supported.
	public IntPtr GetHalftonePalette()
	{
		return IntPtr.Zero;
	}

	// Create a form.
	public IToolkitTopLevelWindow CreateTopLevelWindow(int width, int height, IToolkitEventSink sink)
	{
		DrawingTopLevelWindow window = new DrawingTopLevelWindow(this, string.Empty, width, height, sink);
		AddWindow(window, null);
		window.CreateWindow();
		return window;
	}

	[TODO]
	// Create a top-level dialog shell.
	public IToolkitWindow CreateTopLevelDialog
		(int width, int height, bool modal, bool resizable,
		IToolkitWindow dialogParent, IToolkitEventSink sink)
	{
		DrawingTopLevelWindow window;
		window = new DrawingTopLevelWindow
			(this, String.Empty, width, height, sink);
		/*if(dialogParent is TopLevelWindow)
			{
				window.TransientFor = (TopLevelWindow)dialogParent;
			}
			if(modal)
			{
				window.InputType = MotifInputType.ApplicationModal;
			}
			else
			{
				window.InputType = MotifInputType.Modeless;
			}
			if(!resizable)
			{
				window.Decorations = MotifDecorations.Border |
										MotifDecorations.Title |
										MotifDecorations.Menu;
				window.Functions = MotifFunctions.Move |
									MotifFunctions.Close;
			}*/
		AddWindow(window, dialogParent as DrawingWindow);
		window.CreateWindow();
		return window;
	}

	// Create a top-level popup window.  Popup windows do not have
	// any borders and grab the mouse and keyboard when they are mapped
	// to the screen.  They are used for menus, drop-down lists, etc.
	public IToolkitWindow CreatePopupWindow
		(int x, int y, int width, int height, IToolkitEventSink sink)
	{
		DrawingWindow window = new DrawingPopupWindow(this, x, y, width, height, sink);
		AddWindow(window, null);
		window.CreateWindow();
		return window;
	}


	// Create a child window.  If "parent" is null, then the child
	// does not yet have a "real" parent - it will be reparented later.
	public IToolkitWindow CreateChildWindow
		(IToolkitWindow parent, int x, int y, int width, int height,
		IToolkitEventSink sink)
	{
		DrawingWindow dparent;
		if(parent is DrawingWindow)
		{
			dparent = ((DrawingWindow)parent);
		}
		else
		{
			dparent = null;
		}
		DrawingWindow window = new DrawingControlWindow(this, "", dparent, x, y, width, height, sink);
		AddWindow(window, parent as DrawingWindow);
		window.CreateWindow();
		return window;
	}

	[TODO]
	// Create an MDI client area.
	public IToolkitMdiClient CreateMdiClient
				(IToolkitWindow parent, int x, int y, int width, int height,
				 IToolkitEventSink sink)
			{
				return null;
			}

	[TODO]
	// Get a list of all font families on this system, or all font
	// families that are compatible with a particular IToolkitGraphics.
	public FontFamily[] GetFontFamilies(IToolkitGraphics graphics)
	{
		// We only support three font families.  Extend later.
		return new FontFamily [] {
										new FontFamily("Arial"),
										new FontFamily("Times New Roman"),
										new FontFamily("Courier New"),
		};
	}

	// Get font family metric information.
	public void GetFontFamilyMetrics(GenericFontFamilies genericFamily,
		String name,
		System.Drawing.FontStyle style,
		out int ascent, out int descent,
		out int emHeight, out int lineSpacing)
	{
		//TODO
		switch(genericFamily)
		{
			case GenericFontFamilies.SansSerif:
			default:
			{
				// Metrics for "Arial".
				ascent = 1854;
				descent = 434;
				emHeight = 2048;
				lineSpacing = 2355;
			}
				break;

			case GenericFontFamilies.Serif:
			{
				// Metrics for "Times New Roman".
				ascent = 1825;
				descent = 443;
				emHeight = 2048;
				lineSpacing = 2355;
			}
				break;

			case GenericFontFamilies.Monospace:
			{
				// Metrics for "Courier New".
				ascent = 1705;
				descent = 615;
				emHeight = 2048;
				lineSpacing = 2320;
			}
				break;
		}
	}

	[TODO]
	// Get the IToolkitFont that corresponds to a hdc's current font.
	// Returns null if there is no way to obtain the information.
	public IToolkitFont GetFontFromHdc(IntPtr hdc)
	{
		return null;
	}

	[TODO]
	// Get the IToolkitFont that corresponds to a native font object.
	// Returns null if there is no way to obtain the information.
	public IToolkitFont GetFontFromHfont(IntPtr hfont)
	{
		return null;
	}

	[TODO]
	// Get the IToolkitFont that corresponds to LOGFONT information.
	// Returns null if there is no way to obtain the information.
	public IToolkitFont GetFontFromLogFont(Object lf, IntPtr hdc)
	{
		return null;
	}

	// Get the default IToolkitGraphics object to measure screen sizes.
	public IToolkitGraphics GetDefaultGraphics()
	{
		//Console.WriteLine("DrawingToolkit.GetDefaultGraphics");
		return new DrawingGraphics(this, Win32.Api.GetDC(Win32.Api.GetDesktopWindow()));
	}

	// Get the screen size, in pixels.
	public Size GetScreenSize()
	{
		int x = Win32.Api.GetSystemMetrics(Win32.Api.SystemMetricsType.SM_CXSCREEN);
		int y = Win32.Api.GetSystemMetrics(Win32.Api.SystemMetricsType.SM_CYSCREEN);
		return new Size(x,y);
	}

	// Get the working area of the screen, excluding task bars, etc.
	public System.Drawing.Rectangle GetWorkingArea()
	{
		Win32.Api.RECT r;
		Win32.Api.SystemParametersInfoA(Win32.Api.SystemParametersAction.SPI_GETWORKAREA, 0, out r, 0);
		return new Rectangle(r.left,r.top,r.right-r.left,r.bottom-r.top);
	}

	// Get the adjustment values for a top-level window, to convert
	// between window bounds and client bounds.  Each value should
	// be >= 0 and indicate the number of pixels to subtract from the
	// windows bounds to get the client bounds.
	public void GetWindowAdjust(out int leftAdjust, out int topAdjust,
		out int rightAdjust, out int bottomAdjust,
		ToolkitWindowFlags flags)
	{
		Win32.Api.WindowStyle style;
		Win32.Api.WindowsExtendedStyle extendedStyle;
		GetWin32StylesFromFlags( flags, out style, out extendedStyle);
		Win32.Api.RECT rect = new System.Drawing.Win32.Api.RECT(0,0,0,0);
		Win32.Api.AdjustWindowRectEx( ref rect, style, false, extendedStyle );

		leftAdjust = -rect.left;
		topAdjust = -rect.top;
		rightAdjust = rect.right;
		bottomAdjust = rect.bottom;
	}

	internal void GetWin32StylesFromFlags( ToolkitWindowFlags flags, out Win32.Api.WindowStyle style, out Win32.Api.WindowsExtendedStyle extendedStyle)
	{
		style = Win32.Api.WindowStyle.WS_POPUP | Win32.Api.WindowStyle.WS_CLIPCHILDREN;
		extendedStyle = 0;
	
		//to remove the popup style
		Win32.Api.WindowStyle overlapped = ~Win32.Api.WindowStyle.WS_POPUP;
			
		if((flags & ToolkitWindowFlags.Close) > 0)
			style |= Win32.Api.WindowStyle.WS_SYSMENU;

		if((flags & ToolkitWindowFlags.Minimize) > 0)
			style = style & overlapped | Win32.Api.WindowStyle.WS_MINIMIZEBOX;

		if((flags & ToolkitWindowFlags.Maximize) > 0)
			style = style & overlapped | Win32.Api.WindowStyle.WS_MAXIMIZEBOX;

		if((flags & ToolkitWindowFlags.Caption) > 0)
			style |= Win32.Api.WindowStyle.WS_CAPTION;

		if((flags & ToolkitWindowFlags.Border) > 0)
			style |= Win32.Api.WindowStyle.WS_BORDER;

		if((flags & ToolkitWindowFlags.ResizeHandles) > 0)
			style |= Win32.Api.WindowStyle.WS_THICKFRAME;

		if((flags & ToolkitWindowFlags.Menu) > 0)
			style = style | Win32.Api.WindowStyle.WS_SYSMENU;

		if((flags & ToolkitWindowFlags.Resize) > 0)
			style |= Win32.Api.WindowStyle.WS_THICKFRAME;

		//We dont handle the Move flag in Win32

		//TODO: NOT SURE HERE
		if((flags & ToolkitWindowFlags.Modal) > 0)
			style |= Win32.Api.WindowStyle.WS_POPUP | Win32.Api.WindowStyle.WS_DLGFRAME;

		//TODO: Need a hidden window
		//if((flags & ToolkitWindowFlags.ShowInTaskBar)>0)
	
		if((flags & ToolkitWindowFlags.TopMost)>0)
			extendedStyle |= Win32.Api.WindowsExtendedStyle.WS_EX_TOPMOST;

		if((flags & ToolkitWindowFlags.ToolWindow) > 0)
			extendedStyle |= Win32.Api.WindowsExtendedStyle.WS_EX_TOOLWINDOW;		

	}

	// Register a timer that should fire every "interval" milliseconds.
	// Returns a cookie that can be used to identify the timer.
	public Object RegisterTimer
		(Object owner, int interval, EventHandler expire)
	{
		uint cookie;
		if (timers.Count == 0)
			cookie = 0;
		else
			cookie = (uint)(timers[timers.Count-1] as Timer).cookie + 1;
		// Assume for now that the first window created will service the timers
		Win32.Api.SetTimer( hiddenWindow.hwnd, cookie, (uint)interval, IntPtr.Zero );
		timers.Add(new Timer(owner, cookie, expire));
		return cookie;
	}

	// Unregister a timer.
	public void UnregisterTimer(Object cookie)
	{
		Win32.Api.KillTimer( IntPtr.Zero, (uint)cookie );
		for( int i = 0; i < timers.Count;  i++ )
		{
			if ( (timers[i] as Timer).cookie == (uint)cookie )
			{
				timers.RemoveAt(i);
				break;
			}
		}
	}


	//An instance is created for each timer registered
	private class Timer
	{
		private Object owner;
		private EventHandler expire;
		internal uint cookie;
	
		public Timer( Object owner, uint cookie, EventHandler expire )
		{
			this.owner = owner;
			this.cookie = cookie;
			this.expire = expire;
		}

		public void Fire()
		{
			expire( owner, EventArgs.Empty );
		}

	}

	private static void TimerFired( int wParam)
	{
		foreach( Timer timer in timers)
		{
			if (timer.cookie == wParam)
			{
				timer.Fire();
				break;
			}
		}
		// If we reach here, there was a timer on the message queue when we killed it
		// Its okay just ignore it.
	}

	// Convert a client point for a window into a screen point.
	public Point ClientToScreen(IToolkitWindow window, Point point)
	{
		Win32.Api.POINT p;
		p.x = point.X;
		p.y = point.Y;
		Win32.Api.ClientToScreen( (window as DrawingWindow).hwnd, ref p );
		return new Point( p.x, p.y );
	}

	// Convert a screen point for a window into a client point.
	public Point ScreenToClient(IToolkitWindow window, Point point)
	{
		Win32.Api.POINT p;
		p.x = point.X;
		p.y = point.Y;
		Win32.Api.ScreenToClient( (window as DrawingWindow).hwnd, ref p );
		return new Point( p.x, p.y );
	}

	//The main windows loop. Messages are handed off
	internal int WindowsLoop(IntPtr hwnd, int msg, int wParam, int lParam)  
	{
		int retval = 0;
		switch((Win32.Api.WindowsMessages)msg) 
		{

			case Win32.Api.WindowsMessages.WM_CREATE:
				DrawingWindow(IntPtr.Zero).hwnd = hwnd;
				break;

			case Win32.Api.WindowsMessages.WM_SETFOCUS:
				DrawingWindow(hwnd).SetFocus();
				break;
			case Win32.Api.WindowsMessages.WM_KILLFOCUS:
				DrawingWindow(hwnd).KillFocus();
				break;
			case Win32.Api.WindowsMessages.WM_ACTIVATE:
				return DrawingWindow(hwnd).Activate(wParam, lParam);

			case Win32.Api.WindowsMessages.WM_WINDOWPOSCHANGING:
				DrawingWindow(hwnd).WindowPosChanging(lParam);
				break;

			case Win32.Api.WindowsMessages.WM_SYSCOMMAND:
			switch((Win32.Api.SystemCommand)wParam) 
			{
				case(Win32.Api.SystemCommand.SC_RESTORE):
					((DrawingTopLevelWindow)DrawingWindow(hwnd)).WindowStateChanged(0); // FormWindowState.Normal.
					retval = Win32.Api.DefWindowProcA(hwnd, msg, wParam, lParam);
					break;
				case(Win32.Api.SystemCommand.SC_MAXIMIZE):
					((DrawingTopLevelWindow)DrawingWindow(hwnd)).WindowStateChanged(2); // FormWindowState.Maximized.
					retval = Win32.Api.DefWindowProcA(hwnd, msg, wParam, lParam);
					break;
				case(Win32.Api.SystemCommand.SC_MINIMIZE):
					((DrawingTopLevelWindow)DrawingWindow(hwnd)).WindowStateChanged(1); // FormWindowState.Minimized.
					retval = Win32.Api.DefWindowProcA(hwnd, msg, wParam, lParam);
					break;
				case(Win32.Api.SystemCommand.SC_CLOSE):
					//TODO
					DrawingWindow(hwnd).Close();
					break;
				default:
					retval = Win32.Api.DefWindowProcA(hwnd, msg, wParam, lParam);
					break;
			}
				break;	

			case Win32.Api.WindowsMessages.WM_PAINT:
				DrawingWindow(hwnd).Paint();
				break;

			case Win32.Api.WindowsMessages.WM_SETCURSOR:
				//TEMP
				Win32.Api.SetCursor(Win32.Api.LoadCursorA(IntPtr.Zero, Win32.Api.CursorName.IDC_ARROW));
				retval =1;
				break;

			case Win32.Api.WindowsMessages.WM_MOUSEMOVE:
				DrawingWindow(hwnd).MouseMove( msg, wParam, lParam );
				break;
			case Win32.Api.WindowsMessages.WM_MOUSEWHEEL:
				DrawingWindow(hwnd).MouseMove( msg, wParam, lParam );
				break;
			case Win32.Api.WindowsMessages.WM_LBUTTONDOWN:
				DrawingWindow(hwnd).ButtonDown( msg, wParam | (int)Win32.Api.MouseKeyState.MK_LBUTTON, lParam );
				break;
			case Win32.Api.WindowsMessages.WM_RBUTTONDOWN:
				DrawingWindow(hwnd).ButtonDown( msg, wParam | (int)Win32.Api.MouseKeyState.MK_RBUTTON, lParam );
				break;
			case Win32.Api.WindowsMessages.WM_MBUTTONDOWN:
				DrawingWindow(hwnd).ButtonDown( msg, wParam | (int)Win32.Api.MouseKeyState.MK_MBUTTON, lParam );
				break;
			case Win32.Api.WindowsMessages.WM_XBUTTONDOWN:
				DrawingWindow(hwnd).ButtonDown( msg, wParam, lParam );
				break;
			case Win32.Api.WindowsMessages.WM_LBUTTONUP:
				DrawingWindow(hwnd).ButtonUp( msg, wParam | (int)Win32.Api.MouseKeyState.MK_LBUTTON, lParam );
				break;
			case Win32.Api.WindowsMessages.WM_RBUTTONUP:
				DrawingWindow(hwnd).ButtonUp( msg, wParam | (int)Win32.Api.MouseKeyState.MK_RBUTTON, lParam );
				break;
			case Win32.Api.WindowsMessages.WM_MBUTTONUP:
				DrawingWindow(hwnd).ButtonUp( msg, wParam | (int)Win32.Api.MouseKeyState.MK_MBUTTON, lParam );
				break;
			case Win32.Api.WindowsMessages.WM_XBUTTONUP:
				DrawingWindow(hwnd).ButtonUp( msg, wParam, lParam );
				break;
			case Win32.Api.WindowsMessages.WM_LBUTTONDBLCLK:
				DrawingWindow(hwnd).DoubleClick( msg, wParam | (int)Win32.Api.MouseKeyState.MK_LBUTTON, lParam );
				break;
			case Win32.Api.WindowsMessages.WM_RBUTTONDBLCLK:
				DrawingWindow(hwnd).DoubleClick( msg, wParam | (int)Win32.Api.MouseKeyState.MK_RBUTTON, lParam );
				break;
			case Win32.Api.WindowsMessages.WM_MBUTTONDBLCLK:
				DrawingWindow(hwnd).DoubleClick( msg, wParam | (int)Win32.Api.MouseKeyState.MK_MBUTTON, lParam );
				break;
			case Win32.Api.WindowsMessages.WM_XBUTTONDBLCLK:
				DrawingWindow(hwnd).DoubleClick( msg, wParam, lParam );
				break;
		
			case Win32.Api.WindowsMessages.WM_KEYDOWN:
			case Win32.Api.WindowsMessages.WM_SYSKEYDOWN:
				if (!DrawingWindow(hwnd).KeyDown( wParam, lParam ))
				{
					retval = Win32.Api.DefWindowProcA(hwnd, msg, wParam, lParam);
				}
				break;
			case Win32.Api.WindowsMessages.WM_CHAR:
			case Win32.Api.WindowsMessages.WM_SYSCHAR:
				if (!DrawingWindow(hwnd).Char( wParam, lParam ))
				{
					retval = Win32.Api.DefWindowProcA(hwnd, msg, wParam, lParam);
				}
				break;
			case Win32.Api.WindowsMessages.WM_KEYUP:
			case Win32.Api.WindowsMessages.WM_SYSKEYUP:
				if (!DrawingWindow(hwnd).KeyUp( wParam, lParam ))
				{
					retval = Win32.Api.DefWindowProcA(hwnd, msg, wParam, lParam);
				}
				break;
			case Win32.Api.WindowsMessages.WM_SETTINGCHANGE:
				DrawingWindow(hwnd).SettingsChange( wParam );
				break;

			case Win32.Api.WindowsMessages.WM_TIMER:
				TimerFired( wParam);
				break;

			case Win32.Api.WindowsMessages.WM_MOUSEACTIVATE:
				return DrawingWindow(hwnd).MouseActivate(DrawingWindow(new IntPtr(wParam)));

			default:
				if((Win32.Api.WindowsMessages)msg == registeredBeginInvokeMessage)
				{
					DrawingWindow(hwnd).SendBeginInvoke(wParam);
					return 0;
				}
				retval = Win32.Api.DefWindowProcA(hwnd, msg, wParam, lParam);
				break;
		}
		return retval;
	}

	internal DrawingWindow DrawingWindow(IntPtr hwnd)
	{
		for(int i = 0; i < windowCount; i++)
		{
			DrawingWindow window = windows[i];
			if (window.hwnd == hwnd)
				return window;
		}
		return null;
	}

	public IToolkitImage CreateImage(DotGNU.Images.Image image, int frame)
	{
		return new DrawingImage(image, frame);
	}

	// Add a new DrawingWindow and make room if necessary
	internal void AddWindow(DrawingWindow window, DrawingWindow parent)
	{
		if (windows.Length == windowCount)
		{
			DrawingWindow[] newWindows = new DrawingWindow[windows.Length * 2];
			windows.CopyTo(newWindows, 0);
			windows = newWindows;
		}
		windows[windowCount++] = window;
	}

	internal IntPtr HollowBrushHandle
	{
		get
		{
			if (hollowBrushHandle == IntPtr.Zero)
				hollowBrushHandle = Win32.Api.GetStockObject(Win32.Api.StockObjectType.HOLLOW_BRUSH);
			return hollowBrushHandle;
		}
	}

	internal IntPtr NullPenHandle
	{
		get
		{
			if (nullPenHandle == IntPtr.Zero)
				nullPenHandle = Win32.Api.GetStockObject(Win32.Api.StockObjectType.NULL_PEN);
			return nullPenHandle;
		}
	}

	[TODO]
	// Get the clipboard handler for this toolkit, or null if no clipboard.
	public IToolkitClipboard GetClipboard()
	{
		return null;
	}

	public IToolkitWindowBuffer CreateWindowBuffer(IToolkitWindow window)
	{
		return new DrawingWindowBuffer(window);
	}

	internal static IntPtr RectanglesToRegion(Rectangle[] rects)
	{
		IntPtr region = Win32.Api.CreateRectRgn(0,0,0,0);
		IntPtr region1 = Win32.Api.CreateRectRgn(0,0,0,0);
		for (int i = 0; i < rects.Length; i++)
		{
			Rectangle rect = rects[i];
			Win32.Api.SetRectRgn(region1, rect.Left, rect.Top, rect.Right, rect.Bottom);
			Win32.Api.CombineRgn(region, region, region1, Win32.Api.RegionCombineMode.RGN_OR);
		}
		Win32.Api.DeleteObject(region1);
		return region;
	}

}; // class DrawingToolkit

}; // namespace System.Drawing.Toolkit
