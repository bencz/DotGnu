/*
 * MdiClientWidget.cs - Manage an MDI client area.
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

namespace Xsharp
{

using System;
using OpenSystem.Platform.X11;

/// <summary>
/// <para>The <see cref="T:Xsharp.MdiClientWidget"/> class manages child
/// widgets in a multiple document interface (MDI) arrangement.</para>
/// </summary>
public class MdiClientWidget : InputOutputWidget
{
	// Internal state.
	private CaptionWidget activeChild;
	private bool maximized;
	private int cascadeOffset;
	private CaptionButtonWidget minimize;
	private CaptionButtonWidget restore;
	private CaptionButtonWidget close;

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.MdiClientWidget"/>
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
	/// <para>Raised if <paramref name="parent"/> is disposed, the
	/// root window, or an input-only window.</para>
	/// </exception>
	public MdiClientWidget(Widget parent, int x, int y, int width, int height)
			: base(parent, x, y, width, height,
			       new Color(StandardColor.Foreground),
				   new Color(StandardColor.BottomShadow))
			{
				AutoMapChildren = false;
				Focusable = false;
				TopLevel.mdiClient = this;
				cascadeOffset = 0;

				// Create the button widgets for displaying window controls
				// in the menu area of the top-level window.
				minimize = new CaptionButtonWidget
					(parent, 0, 0, 14, 14, this,
					 CaptionWidget.CaptionFlags.HasMinimize);
				restore = new CaptionButtonWidget
					(parent, 0, 0, 14, 14, this,
					 CaptionWidget.CaptionFlags.HasRestore);
				close = new CaptionButtonWidget
					(parent, 0, 0, 14, 14, this,
					 CaptionWidget.CaptionFlags.HasClose);
			}

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.TopLevelWindow"/>
	/// instance that appears as a child of this MDI client area.</para>
	/// </summary>
	///
	/// <param name="name">
	/// <para>The initial name to display in the title bar.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the new window.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the new window.</para>
	/// </param>
	///
	/// <param name="type">
	/// <para>The type of window to create.  This must inherit from
	/// the <see cref="T:Xsharp.TopLevelWindow"/> class.  The value
	/// <see langword="null"/> indicates to use
	/// <see cref="T:Xsharp.TopLevelWindow"/> as the type.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if <paramref name="width"/> or <paramref name="height"/>
	/// is out of range, or <paramref name="type"/> does not inherit
	/// from <see cref="T:Xsharp.TopLevelWindow"/>.</para>
	/// </exception>
	public TopLevelWindow CreateChild(String name, int width, int height,
									  Type type)
			{
				CaptionWidget widget;
				if(type == null)
				{
					type = typeof(TopLevelWindow);
				}
				else if(!type.IsSubclassOf(typeof(TopLevelWindow)))
				{
					throw new XException(S._("X_TopLevelType"));
				}
				if(this.width != 1 && this.height != 1)
				{
					if(cascadeOffset >= (this.width - 64) ||
					   cascadeOffset >= (this.height - 64))
					{
						cascadeOffset = 0;
					}
				}
				widget = new CaptionWidget
					(this, name, cascadeOffset, cascadeOffset,
					 width, height, type);
				cascadeOffset += 22;
				if(maximized)
				{
					// The MDI client is in the maximized state,
					// so make sure that this window starts maximized.
					widget.MaximizeChild(true, HasControls());
				}
				return widget.Child;
			}

	// Select a new caption widget to become the active one.
	internal void SelectActive()
			{
				Widget widget;
				if(HasFocus())
				{
					widget = TopChild;
					while(widget != null)
					{
						if(widget is CaptionWidget && widget.IsMapped)
						{
							break;
						}
						widget = widget.NextBelow;
					}
				}
				else
				{
					widget = null;
				}
				if(widget != (Widget)activeChild)
				{
					if(activeChild != null)
					{
						activeChild.MakeInactive();
						OnDeactivateChild(activeChild.Child);
					}
					activeChild = (widget as CaptionWidget);
					if(activeChild != null)
					{
						activeChild.MakeActive();
						OnActivateChild(activeChild.Child);
					}
				}
				PositionControls();
			}

	// Position the floating controls that are used in the maximized state.
	internal void PositionControls()
			{
				if(maximized && HasControls() && activeChild != null)
				{
					int buttonWidth = 14;
					int x = Parent.Width - buttonWidth - 2;
					int y = 2;
					close.Move(x, y);
					x -= buttonWidth + 2;
					restore.Move(x, y);
					x -= buttonWidth;
					minimize.Move(x, y);
					close.Map();
					restore.Map();
					minimize.Map();
				}
				else
				{
					close.Unmap();
					restore.Unmap();
					minimize.Unmap();
				}
			}

	// Find the bottom-most child that is mapped.
	private CaptionWidget FindBottomChild()
			{
				Widget widget;
				widget = TopChild;
				while(widget != null && widget.NextBelow != null)
				{
					widget = widget.NextBelow;
				}
				while(widget != null)
				{
					if(widget is CaptionWidget && widget.IsMapped)
					{
						break;
					}
					widget = widget.NextAbove;
				}
				return (widget as CaptionWidget);
			}

	// Determine if this MDI client currently has the focus because the
	// top-level window that contains it has the primary focus.
	private bool HasFocus()
			{
				TopLevelWindow topLevel = TopLevel;
				if(topLevel != null)
				{
					return topLevel.hasPrimaryFocus;
				}
				else
				{
					return false;
				}
			}

	// There has been a change to the primary focus.
	internal void ChangePrimaryFocus()
			{
				SelectActive();
			}

	/// <summary>
	/// <para>Method that is called when a child becomes active within
	/// the MDI client.</para>
	/// </summary>
	///
	/// <param name="child">
	/// <para>The child that was activated.</para>
	/// </param>
	protected virtual void OnActivateChild(TopLevelWindow child)
			{
				// Nothing to do here.
			}

	/// <summary>
	/// <para>Method that is called when a child becomes inactive within
	/// the MDI client.</para>
	/// </summary>
	///
	/// <param name="child">
	/// <para>The child that was deactivated.</para>
	/// </param>
	protected virtual void OnDeactivateChild(TopLevelWindow child)
			{
				// Nothing to do here.
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
	protected override bool OnKeyPress(KeyName key,
									   ModifierMask modifiers, String str)
			{
				// Check for "Ctrl+F4" to close the active MDI child.
				// We also allow "Shift+F4" and "Mod4+F4" because some
				// window managers redirect "Ctrl+F4" for desktop switching.
				// "Mod4" is usually the "Windows" key on PC keyboards.
				if((key == KeyName.XK_F4 || key == KeyName.XK_KP_F4) &&
				   (modifiers & (ModifierMask.ControlMask |
				   				 ModifierMask.ShiftMask |
								 ModifierMask.Mod4Mask)) != 0)
				{
					if(activeChild != null)
					{
						activeChild.Child.Close();
					}
					return true;
				}

				// Check for Ctrl+Tab and Ctrl+Shift+Tab.  "Mod4" can be
				// used in place of "Ctrl", same as above.  Windows also
				// allows "Ctrl+F6" to be used instead of "Ctrl+Tab".
				if((key == KeyName.XK_Tab || key == KeyName.XK_KP_Tab ||
				    key == KeyName.XK_F6) &&
				   (modifiers & (ModifierMask.ControlMask |
								 ModifierMask.Mod4Mask)) != 0)
				{
					if((modifiers & ModifierMask.ShiftMask) != 0)
					{
						TabBackward();
					}
					else
					{
						TabForward();
					}
					return true;
				}
				if(key == KeyName.XK_ISO_Left_Tab &&
				   (modifiers & (ModifierMask.ControlMask |
								 ModifierMask.Mod4Mask)) != 0)
				{
					TabBackward();
					return true;
				}

				// We don't know what this key is.
				return false;
			}

	// Disassociate this object from its window handle.
	internal override void Disassociate()
			{
				base.Disassociate();
				TopLevel.mdiClient = null;
			}

	// Place a minimized window rectangle.
	internal void PlaceMinimizedRectangle(ref Rectangle rect, Widget placed)
			{
				// Create a region that consists of all minimized areas.
				Region region = new Region();
				Widget current = TopChild;
				while(current != null)
				{
					if(current is CaptionWidget && current.IsMapped &&
					   ((CaptionWidget)current).Child.IsIconic)
					{
						if(current != placed)
						{
							region.Union
								(current.x, current.y,
								 current.width, current.height);
						}
					}
					current = current.NextBelow;
				}

				// Place the minimized rectangle.
				int yplace = height - rect.height;
				int xplace = 0;
				for(;;)
				{
					// Move up to the next line if we've overflowed this one.
					if((xplace + rect.width) > width && width >= rect.width)
					{
						yplace -= rect.height;
						xplace = 0;
					}

					// Determine if the rectangle overlaps the region.
					// If it doesn't, then we have found the best location.
					rect.x = xplace;
					rect.y = yplace;
					if(!region.Overlaps(rect))
					{
						region.Dispose();
						return;
					}

					// Move on to the next candidate.
					xplace += rect.width;
				}
			}

	// Determine if we have separate maximized controls outside the
	// boundaries of the MDI client window.
	internal bool HasControls()
			{
				return (y >= 18);
			}

	// Maximize all windows.
	internal void MaximizeAll(Widget dontNotify)
			{
				bool controls = HasControls();
				maximized = true;
				Widget current = TopChild;
				while(current != null)
				{
					if(current is CaptionWidget)
					{
						((CaptionWidget)current).MaximizeChild
							(current != dontNotify, controls);
					}
					current = current.NextBelow;
				}
				PositionControls();
			}

	// Restore all windows from the maximized state.
	internal void RestoreAll(Widget dontNotify)
			{
				maximized = false;
				Widget current = TopChild;
				while(current != null)
				{
					if(current is CaptionWidget)
					{
						((CaptionWidget)current).RestoreChild
							(current != dontNotify);
					}
					current = current.NextBelow;
				}
				PositionControls();
			}

	// Perform a MoveResize request.
	internal override void PerformMoveResize
				(IntPtr display, int newX, int newY,
				 int newWidth, int newHeight)
			{
				// Perform the move/resize normally.
				base.PerformMoveResize
					(display, newX, newY, newWidth, newHeight);

				// If the MDI client area is maximized, then re-maximize all.
				if(maximized)
				{
					MaximizeAll(null);
				}
			}

	/// <summary>
	/// <para>Cascade the windows in the MDI client area.</para>
	/// </summary>
	public void Cascade()
			{
				// TODO
			}

	/// <summary>
	/// <para>Tile the windows in the MDI client area horizontally.</para>
	/// </summary>
	public void TileHorizontally()
			{
				// TODO
			}

	/// <summary>
	/// <para>Tile the windows in the MDI client area vertically.</para>
	/// </summary>
	public void TileVertically()
			{
				// TODO
			}

	/// <summary>
	/// <para>Arrange the minimized icons in the MDI client area.</para>
	/// </summary>
	public void ArrangeIcons()
			{
				// TODO
			}

	/// <summary>
	/// <para>Tab forward through the MDI child windows.</para>
	/// </summary>
	public void TabForward()
			{
				if(activeChild != null)
				{
					activeChild.Lower();
				}
			}

	/// <summary>
	/// <para>Tab backward through the MDI child windows.</para>
	/// </summary>
	public void TabBackward()
			{
				CaptionWidget child = FindBottomChild();
				if(child != null)
				{
					child.Raise();
				}
			}

	// Standalone widget that contains a single caption button.
	// This is used for displaying buttons in the menu area of a
	// top-level window when MDI children are maximized.
	private class CaptionButtonWidget : InputOutputWidget
	{
		// Internal state.
		private MdiClientWidget mdi;
		private CaptionWidget.CaptionFlags flags;
		private bool pressed;
		private bool entered;

		// Constructor.
		public CaptionButtonWidget(Widget parent, int x, int y,
								   int width, int height, MdiClientWidget mdi,
								   CaptionWidget.CaptionFlags flags)
				: base(parent, x, y, width, height)
				{
					this.mdi = mdi;
					this.flags = flags;
					this.pressed = false;
					this.Layer = 10;
				}

		// Paint this widget in response to an "Expose" event.
		protected override void OnPaint(Graphics graphics)
				{
					XPixmap pixmap;
					if((flags & CaptionWidget.CaptionFlags.HasClose) != 0)
					{
						pixmap = graphics.dpy.bitmaps.Close;
					}
					else if((flags & CaptionWidget.CaptionFlags.HasRestore)
									!= 0)
					{
						pixmap = graphics.dpy.bitmaps.Restore;
					}
					else
					{
						pixmap = graphics.dpy.bitmaps.Minimize;
					}
					int x = (width - 9) / 2;
					int y = (height - 9) / 2;
					if(pressed && entered)
					{
						graphics.DrawEffect(0, 0, width, height,
											Effect.CaptionButtonIndented);
						++x;
						++y;
					}
					else
					{
						graphics.DrawEffect(0, 0, width, height,
											Effect.CaptionButtonRaised);
					}
					graphics.DrawBitmap(x, y, 9, 9, pixmap);
				}

		// Redraw this button.
		private void Draw()
				{
					using(Graphics graphics = new Graphics(this))
					{
						OnPaint(graphics);
					}
				}

		// Handle a button press.
		protected override void OnButtonPress
					(int x, int y, ButtonName button, ModifierMask modifiers)
				{
					if(IsSelect(button))
					{
						entered = true;
						pressed = true;
						Draw();
					}
				}

		// Handle a button release.
		protected override void OnButtonRelease
					(int x, int y, ButtonName button, ModifierMask modifiers)
				{
					if(IsSelect(button))
					{
						pressed = false;
						Draw();
						if(entered)
						{
							CaptionWidget child = mdi.activeChild;
							if(child != null)
							{
								if((flags &
									CaptionWidget.CaptionFlags.HasClose) != 0)
								{
									child.Child.Close();
								}
								else if((flags & CaptionWidget.CaptionFlags.
											HasRestore) != 0)
								{
									if(child.Child.iconic)
									{
										child.Child.Deiconify();
									}
									else
									{
										child.Child.Restore();
									}
								}
								else
								{
									child.Child.Iconify();
								}
							}
						}
					}
				}

		// Handle pointer motion events.
		protected override void OnPointerMotion
					(int x, int y, ModifierMask modifiers)
				{
					bool before = (entered && pressed);
					entered = (x >= 0 && y >= 0 && x < width && y < height);
					if((entered && pressed) != before)
					{
						Draw();
					}
				}

	}; // class CaptionButtonWidget

} // class MdiClientWidget

} // namespace Xsharp
