/*
 * DrawingWindow.cs - Abstract DrawingWindow class. This is each win32 window, controls and forms.
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
using System.Collections;
using DotGNU.Images;

internal abstract class DrawingWindow : IToolkitWindow
{
	protected DrawingToolkit toolkit;
	protected IToolkitEventSink sink;
	protected internal IntPtr hwnd;
	protected internal IntPtr hdc;
	protected internal DrawingWindow parent;
	
	private static Win32.Api.WindowsMessages registeredBeginInvokeMessage = 0;

	// The window at the top of the hierarchy
	protected internal DrawingWindow topOfhierarchy;
	private Color background;
	//cached when we create the window
	private IntPtr backgroundBrush = IntPtr.Zero;

	//Windows Class name
	protected string className;
	//This structure needs to be kept in memory
	protected Win32.Api.WNDCLASS windowsClass;
	//Used so we can tell the size of the non client window
	protected Win32.Api.WindowStyle style;
	protected Win32.Api.WindowsExtendedStyle extendedStyle;
	//Does the window have a menu
	protected bool menu = false;
	protected ToolkitWindowFlags flags = 0;

	// The dimensions of the control relative to parent
	protected Rectangle dimensions;
	// The outside (include OS decorations) dimensions
	protected Rectangle outsideDimensions;
	// Whether the control should be visible once its created
	protected bool visible;

	protected bool suspendExternalMoveResizeNotify;
	protected DrawingWindow( DrawingToolkit toolkit )
	{
		this.toolkit = toolkit;
		if( registeredBeginInvokeMessage == 0 )
			registeredBeginInvokeMessage =
				Win32.Api.RegisterWindowMessageA("DOTNET_BEGIN_INVOKE_MESSAGE");
	}

	// Move this window underneath the last window with the same parent.
	void IToolkitWindow.Lower()
	{
		// Find the last window with the same parent as this.
		DrawingWindow toWindow = null;
		for (int to = toolkit.windowCount - 1; to >= 0 ; to--)
		{
			toWindow = toolkit.windows[to];
			if (parent.IsParentOf(toWindow))
				break;
		}
		(this as IToolkitWindow).MoveToBelow(toWindow as IToolkitWindow);
		//Console.WriteLine("DrawingWindow.Lower, " + sink);
	}

	// Move this window to the topmost position amongst the children.
	void IToolkitWindow.Raise()
	{
		MoveWindowTo(this, parent);
		// Make the same changes to win32.
		Win32.Api.SetWindowPos(hwnd, Win32.Api.SetWindowsPosPosition.HWND_TOP, 0, 0, 0, 0, Win32.Api.SetWindowsPosFlags.SWP_NOMOVE | Win32.Api.SetWindowsPosFlags.SWP_NOSIZE | Win32.Api.SetWindowsPosFlags.SWP_NOACTIVATE);
		//Console.WriteLine("DrawingWindow.Raise, " + sink);
	}

	// Move this window to below one of its siblings.
	void IToolkitWindow.MoveToBelow(IToolkitWindow sibling)
	{
		DrawingWindow toWindow = sibling as DrawingWindow;
		MoveWindowTo(this, toWindow);
		// Make the same changes in win32.
		Win32.Api.SetWindowPos(hwnd, toWindow.hwnd, 0, 0, 0, 0, Win32.Api.SetWindowsPosFlags.SWP_NOMOVE | Win32.Api.SetWindowsPosFlags.SWP_NOSIZE | Win32.Api.SetWindowsPosFlags.SWP_NOACTIVATE);
		//Console.WriteLine("DrawingWindow.MoveToBelow, " + sink +", " + sibling);
	}

	// Move this window to above one of its siblings.
	void IToolkitWindow.MoveToAbove(IToolkitWindow sibling)
	{
		throw new NotSupportedException();
		//Console.WriteLine("DrawingWindow.MoveToAbove, "+sink);
	}

	void IToolkitWindow.Reparent(IToolkitWindow parent, int x, int y)
	{
		throw new NotSupportedException("TopLevelWindow can't have a parent");
	}

	public IToolkit Toolkit
	{
		get
		{
			return toolkit;
		}
	}

	// Actual operating system dimensions relative to parent.
	Rectangle IToolkitWindow.Dimensions
	{
		get
		{
			if (hwnd==IntPtr.Zero)
				return dimensions;
			else
			{
				Win32.Api.RECT rect;
				Win32.Api.GetWindowRect(hwnd, out rect);
				System.Drawing.Rectangle r = System.Drawing.Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
				if (parent != null)
				{
					Rectangle parentDimensions = (parent as IToolkitWindow).Dimensions;
					r.X -= parentDimensions.X;
					r.Y -= parentDimensions.Y;
				}
				else if (this is DrawingTopLevelWindow)
				{
					// Get dimensions excluding the window decorations
					int leftAdjust, topAdjust, rightAdjust, bottomAdjust;
					Toolkit.GetWindowAdjust(out leftAdjust, out topAdjust, out rightAdjust, out bottomAdjust, flags);
					r.X += leftAdjust;
					r.Y += topAdjust;
					r.Width -= leftAdjust + rightAdjust;
					r.Height -= topAdjust + bottomAdjust;
				}
				return r;
			}
		}
	}

	// Does the window have focus
	bool IToolkitWindow.Focused
	{
		get
		{
			return Win32.Api.GetFocus() == hwnd;	
		}
	}

	// Get or set the mouse capture on this window.
	bool IToolkitWindow.Capture
	{
		get
		{
			return toolkit.capturedWindow == this;
		}
		set
		{
			//Console.WriteLine("DrawingWindow.Capture, "+ value +", "+ sink);
			if (value)
			{
				toolkit.capturedWindow  = this;
				Win32.Api.SetCapture(hwnd);
			}
			else
			{
				toolkit.capturedWindow  = null;
				Win32.Api.ReleaseCapture();
			}
		}
	}

	//Set the focus to this window
	void IToolkitWindow.Focus()
	{
		//Console.WriteLine("DrawingWindow.Focus, "+ sink);
		Win32.Api.SetFocus(hwnd);
	}

	//Destroy window
	void IToolkitWindow.Destroy()
	{
		// Call will ignore if hwnd = IntPtr.Zero
		Win32.Api.DestroyWindow(hwnd);
		//Console.WriteLine("DrawingWindow.Destroy, "+sink);
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		(this as IToolkitWindow).Destroy();
	}

	~DrawingWindow()
	{
		Dispose(false);
	}

	// Set the background of the window to a solid color.
	void IToolkitWindow.SetBackground(System.Drawing.Color color)
	{
		if (color != background)
		{
			background = color;
			//If there was already a background brush then delete it
			if (backgroundBrush!=IntPtr.Zero)
				Win32.Api.DeleteObject(backgroundBrush);
			//Create the new cached brush
			backgroundBrush = DrawingSolidBrush.CreateSolidBrush(background);
			//Console.WriteLine("DrawingControlWindow.SetBackground, hwnd="+hwnd);
		}
	}

	void IToolkitWindow.MoveResize(int x, int y, int width, int height)
	{
		dimensions = new Rectangle(x, y, width, height);
		if (hwnd != IntPtr.Zero)
		{
			int leftAdjust, topAdjust, rightAdjust, bottomAdjust;
			Toolkit.GetWindowAdjust(out leftAdjust, out topAdjust, out rightAdjust, out bottomAdjust, flags);
			outsideDimensions = new Rectangle( x - leftAdjust, y - topAdjust, width + leftAdjust + rightAdjust, height + topAdjust + bottomAdjust);
			Win32.Api.SetWindowPos(hwnd, 0, outsideDimensions.Left, outsideDimensions.Top, outsideDimensions.Width, outsideDimensions.Height, Win32.Api.SetWindowsPosFlags.SWP_NOSENDCHANGING | Win32.Api.SetWindowsPosFlags.SWP_NOACTIVATE | Win32.Api.SetWindowsPosFlags.SWP_NOZORDER);
		}
		//Console.WriteLine("DrawingWindow.MoveResize, " + sink +",["+x+","+y+","+width+","+height+"]");
	}

	// This is the windows visibility according to OS.
	public virtual bool IsMapped
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
			if (hwnd != IntPtr.Zero)
				SetVisible();
			//Console.WriteLine("DrawingWindow.setIsMapped "+sink+",visible="+value);
		}
	}

	// This is actual visibility based on parent visiblity.
	private bool Visible
	{
		get
		{
			if (!visible)
				return false;
			else if (parent == null)
				return true;
			else
				return parent.Visible;
		}
	}

	// Force an update of all invalidated regions.
	void IToolkitWindow.Update()
	{
		if (hwnd == IntPtr.Zero)
			throw new ApplicationException("Can not update window, window not created yet");
		
		Win32.Api.UpdateWindow(hwnd);
		//Console.WriteLine("DrawingWindow.Update, hwnd="+hwnd);
	}

	// Deprecated
	void IToolkitWindow.SetForeground(Color color)
	{
	}

	// Get a toolkit graphics object for this window.
	IToolkitGraphics IToolkitWindow.GetGraphics()
	{
		if (hwnd == IntPtr.Zero)
			throw new ApplicationException("Can not get graphics object, window not created yet");
		
		//Console.WriteLine("DrawingWindow.GetGraphics, "+sink);
		return new DrawingGraphics (toolkit, Win32.Api.GetDC(hwnd));
	}

	// Get the toolkit parent window.
	IToolkitWindow IToolkitWindow.Parent
	{
		get
		{
			return parent as IToolkitWindow;
		}
	}

	System.IntPtr IToolkitWindow.GetHwnd()
	{
		return hwnd;
	}

	// Set the cursor.  The toolkit may ignore "frame" if it already
	// has a system-defined association for "cursorType".  Setting
	// "cursorType" to "ToolkitCursorType.InheritParent" will reset
	// the cursor to be the same as the parent window's.
	void IToolkitWindow.SetCursor(ToolkitCursorType cursorType, Frame frame)
	{
		// TODO
	}

	// Invalidate a rectangle within this window.
	public void Invalidate(int x, int y, int width, int height)
	{
		if (hwnd == IntPtr.Zero)
			throw new ApplicationException("Can not Invalidate, window not created yet");
		
		Win32.Api.RECT r;
		r.left = x;
		r.top = y;
		r.right = x + width;
		r.bottom = y + height;
		Win32.Api.InvalidateRect(hwnd, ref r, true);
		//Console.WriteLine("DrawingWindow.Invalidate, "+sink + " ["+x+","+y+","+width+","+height+"]");
	}

	// Invalidate this window.
	public void Invalidate()
	{
		if (hwnd != IntPtr.Zero)
			Win32.Api.InvalidateRect(hwnd, IntPtr.Zero, false);
		//Console.WriteLine("DrawingWindow.Invalidate, "+sink);
	}

	// This occurs when a top level window (form) receives focus)
	internal virtual int Activate(int wParam, int lParam)
	{
		//Console.WriteLine("DrawingWindow.Activate, " + sink + "," + ((Win32.Api.ActivateState)(wParam & 0xFF)).ToString());
		if ((Win32.Api.ActivateState)(wParam & 0xFF) == Win32.Api.ActivateState.WA_INACTIVE)
		{
			// Make sure we still dont hold the capture.
			CaptureTopLevel(null);
		}
		// We must handle this ourselves
		//Win32.Api.SetCursor(Win32.Api.LoadCursorA(IntPtr.Zero, Win32.Api.CursorName.IDC_ARROW));
		return 0;
	}

	internal virtual int MouseActivate(DrawingWindow activateWindow)
	{
		//Console.WriteLine("DrawingWindow.MouseActivate, " + sink +"," + activateWindow.sink);
		if (activateWindow is DrawingPopupWindow)
			return (int)Win32.Api.WM_MOUSEACTIVATEReturn.MA_NOACTIVATE;
		return (int)Win32.Api.WM_MOUSEACTIVATEReturn.MA_ACTIVATE;
	}

	// Set the capture to the top most parent of a window, or release the capture.
	private void CaptureTopLevel(DrawingWindow window)
	{
			if (window == null)
			{
				if (toolkit.capturedTopWindow != null)
				{
					//Console.WriteLine("CaptureTopLevel, Releasing " + toolkit.capturedTopWindow);
					Win32.Api.ReleaseCapture();
					toolkit.capturedTopWindow = null;
				}
			}
			else if (toolkit.capturedTopWindow != window.topOfhierarchy)
			{
				Win32.Api.ReleaseCapture();
				Win32.Api.SetCapture(window.topOfhierarchy.hwnd);
				//Console.WriteLine("CaptureTopLevel, Switching " + toolkit.capturedTopWindow + " to " + window.topOfhierarchy);
				toolkit.capturedTopWindow = window.topOfhierarchy;
			}
	}

	// Post Mouse Move to the window that needs it. Handle Mouse Enter and Leave
	protected internal virtual void MouseMove(int msg, int wParam, int lParam) 
	{
		
		ToolkitMouseButtons buttons = MapToToolkitMouseButtons(wParam);
		ToolkitKeys keys = MapMouseToToolkitKeys(wParam);
		int x, y;
		
		DrawingWindow actual;
		DrawingWindow found = FindDeepestChild(lParam, out x, out y, out actual);
		// Do Leave and Entered events.
		if (toolkit.enteredWindow != actual)
		{
			if (toolkit.enteredWindow != null)
			{
				//Console.WriteLine("DrawingWindow.MouseLeave, " + toolkit.enteredWindow.sink);
				toolkit.enteredWindow.sink.ToolkitMouseLeave();
				toolkit.enteredWindow = null;
			}
			if (actual != null)
			{
				//Console.WriteLine("DrawingWindow.MouseEnter, " + actual.sink);
				toolkit.enteredWindow = actual;
				actual.sink.ToolkitMouseEnter();
			}
		}
		
		// If there are no child windows to receive the captured input then give it to the window doing the capturing
		if (toolkit.capturedWindow != null && found == null)
			found = toolkit.capturedWindow;

		// If this is the first time over a window, set the capture and try again
		if (toolkit.capturedTopWindow == null)
		{
			if (toolkit.capturedWindow == null)
			{
				CaptureTopLevel(this);
				MouseMove(msg, wParam, lParam);
			}
			else
				CaptureTopLevel(toolkit.capturedWindow);
			return;
		}

		Win32.Api.POINT mouse;
		mouse.x= MouseX(lParam) + DimensionsScreen.X;
		mouse.y = MouseY(lParam) + DimensionsScreen.Y;
		DrawingWindow actualWindow = toolkit.DrawingWindow(Win32.Api.WindowFromPoint(mouse));

		if (actualWindow != null && actualWindow.topOfhierarchy == toolkit.capturedTopWindow)
		{
			// On the window decorations.
			if (found == null)
				CaptureTopLevel(null);
			else
				// Send the message to the client window
				found.sink.ToolkitMouseMove(buttons, keys, 0 ,x ,y, 0);
			//Console.WriteLine("DrawingWindow.MouseMove, " + (actual==null?"null":actual.sink.ToString()) +", (" + x +", " + y +"), " + buttons +"," + keys );
		}
		else
		{
			if (toolkit.capturedWindow == null)
				CaptureTopLevel(null);
			else
				found.sink.ToolkitMouseMove(buttons, keys,0 ,x ,y, 0);
			// ?Send the last mouse move beyond the window (Win NT+ behaviour)
		}
	}

	// Need to look at z-order so we can't do this ourselves yet..
	private DrawingWindow WindowFromPoint(int x, int y)
	{
		for (int i = toolkit.windowCount - 1; i >= 0; i--)
		{
			DrawingWindow w = toolkit.windows[i];
			DrawingTopLevelWindow top = w as DrawingTopLevelWindow;
			if (top != null)
			{
				if (top.outsideDimensions.Contains(x,y))
					return w;
			}
			else if (w.DimensionsScreen.Contains(x, y))
				return w;
		}
		return null;
	}
	
	// Returns the dimensions relative to the screen.
	private Rectangle DimensionsScreen
	{
		get
		{
			Rectangle r = new Rectangle(0, 0, dimensions.Width, dimensions.Height);
			for (DrawingWindow w = this; w != null; w = w.parent)
				r.Offset(w.dimensions.Location);
			return r;
		}
	}

	// Called when windows receives WM_MOUSEWHEEL
	internal void MouseWheel( int msg, int wParam, int lParam)
	{
		int wheelDelta = (wParam >> 16)/120;
		int x, y;
		DrawingWindow found = FindDeepestChild(lParam, out x, out y);

		if (found != null)
		{
			found.sink.ToolkitMouseWheel(ToolkitMouseButtons.None, MapMouseToToolkitKeys(wParam), 0, x , y, wheelDelta);
			//Console.WriteLine("DrawingWindow.MouseWheel " + found.sink + " [" + x + "," + y + "], key:" + MapMouseToToolkitKeys(wParam) + ", wheel:" + wheelDelta);
		}
	}

	protected virtual internal void ButtonDown(int msg, int wParam, int lParam)
	{
		int x,y;
		
		DrawingWindow found = FindDeepestChild(lParam, out x, out y);

		if (found != null)
		{
			found.sink.ToolkitMouseDown(MapToToolkitMouseButtons(wParam), MapMouseToToolkitKeys(wParam), 1 ,x ,y ,0);
			//Console.WriteLine("DrawingWindow.ButtonDown " + found.sink + " [" + x + "," + y + "], key:" + MapMouseToToolkitKeys(wParam) + ", button:" + MapToToolkitMouseButtons(wParam));
		}
		else
			CaptureTopLevel(null);
	}

	internal void ButtonUp(int msg, int wParam, int lParam)
	{
		int x,y;
		DrawingWindow found = FindDeepestChild(lParam, out x, out y);

		if (found != null)
		{
			found.sink.ToolkitMouseUp(MapToToolkitMouseButtons(wParam), MapMouseToToolkitKeys(wParam), 1 ,x ,y ,0);
			//Console.WriteLine("DrawingWindow.ButtonUp " + found.sink + " [" + x + "," + y + "], key:" + MapMouseToToolkitKeys(wParam) + ", button:" + MapToToolkitMouseButtons(wParam));
		}
	}

	internal void DoubleClick( int msg, int wParam, int lParam)
	{
		int x,y;
		DrawingWindow found = FindDeepestChild(lParam, out x, out y);

		if (found != null)
		{
			found.sink.ToolkitMouseDown(MapToToolkitMouseButtons(wParam), MapMouseToToolkitKeys(wParam), 2 ,x ,y ,0);
			//Console.WriteLine("DrawingWindow.DoubleClick " + found.sink + ",[" + x + "," + y + "], key:" + MapMouseToToolkitKeys(wParam) + ", button:" + MapToToolkitMouseButtons(wParam));
		}
	}

	// Returns the bottom child window that is underneath x, y and that has a top level window of capturedTopWindow or capturedWindow (if some window has captured the mouse). The window must be visible and toplevelwindows decorations are excluded. Returns x and y relative to the found windows coordinates.
	private DrawingWindow FindDeepestChild(int lParam, out int x, out int y, out DrawingWindow actualFound)
	{
		// Convert the mouse to screen coordinates
		Point pt = DimensionsScreen.Location;
		x = MouseX(lParam) + pt.X;
		y = MouseY(lParam) + pt.Y;

		DrawingWindow found = null;

		// Child windows lower down the hierarchy will always be last in the windows list
		for (int i = toolkit.windowCount - 1; i >= 0; i--)
		{
			DrawingWindow w = toolkit.windows[i];
			if (!w.visible || !w.Visible)
				continue;
			if (toolkit.capturedWindow == null)
			{
				if (toolkit.capturedTopWindow == w.topOfhierarchy)
				{
					// This will occur when the mouse is over the titlebar of a toplevel window.
					if (w.DimensionsScreen.Contains(x, y))
					{
						found = w;
						break;
					}
					else if (w is DrawingTopLevelWindow && w.outsideDimensions.Contains(x,y))
					{
						actualFound = null;
						return null;
					}
				}
			}
			else
			{
				if (toolkit.capturedWindow.IsParentOf(w) && w.DimensionsScreen.Contains(x,y))
				{
					found = w;
					break;
				}
			}
		}

		actualFound = found;
		// If there are no child windows to receive the captured input then give it to the window doing the capturing
		if (toolkit.capturedWindow != null && found == null)
			found = toolkit.capturedWindow;
	
		if (found != null)
		{
			// Adjust the coordinates relative to the "found" window.
			Rectangle child = found.DimensionsScreen;
			x -= child.X;
			y -= child.Y;
		}
		return found;
	}

	private DrawingWindow FindDeepestChild(int lParam, out int x, out int y)
	{
		DrawingWindow actual;
		return FindDeepestChild(lParam, out x, out y, out actual);
	}

  	internal bool KeyDown( int wParam, int lParam)
	{
		//Console.WriteLine("DrawingWindow.KeyDown " + sink +", " + (MapKeyToToolkitKeys( wParam)).ToString() + " " + wParam);
		return sink.ToolkitKeyDown(MapKeyToToolkitKeys( wParam));
	}

	internal bool SendBeginInvoke( int wParam )
	{
		sink.ToolkitBeginInvoke( (IntPtr)wParam );
		return true;
	}

	internal bool Char( int wParam, int lParam)
	{
		//Console.WriteLine("DrawingWindow.Char " + sink +", "+ ((char)wParam).ToString());
		return sink.ToolkitKeyChar((char)wParam);
	}
	
	internal bool KeyUp( int wParam, int lParam )
	{
		//Console.WriteLine("DrawingWindow.KeyUp " + sink +", "+ (MapKeyToToolkitKeys( wParam)).ToString());
		return sink.ToolkitKeyUp(MapKeyToToolkitKeys( wParam));
	}

	//TODO:
	internal void SetCursor( int cursor )
	{
		Win32.Api.SetCursor(Win32.Api.LoadCursorA(IntPtr.Zero, Win32.Api.CursorName.IDC_ARROW));
	}

	// WM_PAINT Message
	internal void Paint()
	{
		Win32.Api.PAINTSTRUCT myPS = new System.Drawing.Win32.Api.PAINTSTRUCT();
		hdc = Win32.Api.BeginPaint(hwnd, ref myPS );
		if( sink != null )
		{
			DrawingGraphics g = new DrawingGraphics(toolkit, hdc);
			Region clip = new Region(Rectangle.FromLTRB(myPS.rcPaintLeft, myPS.rcPaintTop, myPS.rcPaintRight, myPS.rcPaintBottom));
			System.Drawing.Graphics gr = ToolkitManager.CreateGraphics(g, clip);
			try
			{
				sink.ToolkitExpose(gr);
			}
			finally
			{
				// EndPaint deletes the hdc but that doesnt matter.
				Win32.Api.EndPaint( hwnd, ref myPS);
				gr.Dispose();
			}
			//Console.WriteLine( "DrawingWindow.Paint "+ sink +","+gr.ClipBounds.ToString());
		}
	}

	//WM_SETFOCUS occurs when either mouse or keyboard sets focus
	internal virtual void SetFocus()
	{
		if (sink != null)
			sink.ToolkitFocusEnter();
		//Console.WriteLine( "DrawingWindow.GotFocus "+sink);
	}

	//WM_KILLFOCUS occurs when either mouse or keyboard causes focus to be lost (or windows does)
	internal virtual void KillFocus()
	{
		if (sink != null)
			sink.ToolkitFocusLeave();
		//Console.WriteLine( "DrawingWindow.LostFocus "+sink) ;
	}

	//WM_WINDOWPOSCHANGING
	internal void WindowPosChanging(int lParam)
	{
		//When window is created - CreateWindow(), WindowPosChanging is called when the initial size is set
		//because sink==null, its size and position will be set
		if (sink != null)
		{
			//TODO Pointers are not available yet
			//Win32.Api.WINDOWPOS *pos = (Win32.Api.WINDOWPOS*)lParam;
			Win32.Api.WINDOWPOS pos = (Win32.Api.WINDOWPOS)Marshal.PtrToStructure(new IntPtr(lParam), typeof(Win32.Api.WINDOWPOS));
			if (suspendExternalMoveResizeNotify)
				return;
			// If moving
			if (((Win32.Api.SetWindowsPosFlags)pos.flags & Win32.Api.SetWindowsPosFlags.SWP_NOMOVE) == 0)
			{
				int leftAdjust, topAdjust, rightAdjust, bottomAdjust;
				Toolkit.GetWindowAdjust(out leftAdjust, out topAdjust, out rightAdjust, out bottomAdjust, flags);
				sink.ToolkitExternalMove(pos.x + leftAdjust, pos.y + topAdjust );
			}

			// If sizing
			if (((Win32.Api.SetWindowsPosFlags)pos.flags & Win32.Api.SetWindowsPosFlags.SWP_NOSIZE) ==0)
			{
				int leftAdjust, topAdjust, rightAdjust, bottomAdjust;
				Toolkit.GetWindowAdjust(out leftAdjust, out topAdjust, out rightAdjust, out bottomAdjust, flags);
				int width = pos.cx;
				int height = pos.cy;
				DrawingTopLevelWindow form = this as DrawingTopLevelWindow;
				// Restrict the size if necessary.
				if (form != null)
				{
					if (form.minimumSize != Size.Empty)
					{
						if (width < form.minimumSize.Width)
							width = form.minimumSize.Width;
						if (height < form.minimumSize.Height)
							height = form.minimumSize.Height;
					}
					if (form.maximumSize != Size.Empty)
					{
						if (width > form.maximumSize.Width)
							width = form.maximumSize.Width;
						if (height > form.maximumSize.Height)
							height = form.maximumSize.Height;
					}
				}
				sink.ToolkitExternalResize( width - leftAdjust - rightAdjust, height - topAdjust - bottomAdjust );
			}

			// Now prevent windows from changing the position or size, System.Windows.Control will do that
			
			//TODO Pointers are not available yet
			//pos.flags |= (uint)0x3;
			Marshal.WriteInt32(new IntPtr(lParam), 24, (int)pos.flags | 0x3);

		}
	}

	//WM_SETTINGSCHANGE occurs when some windows setting changes. This is used to notify the app that system settings have changed eg. button colors or form colors
	//We only want the RootTopLevelWindow to receive this
	internal virtual void SettingsChange(int wParam)
	{
	}

	internal virtual void Close()
	{
	}

	//Extract the mouse positions
	protected short MouseX( int value )
	{
		return (short) (0xFFFF & value);
	}

	protected short MouseY( int value )
	{
		return (short) (value >> 16);
	}

	//Map the win32 MouseKeyState to ToolkitMouseButtons
	private ToolkitMouseButtons MapToToolkitMouseButtons( int wParam )
	{
		ToolkitMouseButtons buttons = ToolkitMouseButtons.None; 
		Win32.Api.MouseKeyState fwKeys = (Win32.Api.MouseKeyState) (wParam & 0xFFFF);

		if ((fwKeys & Win32.Api.MouseKeyState.MK_LBUTTON)>0)
			buttons |= ToolkitMouseButtons.Left;

		if ((fwKeys & Win32.Api.MouseKeyState.MK_MBUTTON)>0)
			buttons |= ToolkitMouseButtons.Middle;

		if ((fwKeys & Win32.Api.MouseKeyState.MK_RBUTTON)>0)
			buttons |= ToolkitMouseButtons.Right;

		if ((fwKeys & Win32.Api.MouseKeyState.MK_XBUTTON1)>0)
			buttons |= ToolkitMouseButtons.XButton1;

		if ((fwKeys & Win32.Api.MouseKeyState.MK_XBUTTON2)>0)
			buttons |= ToolkitMouseButtons.XButton2;

		return buttons;
	}

	//Map the win32 MouseKeyState to ToolkitKeys. Alt is handled differently
	private ToolkitKeys MapMouseToToolkitKeys( int wParam )
	{
		ToolkitKeys keys = ToolkitKeys.None;
		Win32.Api.MouseKeyState fwKeys = (Win32.Api.MouseKeyState) (wParam & 0xFFFF);
		if ((fwKeys & Win32.Api.MouseKeyState.MK_CONTROL)>0)
			keys |= ToolkitKeys.Control;

		if ((fwKeys & Win32.Api.MouseKeyState.MK_SHIFT)>0)
			keys |= ToolkitKeys.Shift;

		if (Win32.Api.GetKeyState(Win32.Api.VirtualKeyType.VK_MENU) < 0)
			keys |= ToolkitKeys.Alt;
		
		return keys;

	}

	private ToolkitKeys MapKeyToToolkitKeys( int wParam)
	{
		ToolkitKeys key =  (ToolkitKeys)(wParam & 0xFFFF);
		if (Win32.Api.GetKeyState(Win32.Api.VirtualKeyType.VK_MENU) < 0)
			key |= ToolkitKeys.Alt;
		if (Win32.Api.GetKeyState(Win32.Api.VirtualKeyType.VK_CONTROL) < 0)
			key |= ToolkitKeys.Control;
		if (Win32.Api.GetKeyState(Win32.Api.VirtualKeyType.VK_SHIFT) < 0)
			key |= ToolkitKeys.Shift;
		return key;
	}

	// This is called to set whether a window is visible or not. Can only happen once the window is created.
	protected void SetVisible()
	{
		if (visible)
			Win32.Api.ShowWindow(hwnd,Win32.Api.ShowWindowCommand.SW_SHOWNA);
		else
			Win32.Api.ShowWindow(hwnd,Win32.Api.ShowWindowCommand.SW_HIDE);
	}

	abstract internal void CreateWindow();

	// Set the child right next to the parent in the heirarchy sure the parent window is before the child.
	// Set the correct TopOfHierarchy for all the childwindows.
	protected internal void ReparentOwn(DrawingWindow parent)
	{
		SetTopOfHierarchy();
		this.parent = parent;
		if (parent != null)
			MoveWindowTo(this, parent);
	}

	protected void SetTopOfHierarchy()
	{
		// Find all children windows and set their new top of hierarchy
		DrawingWindow newTopOfhierarchy;
		if (parent == null)
			newTopOfhierarchy = this;
		else
			newTopOfhierarchy = (parent as DrawingWindow).topOfhierarchy;
		for (int i = 0; i < toolkit.windowCount; i++)
		{
			DrawingWindow w = toolkit.windows[i];
			if (this.IsParentOf(w))
				w.topOfhierarchy = newTopOfhierarchy;
		}
	}

	// Move the window at position from to position to
	// Also move all the children in the from windows hierarchy
	protected void MoveWindowTo(DrawingWindow fromWindow, DrawingWindow toWindow)
	{
		// Find the from position.
		int from;
		for (from = 0; from < toolkit.windowCount; from++)
		{
			if (toolkit.windows[from] == fromWindow)
				break;
		}
		// Find the to position.
		int to;
		for (to = 0; to < toolkit.windowCount; to++)
		{
			if (toolkit.windows[to] == toWindow)
				break;
		}
		if(to == toolkit.windowCount)
		{
			to--;
		}

		if (from == to + 1 || from == to)
			return;
		DrawingWindow[] move = new DrawingWindow[16];
		int nextMove = 0;
		int toPosition = to;
		// Move all children of "win" into the move array.
		// Mark the children with null if they have been moved.
		for (int i = from; i < toolkit.windowCount; i++)
		{
			DrawingWindow current = toolkit.windows[i];
			if (fromWindow.IsParentOf(current) || current == fromWindow)
			{
				if (nextMove == move.Length)
				{
					DrawingWindow[] newMove = new DrawingWindow[move.Length * 2];
					move.CopyTo(newMove, 0);
					move = newMove;
				}
				toolkit.windows[i] = null;
				move[nextMove++] = current;
				if (i <= toPosition)
				{
					to--;
				}
			}
		}

		// Fill in the null "holes".
		int nonNullItem = from;
		for(int i = from; i < toolkit.windowCount - nextMove; i++)
		{
			DrawingWindow window = toolkit.windows[i];
			// Make sure we fill it with the next non null window.
			for(;; nonNullItem++)
			{
				if (toolkit.windows[nonNullItem] != null)
				{
					toolkit.windows[i] = toolkit.windows[nonNullItem++];
					break;
				}
			}
		}

		// Make room for the new windows.
		Array.Copy(toolkit.windows,to + 1, toolkit.windows, to + 1 + nextMove, toolkit.windowCount - 1 - to - nextMove);
		// Copy the move entries after the to location.
		Array.Copy(move, 0, toolkit.windows , to + 1, nextMove);
	}

	// Returns whether a DrawingWindow is the parent of this window or is the window
	protected bool IsParentOf(DrawingWindow window)
	{
		for (DrawingWindow w = window; w != null; w = w.parent)
			if (w == this)
				return true;
		return false;
	}

	void IToolkitWindow.SendBeginInvoke(IntPtr i_gch)
	{
		StaticSendBeginInvoke(hwnd,i_gch);
	}

	public static void StaticSendBeginInvoke(IntPtr hwnd_rcv, IntPtr i_gch)
	{
		Win32.Api.PostMessageA(hwnd_rcv,
			registeredBeginInvokeMessage,
			(int)i_gch,(int)0);
	}

}
}
