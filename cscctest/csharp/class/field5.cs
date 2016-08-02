/*
 * field5.cs - Test forward access of constants.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System2
{
using System;

public class Uri
{
	public static UriHostNameType CheckHostName(String name)
	{
		if (name == null)
		{
			return UriHostNameType.Unknown;
		}
		return UriHostNameType.IPv4;
	}
}

public enum UriHostNameType
{
	Dns = 2,
	IPv4 = 3,
	IPv6 = 4,
	Unknown = 0
}

}
