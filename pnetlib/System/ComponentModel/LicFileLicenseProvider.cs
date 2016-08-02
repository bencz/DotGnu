/*
 * LicFileLicenseProvider.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.LicFileLicenseProvider" class.
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

using System.IO;
using System.ComponentModel.Design;

public class LicFileLicenseProvider : LicenseProvider
{
	// Constructor.
	public LicFileLicenseProvider() {}

	// Get the license for a type.
	public override License GetLicense
				(LicenseContext context, Type type, object instance,
				 bool allowExceptions)
			{
				String key, path;
				StreamReader reader;

				// Bail out if we don't have a license context.
				if(context == null)
				{
					return null;
				}

				// Use the saved key if we saw this type previously.
				if(context.UsageMode == LicenseUsageMode.Runtime)
				{
					key = context.GetSavedLicenseKey(type, null);
					if(key != null && IsKeyValid(key, type))
					{
						return new FileLicense(key);
					}
				}

				// Find the pathname of the assembly containing the type.
			#if CONFIG_COMPONENT_MODEL_DESIGN
				ITypeResolutionService trs;
				trs = (ITypeResolutionService)
					context.GetService(typeof(ITypeResolutionService));
				if(trs != null)
				{
					path = trs.GetPathOfAssembly(type.Assembly.GetName());
					if(path == null)
					{
						path = type.Assembly.Location;
					}
				}
				else
			#endif
				{
					path = type.Assembly.Location;
				}

				// Look for a "*.lic" file for the type.
				path = Path.Combine(Path.GetDirectoryName(path),
									type.FullName + ".lic");
				try
				{
					reader = new StreamReader(path);
				}
				catch(Exception)
				{
					// Could not open the file, so assume unlicensed.
					return null;
				}

				// Read the key from the first line of the license file.
				key = reader.ReadLine();
				reader.Close();

				// Bail out if the key is invalid.
				if(key == null || !IsKeyValid(key, type))
				{
					return null;
				}

				// Cache the key within the context.
				context.SetSavedLicenseKey(type, key);

				// Return a new file license to the caller.
				return new FileLicense(key);
			}

	// Get the key for a type.
	protected virtual string GetKey(Type type)
			{
				// Don't translate this - needed for compatibility.
				return type.FullName + " is a licensed component.";
			}

	// Determine if a key is valid for a particular type.
	protected virtual bool IsKeyValid(String key, Type type)
			{
				if(key != null)
				{
					return key.StartsWith(GetKey(type));
				}
				else
				{
					return false;
				}
			}

	// License class for files.
	private sealed class FileLicense : License
	{
		// Internal state.
		private String key;

		// Constructor.
		public FileLicense(String key)
				{
					this.key = key;
				}

		// Get the license key.
		public override String LicenseKey
				{
					get
					{
						return key;
					}
				}

		// Dispose of this license.
		public override void Dispose()
				{
					// Nothing to do here.
				}

	}; // class FileLicense

}; // class LicFileLicenseProvider

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
