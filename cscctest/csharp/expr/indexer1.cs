/*
 * indexer1.cs - Test the valid cases of indexer usage.
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

using System;

public class Dictionary
{
	public Object this[Object key]
	{
		get
		{
			return null;
		}
		set
		{
		}
	}
}

public class Counters
{
	private int[] counters;

	public int this[int index]
	{
		get
		{
			return counters[index];
		}
		set
		{
			counters[index] = value;
		}
	}
}

class Test
{
	Object m1(Dictionary d, Object x)
	{
		d[x] = "Hello World";
		return d[x];
	}

	Object m2(Dictionary d, Object x)
	{
		return (d[x] = "Hello World");
	}

	int m3(Counters c)
	{
		++(c[3]);
		return --(c[3]);
	}
}
