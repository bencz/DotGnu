/*
 * shift1.cs - Test shift operators.
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
		sbyte sb1 = 34;
		byte b1 = 42;
		short s1 = -126;
		ushort us1 = 67;
		int i1 = -1234;
		uint ui1 = 54321;
		long l1 = -9876543210;
		ulong ul1 = 9876543210;
		char c1 = 'A';
		sbyte sb2 = 34;
		byte b2 = 42;
		short s2 = -126;
		ushort us2 = 67;
		int i2 = -1234;
		uint ui2 = 54321;
		long l2 = -9876543210;
		ulong ul2 = 9876543210;
		char c2 = 'A';
		sbyte sb3;
		byte b3;
		short s3;
		ushort us3;
		int i3;
		uint ui3;
		long l3;
		ulong ul3;
		char c3;
		
		// left shift
		i3 = sb1 << i2;
		i3 = b1 << i2;
		i3 = s1 << i2;
		i3 = us1 << i2;
		i3 = i1 << i2;
		ui3 = ui1 << i2;
		l3 = l1 << i2;
		ul3 = ul1 << i2;
		i3 = c1 << i2;

		// right shift.
		i3 = sb1 >> i2;
		i3 = b1 >> i2;
		i3 = s1 >> i2;
		i3 = us1 >> i2;
		i3 = i1 >> i2;
		ui3 = ui1 >> i2;
		l3 = l1 >> i2;
		ul3 = ul1 >> i2;
		i3 = c1 >> i2;

		// test numeric promotion of second argument.
		i3 = i2 << sb1;
		i3 = i2 >> b1;
		i3 = i2 << s1;
		i3 = i2 >> us1;
		i3 = i2 << c1;
	}
}
