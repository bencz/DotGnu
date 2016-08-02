/*
 * ident2.cs - Test the handling of invalid identifiers in the lexer.
 *
 * "C# Language Specification", Draft 13, Section 9.4.2, "Identifiers"
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

/* Invalid idenfifiers */
class 0 {}
class @0 {}
class struct {}
class \uFF11 {}				// FULLWIDTH DIGIT ONE
class \U00010000 {}			// Outside base Unicode range

/* Identifiers that should be valid, but aren't yet */
class ḁ {}				// \u1E00: LATIN CAPITAL LETTER A WITH RING BELOW
