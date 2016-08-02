/*
 * NotRecommended.cs - Mark a construct as not recommended for use.
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

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
public class NotRecommended : Attribute
{
	// Internal state.
	private String message;

	// Constructor.
	public NotRecommended(String message)
			{
				this.message = message;
			}

	// Determine if this attribute is an error indication.
	public bool IsError
			{
				get
				{
					return false;
				}
			}

	// Get the message.
	public String Message
			{
				get
				{
					return message;
				}
			}

}; // class NotRecommended

}; // namespace Microsoft.JScript
