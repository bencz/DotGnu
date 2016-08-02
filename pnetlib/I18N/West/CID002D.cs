/*
 * CID002d.cs - eu culture handler.
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

// Generated from "eu.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID002d : RootCulture
{
	public CID002d() : base(0x002D) {}
	public CID002d(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "eu";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "eus";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "EUQ";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "eu";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"ig", "al", "as", "az", "og", "or", "lr"};
			dfi.DayNames = new String[] {"igandea", "astelehena", "asteartea", "asteazkena", "osteguna", "ostirala", "larunbata"};
			dfi.AbbreviatedMonthNames = new String[] {"urt", "ots", "mar", "api", "mai", "eka", "uzt", "abu", "ira", "urr", "aza", "abe", ""};
			dfi.MonthNames = new String[] {"urtarrila", "otsaila", "martxoa", "apirila", "maiatza", "ekaina", "uztaila", "abuztua", "iraila", "urria", "azaroa", "abendua", ""};
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
			case "eu": return "euskara";
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
		public override int OEMCodePage
		{
			get
			{
				return 850;
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

}; // class CID002d

public class CNeu : CID002d
{
	public CNeu() : base() {}

}; // class CNeu

}; // namespace I18N.West
