/*
 * CID0809.cs - en-GB culture handler.
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

// Generated from "en_GB.txt".

namespace I18N.Common
{

using System;
using System.Globalization;

public class CID0809 : CID0009
{
	public CID0809() : base(0x0809) {}

	public override String Name
	{
		get
		{
			return "en-GB";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ENG";
		}
	}
	public override String Country
	{
		get
		{
			return "GB";
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
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "dd/MM/yyyy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd, d MMMM yyyy HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:dd/MM/yyyy",
				"D:dddd, d MMMM yyyy",
				"f:dddd, d MMMM yyyy HH:mm:ss z",
				"f:dddd, d MMMM yyyy HH:mm:ss z",
				"f:dddd, d MMMM yyyy HH:mm:ss",
				"f:dddd, d MMMM yyyy HH:mm",
				"F:dddd, d MMMM yyyy HH:mm:ss",
				"g:dd/MM/yyyy HH:mm:ss z",
				"g:dd/MM/yyyy HH:mm:ss z",
				"g:dd/MM/yyyy HH:mm:ss",
				"g:dd/MM/yyyy HH:mm",
				"G:dd/MM/yyyy HH:mm:ss",
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

	private class PrivateTextInfo : _I18NTextInfo
	{
		public PrivateTextInfo(int culture) : base(culture) {}

		public override int EBCDICCodePage
		{
			get
			{
				return 20285;
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

}; // class CID0809

public class CNen_gb : CID0809
{
	public CNen_gb() : base() {}

}; // class CNen_gb

}; // namespace I18N.Common
