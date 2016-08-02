/*
 * foreach4.cs - Test the handling of "foreach" statements for Collections.
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

namespace System.Collections
{

public interface IEnumerator
{
	bool MoveNext();
	void Reset();
	Object Current { get; }
}

public interface IEnumerable
{
	IEnumerator GetEnumerator();
}
}
using System.Collections;
class TestEnumerator: IEnumerator
{
	bool IEnumerator.MoveNext()
	{
		return false;
	}
	public bool MoveNext()
	{
		return false;
	}
	void IEnumerator.Reset()
	{
	}
	public void Reset()
	{
	}
	Object IEnumerator.Current
	{
		get
		{
			return 0;
		}
	}
	public byte Current
	{
		get
		{
			return 0;
		}
	}
}

class TestEnumerable: IEnumerable
{
	IEnumerator IEnumerable.GetEnumerator()
	{
		return null;		
	}
	public TestEnumerator GetEnumerator()
	{
		return null;		
	}
}

class Test
{
	int m1(TestEnumerable en)
	{
		String y="";
		foreach(char x in en)
		{
			y=y+x;
		}
		return 0;
	}
}
