/*
 * RootWindow.cs - Root window handling for X applications.
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

using Xsharp.Events;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using OpenSystem.Platform.X11;

/// <summary>
/// <para>The <see cref="T:Xsharp.RootWindow"/> class manages the
/// root window for an X display screen.</para>
///
/// <para>The root window is special in that it does not have a parent,
/// and that it cannot be destroyed, resized, or moved.</para>
/// </summary>
public sealed class RootWindow : Widget
{
	// Internal state.
	private XAtom resourceManager;
	private String resources;

	// Constructor.  Called from the "Screen" class.
	internal RootWindow(Display dpy, Screen screen, XWindow handle)
			: base(dpy, screen, DrawableKind.Widget, null)
			{
				// Set this window's handle and add it to the handle map.
				this.handle = (XDrawable)handle;
				if(dpy.handleMap == null)
				{
					dpy.handleMap = new HandleMap();
				}
				dpy.handleMap[handle] = this;

				// Adjust the root window object to match the screen state.
				width = (int)(Xlib.XWidthOfScreen(screen.screen));
				height = (int)(Xlib.XHeightOfScreen(screen.screen));
				mapped = true;
				autoMapChildren = false;

				// Get the current state of the RESOURCE_MANAGER property.
				// We extract color theme information from it.
				resourceManager = Xlib.XInternAtom
					(dpy.dpy, "RESOURCE_MANAGER", XBool.False);
				IntPtr resptr = Xlib.XSharpGetResources(dpy.dpy, handle);
				if(resptr != IntPtr.Zero)
				{
					resources = Marshal.PtrToStringAnsi(resptr);
					Xlib.XSharpFreeResources(resptr);
				}

				// Select for property notifications so that we can
				// track changes to the RESOURCE_MANAGER property.
				SelectInput(EventMask.PropertyChangeMask);
			}

	/// <summary>
	/// <para>Destroy this window if it is currently active.</para>
	/// </summary>
	///
	/// <remarks>
	/// <para>The root window cannot be destroyed except by closing
	/// the connection to the X display server.  If this method is
	/// called on a root window, the request will be ignored.</para>
	/// </remarks>
	public override void Destroy()
			{
				// Nothing to do here.
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
	public override void Move(int x, int y)
			{
				throw new XInvalidOperationException
					(S._("X_NonRootOperation"));
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
	public override void Resize(int width, int height)
			{
				throw new XInvalidOperationException
					(S._("X_NonRootOperation"));
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
	public override void MoveResize(int x, int y, int width, int height)
			{
				throw new XInvalidOperationException
					(S._("X_NonRootOperation"));
			}

	/// <summary>
	/// <para>Map this widget to the screen.</para>
	/// </summary>
	public override void Map()
			{
				throw new XInvalidOperationException
					(S._("X_NonRootOperation"));
			}

	/// <summary>
	/// <para>Unmap this widget from the screen.</para>
	/// </summary>
	public override void Unmap()
			{
				throw new XInvalidOperationException
					(S._("X_NonRootOperation"));
			}

	/// <summary>
	/// <para>Reparenting is disabled for the root window.</para>
	/// </summary>
	public override void Reparent(Widget newParent, int x, int y)
			{
				throw new XInvalidOperationException
					(S._("X_NonRootOperation"));
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
	public override Cursor Cursor
			{
				get
				{
					// We don't know what the root window has set
					// cursor to, so we always return the default.
					return null;
				}
				set
				{
					// Cannot change the mouse cursor on the root window.
					throw new XInvalidOperationException
						(S._("X_NonRootOperation"));
				}
			}

	/// <summary>
	/// <para>Get the X resources on the root window.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The X resource string from the <c>RESOURCE_MANAGER</c>
	/// property, or <see langword="null"/> if the property
	/// is not currently set.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>The <c>ResourcesChanged</c> event will be emitted whenever
	/// the X resource string changes.</para>
	/// </remarks>
	public String Resources
			{
				get
				{
					return resources;
				}
			}

	/// <summary>
	/// <para>Event that is emitted when the X resources change.</para>
	/// </summary>
	public event EventHandler ResourcesChanged;

	/// <summary>
	/// <para>Get a named X resource value.</para>
	/// </summary>
	///
	/// <param name="name">
	/// <para>The name of the resource value to retrieve.</para>
	/// </param>
	///
	/// <returns>
	/// <para>The resource value, or <see langword="null"/> if there
	/// is no resource called <paramref name="name"/>.</para>
	/// </returns>
	public String GetResource(String name)
			{
				if((name == null) || (name.Length == 0))
				{
					return null;
				}

				if(resources == null)
				{
					return null;
				}
				int posn = 0;
				int end;
				String value;
				while(posn != -1 && posn < resources.Length)
				{
					if(resources[posn] == '\n')
					{
						++posn;
						continue;
					}
					if((posn + name.Length) >= resources.Length)
					{
						break;
					}
					if(String.CompareOrdinal(resources, posn, name, 0,
											 name.Length) == 0 &&
					   resources[posn + name.Length] == ':')
					{
						end = resources.IndexOf('\n', posn);
						if(end == -1)
						{
							end = resources.Length;
						}
						value = resources.Substring
							(posn + name.Length + 1,
							 end - (posn + name.Length + 1));
						return value.Trim();
					}
					posn = resources.IndexOf('\n', posn);
				}
				return null;
			}

	/// Dispatch an event to this widget.
	internal override void DispatchEvent(ref XEvent xevent)
			{
				if(((EventType)(xevent.xany.type__)) ==
						EventType.PropertyNotify &&
				   xevent.xproperty.atom == resourceManager)
				{
					// The "RESOURCE_MANAGER" property has changed.
					IntPtr resptr = Xlib.XSharpGetResources
							(dpy.dpy, GetWidgetHandle());
					if(resptr != IntPtr.Zero)
					{
						resources = Marshal.PtrToStringAnsi(resptr);
						Xlib.XSharpFreeResources(resptr);
					}
					else
					{
						resources = null;
					}
					if(ResourcesChanged != null)
					{
						ResourcesChanged(this, EventArgs.Empty);
					}
				}
			}

} // class RootWindow

} // namespace Xsharp
