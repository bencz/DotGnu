/*
 * NullToolkit.cs - Implementation of a null toolkit.
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

using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Threading;
using DotGNU.Images;

// This class is used to stub out toolkit support when a toolkit
// cannot be found, and to also act as a base class for printer
// drivers which don't need windows and event loop functionality.

[NonStandardExtra]
public class NullToolkit : IToolkit
{
	// Constructor.
	public NullToolkit() {}

	// Process events in the event queue.  If "waitForEvent" is true,
	// then wait for the next event and return "false" if "Quit" was
	// seen.  If "waitForEvent" is false, then process events in the
	// queue and return "true".  If "waitForEvent" is false and there
	// are no events in the queue, then return "false".
	public virtual bool ProcessEvents(bool waitForEvent)
			{
				return false;
			}

	// Send a quit message to the toolkit, which should cause
	// it to exit from the "Run" method.
	public virtual void Quit() {}

	// Send a wakeup message to a thread's message queue to cause
	// it to return back from "ProcessEvents".
	public virtual void Wakeup(Thread thread) {}

	// Resolve a system color to an RGB value.  Returns -1 if the
	// system does not support the color and a default should be used.
	public virtual int ResolveSystemColor(KnownColor color)
			{
				return -1;
			}

	// Create an IToolkitGraphics object from a HDC.
	public virtual IToolkitGraphics CreateFromHdc(IntPtr hdc, IntPtr hdevice)
			{
				return null;
			}

	// Create an IToolkitGraphics object from a HWND.
	public virtual IToolkitGraphics CreateFromHwnd(IntPtr hwnd)
			{
				return null;
			}

	// Create an IToolkitGraphics object from an image.
	public virtual IToolkitGraphics CreateFromImage(IToolkitImage image)
			{
				return null;
			}

	// Create a solid toolkit brush.
	public virtual IToolkitBrush CreateSolidBrush(Color color)
			{
				return null;
			}

	// Create a hatched toolkit brush.
	public virtual IToolkitBrush CreateHatchBrush
					(HatchStyle style, Color foreColor, Color backColor)
			{
				return null;
			}

	// Create an XOR brush.
	public virtual IToolkitBrush CreateXorBrush(IToolkitBrush innerBrush)
			{
				return null;
			}

	// Create a linear gradient brush.  Returns null if the
	// toolkit does not support linear gradient brushes.
	public virtual IToolkitBrush CreateLinearGradientBrush
				(RectangleF rect, Color color1, Color color2,
				 LinearGradientMode mode)
			{
				return null;
			}
	public virtual IToolkitBrush CreateLinearGradientBrush
				(RectangleF rect, Color color1, Color color2,
				 float angle, bool isAngleScaleable)
			{
				return null;
			}

	// Create a texture brush.
	public virtual IToolkitBrush CreateTextureBrush
				(TextureBrush properties, IToolkitImage image,
				 RectangleF dstRect, ImageAttributes imageAttr)
			{
				return null;
			}

	// Create a toolkit pen from the properties in the specified object.
	// If the toolkit does not support the precise combination of pen
	// properties, it will return the closest matching pen.
	public virtual IToolkitPen CreatePen(Pen pen)
			{
				return null;
			}

	// Create a toolkit font from the properties in the specified object.
	public virtual IToolkitFont CreateFont(Font font, float dpi)
			{
				return null;
			}

	// Create the default system font on this platform.
	public virtual Font CreateDefaultFont()
			{
				return new Font(new FontFamily
					(GenericFontFamilies.SansSerif), 9.0f);
			}

	// Get the handle for the halftone palette.  IntPtr.Zero if not supported.
	public virtual IntPtr GetHalftonePalette()
			{
				return IntPtr.Zero;
			}

	// Create a top-level application window.
	public virtual IToolkitTopLevelWindow CreateTopLevelWindow
				(int width, int height, IToolkitEventSink sink)
			{
				return null;
			}

	// Create a top-level dialog shell.
	public virtual IToolkitWindow CreateTopLevelDialog
				(int width, int height, bool modal, bool resizable,
				 IToolkitWindow dialogParent, IToolkitEventSink sink)
			{
				return null;
			}

	// Create a top-level popup window.  Popup windows do not have
	// any borders and grab the mouse and keyboard when they are mapped
	// to the screen.  They are used for menus, drop-down lists, etc.
	public virtual IToolkitWindow CreatePopupWindow
				(int x, int y, int width, int height, IToolkitEventSink sink)
			{
				return null;
			}

	// Create a child window.  If "parent" is null, then the child
	// does not yet have a "real" parent - it will be reparented later.
	public virtual IToolkitWindow CreateChildWindow
				(IToolkitWindow parent, int x, int y, int width, int height,
				 IToolkitEventSink sink)
			{
				return null;
			}

	// Create an MDI client area.
	public virtual IToolkitMdiClient CreateMdiClient
				(IToolkitWindow parent, int x, int y, int width, int height,
				 IToolkitEventSink sink)
			{
				return null;
			}

	// Get a list of all font families on this system, or all font
	// families that are compatible with a particular IToolkitGraphics.
	public virtual FontFamily[] GetFontFamilies(IToolkitGraphics graphics)
			{
				return null;
			}

	// Get font family metric information.
	public virtual void GetFontFamilyMetrics
				(GenericFontFamilies genericFamily,
				 String name, FontStyle style,
				 out int ascent, out int descent,
				 out int emHeight, out int lineSpacing)
			{
				ascent = 0;
				descent = 0;
				emHeight = 0;
				lineSpacing = 0;
			}

	// Get the IToolkitFont that corresponds to a hdc's current font.
	// Returns null if there is no way to obtain the information.
	public virtual IToolkitFont GetFontFromHdc(IntPtr hdc)
			{
				return null;
			}

	// Get the IToolkitFont that corresponds to a native font object.
	// Returns null if there is no way to obtain the information.
	public virtual IToolkitFont GetFontFromHfont(IntPtr hfont)
			{
				return null;
			}

	// Get the IToolkitFont that corresponds to LOGFONT information.
	// Returns null if there is no way to obtain the information.
	public virtual IToolkitFont GetFontFromLogFont(Object lf, IntPtr hdc)
			{
				return null;
			}

	// Get the default IToolkitGraphics object to measure screen sizes.
	public virtual IToolkitGraphics GetDefaultGraphics()
			{
				return null;
			}

	// Get the screen size, in pixels.
	public virtual Size GetScreenSize()
			{
				return new Size(0, 0);
			}

	// Get the working area of the screen, excluding task bars, etc.
	public virtual Rectangle GetWorkingArea()
			{
				return new Rectangle(0, 0, 0, 0);
			}

	// Get the adjustment values for a top-level window, to convert
	// between window bounds and client bounds.  Each value should
	// be >= 0 and indicate the number of pixels to subtract from the
	// windows bounds to get the client bounds.
	public virtual void GetWindowAdjust
				(out int leftAdjust, out int topAdjust,
				 out int rightAdjust, out int bottomAdjust,
				 ToolkitWindowFlags flags)
			{
				leftAdjust = 0;
				topAdjust = 0;
				rightAdjust = 0;
				bottomAdjust = 0;
			}

	// Register a timer that should fire every "interval" milliseconds.
	// Returns a cookie that can be used to identify the timer.
	public virtual Object RegisterTimer
				(Object owner, int interval, EventHandler expire)
			{
				return null;
			}

	// Unregister a timer.
	public virtual void UnregisterTimer(Object cookie) {}

	// Convert a client point for a window into a screen point.
	public virtual Point ClientToScreen(IToolkitWindow window, Point point)
			{
				return point;
			}

	// Convert a screen point for a window into a client point.
	public virtual Point ScreenToClient(IToolkitWindow window, Point point)
			{
				return point;
			}

	public virtual IToolkitImage CreateImage(DotGNU.Images.Image image, int frame )
			{
				return null;
			}

	// Get the clipboard handler for this toolkit, or null if no clipboard.
	public virtual IToolkitClipboard GetClipboard()
			{
				return null;
			}

	// Create window buffer for use in double buffering window paints.
	public IToolkitWindowBuffer CreateWindowBuffer(IToolkitWindow window)
			{
				return null;
			}

}; // class NullToolkit

}; // namespace System.Drawing.Toolkit
