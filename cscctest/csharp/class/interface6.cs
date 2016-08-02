/*
 * interface6.cs - Test multi-inclusions of interfaces.
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

using System;

public interface A
{
	int t1 { get; }
}

public interface B : A
{
}

public interface C : B, A
{
}

public class Test
{
	int t2(C x)
	{
		// t1 is included via multiple paths, but this should not
		// result in an "ambiguous" error.
		return x.t1;
	}
}
