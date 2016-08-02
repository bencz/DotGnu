/*
 * IReferenceService.cs - Implementation of the
 *		"System.ComponentModel.Design.IReferenceService" class.
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

namespace System.ComponentModel.Design
{

#if CONFIG_COMPONENT_MODEL

public interface IReferenceService
{
	// Get the component for a specific reference.
	IComponent GetComponent(Object reference);

	// Get the component name for a specific reference.
	String GetName(Object reference);

	// Get a reference to a named component.
	Object GetReference(String name);

	// Get references for all project components.
	Object[] GetReferences();

	// Get references for all project components of a specific type.
	Object[] GetReferences(Type baseType);

}; // interface IReferenceService

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel.Design
