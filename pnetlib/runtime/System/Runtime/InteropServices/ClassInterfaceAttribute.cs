/*
 * ClassInterfaceAttribute.cs - Implementation of the
 *			"System.Runtime.InteropServices.ClassInterfaceAttribute" class.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_COM_INTEROP

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly,
			    Inherited=false)]
public sealed class ClassInterfaceAttribute : Attribute
{

	// Internal state.
	private ClassInterfaceType value;

	// Constructors.
	public ClassInterfaceAttribute(ClassInterfaceType classInterfaceType)
			{
				value = classInterfaceType;
			}
	public ClassInterfaceAttribute(short classInterfaceType)
			{
				value = (ClassInterfaceType)classInterfaceType;
			}

	// Get the attribute's value.
	public ClassInterfaceType Value
			{
				get
				{
					return value;
				}
			}

}; // class ClassInterfaceAttribute

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
