/*
 * CID042d.cs - eu-ES culture handler.
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

// Generated from "eu_ES.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID042d : CID002d
{
	public CID042d() : base(0x042D) {}

	public override String Name
	{
		get
		{
			return "eu-ES";
		}
	}
	public override String Country
	{
		get
		{
			return "ES";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.DateSeparator = "'";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "ddd, yyyy'eko' MMM'ren' dd'tt'";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "yy'-'MM'-'dd";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd, yyyy'eko' MMMM'ren' dd'tt' HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:yy'-'MM'-'dd",
				"D:dddd, yyyy'eko' MMMM'ren' dd'tt'",
				"f:dddd, yyyy'eko' MMMM'ren' dd'tt' HH:mm:ss z",
				"f:dddd, yyyy'eko' MMMM'ren' dd'tt' HH:mm:ss z",
				"f:dddd, yyyy'eko' MMMM'ren' dd'tt' HH:mm:ss",
				"f:dddd, yyyy'eko' MMMM'ren' dd'tt' HH:mm",
				"F:dddd, yyyy'eko' MMMM'ren' dd'tt' HH:mm:ss",
				"g:yy'-'MM'-'dd HH:mm:ss z",
				"g:yy'-'MM'-'dd HH:mm:ss z",
				"g:yy'-'MM'-'dd HH:mm:ss",
				"g:yy'-'MM'-'dd HH:mm",
				"G:yy'-'MM'-'dd HH:mm:ss",
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

}; // class CID042d

public class CNeu_es : CID042d
{
	public CNeu_es() : base() {}

}; // class CNeu_es

}; // namespace I18N.West
