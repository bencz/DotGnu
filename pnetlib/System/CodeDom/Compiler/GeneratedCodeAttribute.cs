/*
 * GeneratedCodeAttribute.cs - Implementation of the
 *			"System.CodeDom.Compiler.GeneratedCodeAttribute" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.CodeDom.Compiler
{

#if CONFIG_FRAMEWORK_2_0 && !ECMA_COMPAT

[AttributeUsage(AttributeTargets.All,
			    Inherited=false,
				AllowMultiple=false)]
public sealed class GeneratedCodeAttribute : Attribute
{

	// Internal state.
	private String tool;
	private String version;

	// Constructors.
	public GeneratedCodeAttribute(String tool, String version)
			{
				this.tool = tool;
				this.version = version;
			}

	// Get the attribute's value.
	public String Tool
			{
				get
				{
					return tool;
				}
			}

	public String Version
			{
				get
				{
					return version;
				}
			}
}; // class GeneratedCodeAttribute

#endif // CONFIG_FRAMEWORK_2_0 && !ECMA_COMPAT

}; // namespace System.CodeDom.Compiler
