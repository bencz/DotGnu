/*
 * CID0001.cs - ar culture handler.
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

// Generated from "ar.txt".

namespace I18N.MidEast
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0001 : RootCulture
{
	public CID0001() : base(0x0001) {}
	public CID0001(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "ar";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "ara";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ARA";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "ar";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "\u0635";
			dfi.PMDesignator = "\u0645";
			dfi.AbbreviatedDayNames = new String[] {"\u062D", "\u0646", "\u062B", "\u0631", "\u062E", "\u062C", "\u0633"};
			dfi.DayNames = new String[] {"\u0627\u0644\u0623\u062D\u062F", "\u0627\u0644\u0627\u062B\u0646\u064A\u0646", "\u0627\u0644\u062B\u0644\u0627\u062B\u0627\u0621", "\u0627\u0644\u0623\u0631\u0628\u0639\u0627\u0621", "\u0627\u0644\u062E\u0645\u064A\u0633", "\u0627\u0644\u062C\u0645\u0639\u0629", "\u0627\u0644\u0633\u0628\u062A"};
			dfi.AbbreviatedMonthNames = new String[] {"\u064A\u0646\u0627", "\u0641\u0628\u0631", "\u0645\u0627\u0631", "\u0623\u0628\u0631", "\u0645\u0627\u064A", "\u064A\u0648\u0646", "\u064A\u0648\u0644", "\u0623\u063A\u0633", "\u0633\u0628\u062A", "\u0623\u0643\u062A", "\u0646\u0648\u0641", "\u062F\u064A\u0633", ""};
			dfi.MonthNames = new String[] {"\u064A\u0646\u0627\u064A\u0631", "\u0641\u0628\u0631\u0627\u064A\u0631", "\u0645\u0627\u0631\u0633", "\u0623\u0628\u0631\u064A\u0644", "\u0645\u0627\u064A\u0648", "\u064A\u0648\u0646\u064A\u0648", "\u064A\u0648\u0644\u064A\u0648", "\u0623\u063A\u0633\u0637\u0633", "\u0633\u0628\u062A\u0645\u0628\u0631", "\u0623\u0643\u062A\u0648\u0628\u0631", "\u0646\u0648\u0641\u0645\u0628\u0631", "\u062F\u064A\u0633\u0645\u0628\u0631", ""};
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d MMMM, yyyy";
			dfi.LongTimePattern = "z h:mm:ss tt";
			dfi.ShortDatePattern = "dd/MM/yy";
			dfi.ShortTimePattern = "h:mm tt";
			dfi.FullDateTimePattern = "dddd, d MMMM, yyyy z h:mm:ss tt";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:dd/MM/yy",
				"D:dddd, d MMMM, yyyy",
				"f:dddd, d MMMM, yyyy z h:mm:ss tt",
				"f:dddd, d MMMM, yyyy z h:mm:ss tt",
				"f:dddd, d MMMM, yyyy h:mm:ss tt",
				"f:dddd, d MMMM, yyyy h:mm tt",
				"F:dddd, d MMMM, yyyy HH:mm:ss",
				"g:dd/MM/yy z h:mm:ss tt",
				"g:dd/MM/yy z h:mm:ss tt",
				"g:dd/MM/yy h:mm:ss tt",
				"g:dd/MM/yy h:mm tt",
				"G:dd/MM/yy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:z h:mm:ss tt",
				"t:z h:mm:ss tt",
				"t:h:mm:ss tt",
				"t:h:mm tt",
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

	public override String ResolveLanguage(String name)
	{
		switch(name)
		{
			case "ar": return "\u0627\u0644\u0639\u0631\u0628\u064A\u0629";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "AE": return "\u0627\u0644\u0625\u0645\u0627\u0631\u0627\u062A";
			case "BH": return "\u0627\u0644\u0628\u062D\u0631\u064A\u0646";
			case "DZ": return "\u0627\u0644\u062C\u0632\u0627\u0626\u0631";
			case "EG": return "\u0645\u0635\u0631";
			case "IQ": return "\u0627\u0644\u0639\u0631\u0627\u0642";
			case "IN": return "\u0627\u0644\u0647\u0646\u062F";
			case "JO": return "\u0627\u0644\u0623\u0631\u062F\u0646";
			case "KW": return "\u0627\u0644\u0643\u0648\u064A\u062A";
			case "LB": return "\u0644\u0628\u0646\u0627\u0646";
			case "LY": return "\u0644\u064A\u0628\u064A\u0627";
			case "MA": return "\u0627\u0644\u0645\u063A\u0631\u0628";
			case "OM": return "\u0633\u0644\u0637\u0646\u0629 \u0639\u0645\u0627\u0646";
			case "QA": return "\u0642\u0637\u0631";
			case "SA": return "\u0627\u0644\u0633\u0639\u0648\u062F\u064A\u0629";
			case "SD": return "\u0627\u0644\u0633\u0648\u062F\u0627\u0646";
			case "SY": return "\u0633\u0648\u0631\u064A\u0627";
			case "TN": return "\u062A\u0648\u0646\u0633";
			case "YE": return "\u0627\u0644\u064A\u0645\u0646";
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
				return 1256;
			}
		}
		public override int EBCDICCodePage
		{
			get
			{
				return 20420;
			}
		}
		public override int MacCodePage
		{
			get
			{
				return 10004;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 720;
			}
		}
		public override String ListSeparator
		{
			get
			{
				return ";";
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

}; // class CID0001

public class CNar : CID0001
{
	public CNar() : base() {}

}; // class CNar

}; // namespace I18N.MidEast
