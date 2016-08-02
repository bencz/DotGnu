/*
 * CID0047.cs - gu culture handler.
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

// Generated from "gu.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0047 : RootCulture
{
	public CID0047() : base(0x0047) {}
	public CID0047(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "gu";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "guj";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "GUJ";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "gu";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "\u0AAA\u0AC2\u0AB0\u0ACD\u0AB5\u00A0\u0AAE\u0AA7\u0ACD\u0AAF\u0ABE\u0AB9\u0ACD\u0AA8";
			dfi.PMDesignator = "\u0A89\u0AA4\u0ACD\u0AA4\u0AB0\u00A0\u0AAE\u0AA7\u0ACD\u0AAF\u0ABE\u0AB9\u0ACD\u0AA8";
			dfi.AbbreviatedDayNames = new String[] {"\u0AB0\u0AB5\u0ABF", "\u0AB8\u0ACB\u0AAE", "\u0AAE\u0A82\u0A97\u0AB3", "\u0AAC\u0AC1\u0AA7", "\u0A97\u0AC1\u0AB0\u0AC1", "\u0AB6\u0AC1\u0A95\u0ACD\u0AB0", "\u0AB6\u0AA8\u0ABF"};
			dfi.DayNames = new String[] {"\u0AB0\u0AB5\u0ABF\u0AB5\u0ABE\u0AB0", "\u0AB8\u0ACB\u0AAE\u0AB5\u0ABE\u0AB0", "\u0AAE\u0A82\u0A97\u0AB3\u0AB5\u0ABE\u0AB0", "\u0AAC\u0AC1\u0AA7\u0AB5\u0ABE\u0AB0", "\u0A97\u0AC1\u0AB0\u0AC1\u0AB5\u0ABE\u0AB0", "\u0AB6\u0AC1\u0A95\u0ACD\u0AB0\u0AB5\u0ABE\u0AB0", "\u0AB6\u0AA8\u0ABF\u0AB5\u0ABE\u0AB0"};
			dfi.AbbreviatedMonthNames = new String[] {"\u0A9C\u0ABE\u0AA8\u0ACD\u0AAF\u0AC1", "\u0AAB\u0AC7\u0AAC\u0ACD\u0AB0\u0AC1", "\u0AAE\u0ABE\u0AB0\u0ACD\u0A9A", "\u0A8F\u0AAA\u0ACD\u0AB0\u0ABF\u0AB2", "\u0AAE\u0AC7", "\u0A9C\u0AC2\u0AA8", "\u0A9C\u0AC1\u0AB2\u0ABE\u0A88", "\u0A91\u0A97\u0AB8\u0ACD\u0A9F", "\u0AB8\u0AAA\u0ACD\u0A9F\u0AC7", "\u0A91\u0A95\u0ACD\u0A9F\u0ACB", "\u0AA8\u0AB5\u0AC7", "\u0AA1\u0ABF\u0AB8\u0AC7", ""};
			dfi.MonthNames = new String[] {"\u0A9C\u0ABE\u0AA8\u0ACD\u0AAF\u0AC1\u0A86\u0AB0\u0AC0", "\u0AAB\u0AC7\u0AAC\u0ACD\u0AB0\u0AC1\u0A86\u0AB0\u0AC0", "\u0AAE\u0ABE\u0AB0\u0ACD\u0A9A", "\u0A8F\u0AAA\u0ACD\u0AB0\u0ABF\u0AB2", "\u0AAE\u0AC7", "\u0A9C\u0AC2\u0AA8", "\u0A9C\u0AC1\u0AB2\u0ABE\u0A88", "\u0A91\u0A97\u0AB8\u0ACD\u0A9F", "\u0AB8\u0AAA\u0ACD\u0A9F\u0AC7\u0AAE\u0ACD\u0AAC\u0AB0", "\u0A91\u0A95\u0ACD\u0A9F\u0ACD\u0AAC\u0AB0", "\u0AA8\u0AB5\u0AC7\u0AAE\u0ACD\u0AAC\u0AB0", "\u0AA1\u0ABF\u0AB8\u0AC7\u0AAE\u0ACD\u0AAC\u0AB0", ""};
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
			nfi.CurrencyDecimalSeparator = ".";
			nfi.CurrencyGroupSeparator = ",";
			nfi.NumberGroupSeparator = ",";
			nfi.PercentGroupSeparator = ",";
			nfi.NegativeSign = "-";
			nfi.NumberDecimalSeparator = ".";
			nfi.PercentDecimalSeparator = ".";
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
			case "gu": return "\u0A97\u0AC1\u0A9C\u0AB0\u0ABE\u0AA4\u0AC0";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "IN": return "\u0AAD\u0ABE\u0AB0\u0AA4";
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
				return 0;
			}
		}
		public override int EBCDICCodePage
		{
			get
			{
				return 500;
			}
		}
		public override int MacCodePage
		{
			get
			{
				return 2;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 1;
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

}; // class CID0047

public class CNgu : CID0047
{
	public CNgu() : base() {}

}; // class CNgu

}; // namespace I18N.Other
