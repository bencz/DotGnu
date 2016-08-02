/*
 * var_locals1.cs - Test the allocating of local variables using the var type
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
	public class var
	{
	}
}

namespace Test
{
	class Test1
	{
		static int i1()
		{
			var y = 10;

			return y;
		}

		static float f1()
		{
			var y = 1.0f;

			return y;
		}

		static double d1()
		{
			var y = 1.0d;

			return y;
		}
	}
}

namespace Test
{
	using Test1;

	class Test2
	{
		static var v1()
		{
			var y;

			y = new var();
			return y;
		}
	}

}