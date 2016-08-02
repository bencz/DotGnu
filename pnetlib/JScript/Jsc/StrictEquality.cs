/*
 * StrictEquality.cs - Strict equality operator.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.JScript
{

using System;
using System.Reflection;

// Dummy class for backwards-compatibility.

public sealed class StrictEquality : BinaryOp
{
	// Constructor.
	internal StrictEquality() : base((int)(JSToken.StrictEqual)) {}

	// Determine if two JScript objects are strictly equal.
	public static bool JScriptStrictEquals(Object v1, Object v2)
			{
				// Handle the simple cases first.
				if(v1 is String && v2 is String)
				{
					return ((String)v1) == ((String)v2);
				}
				else if(v1 is double && v2 is double)
				{
					return ((double)v1) == ((double)v2);
				}
				else if(v1 is int && v2 is int)
				{
					return ((int)v1) == ((int)v2);
				}
				else if(v1 is bool && v2 is bool)
				{
					return ((bool)v1) == ((bool)v2);
				}
				else if(v1 == v2)
				{
					return true;
				}

				// Handle the case where one is null or undefined.
				if(v1 == null || v1 is Missing
			#if ECMA_COMPAT
				  )
			#else
				   || v1 is System.Reflection.Missing)
			#endif
				{
					return (v2 == null || v2 is Missing
				#if ECMA_COMPAT
						   );
				#else
							|| v2 is System.Reflection.Missing);
				#endif
				}
				else if(v2 == null || v2 is Missing
			#if ECMA_COMPAT
					   )
			#else
						|| v2 is System.Reflection.Missing)
			#endif
				{
					return false;
				}
				if(DBNull.IsDBNull(v1))
				{
					return DBNull.IsDBNull(v1);
				}
				else if(DBNull.IsDBNull(v2))
				{
					return false;
				}

				// Normalize primitive values and retry.
				v1 = Convert.NormalizePrimitive(v1);
				v2 = Convert.NormalizePrimitive(v2);
				TypeCode tc1 = Support.TypeCodeForObject(v1);
				TypeCode tc2 = Support.TypeCodeForObject(v2);
				if(tc1 != tc2)
				{
					return false;
				}
				switch(tc1)
				{
					case TypeCode.Boolean:
						return ((bool)v1) == ((bool)v2);

					case TypeCode.Double:
						return ((double)v1) == ((double)v2);

					case TypeCode.DateTime:
						return ((DateTime)v1) == ((DateTime)v2);

					case TypeCode.Decimal:
						return ((Decimal)v1) == ((Decimal)v2);

					case TypeCode.String:
						return ((String)v1) == ((String)v2);

					default: break;
				}

				// The values are not strictly equal.
				return false;
			}

}; // class StrictEquality

}; // namespace Microsoft.JScript
