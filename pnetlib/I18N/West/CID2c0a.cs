/*
 * CID2c0a.cs - es-AR culture handler.
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

// Generated from "es_AR.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID2c0a : CID000a
{
	public CID2c0a() : base(0x2C0A) {}

	public override String Name
	{
		get
		{
			return "es-AR";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ESS";
		}
	}
	public override String Country
	{
		get
		{
			return "AR";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d' de 'MMMM' de 'yyyy";
			dfi.LongTimePattern = "H:mm:ss z";
			dfi.ShortDatePattern = "dd/MM/yy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd d' de 'MMMM' de 'yyyy HH'h'''mm z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:dd/MM/yy",
				"D:dddd d' de 'MMMM' de 'yyyy",
				"f:dddd d' de 'MMMM' de 'yyyy HH'h'''mm z",
				"f:dddd d' de 'MMMM' de 'yyyy H:mm:ss z",
				"f:dddd d' de 'MMMM' de 'yyyy HH:mm:ss",
				"f:dddd d' de 'MMMM' de 'yyyy HH:mm",
				"F:dddd d' de 'MMMM' de 'yyyy HH:mm:ss",
				"g:dd/MM/yy HH'h'''mm z",
				"g:dd/MM/yy H:mm:ss z",
				"g:dd/MM/yy HH:mm:ss",
				"g:dd/MM/yy HH:mm",
				"G:dd/MM/yy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:HH'h'''mm z",
				"t:H:mm:ss z",
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

	private class PrivateTextInfo : _I18NTextInfo
	{
		public PrivateTextInfo(int culture) : base(culture) {}

		public override int EBCDICCodePage
		{
			get
			{
				return 20284;
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

}; // class CID2c0a

public class CNes_ar : CID2c0a
{
	public CNes_ar() : base() {}

}; // class CNes_ar

}; // namespace I18N.West
