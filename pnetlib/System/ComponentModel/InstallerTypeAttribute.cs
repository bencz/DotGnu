/*
 * InstallerTypeAttribute.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.InstallerTypeAttribute" class.
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
using System.Globalization;

[AttributeUsage(AttributeTargets.Class)]
public class InstallerTypeAttribute : Attribute
{
	// Internal state.
	private String typeName;

	// Constructors.
	public InstallerTypeAttribute(String typeName)
			{
				this.typeName = typeName;
			}
	public InstallerTypeAttribute(Type type)
			{
				this.typeName = type.AssemblyQualifiedName;
			}

	// Get the name of the converter type.
	public virtual Type InstallerType
			{
				get
				{
					return Type.GetType(typeName);
				}
			}

	// Determine if two type converter attributes are equal.
	public override bool Equals(Object obj)
			{
				InstallerTypeAttribute a = (obj as InstallerTypeAttribute);
				if(a != null)
				{
					return (a.typeName == typeName);
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this instance.
	public override int GetHashCode()
			{
				return base.GetHashCode();
			}

}; // class InstallerTypeAttribute

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
