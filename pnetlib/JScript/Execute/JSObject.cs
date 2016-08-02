/*
 * JSObject.cs - Common base for JScript objects.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.JScript
{

using System;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices.Expando;
using Microsoft.JScript.Vsa;

public class JSObject : ScriptObject, IEnumerable
#if !ECMA_COMPAT
	, IExpando
#endif
{
	// Internal state.
	private Property properties;
	private Hashtable overflow;

	// Constructors.
	public JSObject() : this(null) {}
	internal JSObject(ScriptObject parent)
			: base(parent)
			{
				properties = null;
				overflow = null;
			}
	internal JSObject(ScriptObject parent, VsaEngine engine)
			: this(parent)
			{
				this.engine = engine;
			}

	// Implement the IEnumerable interface.
	IEnumerator IEnumerable.GetEnumerator()
			{
				return ForIn.JScriptGetEnumerator(this);
			}

	// Get member information for this object.
	public override MemberInfo[] GetMember
				(String name, BindingFlags bindingAttr)
			{
				// TODO
				return null;
			}
	public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
			{
				// TODO
				return null;
			}

	// Implement the IExpando interface.
	public FieldInfo AddField(String name)
			{
				// TODO
				return null;
			}
#if !ECMA_COMPAT
	MethodInfo IExpando.AddMethod(String name, Delegate method)
			{
				// Not used by JScript.
				return null;
			}
	PropertyInfo IExpando.AddProperty(String name)
			{
				// Not used by JScript.
				return null;
			}
	void IExpando.RemoveMember(MemberInfo m)
			{
				// TODO
			}
#endif

	// Set the value of a field member.
	public void SetMemberValue2(String name, Object value)
			{
				// TODO
			}

	// Convert this object into a string.
	public override String ToString()
			{
				// Find the "toString" method on the object or its prototype.
				JSObject temp = this;
				ScriptFunction toStr;
				do
				{
					toStr = (temp.Get("toString") as ScriptFunction);
					if(toStr != null)
					{
						// Invoke the "toString" method.
						return (toStr.Invoke(this)).ToString();
					}
					temp = (temp.GetParent() as JSObject);
				}
				while(temp != null);
				return String.Empty;
			}

	// Get a property from this object.  Null if not present.
	internal override Object Get(String name)
			{
				Property prop;

				// Check the overflow hash table first, if it exists.
				if(overflow != null)
				{
					prop = (Property)(overflow[name]);
					if(prop != null)
					{
						if((prop.attrs & PropertyAttributes.Deferred) == 0)
						{
							return prop.value;
						}
						else
						{
							return ((FieldInfo)(prop.value)).GetValue(this);
						}
					}
					else
					{
						return base.Get(name);
					}
				}

				// Scan the property list.
				prop = properties;
				while(prop != null)
				{
					if(prop.name == name)
					{
						if((prop.attrs & PropertyAttributes.Deferred) == 0)
						{
							return prop.value;
						}
						else
						{
							return ((FieldInfo)(prop.value)).GetValue(this);
						}
					}
					prop = prop.next;
				}

				// Try looking in the prototype.
				return base.Get(name);
			}

	// Determine if a property is enumerable.
	internal bool IsEnumerable(String name)
			{
				Property prop;

				// Check the overflow hash table first, if it exists.
				if(overflow != null)
				{
					prop = (Property)(overflow[name]);
					if(prop != null)
					{
						return (prop.attrs & PropertyAttributes.DontEnum) == 0;
					}
					else
					{
						return false;
					}
				}

				// Scan the property list.
				prop = properties;
				while(prop != null)
				{
					if(prop.name == name)
					{
						return (prop.attrs & PropertyAttributes.DontEnum) == 0;
					}
					prop = prop.next;
				}

				// Could not find the property.
				return false;
			}

	// Determine if this object has a specific property.
	internal override bool HasOwnProperty(String name)
			{
				Property prop;

				// Check the overflow hash table first, if it exists.
				if(overflow != null)
				{
					prop = (Property)(overflow[name]);
					return (prop != null);
				}

				// Scan the property list.
				prop = properties;
				while(prop != null)
				{
					if(prop.name == name)
					{
						return true;
					}
					prop = prop.next;
				}

				// Could not find the property.
				return false;
			}

	// Put a property in this object.
	internal override void Put(String name, Object value)
			{
				Put(name, value, PropertyAttributes.None);
			}
	internal void Put(String name, Object value, PropertyAttributes attrs)
			{
				Property prop, prev;
				int num;

				// Normalize null values.
				if(value == null)
				{
					value = DBNull.Value;
				}

				// Check the overflow hash table first, if it exists.
				if(overflow != null)
				{
					prop = (Property)(overflow[name]);
					if(prop == null)
					{
						overflow[name] = new Property(name, value, attrs);
					}
					else if((prop.attrs & PropertyAttributes.ReadOnly) == 0)
					{
						if((prop.attrs & PropertyAttributes.Deferred) == 0)
						{
							prop.value = value;
						}
						else
						{
							((FieldInfo)(prop.value)).SetValue(this, value);
						}
					}
					return;
				}

				// Scan for a property with the given name.
				prop = properties;
				prev = null;
				num = 0;
				while(prop != null)
				{
					if(prop.name == name)
					{
						if((prop.attrs & PropertyAttributes.ReadOnly) == 0)
						{
							if((prop.attrs & PropertyAttributes.Deferred) == 0)
							{
								prop.value = value;
							}
							else
							{
								((FieldInfo)(prop.value)).SetValue(this, value);
							}
						}
						return;
					}
					prev = prop;
					prop = prop.next;
					++num;
				}

				// Add a new property to the list.
				if(num < 8)
				{
					prop = new Property(name, value, attrs);
					if(prev != null)
					{
						prev.next = prop;
					}
					else
					{
						properties = prop;
					}
					return;
				}

				// The list is too big, so switch to a hash table.
				overflow = new Hashtable(32);
				prop = properties;
				while(prop != null)
				{
					overflow[prop.name] = prop;
					prev = prop;
					prop = prop.next;
					prev.next = null;
				}
				properties = null;
				overflow[name] = new Property(name, value, attrs);
			}

	// Delete a property from this object.
	internal override bool Delete(String name)
			{
				Property prop, prev;

				// Check the overflow hash table first, if it exists.
				if(overflow != null)
				{
					prop = (Property)(overflow[name]);
					if(prop != null)
					{
						if((prop.attrs & PropertyAttributes.DontDelete) != 0)
						{
							return false;
						}
						overflow.Remove(name);
					}
					return true;
				}

				// Scan the property list.
				prop = properties;
				prev = null;
				while(prop != null)
				{
					if(prop.name == name)
					{
						if((prop.attrs & PropertyAttributes.DontDelete) != 0)
						{
							return false;
						}
						if(prev != null)
						{
							prev.next = prop.next;
						}
						else
						{
							properties = prop.next;
						}
						return true;
					}
					prev = prop;
					prop = prop.next;
				}

				// Could not find the property, so act as though it is deleted.
				return true;
			}

	// Get an enumerator for the properties in this object.
	internal override IEnumerator GetPropertyEnumerator()
			{
				if(overflow != null)
				{
					return new HashKeyEnumerator(overflow.GetEnumerator());
				}
				else
				{
					return new PropertyEnumerator(this);
				}
			}

	// Add a builtin method to a prototype.
	internal void AddBuiltin(EngineInstance inst, String name)
			{
				MethodInfo method = GetType().GetMethod(name);
				Put(name, new BuiltinFunction
					(inst.GetFunctionPrototype(), name, method),
					PropertyAttributes.None);
			}

	// Storage for a property.
	private sealed class Property
	{
		// Accessible internal state.
		public Property next;
		public String name;
		public Object value;
		public PropertyAttributes attrs;

		// Constructor.
		public Property(String name, Object value, PropertyAttributes attrs)
				{
					this.next = null;
					this.name = name;
					this.value = value;
					this.attrs = attrs;
				}

	}; // class Property

	// Enumerator class for properties in this object.
	private sealed class PropertyEnumerator : IEnumerator
	{
		// Internal state.
		private JSObject obj;
		private Property prop;
		private Property current;

		// Constructor.
		public PropertyEnumerator(JSObject obj)
				{
					this.obj = obj;
					this.prop = obj.properties;
					this.current = null;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					while(prop != null)
					{
						current = prop;
						prop = prop.next;
						if((current.attrs & PropertyAttributes.DontEnum) == 0)
						{
							return true;
						}
					}
					return false;
				}
		public void Reset()
				{
					prop = obj.properties;
					current = null;
				}
		public Object Current
				{
					get
					{
						if(current != null)
						{
							return current.name;
						}
						else
						{
							throw new InvalidOperationException();
						}
					}
				}

	}; // class PropertyEnumerator

	// Enumerator class for keys in a hash table.
	private sealed class HashKeyEnumerator : IEnumerator
	{
		// Internal state.
		private IDictionaryEnumerator e;

		// Constructor.
		public HashKeyEnumerator(IDictionaryEnumerator e)
				{
					this.e = e;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					Property prop;
					while(e.MoveNext())
					{
						prop = (Property)(e.Value);
						if((prop.attrs & PropertyAttributes.DontEnum) == 0)
						{
							return true;
						}
					}
					return false;
				}
		public void Reset()
				{
					e.Reset();
				}
		public Object Current
				{
					get
					{
						return e.Key;
					}
				}

	}; // class HashKeyEnumerator

}; // class JSObject

}; // namespace Microsoft.JScript
