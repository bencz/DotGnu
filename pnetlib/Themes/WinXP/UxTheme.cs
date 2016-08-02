/*
 * UxTheme.cs - Bindings for UxTheme (User eXperience) library
 *
 * Copyright (C) 2004 by Maciek Plewa (http://mil-sim.net/contact)
 * Copyright (C) 2004 by Nordic Compona Solutions (http://www.compona.com)
 *
 * Licence: refer to the Readme file
 */


using System;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Runtime.InteropServices;

namespace ThemeXP.UxTheme
{
	#region Theme classes

	#region Button parts and states
	public enum ButtonParts : int
	{
		BP_NONE = 0,
		BP_PUSHBUTTON = 1,
		BP_RADIOBUTTON = 2,
		BP_CHECKBOX = 3,
		BP_GROUPBOX = 4,
		BP_USERBUTTON = 5,
	}
	
	public enum PushButtonStates : int
	{
		PBS_NONE = 0,
		PBS_NORMAL = 1,
		PBS_HOT = 2,
		PBS_PRESSED = 3,
		PBS_DISABLED = 4,
		PBS_DEFAULTED = 5
	}

	public enum RadioButtonStates : int
	{
		RBS_NONE = 0,
		RBS_UNCHECKEDNORMAL = 1,
		RBS_UNCHECKEDHOT = 2,
		RBS_UNCHECKEDPRESSED = 3,
		RBS_UNCHECKEDDISABLED = 4,
		RBS_CHECKEDNORMAL = 5,
		RBS_CHECKEDHOT = 6,
		RBS_CHECKEDPRESSED = 7,
		RBS_CHECKEDDISABLED = 8
	}

	public enum CheckBoxStates : int
	{
		CBS_NONE = 0,
		CBS_UNCHECKEDNORMAL = 1,
		CBS_UNCHECKEDHOT = 2,
		CBS_UNCHECKEDPRESSED = 3,
		CBS_UNCHECKEDDISABLED = 4,
		CBS_CHECKEDNORMAL = 5,
		CBS_CHECKEDHOT = 6,
		CBS_CHECKEDPRESSED = 7,
		CBS_CHECKEDDISABLED = 8,
		CBS_MIXEDNORMAL = 9,
		CBS_MIXEDHOT = 10,
		CBS_MIXEDPRESSED = 11,
		CBS_MIXEDDISABLED = 12,
	}

	public enum GroupBoxStates : int
	{
		GBS_NONE = 0,
		GBS_NORMAL = 1,
		GBS_DISABLED = 2,
	}
	#endregion

	#region ComboBox parts and states

	public enum ComboBoxParts: int
	{
		CP_NONE = 0,
		CP_DROPDOWNBUTTON = 1
	}

	public enum ComboBoxStates : int
	{
		CBXS_NONE = 0,
		CBXS_NORMAL = 1,
		CBXS_HOT = 2,
		CBXS_PRESSED = 3,
		CBXS_DISABLED = 4
	}

	#endregion

	#region Edit parts and states
	public enum EditParts: int
	{
		EP_NONE = 0,
		EP_EDITTEXT = 1,
		EP_CARET = 2
	}

	public enum EditStates: int
	{
		ETS_NONE = 0,
		ETS_NORMAL = 1,
		ETS_HOT = 2,
		ETS_SELECTED = 3,
		ETS_DISABLED = 4,
		ETS_FOCUSED = 5,
		ETS_READONLY = 6,
		ETS_ASSIST = 7
	}
	#endregion

	#region Header parts and states

	public enum HeaderParts: int
	{
		HP_NONE = 0,
		HP_HEADERITEM = 1,
		HP_HEADERITEMLEFT = 2,
		HP_HEADERITEMRIGHT = 3,
		HP_HEADERSORTARROW = 4
	}

	public enum HeaderItemStates: int
	{
		HIS_NONE = 0,
		HIS_NORMAL = 1,
		HIS_HOT = 2,
		HIS_PRESSED = 3
	}

	public enum HeaderItemLeftStates: int
	{
		HILS_NONE = 0,
		HILS_NORMAL = 1,
		HILS_HOT = 2,
		HILS_PRESSED = 3
	}

	public enum HeaderItemRightStates: int
	{
		HIRS_NONE = 0,
		HIRS_NORMAL = 1,
		HIRS_HOT = 2,
		HIRS_PRESSED = 3
	}

	public enum HeaderSortArrowStates: int
	{
		HSAS_NONE = 0,
		HSAS_SORTEDUP = 1,
		HSAS_SORTEDDOWN = 2
	}

	#endregion

	#region Progress parts and states

	public enum ProgressParts : int
	{
		PP_NONE = 0,
		PP_BAR = 1,
		PP_BARVERT = 2,
		PP_CHUNK = 3,
		PP_CHUNKVERT = 4
	}
	#endregion

	#region ScrollBar parts and states
	public enum ScrollBarParts: int
	{
		SBP_NONE = 0,
		SBP_ARROWBTN = 1,
		SBP_THUMBBTNHORZ = 2,
		SBP_THUMBBTNVERT = 3,
		SBP_LOWERTRACKHORZ = 4,
		SBP_UPPERTRACKHORZ = 5,
		SBP_LOWERTRACKVERT = 6,
		SBP_UPPERTRACKVERT = 7,
		SBP_GRIPPERHORZ = 8,
		SBP_GRIPPERVERT = 9,
		SBP_SIZEBOX = 10
	}

	public enum ArrowButtonStates: int
	{
		ABS_NONE = 0,
		ABS_UPNORMAL = 1,
		ABS_UPHOT = 2,
		ABS_UPPRESSED = 3,
		ABS_UPDISABLED = 4,
		ABS_DOWNNORMAL = 5,
		ABS_DOWNHOT = 6,
		ABS_DOWNPRESSED = 7,
		ABS_DOWNDISABLED = 8,
		ABS_LEFTNORMAL = 9,
		ABS_LEFTHOT = 10,
		ABS_LEFTPRESSED = 11,
		ABS_LEFTDISABLED = 12,
		ABS_RIGHTNORMAL = 13,
		ABS_RIGHTHOT = 14,
		ABS_RIGHTPRESSED = 15,
		ABS_RIGHTDISABLED = 16
	}

	public enum ScrollBarStates: int
	{
		SCRBS_NONE = 0,
		SCRBS_NORMAL = 1,
		SCRBS_HOT = 2,
		SCRBS_PRESSED = 3,
		SCRBS_DISABLED = 4
	}
	public enum SizeBoxStates: int
	{
		SZB_NONE = 0,
		SZB_RIGHTALIGN = 1,
		SZB_LEFTALIGN = 2
	}
	#endregion

	enum DrawTextFlags : int
	{
		DT_TOP = 0x0,
		DT_LEFT = 0x0,
		DT_CENTER = 0x1,
		DT_RIGHT = 0x2,
		DT_VCENTER = 0x4,
		DT_BOTTOM = 0x8,
		DT_WORDBREAK = 0x10,
		DT_SINGLELINE = 0x20,
		DT_EXPANDTABS = 0x40,
		DT_TABSTOP = 0x80,
		DT_NOCLIP = 0x100,
		DT_EXTERNALLEADING = 0x200,
		DT_CALCRECT = 0x400,
		DT_NOPREFIX = 0x800,
		DT_INTERNAL = 0x1000,
		DT_EDITCONTROL = 0x2000,
		DT_PATH_ELLIPSIS = 0x4000,
		DT_END_ELLIPSIS = 0x8000,
		DT_MODIFYSTRING = 0x10000,
		DT_RTLREADING = 0x20000,
		DT_WORD_ELLIPSIS = 0x40000,
		DT_NOFULLWIDTHCHARBREAK = 0x80000,
		DT_HIDEPREFIX = 0x100000,
		DT_PREFIXONLY = 0x200000,
	}

	#endregion

	/// <summary>
	/// User eXperience library bindings
	/// </summary>
	internal class UxThemeAPI : WinAPI
	{
		private const string uxthemeLib = "uxtheme.dll";
		// UxTheme DrawText Additional Flag
		protected const int DTT_GRAYED  = 0x1;

		/// <summary>
		/// Used to identify the size of a visual style part
		/// </summary>
		protected enum THEMESIZE : int 
		{
			TS_MIN  = 0,	// minimum size of a visual style part
			TS_TRUE  = 1,	// size without stretching
			TS_DRAW  = 2,	// size that the theme manager uses to a draw part
		}


		#region DllImports

		[DllImportAttribute(uxthemeLib, SetLastError=true)]
		protected static extern IntPtr OpenThemeData(
			IntPtr hwnd,
			[MarshalAs( UnmanagedType.LPWStr )] string pszClassList );

		[DllImportAttribute(uxthemeLib)]
		protected static extern int ApplyTheme(
			IntPtr hThemeFile,
			string something,
			IntPtr hWnd);

		/* For Wine only
		[DllImportAttribute(uxthemeLib)]
		protected static extern int OpenThemeFile(
			[MarshalAs( UnmanagedType.LPWStr )] string lpThemeFile,
			[MarshalAs( UnmanagedType.LPWStr )] string pszColorName,
			[MarshalAs( UnmanagedType.LPWStr )] string pszSizeName,
			out IntPtr tf);
		*/

		[DllImport(uxthemeLib)]
		[return: MarshalAs(UnmanagedType.Bool)]
		protected extern static bool IsAppThemed();

		[DllImport(uxthemeLib)]
		[return: MarshalAs(UnmanagedType.Bool)]
		protected extern static bool IsThemeActive();

		[DllImport(uxthemeLib)]
		protected extern static int EnableTheming([MarshalAs(UnmanagedType.Bool)]bool fEnable);

		[DllImport(uxthemeLib)]
		protected extern static IntPtr GetWindowTheme(
			IntPtr hWnd);

		[DllImport(uxthemeLib)]
		protected extern static int SetWindowTheme(
			IntPtr hWnd,
			string pszSubAppName,
			string pszSubIdList
			);
			
		[DllImport(uxthemeLib)]
		protected extern static int CloseThemeData(
			IntPtr hTheme);

		[DllImportAttribute(uxthemeLib, CharSet=CharSet.Auto )]
		protected static extern int DrawThemeBackground(
			IntPtr hTheme,
			IntPtr hDC,
			int iPartId,
			int iStateId,
			ref RECT rect,
			ref RECT clipRect );

		[DllImport(uxthemeLib)]
		protected extern static int DrawThemeParentBackground(
			IntPtr hWnd,
			IntPtr hDC, 
			ref RECT prc
			);

		[DllImport(uxthemeLib)]
		protected extern static int GetThemeBackgroundContentRect(
			IntPtr hTheme,
			IntPtr hDC,
			int iPartId,
			int iStateId,
			ref RECT pBoundingRect,
			ref RECT pContentRect
			);

		[DllImport(uxthemeLib)]
		protected extern static int DrawThemeText(
			IntPtr hTheme,
			IntPtr hDC,
			int iPartId,
			int iStateId,
			[MarshalAs( UnmanagedType.LPWStr )] string pszText,
			int iCharCount,
			int dwTextFlag,
			int dwTextFlags2,
			ref RECT pRect
			);

		[DllImport(uxthemeLib)]
		protected extern static int DrawThemeIcon(
			IntPtr hTheme,
			IntPtr hDC,
			int iPartId,
			int iStateId,
			ref RECT pRect,
			IntPtr hIml,
			int iImageIndex
			);

		[DllImport(uxthemeLib)]
		protected extern static int GetThemePartSize(
			IntPtr hTheme,
			IntPtr hDC,
			int iPartId,
			int iStateId,
			ref RECT pRect,
			int iSize,
			ref SIZEAPI pSz
			);

		[DllImport(uxthemeLib)]
		protected extern static int GetThemeTextExtent(
			IntPtr hTheme,
			IntPtr hDC,
			int iPartId,
			int iStateId,
			string pszText,
			int iCharCount,
			int dwTextFlags,   	
			ref RECT pBoundingRect,   	
			ref RECT pExtentRect
			);

		[DllImport(uxthemeLib)]
		protected extern static int DrawThemeBackground(
			IntPtr hTheme,
			IntPtr hDC,
			int iPartId,
			int iStateId,
			ref RECT pDestRect,
			int uEdge,
			int uFlags,
			ref RECT pContentRect
			);

		#endregion
	};	// class UxThemeAPI

}

namespace ThemeXP
{
	internal class Theme : UxTheme.UxThemeAPI
	{
		// theme cache lookup table
		private static Hashtable _hThemeLookup = new Hashtable ();

		private static IntPtr GetCachedTheme(string ThemeName)
		{
			return (IntPtr)_hThemeLookup[ThemeName];
		}


		private static IntPtr GetNewTheme(string ThemeName)
		{
			IntPtr hTheme = IntPtr.Zero;

			// Retrieve theme information from the desktop window
			hTheme = OpenThemeData((IntPtr)HWND_DESKTOP, ThemeName);

			if (hTheme != IntPtr.Zero)
			{
				_hThemeLookup[ThemeName]=hTheme;
				return (IntPtr)_hThemeLookup[ThemeName];
			}
			else
			{
				return IntPtr.Zero;
			}
		}
		

		/// <summary>
		/// Opens the theme data for associated control class. Windows XP and
		/// Windows 2003 support the following classes:
		/// 
		/// Window, Button, ReBar, ToolBar, Status, ListView, Header, Progress,
		/// Tab, TrackBar, Tooltip, TreeView, Spin, Scrollbar, Edit, ComboBox,
		/// TaskBar, TaskBand, StartPanel, ExplorerBar
		/// </summary>
		/// <param name="ctlClass">Control class</param>
		/// <returns>Handle to a theme associated with the control class</returns>
		public static IntPtr GetTheme(string ctlClass)
		{
			if (_hThemeLookup[ctlClass] == null)
				return GetNewTheme(ctlClass);
			else
				return GetCachedTheme(ctlClass);
		}


		public static void DrawControlText(Graphics graphics, String ctlClass,
			Rectangle bounds, String text, Font font,
			int ctlPart, int ctlState)
		{
			// for now i'd rather have it draw nothing than crash or raise an exception
			try
			{
				IntPtr theme = Theme.GetTheme(ctlClass);

				if (theme == IntPtr.Zero)
					return;

				RECT boundsRC = new RECT(bounds);

				// retrieve the device context of the graphics object
				IntPtr dc = graphics.GetHdc();
				IntPtr oldfont = SelectObject(dc, font.ToHfont());

				DrawThemeText(theme, dc, ctlPart, ctlState, text, -1, 0, 0, ref boundsRC);

				SelectObject(dc, oldfont);
			}
			catch
			{
			}
		}


		public static void DrawControl(Graphics graphics, String ctlClass,
			int x, int y, int width, int height,
			int ctlPart, int ctlState)
		{
			try
			{
				IntPtr theme = Theme.GetTheme(ctlClass);

				if (theme == IntPtr.Zero)
					return;

				Rectangle dimensions = new Rectangle(x, y, width, height);

				RECT boundsRC = new RECT(x, y, x+width, y+height);
				RECT clipRC = new RECT(Rectangle.Ceiling(graphics.VisibleClipBounds));

				// retrieve the device context of the graphics object
				IntPtr dc = graphics.GetHdc();
	
				DrawThemeBackground(theme, dc, ctlPart, ctlState,
					ref boundsRC, ref clipRC);

				//dude, where's my drawing context?
				//you have to recreate the graphics to draw on after
				//a graphics.releasehdc (hdc) have been called on it
				//graphics.ReleaseHdc(dc);
			}
			catch {}
		}


		public static void DrawControl(Graphics graphics, String ctlClass,
			Rectangle dimensions,
			int ctlPart, int ctlState)
		{
			DrawControl(graphics, ctlClass, dimensions.X, dimensions.Y,
				dimensions.Width, dimensions.Height, ctlPart, ctlState);
		}


		/* Unused
		public static void DrawFocusRectangle(Graphics graphics, Rectangle dimensions)
		{
			IntPtr dotted = CreatePen(PenStyle.PS_DOT, 1, 0);
			IntPtr dc = graphics.GetHdc();

			RECT bounds = new RECT(dimensions);

			SelectObject(dc, dotted); 
			
			bool result = DrawRectangle(dc,
				bounds.left, bounds.top, bounds.right, bounds.bottom); 

			Console.WriteLine("result: " + result);
		}
		*/

		/*
		public static void DrawParentControl(Graphics graphics, Rectangle dimensions)
		{
			IntPtr dc = graphics.GetHdc();
			IntPtr wnd = WindowFromDC(dc);
			RECT rectum = new RECT(dimensions);

			if (wnd != IntPtr.Zero)
			{
				if (DrawThemeParentBackground(wnd, dc, ref rectum) != S_OK)
					Console.WriteLine("UxTheme:DrawParentControl() FAILS");
			}
		}
		*/


		/// <summary>
		/// Disables Windows XP visual themes for a given window specified by handle.
		/// The change is persistent through the life of the window, even after themes change.
		/// </summary>
		/// <param name="hwnd">Handle to the window</param>
		/// <returns>true/false for success/failure</returns>
		public static bool DisableWindowTheme(IntPtr hwnd)
		{
			if (SetWindowTheme(hwnd, "", "") == S_OK)
				return true;
			else
				return false;
		}

	}; 	// class UxTheme

};	// namespace System.Windows.Forms.Themes.XP
