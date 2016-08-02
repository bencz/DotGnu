/*
 * VBFixedStringAttribute.cs - Implementation of the
 *			"Microsoft.VisualBasic.VBFixedStringAttribute" class.
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

namespace Microsoft.VisualBasic
{

using System;

[AttributeUsage(AttributeTargets.Field, Inherited=false, AllowMultiple=false)]
public sealed class VBFixedStringAttribute : Attribute
{
	// Internal state.
	private int length;

	// Constructor.
	public VBFixedStringAttribute(int length)
			{
				if(length < 1 || length > 0x7FFF)
				{
					throw new ArgumentException(S._("VB_InvalidStringLength"));
				}
				this.length = length;
			}

	// Get this attribute's value.
	public int Length
			{
				get
				{
					return length;
				}
			}

}; // class VBFixedStringAttribute

}; // namespace Microsoft.VisualBasic
