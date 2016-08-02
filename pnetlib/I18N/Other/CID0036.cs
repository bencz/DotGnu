/*
 * CID0036.cs - af culture handler.
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

// Generated from "af.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0036 : RootCulture
{
	public CID0036() : base(0x0036) {}
	public CID0036(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "af";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "afr";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "AFK";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "af";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "VM";
			dfi.PMDesignator = "NM";
			dfi.AbbreviatedDayNames = new String[] {"So", "Ma", "Di", "Wo", "Do", "Vr", "Sa"};
			dfi.DayNames = new String[] {"Sondag", "Maandag", "Dinsdag", "Woensdag", "Donderdag", "Vrydag", "Saterdag"};
			dfi.AbbreviatedMonthNames = new String[] {"Jan", "Feb", "Mar", "Apr", "Mei", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Des", ""};
			dfi.MonthNames = new String[] {"Januarie", "Februarie", "Maart", "April", "Mei", "Junie", "Julie", "Augustus", "September", "Oktober", "November", "Desember", ""};
			return dfi;
		}
		set
		{
			base.DateTimeFormat = value; // not used
		}
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
		public override int OEMCodePage
		{
			get
			{
				return 850;
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

}; // class CID0036

public class CNaf : CID0036
{
	public CNaf() : base() {}

}; // class CNaf

}; // namespace I18N.Other
