/*
 * const1.cs - Test the handling of constants.
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
		object o;
		bool b;
		int i;
		uint ui;
		long l;
		ulong ul;
		float f;
		double d;
		decimal dc;
		string s;

		// Object constants.
		o = null;

		// Boolean constants.
		b = true;
		b = false;

		// Integer constants.
		i = -128;
		i = -2;
		i = -1;
		i = 0;
		i = 1;
		i = 2;
		i = 3;
		i = 4;
		i = 5;
		i = 6;
		i = 7;
		i = 8;
		i = 9;
		i = 127;
		i = -2147483648;
		i = -129;
		i = 128;
		i = 2147483647;

		// Unsigned integer constants.
		ui = 0;
		ui = 1;
		ui = 2;
		ui = 3;
		ui = 4;
		ui = 5;
		ui = 6;
		ui = 7;
		ui = 8;
		ui = 9;
		ui = 127;
		ui = 128;
		ui = 2147483647;
		ui = 2147483648;
		ui = 4294967295;

		// Long constants.
		l = -128;
		l = -2;
		l = -1;
		l = 0;
		l = 1;
		l = 2;
		l = 3;
		l = 4;
		l = 5;
		l = 6;
		l = 7;
		l = 8;
		l = 9;
		l = 127;
		l = -2147483648;
		l = -129;
		l = 128;
		l = 2147483647;
		l = 2147483648;
		l = 4294967295;
		l = 4294967296;
		l = 9223372036854775807;
		l = -9223372036854775808;

		// Unsigned long constants.
		ul = 0;
		ul = 1;
		ul = 2;
		ul = 3;
		ul = 4;
		ul = 5;
		ul = 6;
		ul = 7;
		ul = 8;
		ul = 9;
		ul = 127;
		ul = 128;
		ul = 2147483647;
		ul = 2147483648;
		ul = 4294967295;
		ul = 4294967296;
		ul = 9223372036854775807;
		ul = 9223372036854775808;
		ul = 18446744073709551488;
		ul = 18446744073709551615;

		// Float constants.
		f = 0.0f;
		f = 1.0f;
		f = -1.0f;
		f = 1.234e34f;
		f = 3.40282346638528859e38f;
		f = -3.40282346638528859e38f;
		f = 1.4e-45f;
		f = -128;
		f = -1;
		f = 0;
		f = 1;
		f = 127;
		f = -2147483648;
		f = 2147483647;
		f = 4294967295;
		f = 4294967296;
		f = 9223372036854775807;
		f = -9223372036854775808;
		f = 9223372036854775808;
		f = 18446744073709551488;
		f = 18446744073709551615;

		// Double constants.
		d = 0.0f;
		d = 1.0f;
		d = -1.0f;
		d = 1.234e34f;
		d = 3.40282346638528859e38f;
		d = -3.40282346638528859e38f;
		d = 1.4e-45f;
		d = 0.0d;
		d = 1.0d;
		d = -1.0d;
		d = 1.234e34d;
		d = 1.7976931348623157E+308d;
		d = -1.7976931348623157E+308d;
		d = 4.94065645841247e-324d;
		d = 0.0;
		d = 1.0;
		d = -1.0;
		d = 1.234e34;
		d = 1.7976931348623157E+308;
		d = -1.7976931348623157E+308;
		d = 4.94065645841247e-324;
		d = -128;
		d = -1;
		d = 0;
		d = 1;
		d = 127;
		d = -2147483648;
		d = 2147483647;
		d = 4294967295;
		d = 4294967296;
		d = 9223372036854775807;
		d = -9223372036854775808;
		d = 9223372036854775808;
		d = 18446744073709551488;
		d = 18446744073709551615;

		// Decimal constants.
		dc = 0.0m;
		dc = 1.0m;
		dc = -1.0m;
		dc = 2147483647.0m;
		dc = -2147483648.0m;
		dc = 2147483648.0m;
		dc = 4294967295.0m;
		dc = 4294967296.0m;
		dc = -4294967295.0m;
		dc = 9223372036854775807.0m;
		dc = -9223372036854775808.0m;
		dc = 9223372036854775808.0m;
		dc = 18446744073709551615.0m;
		dc = 79228162514264337593543950335.0m;
		dc = 7.9228162514264337593543950335m;
		dc = 7.92281625142643375935439503350003333333m;	// will be rounded
		dc = 7.9228162514264337593543950336m;			// will be rounded
		dc = 79000000000000000000000000000m;
		dc = 7.9e28m;
		dc = 1e-28m;
		dc = 1e-50m;									// rounded to zero

		// String constants.
		s = "Hello World!";
		s = "";
		s = null;
	}
}
