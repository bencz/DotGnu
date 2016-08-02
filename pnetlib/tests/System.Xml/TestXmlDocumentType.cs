/*
 * TestXmlDocumentType.cs - Tests for the "System.Xml.XmlDocumentType" class.
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

public class TestXmlDocumentType : TestCase
{
	// Internal state.
	private XmlDocument doc;

	// Constructor.
	public TestXmlDocumentType(String name)
			: base(name)
			{
				// Nothing to do here.
			}

	// Set up for the tests.
	protected override void Setup()
			{
				doc = new XmlDocument();
			}

	// Clean up after the tests.
	protected override void Cleanup()
			{
				// Nothing to do here.
			}

	// Check the properties on an XmlDocumentType node.
	private void CheckProperties(String msg, XmlDocumentType type,
								 String name, String publicId,
								 String systemId, String internalSubset,
								 String xml)
			{
				AssertNull(msg + " [1]", type.Attributes);
				AssertEquals(msg + " [2]", String.Empty, type.BaseURI);
				AssertNotNull(msg + " [3]", type.ChildNodes);
				AssertNull(msg + " [4]", type.FirstChild);
				Assert(msg + " [5]", !type.HasChildNodes);
				Assert(msg + " [6]", type.IsReadOnly);
				AssertEquals(msg + " [7]", name, type.LocalName);
				AssertEquals(msg + " [8]", name, type.Name);
				AssertEquals(msg + " [9]", publicId, type.PublicId);
				AssertEquals(msg + " [10]", systemId, type.SystemId);
				AssertEquals(msg + " [11]",
							 internalSubset, type.InternalSubset);
				AssertEquals(msg + " [12]", String.Empty, type.NamespaceURI);
				AssertNull(msg + " [13]", type.NextSibling);
				AssertEquals(msg + " [14]",
							 XmlNodeType.DocumentType, type.NodeType);
				AssertEquals(msg + " [15]", doc, type.OwnerDocument);
				AssertNull(msg + " [16]", type.ParentNode);
				AssertEquals(msg + " [17]", String.Empty, type.Prefix);
				AssertNull(msg + " [18]", type.PreviousSibling);
				AssertEquals(msg + " [19]", String.Empty, type.Value);
				AssertEquals(msg + " [20]", String.Empty, type.InnerXml);
				AssertEquals(msg + " [21]", xml, type.OuterXml);
			}

	// Test the properties of an XmlDocumentType node.
	public void TestXmlDocumentTypeProperties()
			{
				CheckProperties("Properties (1)",
							    doc.CreateDocumentType
									("foo", null, null, null),
								"foo", null, null, null,
								"<!DOCTYPE foo>");
				CheckProperties("Properties (2)",
							    doc.CreateDocumentType
									("foo", "pubid", "", null),
								"foo", "pubid", "", null,
								"<!DOCTYPE foo PUBLIC \"pubid\" \"\">");
				CheckProperties("Properties (3)",
							    doc.CreateDocumentType
									("foo", "pubid&\"", "sysid", null),
								"foo", "pubid&\"", "sysid", null,
								"<!DOCTYPE foo PUBLIC \"pubid&amp;&quot;\" " +
									"\"sysid\">");
				CheckProperties("Properties (4)",
							    doc.CreateDocumentType
									("foo", null, "sysid", null),
								"foo", null, "sysid", null,
								"<!DOCTYPE foo SYSTEM \"sysid\">");
				CheckProperties("Properties (5)",
							    doc.CreateDocumentType
									("foo", null, null, "internal"),
								"foo", null, null, "internal",
								"<!DOCTYPE foo [internal]>");
				CheckProperties("Properties (6)",
							    doc.CreateDocumentType
									("foo", null, null, "internal&\""),
								"foo", null, null, "internal&\"",
								"<!DOCTYPE foo [internal&\"]>");
				CheckProperties("Properties (7)",
							    doc.CreateDocumentType
									("foo", "pubid", "sysid", "internal"),
								"foo", "pubid", "sysid", "internal",
								"<!DOCTYPE foo PUBLIC \"pubid\" " +
									"\"sysid\" [internal]>");
			}

}; // class TestXmlDocumentType

#endif // !ECMA_COMPAT
