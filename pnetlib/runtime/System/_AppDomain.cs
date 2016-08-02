/*
 * _AppDomain.cs - Implementation of the "System._AppDomain" interface.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

namespace System
{

#if CONFIG_RUNTIME_INFRA

using System.Reflection;
using System.Reflection.Emit;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Policy;
using System.Security.Principal;

#if ECMA_COMPAT
[CLSCompliant(false)]
internal
#else
[CLSCompliant(false)]
[Guid("05F696DC-2B29-3663-AD8B-C4389CF2A713")]
#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsDual)]
#endif
public
#endif
interface _AppDomain
{
	// Get the friendly name associated with this application domain.
	String FriendlyName { get; }

	// Event that is emitted when an assembly is loaded into this domain.
	event AssemblyLoadEventHandler AssemblyLoad;

	// Event that is emitted when an application domain is unloaded.
	event EventHandler DomainUnload;

	// Event that is emitted when an exception is unhandled by the domain.
	event UnhandledExceptionEventHandler UnhandledException;

#if !ECMA_COMPAT

	// Base directory used to resolve assemblies.
	String BaseDirectory { get; }

	// Base directory used to resolve dynamically-created assemblies.
	String DynamicDirectory { get; }

	// Get the security evidence for this application domain.
	Evidence Evidence { get; }

	// Search path, relative to "BaseDirectory", for private assemblies.
	String RelativeSearchPath { get; }

	// Determine if the assemblies in the application domain are shadow copies.
	bool ShadowCopyFiles { get; }

	// Append a directory to the private path.
	void AppendPrivatePath(String path);

	// Clear the private path.
	void ClearPrivatePath();

	// Clear the shadow copy path.
	void ClearShadowCopyPath();

#if CONFIG_REFLECTION_EMIT

	// Define a dynamic assembly within this application domain.
	AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access);
	AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access,
				 Evidence evidence);
	AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access,
				 String dir);
	AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access,
				 String dir, Evidence evidence);
	AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access,
				 PermissionSet requiredPermissions,
				 PermissionSet optionalPermissions,
				 PermissionSet refusedPersmissions);
	AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access,
				 Evidence evidence, PermissionSet requiredPermissions,
				 PermissionSet optionalPermissions,
				 PermissionSet refusedPersmissions);
	AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access,
				 String dir, PermissionSet requiredPermissions,
				 PermissionSet optionalPermissions,
				 PermissionSet refusedPersmissions);
	AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access,
				 String dir, Evidence evidence,
				 PermissionSet requiredPermissions,
				 PermissionSet optionalPermissions,
				 PermissionSet refusedPersmissions);
	AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access,
				 String dir, Evidence evidence,
				 PermissionSet requiredPermissions,
				 PermissionSet optionalPermissions,
				 PermissionSet refusedPersmissions,
				 bool isSynchronized);

#endif // CONFIG_REFLECTION_EMIT

	// Execute a particular assembly within this application domain.
	int ExecuteAssembly(String assemblyFile);
	int ExecuteAssembly(String assemblyFile, Evidence assemblySecurity);
	int ExecuteAssembly(String assemblyFile, Evidence assemblySecurity,
						String[] args);

	// Get a list of all assemblies in this application domain.
	Assembly[] GetAssemblies();

	// Fetch the object associated with a particular data name.
	Object GetData(String name);

	// Load an assembly into this application domain by name.
	Assembly Load(AssemblyName assemblyRef);
	Assembly Load(AssemblyName assemblyRef, Evidence assemblySecurity);

	// Load an assembly into this application domain by string name.
	Assembly Load(String assemblyString);
	Assembly Load(String assemblyString, Evidence assemblySecurity);

	// Load an assembly into this application domain by explicit definition.
	Assembly Load(byte[] rawAssembly);
	Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore);
	Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore,
				  Evidence assemblySecurity);

#if CONFIG_POLICY_OBJECTS

	// Set policy information for this application domain.
	void SetAppDomainPolicy(PolicyLevel domainPolicy);

	// Set the policy for principals.
	void SetPrincipalPolicy(PrincipalPolicy policy);

	// Set the default principal object for a thread.
	void SetThreadPrincipal(IPrincipal principal);

#endif

	// Set the cache location for shadow copied assemblies.
	void SetCachePath(String s);

	// Set a data item on this application domain.
	void SetData(String name, Object data);

	// Set the location of the shadow copy directory.
	void SetShadowCopyPath(String s);

	// Methods that are normally in System.Object, but which must be
	// redeclared here for some unknown reason.
	bool Equals(Object obj);
	int GetHashCode();
	Type GetType();
	String ToString();

	// Event that is emitted when an assembly fails to resolve.
	event ResolveEventHandler AssemblyResolve;

	// Event that is emitted when a process exits within this domain.
	event EventHandler ProcessExit;

	// Event that is emitted when a resource fails to resolve.
	event ResolveEventHandler ResourceResolve;

	// Event that is emitted when a type fails to resolve.
	event ResolveEventHandler TypeResolve;

#endif // !ECMA_COMPAT

#if CONFIG_REMOTING

	// Create an instance of a type within this application domain.
	ObjectHandle CreateInstance(String assemblyName, String typeName);
	ObjectHandle CreateInstance(String assemblyName, String typeName,
								Object[] activationAttributes);
	ObjectHandle CreateInstance(String assemblyName, String typeName,
								bool ignoreCase, BindingFlags bindingAttr,
								Binder binder, Object[] args,
								CultureInfo culture,
								Object[] activationAttributes,
								Evidence securityAttributes);

	// Create a remote instance of a type within this application domain.
	ObjectHandle CreateInstanceFrom(String assemblyFile, String typeName);
	ObjectHandle CreateInstanceFrom(String assemblyFile, String typeName,
								    Object[] activationAttributes);
	ObjectHandle CreateInstanceFrom(String assemblyFile, String typeName,
								    bool ignoreCase, BindingFlags bindingAttr,
								    Binder binder, Object[] args,
								    CultureInfo culture,
								    Object[] activationAttributes,
								    Evidence securityAttributes);

	// Execute a delegate in a foreign application domain.
	void DoCallBack(CrossAppDomainDelegate theDelegate);

	// Get an object for controlling the lifetime service.
	Object GetLifetimeService();

	// Give the application domain an infinite lifetime service.
	Object InitializeLifetimeService();

#endif // CONFIG_REMOTING

}; // interface _AppDomain

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System
