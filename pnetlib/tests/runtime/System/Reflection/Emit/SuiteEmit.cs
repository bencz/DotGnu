/*
 * SuiteEmit.cs - Tests for the "System.Reflection.Emit" namespace.
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

#if CONFIG_REFLECTION_EMIT
public class SuiteEmit
{

	public static TestSuite Suite()
			{
				TestSuite suite = new TestSuite("Reflection.Emit tests");
			#if !ECMA_COMPAT
				suite.AddTests(typeof(TestEmit));
			#endif
				return suite;
			}

}; // class SuiteEmit
#endif // CONFIG_REFLECTION_EMIT

