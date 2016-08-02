/*
 * DateType.cs - Implementation of the
 *			"Microsoft.VisualBasic.DateType" class.
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

namespace Microsoft.VisualBasic.CompilerServices
{

using System;
using System.ComponentModel;
using System.Globalization;

[StandardModule]
#if CONFIG_COMPONENT_MODEL
[EditorBrowsable(EditorBrowsableState.Never)]
#endif
public sealed class DateType
{
	// This class cannot be instantiated.
	private DateType() {}

	// Convert an object into a date value.
	public static DateTime FromObject(Object Value)
			{
			#if !ECMA_COMPAT
				IConvertible ic = (Value as IConvertible);
				if(ic != null)
				{
					if(ic.GetTypeCode() != TypeCode.String)
					{
						return ic.ToDateTime(null);
					}
					else
					{
						return FromString(ic.ToString(null));
					}
				}
			#else
				if(Value is DateTime)
				{
					return (DateTime)Value;
				}
			#endif
				throw new InvalidCastException
					(String.Format
						(S._("VB_InvalidCast"),
						 (Value != null ? Value.GetType().ToString() : "null"),
						 "System.DateTime"));
			}

	// Convert a string into a date value.
	public static DateTime FromString(String Value)
			{
				return FromString(Value, CultureInfo.CurrentCulture);
			}
	public static DateTime FromString(String Value, CultureInfo culture)
			{
				try
				{
					return DateTime.Parse(Utils.FixDigits(Value), culture,
										  DateTimeStyles.AllowWhiteSpaces |
										  DateTimeStyles.NoCurrentDateDefault);
				}
				catch(Exception)
				{
					throw new InvalidCastException
						(String.Format
							(S._("VB_InvalidCast"),
						 	 (Value != null ? Value.GetType().ToString()
							 				: "null"),
							 "System.DateTime"));
				}
			}

}; // class DateType

}; // namespace Microsoft.VisualBasic.CompilerServices
