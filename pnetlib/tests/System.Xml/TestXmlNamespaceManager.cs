/*
 * TestXmlNamespaceManager.cs - Tests for the
 *		"System.Xml.XmlNamespaceManager" class.
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
using System.Xml;

public class TestXmlNamespaceManager : TestCase
{
	// Constructor.
	public TestXmlNamespaceManager(String name)
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

	// Test namespace manager construction.
	public void TestXmlNamespaceManagerConstruct()
			{
				try
				{
					XmlNamespaceManager ns = new XmlNamespaceManager(null);
				}
				catch(ArgumentNullException)
				{
				}
			}

	// Test the default namespace manager properties.
	public void TestXmlNamespaceManagerDefaults()
			{
				NameTable table = new NameTable();
				XmlNamespaceManager ns = new XmlNamespaceManager(table);
				AssertEquals("Defaults (1)",
							 "http://www.w3.org/XML/1998/namespace",
							 ns.LookupNamespace("xml"));
				AssertEquals("Defaults (2)",
							 "http://www.w3.org/2000/xmlns/",
							 ns.LookupNamespace("xmlns"));
				AssertEquals("Defaults (3)", "", ns.LookupNamespace(""));
				Assert("Defaults (4)",
					   ReferenceEquals(table, ns.NameTable));
				AssertEquals("Defaults (5)", "", ns.DefaultNamespace);
				AssertNull("Defaults (6)", ns.LookupNamespace("foo"));
			}

	// Test adding items to a namespace manager.
	public void TestXmlNamespaceManagerAdd()
			{
				NameTable table = new NameTable();
				XmlNamespaceManager ns = new XmlNamespaceManager(table);

				// Test exception behaviour.
				try
				{
					ns.AddNamespace(null, "uri");
					Fail("Add (1)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}
				try
				{
					ns.AddNamespace("prefix", null);
					Fail("Add (2)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}
				try
				{
					ns.AddNamespace("xml", "uri");
					Fail("Add (3)");
				}
				catch(ArgumentException)
				{
					// Success
				}
				try
				{
					ns.AddNamespace("xmlns", "uri");
					Fail("Add (4)");
				}
				catch(ArgumentException)
				{
					// Success
				}
				try
				{
					// Work around intern'ed string handling in the engine.
					ns.AddNamespace(String.Concat("xml", "ns"), "uri");
					Fail("Add (5)");
				}
				catch(ArgumentException)
				{
					// Success
				}

				// Try changing the default namespace.
				ns.AddNamespace("", "defuri");
				AssertEquals("Add (6)", "defuri", ns.LookupNamespace(""));
				AssertEquals("Add (7)", "defuri", ns.DefaultNamespace);
				AssertEquals("Add (8)", "", ns.LookupPrefix("defuri"));

				// Try changing some other namespace.
				ns.AddNamespace("foo", "uri");
				AssertEquals("Add (9)", "uri", ns.LookupNamespace("foo"));
				AssertEquals("Add (10)", "foo", ns.LookupPrefix("uri"));

				// Make sure that the standard are still set to their
				// correct values after the modifications above.
				AssertEquals("Add (11)",
							 "http://www.w3.org/XML/1998/namespace",
							 ns.LookupNamespace("xml"));
				AssertEquals("Add (12)",
							 "http://www.w3.org/2000/xmlns/",
							 ns.LookupNamespace("xmlns"));
				AssertEquals("Add (13)", "defuri", ns.LookupNamespace(""));
			}

	// Check that a namespace manager's enumerator returns what we expect.
	private static void CheckEnum(XmlNamespaceManager ns, String fooValue)
			{
				int count = 0;
				foreach(String name in ns)
				{
					++count;
					if(name == "xml")
					{
						AssertEquals
							("Enum (1)",
							 "http://www.w3.org/XML/1998/namespace",
							 ns.LookupNamespace(name));
					}	
					else if(name == "xmlns")
					{
						AssertEquals
							("Enum (2)",
							 "http://www.w3.org/2000/xmlns/",
							 ns.LookupNamespace(name));
					}
					else if(name == "foo")
					{
						AssertEquals
							("Enum (3)", fooValue,
							 ns.LookupNamespace(name));
					}
					else if(name == "")
					{
						AssertEquals
							("Enum (4)", "",
							 ns.LookupNamespace(name));
					}
				}
				AssertEquals("Enum (5)", 4, count);
			}

	// Test enumerating over a namespace manager.
	public void TestXmlNamespaceManagerEnumerate()
			{
				NameTable table = new NameTable();
				XmlNamespaceManager ns = new XmlNamespaceManager(table);
				ns.AddNamespace("foo", "uri1");
				CheckEnum(ns, "uri1");
				ns.PushScope();
				ns.AddNamespace("foo", "uri2");
				CheckEnum(ns, "uri2");
				ns.AddNamespace("foo", "uri3");
				CheckEnum(ns, "uri3");
				ns.PopScope();
				ns.AddNamespace("", "");
				CheckEnum(ns, "uri1");
			}

	// Check the "HasNamespace" method.
	public void TestXmlNamespaceManagerHas()
			{
				NameTable table = new NameTable();
				XmlNamespaceManager ns = new XmlNamespaceManager(table);
				Assert("Has (1)", ns.HasNamespace("xml"));
				Assert("Has (2)", ns.HasNamespace("xmlns"));
				Assert("Has (3)", ns.HasNamespace(""));
				Assert("Has (4)", !ns.HasNamespace("foo"));
				ns.AddNamespace("foo", "uri");
				Assert("Has (5)", ns.HasNamespace("xml"));
				Assert("Has (6)", ns.HasNamespace("xmlns"));
				Assert("Has (7)", ns.HasNamespace(""));
				Assert("Has (8)", ns.HasNamespace("foo"));
				ns.PushScope();
				ns.AddNamespace("bar", "uri2");
				Assert("Has (9)", ns.HasNamespace("xml"));
				Assert("Has (10)", ns.HasNamespace("xmlns"));
				Assert("Has (11)", ns.HasNamespace(""));
				Assert("Has (12)", ns.HasNamespace("foo"));
				Assert("Has (13)", ns.HasNamespace("bar"));
				ns.PopScope();
				Assert("Has (14)", ns.HasNamespace("xml"));
				Assert("Has (15)", ns.HasNamespace("xmlns"));
				Assert("Has (16)", ns.HasNamespace(""));
				Assert("Has (17)", ns.HasNamespace("foo"));
				Assert("Has (18)", !ns.HasNamespace("bar"));
				ns.RemoveNamespace("foo", "uri");
				Assert("Has (19)", ns.HasNamespace("xml"));
				Assert("Has (20)", ns.HasNamespace("xmlns"));
				Assert("Has (21)", ns.HasNamespace(""));
				Assert("Has (22)", !ns.HasNamespace("foo"));
				Assert("Has (23)", !ns.HasNamespace("bar"));
				ns.RemoveNamespace("", "");
				Assert("Has (24)", ns.HasNamespace("xml"));
				Assert("Has (25)", ns.HasNamespace("xmlns"));
				Assert("Has (26)", ns.HasNamespace(""));
				Assert("Has (27)", !ns.HasNamespace("foo"));
				Assert("Has (28)", !ns.HasNamespace("bar"));
				Assert("Has (29)", !ns.HasNamespace(null));
			}

	// Check the "PopScope" method.
	public void TestXmlNamespaceManagerPopScope()
			{
				NameTable table = new NameTable();
				XmlNamespaceManager ns = new XmlNamespaceManager(table);
				Assert("PopScope (1)", !ns.PopScope());
				ns.PushScope();
				Assert("PopScope (2)", ns.PopScope());
			}

	// Check the "RemoveNamespace" method.
	public void TestXmlNamespaceManagerRemove()
			{
				NameTable table = new NameTable();
				XmlNamespaceManager ns = new XmlNamespaceManager(table);

				// Test the exception behaviour.
				try
				{
					ns.AddNamespace(null, "uri");
					Fail("Remove (1)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}
				try
				{
					ns.AddNamespace("prefix", null);
					Fail("Remove (2)");
				}
				catch(ArgumentNullException)
				{
					// Success
				}

				// Cannot remove standard namespaces.
				ns.RemoveNamespace
					("xml", "http://www.w3.org/XML/1998/namespace");
				AssertEquals("Remove (3)",
							 "http://www.w3.org/XML/1998/namespace",
							 ns.LookupNamespace("xml"));
				ns.RemoveNamespace
					("xmlns", "http://www.w3.org/2000/xmlns/");
				AssertEquals("Remove (3)",
							 "http://www.w3.org/2000/xmlns/",
							 ns.LookupNamespace("xmlns"));

				// Add and remove a particular namespace.
				ns.AddNamespace("foo", "uri");
				ns.RemoveNamespace("foo", "uri");
				AssertNull("Remove (4)", ns.LookupNamespace("foo"));

				// Make sure that we cannot remove namespaces in parent scopes.
				ns.AddNamespace("foo", "uri");
				ns.PushScope();
				ns.RemoveNamespace("foo", "uri");
				AssertEquals("Remove (5)", "uri", ns.LookupNamespace("foo"));

				// Try removing a namespace with the wrong URI or prefix.
				ns.AddNamespace("foo", "uri2");
				ns.RemoveNamespace("foo", "uri");
				AssertEquals("Remove (6)", "uri2", ns.LookupNamespace("foo"));
				ns.RemoveNamespace("foo2", "uri");
				AssertEquals("Remove (7)", "uri2", ns.LookupNamespace("foo"));
			}

}; // class TestXmlNamespaceManager
