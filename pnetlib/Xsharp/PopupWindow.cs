/*
 * PopupWindow.cs - Widget handling for popup windows.
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
using Xsharp.Types;

/// <summary>
/// <para>The <see cref="T:Xsharp.PopupWindow"/> class manages
/// popup windows that display top-level on the screen, but do not
/// have window manager borders.  The mouse and keyboard will be
/// grabbed while a popup window is mapped to the screen.</para>
/// </summary>
public class PopupWindow : OverrideWindow
{
	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.PopupWindow"/>
	/// instance.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X position of the new window.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y position of the new window.</para>
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
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if any of the parameters are out of range.</para>
	/// </exception>
	///
	/// <summary>
	/// <para>This version of the constructor creates the window on
	/// the default screen of the primary display.</para>
	/// </summary>
	public PopupWindow(int x, int y, int width, int height)
			: base(TopLevelWindow.GetRoot(null), x, y, width, height)
			{
				// Nothing to do here.
			}

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.PopupWindow"/>
	/// instance.</para>
	/// </summary>
	///
	/// <param name="screen">
	/// <para>The screen to display the window on, or <see langword="null"/>
	/// to use the default screen of the primary display.</para>
	/// </param>
	///
	/// <param name="x">
	/// <para>The X position of the new window.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y position of the new window.</para>
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
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if any of the parameters are out of range.</para>
	/// </exception>
	public PopupWindow(Screen screen, int x, int y, int width, int height)
			: base(TopLevelWindow.GetRoot(screen), x, y, width, height)
			{
				// Nothing to do here.
			}

	// Get the grab window that is associated with this popup.
	private GrabWindow GetGrabWindow()
			{
				return screen.grabWindow;
			}

	/// <summary>
	/// <para>Destroy this widget if it is currently active.</para>
	/// </summary>
	public override void Destroy()
			{
				GetGrabWindow().RemovePopup(this);
				base.Destroy();
			}

	/// <summary>
	/// <para>Map this widget to the screen.</para>
	/// </summary>
	public override void Map()
			{
				if(!IsMapped)
				{
					GetGrabWindow().AddPopup(this);
					Raise();
					base.Map();
				}
			}

	/// <summary>
	/// <para>Unmap this widget from the screen.</para>
	/// </summary>
	public override void Unmap()
			{
				if(IsMapped)
				{
					GetGrabWindow().RemovePopup(this);
					base.Unmap();
				}
			}

	/// <summary>
	/// <para>Raise this widget to the top of its layer.</para>
	/// </summary>
	public override void Raise()
			{
				if(IsMapped)
				{
					GetGrabWindow().AddPopup(this);
				}

				try
				{
					IntPtr display = dpy.Lock();
					XWindowChanges changes = new XWindowChanges();
					changes.stack_mode = 0;		/* Above */

						Xlib.XConfigureWindow
								(display, GetWidgetHandle(),
							     (uint)(ConfigureWindowMask.CWStackMode),
								 ref changes);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Lower this widget to the bottom of its layer.</para>
	/// </summary>
	public override void Lower()
			{
				if(IsMapped)
				{
					GetGrabWindow().LowerPopup(this);
				}

				try
				{
					IntPtr display = dpy.Lock();
					XWindowChanges changes = new XWindowChanges();
					changes.stack_mode = 0;		/* Above */

						Xlib.XConfigureWindow
								(display, GetWidgetHandle(),
							     (uint)(ConfigureWindowMask.CWStackMode),
								 ref changes);
				}
				finally
				{
					dpy.Unlock();
				}
			}

} // class PopupWindow

} // namespace Xsharp
