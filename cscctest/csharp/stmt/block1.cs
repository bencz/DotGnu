/*
 * block1.cs - Test block scoping of local variables.
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

class Test
{
	void m1()
	{
		int x;
		{
			int x, y;
		}
	}

	void m2(int x)
	{
		{
			int x;
		}
	}

	void m3()
	{
		for(int x = 0; x < 10; ++x)
		{
		}
		for(int x = 0; x < 10; ++x)
		{
		}
	}
}
