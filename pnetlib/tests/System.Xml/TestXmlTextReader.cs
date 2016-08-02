/*
 * TestXmlTextReader.cs - Tests for the
 *		"System.Xml.TestXmlTextReader" class.
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
using System.IO;
using System.Text;
using System.Xml;

public class TestXmlTextReader : TestCase
{
	private XmlTextReader xr;
	private StringReader sr;
	private String[] xml =
	{
		"<soda caffeine=\"yes\">\n<size>medium</size>\n</soda>",
		"<soda><size>medium</size></soda>",
		"<free>software's freedom</free>",
		("<?xml version='1.0' ?>" +
		 "<bookstore>" +
		 "	<book>" +
		 "		<title>Understanding The Linux Kernel</title>" +
		 "		<author>Daniel P. Bovet and Marco Cesati</author>" +
		 "	</book>" +
		 "	<book>" +
		 "		<title>Learning Perl</title>" +
		 "		<author>Randal L. Schwartz and Tom Christiansen</author>" +
		 "	</book>" +
		 "</bookstore>"),
		("<?xml version='1.0'?>" +
		 "<!DOCTYPE test [" +
		 "	<!ELEMENT test (#PCDATA) >" +
		 "	<!ENTITY % foo '&#37;bar;'>" +
		 "	<!ENTITY % bar '&#60;!ENTITY GNU \"GNU&#39;s NOT UNIX\" >' >" +
		 "	<!ENTITY fu '&#37;bar;'>" +
		 "	%foo;" +
		 "]>" +
		 "<test>Brave &GNU; World... &fu;<![CDATA[bar]]></test>"),
		("<?xml version='1.0'?>" +
		 "<!DOCTYPE test [" +
		 "	<!ELEMENT test (#PCDATA) >" +
		 "	<!ENTITY hello 'hello world'>" +
		 "]>" +
		 "<!-- a sample comment -->" +
		 "<test>Brave GNU <![CDATA[World]]> &hello;</test>" +
		 "					" +
		 "<?pi HeLlO wOrLd ?>"),
		 "<doc><text/></doc>"
	};


	// Constructor.
	public TestXmlTextReader(String name)
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

	// Clear the current output.
	private void Clear()
			{
				sr = null;
				xr = null;
			}

	// Reset the entire XML text reader.
	private void Reset(int index)
			{
				sr = new StringReader(xml[index]);
				xr = new XmlTextReader(sr);
			}
	private void Reset(TextReader tr)
			{
				sr = null;
				xr = new XmlTextReader(tr);
			}

	// Test the constructors
	public void TestXmlTextReaderCtor()
			{
				// TODO: Test each constructor
			}

	// Test the GetRemainder method.
	public void TestXmlTextReaderGetRemainder()
			{
				Reset(3);
				xr.WhitespaceHandling = WhitespaceHandling.None;
				AssertEquals("GetRemainder (1)", true, xr.Read());
				AssertEquals("GetRemainder (2)", true, xr.Read());
				AssertEquals("GetRemainder (3)", true, xr.Read());
				AssertEquals("GetRemainder (4)", "book", xr.Name);
				AssertEquals("GetRemainder (5)", true, xr.Read());
				AssertEquals("GetRemainder (6)", "title", xr.Name);
				AssertEquals("GetRemainder (7)", true, xr.Read());
				AssertEquals("GetRemainder (8)", "Understanding The Linux Kernel", xr.Value);
				AssertEquals("GetRemainder (9)", true, xr.Read());
				AssertEquals("GetRemainder (10)", "title", xr.Name);
				AssertEquals("GetRemainder (11)", true, xr.Read());
				AssertEquals("GetRemainder (12)", "author", xr.Name);
				AssertEquals("GetRemainder (13)", true, xr.Read());
				AssertEquals("GetRemainder (14)", "Daniel P. Bovet and Marco Cesati", xr.Value);
				AssertEquals("GetRemainder (15)", true, xr.Read());
				AssertEquals("GetRemainder (16)", "author", xr.Name);
				AssertEquals("GetRemainder (17)", true, xr.Read());
				AssertEquals("GetRemainder (18)", "book", xr.Name);

				Reset(xr.GetRemainder());
				xr.WhitespaceHandling = WhitespaceHandling.None;
				AssertEquals("GetRemainder (19)", true, xr.Read());
				AssertEquals("GetRemainder (20)", "book", xr.Name);
				AssertEquals("GetRemainder (21)", true, xr.Read());
				AssertEquals("GetRemainder (22)", "title", xr.Name);
				AssertEquals("GetRemainder (23)", true, xr.Read());
				AssertEquals("GetRemainder (24)", "Learning Perl", xr.Value);
				AssertEquals("GetRemainder (25)", true, xr.Read());
				AssertEquals("GetRemainder (26)", "title", xr.Name);
				AssertEquals("GetRemainder (27)", true, xr.Read());
				AssertEquals("GetRemainder (28)", "author", xr.Name);
				AssertEquals("GetRemainder (29)", true, xr.Read());
				AssertEquals("GetRemainder (30)", "Randal L. Schwartz and Tom Christiansen", xr.Value);
				AssertEquals("GetRemainder (31)", true, xr.Read());
				AssertEquals("GetRemainder (32)", "author", xr.Name);
				AssertEquals("GetRemainder (33)", true, xr.Read());
				AssertEquals("GetRemainder (34)", "book", xr.Name);

				Clear();
			}

	// Test the Read method.
	public void TestXmlTextReaderRead()
			{
				Reset(0);
				AssertEquals("Read (1)", true, xr.Read());
				AssertEquals("Read (2)", "soda", xr.Name);
				AssertEquals("Read (3)", "soda", xr.LocalName);
				AssertEquals("Read (4)", true, xr.Read());
				AssertEquals("Read (5)", true, xr.Read());
				AssertEquals("Read (6)", "size", xr.Name);
				AssertEquals("Read (7)", "size", xr.LocalName);
				AssertEquals("Read (8)", true, xr.Read());
				AssertEquals("Read (9)", "medium", xr.Value);

				Reset(2);
				AssertEquals("Read (10)", true, xr.Read());
				AssertEquals("Read (11)", "free", xr.Name);
				AssertEquals("Read (12)", true, xr.Read());
				AssertEquals("Read (13)", "software's freedom", xr.Value);

				Reset(4);
				AssertEquals("Read (14)", true, xr.Read());
				AssertEquals("Read (15)", "xml", xr.Name);
				AssertEquals("Read (16)", "1.0", xr["version"]);
				AssertEquals("Read (17)", true, xr.Read());
				AssertEquals("Read (18)", "test", xr.Name);
				AssertEquals("Read (19)", true, xr.Read());
				AssertEquals("Read (20)", "test", xr.Name);
				AssertEquals("Read (21)", true, xr.Read());
				AssertEquals("Read (22)", "Brave ", xr.Value);
				AssertEquals("Read (23)", true, xr.Read());
				AssertEquals("Read (24)", "GNU", xr.Name);
				AssertEquals("Read (25)", true, xr.Read());
				AssertEquals("Read (26)", " World... ", xr.Value);
				AssertEquals("Read (27)", true, xr.Read());
				AssertEquals("Read (28)", "fu", xr.Name);
				AssertEquals("Read (29)", true, xr.Read());
				AssertEquals("Read (30)", "bar", xr.Value);
				AssertEquals("Read (31)", true, xr.Read());
				AssertEquals("Read (32)", "test", xr.Name);

				Clear();
			}

	// Test the ReadAttributeValue method.
	public void TestXmlTextReaderReadAttributeValue()
			{
				Reset(0);
				AssertEquals("ReadAttributeValue (1)", true, xr.Read());
				AssertEquals("ReadAttributeValue (2)", true, xr.MoveToFirstAttribute());
				AssertEquals("ReadAttributeValue (3)", true, xr.ReadAttributeValue());
				AssertEquals("ReadAttributeValue (4)", XmlNodeType.Text, xr.NodeType);

				Clear();
			}

	// Test the ReadOuterXml method.
	public void TestXmlTextReaderReadOuterXml()
			{
				Reset(0);
				AssertEquals("ReadOuterXml (1)", true, xr.Read());
				AssertEquals("ReadOuterXml (2)", xml[0], xr.ReadOuterXml());

				Reset(1);
				AssertEquals("ReadOuterXml (3)", true, xr.Read());
				AssertEquals("ReadOuterXml (4)", xml[1], xr.ReadOuterXml());

				Reset(new StringReader("<outer><inner></inner></outer>"));
				AssertEquals("ReadOuterXml (5)", true, xr.Read());
				AssertEquals("ReadOuterXml (6)", true, xr.Read());
				AssertEquals("ReadOuterXml (7)", "<inner></inner>", xr.ReadOuterXml());

				Reset(new StringReader(@"<foo bar=""' '"" />"));
				AssertEquals("ReadOuterXml (8)", true, xr.Read());
				AssertEquals("ReadOuterXml (9)", @"<foo bar=""' '"" />", xr.ReadOuterXml());

				Clear();
			}

	// Test the ReadString method.
	public void TestXmlTextReaderReadString()
			{
				Reset(5);
				AssertEquals("ReadString (1)", XmlNodeType.Element, xr.MoveToContent());
				AssertEquals("ReadString (2)", "Brave GNU World ", xr.ReadString());
			}

	// Test if NodeType after reading empty element with ReadElementString() is XmlNodeType.EndElement (bug #14261).
	public void TestXmlTextReaderReadElementStringOnEmpyElement()
			{
				Reset(6);
				AssertEquals("ReadElementStringOnEmpyElement (1)", XmlNodeType.Element, xr.MoveToContent());
				AssertEquals("ReadElementStringOnEmpyElement (2)", true, xr.Read());
				AssertEquals("ReadElementStringOnEmpyElement (3)", String.Empty, xr.ReadElementString());
				AssertEquals("ReadElementStringOnEmpyElement (4)", XmlNodeType.EndElement, xr.NodeType);
			}

	// Test the Depth property.
	public void TestXmlTextReaderDepth()
			{
				Reset(new StringReader("<node attr=\"1\" />"));
				AssertEquals("Depth (1)", 0, xr.Depth);
				AssertEquals("Depth (2)", XmlNodeType.Element, xr.MoveToContent());
				AssertEquals("Depth (3)", 0, xr.Depth);
				AssertEquals("Depth (4)", true, xr.MoveToFirstAttribute());
				AssertEquals("Depth (5)", 1, xr.Depth);
				AssertEquals("Depth (6)", false, xr.Read());
				AssertEquals("Depth (7)", 0, xr.Depth);
				Clear();
			}

	// Test XML with char references &amp; and entities &xxx; in attributes.
	public void TestXmlTextReaderCharReferenceAndEntityInAttr()
			{
				string xmlText = "<doc a=\"C &amp;&amp; D\" b=\"C &xxx; D\" />";
				Reset(new StringReader(xmlText));
				xr.MoveToContent();
				AssertEquals("CharReferenceAndEntityInAttr (1)", "doc", xr.Name);
				xr.MoveToFirstAttribute();
				AssertEquals("CharReferenceAndEntityInAttr (2)", "a", xr.Name);
				AssertEquals("CharReferenceAndEntityInAttr (3)", "C && D", xr.Value);
				xr.MoveToNextAttribute();
				AssertEquals("CharReferenceAndEntityInAttr (4)", "b", xr.Name);
				AssertEquals("CharReferenceAndEntityInAttr (5)", "C &xxx; D", xr.Value);
			}
}; // class TestXmlTextReader
