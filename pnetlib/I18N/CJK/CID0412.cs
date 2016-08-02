/*
 * CID0412.cs - ko-KR culture handler.
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

// Generated from "ko_KR.txt".

namespace I18N.CJK
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0412 : CID0012
{
	public CID0412() : base(0x0412) {}

	public override String Name
	{
		get
		{
			return "ko-KR";
		}
	}
	public override String Country
	{
		get
		{
			return "KR";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.DateSeparator = ".";
			dfi.TimeSeparator = "'";
			dfi.LongDatePattern = "yyyy'\uB144' M'\uC6D4' d'\uC77C' dd";
			dfi.LongTimePattern = "tt h'\uC2DC' mm'\uBD84' ss'\uCD08'";
			dfi.ShortDatePattern = "yy.MM.dd";
			dfi.ShortTimePattern = "tt h'\uC2Dc' mm'\uBD84'";
			dfi.FullDateTimePattern = "yyyy'\uB144' M'\uC6D4' d'\uC77C' dddd tt h'\uC2DC' mm'\uBD84' ss'\uCD08' z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:yy.MM.dd",
				"D:yyyy'\uB144' M'\uC6D4' d'\uC77C' dddd",
				"f:yyyy'\uB144' M'\uC6D4' d'\uC77C' dddd tt h'\uC2DC' mm'\uBD84' ss'\uCD08' z",
				"f:yyyy'\uB144' M'\uC6D4' d'\uC77C' dddd tt h'\uC2DC' mm'\uBD84' ss'\uCD08'",
				"f:yyyy'\uB144' M'\uC6D4' d'\uC77C' dddd tt h:mm:ss",
				"f:yyyy'\uB144' M'\uC6D4' d'\uC77C' dddd tt h'\uC2Dc' mm'\uBD84'",
				"F:yyyy'\uB144' M'\uC6D4' d'\uC77C' dddd HH'mm'ss",
				"g:yy.MM.dd tt h'\uC2DC' mm'\uBD84' ss'\uCD08' z",
				"g:yy.MM.dd tt h'\uC2DC' mm'\uBD84' ss'\uCD08'",
				"g:yy.MM.dd tt h:mm:ss",
				"g:yy.MM.dd tt h'\uC2Dc' mm'\uBD84'",
				"G:yy.MM.dd HH'mm'ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:tt h'\uC2DC' mm'\uBD84' ss'\uCD08' z",
				"t:tt h'\uC2DC' mm'\uBD84' ss'\uCD08'",
				"t:tt h:mm:ss",
				"t:tt h'\uC2Dc' mm'\uBD84'",
				"T:HH'mm'ss",
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

}; // class CID0412

public class CNko_kr : CID0412
{
	public CNko_kr() : base() {}

}; // class CNko_kr

}; // namespace I18N.CJK
