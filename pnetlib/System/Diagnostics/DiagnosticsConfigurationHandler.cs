/*
 * DiagnosticsConfigurationHandler.cs - Implementation of the
 *		"System.Diagnostics.DiagnosticsConfigurationHandler" interface.
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

namespace System.Diagnostics
{

#if !ECMA_COMPAT

using System;
using System.Collections;
#if SECOND_PASS
using System.Xml;
#endif
using System.Configuration;

/*

Structure of the diagnostics configuration section:

	<system.diagnostics>
		<assert [assertuienabled="FLAG"] [logfilename="FILENAME"]/>

		<switches>
			<add name="NAME1" value="VALUE1"/>
			<add name="NAME2" value="VALUE2"/>
			...
		</switches>

		<trace autoflush="FLAG" indentsize="VALUE">
			<listeners>
				<add [name="NAME"] type="TYPE" [initializeData="DATA"]/>
				<remove name="NAME"/>
				<clear/>
				...
			</listeners>
		</trace>
	</system.diagnostics>

The "assert" tag specifies where debug messages resulting from "Debug.Assert"
should be written.  If "assertuienabled" is "true", then a dialog box will
be displayed to the user.  If "logfilename" is present, then it specifies
the name of a log file to write to.

The "switches" tag specifies the values of diagnostic option switches.

The "trace" tag specifies whether the output trace buffer should be
automatically flushed (default is "false"), and the size of indent levels
for "Trace.Indent".  The "listeners" subtag specifies trace listeners
to add to the primary collection.

*/

public class DiagnosticsConfigurationHandler : IConfigurationSectionHandler
{
	// Constructor.
	public DiagnosticsConfigurationHandler() {}

#if SECOND_PASS

	// Get an attribute value.
	private static String GetAttribute(XmlNode node, String name)
			{
				XmlAttribute attr = node.Attributes[name];
				if(attr != null && attr.Value != null &&
				   attr.Value.Length != 0)
				{
					return attr.Value;
				}
				return null;
			}

	// Get a boolean attribute value.
	private static bool GetBoolAttribute(XmlNode node, String name, bool def)
			{
				String value = GetAttribute(node, name);
				if(value != null)
				{
					return (value == "true");
				}
				return def;
			}

	// Get a boolean value from an option collection.
	private static bool GetBool(Hashtable coll, String name)
			{
				Object value = coll[name];
				if(value != null)
				{
					return (bool)value;
				}
				else
				{
					return false;
				}
			}

	// Load a "listeners" tag from a diagnostic configuration element.
	// This is the odd one out in that it does not return the settings via
	// the return collection, but instead side-effects "Trace" directly.
	private static void LoadListeners(XmlNode node)
			{
				String name, type, data;
				Type resolvedType;
				TraceListener listener;
				foreach(XmlNode child in node.ChildNodes)
				{
					if(child.NodeType != XmlNodeType.Element)
					{
						continue;
					}
					switch(child.Name)
					{
						case "add":
						{
							name = GetAttribute(child, "name");
							type = GetAttribute(child, "type");
							data = GetAttribute(child, "initializeData");
							if(type == null ||
							   (resolvedType = Type.GetType(type)) == null)
							{
								break;
							}
							if(data != null)
							{
								listener = (TraceListener)
									Activator.CreateInstance
										(resolvedType, new Object [] {data});
							}
							else
							{
								listener = (TraceListener)
									Activator.CreateInstance(resolvedType);
							}
							if(name != null)
							{
								listener.Name = name;
							}
							Trace.Listeners.Add(listener);
						}
						break;

						case "remove":
						{
							name = GetAttribute(child, "name");
							if(name != null)
							{
								Trace.Listeners.Remove(name);
							}
						}
						break;

						case "clear":
						{
							Trace.Listeners.Clear();
						}
						break;
					}
				}
			}

	// Create a configuration object for a section.
	public virtual Object Create
				(Object parent, Object configContext, XmlNode section)
			{
				Hashtable coll;

				// Create the collection that will contain the results.
				if(parent != null)
				{
					coll = new Hashtable((Hashtable)parent);
				}
				else
				{
					coll = new Hashtable();
				}

				// Process the child nodes.
				foreach(XmlNode node in section.ChildNodes)
				{
					if(node.NodeType != XmlNodeType.Element)
					{
						continue;
					}
					switch(node.Name)
					{
						case "assert":
						{
							// Process the "assertuienabled" flag.
							coll["assertuienabled"] = GetBoolAttribute
								(node, "assertuienabled",
								 GetBool(coll, "assertuienabled"));

							// Process the "logfilename" value.
							String filename = GetAttribute(node, "logfilename");
							if(filename != null)
							{
								coll["logfilename"] = filename;
							}
						}
						break;

						case "switches":
						{
							// Load the switches and merge them with the
							// current switch settings.
							coll["switches"] =
								(new SwitchesDictionarySectionHandler()).Create
									(coll["switches"], configContext, node);
						}
						break;

						case "trace":
						{
							// Process the "autoflush" flag.
							coll["autoflush"] = GetBoolAttribute
								(node, "autoflush",
								 GetBool(coll, "autoflush"));

							// Process the "indentsize" attribute.
							String newValue = GetAttribute(node, "indentsize");
							if(newValue != null)
							{
								coll["indentsize"] = Int32.Parse(newValue);
							}

							// Load the trace listeners.
							foreach(XmlNode node2 in node.ChildNodes)
							{
								if(node2.NodeType == XmlNodeType.Element &&
								   node2.Name == "listeners")
								{
									LoadListeners(node2);
								}
							}
						}
						break;
					}
				}

				// Return the loaded settings to the caller.
				return coll;
			}

	// Special dictionary section handler for switch values.
	private class SwitchesDictionarySectionHandler : DictionarySectionHandler
	{
		// Constructor.
		public SwitchesDictionarySectionHandler() {}

		// Get the name of the key attribute tag.
		protected override String KeyAttributeName
				{
					get
					{
						return "name";
					}
				}

	}; // class SwitchesDictionarySectionHandler

#endif // SECOND_PASS

}; // class DiagnosticsConfigurationHandler

#endif // !ECMA_COMPAT

}; // namespace System.Diagnostics
