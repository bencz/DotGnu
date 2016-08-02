/*
 * AssemblyFlagsAttribute.cs - Implementation of the
 *			"System.Reflection.AssemblyFlagsAttribute" class.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

#if !ECMA_COMPAT

using System;
using System.Configuration.Assemblies;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=false)]
public sealed class AssemblyFlagsAttribute : Attribute
{

	// Internal state.
	private uint flagValues;

	// Constructors.
	public AssemblyFlagsAttribute(int flags)
			: base()
			{
				flagValues = (uint)flags;
			}
	[CLSCompliant(false)]
	public AssemblyFlagsAttribute(uint flags)
			: base()
			{
				flagValues = flags;
			}

#if CONFIG_FRAMEWORK_2_0
	public AssemblyFlagsAttribute(AssemblyNameFlags flags)
			: base()
			{
				flagValues = (uint)flags;
			}
#endif // CONFIG_FRAMEWORK_2_0

	// Properties.
	public int AssemblyFlags
			{
				get
				{
					return (int)flagValues;
				}
			}
	[CLSCompliant(false)]
	public uint Flags
			{
				get
				{
					return flagValues;
				}
			}

}; // class AssemblyFlagsAttribute

#endif // !ECMA_COMPAT

}; // namespace System.Reflection
