/*
 * constructor2.cs - Test invalid constructor declarations.
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
	// Name mismatch in constructor.
	public Test2()
	{
	}
}

class Test2
{
	public Test2()
	{
	}
}

class Test3 : Test2
{
	// Cannot locate base class constructor.
	public Test3()
		: base(3)
	{
	}
}

class Test4
{
	// Cannot locate "this" constructor.
	public Test4()
		: this(3)
	{
	}
}

class Test5
{
	Test5()
	{
	}
}

class Test6 : Test5
{
	// Inaccessible base class constructor.
	Test6()
		: base()
	{
	}
}
