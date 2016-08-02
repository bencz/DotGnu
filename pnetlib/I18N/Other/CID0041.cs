/*
 * CID0041.cs - sw culture handler.
 *
 * Copyright (c) 2003  Southern Storm Software, Pty Ltd
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

// Generated from "sw.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0041 : RootCulture
{
	public CID0041() : base(0x0041) {}
	public CID0041(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "sw";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "swa";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "SWK";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "sw";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"Jpi", "Jtt", "Jnn", "Jtn", "Alh", "Iju", "Jmo"};
			dfi.DayNames = new String[] {"Jumapili", "Jumatatu", "Jumanne", "Jumatano", "Alhamisi", "Ijumaa", "Jumamosi"};
			dfi.AbbreviatedMonthNames = new String[] {"Jan", "Feb", "Mar", "Apr", "Mei", "Jun", "Jul", "Ago", "Sep", "Okt", "Nov", "Des", ""};
			dfi.MonthNames = new String[] {"Januari", "Februari", "Machi", "Aprili", "Mei", "Juni", "Julai", "Agosti", "Septemba", "Oktoba", "Novemba", "Desemba", ""};
			return dfi;
		}
		set
		{
			base.DateTimeFormat = value; // not used
		}
	}

	public override String ResolveLanguage(String name)
	{
		switch(name)
		{
			case "sw": return "Kiswahili";
		}
		return base.ResolveLanguage(name);
	}

	private class PrivateTextInfo : _I18NTextInfo
	{
		public PrivateTextInfo(int culture) : base(culture) {}

		public override int EBCDICCodePage
		{
			get
			{
				return 500;
			}
		}

	}; // class PrivateTextInfo

	public override TextInfo TextInfo
	{
		get
		{
			return new PrivateTextInfo(LCID);
		}
	}

}; // class CID0041

public class CNsw : CID0041
{
	public CNsw() : base() {}

}; // class CNsw

}; // namespace I18N.Other
