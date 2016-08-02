/*
 * CID100a.cs - es-GT culture handler.
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

// Generated from "es_GT.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID100a : CID000a
{
	public CID100a() : base(0x100A) {}

	public override String Name
	{
		get
		{
			return "es-GT";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ESG";
		}
	}
	public override String Country
	{
		get
		{
			return "GT";
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
			dfi.LongTimePattern = "hh:mm:ss tt z";
			dfi.ShortDatePattern = "d/MM/yy";
			dfi.ShortTimePattern = "hh:mm tt";
			dfi.FullDateTimePattern = "dddd d' de 'MMMM' de 'yyyy hh:mm:ss tt z";
			dfi.I18NSetDateTimePatterns(new String[] {
				"d:d/MM/yy",
				"D:dddd d' de 'MMMM' de 'yyyy",
				"f:dddd d' de 'MMMM' de 'yyyy hh:mm:ss tt z",
				"f:dddd d' de 'MMMM' de 'yyyy hh:mm:ss tt z",
				"f:dddd d' de 'MMMM' de 'yyyy hh:mm:ss tt",
				"f:dddd d' de 'MMMM' de 'yyyy hh:mm tt",
				"F:dddd d' de 'MMMM' de 'yyyy HH:mm:ss",
				"g:d/MM/yy hh:mm:ss tt z",
				"g:d/MM/yy hh:mm:ss tt z",
				"g:d/MM/yy hh:mm:ss tt",
				"g:d/MM/yy hh:mm tt",
				"G:d/MM/yy HH:mm:ss",
				"m:MMMM dd",
				"M:MMMM dd",
				"r:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"R:ddd, dd MMM yyyy HH':'mm':'ss 'GMT'",
				"s:yyyy'-'MM'-'dd'T'HH':'mm':'ss",
				"t:hh:mm:ss tt z",
				"t:hh:mm:ss tt z",
				"t:hh:mm:ss tt",
				"t:hh:mm tt",
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

}; // class CID100a

public class CNes_gt : CID100a
{
	public CNes_gt() : base() {}

}; // class CNes_gt

}; // namespace I18N.West
