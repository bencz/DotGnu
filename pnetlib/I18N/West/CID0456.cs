/*
 * CID0456.cs - gl-ES culture handler.
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

// Generated from "gl_ES.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0456 : CID0056
{
	public CID0456() : base(0x0456) {}

	public override String Name
	{
		get
		{
			return "gl-ES";
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
			dfi.DateSeparator = "/";
			dfi.TimeSeparator = ":";
			dfi.LongDatePattern = "dd MMMM yyyy";
			dfi.LongTimePattern = "HH:mm:ss z";
			dfi.ShortDatePattern = "dd/MM/yy";
			dfi.ShortTimePattern = "HH:mm";
			dfi.FullDateTimePattern = "dddd dd MMMM yyyy HH:mm:ss z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:dd/MM/yy",
				"D:dddd dd MMMM yyyy",
				"f:dddd dd MMMM yyyy HH:mm:ss z",
				"f:dddd dd MMMM yyyy HH:mm:ss z",
				"f:dddd dd MMMM yyyy HH:mm:ss",
				"f:dddd dd MMMM yyyy HH:mm",
				"F:dddd dd MMMM yyyy HH:mm:ss",
				"g:dd/MM/yy HH:mm:ss z",
				"g:dd/MM/yy HH:mm:ss z",
				"g:dd/MM/yy HH:mm:ss",
				"g:dd/MM/yy HH:mm",
				"G:dd/MM/yy HH:mm:ss",
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

}; // class CID0456

public class CNgl_es : CID0456
{
	public CNgl_es() : base() {}

}; // class CNgl_es

}; // namespace I18N.West
