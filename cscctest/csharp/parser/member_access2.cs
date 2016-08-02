/*
 * member_access2.cs - Test invalid member access and other modifiers.
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

public delegate void Del();

public abstract class Test
{
	// Access modifiers.
	public private int f1;
	public protected int f2;
	public internal int f3;
	private internal int f4;
	private protected int f5;

	// Field modifiers.
	abstract int f6;
	sealed int f7;
	virtual int f8;
	override int f9;
	extern int f10;

	// Constant modifiers.
	abstract const int c1 = 0;
	sealed const int c2 = 0;
	static const int c3 = 0;
	readonly const int c4 = 0;
	virtual const int c5 = 0;
	override const int c6 = 0;
	extern const int c7 = 0;
	unsafe const int c8 = 0;
	volatile const int c9 = 0;

	// Method modifiers.
	public static virtual void m1() {}
	public static abstract void m2() {}
	public static override void m3() {}
	public abstract override new void m4();
	public abstract virtual void m5();
	public virtual override void m6() {}
	public override new void m7() {}
	public volatile void m8() {}
	public virtual void m9() {}
	public sealed void m10() {}
	public static sealed void m11() {}
	public abstract sealed void m12() {}
	public virtual sealed void m13() {}

	// Event modifiers.
	public extern event Del e1;
	public volatile event Del e2;

	// Property modifiers.
	public extern int p1 { get { return 0; } set {} }
	public volatile int p2 { get { return 0; } set {} }

	// Operator modifiers.
	static bool operator==(Test t1, Test t2) { return false; }
	public bool operator!=(Test t1, Test t2) { return false; }
	public static new bool operator<(Test t1, Test t2) { return false; }
	public static sealed bool operator>=(Test t1, Test t2) { return false; }

}

public abstract class Test2 : Test
{
	// Method modifiers.
	public static void m1() {}		// Missing "new".
	public virtual void m6() {}		// Missing "new".
	public sealed override void m9() {}

}

public abstract class Test3 : Test2
{
	// Method modifiers.
	public override void m9() {}	// Breaks "sealed" declaration.

}
