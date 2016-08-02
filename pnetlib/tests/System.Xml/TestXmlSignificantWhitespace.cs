/*
 * TestXmlSignificantWhitespace.cs - Tests for the
 *		"System.Xml.TestXmlSignificantWhitespace" class.
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

public class TestXmlSignificantWhitespace : TestCase
{
	// Internal state.
	private XmlDocument doc;

	// Constructor.
	public TestXmlSignificantWhitespace(String name)
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

	// Check the properties on a newly constructed whitespace node.
	private void CheckProperties(String msg, XmlSignificantWhitespace white,
								 String value, bool failXml)
			{
				String temp;
				AssertEquals(msg + " [1]",
						     "#significant-whitespace", white.LocalName);
				AssertEquals(msg + " [2]",
							 "#significant-whitespace", white.Name);
				AssertEquals(msg + " [3]", String.Empty, white.Prefix);
				AssertEquals(msg + " [4]", String.Empty, white.NamespaceURI);
				AssertEquals(msg + " [5]",
							 XmlNodeType.SignificantWhitespace, white.NodeType);
				AssertEquals(msg + " [6]", value, white.Data);
				AssertEquals(msg + " [7]", value, white.Value);
				AssertEquals(msg + " [8]", value, white.InnerText);
				AssertEquals(msg + " [9]", value.Length, white.Length);
				AssertEquals(msg + " [10]", String.Empty, white.InnerXml);
				if(failXml)
				{
					try
					{
						temp = white.OuterXml;
						Fail(msg + " [11]");
					}
					catch(ArgumentException)
					{
						// Success
					}
				}
				else
				{
					AssertEquals(msg + " [12]", value, white.OuterXml);
				}
			}

	// Test the construction of whitespace nodes.
	public void TestXmlSignificantWhitespaceConstruct()
			{
				// Valid significant whitespace strings.
				CheckProperties("Construct (1)",
								doc.CreateSignificantWhitespace(null),
								String.Empty, false);
				CheckProperties("Construct (2)",
								doc.CreateSignificantWhitespace(String.Empty),
								String.Empty, false);
				CheckProperties("Construct (3)",
								doc.CreateSignificantWhitespace(" \f\t\r\n\v"),
								" \f\t\r\n\v", false);

				// Invalid significant whitespace strings.
				try
				{
					doc.CreateWhitespace("abc");
					Fail("Construct (4)");
				}
				catch(ArgumentException)
				{
					// Success
				}
			}

	// Test the setting of whitespace values.
	public void TestXmlSignificantWhitespaceSetValue()
			{
				XmlSignificantWhitespace white;
				white = doc.CreateSignificantWhitespace(null);
				white.Value = String.Empty;
				white.Value = " \f\t\r\n\v";
				white.Value = null;
				try
				{
					white.Value = "abc";
					Fail("SetValue (1)");
				}
				catch(ArgumentException)
				{
					// Success
				}
			}

}; // class TestXmlSignificantWhitespace

#endif // !ECMA_COMPAT
