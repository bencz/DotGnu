/*
 * InputOnlyWidget.cs - Widget handling for input-only widgets.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace Xsharp
{

using System;
using System.Runtime.InteropServices;
using Xsharp.Types;
using Xsharp.Events;
using OpenSystem.Platform.X11;

/// <summary>
/// <para>The <see cref="T:Xsharp.InputOnlyWidget"/> class manages widgets
/// that occupy screen real estate for the purpose of keyboard and pointer
/// handling, but which do not have output functionality.</para>
/// </summary>
public class InputOnlyWidget : Widget
{
	// Internal state.
	private bool focusable;
	internal XTime lastClickTime;
	internal ButtonName lastClickButton;

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.InputOnlyWidget"/>
	/// instance underneath a specified parent widget.</para>
	/// </summary>
	///
	/// <param name="parent">
	/// <para>The parent of the new widget.</para>
	/// </param>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left corner of
	/// the new widget.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left corner of
	/// the new widget.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the new widget.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the new widget.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="parent"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if <paramref name="x"/>, <paramref name="y"/>,
	/// <paramref name="width"/>, or <paramref name="height"/> are
	/// out of range.</para>
	/// </exception>
	///
	/// <exception cref="T.Xsharp.XInvalidOperationException">
	/// <para>Raised if <paramref name="parent"/> is disposed or the
	/// root window.</para>
	/// </exception>
	public InputOnlyWidget(Widget parent, int x, int y, int width, int height)
			: base(GetDisplay(parent, false), GetScreen(parent),
				   DrawableKind.InputOnlyWidget, parent)
			{
				bool ok = false;
				try
				{
					// Validate the position and size.
					if(x < -32768 || x > 32767 ||
					   y < -32768 || y > 32767)
					{
						throw new XException(S._("X_InvalidPosition"));
					}
					if(width < 1 || width > 32767 ||
					   height < 1 || height > 32767 ||
					   !ValidateSize(width, height))
					{
						throw new XException(S._("X_InvalidSize"));
					}

					// Set the initial position and size of the widget.
					this.x = x;
					this.y = y;
					this.width = width;
					this.height = height;
					this.focusable = true;

					// Lock down the display and create the window handle.
					try
					{
						IntPtr display = dpy.Lock();
						XWindow pwindow = parent.GetWidgetHandle();
						XWindow window = Xlib.XCreateWindow
								(display, pwindow,
								 x, y, (uint)width, (uint)height, (uint)0,
								 0 /* depth */, 2 /* InputOnly */,
								 screen.DefaultVisual,
								 (uint)0, IntPtr.Zero);
						SetWidgetHandle(window);
						if(parent.AutoMapChildren)
						{
							Xlib.XMapWindow(display, window);
							mapped = true;
						}
					}
					finally
					{
						dpy.Unlock();
					}

					// Push the widget down to the default layer.
					layer = 0x7FFFFFFF;
					Layer = 0;
					ok = true;

					// Select for mouse events.
					SelectInput(EventMask.ButtonPressMask |
								EventMask.ButtonReleaseMask |
								EventMask.EnterWindowMask |
								EventMask.LeaveWindowMask |
								EventMask.PointerMotionMask);
				}
				finally
				{
					if(!ok)
					{
						// Creation failed, so detach ourselves from
						// the parent's widget tree.
						Detach(false);
					}
				}
			}

	// Internal constructor that is used by the "InputOutputWidget" subclass.
	internal InputOnlyWidget(Widget parent, int x, int y,
							 int width, int height, Color background,
							 bool rootAllowed, bool overrideRedirect)
			: base(GetDisplay(parent, rootAllowed), GetScreen(parent),
				   DrawableKind.Widget, parent)
			{
				bool ok = false;
				try
				{
					// Validate the position and size.
					if(x < -32768 || x > 32767 ||
					   y < -32768 || y > 32767)
					{
						throw new XException(S._("X_InvalidPosition"));
					}
					if(width < 1 || width > 32767 ||
					   height < 1 || height > 32767 ||
					   !ValidateSize(width, height))
					{
						throw new XException(S._("X_InvalidSize"));
					}

					// Set the initial position and size of the widget.
					this.x = x;
					this.y = y;
					this.width = width;
					this.height = height;
					this.focusable = true;

					// Lock down the display and create the window handle.
					try
					{
						IntPtr display = dpy.Lock();
						XWindow pwindow = parent.GetWidgetHandle();
						XSetWindowAttributes attrs = new XSetWindowAttributes();
						attrs.override_redirect = overrideRedirect;
						XWindow window = Xlib.XCreateWindow
								(display, pwindow,
								 x, y, (uint)width, (uint)height, (uint)0,
								 screen.DefaultDepth, 1 /* InputOutput */,
								 screen.DefaultVisual,
								 (uint)(CreateWindowMask.CWOverrideRedirect),
								 ref attrs);
						SetWidgetHandle(window);
						if(background.Index == StandardColor.Inherit)
						{
							Xlib.XSetWindowBackgroundPixmap
								(display, window, XPixmap.ParentRelative);
						}
						else
						{
							Xlib.XSetWindowBackground(display, window,
													  ToPixel(background));
						}
						if(parent.AutoMapChildren)
						{
							Xlib.XMapWindow(display, window);
							mapped = true;
						}
					}
					finally
					{
						dpy.Unlock();
					}

					// Push the widget down to the default layer.
					layer = 0x7FFFFFFF;
					Layer = 0;
					ok = true;

					// Select for mouse events.
					SelectInput(EventMask.ButtonPressMask |
								EventMask.ButtonReleaseMask |
								EventMask.EnterWindowMask |
								EventMask.LeaveWindowMask |
								EventMask.PointerMotionMask);
				}
				finally
				{
					if(!ok)
					{
						// Creation failed, so detach ourselves from
						// the parent's widget tree.
						Detach(false);
					}
				}
			}

	// Get the display associated with a parent widget, and also
	// validate the widget.
	private static Display GetDisplay(Widget parent, bool rootAllowed)
			{
				if(parent == null)
				{
					throw new ArgumentNullException("parent");
				}
				if(!rootAllowed && parent is RootWindow)
				{
					throw new XInvalidOperationException
						(S._("X_NonRootParent"));
				}
				return parent.Display;
			}

	// Get the screen associated with a parent widget.
	private static Screen GetScreen(Widget parent)
			{
				if(parent != null)
				{
					return parent.Screen;
				}
				else
				{
					return null;
				}
			}

	/// <summary>
	/// <para>Get or set the focusable state for this widget.</para>
	/// </summary>
	///
	/// <value>
	/// <para>Set to <see langword="true"/> if this widget can receive
	/// the focus during TAB navigation.  The default value is
	/// <see langword="true"/>.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>This property is used during TAB navigation to determine
	/// where to send the focus next.  Widgets that are not focusable
	/// will be ignored during TAB navigation.</para>
	/// </remarks>
	public bool Focusable
			{
				get
				{
					return focusable;
				}
				set
				{
					focusable = value;
				}
			}

	/// <summary>
	/// <para>Determine if this widget currently has the keyboard focus.</para>
	/// </summary>
	///
	/// <value>
	/// <para>Returns <see langword="true"/> if this widget currently
	/// has the keyboard focus.</para>
	/// </value>
	public bool Focused
			{
				get
				{
					TopLevelWindow topLevel = TopLevel;
					if(topLevel != null)
					{
						return topLevel.HasFocus(this);
					}
					else
					{
						return false;
					}
				}
			}
			
	/// <summary>
	/// <para>Method that is called when the mouse wheel is turned.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the pointer position.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the pointer position.</para>
	/// </param>
	///
	/// <param name="button">
	/// <para>The button that was pressed.</para>
	/// </param>
	///
	/// <param name="modifiers">
	/// <para>Other button and shift flags that were active.</para>
	/// </param>
	///
	/// <param name="iDelta">
	/// <para>Delta indicating how far the wheel was turned.</para>
	/// </param>
	protected virtual bool OnButtonWheel(int x, int y, ButtonName button,
									  	   ModifierMask modifiers, int iDelta)
			{
				// Nothing to do in this class.
				return true;
			}

	/// <summary>
	/// <para>Method that is called when any mouse button is pressed
	/// while the pointer is inside this widget.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the pointer position.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the pointer position.</para>
	/// </param>
	///
	/// <param name="button">
	/// <para>The button that was pressed.</para>
	/// </param>
	///
	/// <param name="modifiers">
	/// <para>Other button and shift flags that were active.</para>
	/// </param>
	protected virtual void OnButtonPress(int x, int y, ButtonName button,
									     ModifierMask modifiers)
			{
				// Nothing to do in this class.
			}

	/// <summary>
	/// <para>Method that is called when any mouse button is released
	/// while the pointer is inside this widget.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the pointer position.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the pointer position.</para>
	/// </param>
	///
	/// <param name="button">
	/// <para>The button that was released.</para>
	/// </param>
	///
	/// <param name="modifiers">
	/// <para>Other button and shift flags that were active.</para>
	/// </param>
	protected virtual void OnButtonRelease(int x, int y, ButtonName button,
									  	   ModifierMask modifiers)
			{
				// Nothing to do in this class.
			}

	/// <summary>
	/// <para>Method that is called when any mouse button is double-clicked
	/// while the pointer is inside this widget.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the pointer position.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the pointer position.</para>
	/// </param>
	///
	/// <param name="button">
	/// <para>The button that was pressed.</para>
	/// </param>
	///
	/// <param name="modifiers">
	/// <para>Other button and shift flags that were active.</para>
	/// </param>
	protected virtual void OnButtonDoubleClick
				(int x, int y, ButtonName button, ModifierMask modifiers)
			{
				// Nothing to do in this class.
			}

	/// <summary>
	/// <para>Method that is called when the mouse pointer is moved inside
	/// this widget.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the pointer position.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the pointer position.</para>
	/// </param>
	///
	/// <param name="modifiers">
	/// <para>Other button and shift flags that were active.</para>
	/// </param>
	protected virtual void OnPointerMotion
				(int x, int y, ModifierMask modifiers)
			{
				// Nothing to do in this class.
			}

	/// <summary>
	/// <para>Method that is called if a key is pressed when this
	/// widget has the focus.</para>
	/// </summary>
	///
	/// <param name="key">
	/// <para>The key code.</para>
	/// </param>
	///
	/// <param name="modifiers">
	/// <para>Other button and shift flags that were active.</para>
	/// </param>
	///
	/// <param name="str">
	/// <para>The translated string that corresponds to the key, or
	/// <see langword="null"/> if the key does not have a translation.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if the key has been processed
	/// and it should not be passed further up the focus tree.  Returns
	/// <see langword="false"/> if the key should be passed further up
	/// the focus tree.</para>
	/// </returns>
	///
	/// <remarks>The <paramref name="key"/> parameter indicates the X11
	/// symbol that corresponds to the key, which allows cursor control
	/// and function keys to be easily distinguished.  The
	/// <paramref name="str"/> is primarily of use to text
	/// input widgets.</remarks>
	protected virtual bool OnKeyPress(KeyName key,
									  ModifierMask modifiers, String str)
			{
				// Nothing to do in this class.
				return false;
			}

	/// <summary>
	/// <para>Method that is called if a key is released when this
	/// widget has the focus.</para>
	/// </summary>
	///
	/// <param name="key">
	/// <para>The key code.</para>
	/// </param>
	///
	/// <param name="modifiers">
	/// <para>Other button and shift flags that were active.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if the key has been processed
	/// and it should not be passed further up the focus tree.  Returns
	/// <see langword="false"/> if the key should be passed further up
	/// the focus tree.</para>
	/// </returns>
	protected virtual bool OnKeyRelease(KeyName key, ModifierMask modifiers)
			{
				// Nothing to do in this class.
				return false;
			}

	/// <summary>
	/// <para>Method that is called when the mouse pointer enters
	/// this widget.</para>
	/// </summary>
	///
	/// <param name="child">
	/// <para>The child widget that contained the previous or final
	/// position, or <see langword="null"/> if no applicable child
	/// widget.</para>
	/// </param>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the pointer position.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the pointer position.</para>
	/// </param>
	///
	/// <param name="modifiers">
	/// <para>Button and shift flags that were active.</para>
	/// </param>
	///
	/// <param name="mode">
	/// <para>The notification mode value from the event.</para>
	/// </param>
	///
	/// <param name="detail">
	/// <para>The notification detail value from the event.</para>
	/// </param>
	protected virtual void OnEnter(Widget child, int x, int y,
								   ModifierMask modifiers,
								   CrossingMode mode,
								   CrossingDetail detail)
			{
				// Nothing to do in this class.
			}

	/// <summary>
	/// <para>Method that is called when the mouse pointer leaves
	/// this widget.</para>
	/// </summary>
	///
	/// <param name="child">
	/// <para>The child widget that contained the previous or final
	/// position, or <see langword="null"/> if no applicable child
	/// widget.</para>
	/// </param>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the pointer position.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the pointer position.</para>
	/// </param>
	///
	/// <param name="modifiers">
	/// <para>Button and shift flags that were active.</para>
	/// </param>
	///
	/// <param name="mode">
	/// <para>The notification mode value from the event.</para>
	/// </param>
	///
	/// <param name="detail">
	/// <para>The notification detail value from the event.</para>
	/// </param>
	protected virtual void OnLeave(Widget child, int x, int y,
								   ModifierMask modifiers,
								   CrossingMode mode,
								   CrossingDetail detail)
			{
				// Nothing to do in this class.
			}

	/// <summary>
	/// <para>Method that is called when the keyboard focus enters
	/// this widget.</para>
	/// </summary>
	///
	/// <param name="other">
	/// <para>The previous widget within the same top-level window that had
	/// the focus, or <see langword="null"/> if the focus is entering the
	/// top-level window from outside.</para>
	/// </param>
	protected virtual void OnFocusIn(Widget other)
			{
				// Nothing to do in this class.
			}

	/// <summary>
	/// <para>Method that is called when the keyboard focus leaves
	/// this widget.</para>
	/// </summary>
	///
	/// <param name="other">
	/// <para>The new widget within the same top-level window that will receive
	/// the focus, or <see langword="null"/> if the focus is leaving the
	/// top-level window.</para>
	/// </param>
	protected virtual void OnFocusOut(Widget other)
			{
				// Nothing to do in this class.
			}

	/// <summary>
	/// <para>Request that the keyboard focus be assigned to this widget.
	/// </para>
	/// </summary>
	///
	/// <remarks>
	/// <para>If the top-level window has the primary focus, then this widget
	/// will receive a <c>FocusIn</c> event immediately.  Otherwise,
	/// this widget will receive a <c>FocusIn</c> event the next time
	/// the top-level window obtains the primary focus.</para>
	///
	/// <para>If the top-level window does not currently have the primary
	/// focus, and its <c>DefaultFocus</c> is set to something other than
	/// this widget, then this widget may never receive the focus as a result
	/// of calling this method.</para>
	/// </remarks>
	public void RequestFocus()
			{
				TopLevelWindow topLevel = TopLevel;
				if(topLevel != null)
				{
					topLevel.SetFocus(this);
				}
			}

	// Dispatch an event to this widget.
	internal override void DispatchEvent(ref XEvent xevent)
			{
				ButtonName button;
				XTime time;
	
				switch((EventType)(xevent.xany.type__))
				{
					case EventType.ButtonPress:
					{
						// Process button events.
						button = xevent.xbutton.button;
						time = xevent.xbutton.time;
						if(lastClickButton == button &&
						   lastClickTime != XTime.CurrentTime &&
						   (time - lastClickTime) < 500)
						{
							OnButtonDoubleClick(xevent.xbutton.x,
								        		xevent.xbutton.y, button,
								        		xevent.xbutton.state);
							time = XTime.CurrentTime;
						}
						else
						{
							OnButtonPress(xevent.xbutton.x,
								  		  xevent.xbutton.y, button,
								  		  xevent.xbutton.state);
						}
						lastClickTime = time;
						lastClickButton = button;
					}
					break;

					case EventType.ButtonRelease:
					{
						// Dispatch a button release event.
						button = xevent.xbutton.button;
						OnButtonRelease(xevent.xbutton.x,
										xevent.xbutton.y, button,
										xevent.xbutton.state);
					}
					break;

					case EventType.MotionNotify:
					{
						// Dispatch a pointer motion event.
						OnPointerMotion(xevent.xmotion.x,
								   	    xevent.xmotion.y,
								   	    xevent.xmotion.state);
					}
					break;

					case EventType.EnterNotify:
					{
						// Dispatch a widget enter event.
						Widget child = dpy.handleMap
							[xevent.xcrossing.subwindow];
						OnEnter(child,
							    xevent.xcrossing.x,
							    xevent.xcrossing.y,
							    xevent.xcrossing.state,
							    xevent.xcrossing.mode,
							    xevent.xcrossing.detail);
					}
					break;

					case EventType.LeaveNotify:
					{
						// Dispatch a widget leave event.
						Widget child = dpy.handleMap
							[xevent.xcrossing.subwindow];
						OnLeave(child,
							    xevent.xcrossing.x,
							    xevent.xcrossing.y,
							    xevent.xcrossing.state,
							    xevent.xcrossing.mode,
							    xevent.xcrossing.detail);
					}
					break;
				}
			}

	// Dispatch a key event to this widget from the top-level window.
	internal bool DispatchKeyEvent(KeyName key, ModifierMask modifiers,
								   String str)
			{
				if(FullSensitive)
				{
					return OnKeyPress(key, modifiers, str);
				}
				else
				{
					return false;
				}
			}

	// Dispatch a key release event to this widget from the top-level window.
	internal bool DispatchKeyReleaseEvent(KeyName key, ModifierMask modifiers)
			{
				if(FullSensitive)
				{
					return OnKeyRelease(key, modifiers);
				}
				else
				{
					return false;
				}
			}

	// Dispatch a mouse wheel event to this widget from the top-level window.
	internal bool DispatchWheelEvent(ref XEvent xevent)
			{
				if(FullSensitive)
				{
					return OnButtonWheel (xevent.xbutton.x, xevent.xbutton.y,
						xevent.xbutton.button, xevent.xbutton.state,
						(xevent.xbutton.button == ButtonName.Button4 ? 120 : -120));
				}
				else
				{
					return false;
				}
			}

	// Dispatch a focus in event to this widget from the top-level window.
	internal void DispatchFocusIn(Widget oldWidget)
			{
				OnFocusIn(oldWidget);
			}

	// Dispatch a focus out event to this widget from the top-level window.
	internal void DispatchFocusOut(Widget newWidget)
			{
				OnFocusOut(newWidget);
			}

} // class InputOnlyWidget

} // namespace Xsharp
