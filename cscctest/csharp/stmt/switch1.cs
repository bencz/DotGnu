/*
 * switch1.cs - Test the handling of valid "switch" statements.
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
		int y;

		// Collapsed to a list of "if" statements.
		switch(x)
		{
			case 0: y = 0; break;
			case 1: y = 1; break;
			case 2: y = 2; break;
			case 3: y = 3; break;
		}

		// Indexed table.
		switch(x)
		{
			case 0: y = 0; break;
			case 1: y = 1; break;
			case 2: y = 2; break;
			case 3: y = 3; break;
			case 4: y = 4; break;
			case 5: y = 5; break;
			case 6: y = 6; break;
			case 7: y = 7; break;
		}

		// Indexed table with adjusted base.
		switch(x)
		{
			case 1: y = 1; break;
			case 2: y = 2; break;
			case 3: y = 3; break;
			case 4: y = 4; break;
			case 5: y = 5; break;
			case 6: y = 6; break;
			case 7: y = 7; break;
			case 8: y = 8; break;
		}

		// Binary chop (IL) or lookup (JVM) table.
		// Also tests case label sorting.
		switch(x)
		{
			case 1600: y = 1600; break;
			case 1500: y = 1500; break;
			case 1400: y = 1400; break;
			case 1300: y = 1300; break;
			case 1200: y = 1200; break;
			case 1100: y = 1100; break;
			case 1000: y = 1000; break;
			case  900: y =  900; break;
			case  800: y =  800; break;
			case  700: y =  700; break;
			case  600: y =  600; break;
			case  500: y =  500; break;
			case  400: y =  400; break;
			case  300: y =  300; break;
			case  200: y =  200; break;
			case  100: y =  100; break;
		}
	}

	void m2(long x)
	{
		int y;

		// Collapsed to a list of "if" statements.
		switch(x)
		{
			case 0: y = 0; break;
			case 1: y = 1; break;
			case 2: y = 2; break;
			case 3: y = 3; break;
		}

		// Binary chop table.
		switch(x)
		{
			case 0: y = 0; break;
			case 1: y = 1; break;
			case 2: y = 2; break;
			case 3: y = 3; break;
			case 4: y = 4; break;
			case 5: y = 5; break;
			case 6: y = 6; break;
			case 7: y = 7; break;
		}
	}

	void m3(string x)
	{
		int y;

		// Collapsed to a list of "if" statements.
		switch(x)
		{
			case null: y = 0; break;
			case "1":  y = 1; break;
			case "2":  y = 2; break;
			case "3":  y = 3; break;
		}

		// Binary chop table, with string sorting tests.
		switch(x)
		{
			case "7":  y = 7; break;
			case "6":  y = 6; break;
			case "5":  y = 5; break;
			case "4":  y = 4; break;
			case "3":  y = 3; break;
			case "2":  y = 2; break;
			case "1":  y = 1; break;
			case null: y = 0; break;
		}
	}

	void m4(Color x)
	{
		int y;

		// Collapsed to a list of "if" statements.
		switch(x)
		{
			case Color.Red: y = 0; break;
			case Color.Green: y = 1; break;
			case Color.Blue: y = 2; break;
			case Color.Yellow: y = 3; break;
		}

		// Indexed table.
		switch(x)
		{
			case Color.Red: y = 0; break;
			case Color.Green: y = 1; break;
			case Color.Blue: y = 2; break;
			case Color.Yellow: y = 3; break;
			case Color.Cyan: y = 4; break;
			case Color.Magenta: y = 5; break;
			case Color.Black: y = 6; break;
			case Color.White: y = 7; break;
		}
	}
}
