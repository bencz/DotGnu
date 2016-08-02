/*
 * DoubleBuffer.cs - Double buffer drawable for widgets.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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
/// <para>The <see cref="T:Xsharp.DoubleBuffer"/> class manages a
/// background buffer for an input-output widget, to streamline
/// paint operations and reduce flicker.</para>
///
/// <para>To draw into a double-buffer, create a
/// <see cref="T:Xsharp.Graphics"/> object, passing the double buffer
/// as the argument.  When the graphics object is disposed, the contents
/// of the buffer will be flushed to the on-screen widget.</para>
/// </summary>
public class DoubleBuffer : Drawable
{
	// Internal state.
	private InputOutputWidget widget;
	private bool usesXdbe;

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.DoubleBuffer"/>
	/// instance.</para>
	/// </summary>
	///
	/// <param name="widget">
	/// <para>The widget to attach the double buffer to.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="widget"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	public DoubleBuffer(InputOutputWidget widget)
			: base(GetDisplay(widget), GetScreen(widget),
			       DrawableKind.DoubleBuffer)
			{
				this.widget = widget;
				this.width = widget.width;
				this.height = widget.height;
				try
				{
					IntPtr display = dpy.Lock();

					// Determine if the X server supports double buffering.
					try
					{
						Xlib.Xint major, minor;
						if(Xlib.XdbeQueryExtension
							(display, out major, out minor)
								!= XStatus.Zero)
						{
							usesXdbe = true;
						}
						else
						{
							usesXdbe = false;
						}
					}
					catch(Exception)
					{
						// Xdbe functions are not present in "Xext".
						usesXdbe = false;
					}

					// Create the back buffer or pixmap, as appropriate.
					if(usesXdbe)
					{
						handle = Xlib.XdbeAllocateBackBufferName
							(display, widget.GetWidgetHandle(),
							 Xlib.XdbeSwapAction.Background);
					}
					else
					{
						handle = (XDrawable)
							Xlib.XCreatePixmap
								(display, (XDrawable)
								   Xlib.XRootWindowOfScreen(screen.screen),
								 (uint)width, (uint)height,
								 (uint)Xlib.XDefaultDepthOfScreen
								 	(screen.screen));
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Get the display and screen from a widget, after checking for null.
	private static Display GetDisplay(InputOutputWidget widget)
			{
				if(widget == null)
				{
					throw new ArgumentNullException("widget");
				}
				return widget.dpy;
			}
	private static Screen GetScreen(InputOutputWidget widget)
			{
				if(widget == null)
				{
					throw new ArgumentNullException("widget");
				}
				return widget.screen;
			}

	/// <summary>
	/// <para>Destroy this drawable if it is currently active.</para>
	/// </summary>
	public override void Destroy()
			{
				try
				{
					IntPtr display = dpy.Lock();
					if(handle != XDrawable.Zero)
					{
						if( widget.HasWidgetHandle() ) {
							if(usesXdbe)
							{
								Xlib.XdbeDeallocateBackBufferName(display, handle);
							}
							else
							{
								Xlib.XFreePixmap(display, (XPixmap)handle);
							}
						}
						else {
							// the widget was destroyed
						}
						handle = XDrawable.Zero;
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Get the widget that underlies this double buffer.</para>
	/// </summary>
	///
	/// <value>
	/// <para>Returns the widget object.</para>
	/// </value>
	public InputOutputWidget Widget
			{
				get
				{
					return widget;
				}
			}

	// Start a double buffer drawing operation.
	internal void Start(Graphics graphics)
			{
				// Re-create the pixmap object if the widget size has changed.
				if(!usesXdbe)
				{
					if(widget.width != width || widget.height != height)
					{
						try
						{
							IntPtr display = dpy.Lock();
							if(handle != XDrawable.Zero)
							{
								Xlib.XFreePixmap(display, (XPixmap)handle);
							}
							handle = (XDrawable)
								Xlib.XCreatePixmap
									(display, (XDrawable)
									   Xlib.XRootWindowOfScreen(screen.screen),
									 (uint)(widget.Width),
									 (uint)(widget.Height),
									 (uint)Xlib.XDefaultDepthOfScreen
									 	(screen.screen));
						}
						finally
						{
							dpy.Unlock();
						}
					}
				}

				// Copy the width and height values from the widget.
				width = widget.Width;
				height = widget.Height;
			}

	// Clear a double buffer at the start of a drawing operation.
	internal void ClearAtStart(Graphics graphics)
			{
				// Fill the pixmap with the background color if necessary.
				// We don't have to do this with Xdbe buffers because the
				// X server should have already taken care of it for us
				// during the last expose operation on the widget.
				
				// [Marc Haisenko] I'm experiencing a strange bug if double
				// buffering is enabled and certain widgets use a transparent
				// background color... this is fixed by clearing the buffer.
				// But I don't think this is the cause...
				if ((!usesXdbe) || (widget.Background.Index == StandardColor.Inherit))
				{
					graphics.Clear();
				}
			}

	// End a double buffer drawing operation.
	internal void End(Graphics graphics)
			{
				try
				{
					IntPtr display = dpy.Lock();
					if(handle != XDrawable.Zero)
					{
						if(usesXdbe)
						{
							Xlib.XdbeSwapInfo info = new Xlib.XdbeSwapInfo();
							info.swap_window = widget.GetWidgetHandle();
							info.swap_action = Xlib.XdbeSwapAction.Background;
							Xlib.XdbeSwapBuffers(display, ref info, 1);
						}
						else
						{
							using(Graphics g = new Graphics(widget))
							{
								Xlib.XCopyArea
									(display, handle,
									 widget.GetGCHandle(), g.gc, 0, 0,
									 (uint)width, (uint)height, 0, 0);
							}
						}
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

} // class DoubleBuffer

} // namespace Xsharp
