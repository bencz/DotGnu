/*
 * DrawingTopLevelWindow.cs - Implementation of windows for System.Drawing.
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

internal sealed class DrawingTopLevelWindow
	: TopLevelWindow, IToolkitTopLevelWindow
{
	// Internal state.
	private IToolkit toolkit;
	private IToolkitEventSink sink;
	private bool hasCapture;

	// Constructors.
	public DrawingTopLevelWindow(IToolkit toolkit, String name,
						 		 int width, int height, IToolkitEventSink sink)
			: base(name, width, height)
			{
				this.sink = sink;
				this.toolkit = toolkit;
				this.AutoMapChildren = false;
			}
	public DrawingTopLevelWindow(Widget parent, String name,
						 		 int x, int y, int width, int height)
			: base(parent, name, x, y, width, height)
			{
				this.toolkit = ((IToolkitWindow)(parent.Parent)).Toolkit;
				this.AutoMapChildren = false;
			}

	// Set the sink.
	public void SetSink(IToolkitEventSink sink)
			{
				this.sink = sink;
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

	// Set the focus to this window.
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

	// Move this window to above one of its siblings.
	void IToolkitWindow.MoveToAbove(IToolkitWindow sibling)
			{
				// Move this window below the sibling widget.
				MoveToAbove(sibling as Widget);
			}

	// Move this window to below one of its siblings.
	void IToolkitWindow.MoveToBelow(IToolkitWindow sibling)
			{
				// Move this window below the sibling widget.
				MoveToBelow(sibling as Widget);
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
				Repaint(x, y, width + 1, height + 1);
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

	// Iconify the window.
	void IToolkitTopLevelWindow.Iconify()
			{
				Iconify();
			}

	// Maximize the window.
	void IToolkitTopLevelWindow.Maximize()
			{
				Maximize();
				if(IsIconic)
				{
					Deiconify();
				}
			}

	// Restore the window from its iconified or maximized state.
	void IToolkitTopLevelWindow.Restore()
			{
				if(IsIconic)
				{
					Deiconify();
				}
				if(IsMaximized)
				{
					Restore();
				}
			}

	// Set the owner for modal and modeless dialog support.
	void IToolkitTopLevelWindow.SetDialogOwner(IToolkitTopLevelWindow owner)
			{
				TransientFor = (owner as TopLevelWindow);
			}

	// Set this window's icon.
	void IToolkitTopLevelWindow.SetIcon(Icon icon)
			{
				DotGNU.Images.Frame frame = ToolkitManager.GetImageFrame(icon);
				Xsharp.Image origIcon = Icon;
				if(frame != null)
				{
					Icon = new Xsharp.Image(Screen, frame);
				}
				else
				{
					Icon = null;
				}
				if(origIcon != null)
				{
					origIcon.Dispose();
				}
			}

	// Set this window's maximum size.
	void IToolkitTopLevelWindow.SetMaximumSize(Size size)
			{
				SetMaximumSize(DrawingGraphics.RestrictXY(size.Width),
							   DrawingGraphics.RestrictXY(size.Height));
			}

	// Set this window's minimum size.
	void IToolkitTopLevelWindow.SetMinimumSize(Size size)
			{
				SetMinimumSize(DrawingGraphics.RestrictXY(size.Width),
							   DrawingGraphics.RestrictXY(size.Height));
			}

	// Set the window title (top-level windows only).
	void IToolkitTopLevelWindow.SetTitle(String title)
			{
				if(title == null)
				{
					title = String.Empty;
				}
				Name = title;
			}

	// Change the set of supported window decorations and functions.
	void IToolkitTopLevelWindow.SetWindowFlags(ToolkitWindowFlags flags)
			{
				// Set the default hint flags.
				MotifDecorations decorations = 0;
				MotifFunctions functions = 0;
				MotifInputType inputType = MotifInputType.Normal;
				OtherHints otherHints = OtherHints.None;

				// Alter decorations according to the window flags.
				if((flags & ToolkitWindowFlags.Close) != 0)
				{
					functions |= MotifFunctions.Close;
				}
				if((flags & ToolkitWindowFlags.Minimize) != 0)
				{
					decorations |= MotifDecorations.Minimize;
					functions |= MotifFunctions.Minimize;
				}
				if((flags & ToolkitWindowFlags.Caption) != 0)
				{
					decorations |= MotifDecorations.Title;
				}
				if((flags & ToolkitWindowFlags.Border) != 0)
				{
					decorations |= MotifDecorations.Border;
				}
				if((flags & ToolkitWindowFlags.ResizeHandles) != 0)
				{
					decorations |= MotifDecorations.ResizeHandles;
				}
				if((flags & ToolkitWindowFlags.Menu) != 0)
				{
					decorations |= MotifDecorations.Menu;
				}
				if((flags & ToolkitWindowFlags.Resize) != 0)
				{
					decorations |= MotifDecorations.Maximize |
								   MotifDecorations.ResizeHandles;
					functions |= MotifFunctions.Maximize |
								 MotifFunctions.Resize;
				}
				if((flags & ToolkitWindowFlags.Move) != 0)
				{
					functions |= MotifFunctions.Move;
				}
				if((flags & ToolkitWindowFlags.Modal) != 0)
				{
					inputType = MotifInputType.ApplicationModal;
				}
				if((flags & ToolkitWindowFlags.ToolWindow) != 0)
				{
					otherHints |= OtherHints.ToolWindow;
				}
				else if((flags & ToolkitWindowFlags.Dialog) != 0)
				{
					otherHints |= OtherHints.Dialog;
				}
				if((flags & ToolkitWindowFlags.ShowInTaskbar) == 0)
				{
					otherHints |= OtherHints.HideFromTaskBar;
				}
				if((flags & ToolkitWindowFlags.Help) != 0)
				{
					otherHints |= OtherHints.HelpButton;
				}
				if((flags & ToolkitWindowFlags.TopMost) != 0)
				{
					otherHints |= OtherHints.TopMost;
				}

				// Remove the "transient for" hint if we are changing a
				// modal form back into a regular form.
				if(InputType == MotifInputType.ApplicationModal &&
				   inputType == MotifInputType.Normal)
				{
					RemoveTransientFor();
				}

				// Send the Motif flags to the window manager.
				Decorations = decorations;
				Functions = functions;
				InputType = inputType;
				OtherHints = otherHints;
			}

	void IToolkitTopLevelWindow.SetOpacity(double opacity)
			{
				Opacity = opacity;
			}

	protected override void OnBeginInvokeMessage(IntPtr i_gch)
			{
				if( sink != null )
					sink.ToolkitBeginInvoke(i_gch);
			}

	// Override the button press event from Xsharp.
	protected override void OnButtonPress(int x, int y, ButtonName button,
									      ModifierMask modifiers)
			{
				if(sink != null)
				{
					sink.ToolkitMouseDown
						(DrawingWindow.MapButton(button),
						 DrawingWindow.MapKey
						 	(KeyName.XK_VoidSymbol, modifiers),
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
						 DrawingWindow.MapKey
						 	(KeyName.XK_VoidSymbol, modifiers),
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
						 DrawingWindow.MapKey
						 	(KeyName.XK_VoidSymbol, modifiers),
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
						 DrawingWindow.MapKey
						 	(KeyName.XK_VoidSymbol, modifiers),
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
					if(sink.ToolkitKeyDown(keyData))
					{
						processed = true;
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
					return sink.ToolkitKeyUp(keyData);
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

	// Override the primary focus enter event from Xsharp.
	protected override void OnPrimaryFocusIn()
			{
				base.OnPrimaryFocusIn();
				if(sink != null)
				{
					sink.ToolkitPrimaryFocusEnter();
				}
			}

	// Override the primary focus leave event from Xsharp.
	protected override void OnPrimaryFocusOut()
			{
				base.OnPrimaryFocusOut();
				if(sink != null)
				{
					sink.ToolkitPrimaryFocusLeave();
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

	// Override the resize event from Xsharp.
	protected override void OnMoveResize(int x, int y, int width, int height)
			{
				base.OnMoveResize(x, y, width, height);
				if(sink != null)
				{
					sink.ToolkitExternalMove(x, y);
					sink.ToolkitExternalResize(width, height);
				}
			}

	// Override the "Close" event from Xsharp.  We pass control to
	// the event sink to deal with it, and avoid calling the base.
	public override bool Close()
			{
				if(sink != null)
				{
					sink.ToolkitClose();
				}
				return false;
			}

	// Override the "OnHelp" event from Xsharp.
	protected override void OnHelp()
			{
				if(sink != null)
				{
					sink.ToolkitHelp();
				}
			}

	// Process a change in window state from Xsharp.
	private void WindowStateChanged()
			{
				int state;
				if(IsIconic)
				{
					state = 1;		// FormWindowState.Minimized.
				}
				else if(IsMaximized)
				{
					state = 2;		// FormWindowState.Maximized.
				}
				else
				{
					state = 0;		// FormWindowState.Normal.
				}
				if(sink != null)
				{
					sink.ToolkitStateChanged(state);
				}
			}

	// Override the "OnIconicStateChanged" event from Xsharp.
	protected override void OnIconicStateChanged(bool value)
			{
				WindowStateChanged();
			}

	// Override the "OnMaximizedStateChanged" event from Xsharp.
	protected override void OnMaximizedStateChanged(bool value)
			{
				WindowStateChanged();
			}

	void IToolkitWindow.SendBeginInvoke(IntPtr i_gch)
			{
				base.SendBeginInvoke(i_gch);
			}
			
	protected override bool OnButtonWheel(int x, int y, ButtonName button,
									  	   ModifierMask modifiers, int iDelta)
			{
				if(sink != null)
				{
					sink.ToolkitMouseWheel
						(MapButton(button),
						MapKey(KeyName.XK_VoidSymbol, modifiers),
						1, x, y, iDelta);
				}

				return true;
			}
			
	// Map an Xsharp key description into a "ToolkitKeys" value.
	internal static ToolkitKeys MapKey(KeyName key, ModifierMask modifiers)
			{
				ToolkitKeys toolkitKey =  ToolkitKeys.None;
				if((modifiers & ModifierMask.ControlMask) != 0)
				{
					toolkitKey |= ToolkitKeys.Control;
				}
				if((modifiers & ModifierMask.ShiftMask) != 0)
				{
					toolkitKey |= ToolkitKeys.Shift;
				}
				if((modifiers & ModifierMask.Mod1Mask) != 0)
				{
					toolkitKey |= ToolkitKeys.Alt;
				}
				return toolkitKey;
			}

	// Map an Xsharp button name into a "ToolkitMouseButtons" value.
	internal static ToolkitMouseButtons MapButton(ButtonName button)
			{
				switch(button)
				{
					case ButtonName.Button1:
						return ToolkitMouseButtons.Left;
					case ButtonName.Button2:
						return ToolkitMouseButtons.Middle;
					case ButtonName.Button3:
						return ToolkitMouseButtons.Right;
					case ButtonName.Button4:
						return ToolkitMouseButtons.XButton1;
					case ButtonName.Button5:
						return ToolkitMouseButtons.XButton2;
					default:
						return ToolkitMouseButtons.None;
				}
			}	

}; // class DrawingTopLevelWindow

}; // namespace System.Drawing.Toolkit
