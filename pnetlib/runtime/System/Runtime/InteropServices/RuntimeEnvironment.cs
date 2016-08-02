/*
 * RuntimeEnvironment.cs - Implementation of the
 *			"System.Runtime.InteropServices.RuntimeEnvironment" class.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_COM_INTEROP

using System.Reflection;

public class RuntimeEnvironment
{

	// Constructor.
	public RuntimeEnvironment() {}

	// Get the system configuration file.
	public static String SystemConfigurationFile
			{
				get
				{
					// We don't use system config files in this implementation.
					throw new NotImplementedException();
				}
			}

	// Determine if an assembly was loaded from the global access cache.
	public static bool FromGlobalAccessCache(Assembly assembly)
			{
				// This implementation doesn't use a GAC.
				return false;
			}

	// Get the directory where the runtime engine is installed.
	public static String GetRuntimeDirectory()
			{
				// We don't allow this to be inspected in this implementation.
				throw new NotImplementedException();
			}

	// Get the version of the runtime being used for the current process.
	public static String GetSystemVersion()
			{
				return Environment.Version.ToString();
			}

}; // class RuntimeEnvironment

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
