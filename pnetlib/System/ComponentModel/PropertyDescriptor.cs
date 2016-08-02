/*
 * PropertyDescriptor.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.PropertyDescriptor" class.
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

using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

[ComVisible(true)]
public abstract class PropertyDescriptor : MemberDescriptor
{
	// Internal state.
	private TypeConverter converter;
	private Hashtable changedHandlers;

	// Constructors.
	protected PropertyDescriptor(MemberDescriptor descr) : base(descr) {}
	protected PropertyDescriptor(String name, Attribute[] attrs)
			: base(name, attrs) {}
	protected PropertyDescriptor(MemberDescriptor descr, Attribute[] attrs) 
			: base(descr, attrs) {}

	// Get the component type that owns this property.
	public abstract Type ComponentType { get; }

	// Get the type converter for this property's value.
	public virtual TypeConverter Converter
			{
				get
				{
					if(converter == null)
					{
						TypeConverterAttribute attr;
						attr = (TypeConverterAttribute)
							(Attributes[typeof(TypeConverterAttribute)]);
						if(attr != null)
						{
							// The property has a converter declaration.
							Type type = GetTypeFromName(attr.ConverterTypeName);
							if(type != null)
							{
								converter = (TypeConverter)CreateInstance(type);
							}
						}
						if(converter == null)
						{
							// Check the property type for a converter.
							converter =
								TypeDescriptor.GetConverter(PropertyType);
						}
					}
					return converter;
				}
			}

	// Determine if this property is localizable.
	public virtual bool IsLocalizable
			{
				get
				{
					LocalizableAttribute attr;
					attr = (LocalizableAttribute)
						(Attributes[typeof(LocalizableAttribute)]);
					if(attr != null)
					{
						return attr.IsLocalizable;
					}
					else
					{
						return false;
					}
				}
			}

	// Determine if this property is read-only.
	public abstract bool IsReadOnly { get; }

	// Get the type of this property.
	public abstract Type PropertyType { get; }

	// Get the serialization visibility of this property.
	public DesignerSerializationVisibility SerializationVisibility
			{
				get
				{
					DesignerSerializationVisibilityAttribute attr;
					attr = (DesignerSerializationVisibilityAttribute)
						(Attributes
						  [typeof(DesignerSerializationVisibilityAttribute)]);
					if(attr != null)
					{
						return attr.Visibility;
					}
					else
					{
						return DesignerSerializationVisibility.Visible;
					}
				}
			}

	// Add an event delegate that tracks when this value is changed.
	public virtual void AddValueChanged(Object component, EventHandler handler)
			{
				if(component == null)
				{
					throw new ArgumentNullException("component");
				}
				if(handler == null)
				{
					throw new ArgumentNullException("handler");
				}
				if(changedHandlers == null)
				{
					changedHandlers = new Hashtable();
				}
				Object list = changedHandlers[component];
				if(list == null)
				{
					changedHandlers[component] = handler;
				}
				else
				{
					changedHandlers[component] =
						((EventHandler)list) + handler;
				}
			}

	// Determine if resetting a component's property will change its value.
	public abstract bool CanResetValue(Object component);

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				if(obj is PropertyDescriptor)
				{
					return base.Equals(obj);
				}
				else
				{
					return false;
				}
			}

	// Get the child properties.
	public PropertyDescriptorCollection GetChildProperties()
			{
				return GetChildProperties(null, null);
			}
	public PropertyDescriptorCollection GetChildProperties(Attribute[] filter)
			{
				return GetChildProperties(null, filter);
			}
	public PropertyDescriptorCollection GetChildProperties(Object instance)
			{
				return GetChildProperties(instance, null);
			}
	public virtual PropertyDescriptorCollection GetChildProperties
				(Object instance, Attribute[] filter)
			{
				if(instance != null)
				{
					return TypeDescriptor.GetProperties(instance, filter);
				}
				else
				{
					return TypeDescriptor.GetProperties
						(PropertyType, filter);
				}
			}

	// Get an editor of the specified type.
	public virtual Object GetEditor(Type editorBaseType)
			{
				// Look for an editor declaration on the property itself.
				foreach(Attribute attr in Attributes)
				{
					if(attr is EditorAttribute)
					{
						if(GetTypeFromName(((EditorAttribute)attr)
												.EditorBaseTypeName)
								== editorBaseType)
						{
							Type type = GetTypeFromName
								(((EditorAttribute)attr).EditorTypeName);
							if(type != null)
							{
								return CreateInstance(type);
							}
						}
					}
				}

				// Look for an editor for the property's type.
				return TypeDescriptor.GetEditor(PropertyType, editorBaseType);
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return base.GetHashCode();
			}

	// Get the property value associated with a component.
	public abstract Object GetValue(Object component);

	// Remove an event delegate that tracks when this value is changed.
	public virtual void RemoveValueChanged
				(Object component, EventHandler handler)
			{
				if(component == null)
				{
					throw new ArgumentNullException("component");
				}
				if(handler == null)
				{
					throw new ArgumentNullException("handler");
				}
				if(changedHandlers == null)
				{
					return;
				}
				Object list = changedHandlers[component];
				if(list != null)
				{
					list = ((EventHandler)list) - handler;
					if(list != null)
					{
						changedHandlers[component] = list;
					}
					else
					{
						changedHandlers.Remove(component);
					}
				}
			}

	// Reset the property value associated with a component.
	public abstract void ResetValue(Object component);

	// Set the property value associated with a component.
	public abstract void SetValue(Object component, Object value);

	// Determine if a property value needs to be serialized.
	public abstract bool ShouldSerializeValue(Object component);

	// Create an instance of a type.
	protected Object CreateInstance(Type type)
			{
				return Activator.CreateInstance
					(type, new Object[] {PropertyType});
			}

	// Get a type from its name.
	protected Type GetTypeFromName(String typeName)
			{
				return Type.GetType(typeName);
			}

	// Raise a value changed event for a component.
	protected virtual void OnValueChanged(Object component, EventArgs e)
			{
				if(component != null && changedHandlers != null)
				{
					EventHandler list = (EventHandler)
						changedHandlers[component];
					if(list != null)
					{
						list(component, e);
					}
				}
			}

}; // class PropertyDescriptor

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
