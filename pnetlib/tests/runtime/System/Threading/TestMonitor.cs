/*
 * TestMonitor.cs - Tests for the "Monitor" class.
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
using System.Collections;
using System.Threading;

public class TestMonitor
	: TestCase
{
	public TestMonitor(String name)
		: base(name)
	{
	}
	protected override void Setup()
	{
	}

	protected override void Cleanup()
	{
	}

	public void TestMonitorSingleThreaded()
	{
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}

		object o = new object();
		
		Monitor.Enter(o);
		Monitor.Enter(o);
		Monitor.Exit(o);
		Monitor.Exit(o);
	}	

	public void TestMonitorExitNoEnter()
	{
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}
		
		object o = new object();
		
		try
		{
			Monitor.Exit(o);
			
			Assert("Expected SynchronizationLockException", false);
		}
		catch (SynchronizationLockException)
		{
		}
	}

	public void TestMonitorEnterExitMismatch()
	{
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}
		
		object o = new object();

		try
		{
			Monitor.Enter(o);
			Monitor.Exit(o);
			Monitor.Exit(o);

			Assert("Expected SynchronizationLockException", false);
		}
		catch (SynchronizationLockException)
		{
		}
	}

	public void TestMonitorEnterExitMultiple()
	{
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}
	
		object o1 = new object();
		object o2 = new object();
		
		Monitor.Enter(o1);
		Monitor.Enter(o2);
		Monitor.Exit(o1);
		Monitor.Exit(o2);
	}

	bool flag;
	volatile bool xseen = false;
	object monitor = new object();
	
	private void ExclusionRun1()
	{
		lock (monitor)
		{	
			xseen = true;
			
			Thread.Sleep(800);
			
			flag = true;
		}		
	}
		
	private void ExclusionRun2()
	{
		/* Wait for thread1 to obtain lock */
		
		while (!xseen)
		{
			Thread.Sleep(10);
		}
		
		lock (monitor)
		{
			/* Fails if lock didn't wait for thread1 */
			
			failed = !flag;	
		}
	}
	
	public void TestMonitorExclusion()
	{
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}
		
		Thread thread1, thread2;
		
		flag = false;
		failed = true;
		xseen = false;
		
		thread1 = new Thread(new ThreadStart(ExclusionRun1));
		thread2 = new Thread(new ThreadStart(ExclusionRun2));	
		
		thread1.Start();
		thread2.Start();

		thread1.Join();
		thread2.Join();
		
		Assert("Exclusion failed", !failed);
	}

	/*
	 * Variables used for monitor thrashing..
	 */

	private int state = 0;
	private bool failed = false;
	private bool stop = false;
	
	private void Run1()
	{
		while (!stop)
		{
			lock (typeof(TestMonitor))
			{
				bool ok = false;
			
				// state can be either 0 or 1 but it can't
				// be 1 on the first check and then 0 on 
				// the second check.
				
				if (state == 0)
				{
					// OK!
					
					ok = true;
				}
				else
				{
					// Allow other threads to preempt.
					
					Thread.Sleep(5);
					
					if (state == 1)
					{
						ok = true;
					}
				}
				
				if (!ok)
				{
					failed = true;
					stop = true;

					break;
				}
			}
		}
	}
	
	private void Run2()
	{
		while (!stop)
		{
			lock (typeof(TestMonitor))
			{
				if (state == 1)
				{
					state = 0;
				}
				else
				{
					state = 1;
				}
			}
			
			// Allow other threads to preempt
			Thread.Sleep(15);
		}
	}
	
	public void TestMonitorMultiThreadedThrash()
	{
		Thread thread1, thread2;
		
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}
		
		thread1 = new Thread(new ThreadStart(Run1));
		thread2 = new Thread(new ThreadStart(Run2));

		state = 0;
		failed = false;
		stop = false;

		Console.Write("Thrashing will take 1 second ... ");
				
		thread1.Start();
		thread2.Start();
		
		Thread.Sleep(1000);
		
		stop = true;

		thread1.Join();
		thread2.Join();
		
		AssertEquals("Monitor locking", failed, false);
	}

	public void TestMonitorExtremeMultiThreadedThrash()
	{
		Thread[] threads;
			
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}

		state = 0;
		failed = false;
		stop = false;		

		threads = new Thread[10];

		Console.Write("Thrashing will take 1 second ... ");
				
		for (int i = 0; i < 10; i++)
		{
			if (i % 2 == 0)
			{
				threads[i] = new Thread(new ThreadStart(Run1));
			}
			else
			{
				threads[i] = new Thread(new ThreadStart(Run2));
			}
			
			threads[i].Start();
		}

		Thread.Sleep(1000);
		
		stop = true;

		for (int i = 0; i < 10; i++)
		{
			threads[i].Join();
		}
		
		AssertEquals("Monitor locking", failed, false);
	}
	
	public void TestMonitorWaitWithoutLock()
	{
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}

		object o = new object();
		
		try
		{
			Monitor.Wait(o);
		}
		catch (SynchronizationLockException)
		{
			return;
		}
		
		Fail("Monitor.Wait() without a lock should throw a synchronization lock exception");
	}
		
	public void TestMonitorWaitWithLockTimeout()
	{
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}

		object o = new object();
		
		try
		{
			lock (o)
			{
				Monitor.Wait(o, 100);
			}
		}
		catch (SynchronizationLockException)
		{
			Fail("Monitor.Wait() without a lock should throw a synchronization lock exception");
			
			return;
		}	
	}
	
	private class TestEnterFalseLeave
	{
		bool e = false;
		public object o = new object();
		
		public void Run()
		{
			try
			{
				Monitor.Exit(o);
			}
			catch (SynchronizationLockException)
			{
				e = true;
			}
		}
		
		public bool Test()
		{
			Thread thread = new Thread(new ThreadStart(Run));
			
			Monitor.Enter(o);
						
			thread.Start();
			thread.Join();
			
			return e;
		}
	}
	
	public void TestMonitorEnterFalseLeave()
	{
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}

		Assert("Monitor.Exit should throw an exception if the thread doesn't own the lock", new TestEnterFalseLeave().Test());
	}
	
	public class TestWaitThenPulse
	{
		private volatile bool seen = false;
		private object o = new object();
		private ArrayList results = new ArrayList();
		
		private void AddResult(int value)
		{
			lock (this)
			{
				results.Add(value);
			}
		}
		
		public void Run1()
		{
			AddResult(1);
			
			lock (o)
			{
				seen = true;
				
				AddResult(2);
				
				Monitor.Wait(o);
				
				AddResult(6);
			}
		}
		
		public void Run2()
		{
			Console.Write("Waiting to pulse ... ");

			while (!seen)
			{
				Thread.Sleep(10);
			}

			AddResult(3);
			
			lock (o)
			{
				AddResult(4);
				
				Monitor.Pulse(o);
				
				AddResult(5);
			}		
		}
		
		public bool Test()
		{
			bool success = true;
			Thread thread = new Thread(new ThreadStart(Run2));
			
			thread.Start();
			Run1();
			
			if (results.Count == 6)
			{
				for (int i = 0; i < results.Count; i++)
				{
					if ((int)results[i] != i + 1)
					{
						success = false;
					}
				}
			}
			else
			{
				success = false;
			}
			
			return success;
		}
	}
	
	public void TestMonitorWaitThenPulse()
	{
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}
		
		Assert("Monitor.Wait doesn't work", new TestWaitThenPulse().Test());
	}

	/*
	 * Test that a Monitor re-aquires the lock if interrupted during a
	 * Wait().
	 */
	public void TestMonitorInterruptDuringWait()
	{
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}

		MonitorInterruptDuringWait test = new MonitorInterruptDuringWait();
		String result = test.testMonitorInterruptDuringWait();
		if (result != null)
	  		Assert(result, result == null);
	}

	public class MonitorInterruptDuringWait
	{
		Object o = new Object();
		bool seen;
		String result = "Oops - something went wrong!";

		public String testMonitorInterruptDuringWait()
		{
			Thread thread;
			lock (this)
			{
				thread = new Thread(new ThreadStart(threadFunc));
				thread.Start();
				this.seen = false;
				Monitor.Wait(this);
			}
			lock (this.o)
			{
				thread.Interrupt();
				Thread.Sleep(800);
				this.seen = true;
			}
			thread.Join();
			return this.result;
		}

		void threadFunc()
		{
			Monitor.Enter(this.o);
			lock (this)
			{
				Monitor.Pulse(this);
			}
			try
			{
				Monitor.Wait(this.o);
				this.result = "Expected System.Threading.ThreadInterruptedException";
				return;
			}
			catch (ThreadInterruptedException)
			{
			}
			if (!this.seen)
			{
				this.result = "Wait did not re-aquire lock after interrupt";
				return;
			}
			try
			{
				Monitor.Exit(this.o);
			}
			catch (System.Exception e)
			{
				this.result = "Got unexpected exception during Exit: " + e;
				return;
			}
			this.result = null;
		}
	}

	/*
	 * Test that Interrupt of Sleep() works.  Note: The MS doco on Sleep
	 * does NOT say that it will throw a ThreadInterruptedException.  However
	 * the example code for Thread.Interrupt demonstrates it can.
	 */
	public void TestMonitorInterruptDuringSleep()
	{
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}

		MonitorInterruptDuringSleep test = new MonitorInterruptDuringSleep();
		String result = test.testMonitorInterruptDuringSleep();
		if (result != null)
	  		Assert(result, result == null);
	}

	public class MonitorInterruptDuringSleep
	{
		String result = "Oops - something went wrong!";

		public String testMonitorInterruptDuringSleep()
		{
			Thread thread;
			thread = new Thread(new ThreadStart(threadFunc));
			thread.Start();
			while ((thread.ThreadState & ThreadState.WaitSleepJoin) == 0)
				continue;
			thread.Interrupt();
			thread.Join();
			return this.result;
		}

		void threadFunc()
		{
			try
			{
				Thread.Sleep(1000 * 1000);
				this.result = "Expected System.Threading.ThreadInterruptedException";
				return;
			}
			catch (ThreadInterruptedException)
			{
			}
			this.result = null;
		}
	}

	/*
	 * Test that an Interrupt outside of a lock hits the next attempt to lock.
	 */
	public void TestMonitorInterruptEnter()
	{
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}

		MonitorInterruptEnter test = new MonitorInterruptEnter();
		String result = test.testMonitorInterruptEnter();
		if (result != null)
	  		Assert(result, result == null);
	}

	public class MonitorInterruptEnter
	{
		Object o = new Object();
		bool seen = false;
		String result = "Oops - something went wrong!";

		public String testMonitorInterruptEnter()
		{
			Thread thread;
			lock (this)
			{
				thread = new Thread(new ThreadStart(threadFunc));
				thread.Start();
				Monitor.Wait(this);
			}
			thread.Interrupt();
			lock (this.o)
			{
				this.seen = true;
				Thread.MemoryBarrier();
				thread.Join();
			}
			return this.result;
		}

		void threadFunc()
		{
			try
			{
				Monitor.Enter(this.o);
			}
			catch (System.Exception e)
			{
				this.result = "Got unexpected exception during Enter: " + e;
				return;
			}
			lock (this)
			{
				Monitor.Pulse(this);
			}
			try
			{
				Monitor.Exit(this.o);
			}
			catch (System.Exception e)
			{
				this.result = "Got unexpected exception during Exit: " + e;
				return;
			}
			while (!this.seen)
			{
				Thread.MemoryBarrier();
			}
			try
			{
				Monitor.Enter(this.o);
				this.result = "Expected System.Threading.ThreadInterruptedException";
				return;
			}
			catch (ThreadInterruptedException)
			{
			}
			this.result = null;
		}
	}

	/*
	 * Test that an Abort breaks a Monitor.Enter wait.
	 */
	public void TestMonitorAbortEnter()
	{
		if (!TestThread.IsThreadingSupported)
			return;
		MonitorAbortEnter test = new MonitorAbortEnter();
		String result = test.testMonitorAbortEnter();
		if (result != null)
	  		Assert(result, result == null);
	}

	public class MonitorAbortEnter
	{
		Object o = new Object();
		bool seen = false;
		String result = "Oops - something went wrong!";

		public String testMonitorAbortEnter()
		{
			Thread thread;
			lock (this)
			{
				thread = new Thread(new ThreadStart(threadFunc));
				thread.Start();
				Monitor.Wait(this);
			}
			lock (this.o)
			{
				this.seen = true;
				Thread.MemoryBarrier();
				Thread.Sleep(800);
				thread.Abort();
				thread.Join();
			}
			return this.result;
		}

		void threadFunc()
		{
			try
			{
				Monitor.Enter(this.o);
			}
			catch (System.Exception e)
			{
				this.result = "Got unexpected exception during Enter: " + e;
				return;
			}
			try
			{
				lock (this)
				{
					try 
					{
						Monitor.Pulse(this);
					}
					catch (System.Exception e)
					{
						this.result = "Pulse throw exception: " + e;
						return;
					}
				}
			}
			catch (System.Exception e)
			{
				this.result = "lock throw exception: " + e;
				return;
			}
			try
			{
				Monitor.Exit(this.o);
			}
			catch (System.Exception e)
			{
				this.result = "Got unexpected exception during Exit: " + e;
				return;
			}
			while (!this.seen)
			{
				Thread.MemoryBarrier();
			}
			try 
			{
				this.result = "Monitor.Enter threw an exception we couldn't catch!";
				Monitor.Enter(this.o);
				this.result = "Expected System.Threading.ThreadAbortException";
				return;
			}
			catch (ThreadAbortException)
			{
				this.result = "Oops - something went wrong after catching ThreadAbortException";
				if (!this.seen)
				{
					this.result = "Wait did not re-aquire lock after abort";
					return;
				}
				try
				{
					System.Threading.Monitor.Exit(this.o);
					this.result = "Incorrectly gained monitor on abort while entering";
					return;
				}
				catch (SynchronizationLockException)
				{
					this.result = null;
				}
			}
			catch (System.Exception e)
			{
				this.result = "Monitor.Enter threw wrong exception: " + e;
				return;
			}
			this.result = "Abort was not automatically re-thrown in catch";
		}
	}

	/*
	 * Test that an Interrupt outside of a Sleep hits the next
	 * attempt to Sleep.
	 */
	public void TestMonitorInterruptSleep()
	{
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}

		MonitorInterruptSleep test = new MonitorInterruptSleep();
		String result = test.testMonitorInterruptSleep();
		if (result != null)
	  		Assert(result, result == null);
	}

	public class MonitorInterruptSleep
	{
		Object o = new Object();
		bool seen = false;
		String result = "Oops - something went wrong!";

		public String testMonitorInterruptSleep()
		{
			Thread thread;
			lock (this)
			{
				thread = new Thread(new ThreadStart(threadFunc));
				thread.Start();
				Monitor.Wait(this);
			}
			thread.Interrupt();
			this.seen = true;
			Thread.MemoryBarrier();
			thread.Join();
			return this.result;
		}

		void threadFunc()
		{
			lock (this)
			{
				Monitor.Pulse(this);
			}
			while (!this.seen)
			{
				Thread.MemoryBarrier();
			}
			try
			{
				Thread.Sleep(1000 * 1000);
				this.result = "Expected System.Threading.ThreadInterruptedException";
				return;
			}
			catch (ThreadInterruptedException)
			{
			}
			this.result = null;
		}
	}

	/*
	 * Test that a Monitor re-aquires the lock if aborted during a
	 * Wait().
	 */
	public void TestMonitorAbortDuringWait()
	{
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}

		MonitorAbortDuringWait test = new MonitorAbortDuringWait();
		String result = test.testMonitorAbortDuringWait();
		if (result != null)
	  		Assert(result, result == null);
	}

	public class MonitorAbortDuringWait
	{
		Object o = new Object();
		bool seen;
		String result = "Oops - something went wrong!";

		public String testMonitorAbortDuringWait()
		{
			Thread thread;
			lock (this)
			{
				thread = new Thread(new ThreadStart(threadFunc));
				thread.Start();
				this.seen = false;
				Monitor.Wait(this);
			}
			lock (this.o)
			{
				thread.Abort();
				Thread.Sleep(800);
				this.seen = true;
			}
			thread.Join();
			return result;
		}

		void threadFunc()
		{
			Monitor.Enter(this.o);
			try
			{
				lock (this)
				{
					try
					{
						Monitor.Pulse(this);
					}
					catch (System.Exception e)
					{
						this.result = "Pulse threw exception: " + e;
						return;
					}

				}
			}
			catch (System.Exception e)
			{
				this.result = "lock threw exception: " + e;
				return;
			}
			try 
			{
				this.result = "Monitor.Wait threw an exception we couldn't catch!";
				Monitor.Wait(this.o);
				this.result = "Expected System.Threading.ThreadAbortException";
				return;
			}
			catch (ThreadAbortException)
			{
				this.result = "Oops - something went wrong after catching ThreadAbortException";
				if (!this.seen)
				{
					this.result = "Wait did not re-aquire lock after abort";
					return;
				}
				try
				{
					System.Threading.Monitor.Exit(this.o);
					this.result = null;
				}
				catch (System.Exception e)
				{
					this.result = "Got unexpected exception during Exit: " + e;
					return;
				}
			}
			catch (System.Exception e)
			{
				this.result = "Monitor.Wait threw wrong exception: " + e;
				return;
			}
			this.result = "Abort was not automatically re-thrown in catch";
		}
	}

	/*
	 * Test that a thread can not be interrupted re-aquiring the monitor
	 * after a Wait().
	 */
	public void TestMonitorInterruptAfterWait()
	{
		if (!TestThread.IsThreadingSupported)
			return;
		MonitorInterruptAfterWait test = new MonitorInterruptAfterWait();
		String result = test.testMonitorInterruptAfterWait();
		if (result != null)
	  		Assert(result, result == null);
	}

	public class MonitorInterruptAfterWait
	{
		Object o = new Object();
		bool seen;
		String result = "Oops - something went wrong!";

		public String testMonitorInterruptAfterWait()
		{
			Thread thread;
			lock (this)
			{
				thread = new Thread(new ThreadStart(threadFunc));
				thread.Start();
				this.seen = false;
				Monitor.Wait(this);
			}
			lock (this.o)
			{
				Monitor.Pulse(o);
				Thread.Sleep(800);
				thread.Interrupt();
				Thread.Sleep(800);
				this.seen = true;
			}
			thread.Join();
			return this.result;
		}

		void threadFunc()
		{
			Monitor.Enter(this.o);
			lock (this)
			{
				Monitor.Pulse(this);
			}
			try
			{
				Monitor.Wait(this.o);
				this.result = "Expected System.Threading.ThreadInterruptedException";
				return;
			}
			catch (ThreadInterruptedException)
			{
			}
			if (!this.seen)
			{
				this.result = "Wait did not re-aquire lock after interrupt";
				return;
			}
			try
			{
				Monitor.Exit(this.o);
			}
			catch (System.Exception e)
			{
				this.result = "Got unexpected exception during Exit: " + e;
				return;
			}
			this.result = null;
		}
	}

	/*
	 * Test that a thread can not be aborted re-aquiring the monitor
	 * after a Wait().
	 */
	public void TestMonitorAbortAfterWait()
	{
		if (!TestThread.IsThreadingSupported)
			return;
		MonitorAbortAfterWait test = new MonitorAbortAfterWait();
		String result = test.testMonitorAbortAfterWait();
		if (result != null)
	  		Assert(result, result == null);
	}

	public class MonitorAbortAfterWait
	{
		Object o = new Object();
		bool seen;
		String result = "Oops - something went wrong!";

		public String testMonitorAbortAfterWait()
		{
			Thread thread;
			lock (this)
			{
				thread = new Thread(new ThreadStart(threadFunc));
				thread.Start();
				this.seen = false;
				Monitor.Wait(this);
			}
			lock (this.o)
			{
				Monitor.Pulse(o);
				Thread.Sleep(800);
				thread.Abort();
				Thread.Sleep(800);
				this.seen = true;
			}
			thread.Join();
			return this.result;
		}

		void threadFunc()
		{
			Monitor.Enter(this.o);
			try
			{
				lock (this)
				{
					try
					{
						Monitor.Pulse(this);
					}
					catch (System.Exception e)
					{
						this.result = "Pulse threw an exception: " + e;
						return;
					}
				}
			}
			catch (System.Exception e)
			{
				this.result = "lock threw an exception: " + e;
				return;
			}
			try 
			{
				this.result = "Monitor.Wait threw an exception we couldn't catch!";
				Monitor.Wait(this.o);
				this.result = "Expected System.Threading.ThreadAbortException";
				return;
			}
			catch (ThreadAbortException)
			{
				this.result = "Oops - something went wrong after catching ThreadAbortException";
				if (!this.seen)
				{
					this.result = "Wait did not re-aquire lock after abort";
				Thread.Sleep(800);
					return;
				}
				try
				{
					System.Threading.Monitor.Exit(this.o);
					this.result = "finally not executed";
				}
				catch (System.Exception e)
				{
					this.result = "Got unexpected exception during Exit: " + e;
					return;
				}
				finally
				{
					if (this.result == "finally not executed")
					{				
						this.result = null;
					}
				}
			}
			catch (System.Exception e)
			{
				this.result = "Monitor.Wait threw wrong exception: " + e;
				return;
			}
			this.result = "Abort was not automatically re-thrown in catch";
		}
	}
}
