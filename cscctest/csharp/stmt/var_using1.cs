/*
 * var_using1.cs - Test using statements using the var type
 *
 * Copyright (C) 2009  Southern Storm Software, Pty Ltd.
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

namespace Test1
{
	using System;

	public class var : IDisposable
	{
		public void Dispose()
		{
		}
	}

	public class TestVar : var
	{
	}
}

namespace Test
{
	using System;

	class TestDisposable : IDisposable
	{
		public int i = 1;

		public void Dispose()
		{
		}
	}

	class TestDisposable1 : IDisposable
	{
		public int i = 2;

		public void Dispose()
		{
		}
	}

	class Test1
	{
		static void t1()
		{
			using(var a = new TestDisposable(), b = new TestDisposable1())
			{
				a.i += b.i;
			}
		}
	}
}

namespace Test
{
	using Test1;

	class Test2
	{
		static void t1()
		{
			using(var a = new TestVar())
			{
			}
		}
	}

}