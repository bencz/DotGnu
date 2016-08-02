/*
 * relop1.cs - Test relational operators.
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

class Test
{
	void m1()
	{
		bool bl;
		sbyte sb1 = 34;
		byte b1 = 42;
		short s1 = -126;
		ushort us1 = 67;
		int i1 = -1234;
		uint ui1 = 54321;
		long l1 = -9876543210;
		ulong ul1 = 9876543210;
		char c1 = 'A';
		float f1 = 1.5f;
		double d1 = 6.7d;
		decimal dc1 = 3.5m;
		object o1 = null;
		sbyte sb2 = 34;
		byte b2 = 42;
		short s2 = -126;
		ushort us2 = 67;
		int i2 = -1234;
		uint ui2 = 54321;
		long l2 = -9876543210;
		ulong ul2 = 9876543210;
		char c2 = 'A';
		float f2 = 1.5f;
		double d2 = 6.7d;
		decimal dc2 = 3.5m;
		object o2 = null;
		
		// Value code generation

		// sbyte op ???
		bl = (sb1 == sb2);
		bl = (sb1 == b2);
		bl = (sb1 == s2);
		bl = (sb1 == us2);
		bl = (sb1 == i2);
		bl = (sb1 == ui2);
		bl = (sb1 == l2);
		bl = (sb1 == c2);
		bl = (sb1 == f2);
		bl = (sb1 == d2);
		bl = (sb1 == dc2);

		// byte op ???
		bl = (b1 != sb2);
		bl = (b1 != b2);
		bl = (b1 != s2);
		bl = (b1 != us2);
		bl = (b1 != i2);
		bl = (b1 != ui2);
		bl = (b1 != l2);
		bl = (b1 != ul2);
		bl = (b1 != c2);
		bl = (b1 != f2);
		bl = (b1 != d2);
		bl = (b1 != dc2);

		// short op ???
		bl = (s1 < sb2);
		bl = (s1 < b2);
		bl = (s1 < s2);
		bl = (s1 < us2);
		bl = (s1 < i2);
		bl = (s1 < ui2);
		bl = (s1 < l2);
		bl = (s1 < c2);
		bl = (s1 < f2);
		bl = (s1 < d2);
		bl = (s1 < dc2);

		// ushort op ???
		bl = (us1 <= sb2);
		bl = (us1 <= b2);
		bl = (us1 <= s2);
		bl = (us1 <= us2);
		bl = (us1 <= i2);
		bl = (us1 <= ui2);
		bl = (us1 <= l2);
		bl = (us1 <= ul2);
		bl = (us1 <= c2);
		bl = (us1 <= f2);
		bl = (us1 <= d2);
		bl = (us1 <= dc2);

		// int op ???
		bl = (i1 > sb2);
		bl = (i1 > b2);
		bl = (i1 > s2);
		bl = (i1 > us2);
		bl = (i1 > i2);
		bl = (i1 > ui2);
		bl = (i1 > l2);
		bl = (i1 > c2);
		bl = (i1 > f2);
		bl = (i1 > d2);
		bl = (i1 > dc2);

		// uint op ???
		bl = (ui1 >= sb2);
		bl = (ui1 >= b2);
		bl = (ui1 >= s2);
		bl = (ui1 >= us2);
		bl = (ui1 >= i2);
		bl = (ui1 >= ui2);
		bl = (ui1 >= l2);
		bl = (ui1 >= ul2);
		bl = (ui1 >= c2);
		bl = (ui1 >= f2);
		bl = (ui1 >= d2);
		bl = (ui1 >= dc2);

		// long op ???
		bl = (l1 == sb2);
		bl = (l1 == b2);
		bl = (l1 == s2);
		bl = (l1 == us2);
		bl = (l1 == i2);
		bl = (l1 == ui2);
		bl = (l1 == l2);
		bl = (l1 == c2);
		bl = (l1 == f2);
		bl = (l1 == d2);
		bl = (l1 == dc2);

		// ulong op ???
		bl = (ul1 != b2);
		bl = (ul1 != us2);
		bl = (ul1 != ui2);
		bl = (ul1 != ul2);
		bl = (ul1 != c2);
		bl = (ul1 != f2);
		bl = (ul1 != d2);
		bl = (ul1 != dc2);

		// float op ???
		bl = (f1 < sb2);
		bl = (f1 < b2);
		bl = (f1 < s2);
		bl = (f1 < us2);
		bl = (f1 < i2);
		bl = (f1 < ui2);
		bl = (f1 < l2);
		bl = (f1 < ul2);
		bl = (f1 < c2);
		bl = (f1 < f2);
		bl = (f1 < d2);

		// double op ???
		bl = (d1 <= sb2);
		bl = (d1 <= b2);
		bl = (d1 <= s2);
		bl = (d1 <= us2);
		bl = (d1 <= i2);
		bl = (d1 <= ui2);
		bl = (d1 <= l2);
		bl = (d1 <= ul2);
		bl = (d1 <= c2);
		bl = (d1 <= f2);
		bl = (d1 <= d2);

		// decimal op ???
		bl = (dc1 > sb2);
		bl = (dc1 >= b2);
		bl = (dc1 > s2);
		bl = (dc1 >= us2);
		bl = (dc1 > i2);
		bl = (dc1 >= ui2);
		bl = (dc1 > l2);
		bl = (dc1 >= ul2);
		bl = (dc1 > c2);
		bl = (dc1 >= dc2);

		// object comparisons
		bl = (o1 == o2);
		bl = (o1 != o2);
		
		// Then code generation

		// sbyte op ???
		while(sb1 == sb2) bl = true;
		while(sb1 == b2) bl = true;
		while(sb1 == s2) bl = true;
		while(sb1 == us2) bl = true;
		while(sb1 == i2) bl = true;
		while(sb1 == ui2) bl = true;
		while(sb1 == l2) bl = true;
		while(sb1 == c2) bl = true;
		while(sb1 == f2) bl = true;
		while(sb1 == d2) bl = true;
		while(sb1 == dc2) bl = true;

		// byte op ???
		while(b1 != sb2) bl = true;
		while(b1 != b2) bl = true;
		while(b1 != s2) bl = true;
		while(b1 != us2) bl = true;
		while(b1 != i2) bl = true;
		while(b1 != ui2) bl = true;
		while(b1 != l2) bl = true;
		while(b1 != ul2) bl = true;
		while(b1 != c2) bl = true;
		while(b1 != f2) bl = true;
		while(b1 != d2) bl = true;
		while(b1 != dc2) bl = true;

		// short op ???
		while(s1 < sb2) bl = true;
		while(s1 < b2) bl = true;
		while(s1 < s2) bl = true;
		while(s1 < us2) bl = true;
		while(s1 < i2) bl = true;
		while(s1 < ui2) bl = true;
		while(s1 < l2) bl = true;
		while(s1 < c2) bl = true;
		while(s1 < f2) bl = true;
		while(s1 < d2) bl = true;
		while(s1 < dc2) bl = true;

		// ushort op ???
		while(us1 <= sb2) bl = true;
		while(us1 <= b2) bl = true;
		while(us1 <= s2) bl = true;
		while(us1 <= us2) bl = true;
		while(us1 <= i2) bl = true;
		while(us1 <= ui2) bl = true;
		while(us1 <= l2) bl = true;
		while(us1 <= ul2) bl = true;
		while(us1 <= c2) bl = true;
		while(us1 <= f2) bl = true;
		while(us1 <= d2) bl = true;
		while(us1 <= dc2) bl = true;

		// int op ???
		while(i1 > sb2) bl = true;
		while(i1 > b2) bl = true;
		while(i1 > s2) bl = true;
		while(i1 > us2) bl = true;
		while(i1 > i2) bl = true;
		while(i1 > ui2) bl = true;
		while(i1 > l2) bl = true;
		while(i1 > c2) bl = true;
		while(i1 > f2) bl = true;
		while(i1 > d2) bl = true;
		while(i1 > dc2) bl = true;

		// uint op ???
		while(ui1 >= sb2) bl = true;
		while(ui1 >= b2) bl = true;
		while(ui1 >= s2) bl = true;
		while(ui1 >= us2) bl = true;
		while(ui1 >= i2) bl = true;
		while(ui1 >= ui2) bl = true;
		while(ui1 >= l2) bl = true;
		while(ui1 >= ul2) bl = true;
		while(ui1 >= c2) bl = true;
		while(ui1 >= f2) bl = true;
		while(ui1 >= d2) bl = true;
		while(ui1 >= dc2) bl = true;

		// long op ???
		while(l1 == sb2) bl = true;
		while(l1 == b2) bl = true;
		while(l1 == s2) bl = true;
		while(l1 == us2) bl = true;
		while(l1 == i2) bl = true;
		while(l1 == ui2) bl = true;
		while(l1 == l2) bl = true;
		while(l1 == c2) bl = true;
		while(l1 == f2) bl = true;
		while(l1 == d2) bl = true;
		while(l1 == dc2) bl = true;

		// ulong op ???
		while(ul1 != b2) bl = true;
		while(ul1 != us2) bl = true;
		while(ul1 != ui2) bl = true;
		while(ul1 != ul2) bl = true;
		while(ul1 != c2) bl = true;
		while(ul1 != f2) bl = true;
		while(ul1 != d2) bl = true;
		while(ul1 != dc2) bl = true;

		// float op ???
		while(f1 < sb2) bl = true;
		while(f1 < b2) bl = true;
		while(f1 < s2) bl = true;
		while(f1 < us2) bl = true;
		while(f1 < i2) bl = true;
		while(f1 < ui2) bl = true;
		while(f1 < l2) bl = true;
		while(f1 < ul2) bl = true;
		while(f1 < c2) bl = true;
		while(f1 < f2) bl = true;
		while(f1 < d2) bl = true;

		// double op ???
		while(d1 <= sb2) bl = true;
		while(d1 <= b2) bl = true;
		while(d1 <= s2) bl = true;
		while(d1 <= us2) bl = true;
		while(d1 <= i2) bl = true;
		while(d1 <= ui2) bl = true;
		while(d1 <= l2) bl = true;
		while(d1 <= ul2) bl = true;
		while(d1 <= c2) bl = true;
		while(d1 <= f2) bl = true;
		while(d1 <= d2) bl = true;

		// decimal op ???
		while(dc1 > sb2) bl = true;
		while(dc1 >= b2) bl = true;
		while(dc1 > s2) bl = true;
		while(dc1 >= us2) bl = true;
		while(dc1 > i2) bl = true;
		while(dc1 >= ui2) bl = true;
		while(dc1 > l2) bl = true;
		while(dc1 >= ul2) bl = true;
		while(dc1 > c2) bl = true;
		while(dc1 >= dc2) bl = true;

		// object comparisons
		while(o1 == o2) bl = true;
		while(o1 != o2) bl = true;
		
		// Else code generation

		// sbyte op ???
		if(sb1 == sb2) bl = true;
		if(sb1 == b2) bl = true;
		if(sb1 == s2) bl = true;
		if(sb1 == us2) bl = true;
		if(sb1 == i2) bl = true;
		if(sb1 == ui2) bl = true;
		if(sb1 == l2) bl = true;
		if(sb1 == c2) bl = true;
		if(sb1 == f2) bl = true;
		if(sb1 == d2) bl = true;
		if(sb1 == dc2) bl = true;

		// byte op ???
		if(b1 != sb2) bl = true;
		if(b1 != b2) bl = true;
		if(b1 != s2) bl = true;
		if(b1 != us2) bl = true;
		if(b1 != i2) bl = true;
		if(b1 != ui2) bl = true;
		if(b1 != l2) bl = true;
		if(b1 != ul2) bl = true;
		if(b1 != c2) bl = true;
		if(b1 != f2) bl = true;
		if(b1 != d2) bl = true;
		if(b1 != dc2) bl = true;

		// short op ???
		if(s1 < sb2) bl = true;
		if(s1 < b2) bl = true;
		if(s1 < s2) bl = true;
		if(s1 < us2) bl = true;
		if(s1 < i2) bl = true;
		if(s1 < ui2) bl = true;
		if(s1 < l2) bl = true;
		if(s1 < c2) bl = true;
		if(s1 < f2) bl = true;
		if(s1 < d2) bl = true;
		if(s1 < dc2) bl = true;

		// ushort op ???
		if(us1 <= sb2) bl = true;
		if(us1 <= b2) bl = true;
		if(us1 <= s2) bl = true;
		if(us1 <= us2) bl = true;
		if(us1 <= i2) bl = true;
		if(us1 <= ui2) bl = true;
		if(us1 <= l2) bl = true;
		if(us1 <= ul2) bl = true;
		if(us1 <= c2) bl = true;
		if(us1 <= f2) bl = true;
		if(us1 <= d2) bl = true;
		if(us1 <= dc2) bl = true;

		// int op ???
		if(i1 > sb2) bl = true;
		if(i1 > b2) bl = true;
		if(i1 > s2) bl = true;
		if(i1 > us2) bl = true;
		if(i1 > i2) bl = true;
		if(i1 > ui2) bl = true;
		if(i1 > l2) bl = true;
		if(i1 > c2) bl = true;
		if(i1 > f2) bl = true;
		if(i1 > d2) bl = true;
		if(i1 > dc2) bl = true;

		// uint op ???
		if(ui1 >= sb2) bl = true;
		if(ui1 >= b2) bl = true;
		if(ui1 >= s2) bl = true;
		if(ui1 >= us2) bl = true;
		if(ui1 >= i2) bl = true;
		if(ui1 >= ui2) bl = true;
		if(ui1 >= l2) bl = true;
		if(ui1 >= ul2) bl = true;
		if(ui1 >= c2) bl = true;
		if(ui1 >= f2) bl = true;
		if(ui1 >= d2) bl = true;
		if(ui1 >= dc2) bl = true;

		// long op ???
		if(l1 == sb2) bl = true;
		if(l1 == b2) bl = true;
		if(l1 == s2) bl = true;
		if(l1 == us2) bl = true;
		if(l1 == i2) bl = true;
		if(l1 == ui2) bl = true;
		if(l1 == l2) bl = true;
		if(l1 == c2) bl = true;
		if(l1 == f2) bl = true;
		if(l1 == d2) bl = true;
		if(l1 == dc2) bl = true;

		// ulong op ???
		if(ul1 != b2) bl = true;
		if(ul1 != us2) bl = true;
		if(ul1 != ui2) bl = true;
		if(ul1 != ul2) bl = true;
		if(ul1 != c2) bl = true;
		if(ul1 != f2) bl = true;
		if(ul1 != d2) bl = true;
		if(ul1 != dc2) bl = true;

		// float op ???
		if(f1 < sb2) bl = true;
		if(f1 < b2) bl = true;
		if(f1 < s2) bl = true;
		if(f1 < us2) bl = true;
		if(f1 < i2) bl = true;
		if(f1 < ui2) bl = true;
		if(f1 < l2) bl = true;
		if(f1 < ul2) bl = true;
		if(f1 < c2) bl = true;
		if(f1 < f2) bl = true;
		if(f1 < d2) bl = true;

		// double op ???
		if(d1 <= sb2) bl = true;
		if(d1 <= b2) bl = true;
		if(d1 <= s2) bl = true;
		if(d1 <= us2) bl = true;
		if(d1 <= i2) bl = true;
		if(d1 <= ui2) bl = true;
		if(d1 <= l2) bl = true;
		if(d1 <= ul2) bl = true;
		if(d1 <= c2) bl = true;
		if(d1 <= f2) bl = true;
		if(d1 <= d2) bl = true;

		// decimal op ???
		if(dc1 > sb2) bl = true;
		if(dc1 >= b2) bl = true;
		if(dc1 > s2) bl = true;
		if(dc1 >= us2) bl = true;
		if(dc1 > i2) bl = true;
		if(dc1 >= ui2) bl = true;
		if(dc1 > l2) bl = true;
		if(dc1 >= ul2) bl = true;
		if(dc1 > c2) bl = true;
		if(dc1 >= dc2) bl = true;

		// object comparisons
		if(o1 == o2) bl = true;
		if(o1 != o2) bl = true;
	}
}
