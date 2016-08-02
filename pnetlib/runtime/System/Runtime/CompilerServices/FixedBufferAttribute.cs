/*
 * FixedBufferAttribute.cs - Implementation of the
 *	"System.Runtime.CompilerServices.FixedBufferAttribute" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.CompilerServices
{

#if CONFIG_FRAMEWORK_1_2

[AttributeUsage(AttributeTargets.Field, Inherited=false)]
public sealed class FixedBufferAttribute : Attribute
{
	// Internal state.
	private Type elementType;
	private int length;

	// Constructors.
	public FixedBufferAttribute(Type elementType, int length)
			{
				this.elementType = elementType;
				this.length = length;
			}

	// Get this attribute's properties.
	public Type ElementType
			{
				get
				{
					return elementType;
				}
			}
	public int Length
			{
				get
				{
					return length;
				}
			}

}; // class FixedBufferAttribute

#endif // CONFIG_FRAMEWORK_1_2

}; // namespace System.Runtime.CompilerServices
