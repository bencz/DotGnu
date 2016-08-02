/*
 * S.cs - Implementation of string resource handling for "System.Xml".
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Xml
{

using System.Reflection;
using System.Resources;

// This class provides string resource support to the rest
// of the System.Xml library assembly.  It is accessed using
// the "S._(tag)" convention.

internal sealed class S
{

#if CONFIG_RUNTIME_INFRA

	// Cached copy of the resources for this assembly and mscorlib.
#if ECMA_COMPAT
	private static ECMAResourceManager xmlResources = null;
	private static ECMAResourceManager runtimeResources = null;
#else
	private static ResourceManager xmlResources = null;
	private static ResourceManager runtimeResources = null;
#endif

	// Helper for obtaining string resources for this assembly.
	public static String _(String tag)
			{
				lock(typeof(S))
				{
					String value;

					// Try the resources in the "System.Xml" assembly first.
					if(xmlResources == null)
					{
					#if ECMA_COMPAT
						xmlResources = new ECMAResourceManager
							("System.Xml", (typeof(S)).Assembly);
					#else
						xmlResources = new ResourceManager
							("System.Xml", (typeof(S)).Assembly);
					#endif
					}
					value = xmlResources.GetString(tag, null);
					if(value != null)
					{
						return value;
					}

					// Try the fallbacks in the runtime library.
					if(runtimeResources == null)
					{
					#if ECMA_COMPAT
						runtimeResources = new ECMAResourceManager
							("runtime", (typeof(String)).Assembly);
					#else
						runtimeResources = new ResourceManager
							("runtime", (typeof(String)).Assembly);
					#endif
					}
					return runtimeResources.GetString(tag, null);
				}
			}

#else // !CONFIG_RUNTIME_INFRA

	// We don't have sufficient runtime infrastructure to load resources.
	public static String _(String tag)
			{
				return tag;
			}

#endif // !CONFIG_RUNTIME_INFRA

}; // class S

}; // namespace System.Xml
