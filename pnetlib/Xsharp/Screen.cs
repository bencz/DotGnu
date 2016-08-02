/*
 * Screen.cs - Access an X display screen.
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

/// <summary>
/// <para>The <see cref="T:Xsharp.Screen"/> class manages a single
/// screen on an X display server connection.</para>
/// </summary>
public sealed class Screen
{
	// Internal state.
	private Display dpy;
	private int number;
	internal IntPtr screen;
	private Xsharp.RootWindow rootWindow;
	private Widget placeholder;
	private IntPtr visual;
	private Colormap defaultColormap;
	private IntPtr[] defaultGCs;
	private int numDefaultGCs;
	private IntPtr[] bitmapGCs;
	private int numBitmapGCs;
	private const int GCCacheSize = 16;
	private Color[] standardColors;
	internal GrabWindow grabWindow;

	// Constructor.
	internal Screen(Display dpy, int number, IntPtr screen)
			{
				// Copy parameters in from the create process.
				this.dpy = dpy;
				this.number = number;
				this.screen = screen;

				// Create the root window instance for this screen.
				rootWindow = new Xsharp.RootWindow
					(dpy, this, Xlib.XRootWindowOfScreen(screen));

				// Get the default root visual for this screen.
				visual = Xlib.XDefaultVisualOfScreen(screen);

				// Create a "Colormap" object for the default colormap.
				defaultColormap = new Colormap
					(dpy, this, Xlib.XDefaultColormapOfScreen(screen));

				// Create the GC cache.
				defaultGCs = new IntPtr [GCCacheSize];
				bitmapGCs = new IntPtr [GCCacheSize];

				// Initialize the standard colors.
				InitStandardColors();

				// Create the placeholder window for parent-less widgets.
				placeholder = new PlaceholderWindow(rootWindow);

				// Create the grab window for managing popup window events.
				grabWindow = new GrabWindow(rootWindow);
			}

	/// <summary>
	/// <para>Get the display that owns this screen.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The owning <see cref="T:Xsharp.Display"/> instance.</para>
	/// </value>
	public Display DisplayOfScreen
			{
				get
				{
					return dpy;
				}
			}

	/// <summary>
	/// <para>Get the index of this screen within its owning display.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The screen index, from <c>0</c> to
	/// <c>DisplayOfScreen.ScreenCount - 1</c></para>.
	/// </value>
	public int ScreenNumber
			{
				get
				{
					return number;
				}
			}

	/// <summary>
	/// <para>Get the root window of this screen.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.Window"/> instance that corresponds
	/// to the root window of this screen.</para>
	/// </value>
	public Xsharp.RootWindow RootWindow
			{
				get
				{
					return rootWindow;
				}
			}

	/// <summary>
	/// <para>Get the default colormap for this screen.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The default <see cref="T:Xsharp.Colormap"/> instance.</para>
	/// </value>
	public Colormap DefaultColormap
			{
				get
				{
					return defaultColormap;
				}
			}

	/// <summary>
	/// <para>Get the default depth of this screen.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The default depth value.</para>
	/// </value>
	public int DefaultDepth
			{
				get
				{
					try
					{
						dpy.Lock();
						return (int)(Xlib.XDefaultDepthOfScreen(screen));
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Get the width of this screen.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The screen width.</para>
	/// </value>
	public int Width
			{
				get
				{
					try
					{
						dpy.Lock();
						return (int)(Xlib.XWidthOfScreen(screen));
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Get the height of this screen.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The screen height.</para>
	/// </value>
	public int Height
			{
				get
				{
					try
					{
						dpy.Lock();
						return (int)(Xlib.XHeightOfScreen(screen));
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Get the placeholder widget for this screen.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The placeholder widget.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>All widgets in X11 must have a parent, but sometimes
	/// upper layers want to create widgets independently of their final
	/// position and then reparent them into place afterwards.  The
	/// placeholder widget can be used to hold a widget while it is
	/// in the "parent-less" state.</para>
	///
	/// <para>When a widget is a child of the placeholder, its
	/// <c>Parent</c>, <c>NextAbove</c>, and <c>NextBelow</c> properties
	/// will all return <see langword="null"/>.</para>
	/// </remarks>
	public Widget Placeholder
			{
				get
				{
					return placeholder;
				}
			}

	/// <summary>
	/// <para>Send a wakeup message to the event queue for this screen.</para>
	/// </summary>
	///
	/// <remarks>
	/// <para>Wakeup messages are sent from one thread to the event queue
	/// thread, to cause the event queue to stop blocking and return back to
	/// the caller.  The wakeup message involves a round trip to the
	/// X server, so it should be used sparingly.</para>
	/// </remarks>
	public void Wakeup()
			{
				try
				{
					IntPtr display = dpy.Lock();
					Xlib.XSharpSendWakeup
						(display, placeholder.GetWidgetHandle());
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Get the default visual for this screen.  We currently only
	// support screens with one visual.
	internal IntPtr DefaultVisual
			{
				get
				{
					return visual;
				}
			}

	// Reuse an Xlib GC value that was previously allocated to this screen.
	// Returns IntPtr.Zero if there are no cached GC's available.
	internal IntPtr GetGC(bool isBitmapGC)
			{
				if(!isBitmapGC)
				{
					if(numDefaultGCs > 0)
					{
						return defaultGCs[--numDefaultGCs];
					}
				}
				else
				{
					if(numBitmapGCs > 0)
					{
						return bitmapGCs[--numBitmapGCs];
					}
				}
				return IntPtr.Zero;
			}

	// Release an Xlib GC value from a GC object that was disposed.
	internal void ReleaseGC(IntPtr gc, bool isBitmapGC)
			{
				if(!isBitmapGC)
				{
					if(numDefaultGCs < GCCacheSize)
					{
						defaultGCs[numDefaultGCs++] = gc;
					}
					else
					{
						Xlib.XFreeGC(dpy.dpy, gc);
					}
				}
				else
				{
					if(numBitmapGCs < GCCacheSize)
					{
						bitmapGCs[numBitmapGCs++] = gc;
					}
					else
					{
						Xlib.XFreeGC(dpy.dpy, gc);
					}
				}
			}

	// Initialize the standard colors on this screen.
	private void InitStandardColors()
			{
				standardColors = new Color [((int)(StandardColor.Last)) - 1];

				standardColors[((int)StandardColor.Foreground) - 1]
					= new Color(0x00, 0x00, 0x00);
				standardColors[((int)StandardColor.Background) - 1]
					= new Color(0xD4, 0xD0, 0xC8);
				standardColors[((int)StandardColor.EndBackground) - 1]
					= new Color(0xC0, 0xC0, 0xC0);
				standardColors[((int)StandardColor.HighlightForeground) - 1]
					= new Color(0xFF, 0xFF, 0xFF);
				standardColors[((int)StandardColor.HighlightBackground) - 1]
					= new Color(0x0A, 0x24, 0x6A);
				standardColors[((int)StandardColor.HighlightEndBackground) - 1]
					= new Color(0xA6, 0xCA, 0xF0);
				standardColors[((int)StandardColor.TopShadow) - 1]
					= new Color(0xFF, 0xFF, 0xFF);
				standardColors[((int)StandardColor.TopShadowEnhance) - 1]
					= new Color(0xD4, 0xD0, 0xC8);
				standardColors[((int)StandardColor.BottomShadow) - 1]
					= new Color(0x80, 0x80, 0x80);
				standardColors[((int)StandardColor.BottomShadowEnhance) - 1]
					= new Color(0x40, 0x40, 0x40);
				standardColors[((int)StandardColor.Trim) - 1]
					= new Color(0x00, 0x00, 0x00);

				standardColors[((int)StandardColor.ContentForeground) - 1]
					= new Color(0x00, 0x00, 0x00);
				standardColors[((int)StandardColor.ContentBackground) - 1]
					= new Color(0xFF, 0xFF, 0xFF);
				standardColors
					[((int)StandardColor.ContentHighlightForeground) - 1]
					= new Color(0xFF, 0xFF, 0xFF);
				standardColors
					[((int)StandardColor.ContentHighlightBackground) - 1]
					= new Color(0x0A, 0x24, 0x6A);
				standardColors[((int)StandardColor.ContentTopShadow) - 1]
					= new Color(0xFF, 0xFF, 0xFF);
				standardColors
					[((int)StandardColor.ContentTopShadowEnhance) - 1]
					= new Color(0xD4, 0xD0, 0xC8);
				standardColors[((int)StandardColor.ContentBottomShadow) - 1]
					= new Color(0x80, 0x80, 0x80);
				standardColors
					[((int)StandardColor.ContentBottomShadowEnhance) - 1]
					= new Color(0x40, 0x40, 0x40);
				standardColors[((int)StandardColor.ContentTrim) - 1]
					= new Color(0x00, 0x00, 0x00);

				standardColors[((int)StandardColor.Inherit) - 1]
					= standardColors[((int)StandardColor.Background) - 1];
				standardColors[((int)StandardColor.Pixmap) - 1]
					= standardColors[((int)StandardColor.Background) - 1];
			}

	// Expand a standard color into a full color value.
	internal Color ToColor(StandardColor color)
			{
				if(color > StandardColor.RGB && color < StandardColor.Last)
				{
					return standardColors[((int)color) - 1];
				}
				else
				{
					return new Color(0x00, 0x00, 0x00);
				}
			}

} // class Screen

} // namespace Xsharp
