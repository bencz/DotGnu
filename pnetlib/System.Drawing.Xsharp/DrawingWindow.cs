/*
 * DrawingWindow.cs - Implementation of windows for System.Drawing.
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

internal sealed class DrawingWindow : InputOutputWidget, IToolkitWindow
{
	// Internal state.
	private IToolkit toolkit;
	private IToolkitEventSink sink;
	private ButtonName button;
	private bool hasCapture;
	private static Xsharp.Cursor[] cursors;


	// Constructor.
	public DrawingWindow(IToolkit toolkit, Widget parent,
						 int x, int y, int width, int height, IToolkitEventSink sink)
			: base(parent, x, y, width, height)
			{
				this.sink = sink;
				this.toolkit = toolkit;
				this.AutoMapChildren = false;
				this.DrawBackground = false;
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
				/*
				 * if the window is destroyed, set toolkit and sink to zero.
				 * otherwise, we couold get events like LostFocus...
				*/
				toolkit = null;
				sink    = null;
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
				DrawingToolkit.ValidateWindowPositionPaint(ref x, ref y);
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
				ModifyCursor(this, cursorType, frame);
			}


	// Map an Xsharp key description into a "ToolkitKeys" value.
	private static ToolkitKeys MapKey(KeyName key)
			{
				switch(key)
				{
					case KeyName.XK_BackSpace:		return ToolkitKeys.Back;
					case KeyName.XK_Tab:			return ToolkitKeys.Tab;
					case KeyName.XK_KP_Tab:			return ToolkitKeys.Tab;
					case KeyName.XK_ISO_Left_Tab:	return ToolkitKeys.Tab;
					case KeyName.XK_Linefeed:		return ToolkitKeys.LineFeed;
					case KeyName.XK_Clear:			return ToolkitKeys.Clear;
					case KeyName.XK_Return:			return ToolkitKeys.Enter;
					case KeyName.XK_KP_Enter:		return ToolkitKeys.Enter;
					case KeyName.XK_Pause:			return ToolkitKeys.Pause;
					case KeyName.XK_Scroll_Lock:	return ToolkitKeys.Scroll;
					case KeyName.XK_Escape:			return ToolkitKeys.Escape;
					case KeyName.XK_Delete:			return ToolkitKeys.Delete;
					case KeyName.XK_KP_Delete:		return ToolkitKeys.Delete;
					case KeyName.XK_Home:			return ToolkitKeys.Home;
					case KeyName.XK_KP_Home:		return ToolkitKeys.Home;
					case KeyName.XK_Begin:			return ToolkitKeys.Home;
					case KeyName.XK_KP_Begin:		return ToolkitKeys.Home;
					case KeyName.XK_Left:			return ToolkitKeys.Left;
					case KeyName.XK_KP_Left:		return ToolkitKeys.Left;
					case KeyName.XK_Up:				return ToolkitKeys.Up;
					case KeyName.XK_KP_Up:			return ToolkitKeys.Up;
					case KeyName.XK_Right:			return ToolkitKeys.Right;
					case KeyName.XK_KP_Right:		return ToolkitKeys.Right;
					case KeyName.XK_Down:			return ToolkitKeys.Down;
					case KeyName.XK_KP_Down:		return ToolkitKeys.Down;
					case KeyName.XK_Prior:			return ToolkitKeys.Prior;
					case KeyName.XK_KP_Prior:		return ToolkitKeys.Prior;
					case KeyName.XK_Next:			return ToolkitKeys.Next;
					case KeyName.XK_KP_Next:		return ToolkitKeys.Next;
					case KeyName.XK_End:			return ToolkitKeys.End;
					case KeyName.XK_KP_End:			return ToolkitKeys.End;
					case KeyName.XK_Select:			return ToolkitKeys.Select;
					case KeyName.XK_Print:			return ToolkitKeys.Print;
					case KeyName.XK_Execute:		return ToolkitKeys.Execute;
					case KeyName.XK_Insert:			return ToolkitKeys.Insert;
					case KeyName.XK_KP_Insert:		return ToolkitKeys.Insert;
					case KeyName.XK_Help:			return ToolkitKeys.Help;
					case KeyName.XK_Num_Lock:		return ToolkitKeys.NumLock;
					case KeyName.XK_space:			return ToolkitKeys.Space;
					case KeyName.XK_KP_Space:		return ToolkitKeys.Space;
					case KeyName.XK_F1:				return ToolkitKeys.F1;
					case KeyName.XK_KP_F1:			return ToolkitKeys.F1;
					case KeyName.XK_F2:				return ToolkitKeys.F2;
					case KeyName.XK_KP_F2:			return ToolkitKeys.F2;
					case KeyName.XK_F3:				return ToolkitKeys.F3;
					case KeyName.XK_KP_F3:			return ToolkitKeys.F3;
					case KeyName.XK_F4:				return ToolkitKeys.F4;
					case KeyName.XK_KP_F4:			return ToolkitKeys.F4;
					case KeyName.XK_F5:				return ToolkitKeys.F5;
					case KeyName.XK_F6:				return ToolkitKeys.F6;
					case KeyName.XK_F7:				return ToolkitKeys.F7;
					case KeyName.XK_F8:				return ToolkitKeys.F8;
					case KeyName.XK_F9:				return ToolkitKeys.F9;
					case KeyName.XK_F10:			return ToolkitKeys.F10;
					case KeyName.XK_F11:			return ToolkitKeys.F11;
					case KeyName.XK_F12:			return ToolkitKeys.F12;
					case KeyName.XK_F13:			return ToolkitKeys.F13;
					case KeyName.XK_F14:			return ToolkitKeys.F14;
					case KeyName.XK_F15:			return ToolkitKeys.F15;
					case KeyName.XK_F16:			return ToolkitKeys.F16;
					case KeyName.XK_F17:			return ToolkitKeys.F17;
					case KeyName.XK_F18:			return ToolkitKeys.F18;
					case KeyName.XK_F19:			return ToolkitKeys.F19;
					case KeyName.XK_F20:			return ToolkitKeys.F20;
					case KeyName.XK_F21:			return ToolkitKeys.F21;
					case KeyName.XK_F22:			return ToolkitKeys.F22;
					case KeyName.XK_F23:			return ToolkitKeys.F23;
					case KeyName.XK_F24:			return ToolkitKeys.F24;
					case KeyName.XK_KP_Multiply:	return ToolkitKeys.Multiply;
					case KeyName.XK_KP_Add:			return ToolkitKeys.Add;
					case KeyName.XK_KP_Separator:
							return ToolkitKeys.Separator;
					case KeyName.XK_KP_Subtract:	return ToolkitKeys.Subtract;
					case KeyName.XK_KP_Decimal:		return ToolkitKeys.Decimal;
					case KeyName.XK_KP_Divide:		return ToolkitKeys.Divide;
					case KeyName.XK_KP_0:			return ToolkitKeys.NumPad0;
					case KeyName.XK_KP_1:			return ToolkitKeys.NumPad1;
					case KeyName.XK_KP_2:			return ToolkitKeys.NumPad2;
					case KeyName.XK_KP_3:			return ToolkitKeys.NumPad3;
					case KeyName.XK_KP_4:			return ToolkitKeys.NumPad4;
					case KeyName.XK_KP_5:			return ToolkitKeys.NumPad5;
					case KeyName.XK_KP_6:			return ToolkitKeys.NumPad6;
					case KeyName.XK_KP_7:			return ToolkitKeys.NumPad7;
					case KeyName.XK_KP_8:			return ToolkitKeys.NumPad8;
					case KeyName.XK_KP_9:			return ToolkitKeys.NumPad9;
					case KeyName.XK_Shift_L:
							return ToolkitKeys.LShiftKey;
					case KeyName.XK_Shift_R:
							return ToolkitKeys.RShiftKey;
					case KeyName.XK_Control_L:
							return ToolkitKeys.LControlKey;
					case KeyName.XK_Control_R:
							return ToolkitKeys.RControlKey;
					case KeyName.XK_Meta_L:
					case KeyName.XK_Alt_L:
					case KeyName.XK_Super_L:
					case KeyName.XK_Hyper_L:
							return ToolkitKeys.LMenu;
					case KeyName.XK_Meta_R:
					case KeyName.XK_Alt_R:
					case KeyName.XK_Super_R:
					case KeyName.XK_Hyper_R:
							return ToolkitKeys.RMenu;
					case KeyName.XK_Caps_Lock:		return ToolkitKeys.CapsLock;
					case KeyName.XK_0:				return ToolkitKeys.D0;
					case KeyName.XK_1:				return ToolkitKeys.D1;
					case KeyName.XK_2:				return ToolkitKeys.D2;
					case KeyName.XK_3:				return ToolkitKeys.D3;
					case KeyName.XK_4:				return ToolkitKeys.D4;
					case KeyName.XK_5:				return ToolkitKeys.D5;
					case KeyName.XK_6:				return ToolkitKeys.D6;
					case KeyName.XK_7:				return ToolkitKeys.D7;
					case KeyName.XK_8:				return ToolkitKeys.D8;
					case KeyName.XK_9:				return ToolkitKeys.D9;
					case KeyName.XK_A:				return ToolkitKeys.A;
					case KeyName.XK_B:				return ToolkitKeys.B;
					case KeyName.XK_C:				return ToolkitKeys.C;
					case KeyName.XK_D:				return ToolkitKeys.D;
					case KeyName.XK_E:				return ToolkitKeys.E;
					case KeyName.XK_F:				return ToolkitKeys.F;
					case KeyName.XK_G:				return ToolkitKeys.G;
					case KeyName.XK_H:				return ToolkitKeys.H;
					case KeyName.XK_I:				return ToolkitKeys.I;
					case KeyName.XK_J:				return ToolkitKeys.J;
					case KeyName.XK_K:				return ToolkitKeys.K;
					case KeyName.XK_L:				return ToolkitKeys.L;
					case KeyName.XK_M:				return ToolkitKeys.M;
					case KeyName.XK_N:				return ToolkitKeys.N;
					case KeyName.XK_O:				return ToolkitKeys.O;
					case KeyName.XK_P:				return ToolkitKeys.P;
					case KeyName.XK_Q:				return ToolkitKeys.Q;
					case KeyName.XK_R:				return ToolkitKeys.R;
					case KeyName.XK_S:				return ToolkitKeys.S;
					case KeyName.XK_T:				return ToolkitKeys.T;
					case KeyName.XK_U:				return ToolkitKeys.U;
					case KeyName.XK_V:				return ToolkitKeys.V;
					case KeyName.XK_W:				return ToolkitKeys.W;
					case KeyName.XK_X:				return ToolkitKeys.X;
					case KeyName.XK_Y:				return ToolkitKeys.Y;
					case KeyName.XK_Z:				return ToolkitKeys.Z;
					case KeyName.XK_a:				return ToolkitKeys.A;
					case KeyName.XK_b:				return ToolkitKeys.B;
					case KeyName.XK_c:				return ToolkitKeys.C;
					case KeyName.XK_d:				return ToolkitKeys.D;
					case KeyName.XK_e:				return ToolkitKeys.E;
					case KeyName.XK_f:				return ToolkitKeys.F;
					case KeyName.XK_g:				return ToolkitKeys.G;
					case KeyName.XK_h:				return ToolkitKeys.H;
					case KeyName.XK_i:				return ToolkitKeys.I;
					case KeyName.XK_j:				return ToolkitKeys.J;
					case KeyName.XK_k:				return ToolkitKeys.K;
					case KeyName.XK_l:				return ToolkitKeys.L;
					case KeyName.XK_m:				return ToolkitKeys.M;
					case KeyName.XK_n:				return ToolkitKeys.N;
					case KeyName.XK_o:				return ToolkitKeys.O;
					case KeyName.XK_p:				return ToolkitKeys.P;
					case KeyName.XK_q:				return ToolkitKeys.Q;
					case KeyName.XK_r:				return ToolkitKeys.R;
					case KeyName.XK_s:				return ToolkitKeys.S;
					case KeyName.XK_t:				return ToolkitKeys.T;
					case KeyName.XK_u:				return ToolkitKeys.U;
					case KeyName.XK_v:				return ToolkitKeys.V;
					case KeyName.XK_w:				return ToolkitKeys.W;
					case KeyName.XK_x:				return ToolkitKeys.X;
					case KeyName.XK_y:				return ToolkitKeys.Y;
					case KeyName.XK_z:				return ToolkitKeys.Z;
				}
				return ToolkitKeys.None;
			}

	// Map an Xsharp key description into a "ToolkitKeys" value.
	internal static ToolkitKeys MapKey(KeyName key, ModifierMask modifiers)
			{
				ToolkitKeys toolkitKey = MapKey(key);
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

	// Override the button press event from Xsharp.
	protected override void OnButtonPress(int x, int y, ButtonName button,
									      ModifierMask modifiers)
			{
				if(sink != null)
				{
					sink.ToolkitMouseDown
						(MapButton(button),
						 MapKey(KeyName.XK_VoidSymbol, modifiers),
						 1, x, y, 0);
				}
				this.button = button;
			}

	// Override the button release event from Xsharp.
	protected override void OnButtonRelease(int x, int y, ButtonName button,
									  	    ModifierMask modifiers)
			{
				if(sink != null)
				{
					sink.ToolkitMouseUp
						(MapButton(button),
						 MapKey(KeyName.XK_VoidSymbol, modifiers),
						 1, x, y, 0);
				}
				this.button = 0;
			}
			
	// Override the mouse wheel event from Xsharp.
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
				this.button = 0;

				return true;
			}			

	// Override the button double click event from Xsharp.
	protected override void OnButtonDoubleClick
				(int x, int y, ButtonName button, ModifierMask modifiers)
			{
				if(sink != null)
				{
					sink.ToolkitMouseDown
						(MapButton(button),
						 MapKey(KeyName.XK_VoidSymbol, modifiers),
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
						(MapButton(button),
						 MapKey(KeyName.XK_VoidSymbol, modifiers),
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
					ToolkitKeys keyData = MapKey(key, modifiers);
					if(sink.ToolkitKeyDown(keyData))
					{
						processed = true;
					}

					// Emit the "KeyChar" event if necessary.
					if(str != null)
					{
						foreach(char ch in str)
						{
							// Clean old ASCII DEL (0x7F)
							if(ch != (char)0x7F)
							{
								if( sink != null ) {  // window might be closed by sink.ToolkitKeyDown
									if(sink.ToolkitKeyChar(ch))
									{
										processed = true;
									}
								}
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
					ToolkitKeys keyData = MapKey(key, modifiers);
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

	// Handle a paint event from Xsharp.
	protected override void OnPaint(Xsharp.Graphics graphics)
			{
				if(sink != null)
				{
					System.Drawing.Region clip = RegionToDrawingRegion(graphics.ExposeRegion);
					DrawingGraphics g = new DrawingGraphics(toolkit, graphics);
					using(System.Drawing.Graphics gr =
								ToolkitManager.CreateGraphics(g, clip))
					{
						sink.ToolkitExpose(gr);
					}
				}
			}

	// Convert an Xsharp.Region to System.Drawing.Region
	internal static System.Drawing.Region RegionToDrawingRegion
				(Xsharp.Region region)
			{
				Xsharp.Rectangle[] rectangles = region.GetRectangles();
				System.Drawing.Region newRegion = new System.Drawing.Region();
				newRegion.MakeEmpty();
				for(int i = 0; i < rectangles.Length; i++)
				{
					Xsharp.Rectangle rect = rectangles[i];
					newRegion.Union(new System.Drawing.Rectangle
							(rect.x, rect.y, rect.width, rect.height));
				}
				return newRegion;
			}

	// Modify the cursor on a widget.
	internal static void ModifyCursor
				(Widget widget, ToolkitCursorType cursorType, Frame frame)
			{
				if(cursorType == ToolkitCursorType.InheritParent)
				{
					// Change the cursor back to the same value as the parent.
					widget.Cursor = null;
				}
				else if(cursorType == ToolkitCursorType.Default)
				{
					// Set the default cursor to something matching X.
					widget.Cursor = new Cursor(CursorType.XC_left_ptr);
				}
				else
				{
					// Create a cursor based on the supplied image.
					if(frame != null &&
									 /* irgnore pixel format !
									 frame.PixelFormat == DotGNU.Images.PixelFormat.Format1bppIndexed 
									 &&
									 */
					   frame.Mask != null)
					{
						if(cursorType != ToolkitCursorType.UserDefined)
						{
							lock(typeof(DrawingWindow))
							{
								if(cursors == null)
								{
									cursors = new Xsharp.Cursor [32];
								}
								Xsharp.Cursor curs = cursors[(int)cursorType];
								if(curs == null)
								{
									cursors[(int)cursorType] = curs =
										new Xsharp.Cursor
											(widget.Screen, frame);
								}
								widget.Cursor = curs;
							}
						}
						else
						{
							widget.Cursor = new Xsharp.Cursor
								(widget.Screen, frame);
						}
						return;
					}

					// Last ditch attempt - use the nearest X11 cursor.
					CursorType type;
					switch(cursorType)
					{
						case ToolkitCursorType.AppStarting:
							type = CursorType.XC_watch;
							break;
						case ToolkitCursorType.Arrow:
						case ToolkitCursorType.Default:
							type = CursorType.XC_left_ptr;
							break;
						case ToolkitCursorType.Cross:
							type = CursorType.XC_crosshair;
							break;
						case ToolkitCursorType.IBeam:
							type = CursorType.XC_xterm;
							break;
						case ToolkitCursorType.No:
							type = CursorType.XC_X_cursor;
							break;
						case ToolkitCursorType.SizeAll:
							type = CursorType.XC_fleur;
							break;
						case ToolkitCursorType.SizeNS:
						case ToolkitCursorType.VSplit:
							type = CursorType.XC_sb_v_double_arrow;
							break;
						case ToolkitCursorType.SizeWE:
						case ToolkitCursorType.HSplit:
							type = CursorType.XC_sb_h_double_arrow;
							break;
						case ToolkitCursorType.UpArrow:
							type = CursorType.XC_sb_up_arrow;
							break;
						case ToolkitCursorType.WaitCursor:
							type = CursorType.XC_watch;
							break;
						case ToolkitCursorType.Help:
							type = CursorType.XC_question_arrow;
							break;
						case ToolkitCursorType.Hand:
							type = CursorType.XC_hand2;
							break;
						case ToolkitCursorType.SizeNWSE:
						case ToolkitCursorType.SizeNESW:
						case ToolkitCursorType.NoMove2D:
						case ToolkitCursorType.NoMoveHoriz:
						case ToolkitCursorType.NoMoveVert:
						case ToolkitCursorType.PanEast:
						case ToolkitCursorType.PanNE:
						case ToolkitCursorType.PanNorth:
						case ToolkitCursorType.PanNW:
						case ToolkitCursorType.PanSE:
						case ToolkitCursorType.PanSouth:
						case ToolkitCursorType.PanSW:
						case ToolkitCursorType.PanWest:
						case ToolkitCursorType.UserDefined:
						default:
							type = CursorType.XC_circle;
							break;
					}
					widget.Cursor = new Cursor(type);
				}
			}

	void IToolkitWindow.SendBeginInvoke(IntPtr i_gch)
			{
				base.SendBeginInvoke(i_gch);
			}
			
	protected override void OnBeginInvokeMessage(IntPtr i_gch)
			{
				if( sink != null )
					sink.ToolkitBeginInvoke(i_gch);
			}

}; // class DrawingWindow

}; // namespace System.Drawing.Toolkit
