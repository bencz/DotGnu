/*
 * TestObjectManager.cs - Test the "ObjectManager" class.
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

public class TestObjectManager : TestCase
{
	// Constructor.
	public TestObjectManager(String name)
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

	// Test object retrieval.
	public void TestObjectManagerGetObject()
			{
				ObjectManager mgr = new ObjectManager
					(null, new StreamingContext(StreamingContextStates.All));

				// Register a particular object and then look for it.
				Object obj = new Object();
				mgr.RegisterObject(obj, 1234);
				AssertSame("GetObject (1)", obj, mgr.GetObject(1234));

				// Look for a non-existent object.
				AssertNull("GetObject (2)", mgr.GetObject(1235));

				// Record a delayed fixup for some other object.
				mgr.RecordDelayedFixup(1236, "x", 1234);

				// Object 1236 shouldn't exist yet.
				AssertNull("GetObject (3)", mgr.GetObject(1236));
			}

	// Test object registration.
	public void TestObjectManagerRegisterObject()
			{
				ObjectManager mgr = new ObjectManager
					(null, new StreamingContext(StreamingContextStates.All));

				// Test error cases.
				try
				{
					mgr.RegisterObject(null, 0);
					Fail("RegisterObject (1)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
				try
				{
					mgr.RegisterObject(new Object(), 0);
					Fail("RegisterObject (2)");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Success.
				}
				try
				{
					mgr.RegisterObject(new Object(), -1);
					Fail("RegisterObject (3)");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Success.
				}
				mgr.RegisterObject(new Object(), 1);
				try
				{
					// Register a different object with the same ID.
					mgr.RegisterObject(new Object(), 1);
					Fail("RegisterObject (4)");
				}
				catch(SerializationException)
				{
					// Success.
				}
			}

#endif // CONFIG_SERIALIZATION

}; // class TestObjectManager
