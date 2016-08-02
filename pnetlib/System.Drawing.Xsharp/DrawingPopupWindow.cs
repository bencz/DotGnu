/*
 * DrawingPopupWindow.cs - Implementation of popup windows for System.Drawing.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Toolkit;
using DotGNU.Images;
using Xsharp;

internal sealed class DrawingPopupWindow : PopupWindow, IToolkitWindow
{
	// Internal state.
	private IToolkit toolkit;
	private IToolkitEventSink sink;
	private bool hasCapture;

	// Constructor.
	public DrawingPopupWindow
				(IToolkit toolkit, int x, int y, int width, int height,
				 IToolkitEventSink sink)
			: base(x, y, width, height)
			{
				this.sink = sink;
				this.toolkit = toolkit;
				this.AutoMapChildren = false;
			}

	// Get the toolkit that owns this window.
	public IToolkit Toolkit
			{
				get
				{
					return toolkit;
				}
			}

	// Get the toolkit parent window.
	IToolkitWindow IToolkitWindow.Parent
			{
				get
				{
					return (IToolkitWindow)Parent;
				}
			}

	// Get the current dimensions of this window.
	public System.Drawing.Rectangle Dimensions
			{
				get
				{
					return new System.Drawing.Rectangle(X, Y, Width, Height);
				}
			}

	// Get or set the mapped state of the window.
	bool IToolkitWindow.IsMapped
			{
				get
				{
					return IsMapped;
				}
				set
				{
					IsMapped = value;
				}
			}

	// Determine if this window currently has the input focus.
	bool IToolkitWindow.Focused
			{
				get
				{
					return Focused;
				}
			}

	// Get or set the mouse capture on this window.  Mouse captures
	// typically aren't required in the same place where Windows
	// needs them.  It is also highly dangerous to allow X applications
	// to capture the mouse without very careful thought.
	bool IToolkitWindow.Capture
			{
				get
				{
					return hasCapture;
				}
				set
				{
					hasCapture = value;
				}
			}

	// Set the focus to this window
	void IToolkitWindow.Focus()
			{
				RequestFocus();
			}

	// Destroy this window and all of its children.
	void IToolkitWindow.Destroy()
			{
				Destroy();
			}

	// Move or resize this window.
	void IToolkitWindow.MoveResize(int x, int y, int width, int height)
			{
				DrawingToolkit.ValidateWindowPosition(ref x, ref y);
				DrawingToolkit.ValidateWindowSize(ref width, ref height);
				Move(x, y);
				Resize(width, height);
			}

	// Raise this window respective to its siblings.
	void IToolkitWindow.Raise()
			{
				Raise();
			}

	// Lower this window respective to its siblings.
	void IToolkitWindow.Lower()
			{
				Lower();
			}

	// Reparent this window to underneath a new parent.
	void IToolkitWindow.Reparent(IToolkitWindow parent, int x, int y)
			{
				if(parent == null)
				{
					Reparent(((DrawingToolkit)Toolkit).placeholder, x, y);
				}
				else
				{
					Reparent((Widget)parent, x, y);
				}
			}

	// Get a toolkit graphics object for this window.
	IToolkitGraphics IToolkitWindow.GetGraphics()
			{
				return new DrawingGraphics
					(toolkit, new Xsharp.Graphics(this));
			}

	// Set the foreground of the window to a solid color.
	void IToolkitWindow.SetForeground(System.Drawing.Color color)
			{
				Foreground = DrawingToolkit.DrawingToXColor(color);
			}

	// Set the background of the window to a solid color.
	void IToolkitWindow.SetBackground(System.Drawing.Color color)
			{
				if(color.A < 128)
				{
					BackgroundPixmap = null;
				}
				else
				{
					Background = DrawingToolkit.DrawingToXColor(color);
				}
			}

	[TODO]
	// Move this window to above one of its siblings.
	void IToolkitWindow.MoveToAbove(IToolkitWindow sibling)
			{
				return;
			}

	[TODO]
	// Move this window to below one of its siblings.
	void IToolkitWindow.MoveToBelow(IToolkitWindow sibling)
			{
				return;
			}

	// Get the HWND for this window.  IntPtr.Zero if not supported.
	IntPtr IToolkitWindow.GetHwnd()
			{
				return new IntPtr( (int)GetWidgetHandle() );
			}

	// Invalidate this window.
	void IToolkitWindow.Invalidate()
			{
				Repaint();
			}

	// Invalidate a rectangle within this window.
	void IToolkitWindow.Invalidate(int x, int y, int width, int height)
			{
				DrawingToolkit.ValidateWindowPosition(ref x, ref y);
				DrawingToolkit.ValidateWindowSize(ref width, ref height);
				Repaint(x, y, width, height);
			}

	// Force an update of all invalidated regions.
	void IToolkitWindow.Update()
			{
				Update(false);
				Display.Flush();
			}

	// Set the cursor.  The toolkit may ignore "frame" if it already
	// has a system-defined association for "cursorType".  Setting
	// "cursorType" to "ToolkitCursorType.InheritParent" will reset
	// the cursor to be the same as the parent window's.
	void IToolkitWindow.SetCursor(ToolkitCursorType cursorType, Frame frame)
			{
				DrawingWindow.ModifyCursor(this, cursorType, frame);
			}

	// Override the button press event from Xsharp.
	protected override void OnButtonPress(int x, int y, ButtonName button,
									      ModifierMask modifiers)
			{
				if(sink != null)
				{
					sink.ToolkitMouseDown
						(DrawingWindow.MapButton(button),
						 DrawingWindow.MapKey(KeyName.XK_VoidSymbol, modifiers),
						 1, x, y, 0);
				}
			}

	// Override the button release event from Xsharp.
	protected override void OnButtonRelease(int x, int y, ButtonName button,
									  	    ModifierMask modifiers)
			{
				if(sink != null)
				{
					sink.ToolkitMouseUp
						(DrawingWindow.MapButton(button),
						 DrawingWindow.MapKey(KeyName.XK_VoidSymbol, modifiers),
						 1, x, y, 0);
				}
			}

	// Override the button double click event from Xsharp.
	protected override void OnButtonDoubleClick
				(int x, int y, ButtonName button, ModifierMask modifiers)
			{
				if(sink != null)
				{
					sink.ToolkitMouseDown
						(DrawingWindow.MapButton(button),
						 DrawingWindow.MapKey(KeyName.XK_VoidSymbol, modifiers),
						 2, x, y, 0);
				}
			}

	// Override the pointer motion event from Xsharp.
	protected override void OnPointerMotion
				(int x, int y, ModifierMask modifiers)
			{
				if(sink != null)
				{
					sink.ToolkitMouseMove
						(ToolkitMouseButtons.None,
						 DrawingWindow.MapKey(KeyName.XK_VoidSymbol, modifiers),
						 0, x, y, 0);
				}
			}

	// Override the key press event from Xsharp.
	protected override bool OnKeyPress(KeyName key,
									   ModifierMask modifiers, String str)
			{
				bool processed = false;
				if(sink != null)
				{
					// Emit the "KeyDown" event.
					ToolkitKeys keyData = DrawingWindow.MapKey(key, modifiers);
					if(keyData != ToolkitKeys.None)
					{
						if(sink.ToolkitKeyDown(keyData))
						{
							processed = true;
						}
					}

					// Emit the "KeyChar" event if necessary.
					if(str != null)
					{
						foreach(char ch in str)
						{
							if(sink.ToolkitKeyChar(ch))
							{
								processed = true;
							}
						}
					}
				}
				return processed;
			}

	// Override the key release event from Xsharp.
	protected override bool OnKeyRelease(KeyName key, ModifierMask modifiers)
			{
				if(sink != null)
				{
					// Emit the "KeyUp" event.
					ToolkitKeys keyData = DrawingWindow.MapKey(key, modifiers);
					if(keyData != ToolkitKeys.None)
					{
						return sink.ToolkitKeyUp(keyData);
					}
				}
				return false;
			}

	// Override the mouse enter event from Xsharp.
	protected override void OnEnter(Widget child, int x, int y,
								    ModifierMask modifiers,
								    CrossingMode mode,
								    CrossingDetail detail)
			{
				if(sink != null)
				{
					sink.ToolkitMouseEnter();
				}
			}

	// Override the mouse leave event from Xsharp.
	protected override void OnLeave(Widget child, int x, int y,
								    ModifierMask modifiers,
								    CrossingMode mode,
								    CrossingDetail detail)
			{
				if(sink != null)
				{
					sink.ToolkitMouseLeave();
				}
			}

	// Override the focus enter event from Xsharp.
	protected override void OnFocusIn(Widget other)
			{
				if(sink != null)
				{
					sink.ToolkitFocusEnter();
				}
			}

	// Override the focus leave event from Xsharp.
	protected override void OnFocusOut(Widget other)
			{
				if(sink != null)
				{
					sink.ToolkitFocusLeave();
				}
			}

	// Handle a paint event from Xsharp.
	protected override void OnPaint(Xsharp.Graphics graphics)
			{
				if(sink != null)
				{
					System.Drawing.Region clip = DrawingWindow.RegionToDrawingRegion
									(graphics.ExposeRegion); 
					DrawingGraphics g = new DrawingGraphics(toolkit, graphics);
					using(System.Drawing.Graphics gr =
							  ToolkitManager.CreateGraphics(g, clip))
					{
						sink.ToolkitExpose(gr);
					}
				}
			}


	void IToolkitWindow.SendBeginInvoke(IntPtr i_gch)
		{
			base.SendBeginInvoke(i_gch);
		}

}; // class DrawingPopupWindow

}; // namespace System.Drawing.Toolkit
