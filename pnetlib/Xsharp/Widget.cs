/*
 * Widget.cs - Basic widget handling for X applications.
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
using System.Collections;
using Xsharp.Types;
using Xsharp.Events;
using OpenSystem.Platform.X11;

/// <summary>
/// <para>The <see cref="T:Xsharp.Widget"/> class manages widget
/// windows on an X display screen.</para>
///
/// <para>This is an abstract class.  Instantiate or inherit one of
/// the classes <see cref="T:Xsharp.InputOutputWidget"/>,
/// <see cref="T:Xsharp.InputOnlyWidget"/>, or
/// <see cref="T:Xsharp.TopLevelWindow"/> in user applications.</para>
/// </summary>
public abstract class Widget : Drawable, ICollection, IEnumerable
{
	// Internal state.
	internal int x, y;
	internal int layer;
	internal bool mapped;
	internal bool autoMapChildren;
	private bool sensitive;
	private bool ancestorSensitive;
	private Cursor cursor;
	private Widget parent;
	private Widget topChild;
	private Widget nextAbove;
	private Widget nextBelow;
	private EventMask eventMask;

	// Constructor.
	internal Widget(Display dpy, Screen screen,
					DrawableKind kind, Widget parent)
			: base(dpy, screen, kind)
			{
				// Set the initial widget properties.
				cursor = null;
				autoMapChildren = true;
				sensitive = true;

				// Insert this widget into the widget tree under its parent.
				this.parent = parent;
				this.topChild = null;
				this.nextAbove = null;
				if(parent != null)
				{
					ancestorSensitive =
						(parent.sensitive && parent.ancestorSensitive);
					nextBelow = parent.topChild;
					if(parent.topChild != null)
					{
						parent.topChild.nextAbove = this;
					}
					parent.topChild = this;
				}
				else
				{
					ancestorSensitive = true;
					nextBelow = null;
				}
				this.eventMask = 0;
			}

	// Detach this widget from its position in the widget tree.
	internal void Detach(bool parentOnly)
			{
				// Detach ourselves from our siblings and parent.
				if(nextBelow != null)
				{
					nextBelow.nextAbove = nextAbove;
				}
				if(nextAbove != null)
				{
					nextAbove.nextBelow = nextBelow;
				}
				else if(parent != null)
				{
					parent.topChild = nextBelow;
				}

				// Detach ourselves from our children.
				if(!parentOnly)
				{
					Widget current, next;
					current = topChild;
					while(current != null)
					{
						next = current.nextBelow;
						current.parent = null;
						current.nextAbove = null;
						current.nextBelow = null;
						current = next;
					}
					topChild = null;
				}

				// Clear all of our link fields.
				parent = null;
				nextAbove = null;
				nextBelow = null;
			}

	// Disassociate this widget instance and all of its children
	// from their X window handles, as the mirror copy in the X
	// server has been lost.
	internal virtual void Disassociate()
			{
				if(handle != XDrawable.Zero)
				{
					dpy.handleMap.Remove((XWindow)handle);
				}
				if(this is InputOutputWidget)
				{
					((InputOutputWidget)this).RemovePendingExpose();
				}
				handle = XDrawable.Zero;
				Widget child = topChild;
				while(child != null)
				{
					child.Disassociate();
					child = child.nextBelow;
				}
			}

	/// <summary>
	/// <para>Destroy this widget if it is currently active.</para>
	/// </summary>
	public override void Destroy()
			{
				try
				{
					IntPtr d = dpy.Lock();
					if(handle != XDrawable.Zero)
					{
						XDrawable tempHandle = handle;
						Disassociate();
						Xlib.XDestroyWindow(d, (XWindow)tempHandle);
						
					}
					Detach(false); // must detach
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Get the X position of this widget relative to its parent.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The X position of this widget in pixels.</para>
	/// </value>
	public int X
			{
				get
				{
					return x;
				}
			}

	/// <summary>
	/// <para>Get the Y position of this widget relative to its parent.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The Y position of this widget in pixels.</para>
	/// </value>
	public int Y
			{
				get
				{
					return y;
				}
			}

	/// <summary>
	/// <para>Determine if this widget is currently mapped.</para>
	/// </summary>
	///
	/// <value>
	/// <para>Returns <see langword="true"/> if the widget is mapped;
	/// <see langword="false"/> otherwise.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>A mapped widget may still be invisible if it is mapped
	/// because its parent is unmapped, because it is covered by
	/// a sibling widget, or because its co-ordinates are outside
	/// the range of its parent widget.</para>
	///
	/// <para>Setting this property is equivalent to calling either
	/// <c>Map</c> or <c>Unmap</c>.</para>
	/// </remarks>
	public bool IsMapped
			{
				get
				{
					return mapped;
				}
				set
				{
					if(value)
					{
						Map();
					}
					else
					{
						Unmap();
					}
				}
			}

	/// <summary>
	/// <para>Determine if all of the ancestor widgets are mapped.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The value is <see langword="true"/> if all of the ancestors
	/// of this widget are mapped; <see langword="false"/> otherwise.</para>
	/// </value>
	public bool AncestorsMapped
			{
				get
				{
					Widget widget = parent;
					while(widget != null)
					{
						if(!(widget.mapped))
						{
							return false;
						}
						widget = widget.parent;
					}
					return true;
				}
			}

	/// <summary>
	/// <para>Get or set the cursor that is associated with this widget.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The cursor shape to set for the widget.  If the value is
	/// <see langword="null"/>, then the widget inherits the
	/// cursor that is set on the parent widget.</para>
	/// </value>
	public virtual Cursor Cursor
			{
				get
				{
					return cursor;
				}
				set
				{
					try
					{
						IntPtr display = dpy.Lock();
						if(handle != XDrawable.Zero && cursor != value)
						{
							cursor = value;
							if(value == null)
							{
								// Revert to inheriting our parent's cursor.
								Xlib.XUndefineCursor
									(display, GetWidgetHandle());
							}
							else
							{
								// Change our cursor to the defined shape.
								value.SetCursor(this);
							}
						}
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Get or set the flag that indicates if child widgets
	/// should be automatically mapped when they are created.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The value is <see langword="true"/> to automatically map
	/// children; <see langword="false"/> otherwise.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>Normally, child widgets are automatically mapped when
	/// they are created, to avoid the need for the application to
	/// explicitly map each child as it is created.</para>
	///
	/// <para>By setting this flag to <see langword="false"/>, the
	/// program can control when children are mapped, which may be
	/// useful in certain circumstances (e.g. widgets that are
	/// hidden unless explicitly called for).</para>
	///
	/// <para>The root window has its <c>AutoMapChildren</c> flag set
	/// to <see langword="false"/> by default, and this cannot be
	/// changed.  This allows the application to fully create the
	/// widget tree before it is mapped to the screen.</para>
	/// </remarks>
	public bool AutoMapChildren
			{
				get
				{
					return autoMapChildren;
				}
				set
				{
					// Ignore the request if this is the root window.
					if(!(this is RootWindow))
					{
						autoMapChildren = value;
					}
				}
			}

	/// <summary>
	/// <para>Get or set the flag that indicates if this widget
	/// is sensitive to input events.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The value is <see langword="true"/> if the widget is sensitive;
	/// <see langword="false"/> otherwise.</para>
	/// </value>
	public bool Sensitive
			{
				get
				{
					return sensitive;
				}
				set
				{
					if(value != sensitive)
					{
						sensitive = value;
						UpdateSensitivity();
					}
				}
			}

	/// <summary>
	/// <para>Get the flag that indicates if all of this widget's
	/// ancestors are sensitive to input events.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The value is <see langword="true"/> if all ancestors
	/// are sensitive; <see langword="false"/> otherwise.</para>
	/// </value>
	public bool AncestorSensitive
			{
				get
				{
					return ancestorSensitive;
				}
			}

	/// <summary>
	/// <para>Get the flag that indicates if this widget and all of its
	/// ancestors are sensitive to input events.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The value is <see langword="true"/> if this widget
	/// and all of its ancestors are sensitive;
	/// <see langword="false"/> otherwise.</para>
	/// </value>
	public bool FullSensitive
			{
				get
				{
					return (sensitive && ancestorSensitive);
				}
			}

	/// <summary>
	/// <para>Get the parent of this widget.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.Widget"/> instance that
	/// corresponds to this widget's parent, or <see langword="null"/>
	/// if this widget is an instance of <see cref="T:Xsharp.RootWindow"/>.
	/// </para>
	/// </value>
	public Widget Parent
			{
				get
				{
					if(parent is PlaceholderWindow)
					{
						return null;
					}
					else
					{
						return parent;
					}
				}
			}

	/// <summary>
	/// <para>Get the top-level ancestor widget.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.TopLevelWindow"/> instance that
	/// corresponds to this widget's top-level ancestor, or
	/// <see langword="null"/> if the ancestor is not an instance
	/// of <see cref="T:Xsharp.TopLevelWindow"/>.</para>
	/// </value>
	public TopLevelWindow TopLevel
			{
				get
				{
					Widget widget = this;
					while(widget != null &&	!((widget is TopLevelWindow) &&
									!(widget.parent is CaptionWidget)))
					{
						// The MDI windows will be TopLevelWindows but
						// will have CaptionWidget as the parent
						// Other TopLevelWindows are not always having
						// RootWindow as their parent.
						widget = widget.parent;
					}
					return (TopLevelWindow)widget;
				}
			}

	/// <summary>
	/// <para>Get the next widget above this one in stacking order.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.Widget"/> instance that
	/// corresponds to the next widget above this one in stacking order,
	/// or <see langword="null"/> if this is the top-most widget.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>The actual ordering of top-level widgets may not match the
	/// value returned from this property because the window manager has
	/// changed the order itself based on user requests.</para>
	/// </remarks>
	public Widget NextAbove
			{
				get
				{
					if(parent is PlaceholderWindow)
					{
						return null;
					}
					else
					{
						return nextAbove;
					}
				}
			}

	/// <summary>
	/// <para>Get the next widget below this one in stacking order.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.Widget"/> instance that
	/// corresponds to the next widget below this one in stacking order,
	/// or <see langword="null"/> if this is the bottom-most widget.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>The actual ordering of top-level widgets may not match the
	/// value returned from this property because the window manager has
	/// changed the order itself based on user requests.</para>
	/// </remarks>
	public Widget NextBelow
			{
				get
				{
					if(parent is PlaceholderWindow)
					{
						return null;
					}
					else
					{
						return nextBelow;
					}
				}
			}

	/// <summary>
	/// <para>Get the top-most child widget in stacking order that
	/// has this widget as a parent.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.Widget"/> instance that
	/// corresponds to the top-most child widget in stacking order,
	/// or <see langword="null"/> if there are no child widgets.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>The actual ordering of top-level widgets may not match the
	/// value returned from this property because the window manager has
	/// changed the order itself based on user requests.</para>
	/// </remarks>
	public Widget TopChild
			{
				get
				{
					return topChild;
				}
			}

	/// <summary>
	/// <para>Get or set the stacking layer that this widget
	/// resides in.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The stacking layer value.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>Widgets in higher layers always appear above widgets in
	/// lower layers.  This class will attempt to keep the widgets in
	/// the correct order with respect to each other.</para>
	///
	/// <para>The value zero corresponds to the "normal" widget layer.
	/// Negative layers appear below the normal layer and positive layers
	/// appear above the normal layer.</para>
	///
	/// <para>The actual ordering of top-level widgets may not match the
	/// value returned from this property because the window manager has
	/// changed the order itself based on user requests, or because the
	/// window manager does not support widget layering.</para>
	/// </remarks>
	public int Layer
			{
				get
				{
					return layer;
				}
				set
				{
					Widget child;
					int origLayer = layer;
					layer = value;
					if(parent != null)
					{
						if(origLayer < value)
						{
							// Push the child further down the parent's stack.
							if(nextBelow == null ||
							   nextBelow.layer <= value)
							{
								return;
							}
							child = nextBelow;
							while(child.nextBelow != null &&
							      child.nextBelow.layer > value)
							{
								child = child.nextBelow;
							}
							RepositionBelow(child);
						}
						else if(origLayer > value)
						{
							// Raise the child further up the parent's stack.
							if(nextAbove == null ||
							   nextAbove.layer >= value)
							{
								return;
							}
							child = nextAbove;
							while(child.nextAbove != null &&
							      child.nextAbove.layer < value)
							{
								child = child.nextAbove;
							}
							RepositionAbove(child);
						}
					}
				}
			}


	// Reposition this widget below one of its siblings.
	private void RepositionBelow(Widget child)
			{
				// Detach ourselves from the widget tree.
				if(nextAbove != null)
				{
					nextAbove.nextBelow = nextBelow;
				}
				else
				{
					parent.topChild = nextBelow;
				}
				if(nextBelow != null)
				{
					nextBelow.nextAbove = nextAbove;
				}

				// Re-insert at the new position.
				nextAbove = child;
				nextBelow = child.nextBelow;
				if(nextBelow != null)
				{
					nextBelow.nextAbove = this;
				}
				child.nextBelow = this;

				try
				{
					IntPtr display = dpy.Lock();
					XWindowChanges changes = new XWindowChanges();
					changes.stack_mode = 1;		/* Below */
					if(child is TopLevelWindow)
					{
						Xlib.XConfigureWindow
								(display, GetWidgetHandle(),
							     (uint)(ConfigureWindowMask.CWStackMode),
								 ref changes);
					}
					else
					{
						changes.sibling = child.GetWidgetHandle();
						Xlib.XConfigureWindow
								(display, GetWidgetHandle(),
							     (uint)(ConfigureWindowMask.CWSibling |
								 	    ConfigureWindowMask.CWStackMode),
								 ref changes);
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Reposition this widget above one of its siblings.
	private void RepositionAbove(Widget child)
			{
				// Detach ourselves from the widget tree.
				if(nextAbove != null)
				{
					nextAbove.nextBelow = nextBelow;
				}
				else
				{
					parent.topChild = nextBelow;
				}
				if(nextBelow != null)
				{
					nextBelow.nextAbove = nextAbove;
				}

				// Re-insert at the new position.
				nextAbove = child.nextAbove;
				nextBelow = child;
				if(nextAbove != null)
				{
					nextAbove.nextBelow = this;
				}
				else
				{
					parent.topChild = this;
				}
				child.nextAbove = this;

				try
				{
					IntPtr display = dpy.Lock();
					XWindowChanges changes = new XWindowChanges();
					changes.stack_mode = 0;		/* Above */
					if(child is TopLevelWindow)
					{
						Xlib.XConfigureWindow
								(display, GetWidgetHandle(),
							     (uint)(ConfigureWindowMask.CWStackMode),
								 ref changes);
					}
					else
					{
						changes.sibling = child.GetWidgetHandle();
						Xlib.XConfigureWindow
								(display, GetWidgetHandle(),
							     (uint)(ConfigureWindowMask.CWSibling |
								 	    ConfigureWindowMask.CWStackMode),
								 ref changes);
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Perform a MoveResize request.
	internal virtual void PerformMoveResize
				(IntPtr display, int newX, int newY,
				 int newWidth, int newHeight)
			{
				if( handle != XDrawable.Zero ) {
					try {
						if(newX != x || newY != y)
						{
							if(newWidth != width || newHeight != height)
							{
								Xlib.XMoveResizeWindow(display, GetWidgetHandle(),
														newX, newY, (uint)newWidth,
														(uint)newHeight);
							}
							else
							{
								Xlib.XMoveWindow(display, GetWidgetHandle(),
												newX, newY);
							}
						}
						else if(newWidth != width || newHeight != height)
						{
							Xlib.XResizeWindow(display, GetWidgetHandle(),
												(uint)newWidth, (uint)newHeight);
						}
					}
					catch( XInvalidOperationException ) { // irgnore Widget disposed exception
					}
				}
			}

	// Adjust the position and/or size of this widget.
	private void AdjustPositionAndSize(IntPtr display, int newX, int newY,
									   int newWidth, int newHeight)
			{
				// Make sure that the values are in range.
				if(newX < -32768 || newX > 32767 ||
				   newY < -32768 || newY > 32767)
				{
					throw new XException(S._("X_InvalidPosition"));
				}
				else if(newWidth > 32767 || newHeight > 32767)
				{
					throw new XException(S._("X_InvalidSize"));
				}

				// Send requests to the X server to update its state.
				PerformMoveResize(display, newX, newY, newWidth, newHeight);

				// Record the new widget information locally.
				x = newX;
				y = newY;
				width = newWidth;
				height = newHeight;
			}

	/// <summary>
	/// <para>Raise this widget to the top of its layer.</para>
	/// </summary>
	public virtual void Raise()
			{
				Widget sibling = nextAbove;
				Widget last = this;
				while(sibling != null && sibling.layer == layer)
				{
					last = sibling;
					sibling = sibling.nextAbove;
				}
				if(sibling != null)
				{
					RepositionBelow(sibling);
				}
				else if(last != this)
				{
					RepositionAbove(last);
				}
			}

	/// <summary>
	/// <para>Lower this widget to the bottom of its layer.</para>
	/// </summary>
	public virtual void Lower()
			{
				Widget sibling = nextBelow;
				Widget last = this;
				while(sibling != null && sibling.layer == layer)
				{
					last = sibling;
					sibling = sibling.nextBelow;
				}
				if(sibling != null)
				{
					RepositionAbove(sibling);
				}
				else if(last != this)
				{
					RepositionBelow(last);
				}
			}

	/// <summary>
	/// <para>Move this widget to above one of its siblings.</para>
	/// </summary>
	///
	/// <param name="sibling">
	/// <para>The sibling to move this widget above.</para>
	/// </param>
	public virtual void MoveToAbove(Widget sibling)
			{
				if(sibling != null && sibling.layer == layer)
				{
					RepositionAbove(sibling);
				}
			}

	/// <summary>
	/// <para>Move this widget to above one of its siblings.</para>
	/// </summary>
	///
	/// <param name="sibling">
	/// <para>The sibling to move this widget below.</para>
	/// </param>
	public virtual void MoveToBelow(Widget sibling)
			{
				if(sibling != null && sibling.layer == layer)
				{
					RepositionBelow(sibling);
				}
			}

	/// <summary>
	/// <para>Move this widget to a new location relative to its parent.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the new top-left widget corner.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the new top-left widget corner.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if <paramref name="x"/> or <paramref name="y"/>
	/// is out of range.</para>
	/// </exception>
	public virtual void Move(int x, int y)
			{
				try
				{
					IntPtr display = dpy.Lock();
					if(x != this.x || y != this.y)
					{
						AdjustPositionAndSize(display, x, y,
											  this.width, this.height);
						OnMoveResize(x, y, this.width, this.height);
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Resize this widget to a new sie.</para>
	/// </summary>
	///
	/// <param name="width">
	/// <para>The new width for the widget.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The new width for the widget.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if <paramref name="width"/> or <paramref name="height"/>
	/// is out of range.</para>
	/// </exception>
	public virtual void Resize(int width, int height)
			{
				if(width < 1 || height < 1 ||
				   !ValidateSize(width, height))
				{
					throw new XException(S._("X_InvalidSize"));
				}
				try
				{
					IntPtr display = dpy.Lock();
					if(width != this.width || height != this.height)
					{
						AdjustPositionAndSize(display, this.x, this.y,
											  width, height);
						OnMoveResize(this.x, this.y, width, height);
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Move and resize this widget.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the new top-left widget corner.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the new top-left widget corner.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The new width for the widget.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The new width for the widget.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if <paramref name="width"/> or <paramref name="height"/>
	/// is out of range.</para>
	/// </exception>
	public virtual void MoveResize(int x, int y, int width, int height)
			{
				if(width < 1 || height < 1 ||
				   !ValidateSize(width, height))
				{
					throw new XException(S._("X_InvalidSize"));
				}
				try
				{
					IntPtr display = dpy.Lock();
					bool moved = (x != this.x || y != this.y);
					bool resized = (width != this.width ||
									height != this.height);
					if(moved || resized)
					{
						AdjustPositionAndSize(display, x, y, width, height);
						if(moved || resized)
						{
							OnMoveResize(x, y, width, height);
						}
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Validate a widget size change request.</para>
	/// </summary>
	///
	/// <param name="width">
	/// <para>The widget width to be validated.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The widget height to be validated.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns <see langword="true"/> if the size is acceptable;
	/// <see langword="false"/> otherwise.</para>
	/// </returns>
	///
	/// <remarks>
	/// <para>The implementation in the <see cref="T:Xsharp.Widget"/>
	/// base class always returns <see langword="true"/>.</para>
	/// </remarks>
	protected virtual bool ValidateSize(int width, int height)
			{
				return true;
			}

	/// <summary>
	/// <para>Map this widget to the screen.</para>
	/// </summary>
	public virtual void Map()
			{
				try
				{
					IntPtr display = dpy.Lock();
					if(!mapped)
					{
						Xlib.XMapWindow(display, GetWidgetHandle());
						mapped = true;
						OnMapStateChanged();
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Unmap this widget from the screen.</para>
	/// </summary>
	public virtual void Unmap()
			{
				try
				{
					IntPtr display = dpy.Lock();
					if(mapped)
					{
						Xlib.XUnmapWindow(display, GetWidgetHandle());
						mapped = false;
						OnMapStateChanged();
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Process a color theme change for this widget.</para>
	/// </summary>
	public virtual void ThemeChange()
			{
				Widget child = topChild;
				while(child != null)
				{
					child.ThemeChange();
					child = child.nextBelow;
				}
			}

	/// <summary>
	/// <para>Reparent this widget underneath a new parent.</para>
	/// </summary>
	///
	/// <param name="newParent">
	/// <para>The new parent widget.  This should be the placeholder widget
	/// for the screen if you wish to give this widget "no parent".</para>
	/// </param>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the new top-left widget corner.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the new top-left widget corner.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="newParent"/> is
	/// <see langword="null"/>.</para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if <paramref name="x"/> or <paramref name="y"/>
	/// is out of range.</para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XInvalidOperationException">
	/// <para>Raised if <paramref name="newParent"/> is a descendent
	/// of this widget, which would create a circularity.</para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XInvalidOperationException">
	/// <para>Raised if <paramref name="newParent"/> is on a different
	/// screen from this widget.</para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XInvalidOperationException">
	/// <para>Raised if <paramref name="newParent"/> is an input only
	/// widget, but this widget is input-output.</para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XInvalidOperationException">
	/// <para>Raised if an attempt is made to reparent the root window
	/// or a top-level window.</para>
	/// </exception>
	public virtual void Reparent(Widget newParent, int x, int y)
			{
				// Validate the parameters.
				if(newParent == null)
				{
					throw new ArgumentNullException("newParent");
				}
				if(x < -32768 || x > 32767 ||
				   y < -32768 || y > 32767)
				{
					throw new XException(S._("X_InvalidPosition"));
				}
				Widget temp = newParent;
				while(temp != null && temp != this)
				{
					temp = temp.parent;
				}
				if(temp != null)
				{
					throw new XInvalidOperationException 
						(S._("X_InvalidReparent"));
				}
				if(screen != newParent.screen)
				{
					throw new XInvalidOperationException 
						(S._("X_InvalidReparent"));
				}
				if(!(newParent is InputOutputWidget) &&
				   this is InputOutputWidget)
				{
					throw new XInvalidOperationException 
						(S._("X_InvalidReparent"));
				}

				// If the new parent is the same as the old, then simply
				// move and raise the widget, but do nothing else.
				if(newParent == parent)
				{
					Move(x, y);
					Raise();
					return;
				}

				// Detach the widget from its current parent.
				if(nextBelow != null)
				{
					nextBelow.nextAbove = nextAbove;
				}
				if(nextAbove != null)
				{
					nextAbove.nextBelow = nextBelow;
				}
				else if(parent != null)
				{
					parent.topChild = nextBelow;
				}

				// Attach the widget to its new parent as the top-most child.
				nextBelow = newParent.topChild;
				nextAbove = null;
				if(newParent.topChild != null)
				{
					newParent.topChild.nextAbove = this;
				}
				newParent.topChild = this;
				parent = newParent;

				// Temporarily put the widget in the top-most layer.
				int saveLayer = layer;
				layer = 0x7FFFFFFF;

				// Perform the actual reparent operation.  This will
				// put the window at the top of the stacking order.
				try
				{
					IntPtr display = dpy.Lock();
					XWindow widget = GetWidgetHandle();
					XWindow pwidget = newParent.GetWidgetHandle();
					Xlib.XReparentWindow(display, widget, pwidget, x, y);
					this.x = x;
					this.y = y;
				}
				finally
				{
					dpy.Unlock();
				}

				// Push the widget down to its original layer position.
				Layer = saveLayer;
			}

	// Update the sensitivity on this widget and all of its children.
	private void UpdateSensitivity()
			{
				// Notify everyone who is interested in our sensitivity change.
				OnSensitivityChanged();

				// Modify the ancestor sensitivity of the child widgets.
				Widget child = topChild;
				bool thisAncestorSensitive = (sensitive && ancestorSensitive);
				while(child != null)
				{
					if(child.ancestorSensitive != thisAncestorSensitive)
					{
						child.ancestorSensitive = thisAncestorSensitive;
						child.UpdateSensitivity();
					}
					child = child.nextBelow;
				}
			}

	/// <summary>
	/// <para>Determine if a mouse button corresponds to "Select".
	/// Usually this is the "Left" mouse button.</para>
	/// </summary>
	///
	/// <param name="button">
	/// <para>The button name to test.</para>
	/// </param>
	public bool IsSelect(ButtonName button)
			{
				return (button == dpy.selectButton);
			}

	/// <summary>
	/// <para>Determine if the mouse button that corresponds to "Select"
	/// is part of a modifier mask.</para>
	/// </summary>
	///
	/// <param name="modifiers">
	/// <para>The modifier mask to test.</para>
	/// </param>
	public bool IsSelect(ModifierMask modifiers)
			{
				return ((((int)modifiers) &
							(((int)(ModifierMask.Button1Mask)) <<
								(((int)(dpy.selectButton)) - 1))) != 0);
			}

	/// <summary>
	/// <para>Determine if a mouse button corresponds to "Menu".
	/// Usually this is the "Right" mouse button.</para>
	/// </summary>
	///
	/// <param name="button">
	/// <para>The button name to test.</para>
	/// </param>
	public bool IsMenu(ButtonName button)
			{
				return (button == dpy.menuButton);
			}

	/// <summary>
	/// <para>Determine if the mouse button that corresponds to "Menu"
	/// is part of a modifier mask.</para>
	/// </summary>
	///
	/// <param name="modifiers">
	/// <para>The modifier mask to test.</para>
	/// </param>
	public bool IsMenu(ModifierMask modifiers)
			{
				return ((((int)modifiers) &
							(((int)(ModifierMask.Button1Mask)) <<
								(((int)(dpy.menuButton)) - 1))) != 0);
			}

	/// <summary>
	/// <para>Copy the children of this widget into an array.</para>
	/// </summary>
	///
	/// <param name="array">
	/// <para>The array to copy the children to.</para>
	/// </param>
	///
	/// <param name="index">
	/// <para>The index within the array to being copying.</para>
	/// </param>
	///
	/// <remarks>
	/// <para>This method implements the
	/// <see cref="T:System.Collections.ICollection"/> interface.</para>
	/// </remarks>
	public void CopyTo(Array array, int index)
			{
				if(array == null)
				{
					throw new ArgumentNullException("array", "Argument cannot be null");
				}

				if(index < 0)
				{
					throw new ArgumentOutOfRangeException("index");
				}

				Widget child = topChild;
				while(child != null)
				{
					array.SetValue(child, index++);
					child = child.nextBelow;
				}
			}

	/// <summary>
	/// <para>Get the number of children of this widget.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The number of children of this widget.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>This property implements the
	/// <see cref="T:System.Collections.ICollection"/> interface.</para>
	/// </remarks>
	public int Count
			{
				get
				{
					Widget child = topChild;
					int count = 0;
					while(child != null)
					{
						++count;
						child = child.nextBelow;
					}
					return count;
				}
			}

	/// <summary>
	/// <para>Determine if this collection is synchronized.</para>
	/// </summary>
	///
	/// <value>
	/// <para>Always returns <see langword="false"/>.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>This property implements the
	/// <see cref="T:System.Collections.ICollection"/> interface.</para>
	/// </remarks>
	public bool IsSynchronized
			{
				get
				{
					return false;
				}
			}

	/// <summary>
	/// <para>Get the synchronization root for this collection.</para>
	/// </summary>
	///
	/// <value>
	/// <para>Always returns <see langword="this"/>.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>This property implements the
	/// <see cref="T:System.Collections.ICollection"/> interface.</para>
	/// </remarks>
	public Object SyncRoot
			{
				get
				{
					return this;
				}
			}

	/// <summary>
	/// <para>Get an enumerator for the children of this widget.</para>
	/// </summary>
	///
	/// <returns>
	/// <para>Returns the enumerator instance.</para>
	/// </returns>
	///
	/// <remarks>
	/// <para>This method implements the
	/// <see cref="T:System.Collections.IEnumerable"/> interface.</para>
	/// </remarks>
	public IEnumerator GetEnumerator()
			{
				return new WidgetEnumerator(this);
			}

	// Private enumerator class for "Widget.GetEnumerator()".
	private sealed class WidgetEnumerator : IEnumerator
	{
		// Internal state.
		private Widget parent;
		private Widget child;
		private bool atStart;

		// Constructor.
		public WidgetEnumerator(Widget parent)
				{
					this.parent = parent;
					this.child = null;
					this.atStart = true;
				}

		// Move to the next element in the enumeration order.
		public bool MoveNext()
				{
					if(atStart)
					{
						child = parent.topChild;
						atStart = false;
					}
					else
					{
						child = child.nextBelow;
					}
					return (child != null);
				}

		// Reset the enumeration order.
		public void Reset()
				{
					child = null;
					atStart = true;
				}

		// Get the current value in the enumeration.
		public Object Current
				{
					get
					{
						if(child == null)
						{
							throw new InvalidOperationException
								(S._("X_BadEnumeratorPosition"));
						}
						return child;
					}
				}

	} // class WidgetEnumerator

	/// Dispatch an event to this widget.
	internal virtual void DispatchEvent(ref XEvent xevent)
			{
				// Nothing to do here: overridden by subclasses.
			}

	// Select for particular events on this widget.
	internal void SelectInput(EventMask mask)
			{
				try
				{
					IntPtr display = dpy.Lock();
					EventMask newMask = (eventMask | mask);
					XWindow handle = GetWidgetHandle();
					if(newMask != eventMask)
					{
						eventMask = newMask;
						Xlib.XSelectInput(display, handle, (int)newMask);
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Deselect particular events on this widget.
	internal void DeselectInput(EventMask mask)
			{
				try
				{
					IntPtr display = dpy.Lock();
					EventMask newMask = (eventMask & ~mask);
					XWindow handle = GetWidgetHandle();
					if(newMask != eventMask)
					{
						eventMask = newMask;
						Xlib.XSelectInput(display, handle, (int)newMask);
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Method that is called when the widget is moved to a
	/// new position or given a new size.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the new top-left widget corner.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the new top-left widget corner.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The new width for the widget.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The new width for the widget.</para>
	/// </param>
	protected virtual void OnMoveResize(int x, int y, int width, int height)
			{
				// Nothing to do in the base class.
			}

	/// <summary>
	/// <para>Method that is called when the widget is mapped or
	/// unmapped.</para>
	/// </summary>
	protected virtual void OnMapStateChanged()
			{
				// Nothing to do in the base class.
			}

	/// <summary>
	/// <para>Method that is called when the widget's sensitivity
	/// changes.</para>
	/// </summary>
	protected virtual void OnSensitivityChanged()
			{
				// Nothing to do in the base class.
			}

	/// <summary>
	/// <para>Convert a set of widget co-ordinates into screen
	/// co-ordinates.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the point to convert.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the point to convert.</para>
	/// </param>
	///
	/// <returns>
	/// <para>The screen point.</para>
	/// </returns>
	public Point WidgetToScreen(int x, int y)
			{
				Widget current = this;
				while(current.parent != null)
				{
					x += current.x;
					y += current.y;
					current = current.parent;
				}
				return new Point(x, y);
			}

	/// <summary>
	/// <para>Convert a set of screen co-ordinates into widget
	/// co-ordinates.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the point to convert.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the point to convert.</para>
	/// </param>
	///
	/// <returns>
	/// <para>The widget point.</para>
	/// </returns>
	public Point ScreenToWidget(int x, int y)
			{
				Widget current = this;
				while(current.parent != null)
				{
					x -= current.x;
					y -= current.y;
					current = current.parent;
				}
				return new Point(x, y);
			}

	protected void SendBeginInvoke(IntPtr i_gch)
			{
				XEvent xevent = new XEvent();
				xevent.xany.type = (int)(EventType.ClientMessage);
				xevent.xany.window = GetWidgetHandle();
				xevent.xclient.format = 32;
				xevent.xclient.setl(0,(int)i_gch);

				try
				{
					IntPtr display = dpy.Lock();
					xevent.xclient.message_type = Xlib.XInternAtom
							(display, "INTERNAL_BEGIN_INVOKE", XBool.False);
					Xlib.XSendEvent (display, GetWidgetHandle(),
							XBool.False, (int)(EventMask.NoEventMask), ref xevent);
					Xlib.XFlush(display);
				}
				finally
				{
					dpy.Unlock();
				}
			}

} // class Widget

} // namespace Xsharp
