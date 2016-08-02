/*
 * SecurityElement.cs - Implementation of the
 *		"System.Security.SecurityElement" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Security
{

#if CONFIG_PERMISSIONS || CONFIG_POLICY_OBJECTS || CONFIG_REMOTING

using System;
using System.Text;
using System.Collections;

// Note: ECMA only specifies the "ToString()" method for this
// class, but it isn't very useful unless the other framework
// methods are also present.

public sealed class SecurityElement
{

	// Internal state.
	private String tag;
	private String text;
	private ArrayList attributes;
	private ArrayList children;

	// Attribute name/value information
	private sealed class AttrNameValue
	{
		// Internal state.
		public String name;
		public String value;

		// Constructor.
		public AttrNameValue(String name, String value)
				{
					this.name  = name;
					this.value = value;
				}

	}; // class AttrNameValue

	// Constructors.
	public SecurityElement(String tag)
			: this(tag, null)
			{
				// Nothing to do here.
			}
	public SecurityElement(String tag, String text)
			{
				if(tag == null)
				{
					throw new ArgumentNullException("tag");
				}
				else if(!IsValidTag(tag))
				{
					throw new ArgumentException(_("Arg_InvalidXMLTag"));
				}
				else if(text != null && !IsValidText(text))
				{
					throw new ArgumentException(_("Arg_InvalidXMLText"));
				}
				this.tag = tag;
				this.text = text;
			}

	// Invalid XML characters.
	private static readonly char[] InvalidChars = {'<', '>', '&', '"', '\''};

	// Invalid XML tag and attribute name characters.
	private static readonly char[] InvalidNameChars =
			{'<', '>', '&', '"', '\'', '=',
		     '\u0009', '\u000A', '\u000B', '\u000C', '\u000D', '\u0020',
		     '\u00A0', '\u2001', '\u2002', '\u2003', '\u2004', '\u2005',
		     '\u2006', '\u2007', '\u2008', '\u2009', '\u200A', '\u200B',
		     '\u3000', '\uFEFF'};

	// Invalid XML characters in text sections.  It is assumed that
	// occurrences of '&' are correctly-formatted XML escapes.
	private static readonly char[] InvalidTextChars = {'<', '>'};

	// Invalid XML characters in attribute values.  It is assumed that
	// occurrences of '&' are correctly-formatted XML escapes.
	private static readonly char[] InvalidAttrChars = {'<', '>', '"'};

	// Escape invalid XML characters in a string.
	public static String Escape(String str)
			{
				StringBuilder newStr;
				int start;
				int index;
				char ch;

				// The null string is encoded as itself.
				if(str == null)
				{
					return null;
				}

				// Do nothing if the string does not contain invalid chars.
				index = str.IndexOfAny(InvalidChars);
				if(index < 0)
				{
					return str;
				}

				// Replace the invalid characters and build a new string.
				newStr = new StringBuilder(str.Substring(0, index));
				for(;;)
				{
					ch = str[index++];
					if(ch == '<')
					{
						newStr.Append("&lt;");
					}
					else if(ch == '>')
					{
						newStr.Append("&gt;");
					}
					else if(ch == '&')
					{
						newStr.Append("&amp;");
					}
					else if(ch == '"')
					{
						newStr.Append("&quot;");
					}
					else
					{
						newStr.Append("&apos;");
					}
					start = index;
					if(start >= str.Length)
					{
						break;
					}
					index = str.IndexOfAny(InvalidChars,
										   start, str.Length - start);
					if(index == -1)
					{
						newStr.Append(str.Substring(start));
						break;
					}
					else if(index > start)
					{
						newStr.Append(str.Substring(start, index - start));
					}
				}

				// Return the escaped string to the caller.
				return newStr.ToString();
			}

	// Unescape invalid XML characters in a string.
	private static String Unescape(String str)
			{
				// Bail out early if there are no escapes in the string.
				if(str == null || str.IndexOf('&') == -1)
				{
					return str;
				}

				// Construct a new string with the escapes removed.
				StringBuilder newStr = new StringBuilder();
				int posn = 0;
				char ch;
				String name;
				while(posn < str.Length)
				{
					ch = str[posn++];
					if(ch == '&')
					{
						name = String.Empty;
						while(posn < str.Length && str[posn] != ';')
						{
							name += str[posn];
							++posn;
						}
						if(posn < str.Length)
						{
							++posn;
						}
						if(name == "lt")
						{
							newStr.Append('<');
						}
						else if(name == "gt")
						{
							newStr.Append('>');
						}
						else if(name == "amp")
						{
							newStr.Append('&');
						}
						else if(name == "quot")
						{
							newStr.Append('"');
						}
						else if(name == "apos")
						{
							newStr.Append('\'');
						}
					}
					else
					{
						newStr.Append(ch);
					}
				}
				return newStr.ToString();
			}

	// Determine if a string is a valid attribute name.
	public static bool IsValidAttributeName(String name)
			{
				if(name == null || name.Length == 0)
				{
					return false;
				}
				else
				{
					return (name.IndexOfAny(InvalidNameChars) < 0);
				}
			}

	// Determine if a string is a valid attribute value.
	public static bool IsValidAttributeValue(String value)
			{
				if(value == null)
				{
					return false;
				}
				else
				{
					return (value.IndexOfAny(InvalidAttrChars) < 0);
				}
			}

	// Determine if a string is a valid tag name.
	public static bool IsValidTag(String name)
			{
				if(name == null || name.Length == 0)
				{
					return false;
				}
				else
				{
					return (name.IndexOfAny(InvalidNameChars) < 0);
				}
			}

	// Determine if a string is valid element text.
	public static bool IsValidText(String text)
			{
				if(text == null)
				{
					return false;
				}
				else
				{
					return (text.IndexOfAny(InvalidTextChars) < 0);
				}
			}

	// Get the attributes for this element.
	public Hashtable Attributes
			{
				get
				{
					if(attributes == null)
					{
						return null;
					}
					else
					{
						Hashtable table = new Hashtable();
						IEnumerator e = attributes.GetEnumerator();
						AttrNameValue nv;
						while(e.MoveNext())
						{
							nv = (AttrNameValue)(e.Current);
							table.Add(nv.name, nv.value);
						}
						return table;
					}
				}
				set
				{
					if(value == null)
					{
						attributes = null;
					}
					else
					{
						attributes = new ArrayList(value.Count);
						IDictionaryEnumerator e = value.GetEnumerator();
						String name, val;
						while(e.MoveNext())
						{
							name = (String)(e.Key);
							val = (String)(e.Value);
							if(!IsValidAttributeName(name))
							{
								throw new ArgumentException
									(_("Arg_InvalidXMLAttrName"));
							}
							if(!IsValidAttributeValue(val))
							{
								throw new ArgumentException
									(_("Arg_InvalidXMLAttrValue"));
							}
							attributes.Add(new AttrNameValue(name, val));
						}
					}
				}
			}

	// Get or set the children of this XML element.
	public ArrayList Children
			{
				get
				{
					return children;
				}
				set
				{
					if(value == null)
					{
						children = null;
					}
					else
					{
						int posn;
						for(posn = 0; posn < value.Count; ++posn)
						{
							if(value[posn] == null)
							{
								throw new ArgumentNullException
									("value[" + posn.ToString() + "]");
							}
						}
						children = value;
					}
				}
			}

	// Get or set the tag name.
	public String Tag
			{
				get
				{
					return tag;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					else if(!IsValidTag(value))
					{
						throw new ArgumentException(_("Arg_InvalidXMLTag"));
					}
					tag = value;
				}
			}

	// Get or set the element text.
	public String Text
			{
				get
				{
					return Unescape(text);
				}
				set
				{
					if(value != null && !IsValidText(value))
					{
						throw new ArgumentException(_("Arg_InvalidXMLText"));
					}
					text = value;
				}
			}

	// Add an attribute to this element.
	public void AddAttribute(String name, String value)
			{
				// Validate the parameters.
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(!IsValidAttributeName(name))
				{
					throw new ArgumentException
						(_("Arg_InvalidXMLAttrName"));
				}
				if(!IsValidAttributeValue(value))
				{
					throw new ArgumentException
						(_("Arg_InvalidXMLAttrValue"));
				}

				// Search for an attribute with the same name.
				if(attributes != null)
				{
					int posn;
					for(posn = 0; posn < attributes.Count; ++posn)
					{
						if(((AttrNameValue)(attributes[posn])).name == name)
						{
							throw new ArgumentException
								(_("Arg_DuplicateXMLAttr"));
						}
					}
				}
				else
				{
					attributes = new ArrayList();
				}

				// Add the new attribute.
				attributes.Add(new AttrNameValue(name, value));
			}

	// Set an attribute within this element.
	internal void SetAttribute(String name, String value)
			{
				// Validate the parameters.
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(!IsValidAttributeName(name))
				{
					throw new ArgumentException
						(_("Arg_InvalidXMLAttrName"));
				}
				if(!IsValidAttributeValue(value))
				{
					throw new ArgumentException
						(_("Arg_InvalidXMLAttrValue"));
				}

				// Search for an attribute with the same name.
				if(attributes != null)
				{
					int posn;
					for(posn = 0; posn < attributes.Count; ++posn)
					{
						if(((AttrNameValue)(attributes[posn])).name == name)
						{
							((AttrNameValue)(attributes[posn])).value = value;
							return;
						}
					}
				}
				else
				{
					attributes = new ArrayList();
				}

				// Add the new attribute.
				attributes.Add(new AttrNameValue(name, value));
			}

	// Add a child to this element.
	public void AddChild(SecurityElement child)
			{
				if(child == null)
				{
					throw new ArgumentNullException("child");
				}
				if(children == null)
				{
					children = new ArrayList();
				}
				children.Add(child);
			}

	// Get the value of a specific attribute.
	public String Attribute(String name)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				else if(attributes == null)
				{
					return null;
				}
				int posn;
				AttrNameValue nv;
				for(posn = 0; posn < attributes.Count; ++posn)
				{
					nv = (AttrNameValue)(attributes[posn]);
					if(name == nv.name)
					{
						return Unescape(nv.value);
					}
				}
				return null;
			}

	// Compare two security elements for equality.
	public bool Equal(SecurityElement other)
			{
				int posn;
				AttrNameValue nv;

				// Check the easy cases first.
				if(other == null)
				{
					return false;
				}
				else if(tag != other.tag || text != other.text)
				{
					return false;
				}

				// Compare the attribute values.
				if(attributes == null && other.attributes != null)
				{
					return false;
				}
				else if(attributes != null && other.attributes == null)
				{
					return false;
				}
				else if(attributes != null)
				{
					if(attributes.Count != other.attributes.Count)
					{
						return false;
					}
					for(posn = 0; posn < attributes.Count; ++posn)
					{
						nv = (AttrNameValue)(attributes[posn]);
						if(other.Attribute(nv.name) != nv.value)
						{
							return false;
						}
					}
				}

				// Compare the children.
				if(children == null && other.children != null)
				{
					return false;
				}
				else if(children != null && other.children == null)
				{
					return false;
				}
				else if(children != null)
				{
					if(children.Count != other.children.Count)
					{
						return false;
					}
					for(posn = 0; posn < children.Count; ++posn)
					{
						if(!((SecurityElement)(children[posn])).Equal
								((SecurityElement)(other.children[posn])))
						{
							return false;
						}
					}
				}

				// The elements are identical.
				return true;
			}

	// Search for a child by tag name.
	public SecurityElement SearchForChildByTag(String tag)
			{
				if(tag == null)
				{
					throw new ArgumentNullException("tag");
				}
				if(children != null)
				{
					int posn;
					SecurityElement child;
					for(posn = 0; posn < children.Count; ++posn)
					{
						child = (SecurityElement)(children[posn]);
						if(child.Tag == tag)
						{
							return child;
						}
					}
				}
				return null;
			}

	// Search for the text of a named child.
	public String SearchForTextOfTag(String tag)
			{
				if(tag == null)
				{
					throw new ArgumentNullException("tag");
				}
				if(children != null)
				{
					int posn;
					SecurityElement child;
					for(posn = 0; posn < children.Count; ++posn)
					{
						child = (SecurityElement)(children[posn]);
						if(child.Tag == tag)
						{
							return child.Text;
						}
					}
				}
				return null;
			}

	// Convert this security element into an XML string.
	public override String ToString()
			{
				int posn;
				AttrNameValue nv;
				String result = "<" + tag;
				if(attributes != null)
				{
					// Add the attribute values.
					for(posn = 0; posn < attributes.Count; ++posn)
					{
						nv = (AttrNameValue)(attributes[posn]);
						result += " " + nv.name + "=\"" + nv.value + "\"";
					}
				}
				if(text == null &&
				   (children == null || children.Count == 0))
				{
					// The element has no contents, so use the short-cut syntax.
					result += "/>" + Environment.NewLine;
				}
				else
				{
					// Add the text and child elements.
					result += ">";
					if(text != null)
					{
						result += Escape(text);
					}
					else
					{
						result += Environment.NewLine;
					}
					if(children != null)
					{
						for(posn = 0; posn < children.Count; ++posn)
						{
							result +=
								((SecurityElement)(children[posn])).ToString();
						}
					}
					result += "</" + tag + ">" + Environment.NewLine;
				}
				return result;
			}

	// Parse an XML string into a tree of "SecurityElement" values.
	internal static SecurityElement Parse(String xmlString)
			{
				MiniXml xml = new MiniXml(xmlString);
				return xml.Parse();
			}

	// Search for a child by tag path.
	internal SecurityElement SearchForChildByPath(params String[] tags)
			{
				if(tags == null)
				{
					throw new ArgumentNullException("tags");
				}
				SecurityElement current = this;
				foreach(String tag in tags)
				{
					current = current.SearchForChildByTag(tag);
					if(current == null)
					{
						break;
					}
				}
				return current;
			}

	// Get the value of an attribute by tag path.  The last string
	// in the path is the attribute name.
	internal String AttributeByPath(params String[] tags)
			{
				if(tags == null)
				{
					throw new ArgumentNullException("tags");
				}
				SecurityElement current = this;
				int posn = 0;
				while(posn < (tags.Length - 1))
				{
					current = current.SearchForChildByTag(tags[posn]);
					if(current == null)
					{
						return null;
					}
					++posn;
				}
				return current.Attribute(tags[posn]);
			}

}; // class SecurityElement

#endif // CONFIG_PERMISSIONS || CONFIG_POLICY_OBJECTS || CONFIG_REMOTING

}; // namespace System.Security
