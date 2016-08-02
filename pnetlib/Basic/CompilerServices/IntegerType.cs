/*
 * IntegerType.cs - Implementation of the
 *			"Microsoft.VisualBasic.IntegerType" class.
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

[StandardModule]
#if CONFIG_COMPONENT_MODEL
[EditorBrowsable(EditorBrowsableState.Never)]
#endif
public sealed class IntegerType
{
	// This class cannot be instantiated.
	private IntegerType() {}

	// Convert an object into an integer value.
	public static int FromObject(Object Value)
			{
			#if !ECMA_COMPAT
				if(Value != null)
				{
					IConvertible ic = (Value as IConvertible);
					if(ic != null)
					{
						if(ic.GetTypeCode() != TypeCode.String)
						{
							return ic.ToInt32(null);
						}
						else
						{
							return FromString(ic.ToString(null));
						}
					}
					else
					{
						throw new InvalidCastException
							(String.Format
								(S._("VB_InvalidCast"),
								 Value.GetType(), "System.ToInt32"));
					}
				}
				else
				{
					return 0;
				}
			#else
				return checked((int)(LongType.FromObject(Value)));
			#endif
			}

	// Convert a string into an integer value.
	public static int FromString(String Value)
			{
				if(Value != null)
				{
					try
					{
						long lvalue;
						if(LongType.TryConvertHexOct(Value, out lvalue))
						{
							return checked((int)lvalue);
						}
						return Convert.ToInt32
							(Math.Round(DoubleType.Parse(Value)));
					}
					catch(OverflowException)
					{
						throw new InvalidCastException
							(String.Format
								(S._("VB_InvalidCast"),
								 "System.String", "System.Int32"));
					}
				}
				else
				{
					return 0;
				}
			}

}; // class IntegerType

}; // namespace Microsoft.VisualBasic.CompilerServices
