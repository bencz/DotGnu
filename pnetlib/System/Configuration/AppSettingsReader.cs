/*
 * AppSettingsReader.cs - Implementation of the
 *		"System.Configuration.AppSettingsReader" interface.
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

namespace System.Configuration
{

#if !ECMA_COMPAT

using System;
using System.Collections.Specialized;
using System.Globalization;

public class AppSettingsReader
{
	// Internal state.
	private NameValueCollection coll;

	// Constructor.
	public AppSettingsReader()
			{
				coll = ConfigurationSettings.AppSettings;
			}

	// Get a specific application setting value.
	public Object GetValue(String key, Type type)
			{
				if(key == null)
				{
					throw new ArgumentNullException("key");
				}
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}
				String value = coll[key];
				if(value != null)
				{
					if(String.Compare(value, "None", true,
									  CultureInfo.InvariantCulture) == 0)
					{
						return null;
					}
					try
					{
						return Convert.ChangeType(value, type);
					}
					catch(Exception)
					{
						throw new InvalidOperationException
							(S._("Config_CannotConvert"));
					}
				}
				else
				{
					throw new InvalidOperationException
						(S._("Config_KeyNotPresent"));
				}
			}

}; // class AppSettingsReader

#endif // !ECMA_COMPAT

}; // namespace System.Configuration
