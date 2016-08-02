/*
 * ResourceManager.cs - Implementation of the
 *		"System.Resources.ResourceManager" class.
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Resources
{

#if CONFIG_RUNTIME_INFRA

using System;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Text;

#if ECMA_COMPAT
internal
#else
public
#endif
class ResourceManager
{
	// Current version supported by this resource manager implementation.
	public static readonly int HeaderVersionNumber = 1;

	// Magic number used in binary resource file headers.
	public static readonly int MagicNumber = unchecked((int)0xBEEFCACE);

	// Internal state.
	protected String BaseNameField;
	protected Assembly MainAssembly;
	protected Hashtable ResourceSets;
	private Hashtable notPresent;
	private bool ignoreCase;
	private Type resourceSetType;
	private String resourceDir;
	private static bool reEnterCheck = false;

	// Constructors.
	protected ResourceManager()
			{
				BaseNameField = null;
				MainAssembly = null;
				ResourceSets = new Hashtable();
				notPresent = new Hashtable();
				ignoreCase = false;
				resourceSetType = null;
				resourceDir = null;
			}
	public ResourceManager(Type resourceSource)
			{
				if(resourceSource == null)
				{
					throw new ArgumentNullException("resourceSource");
				}
				BaseNameField = resourceSource.FullName;
				MainAssembly = resourceSource.Assembly;
				ResourceSets = new Hashtable();
				notPresent = new Hashtable();
				ignoreCase = false;
				resourceSetType = typeof(BuiltinResourceSet);
				resourceDir = null;
			}
	public ResourceManager(String baseName, Assembly assembly)
			{
				if(baseName == null)
				{
					throw new ArgumentNullException("baseName");
				}
				if(assembly == null)
				{
					throw new ArgumentNullException("assembly");
				}
				BaseNameField = baseName;
				MainAssembly = assembly;
				ResourceSets = new Hashtable();
				notPresent = new Hashtable();
				ignoreCase = false;
				resourceSetType = typeof(BuiltinResourceSet);
				resourceDir = null;
			}
	public ResourceManager(String baseName, Assembly assembly,
						   Type usingResourceSet)
			{
				if(baseName == null)
				{
					throw new ArgumentNullException("baseName");
				}
				if(assembly == null)
				{
					throw new ArgumentNullException("assembly");
				}
				BaseNameField = baseName;
				MainAssembly = assembly;
				ResourceSets = new Hashtable();
				notPresent = new Hashtable();
				ignoreCase = false;
				resourceDir = null;
				if(usingResourceSet != null)
				{
					if(!usingResourceSet.IsSubclassOf(typeof(ResourceSet)))
					{
						throw new ArgumentException
							(_("Arg_MustBeResourceSet"),
							 "usingResourceSet");
					}
					resourceSetType = usingResourceSet;
				}
				else
				{
					resourceSetType = typeof(BuiltinResourceSet);
				}
			}
	private ResourceManager(String baseName, String resourceDir,
							Type usingResourceSet)
			{
				BaseNameField = baseName;
				MainAssembly = null;
				ResourceSets = new Hashtable();
				notPresent = new Hashtable();
				ignoreCase = false;
				resourceSetType = usingResourceSet;
				this.resourceDir = resourceDir;
			}

	// Create a resource manager from a file.
	public static ResourceManager CreateFileBasedResourceManager
				(String baseName, String resourceDir, Type usingResourceSet)
			{
				if(baseName == null)
				{
					throw new ArgumentNullException("baseName");
				}
				else if(baseName.EndsWith(".resources"))
				{
					throw new ArgumentException
						(_("Arg_EndsWithResources"), "baseName");
				}
				else if(resourceDir == null)
				{
					throw new ArgumentNullException("baseName");
				}
				if(usingResourceSet != null)
				{
					if(!usingResourceSet.IsSubclassOf(typeof(ResourceSet)))
					{
						throw new ArgumentException
							(_("Arg_MustBeResourceSet"),
							 "usingResourceSet");
					}
				}
				else
				{
					usingResourceSet = typeof(BuiltinResourceSet);
				}
				return new ResourceManager(baseName, resourceDir,
										   usingResourceSet);
			}

	// Get the base name for this resource manager.
	public virtual String BaseName
			{
				get
				{
					return BaseNameField;
				}
			}

	// Get or set the "ignore case" property when searching for resources.
	public virtual bool IgnoreCase
			{
				get
				{
					return ignoreCase;
				}
				set
				{
					ignoreCase = value;
				}
			}

	// Get the resource set type that underlies this resource manager.
	public virtual Type ResourceSetType
			{
				get
				{
					return resourceSetType;
				}
			}

	// Get an object from this resource manager.
	public virtual Object GetObject(String name)
			{
				return GetObject(name, null);
			}
	public virtual Object GetObject(String name, CultureInfo culture)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(culture == null)
				{
					culture = CultureInfo.CurrentUICulture;
				}
				lock(this)
				{
					do 
					{
						ResourceSet set = InternalGetResourceSet
											(culture, true, true);
						if(set != null)
						{
							Object ret = set.GetObject(name);
							if(ret != null)
							{
								return ret;
							}
						}
						culture = culture.Parent;
					} while(culture != null);
				}
				return null;
			}

	// Get the resource set for a particular culture.
	public virtual ResourceSet GetResourceSet
				(CultureInfo culture, bool createIfNotExists,
				 bool tryParents)
			{
				if(culture == null)
				{
					throw new ArgumentNullException("culture");
				}
				lock(this)
				{
					return InternalGetResourceSet
						(culture, createIfNotExists, tryParents);
				}
			}

	// Get a string from this resource manager.
	public virtual String GetString(String name)
			{
				return GetString(name, null);
			}
	public virtual String GetString(String name, CultureInfo culture)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(culture == null)
				{
					culture = CultureInfo.CurrentUICulture;
				}
				lock(this)
				{
					// Scan up through the parent cultures until we
					// find a string that matches the tag.  We do this
					// so that we can pick up the neutral fallbacks if
					// language-specific versions are not available.
					do
					{
						ResourceSet set = InternalGetResourceSet
							(culture, true, true);
						if(set != null)
						{
							String ret = set.GetString(name);
							if(ret != null)
							{
								return ret;
							}
						}
						if(culture.Equals(CultureInfo.InvariantCulture))
						{
							break;
						}
						culture = culture.Parent;
					}
					while(culture != null);
				}
				return name;
			}

	// Release all cached resources.
	public virtual void ReleaseAllResources()
			{
				lock(this)
				{
					IDictionaryEnumerator e = ResourceSets.GetEnumerator();
					while(e.MoveNext())
					{
						((ResourceSet)(e.Value)).Dispose();
					}
					ResourceSets = new Hashtable();
					notPresent = new Hashtable();
				}
			}

#if !ECMA_COMPAT

	// Flag that protects "GetNeutralResourcesLanguage" from re-entry.
	private static bool gettingNeutralLanguage;

#endif

	// Get the neutral culture to use, based on an assembly's attributes.
	protected static CultureInfo GetNeutralResourcesLanguage(Assembly a)
			{
			#if !ECMA_COMPAT
				lock(typeof(ResourceManager))
				{
					if(gettingNeutralLanguage)
					{
						// We were recursively re-entered, so bail out.
						return CultureInfo.InvariantCulture;
					}
					gettingNeutralLanguage = true;
					Object[] attrs = a.GetCustomAttributes
						(typeof(NeutralResourcesLanguageAttribute), false);
					if(attrs != null && attrs.Length > 0)
					{
						String culture;
						culture = ((NeutralResourcesLanguageAttribute)
							(attrs[0])).CultureName;
						if(culture != null)
						{
							// Make sure that the culture name exists.
							Object value = Encoding.InvokeI18N
								("HasCulture", culture);
							if(value != null && ((bool)value))
							{
								gettingNeutralLanguage = false;
								return new CultureInfo(culture);
							}
						}
					}
					gettingNeutralLanguage = false;
				}
			#endif
				return CultureInfo.InvariantCulture;
			}

	// Get the satellite contract version from an assembly.
	protected static Version GetSatelliteContractVersion(Assembly a)
			{
			#if !ECMA_COMPAT
				// Check for recursive re-entry and bail out if necessary.
				lock(typeof(ResourceManager))
				{
					if(reEnterCheck)
					{
						return null;
					}
					reEnterCheck = true;
				}
				try
				{
					Object[] attrs = a.GetCustomAttributes
						(typeof(SatelliteContractVersionAttribute), false);
					if(attrs != null && attrs.Length > 0)
					{
						String version;
						version =
							((SatelliteContractVersionAttribute)(attrs[0]))
										.Version;
						if(version != null)
						{
							try
							{
								return new Version(version);
							}
							catch(Exception)
							{
								// Ignore version parsing errors.
							}
						}
					}
				}
				finally
				{
					lock(typeof(ResourceManager))
					{
						reEnterCheck = false;
					}
				}
			#endif
				return null;
			}

	// Get the name of a resource file for a particular culture.
	protected virtual String GetResourceFileName(CultureInfo culture)
			{
			#if CONFIG_REFLECTION
				if(culture == null || culture.LCID == 0x7F)
				{
					// This is the invariant culture.
					return BaseNameField + ".resources";
				}
				else
				{
					// This is a named culture.
					return BaseNameField + "." + culture.Name + ".resources";
				}
			#else
				return BaseNameField + ".resources";
			#endif
			}

	// Find a resource set for a particular culture.
	protected virtual ResourceSet InternalGetResourceSet
				(CultureInfo culture, bool createIfNotExists,
				 bool tryParents)
			{
				CultureInfo current = culture;
				ResourceSet set;
				do
				{
					// If this is the invariant culture, then stop.
					if(current.Equals(CultureInfo.InvariantCulture))
					{
						break;
					}

					// If "notPresent" is set, then we know that we looked
					// for the culture previously and didn't find it.
					if(notPresent[current.Name] != null)
					{
						current = current.Parent;
						continue;
					}

					// See if we have a cached resource set for this culture.
					set = (ResourceSet)(ResourceSets[current.Name]);
					if(set != null)
					{
						return set;
					}

					// Attempt to load a resource for this culture.
					if(createIfNotExists)
					{
						set = AttemptLoad(current);
						if(set != null)
						{
							ResourceSets[current.Name] = set;
							return set;
						}
						notPresent[current.Name] = (Object)true;
					}

					// Move up to the parent culture.
					current = current.Parent;
				}
				while(current != null && tryParents);

				// Try looking for the neutral language attribute.
				if(MainAssembly != null)
				{
					current = GetNeutralResourcesLanguage(MainAssembly);
				}
				else
				{
					current = null;
				}
				if(current != null)
				{
					// See if we have a cached resource set for this culture.
					set = (ResourceSet)(ResourceSets[current.Name]);
					if(set != null)
					{
						return set;
					}

					// Attempt to load a resource for this culture.
					if(notPresent[current.Name] == null && createIfNotExists)
					{
						set = AttemptLoad(current);
						if(set != null)
						{
							ResourceSets[current.Name] = set;
							return set;
						}
						notPresent[current.Name] = (Object)true;
					}
				}

				// Last ditch attempt: try the invariant culture.
				current = CultureInfo.InvariantCulture;
				set = (ResourceSet)(ResourceSets[current.Name]);
				if(set != null)
				{
					return set;
				}
				if(notPresent[current.Name] == null && createIfNotExists)
				{
					set = AttemptLoad(current);
					if(set != null)
					{
						ResourceSets[current.Name] = set;
						return set;
					}
					notPresent[current.Name] = (Object)true;
				}

				// We could not find an appropriate resource set.
				return null;
			}

	// Create a resource set using a type.
	private static ResourceSet AttemptCreate(Type resourceSetType,
											 Stream stream)
			{
				if(resourceSetType == typeof(BuiltinResourceSet))
				{
					// Special case the builtin resources, because we
					// may not have sufficient reflection support to
					// create the resource set via "CreateInstance".
					return new BuiltinResourceSet(stream);
				}
			#if CONFIG_REFLECTION
				else
				{
					try
					{
						Object[] args = new Object [1];
						args[0] = stream;
						return (ResourceSet)
							(Activator.CreateInstance(resourceSetType, args));
					}
					catch(NotSupportedException)
					{
						// We don't have enough reflection support.
						return null;
					}
				}
			#else
				return null;
			#endif
			}

	// Attempt to load a resource set for a particular culture.
	private ResourceSet AttemptLoad(CultureInfo culture)
			{
				Stream stream;
				Assembly assembly;
				int error;

				if(MainAssembly != null)
				{
					// Try loading the resources from the main assembly.
					stream = MainAssembly.GetManifestResourceStream
							(GetResourceFileName(culture));
					if(stream != null)
					{
						return AttemptCreate(resourceSetType, stream);
					}

					// Try loading the resources from a satellite assembly.
					String assemblyName = MainAssembly.FullName;
					if(assemblyName != null)
					{
						int index = assemblyName.IndexOf(',');
						if(index != -1)
						{
							assemblyName = assemblyName.Substring(0, index);
						}
					}
					String path = MainAssembly.GetSatellitePath
						(culture.Name + Path.DirectorySeparatorChar +
						 assemblyName + ".resources.dll");
					if(path != null)
					{
						error = 1;
						assembly = Assembly.LoadFromFile
							(path, out error, MainAssembly);
						if(assembly != null && error == 0)
						{
							stream = assembly.GetManifestResourceStream
									(GetResourceFileName(culture));
							if(stream != null)
							{
								return AttemptCreate(resourceSetType, stream);
							}
						}
					}
				}
				else if(resourceDir != null)
				{
					// Try loading the resources from a directory.
					try
					{
						stream = new FileStream(resourceDir +
												Path.DirectorySeparatorChar +
												GetResourceFileName(culture),
												FileMode.Open,
												FileAccess.Read);
					}
					catch(IOException)
					{
						// The file or directory probably does not exist.
						return null;
					}
					return AttemptCreate(resourceSetType, stream);
				}
				return null;
			}

}; // class ResourceManager

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System.Resources
