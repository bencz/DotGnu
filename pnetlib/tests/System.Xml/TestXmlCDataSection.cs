/*
 * TestXmlCDataSection.cs - Tests for the "System.Xml.XmlCDataSection" class.
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

public class TestXmlCDataSection : TestCase
{
	// Internal state.
	private XmlDocument doc;

	// Constructor.
	public TestXmlCDataSection(String name)
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

	// Check the properties on a newly constructed CDATA section.
	private void CheckProperties(String msg, XmlCDataSection cdata,
								 String value, bool failXml)
			{
				String temp;
				AssertEquals(msg + " [1]", "#cdata-section", cdata.LocalName);
				AssertEquals(msg + " [2]", "#cdata-section", cdata.Name);
				AssertEquals(msg + " [3]", String.Empty, cdata.Prefix);
				AssertEquals(msg + " [4]", String.Empty, cdata.NamespaceURI);
				AssertEquals(msg + " [5]", XmlNodeType.CDATA, cdata.NodeType);
				AssertEquals(msg + " [6]", value, cdata.Data);
				AssertEquals(msg + " [7]", value, cdata.Value);
				AssertEquals(msg + " [8]", value, cdata.InnerText);
				AssertEquals(msg + " [9]", value.Length, cdata.Length);
				AssertEquals(msg + " [10]", String.Empty, cdata.InnerXml);
				if(failXml)
				{
					try
					{
						temp = cdata.OuterXml;
						Fail(msg + " [11]");
					}
					catch(ArgumentException)
					{
						// Success
					}
				}
				else
				{
					AssertEquals(msg + " [12]",
								 "<![CDATA[" + value + "]]>",
								 cdata.OuterXml);
				}
			}

	// Test the construction of CDATA section nodes.
	public void TestXmlCDataSectionConstruct()
			{
				CheckProperties("Construct (1)",
								doc.CreateCDataSection(null),
								String.Empty, false);
				CheckProperties("Construct (2)",
								doc.CreateCDataSection(String.Empty),
								String.Empty, false);
				CheckProperties("Construct (3)",
								doc.CreateCDataSection("xyzzy"),
								"xyzzy", false);
				CheckProperties("Construct (4)",
								doc.CreateCDataSection("]]>"),
								"]]>", true);
				CheckProperties("Construct (5)",
								doc.CreateCDataSection("<&>"),
								"<&>", false);
			}

}; // class TestXmlCDataSection

#endif // !ECMA_COMPAT
