/*
 * CID0014.cs - no culture handler.
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

// Generated from "no.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0014 : RootCulture
{
	public CID0014() : base(0x0014) {}
	public CID0014(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "no";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "nor";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "NOR";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "no";
		}
	}

	private class PrivateTextInfo : _I18NTextInfo
	{
		public PrivateTextInfo(int culture) : base(culture) {}

		public override int EBCDICCodePage
		{
			get
			{
				return 20277;
			}
		}
		public override int OEMCodePage
		{
			get
			{
				return 850;
			}
		}
		public override String ListSeparator
		{
			get
			{
				return ";";
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

}; // class CID0014

public class CNno : CID0014
{
	public CNno() : base() {}

}; // class CNno

public class CNno_no : CID0014
{
	public CNno_no() : base() {}

}; // class CNno_no

}; // namespace I18N.West
