/*
 * using_alias3.cs - Test scoping of using aliases - ECMA 334 V4 16.4.1.
 *
 * Copyright (C) 2007  Southern Storm Software, Pty Ltd.
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

namespace N1.N2
{
	class B {}
}

namespace N3
{
	class A {}
	class B : A {}
}

namespace N3
{
	using A = N1.N2;
	using B = N1.N2.B;

	class W : B {}		// Error: B is ambiguous
	class X : A.B {}	// Error: A is ambiguous
}
