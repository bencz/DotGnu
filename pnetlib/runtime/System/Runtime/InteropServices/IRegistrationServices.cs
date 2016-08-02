/*
 * IRegistrationServices.cs - Implementation of the
 *			"System.Runtime.InteropServices.IRegistrationServices" class.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_COM_INTEROP

using System.Reflection;

[Guid("CCBD682C-73A5-4568-B8B0-C7007E11ABA2")]
public interface IRegistrationServices
{

	// Get the GUID of the managed category.
	Guid GetManagedCategoryGuid();

	// Get the program identifier for a specific type.
	String GetProgIdForType(Type type);

	// Get a list of types that can be registered.
	Type[] GetRegistrableTypesInAssembly(Assembly assembly);

	// Register the types in a particular assembly.
	bool RegisterAssembly(Assembly assembly, AssemblyRegistrationFlags flags);

	// Register a particular type for COM clients.
	void RegisterTypeForComClients(Type type, ref Guid g);

	// Determine if a particular type is a COM type.
	bool TypeRepresentsComType(Type type);

	// Determine if a particular type requires registration.
	bool TypeRequiresRegistration(Type type);

	// Unregister the types in a particular assembly.
	bool UnregisterAssembly(Assembly assembly);

}; // interface IRegistrationServices

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
