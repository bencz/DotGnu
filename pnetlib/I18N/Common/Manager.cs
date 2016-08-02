/*
 * Manager.cs - Implementation of the "I18N.Common.Manager" class.
 *
 * Copyright (c) 2002  Southern Storm Software, Pty Ltd
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

namespace I18N.Common
{

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Reflection;
using System.Security;

// This class provides the primary entry point into the I18N
// library.  Users of the library start by getting the value
// of the "PrimaryManager" property.  They then invoke methods
// on the manager to obtain further I18N information.

public class Manager
{
	// The primary I18N manager.
	private static Manager manager;

	// Internal state.
	private HandlerCollection handlers; // List of all handler classes.
	private Hashtable active;		// Currently active handlers.
	private Hashtable assemblies;	// Currently loaded region assemblies.

	// static constructor
	static Manager()
			{
				manager = new Manager();
			}

	// Constructor.
	private Manager()
			{
				handlers = new HandlerCollection();
				active = new Hashtable(16);
				assemblies = new Hashtable(8);
				LoadClassList();
			}

	// Get the primary I18N manager instance.
	public static Manager PrimaryManager
			{
				get
				{
					return manager;
				}
			}

	// Normalize a name.
	private static String Normalize(String name)
			{
			#if ECMA_COMPAT
				return (name.ToLower()).Replace('-', '_');
			#else
				return (name.ToLower(CultureInfo.InvariantCulture))
							.Replace('-', '_');
			#endif
			}

	// Get an encoding object for a specific code page.
	// Returns NULL if the code page is not available.
	public Encoding GetEncoding(int codePage)
			{
				return (Instantiate("CP" + codePage.ToString()) as Encoding);
			}

	// Get an encoding object for a specific Web encoding.
	// Returns NULL if the encoding is not available.
	public Encoding GetEncoding(String name)
			{
				// Validate the parameter.
				if(name == null)
				{
					return null;
				}

				// Normalize the encoding name.
				name = Normalize(name);

				// Try to find a class called "ENCname".
				return (Instantiate("ENC" + name) as Encoding);
			}

	// List of hex digits for use by "GetCulture".
	private const String hex = "0123456789abcdef";

	// Get a specific culture by identifier.  Returns NULL
	// if the culture information is not available.
	public CultureInfo GetCulture(int culture, bool useUserOverride)
			{
				// Create the hex version of the culture identifier.
				StringBuilder builder = new StringBuilder();
				builder.Append(hex[(culture >> 12) & 0x0F]);
				builder.Append(hex[(culture >> 8) & 0x0F]);
				builder.Append(hex[(culture >> 4) & 0x0F]);
				builder.Append(hex[culture & 0x0F]);
				String name = builder.ToString();

				// Try looking for an override culture handler.
				if(useUserOverride)
				{
					Object obj = Instantiate("CIDO" + name);
					if(obj != null)
					{
						return (obj as CultureInfo);
					}
				}

				// Look for the generic non-override culture.
				return (Instantiate("CID" + name) as CultureInfo);
			}

	// Get a specific culture by name.  Returns NULL if the
	// culture informaion is not available.
	public CultureInfo GetCulture(String name, bool useUserOverride)
			{
				// Validate the parameter.
				if(name == null)
				{
					return null;
				}

				// Normalize the culture name.
				name = Normalize(name);

				// Try looking for an override culture handler.
				if(useUserOverride)
				{
					Object obj = Instantiate("CNO" + name.ToString());
					if(obj != null)
					{
						return (obj as CultureInfo);
					}
				}

				// Look for the generic non-override culture.
				return (Instantiate("CN" + name.ToString()) as CultureInfo);
			}

	// Determine if a particular culture exists.
	public bool HasCulture(int culture)
			{
				// Create the hex version of the culture identifier.
				StringBuilder builder = new StringBuilder();
				builder.Append("CID");
				builder.Append(hex[(culture >> 12) & 0x0F]);
				builder.Append(hex[(culture >> 8) & 0x0F]);
				builder.Append(hex[(culture >> 4) & 0x0F]);
				builder.Append(hex[culture & 0x0F]);
				String name = builder.ToString();

				// Determine if a handler exists for this culture.
				return (handlers.IndexOf(name) != -1);
			}
	public bool HasCulture(String name)
			{
				// Validate the parameter.
				if(name == null)
				{
					return false;
				}

				// Normalize the culture name.
				name = Normalize(name);

				// Determine if a handler exists for this culture.
				return (handlers.IndexOf(name) != -1);
			}

	// Resolve a culture name using a string culture.
	private static String GetCultureName(RootCulture info, RootCulture str)
			{
				String name = str.ResolveLanguage
					(info.TwoLetterISOLanguageName);
				String country = info.Country;
				if(country != null)
				{
					name = name + " (" + str.ResolveCountry(country) + ")";
				}
				return name;
			}

	// Get the culture handler for the current culture.
	public static RootCulture GetCurrentCulture()
			{
				// Get the culture handler for the current culture.
				int cultureID = CultureInfo.CurrentCulture.LCID;
				StringBuilder builder = new StringBuilder();
				builder.Append(hex[(cultureID >> 12) & 0x0F]);
				builder.Append(hex[(cultureID >> 8) & 0x0F]);
				builder.Append(hex[(cultureID >> 4) & 0x0F]);
				builder.Append(hex[cultureID & 0x0F]);
				String name = builder.ToString();
				RootCulture culture =
					(PrimaryManager.Instantiate("CID" + name) as RootCulture);

				// Use invariant English if we couldn't find the culture.
				if(culture == null)
				{
					culture = new CNen();
				}
				return culture;
			}

	// Get the display name for a particular culture.
	public static String GetDisplayName(RootCulture info)
			{
				return GetCultureName(info, GetCurrentCulture());
			}

	// Get the English name for a particular culture.
	public static String GetEnglishName(RootCulture info)
			{
				return GetCultureName(info, new CNen());
			}

	// Get the native name for a particular culture.
	public static String GetNativeName(RootCulture info)
			{
				return GetCultureName(info, info);
			}

	// Convert a culture identifier from hex.
	private static int FromHex(String name, int index)
			{
				int num = 0;
				char ch;
				while(index < name.Length)
				{
					ch = name[index++];
					if(ch >= '0' && ch <= '9')
					{
						num = num * 16 + (int)(ch - '0');
					}
					else if(ch >= 'A' && ch <= 'F')
					{
						num = num * 16 + (int)(ch - 'A' + 10);
					}
					else if(ch >= 'a' && ch <= 'f')
					{
						num = num * 16 + (int)(ch - 'a' + 10);
					}
				}
				return num;
			}

#if !ECMA_COMPAT

	// Determine if a handler name matches the "GetCultures" requirements.
	private static bool CultureMatch(String name, CultureTypes types)
			{
				if(name.Length > 3 &&
				   name[0] == 'C' && name[1] == 'I' &&
				   name[2] == 'D' && name[3] != 'O')
				{
					int num = FromHex(name, 3);
					if((num & 0x03FF) == 0)
					{
						// This is a neutral culture.
						if((types & CultureTypes.NeutralCultures) != 0)
						{
							return true;
						}
					}
					else
					{
						// This is a specific culture.
						if((types & CultureTypes.SpecificCultures) != 0)
						{
							return true;
						}
					}
				}
				return false;
			}

	// Get an array of all cultures in the system.
	public CultureInfo[] GetCultures(CultureTypes types)
			{
				// Count the number of culture handlers in the system.
				int count = 0;
				if((types & CultureTypes.NeutralCultures) != 0)
				{
					++count;
				}
				IDictionaryEnumerator e = handlers.GetEnumerator();
				String name;
				while(e.MoveNext())
				{
					name = (String)(e.Key);
					if(CultureMatch(name, types))
					{
						++count;
					}
				}

				// Build the list of cultures.
				CultureInfo[] cultures = new CultureInfo [count];
				count = 0;
				if((types & CultureTypes.NeutralCultures) != 0)
				{
					cultures[count++] = CultureInfo.InvariantCulture;
				}
				e.Reset();
				while(e.MoveNext())
				{
					name = (String)(e.Key);
					if(CultureMatch(name, types))
					{
						cultures[count++] =
							new CultureInfo(FromHex(name, 3));
					}
				}

				// Return the culture list to the caller.
				return cultures;
			}

	// Get region information for an identifier or name.
	public RegionInfo GetRegion(int culture)
			{
				RegionName regionName;
				regionName = RegionNameTable.GetNameInfoByID(culture);
				if(regionName != null)
				{
					return new RegionData(regionName);
				}
				else
				{
					return null;
				}
			}
	public RegionInfo GetRegion(String name)
			{
				RegionName regionName;
				regionName = RegionNameTable.GetNameInfoByName(name);
				if(regionName != null)
				{
					return new RegionData(regionName);
				}
				else
				{
					return null;
				}
			}

#endif // !ECMA_COMPAT

	// Instantiate a handler class.  Returns null if it is not
	// possible to instantiate the class.
	internal Object Instantiate(String name)
			{
			#if CONFIG_REFLECTION
				Object handler;
				Object handlerRenter;
				String region;
				Assembly assembly;
				Type type;

				lock(this)
				{
					// See if we already have an active handler by this name.
					handler = active[name];
					if(handler != null)
					{
						return handler;
					}

					// Determine which region assembly handles the class.
					region = (String)(handlers[name]);
					if(region == null)
					{
						// The class does not exist in any region assembly.
						return null;
					}

					// Find the region-specific assembly and load it.
					assembly = (Assembly)(assemblies[region]);
					if(assembly == null)
					{
						try
						{
							if(region == "I18N.Common")
							{
								assembly = Assembly.GetExecutingAssembly();
							}
							else
							{
								assembly = Assembly.Load(region);
							}
						}
						catch(SystemException)
						{
							assembly = null;
						}
						if(assembly == null)
						{
							return null;
						}
						assemblies[region] = assembly;
					}

					// Look for the class within the region-specific assembly.
					type = assembly.GetType(region + "." + name);
					if(type == null)
					{
						return null;
					}

					// Invoke the constructor, which we assume is public
					// and has zero arguments.
					try
					{
						handler = type.InvokeMember
								(String.Empty,
								 BindingFlags.CreateInstance |
								 	BindingFlags.Public |
								 	BindingFlags.NonPublic |
								 	BindingFlags.Instance,
								 null, null, null, null, null, null);
					}
					catch(MissingMethodException)
					{
						// The constructor was not present.
						return null;
					}
					catch(SecurityException)
					{
						// The constructor was inaccessible.
						return null;
					}

					// Check the "active" list again, because we may
					// have been recursively re-entered and already
					// created the handler on the recursive call.
					handlerRenter = active[name];
					if(handlerRenter != null)
					{
						return handlerRenter;
					}

					// Add the handler to the active handlers cache.
					active.Add(name, handler);

					// Return the handler to the caller.
					return handler;
				}
			#else // !CONFIG_REFLECTION
				return null;
			#endif // !CONFIG_REFLECTION
			}

	// Load the list of classes that are present in all region assemblies.
	private void LoadClassList()
			{
				Stream stream;

				// Look for "I18N-handlers.def" in the manifest resources.
				try
				{
					stream = Assembly.GetExecutingAssembly()
						.GetManifestResourceStream("I18N-handlers.def");
					if(stream == null)
					{
						return;
					}
				}
				catch(FileNotFoundException)
				{
					// The file does not exist.
					return;
				}

				// Load the class list from the stream.
				StreamReader reader = new StreamReader(stream);
				String line;
				int posn;
				while((line = reader.ReadLine()) != null)
				{
					// Skip comment lines in the input.
					if(line.Length == 0 || line[0] == '#')
					{
						continue;
					}

					// Split the line into namespace and name.  We assume
					// that the line has the form "I18N.<Region>.<Name>".
					posn = line.LastIndexOf('.');
					if(posn != -1)
					{
						// Add the namespace to the "handlers" hash,
						// attached to the name of the handler class.
						String name = line.Substring(posn + 1);
						handlers.Add(name, line.Substring(0, posn));
					}
				}
				reader.Close();
			}

}; // class Manager

}; // namespace I18N.Common
