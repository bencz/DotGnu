/*
 * type_access.cs - Test error reporting for type accessibility declarations.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

// Success.
public class A1 {}
internal class A2 {}
class A3 {}
abstract public class A4 {}
sealed class A5 {}
public class A6
{
	public class AN1 {}
	class AN2 {}
	private class AN3 {}
	internal class AN4 {}
	protected internal class AN5 {}
	protected class AN6 {}
	new public class AN7 {}
}

// Cannot be used on non-nested types.
private class B1 {}
protected class B2 {}
new public class B3 {}

// Cannot use both.
public private class C1 {}
public protected class C2 {}
public internal class C3 {}
internal private class C4 {}
internal protected class C5 {}
public class C6
{
	public private class CN1 {}
	public protected class CN2 {}
	public internal class CN3 {}
	private internal class CN4 {}
	private protected class CN5 {}
}

// Bad modifiers.
static class D1 {}
readonly class D2 {}
virtual class D3 {}
override class D4 {}
extern class D5 {}
public class D6
{
	static class DN1 {}
	readonly class DN2 {}
	virtual class DN3 {}
	override class DN4 {}
	extern class DN5 {}
}

// Modifier specified more than once.
public public class E1 {}
internal internal class E2 {}
abstract abstract public class E4 {}
sealed sealed class E5 {}
sealed sealed sealed class E6 {}

// Warning.
unsafe public class F1 {}
public class F2
{
	unsafe class FN1 {}
}
