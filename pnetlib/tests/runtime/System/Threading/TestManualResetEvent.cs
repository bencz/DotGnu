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

public class TestManualResetEvent
	: TestCase
{
	private ManualResetEvent e1;

	public TestManualResetEvent(String name)
		: base(name)
	{
	}

	protected override void Setup()
	{
	}

	protected override void Cleanup()
	{
	}

	public void TestWaitOneSingleThreaded()
	{
		bool x;

		e1 = new ManualResetEvent(false);

		x = e1.WaitOne(10,false);

		AssertEquals("WaitOne(unset)", x, false);

		e1.Set();

		x = e1.WaitOne(10,false);

		AssertEquals("WaitOne(set)", x, true);

		// It should still be set.

		x = e1.WaitOne(10,false);

		AssertEquals("WaitOne(set)", x, true);
	}

	private void SetE1()
	{
		e1.Set();
	}
	
	public void TestWaitOneMultiThreaded()
	{
		bool x;
		Thread thread1;

		if (!TestThread.IsThreadingSupported)
		{
			return;
		}
		
		e1 = new ManualResetEvent(false);

		x = e1.WaitOne(10,false);

		AssertEquals("WaitOne(unset)", x, false);

		thread1 = new Thread(new ThreadStart(SetE1));
		
		thread1.Start();

		x = e1.WaitOne(4000,false);

		AssertEquals("WaitOne(set)", x, true);

		// It should still be set.

		x = e1.WaitOne(10,false);

		AssertEquals("WaitOne(set)", x, true);
	}
}

#endif
