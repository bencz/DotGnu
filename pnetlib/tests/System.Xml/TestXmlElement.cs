/*
 * TestXmlElement.cs - Tests for the "System.Xml.XmlElement" class.
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

public class TestXmlElement : TestCase
{
	// Internal state.
	private XmlDocument doc;

	// Constructor.
	public TestXmlElement(String name)
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

	// Check the properties on an XmlElement node.
	private void CheckProperties(String msg, XmlElement type,
								 String prefix, String localName,
								 String namespaceURI, bool empty)
			{
				prefix = doc.NameTable.Add(String.Copy(prefix));
				localName = doc.NameTable.Add(String.Copy(localName));
				namespaceURI = doc.NameTable.Add(String.Copy(namespaceURI));
				String name;
				if(prefix != String.Empty)
				{
					name = doc.NameTable.Add(prefix + ":" + localName);
				}
				else
				{
					name = localName;
				}
				Assert(msg + " [1]", !type.HasAttributes);    // before create
				AssertNotNull(msg + " [2]", type.Attributes); // create attrs
				Assert(msg + " [3]", !type.HasAttributes);    // after create
				AssertEquals(msg + " [4]", String.Empty, type.BaseURI);
				AssertNotNull(msg + " [5]", type.ChildNodes);
				AssertNull(msg + " [6]", type.FirstChild);
				Assert(msg + " [7]", !type.HasChildNodes);
				Assert(msg + " [8]", !type.IsReadOnly);
				AssertSame(msg + " [9]", prefix, type.Prefix);
				AssertSame(msg + " [10]", localName, type.LocalName);
				AssertSame(msg + " [11]", namespaceURI, type.NamespaceURI);
				AssertSame(msg + " [12]", name, type.Name);
				AssertNull(msg + " [13]", type.NextSibling);
				AssertEquals(msg + " [14]", XmlNodeType.Element, type.NodeType);
				AssertEquals(msg + " [15]", doc, type.OwnerDocument);
				AssertNull(msg + " [16]", type.ParentNode);
				AssertNull(msg + " [17]", type.PreviousSibling);
				AssertEquals(msg + " [18]", empty, type.IsEmpty);
				AssertEquals(msg + " [19]", String.Empty, type.InnerXml);
				AssertEquals(msg + " [20]", String.Empty, type.InnerText);
				AssertEquals(msg + " [21]", String.Empty, type.Value);
			}

	// Test element construction.
	public void TestXmlElementConstruct()
			{
				XmlElement element;

				element = doc.CreateElement("foo");
				CheckProperties("Construct (1)", element,
								String.Empty, "foo", String.Empty, true);

				element = doc.CreateElement("prefix:foo");
				CheckProperties("Construct (2)", element,
								"prefix", "foo", String.Empty, true);

				element = doc.CreateElement("foo", "uri");
				CheckProperties("Construct (3)", element,
								String.Empty, "foo", "uri", true);

				element = doc.CreateElement("prefix:foo", "uri");
				CheckProperties("Construct (4)", element,
								"prefix", "foo", "uri", true);

				element = doc.CreateElement("prefix1:prefix2:foo", "uri");
				CheckProperties("Construct (5)", element,
								"prefix1:prefix2", "foo", "uri", true);

				element = doc.CreateElement("prefix", "foo:bar", "uri");
				CheckProperties("Construct (6)", element,
								"prefix", "foo:bar", "uri", true);
			}

	// Test element property changes.
	public void TestXmlElementProperties()
			{
				XmlElement element;

				// Create an element.
				element = doc.CreateElement("foo");
				CheckProperties("Properties (1)", element,
								String.Empty, "foo", String.Empty, true);

				// Check that it is initially serialized in the short form.
				AssertEquals("Properties (2)", "<foo />", element.OuterXml);

				// Turn off the "empty" flag.
				element.IsEmpty = false;

				// Check that it is now serialized in the long form.
				AssertEquals("Properties (3)", "<foo></foo>", element.OuterXml);
				AssertEquals("Properties (4)", "", element.InnerXml);
				Assert("Properties (5)", !element.IsEmpty);

				// Add a text child and re-check.
				XmlText text = doc.CreateTextNode("bar");
				element.AppendChild(text);
				AssertEquals("Properties (6)", "bar", element.InnerXml);
				AssertEquals("Properties (7)",
							 "<foo>bar</foo>", element.OuterXml);
				Assert("Properties (8)", !element.IsEmpty);

				// Turn on the "empty" flag.
				element.IsEmpty = true;
				AssertNull("Properties (9)", element.FirstChild);
				Assert("Properties (10)", element.IsEmpty);
				AssertEquals("Properties (11)", "<foo />", element.OuterXml);
				AssertEquals("Properties (12)", "", element.InnerXml);

				// Add the text node back, switch to non-empty, and remove.
				element.AppendChild(text);
				Assert("Properties (13)", !element.IsEmpty);
				element.RemoveChild(text);
				Assert("Properties (14)", !element.IsEmpty);
				AssertEquals("Properties (15)",
							 "<foo></foo>", element.OuterXml);
				AssertEquals("Properties (16)", "", element.InnerXml);
			}

	// Test element remove calls.
	public void TestXmlElementRemove()
			{
				XmlElement element;
				XmlNode node;

				// Test removing just the attributes.
				element = doc.CreateElement("foo");
				element.SetAttribute("bar", "xyzzy");
				node = element.AppendChild(doc.CreateElement("baz"));
				AssertEquals("Remove (1)", 1, element.Attributes.Count);
				AssertEquals("Remove (2)", "xyzzy",
							 element.GetAttribute("bar"));
				AssertEquals("Remove (3)", node, element.FirstChild);
				AssertEquals("Remove (4)", node, element.LastChild);
				element.RemoveAllAttributes();
				AssertEquals("Remove (5)", 0, element.Attributes.Count);
				AssertEquals("Remove (6)", node, element.FirstChild);
				AssertEquals("Remove (7)", node, element.LastChild);
				AssertEquals("Remove (8)", String.Empty,
							 element.GetAttribute("bar"));

				// Test removing just the contents.
				element = doc.CreateElement("foo");
				element.SetAttribute("bar", "xyzzy");
				node = element.AppendChild(doc.CreateElement("baz"));
				AssertEquals("Remove (9)", 1, element.Attributes.Count);
				AssertEquals("Remove (10)", "xyzzy",
							 element.GetAttribute("bar"));
				AssertEquals("Remove (11)", node, element.FirstChild);
				AssertEquals("Remove (12)", node, element.LastChild);
				element.IsEmpty = true;
				AssertEquals("Remove (13)", 1, element.Attributes.Count);
				AssertNull("Remove (14)", element.FirstChild);
				AssertNull("Remove (15)", element.LastChild);
				AssertEquals("Remove (16)", "xyzzy",
							 element.GetAttribute("bar"));

				// Test removing both the attributes and the contents.
				element = doc.CreateElement("foo");
				element.SetAttribute("bar", "xyzzy");
				node = element.AppendChild(doc.CreateElement("baz"));
				AssertEquals("Remove (17)", 1, element.Attributes.Count);
				AssertEquals("Remove (18)", "xyzzy",
							 element.GetAttribute("bar"));
				AssertEquals("Remove (19)", node, element.FirstChild);
				AssertEquals("Remove (20)", node, element.LastChild);
				element.RemoveAll();
				AssertEquals("Remove (21)", 0, element.Attributes.Count);
				AssertNull("Remove (22)", element.FirstChild);
				AssertNull("Remove (23)", element.LastChild);
				AssertEquals("Remove (24)", String.Empty,
							 element.GetAttribute("bar"));
			}

}; // class TestXmlElement

#endif // !ECMA_COMPAT
