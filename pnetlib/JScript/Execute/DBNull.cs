/*
 * DBNull.cs - Replacement for "System.DBNull" on ECMA systems.
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

// This is a non-standard JScript class, but is needed to work around
// the missing "System.DBNull" class in ECMA-compatibility modes.

public sealed class DBNull
{
	// The only DBNull object in the system.
#if ECMA_COMPAT
	public readonly static DBNull Value = new DBNull();
#else
	public readonly static System.DBNull Value = System.DBNull.Value;
#endif

	// Constructors.
	private DBNull() {}

	// Override inherited methods.
	public override String ToString() { return String.Empty; }

	// Determine if a value is DBNull.
	internal static bool IsDBNull(Object value)
			{
				return (value == Value);
			}

}; // class DBNull

}; // namespace Microsoft.JScript
