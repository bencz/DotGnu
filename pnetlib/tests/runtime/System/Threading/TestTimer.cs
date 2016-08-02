/*
 * TestTimer.cs - Tests for the "Boolean" class.
 *
 * Copyright (C) 2002  Free Software Foundation.
 *
 * Author: Russell Stuart <russell-savannah@stuart.id.au>
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
using System.Collections;
using System.Threading;

public class TestTimer : TestCase
{
	AutoResetEvent autoResetEvent;
	int checkTimeoutCount;
	Timer timer;
	DateTime timeTimerExpired;
	TimerCallback timerCallbackDelegate = new TimerCallback(timerCallback);
	System.DateTime startTest;

	public TestTimer(String name) : base(name)
	{
	}

	protected override void Setup()
	{
		if (!TestThread.IsThreadingSupported)
			return;
		this.timer = new Timer(this.timerCallbackDelegate, this,
			Timeout.Infinite, Timeout.Infinite);
		this.timeTimerExpired = DateTime.MaxValue;
		this.checkTimeoutCount = 0;
		this.startTest = System.DateTime.Now;
		this.autoResetEvent = new AutoResetEvent(false);
	}

	protected override void Cleanup()
	{
		if (!TestThread.IsThreadingSupported)
			return;
		this.timer.Dispose();
	}

	public void TestTimerAAAAA()
	{
		System.Console.Write("if timers fail up MIN_TIMEOUT in TimerTest.cs .. ");
	}

	//
	// MIN_TIMEOUT is the minimum amount of time (in milliseconds) the tests
	// try to measure.  The bigger this is the longer the tests will take
	// to run.  But if it takes your machine longer than this to execute
	// the steps in the test, the test will appear to fail.
	//
	const int MIN_TIMEOUT 			= 20;
	const int SHORT_TIMEOUT 		= MIN_TIMEOUT * 2;

	private static void timerCallback(Object state)
	{
		TestTimer testTimer = (TestTimer)state;
		lock (testTimer)
		{
			testTimer.timeTimerExpired = DateTime.Now;
			testTimer.autoResetEvent.Set();
		}
	}

	private static void timerCallbackDummy(Object state)
	{
	}

	//
	// Test the constructor builds a reasonable looking object.
	//
	public void TestTimerConstructor()
	{
		if (!TestThread.IsThreadingSupported)
			return;
		TimerCallback timerCallbackDummyDelegate =
			new TimerCallback(timerCallbackDummy);
		//
		// Verify it checks the arguments correctly.
		//
		try
		{
			new Timer(null, this, 0, 0);
			Assert("Expected ArgumentNullException", false);
		}
		catch (ArgumentNullException)
		{
		}
		Timer timer = new Timer(
			timerCallbackDummyDelegate, null, 0, Timeout.Infinite);
		timer.Dispose();
		timer = new Timer(timerCallbackDummyDelegate, null, 0, 0);
		timer.Dispose();
		try
		{
			new Timer(this.timerCallbackDelegate, this, -2, 0);
			Assert("Expected ArgumentOutOfRangeException", false);
		}
		catch (ArgumentOutOfRangeException)
		{
		}
		try
		{
			new Timer(this.timerCallbackDelegate, this, 0, -2);
			Assert("Expected ArgumentOutOfRangeException", false);
		}
		catch (ArgumentOutOfRangeException)
		{
		}
	}

	//
	// Test the various constructors set the timeout required.
	//
	public void TestTimerConstructorTimeout()
	{
		if (!TestThread.IsThreadingSupported)
			return;
		//
		// Test various conbinations of due and period.  All we are really
		// checking here is the right parameters are passed to Change() by
		// the constructor.
		//
		Timer t = new Timer(this.timerCallbackDelegate,
			this, 0, Timeout.Infinite);
		this.checkTimeout(0);
		this.checkTimeout(Timeout.Infinite);
		t.Dispose();
		t = new Timer(this.timerCallbackDelegate, this, 0, SHORT_TIMEOUT);
		this.checkTimeout(0);
		this.checkTimeout(SHORT_TIMEOUT);
		t.Dispose();
		//
		// Ditto, unit time units.
		//
		t = new Timer(this.timerCallbackDelegate, this, (uint)0, (uint)0);
		this.checkTimeout(0);
		this.checkTimeout(Timeout.Infinite);
		t.Dispose();
		t = new Timer(this.timerCallbackDelegate, this,
			(uint)SHORT_TIMEOUT, (uint)0);
		this.checkTimeout(SHORT_TIMEOUT);
		this.checkTimeout(Timeout.Infinite);
		t.Dispose();
		//
		// Ditto, long time units.
		//
		t = new Timer(this.timerCallbackDelegate, this, (long)0, (long)0);
		this.checkTimeout(0);
		this.checkTimeout(Timeout.Infinite);
		t.Dispose();
		t = new Timer(this.timerCallbackDelegate,
			this, (long)SHORT_TIMEOUT, (long)0);
		this.checkTimeout(SHORT_TIMEOUT);
		this.checkTimeout(Timeout.Infinite);
		t.Dispose();
		//
		// Ditto, TimeSpan time units.
		//
		t = new Timer(this.timerCallbackDelegate, this,
			TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(0));
		this.checkTimeout(0);
		this.checkTimeout(Timeout.Infinite);
		t.Dispose();
		t = new Timer(this.timerCallbackDelegate, this,
			TimeSpan.FromMilliseconds(SHORT_TIMEOUT),
			TimeSpan.FromMilliseconds(0));
		this.checkTimeout(SHORT_TIMEOUT);
		this.checkTimeout(Timeout.Infinite);
		t.Dispose();
	}

	//
	// Check that all forms of Dispose() work as advertised.
	//
	public void TestTimerDispose()
	{
		if (!TestThread.IsThreadingSupported)
			return;
		Timer t = new Timer(this.timerCallbackDelegate, this, SHORT_TIMEOUT, SHORT_TIMEOUT);
		t.Dispose();
		t.Dispose();
		Assert(!t.Change(0, Timeout.Infinite));
		this.checkTimeout(Timeout.Infinite);
		AutoResetEvent autoResetEvent = new AutoResetEvent(false);
		t = new Timer(this.timerCallbackDelegate, this, SHORT_TIMEOUT, SHORT_TIMEOUT);
		Assert(t.Dispose(autoResetEvent));
		Assert(!t.Dispose(autoResetEvent));
		Assert(autoResetEvent.WaitOne(SHORT_TIMEOUT/2, false));
		Assert(this.timeTimerExpired == DateTime.MaxValue);
		try
		{
			this.timer.Dispose(null);
			Assert("Expected ArgumentNullException", false);
		}
		catch (ArgumentNullException)
		{
		}
	}

	//
	// Test all combinations of due and period.
	//
	public void TestTimerTimeout()
	{
		if (!TestThread.IsThreadingSupported)
			return;
		//
		// Test various conbinations of due and period.
		//
		Assert(this.timer.Change(0, Timeout.Infinite));
		this.checkTimeout(0);
		this.checkTimeout(Timeout.Infinite);
		Assert(this.timer.Change(SHORT_TIMEOUT, Timeout.Infinite));
		this.checkTimeout(SHORT_TIMEOUT);
		this.checkTimeout(Timeout.Infinite);
		Assert(this.timer.Change(SHORT_TIMEOUT, 0));
		this.checkTimeout(SHORT_TIMEOUT);
		this.checkTimeout(Timeout.Infinite);
		Assert(this.timer.Change(0, SHORT_TIMEOUT));
		this.checkTimeout(0);
		this.checkTimeout(SHORT_TIMEOUT);
		this.checkTimeout(SHORT_TIMEOUT);
		this.checkTimeout(SHORT_TIMEOUT);
		Assert(this.timer.Change(SHORT_TIMEOUT, SHORT_TIMEOUT));
		this.checkTimeout(SHORT_TIMEOUT);
		this.checkTimeout(SHORT_TIMEOUT);
		this.checkTimeout(SHORT_TIMEOUT);
		//
		// Test setting the timeouts in various units.
		//
		Assert(this.timer.Change((uint)SHORT_TIMEOUT, (uint)SHORT_TIMEOUT));
		this.checkTimeout(SHORT_TIMEOUT);
		this.checkTimeout(SHORT_TIMEOUT);
		Assert(this.timer.Change((long)SHORT_TIMEOUT, (long)SHORT_TIMEOUT));
		this.checkTimeout(SHORT_TIMEOUT);
		this.checkTimeout(SHORT_TIMEOUT);
		Assert(this.timer.Change(
			TimeSpan.FromMilliseconds(SHORT_TIMEOUT),
			TimeSpan.FromMilliseconds(SHORT_TIMEOUT)));
		this.checkTimeout(SHORT_TIMEOUT);
		this.checkTimeout(SHORT_TIMEOUT);
		//
		// Test cancelling a timeout.
		//
		Assert(this.timer.Change(Timeout.Infinite, Timeout.Infinite));
		this.checkTimeout(Timeout.Infinite);
	}

	private void checkTimeout(int delay)
	{
		checkTimeoutCount += 1;
		DateTime dateTime;
		//
		// A zero delay.
		//
		if (delay == 0)
		{
			Assert(this.autoResetEvent.WaitOne(MIN_TIMEOUT, false));
			lock (this)
				this.timeTimerExpired = DateTime.MaxValue;
			return;
		}
		//
		// A non-Infinite delay.
		//
		if (delay != Timeout.Infinite)
		{
			long timeStarted = DateTime.UtcNow.Ticks;
			Assert(this.autoResetEvent.WaitOne(delay + MIN_TIMEOUT, false));
			lock (this)
				this.timeTimerExpired = DateTime.MaxValue;
			Assert(DateTime.UtcNow.Ticks - timeStarted > SHORT_TIMEOUT - MIN_TIMEOUT);
			return;
		}
		//
		// The timer should NOT expire.
		//
		Thread.Sleep(SHORT_TIMEOUT + MIN_TIMEOUT);
		lock (this)
		{
			dateTime = this.timeTimerExpired;
			this.timeTimerExpired = DateTime.MaxValue;
		}
		Assert(dateTime == DateTime.MaxValue);
	}

	private int recurseCount;
	private static void timerCallbackRecursive(Object state)
	{
		TestTimer testTimer = (TestTimer)state;
		lock (testTimer)
		{
			testTimer.timeTimerExpired = DateTime.Now;
			testTimer.autoResetEvent.Set();
			if (--testTimer.recurseCount > 0)
				Assert(testTimer.timer.Change(SHORT_TIMEOUT, Timeout.Infinite));
		}
	}

	//
	// Test the callback changing the timeout.
	//
	public void TestTimerChangeRecursive()
	{
		if (!TestThread.IsThreadingSupported)
			return;
		this.recurseCount = 3;
		this.timer.Dispose();
		this.timer = new Timer(new TimerCallback(timerCallbackRecursive),
			this, 0, SHORT_TIMEOUT);
		this.checkTimeout(0);
		this.checkTimeout(SHORT_TIMEOUT);
		this.checkTimeout(SHORT_TIMEOUT);
		this.checkTimeout(Timeout.Infinite);
	}

	//
	// Test timeouts fire in the correct order.
	//
	// Brute force is used here.  Every possible ordering of 1, 2, 3,
	// 4 and 5 timers is checked.
	//
	private class TimerTester : IDisposable
	{
		private Timer timer;
		private int id;
		private bool armed;
		private TestTimer testTimer;

		public TimerTester(TestTimer testTimer, int id)
		{
			this.testTimer = testTimer;
			this.id = id;
			this.timer = new Timer(new TimerCallback(expired),
				this, Timeout.Infinite, Timeout.Infinite);
			this.armed = false;
		}

		public void Arm()
		{
			this.timer.Change(
				SHORT_TIMEOUT + this.id * MIN_TIMEOUT, Timeout.Infinite);
			this.armed = true;
		}

		public void Disarm()
		{
			lock (this.testTimer)
			{
				this.timer.Change(Timeout.Infinite, Timeout.Infinite);
				this.armed = false;
			}
		}

		public void Dispose()
		{
			this.timer.Dispose();
		}

		public int Id
		{
			get { return this.id; }
		}

		public bool IsArmed
		{
			get { return this.armed; }
		}

		private static void expired(Object state)
		{
			TimerTester timerTester = (TimerTester)state;
			lock (timerTester.testTimer)
			{
				//
				// Did somebody else fail?
				//
				if (timerTester.testTimer.vectorIndex < 0)
					return;
				//
				// Were we supposed to go off?
				//
				if (!timerTester.armed || timerTester.testTimer.vectorIndex > timerTester.id)
				{
					timerTester.testTimer.vectorIndex = -timerTester.id;
					return;
				}
				//
				// We were supposed to be next.
				//
				timerTester.testTimer.vectorIndex = timerTester.id + 1;
				timerTester.testTimer.firedCount -= 1;
				if (timerTester.testTimer.firedCount == 0)
					Monitor.Pulse(timerTester.testTimer);
			}
		}
	}

	private TimerTester[] timerVector;
	private int vectorIndex;
	private int firedCount;

	public void TestTimerTimeoutOrdering()
	{
		if (!TestThread.IsThreadingSupported)
			return;
		System.Console.Write("this will take a while ... ");
		for (int i = 1; i <= 5; i += 1)
		{
			this.timerVector = new TimerTester[i];
			try
			{
				for (int j = 0; j < this.timerVector.Length; j += 1)
					this.timerVector[j] = new TimerTester(this, j + 1);
				permuteTimeoutOrdering(0);
			}
			finally
			{
				for (int j = 0; j < this.timerVector.Length; j += 1)
					this.timerVector[j].Dispose();
				this.timerVector = null;
			}
		}
	}

	private void permuteTimeoutOrdering(int index)
	{
		//
		// Have we finished permuting this.timerVector?
		//
		if (index == this.timerVector.Length)
		{
			//
			// Arm all the timers and do the test.
			//
			lock (this)
			{
				this.vectorIndex = 1;
				this.firedCount = this.timerVector.Length;
				for (int i = 0; i < this.timerVector.Length; i += 1)
					this.timerVector[i].Arm();
				Assert(Monitor.Wait(this,
					SHORT_TIMEOUT*2 + this.firedCount*MIN_TIMEOUT));
				Assert(this.firedCount == 0);
			}
			return;
		}
		//
		// Permute the vector at the passed index.
		//
		TimerTester first = this.timerVector[index];
		for (int i = index; i < this.timerVector.Length; i += 1)
		{
		 	this.timerVector[index] = this.timerVector[i];
			this.timerVector[i] = first;
			this.permuteTimeoutOrdering(index + 1);
		 	this.timerVector[i] = this.timerVector[index];
			this.timerVector[index] = first;
		}
	}


	//
	// Test timer cancelling.
	//
	// Again a brute force test.  Create a number of times, then delete every
	// 2nd one, every 3rd one, and so on.
	//
	public void TestTimerCancel()
	{
		if (!TestThread.IsThreadingSupported)
			return;
		this.timerVector = new TimerTester[10];
		for (int i = 0; i < this.timerVector.Length; i += 1)
		{
			int id = (i + this.timerVector.Length/2) % this.timerVector.Length + 1;
			this.timerVector[i] = new TimerTester(this, id);
		}
		try
		{
			for (int i = 2; i <= this.timerVector.Length; i += 1)
			{
				lock (this)
				{
					this.firedCount = this.timerVector.Length;
					this.vectorIndex = 1;
					for (int j = 0; j < this.timerVector.Length; j += 1)
						this.timerVector[j].Arm();
					for (int j = i - 1; j < this.timerVector.Length; j += i)
					{
						this.timerVector[j].Disarm();
						this.firedCount -= 1;
					}
					Assert(Monitor.Wait(this,
						SHORT_TIMEOUT*2 + this.timerVector.Length*MIN_TIMEOUT));
				}

			}
		}
		finally
		{
			for (int i = 0; i < this.timerVector.Length; i += 1)
				this.timerVector[i].Dispose();
			this.timerVector = null;
		}
	}
}
