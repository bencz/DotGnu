/*
 * SuiteSystem.cs - Tests for the "System" namespace.
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

using CSUnit;
using System;

public class SuiteSystem
{

	public static TestSuite Suite()
			{
				TestSuite suite = new TestSuite("Core Class Tests");
#if CONFIG_FRAMEWORK_2_0	
				suite.AddTests(typeof(TestActivationArguments));
				suite.AddTests(typeof(TestActivationContext));
#endif
				suite.AddTests(typeof(TestAppDomain));
#if CONFIG_FRAMEWORK_2_0	
				suite.AddTests(typeof(TestApplicationId));
				suite.AddTests(typeof(TestApplicationIdentity));
#endif
				suite.AddTests(typeof(TestArgIterator));
				suite.AddTests(typeof(TestArray));
				suite.AddTests(typeof(TestAttribute));
				suite.AddTests(typeof(TestBoolean));
				suite.AddTests(typeof(TestChar));
				suite.AddTests(typeof(TestConvert));
				suite.AddTests(typeof(TestDecimal));
				suite.AddTests(typeof(TestDelegate));
				suite.AddTests(typeof(TestDouble));
				suite.AddTests(typeof(TestMath));
				suite.AddTests(typeof(TestSByte));
				suite.AddTests(typeof(TestSingle));
				suite.AddTests(typeof(TestString));
			#if !ECMA_COMPAT
				suite.AddTests(typeof(TestGuid));
			#endif
				suite.AddTests(typeof(TestSystemExceptions));
				suite.AddTests(typeof(TestVersion));
				return suite;
			}

}; // class SuiteSystem
