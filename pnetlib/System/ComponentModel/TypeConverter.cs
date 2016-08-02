/*
 * TypeConverter.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.TypeConverter" class.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL

using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design.Serialization;

[ComVisible(true)]
public class TypeConverter
{
	// Constructor.
	public TypeConverter()
			{
				// Nothing to do here.
			}

	// Determine if we can convert from a specific type to this one.
	public bool CanConvertFrom(Type sourceType)
			{
				return CanConvertFrom(null, sourceType);
			}
	public virtual bool CanConvertFrom
				(ITypeDescriptorContext context, Type sourceType)
			{
			#if CONFIG_COMPONENT_MODEL_DESIGN
				// By default, we can always convert instance descriptors.
				return (sourceType == typeof(InstanceDescriptor));
			#else
				return false;
			#endif
			}

	// Determine if we can convert from this type to a specific type.
	public bool CanConvertTo(Type destinationType)
			{
				return CanConvertTo(null, destinationType);
			}
	public virtual bool CanConvertTo
				(ITypeDescriptorContext context, Type destinationType)
			{
				// By default, we can always convert to the string type.
				return (destinationType == typeof(String));
			}

	// Convert from another type to the one represented by this class.
	public Object ConvertFrom(Object value)
			{
				return ConvertFrom(null, CultureInfo.CurrentCulture, value);
			}
	public virtual Object ConvertFrom(ITypeDescriptorContext context,
									  CultureInfo culture,
									  Object value)
			{
			#if CONFIG_COMPONENT_MODEL_DESIGN
				if(value is InstanceDescriptor)
				{
					return ((InstanceDescriptor)value).Invoke();
				}
			#endif
				throw GetConvertFromException(value);
			}

	// Convert a string into this type using the invariant culture.
	public Object ConvertFromInvariantString(String text)
			{
				return ConvertFromString
					(null, CultureInfo.InvariantCulture, text);
			}
	public Object ConvertFromInvariantString
				(ITypeDescriptorContext context, String text)
			{
				return ConvertFromString
					(context, CultureInfo.InvariantCulture, text);
			}

	// Convert a string into this type.
	public Object ConvertFromString(String text)
			{
				return ConvertFrom(null, CultureInfo.CurrentCulture, text);
			}
	public Object ConvertFromString
				(ITypeDescriptorContext context, String text)
			{
				return ConvertFrom(context, CultureInfo.CurrentCulture, text);
			}
	public Object ConvertFromString
				(ITypeDescriptorContext context,
				 CultureInfo culture, String text)
			{
				return ConvertFrom(context, culture, text);
			}

	// Convert this object into another type.
	public Object ConvertTo(Object value, Type destinationType)
			{
				return ConvertTo(null, null, value, destinationType);
			}
	public virtual Object ConvertTo(ITypeDescriptorContext context,
									CultureInfo culture,
									Object value, Type destinationType)
			{
				if(destinationType == null)
				{
					throw new ArgumentNullException("destinationType");
				}
				if(destinationType == typeof(String))
				{
					if(value != null)
					{
						return value.ToString();
					}
					else
					{
						return String.Empty;
					}
				}
				else
				{
					throw GetConvertToException(value, destinationType);
				}
			}

	// Convert an object to a culture-invariant string.
	public String ConvertToInvariantString(Object value)
			{
				return ConvertToString
					(null, CultureInfo.InvariantCulture, value);
			}
	public String ConvertToInvariantString
				(ITypeDescriptorContext context, Object value)
			{
				return ConvertToString
					(context, CultureInfo.InvariantCulture, value);
			}

	// Convert an object into a string.
	public String ConvertToString(Object value)
			{
				return (String)ConvertTo(null, CultureInfo.CurrentCulture,
								 		 value, typeof(String));
			}
	public String ConvertToString
				(ITypeDescriptorContext context, Object value)
			{
				return (String)ConvertTo(context, CultureInfo.CurrentCulture,
								 		 value, typeof(String));
			}
	public String ConvertToString
				(ITypeDescriptorContext context,
				 CultureInfo culture, Object value)
			{
				return (String)ConvertTo(context, culture,
										 value, typeof(String));
			}

	// Create an instance of this type of object.
	public Object CreateInstance(IDictionary propertyValues)
			{
				return CreateInstance(null, propertyValues);
			}
	public virtual Object CreateInstance
				(ITypeDescriptorContext context, IDictionary propertyValues)
			{
				return null;
			}

	// Determine if creating new instances is supported.
	public bool GetCreateInstanceSupported()
			{
				return GetCreateInstanceSupported(null);
			}
	public virtual bool GetCreateInstanceSupported
				(ITypeDescriptorContext context)
			{
				return false;
			}

	// Get the properties for an object.
	public PropertyDescriptorCollection GetProperties(Object value)
			{
				return GetProperties(null, value, null);
			}
	public PropertyDescriptorCollection GetProperties
				(ITypeDescriptorContext context, Object value)
			{
				return GetProperties(context, value, null);
			}
	public virtual PropertyDescriptorCollection GetProperties
				(ITypeDescriptorContext context, Object value,
				 Attribute[] attributes)
			{
				return null;
			}

	// Determine if the "GetProperties" method is supported.
	public bool GetPropertiesSupported()
			{
				return GetPropertiesSupported(null);
			}
	public virtual bool GetPropertiesSupported
				(ITypeDescriptorContext context)
			{
				return false;
			}

	// Return a collection of standard values for this data type.
	public ICollection GetStandardValues()
			{
				return GetStandardValues(null);
			}
	public virtual StandardValuesCollection GetStandardValues
				(ITypeDescriptorContext context)
			{
				return null;
			}

	// Determine if the list of standard values is an exclusive list.
	public bool GetStandardValuesExclusive()
			{
				return GetStandardValuesExclusive(null);
			}
	public virtual bool GetStandardValuesExclusive
				(ITypeDescriptorContext context)
			{
				return false;
			}

	// Determine if "GetStandardValues" is supported.
	public bool GetStandardValuesSupported()
			{
				return GetStandardValuesSupported(null);
			}
	public virtual bool GetStandardValuesSupported
				(ITypeDescriptorContext context)
			{
				return false;
			}

	// Determine if an object is valid for this type.
	public bool IsValid(Object value)
			{
				return IsValid(null, value);
			}
	public virtual bool IsValid(ITypeDescriptorContext context, Object value)
			{
				return true;
			}

	// Get the exception to use when "ConvertFrom" cannot be performed.
	protected Exception GetConvertFromException(Object value)
			{
				return new NotSupportedException(S._("NotSupp_Conversion"));
			}

	// Get the exception to use when "ConvertTo" cannot be performed.
	protected Exception GetConvertToException
				(Object value, Type destinationType)
			{
				return new NotSupportedException(S._("NotSupp_Conversion"));
			}

	// Sort a collection of properties.
	protected PropertyDescriptorCollection SortProperties
				(PropertyDescriptorCollection props, String[] names)
			{
				props.Sort(names);
				return props;
			}

	// Wrap a collection to make it indexable.
	public class StandardValuesCollection : ICollection, IEnumerable
	{
		// Internal state.
		private ICollection values;
		
		// Constructor.
		public StandardValuesCollection(ICollection values)
				{
					if(values != null)
					{
						this.values = values;
					}
					else
					{
						this.values = new Object[0];
					}
				}

		// Get the number of elements in this collection.
		public int Count
				{
					get
					{
						return values.Count;
					}
				}

		// Copy the elements of this collection into an array.
		public void CopyTo(Array array, int index)
				{
					values.CopyTo(array, index);
				}

		// Get an enumerator for this collection.
		public IEnumerator GetEnumerator()
				{
					return values.GetEnumerator();
				}

		// Implement the ICollection interface.
		void ICollection.CopyTo(Array array, int index)
				{
					values.CopyTo(array, index);
				}
		int ICollection.Count
				{
					get 
					{
						return values.Count;
					}
				}
		bool ICollection.IsSynchronized
				{
					get 
					{
						return false; 
					}
				}
		Object ICollection.SyncRoot
				{
					get 
					{ 
						return null;
					}
				}

		// Implement the IEnumerable interface.
		IEnumerator IEnumerable.GetEnumerator()
				{
					return values.GetEnumerator();
				}

		// Get an item from this collection.
		public Object this[int index]
				{
					get
					{
						if(values is IList)
						{
							return ((IList)values)[index];
						}
						else
						{
							if(index < 0 || index >= values.Count)
							{
								throw new IndexOutOfRangeException
									(S._("Arg_InvalidArrayIndex"));
							}
							IEnumerator e = values.GetEnumerator();
							while(e.MoveNext())
							{
								if(index == 0)
								{
									return e.Current;
								}
								--index;
							}
							throw new IndexOutOfRangeException
								(S._("Arg_InvalidArrayIndex"));
						}
					}
				}

	}; // class StandardValuesCollection

	// Simple property descriptor for objects that don't have properties.
	protected abstract class SimplePropertyDescriptor : PropertyDescriptor
	{
		// Internal state.
		private Type componentType;
		private Type propertyType;

		// Constructors.
		public SimplePropertyDescriptor
					(Type componentType, String name, Type propertyType)
				: base(name, null)
				{
					this.componentType = componentType;
					this.propertyType = propertyType;
				}
		public SimplePropertyDescriptor
					(Type componentType, String name, Type propertyType,
					 Attribute[] attributes)
				: base(name, attributes)
				{
					this.componentType = componentType;
					this.propertyType = propertyType;
				}

		// Get the component type that owns this property.
		public override Type ComponentType
				{
					get
					{
						return componentType;
					}
				}

		// Determine if this property is read-only.
		public override bool IsReadOnly
				{
					get
					{
						ReadOnlyAttribute attr;
						attr = (ReadOnlyAttribute)
							(Attributes[typeof(ReadOnlyAttribute)]);
						if(attr != null)
						{
							return attr.IsReadOnly;
						}
						else
						{
							return false;
						}
					}
				}

		// Get the type of this property.
		public override Type PropertyType
				{
					get
					{
						return propertyType;
					}
				}

		// Determine if resetting a component's property will change its value.
		public override bool CanResetValue(Object component)
				{
					DefaultValueAttribute attr;
					attr = (DefaultValueAttribute)
						(Attributes[typeof(DefaultValueAttribute)]);
					if(attr != null)
					{
						// Strictly speaking, this should probably
						// check that the values are *not* equal, but
						// other implementations check for equality.
						// So we do that as well, for compatibility.
						Object value1 = GetValue(component);
						Object value2 = attr.Value;
						if(value1 == null)
						{
							return (value2 == null);
						}
						else
						{
							return value1.Equals(value2);
						}
					}
					else
					{
						return false;
					}
				}

		// Reset the property value associated with a component.
		public override void ResetValue(Object component)
				{
					DefaultValueAttribute attr;
					attr = (DefaultValueAttribute)
						(Attributes[typeof(DefaultValueAttribute)]);
					if(attr != null)
					{
						SetValue(component, attr.Value);
					}
				}

		// Determine if a property value needs to be serialized.
		public override bool ShouldSerializeValue(Object component)
				{
					return false;
				}

	}; // class SimplePropertyDescriptor

}; // class TypeConverter

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
