/*
 * DrawingFont.cs - Implementation of fonts for System.Drawing.Win32.
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

namespace System.Drawing.Toolkit
{

using System;
using System.Runtime.InteropServices;

internal class DrawingFont : IToolkitFont
{
	// Internal state.
	internal System.Drawing.Font properties;
	private float dpi;
	private IToolkit toolkit;
	internal IntPtr hFont;

	public DrawingFont(IToolkit toolkit, System.Drawing.Font properties, float dpi)
	{
		this.toolkit = toolkit;
		this.properties = properties;
		this.dpi = dpi;
		CreateFont();
	}

	// Select this font into a graphics object.
	public void Select(IToolkitGraphics graphics)
	{
		if ((graphics as DrawingGraphics).Font == this)
			return;
		Win32.Api.SelectObject((graphics as DrawingGraphics).hdc, hFont);
		(graphics as DrawingGraphics).Font = this;
		Win32.Api.SetBkMode((graphics as DrawingGraphics).hdc,Win32.Api.BackGroundModeType.TRANSPARENT);
			
	}

	protected virtual void Dispose(bool disposing)
	{
		Win32.Api.DeleteObject(hFont);
		hFont = IntPtr.Zero;
	}

	// Dispose of this font.
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	~DrawingFont()
	{
		Dispose(false);
	}

	// Get the raw HFONT for this toolkit font.  IntPtr.Zero if none.
	public IntPtr GetHfont()
	{
		return hFont;
	}

	// Get the LOGFONT information for this toolkit font.
	public void ToLogFont(Object lf, IToolkitGraphics graphics)
	{
		Win32.Api.LOGFONT logFont;
		Win32.Api.GetObject( hFont, Marshal.SizeOf(typeof(Win32.Api.LOGFONT)), out logFont);
		lf = logFont;
	}

	private void CreateFont()
	{
		properties.FontFamily.GetEmHeight(properties.Style);
		Win32.Api.LOGFONT lf = new Win32.Api.LOGFONT();
		lf.lfHeight=(int)(-properties.SizeInPoints*dpi/75);
		lf.lfFaceName=properties.FontFamily.Name;
		switch (properties.Style)
		{
			case(FontStyle.Bold):
				lf.lfWeight = 600;
				break;
			case(FontStyle.Italic):
				lf.lfItalic = 1;
				break;
			case(FontStyle.Strikeout):
				lf.lfStrikeout = 1;
				break;
			case(FontStyle.Underline):
				lf.lfUnderline = 1;
				break;
		}

		// Don't know why but the next flag makes Windows text look ugly
		// lf.lfQuality = Win32.Api.FontQuality.CLEARTYPE_QUALITY;

		hFont = Win32.Api.CreateFontIndirectA(ref lf);
	}

}; // class DrawingFont

}; // namespace System.Drawing.Toolkit
