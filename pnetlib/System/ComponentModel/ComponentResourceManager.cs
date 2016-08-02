/*
 * ComponentResourceManager.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.ComponentResourceManager" class.
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

using System.Resources;
using System.Collections;
using System.Globalization;

public class ComponentResourceManager : ResourceManager
{
	// Constructors.
	public ComponentResourceManager() {}
	public ComponentResourceManager(Type t) : base(t) {}

	// Apply resources to a property value.
	public void ApplyResources(Object value, String objectName)
			{
				ApplyResources(value, objectName, null);
			}
	public virtual void ApplyResources
				(Object value, String objectName, CultureInfo culture)
			{
				// Validate the parameters.
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(objectName == null)
				{
					throw new ArgumentNullException("objectName");
				}

				// Get the default UI culture if necessary.
				if(culture == null)
				{
					culture = CultureInfo.CurrentUICulture;
				}

				// Read the resources for the specified culture.
				ResourceSet set = GetResourceSet(culture, true, true);
				if(set == null)
				{
					return;
				}

				// Get the properties for the object.
				PropertyDescriptorCollection props;
				props = TypeDescriptor.GetProperties(value);

				// Set the resources for the value.
				IDictionaryEnumerator e = set.GetEnumerator();
				while(e.MoveNext())
				{
					// Check that this key is appropriate to the object.
					String key = (String)(e.Key);
					if(objectName == null || key.StartsWith(objectName))
					{
						// Remove the object name prefix from the key.
						if(objectName != null)
						{
							key = key.Substring(objectName.Length);
						}

						// Find the specified property and set it.
						PropertyDescriptor prop;
						prop = props[key];
						if(prop != null && !prop.IsReadOnly)
						{
							prop.SetValue(value, e.Value);
						}
					}
				}
			}

}; // class ComponentResourceManager

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
