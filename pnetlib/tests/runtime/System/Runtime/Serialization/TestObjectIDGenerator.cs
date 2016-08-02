/*
 * TestObjectIDGenerator.cs - Test the "ObjectIDGenerator" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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
using System.Runtime.Serialization;

public class TestObjectIDGenerator : TestCase
{
	// Constructor.
	public TestObjectIDGenerator(String name)
			: base(name)
			{
				// Nothing to do here.
			}

	// Set up for the tests.
	protected override void Setup()
			{
				// Nothing to do here.
			}

	// Clean up after the tests.
	protected override void Cleanup()
			{
				// Nothing to do here.
			}

#if CONFIG_SERIALIZATION

	// Test simple object identifier generation.
	public void TestObjectIDGeneratorSimple()
			{
				ObjectIDGenerator gen = new ObjectIDGenerator();
				Object obj1 = new Object();
				Object obj2 = new Object();
				Object obj3 = new Object();
				bool firstTime;

				AssertEquals("Simple (1)", 1, gen.GetId(obj1, out firstTime));
				Assert("Simple (2)", firstTime);

				AssertEquals("Simple (3)", 2, gen.GetId(obj2, out firstTime));
				Assert("Simple (4)", firstTime);

				AssertEquals("Simple (5)", 2, gen.GetId(obj2, out firstTime));
				Assert("Simple (6)", !firstTime);

				AssertEquals("Simple (7)", 1, gen.HasId(obj1, out firstTime));
				Assert("Simple (8)", !firstTime);

				AssertEquals("Simple (9)", 0, gen.HasId(obj3, out firstTime));
				Assert("Simple (10)", firstTime);

				AssertEquals("Simple (11)", 3, gen.GetId(obj3, out firstTime));
				Assert("Simple (12)", firstTime);
			}

	// Add large numbers of objects to an ObjectIDGenerator.
	public void TestObjectIDGeneratorMany()
			{
				ObjectIDGenerator gen = new ObjectIDGenerator();
				Object[] list = new Object [1024];
				bool firstTime;
				int posn;
				for(posn = 0; posn < 1024; ++posn)
				{
					list[posn] = new Object();
					AssertEquals("Many (1)", posn + 1,
								 gen.GetId(list[posn], out firstTime));
					Assert("Many (2)", firstTime);
					AssertEquals("Many (3)", (posn / 2) + 1,
								 gen.GetId(list[posn / 2], out firstTime));
					Assert("Many (4)", !firstTime);
					AssertEquals("Many (5)", (posn / 2) + 1,
								 gen.HasId(list[posn / 2], out firstTime));
					Assert("Many (6)", !firstTime);
				}
			}

	// Check that value-equal objects are given distinct object identifiers.
	public void TestObjectIDGeneratorDistinct()
			{
				ObjectIDGenerator gen = new ObjectIDGenerator();
				Object obj1 = (Object)3;
				Object obj2 = (Object)3;
				long id1;
				long id2;
				bool firstTime;
				id1 = gen.GetId(obj1, out firstTime);
				id2 = gen.GetId(obj2, out firstTime);
				Assert("Distinct (1)", (id1 != id2));
			}

	// Test exception cases.
	public void TestObjectIDGeneratorExceptions()
			{
				ObjectIDGenerator gen = new ObjectIDGenerator();
				bool firstTime;
				try
				{
					gen.GetId(null, out firstTime);
					Fail("Exceptions (1)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
				try
				{
					gen.HasId(null, out firstTime);
					Fail("Exceptions (1)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
			}

#endif // CONFIG_SERIALIZATION

}; // class TestObjectIDGenerator
