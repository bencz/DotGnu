/*
 * static1.cs - Test static class declarations.
 *
 * Copyright (C) 2009  Southern Storm Software, Pty Ltd.
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

namespace Test
{
	public delegate int E();

	public static class Test1
	{
		static int g = 1;
		public static int H = 2;
		const int I = 3;
		public const int J = 4;

		public static int G
		{
			get
			{
				return g;
			}
			set
			{
				g = value;
			}
		}

		public static event E E;

		public static int this[int i]
		{
			get
			{
				return i;
			}
			set
			{
				// Nothing
			}
		}
	}
}
