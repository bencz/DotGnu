/*
 * CID3801.cs - ar-YE culture handler.
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

// Generated from "ar_YE.txt".

namespace I18N.MidEast
{

using System;
using System.Globalization;
using I18N.Common;

public class CID3801 : CID0001
{
	public CID3801() : base(0x3801) {}

	public override String Name
	{
		get
		{
			return "ar-YE";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ARY";
		}
	}
	public override String Country
	{
		get
		{
			return "YE";
		}
	}

}; // class CID3801

public class CNar_ye : CID3801
{
	public CNar_ye() : base() {}

}; // class CNar_ye

}; // namespace I18N.MidEast
