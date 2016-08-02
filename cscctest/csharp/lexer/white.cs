/*
 * white.cs - Test the handling of white space in the lexer.
 *
 * "C# Language Specification", Draft 13, Section 9.3.3, "White space"
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

/* Space character */
 x;

/* Tab character */
	y;

/* Vertical tab character */
z;

/* Form feed character */
w;

/* Control-Z */
v;

/* UTF-8 marker */
﻿u;

/* Non-breakable space (\u00A0): not handled properly yet */
 t;

/* "em" space (\u2003): not handled properly yet */
 s;
