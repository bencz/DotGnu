/*
 * return1.cs - Test the handling of return value errors.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

abstract class Test
{
	int x;

	Test()
	{
		// ok
	}

	Test(int y)
	{
		// `return' with a value, in method returning void
		return y;
	}

	Test(long y)
	{
		// ok
		return;
	}

	static Test()
	{
		// ok
	}

	static Test()
	{
		// `return' with a value, in method returning void
		return 1;
	}

	static Test()
	{
		// ok
		return;
	}

	int m1()
	{
		// control reaches end of non-void method
	}

	int m2()
	{
		// control reaches end of non-void method
		if(x == 1)
			return 1;
	}

	int m3()
	{
		// ok
		if(x == 1)
			return 1;
		else
			return 0;
	}

	int m4()
	{
	  	// `return' with no value, in method returning non-void
		return;
	}

	void m5()
	{
		// `return' with a value, in method returning void
		return 1;
	}

	void m6()
	{
		// ok
		if(x == 1)
			return;
	}

	void m7()
	{
		// ok
	}

	public abstract void m8(); // ok

	extern static int m9(); // ok

	int m10()
	{
		// invalid return value
		return Test;
	}

	int m11()
	{
		// incompatible types in return
		return 0.0m;
	}
}
