/*
 * CID0813.cs - nl-BE culture handler.
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

// Generated from "nl_BE.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0813 : CID0013
{
	public CID0813() : base(0x0813) {}

	public override String Name
	{
		get
		{
			return "nl-BE";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "NLB";
		}
	}
	public override String Country
	{
		get
		{
			return "BE";
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
			dfi.ShortDatePattern = "d/MM/yy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd d MMMM yyyy HH.mm' u. 'z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:d/MM/yy",
				"D:dddd d MMMM yyyy",
				"f:dddd d MMMM yyyy HH.mm' u. 'z",
				"f:dddd d MMMM yyyy HH:mm:ss z",
				"f:dddd d MMMM yyyy HH:mm:ss",
				"f:dddd d MMMM yyyy HH:mm",
				"F:dddd d MMMM yyyy HH:mm:ss",
				"g:d/MM/yy HH.mm' u. 'z",
				"g:d/MM/yy HH:mm:ss z",
				"g:d/MM/yy HH:mm:ss",
				"g:d/MM/yy HH:mm",
				"G:d/MM/yy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:HH.mm' u. 'z",
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

}; // class CID0813

public class CNnl_be : CID0813
{
	public CNnl_be() : base() {}

}; // class CNnl_be

}; // namespace I18N.West
