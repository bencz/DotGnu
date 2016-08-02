/*
 * IMessageDictionary.cs - Implementation of the
 *			"System.Runtime.Remoting.Messaging.IMessageDictionary" class.
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

namespace System.Runtime.Remoting.Messaging
{

#if CONFIG_SERIALIZATION

internal interface IMessageDictionary
{
	// Get the list of special property values.
	String[] SpecialProperties { get; }

	// Get the value of a special property.
	Object GetSpecialProperty(String name);

	// Set the value of a special property.
	void SetSpecialProperty(String name, Object value);

}; // interface IMessageDictionary

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Messaging
