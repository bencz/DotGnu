/*
 * CID3409.cs - en-PH culture handler.
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

// Generated from "en_PH.txt".

namespace I18N.Common
{

using System;
using System.Globalization;

public class CID3409 : CID0009
{
	public CID3409() : base(0x3409) {}

	public override String Name
	{
		get
		{
			return "en-PH";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ENP";
		}
	}
	public override String Country
	{
		get
		{
			return "PH";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "MMMM d, yyyy";
			dfi.LongTimePattern = "h:mm:ss tt z";
			dfi.ShortDatePattern = "M/d/yy";
			dfi.ShortTimePattern = "h:mm tt";
			dfi.FullDateTimePattern = "dddd, MMMM d, yyyy h:mm:ss tt z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:M/d/yy",
				"D:dddd, MMMM d, yyyy",
				"f:dddd, MMMM d, yyyy h:mm:ss tt z",
				"f:dddd, MMMM d, yyyy h:mm:ss tt z",
				"f:dddd, MMMM d, yyyy h:mm:ss tt",
				"f:dddd, MMMM d, yyyy h:mm tt",
				"F:dddd, MMMM d, yyyy HH:mm:ss",
				"g:M/d/yy h:mm:ss tt z",
				"g:M/d/yy h:mm:ss tt z",
				"g:M/d/yy h:mm:ss tt",
				"g:M/d/yy h:mm tt",
				"G:M/d/yy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:h:mm:ss tt z",
				"t:h:mm:ss tt z",
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

	}; // class PrivateTextInfo

	public override TextInfo TextInfo
	{
		get
		{
			return new PrivateTextInfo(LCID);
		}
	}

}; // class CID3409

public class CNen_ph : CID3409
{
	public CNen_ph() : base() {}

}; // class CNen_ph

}; // namespace I18N.Common
