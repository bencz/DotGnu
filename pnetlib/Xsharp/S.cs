/*
 * S.cs - Process string resources for the X# library.
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

namespace Xsharp
{

using System;
using System.Resources;
using System.Reflection;

internal sealed class S
{
#if CONFIG_RUNTIME_INFRA

	// Cached copy of the resources for this assembly and mscorlib.
#if ECMA_COMPAT
	private static ECMAResourceManager stringResources = null;
#else
	private static ResourceManager stringResources = null;
#endif

	// Helper for obtaining string resources for this assembly.
	public static String _(String tag)
			{
				lock(typeof(S))
				{
					if(stringResources == null)
					{
					#if ECMA_COMPAT
						stringResources = new ECMAResourceManager
							("Xsharp", (typeof(S)).Assembly);
					#else
						stringResources = new ResourceManager
							("Xsharp", (typeof(S)).Assembly);
					#endif
					}
					return stringResources.GetString(tag, null);
				}
			}

#else // !CONFIG_RUNTIME_INFRA

	// We don't have sufficient runtime infrastructure to load resources.
	public static String _(String tag)
			{
				return tag;
			}

#endif // !CONFIG_RUNTIME_INFRA

} // class S

} // namespace Xsharp
