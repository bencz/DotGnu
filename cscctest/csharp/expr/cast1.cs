/*
 * cast1.cs - Test the handling of valid cast and coerce expressions.
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

		checked
		{
			// Convert from sbyte.
			sb = sb;
			b = (byte)sb;
			s = sb;
			us = (ushort)sb;
			i = sb;
			ui = (uint)sb;
			l = sb;
			ul = (ulong)sb;
			c = (char)sb;
			f = sb;
			d = sb;
			dc = sb;
	
			// Convert from byte.
			sb = (sbyte)b;
			b = b;
			s = b;
			us = b;
			i = b;
			ui = b;
			l = b;
			ul = b;
			c = (char)b;
			f = b;
			d = b;
			dc = b;
	
			// Convert from short.
			sb = (sbyte)s;
			b = (byte)s;
			s = s;
			us = (ushort)s;
			i = s;
			ui = (uint)s;
			l = s;
			ul = (ulong)s;
			c = (char)s;
			f = s;
			d = s;
			dc = s;
	
			// Convert from ushort.
			sb = (sbyte)us;
			b = (byte)us;
			s = (short)us;
			us = us;
			i = us;
			ui = us;
			l = us;
			ul = us;
			c = (char)us;
			f = us;
			d = us;
			dc = us;
	
			// Convert from int.
			sb = (sbyte)i;
			b = (byte)i;
			s = (short)i;
			us = (ushort)i;
			i = i;
			ui = (uint)i;
			l = i;
			ul = (ulong)i;
			c = (char)i;
			f = i;
			d = i;
			dc = i;
	
			// Convert from uint.
			sb = (sbyte)ui;
			b = (byte)ui;
			s = (short)ui;
			us = (ushort)ui;
			i = (int)ui;
			ui = ui;
			l = ui;
			ul = ui;
			c = (char)ui;
			f = ui;
			d = ui;
			dc = ui;
	
			// Convert from long.
			sb = (sbyte)l;
			b = (byte)l;
			s = (short)l;
			us = (ushort)l;
			i = (int)l;
			ui = (uint)l;
			l = l;
			ul = (ulong)l;
			c = (char)l;
			f = l;
			d = l;
			dc = l;
	
			// Convert from ulong.
			sb = (sbyte)ul;
			b = (byte)ul;
			s = (short)ul;
			us = (ushort)ul;
			i = (int)ul;
			ui = (uint)ul;
			l = (long)ul;
			ul = ul;
			c = (char)ul;
			f = ul;
			d = ul;
			dc = ul;
	
			// Convert from char.
			sb = (sbyte)c;
			b = (byte)c;
			s = (short)c;
			us = c;
			i = c;
			ui = c;
			l = c;
			ul = c;
			c = c;
			f = c;
			d = c;
			dc = c;
	
			// Convert from float.
			sb = (sbyte)f;
			b = (byte)f;
			s = (short)f;
			us = (ushort)f;
			i = (int)f;
			ui = (uint)f;
			l = (long)f;
			ul = (ulong)f;
			c = (char)f;
			f = f;
			d = f;
			dc = (decimal)f;
	
			// Convert from double.
			sb = (sbyte)d;
			b = (byte)d;
			s = (short)d;
			us = (ushort)d;
			i = (int)d;
			ui = (uint)d;
			l = (long)d;
			ul = (ulong)d;
			c = (char)d;
			f = (float)d;
			d = d;
			dc = (decimal)d;
	
			// Convert from decimal.
			sb = (sbyte)dc;
			b = (byte)dc;
			s = (short)dc;
			us = (ushort)dc;
			i = (int)dc;
			ui = (uint)dc;
			l = (long)dc;
			ul = (ulong)dc;
			c = (char)dc;
			f = (float)dc;
			d = (double)dc;
			dc = dc;
		}
	}

	void m2()
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

		unchecked
		{
			// Convert from sbyte.
			sb = sb;
			b = (byte)sb;
			s = sb;
			us = (ushort)sb;
			i = sb;
			ui = (uint)sb;
			l = sb;
			ul = (ulong)sb;
			c = (char)sb;
			f = sb;
			d = sb;
			dc = sb;
	
			// Convert from byte.
			sb = (sbyte)b;
			b = b;
			s = b;
			us = b;
			i = b;
			ui = b;
			l = b;
			ul = b;
			c = (char)b;
			f = b;
			d = b;
			dc = b;
	
			// Convert from short.
			sb = (sbyte)s;
			b = (byte)s;
			s = s;
			us = (ushort)s;
			i = s;
			ui = (uint)s;
			l = s;
			ul = (ulong)s;
			c = (char)s;
			f = s;
			d = s;
			dc = s;
	
			// Convert from ushort.
			sb = (sbyte)us;
			b = (byte)us;
			s = (short)us;
			us = us;
			i = us;
			ui = us;
			l = us;
			ul = us;
			c = (char)us;
			f = us;
			d = us;
			dc = us;
	
			// Convert from int.
			sb = (sbyte)i;
			b = (byte)i;
			s = (short)i;
			us = (ushort)i;
			i = i;
			ui = (uint)i;
			l = i;
			ul = (ulong)i;
			c = (char)i;
			f = i;
			d = i;
			dc = i;
	
			// Convert from uint.
			sb = (sbyte)ui;
			b = (byte)ui;
			s = (short)ui;
			us = (ushort)ui;
			i = (int)ui;
			ui = ui;
			l = ui;
			ul = ui;
			c = (char)ui;
			f = ui;
			d = ui;
			dc = ui;
	
			// Convert from long.
			sb = (sbyte)l;
			b = (byte)l;
			s = (short)l;
			us = (ushort)l;
			i = (int)l;
			ui = (uint)l;
			l = l;
			ul = (ulong)l;
			c = (char)l;
			f = l;
			d = l;
			dc = l;
	
			// Convert from ulong.
			sb = (sbyte)ul;
			b = (byte)ul;
			s = (short)ul;
			us = (ushort)ul;
			i = (int)ul;
			ui = (uint)ul;
			l = (long)ul;
			ul = ul;
			c = (char)ul;
			f = ul;
			d = ul;
			dc = ul;
	
			// Convert from char.
			sb = (sbyte)c;
			b = (byte)c;
			s = (short)c;
			us = c;
			i = c;
			ui = c;
			l = c;
			ul = c;
			c = c;
			f = c;
			d = c;
			dc = c;
	
			// Convert from float.
			sb = (sbyte)f;
			b = (byte)f;
			s = (short)f;
			us = (ushort)f;
			i = (int)f;
			ui = (uint)f;
			l = (long)f;
			ul = (ulong)f;
			c = (char)f;
			f = f;
			d = f;
			dc = (decimal)f;
	
			// Convert from double.
			sb = (sbyte)d;
			b = (byte)d;
			s = (short)d;
			us = (ushort)d;
			i = (int)d;
			ui = (uint)d;
			l = (long)d;
			ul = (ulong)d;
			c = (char)d;
			f = (float)d;
			d = d;
			dc = (decimal)d;
	
			// Convert from decimal.
			sb = (sbyte)dc;
			b = (byte)dc;
			s = (short)dc;
			us = (ushort)dc;
			i = (int)dc;
			ui = (uint)dc;
			l = (long)dc;
			ul = (ulong)dc;
			c = (char)dc;
			f = (float)dc;
			d = (double)dc;
			dc = dc;
		}
	}
}
