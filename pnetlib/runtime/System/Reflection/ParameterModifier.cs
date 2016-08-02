/*
 * ParameterModifier.cs - Implementation of the
 *		"System.Reflection.ParameterModifier" class.
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

namespace System.Reflection
{

#if CONFIG_REFLECTION

public struct ParameterModifier
{
	// Internal state.
	private bool[] flags;

	// Constructor.
	public ParameterModifier(int parameterCount)
			{
				if(parameterCount < 0)
				{
					throw new ArgumentException(_("ArgRange_NonNegative"));
				}
				flags = new bool [parameterCount];
			}

	// Get or set a parameter modifier flag.
	public bool this [int index]
			{
				get
				{
					return flags[index];
				}
				set
				{
					flags[index] = value;
				}
			}

}; // class ParameterModifier

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
