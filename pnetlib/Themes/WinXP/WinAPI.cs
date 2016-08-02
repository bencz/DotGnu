/*
 * WinAPI.cs - Utility structures and functions for accessing Windows API,
 * including GDI and User libraries
 * 
 * Note: this class includes some definitions and methods already present
 * in the internal class System.Drawing.Win32.Api - it should be made public
 * instead, adding new definitions from this file.
 *
 * Copyright (C) 2004 by Maciek Plewa (http://mil-sim.net/contact)
 *
 * Licence: refer to the Readme file
 */

using System;
using System.Drawing;
using System.Drawing.Win32;
using System.Runtime.InteropServices;

namespace ThemeXP
{
	/// <summary>
	/// Utility structures and functions for accessing Windows API
	/// </summary>
	internal class WinAPI
	{
		protected const int S_OK = 0;
		protected const int HWND_DESKTOP = 0;

		#region SIZEAPI
		[StructLayout(LayoutKind.Sequential)]
		protected struct SIZEAPI
		{
			public int cx;
			public int cy;

			public override string ToString()
			{
				return String.Format("{0} x {1}", cx, cy);
			}
		}
		#endregion

		#region WINDOWINFO
		[StructLayout(LayoutKind.Sequential)]
		protected struct WINDOWINFO
		{
			public UInt32 cbSize;
			public RECT rcWindow;
			public RECT rcClient;
			public UInt32 dwStyle;
			public UInt32 dwExStyle;
			public UInt32 dwWindowStatus;
			public uint cxWindowBorders;
			public uint cyWindowBorders;
			public byte atomWindowType;
			public UInt16 wCreatorVersion;
		}
		#endregion

		#region POINT
		[StructLayout(LayoutKind.Sequential)]
		protected struct POINT 
		{
			public int x;
			public int y;

			public POINT( Point point )
			{
				x = point.X;
				y = point.Y;
			}

			public POINT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}   
		#endregion

		#region RECT
		[StructLayout(LayoutKind.Sequential)]
		protected struct RECT 
		{
			public int left;
			public int top;
			public int right;
			public int bottom;

			public RECT(Rectangle rect)
			{
				bottom = rect.Bottom;
				left = rect.Left;
				right = rect.Right;
				top = rect.Top;
			}

			public RECT(int left, int top, int right, int bottom )
			{
				this.bottom = bottom;
				this.left = left;
				this.right = right;
				this.top = top;
			}
		}
		#endregion

		#region TEXTMETRIC
		/// <summary>
		/// Describes a physical font. The size is specified in logical units.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct TEXTMETRIC
		{
			int Height;
			int Ascent;
			int Descent;
			int InternalLeading;
			int ExternalLeading;
			int AveCharWidth;
			int MaxCharWidth;
			int Weight;
			int Overhang;
			int DigitizedAspectX;
			int DigitizedAspectY;
			Char FirstChar;
			Char LastChar;
			Char DefaultChar;
			Char BreakChar;
			byte Italic;
			byte Underlined;
			byte StruckOut;
			byte PitchAndFamily;
			byte CharSet;
		}
		#endregion

		#region Pen styles
		public enum PenStyle : int
		{
			PS_SOLID = 0,
			PS_DASH = 1,
			PS_DOT = 2,
			PS_DASHDOT = 3,
			PS_DASHDOTDOT = 4,
			PS_NULL = 5,
			PS_INSIDEFRAME = 6,
			PS_USERSTYLE = 7,
			PS_ALTERNATE = 8,
			PS_STYLE_MASK = 0x0000000F
		}
		#endregion

		#region User32 library bindings

		/// <summary>
		/// Obtains information about an existing window
		/// </summary>
		/// <param name="hWnd">Window handle</param>
		/// <param name="pwi">Pointer to WINDOWINFO structure
		/// receiving the information [out]</param>
		/// <returns>Returns true if function succeeds or false if it fails</returns>
		[DllImport("user32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		protected extern static bool GetWindowInfo(
			IntPtr hWnd,
			out WINDOWINFO pwi);

		/// <summary>
		/// Returns a handle to the window associated with the
		/// specified display device context
		/// </summary>
		/// <param name="hDC">Display device context</param>
		/// <returns>Window handle</returns>
		[DllImport("user32")]
		protected extern static IntPtr WindowFromDC(IntPtr hDC);

		/// <summary>
		/// Determines whether the specified window handle
		/// identifies an existing window
		/// </summary>
		/// <param name="hWnd">Handle to the window to test</param>
		/// <returns></returns>
		[DllImport("user32")]
		[return: MarshalAs(UnmanagedType.Bool)]
		protected extern static bool IsWindow(IntPtr hWnd);

		/// <summary>
		/// Retrieves the desktop window handle
		/// </summary>
		/// <returns>Handle to desktop window</returns>
		[DllImport("user32")]
		protected extern static IntPtr GetDesktopWindow();

		/// <summary>
		/// Retrieves a handle to specified window's parent window
		/// </summary>
		/// <param name="hWnd">Window handle</param>
		/// <returns>Handle to parent window</returns>
		[DllImport("user32")]
		protected extern static IntPtr GetParent(IntPtr hWnd);

		#endregion

		#region Kernel and CommCtl library bindings

		[DllImport("kernel32.dll")]
		private extern static uint GetVersion();

		/// <summary>
		/// Enables Windows common controls to use XP visual styles
		/// </summary>
		[DllImport("comctl32")]
		protected extern static void InitCommonControls();

		#endregion

		#region GDI

		/// <summary>
		/// Selects an object into the specified device context
		/// </summary>
		/// <param name="hDc">Handle of the device context</param>
		/// <param name="hObject">Object handle</param>
		/// <returns>Returns IntPtr.Zero if function fails</returns>
		[DllImport("gdi32")]
		protected extern static IntPtr SelectObject(
			IntPtr hDc,
			IntPtr hObject);

		[DllImport("gdi32")]
		public static extern IntPtr CreatePen(PenStyle fnPenStyle, int nWidth, int crColor);

		[DllImport("gdi32", EntryPoint="Rectangle")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DrawRectangle(
			IntPtr hDc,
			int left, 
			int top,
			int right,
			int bottom);

		#endregion

		/// <summary>
		/// Retrieves the position and dimensions of a specified window
		/// </summary>
		/// <param name="hwnd">Window handle</param>
		/// <returns>Returns a rectangle object describing the client area</returns>
		public static Rectangle GetWindowClientRect(IntPtr hwnd)
		{
			if (hwnd == IntPtr.Zero)
				return System.Drawing.Rectangle.Empty;

			WINDOWINFO wi = new WINDOWINFO();

			if (GetWindowInfo(hwnd, out wi))
			{
				Rectangle rect = new Rectangle(
					wi.rcClient.left,
					wi.rcClient.top,
					wi.rcClient.right - wi.rcClient.left,
					wi.rcClient.bottom - wi.rcClient.top);

				return rect;
			}

			return System.Drawing.Rectangle.Empty;
		}


		/// <summary>
		/// Retrieves Windows OS version
		/// </summary>
		/// <returns>
		/// System.Version object representing Windows version
		/// </returns>
		public static System.Version GetSystemVersion()
		{
			uint version = GetVersion();

			int major = (int)version & 0xFF;
			int minor = ((int)version & 0xFF00) >> 8;
			int build = ((int)version & 0xFFF0000) >> 16;

			return new Version(major, minor, build);
		}

	}	// WinAPI class
};	// namespace System.Windows.Forms.Themes.XP
