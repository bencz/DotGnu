/*
 * relop3.cs - Test relational operators on strings and objects.
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
	void m1()
	{
		string s1 = "Hello";
		string s2 = "Hello";
		object o1 = null;
		object o2 = null;
		bool bl;

		// Equality testing.
		bl = (s1 == s2);
		bl = (s1 == null);
		bl = (null == s1);
		bl = (o1 == o2);
		bl = (o1 == null);
		bl = (null == o1);

		// Inequality testing.
		bl = (s1 != s2);
		bl = (s1 != null);
		bl = (null != s1);
		bl = (o1 != o2);
		bl = (o1 != null);
		bl = (null != o1);

		// If equality testing.
		if(s1 == s2) bl = true; else bl = false;
		if(s1 == null) bl = true; else bl = false;
		if(null == s1) bl = true; else bl = false;
		if(o1 == o2) bl = true; else bl = false;
		if(o1 == null) bl = true; else bl = false;
		if(null == o1) bl = true; else bl = false;

		// If inequality testing.
		if(s1 != s2) bl = true; else bl = false;
		if(s1 != null) bl = true; else bl = false;
		if(null != s1) bl = true; else bl = false;
		if(o1 != o2) bl = true; else bl = false;
		if(o1 != null) bl = true; else bl = false;
		if(null != o1) bl = true; else bl = false;
	}
}
