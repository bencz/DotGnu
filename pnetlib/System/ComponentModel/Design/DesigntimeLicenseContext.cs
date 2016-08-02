/*
 * DesigntimeLicenseContext.cs - Implementation of the
 *		"System.ComponentModel.Design.DesigntimeLicenseContext" class.
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

namespace System.ComponentModel.Design
{

#if CONFIG_COMPONENT_MODEL_DESIGN

using System.Reflection;
using System.Collections;

public class DesigntimeLicenseContext : LicenseContext
{
	// Internal state.
	internal Hashtable keys;

	// Constructor.
	public DesigntimeLicenseContext()
			{
				keys = new Hashtable();
			}

	// Get the license usage mode.
	public override LicenseUsageMode UsageMode
			{
				get
				{
					return LicenseUsageMode.Designtime;
				}
			}

	// Get the saved form of a license key.
	public override String GetSavedLicenseKey
				(Type type, Assembly resourceAssembly)
			{
				return (String)(keys[type.AssemblyQualifiedName]);
			}

	// Set the license key for a specific type.
	public override void SetSavedLicenseKey(Type type, String key)
			{
				keys[type.AssemblyQualifiedName] = key;
			}

}; // class DesigntimeLicenseContext

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design
