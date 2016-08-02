/*
 * DictionarySectionHandler.cs - Implementation of the
 *		"System.Configuration.DictionarySectionHandler" interface.
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
using System.Collections;
using System.Collections.Specialized;
#if SECOND_PASS
using System.Xml;
#endif

public class DictionarySectionHandler : IConfigurationSectionHandler
{
	// Constructor.
	public DictionarySectionHandler() {}

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

	// Create a configuration object for a section.
	public virtual Object Create
				(Object parent, Object configContext, XmlNode section)
			{
				Hashtable coll;
				String key, value;

				// Create the name/value collection for the result.
				if(parent != null)
				{
					coll = (Hashtable)(((Hashtable)parent).Clone());
				}
				else
				{
					coll = CollectionsUtil.CreateCaseInsensitiveHashtable();
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
						key = NameValueSectionHandler.GetRequiredAttribute
							(node, KeyAttributeName, 1);
						value = NameValueSectionHandler.GetAttribute
							(node, ValueAttributeName, 0);
						coll[key] = value;
					}
					else if(node.Name == "remove")
					{
						key = NameValueSectionHandler.GetRequiredAttribute
							(node, KeyAttributeName, 0);
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

				// Return the final collection
				return coll;
			}

#endif // SECOND_PASS

}; // class DictionarySectionHandler

#endif // !ECMA_COMPAT

}; // namespace System.Configuration
