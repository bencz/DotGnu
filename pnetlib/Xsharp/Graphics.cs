/*
 * Graphics.cs - Graphic drawing objects.
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
using Xsharp.Types;
using OpenSystem.Platform.X11;

/// <summary>
/// <para>The <see cref="T:Xsharp.Graphics"/> class manages a graphic
/// context for a <see cref="T:Xsharp.Drawable"/> instance for
/// drawing to an X display server.</para>
/// </summary>
///
/// <remarks>
/// <para>Graphics objects should not be created and used for long
/// periods of time.  This is in opposition to normal X practice.</para>
///
/// <para>Most X widget systems initialize graphical contexts at startup
/// time and then use them over and over after that.  However, this can make
/// it difficult to handle changes to the user's preferred color theme.</para>
///
/// <para>Note: although <see cref="T:Xsharp.Graphics"/> objects should not
/// be used for long periods of time, the underlying Xlib graphics contexts
/// are reused to avoid over-allocation of X display server resources.
/// The color theme will be properly tracked when Xlib contexts are reused.
/// This behaviour is transparent to the user of the class.</para>
///
/// <para>There are two ways to obtain a graphics object.  The first is via the
/// <c>Paint</c> event on a widget.  You should always use the supplied
/// object in this case, because it has already been initialized with the
/// correct clipping region to perform the paint operation.</para>
///
/// <para>The second way to obtain a graphics object is by directly
/// constructing it from a widget or pixmap, and then disposing it
/// when drawing operations are complete.</para>
/// </remarks>
public sealed class Graphics : IDisposable
{
	// Internal state.
	internal Display dpy;
	private Drawable drawable;
	internal XDrawable drawableHandle;
	internal IntPtr gc;
	private float dpiX;
	private float dpiY;
	private Color foreground;
	private Color background;
	private Pixmap tile;
	private Bitmap stipple;
	private byte[] dashPattern;
	internal Region exposeRegion;
	internal Region clipRegion;
	private bool isDisposed;


	/// <summary>
	/// <para>Constructs a new <see cref="T:Xsharp.Graphics"/> object and
	/// attaches it to a <see cref="T:Xsharp.Drawable"/> instance.</para>
	/// </summary>
	///
	/// <param name="drawable">
	/// <para>The drawable to attach this graphics context to.  If the
	/// drawable is a widget, the foreground and background colors of the
	/// graphics object will be initially set to the widget's standard
	/// foreground and background colors.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="drawable"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XInvalidOperationException">
	/// <para>Raised if <paramref name="drawable"/> does not support
	/// output, is disposed, or is the root window.</para>
	/// </exception>
	public Graphics(Drawable drawable)
			{
				if(drawable == null)
				{
					throw new ArgumentNullException("drawable");
				}
				else if(drawable.Kind == DrawableKind.InputOnlyWidget)
				{
					throw new XInvalidOperationException
						(S._("X_GraphicsIsOutputOnly"));
				}
				else if(drawable is RootWindow)
				{
					throw new XInvalidOperationException
						(S._("X_NonRootOperation"));
				}
				dpy = drawable.dpy;
				this.drawable = drawable;
				XGCValues gcValues = new XGCValues();
				InputOutputWidget widget = (drawable as InputOutputWidget);
				DoubleBuffer buffer = (drawable as DoubleBuffer);
				Bitmap bitmap = (drawable as Bitmap);
				if(widget != null)
				{
					foreground = widget.Foreground;
					background = widget.Background;
				}
				else if(buffer != null)
				{
					foreground = buffer.Widget.Foreground;
					background = buffer.Widget.Background;
				}
				else if(bitmap != null)
				{
					foreground = new Color(0x00, 0x00, 0x00);
					background = new Color(0xFF, 0xFF, 0xFF);
				}
				else
				{
					foreground = new Color (StandardColor.Foreground);
					background = new Color (StandardColor.Background);
				}
				gcValues.foreground = drawable.ToPixel(foreground);
				gcValues.background = drawable.ToPixel(background);
				if(drawable is DoubleBuffer)
				{
					((DoubleBuffer)drawable).Start(this);
				}
				try
				{
					IntPtr display = dpy.Lock();
					drawableHandle = drawable.GetGCHandle();
					gc = drawable.screen.GetGC(bitmap != null);
					if(gc == IntPtr.Zero)
					{
						// Create a new GC because the cache is empty.
						gc = Xlib.XCreateGC(display, drawableHandle,
											(uint)(GCValueMask.GCForeground |
												   GCValueMask.GCBackground),
											ref gcValues);
						if(gc == IntPtr.Zero)
						{
							Display.OutOfMemory();
						}
					}
					else
					{
						// Reset the cached GC back to the default settings.
						// Xlib will take care of stripping the list down
						// to just the changes that need to be applied.
						gcValues.function = Xsharp.GCFunction.GXcopy;
						gcValues.plane_mask = ~((XPixel)0);
						gcValues.line_width = 0;
						gcValues.line_style = Xsharp.LineStyle.LineSolid;
						gcValues.cap_style = Xsharp.CapStyle.CapButt;
						gcValues.join_style = Xsharp.JoinStyle.JoinMiter;
						gcValues.fill_style = Xsharp.FillStyle.FillSolid;
						gcValues.fill_rule = Xsharp.FillRule.EvenOddRule;
						gcValues.arc_mode = Xsharp.ArcMode.ArcPieSlice;
						gcValues.ts_x_origin = 0;
						gcValues.ts_y_origin = 0;
						gcValues.subwindow_mode =
							Xsharp.SubwindowMode.ClipByChildren;
						gcValues.graphics_exposures = true;
						gcValues.clip_x_origin = 0;
						gcValues.clip_y_origin = 0;
						gcValues.clip_mask = XPixmap.Zero;
						gcValues.dash_offset = 0;
						gcValues.dashes = (sbyte)4;
						Xlib.XChangeGC(display, gc,
									   (uint)(GCValueMask.GCFunction |
											  GCValueMask.GCPlaneMask |
											  GCValueMask.GCForeground |
											  GCValueMask.GCBackground |
											  GCValueMask.GCLineWidth |
											  GCValueMask.GCLineStyle |
											  GCValueMask.GCCapStyle |
											  GCValueMask.GCJoinStyle |
											  GCValueMask.GCFillStyle |
											  GCValueMask.GCFillRule |
											  GCValueMask.GCTileStipXOrigin |
											  GCValueMask.GCTileStipYOrigin |
											  GCValueMask.GCSubwindowMode |
											  GCValueMask.GCGraphicsExposures |
											  GCValueMask.GCClipXOrigin |
											  GCValueMask.GCClipYOrigin |
											  GCValueMask.GCClipMask |
											  GCValueMask.GCDashOffset |
											  GCValueMask.GCDashList |
											  GCValueMask.GCArcMode),
									   ref gcValues);
					}

					int sn = drawable.screen.ScreenNumber;
					double px, mm;

					px = (double)Xlib.XDisplayWidth(display, sn);
					mm = (double)Xlib.XDisplayWidthMM(display, sn);
					dpiX = (float)((px * 25.4) / mm);

					px = (double)Xlib.XDisplayHeight(display, sn);
					mm = (double)Xlib.XDisplayHeightMM(display, sn);
					dpiY = (float)((px * 25.4) / mm);
				}
				finally
				{
					dpy.Unlock();
				}
				if(drawable is DoubleBuffer)
				{
					((DoubleBuffer)drawable).ClearAtStart(this);
				}

				isDisposed = false;
			}

	/// <summary>
	/// <para>Finalizer to clean up any managed resources by calling the
	/// Dispose method.</para>
	/// </summary>
	~Graphics()
			{
				if(isDisposed == false)
				{
					this.Dispose();
				}
			}

	/// <summary>
	/// <para>Dispose this graphics context object.</para>
	/// </summary>
	///
	/// <remarks>
	/// <para>This method implements the <see cref="T:System.IDisposable"/>
	/// interface.</para>
	/// </remarks>
	public void Dispose()
			{
				try
				{
					IntPtr display = dpy.Lock();
					if(gc != IntPtr.Zero)
					{
						// Flush the double buffer if necessary.
						if(drawable is DoubleBuffer)
						{
							((DoubleBuffer)drawable).End(this);
						}

						// Release the GC back to the screen's cache so
						// that we can reuse it the next time we need a GC.
						drawable.screen.ReleaseGC(gc, (drawable is Bitmap));
						gc = IntPtr.Zero;
					}
				}
				finally
				{
					dpy.Unlock();
				}
				if(exposeRegion != null)
				{
					exposeRegion.Dispose();
					exposeRegion = null;
				}
				if(clipRegion != null ) {
					clipRegion.Dispose();
					clipRegion = null;
				}
				isDisposed = true;
			}

	/// <summary>
	/// <para>Lock this graphics' display and validate the parameters.</para>
	/// </summary>
	///
	/// <returns>
	/// <para>Returns the display pointer.</para>
	/// </returns>
	public IntPtr Lock()
			{
				IntPtr display = dpy.Lock();
				if(drawable.handle != XDrawable.Zero && gc != IntPtr.Zero)
				{
					// All of the relevant handles are still valid.
					return display;
				}
				else
				{
					// Validate the drawable handle to see if it is disposed.
					drawable.GetGCHandle();

					// The graphics object must have been destroyed.
					throw new XInvalidOperationException
								(S._("X_GraphicsDestroyed"));
				}
			}

	/// <summary>
	/// <para>Unlock this graphics' display.</para>
	/// </summary>
	public void Unlock()
			{
				dpy.Unlock();
			}


	/// <summary>
	/// <para>Get the dpi of the screen for the x axis.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The dpi of the screen for the x axis.</para>
	/// </value>
	public float DpiX
			{
				get { return dpiX; }
			}

	/// <summary>
	/// <para>Get the dpi of the screen for the y axis.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The dpi of the screen for the y axis.</para>
	/// </value>
	public float DpiY
			{
				get { return dpiY; }
			}

	/// <summary>
	/// <para>Get the X Drawable related to this graphics object.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The drawable handle <see cref="T:Xsharp.XDrawable"/> value.</para>
	/// </value>
	public XDrawable DrawableHandle
			{
				get { return drawableHandle; }
			}

	/// <summary>
	/// <para>Get or set the foreground drawing color.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The foreground <see cref="T:Xsharp.Color"/> value.</para>
	/// </value>
	public Color Foreground
			{
				get
				{
					return foreground;
				}
				set
				{
					if(foreground != value)
					{
						try
						{
							IntPtr display = Lock();
							foreground = value;
							Xlib.XSetForeground
								(display, gc, drawable.ToPixel(value));
						}
						finally
						{
							dpy.Unlock();
						}
					}
				}
			}

	/// <summary>
	/// <para>Get or set the background drawing color.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The background <see cref="T:Xsharp.Color"/> value.</para>
	/// </value>
	public Color Background
			{
				get
				{
					return background;
				}
				set
				{
					if(background != value)
					{
						try
						{
							IntPtr display = Lock();
							background = value;
							Xlib.XSetBackground
								(display, gc, drawable.ToPixel(value));
						}
						finally
						{
							dpy.Unlock();
						}
					}
				}
			}

	/// <summary>
	/// <para>Get or set the drawing function mode.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.GCFunction"/> value for the mode.</para>
	/// </value>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if set to an invalid value.</para>
	/// </exception>
	public Xsharp.GCFunction Function
			{
				get
				{
					try
					{
						IntPtr display = Lock();
						XGCValues values;
						Xlib.XGetGCValues(display, gc,
										  (uint)(GCValueMask.GCFunction),
										  out values);
						return values.function;
					}
					finally
					{
						dpy.Unlock();
					}
				}
				set
				{
					if(value < Xsharp.GCFunction.GXclear ||
					   value > Xsharp.GCFunction.GXset)
					{
						throw new XException
							(String.Format(S._("X_Function"), (int)value));
					}
					try
					{
						IntPtr display = Lock();
						Xlib.XSetFunction(display, gc, (int)value);
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Get or set the subwindow mode.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.SubwindowMode"/> value
	/// for the mode.</para>
	/// </value>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if set to an invalid value.</para>
	/// </exception>
	public Xsharp.SubwindowMode SubwindowMode
			{
				get
				{
					try
					{
						IntPtr display = Lock();
						XGCValues values;
						Xlib.XGetGCValues(display, gc,
										  (uint)(GCValueMask.GCSubwindowMode),
										  out values);
						return values.subwindow_mode;
					}
					finally
					{
						dpy.Unlock();
					}
				}
				set
				{
					if(value < Xsharp.SubwindowMode.ClipByChildren ||
					   value > Xsharp.SubwindowMode.IncludeInferiors)
					{
						throw new XException
							(String.Format(S._("X_SubwindowMode"), (int)value));
					}
					try
					{
						IntPtr display = Lock();
						Xlib.XSetSubwindowMode(display, gc, (int)value);
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Get or set the line width.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The width of the lines to draw.</para>
	/// </value>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if set to an invalid value.</para>
	/// </exception>
	public int LineWidth
			{
				get
				{
					try
					{
						IntPtr display = Lock();
						XGCValues values;
						Xlib.XGetGCValues(display, gc,
										  (uint)(GCValueMask.GCLineWidth),
										  out values);
						return values.line_width;
					}
					finally
					{
						dpy.Unlock();
					}
				}
				set
				{
					if(value < 0)
					{
						throw new XException
							(String.Format(S._("X_LineWidth"), (int)value));
					}
					try
					{
						IntPtr display = Lock();
						XGCValues values = new XGCValues();
						values.line_width = value;
						Xlib.XChangeGC(display, gc,
									   (uint)(GCValueMask.GCLineWidth),
									   ref values);
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Get or set the line style mode.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.LineStyle"/> value for the mode.</para>
	/// </value>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if set to an invalid value.</para>
	/// </exception>
	public Xsharp.LineStyle LineStyle
			{
				get
				{
					try
					{
						IntPtr display = Lock();
						XGCValues values;
						Xlib.XGetGCValues(display, gc,
										  (uint)(GCValueMask.GCLineStyle),
										  out values);
						return values.line_style;
					}
					finally
					{
						dpy.Unlock();
					}
				}
				set
				{
					if(value < Xsharp.LineStyle.LineSolid ||
					   value > Xsharp.LineStyle.LineDoubleDash)
					{
						throw new XException
							(String.Format(S._("X_LineStyle"), (int)value));
					}
					try
					{
						IntPtr display = Lock();
						XGCValues values = new XGCValues();
						values.line_style = value;
						Xlib.XChangeGC(display, gc,
									   (uint)(GCValueMask.GCLineStyle),
									   ref values);
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Get or set the line capping style mode.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.CapStyle"/> value for the mode.</para>
	/// </value>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if set to an invalid value.</para>
	/// </exception>
	public Xsharp.CapStyle CapStyle
			{
				get
				{
					try
					{
						IntPtr display = Lock();
						XGCValues values;
						Xlib.XGetGCValues(display, gc,
										  (uint)(GCValueMask.GCCapStyle),
										  out values);
						return values.cap_style;
					}
					finally
					{
						dpy.Unlock();
					}
				}
				set
				{
					if(value < Xsharp.CapStyle.CapNotLast ||
					   value > Xsharp.CapStyle.CapProjecting)
					{
						throw new XException
							(String.Format(S._("X_CapStyle"), (int)value));
					}
					try
					{
						IntPtr display = Lock();
						XGCValues values = new XGCValues();
						values.cap_style = value;
						Xlib.XChangeGC(display, gc,
									   (uint)(GCValueMask.GCCapStyle),
									   ref values);
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Get or set the line join style mode.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.JoinStyle"/> value for the mode.</para>
	/// </value>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if set to an invalid value.</para>
	/// </exception>
	public Xsharp.JoinStyle JoinStyle
			{
				get
				{
					try
					{
						IntPtr display = Lock();
						XGCValues values;
						Xlib.XGetGCValues(display, gc,
										  (uint)(GCValueMask.GCJoinStyle),
										  out values);
						return values.join_style;
					}
					finally
					{
						dpy.Unlock();
					}
				}
				set
				{
					if(value < Xsharp.JoinStyle.JoinMiter ||
					   value > Xsharp.JoinStyle.JoinBevel)
					{
						throw new XException
							(String.Format(S._("X_JoinStyle"), (int)value));
					}
					try
					{
						IntPtr display = Lock();
						XGCValues values = new XGCValues();
						values.join_style = value;
						Xlib.XChangeGC(display, gc,
									   (uint)(GCValueMask.GCJoinStyle),
									   ref values);
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Get or set the dash pattern for line drawing.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The pattern to draw with.  Alternating sizes for the
	/// dashes and the gaps.</para>
	/// </value>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if the value is set to <see langword="null"/>.</para>
	/// </exception>
	public byte[] DashPattern
			{
				get
				{
					return dashPattern;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					if(value != dashPattern)
					{
						try
						{
							IntPtr display = Lock();
							dashPattern = value;
							Xlib.XSetDashes(display, gc, 0, dashPattern,
											dashPattern.Length);
						}
						finally
						{
							dpy.Unlock();
						}
					}
				}
			}

	/// <summary>
	/// <para>Get the fill style mode.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.FillStyle"/> value for the mode.</para>
	/// </value>
	public Xsharp.FillStyle FillStyle
			{
				get
				{
					try
					{
						IntPtr display = Lock();
						XGCValues values;
						Xlib.XGetGCValues(display, gc,
										  (uint)(GCValueMask.GCFillStyle),
										  out values);
						return values.fill_style;
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Get or set the fill rule mode.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.FillRule"/> value for the mode.</para>
	/// </value>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if set to an invalid value.</para>
	/// </exception>
	public Xsharp.FillRule FillRule
			{
				get
				{
					try
					{
						IntPtr display = Lock();
						XGCValues values;
						Xlib.XGetGCValues(display, gc,
										  (uint)(GCValueMask.GCFillRule),
										  out values);
						return values.fill_rule;
					}
					finally
					{
						dpy.Unlock();
					}
				}
				set
				{
					if(value < Xsharp.FillRule.EvenOddRule ||
					   value > Xsharp.FillRule.WindingRule)
					{
						throw new XException
							(String.Format(S._("X_FillRule"), (int)value));
					}
					try
					{
						IntPtr display = Lock();
						XGCValues values = new XGCValues();
						values.fill_rule = value;
						Xlib.XChangeGC(display, gc,
									   (uint)(GCValueMask.GCFillRule),
									   ref values);
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Get or set the arc drawing mode.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.ArcMode"/> value for the mode.</para>
	/// </value>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>Raised if set to an invalid value.</para>
	/// </exception>
	public Xsharp.ArcMode ArcMode
			{
				get
				{
					try
					{
						IntPtr display = Lock();
						XGCValues values;
						Xlib.XGetGCValues(display, gc,
										  (uint)(GCValueMask.GCArcMode),
										  out values);
						return values.arc_mode;
					}
					finally
					{
						dpy.Unlock();
					}
				}
				set
				{
					if(value < Xsharp.ArcMode.ArcChord ||
					   value > Xsharp.ArcMode.ArcPieSlice)
					{
						throw new XException
							(String.Format(S._("X_ArcMode"), (int)value));
					}
					try
					{
						IntPtr display = Lock();
						XGCValues values = new XGCValues();
						values.arc_mode = value;
						Xlib.XChangeGC(display, gc,
									   (uint)(GCValueMask.GCArcMode),
									   ref values);
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Get the fill style tiling pixmap.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.Pixmap"/> value for the pixmap,
	/// or <see langword="null"/> if the fill style is not
	/// <c>FillTiled</c>.</para>
	/// </value>
	public Pixmap Tile
			{
				get
				{
					return tile;
				}
			}

	/// <summary>
	/// <para>Get the fill style stippling bitmap.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.Bitmap"/> value for the bitmap,
	/// or <see langword="null"/> if the fill style is not
	/// <c>FillStippled</c> or <c>FillOpaqueStippled</c>.</para>
	/// </value>
	public Bitmap Stipple
			{
				get
				{
					return stipple;
				}
			}

	/// <summary>
	/// <para>Get the expose region for this graphics context.</para>
	/// </summary>
	///
	/// <value>
	/// <para>The <see cref="T:Xsharp.Region"/> value that was used
	/// to define the exposed area during an <c>OnPaint</c> call.
	/// Returns <see langword="null"/> if this object was not created
	/// in response to an expose.</para>
	/// </value>
	///
	/// <remarks>
	/// <para>The expose region will be automatically set as the clipping
	/// region when the graphics object is created.</para>
	/// </remarks>
	public Region ExposeRegion
			{
				get
				{
					return exposeRegion;
				}
			}

	/// <summary>
	/// <para>Set the fill style mode to "solid".</para>
	/// </summary>
	public void SetFillSolid()
			{
				try
				{
					IntPtr display = Lock();
					Xlib.XSetFillStyle
						(display, gc, (int)(Xsharp.FillStyle.FillSolid));
					if(tile != null)
					{
						Xlib.XSetTSOrigin(display, gc, 0, 0);
						tile = null;
					}
					else if(stipple != null)
					{
						Xlib.XSetTSOrigin(display, gc, 0, 0);
						stipple = null;
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Set the fill style mode to "tiled", with a specific
	/// tiling pixmap.</para>
	/// </summary>
	///
	/// <param name="tile">
	/// <para>The tiling pixmap to use.</para>
	/// </param>
	///
	/// <param name="xorigin">
	/// <para>The X co-ordinate of the tiling origin.</para>
	/// </param>
	///
	/// <param name="yorigin">
	/// <para>The Y co-ordinate of the tiling origin.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>The <paramref name="tile"/> value is
	/// <see langword="null"/>.</para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>The <paramref name="xorigin"/> or <paramref name="yorigin"/>
	/// value is out of range.</para>
	/// </exception>
	///
	/// <exception cref="T:System.XInvalidOperationException">
	/// <para>The <paramref name="tile"/> value is disposed.</para>
	/// </exception>
	public void SetFillTiled(Pixmap tile, int xorigin, int yorigin)
			{
				if(tile == null)
				{
					throw new ArgumentNullException("tile");
				}
				if(xorigin < -32768 || xorigin > 32767 ||
				   yorigin < -32768 || yorigin > 32767)
				{
					throw new XException(S._("X_PointCoordRange"));
				}
				try
				{
					IntPtr display = Lock();
					Xlib.XSetTile(display, gc, tile.GetPixmapHandle());
					Xlib.XSetFillStyle
						(display, gc, (int)(Xsharp.FillStyle.FillTiled));
					Xlib.XSetTSOrigin(display, gc, xorigin, yorigin);
					this.tile = tile;
					stipple = null;
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Set the fill style mode to "stippled", with a specific
	/// stippling bitmap.</para>
	/// </summary>
	///
	/// <param name="stipple">
	/// <para>The stippling bitmap to use.</para>
	/// </param>
	///
	/// <param name="xorigin">
	/// <para>The X co-ordinate of the stippling origin.</para>
	/// </param>
	///
	/// <param name="yorigin">
	/// <para>The Y co-ordinate of the stippling origin.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>The <paramref name="stipple"/> value is
	/// <see langword="null"/>.</para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>The <paramref name="xorigin"/> or <paramref name="yorigin"/>
	/// value is out of range.</para>
	/// </exception>
	///
	/// <exception cref="T:System.XInvalidOperationException">
	/// <para>The <paramref name="tile"/> value is disposed.</para>
	/// </exception>
	public void SetFillStippled(Bitmap stipple, int xorigin, int yorigin)
			{
				if(stipple == null)
				{
					throw new ArgumentNullException("stipple");
				}
				if(xorigin < -32768 || xorigin > 32767 ||
				   yorigin < -32768 || yorigin > 32767)
				{
					throw new XException(S._("X_PointCoordRange"));
				}
				try
				{
					IntPtr display = Lock();
					Xlib.XSetStipple(display, gc, stipple.GetPixmapHandle());
					Xlib.XSetFillStyle
						(display, gc, (int)(Xsharp.FillStyle.FillStippled));
					Xlib.XSetTSOrigin(display, gc, xorigin, yorigin);
					this.stipple = stipple;
					tile = null;
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Set the fill style mode to "opaque stippled", with a specific
	/// stippling bitmap.</para>
	/// </summary>
	///
	/// <param name="stipple">
	/// <para>The stippling bitmap to use.</para>
	/// </param>
	///
	/// <param name="xorigin">
	/// <para>The X co-ordinate of the stippling origin.</para>
	/// </param>
	///
	/// <param name="yorigin">
	/// <para>The Y co-ordinate of the stippling origin.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>The <paramref name="stipple"/> value is
	/// <see langword="null"/>.</para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>The <paramref name="xorigin"/> or <paramref name="yorigin"/>
	/// value is out of range.</para>
	/// </exception>
	///
	/// <exception cref="T:System.XInvalidOperationException">
	/// <para>The <paramref name="tile"/> value is disposed.</para>
	/// </exception>
	public void SetFillOpaqueStippled(Bitmap stipple, int xorigin, int yorigin)
			{
				if(stipple == null)
				{
					throw new ArgumentNullException("stipple");
				}
				if(xorigin < -32768 || xorigin > 32767 ||
				   yorigin < -32768 || yorigin > 32767)
				{
					throw new XException(S._("X_PointCoordRange"));
				}
				try
				{
					IntPtr display = Lock();
					Xlib.XSetStipple(display, gc, stipple.GetPixmapHandle());
					Xlib.XSetFillStyle
						(display, gc,
						 (int)(Xsharp.FillStyle.FillOpaqueStippled));
					Xlib.XSetTSOrigin(display, gc, xorigin, yorigin);
					this.stipple = stipple;
					tile = null;
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Set the clip area to a region object.</para>
	/// </summary>
	///
	/// <param name="r">
	/// <para>The clipping region to set.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>The <paramref name="r"/> value is
	/// <see langword="null"/>.</para>
	/// </exception>
	public void SetClipRegion(Region r)
			{
				if(r == null)
				{
					throw new ArgumentNullException("r");
				}
				try
				{
					IntPtr display = Lock();
					Xlib.XSetClipOrigin(display, gc, 0, 0);
					Xlib.XSetRegion(display, gc, r.GetRegion());
					if(clipRegion != r)
					{
						if(clipRegion != null)
							clipRegion.Dispose();
						clipRegion = new Region(r);
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Set the clip area to a region object, with a
	/// specified origin.</para>
	/// </summary>
	///
	/// <param name="r">
	/// <para>The clipping region to set.</para>
	/// </param>
	///
	/// <param name="xorigin">
	/// <para>The X co-ordinate of the clipping origin.</para>
	/// </param>
	///
	/// <param name="yorigin">
	/// <para>The Y co-ordinate of the clipping origin.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>The <paramref name="r"/> value is
	/// <see langword="null"/>.</para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>The <paramref name="xorigin"/> or <paramref name="yorigin"/>
	/// value is out of range.</para>
	/// </exception>
	public void SetClipRegion(Region r, int xorigin, int yorigin)
			{
				if(r == null)
				{
					throw new ArgumentNullException("r");
				}
				if(xorigin < -32768 || xorigin > 32767 ||
				   yorigin < -32768 || yorigin > 32767)
				{
					throw new XException(S._("X_PointCoordRange"));
				}
				try
				{
					IntPtr display = Lock();
					Xlib.XSetClipOrigin(display, gc, xorigin, yorigin);
					Xlib.XSetRegion(display, gc, r.GetRegion());
					if(clipRegion != r)
					{
						if(clipRegion != null)
							clipRegion.Dispose();
						clipRegion = new Region(r);
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Set the clip area to a bitmap mask</para>
	/// </summary>
	///
	/// <param name="mask">
	/// <para>The mask to set, or <see langword="null"/> to clear the
	/// clip mask.</para>
	/// </param>
	///
	/// <exception cref="T:System.XInvalidOperationException">
	/// <para>The <paramref name="mask"/> value is disposed.</para>
	/// </exception>
	public void SetClipMask(Bitmap mask)
			{
				try
				{
					IntPtr display = Lock();
					Xlib.XSetClipOrigin(display, gc, 0, 0);
					if(mask != null)
					{
						Xlib.XSetClipMask(display, gc, mask.GetPixmapHandle());
					}
					else
					{
						Xlib.XSetClipMask(display, gc, XPixmap.Zero);
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Set the clip area to a bitmap mask, with a specified
	/// clip origin.</para>
	/// </summary>
	///
	/// <param name="mask">
	/// <para>The mask to set, or <see langword="null"/> to clear the
	/// clip mask.</para>
	/// </param>
	///
	/// <param name="xorigin">
	/// <para>The X co-ordinate of the clipping origin.</para>
	/// </param>
	///
	/// <param name="yorigin">
	/// <para>The Y co-ordinate of the clipping origin.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>The <paramref name="xorigin"/> or <paramref name="yorigin"/>
	/// value is out of range.</para>
	/// </exception>
	///
	/// <exception cref="T:System.XInvalidOperationException">
	/// <para>The <paramref name="mask"/> value is disposed.</para>
	/// </exception>
	public void SetClipMask(Bitmap mask, int xorigin, int yorigin)
			{
				if(xorigin < -32768 || xorigin > 32767 ||
				   yorigin < -32768 || yorigin > 32767)
				{
					throw new XException(S._("X_PointCoordRange"));
				}
				try
				{
					IntPtr display = Lock();
					Xlib.XSetClipOrigin(display, gc, xorigin, yorigin);
					if(mask != null)
					{
						Xlib.XSetClipMask(display, gc, mask.GetPixmapHandle());
					}
					else
					{
						Xlib.XSetClipMask(display, gc, XPixmap.Zero);
					}
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Clear a rectangular area within a drawable.
	private void ClearArea(IntPtr display, int x, int y, int width, int height)
			{
				InputOutputWidget widget;
				Color bg;
				Pixmap pixmap;
				int tx, ty;
				XGCValues values;

				// Bail out if the area is zero-sized.
				if(width <= 0 || height <= 0)
				{
					return;
				}

				// Determine the background pattern to clear with.
				if(drawable is DoubleBuffer)
				{
					widget = ((DoubleBuffer)drawable).Widget;
				}
				else
				{
					widget = (drawable as InputOutputWidget);
				}
				if(widget != null)
				{
					widget.GetBackgroundInfo(out bg, out pixmap,
											 out tx, out ty);
				}
				else
				{
					bg = background;
					pixmap = null;
					tx = 0;
					ty = 0;
				}

				// Save the current GC settings that we are about to modify.
				Xlib.XGetGCValues(display, gc,
								  (uint)(GCValueMask.GCForeground |
								  		 GCValueMask.GCFillStyle |
								  		 GCValueMask.GCTileStipXOrigin |
										 GCValueMask.GCTileStipYOrigin),
								  out values);

				// Draw a filled rectangle using the background pattern.
				if(pixmap != null)
				{
					Xlib.XSetTile(display, gc, pixmap.GetPixmapHandle());
					Xlib.XSetTSOrigin(display, gc, tx, ty);
					Xlib.XSetFillStyle
						(display, gc, (int)(Xsharp.FillStyle.FillTiled));
				}
				else
				{
					Xlib.XSetForeground(display, gc, drawable.ToPixel(bg));
					Xlib.XSetFillStyle
						(display, gc, (int)(Xsharp.FillStyle.FillSolid));
				}
				Xlib.XFillRectangle(display, drawableHandle, gc,
							        x, y, width, height);

				// Restore the previous GC settings.
				Xlib.XSetForeground(display, gc, values.foreground);
				if(tile != null)
				{
					Xlib.XSetTile(display, gc, tile.GetPixmapHandle());
				}
				else if(stipple != null)
				{
					Xlib.XSetStipple(display, gc, stipple.GetPixmapHandle());
				}
				Xlib.XSetTSOrigin(display, gc, values.ts_x_origin,
								  values.ts_y_origin);
				Xlib.XSetFillStyle(display, gc, (int)(values.fill_style));
			}

	/// <summary>
	/// <para>Clear the entire drawable to its background color.</para>
	/// </summary>
	///
	/// <remarks>
	/// <para>If the drawable is an input-output widget, it uses the
	/// background color (or pixmap) of the widget, not the background
	/// color of the graphics object.</para>
	///
	/// <para>If the graphics object has an active clipping region, then the
	/// cleared area will be clipped to the region.</para>
	/// </remarks>
	public void Clear()
			{
				try
				{
					IntPtr display = Lock();
					ClearArea(display, 0, 0, drawable.Width, drawable.Height);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Clear a rectangular area within the drawable to
	/// its background color.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left point.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left point.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the rectangle.  The pixel at
	/// <i>x + width - 1</i> is the right-most side of the rectangle.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the rectangle.  The pixel at
	/// <i>y + height - 1</i> is the bottom-most side of the rectangle.</para>
	/// </param>
	///
	/// <remarks>
	/// <para>If the drawable is an input-output widget, it uses the
	/// background color (or pixmap) of the widget, not the background
	/// color of the graphics object.</para>
	///
	/// <para>If the graphics object has an active clipping region, then the
	/// cleared area will be clipped to the region.</para>
	/// </remarks>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate or size values is out of range.</para>
	/// </exception>
	public void Clear(int x, int y, int width, int height)
			{
				if(x < -32768 || x > 32767 || width < 0 || width > 65535 ||
				   y < -32768 || y > 32767 || height < 0 || height > 65535)
				{
					throw new XException(S._("X_RectCoordRange"));
				}
				try
				{
					IntPtr display = Lock();
					ClearArea(display, x, y, width, height);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Clear a rectangular area within the drawable to
	/// its background color.</para>
	/// </summary>
	///
	/// <param name="rect">
	/// <para>The rectangle to be cleared.</para>
	/// </param>
	///
	/// <remarks>
	/// <para>If the drawable is an input-output widget, it uses the
	/// background color (or pixmap) of the widget, not the background
	/// color of the graphics object.</para>
	///
	/// <para>If the graphics object has an active clipping region, then the
	/// cleared area will be clipped to the region.</para>
	/// </remarks>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate or size values is out of range.</para>
	/// </exception>
	public void Clear(Rectangle rect)
			{
				Clear(rect.x, rect.y, rect.width, rect.height);
			}

	/// <summary>
	/// <para>Draw a line between two points.</para>
	/// </summary>
	///
	/// <param name="x1">
	/// <para>The X co-ordinate of the first point.</para>
	/// </param>
	///
	/// <param name="y1">
	/// <para>The Y co-ordinate of the first point.</para>
	/// </param>
	///
	/// <param name="x2">
	/// <para>The X co-ordinate of the second point.</para>
	/// </param>
	///
	/// <param name="y2">
	/// <para>The Y co-ordinate of the second point.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate values is out of range.</para>
	/// </exception>
	public void DrawLine(int x1, int y1, int x2, int y2)
			{
				if(x1 < -32768 || x1 > 32767 ||
				   y1 < -32768 || y1 > 32767 ||
				   x2 < -32768 || x2 > 32767 ||
				   y2 < -32768 || y2 > 32767)
				{
					throw new XException(S._("X_PointCoordRange"));
				}
				try
				{
					IntPtr display = Lock();
					Xlib.XDrawLine(display, drawableHandle, gc,
								   x1, y1, x2, y2);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Draw a line between two points.</para>
	/// </summary>
	///
	/// <param name="p1">
	/// <para>The first point.</para>
	/// </param>
	///
	/// <param name="p2">
	/// <para>The second point.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate values is out of range.</para>
	/// </exception>
	public void DrawLine(Point p1, Point p2)
			{
				DrawLine(p1.x, p1.y, p2.x, p2.y);
			}

	/// <summary>
	/// <para>Draw a list of connected lines.</para>
	/// </summary>
	///
	/// <param name="points">
	/// <para>An array of points that define the connected lines.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="points"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate values is out of range, or
	/// <paramref name="points"/> has less than 2 elements.</para>
	/// </exception>
	public void DrawLines(Point[] points)
			{
				int len;

				// Validate the parameter.
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				len = points.Length;
				if(len < 2)
				{
					throw new XException(S._("X_Need2Points"));
				}
				else if(len > ((dpy.MaxRequestSize() - 3) / 2))
				{
					throw new XException(S._("X_MaxReqSizeExceeded"));
				}

				// Convert the "Point" array into an "XPoint" array.
				XPoint[] xpoints = new XPoint [len];
				int pt;
				for(pt = 0; pt < len; ++pt)
				{
					xpoints[pt] = new XPoint(points[pt].x, points[pt].y);
				}

				// Draw the connected series of lines.
				try
				{
					IntPtr display = Lock();
					Xlib.XDrawLines(display, drawableHandle, gc, xpoints,
									len, 0 /* CoordModeOrigin */);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Draw a filled polygon.</para>
	/// </summary>
	///
	/// <param name="points">
	/// <para>An array of points that define the polygon.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="points"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate values is out of range, or
	/// <paramref name="points"/> has less than 2 elements.</para>
	/// </exception>
	public void FillPolygon(Point[] points)
			{
				int len;

				// Validate the parameter.
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				len = points.Length;
				if(len < 2)
				{
					throw new XException(S._("X_Need2Points"));
				}
				else if(len > ((dpy.MaxRequestSize() - 3) / 2))
				{
					throw new XException(S._("X_MaxReqSizeExceeded"));
				}

				// Convert the "Point" array into an "XPoint" array.
				XPoint[] xpoints = new XPoint [len];
				int pt;
				for(pt = 0; pt < len; ++pt)
				{
					xpoints[pt] = new XPoint(points[pt].x, points[pt].y);
				}

				// Fill the polygon.
				try
				{
					IntPtr display = Lock();
					Xlib.XFillPolygon(display, drawableHandle, gc, xpoints,
									  len, 0 /* Complex */,
									  0 /* CoordModeOrigin */);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Draw a single point.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the point.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the point.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate values is out of range.</para>
	/// </exception>
	public void DrawPoint(int x, int y)
			{
				if(x < -32768 || x > 32767 ||
				   y < -32768 || y > 32767)
				{
					throw new XException(S._("X_PointCoordRange"));
				}
				try
				{
					IntPtr display = Lock();
					Xlib.XDrawPoint(display, drawableHandle, gc, x, y);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Draw a single point.</para>
	/// </summary>
	///
	/// <param name="point">
	/// <para>The co-ordinates of the point.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate values is out of range.</para>
	/// </exception>
	public void DrawPoint(Point point)
			{
				DrawPoint(point.x, point.y);
			}

	/// <summary>
	/// <para>Draw a list of points.</para>
	/// </summary>
	///
	/// <param name="points">
	/// <para>An array of points to be drawn.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="points"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate values is out of range, or
	/// <paramref name="points"/> has less than 1 element.</para>
	/// </exception>
	public void DrawPoints(Point[] points)
			{
				int len;

				// Validate the parameter.
				if(points == null)
				{
					throw new ArgumentNullException("points");
				}
				len = points.Length;
				if(len < 1)
				{
					throw new XException(S._("X_Need1Point"));
				}
				else if(len > (dpy.MaxRequestSize() - 3))
				{
					throw new XException(S._("X_MaxReqSizeExceeded"));
				}

				// Convert the "Point" array into an "XPoint" array.
				XPoint[] xpoints = new XPoint [len];
				int pt;
				for(pt = 0; pt < len; ++pt)
				{
					xpoints[pt] = new XPoint(points[pt].x, points[pt].y);
				}

				// Draw the points.
				try
				{
					IntPtr display = Lock();
					Xlib.XDrawPoints(display, drawableHandle, gc, xpoints,
									 len, 0 /* CoordModeOrigin */);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Draw a rectangle.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left point.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left point.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the rectangle.  The pixel at
	/// <i>x + width - 1</i> is the right-most side of the rectangle.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the rectangle.  The pixel at
	/// <i>y + height - 1</i> is the bottom-most side of the rectangle.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate or size values is out of range.</para>
	/// </exception>
	public void DrawRectangle(int x, int y, int width, int height)
			{
				if(x < -32768 || x > 32767 || width <  -32768 || width > 32767 ||
				   y < -32768 || y > 32767 || height < -32768 || height > 32767)
				{
					throw new XException(S._("X_RectCoordRange"));
				}
				if(width > 0 && height > 0)
				{
					try
					{
						// We subtract 1 from the values to convert sizes
						// that make sense into the "off by 1" values used
						// by the X protocol.  This makes "DrawRectangle"
						// consistent with "FillRectangle".
						IntPtr display = Lock();
						Xlib.XDrawRectangle(display, drawableHandle, gc,
									        x, y, width - 1, height - 1);
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Draw a rectangle.</para>
	/// </summary>
	///
	/// <param name="rect">
	/// <para>The position and size of the rectangle.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate or size values is out of range.</para>
	/// </exception>
	public void DrawRectangle(Rectangle rect)
			{
				DrawRectangle(rect.x, rect.y, rect.width, rect.height);
			}

	/// <summary>
	/// <para>Fill a rectangle.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left point.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left point.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the rectangle.  The pixel at
	/// <i>x + width - 1</i> is the right-most side of the rectangle.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the rectangle.  The pixel at
	/// <i>y + height - 1</i> is the bottom-most side of the rectangle.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate or size values is out of range.</para>
	/// </exception>
	public void FillRectangle(int x, int y, int width, int height)
			{
				if(x < -32768 || x > 32767 || width < -32768 || width > 32767 ||
				   y < -32768 || y > 32767 || height < -32768 || height > 32767)
				{
					throw new XException(S._("X_RectCoordRange"));
				}
				if(width > 0 && height > 0)
				{
					try
					{
						IntPtr display = Lock();
						Xlib.XFillRectangle(display, drawableHandle, gc,
									        x, y, width, height);
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Fill a rectangle.</para>
	/// </summary>
	///
	/// <param name="rect">
	/// <para>The position and size of the rectangle.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate or size values is out of range.</para>
	/// </exception>
	public void FillRectangle(Rectangle rect)
			{
				FillRectangle(rect.x, rect.y, rect.width, rect.height);
			}

	/// <summary>
	/// <para>Draw a list of rectangles.</para>
	/// </summary>
	///
	/// <param name="rects">
	/// <para>The list of rectangles to be drawn.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="rects"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate or size values is out of range, or
	/// <paramref name="rects"/> has less than 1 element.</para>
	/// </exception>
	public void DrawRectangles(Rectangle[] rects)
			{
				int len;

				// Validate the parameter.
				if(rects == null)
				{
					throw new ArgumentNullException("rects");
				}
				len = rects.Length;
				if(len < 1)
				{
					throw new XException(S._("X_Need1Rect"));
				}
				else if(len > ((dpy.MaxRequestSize() - 3) / 2))
				{
					throw new XException(S._("X_MaxReqSizeExceeded"));
				}

				// Convert the "Rectangle" array into an "XRectangle" array.
				XRectangle[] xrects = new XRectangle [len];
				int r;
				for(r = 0; r < len; ++r)
				{
					xrects[r] = new XRectangle(rects[r].x, rects[r].y,
											   rects[r].width - 1,
											   rects[r].height - 1);
				}

				// Draw the rectangles.
				try
				{
					IntPtr display = Lock();
					Xlib.XDrawRectangles(display, drawableHandle, gc,
										 xrects, len);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Fill a list of rectangles.</para>
	/// </summary>
	///
	/// <param name="rects">
	/// <para>The list of rectangles to be drawn.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="rects"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate or size values is out of range, or
	/// <paramref name="rects"/> has less than 1 element.</para>
	/// </exception>
	public void FillRectangles(Rectangle[] rects)
			{
				int len;

				// Validate the parameter.
				if(rects == null)
				{
					throw new ArgumentNullException("rects");
				}
				len = rects.Length;
				if(len < 1)
				{
					throw new XException(S._("X_Need1Rect"));
				}
				else if(len > ((dpy.MaxRequestSize() - 3) / 2))
				{
					throw new XException(S._("X_MaxReqSizeExceeded"));
				}

				// Convert the "Rectangle" array into an "XRectangle" array.
				XRectangle[] xrects = new XRectangle [len];
				int r;
				for(r = 0; r < len; ++r)
				{
					xrects[r] = new XRectangle(rects[r].x, rects[r].y,
											   rects[r].width,
											   rects[r].height);
				}

				// Draw the rectangles.
				try
				{
					IntPtr display = Lock();
					Xlib.XFillRectangles(display, drawableHandle, gc,
										 xrects, len);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Draw an arc.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left point of
	/// the arc's bounding rectangle.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left point of
	/// the arc's bounding rectangle.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the arc's bounding rectangle.  The pixel at
	/// <i>x + width - 1</i> is the right-most side of the rectangle.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the arc's bounding rectangle.  The pixel at
	/// <i>y + height - 1</i> is the bottom-most side of the rectangle.</para>
	/// </param>
	///
	/// <param name="startAngle">
	/// <para>The starting angle for the arc, in degrees.</para>
	/// </param>
	///
	/// <param name="sweepAngle">
	/// <para>The sweep angle for the arc, in degrees.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate or size values is out of range.</para>
	/// </exception>
	public void DrawArc(int x, int y, int width, int height,
						float startAngle, float sweepAngle)
			{
				if(x < -32768 || x > 32767 || width < -32768 || width > 32767 ||
				   y < -32768 || y > 32767 || height < -32768 || height > 32767)
				{
					throw new XException(S._("X_RectCoordRange"));
				}
				if(width > 0 && height > 0)
				{
					try
					{
						IntPtr display = Lock();
						Xlib.XDrawArc(display, drawableHandle, gc,
									  x, y, width, height,
									  (int)(startAngle * 64.0f),
									  (int)(sweepAngle * 64.0f));
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Draw an arc, with lines joining to the center.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left point of
	/// the arc's bounding rectangle.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left point of
	/// the arc's bounding rectangle.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the arc's bounding rectangle.  The pixel at
	/// <i>x + width - 1</i> is the right-most side of the rectangle.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the arc's bounding rectangle.  The pixel at
	/// <i>y + height - 1</i> is the bottom-most side of the rectangle.</para>
	/// </param>
	///
	/// <param name="startAngle">
	/// <para>The starting angle for the arc, in degrees.</para>
	/// </param>
	///
	/// <param name="sweepAngle">
	/// <para>The sweep angle for the arc, in degrees.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate or size values is out of range.</para>
	/// </exception>
	public void DrawPie(int x, int y, int width, int height,
						float startAngle, float sweepAngle)
			{
				if(x < -32768 || x > 32767 || width < -32768 || width > 32767 ||
				   y < -32768 || y > 32767 || height < -32768 || height > 32767)
				{
					throw new XException(S._("X_RectCoordRange"));
				}
				if(width > 0 && height > 0)
				{
					try
					{
						// Draw the arc portion.
						IntPtr display = Lock();
						Xlib.XDrawArc(display, drawableHandle, gc,
									  x, y, width, height,
									  (int)(startAngle * 64.0f),
									  (int)(sweepAngle * 64.0f));

					#if CONFIG_EXTENDED_NUMERICS
						// Calculate the location of the arc end-points
						// and then draw the connecting arc pie lines.
						XPoint[] xpoints = new XPoint [3];
						int xaxis = width / 2;
						int yaxis = height / 2;
						int xmiddle = x + xaxis;
						int ymiddle = y + yaxis;
						double radians = startAngle * Math.PI / 180.0;
						xpoints[0].x =
							(short)(xmiddle + Math.Cos(radians) * xaxis);
						xpoints[0].y =
							(short)(ymiddle - Math.Sin(radians) * yaxis);
						xpoints[1].x = (short)xmiddle;
						xpoints[1].y = (short)ymiddle;
						radians = (startAngle + sweepAngle) * Math.PI / 180.0;
						xpoints[2].x =
							(short)(xmiddle + Math.Cos(radians) * xaxis);
						xpoints[2].y =
							(short)(ymiddle - Math.Sin(radians) * yaxis);
						Xlib.XDrawLines(display, drawableHandle, gc, xpoints,
										3, 0 /* CoordModeOrigin */);
					#endif
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Fill an arc.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left point of
	/// the arc's bounding rectangle.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left point of
	/// the arc's bounding rectangle.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the arc's bounding rectangle.  The pixel at
	/// <i>x + width - 1</i> is the right-most side of the rectangle.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the arc's bounding rectangle.  The pixel at
	/// <i>y + height - 1</i> is the bottom-most side of the rectangle.</para>
	/// </param>
	///
	/// <param name="startAngle">
	/// <para>The starting angle for the arc, in degrees.</para>
	/// </param>
	///
	/// <param name="sweepAngle">
	/// <para>The sweep angle for the arc, in degrees.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate or size values is out of range.</para>
	/// </exception>
	public void FillArc(int x, int y, int width, int height,
						float startAngle, float sweepAngle)
			{
				if(x < -32768 || x > 32767 || width < -32768 || width > 32767 ||
				   y < -32768 || y > 32767 || height < -32768 || height > 32767)
				{
					throw new XException(S._("X_RectCoordRange"));
				}
				if(width > 0 && height > 0)
				{
					try
					{
						IntPtr display = Lock();
						Xlib.XFillArc(display, drawableHandle, gc,
									  x, y, width, height,
									  (int)(startAngle * 64.0f),
									  (int)(sweepAngle * 64.0f));
					}
					finally
					{
						dpy.Unlock();
					}
				}
			}

	/// <summary>
	/// <para>Draw an image to this graphics context.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left point of
	/// the image's bounding rectangle.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left point of
	/// the image's bounding rectangle.</para>
	/// </param>
	///
	/// <param name="image">
	/// <para>The image to be drawn.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="image"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate values is out of range.</para>
	/// </exception>
	public void DrawImage(int x, int y, Image image)
			{
				DrawImage(x,y, image, 0,0, image.Width, image.Height);
			}

	/// <summary>
	/// <para>Draw an image sub-part to this graphics context.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left point of
	/// the image's bounding rectangle.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left point of
	/// the image's bounding rectangle.</para>
	/// </param>
	///
	/// <param name="image">
	/// <para>The image to be drawn.</para>
	/// </param>
	///
	/// <param name="srcX">
	/// <para>The X co-ordinate of the top-left point of
	/// the image sub-part to be drawn.</para>
	/// </param>
	///
	/// <param name="srcY">
	/// <para>The Y co-ordinate of the top-left point of
	/// the image sub-part to be drawn.</para>
	/// </param>
	///
	/// <param name="srcWidth">
	/// <para>The width of the image sub-part to be drawn.</para>
	/// </param>
	///
	/// <param name="srcHeight">
	/// <para>The height of the image sub-part to be drawn.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="image"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate or size values is out of range.</para>
	/// </exception>
	public void DrawImage(int x, int y, Image image,
						  int srcX, int srcY, int srcWidth, int srcHeight)
			{
				// Validate the image parameter.
				if(image == null)
				{
					throw new ArgumentNullException("image", "Argument cannot be null");
				}

				// Bail out if the source co-ordinates are out of range.
				int width, height;
				width = image.Width;
				height = image.Height;
				if(srcX < 0 || srcX >= width ||
				   srcY < 0 || srcY >= height ||
				   srcWidth <= 0 || srcHeight <= 0 ||
				   srcWidth > width || srcHeight > height ||
				   srcX > (width - srcWidth) ||
				   srcY > (height - srcHeight))
				{
					throw new XException(S._("X_RectCoordRange"));
				}
				
				// Intersect image with clip rect
				Rectangle clipRect = clipRegion.ClipBox();
				
				int right = x + srcWidth;
				int clipRight = clipRect.x + clipRect.width;
				int bottom = y + srcHeight;
				int clipBottom = clipRect.y + clipRect.height;
				if(right < clipRect.x || x > clipRight || bottom < clipRect.y || y > clipBottom)
					return;

				if(right > clipRight)
				{
					srcWidth -= right - clipRight;
				}
				if(x < clipRect.x)
				{
					int i = clipRect.x - x;
					srcX += i;
					srcWidth -= i;
					x += i;
				}
				

				if(bottom > clipBottom)
				{
					srcHeight -= bottom - clipBottom;
				}
				if(y < clipRect.y)
				{
					int i = clipRect.y - y;
					srcY += i;
					srcHeight -= i;
					y += i;
				}
	
				// Can we take a short-cut using the XImage?
				if(image.ShouldUseXImage && !(drawable is Pixmap) &&
				   !(drawable is DoubleBuffer))
				{
					// Use "PutXImage" to draw through the clip mask,
					// to avoid having to create a Pixmap in the server.
					SetFillSolid();
					if(image.Mask != null) SetClipMask(image.Mask, x - srcX, y - srcY);
					PutXImage(image.XImage, srcX, srcY, x, y,
							  srcWidth, srcHeight);
				}
				else
				{
					// Set the context to "tiling" and fill the region.
					SetFillTiled(image.Pixmap, x - srcX, y - srcY);
					if(image.Mask != null) SetClipMask(image.Mask, x - srcX, y - srcY);
					FillRectangle(x, y, srcWidth, srcHeight);
				}

				// Revert the context to a sane fill mode.
				SetFillSolid();
				if(image.Mask != null) SetClipRegion(clipRegion);

			}

	/// <summary>
	/// <para>Blit a drawable sub-part to this graphics context.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left point of
	/// the destination bounding rectangle.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left point of
	/// the destination bounding rectangle.</para>
	/// </param>
	///
	/// <param name="drawable">
	/// <para>The drawable to copy from.</para>
	/// </param>
	///
	/// <param name="srcX">
	/// <para>The X co-ordinate of the top-left point of
	/// the drawable sub-part to be drawn.</para>
	/// </param>
	///
	/// <param name="srcY">
	/// <para>The Y co-ordinate of the top-left point of
	/// the drawable sub-part to be drawn.</para>
	/// </param>
	///
	/// <param name="srcWidth">
	/// <para>The width of the drawable sub-part to be drawn.</para>
	/// </param>
	///
	/// <param name="srcHeight">
	/// <para>The height of the drawable sub-part to be drawn.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="drawable"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate or size values is out of range.</para>
	/// </exception>
	public void BitBlt(int x, int y, Drawable drawable,
					   int srcX, int srcY, int srcWidth, int srcHeight)
			{
				// Validate the drawable parameter.
				if(drawable == null)
				{
					throw new ArgumentNullException("drawable");
				}

				// Bail out if the source co-ordinates are out of range.
				int width, height;
				width = drawable.Width;
				height = drawable.Height;
				if(srcX < 0 || srcX >= width ||
				   srcY < 0 || srcY >= height ||
				   srcWidth <= 0 || srcHeight <= 0 ||
				   srcWidth > width || srcHeight > height ||
				   srcX > (width - srcWidth) ||
				   srcY > (height - srcHeight))
				{
					throw new XException(S._("X_RectCoordRange"));
				}

				// Blit the contents of the drawable.
				try
				{
					IntPtr display = Lock();
					Xlib.XCopyArea(display, drawable.GetGCHandle(),
								   drawableHandle, gc,
								   srcX, srcY,
								   (uint)srcWidth, (uint)srcHeight,
								   x, y);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Blit a drawable's entire contents to this graphics context.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left point of
	/// the destination bounding rectangle.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left point of
	/// the destination bounding rectangle.</para>
	/// </param>
	///
	/// <param name="drawable">
	/// <para>The drawable to copy from.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="drawable"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate or size values is out of range.</para>
	/// </exception>
	public void BitBlt(int x, int y, Drawable drawable)
			{
				// Validate the drawable parameter.
				if(drawable == null)
				{
					throw new ArgumentNullException("drawable");
				}

				// Perform the blit operation.
				BitBlt(x, y, drawable, 0, 0, drawable.Width, drawable.Height);
			}

	/// <summary>
	/// <para>Draw a string at a particular position using a
	/// specified font.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the position to start drawing text.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the position to start drawing text.</para>
	/// </param>
	///
	/// <param name="str">
	/// <para>The string to be drawn.</para>
	/// </param>
	///
	/// <param name="font">
	/// <para>The font to use to draw the string.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate values is out of range.</para>
	/// </exception>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="font"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	public void DrawString(int x, int y, String str, Font font)
			{
				if(font == null)
				{
					throw new ArgumentNullException("font");
				}
				if(str == null || (str.Length == 0))
				{
					return;
				}
				font.DrawString(this, x, y, str, 0, str.Length);
			}

	/// <summary>
	/// <para>Get extent information for a particular font, when drawing
	/// onto this graphics context.</para>
	/// </summary>
	///
	/// <param name="font">
	/// <para>The font to obtain extents for.</para>
	/// </param>
	///
	/// <returns>
	/// <para>Returns the extent information.</para>
	/// </returns>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="font"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	public FontExtents GetFontExtents(Font font)
			{
				if(font == null)
				{
					throw new ArgumentNullException("font");
				}
				return font.GetFontExtents(this);
			}

	/// <summary>
	/// <para>Measure the width, ascent, and descent of a string,
	/// to calculate its extents when drawn on this graphics context
	/// using a specific font.</para>
	/// </summary>
	///
	/// <param name="str">
	/// <para>The string to be measured.</para>
	/// </param>
	///
	/// <param name="font">
	/// <para>The font to use to measure the string.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the string, in pixels.</para>
	/// </param>
	///
	/// <param name="ascent">
	/// <para>The ascent of the string, in pixels.</para>
	/// </param>
	///
	/// <param name="descent">
	/// <para>The descent of the string, in pixels.</para>
	/// </param>
	///
	/// <exception cref="T:System.ArgumentNullException">
	/// <para>Raised if <paramref name="font"/> is <see langword="null"/>.
	/// </para>
	/// </exception>
	public void MeasureString(String str, Font font, out int width,
							  out int ascent, out int descent)
			{
				if(font == null)
				{
					throw new ArgumentNullException("font");
				}
				if(str == null || (str.Length == 0))
				{
					width = 0;
					ascent = 0;
					descent = 0;
					return;
				}
				font.MeasureString(this, str, 0, str.Length,
								   out width, out ascent, out descent);
			}

	// Draw a radio button.
	private void DrawRadio(IntPtr display,
						   int x, int y, int width, int height,
						   XPixel topShadow,
						   XPixel topShadowEnhanced,
						   XPixel bottomShadow,
						   XPixel bottomShadowEnhanced,
						   XPixel foreground,
						   XPixel background)
			{
				// Adjust the top-left position of the radio button.
				x += (width - BuiltinBitmaps.RadioWidth) / 2;
				y += (height - BuiltinBitmaps.RadioHeight) / 2;
				width = BuiltinBitmaps.RadioWidth;
				height = BuiltinBitmaps.RadioHeight;

				// Draw the bitmaps.
				BuiltinBitmaps bitmaps = dpy.bitmaps;
				Xlib.XSetStipple(display, gc, bitmaps.RadioBottom);
				Xlib.XSetTSOrigin(display, gc, x, y);
				Xlib.XSetForeground(display, gc, bottomShadow);
				Xlib.XSetFillStyle(display, gc, (int)(FillStyle.FillStippled));
				Xlib.XFillRectangle(display, drawableHandle, gc,
									x, y, width, height);

				Xlib.XSetStipple(display, gc, bitmaps.RadioBottomEnhanced);
				Xlib.XSetForeground(display, gc, bottomShadowEnhanced);
				Xlib.XFillRectangle(display, drawableHandle, gc,
									x, y, width, height);

				Xlib.XSetStipple(display, gc, bitmaps.RadioTop);
				Xlib.XSetForeground(display, gc, topShadow);
				Xlib.XFillRectangle(display, drawableHandle, gc,
									x, y, width, height);

				Xlib.XSetStipple(display, gc, bitmaps.RadioTopEnhanced);
				Xlib.XSetForeground(display, gc, topShadowEnhanced);
				Xlib.XFillRectangle(display, drawableHandle, gc,
									x, y, width, height);

				Xlib.XSetStipple(display, gc, bitmaps.RadioForeground);
				Xlib.XSetForeground(display, gc, foreground);
				Xlib.XFillRectangle(display, drawableHandle, gc,
									x, y, width, height);

				Xlib.XSetStipple(display, gc, bitmaps.RadioBackground);
				Xlib.XSetForeground(display, gc, background);
				Xlib.XFillRectangle(display, drawableHandle, gc,
									x, y, width, height);

				// Restore the previously stipple pixmap.
				if(stipple != null)
				{
					Xlib.XSetStipple(display, gc, stipple.GetPixmapHandle());
				}
			}

	// Draw a simple bitmap.
	internal void DrawBitmap(int x, int y, int width, int height,
							 XPixmap bitmap)
			{
				try
				{
					IntPtr display = Lock();
					Xlib.XSetStipple(display, gc, bitmap);
					Xlib.XSetTSOrigin(display, gc, x, y);
					Xlib.XSetForeground(display, gc,
						drawable.ToPixel
							(new Color(StandardColor.Foreground)));
					Xlib.XSetFillStyle
						(display, gc, (int)(FillStyle.FillStippled));
					Xlib.XFillRectangle(display, drawableHandle, gc,
										x, y, width, height);
					Xlib.XSetTSOrigin(display, gc, 0, 0);
					Xlib.XSetFillStyle
						(display, gc, (int)(FillStyle.FillSolid));
				}
				finally
				{
					dpy.Unlock();
				}
			}

	/// <summary>
	/// <para>Draw a three-dimensional effect.</para>
	/// </summary>
	///
	/// <param name="x">
	/// <para>The X co-ordinate of the top-left point.</para>
	/// </param>
	///
	/// <param name="y">
	/// <para>The Y co-ordinate of the top-left point.</para>
	/// </param>
	///
	/// <param name="width">
	/// <para>The width of the rectangle.  The pixel at
	/// <i>x + width - 1</i> is the right-most side of the rectangle.</para>
	/// </param>
	///
	/// <param name="height">
	/// <para>The height of the rectangle.  The pixel at
	/// <i>y + height - 1</i> is the bottom-most side of the rectangle.</para>
	/// </param>
	///
	/// <param name="effect">
	/// <para>The particular effect to draw.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.XException">
	/// <para>One of the co-ordinate or size values is out of range.</para>
	/// </exception>
	public void DrawEffect(int x, int y, int width, int height, Effect effect)
			{
				if(x < -32768 || x > 32767 || width < 0 || width > 65535 ||
				   y < -32768 || y > 32767 || height < 0 || height > 65535)
				{
					throw new XException(S._("X_RectCoordRange"));
				}
				try
				{
					// Lock down the display while we do this.
					IntPtr display = Lock();

					// Get the colors that we need to draw the effect.
					XPixel topShadow;
					XPixel topShadowEnhance;
					XPixel bottomShadow;
					XPixel bottomShadowEnhance;
					XPixel background;
					XPixel trim;
					if((effect & Effect.ContentColors) != 0)
					{
						topShadow = drawable.ToPixel
							(new Color(StandardColor.ContentTopShadow));
						topShadowEnhance = drawable.ToPixel
							(new Color(StandardColor.ContentTopShadowEnhance));
						bottomShadow = drawable.ToPixel
							(new Color(StandardColor.ContentBottomShadow));
						bottomShadowEnhance = drawable.ToPixel(new Color
							(StandardColor.ContentBottomShadowEnhance));
						background = drawable.ToPixel
							(new Color(StandardColor.ContentBackground));
						trim = drawable.ToPixel
							(new Color(StandardColor.ContentTrim));
					}
					else
					{
						topShadow = drawable.ToPixel
							(new Color(StandardColor.TopShadow));
						topShadowEnhance = drawable.ToPixel
							(new Color(StandardColor.TopShadowEnhance));
						bottomShadow = drawable.ToPixel
							(new Color(StandardColor.BottomShadow));
						bottomShadowEnhance = drawable.ToPixel
							(new Color(StandardColor.BottomShadowEnhance));
						background = drawable.ToPixel
							(new Color(StandardColor.Background));
						trim = drawable.ToPixel(new Color(StandardColor.Trim));
					}

					// Save the current GC settings and set things
					// up for drawing 3D effect lines.
					XGCValues values = new XGCValues();
					Xlib.XGetGCValues(display, gc,
									  (uint)(GCValueMask.GCFunction |
											 GCValueMask.GCForeground |
									  		 GCValueMask.GCFillStyle |
									  		 GCValueMask.GCLineWidth |
											 GCValueMask.GCLineStyle |
											 GCValueMask.GCJoinStyle |
										     GCValueMask.GCCapStyle |
										     GCValueMask.GCTileStipXOrigin |
										     GCValueMask.GCTileStipYOrigin),
									  out values);
					XGCValues newValues = values;
					newValues.function = Function.GXcopy;
					newValues.line_width = 1;
					newValues.line_style = LineStyle.LineSolid;
					newValues.join_style = JoinStyle.JoinMiter;
					newValues.cap_style = CapStyle.CapProjecting;
					newValues.fill_style = FillStyle.FillSolid;
					Xlib.XChangeGC(display, gc,
								   (uint)(GCValueMask.GCFunction |
								  		  GCValueMask.GCLineWidth |
										  GCValueMask.GCLineStyle |
										  GCValueMask.GCJoinStyle |
										  GCValueMask.GCCapStyle |
										  GCValueMask.GCFillStyle),
								   ref newValues);

					// Draw the effect.
					switch(effect & ~(Effect.ContentColors))
					{
						case Effect.Raised:
						case Effect.Raised | Effect.DefaultButton:
						{
							if((effect & Effect.DefaultButton) != 0)
							{
								if(width >= 2 && height >= 2)
								{
									Xlib.XSetForeground(display, gc, trim);
									Xlib.XDrawRectangle
										(display, drawableHandle, gc,
										 x, y, width - 1, height - 1);
								}
								++x;
								++y;
								width -= 2;
								height -= 2;
							}
							if(width >= 4 && height >= 4)
							{
								Xlib.XSetForeground(display, gc, topShadow);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x, y, x + width - 1, y);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x, y + 1, x, y + height - 2);
								Xlib.XSetForeground
									(display, gc, topShadowEnhance);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + 1, y + 1, x + width - 3, y + 1);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + 1, y + 2, x + 1, y + height - 3);
								Xlib.XSetForeground(display, gc, bottomShadow);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + 1, y + height - 2, x + width - 2,
									y + height - 2);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + width - 2, y + 1, x + width - 2,
									y + height - 3);
								Xlib.XSetForeground
									(display, gc, bottomShadowEnhance);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x, y + height - 1, x + width - 1,
									y + height - 1);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + width - 1, y, x + width - 1,
									y + height - 2);
							}
						}
						break;

						case Effect.Indented:
						{
							if(width >= 4 && height >= 4)
							{
								Xlib.XSetForeground(display, gc, bottomShadow);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x, y, x + width - 2, y);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x, y + 1, x, y + height - 2);
								Xlib.XSetForeground
									(display, gc, bottomShadowEnhance);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + 1, y + 1, x + width - 3, y + 1);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + 1, y + 2, x + 1, y + height - 3);
								Xlib.XSetForeground(display, gc, topShadow);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x, y + height - 1, x + width - 1,
									y + height - 1);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + width - 1, y, x + width - 1,
									y + height - 2);
								Xlib.XSetForeground
									(display, gc, topShadowEnhance);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + 1, y + height - 2, x + width - 2,
									y + height - 2);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + width - 2, y + 1, x + width - 2,
									y + height - 3);
							}
						}
						break;

						case Effect.Indented | Effect.DefaultButton:
						{
							if(width >= 4 && height >= 4)
							{
								Xlib.XSetForeground(display, gc, trim);
								Xlib.XDrawRectangle
									(display, drawableHandle, gc,
									 x, y, width - 1, height - 1);
								Xlib.XSetForeground(display, gc, bottomShadow);
								Xlib.XDrawRectangle
									(display, drawableHandle, gc,
									 x + 1, y + 1, width - 3, height - 3);
							}
						}
						break;

						case Effect.Etched:
						{
							if(width >= 4 && height >= 4)
							{
								Xlib.XSetForeground(display, gc, bottomShadow);
								Xlib.XDrawRectangle(display, drawableHandle, gc,
									x, y, width - 2, height - 2);
								Xlib.XSetForeground(display, gc, topShadow);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + 1, y + 1, x + width - 3, y + 1);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + width - 1, y, x + width - 1,
									y + height - 1);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x, y + height - 1, x + width - 2,
									y + height - 1);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + 1, y + 2, x + 1, y + height - 3);
							}
						}
						break;

						case Effect.Horizontal:
						{
							if(width > 0 && height >= 2)
							{
								y += (height - 2) / 2;
								Xlib.XSetForeground(display, gc, bottomShadow);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x, y, x + width - 1, y);
								Xlib.XSetForeground(display, gc, topShadow);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x, y + 1, x + width - 1, y + 1);
							}
						}
						break;

						case Effect.Vertical:
						{
							if(width >= 2 && height > 0)
							{
								x += (width - 2) / 2;
								Xlib.XSetForeground(display, gc, bottomShadow);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x, y, x, y + height - 1);
								Xlib.XSetForeground(display, gc, topShadow);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + 1, y, x + 1, y + height - 1);
							}
						}
						break;

						case Effect.RadioBlank:
						{
							DrawRadio(display, x, y, width, height,
									  topShadow, topShadowEnhance,
									  bottomShadow, bottomShadowEnhance,
									  topShadow, topShadow);
						}
						break;

						case Effect.RadioSelected:
						{
							DrawRadio(display, x, y, width, height,
									  topShadow, topShadowEnhance,
									  bottomShadow, bottomShadowEnhance,
									  trim, topShadow);
						}
						break;

						case Effect.RadioDisabled:
						{
							DrawRadio(display, x, y, width, height,
									  topShadow, topShadowEnhance,
									  bottomShadow, bottomShadowEnhance,
									  topShadowEnhance, topShadowEnhance);
						}
						break;

						case Effect.RadioSelectedDisabled:
						{
							DrawRadio(display, x, y, width, height,
									  topShadow, topShadowEnhance,
									  bottomShadow, bottomShadowEnhance,
									  bottomShadow, topShadowEnhance);
						}
						break;

						case Effect.CheckBlank:
						{
							// TODO
						}
						break;

						case Effect.CheckSelected:
						{
							// TODO
						}
						break;

						case Effect.CheckDisabled:
						{
							// TODO
						}
						break;

						case Effect.CheckSelectedDisabled:
						{
							// TODO
						}
						break;

						case Effect.MenuSelected:
						{
							// TODO
						}
						break;

						case Effect.MenuSelectedHighlighted:
						{
							// TODO
						}
						break;

						case Effect.MenuSelectedDisabled:
						{
							// TODO
						}
						break;

						case Effect.CaptionButtonRaised:
						{
							if(width >= 4 && height >= 4)
							{
								Xlib.XSetForeground(display, gc, topShadow);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x, y, x + width - 1, y);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x, y + 1, x, y + height - 2);
								Xlib.XSetForeground(display, gc, bottomShadow);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + 1, y + height - 2, x + width - 2,
									y + height - 2);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + width - 2, y + 1, x + width - 2,
									y + height - 3);
								Xlib.XSetForeground
									(display, gc, bottomShadowEnhance);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x, y + height - 1, x + width - 1,
									y + height - 1);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + width - 1, y, x + width - 1,
									y + height - 2);
								Xlib.XSetForeground
									(display, gc, background);
								Xlib.XFillRectangle(display, drawableHandle, gc,
									x + 1, y + 1, width - 3, height - 3);
							}
						}
						break;

						case Effect.CaptionButtonIndented:
						{
							if(width >= 4 && height >= 4)
							{
								Xlib.XSetForeground(display, gc, bottomShadow);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x, y, x + width - 2, y);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x, y + 1, x, y + height - 2);
								Xlib.XSetForeground
									(display, gc, bottomShadowEnhance);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + 1, y + 1, x + width - 3, y + 1);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + 1, y + 2, x + 1, y + height - 3);
								Xlib.XSetForeground(display, gc, topShadow);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x, y + height - 1, x + width - 1,
									y + height - 1);
								Xlib.XDrawLine(display, drawableHandle, gc,
									x + width - 1, y, x + width - 1,
									y + height - 2);
								Xlib.XSetForeground
									(display, gc, background);
								Xlib.XFillRectangle(display, drawableHandle, gc,
									x + 2, y + 2, width - 3, height - 3);
							}
						}
						break;
					}

					// Restore the previous GC settings.
					Xlib.XChangeGC(display, gc,
								   (uint)(GCValueMask.GCFunction |
										  GCValueMask.GCForeground |
										  GCValueMask.GCFillStyle |
								  		  GCValueMask.GCLineWidth |
										  GCValueMask.GCLineStyle |
										  GCValueMask.GCJoinStyle |
										  GCValueMask.GCCapStyle |
										  GCValueMask.GCTileStipXOrigin |
										  GCValueMask.GCTileStipYOrigin),
								   ref values);
				}
				finally
				{
					dpy.Unlock();
				}
			}

	// Put an XImage into this graphics context.
	internal void PutXImage(IntPtr ximage, int srcX, int srcY,
							int destX, int destY, int width, int height)
			{
				try
				{
					IntPtr display = Lock();
					Xlib.XPutImage(display, drawableHandle, gc, ximage,
								   srcX, srcY, destX, destY, width, height);
				}
				finally
				{
					dpy.Unlock();
				}

			}

} // class Graphics

} // namespace Xsharp
