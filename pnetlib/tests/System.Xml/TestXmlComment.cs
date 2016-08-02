/*
 * TestXmlComment.cs - Tests for the "System.Xml.TestXmlComment" class.
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

public class TestXmlComment : TestCase
{
	// Internal state.
	private XmlDocument doc;

	// Constructor.
	public TestXmlComment(String name)
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

	// Check the properties on a newly constructed comment node.
	private void CheckProperties(String msg, XmlComment comment,
								 String value, bool failXml)
			{
				String temp;
				AssertEquals(msg + " [1]", "#comment", comment.LocalName);
				AssertEquals(msg + " [2]", "#comment", comment.Name);
				AssertEquals(msg + " [3]", String.Empty, comment.Prefix);
				AssertEquals(msg + " [4]", String.Empty, comment.NamespaceURI);
				AssertEquals(msg + " [5]",
							 XmlNodeType.Comment, comment.NodeType);
				AssertEquals(msg + " [6]", value, comment.Data);
				AssertEquals(msg + " [7]", value, comment.Value);
				AssertEquals(msg + " [8]", value, comment.InnerText);
				AssertEquals(msg + " [9]", value.Length, comment.Length);
				AssertEquals(msg + " [10]", String.Empty, comment.InnerXml);
				if(failXml)
				{
					try
					{
						temp = comment.OuterXml;
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
								 "<!--" + value + "-->",
								 comment.OuterXml);
				}
			}

	// Test the construction of comment nodes.
	public void TestXmlCommentConstruct()
			{
				CheckProperties("Construct (1)",
								doc.CreateComment(null),
								String.Empty, false);
				CheckProperties("Construct (2)",
								doc.CreateComment(String.Empty),
								String.Empty, false);
				CheckProperties("Construct (3)",
								doc.CreateComment("xyzzy"),
								"xyzzy", false);
				CheckProperties("Construct (4)",
								doc.CreateComment("-->"),
								"-->", true);
				CheckProperties("Construct (5)",
								doc.CreateComment("<&>"),
								"<&>", false);
				CheckProperties("Construct (6)",
								doc.CreateComment("-"),
								"-", false);
			}

}; // class TestXmlComment

#endif // !ECMA_COMPAT
