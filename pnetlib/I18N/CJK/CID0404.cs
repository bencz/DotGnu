/*
 * CID0404.cs - zh-TW culture handler.
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

// Generated from "zh_TW.txt".

namespace I18N.CJK
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0404 : CID0004
{
	public CID0404() : base(0x0404) {}

	public override String Name
	{
		get
		{
			return "zh-TW";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "CHT";
		}
	}
	public override String Country
	{
		get
		{
			return "TW";
		}
	}

	private class PrivateTextInfo : _I18NTextInfo
	{
		public PrivateTextInfo(int culture) : base(culture) {}

		public override int ANSICodePage
		{
			get
			{
				return 950;
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
				return 10002;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 950;
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

}; // class CID0404

public class CNzh_tw : CID0404
{
	public CNzh_tw() : base() {}

}; // class CNzh_tw

public class CNzh_cht : CID0404
{
	public CNzh_cht() : base() {}

}; // class CNzh_cht

}; // namespace I18N.CJK
