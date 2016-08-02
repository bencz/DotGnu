/*
 * RegistrationServices.cs - Implementation of the
 *			"System.Runtime.InteropServices.RegistrationServices" class.
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

using System.Reflection;

// We don't support COM registration in this implementation.

[ClassInterface(ClassInterfaceType.None)]
[Guid("475E398F-8AFA-43a7-A3BE-F4EF8D6787C9")]
public class RegistrationServices : IRegistrationServices
{

	// Constructor.
	public RegistrationServices() {}

	// Get the GUID of the managed category.
	public virtual Guid GetManagedCategoryGuid()
			{
				throw new NotImplementedException();
			}

	// Get the program identifier for a specific type.
	public virtual String GetProgIdForType(Type type)
			{
				throw new NotImplementedException();
			}

	// Get a list of types that can be registered.
	public virtual Type[] GetRegistrableTypesInAssembly(Assembly assembly)
			{
				throw new NotImplementedException();
			}

	// Register the types in a particular assembly.
	public virtual bool RegisterAssembly
				(Assembly assembly, AssemblyRegistrationFlags flags)
			{
				throw new NotImplementedException();
			}

	// Register a particular type for COM clients.
	public virtual void RegisterTypeForComClients(Type type, ref Guid g)
			{
				throw new NotImplementedException();
			}

	// Determine if a particular type is a COM type.
	public virtual bool TypeRepresentsComType(Type type)
			{
				throw new NotImplementedException();
			}

	// Determine if a particular type requires registration.
	public virtual bool TypeRequiresRegistration(Type type)
			{
				throw new NotImplementedException();
			}

	// Unregister the types in a particular assembly.
	public virtual bool UnregisterAssembly(Assembly assembly)
			{
				throw new NotImplementedException();
			}

}; // class RegistrationServices

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
