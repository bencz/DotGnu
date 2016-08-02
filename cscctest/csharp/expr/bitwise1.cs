/*
 * bitwise1.cs - Test binary bitwise operators.
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
		
		// sbyte op ???
		i3 = sb1 & sb2;
		i3 = sb1 & b2;
		i3 = sb1 & s2;
		i3 = sb1 & us2;
		i3 = sb1 & i2;
		l3 = sb1 & ui2;
		l3 = sb1 & l2;
		i3 = sb1 & c2;

		// byte op ???
		i3 = b1 | sb2;
		i3 = b1 | b2;
		i3 = b1 | s2;
		i3 = b1 | us2;
		i3 = b1 | i2;
		ui3 = b1 | ui2;
		l3 = b1 | l2;
		ul3 = b1 | ul2;
		i3 = b1 | c2;

		// short op ???
		i3 = s1 ^ sb2;
		i3 = s1 ^ b2;
		i3 = s1 ^ s2;
		i3 = s1 ^ us2;
		i3 = s1 ^ i2;
		l3 = s1 ^ ui2;
		l3 = s1 ^ l2;
		i3 = s1 ^ c2;

		// ushort op ???
		i3 = us1 & sb2;
		i3 = us1 & b2;
		i3 = us1 & s2;
		i3 = us1 & us2;
		i3 = us1 & i2;
		ui3 = us1 & ui2;
		l3 = us1 & l2;
		ul3 = us1 & ul2;
		i3 = us1 & c2;

		// int op ???
		i3 = i1 | sb2;
		i3 = i1 | b2;
		i3 = i1 | s2;
		i3 = i1 | us2;
		i3 = i1 | i2;
		l3 = i1 | ui2;
		l3 = i1 | l2;
		i3 = i1 | c2;

		// uint op ???
		l3 = ui1 ^ sb2;
		ui3 = ui1 ^ b2;
		l3 = ui1 ^ s2;
		ui3 = ui1 ^ us2;
		l3 = ui1 ^ i2;
		ui3 = ui1 ^ ui2;
		l3 = ui1 ^ l2;
		ul3 = ui1 ^ ul2;
		ui3 = ui1 ^ c2;

		// long op ???
		l3 = l1 & sb2;
		l3 = l1 & b2;
		l3 = l1 & s2;
		l3 = l1 & us2;
		l3 = l1 & i2;
		l3 = l1 & ui2;
		l3 = l1 & l2;
		l3 = l1 & c2;

		// ulong op ???
		ul3 = ul1 | b2;
		ul3 = ul1 | us2;
		ul3 = ul1 | ui2;
		ul3 = ul1 | ul2;
		ul3 = ul1 | c2;
	}
}
