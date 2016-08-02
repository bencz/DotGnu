/*
 * CID002a.cs - vi culture handler.
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

// Generated from "vi.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID002a : RootCulture
{
	public CID002a() : base(0x002A) {}
	public CID002a(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "vi";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "vie";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "VIT";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "vi";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"Th 2", "Th 3", "Th 4", "Th 5", "Th 6", "Th 7", "CN"};
			dfi.DayNames = new String[] {"Th\u1EE9 hai", "Th\u1EE9 ba", "Th\u1EE9 t\u01B0", "Th\u1EE9 n\u0103m", "Th\u1EE9 s\u00E1u", "Th\u1EE9 b\u1EA3y", "Ch\u1EE7 nh\u1EADt"};
			dfi.AbbreviatedMonthNames = new String[] {"Thg 1", "Thg 2", "Thg 3", "Thg 4", "Thg 5", "Thg 6", "Thg 7", "Thg 8", "Thg 9", "Thg 10", "Thg 11", "Thg 12", ""};
			dfi.MonthNames = new String[] {"Th\u00E1ng m\u1ED9t", "Th\u00E1ng hai", "Th\u00E1ng ba", "Th\u00E1ng t\u01B0", "Th\u00E1ng n\u0103m", "Th\u00E1ng s\u00E1u", "Th\u00E1ng b\u1EA3y", "Th\u00E1ng t\u00E1m", "Th\u00E1ng ch\u00EDn", "Th\u00E1ng m\u01B0\u1EDDi", "Th\u00E1ng m\u01B0\u1EDDi m\u1ED9t", "Th\u00E1ng m\u01B0\u1EDDi hai", ""};
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "ddd dd MMM yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "dd/MM/yy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd dd MMMM yyyy HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:dd/MM/yy",
				"D:dddd dd MMMM yyyy",
				"f:dddd dd MMMM yyyy HH:mm:ss z",
				"f:dddd dd MMMM yyyy HH:mm:ss z",
				"f:dddd dd MMMM yyyy HH:mm:ss",
				"f:dddd dd MMMM yyyy HH:mm",
				"F:dddd dd MMMM yyyy HH:mm:ss",
				"g:dd/MM/yy HH:mm:ss z",
				"g:dd/MM/yy HH:mm:ss z",
				"g:dd/MM/yy HH:mm:ss",
				"g:dd/MM/yy HH:mm",
				"G:dd/MM/yy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:HH:mm:ss z",
				"t:HH:mm:ss z",
				"t:HH:mm:ss",
				"t:HH:mm",
				"T:HH:mm:ss",
				"u:yyyy'-'MM'-'dd HH':'mm':'ss'Z'",
				"U:dddd, dd MMMM yyyy HH:mm:ss",
				"y:yyyy MMMM",
				"Y:yyyy MMMM",
			});
			return dfi;
		}
		set
		{
			base.DateTimeFormat = value; // not used
		}
	}

	public override NumberFormatInfo NumberFormat
	{
		get
		{
			NumberFormatInfo nfi = base.NumberFormat;
			nfi.CurrencyDecimalSeparator = ",";
			nfi.CurrencyGroupSeparator = ".";
			nfi.NumberGroupSeparator = ".";
			nfi.PercentGroupSeparator = ".";
			nfi.NegativeSign = "-";
			nfi.NumberDecimalSeparator = ",";
			nfi.PercentDecimalSeparator = ",";
			nfi.PercentSymbol = "%";
			nfi.PerMilleSymbol = "\u2030";
			return nfi;
		}
		set
		{
			base.NumberFormat = value; // not used
		}
	}

	public override String ResolveLanguage(String name)
	{
		switch(name)
		{
			case "vi": return "Ti\u1EBFng Vi\u1EC7t";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "VN": return "Vi\u1EC7t Nam";
		}
		return base.ResolveCountry(name);
	}

	private class PrivateTextInfo : _I18NTextInfo
	{
		public PrivateTextInfo(int culture) : base(culture) {}

		public override int ANSICodePage
		{
			get
			{
				return 1258;
			}
		}
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
				return 1258;
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

}; // class CID002a

public class CNvi : CID002a
{
	public CNvi() : base() {}

}; // class CNvi

}; // namespace I18N.Other
