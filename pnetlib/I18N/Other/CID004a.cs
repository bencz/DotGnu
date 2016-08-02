/*
 * CID004a.cs - te culture handler.
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

// Generated from "te.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID004a : RootCulture
{
	public CID004a() : base(0x004A) {}
	public CID004a(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "te";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "tel";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "TEL";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "te";
		}
	}

	public override DateTimeFormatInfo DateTimeFormat
	{
		get
		{
			DateTimeFormatInfo dfi = base.DateTimeFormat;
			dfi.AbbreviatedDayNames = new String[] {"\u0c06\u0c26\u0c3f", "\u0c38\u0c4b\u0c2e", "\u0c2e\u0c02\u0c17\u0c33", "\u0c2c\u0c41\u0c27", "\u0c17\u0c41\u0c30\u0c41", "\u0c36\u0c41\u0c15\u0c4d\u0c30", "\u0c36\u0c28\u0c3f"};
			dfi.DayNames = new String[] {"\u0c06\u0c26\u0c3f\u0c35\u0c3e\u0c30\u0c02", "\u0c38\u0c4b\u0c2e\u0c35\u0c3e\u0c30\u0c02", "\u0c2e\u0c02\u0c17\u0c33\u0c35\u0c3e\u0c30\u0c02", "\u0c2c\u0c41\u0c27\u0c35\u0c3e\u0c30\u0c02", "\u0c17\u0c41\u0c30\u0c41\u0c35\u0c3e\u0c30\u0c02", "\u0c36\u0c41\u0c15\u0c4d\u0c30\u0c35\u0c3e\u0c30\u0c02", "\u0c36\u0c28\u0c3f\u0c35\u0c3e\u0c30\u0c02"};
			dfi.AbbreviatedMonthNames = new String[] {"\u0c1c\u0c28\u0c35\u0c30\u0c3f", "\u0c2b\u0c3f\u0c2c\u0c4d\u0c30\u0c35\u0c30\u0c3f", "\u0c2e\u0c3e\u0c30\u0c4d\u0c1a\u0c3f", "\u0c0f\u0c2a\u0c4d\u0c30\u0c3f\u0c32\u0c4d", "\u0c2e\u0c47", "\u0c1c\u0c42\u0c28\u0c4d", "\u0c1c\u0c42\u0c32\u0c48", "\u0c06\u0c17\u0c38\u0c4d\u0c1f\u0c41", "\u0c38\u0c46\u0c2a\u0c4d\u0c1f\u0c46\u0c02\u0c2c\u0c30\u0c4d", "\u0c05\u0c15\u0c4d\u0c1f\u0c4b\u0c2c\u0c30\u0c4d", "\u0c28\u0c35\u0c02\u0c2c\u0c30\u0c4d", "\u0c21\u0c3f\u0c38\u0c46\u0c02\u0c2c\u0c30\u0c4d", ""};
			dfi.MonthNames = new String[] {"\u0c1c\u0c28\u0c35\u0c30\u0c3f", "\u0c2b\u0c3f\u0c2c\u0c4d\u0c30\u0c35\u0c30\u0c3f", "\u0c2e\u0c3e\u0c30\u0c4d\u0c1a\u0c3f", "\u0c0f\u0c2a\u0c4d\u0c30\u0c3f\u0c32\u0c4d", "\u0c2e\u0c47", "\u0c1c\u0c42\u0c28\u0c4d", "\u0c1c\u0c42\u0c32\u0c48", "\u0c06\u0c17\u0c38\u0c4d\u0c1f\u0c41", "\u0c38\u0c46\u0c2a\u0c4d\u0c1f\u0c46\u0c02\u0c2c\u0c30\u0c4d", "\u0c05\u0c15\u0c4d\u0c1f\u0c4b\u0c2c\u0c30\u0c4d", "\u0c28\u0c35\u0c02\u0c2c\u0c30\u0c4d", "\u0c21\u0c3f\u0c38\u0c46\u0c02\u0c2c\u0c30\u0c4d", ""};
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
			case "te": return "\u0c24\u0c46\u0c32\u0c41\u0c17\u0c41";
		}
		return base.ResolveLanguage(name);
	}

	public override String ResolveCountry(String name)
	{
		switch(name)
		{
			case "IN": return "\u0c2d\u0c3e\u0c30\u0c24 \u0c26\u0c46\u0c33\u0c66";
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

}; // class CID004a

public class CNte : CID004a
{
	public CNte() : base() {}

}; // class CNte

}; // namespace I18N.Other
