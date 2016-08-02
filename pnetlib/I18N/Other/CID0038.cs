/*
 * CID0038.cs - fo culture handler.
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

// Generated from "fo.txt".

namespace I18N.Other
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0038 : RootCulture
{
	public CID0038() : base(0x0038) {}
	public CID0038(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "fo";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "fao";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "FOS";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "fo";
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
		public override int MacCodePage
		{
			get
			{
				return 10079;
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

}; // class CID0038

public class CNfo : CID0038
{
	public CNfo() : base() {}

}; // class CNfo

}; // namespace I18N.Other
