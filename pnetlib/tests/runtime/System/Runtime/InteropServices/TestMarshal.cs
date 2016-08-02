/*
 * TestMarshal.cs - Test the "Marshal" class.
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
using System.Reflection;
using System.Runtime.InteropServices;

public class TestMarshal : TestCase
{
	// Test values.
	private static readonly byte[] foobar =
		{(byte)'f', (byte)'o', (byte)'o', (byte)'b', (byte)'a', (byte)'r'};
	private static readonly long[] numbers =
		{1, 2, 3, 4, 5, 6};

	// Constructor.
	public TestMarshal(String name)
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

	// Test HGlobal memory allocation and reallocation.
	public void TestMarshalHGlobal()
			{
				int size = 16384;
				int size2 = size + 1024;
				IntPtr ptr;
				int posn;

				// Allocate a 16k block of memory.
				ptr = Marshal.AllocHGlobal(size);
				Assert("NullCheck (1)", (ptr != IntPtr.Zero));

				// Verify that the block is all-zeroes.
				for(posn = 0; posn < size; ++posn)
				{
					if(Marshal.ReadByte(ptr, posn) != 0)
					{
						Fail("ZeroCheck (" + posn.ToString() + ")");
					}
				}

				// Fill the block with 0xDD.
				for(posn = 0; posn < size; ++posn)
				{
					Marshal.WriteByte(ptr, posn, (byte)0xDD);
				}

				// Verify that the block is now all-0xDD.
				for(posn = 0; posn < size; ++posn)
				{
					if(Marshal.ReadByte(ptr, posn) != 0xDD)
					{
						Fail("SetCheck (" + posn.ToString() + ")");
					}
				}

				// Re-allocate the block to a larger size.
				ptr = Marshal.ReAllocHGlobal(ptr, new IntPtr(size2));
				Assert("NullCheck (2)", (ptr != IntPtr.Zero));

				// Verify that the bottom part of the block is still all-0xDD.
				for(posn = 0; posn < size; ++posn)
				{
					if(Marshal.ReadByte(ptr, posn) != 0xDD)
					{
						Fail("SetCheck2 (" + posn.ToString() + ")");
					}
				}

				// Fill from the mid-point to the end with 0xAA.
				for(posn = size / 2; posn < size2; ++posn)
				{
					Marshal.WriteByte(ptr, posn, (byte)0xAA);
				}

				// Re-validate the block's contents.
				for(posn = 0; posn < size / 2; ++posn)
				{
					if(Marshal.ReadByte(ptr, posn) != 0xDD)
					{
						Fail("SetCheck3 (" + posn.ToString() + ")");
					}
				}
				for(posn = size / 2; posn < size2; ++posn)
				{
					if(Marshal.ReadByte(ptr, posn) != 0xAA)
					{
						Fail("SetCheck4 (" + posn.ToString() + ")");
					}
				}

				// Re-allocate the block to a smaller size.
				size2 = size * 3 / 4;
				ptr = Marshal.ReAllocHGlobal(ptr, new IntPtr(size2));
				Assert("NullCheck (3)", (ptr != IntPtr.Zero));

				// Make sure the block's contents are still correct.
				for(posn = 0; posn < size / 2; ++posn)
				{
					if(Marshal.ReadByte(ptr, posn) != 0xDD)
					{
						Fail("SetCheck5 (" + posn.ToString() + ")");
					}
				}
				for(posn = size / 2; posn < size2; ++posn)
				{
					if(Marshal.ReadByte(ptr, posn) != 0xAA)
					{
						Fail("SetCheck6 (" + posn.ToString() + ")");
					}
				}

				// Free the block.
				Marshal.FreeHGlobal(ptr);
			}

	// Test copies from managed to unmanaged.
	public unsafe void TestMarshalCopyToUnmanaged()
			{
				// Check exception conditions.
				try
				{
					Marshal.Copy((byte[])null, 0, new IntPtr(3), 0);
					Fail("NullCheck (1)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
				try
				{
					Marshal.Copy(new int [3], 0, IntPtr.Zero, 0);
					Fail("NullCheck (2)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
				try
				{
					Marshal.Copy(new long [3], -1, new IntPtr(3), 0);
					Fail("RangeCheck (3)");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Success.
				}
				try
				{
					Marshal.Copy(new byte [3], 0, new IntPtr(3), 4);
					Fail("RangeCheck (4)");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Success.
				}
				try
				{
					Marshal.Copy(new byte [3], 3, new IntPtr(3), 1);
					Fail("RangeCheck (5)");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Success.
				}

				// Allocate a block and copy "foobar" into it.
				IntPtr ptr = Marshal.AllocHGlobal(6);
				Marshal.Copy(foobar, 0, ptr, 6);

				// Check that the block does indeed contain "foobar".
				int posn;
				for(posn = 0; posn < 6; ++posn)
				{
					if(Marshal.ReadByte(ptr, posn) != foobar[posn])
					{
						Fail("SetCheck1 (" + posn.ToString() + ")");
					}
				}
				Marshal.FreeHGlobal(ptr);

				// Allocate another block and copy numbers into it.
				ptr = Marshal.AllocHGlobal(6 * sizeof(long));
				Marshal.Copy(numbers, 0, ptr, 6);

				// Check that the block does indeed contain the numbers.
				for(posn = 0; posn < 6; ++posn)
				{
					if(Marshal.ReadInt64(ptr, posn * 8) != numbers[posn])
					{
						Fail("SetCheck2 (" + posn.ToString() + ")");
					}
				}
				Marshal.FreeHGlobal(ptr);
			}

	// Test copies from unmanaged to managed.
	public unsafe void TestMarshalCopyToManaged()
			{
				// Check exception conditions.
				try
				{
					Marshal.Copy(new IntPtr(3), (byte[])null, 0, 0);
					Fail("NullCheck (1)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
				try
				{
					Marshal.Copy(IntPtr.Zero, new int [3], 0, 0);
					Fail("NullCheck (2)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
				try
				{
					Marshal.Copy(new IntPtr(3), new long [3], -1, 0);
					Fail("RangeCheck (3)");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Success.
				}
				try
				{
					Marshal.Copy(new IntPtr(3), new byte [3], 0, 4);
					Fail("RangeCheck (4)");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Success.
				}
				try
				{
					Marshal.Copy(new IntPtr(3), new byte [3], 3, 1);
					Fail("RangeCheck (5)");
				}
				catch(ArgumentOutOfRangeException)
				{
					// Success.
				}

				// Allocate a block and copy "foobar" into it.
				IntPtr ptr = Marshal.AllocHGlobal(6);
				int posn;
				for(posn = 0; posn < 6; ++posn)
				{
					Marshal.WriteByte(ptr, posn, foobar[posn]);
				}

				// Copy the data out into a byte buffer.
				byte[] buffer = new byte [6];
				Marshal.Copy(ptr, buffer, 0, 6);

				// Check that the buffer does indeed contain "foobar".
				for(posn = 0; posn < 6; ++posn)
				{
					if(buffer[posn] != foobar[posn])
					{
						Fail("SetCheck1 (" + posn.ToString() + ")");
					}
				}
				Marshal.FreeHGlobal(ptr);

				// Allocate another block and copy numbers into it.
				ptr = Marshal.AllocHGlobal(6 * sizeof(long));
				for(posn = 0; posn < 6; ++posn)
				{
					Marshal.WriteInt64(ptr, posn * 8, numbers[posn]);
				}

				// Copy the data out into a long buffer.
				long[] lbuffer = new long [6];
				Marshal.Copy(ptr, lbuffer, 0, 6);

				// Check that the buffer does indeed contain the numbers.
				for(posn = 0; posn < 6; ++posn)
				{
					if(lbuffer[posn] != numbers[posn])
					{
						Fail("SetCheck2 (" + posn.ToString() + ")");
					}
				}
				Marshal.FreeHGlobal(ptr);
			}

	// Test structure for "OffsetOf" and "SizeOf".
	private struct OffsetStruct
	{
		public int x;
		public int y;
	};

	// Test the "OffsetOf" method.
	public void TestMarshalOffsetOf()
			{
				// Simple tests that should succeed.
				AssertEquals("OffsetOf (1)", IntPtr.Zero,
							 Marshal.OffsetOf(typeof(OffsetStruct), "x"));
				AssertEquals("OffsetOf (2)", new IntPtr(4),
							 Marshal.OffsetOf(typeof(OffsetStruct), "y"));

				// Test exception cases.
				try
				{
					Marshal.OffsetOf(null, "x");
					Fail("OffsetOf (3)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
				try
				{
					Marshal.OffsetOf(typeof(OffsetStruct), null);
					Fail("OffsetOf (4)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
			#if CONFIG_REFLECTION && !ECMA_COMPAT
				try
				{
					// TypeDelegator is not a runtime type.
					Marshal.OffsetOf(new TypeDelegator(typeof(int)), "value_");
					Fail("OffsetOf (5)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
			#endif
				try
				{
					Marshal.OffsetOf(typeof(OffsetStruct), "z");
					Fail("OffsetOf (6)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
			}

	// Test the "SizeOf" method.
	public void TestMarshalSizeOf()
			{
				// Simple tests that should succeed.
				Assert("SizeOf (1)",
				       (Marshal.SizeOf(typeof(OffsetStruct)) >= 8));
				AssertEquals("SizeOf (2)", 4, Marshal.SizeOf((Object)3));

				// Test exception cases.
				try
				{
					Marshal.SizeOf((Type)null);
					Fail("SizeOf (3)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
			#if CONFIG_REFLECTION && !ECMA_COMPAT
				try
				{
					Marshal.SizeOf(new TypeDelegator(typeof(int)));
					Fail("SizeOf (4)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
			#endif
				try
				{
					Marshal.SizeOf((Object)null);
					Fail("SizeOf (5)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
			}

}; // class TestMarshal
