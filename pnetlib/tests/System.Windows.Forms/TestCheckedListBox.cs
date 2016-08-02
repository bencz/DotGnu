/*
 * TestCheckListBox.cs - Tests for the
 *		"System.Windows.Forms.CheckListBox" class.
 *
 * Copyright (C) 2007  Southern Storm Software, Pty Ltd.
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
using System.Windows.Forms;

public class TestCheckedListBox : TestCase
{
	// Constructor.
	public TestCheckedListBox(String name)
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

	// Test the constructors
	public void TestCheckedCollections()
			{
				CheckedListBox clb = new CheckedListBox();
				CheckedListBox.CheckedIndexCollection indexes = clb.CheckedIndices;
				CheckedListBox.CheckedItemCollection items = clb.CheckedItems;
				
				string item0 = "item 0";
				string item1 = "item 1";
				string item2 = "item 2";
				string item3 = "item 3";
		
				AssertEquals("Count (1)", 0, indexes.Count);
				AssertEquals("Count (2)", 0, items.Count);

				clb.Items.Add(item0, false);
				AssertEquals("Count (3)", 0, indexes.Count);
				AssertEquals("Count (4)", 0, items.Count);

				clb.Items.Add(item1, true);
				AssertEquals("Count (5)", 1, indexes.Count);
				AssertEquals("Count (6)", 1, items.Count);

				clb.Items.Add(item2, true);
				AssertEquals("Count (7)", 2, indexes.Count);
				AssertEquals("Count (8)", 2, items.Count);

				clb.Items.Add(item3, false);
				AssertEquals("Count (9)", 2, indexes.Count);
				AssertEquals("Count (10)", 2, items.Count);

				AssertEquals("Indexer (1)", 1, indexes[0]);
				AssertEquals("Indexer (2)", 2, indexes[1]);
				AssertEquals("Indexer (3)", item1, items[0]);
				AssertEquals("Indexer (4)", item2, items[1]);

				AssertEquals("Index of (1)", -1, items.IndexOf(item0));
				AssertEquals("Index of (2)", 0, items.IndexOf(item1));
				AssertEquals("Index of (3)", 1, items.IndexOf(item2));
				AssertEquals("Index of (4)", -1, items.IndexOf(item3));
			}
}; // class TestCheckedListBox
