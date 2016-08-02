/*
 * ConfigurationSettings.cs - Implementation of the
 *		"System.Configuration.ConfigurationSettings" interface.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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
using System.Collections;
using System.Collections.Specialized;
#if SECOND_PASS
using System.Xml;
#endif
using Platform;

/*

Layout of the configuration file for data accessible by this API:

	<configuration>
		<configSections>
			<sectionGroup name="NAME">
				<!-- sections and groups -->
			</sectionGroup>

			<section name="NAME" type="TYPE" [allowLocation="?"]
					 [allowDefinition="?"] />

			<remove name="NAME"/>

			<clear/>
		</configSections>

		<!-- actual settings sections -->
	</configuration>

Configuration section names have the form "NAME1/NAME2/NAME3",
for each level down through the XML file.

The "configSections" tag defines the "schema" for the real configuration
data that follows.  Each section name has a type associated with it, which
is the name of the handler for parsing settings in that section.

We load configuration files in the following order:

	GLOBAL/machine.default
	GLOBAL/machine.config
	USER/machine.default
	USER/machine.config
	application.config

The "machine.default" files are unique to this implementation.  They provide
the bulk of the schema definition data and fallback defaults for configuration
settings.  The default files are expected to remain static across all
installations of the software, whereas "machine.config" files might change
from machine to machine.

*/

public sealed class ConfigurationSettings
{
	// Internal state.
	private static BuiltinConfigurationSystem configSystem;
	private static Exception configError;

	// Constructor - cannot be created by external entities.
	private ConfigurationSettings() {}

	// Get a configuration object for a specific section.
	public static Object GetConfig(String sectionName)
			{
				return GetConfig(sectionName, null);
			}

	// Get a configuration object, using a particular handler.
	// This is for internal use, to provide a fallback handler
	// if the section is not mentioned in "machine.default".
	internal static Object GetConfig
				(String sectionName, IConfigurationSectionHandler handler)
			{
				// Make sure that the configuration system is initialized.
				BuiltinConfigurationSystem system;
				lock(typeof(ConfigurationSettings))
				{
					if(configSystem != null)
					{
						system = configSystem;
					}
					else if(configError != null)
					{
						throw configError;
					}
					else
					{
						configSystem = new BuiltinConfigurationSystem();
						try
						{
							configSystem.Init();
						}
						catch(Exception e)
						{
							configError = e;
							throw;
						}
						system = configSystem;
					}
				}

				// Look up the specified configuration item.
				return system.GetConfig(sectionName, handler);
			}

	// Get the application settings.
	public static NameValueCollection AppSettings
			{
				get
				{
					ReadOnlyNameValueCollection settings;
					settings = (ReadOnlyNameValueCollection)
							(GetConfig("appSettings",
									   new NameValueFileSectionHandler()));
					if(settings == null)
					{
						settings = new ReadOnlyNameValueCollection();
						settings.MakeReadOnly();
					}
					return settings;
				}
			}

#if CONFIG_FRAMEWORK_1_2

	// Get the connection string settings.
	public static NameValueCollection ConnectionStrings
			{
				get
				{
					ReadOnlyNameValueCollection settings;
					settings = (ReadOnlyNameValueCollection)
							(GetConfig("connectionStrings",
									   new NameValueFileSectionHandler()));
					if(settings == null)
					{
						settings = new ReadOnlyNameValueCollection();
						settings.MakeReadOnly();
					}
					return settings;
				}
			}

#endif // CONFIG_FRAMEWORK_1_2

	// The builtin configuration system handler.
	private class BuiltinConfigurationSystem : IConfigurationSystem
	{
		// Internal state.
		private bool initialized;
	#if SECOND_PASS
		private XmlDocument[] documents;
		private int numDocuments;
		private Hashtable sectionSchema;
		private Hashtable cachedInfo;
	#endif

		// Constructor.
		public BuiltinConfigurationSystem()
				{
					initialized = false;
				#if SECOND_PASS
					documents = new XmlDocument [8];
					numDocuments = 0;
					sectionSchema = new Hashtable(128);
					cachedInfo = new Hashtable(128);
				#endif
				}

		// Check for a machine default file's existence in a directory.
		private static String CheckForMachineDefault(String dir)
				{
					// Bail out if the directory was not specified.
					if(dir == null || dir.Length == 0)
					{
						return null;
					}

					// Build the full pathname and check for its existence.
					String pathname = Path.Combine(dir, "machine.default");
					if(!File.Exists(pathname))
					{
						return null;
					}
					return pathname;
				}

		// Check for a machine configuration file's existence in a directory.
		private static String CheckForMachineConfig(String dir)
				{
					// Bail out if the directory was not specified.
					if(dir == null || dir.Length == 0)
					{
						return null;
					}

					// Build the full pathname and check for its existence.
					String pathname = Path.Combine(dir, "machine.config");
					if(!File.Exists(pathname))
					{
						return null;
					}
					return pathname;
				}

	#if SECOND_PASS

		// Object that is used to mark a group in the "sectionSchema" table.
		private static readonly Object groupMarker = new Object();

		// Get an attribute value from an XML node.
		private static String GetAttribute(XmlNode node, String name)
				{
					XmlAttribute attr = (XmlAttribute)(node.Attributes[name]);
					if(attr != null && attr.Value.Length > 0)
					{
						return attr.Value;
					}
					else
					{
						return null;
					}
				}

		// Get a section name from an attribute value.
		private static String GetSectionName
					(XmlNode node, String parentName, String name)
				{
					XmlAttribute attr = (XmlAttribute)(node.Attributes[name]);
					if(attr != null && attr.Value.Length > 0)
					{
						if(parentName != null)
						{
							return parentName + "/" + attr.Value;
						}
						else
						{
							return attr.Value;
						}
					}
					else
					{
						return null;
					}
				}

		// Load section information from "configSections" or "sectionGroup".
		private void LoadSectionInfo(XmlNode parent, String parentName)
				{
					String name, type;
					foreach(XmlNode node in parent.ChildNodes)
					{
						if(node.NodeType != XmlNodeType.Element)
						{
							continue;
						}
						switch(node.Name)
						{
							case "sectionGroup":
							{
								// Define a group of sections.
								name = GetSectionName(node, parentName, "name");
								sectionSchema[name] = groupMarker;
								LoadSectionInfo(node, name);
							}
							break;

							case "section":
							{
								// Define a single section schema.
								name = GetSectionName(node, parentName, "name");
								type = GetAttribute(node, "type");
								if(name != null && type != null)
								{
									sectionSchema[name] = type;
								}
							}
							break;

							case "remove":
							{
								// Remove a section from the schema.
								name = GetSectionName(node, parentName, "name");
								if(name != null)
								{
									sectionSchema.Remove(name);
								}
							}
							break;

							case "clear":
							{
								// Clear all schema definitions.
								sectionSchema.Clear();
							}
							break;
						}
					}
				}

		// Find the XML node corresponding to a section name.
		private static XmlNode FindSectionByName(XmlNode parent, String name)
				{
					int index = name.IndexOf('/');
					if(index != -1)
					{
						parent = FindSectionByName
							(parent, name.Substring(0, index));
						name = name.Substring(index + 1);
					}
					if(parent != null)
					{
						foreach(XmlNode node in parent.ChildNodes)
						{
							if(node.NodeType == XmlNodeType.Element &&
							   node.Name == name)
							{
								return node;
							}
						}
					}
					return null;
				}

	#endif // SECOND_PASS

		// Load a configuration file and merge it with the current settings.
		private void Load(String filename)
				{
				#if SECOND_PASS
					// Load the contents of the file as XML.
					ConfigXmlDocument doc = new ConfigXmlDocument();
					doc.Load(filename);
					documents[numDocuments++] = doc;

					// Process the "configSections" element in the document.
					foreach(XmlNode node in doc.DocumentElement.ChildNodes)
					{
						if(node.NodeType == XmlNodeType.Element &&
						   node.Name == "configSections")
						{
							LoadSectionInfo(node, null);
							break;
						}
					}
				#endif
				}

		// Initialize the configuration system.
		public void Init()
				{
					// Bail out if already initialized.
					if(initialized)
					{
						return;
					}
					try
					{
						// Find the user/global machine defaults files.
						String userDefaultFile, globalDefaultFile;
						userDefaultFile = CheckForMachineDefault
							(InfoMethods.GetUserStorageDir());
						globalDefaultFile = CheckForMachineDefault
							(InfoMethods.GetGlobalConfigDir());

						// Find the user/global machine configuration files.
						String userConfigFile, globalConfigFile;
						userConfigFile = CheckForMachineConfig
							(InfoMethods.GetUserStorageDir());
						globalConfigFile = CheckForMachineConfig
							(InfoMethods.GetGlobalConfigDir());

						// Find the application's configuration file.
						String appConfigFile =
							AppDomain.CurrentDomain.SetupInformation
								.ConfigurationFile;

						// Load all of the configuration files that we found.
						if(globalDefaultFile != null)
						{
							Load(globalDefaultFile);
						}
						if(globalConfigFile != null)
						{
							Load(globalConfigFile);
						}
						if(userDefaultFile != null)
						{
							Load(userDefaultFile);
						}
						if(userConfigFile != null)
						{
							Load(userConfigFile);
						}
						if(appConfigFile != null && File.Exists(appConfigFile))
						{
							Load(appConfigFile);
						}
					}
					finally
					{
						// The system has been initialized.
						initialized = true;
					}
				}

		// Get the object for a specific configuration key.
		public Object GetConfig(String configKey)
				{
					return GetConfig(configKey, null);
				}

		// Get the object for a specific configuration key and handler.
		public Object GetConfig
					(String configKey, IConfigurationSectionHandler handler)
				{
				#if SECOND_PASS
					// Bail out if the configuration key is invalid.
					if(configKey == null || configKey.Length == 0)
					{
						return null;
					}

					// See if we have cached information from last time.
					if(cachedInfo.Contains(configKey))
					{
						return cachedInfo[configKey];
					}

					// Get the section handler, if necessary.
					if(handler == null)
					{
						Object schema = sectionSchema[configKey];
						if(schema == null)
						{
							// We don't know how to handle the section.
							cachedInfo[configKey] = null;
							return null;
						}
						else if(schema == groupMarker)
						{
							// This section is a group.
							cachedInfo[configKey] = null;
							return null;
						}
						else
						{
							// Create an instance of the specified handler.
							Type handlerType = Type.GetType((String)schema);
							if(handlerType == null)
							{
								cachedInfo[configKey] = null;
								return null;
							}
							handler = Activator.CreateInstance(handlerType)
										as IConfigurationSectionHandler;
							if(handler == null)
							{
								cachedInfo[configKey] = null;
								return null;
							}
						}
					}

					// Scan all documents, and collect up the data.
					Object data = null;
					int posn;
					XmlNode section;
					for(posn = 0; posn < numDocuments; ++posn)
					{
						section = FindSectionByName
							(documents[posn].DocumentElement, configKey);
						if(section != null)
						{
							data = handler.Create(data, null, section);
						}
					}

					// Cache the data for next time and then return it.
					cachedInfo[configKey] = data;
					return data;
				#else
					// Configuration data is not available, so bail out.
					return null;
				#endif
				}
				
				/* This is a stupid API simulated to satisfy System.Web 
				   implementation */
				internal static String GetMachineConfigPath()
				{
					return CheckForMachineDefault(InfoMethods.GetGlobalConfigDir());
				}

	}; // class BuiltinConfigurationSystem

	/* This is a stupid API simulated to satisfy System.Web implementation */
	internal static IConfigurationSystem ChangeConfigurationSystem(IConfigurationSystem config)
	{
		return configSystem;
	}


}; // class ConfigurationSettings

#endif // !ECMA_COMPAT

}; // namespace System.Configuration
