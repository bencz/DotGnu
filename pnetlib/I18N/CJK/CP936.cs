/*
 * CP936.cs - Code page handling for Chinese (GB2312).
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

namespace I18N.CJK
{

using System;
using System.Text;
using I18N.Common;

// Note: "gb.table" is generated from "CP936.TXT" (ftp.unicode.org).

public class CP936 : MultiByteEncoding
{
	// Constructor.
	public CP936() : base(936, "gb2312", "Chinese Simplified (GB2312)",
						  "gb2312", "gb2312", 936, "gb.table")
			{
				// Nothing to do here.
			}

}; // class CP936

// Name form of the above class.
public class ENCgb2312 : CP936 {}

}; // namespace I18N.CJK
