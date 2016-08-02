/*
 * TestXmlAttribute.cs - Tests for the "System.Xml.XmlAttribute" class.
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

public class TestXmlAttribute : TestCase
{
	// Internal state.
	private XmlDocument doc;

	// Constructor.
	public TestXmlAttribute(String name)
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

	// Check the properties on an attribute.
	private void CheckProperties(String msg, XmlAttribute attr,
								 String prefix, String name, String ns,
								 String value)
			{
				XmlNameTable nt = doc.NameTable;
				prefix = nt.Add(prefix);
				name = nt.Add(name);
				ns = nt.Add(ns);
				AssertSame(msg + " [1]", prefix, attr.Prefix);
				AssertSame(msg + " [2]", name, attr.LocalName);
				AssertSame(msg + " [3]", ns, attr.NamespaceURI);
				if(prefix != String.Empty)
				{
					String combined = nt.Add(prefix + ":" + name);
					AssertEquals(msg + " [4]", combined, attr.Name);
				}
				else
				{
					AssertSame(msg + " [5]", name, attr.Name);
				}
				AssertEquals(msg + " [6]",
							 XmlNodeType.Attribute, attr.NodeType);
				AssertEquals(msg + " [7]", doc, attr.OwnerDocument);
				AssertNull(msg + " [8]", attr.OwnerElement);
				AssertNull(msg + " [9]", attr.ParentNode);
				AssertEquals(msg + " [10]", value, attr.InnerText);
				AssertEquals(msg + " [11]", value, attr.Value);
				Assert(msg + " [12]", attr.Specified);
			}
	private void CheckProperties(String msg, XmlAttribute attr,
								 String prefix, String name, String ns)
			{
				CheckProperties(msg, attr, prefix, name, ns, String.Empty);
			}
	private void CheckProperties(String msg, XmlAttribute attr, String value)
			{
				CheckProperties(msg, attr, attr.Prefix, attr.LocalName,
								attr.NamespaceURI, value);
			}

	// Test attribute construction.
	public void TestXmlAttributeConstruct()
			{
				XmlAttribute attr;

				attr = doc.CreateAttribute("foo");
				CheckProperties("Construct (1)", attr,
								String.Empty, "foo", String.Empty);

				attr = doc.CreateAttribute("prefix:foo");
				CheckProperties("Construct (2)", attr,
								"prefix", "foo", String.Empty);

				attr = doc.CreateAttribute("foo", "uri");
				CheckProperties("Construct (3)", attr,
								String.Empty, "foo", "uri");

				attr = doc.CreateAttribute("prefix:foo", "uri");
				CheckProperties("Construct (4)", attr,
								"prefix", "foo", "uri");

				attr = doc.CreateAttribute("prefix1:prefix2:foo", "uri");
				CheckProperties("Construct (5)", attr,
								"prefix1:prefix2", "foo", "uri");

				attr = doc.CreateAttribute("prefix", "foo:bar", "uri");
				CheckProperties("Construct (6)", attr,
								"prefix", "foo:bar", "uri");
			}

	// Test attribute property changes.
	public void TestXmlAttributeProperties()
			{
				XmlAttribute attr;

				attr = doc.CreateAttribute("prefix", "foo", "uri");
				CheckProperties("Properties (1)", attr, String.Empty);

				attr.Value = "xyzzy";
				CheckProperties("Properties (2)", attr, "xyzzy");

				attr.Value = "&lt;hello&gt;";
				CheckProperties("Properties (3)", attr, "&lt;hello&gt;");
			}

	// Test attribute XML output.
	public void TestXmlAttributeToXml()
			{
				XmlAttribute attr;

				attr = doc.CreateAttribute("prefix", "foo", String.Empty);
				attr.Value = "xyzzy";

				AssertEquals("ToXml (1)", "xyzzy", attr.InnerXml);
				AssertEquals("ToXml (2)",
							 "foo=\"xyzzy\"", attr.OuterXml);

				attr.Value = "&lt;hello&gt;\"";
				AssertEquals("ToXml (3)", "&amp;lt;hello&amp;gt;\"",
							 attr.InnerXml);
				AssertEquals("ToXml (4)",
							 "foo=\"&amp;lt;hello&amp;gt;&quot;\"",
							 attr.OuterXml);
			}

	// Test setting attributes via direct text node inserts.
	public void TestXmlAttributeInsert()
			{
				XmlAttribute attr;
				attr = doc.CreateAttribute("prefix", "foo", "uri");

				XmlText text1 = doc.CreateTextNode("hello");
				XmlText text2 = doc.CreateTextNode(" and goodbye");

				attr.AppendChild(text1);
				AssertEquals("Insert (1)", "hello", attr.Value);
				AssertEquals("Insert (2)", text1, attr.FirstChild);
				AssertEquals("Insert (3)", text1, attr.LastChild);

				attr.AppendChild(text2);
				AssertEquals("Insert (4)", "hello and goodbye", attr.Value);
				AssertEquals("Insert (5)", text1, attr.FirstChild);
				AssertEquals("Insert (6)", text2, attr.LastChild);

				// Entity references do not affect the combined value,
				// but they do affect the XML.
				XmlEntityReference entity = doc.CreateEntityReference("foo");
				attr.AppendChild(entity);
				AssertEquals("Insert (7)", "hello and goodbye", attr.Value);
				AssertEquals("Insert (8)",
							 "hello and goodbye&foo;", attr.InnerXml);

				// Cannot insert whitespace into attributes.
				try
				{
					attr.AppendChild(doc.CreateWhitespace("   "));
					Fail("Insert (9)");
				}
				catch(InvalidOperationException)
				{
					// Success
				}
				try
				{
					attr.AppendChild(doc.CreateSignificantWhitespace("   "));
					Fail("Insert (9)");
				}
				catch(InvalidOperationException)
				{
					// Success
				}
			}

}; // class TestXmlAttribute

#endif // !ECMA_COMPAT
