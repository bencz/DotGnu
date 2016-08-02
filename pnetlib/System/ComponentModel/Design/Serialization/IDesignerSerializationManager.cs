/*
 * IDesignerSerializationManager.cs - Implementation of
 *	"System.ComponentModel.Design.Serialization.IDesignerSerializationManager".
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

namespace System.ComponentModel.Design.Serialization
{

#if CONFIG_COMPONENT_MODEL_DESIGN

using System.Collections;

public interface IDesignerSerializationManager : IServiceProvider
{
	// Get the context stack.
	ContextStack Context { get; }

	// Get the custom properties collection.
	PropertyDescriptorCollection Properties { get; }

	// Add a serialization provider to this manager.
	void AddSerializationProvider(IDesignerSerializationProvider provider);

	// Create an instance of a particular type.
	Object CreateInstance(Type type, ICollection arguments,
		     			  String name, bool addToContainer);

	// Get an object instance with a specified name.
	Object GetInstance(String name);

	// Get the name associated with a specified object.
	String GetName(Object value);

	// Get a serializer of a specific type.
	Object GetSerializer(Type objectType, Type serializerType);

	// Get a type with a specific name.
	Type GetType(String typeName);

	// Remove a serialization provider from this manager.
	void RemoveSerializationProvider(IDesignerSerializationProvider provider);

	// Report a serialization error.
	void ReportError(Object errorInformation);

	// Set the name of an existing object.
	void SetName(Object instance, String name);

	// Event that is emitted when a name is resolved.
	event ResolveNameEventHandler ResolveName;

	// Event that is emitted when serialization has completed.
	event EventHandler SerializationComplete;

}; // interface IDesignerSerializationManager

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design.Serialization
