/*
 * TestGCHandle.cs - Test the "GCHandle" class.
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
using System.Runtime.InteropServices;

public class TestGCHandle : TestCase
{
	private GCHandle unallocated;

	// Constructor.
	public TestGCHandle(String name)
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

	// Test the properties of uninitialized GCHandle's.
	public void TestGCHandleUninit()
			{
				Object target;

				// IsAllocated == false.
				Assert("Uninit (1)", !(unallocated.IsAllocated));

				// Cannot get or set the target.
				try
				{
					target = unallocated.Target;
					Fail("Uninit (2)");
				}
				catch(InvalidOperationException)
				{
					// Success.
				}
				try
				{
					unallocated.Target = new Object();
					Fail("Uninit (3)");
				}
				catch(InvalidOperationException)
				{
					// Success.
				}

				// Cannot get the pinned address.
				try
				{
					unallocated.AddrOfPinnedObject();
					Fail("Uninit (4)");
				}
				catch(InvalidOperationException)
				{
					// Success.
				}

				// The IntPtr version is zero.
				AssertEquals("Uninit (5)", IntPtr.Zero, (IntPtr)unallocated);
			}

	// Test allocation of GCHandle's.
	public void TestGCHandleAlloc()
			{
				GCHandle handle;
				Object target;

				// Allocate a normal handle.
				handle = GCHandle.Alloc(this);
				AssertSame("Alloc (1)", this, handle.Target);
				Assert("Alloc (2)", handle.IsAllocated);
				handle.Free();

				// Allocate a weak handle.
				Object obj = new Object();
				handle = GCHandle.Alloc(obj, GCHandleType.Weak);
				AssertSame("Alloc (3)", obj, handle.Target);
				Assert("Alloc (4)", handle.IsAllocated);
				handle.Free();

				// The handle is now invalid.
				try
				{
					target = handle.Target;
					Fail("Alloc (5)");
				}
				catch(InvalidOperationException)
				{
					// Success.
				}
			}

	// Test setting the target of a GCHandle.
	public void TestGCHandleSetTarget()
			{
				GCHandle handle;

				// Allocate a normal handle.
				handle = GCHandle.Alloc(this);
				AssertSame("SetTarget (1)", this, handle.Target);
				Assert("SetTarget (2)", handle.IsAllocated);

				// Change the target to another object.
				Object obj = new Object();
				handle.Target = obj;
				AssertSame("SetTarget (3)", obj, handle.Target);
				Assert("SetTarget (4)", handle.IsAllocated);

				// Free the handle.
				handle.Free();
			}

	// Test the retrieval of the pinned address.
	public void TestGCHandleAddrOfPinned()
			{
				GCHandle handle;

				// Cannot get the pinned address for normal handles.
				handle = GCHandle.Alloc(this);
				try
				{
					handle.AddrOfPinnedObject();
					Fail("AddrOfPinned (1)");
				}
				catch(InvalidOperationException)
				{
					// Success.
				}
				handle.Free();

				// Cannot get the pinned address for weak handles.
				handle = GCHandle.Alloc(this, GCHandleType.Weak);
				try
				{
					handle.AddrOfPinnedObject();
					Fail("AddrOfPinned (2)");
				}
				catch(InvalidOperationException)
				{
					// Success.
				}
				handle.Free();

				// Cannot get the pinned address for track resurrection handles.
				handle = GCHandle.Alloc
					(this, GCHandleType.WeakTrackResurrection);
				try
				{
					handle.AddrOfPinnedObject();
					Fail("AddrOfPinned (3)");
				}
				catch(InvalidOperationException)
				{
					// Success.
				}
				handle.Free();

				// Can get the pinned address for pinned handles.
				handle = GCHandle.Alloc
					(this, GCHandleType.Pinned);
				IntPtr addr = handle.AddrOfPinnedObject();
				Assert("AddrOfPinned (4)", (addr != IntPtr.Zero));
				handle.Free();
			}

	// Test conversion between IntPtr and GCHandle forms.
	public void TestGCHandleConvert()
			{
				Object obj = new Object();
				GCHandle handle;
				GCHandle handle2;
				GCHandle handle3;
				IntPtr ptr;

				// Allocate two normal handles.
				handle = GCHandle.Alloc(this);
				AssertSame("Convert (1)", this, handle.Target);
				Assert("Convert (2)", handle.IsAllocated);
				handle2 = GCHandle.Alloc(obj);
				AssertSame("Convert (3)", obj, handle2.Target);
				Assert("Convert (4)", handle2.IsAllocated);

				// Make sure that the raw handles are different.
				Assert("Convert (5)", (((IntPtr)handle) != ((IntPtr)handle2)));

				// Extract a handle and then wrap it up again.
				ptr = (IntPtr)handle;
				handle3 = (GCHandle)ptr;

				// Verify that handle and handle3 are identical.
				AssertSame("Convert (6)", handle.Target, handle3.Target);

				// Free the handles.
				handle.Free();
				handle2.Free();

				// "handle3" should now be invalid.  Because this is a
				// copy of "handle", the target goes null.
				try
				{
					obj = handle3.Target;
					AssertNull("Convert (7)", obj);
				}
				catch(InvalidOperationException)
				{
					// Success.
				}
			}

}; // class TestGCHandle
