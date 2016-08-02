/*
 * ManifestResourceInfo.cs - Implementation of the
 *		"System.Reflection.ManifestResourceInfo" class.
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

namespace System.Reflection
{

#if CONFIG_REFLECTION

using System;

#if ECMA_COMPAT
internal
#else
public
#endif
class ManifestResourceInfo
{
	// Internal state.
	private String fileName;
	private Assembly assembly;
	private ResourceLocation location;

	// Internal constructor used by the engine to build
	// an instance of this class.
	internal ManifestResourceInfo(String fileName,
								  Assembly assembly,
								  ResourceLocation location)
			{
				this.fileName = fileName;
				this.assembly = assembly;
				this.location = location;
			}

	// Get the filename associated with this resource.
	public virtual String FileName
			{
				get
				{
					return fileName;
				}
			}

	// Get the assembly that contains the resource.
	public virtual Assembly ReferencedAssembly
			{
				get
				{
					return assembly;
				}
			}

	// Get the location of the resource.
	public virtual ResourceLocation ResourceLocation
			{
				get
				{
					return location;
				}
			}

}; // class ManifestResourceInfo

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
