/*
 * ECMAResourceManager.cs - Implementation of the
 *		"System.Resources.ECMAResourceManager" class.
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

namespace System.Resources
{

#if ECMA_COMPAT && CONFIG_RUNTIME_INFRA

using System;
using System.Globalization;
using System.Reflection;

// This class is intended to be used by other library assemblies
// in ECMA_COMPAT mode, when ResourceManager is not available.
// Use of this class in non-pnetlib code is not recommended.

[NonStandardExtra]
public sealed class ECMAResourceManager
{
	// Internal state.
	private ResourceManager manager;

	// Constructor.
	public ECMAResourceManager(String baseName, Assembly assembly)
			{
				manager = new ResourceManager(baseName, assembly);
			}

	// Get a string from this resource manager.
	public String GetString(String name)
			{
				return manager.GetString(name, null);
			}
	public String GetString(String name, CultureInfo culture)
			{
				return manager.GetString(name, culture);
			}

}; // class ECMAResourceManager

#endif // ECMA_COMPAT

}; // namespace System.Resources
