/*
 * CID1409.cs - en-NZ culture handler.
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

// Generated from "en_NZ.txt".

namespace I18N.Common
{

using System;
using System.Globalization;

public class CID1409 : CID0009
{
	public CID1409() : base(0x1409) {}

	public override String Name
	{
		get
		{
			return "en-NZ";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ENZ";
		}
	}
	public override String Country
	{
		get
		{
			return "NZ";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "d MMMM yyyy";
			dfi.LongTimePattern = "h:mm:ss tt";
			dfi.ShortDatePattern = "d/MM/yy";
			dfi.ShortTimePattern = "h:mm tt";
			dfi.FullDateTimePattern = "dddd, d MMMM yyyy h:mm:ss tt z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:d/MM/yy",
				"D:dddd, d MMMM yyyy",
				"f:dddd, d MMMM yyyy h:mm:ss tt z",
				"f:dddd, d MMMM yyyy h:mm:ss tt",
				"f:dddd, d MMMM yyyy h:mm:ss tt",
				"f:dddd, d MMMM yyyy h:mm tt",
				"F:dddd, d MMMM yyyy HH:mm:ss",
				"g:d/MM/yy h:mm:ss tt z",
				"g:d/MM/yy h:mm:ss tt",
				"g:d/MM/yy h:mm:ss tt",
				"g:d/MM/yy h:mm tt",
				"G:d/MM/yy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:h:mm:ss tt z",
				"t:h:mm:ss tt",
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

}; // class CID1409

public class CNen_nz : CID1409
{
	public CNen_nz() : base() {}

}; // class CNen_nz

}; // namespace I18N.Common
