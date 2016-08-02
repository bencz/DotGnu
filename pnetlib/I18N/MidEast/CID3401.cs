/*
 * CID3401.cs - ar-KW culture handler.
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

// Generated from "ar_KW.txt".

namespace I18N.MidEast
{

using System;
using System.Globalization;
using I18N.Common;

public class CID3401 : CID0001
{
	public CID3401() : base(0x3401) {}

	public override String Name
	{
		get
		{
			return "ar-KW";
		}
	}
	public override String ThreeLetterWindowsLanguageName
	{
		get
		{
			return "ARK";
		}
	}
	public override String Country
	{
		get
		{
			return "KW";
		}
	}

}; // class CID3401

public class CNar_kw : CID3401
{
	public CNar_kw() : base() {}

}; // class CNar_kw

}; // namespace I18N.MidEast
