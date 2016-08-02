/*
 * ValueType.cs - Implementation of the "System.ValueType" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System
{

using System.Reflection;

public abstract class ValueType
{
	// Constructor.
	protected ValueType() : base() {}

#if CONFIG_REFLECTION

	// Determine if this value type instance is identical to another.
	[ClrReflection]
	public override bool Equals(Object obj)
			{
				if(obj == null)
				{
					return false;
				}
				else
				{
					// The types must be identical.
					Type type = GetType();
					if(type != obj.GetType())
					{
						return false;
					}

					// All fields must be identical.  Because of
					// the "ClrReflection" attribute on this method,
					// the runtime engine will allow us to access
					// every field, including private fields.
					FieldInfo[] fields = type.GetFields
							(BindingFlags.Public |
							 BindingFlags.NonPublic |
							 BindingFlags.Instance);

					// Check each of the fields for equality in turn.
					int posn;
					Object value1;
					Object value2;
					for(posn = 0; posn < fields.Length; ++posn)
					{
						value1 = fields[posn].GetValue(this);
						value2 = fields[posn].GetValue(obj);
						if(value1 == null)
						{
							if(value2 != null)
							{
								return false;
							}
						}
						else if(value2 == null)
						{
							return false;
						}
						else if(!value1.Equals(value2))
						{
							return false;
						}
					}
					return true;
				}
			}

	// Get a hash code for this instance.
	[ClrReflection]
	public override int GetHashCode()
			{
				// Use the hash value for the first non-null field.
				// The "ClrReflection" attribute will allow us to
				// access the private internals of the value.
				FieldInfo[] fields = GetType().GetFields
						(BindingFlags.Public |
						 BindingFlags.NonPublic |
						 BindingFlags.Instance);
				int posn;
				Object value;
				for(posn = 0; posn < fields.Length; ++posn)
				{
					value = fields[posn].GetValue(this);
					if(value != null)
					{
						return value.GetHashCode();
					}
				}

				// There are no non-null fields, so hash the type instead.
				return GetType().GetHashCode();
			}

#endif // CONFIG_REFLECTION

	// Get a string that corresponds to this instance.
	public override String ToString()
			{
				return GetType().FullName;
			}

}; // class ValueType

}; // namespace System
