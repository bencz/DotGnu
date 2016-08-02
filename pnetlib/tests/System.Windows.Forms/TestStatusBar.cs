/*
 * TestStatusBar.cs - Tests for the
 *		"System.Windows.Forms.StatusBar" class.
 *
 * Copyright (C) 2009  Free Software Foundation Inc.
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

public class TestStatusBar : TestCase
{
	// Constructor.
	public TestStatusBar(String name)
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
	public void TestPanelCollections()
			{
				StatusBar bar = new StatusBar();
				StatusBar.StatusBarPanelCollection panels = bar.Panels;

				StatusBarPanel p1 = new StatusBarPanel();
				StatusBarPanel p2 = new StatusBarPanel();
				StatusBarPanel p3 = new StatusBarPanel();
				StatusBarPanel p4 = new StatusBarPanel();
		
				AssertEquals("Count (1)", 0, panels.Count);

				panels.Add(p2);
				AssertEquals("Count (2)", 1, panels.Count);

				panels[0] = p2;
				AssertEquals("Count (3)", 1, panels.Count);
				AssertEquals("Indexer (1)", p2, panels[0]);

				panels[0] = p1;
				AssertEquals("Count (4)", 1, panels.Count);
				AssertEquals("Indexer (2)", p1, panels[0]);

				panels.AddRange(new StatusBarPanel[] { p2, p3 });
				AssertEquals("Count (5)", 3, panels.Count);
				AssertEquals("Indexer (3)", p1, panels[0]);
				AssertEquals("Indexer (4)", p2, panels[1]);
				AssertEquals("Indexer (5)", p3, panels[2]);

				AssertEquals("Contains (1)", true, panels.Contains(p1));
				AssertEquals("Contains (2)", true, panels.Contains(p2));
				AssertEquals("Contains (3)", true, panels.Contains(p3));
				AssertEquals("Contains (4)", false, panels.Contains(p4));

				AssertEquals("IndexOf (1)", 0, panels.IndexOf(p1));
				AssertEquals("IndexOf (2)", 1, panels.IndexOf(p2));
				AssertEquals("IndexOf (3)", -1, panels.IndexOf(p4));

				panels.Insert(0, p4);
				AssertEquals("IndexOf (4)", 1, panels.IndexOf(p1));
				AssertEquals("IndexOf (5)", 2, panels.IndexOf(p2));
				AssertEquals("IndexOf (6)", 0, panels.IndexOf(p4));

				panels.Remove(p4);
				AssertEquals("Count (6)", 3, panels.Count);
				AssertEquals("IndexOf (7)", 0, panels.IndexOf(p1));
				AssertEquals("IndexOf (8)", 1, panels.IndexOf(p2));
				AssertEquals("IndexOf (9)", -1, panels.IndexOf(p4));

				try
				{
					panels.Add(p1);
					Fail("Add (1)");
				}
				catch(ArgumentException)
				{
					// Success - already in collection
				}

				try
				{
					StatusBarPanel nullPanel = null;
					panels.Add(nullPanel);
					Fail("Add (2)");
				}
				catch(ArgumentNullException)
				{
					// Success - null argument
				}

				try
				{
					panels.AddRange(new StatusBarPanel[] {p1, p2});
					Fail("AddRange (1)");
				}
				catch(ArgumentException)
				{
					// Success - already in collection
				}

				try
				{
					panels[1] = p1;
					Fail("Indexer (6)");
				}
				catch(ArgumentException)
				{
					// Success- already in collection
				}

				panels.Clear();
				AssertEquals("Count (7)", 0, panels.Count);
			}
}; // class TestStatusBar
