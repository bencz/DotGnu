/*
 * SystemInformation.cs - Implementation of the
 *			"System.Windows.Forms.SystemInformation" class.
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

using System.Drawing;

public class SystemInformation
{
	// This class cannot be instantiated.
	private SystemInformation() {}

	// Get various system properties.  Most of the values are faked,
	// because non-Windows platforms don't have any way to get them.
#if !CONFIG_COMPACT_FORMS
	public static ArrangeDirection ArrangeDirection
			{
				get
				{
					return ArrangeDirection.Left;
				}
			}
	public static ArrangeStartingPosition ArrangeStartingPosition
			{
				get
				{
					return ArrangeStartingPosition.BottomLeft;
				}
			}
	public static BootMode BootMode
			{
				get
				{
					return BootMode.Normal;
				}
			}
	public static Size Border3DSize
			{
				get
				{
					return new Size(2, 2);
				}
			}
	public static Size BorderSize
			{
				get
				{
					return new Size(1, 1);
				}
			}
	public static Size CaptionButtonSize
			{
				get
				{
					return new Size(18, 18);
				}
			}
	public static int CaptionHeight
			{
				get
				{
					return 19;
				}
			}
#if !ECMA_COMPAT
	public static String ComputerName
			{
				get
				{
					return Environment.MachineName;
				}
			}
#endif
	public static Size CursorSize
			{
				get
				{
					return new Size(32, 32);
				}
			}
	public static bool DbcsEnabled
			{
				get
				{
					return false;
				}
			}
	public static bool DebugOS
			{
				get
				{
					return false;
				}
			}
	public static Size DoubleClickSize
			{
				get
				{
					return new Size(4, 4);
				}
			}
	public static bool DrawFullWindows
			{
				get
				{
					return true;
				}
			}
	public static Size DragSize
			{
				get
				{
					return new Size(4, 4);
				}
			}
	public static Size FixedFrameBorderSize
			{
				get
				{
					return new Size(3, 3);
				}
			}
	public static Size FrameBorderSize
			{
				get
				{
					return new Size(4, 4);
				}
			}
	public static bool HighContrast
			{
				get
				{
					return false;
				}
			}
	public static int HorizontalScrollBarArrowWidth
			{
				get
				{
					return 16;
				}
			}
	public static int HorizontalScrollBarHeight
			{
				get
				{
					return 16;
				}
			}
	public static int HorizontalScrollBarThumbWidth
			{
				get
				{
					return 16;
				}
			}
	public static Size IconSize
			{
				get
				{
					return new Size(32, 32);
				}
			}
	public static Size IconSpacingSize
			{
				get
				{
					return new Size(75, 75);
				}
			}
	public static int KanjiWindowHeight
			{
				get
				{
					return 0;
				}
			}
	public static Size MaxWindowTrackSize
			{
				get
				{
					return Screen.PrimaryScreen.Bounds.Size + new Size(12, 12);
				}
			}
	public static Size MenuButtonSize
			{
				get
				{
					return new Size(18, 18);
				}
			}
	public static Size MenuCheckSize
			{
				get
				{
					return new Size(13, 13);
				}
			}
	public static Font MenuFont
			{
				get
				{
					return Control.DefaultFont;
				}
			}
	public static bool MidEastEnabled
			{
				get
				{
					return false;
				}
			}
	public static Size MinimizedWindowSize
			{
				get
				{
					return new Size(160, 24);
				}
			}
	public static Size MinimizedWindowSpacingSize
			{
				get
				{
					return new Size(160, 24);
				}
			}
	public static Size MinimumWindowSize
			{
				get
				{
					return new Size(112, 27);
				}
			}
	public static Size MinWindowTrackSize
			{
				get
				{
					return new Size(112, 27);
				}
			}
	public static int MonitorCount
			{
				get
				{
					return 1;
				}
			}
	public static bool MonitorsSameDisplayFormat
			{
				get
				{
					return true;
				}
			}
	public static int MouseButtons
			{
				get
				{
					return 2;
				}
			}
	public static bool MouseButtonsSwapped
			{
				get
				{
					return false;
				}
			}
	public static bool MousePresent
			{
				get
				{
					return true;
				}
			}
	public static bool MouseWheelPresent
			{
				get
				{
					return true;
				}
			}
	public static int MouseWheelScrollLines
			{
				get
				{
					return 3;
				}
			}
	public static bool NativeMouseWheelSupport
			{
				get
				{
					return true;
				}
			}
	public static bool Network
			{
				get
				{
					return true;
				}
			}
	public static bool PenWindows
			{
				get
				{
					return false;
				}
			}
	public static Size PrimaryMonitorMaximizedWindowSize
			{
				get
				{
					return MaxWindowTrackSize;
				}
			}
	public static Size PrimaryMonitorSize
			{
				get
				{
					return Screen.PrimaryScreen.Bounds.Size;
				}
			}
	public static bool RightAlignedMenus
			{
				get
				{
					return false;
				}
			}
	public static bool Secure
			{
				get
				{
					return true;
				}
			}
	public static bool ShowSounds
			{
				get
				{
					return false;
				}
			}
	public static Size SmallIconSize
			{
				get
				{
					return new Size(16, 16);
				}
			}
	public static Size ToolWindowCaptionButtonSize
			{
				get
				{
					return new Size(12, 15);
				}
			}
	public static int ToolWindowCaptionHeight
			{
				get
				{
					return 16;
				}
			}
#if !ECMA_COMPAT
	public static String UserDomainName
			{
				get
				{
					return Environment.UserDomainName;
				}
			}
	public static bool UserInteractive
			{
				get
				{
					return true;
				}
			}
	public static String UserName
			{
				get
				{
					return Environment.UserName;
				}
			}
#endif
	public static int VerticalScrollBarArrowHeight
			{
				get
				{
					return 16;
				}
			}
	public static int VerticalScrollBarThumbHeight
			{
				get
				{
					return 16;
				}
			}
	public static int VerticalScrollBarWidth
			{
				get
				{
					return 16;
				}
			}
	public static Rectangle VirtualScreen
			{
				get
				{
					return Screen.PrimaryScreen.Bounds;
				}
			}
	public static Rectangle WorkingArea
			{
				get
				{
					return Screen.PrimaryScreen.WorkingArea;
				}
			}
#endif
	public static int DoubleClickTime
			{
				get
				{
					return 900;
				}
			}
	public static int MenuHeight
			{
				get
				{
					return ((int)(Control.DefaultFont.GetHeight())) + 6;
				}
			}

}; // class SystemInformation

}; // namespace System.Windows.Forms
