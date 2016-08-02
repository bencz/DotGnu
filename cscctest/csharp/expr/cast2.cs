/*
 * cast2.cs - Test the handling of invalid cast and coerce expressions.
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
		sbyte sb = 34;
		byte b = 42;
		short s = -126;
		ushort us = 67;
		int i = -1234;
		uint ui = 54321;
		long l = -9876543210;
		ulong ul = 9876543210;
		char c = 'A';
		float f = 1.5f;
		double d = 6.7d;
		decimal dc = 3.5m;

		// Convert from sbyte.
		b = sb;
		us = sb;
		ui = sb;
		ul = sb;
		c = sb;
	
		// Convert from byte.
		sb = b;
		c = b;
	
		// Convert from short.
		sb = s;
		b = s;
		us = s;
		ui = s;
		ul = s;
		c = s;
	
		// Convert from ushort.
		sb = us;
		b = us;
		s = us;
		c = us;

		// Convert from int.
		sb = i;
		b = i;
		s = i;
		us = i;
		ui = i;
		ul = i;
		c = i;
	
		// Convert from uint.
		sb = ui;
		b = ui;
		s = ui;
		us = ui;
		i = ui;
		c = ui;
	
		// Convert from long.
		sb = l;
		b = l;
		s = l;
		us = l;
		i = l;
		ui = l;
		ul = l;
		c = l;
	
		// Convert from ulong.
		sb = ul;
		b = ul;
		s = ul;
		us = ul;
		i = ul;
		ui = ul;
		l = ul;
		c = ul;
	
		// Convert from char.
		sb = c;
		b = c;
		s = c;

		// Convert from float.
		sb = f;
		b = f;
		s = f;
		us = f;
		i = f;
		ui = f;
		l = f;
		ul = f;
		c = f;
		dc = f;
	
		// Convert from double.
		sb = d;
		b = d;
		s = d;
		us = d;
		i = d;
		ui = d;
		l = d;
		ul = d;
		c = d;
		f = d;
		dc = d;

		// Convert from decimal.
		sb = dc;
		b = dc;
		s = dc;
		us = dc;
		i = dc;
		ui = dc;
		l = dc;
		ul = dc;
		c = dc;
		f = dc;
		d = dc;

		// Convert from string.
		i = "hello";
		i = (int)"hello";
	}
}
