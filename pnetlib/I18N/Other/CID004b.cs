/*
 * CID004b.cs - kn culture handler.
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

// Generated from "kn.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID004b : RootCulture
{
	public CID004b() : base(0x004B) {}
	public CID004b(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "kn";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "kan";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "KAN";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "kn";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AMDesignator = "\u0CAA\u0CC2\u0CB0\u0CCD\u0CB5\u0CBE\u0CB9\u0CCD\u0CA8";
			dfi.PMDesignator = "\u0C85\u0CAA\u0CB0\u0CBE\u0CB9\u0CCD\u0CA8";
			dfi.AbbreviatedDayNames = new String[] {"\u0CB0.", "\u0CB8\u0CCB.", "\u0CAE\u0C82.", "\u0CAC\u0CC1.", "\u0C97\u0CC1.", "\u0CB6\u0CC1.", "\u0CB6\u0CA8\u0CBF."};
			dfi.DayNames = new String[] {"\u0CB0\u0CB5\u0CBF\u0CB5\u0CBE\u0CB0", "\u0CB8\u0CCB\u0CAE\u0CB5\u0CBE\u0CB0", "\u0CAE\u0C82\u0C97\u0CB3\u0CB5\u0CBE\u0CB0", "\u0CAC\u0CC1\u0CA7\u0CB5\u0CBE\u0CB0", "\u0C97\u0CC1\u0CB0\u0CC1\u0CB5\u0CBE\u0CB0", "\u0CB6\u0CC1\u0C95\u0CCD\u0CB0\u0CB5\u0CBE\u0CB0", "\u0CB6\u0CA8\u0CBF\u0CB5\u0CBE\u0CB0"};
			dfi.AbbreviatedMonthNames = new String[] {"\u0C9C\u0CA8\u0CB5\u0CB0\u0CC0", "\u0CAB\u0CC6\u0CAC\u0CCD\u0CB0\u0CB5\u0CB0\u0CC0", "\u0CAE\u0CBE\u0CB0\u0CCD\u0C9A\u0CCD", "\u0C8E\u0CAA\u0CCD\u0CB0\u0CBF\u0CB2\u0CCD", "\u0CAE\u0CC6", "\u0C9C\u0CC2\u0CA8\u0CCD", "\u0C9C\u0CC1\u0CB2\u0CC8", "\u0C86\u0C97\u0CB8\u0CCD\u0C9F\u0CCD", "\u0CB8\u0CAA\u0CCD\u0C9F\u0CC6\u0C82\u0CAC\u0CB0\u0CCD", "\u0C85\u0C95\u0CCD\u0C9F\u0CCB\u0CAC\u0CB0\u0CCD", "\u0CA8\u0CB5\u0CC6\u0C82\u0CAC\u0CB0\u0CCD", "\u0CA1\u0CBF\u0CB8\u0CC6\u0C82\u0CAC\u0CB0\u0CCD", ""};
			dfi.MonthNames = new String[] {"\u0C9C\u0CA8\u0CB5\u0CB0\u0CC0", "\u0CAB\u0CC6\u0CAC\u0CCD\u0CB0\u0CB5\u0CB0\u0CC0", "\u0CAE\u0CBE\u0CB0\u0CCD\u0C9A\u0CCD", "\u0C8E\u0CAA\u0CCD\u0CB0\u0CBF\u0CB2\u0CCD", "\u0CAE\u0CC6", "\u0C9C\u0CC2\u0CA8\u0CCD", "\u0C9C\u0CC1\u0CB2\u0CC8", "\u0C86\u0C97\u0CB8\u0CCD\u0C9F\u0CCD", "\u0CB8\u0CAA\u0CCD\u0C9F\u0CC6\u0C82\u0CAC\u0CB0\u0CCD", "\u0C85\u0C95\u0CCD\u0C9F\u0CCB\u0CAC\u0CB0\u0CCD", "\u0CA8\u0CB5\u0CC6\u0C82\u0CAC\u0CB0\u0CCD", "\u0CA1\u0CBF\u0CB8\u0CC6\u0C82\u0CAC\u0CB0\u0CCD", ""};
			return dfi;
		}
		set
		{
			base.DateTimeFormat = value; // not used
		}
	}

	public override String ResolveLanguage(String name)
	{
		switch(name)
		{
			case "kn": return "\u0c95\u0ca8\u0ccd\u0ca8\u0ca1";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "IN": return "\u0cad\u0cbe\u0cb0\u0ca4";
		}
		return base.ResolveCountry(name);
	}

	private class PrivateTextInfo : _I18NTextInfo
	{
		public PrivateTextInfo(int culture) : base(culture) {}

		public override int ANSICodePage
		{
			get
			{
				return 0;
			}
		}
		public override int EBCDICCodePage
		{
			get
			{
				return 500;
			}
		}
		public override int MacCodePage
		{
			get
			{
				return 2;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 1;
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

}; // class CID004b

public class CNkn : CID004b
{
	public CNkn() : base() {}

}; // class CNkn

}; // namespace I18N.Other
