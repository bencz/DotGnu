/*
 * member_access1.cs - Test valid member access and other modifiers.
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

public abstract class Test
{
	// Access modifiers.
	public int f1;
	private int f2;
	protected int f3;
	protected internal int f4;
	internal int f5;
	int f6;					// Defaults to "private".

	// Field modifiers.
	static int f7;
	static readonly int f8;
	public readonly int f9;
	protected volatile int f10;

	// Method modifiers.
	public static void m1() {}
	public abstract void m2();
	public virtual void m3() {}
	public void m4() {}
	public virtual void m5() {}
	public virtual void m6() {}
	public extern void m7();
}

public abstract class Test2 : Test
{
	// The "new" modifier.
	public new int f1;
	public new int f11;		// Should give a warning.

	// Method modifiers.
	public abstract override void m2();
	public override void m3() {}
	public new void m4() {}
	public new virtual void m5() {}
	public override sealed void m6() {}

}
