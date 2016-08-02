/*
 * typedref2.cs - Test invalid expressions involved typedref values.
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

struct W
{
	int w;
}

class Test
{
	void m1(TypedReference tref)
	{
		int x = __refvalue(tref, fld); // second argument is not a type.
	}

	static readonly int fld;
	int Prop { get { return 0; } set {} }
	int Prop2 { set {} }

	void m2()
	{
		m1(__makeref(Test));		// not an l-value
		m1(__makeref(fld));			// r-value
		m1(__makeref(Prop));		// l-value, but not addressable
		m1(__makeref(Prop2));		// s-value
	}

	void m3()
	{
		Type type = __reftype(3);	// not an instance of TypedReference.
		int x = __refvalue(3, int);	// not an instance of TypedReference.
	}
}
