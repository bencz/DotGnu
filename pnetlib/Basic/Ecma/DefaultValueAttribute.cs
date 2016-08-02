/*
 * DefaultValueAttribute.cs - Implementation of the
 *			"System.ComponentModel.DefaultValueAttribute" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

#if !CONFIG_COMPONENT_MODEL && !CONFIG_EXTENDED_DIAGNOSTICS

[AttributeUsage(AttributeTargets.All)]
internal sealed class DefaultValueAttribute : Attribute
{
	// Internal state.
	private Object obj;

	// Constructor.
	public DefaultValueAttribute(bool value)
			{
				obj = value;
			}
	public DefaultValueAttribute(byte value)
			{
				obj = value;
			}
	public DefaultValueAttribute(char value)
			{
				obj = value;
			}
	public DefaultValueAttribute(double value)
			{
				obj = value;
			}
	public DefaultValueAttribute(short value)
			{
				obj = value;
			}
	public DefaultValueAttribute(int value)
			{
				obj = value;
			}
	public DefaultValueAttribute(long value)
			{
				obj = value;
			}
	public DefaultValueAttribute(Object value)
			{
				obj = value;
			}
	public DefaultValueAttribute(float value)
			{
				obj = value;
			}
	public DefaultValueAttribute(String value)
			{
				obj = value;
			}
#if false
	public DefaultValueAttribute(Type type, String value)
			{
				try
				{
					obj = TypeDescriptor.GetConverter(type)
							.ConvertFromInvariantString(value);
				}
				catch(Exception)
				{
					// Ignore exceptions during type conversion.
				}
			}
#endif

	// Get the attribute's value.
	public Object Value
			{
				get
				{
					return obj;
				}
			}

	// Determine if two instances of this class are equal.
	public override bool Equals(Object obj)
			{
				DefaultValueAttribute other = (obj as DefaultValueAttribute);
				if(other != null)
				{
					return obj.Equals(other.obj);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this attribute.
	public override int GetHashCode()
			{
				if(obj != null)
				{
					return obj.GetHashCode();
				}
				else
				{
					return 0;
				}
			}

}; // class DefaultValueAttribute

#endif // !CONFIG_COMPONENT_MODEL && !CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.ComponentModel
