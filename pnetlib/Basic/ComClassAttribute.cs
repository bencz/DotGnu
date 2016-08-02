/*
 * ComClassAttribute.cs - Implementation of the
 *			"Microsoft.VisualBasic.ComClassAttribute" class.
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

namespace Microsoft.VisualBasic
{

using System;

[AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
public sealed class ComClassAttribute : Attribute
{
	// Internal state.
	private String classID;
	private String interfaceID;
	private String eventID;
	private bool interfaceShadows;

	// Constructors.
	public ComClassAttribute() {}
	public ComClassAttribute(String _ClassID)
			{
				this.classID = _ClassID;
			}
	public ComClassAttribute(String _ClassID, String _InterfaceID)
			{
				this.classID = _ClassID;
				this.interfaceID = _InterfaceID;
			}
	public ComClassAttribute(String _ClassID, String _InterfaceID,
							 String _EventID)
			{
				this.classID = _ClassID;
				this.interfaceID = _InterfaceID;
				this.eventID = _EventID;
			}

	// Get this attribute's values.
	public String ClassID
			{
				get
				{
					return classID;
				}
			}
	public String InterfaceID
			{
				get
				{
					return interfaceID;
				}
			}
	public String EventID
			{
				get
				{
					return eventID;
				}
			}
	public bool InterfaceShadows
			{
				get
				{
					return interfaceShadows;
				}
				set
				{
					interfaceShadows = value;
				}
			}

}; // class ComClassAttribute

}; // namespace Microsoft.VisualBasic
