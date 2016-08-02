/*
 * InputOutputWidget.cs - Widget handling for input-output widgets.
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
using Xsharp.Types;
using Xsharp.Events;
using OpenSystem.Platform.X11;

/// <summary>
/// <para>The <see cref="T:Xsharp.InputOutputWidget"/> class manages widgets
/// that occupy screen real estate for the purpose of keyboard and pointer
/// handling, and which have output functionality.</para>
/// </summary>
public class InputOutputWidget : InputOnlyWidget
{
	// Internal state.
	private Color foreground;
	private Color background;
	private Pixmap backgroundPixmap;
	private Region exposeRegion;
	internal InputOutputWidget nextExpose;
	private Region invalidateRegion;
	internal InputOutputWidget nextInvalidate;
	private bool drawBackground = true;

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.InputOutputWidget"/>
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
	public InputOutputWidget(Widget parent, int x, int y,
							 int width, int height)
			: base(parent, x, y, width, height,
			       new Color(StandardColor.Inherit), false, false)
			{
				foreground = new Color(StandardColor.Foreground);
				background = new Color(StandardColor.Inherit);
				SelectInput(EventMask.ExposureMask);
			}

	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.InputOutputWidget"/>
	/// instance underneath a specified parent widget, with
	/// specified foreground and background colors.</para>
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
	/// <param name="foreground">
	/// <para>The foreground color for the widget.</para>
	/// </param>
	///
	/// <param name="background">
	/// <para>The background color for the widget.</para>
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
	public InputOutputWidget(Widget parent, int x, int y,
							 int width, int height,
							 Color foreground, Color background)
			: base(parent, x, y, width, height, background, false, false)
			{
				this.foreground = foreground;
				this.background = background;
				SelectInput(EventMask.ExposureMask);
			}

	// Internal constructor that is used by "TopLevelWindow" and
	// "OverrideWindow".
	internal InputOutputWidget(Widget parent, int x, int y,
							   int width, int height,
							   Color foreground, Color background,
							   bool rootAllowed, bool overrideRedirect)
			: base(parent, x, y, width, height, background,
				   rootAllowed, overrideRedirect)
			{
				this.foreground = foreground;
				this.background = background;
				SelectInput(EventMask.ExposureMask);
			}

	/// <summary>
	/// <para>Get or set whether the background should be drawn before OnPaint is invoked.
	/// </para>
	/// </summary>
	///
	/// <remarks>
	/// <para>
	/// If this property is false, the background may not necessarily be drawn (by the server)
	/// before the user painting commences.  If this property is true, the background
	/// is guaranteed to always be drawn by the server before user painting commences.
	/// </para>
	/// </remarks>
	public bool DrawBackground
	{
		get
		{
			return drawBackground;
		}
		set
		{
			drawBackground = value;
		}
	}

	/// <summary>
	/// <para>Get or set the foreground color for this widget.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The foreground color to use.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>The foreground color does not become active until the
	/// next time a <see cref="T:Xsharp.Graphics"/> object is created
	/// for the widget.  Usually this happens the next time the
	/// widget is repainted.</para>
	/// </remarks>
	public Color Foreground
			{
				get
				{
					return foreground;
				}
				set
				{
					foreground = value;
				}
			}

	/// <summary>
	/// <para>Get or set the background color for this widget.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The background color to use.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>The background color becomes active immediately.  If the
	/// widget is mapped, a repaint will be forced.</para>
	/// </remarks>
	public Color Background
			{
				get
				{
					return background;
				}
				set
				{
					if(background != value || backgroundPixmap != null)
					{
						try
						{
							IntPtr display = dpy.Lock();
							XWindow window = GetWidgetHandle();
							background = value;
							backgroundPixmap = null;
							Xlib.XSetWindowBackground
								(display, window, ToPixel(value));
							if(mapped && AncestorsMapped)
							{
								Invalidate();
							}
						}
						finally
						{
							dpy.Unlock();
						}
					}
				}
			}

	/// <summary>
	/// <para>Get or set the background pixmap for this widget.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The background pixmap to use.  Returns <see langword="null"/>
	/// if the background is set to a color or if it inherits the parent's
	/// background.  Setting the pixmap to <see langword="null"/> makes
	/// this widget inherit the parent's background.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>The background pixmap becomes active immediately.  If the
	/// widget is mapped, a repaint will be forced.</para>
	/// </remarks>
	public Pixmap BackgroundPixmap
			{
				get
				{
					return backgroundPixmap;
				}
				set
				{
					// Bail out if the background will be unchanged.
					if(background.Index == StandardColor.Pixmap &&
					   value != null && backgroundPixmap == value)
					{
						return;
					}
					else if(background.Index == StandardColor.Inherit &&
							value == null && backgroundPixmap == value)
					{
						return;
					}

					// Change the background to the new value.
					try
					{
						IntPtr display = dpy.Lock();
						XWindow window = GetWidgetHandle();
						if(value == null)
						{
							background = new Color(StandardColor.Inherit);
							backgroundPixmap = null;
							Xlib.XSetWindowBackgroundPixmap
								(display, window, XPixmap.ParentRelative);
						}
						else
						{
							background = new Color(StandardColor.Pixmap);
							backgroundPixmap = value;
							Xlib.XSetWindowBackgroundPixmap
								(display, window, value.GetPixmapHandle());
						}
						if(mapped && AncestorsMapped)
						{
							Invalidate();
						}
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Force a repaint of this widget.</para>
	/// </summary>
	public void Repaint()
			{
				Repaint(0, 0, width, height);
			}

	/// <summary>
	/// <para>Force a repaint on a section of this widget.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left corner of
	/// the section to repaint.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left corner of
	/// the section to repaint.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the section to repaint.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the section to repaint.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if <paramref name="x"/>, <paramref name="y"/>,
	/// <paramref name="width"/>, or <paramref name="height"/> are
	/// out of range.</para>
	/// </exception>
	public void Repaint(int x, int y, int width, int height)
			{
				if(x < -32768 || x > 32767 ||
				   y < -32768 || y > 32767)
				{
					throw new XException(S._("X_InvalidPosition"));
				}
				if(width < 1 || width > 32767 ||
				   height < 1 || height > 32767)
				{
					throw new XException(S._("X_InvalidSize"));
				}
				try
				{
					IntPtr display = dpy.Lock();
					if(mapped && AncestorsMapped)
					{
						if(invalidateRegion == null)
						{
							// Create a new invalidate region for this widget.
							invalidateRegion = new Region(x, y, width, height);
							dpy.AddPendingInvalidate(this);
						}
						else
						{
							// Add the rectangle to the invalidate region.
							invalidateRegion.Union(x, y, width, height);
						}
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Clear a region to the background and optionally queue expose events.
	private bool ClearRegion(Region region, XBool exposures)
			{
				// Intersect the region with the widget boundaries.
				region.Intersect(0, 0, width, height);

				// Remove areas that are occupied by mapped child widgets.
				Widget child = TopChild;
				while(child != null)
				{
					if(child.mapped)
					{
						region.Subtract(child.x, child.y,
										child.width, child.height);
					}
					child = child.NextBelow;
				}

				// Bail out if the region is now empty.
				if(region.IsEmpty())
				{
					return false;
				}

				// Lock down the display and send the "XClearArea" requests.
				try
				{
					IntPtr display = dpy.Lock();
					XWindow handle = GetWidgetHandle();
					IntPtr xregion = region.GetRegion();
					XRectangle xrect;
					int size, index;
					size = Xlib.XSharpGetRegionSize(xregion);
					for(index = 0; index < size; ++index)
					{
						Xlib.XSharpGetRegionRect(xregion, index, out xrect);
						Xlib.XClearArea(display, handle, xrect.x, xrect.y,
										xrect.width, xrect.height, exposures);
					}
				}
				finally
				{
					dpy.Unlock();
				}
				return true;
			}

	/// <summary>
	/// <para>Update this widget by immediately redrawing invalidated
	/// regions.</para>
	/// </summary>
	///
	/// <param name="clear">
	/// <para>Setting clear to true clears the region to the background.
	/// </para>
	/// </param>
	public void Update(bool clear)
			{
				Region region = invalidateRegion;
				invalidateRegion = null;
				if(region != null)
				{
					// Remove the region from the pending list.
					dpy.RemovePendingInvalidate(this);

					// No point redrawing if we are unmapped or the
					// region to be drawn is empty.
					if(mapped && AncestorsMapped && (!clear ||
					   ClearRegion(region, XBool.False)))
					{
						// Paint the region as if we got a regular expose.
						Graphics graphics = new Graphics(this);
						graphics.exposeRegion = region;
						graphics.SetClipRegion(region);
						try
						{
							OnPaint(graphics);
						}
						finally
						{
							graphics.Dispose();
						}
					}
					// Dispose the region that we no longer require.
					region.Dispose();
				}
			}

	// Flush pending invalidates to the X server.
	internal void FlushInvalidates()
			{
				Region region = invalidateRegion;
				invalidateRegion = null;
				nextInvalidate = null;
				if(region != null)
				{
					// No point redrawing if we are unmapped.
					if(handle != XDrawable.Zero && mapped && AncestorsMapped)
					{
						Invalidate(region);
					}
				}
			}

	/*
	 * <summary><p>Invalidate the whole widget and flush the request</p></summary>
	 */
	private void Invalidate()
	{
		Invalidate(0, 0, width, height);
	}
	
	/*
	 * <summary><p>Invalidate the given region and flush the request</p></summary>
	 */
	private void Invalidate(int x, int y, int width, int height)
	{
		Region region = new Region();
		
		region.Union(x, y, width, height);
		
		Invalidate(region);
	}
	
	/*
	 * <summary><p>Invalidate the given region and flush the request</p></summary>
	 */		
	private void Invalidate(Region region)
	{
		if (drawBackground)
		{
			ClearRegion(region, XBool.True);
		}
		else
		{
			/* Don't flush to the X server cause we don't want it to draw
				the background */
			
			region.Intersect(0, 0, width, height);
			
			if (exposeRegion == null)
			{
				exposeRegion = region;
				
				dpy.AddPendingExpose(this);
			}
			else
			{
				exposeRegion.Union(region);
				
				region.Dispose();
			}
		}
	}

	/// <summary>
	/// <para>Process a color theme change for this widget.</para>
	/// </summary>
	public override void ThemeChange()
			{
				// Pass the theme change on to all of the child widgets.
				base.ThemeChange();

				// Change the background pixel to match the new theme
				// settings, and then force a repaint on the widget.
				try
				{
					IntPtr display = dpy.Lock();
					XWindow window = GetWidgetHandle();
					StandardColor sc = background.Index;
					if(sc != StandardColor.Inherit &&
					   sc != StandardColor.Pixmap &&
					   sc != StandardColor.RGB)
					{
						Xlib.XSetWindowBackground
							(display, window, ToPixel(background));
					}
					if(mapped && AncestorsMapped)
					{
						Xlib.XClearArea(display, window,
										0, 0, (uint)0, (uint)0,
										XBool.True);
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Method that is raised when the widget needs to be
	/// painted in reponse to an <c>Expose</c> event.</para>
	/// </summary>
	///
	/// <param name="graphics">
	/// <para>The graphics object to use to repaint the widget.  This
	/// graphics object will have been initialised with the widgets foreground
	/// and background colors, and with the clipping region set to the area
	/// that needs to be repainted.</para>
	/// </param>
	protected virtual void OnPaint(Graphics graphics)
			{
				// Nothing to do in this class.
			}

	// Dispatch an event to this widget.
	internal override void DispatchEvent(ref XEvent xevent)
			{
				switch((EventType)(xevent.xany.type__))
				{
					case EventType.Expose:
					case EventType.GraphicsExpose:
					{
						// Add the area to the expose region.
						if(exposeRegion == null)
						{
							// This is the first rectangle in an expose.
							exposeRegion = new Region
								((int)(xevent.xexpose.x__),
								 (int)(xevent.xexpose.y__),
								 (int)(xevent.xexpose.width__),
								 (int)(xevent.xexpose.height__));

							// Queue this widget for later repainting.
							// We don't do it now or the system will be
							// very slow during opaque window drags.
							dpy.AddPendingExpose(this);
						}
						else
						{
							// This is an extra rectangle in an expose.
							exposeRegion.Union
								((int)(xevent.xexpose.x__),
								 (int)(xevent.xexpose.y__),
								 (int)(xevent.xexpose.width__),
								 (int)(xevent.xexpose.height__));
						}
					}
					break;
					
					case Xsharp.Events.EventType.ClientMessage:
					{
						if(xevent.xclient.message_type == dpy.internalBeginInvoke)
						{
							OnBeginInvokeMessage((IntPtr)xevent.xclient.l(0));
						}
					}
					break;
				}
				base.DispatchEvent(ref xevent);
			}
			
	/// <summary>
	/// <para>Method that is called when a custom "BeginInvoke" message comes in
	/// changes.</para>
	/// </summary>
	protected virtual void OnBeginInvokeMessage(IntPtr i_gch)
			{
				// Nothing to do in this base class.
			}


	// Remove this widget from the pending expose list.
	internal void RemovePendingExpose()
			{
				dpy.RemovePendingExpose(this);
				if(exposeRegion != null)
				{
					exposeRegion.Dispose();
					exposeRegion = null;
				}
			}

	// Process pending exposures on this widget.
	internal void Expose()
			{
				Region region = exposeRegion;
				if(region != null)
				{
					exposeRegion = null;
					// sometimes it could be that Expose is called but the handle is destroyed.
					// so check here, if handle not null.
					if( handle != XDrawable.Zero ) {
						try {
							Graphics graphics = new Graphics(this);
							graphics.exposeRegion = region;
							graphics.SetClipRegion(region);
							OnPaint(graphics);
							graphics.Dispose();
							region.Dispose();
						}
						catch( XInvalidOperationException ) { // irgnore Widget disposed exception
						}
					}
				}
			}

	// Get the background information for use in graphics object clears.
	internal void GetBackgroundInfo(out Color bg, out Pixmap pixmap,
									out int tx, out int ty)
			{
				if(background.Index == StandardColor.Inherit)
				{
					InputOutputWidget parent =
						(Parent as InputOutputWidget);
					if(parent != null)
					{
						parent.GetBackgroundInfo
							(out bg, out pixmap, out tx, out ty);
						tx -= x;
						ty -= y;
					}
					else
					{
						bg = new Color(StandardColor.Background);
						pixmap = null;
						tx = 0;
						ty = 0;
					}
				}
				else if(background.Index == StandardColor.Pixmap)
				{
					bg = background;
					pixmap = backgroundPixmap;
					tx = 0;
					ty = 0;
				}
				else
				{
					bg = background;
					pixmap = null;
					tx = 0;
					ty = 0;
				}
			}

} // class InputOutputWidget

} // namespace Xsharp
