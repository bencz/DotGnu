/*
 * DefaultCharSetAttribute.cs - Implementation of the
 *		"System.Runtime.InteropServices.DefaultCharSetAttribute" class.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2

[AttributeUsage(AttributeTargets.Module, Inherited=false)]
public sealed class DefaultCharSetAttribute : Attribute
{
	// Internal state.
	private CharSet charSet;

	// Constructors.
	public DefaultCharSetAttribute(CharSet charSet)
			{
				this.charSet = charSet;
			}

	// Get this attribute's properties.
	public CharSet CharSet
			{
				get
				{
					return charSet;
				}
			}

}; // class DefaultCharSetAttribute

#endif // CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2

}; // namespace System.Runtime.InteropServices
