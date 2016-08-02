/*
 * switch2.cs - Test the handling of invalid "switch" statements.
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
	Blue,
	Yellow,
	Cyan,
	Magenta,
	Black,
	White
}

class Test
{
	void m1(int x)
	{
		Object o1;

		// The expression must be an r-value or l-value.
		switch(Int32)
		{
			case 1: break;
		}

		// Invalid governing type.
		switch(o1)
		{
		}

		// Multiple "default" cases.
		switch(x)
		{
			case 0:		break;
			default:	break;
			default:	break;
		}

		// Invalid case label expression.
		switch(x)
		{
			case Int32:	break;
		}

		// Case not convertible to governing type.
		switch(x)
		{
			case 3L: break;
		}

		// Switch section falls through.
		switch(x)
		{
			case 1: ;
		}

		// Duplicate case labels.
		switch(x)
		{
			case 4:
			case 4: break;
		}
	}
}
