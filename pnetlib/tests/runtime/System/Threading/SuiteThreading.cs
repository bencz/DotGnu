/*
 * SuiteThreading.cs - Tests for the "System.Threading" namespace.
 *
 * Copyright (C) 2002  Free Software Foundation
 * 
 * Authors:  Thong Nguyen (tum@veridicus.com)
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

public class SuiteThreading
{
	public static TestSuite Suite()
	{
		TestSuite suite = new TestSuite("Threading Tests");
		
	#if !ECMA_COMPAT
		suite.AddTests(typeof(TestWaitHandle));
		suite.AddTests(typeof(TestManualResetEvent));
		suite.AddTests(typeof(TestAutoResetEvent));
		suite.AddTests(typeof(TestTimer));
		suite.AddTests(typeof(TestMutex));
	#endif
	
		suite.AddTests(typeof(TestThread));
		suite.AddTests(typeof(TestMonitor));
		
		return suite;
	}
}
