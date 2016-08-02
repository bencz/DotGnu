/*
 * InheritanceAttribute.cs - Implementation of the
 *			"System.ComponentModel.InheritanceAttribute" class.
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

[AttributeUsage(AttributeTargets.Property |
				AttributeTargets.Field |
				AttributeTargets.Event)]
public sealed class InheritanceAttribute : Attribute
{
	// Internal state.
	private InheritanceLevel level;

	// Standard inheritance values.
	public static readonly InheritanceAttribute Default
			= new InheritanceAttribute();
	public static readonly InheritanceAttribute Inherited
			= new InheritanceAttribute(InheritanceLevel.Inherited);
	public static readonly InheritanceAttribute InheritedReadOnly
			= new InheritanceAttribute(InheritanceLevel.InheritedReadOnly);
	public static readonly InheritanceAttribute NotInherited
			= new InheritanceAttribute(InheritanceLevel.NotInherited);

	// Constructors.
	public InheritanceAttribute()
			{
				level = InheritanceLevel.NotInherited;
			}
	public InheritanceAttribute(InheritanceLevel inheritanceLevel)
			{
				level = inheritanceLevel;
			}

	// Get the inheritance level within this object.
	public InheritanceLevel InheritanceLevel
			{
				get
				{
					return level;
				}
				set
				{
					level = value;
				}
			}

	// Determine if two objects are equal.
	public override bool Equals(Object value)
			{
				InheritanceAttribute other = (value as InheritanceAttribute);
				if(other != null)
				{
					return (level == other.level);
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return (int)level;
			}

	// Determine if this attribute has the default value.
	public override bool IsDefaultAttribute()
			{
				return (level == Default.level);
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return TypeDescriptor.GetConverter(typeof(InheritanceLevel))
						.ConvertToString(level);
			}

}; // class InheritanceAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
