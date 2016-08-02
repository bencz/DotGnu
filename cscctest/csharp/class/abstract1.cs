/*
 * abstract1.cs - Test abstract class declarations.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

public class Test
{
	// abstract used on members in a non-abstract class.
	public abstract void t1();
	public abstract int X { get; set; }
}

public abstract class Test2
{
	// These definitions should be ok because Test2 is abstract.
	public abstract void t1();
	public abstract int X { get; set; }
}

public abstract class Test3 : Test2
{
	// OK - no need to override parent definitions.
}

public class Test4 : Test2
{
	// Error - must override parent definitions.
}

public abstract class Test5 : Test2
{
	// OK - all abstract parent definitions overrriden.
	public override void t1() {}
	public override int X { get { return 0; } set {} }
}
