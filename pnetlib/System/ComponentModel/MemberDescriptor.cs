/*
 * MemberDescriptor.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.MemberDescriptor" class.
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

using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;

[ComVisible(true)]
public abstract class MemberDescriptor
{
	// Internal state.
	private AttributeCollection attributeCollection;
	private Attribute[] attributes;
	private String category;
	private String description;
	private bool designTimeOnly;
	private bool loadedDesignTimeOnly;
	private bool isBrowsable;
	private bool loadedBrowsable;
	private String displayName;
	private String name;

	// Constructors.
	protected MemberDescriptor(MemberDescriptor descr)
			: this(descr, null) {}
	protected MemberDescriptor(String name)
			: this(name, null) {}
	protected MemberDescriptor
				(MemberDescriptor descr, Attribute[] newAttributes)
			{
				this.name = descr.Name;
				this.displayName = descr.DisplayName;
				this.attributes =
					MergeAttributes(descr.AttributeArray, newAttributes);
			}
	protected MemberDescriptor(String name, Attribute[] newAttributes)
			{
				if(name == null || name.Length == 0)
				{
					throw new ArgumentException
						(S._("ArgRange_StringNonEmpty"), "name");
				}
				this.name = name;
				this.displayName = name;
				this.attributes = newAttributes;
			}

	// Merge two attribute lists.
	internal static Attribute[] MergeAttributes
				(Attribute[] list1, Attribute[] list2)
			{
				if(list1 == null)
				{
					if(list2 == null)
					{
						return null;
					}
					else
					{
						return (Attribute[])(list2.Clone());
					}
				}
				else if(list2 == null)
				{
					return (Attribute[])(list1.Clone());
				}
				Attribute[] list = new Attribute [list1.Length + list2.Length];
				Array.Copy(list1, 0, list, 0, list1.Length);
				Array.Copy(list2, 0, list, list1.Length, list2.Length);
				return list;
			}

	// Properties.
	public virtual AttributeCollection Attributes
			{
				get
				{
					if(attributeCollection == null)
					{
						attributeCollection = CreateAttributeCollection();
					}
					return attributeCollection;
				}
			}
	public virtual String Category
			{
				get
				{
					if(category == null)
					{
						CategoryAttribute attr;
						attr = (CategoryAttribute)
							(Attributes[typeof(CategoryAttribute)]);
						if(attr != null)
						{
							category = attr.Category;
						}
					}
					return category;
				}
			}
	public virtual String Description
			{
				get
				{
					if(description == null)
					{
						DescriptionAttribute attr;
						attr = (DescriptionAttribute)
							(Attributes[typeof(DescriptionAttribute)]);
						if(attr != null)
						{
							description = attr.Description;
						}
					}
					return description;
				}
			}
	public virtual bool DesignTimeOnly
			{
				get
				{
					if(!loadedDesignTimeOnly)
					{
						DesignOnlyAttribute attr;
						attr = (DesignOnlyAttribute)
							(Attributes[typeof(DesignOnlyAttribute)]);
						if(attr != null)
						{
							designTimeOnly = attr.IsDesignOnly;
						}
						loadedDesignTimeOnly = true;
					}
					return designTimeOnly;
				}
			}
	public virtual String DisplayName
			{
				get
				{
					return displayName;
				}
			}
	public virtual bool IsBrowsable
			{
				get
				{
					if(!loadedBrowsable)
					{
						BrowsableAttribute attr;
						attr = (BrowsableAttribute)
							(Attributes[typeof(BrowsableAttribute)]);
						if(attr != null)
						{
							isBrowsable = attr.Browsable;
						}
						loadedBrowsable = true;
					}
					return isBrowsable;
				}
			}
	public virtual String Name
			{
				get
				{
					if(name != null)
					{
						return name;
					}
					else
					{
						return "";
					}
				}
			}

	// Determine if this member descriptor is equal to another.
	public override bool Equals(Object obj)
			{
				MemberDescriptor other = (obj as MemberDescriptor);
				if(other != null)
				{
					if(other.Name != Name ||
					   other.Category != Category ||
					   other.Description != Description)
					{
						return false;
					}
					Attribute[] attrs = other.AttributeArray;
					if(attrs == null)
					{
						return (attributes == null || attributes.Length == 0);
					}
					else if(attributes == null)
					{
						return (attrs.Length == 0);
					}
					else if(attrs.Length != attributes.Length)
					{
						return false;
					}
					int index;
					for(index = 0; index < attrs.Length; ++index)
					{
						if(!attributes[index].Equals(attrs[index]))
						{
							return false;
						}
					}
					return true;
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return NameHashCode;
			}

	// Get or set the entire attribute array.
	protected virtual Attribute[] AttributeArray
			{
				get
				{
					return attributes;
				}
				set
				{
					attributeCollection = null;
					attributes = value;
				}
			}

	// Get the hash code for the member name.
	protected virtual int NameHashCode
			{
				get
				{
					if(name != null)
					{
						return name.GetHashCode();
					}
					else
					{
						return 0;
					}
				}
			}

	// Create the attribute collection.
	protected virtual AttributeCollection CreateAttributeCollection()
			{
				return new AttributeCollection(AttributeArray);
			}

	// Fill a list with all attributes.
	protected virtual void FillAttributes(IList attributeList)
			{
				if(attributes != null)
				{
					foreach(Object obj in attributes)
					{
						attributeList.Add(obj);
					}
				}
			}

	// Find a method by reflection in a component class.
	protected static MethodInfo FindMethod
				(Type componentClass, String name,
				 Type[] args, Type returnType)
			{
				return FindMethod(componentClass, name, args, returnType, true);
			}
	protected static MethodInfo FindMethod
				(Type componentClass, String name,
				 Type[] args, Type returnType, bool publicOnly)
			{
				MethodInfo method;
				method = componentClass.GetMethod
					(name, (publicOnly ? BindingFlags.Public |
						   				 BindingFlags.Static |
										 BindingFlags.Instance
									   : BindingFlags.Public |
									     BindingFlags.NonPublic |
										 BindingFlags.Static |
										 BindingFlags.Instance),
					 null, args, null);
				if(method != null && method.ReturnType == returnType)
				{
					return method;
				}
				else
				{
					return null;
				}
			}

	// Get the object to be invoked.
	protected static Object GetInvokee(Type componentClass, Object component)
			{
				if(componentClass.IsInstanceOfType(component))
				{
					return component;
				}
				if(!(component is IComponent))
				{
					return component;
				}
				ISite site = ((IComponent)component).Site;
				if(site == null || !(site.DesignMode))
				{
					return component;
				}
			#if CONFIG_COMPONENT_MODEL_DESIGN
				IDesignerHost host = (IDesignerHost)
					site.GetService(typeof(IDesignerHost));
				if(host == null)
				{
					return component;
				}
				IDesigner designer = host.GetDesigner((IComponent)component);
				if(designer == null ||
				   !(componentClass.IsInstanceOfType(designer)))
				{
					return component;
				}
				return designer;
			#else
				return component;
			#endif
			}

	// Get a component site for an object.
	protected static ISite GetSite(Object component)
			{
				IComponent comp = (component as IComponent);
				if(comp != null)
				{
					return comp.Site;
				}
				else
				{
					return null;
				}
			}

}; // class MemberDescriptor

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
