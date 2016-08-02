/*
 * VBFixedArrayAttribute.cs - Implementation of the
 *			"Microsoft.VisualBasic.VBFixedArrayAttribute" class.
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
public sealed class VBFixedArrayAttribute : Attribute
{
	// Internal state.
	private int upperBound1;
	private int upperBound2;

	// Constructors.
	public VBFixedArrayAttribute(int UpperBound1)
			{
				if(UpperBound1 < 0)
				{
					throw new ArgumentException(S._("VB_InvalidArrayBound"));
				}
				this.upperBound1 = UpperBound1;
				this.upperBound2 = -1;
			}
	public VBFixedArrayAttribute(int UpperBound1, int UpperBound2)
			{
				if(UpperBound1 < 0 || UpperBound2 < 0)
				{
					throw new ArgumentException(S._("VB_InvalidArrayBound"));
				}
				this.upperBound1 = UpperBound1;
				this.upperBound2 = UpperBound2;
			}

	// Get this attribute's values.
	public int[] Bounds
			{
				get
				{
					if(upperBound2 != -1)
					{
						return new int[] {upperBound1, upperBound2};
					}
					else
					{
						return new int[] {upperBound1};
					}
				}
			}
	public int Length
			{
				get
				{
					checked
					{
						if(upperBound2 != -1)
						{
							return (upperBound1 + 1) * (upperBound2 + 1);
						}
						else
						{
							return upperBound1 + 1;
						}
					}
				}
			}

}; // class VBFixedArrayAttribute

}; // namespace Microsoft.VisualBasic
