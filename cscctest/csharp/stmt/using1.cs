/*
 * using1.cs - Test scoping of using aliases - valid cases.
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

using f = Foo;
using f2 = Foo.Child;
using c = Bar;
using i32 = Int32;

public class Foo 
{
	public static int c; // overrides using c = Bar;
	public static f2 child = new Child(); // check type usage as well
	public static void Override1() 
	{
		c=12;
	}
	public class Child
	{
		public i32 i32 // the dreaded multiple naming
		{
			get
			{
				return 12;
			}
		}
	}
}
class Bar 
{
	public static void NormalCase() 
	{
		for(i32 x=0;x<10;x++);
		int x=f.child.i32;
		f.Override1();
	}
}
