/*
 * CID0029.cs - fa culture handler.
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

// Generated from "fa.txt".

namespace I18N.MidEast
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0029 : RootCulture
{
	public CID0029() : base(0x0029) {}
	public CID0029(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "fa";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "fas";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "FAR";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "fa";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "\u0642.\u0638.";
			dfi.PMDesignator = "\u0628.\u0638.";
			dfi.AbbreviatedDayNames = new String[] {"\u06cc.", "\u062f.", "\u0633.", "\u0686.", "\u067e.", "\u062c.", "\u0634."};
			dfi.DayNames = new String[] {"\u06cc\u06a9\u200c\u0634\u0646\u0628\u0647", "\u062f\u0648\u0634\u0646\u0628\u0647", "\u0633\u0647\u200c\u0634\u0646\u0628\u0647", "\u0686\u0647\u0627\u0631\u0634\u0646\u0628\u0647", "\u067e\u0646\u062c\u200c\u0634\u0646\u0628\u0647", "\u062c\u0645\u0639\u0647", "\u0634\u0646\u0628\u0647"};
			dfi.AbbreviatedMonthNames = new String[] {"\u0698\u0627\u0646", "\u0641\u0648\u0631", "\u0645\u0627\u0631", "\u0622\u0648\u0631", "\u0645\u0640\u0647", "\u0698\u0648\u0646", "\u0698\u0648\u06cc", "\u0627\u0648\u062a", "\u0633\u067e\u062a", "\u0627\u06a9\u062a", "\u0646\u0648\u0627", "\u062f\u0633\u0627", ""};
			dfi.MonthNames = new String[] {"\u0698\u0627\u0646\u0648\u06cc\u0647", "\u0641\u0648\u0631\u06cc\u0647", "\u0645\u0627\u0631\u0633", "\u0622\u0648\u0631\u06cc\u0644", "\u0645\u0647", "\u0698\u0648\u0626\u0646", "\u0698\u0648\u0626\u06cc\u0647", "\u0627\u0648\u062a", "\u0633\u067e\u062a\u0627\u0645\u0628\u0631", "\u0627\u06a9\u062a\u0628\u0631", "\u0646\u0648\u0627\u0645\u0628\u0631", "\u062f\u0633\u0627\u0645\u0628\u0631", ""};
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d MMMM yyyy";
			dfi.LongTimePattern = "HH:mm:ss (z)";
			dfi.ShortDatePattern = "yyyy/MM/d";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd\u060c d MMMM yyyy HH:mm:ss (z)";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:yyyy/MM/d",
				"D:dddd\u060c d MMMM yyyy",
				"f:dddd\u060c d MMMM yyyy HH:mm:ss (z)",
				"f:dddd\u060c d MMMM yyyy HH:mm:ss (z)",
				"f:dddd\u060c d MMMM yyyy HH:mm:ss",
				"f:dddd\u060c d MMMM yyyy HH:mm",
				"F:dddd\u060c d MMMM yyyy HH:mm:ss",
				"g:yyyy/MM/d HH:mm:ss (z)",
				"g:yyyy/MM/d HH:mm:ss (z)",
				"g:yyyy/MM/d HH:mm:ss",
				"g:yyyy/MM/d HH:mm",
				"G:yyyy/MM/d HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:HH:mm:ss (z)",
				"t:HH:mm:ss (z)",
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

}; // class CID0029

public class CNfa : CID0029
{
	public CNfa() : base() {}

}; // class CNfa

}; // namespace I18N.MidEast
