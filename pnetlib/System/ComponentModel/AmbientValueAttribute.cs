/*
 * AmbientValueAttribute.cs - Implementation of the
 *			"System.ComponentModel.AmbientValueAttribute" class.
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

[AttributeUsage(AttributeTargets.All)]
public sealed class AmbientValueAttribute : Attribute
{
	// Internal state.
	private Object obj;

	// Constructors.
	public AmbientValueAttribute(bool value)
			{
				obj = value;
			}
	public AmbientValueAttribute(byte value)
			{
				obj = value;
			}
	public AmbientValueAttribute(char value)
			{
				obj = value;
			}
	public AmbientValueAttribute(double value)
			{
				obj = value;
			}
	public AmbientValueAttribute(short value)
			{
				obj = value;
			}
	public AmbientValueAttribute(int value)
			{
				obj = value;
			}
	public AmbientValueAttribute(long value)
			{
				obj = value;
			}
	public AmbientValueAttribute(Object value)
			{
				obj = value;
			}
	public AmbientValueAttribute(float value)
			{
				obj = value;
			}
	public AmbientValueAttribute(String value)
			{
				obj = value;
			}
	public AmbientValueAttribute(Type type, String value)
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

	// Get the attribute's value.
	public Object Value
			{
				get
				{
					return obj;
				}
			}

	// Determine if two ambient attribute values are equal.
	public override bool Equals(Object value)
			{
				AmbientValueAttribute other = (value as AmbientValueAttribute);
				if(other != null)
				{
					if(obj == null)
					{
						return (other.obj == null);
					}
					else
					{
						return obj.Equals(other.obj);
					}
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this ambient value.
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

}; // class AmbientValueAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
