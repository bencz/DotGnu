/*
 * CID0056.cs - gl culture handler.
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

// Generated from "gl.txt".

namespace I18N.West
{

using System;
using System.Globalization;
using I18N.Common;

public class CID0056 : RootCulture
{
	public CID0056() : base(0x0056) {}
	public CID0056(int culture) : base(culture) {}

	public override String Name
	{
		get
		{
			return "gl";
		}
	}
	public override String ThreeLetterISOLanguageName
	{
		get
		{
			return "glg";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "GLC";
		}
	}
	public override String TwoLetterISOLanguageName
	{
		get
		{
			return "gl";
		}
	}

	private class PrivateTextInfo : _I18NTextInfo
	{
		public PrivateTextInfo(int culture) : base(culture) {}

		public override int EBCDICCodePage
		{
			get
			{
				return 500;
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

}; // class CID0056

public class CNgl : CID0056
{
	public CNgl() : base() {}

}; // class CNgl

}; // namespace I18N.West
