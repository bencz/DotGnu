/*
 * DrawingToolkit.cs - Implementation of IToolkit for Xsharp.
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

namespace System.Drawing.Toolkit
{

using System;
using System.Globalization;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Toolkit;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Threading;
using System.Reflection;
using Xsharp;
using DotGNU.Images;

public sealed class DrawingToolkit : IToolkit
{
	// Internal state.
	internal Application app;
	internal Widget placeholder;
	private int background;
	private int foreground;
	private int textBackground;
	private int textForeground;
	private FontFamily[] families;

	// Constructor.
	public DrawingToolkit()
			{
				// Create an Xsharp application instance.
				app = new Xsharp.Application(null, null);

				// Register the additional fonts that we required.
				RegisterFonts();

				// Get the placeholder widget for the screen.
				placeholder = app.Display.DefaultScreenOfDisplay.Placeholder;

				// Track changes to RESOURCE_MANAGER on the root window.
				app.Display.DefaultScreenOfDisplay.RootWindow.ResourcesChanged
					+= new EventHandler(ResourcesChanged);

				// And then load the initial state.
				LoadResources();
			}

	// Parse an X resource color.
	private static int ParseColor(String name)
			{
				if(name == null || name[0] != '#')
				{
					return -1;
				}
				long value = 0;
				int posn = 1;
				char ch;
				while(posn < name.Length)
				{
					ch = name[posn++];
					if(ch >= '0' && ch <= '9')
					{
						value = value * 16 + (long)(ch - '0');
					}
					else if(ch >= 'A' && ch <= 'F')
					{
						value = value * 16 + (long)(ch - 'A' + 10);
					}
					else if(ch >= 'a' && ch <= 'f')
					{
						value = value * 16 + (long)(ch - 'a' + 10);
					}
				}
				int red, green, blue;
				if(name.Length <= 7)
				{
					red = ((int)(value >> 16)) & 0xFF;
					green = ((int)(value >> 8)) & 0xFF;
					blue = ((int)value) & 0xFF;
				}
				else
				{
					red = ((int)(value >> 40)) & 0xFF;
					green = ((int)(value >> 24)) & 0xFF;
					blue = ((int)(value >> 8)) & 0xFF;
				}
				return ((red << 16) | (green << 8) | blue);
			}

	// Load the resources that are relevant to system colors.
	private void LoadResources()
			{
				RootWindow root;
				root = app.Display.DefaultScreenOfDisplay.RootWindow;
				background = ParseColor(root.GetResource("*background"));
				foreground = ParseColor(root.GetResource("*foreground"));
				textBackground = ParseColor
					(root.GetResource("*XmText.background"));
				textForeground = ParseColor
					(root.GetResource("*XmText.foreground"));
			}

	// Track a change in resources on the X display server.
	private void ResourcesChanged(Object sender, EventArgs e)
			{
				LoadResources();
				// TODO: raise a repaint on all windows
			}

	// Register extra fonts that we need to imitate Winforms exactly.
	private void RegisterFonts()
			{
				Assembly assembly = Assembly.GetExecutingAssembly();
				String family = Xsharp.Font.DefaultSansSerif;
				try
				{
					// "mss" is 8.25 points in the Windows style,
					// which makes it approximately 12 in the X style.
					Xsharp.Font.RegisterFont
						(family, 120, Xsharp.FontStyle.Normal,
						 assembly, "mssR08.pcf");
					Xsharp.Font.RegisterFont
						(family, 120, Xsharp.FontStyle.Bold,
						 assembly, "mssB08.pcf");
					Xsharp.Font.RegisterFont
						(family, 120, Xsharp.FontStyle.Italic,
						 assembly, "mssI08.pcf");
					Xsharp.Font.RegisterFont
						(family, 120,
						 Xsharp.FontStyle.Bold | Xsharp.FontStyle.Italic,
						 assembly, "mssBI08.pcf");
				}
				catch(Exception)
				{
					// Ignore errors if we couldn't register a font,
					// because we can fall back to normal X fonts.
				}
			}

	// Process events in the event queue.  If "waitForEvent" is true,
	// then wait for the next event and return "false" if "Quit" was
	// seen.  If "waitForEvent" is false, then process events in the
	// queue and return "true".  If "waitForEvent" is false and there
	// are no events in the queue, then return "false".
	public bool ProcessEvents(bool waitForEvent)
			{
				if(waitForEvent)
				{
					return app.WaitForEvent();
				}
				else
				{
					return app.ProcessPendingEvents();
				}
			}

	// Send a quit message to the toolkit, which should cause
	// it to exit from the "Run" method.
	public void Quit()
			{
				app.Display.Quit();
			}

	// Send a wakeup message to a thread's message queue to cause
	// it to return back from "ProcessEvents".
	public void Wakeup(Thread thread)
			{
				app.Display.DefaultScreenOfDisplay.Wakeup();
			}

	[TODO]
	// Get lighter or darker versions of a color.
	private static int Light(int color)
			{
				return -1;
			}

	[TODO]
	private static int LightLight(int color)
			{
				return -1;
			}

	[TODO]
	private static int Dark(int color)
			{
				return -1;
			}

	[TODO]
	private static int DarkDark(int color)
			{
				return -1;
			}

	// Resolve a system color to an RGB value.  Returns -1 if the
	// system does not support the color and a default should be used.
	public int ResolveSystemColor(KnownColor color)
			{
			#if false
				switch(color)
				{
					case KnownColor.Control:
						return background;

					case KnownColor.ControlLight:
						return Light(background);

					case KnownColor.ControlLightLight:
						return LightLight(background);

					case KnownColor.ControlDark:
						return Dark(background);

					case KnownColor.ControlDarkDark:
						return DarkDark(background);

					case KnownColor.ControlText:
						return foreground;

					case KnownColor.Window:
						return textBackground;

					case KnownColor.WindowText:
						return textForeground;
				}
			#endif
				return -1;
			}

	// Create an IToolkitGraphics object from a HDC.
	public IToolkitGraphics CreateFromHdc(IntPtr hdc, IntPtr hdevice)
			{
				// We don't use HDC's in this implementation.
				return null;
			}

	// Create an IToolkitGraphics object from an image.
	public IToolkitGraphics CreateFromImage(IToolkitImage image)
			{
				return new DrawingGraphicsImage(this, image);
			}

	// Create an IToolkitGraphics object from a HWND.
	public IToolkitGraphics CreateFromHwnd(IntPtr hwnd)
			{
				// We don't use HWND's in this implementation.
				return null;
			}

	// Create a solid toolkit brush.
	public IToolkitBrush CreateSolidBrush(System.Drawing.Color color)
			{
				return new DrawingSolidBrush(color);
			}

	// Create a hatched toolkit brush.
	public IToolkitBrush CreateHatchBrush
					(HatchStyle style, System.Drawing.Color foreColor,
					 System.Drawing.Color backColor)
			{
				return new DrawingHatchBrush(style, foreColor, backColor);
			}

	// Create an XOR brush.
	public IToolkitBrush CreateXorBrush(IToolkitBrush innerBrush)
			{
				return new DrawingXorBrush(innerBrush);
			}

	// Create a linear gradient brush.  Returns null if the
	// toolkit does not support linear gradient brushes.
	public IToolkitBrush CreateLinearGradientBrush
				(RectangleF rect, System.Drawing.Color color1,
				 System.Drawing.Color color2,
				 LinearGradientMode mode)
			{
				return null;
			}
	public IToolkitBrush CreateLinearGradientBrush
				(RectangleF rect, System.Drawing.Color color1,
				 System.Drawing.Color color2, float angle,
				 bool isAngleScaleable)
			{
				return null;
			}

	// Create a texture brush.
	public IToolkitBrush CreateTextureBrush
				(TextureBrush properties, IToolkitImage image,
				 RectangleF dstRect, ImageAttributes imageAttr)
			{
				return new DrawingTextureBrush
					(properties, image as DrawingImage, dstRect, imageAttr);
			}

	// Create a toolkit pen from the properties in the specified object.
	// If the toolkit does not support the precise combination of pen
	// properties, it will return the closest matching pen.
	public IToolkitPen CreatePen(Pen pen)
			{
				return new DrawingPen(pen);
			}

	// Create a toolkit font from the properties in the specified object.
	public IToolkitFont CreateFont(System.Drawing.Font font, float dpi)
			{
				return new DrawingFont(font, dpi);
			}

	// Create the default system font on this platform.
	public System.Drawing.Font CreateDefaultFont()
			{
				return new System.Drawing.Font
					(new FontFamily(GenericFontFamilies.SansSerif), 8.25f);
			#if false
				// X applications typically use a 12pt font by default.
				return new System.Drawing.Font
					(new FontFamily(GenericFontFamilies.SansSerif), 12.0f);
			#endif
			}

	// Get the handle for the halftone palette.  IntPtr.Zero if not supported.
	public IntPtr GetHalftonePalette()
			{
				return IntPtr.Zero;
			}

	// Validate a window size to ensure that it is in-range for X.
	internal static void ValidateWindowSize(ref int width, ref int height)
			{
				if(width < 1)
				{
					width = 1;
				}
				else if(width > 32767)
				{
					width = 32767;
				}
				if(height < 1)
				{
					height = 1;
				}
				else if(height > 32767)
				{
					height = 32767;
				}
			}

	// Validate a window position to ensure that it is in-range for X.
	internal static void ValidateWindowPosition(ref int x, ref int y)
			{
				if(x < -32767)
				{
					x = -32767;
				}
				else if(x > 32767)
				{
					x = 32767;
				}
				if(y < -32767)
				{
					y = -32767;
				}
				else if(y > 32767)
				{
					y = 32767;
				}
			}

	// Quirk in X, XClearArea, doesnt support a negative position.
	internal static void ValidateWindowPositionPaint(ref int x, ref int y)
			{
				if(x < -32767)
				{
					x = 0;
				}
				else if(x > 32767)
				{
					x = 32767;
				}
				if(y < -32767)
				{
					y = 0;
				}
				else if(y > 32767)
				{
					y = 32767;
				}
			}

	// Create a top-level application window.
	public IToolkitTopLevelWindow CreateTopLevelWindow
				(int width, int height, IToolkitEventSink sink)
			{
				ValidateWindowSize(ref width, ref height);
				return new DrawingTopLevelWindow
					(this, String.Empty, width, height, sink);
			}

	// Create a top-level dialog shell.
	public IToolkitWindow CreateTopLevelDialog
				(int width, int height, bool modal, bool resizable,
				 IToolkitWindow dialogParent, IToolkitEventSink sink)
			{
				DrawingTopLevelWindow window;
				ValidateWindowSize(ref width, ref height);
				window = new DrawingTopLevelWindow
					(this, String.Empty, width, height, sink);
				if(dialogParent is TopLevelWindow)
				{
					window.TransientFor = (TopLevelWindow)dialogParent;
				}
				if(modal)
				{
					window.InputType = MotifInputType.ApplicationModal;
				}
				else
				{
					window.InputType = MotifInputType.Modeless;
				}
				if(!resizable)
				{
					window.Decorations = MotifDecorations.Border |
										 MotifDecorations.Title |
										 MotifDecorations.Menu;
					window.Functions = MotifFunctions.Move |
									   MotifFunctions.Close;
				}
				return window;
			}

	// Create a top-level popup window.  Popup windows do not have
	// any borders and grab the mouse and keyboard when they are mapped
	// to the screen.  They are used for menus, drop-down lists, etc.
	public IToolkitWindow CreatePopupWindow
				(int x, int y, int width, int height, IToolkitEventSink sink)
			{
				return new DrawingPopupWindow(this, x, y, width, height, sink);
			}

	// Create a child window.  If "parent" is null, then the child
	// does not yet have a "real" parent - it will be reparented later.
	public IToolkitWindow CreateChildWindow
				(IToolkitWindow parent, int x, int y, int width, int height, 
				 IToolkitEventSink sink)
			{
				Widget wparent;
				if(parent is Widget)
				{
					wparent = ((Widget)parent);
				}
				else
				{
					wparent = placeholder;
				}
				ValidateWindowPosition(ref x, ref y);
				ValidateWindowSize(ref width, ref height);
				return new DrawingWindow(this, wparent, x, y, width, height, sink);
			}

	// Create an MDI client area.
	public IToolkitMdiClient CreateMdiClient
				(IToolkitWindow parent, int x, int y, int width, int height,
				 IToolkitEventSink sink)
			{
				Widget wparent;
				if(parent is Widget)
				{
					wparent = ((Widget)parent);
				}
				else
				{
					wparent = placeholder;
				}
				ValidateWindowPosition(ref x, ref y);
				ValidateWindowSize(ref width, ref height);
				return new DrawingMdiClient
					(this, wparent, x, y, width, height, sink);
			}

	// Map a System.Drawing color into an Xsharp color.
	public static Xsharp.Color DrawingToXColor(System.Drawing.Color color)
			{
				int argb = color.ToArgb();
				return new Xsharp.Color((argb >> 16) & 0xFF,
										(argb >> 8) & 0xFF, argb & 0xFF);
			}

	// Utility class for comparing font families.
	private class FamilyComparer : IComparer
	{
		// Implement the IComparer interface.
		public int Compare(Object x, Object y)
				{
					return String.Compare(x.ToString(), y.ToString(), true);
				}

	}; // class FamilyComparer

	// Get a list of all font families on this system, or all font
	// families that are compatible with a particular IToolkitGraphics.
	public FontFamily[] GetFontFamilies(IToolkitGraphics graphics)
			{
				lock(this)
				{
					if(families != null)
					{
						return families;
					}
					String[] names = app.Display.FontFamilies;
					int size = names.Length;
					if(Array.IndexOf(names, "microsoft sans serif") == -1)
					{
						++size;
					}
					if(Array.IndexOf(names, "arial") == -1)
					{
						++size;
					}
					if(Array.IndexOf(names, "times new roman") == -1)
					{
						++size;
					}
					if(Array.IndexOf(names, "courier new") == -1)
					{
						++size;
					}
					families = new FontFamily [size];
					size = 0;
					foreach(String name in names)
					{
						families[size++] = new FontFamily
							(CultureInfo.CurrentCulture.TextInfo.
								ToTitleCase(name));
					}
					if(Array.IndexOf(names, "microsoft sans serif") == -1)
					{
						families[size++] = new FontFamily
							("Microsoft Sans Serif");
					}
					if(Array.IndexOf(names, "arial") == -1)
					{
						families[size++] = new FontFamily("Arial");
					}
					if(Array.IndexOf(names, "times new roman") == -1)
					{
						families[size++] = new FontFamily("Times New Roman");
					}
					if(Array.IndexOf(names, "courier new") == -1)
					{
						families[size++] = new FontFamily("Courier New");
					}
					Array.Sort(families, new FamilyComparer());
					return families;
				}
			}

	// Get font family metric information.
	public void GetFontFamilyMetrics(GenericFontFamilies genericFamily,
							  		 String name,
									 System.Drawing.FontStyle style,
							  		 out int ascent, out int descent,
							  		 out int emHeight, out int lineSpacing)
			{
				// X doesn't have family metric information, so return
				// dummy information based on the generic font family.
				switch(genericFamily)
				{
					case GenericFontFamilies.SansSerif:
					default:
					{
						// Metrics for "Arial".
						ascent = 1854;
						descent = 434;
						emHeight = 2048;
						lineSpacing = 2355;
					}
					break;

					case GenericFontFamilies.Serif:
					{
						// Metrics for "Times New Roman".
						ascent = 1825;
						descent = 443;
						emHeight = 2048;
						lineSpacing = 2355;
					}
					break;

					case GenericFontFamilies.Monospace:
					{
						// Metrics for "Courier New".
						ascent = 1705;
						descent = 615;
						emHeight = 2048;
						lineSpacing = 2320;
					}
					break;
				}
			}

	// Get the IToolkitFont that corresponds to a hdc's current font.
	// Returns null if there is no way to obtain the information.
	public IToolkitFont GetFontFromHdc(IntPtr hdc)
			{
				return null;
			}

	// Get the IToolkitFont that corresponds to a native font object.
	// Returns null if there is no way to obtain the information.
	public IToolkitFont GetFontFromHfont(IntPtr hfont)
			{
				return null;
			}

	// Get the IToolkitFont that corresponds to LOGFONT information.
	// Returns null if there is no way to obtain the information.
	public IToolkitFont GetFontFromLogFont(Object lf, IntPtr hdc)
			{
				return null;
			}

	// Get the default IToolkitGraphics object to measure screen sizes.
	public IToolkitGraphics GetDefaultGraphics()
			{
				return new DrawingGraphics
					(this, new Xsharp.Graphics(placeholder));
			}

	// Get the screen size, in pixels.
	public Size GetScreenSize()
			{
				Screen screen = app.Display.DefaultScreenOfDisplay;
				return new Size(screen.Width, screen.Height);
			}

	// Get the working area of the screen, excluding task bars, etc.
	public System.Drawing.Rectangle GetWorkingArea()
			{
				// There's no straight-forward way to get the working
				// area under X, so we return the full screen size.
				return new System.Drawing.Rectangle
						(new System.Drawing.Point(0, 0), GetScreenSize());
			}

	// Get the adjustment values for a top-level window, to convert
	// between window bounds and client bounds.  Each value should
	// be >= 0 and indicate the number of pixels to subtract from the
	// windows bounds to get the client bounds.
	public void GetWindowAdjust(out int leftAdjust, out int topAdjust,
						        out int rightAdjust, out int bottomAdjust,
								ToolkitWindowFlags flags)
			{
				// X adds borders separately in the window manager.
				// They are not counted in the size of the app window.
				// There is no portable way to get the window borders before
				// a window is created. These are representive values.
				leftAdjust = 2;
				topAdjust = 19 + 2;
				rightAdjust = 2;
				bottomAdjust = 2;
			}

	// Register a timer that should fire every "interval" milliseconds.
	// Returns a cookie that can be used to identify the timer.
	public Object RegisterTimer
				(Object owner, int interval, EventHandler expire)
			{
				return new Xsharp.Timer
					(app.Display, expire, owner, interval, interval);
			}

	// Unregister a timer.
	public void UnregisterTimer(Object cookie)
			{
				((Xsharp.Timer)cookie).Dispose();
			}

	// Convert a client point for a window into a screen point.
	public System.Drawing.Point ClientToScreen
				(IToolkitWindow window, System.Drawing.Point point)
			{
				Widget widget = (window as Widget);
				if(widget != null)
				{
					Xsharp.Point pt = widget.WidgetToScreen(point.X, point.Y);
					return new System.Drawing.Point(pt.x, pt.y);
				}
				else
				{
					return point;
				}
			}

	// Convert a screen point for a window into a client point.
	public System.Drawing.Point ScreenToClient
				(IToolkitWindow window, System.Drawing.Point point)
			{
				Widget widget = (window as Widget);
				if(widget != null)
				{
					Xsharp.Point pt = widget.ScreenToWidget(point.X, point.Y);
					return new System.Drawing.Point(pt.x, pt.y);
				}
				else
				{
					return point;
				}
			}

	public IToolkitImage CreateImage(DotGNU.Images.Image image, int frame)
			{
				return new DrawingImage
					(app.Display.DefaultScreenOfDisplay, image, frame);
			}

	[TODO]
	// Get the clipboard handler for this toolkit, or null if no clipboard.
	public IToolkitClipboard GetClipboard()
			{
				return null;
			}

	// Create a new window buffer.
	public IToolkitWindowBuffer CreateWindowBuffer(IToolkitWindow window)
			{
				return new DrawingWindowBuffer(window);
			}

}; // class DrawingToolkit

}; // namespace System.Drawing.Toolkit
