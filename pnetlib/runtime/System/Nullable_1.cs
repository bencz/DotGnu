/*
 * Nullable_1.cs - Implementation of the "System.Nullable<T>" class.
 *
 * Copyright (C) 2008  Southern Storm Software, Pty Ltd.
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

#if CONFIG_FRAMEWORK_2_0 && CONFIG_GENERICS

#if !ECMA_COMPAT && CONFIG_SERIALIZATION
using System.Runtime.Serialization;

[Serializable]
#endif
public struct Nullable<T> where T : struct
{
	private T value;
	private bool hasValue;

	public Nullable(T value)
			{
				this.value = value;
				hasValue = true;
			}

	public static T FromNullable(Nullable<T> value)
			{
				if(!value.hasValue)
				{
					throw new InvalidOperationException();
				}
				return value.value;
			}

	public static Nullable<T> ToNullable(T value)
			{
				return new Nullable<T>(value);
			}

	public static explicit operator T(Nullable<T> value)
			{
				if(!value.hasValue)
				{
					throw new InvalidOperationException();
				}
				return value.value;
			}

	public static implicit operator Nullable<T>(T value)
			{
				return new Nullable<T>(value);
			}

	public override bool Equals(Object obj)
			{
				if(obj == null)
				{
					return !hasValue;
				}
				if(hasValue)
				{
					return value.Equals(obj);
				}
				return false;
			}

	public override int GetHashCode()
			{
				if(hasValue)
				{
					return value.GetHashCode();
				}
				return 0;
			}

	public override string ToString()
			{
				if(hasValue)
				{
					return value.ToString();
				}
				return String.Empty;
			}

	public T GetValueOrDefault()
			{
				if(hasValue)
				{
					return value;
				}
				return new T();
			}

	public T GetValueOrDefault(T alternateDefaultValue)
			{
				if(hasValue)
				{
					return value;
				}
				return alternateDefaultValue;
			}

	public bool HasValue
			{
				get
				{
					return hasValue;	
				}
			}

	public T Value
			{
				get
				{
					if(!hasValue)
					{
						throw new InvalidOperationException();
					}
					return value;
				}
			}

}; // struct Nullable<T>

#endif // CONFIG_FRAMEWORK_2_0

}; // namespace System
