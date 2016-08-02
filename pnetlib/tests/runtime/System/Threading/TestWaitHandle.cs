/*
 * TestBoolean.cs - Tests for the "Boolean" class.
 *
 * Copyright (C) 2002  Free Software Foundation.
 *
 * Authors: Thong Nguyen (tum@veridicus.com)
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

using CSUnit;
using System;
using System.Threading;

#if !ECMA_COMPAT

public class TestWaitHandle
	: TestCase
{
	private ManualResetEvent e1, e2;

	public TestWaitHandle(String name)
		: base(name)
	{
	}

	protected override void Setup()
	{
	}

	protected override void Cleanup()
	{
	}

	public void TestWaitAny()
	{
		int x;

		e1 = new ManualResetEvent(false);
		e2 = new ManualResetEvent(false);

		x = WaitHandle.WaitAny(new WaitHandle[] {e1,e2}, 100,false);

		AssertEquals("WaitAny(unset, unset)", x, WaitHandle.WaitTimeout);

		e1.Set();

		x = WaitHandle.WaitAny(new WaitHandle[] {e1,e2},100, false);

		AssertEquals("WaitAny(set, unset)", x, 0);

		e1.Reset();
		e2.Set();

		x = WaitHandle.WaitAny(new WaitHandle[] {e1,e2},100, false);

		AssertEquals("WaitAny(set, unset)", x, 1);

		e1.Set();
		e2.Set();

		x = WaitHandle.WaitAny(new WaitHandle[] {e1,e2},100, false);

		AssertEquals("WaitAny(set, set)", x, 0);
	}

	public void TestWaitAll()
	{
		bool x;

		e1 = new ManualResetEvent(false);
		e2 = new ManualResetEvent(false);

		x = WaitHandle.WaitAll(new WaitHandle[] {e1,e2}, 100,false);

		AssertEquals("WaitAll(unset, unset)", x, false);

		e1.Set();

		x = WaitHandle.WaitAll(new WaitHandle[] {e1,e2},100, false);

		AssertEquals("WaitAll(set, unset)", x, false);

		e1.Reset();
		e2.Set();

		x = WaitHandle.WaitAll(new WaitHandle[] {e1,e2},100, false);

		AssertEquals("WaitAll(set, unset)", x, false);

		e1.Set();
		e2.Set();

		x = WaitHandle.WaitAll(new WaitHandle[] {e1,e2},100, true);

		AssertEquals("WaitAll(set, set)", x, true);
	}
}

#endif
