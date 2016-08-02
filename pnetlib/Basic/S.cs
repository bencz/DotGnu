/*
 * S.cs - Implementation of string resource handling.
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

namespace Microsoft.VisualBasic
{
using System;
using System.Reflection;
using System.Resources;

// This class provides string resource support to the rest
// of the Microsoft.VisualBasic library assembly.  It is accessed using
// the "S._(tag)" convention.

internal sealed class S
{

#if CONFIG_RUNTIME_INFRA

	// Cached copy of the resources for this assembly and mscorlib.
	// We avoid loading the mscorlib resources in non-ECMA mode,
	// so that the assembly can be used with other CLR's.
#if ECMA_COMPAT
	private static ECMAResourceManager ourResources = null;
	private static ECMAResourceManager runtimeResources = null;
#else
	private static ResourceManager ourResources = null;
#endif

	// Helper for obtaining string resources for this assembly.
	public static String _(String tag)
			{
				lock(typeof(S))
				{
					String value;

					// Try the resources in this assembly first.
					if(ourResources == null)
					{
					#if ECMA_COMPAT
						ourResources = new ECMAResourceManager
							("Microsoft.VisualBasic", (typeof(S)).Assembly);
					#else
						ourResources = new ResourceManager
							("Microsoft.VisualBasic", (typeof(S)).Assembly);
					#endif
					}
					value = ourResources.GetString(tag, null);
					if(value != null)
					{
						return value;
					}

				#if ECMA_COMPAT
					// Try the fallbacks in the runtime library.
					if(runtimeResources == null)
					{
						runtimeResources = new ECMAResourceManager
							("runtime", (typeof(String)).Assembly);
					}
					return runtimeResources.GetString(tag, null);
				#else
					return tag;
				#endif
				}
			}

#else // !CONFIG_RUNTIME_INFRA

	// We don't have sufficient runtime infrastructure to load resources.
	public static String _(String tag)
			{
				return tag;
			}

#endif // !CONFIG_RUNTIME_INFRA

	// Fetch a string with a default value.
	public static String _(String tag, String defaultValue)
			{
				String value = _(tag);
				if(value == null || value == tag)
				{
					return defaultValue;
				}
				else
				{
					return value;
				}
			}

}; // class S

}; // namespace Microsoft.VisualBasic
