/*
 * TestHashTable.cs - Tests for the "HashTable" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

public class TestHashTable : TestCase
{
	// Constructor.
	public TestHashTable(String name)
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

	// test CopyTo on a empty HashTable
	public void TestHashTableCopyToEmpty ()
			{
				Hashtable hashTable = new Hashtable ();
				AssertEquals ("count", 0, hashTable.Count);
				object[] array = new object [hashTable.Count];
				hashTable.CopyTo (array, 0);
			}
			
	public void TestHashTableAddRemove() {
		Hashtable h = new Hashtable();

		// these keys produces equal hashkeys
		string [] keys = { "lcu.long_name_one", "lcu.long_name_two", "lcu.long_three" };
		h.Add( keys[0], "Value Of 1" );
		h.Add( keys[1], "Value Of 2" );
		h.Add( keys[2], "Value Of 3" );

		AssertEquals( "count", 3, h.Count );
		
		h.Remove( keys[0] );
		h.Remove( keys[1] );
		h.Remove( keys[2] );
		
		AssertEquals( "count", 0, h.Count );

		}

}; // class TestHashTable

