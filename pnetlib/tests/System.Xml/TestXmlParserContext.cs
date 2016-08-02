/*
 * TestXmlParserContext.cs - Tests for the
 *		"System.Xml.TestXmlParserContext" class.
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
using System.Text;
using System.Xml;

public class TestXmlParserContext : TestCase
{
	// Constructor.
	public TestXmlParserContext(String name)
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

	// Test parser context creation.
	public void TestXmlParserContextConstruct()
			{
				NameTable nt = new NameTable();
				NameTable nt2 = new NameTable();
				XmlNamespaceManager ns = new XmlNamespaceManager(nt);
				XmlParserContext ctx;

				// Try to construct with the wrong name table.
				try
				{
					ctx = new XmlParserContext(nt2, ns, null, XmlSpace.None);
					Fail("Construct (1)");
				}
				catch(XmlException)
				{
					// Success
				}

				// Check default property values.
				ctx = new XmlParserContext(null, ns, null, XmlSpace.None);
				AssertEquals("Construct (2)", "", ctx.BaseURI);
				AssertEquals("Construct (3)", "", ctx.DocTypeName);
				AssertEquals("Construct (4)", null, ctx.Encoding);
				AssertEquals("Construct (5)", "", ctx.InternalSubset);
				AssertEquals("Construct (6)", nt, ctx.NameTable);
				AssertEquals("Construct (7)", ns, ctx.NamespaceManager);
				AssertEquals("Construct (8)", "", ctx.PublicId);
				AssertEquals("Construct (9)", "", ctx.SystemId);
				AssertEquals("Construct (10)", "", ctx.XmlLang);
				AssertEquals("Construct (11)", XmlSpace.None, ctx.XmlSpace);

				// Check overridden property values.
				ctx = new XmlParserContext(nt, null, "doctype",
										   "pubid", "sysid", "internal",
										   "base", "lang", XmlSpace.Preserve,
										   Encoding.UTF8);
				AssertEquals("Construct (12)", "base", ctx.BaseURI);
				AssertEquals("Construct (13)", "doctype", ctx.DocTypeName);
				AssertEquals("Construct (14)", Encoding.UTF8, ctx.Encoding);
				AssertEquals("Construct (15)", "internal", ctx.InternalSubset);
				AssertEquals("Construct (16)", nt, ctx.NameTable);
				AssertEquals("Construct (17)", null, ctx.NamespaceManager);
				AssertEquals("Construct (18)", "pubid", ctx.PublicId);
				AssertEquals("Construct (19)", "sysid", ctx.SystemId);
				AssertEquals("Construct (20)", "lang", ctx.XmlLang);
				AssertEquals("Construct (21)", XmlSpace.Preserve, ctx.XmlSpace);
			}

	// Test parser context property changes.
	public void TestXmlParserContextProperties()
			{
				NameTable nt = new NameTable();
				NameTable nt2 = new NameTable();
				XmlNamespaceManager ns = new XmlNamespaceManager(nt);
				XmlParserContext ctx = new XmlParserContext
					(nt, ns, null, XmlSpace.None);

				// Set and check the various properties.
				ctx.BaseURI = "xyzzy";
				AssertEquals("BaseURI (1)", "xyzzy", ctx.BaseURI);
				ctx.BaseURI = null;
				AssertEquals("BaseURI (2)", "", ctx.BaseURI);

				ctx.DocTypeName = "xyzzy";
				AssertEquals("DocTypeName (1)", "xyzzy", ctx.DocTypeName);
				ctx.DocTypeName = null;
				AssertEquals("DocTypeName (2)", "", ctx.DocTypeName);

				ctx.Encoding = Encoding.UTF8;
				AssertEquals("Encoding (1)", Encoding.UTF8, ctx.Encoding);
				ctx.Encoding = null;
				AssertEquals("Encoding (2)", null, ctx.Encoding);

				ctx.InternalSubset = "xyzzy";
				AssertEquals("InternalSubset (1)", "xyzzy", ctx.InternalSubset);
				ctx.InternalSubset = null;
				AssertEquals("InternalSubset (2)", "", ctx.InternalSubset);

				ctx.NameTable = nt2;
				AssertEquals("NameTable (1)", nt2, ctx.NameTable);
				ctx.NameTable = null;
				AssertEquals("NameTable (2)", null, ctx.NameTable);

				ctx.NamespaceManager = null;
				AssertEquals("NamespaceManager (1)", null,
							 ctx.NamespaceManager);
				ctx.NamespaceManager = ns;
				AssertEquals("NamespaceManager (2)", ns, ctx.NamespaceManager);

				ctx.PublicId = "xyzzy";
				AssertEquals("PublicId (1)", "xyzzy", ctx.PublicId);
				ctx.PublicId = null;
				AssertEquals("PublicId (2)", "", ctx.PublicId);

				ctx.SystemId = "xyzzy";
				AssertEquals("SystemId (1)", "xyzzy", ctx.SystemId);
				ctx.SystemId = null;
				AssertEquals("PublicId (2)", "", ctx.SystemId);

				ctx.XmlLang = "xyzzy";
				AssertEquals("XmlLang (1)", "xyzzy", ctx.XmlLang);
				ctx.XmlLang = null;
				AssertEquals("XmlLang (2)", "", ctx.XmlLang);

				ctx.XmlSpace = XmlSpace.Default;
				AssertEquals("XmlSpace (1)", XmlSpace.Default, ctx.XmlSpace);
				ctx.XmlSpace = XmlSpace.None;
				AssertEquals("XmlSpace (2)", XmlSpace.None, ctx.XmlSpace);
			}

}; // class TestXmlParserContext
