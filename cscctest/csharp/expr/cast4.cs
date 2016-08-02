/*
 * cast4.cs - Test the valid casts and coercions for enumerated types.
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

enum Color
{
	Red,
	Green,
	Blue
}

enum ColorSmall : sbyte
{
	Red,
	Green,
	Blue
}

class Test
{
	public const Color red = 0;		// Can coerce zero to any enum type.

	void m1()
	{
		Color col1;
		ColorSmall cols1;
		sbyte sb1 = 3;
		byte b1 = 3;
		short s1 = 3;
		ushort us1 = 3;
		int i1 = 3;
		uint ui1 = 3;
		long l1 = 3;
		ulong ul1 = 3;
		char c1 = '3';
		float f1 = 3.0f;
		double d1 = 3.0;
		decimal dc1 = 3.0m;

		// Can coerce zero to any enum type.
		col1 = 0;
		cols1 = 0;

		// Any numeric type can be cast to any enumerated type.
		checked
		{
			col1 = (Color)sb1;
			col1 = (Color)b1;
			col1 = (Color)s1;
			col1 = (Color)us1;
			col1 = (Color)i1;
			col1 = (Color)ui1;
			col1 = (Color)l1;
			col1 = (Color)ul1;
			col1 = (Color)c1;
			col1 = (Color)f1;
			col1 = (Color)d1;
			col1 = (Color)dc1;
			cols1 = (ColorSmall)sb1;
			cols1 = (ColorSmall)b1;
			cols1 = (ColorSmall)s1;
			cols1 = (ColorSmall)us1;
			cols1 = (ColorSmall)i1;
			cols1 = (ColorSmall)ui1;
			cols1 = (ColorSmall)l1;
			cols1 = (ColorSmall)ul1;
			cols1 = (ColorSmall)c1;
			cols1 = (ColorSmall)f1;
			cols1 = (ColorSmall)d1;
			cols1 = (ColorSmall)dc1;
		}
		unchecked
		{
			col1 = (Color)sb1;
			col1 = (Color)b1;
			col1 = (Color)s1;
			col1 = (Color)us1;
			col1 = (Color)i1;
			col1 = (Color)ui1;
			col1 = (Color)l1;
			col1 = (Color)ul1;
			col1 = (Color)c1;
			col1 = (Color)f1;
			col1 = (Color)d1;
			col1 = (Color)dc1;
			cols1 = (ColorSmall)sb1;
			cols1 = (ColorSmall)b1;
			cols1 = (ColorSmall)s1;
			cols1 = (ColorSmall)us1;
			cols1 = (ColorSmall)i1;
			cols1 = (ColorSmall)ui1;
			cols1 = (ColorSmall)l1;
			cols1 = (ColorSmall)ul1;
			cols1 = (ColorSmall)c1;
			cols1 = (ColorSmall)f1;
			cols1 = (ColorSmall)d1;
			cols1 = (ColorSmall)dc1;
		}

		// Any enumerated type can be cast to any numeric type.
		checked
		{
			sb1 = (sbyte)col1;
			b1  = (byte)col1;
			s1  = (short)col1;
			us1 = (ushort)col1;
			i1  = (int)col1;
			ui1 = (uint)col1;
			l1  = (long)col1;
			ul1 = (ulong)col1;
			c1  = (char)col1;
			f1  = (float)col1;
			d1  = (double)col1;
			dc1 = (decimal)col1;
			sb1 = (sbyte)cols1;
			b1  = (byte)cols1;
			s1  = (short)cols1;
			us1 = (ushort)cols1;
			i1  = (int)cols1;
			ui1 = (uint)cols1;
			l1  = (long)cols1;
			ul1 = (ulong)cols1;
			c1  = (char)cols1;
			f1  = (float)cols1;
			d1  = (double)cols1;
			dc1 = (decimal)cols1;
		}
		unchecked
		{
			sb1 = (sbyte)col1;
			b1  = (byte)col1;
			s1  = (short)col1;
			us1 = (ushort)col1;
			i1  = (int)col1;
			ui1 = (uint)col1;
			l1  = (long)col1;
			ul1 = (ulong)col1;
			c1  = (char)col1;
			f1  = (float)col1;
			d1  = (double)col1;
			dc1 = (decimal)col1;
			sb1 = (sbyte)cols1;
			b1  = (byte)cols1;
			s1  = (short)cols1;
			us1 = (ushort)cols1;
			i1  = (int)cols1;
			ui1 = (uint)cols1;
			l1  = (long)cols1;
			ul1 = (ulong)cols1;
			c1  = (char)cols1;
			f1  = (float)cols1;
			d1  = (double)cols1;
			dc1 = (decimal)cols1;
		}

		// Any enumerated type can be cast to any other enumerated type.
		checked
		{
			col1 = (Color)cols1;
			cols1 = (ColorSmall)col1;
		}
		unchecked
		{
			col1 = (Color)cols1;
			cols1 = (ColorSmall)col1;
		}
	}
}
