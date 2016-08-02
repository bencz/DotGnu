/*
 * TestXmlDocumentFragment.cs - Tests for the
 *		"System.Xml.XmlDocumentFragment" class.
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

#if !ECMA_COMPAT

public class TestXmlDocumentFragment : TestCase
{
	// Internal state.
	private XmlDocument doc;
	private XmlDocumentFragment fragment;

	// Constructor.
	public TestXmlDocumentFragment(String name)
			: base(name)
			{
				// Nothing to do here.
			}

	// Set up for the tests.
	protected override void Setup()
			{
				doc = new XmlDocument();
				fragment = doc.CreateDocumentFragment();
			}

	// Clean up after the tests.
	protected override void Cleanup()
			{
				// Nothing to do here.
			}

	// Test the properties of an XmlDocumentFragment node.
	public void TestXmlDocumentFragmentProperties()
			{
				// Verify the initial conditions.
				AssertNull("Properties (1)", fragment.Attributes);
				AssertEquals("Properties (2)", String.Empty, fragment.BaseURI);
				AssertNotNull("Properties (3)", fragment.ChildNodes);
				AssertNull("Properties (4)", fragment.FirstChild);
				Assert("Properties (5)", !fragment.HasChildNodes);
				Assert("Properties (6)", !fragment.IsReadOnly);
				AssertEquals("Properties (7)",
							 "#document-fragment", fragment.LocalName);
				AssertEquals("Properties (8)",
							 "#document-fragment", fragment.Name);
				AssertEquals("Properties (9)",
							 String.Empty, fragment.NamespaceURI);
				AssertNull("Properties (10)", fragment.NextSibling);
				AssertEquals("Properties (11)",
							 XmlNodeType.DocumentFragment, fragment.NodeType);
				AssertEquals("Properties (12)", doc, fragment.OwnerDocument);
				AssertNull("Properties (13)", fragment.ParentNode);
				AssertEquals("Properties (14)", String.Empty, fragment.Prefix);
				AssertNull("Properties (15)", fragment.PreviousSibling);
				AssertEquals("Properties (16)", String.Empty, fragment.Value);
			}

}; // class TestXmlDocumentFragment

#endif // !ECMA_COMPAT
