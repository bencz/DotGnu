/*
 * SingleType.cs - Implementation of the
 *			"Microsoft.VisualBasic.SingleType" class.
 *
 * Copyright (C) 2003, 2004  Southern Storm Software, Pty Ltd.
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

namespace Microsoft.VisualBasic.CompilerServices
{

using System;
using System.ComponentModel;
using System.Globalization;

[StandardModule]
#if CONFIG_COMPONENT_MODEL
[EditorBrowsable(EditorBrowsableState.Never)]
#endif
public sealed class SingleType
{
	// This class cannot be instantiated.
	private SingleType() {}

	// Convert an object into a float value.
	public static float FromObject(Object Value)
			{
				return FromObject(Value, null);
			}
	public static float FromObject
				(Object Value, NumberFormatInfo NumberFormat)
			{
			#if !ECMA_COMPAT
				if(Value != null)
				{
					IConvertible ic = (Value as IConvertible);
					if(ic != null)
					{
						if(ic.GetTypeCode() != TypeCode.String)
						{
							return ic.ToSingle(NumberFormat);
						}
						else
						{
							return FromString(ic.ToString(null), NumberFormat);
						}
					}
					else
					{
						throw new InvalidCastException
							(String.Format
								(S._("VB_InvalidCast"),
								 Value.GetType(), "System.Single"));
					}
				}
				else
				{
					return 0.0f;
				}
			#else
				return (float)(DoubleType.FromObject(Value, NumberFormat));
			#endif
			}

	// Convert a string into a float value.
	public static float FromString(String Value)
			{
				return FromString(Value, null);
			}
	public static float FromString
				(String Value, NumberFormatInfo NumberFormat)
			{
				return (float)(DoubleType.FromString(Value, NumberFormat));
			}

}; // class SingleType

}; // namespace Microsoft.VisualBasic.CompilerServices
