/*
 * method8.cs - Test method duplicate errors.
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

class Test
{
	static int m1()
	{
		return 3;
	}
	static int m1()		// error
	{
		return 3;
	}
	int m1()			// error
	{
		return 3;
	}

	public static void m2()
	{
	}

	public static void m3()
	{
	}

	public virtual void m4()
	{
	}

	public virtual void m5()
	{
	}

	public virtual void m6()
	{
	}
}

class Test2 : Test
{
	public new void m2()	// ok
	{
	}

	public void m3()		// error
	{
	}
	public void m3(int x)	// ok
	{
	}

	public void m4()		// error
	{
	}

	public override void m5()	// ok
	{
	}
}

class Test3 : Test2
{
	public override void m6()	// ok
	{
	}
}
