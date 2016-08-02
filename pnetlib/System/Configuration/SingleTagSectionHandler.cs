/*
 * SingleTagSectionHandler.cs - Implementation of the
 *		"System.Configuration.SingleTagSectionHandler" interface.
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
#if SECOND_PASS
using System.Xml;
#endif

public class SingleTagSectionHandler : IConfigurationSectionHandler
{
	// Constructor.
	public SingleTagSectionHandler() {}

#if SECOND_PASS

	// Create a configuration object for a section.
	public virtual Object Create
				(Object parent, Object configContext, XmlNode section)
			{
				// The section must not have child nodes.
				if(section.HasChildNodes)
				{
					throw new ConfigurationException
						(S._("Config_HasChildNodes"), section.FirstChild);
				}

				// Create the hash table to hold the results.
				Hashtable hash;
				if(parent != null)
				{
					hash = new Hashtable((Hashtable)parent);
				}
				else
				{
					hash = new Hashtable();
				}

				// Add configuration information from the specified section.
				foreach(XmlAttribute attr in section.Attributes)
				{
					hash[attr.Name] = attr.Value;
				}

				// Return the result hash table to the caller.
				return hash;
			}

#endif // SECOND_PASS

}; // class SingleTagSectionHandler

#endif // !ECMA_COMPAT

}; // namespace System.Configuration
