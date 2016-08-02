/*
 * Screen.cs - Implementation of the
 *			"System.Windows.Forms.Screen" class.
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

namespace System.Windows.Forms
{

#if !CONFIG_COMPACT_FORMS

using System.Drawing;
using System.Drawing.Toolkit;

public class Screen
{
	// Internal state.  We only have one (virtual) screen.
	private static Screen[] allScreens = {new Screen()};

	// Constructor.
	private Screen() {}

	// Get an array of all screens on the system.
	public static Screen[] AllScreens
			{
				get
				{
					return allScreens;
				}
			}

	// Get the bounds for this screen.
	public Rectangle Bounds
			{
				get
				{
					return new Rectangle
						(new Point(0, 0),
						 ToolkitManager.Toolkit.GetScreenSize());
				}
			}

	// Get the device name for this screen.
	public String DeviceName
			{
				get
				{
					// We use the name of the toolkit as the device name.
					return ToolkitManager.Toolkit.GetType().ToString();
				}
			}

	// Determine if this is the primary screen.
	public bool Primary
			{
				get
				{
					// We only have one screen in this implementation.
					return true;
				}
			}

	// Get the primary screen.
	public static Screen PrimaryScreen
			{
				get
				{
					return allScreens[0];
				}
			}

	// Get the working area for the screen.
	public Rectangle WorkingArea
			{
				get
				{
					return ToolkitManager.Toolkit.GetWorkingArea();
				}
			}

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				// We only have one screen, so the test is easy.
				return (obj == this);
			}

	// Get the screen that contains a particular control.
	public static Screen FromControl(Control control)
			{
				return PrimaryScreen;
			}

	// Get the screen that contains a particular window.
	public static Screen FromHandle(IntPtr hwnd)
			{
				return PrimaryScreen;
			}

	// Get the screen that contains a particular point.
	public static Screen FromPoint(Point point)
			{
				return PrimaryScreen;
			}

	// Get the screen that contains a particular rectangle.
	public static Screen FromRectangle(Rectangle rect)
			{
				return PrimaryScreen;
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return base.GetHashCode();
			}

	// Get the working area for the screen containing a control.
	public static Rectangle GetWorkingArea(Control control)
			{
				return FromControl(control).WorkingArea;
			}

	// Get the working area for the screen containing a point.
	public static Rectangle GetWorkingArea(Point pt)
			{
				return FromPoint(pt).WorkingArea;
			}

	// Get the working area for the screen containing a rectangle.
	public static Rectangle GetWorkingArea(Rectangle rect)
			{
				return FromRectangle(rect).WorkingArea;
			}
	
	public static Rectangle GetBounds(Control ctl)
			{
				return GetBounds(ctl.Bounds);	
			}

	public static Rectangle GetBounds(Point pt)
			{
				Rectangle r = PrimaryScreen.Bounds;
				if(r.Contains(pt))
				{
					return r;
				}
				return new Rectangle(pt.X, pt.Y, 0, 0);
			}

	public static Rectangle GetBounds(Rectangle rect)
			{
				Rectangle r = PrimaryScreen.Bounds;
				if(r.Contains(rect))
				{
					return(r);
				}
				return rect;
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return String.Format
					("Screen [Bounds={0} WorkingArea={1} " +
					 "Primary={2} DeviceName={3}]",
					 Bounds, WorkingArea, Primary, DeviceName);
			}

}; // class Screen

#endif // !CONFIG_COMPACT_FORMS

}; // namespace System.Windows.Forms
