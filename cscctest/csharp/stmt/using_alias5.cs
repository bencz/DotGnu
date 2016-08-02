/*
 * using_alias5.cs - Test scoping of using aliases - ECMA 334 V4 16.4.1.
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

using R = N1.N2;

namespace N1.N2
{
	class A {}
}

namespace N3
{
	using R2 = N1;
	using R3 = N1.N2;
	using R4 = R2.N2;	// Error: R2 unknown

	class B : R4.A {}
}

