/*
 * DecimalConstantAttribute.cs - Implementation of the
 *		"System.Runtime.CompilerServices.DecimalConstantAttribute" class.
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

namespace System.Runtime.CompilerServices
{

#if CONFIG_EXTENDED_NUMERICS

[CLSCompliant(false)]
#if CONFIG_FRAMEWORK_2_0
[AttributeUsage(AttributeTargets.Field |
				AttributeTargets.Parameter,
				AllowMultiple=false,
				Inherited=false)]
#else
[AttributeUsage(AttributeTargets.Field |
				AttributeTargets.Parameter, Inherited=false)]
#endif
public sealed class DecimalConstantAttribute : Attribute
{

	// Internal state.
	private Decimal value;

	// Constructors.
	public DecimalConstantAttribute(byte scale, byte sign,
									uint hi, uint mid, uint low)
			{
				unchecked
				{
					value = new Decimal((int)low, (int)mid, (int)hi,
										(sign != 0), scale);
				}
			}

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

	public DecimalConstantAttribute(byte scale, byte sign,
									int hi, int mid, int low)
			{
				value = new Decimal(low, mid, hi, (sign != 0), scale);
			}

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

	// Properties.
	public Decimal Value
			{
				get
				{
					return value;
				}
			}

}; // class DecimalConstantAttribute

#endif // CONFIG_EXTENDED_NUMERICS

}; // namespace System.Runtime.CompilerServices
