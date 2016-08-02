/*
 * NameValueFileSectionHandler.cs - Implementation of the
 *		"System.Configuration.NameValueFileSectionHandler" interface.
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
using System.IO;
#if SECOND_PASS
using System.Xml;
#endif

public class NameValueFileSectionHandler : IConfigurationSectionHandler
{
	// Constructor.
	public NameValueFileSectionHandler() {}

#if SECOND_PASS

	// Create a configuration object for a section.
	public Object Create(Object parent, Object configContext, XmlNode section)
			{
				Object coll;
				String filename;
				ConfigXmlDocument doc;

				// Remove the "file" attribute from the section.
				XmlAttribute file =
					(section.Attributes.RemoveNamedItem("file")
							as XmlAttribute);

				// Load the main name/value pairs from the children.
				coll = NameValueSectionHandler.Create
					(parent, section, "key", "value");

				// Process the "file" attribute, if it is present.
				if(file != null && file.Value != String.Empty)
				{
					// Combine the base filename with the new filename.
					filename = file.Value;
					if(file is IConfigXmlNode)
					{
						filename = Path.Combine
							(Path.GetDirectoryName
								(((IConfigXmlNode)file).Filename), filename);
					}
					if(File.Exists(filename))
					{
						// Load the new configuration file into memory.
						doc = new ConfigXmlDocument();
						try
						{
							doc.Load(filename);
						}
						catch(XmlException xe)
						{
							throw new ConfigurationException
								(xe.Message, xe, filename, xe.LineNumber);
						}

						// The document root must match the section name.
						if(doc.DocumentElement.Name != section.Name)
						{
							throw new ConfigurationException
								(S._("Config_DocNameMatch"),
								 doc.DocumentElement);
						}

						// Load the contents of the file.
						coll = NameValueSectionHandler.Create
							(coll, doc.DocumentElement, "key", "value");
					}
				}

				// Return the final collection to the caller.
				return coll;
			}

#endif // SECOND_PASS

}; // class NameValueFileSectionHandler

#endif // !ECMA_COMPAT

}; // namespace System.Configuration
