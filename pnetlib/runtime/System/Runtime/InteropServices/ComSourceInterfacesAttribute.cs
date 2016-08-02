/*
 * ComSourceInterfacesAttribute.cs - Implementation of the
 *			"System.Runtime.InteropServices.ComSourceInterfacesAttribute" class.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_COM_INTEROP

[AttributeUsage(AttributeTargets.Class, Inherited=false)]
public sealed class ComSourceInterfacesAttribute : Attribute
{
	// Internal state.
	private String value;

	// Constructors.
	public ComSourceInterfacesAttribute(String sourceInterfaces)
			{
				this.value = sourceInterfaces;
			}
	public ComSourceInterfacesAttribute(Type sourceInterface)
			{
				this.value = sourceInterface.ToString();
			}
	public ComSourceInterfacesAttribute(Type sourceInterface1,
										Type sourceInterface2)
			{
				this.value = sourceInterface1.ToString() + "," +
							 sourceInterface2.ToString();
			}
	public ComSourceInterfacesAttribute(Type sourceInterface1,
										Type sourceInterface2,
										Type sourceInterface3)
			{
				this.value = sourceInterface1.ToString() + "," +
							 sourceInterface2.ToString() + "," +
							 sourceInterface3.ToString();
			}
	public ComSourceInterfacesAttribute(Type sourceInterface1,
										Type sourceInterface2,
										Type sourceInterface3,
										Type sourceInterface4)
			{
				this.value = sourceInterface1.ToString() + "," +
							 sourceInterface2.ToString() + "," +
							 sourceInterface3.ToString() + "," +
							 sourceInterface4.ToString();
			}

	// Fetch the attribute's value.
	public String Value
			{
				get
				{
					return value;
				}
			}

}; // class ComSourceInterfacesAttribute

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
