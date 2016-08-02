/*
 * Win32.cs - Api class for all native constants, structures and calls.
 * Copyright (C) 2003  Neil Cawse.
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

namespace System.Drawing.Win32
{

using System;
using System.Runtime.InteropServices;

internal class Api
{
	// Utility function for swapping colors - BGR to RGB and vice versa
	public static int SwapRGB(int color)
	{
		return (color & 0xFF) << 16 | color & 0xFF00 | color >> 16 & 0xFF;
	}

	public delegate int WNDPROC( IntPtr hwnd, int msg, int wParam, int lParam);

	public enum WindowClassStyle :uint
	{
		CS_VREDRAW = 0x0001,
		CS_HREDRAW = 0x0002,
		CS_DBLCLKS = 0x0008,
		CS_OWNDC = 0x0020,
		CS_CLASSDC = 0x0040,
		CS_PARENTDC = 0x0080,
		CS_NOCLOSE = 0x0200,
		CS_SAVEBITS = 0x0800,
		CS_BYTEALIGNCLIENT = 0x1000,
		CS_BYTEALIGNWINDOW = 0x2000,
		CS_GLOBALCLASS = 0x4000,
		CS_IME = 0x00010000,
		CS_DROPSHADOW = 0x00020000
	}

	public enum WindowsMessages : int 
		{
			WM_NULL = 0x0,
			WM_CREATE = 0x1,
			WM_DESTROY = 0x2,
			WM_ACTIVATE = 0x6,
			WM_SETFOCUS = 0x7,
			WM_KILLFOCUS = 0x8,
			WM_PAINT = 0xF,
			WM_CLOSE = 0x10,
			WM_QUIT = 0x12,
			WM_ERASEBKGND = 0x14,
			WM_SETTINGCHANGE = 0x1A,
			WM_SETCURSOR = 0x20,
			WM_MOUSEACTIVATE = 0x21,
			WM_WINDOWPOSCHANGING = 0x46,
			WM_SETICON = 0x80,
			WM_NCCALCSIZE = 0x83,
			WM_NCHITTEST = 0x84,
			WM_NCPAINT = 0x0085,
			WM_NCACTIVATE = 0x86,
			WM_KEYDOWN = 0x0100,
			WM_KEYUP = 0x0101,
			WM_CHAR = 0x0102,
			WM_SYSKEYDOWN = 0x0104,
			WM_SYSKEYUP = 0x0105,
			WM_SYSCHAR = 0x0106,
			WM_SYSCOMMAND = 0x112,
			WM_TIMER = 0x0113,
			WM_MOUSEMOVE = 0x0200,
			WM_LBUTTONDOWN = 0x0201,
			WM_LBUTTONUP = 0x0202,
			WM_LBUTTONDBLCLK = 0x0203,
			WM_RBUTTONDOWN = 0x0204,
			WM_RBUTTONUP = 0x0205,
			WM_RBUTTONDBLCLK = 0x0206,
			WM_MBUTTONDOWN = 0x0207,
			WM_MBUTTONUP = 0x0208,
			WM_MBUTTONDBLCLK = 0x0209,
			WM_MOUSEWHEEL = 0x020A,
			WM_XBUTTONDOWN = 0x020B,
			WM_XBUTTONUP = 0x020C,
			WM_XBUTTONDBLCLK = 0x020D,
			WM_SIZING = 0x0214,
			WM_MOVING = 0x0216,
			WM_MOUSELEAVE = 0x02A3

		}

	public enum ActivateState
	{
		WA_INACTIVE = 0,
		WA_ACTIVE = 1,
		WA_CLICKACTIVE =2
	}

	public static IntPtr HWND_BROADCAST = new IntPtr(0xffff);

	public enum MouseKeyState : ushort
	{
		MK_LBUTTON = 0x1,
		MK_RBUTTON = 0x2,
		MK_SHIFT = 0x4,
		MK_CONTROL = 0x8,
		MK_MBUTTON = 0x10,
		MK_XBUTTON1 = 0x20,
		MK_XBUTTON2 = 0x40
	}

	//Window Styles used when creating a window
	public enum WindowStyle : uint 
	{
		WS_OVERLAPPED = 0x00000000,
		WS_POPUP = 0x80000000,
		WS_CHILD = 0x40000000,
		WS_MINIMIZE = 0x20000000,
		WS_VISIBLE = 0x10000000,
		WS_DISABLED = 0x08000000,
		WS_CLIPSIBLINGS = 0x04000000,
		WS_CLIPCHILDREN = 0x02000000,
		WS_MAXIMIZE = 0x01000000,
		WS_CAPTION = 0x00C00000,    /* WS_BORDER | WS_DLGFRAME  */
		WS_BORDER = 0x00800000,
		WS_DLGFRAME = 0x00400000,
		WS_VSCROLL = 0x00200000,
		WS_HSCROLL = 0x00100000,
		WS_SYSMENU = 0x00080000,
		WS_THICKFRAME = 0x00040000,
		WS_GROUP = 0x00020000,
		WS_TABSTOP = 0x00010000,
		WS_MINIMIZEBOX = 0x00020000,
		WS_MAXIMIZEBOX = 0x00010000,
		WS_TILED = WS_OVERLAPPED,
		WS_ICONIC = WS_MINIMIZE,
		WS_SIZEBOX = WS_THICKFRAME,
		WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
		WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
		WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
		WS_CHILDWINDOW = WS_CHILD
	}

	public enum WindowsExtendedStyle : uint
	{
		WS_EX_DLGMODALFRAME = 0x00000001,
		WS_EX_NOPARENTNOTIFY = 0x00000004,
		WS_EX_TOPMOST = 0x00000008,
		WS_EX_ACCEPTFILES = 0x00000010,
		WS_EX_TRANSPARENT = 0x00000020,
		WS_EX_MDICHILD = 0x00000040,
		WS_EX_TOOLWINDOW = 0x00000080,
		WS_EX_WINDOWEDGE = 0x00000100,
		WS_EX_CLIENTEDGE = 0x00000200,
		WS_EX_CONTEXTHELP = 0x00000400,
		WS_EX_RIGHT = 0x00001000,
		WS_EX_LEFT = 0x00000000,
		WS_EX_RTLREADING = 0x00002000,
		WS_EX_LTRREADING = 0x00000000,
		WS_EX_LEFTSCROLLBAR = 0x00004000,
		WS_EX_RIGHTSCROLLBAR = 0x00000000,
		WS_EX_CONTROLPARENT = 0x00010000,
		WS_EX_STATICEDGE = 0x00020000,
		WS_EX_APPWINDOW = 0x00040000,
		WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
		WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST)
	}

	//For ShowWindow API
	public enum SetWindowsPosFlags : uint 
	{
		SWP_NOSIZE = 0x1,
		SWP_NOMOVE = 0x2,
		SWP_NOZORDER = 0x4,
		SWP_NOREDRAW = 0x8,
		SWP_NOACTIVATE = 0x10,
		SWP_SHOWWINDOW = 0x40,
		SWP_HIDEWINDOW = 0x80,
		SWP_NOSENDCHANGING = 0x400
	}

	public enum SetWindowsPosPosition : int
	{
		HWND_TOP =0,
		HWND_BOTTOM = 1,
		HWND_TOPMOST = -1,
		HWND_NOTOPMOST = -2
	}

	public enum SystemMetricsType 
	{
		SM_CXSCREEN = 0,
		SM_CYSCREEN = 1,
		SM_CYCAPTION = 4,
		SM_CXBORDER = 5,
		SM_CYBORDER = 6,
		SM_CXFIXEDFRAME = 7,
		SM_CYFIXEDFRAME = 8,
		SM_CXSIZE = 30,
		SM_CYSIZE = 31,
		SM_CXFRAME = 32,
		SM_CYFRAME = 33,
		SM_CXEDGE = 45,
		SM_CYEDGE = 46,
		SM_CXSMICON = 49,
		SM_CYSMICON = 50,
		SM_CXSMSIZE = 52,
		SM_CYSMSIZE = 53
	}
	
	public enum SystemParametersAction : uint 
	{
		SPI_GETBORDER = 0x5,
		SPI_GETNONCLIENTMETRICS=0x29,
		SPI_GETWORKAREA=0x30,
		SPI_GETGRADIENTCAPTIONS=0x1008
	}

	public const int CW_USEDEFAULT = unchecked((int)0x80000000);

	public enum ShowWindowCommand 
	{
		SW_HIDE = 0,
		SW_SHOWNORMAL = 1,
		SW_MAXIMIZE = 3,
		SW_SHOW = 5,
		SW_MINIMIZE = 6,
		SW_SHOWNA = 8, //Visible but no activate
		SW_RESTORE = 9
	}

	public enum SystemCommand
	{
		SC_MINIMIZE = 0xF020,
		SC_MAXIMIZE = 0xF030,
		SC_CLOSE = 0xF060,
		SC_RESTORE = 0xF120
	}

	public enum StockObjectType
	{
		WHITE_BRUSH = 0,
		LTGRAY_BRUSH = 1,
		GRAY_BRUSH = 2,
		DKGRAY_BRUSH = 3,
		BLACK_BRUSH = 4,
		HOLLOW_BRUSH = 5, //also NULL_BRUSH
		WHITE_PEN = 6,
		BLACK_PEN = 7,
		NULL_PEN = 8,
		OEM_FIXED_FONT = 10,
		ANSI_FIXED_FONT = 11,
		ANSI_VAR_FONT = 12,
		SYSTEM_FONT = 13,
		DEVICE_DEFAULT_FONT = 14,
		DEFAULT_PALETTE = 15,
		SYSTEM_FIXED_FONT = 16
	}

	public enum LogBrushStyles
	{
		BS_SOLID = 0,
		BS_HOLLOW = 1,
		BS_HATCHED = 2,
		BS_PATTERN = 3,
		BS_INDEXED = 4,
		BS_DIBPATTERN = 5,
		BS_DIBPATTERNPT = 6,
		BS_PATTERN8X8 = 7,
		BS_DIBPATTERN8X8 = 8,
		BS_MONOPATTERN = 9
	}

	// Color types - winuser.h
	private enum WinUserColor : int
	{
		COLOR_SCROLLBAR = 0,
		COLOR_BACKGROUND = 1,
		COLOR_ACTIVECAPTION = 2,
		COLOR_INACTIVECAPTION = 3,
		COLOR_MENU = 4,
		COLOR_WINDOW = 5,
		COLOR_WINDOWFRAME = 6,
		COLOR_MENUTEXT = 7,
		COLOR_WINDOWTEXT = 8,
		COLOR_CAPTIONTEXT = 9,
		COLOR_ACTIVEBORDER = 10,
		COLOR_INACTIVEBORDER = 11,
		COLOR_APPWORKSPACE = 12,
		COLOR_HIGHLIGHT = 13,
		COLOR_HIGHLIGHTTEXT = 14,
		COLOR_BTNFACE = 15,
		COLOR_BTNSHADOW = 16,
		COLOR_GRAYTEXT = 17,
		COLOR_BTNTEXT = 18,
		COLOR_INACTIVECAPTIONTEXT = 19,
		COLOR_BTNHIGHLIGHT = 20,
		COLOR_3DDKSHADOW = 21,
		COLOR_3DLIGHT = 22,
		COLOR_INFOTEXT = 23,
		COLOR_INFOBK = 24,
		COLOR_HOTLIGHT = 26,
		COLOR_GRADIENTACTIVECAPTION = 27,
		COLOR_GRADIENTINACTIVECAPTION = 28,
		COLOR_MENUHILIGHT = 29,
		COLOR_MENUBAR = 30
	}

	// Utility array for mapping KnownColor to WinUserColor
	public static readonly int[] KnownColorWindowsMap = {
		0x00000000,
		(int)WinUserColor.COLOR_ACTIVEBORDER,			// ActiveBorder
		(int)WinUserColor.COLOR_ACTIVECAPTION,			// ActiveCaption
		(int)WinUserColor.COLOR_CAPTIONTEXT,			// ActiveCaptionText
		(int)WinUserColor.COLOR_APPWORKSPACE,			// AppWorkspace
		(int)WinUserColor.COLOR_BTNFACE,				// Control
		(int)WinUserColor.COLOR_BTNSHADOW,				// ControlDark
		(int)WinUserColor.COLOR_3DDKSHADOW,				// ControlDarkDark
		(int)WinUserColor.COLOR_3DLIGHT,				// ControlLight
		(int)WinUserColor.COLOR_BTNHIGHLIGHT,			// ControlLightLight
		(int)WinUserColor.COLOR_BTNTEXT,				// ControlText
		(int)WinUserColor.COLOR_BACKGROUND,				// Desktop
		(int)WinUserColor.COLOR_GRAYTEXT,				// GrayText
		(int)WinUserColor.COLOR_HIGHLIGHT,				// Highlight
		(int)WinUserColor.COLOR_HIGHLIGHTTEXT,			// HighlightText
		(int)WinUserColor.COLOR_HOTLIGHT,				// HotTrack
		(int)WinUserColor.COLOR_INACTIVEBORDER,			// InactiveBorder
		(int)WinUserColor.COLOR_INACTIVECAPTION,		// InactiveCaption
		(int)WinUserColor.COLOR_INACTIVECAPTIONTEXT,	// InactiveCaptionText
		(int)WinUserColor.COLOR_INFOBK,					// Info
		(int)WinUserColor.COLOR_INFOTEXT,				// InfoText
		(int)WinUserColor.COLOR_MENUBAR,				// Menu
		(int)WinUserColor.COLOR_MENU,					// MenuText
		(int)WinUserColor.COLOR_BTNFACE,				// ScrollBar
		(int)WinUserColor.COLOR_WINDOW,					// Window
		(int)WinUserColor.COLOR_WINDOWFRAME,			// WindowFrame
		(int)WinUserColor.COLOR_WINDOWTEXT				// WindowText
	};

	public enum FontQuality : byte 
	{
		NONANTIALIASED_QUALITY = 3,
		ANTIALIASED_QUALITY = 4,
		CLEARTYPE_QUALITY =5,
		CLEARTYPE_NATURAL_QUALITY = 6
	}

	public enum BackGroundModeType
	{
		TRANSPARENT = 1
	}

	public enum TrackMouseEventFlags : uint
	{
		TME_LEAVE = 2
	}

	public enum CursorName
	{
		IDC_ARROW = 32512,
		IDC_IBEAM = 32513,
		IDC_WAIT = 32514,
		IDC_CROSS = 32515,
		IDC_UPARROW = 32516,
		IDC_SIZENWSE = 32642,
		IDC_SIZENESW = 32643,
		IDC_SIZEWE = 32644,
		IDC_SIZENS = 32645,
		IDC_SIZEALL = 32646
	}

	public enum PeekMessageType : uint
	{
		PM_NOREMOVE = 0x0000,
		PM_REMOVE = 0x0001,
		PM_NOYIELD = 0x0002
	}

	public enum SetWindowLongType
	{
		GWL_STYLE = -16,
		GWL_EXSTYLE = -20
	}

	public enum RedrawWindowFlags
	{
		RDW_INVALIDATE = 0x1,
		RDW_FRAME = 0x400
	}

	public enum VirtualKeyType
	{
		VK_SHIFT = 0x10,
		VK_CONTROL = 0x11,
		VK_MENU = 0x12, //ALT KEY
		VK_LSHIFT = 0xA0,
		VK_RSHIFT = 0xA1,
		VK_LCONTROL = 0xA2,
		VK_RCONTROL = 0xA3,
		VK_LMENU = 0xA4,
		VK_RMENU = 0xA5

	}

	public enum RegionCombineMode
	{
		RGN_AND = 1,
		RGN_OR = 2,
		RGN_XOR = 3,
		RGN_DIFF = 4,
		RGN_COPY = 5,
		RGN_MIN = RGN_AND,
		RGN_MAX = RGN_COPY
	}

	public enum RopType : uint
	{
		SRCCOPY = 0x00CC0020, /* dest = source */
		SRCPAINT = 0x00EE0086, /* dest = source OR dest */
		SRCAND =0x008800C6, /* dest = source AND dest */
		SRCINVERT = 0x00660046, /* dest = source XOR dest */
		SRCERASE =0x00440328, /* dest = source AND (NOT dest ) */
		NOTSRCCOPY =0x00330008, /* dest = (NOT source) */
		NOTSRCERASE =0x001100A6, /* dest = (NOT src) AND (NOT dest) */
		MERGECOPY =0x00C000CA, /* dest = (source AND pattern) */
		MERGEPAINT =0x00BB0226, /* dest = (NOT source) OR dest */
		PATCOPY =0x00F00021, /* dest = pattern */
		PATPAINT =0x00FB0A09, /* dest = DPSnoo */
		PATINVERT =0x005A0049, /* dest = pattern XOR dest */
		DSTINVERT =0x00550009 ,/* dest = (NOT dest) */
		BLACKNESS =0x00000042, /* dest = BLACK */
		WHITENESS =0x00FF0062 /* dest = WHITE */
	}

	public enum DibColorTableType : uint
	{
		DIB_RGB_COLORS = 0, /* color table in RGBs */
		DIB_PAL_COLORS = 1 /* color table in palette indices */
	}

	public enum BitMapInfoCompressionType : uint
	{
		BI_RGB = 0,
		BI_RLE8 = 1,
		BI_RLE4 = 2,
		BI_BITFIELDS = 3,
		BI_JPEG = 4,
		BI_PNG = 5
	}

	public enum WM_MOUSEACTIVATEReturn
	{
		MA_ACTIVATE = 1,
		MA_ACTIVATEANDEAT = 2,
		MA_NOACTIVATE = 3,
		MA_NOACTIVATEANDEAT = 4
	}

	public const uint CBM_INIT = 0x04;

	[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Auto)]
	public struct WNDCLASS 
	{
		public WindowClassStyle style;
		public WNDPROC lpfnWndProc;
		public int cbClsExtra;
		public int cbWndExtra;
		public IntPtr hInstance;
		public IntPtr hIcon;
		public IntPtr hCursor;
		public IntPtr hbrBackground;
		private IntPtr lpszMenuName__;
		private IntPtr lpszClassName__;

		// Hack - Portable.NET doesn't support structure marshaling yet.
		public string lpszMenuName
		{
			set
			{
				if(value == null)
				{
					lpszMenuName__ = IntPtr.Zero;
				}
				else
				{
					if((GetVersion() & 0xC0000000) != 0)
					{
						// Windows 9x and less.							
						lpszMenuName__ =
							Marshal.StringToHGlobalAnsi(value);
					}
					else
					{
						lpszMenuName__ =
							Marshal.StringToHGlobalUni(value);
					}
				}
			}
		}
		public string lpszClassName
		{
			set
			{
				if(value == null)
				{
					lpszClassName__ = IntPtr.Zero;
				}
				else
				{
					if((GetVersion() & 0xC0000000) != 0)
					{
						// Windows 9x and less.
						lpszClassName__ =
							Marshal.StringToHGlobalAnsi(value);
					}
					else
					{
						lpszClassName__ =
							Marshal.StringToHGlobalUni(value);
					}
				}
			}
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MSG 
	{
		public IntPtr hwnd;
		public WindowsMessages message;
		public int wParam;
		public int lParam;
		public int itime;
		public int pt_x;
		public int pt_y;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct PAINTSTRUCT 
	{
		public IntPtr hdc;
		public bool fErase;
		public int rcPaintLeft;
		public int rcPaintTop;
		public int rcPaintRight;
		public int rcPaintBottom;
		public bool fRestore;
		public bool fIncUpdate;
		public int reserved1;
		public int reserved2;
		public int reserved3;
		public int reserved4;
		public int reserved5;
		public int reserved6;
		public int reserved7;
		public int reserved8;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct RECT 
	{
		public int left; 
		public int top; 
		public int right; 
		public int bottom;
		public RECT ( int left, int top, int right, int bottom )
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct POINT 
	{
		public int x;
		public int y;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SIZE
	{
		public int cx;
		public int cy;
	}

	// Logical Brush (or Pattern)
	[StructLayout(LayoutKind.Sequential)]
	public struct LOGBRUSH 
	{
		public LogBrushStyles lbStyle;
		public int lbColor;
		public int lbHatch;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct NONCLIENTMETRICS 
	{
		public uint cbSize;
		public int iBorderWidth;
		public int iScrollWidth;
		public int iScrollHeight;
		public int iCaptionWidth;
		public int iCaptionHeight;
		public LOGFONT lfCaptionFont;
		public int iSmCaptionWidth;
		public int iSmCaptionHeight;
		public LOGFONT lfSmCaptionFont;
		public int iMenuWidth;
		public int iMenuHeight;
		public LOGFONT lfMenuFont;
		public LOGFONT lfStatusFont;
		public LOGFONT lfMessageFont;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LOGFONT
	{
		public int lfHeight;
		public int lfWidth;
		public int lfEscapement;
		public int lfOrientation;
		public int lfWeight;
		public byte lfItalic;
		public byte lfUnderline;
		public byte lfStrikeout;
		public byte lfCharSet;
		public byte lfOutPrecision;
		public byte lfClipPrecision;
		public FontQuality lfQuality;
		public byte lfPitchAndFamily;

		// Hack - Portable.NET doesn't support structure marshaling yet.
		private byte lfFaceName_0;
		private byte lfFaceName_1;
		private byte lfFaceName_2;
		private byte lfFaceName_3;
		private byte lfFaceName_4;
		private byte lfFaceName_5;
		private byte lfFaceName_6;
		private byte lfFaceName_7;
		private byte lfFaceName_8;
		private byte lfFaceName_9;
		private byte lfFaceName_10;
		private byte lfFaceName_11;
		private byte lfFaceName_12;
		private byte lfFaceName_13;
		private byte lfFaceName_14;
		private byte lfFaceName_15;
		private byte lfFaceName_16;
		private byte lfFaceName_17;
		private byte lfFaceName_18;
		private byte lfFaceName_19;
		private byte lfFaceName_20;
		private byte lfFaceName_21;
		private byte lfFaceName_22;
		private byte lfFaceName_23;
		private byte lfFaceName_24;
		private byte lfFaceName_25;
		private byte lfFaceName_26;
		private byte lfFaceName_27;
		private byte lfFaceName_28;
		private byte lfFaceName_29;
		private byte lfFaceName_30;
		private byte lfFaceName_31;

		private static void SetFaceName(out byte dest, string value, int index)
		{
			if(value == null || index >= value.Length)
			{
				dest = 0;
			}
			else
			{
				dest = (byte)(value[index]);
			}
		}

		public string lfFaceName
		{
			set
			{
				SetFaceName(out lfFaceName_0, value, 0);
				SetFaceName(out lfFaceName_1, value, 1);
				SetFaceName(out lfFaceName_2, value, 2);
				SetFaceName(out lfFaceName_3, value, 3);
				SetFaceName(out lfFaceName_4, value, 4);
				SetFaceName(out lfFaceName_5, value, 5);
				SetFaceName(out lfFaceName_6, value, 6);
				SetFaceName(out lfFaceName_7, value, 7);
				SetFaceName(out lfFaceName_8, value, 8);
				SetFaceName(out lfFaceName_9, value, 9);
				SetFaceName(out lfFaceName_10, value, 10);
				SetFaceName(out lfFaceName_11, value, 11);
				SetFaceName(out lfFaceName_12, value, 12);
				SetFaceName(out lfFaceName_13, value, 13);
				SetFaceName(out lfFaceName_14, value, 14);
				SetFaceName(out lfFaceName_15, value, 15);
				SetFaceName(out lfFaceName_16, value, 16);
				SetFaceName(out lfFaceName_17, value, 17);
				SetFaceName(out lfFaceName_18, value, 18);
				SetFaceName(out lfFaceName_19, value, 19);
				SetFaceName(out lfFaceName_20, value, 20);
				SetFaceName(out lfFaceName_21, value, 21);
				SetFaceName(out lfFaceName_22, value, 22);
				SetFaceName(out lfFaceName_23, value, 23);
				SetFaceName(out lfFaceName_24, value, 24);
				SetFaceName(out lfFaceName_25, value, 25);
				SetFaceName(out lfFaceName_26, value, 26);
				SetFaceName(out lfFaceName_27, value, 27);
				SetFaceName(out lfFaceName_28, value, 28);
				SetFaceName(out lfFaceName_29, value, 29);
				SetFaceName(out lfFaceName_30, value, 30);
				SetFaceName(out lfFaceName_31, value, 31);
			}
		}
	}

	[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Auto)]
	public struct TEXTMETRIC 
	{ 
		public int tmHeight; 
		public int tmAscent; 
		public int tmDescent; 
		public int tmInternalLeading; 
		public int tmExternalLeading; 
		public int tmAveCharWidth; 
		public int tmMaxCharWidth; 
		public int tmWeight; 
		public int tmOverhang; 
		public int tmDigitizedAspectX; 
		public int tmDigitizedAspectY; 
		public char tmFirstChar; 
		public char tmLastChar; 
		public char tmDefaultChar; 
		public char tmBreakChar; 
		public byte tmItalic; 
		public byte tmUnderlined; 
		public byte tmStruckOut; 
		public byte tmPitchAndFamily; 
		public byte tmCharSet; 
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct TRACKMOUSEEVENT
	{
		public int cbSize;
		public TrackMouseEventFlags dwFlags;
		public IntPtr hwndTrack;
		public uint dwHoverTime;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct WINDOWPOS
	{
		public IntPtr hwnd;
		public IntPtr hwndInsertAfter;
		public int x;
		public int y;
		public int cx;
		public int cy;
		public uint flags;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct RGNDATA
	{
		public uint dwSize; 
		public uint iType; 
		public uint nCount; 
		public uint nRgnSize; 
		public RECT rcBound;
		public byte[] buffer;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct BITMAPV4INFO
	{
		public uint biSize; 
		public int biWidth; 
		public int biHeight; 
		public ushort biPlanes; 
		public ushort biBitCount; 
		public uint biCompression; 
		public uint biSizeImage; 
		public int biXPelsPerMeter; 
		public int biYPelsPerMeter; 
		public uint biClrUsed; 
		public uint biClrImportant; 
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct ICONINFO
	{
		public bool fIcon;
		public uint xHotspot;
		public uint yHotspot;
		public IntPtr hbmMask;
		public IntPtr hbmColor;
	}

	public delegate void TimerProc(IntPtr hwnd, uint uMsg, uint idEvent, uint dwTime);

	[DllImport("user32", EntryPoint="RegisterClass", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	public static extern int RegisterClassA(ref WNDCLASS wc);

	[DllImport("user32", EntryPoint="DefWindowProc", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	public static extern int DefWindowProcA(IntPtr hwnd, int msg, int wParam, int lParam);
		
	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern void PostQuitMessage(int nExitCode);

	[DllImport("user32", EntryPoint="GetMessage", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	public static extern int GetMessageA(out MSG msg, IntPtr hwnd, int minFilter, int maxFilter);

	[DllImport("user32", EntryPoint="PeekMessage", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool PeekMessageA(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, PeekMessageType wRemoveMsg );
	
	[DllImport("user32", EntryPoint="DispatchMessage", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	public static extern int DispatchMessageA(ref MSG msg);
	
	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool TranslateMessage(ref MSG msg);

	[DllImport("gdi32", EntryPoint="TextOut", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool TextOutA(IntPtr hdc, int x, int y, string textstring, int charCount);

	[DllImport("gdi32", EntryPoint="ExtTextOut", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ExtTextOutA( IntPtr hdc, int X, int Y, uint fuOptions, IntPtr lprc, String lpString, uint cbCount,IntPtr lpDx);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr BeginPaint(IntPtr hwnd, ref PAINTSTRUCT ps);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool EndPaint (IntPtr hwnd, ref PAINTSTRUCT ps);
		
	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetClientRect(IntPtr hwnd, out RECT rect);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern int GetClipBox(IntPtr hdc, out RECT lprc);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)] 
	public extern static System.IntPtr GetDC(System.IntPtr hwnd);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern int ReleaseDC(IntPtr hwnd, IntPtr hDC);

	[DllImport("user32", EntryPoint="CreateWindowEx", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	public static extern IntPtr CreateWindowExA(WindowsExtendedStyle dwExStyle, string lpszClassName, string lpszWindowName, WindowStyle style, int x, int y, int width, int height, IntPtr hWndParent, IntPtr hMenu, IntPtr hInst, IntPtr pvParam);
	
	[DllImport("kernel32", EntryPoint="GetModuleHandle", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	public static extern IntPtr GetModuleHandleA(string modName);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetWindowPos( IntPtr hwnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowsPosFlags uFlags );

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetWindowPos( IntPtr hwnd, SetWindowsPosPosition hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowsPosFlags uFlags );

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ShowWindow(IntPtr hwnd, ShowWindowCommand nCmdShow );

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool UpdateWindow(IntPtr hwnd);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool LineTo(IntPtr hdc, int nXEnd, int nYEnd);
	
	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool MoveToEx(IntPtr hdc, int X, int Y, IntPtr lpPoint);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern int SetPixel(IntPtr hdc, int X, int Y, int crColor );

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool Polygon( IntPtr hdc, POINT[] lpPoints, int nCount);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern int SetPolyFillMode( IntPtr hdc, int iPolyFillMode );

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool DeleteObject(IntPtr hObject);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr CreatePen(int fnPenStyle, int nWidth, int crColor);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr CreateBrushIndirect(ref LOGBRUSH lplb);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr CreatePatternBrush(IntPtr hbmp);

	[DllImport("user32", EntryPoint="PostMessage", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool PostMessageA(IntPtr hwnd, WindowsMessages Msg, int wParam, int lParam);

  	[DllImport("user32", EntryPoint="SendMessage", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)]  //Auto
	public static extern int SendMessageA( IntPtr hWnd, WindowsMessages Msg, int wParam, IntPtr lParam);

	[DllImport("user32", EntryPoint="RegisterWindowMessage", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)]	// Auto
	public static extern WindowsMessages RegisterWindowMessageA (string msgName);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern int GetSystemMetrics (SystemMetricsType nIndex);

	[DllImport("user32", EntryPoint="SystemParametersInfo", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SystemParametersInfoA(SystemParametersAction uiAction, uint uiParam, out RECT pvParam, uint fWinIni );

	[DllImport("user32", EntryPoint="SystemParametersInfo", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SystemParametersInfoA(SystemParametersAction uiAction, uint uiParam, out int pvParam, uint fWinIni );
		
	[DllImport("user32", EntryPoint="SystemParametersInfo", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SystemParametersInfoA(SystemParametersAction uiAction, uint uiParam, [MarshalAs(UnmanagedType.Bool)] out bool pvParam, uint fWinIni );
		
	[DllImport("user32", EntryPoint="SystemParametersInfo", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SystemParametersInfoA(uint uiAction, uint uiParam, ref NONCLIENTMETRICS pvParam,uint fWinIni);

	//Get font information
	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetTextMetricsA( IntPtr hdc, out TEXTMETRIC lptm);

	//Measure size and width of text
	[DllImport("gdi32", EntryPoint="GetTextExtentPoint32", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	public static extern int GetTextExtentPoint32A(IntPtr hdc, string str, int len, out SIZE size);
	
	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr GetStockObject( StockObjectType fnObject );

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern int SetBkMode(IntPtr hdc, BackGroundModeType iBkMode);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern int FillRect(IntPtr hDC, ref RECT lprc, IntPtr hbr);

	[DllImport("user32", EntryPoint="SetWindowText", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetWindowTextA( IntPtr hWnd, string lpString);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GetWindowRect( IntPtr hWnd, out RECT lpRect );

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool InvalidateRect(IntPtr hWnd, ref RECT hRgn, [MarshalAs(UnmanagedType.Bool)] bool bErase);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool InvalidateRect(IntPtr hWnd, IntPtr hRgn, [MarshalAs(UnmanagedType.Bool)] bool bErase);
	
	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool TrackMouseEvent( ref TRACKMOUSEEVENT lpEventTrack);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr SetCursor( IntPtr hCursor);
		
	[DllImport("user32", EntryPoint="LoadCursor", CharSet=CharSet.Auto, ExactSpelling=false, CallingConvention = CallingConvention.Winapi)] //Auto
	public static extern IntPtr LoadCursorA( IntPtr hInstance, CursorName lpCursorName);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool DestroyWindow( IntPtr hWnd );

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr GetDesktopWindow();

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr GetForegroundWindow();

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr GetFocus();

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CloseWindow(IntPtr hWnd);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public extern static int GetSysColor(int nIndex);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool Arc( IntPtr hdc, int nLeftRect,int nTopRect, int nRightRect, int nBottomRect, int nXStartArc, int nYStartArc, int nXEndArc, int nYEndArc );

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool Pie( IntPtr hdc, int nLeftRect,int nTopRect, int nRightRect, int nBottomRect, int nXRadial1, int nYRadial1, int nXRadial2, int nYRadial2 );

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool Ellipse( IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect );

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr SetTextColor( IntPtr hdc, int crColor);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern int SetBkColor( IntPtr hdc,int crColor);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool IsWindowVisible( IntPtr hWnd);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr CreateBitmap( int nWidth, int nHeight, uint cPlanes, uint cBitsPerPel, byte[] lpvBits);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr CreateDIBitmap( IntPtr hdc, byte[] lpbmih, uint fdwInit, byte[] lpbInit, byte[] lpbmi, uint fuUsage);
	
	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern int SetWindowLongA( IntPtr hWnd, SetWindowLongType nIndex, WindowStyle dwNewLong);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern int SetWindowLongA( IntPtr hWnd, SetWindowLongType nIndex, WindowsExtendedStyle dwNewLong);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern int GetWindowLongA( IntPtr hWnd, SetWindowLongType nIndex);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool RedrawWindow( IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool AdjustWindowRectEx(ref RECT lpRect, WindowStyle dwStyle, [MarshalAs(UnmanagedType.Bool)] bool bMenu, WindowsExtendedStyle dwExStyle);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern short GetKeyState( VirtualKeyType nVirtKey );

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
	
	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetRectRgn(IntPtr hrgn, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern int CombineRgn( IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, RegionCombineMode fnCombineMode);
		
	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern int ExtSelectClipRgn( IntPtr hdc, IntPtr hrgn, RegionCombineMode fnMode );

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern int GetClipRgn( IntPtr hdc, IntPtr hrgn );

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern int SelectClipRgn(IntPtr hdc, IntPtr hrgn);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern int OffsetRgn( IntPtr hrgn, int nXOffset, int nYOffset );

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern int GetObject( IntPtr hgdiobj, int cbBuffer, out LOGFONT lpvObject );

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr CreateFontIndirectA(ref LOGFONT lf);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern uint SetTimer(IntPtr hwnd, uint nIDEvent, uint uElapse, TimerProc lpTimerFunc);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern uint SetTimer(IntPtr hwnd, uint nIDEvent, uint uElapse, IntPtr lpTimerFunc);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool KillTimer(IntPtr hwnd, uint uIDEvent);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ScreenToClient( IntPtr hWnd, ref POINT lpPoint );

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool ClientToScreen( IntPtr hWnd, ref POINT lpPoint );

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr SetParent( IntPtr hWndChild, IntPtr hWndNewParent );

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr SetFocus( IntPtr hWnd );

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr SetCapture( IntPtr hWnd );

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr GetCapture();

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr ReleaseCapture();

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr WindowFromPoint( POINT Point);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr CreateCompatibleBitmap( IntPtr hdc, int nWidth, int nHeight);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern int SetDIBits( IntPtr hdc, IntPtr hbmp, uint uStartScan, uint cScanLines, ref byte lpvBits, byte[] lpbmi, DibColorTableType fuColorUse);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern int SetDIBitsToDevice( IntPtr hdc, int XDest, int YDest, uint dwWidth, uint dwHeight, int XSrc, int YSrc, uint uStartScan, uint cScanLines, ref byte lpvBits, byte[] lpbmi, DibColorTableType fuColorUse);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern int BitBlt (IntPtr hdcDest, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, RopType dwRop);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr CreateCompatibleDC( IntPtr hdc);

	[DllImport("kernel32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr GlobalLock( IntPtr hMem);

	[DllImport("kernel32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool GlobalUnlock(IntPtr hMem);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern int SaveDC( IntPtr hdc );

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool RestoreDC( IntPtr hdc, int nSavedDC );

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr ExtCreateRegion( IntPtr lpXform, uint nCount, ref byte lpRgnData);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	public static extern uint GetRegionData( IntPtr hRgn, uint dwCount, ref byte lpRgnData);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr CreateIcon( IntPtr hInstance, int nWidth, int nHeight, byte cPlanes, byte cBitsPixel, ref byte lpbANDbits, ref byte lpbXORbits);

	[DllImport("user32", CallingConvention = CallingConvention.Winapi)]
	public static extern IntPtr CreateIconIndirect(ref ICONINFO piconinfo);

	[DllImport("gdi32", CallingConvention = CallingConvention.Winapi)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool DeleteDC(IntPtr hdc);
	
	[DllImport("kernel32", CallingConvention = CallingConvention.Winapi)]
	public static extern uint GetLastError();
	
	[DllImport("kernel32", CallingConvention = CallingConvention.Winapi)]
	public static extern uint SetLastError(uint errorCode);

	// Retrieves Windows version
	// To obtain e.g. the major version:
	// windowsMajorVer = GetVersion() & 0xFF;
	[DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
	public extern static uint GetVersion();
}//Api

}
