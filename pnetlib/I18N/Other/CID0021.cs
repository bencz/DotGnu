/*
 * CID0021.cs - id culture handler.
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

// Generated from "id.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0021 : RootCulture
{
	public CID0021() : base(0x0021) {}
	public CID0021(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "id";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "ind";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "IND";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "id";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"Minggu", "Sen", "Sel", "Rabu", "Kamis", "Jumat", "Sabtu"};
			dfi.DayNames = new String[] {"Minggu", "Senin", "Selasa", "Rabu", "Kamis", "Jumat", "Sabtu"};
			dfi.AbbreviatedMonthNames = new String[] {"Jan", "Feb", "Mar", "Apr", "Mei", "Jun", "Jul", "August", "Sep", "Okt", "Nov", "Des", ""};
			dfi.MonthNames = new String[] {"Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli", "Agustus", "September", "Oktober", "November", "Desember", ""};
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

}; // class CID0021

public class CNid : CID0021
{
	public CNid() : base() {}

}; // class CNid

}; // namespace I18N.Other
