/*
 * pointer3.cs - Test valid pointer arithmetic.
 *
 * Copyright (C) 2008  Southern Storm Software, Pty Ltd.
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

unsafe class Test
{
	public unsafe void t1(byte *x, byte *y, int i)
	{
		long l;
		byte *tmp;

		l = x - y;
		tmp = x + 4;
		tmp = x + i;
		tmp = 4 + x;
		tmp = i + x;
		tmp = x - 4;
		tmp += 2;
		tmp -= 2;
	}

	public unsafe void t1(int *x, int *y, int i)
	{
		long l;
		int *tmp;

		l = x - y;
		tmp = x + 4;
		tmp = x + i;
		tmp = 4 + x;
		tmp = i + x;
		tmp = x - 4;
		tmp += 2;
		tmp -= 2;
	}

	public unsafe void t1(long *x, long *y, int i)
	{
		long l;
		long *tmp;

		l = x - y;
		tmp = x + 4;
		tmp = x + i;
		tmp = 4 + x;
		tmp = i + x;
		tmp = x - 4;
		tmp += 2;
		tmp -= 2;
	}

	public unsafe void t1(void **x, void **y, int i)
	{
		long l;
		void **tmp;

		l = x - y;
		tmp = x + 4;
		tmp = x + i;
		tmp = 4 + x;
		tmp = i + x;
		tmp = x - 4;
		tmp += 2;
		tmp -= 2;
	}

	public unsafe void t2()
	{
		byte *bytePtr;
		byte b;

		b = *++bytePtr;
		b = *bytePtr++;
		b = *--bytePtr;
		b = *bytePtr--;
		b = bytePtr[1];
		b = bytePtr[0];
	}

	public unsafe void t3()
	{
		int *intPtr;
		int i;

		i = *++intPtr;
		i = *intPtr++;
		i = *--intPtr;
		i = *intPtr--;
		i = intPtr[1];
		i = intPtr[0];
	}
}
