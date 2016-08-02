/*
 * NameValueSectionHandler.cs - Implementation of the
 *		"System.Configuration.NameValueSectionHandler" interface.
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

namespace System.Configuration
{

#if !ECMA_COMPAT

using System;
using System.Collections.Specialized;
#if SECOND_PASS
using System.Xml;
#endif

public class NameValueSectionHandler : IConfigurationSectionHandler
{
	// Constructor.
	public NameValueSectionHandler() {}

	// Get the name of the key attribute tag.
	protected virtual String KeyAttributeName
			{
				get
				{
					return "key";
				}
			}

	// Get the name of the value attribute tag.
	protected virtual String ValueAttributeName
			{
				get
				{
					return "value";
				}
			}

#if SECOND_PASS

	// Get a required attribute and throw an exception if there are others.
	internal static String GetRequiredAttribute
				(XmlNode node, String name, int expected)
			{
				XmlAttribute attr =
					(node.Attributes.RemoveNamedItem(name) as XmlAttribute);
				if(attr == null || attr.Value == String.Empty)
				{
					throw new ConfigurationException
						(S._("Config_RequiredAttribute"), node);
				}
				if(node.Attributes.Count != expected)
				{
					throw new ConfigurationException
						(S._("Config_UnrecognizedAttribute"), node);
				}
				return attr.Value;
			}

	// Get an attribute and throw an exception if there are others.
	internal static String GetAttribute
				(XmlNode node, String name, int expected)
			{
				XmlAttribute attr =
					(node.Attributes.RemoveNamedItem(name) as XmlAttribute);
				if(node.Attributes.Count != expected)
				{
					throw new ConfigurationException
						(S._("Config_UnrecognizedAttribute"), node);
				}
				if(attr != null)
				{
					return attr.Value;
				}
				else
				{
					return String.Empty;
				}
			}

	// Create a configuration object for a section.
	public Object Create(Object parent, Object configContext, XmlNode section)
			{
				return Create(parent, section, KeyAttributeName,
							  ValueAttributeName);
			}
	internal static Object Create(Object parent, XmlNode section,
								  String keyName, String valueName)
			{
				ReadOnlyNameValueCollection coll;
				String key, value;

				// Create the name/value collection for the result.
				if(parent != null)
				{
					coll = new ReadOnlyNameValueCollection
						((NameValueCollection)parent);
				}
				else
				{
					coll = new ReadOnlyNameValueCollection();
				}

				// Must not be any attributes remaining on the section node.
				if(section.Attributes.Count != 0)
				{
					throw new ConfigurationException
						(S._("Config_UnrecognizedAttribute"), section);
				}

				// Process the child nodes.
				foreach(XmlNode node in section.ChildNodes)
				{
					// Ignore comments and white space.
					if(node.NodeType == XmlNodeType.Comment ||
					   node.NodeType == XmlNodeType.Whitespace ||
					   node.NodeType == XmlNodeType.SignificantWhitespace)
					{
						continue;
					}

					// Must be an element node.
					if(node.NodeType != XmlNodeType.Element)
					{
						throw new ConfigurationException
							(S._("Config_MustBeElement"), node);
					}

					// Process "add", "remove", and "clear" child tags.
					if(node.Name == "add")
					{
						key = GetRequiredAttribute(node, keyName, 1);
						value = GetRequiredAttribute(node, valueName, 0);
						coll[key] = value;
					}
					else if(node.Name == "remove")
					{
						key = GetRequiredAttribute(node, keyName, 0);
						coll.Remove(key);
					}
					else if(node.Name == "clear")
					{
						if(node.Attributes.Count != 0)
						{
							throw new ConfigurationException
								(S._("Config_UnrecognizedAttribute"), node);
						}
						coll.Clear();
					}
					else
					{
						throw new ConfigurationException
							(S._("Config_NotRecognized"), node);
					}
				}

				// Make the collection read-only and return it.
				coll.MakeReadOnly();
				return coll;
			}

#endif // SECOND_PASS

}; // class NameValueSectionHandler

#endif // !ECMA_COMPAT

}; // namespace System.Configuration
