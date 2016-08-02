/*
 * TestMutex.cs - Tests for the "Mutex" class.
 *
 * Copyright (C) 2004  Free Software Foundation.
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

public class TestMutex
	: TestCase
{
	public TestMutex(String name)
		: base(name)
	{
	}
	protected override void Setup()
	{
	}

	protected override void Cleanup()
	{
	}

	bool flag;
	bool failed;
	string reason;
	volatile bool xseen = false;
	Mutex mutex;
	
	private void ExclusionRun1()
	{
		mutex.WaitOne();
		{	
			xseen = true;
			
			Thread.Sleep(800);
			
			flag = true;
		}
		mutex.ReleaseMutex();
	}

	private void ExclusionRun1NoRelease()
	{
		mutex.WaitOne();
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
		
		if (!mutex.WaitOne(5000, false))
		{
			failed = true;
			reason = "Thread 1 failed to automatically release mutex when exiting";
			
			return;
		}
		
		/* Fails if lock didn't wait for thread1 */
		
		failed = !flag;	
		reason = "Exclusion failed";

		mutex.ReleaseMutex();
	}

	///
	/// Tests if a mutex synchronizes threads.
	///	
	public void TestMutexExclusion()
	{
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}
		
		Thread thread1, thread2;
		
		flag = false;
		failed = true;
		xseen = false;
		
		mutex = new Mutex();

		thread1 = new Thread(new ThreadStart(ExclusionRun1));
		thread2 = new Thread(new ThreadStart(ExclusionRun2));	
		
		thread1.Start();
		thread2.Start();

		thread1.Join();
		thread2.Join();
		
		Assert("Exclusion failed", !failed);
	}

	///
	/// Tests to see if a thread releases all its mutexes before it
	/// exits.
	///
	public void TestMutexExclusionNoRelease()
	{
		if (!TestThread.IsThreadingSupported)
		{
			return;
		}
		
		Thread thread1, thread2;
		
		flag = false;
		failed = true;
		xseen = false;

		mutex = new Mutex();
		
		thread1 = new Thread(new ThreadStart(ExclusionRun1NoRelease));
		thread2 = new Thread(new ThreadStart(ExclusionRun2));	
		
		thread1.Start();
		thread2.Start();

		thread1.Join();
		thread2.Join();
		
		Assert(reason, !failed);
	}

}
