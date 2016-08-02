/*
 * DispIdAttribute.cs - Implementation of the
 *			"System.Runtime.InteropServices.DispIdAttribute" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

#if !ECMA_COMPAT

// Needed for Compact .NET Framework compatibility, so not CONFIG_COM_INTEROP.

[AttributeUsage(AttributeTargets.Method |
				AttributeTargets.Field |
				AttributeTargets.Property |
				AttributeTargets.Event,
				Inherited=false)]
public sealed class DispIdAttribute : Attribute
{
	// Internal state.
	private int dispId;

	// Constructor.
	public DispIdAttribute(int dispId)
			{
				this.dispId = dispId;
			}

	// Get this attribute's value.
	public int Value
			{
				get
				{
					return dispId;
				}
			}

}; // class DispIdAttribute

#endif // !ECMA_COMPAT

}; // namespace System.Runtime.InteropServices
