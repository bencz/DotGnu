/*
 * TestGuid.cs - Tests for the "Guid" class.
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

#if !ECMA_COMPAT

public class TestGuid : TestCase
{
	// Constructor.
	public TestGuid(String name)
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

	// Test that the empty Guid is actually zero.
	public void TestGuidEmpty()
			{
				AssertEquals("00000000-0000-0000-0000-000000000000",
							 Guid.Empty.ToString());
			}

	// Various test strings.
	private const String testGuid1 = "{c0093632-b44c-4cf7-a279-d82fe8a8890d}";
	private const String testGuid2 = "{C0093632-B44C-4CF7-A279-D82FE8A8890D}";
	private const String testGuid3 = "C0093632-B44C-4CF7-A279-D82FE8A8890D";
	private const String testGuid4 =
		"{0xc0093632,0xb44c,0x4cf7,{0xa2},{0x79},{0xd8},{0x2f},{0xe8},{0xa8},{0x89},{0x0d}}";
	private const String testGuid5 = "{C0093632-B44C-4CF7-A279-D82FE8A8890D";
	private const String testGuid6 = "{0xC0093632-B44C-4CF7-A279-D82FE8A8890D";
	private const String testGuid7 = "c0093632-b44c-4cf7-a279-d82fe8a8890d";
	private const String testGuid8 = "c0093632b44c4cf7a279d82fe8a8890d";
	private const String testGuid9 = "(c0093632-b44c-4cf7-a279-d82fe8a8890d)";

	// Test the parsing of various Guid strings.
	public void TestGuidParse()
			{
				// Tests that are expected to succeed.
				Guid expected = new Guid(unchecked((int)0xc0093632),
									     unchecked((short)0xb44c),
									     unchecked((short)0x4cf7),
									     unchecked((byte)0xa2),
										 unchecked((byte)0x79),
									     unchecked((byte)0xd8),
										 unchecked((byte)0x2f),
									     unchecked((byte)0xe8),
										 unchecked((byte)0xa8),
									     unchecked((byte)0x89),
										 unchecked((byte)0x0d));
				Guid guid = new Guid(testGuid1);
				AssertEquals("guid1", expected, guid);
				guid = new Guid(testGuid2);
				AssertEquals("guid2", expected, guid);
				guid = new Guid(testGuid3);
				AssertEquals("guid3", expected, guid);
				guid = new Guid(testGuid4);
				AssertEquals("guid4", expected, guid);

				// Tests that are expected to fail.
				try
				{
					guid = new Guid((String)null);
					Fail("guid5");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
				try
				{
					guid = new Guid("xx");
					Fail("guid6");
				}
				catch(FormatException)
				{
					// Success.
				}
				try
				{
					guid = new Guid(testGuid5);
					Fail("guid7");
				}
				catch(FormatException)
				{
					// Success.
				}
				try
				{
					guid = new Guid(testGuid6);
					Fail("guid8");
				}
				catch(FormatException)
				{
					// Success.
				}
			}

	// Test the formatting of Guid's.
	public void TestGuidFormat()
			{
				// Tests that are expected to succeed.
				Guid guid = new Guid(unchecked((int)0xc0093632),
								     unchecked((short)0xb44c),
								     unchecked((short)0x4cf7),
								     unchecked((byte)0xa2),
									 unchecked((byte)0x79),
								     unchecked((byte)0xd8),
									 unchecked((byte)0x2f),
								     unchecked((byte)0xe8),
									 unchecked((byte)0xa8),
								     unchecked((byte)0x89),
									 unchecked((byte)0x0d));
				AssertEquals("guid1", testGuid1, guid.ToString("B"));
				AssertEquals("guid2", testGuid7, guid.ToString("d"));
				AssertEquals("guid3", testGuid8, guid.ToString("N"));
				AssertEquals("guid4", testGuid9, guid.ToString("p"));
				AssertEquals("guid5", testGuid7, guid.ToString(null));
				AssertEquals("guid6", testGuid7, guid.ToString(""));

				// Tests that are expected to fail.
				try
				{
					guid.ToString("x");
				}
				catch(FormatException)
				{
					// Success
				}
			}

	// Test the equality comparison of Guid's.
	public void TestGuidEquals()
			{
				Guid guid1 = new Guid(unchecked((int)0xc0093632),
								      unchecked((short)0xb44c),
								      unchecked((short)0x4cf7),
								      unchecked((byte)0xa2),
									  unchecked((byte)0x79),
								      unchecked((byte)0xd8),
									  unchecked((byte)0x2f),
								      unchecked((byte)0xe8),
									  unchecked((byte)0xa8),
								      unchecked((byte)0x89),
									  unchecked((byte)0x0d));
				Guid guid2 = new Guid(unchecked((int)0xc0093632),
								      unchecked((short)0xb44e),
								      unchecked((short)0x4cf7),
								      unchecked((byte)0xa2),
									  unchecked((byte)0x79),
								      unchecked((byte)0xd8),
									  unchecked((byte)0x2f),
								      unchecked((byte)0xe8),
									  unchecked((byte)0xa8),
								      unchecked((byte)0x89),
									  unchecked((byte)0x0d));
				Assert(!guid1.Equals(null));
				Assert(guid1.Equals(guid1));
				Assert(!guid1.Equals(guid2));
				Assert(!guid1.Equals(Guid.Empty));
			}

}; // class TestGuid

#endif // !ECMA_COMPAT
