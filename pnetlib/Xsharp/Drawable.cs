/*
 * Drawable.cs - Base class for widgets and pixmaps.
 *
 * Copyright (C) 2002, 2003, 2004  Southern Storm Software, Pty Ltd.
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
/// <para>The <see cref="T:Xsharp.Drawable"/> class manages widget
/// windows or pixmaps on an X display screen.</para>
///
/// <para>This is an abstract base class.  Instantiate or inherit one of
/// the classes <see cref="T:Xsharp.Pixmap"/>,
/// <see cref="T:Xsharp.InputOutputWidget"/>,
/// <see cref="T:Xsharp.InputOnlyWidget"/>, or
/// <see cref="T:Xsharp.TopLevelWindow"/> in user applications.</para>
/// </summary>
public abstract class Drawable : IDisposable
{
	// Internal state.
	internal Display dpy;
	internal Screen screen;
	internal DrawableKind kind;
	internal XDrawable handle;
	internal int width, height;

	// Constructor.
	internal Drawable(Display dpy, Screen screen, DrawableKind kind)
			{
				this.dpy = dpy;
				this.screen = screen;
				this.kind = kind;
			}

	/// <summary>
	/// <para>Destroy the drawable if it is currently active.</para>
	/// </summary>
	~Drawable()
			{
				Destroy();
			}

	// Set the handle for this drawable, assuming it is a widget.
	internal void SetWidgetHandle(XWindow handle)
			{
				try
				{
					dpy.Lock();
					this.handle = (XDrawable)handle;
					dpy.handleMap[handle] = (Widget)this;
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Set the handle for this drawable, assuming it is a pixmap.
	internal void SetPixmapHandle(XPixmap handle)
			{
				try
				{
					dpy.Lock();
					this.handle = (XDrawable)handle;
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Get the handle for drawable, assuming that it is
	// a widget.  Should be called with the display lock.
	public XWindow GetWidgetHandle()
			{
				if(handle != XDrawable.Zero)
				{
					return (XWindow)handle;
				}
				else
				{
					throw new XInvalidOperationException
						(S._("X_WidgetDestroyed"));
				}
			}
			
	public bool HasWidgetHandle() 
		{
			return handle != XDrawable.Zero;
		}

	// Get the handle for drawable, assuming that it is
	// a pixmap.  Should be called with the display lock.
	internal XPixmap GetPixmapHandle()
			{
				if(handle != XDrawable.Zero)
				{
					return (XPixmap)handle;
				}
				else if(kind == DrawableKind.Pixmap)
				{
					throw new XInvalidOperationException
						(S._("X_PixmapDestroyed"));
				}
				else
				{
					throw new XInvalidOperationException
						(S._("X_BitmapDestroyed"));
				}
			}

	// Get the handle for drawable, for use in a graphics object.
	// Should be called with the display lock.
	internal XDrawable GetGCHandle()
			{
				if(kind == DrawableKind.InputOnlyWidget)
				{
					throw new XInvalidOperationException
						(S._("X_GraphicsIsOutputOnly"));
				}
				else if(handle != XDrawable.Zero)
				{
					return handle;
				}
				else if(kind == DrawableKind.Widget)
				{
					throw new XInvalidOperationException
						(S._("X_WidgetDestroyed"));
				}
				else if(kind == DrawableKind.Pixmap)
				{
					throw new XInvalidOperationException
						(S._("X_PixmapDestroyed"));
				}
				else if(kind == DrawableKind.DoubleBuffer)
				{
					throw new XInvalidOperationException
						(S._("X_DoubleBufferDestroyed"));
				}
				else
				{
					throw new XInvalidOperationException
						(S._("X_BitmapDestroyed"));
				}
			}

	/// <summary>
	/// <para>Destroy this drawable if it is currently active.</para>
	/// </summary>
	///
	/// <remarks>
	/// <para>This method implements the <see cref="T:System.IDisposable"/>
	/// interface.</para>
	/// </remarks>
	public void Dispose()
			{
				Destroy();
			}

	/// <summary>
	/// <para>Destroy this drawable if it is currently active.</para>
	/// </summary>
	public virtual void Destroy()
			{
				// Nothing to do here: overridden in subclasses.
			}

	/// <summary>
	/// <para>Get the display that is associated with this drawable.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.Display"/> instance.</para>
	/// </value>
	public Display Display
			{
				get
				{
					return dpy;
				}
			}

	/// <summary>
	/// <para>Get the screen that is associated with this drawable.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.Screen"/> instance.</para>
	/// </value>
	public Screen Screen
			{
				get
				{
					return screen;
				}
			}

	/// <summary>
	/// <para>Get the kind of this drawable.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.DrawableKind"/> value.</para>
	/// </value>
	public DrawableKind Kind
			{
				get
				{
					return kind;
				}
			}

	/// <summary>
	/// <para>Get the width of this drawable.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The width of this drawable in pixels.</para>
	/// </value>
	public int Width
			{
				get
				{
					return width;
				}
			}

	/// <summary>
	/// <para>Get the height of this drawable.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The height of this widget in drawable.</para>
	/// </value>
	public int Height
			{
				get
				{
					return height;
				}
			}

	/// <summary>
	/// <para>Get the application object that owns this drawable.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The application object.</para>
	/// </value>
	public Application Application
			{
				get
				{
					return dpy.Application;
				}
			}

	// Convert a color into a pixel value, relative to this drawable.
	internal virtual XPixel ToPixel(Color color)
			{
				// Expand standard color indices.
				if(color.Index != StandardColor.RGB)
				{
					color = screen.ToColor(color.Index);
				}

				// Map the color using the screen's colormap.
				return screen.DefaultColormap.RGBToPixel(color);
			}

} // class Drawable

} // namespace Xsharp
