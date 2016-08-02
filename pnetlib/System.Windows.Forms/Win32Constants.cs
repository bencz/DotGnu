/*
 * Win32Constants.cs - Implementation of the
 *			"System.Windows.Forms.Win32Constants" class.
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

internal sealed class Win32Constants
{
	// Class style values.
	public const int CS_BYTEALIGNCLIENT		= 4096;
	public const int CS_BYTEALIGNWINDOW		= 8192;
	public const int CS_KEYCVTWINDOW		= 4;
	public const int CS_NOKEYCVT			= 256;
	public const int CS_CLASSDC				= 64;
	public const int CS_DBLCLKS				= 8;
	public const int CS_GLOBALCLASS			= 16384;
	public const int CS_HREDRAW				= 2;
	public const int CS_NOCLOSE				= 512;
	public const int CS_OWNDC				= 32;
	public const int CS_PARENTDC			= 128;
	public const int CS_SAVEBITS			= 2048;
	public const int CS_VREDRAW				= 1;
	public const int CS_IME					= 0x10000;

	// Window style values.
	public const int WS_BORDER				= 0x00800000;
	public const int WS_CAPTION				= 0x00c00000;
	public const int WS_CHILD				= 0x40000000;
	public const int WS_CHILDWINDOW			= 0x40000000;
	public const int WS_CLIPCHILDREN		= 0x02000000;
	public const int WS_CLIPSIBLINGS		= 0x04000000;
	public const int WS_DISABLED			= 0x08000000;
	public const int WS_DLGFRAME			= 0x00400000;
	public const int WS_GROUP				= 0x00020000;
	public const int WS_HSCROLL				= 0x00100000;
	public const int WS_ICONIC				= 0x20000000;
	public const int WS_MAXIMIZE			= 0x01000000;
	public const int WS_MAXIMIZEBOX			= 0x00010000;
	public const int WS_MINIMIZE			= 0x20000000;
	public const int WS_MINIMIZEBOX			= 0x00020000;
	public const int WS_OVERLAPPED			= 0x00000000;
	public const int WS_OVERLAPPEDWINDOW	= 0x00cf0000;
	public const int WS_POPUP				= unchecked((int)0x80000000);
	public const int WS_POPUPWINDOW			= unchecked((int)0x80880000);
	public const int WS_SIZEBOX				= 0x00040000;
	public const int WS_SYSMENU				= 0x00080000;
	public const int WS_TABSTOP				= 0x00010000;
	public const int WS_THICKFRAME			= 0x00040000;
	public const int WS_TILED				= 0x00000000;
	public const int WS_TILEDWINDOW			= 0x00cf0000;
	public const int WS_VISIBLE				= 0x10000000;
	public const int WS_VSCROLL				= 0x00200000;

	// Extended window styles.
	public const int WS_EX_ACCEPTFILES		= 0x00000010;
	public const int WS_EX_APPWINDOW		= 0x00040000;
	public const int WS_EX_CLIENTEDGE		= 0x00000200;
	public const int WS_EX_COMPOSITED		= 0x02000000;
	public const int WS_EX_CONTEXTHELP		= 0x00000400;
	public const int WS_EX_CONTROLPARENT	= 0x00010000;
	public const int WS_EX_DLGMODALFRAME	= 0x00000001;
	public const int WS_EX_LAYERED			= 0x00080000;
	public const int WS_EX_LAYOUTRTL		= 0x00400000;
	public const int WS_EX_LEFT				= 0x00000000;
	public const int WS_EX_LEFTSCROLLBAR	= 0x00004000;
	public const int WS_EX_LTRREADING		= 0x00000000;
	public const int WS_EX_MDICHILD			= 0x00000040;
	public const int WS_EX_NOACTIVATE		= 0x08000000;
	public const int WS_EX_NOINHERITLAYOUT	= 0x00100000;
	public const int WS_EX_NOPARENTNOTIFY	= 0x00000004;
	public const int WS_EX_OVERLAPPEDWINDOW	= 0x00000300;
	public const int WS_EX_PALETTEWINDOW	= 0x00000188;
	public const int WS_EX_RIGHT			= 0x00001000;
	public const int WS_EX_RIGHTSCROLLBAR	= 0x00000000;
	public const int WS_EX_RTLREADING		= 0x00002000;
	public const int WS_EX_STATICEDGE		= 0x00020000;
	public const int WS_EX_TOOLWINDOW		= 0x00000080;
	public const int WS_EX_TOPMOST			= 0x00000008;
	public const int WS_EX_TRANSPARENT		= 0x00000020;
	public const int WS_EX_WINDOWEDGE		= 0x00000100;

	// Message codes.
	public const int WM_KEYDOWN				= 256;
	public const int WM_KEYUP				= 257;
	public const int WM_CHAR				= 258;
	public const int WM_DEADCHAR			= 259;
	public const int WM_SYSKEYDOWN			= 260;
	public const int WM_SYSKEYUP			= 261;
	public const int WM_SYSCHAR				= 262;
	public const int WM_SYSDEADCHAR			= 263;

}; // class Win32Constants

}; // namespace System.Windows.Forms
