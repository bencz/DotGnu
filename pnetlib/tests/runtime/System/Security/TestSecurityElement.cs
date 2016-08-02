/*
 * TestSecurityElement.cs - Test the "SecurityElement" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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
using System.Security;

public class TestSecurityElement : TestCase
{
	// Constructor.
	public TestSecurityElement(String name)
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

#if CONFIG_PERMISSIONS || CONFIG_POLICY_OBJECTS

	// Test the constructor.
	public void TestSecurityElementConstructor()
			{
				SecurityElement e;

				try
				{
					e = new SecurityElement(null);
					Fail("Constructor (1)");
				}
				catch(ArgumentNullException)
				{
					// Success.
				}
				try
				{
					e = new SecurityElement("");
					Fail("Constructor (2)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
				try
				{
					e = new SecurityElement("<");
					Fail("Constructor (3)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
				try
				{
					e = new SecurityElement("a", "<");
					Fail("Constructor (4)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
				try
				{
					e = new SecurityElement("&amp;");
					Fail("Constructor (5)");
				}
				catch(ArgumentException)
				{
					// Success.
				}
				e = new SecurityElement("foo", "bar");
				AssertEquals("Constructor (6)", "foo", e.Tag);
				AssertEquals("Constructor (7)", "bar", e.Text);
				e = new SecurityElement("foo");
				AssertEquals("Constructor (8)", "foo", e.Tag);
				AssertNull("Constructor (9)", e.Text);
				e = new SecurityElement("foo", "&amp;");
				AssertEquals("Constructor (10)", "foo", e.Tag);
				AssertEquals("Constructor (11)", "&", e.Text);
			}

	// Test the valid string testing operators.
	public void TestSecurityElementValidStrings()
			{
				Assert("ValidAttrName (1)",
					   !SecurityElement.IsValidAttributeName(null));
				Assert("ValidAttrName (2)",
					   !SecurityElement.IsValidAttributeName(""));
				Assert("ValidAttrName (3)",
					   !SecurityElement.IsValidAttributeName("a<b"));
				Assert("ValidAttrName (4)",
					   !SecurityElement.IsValidAttributeName("a>b"));
				Assert("ValidAttrName (5)",
					   !SecurityElement.IsValidAttributeName("&amp;"));
				Assert("ValidAttrName (6)",
					   !SecurityElement.IsValidAttributeName(" "));
				Assert("ValidAttrName (7)",
					   SecurityElement.IsValidAttributeName("fooBar"));
				Assert("ValidAttrName (8)",
					   SecurityElement.IsValidAttributeName("123"));

				Assert("ValidAttrValue (1)",
					   !SecurityElement.IsValidAttributeValue(null));
				Assert("ValidAttrValue (2)",
					   SecurityElement.IsValidAttributeValue(""));
				Assert("ValidAttrValue (3)",
					   !SecurityElement.IsValidAttributeValue("a<b"));
				Assert("ValidAttrValue (4)",
					   !SecurityElement.IsValidAttributeValue("a>b"));
				Assert("ValidAttrValue (5)",
					   SecurityElement.IsValidAttributeValue("&amp;"));
				Assert("ValidAttrValue (6)",
					   SecurityElement.IsValidAttributeValue(" "));
				Assert("ValidAttrValue (7)",
					   SecurityElement.IsValidAttributeValue("fooBar"));
				Assert("ValidAttrValue (8)",
					   SecurityElement.IsValidAttributeValue("123"));
				Assert("ValidAttrValue (9)",
					   !SecurityElement.IsValidAttributeValue("\""));

				Assert("ValidTag (1)",
					   !SecurityElement.IsValidTag(null));
				Assert("ValidTag (2)",
					   !SecurityElement.IsValidTag(""));
				Assert("ValidTag (3)",
					   !SecurityElement.IsValidTag("a<b"));
				Assert("ValidTag (4)",
					   !SecurityElement.IsValidTag("a>b"));
				Assert("ValidTag (5)",
					   !SecurityElement.IsValidTag("&amp;"));
				Assert("ValidTag (6)",
					   !SecurityElement.IsValidTag(" "));
				Assert("ValidTag (7)",
					   SecurityElement.IsValidTag("fooBar"));
				Assert("ValidTag (8)",
					   SecurityElement.IsValidTag("123"));

				Assert("ValidText (1)",
					   !SecurityElement.IsValidText(null));
				Assert("ValidText (2)",
					   SecurityElement.IsValidText(""));
				Assert("ValidText (3)",
					   !SecurityElement.IsValidText("a<b"));
				Assert("ValidText (4)",
					   !SecurityElement.IsValidText("a>b"));
				Assert("ValidText (5)",
					   SecurityElement.IsValidText("&amp;"));
				Assert("ValidText (6)",
					   SecurityElement.IsValidText(" "));
				Assert("ValidText (7)",
					   SecurityElement.IsValidText("fooBar"));
				Assert("ValidText (8)",
					   SecurityElement.IsValidText("123"));
				Assert("ValidText (9)",
					   SecurityElement.IsValidText("\""));
			}

	// Test the "Escape" method.
	public void TestSecurityElementEscape()
			{
				AssertNull("Escape (1)", SecurityElement.Escape(null));
				AssertEquals("Escape (2)", "abc65 *",
							 SecurityElement.Escape("abc65 *"));
				AssertEquals("Escape (3)", "ab&lt;&gt;c&amp;",
							 SecurityElement.Escape("ab<>c&"));
				AssertEquals("Escape (4)", "ab&lt;&gt;c&amp;d",
							 SecurityElement.Escape("ab<>c&d"));
			}

	// Test the "Unescape" method.  Note: "SecurityElement.Unescape"
	// is not public, so we have to do this via the "Text" property.
	private String Unescape(String value)
			{
				SecurityElement e = new SecurityElement("foo", value);
				return e.Text;
			}
	public void TestSecurityElementUnescape()
			{
				AssertNull("Unescape (1)", Unescape(null));
				AssertEquals("Unescape (2)", "abc65 *",
							 Unescape("abc65 *"));
				AssertEquals("Unescape (3)", "ab<>c&",
							 Unescape("ab&lt;&gt;c&amp;"));
				AssertEquals("Unescape (4)", "ab<>c&d",
							 Unescape("ab&lt;&gt;c&amp;d"));
			}

#endif

}; // class TestSecurityElement
