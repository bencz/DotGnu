/*
 * TestSystem.cs - Tests for the "System" namespace.
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

using CSUnit;
using System;

public class TestSystem
{

	public static TestSuite Suite()
			{
				TestSuite fullSuite, suite;
				fullSuite = new TestSuite("System Assembly Tests");

				suite = new TestSuite("Uri Tests");
				suite.AddTests(typeof(TestUri));
				fullSuite.AddTest(suite);

				suite = new TestSuite("Network Tests");
				suite.AddTests(typeof(TestIPAddress));
				suite.AddTests(typeof(TestWebHeaderCollection));
				suite.AddTest(SuiteDiagnostics.Suite());
				fullSuite.AddTest(suite);

				return fullSuite;
			}

}; // class TestSystem
