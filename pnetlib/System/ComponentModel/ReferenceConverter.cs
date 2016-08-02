/*
 * ReferenceConverter.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.ReferenceConverter" class.
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
using System.Collections;
using System.Globalization;
using System.ComponentModel.Design;

public class ReferenceConverter : TypeConverter
{
	// Internal state.
	private Type type;

	// Constructor.
	public ReferenceConverter(Type type)
			{
				this.type = type;
			}

	// Determine if we can convert from a specific type to this one.
	public override bool CanConvertFrom
				(ITypeDescriptorContext context, Type sourceType)
			{
				if(sourceType == typeof(String))
				{
					return true;
				}
				else
				{
					return base.CanConvertFrom(context, sourceType);
				}
			}

	// Convert from another type to the one represented by this class.
	public override Object ConvertFrom(ITypeDescriptorContext context,
									   CultureInfo culture,
									   Object value)
			{
				if(value is String)
				{
					String val = (String)value;
					IReferenceService service;
					IContainer container;
					if(val == "(none)")
					{
						return null;
					}
					if(context != null)
					{
						service = (IReferenceService)
							(context.GetService(typeof(IReferenceService)));
						if(service != null)
						{
							return service.GetReference(val);
						}
						container = context.Container;
						if(container != null)
						{
							return container.Components[val];
						}
					}
					return null;
				}
				return base.ConvertFrom(context, culture, value);
			}

	// Convert this object into another type.
	public override Object ConvertTo(ITypeDescriptorContext context,
									 CultureInfo culture,
									 Object value, Type destinationType)
			{
				if(destinationType == null)
				{
					throw new ArgumentNullException("destinationType");
				}
				if(destinationType == typeof(String))
				{
					IReferenceService service;
					String name;
					if(value == null)
					{
						return "(none)";
					}
					if(context != null)
					{
						service = (IReferenceService)
							(context.GetService(typeof(IReferenceService)));
						if(service != null)
						{
							name = service.GetName(value);
							if(name != null)
							{
								return name;
							}
						}
					}
					if(value is IComponent)
					{
						ISite site = ((IComponent)value).Site;
						if(site != null)
						{
							name = site.Name;
							if(name != null)
							{
								return name;
							}
						}
					}
					return String.Empty;
				}
				else
				{
					return base.ConvertTo
						(context, culture, value, destinationType);
				}
			}

	// Return a collection of standard values for this data type.
	public override StandardValuesCollection GetStandardValues
				(ITypeDescriptorContext context)
			{
				IReferenceService service;
				Object[] refs;
				ArrayList list;

				// Bail out if there is no context.
				if(context == null)
				{
					return new StandardValuesCollection(new Object [] {null});
				}

				// Start with a list that contains null.
				list = new ArrayList();
				list.Add(null);

				// Try using the reference service to get the values.
				service = (IReferenceService)
					(context.GetService(typeof(IReferenceService)));
				if(service != null)
				{
					refs = service.GetReferences();
					foreach(Object obj in refs)
					{
						if(IsValueAllowed(context, obj))
						{
							list.Add(obj);
						}
					}
				}
				else if(context.Container != null)
				{
					// Determine the references from the context components.
					foreach(IComponent component in
								context.Container.Components)
					{
						if(type.IsInstanceOfType(component) &&
						   IsValueAllowed(context, component))
						{
							list.Add(component);
						}
					}
				}

				// Sort the list of references.
				list.Sort(new ValueComparer(this));

				// Convert the list into a standard values colleciton.
				return new StandardValuesCollection(list);
			}

	// Determine if the list of standard values is an exclusive list.
	public override bool GetStandardValuesExclusive
				(ITypeDescriptorContext context)
			{
				return true;
			}

	// Determine if "GetStandardValues" is supported.
	public override bool GetStandardValuesSupported
				(ITypeDescriptorContext context)
			{
				return true;
			}

	// Determine if a particular value is allowed.
	protected virtual bool IsValueAllowed
					(ITypeDescriptorContext context, Object value)
			{
				return true;
			}

	// Compare values in a standard values list.
	private sealed class ValueComparer : IComparer
	{
		// Internal state.
		private ReferenceConverter converter;

		// Constructor.
		public ValueComparer(ReferenceConverter converter)
				{
					this.converter = converter;
				}

		// Implement the IComparer interface.
		public int Compare(Object obj1, Object obj2)
				{
					return String.CompareOrdinal
						(converter.ConvertToString(obj1),
						 converter.ConvertToString(obj2));
				}

	}; // class ValueComparer

}; // class ReferenceConverter

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
