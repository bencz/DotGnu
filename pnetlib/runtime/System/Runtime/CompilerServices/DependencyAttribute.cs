/*
 * DependencyAttribute.cs - Implementation of the
 *	"System.Runtime.CompilerServices.DependencyAttribute" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

using System.Runtime.InteropServices;

#if CONFIG_FRAMEWORK_2_0

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
public sealed class DependencyAttribute : Attribute
{
	// Internal state.
	private String dependentAssembly;
	private LoadHint loadHint;

	// Constructors.
	public DependencyAttribute(String dependentAssemblyAttribute,
							   LoadHint loadHintArgument)
			{
				this.dependentAssembly = dependentAssemblyAttribute;
				this.loadHint = loadHintArgument;
			}

	// Get this attribute's properties.
	public String DependentAssembly
			{
				get
				{
					return dependentAssembly;
				}
			}
	public LoadHint LoadHint
			{
				get
				{
					return loadHint;
				}
			}

}; // class DependencyAttribute

#endif // CONFIG_FRAMEWORK_2_0

}; // namespace System.Runtime.CompilerServices
