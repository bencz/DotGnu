/*
 * LicenseContext.cs - Implementation of the
 *		"System.ComponentModel.LicenseContext" class.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL

using System;
using System.Reflection;

public class LicenseContext : IServiceProvider
{
	// Constructor.
	public LicenseContext() {}

	// Get the license usage mode.
	public virtual LicenseUsageMode UsageMode
			{
				get
				{
					return LicenseUsageMode.Runtime;
				}
			}

	// Get the saved form of a license key.
	public virtual String GetSavedLicenseKey
				(Type type, Assembly resourceAssembly)
			{
				// Nothing to do here in the base class.
				return null;
			}

	// Implement the IServiceProvider interface.
	public virtual Object GetService(Type type)
			{
				// Nothing to do here in the base class.
				return null;
			}

	// Set the license key for a specific type.
	public virtual void SetSavedLicenseKey(Type type, String key)
			{
				// Nothing to do here in the base class.
			}

}; // class LicenseContext

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
