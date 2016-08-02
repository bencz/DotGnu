/*
 * CID1004.cs - zh-SG culture handler.
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

// Generated from "zh_SG.txt".

namespace I18N.CJK
{

using System;
using System.Globalization;
using I18N.Common;

public class CID1004 : CID0004
{
	public CID1004() : base(0x1004) {}

	public override String Name
	{
		get
		{
			return "zh-SG";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ZHI";
		}
	}
	public override String Country
	{
		get
		{
			return "SG";
		}
	}

	private class PrivateTextInfo : _I18NTextInfo
	{
		public PrivateTextInfo(int culture) : base(culture) {}

		public override int ANSICodePage
		{
			get
			{
				return 936;
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
				return 10008;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 936;
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

}; // class CID1004

public class CNzh_sg : CID1004
{
	public CNzh_sg() : base() {}

}; // class CNzh_sg

}; // namespace I18N.CJK
