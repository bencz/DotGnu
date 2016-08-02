/*
 * AppDomain.cs - Implementation of the "System.AppDomain" class.
 *
 * Copyright (C) 2001, 2002, 2003, 2008  Southern Storm Software, Pty Ltd.
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

using System.Security;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Globalization;
using System.Reflection.Emit;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Security.Principal;
using System.Configuration.Assemblies;
using System.Threading;

#if CONFIG_RUNTIME_INFRA

#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.None)]
#endif
public sealed class AppDomain
	: MarshalByRefObject, _AppDomain
#if !ECMA_COMPAT
	, IEvidenceFactory
#endif
{
	// Internal state.
	// Pointer to the ILExecProcess this AppDomain represents
	// This is an IntPtr but has to be an object reference to be
	// scanned by the gc to prevent the appdomain from being destroyed.
	// and deallocated while an AppDomain referencing it still exists.
	private Object appDomain;
	internal int domainID;
#if !ECMA_COMPAT
	private Evidence evidence;
	private AppDomainSetup setup;
	private Hashtable items;
#endif
	private static AppDomain currentDomain;
#if CONFIG_REMOTING
	internal Context defaultContext;
	internal LifetimeServices.Manager lifetimeManager;
#endif

	// InternalCalls
	// Append the Paths to the private paths for the appDomain
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void AppendPrivatePathsInternal(Object appDomain, String[] paths);

	// Clear the search paths for private assemblies
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void ClearPrivatePathInternal(Object appDomain);

	// Clear the paths which assemblies should be shadow copied
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void ClearShadowCopyPathInternal(Object appDomain);

	// Create a new instance of ILExecProcess
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void CreateAppDomain(ref Object appDomain);

	// Get the current instance of ILExecProcess
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void CurrentAppDomain(ref Object appDomain);

	// Get the base directory for this application domain.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static String GetBaseDirectoryInternal(Object appDomain);

	// Get the id of this application domain.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int GetIdInternal(Object appDomain);

	// Get the friendly name associated with this application domain.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static String GetFriendlyNameInternal(Object appDomain);

	// Get the search path relative to the application base for this application domain.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static String GetRelativeSearchPathInternal(Object appDomain);

	// Get the flag if files should be shadow copied.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool GetShadowCopyFilesInternal(Object appDomain);

	// Check if this is the default AppDomain
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool IsDefaultAppDomainInternal(Object appDomain);

	// Set the base directory for this application domain.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void SetBaseDirectoryInternal(Object appDomain, String baseDirectory);

	// Set the friendly name associated with this application domain.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void SetFriendlyNameInternal(Object appDomain, String friendlyName);

	// Set the search path relative to the application base for this application domain.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void SetRelativeSearchPathInternal(Object appDomain, String appRelativePath);

	// Set the flag if files should be shadow copied.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void SetShadowCopyFilesInternal(Object appDomain, bool shadowCopyFiles);

	// Set the paths which assemblies should be shadow copied for the appDomain
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void SetShadowCopyPathInternal(Object appDomain, String[] paths);

	// Unload the instance of ILExecProcess
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void UnloadAppDomain(Object appDomain);


	// Constructor for the CurrentDomain
	private AppDomain(Object appDomain)
	{
		domainID = GetIdInternal(appDomain);
		this.appDomain = appDomain;
#if !ECMA_COMPAT
				setup = new AppDomainSetup(appDomain);
				items = new Hashtable();
				this.evidence = new Evidence();
#endif
	}

	// Construct a new AppDomain instance.
	private AppDomain(String name, Object appDomain)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				domainID = GetIdInternal(appDomain);
				this.appDomain = appDomain;
				SetFriendlyNameInternal(appDomain, name);
#if !ECMA_COMPAT
				setup = new AppDomainSetup(appDomain);
				items = new Hashtable();
#endif
			}
#if !ECMA_COMPAT
	private AppDomain(String name, Evidence evidence, AppDomainSetup setup, Object appDomain)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				this.appDomain = appDomain;
				SetFriendlyNameInternal(appDomain, name);
				domainID = GetIdInternal(appDomain);
				this.evidence = evidence;
				this.setup = setup;
				this.items = new Hashtable();
			}
#endif

	// Create a new application domain with a specified name.
	public static AppDomain CreateDomain(String friendlyName)
			{
				Object appDomain = null;

				CreateAppDomain(ref appDomain);
				return new AppDomain(friendlyName, appDomain);
			}
#if !ECMA_COMPAT
	public static AppDomain CreateDomain(String friendlyName,
										 Evidence securityInfo)
			{
				Object appDomain = null;

				CreateAppDomain(ref appDomain);
				return new AppDomain(friendlyName, securityInfo,
									 new AppDomainSetup(appDomain),
									 appDomain);
			}
	public static AppDomain CreateDomain(String friendlyName,
										 Evidence securityInfo,
										 AppDomainSetup info)
			{
				Object appDomain = null;

				CreateAppDomain(ref appDomain);
				return new AppDomain(friendlyName, securityInfo, info, appDomain);
			}
	public static AppDomain CreateDomain(String friendlyName,
										 Evidence securityInfo,
										 String appBasePath,
										 String appRelativeSearchPath,
										 bool shadowCopyFiles)
			{
				Object appDomain = null;

				CreateAppDomain(ref appDomain);
				SetBaseDirectoryInternal(appDomain, appBasePath);
				SetRelativeSearchPathInternal(appDomain, appRelativeSearchPath);
				SetShadowCopyFilesInternal(appDomain, shadowCopyFiles);
				return new AppDomain(friendlyName, securityInfo,
									 new AppDomainSetup(appDomain),
									 appDomain);
			}
#endif

	// Return a string representing the current instance.
	public override String ToString()
			{
				return FriendlyName;
			}

	// Unload a specific application domain.
	public static void Unload(AppDomain domain)
			{
				if(domain == null)
				{
					throw new ArgumentNullException("domain");
				}
				// We must not unload the default AppDomain
				if (IsDefaultAppDomainInternal(domain.appDomain))
				{
					throw new CannotUnloadAppDomainException();
				}
				if (domain.DomainUnload != null)
				{
					domain.DomainUnload(domain, new EventArgs());
				}
				UnloadAppDomain(domain.appDomain);
			}

	// Get the friendly name associated with this application domain.
	public String FriendlyName
			{
				get
				{
					String name = GetFriendlyNameInternal(appDomain);
					if(name == null)
					{
						return String.Empty;
					}
					return name;
				}
			}

	// Event that is emitted when an assembly is loaded into this domain.
	public event AssemblyLoadEventHandler AssemblyLoad;

	// Event that is emitted when an application domain is unloaded.
	public event EventHandler DomainUnload;

	// Event that is emitted when an exception is unhandled by the domain.
	public event UnhandledExceptionEventHandler UnhandledException;

#if !ECMA_COMPAT

	// Create the setup information block for the current domain.
	private static AppDomainSetup CreateCurrentSetup(IntPtr appDomain)
			{
				AppDomainSetup setup = new AppDomainSetup(appDomain);

				// Get the location information for the assembly
				// that contains the program entry point.
			#if CONFIG_RUNTIME_INFRA
				Assembly entry = Assembly.GetEntryAssembly();
				String location = entry.Location;
				if(location != null && location != String.Empty)
				{
					// "ApplicationBase" is the directory containing
					// the application, ending with a directory separator.
					String dir = Path.GetDirectoryName(location);
					if(dir != null && dir.Length > 0 &&
					   !Path.IsSeparator(dir[dir.Length - 1]))
					{
						dir += Path.DirectorySeparatorChar;
					}
					setup.ApplicationBase = dir;

					// The configuration file is the name of the entry
					// assembly with ".config" added to the end.
					setup.ConfigurationFile = location + ".config";
				}
			#endif

				return setup;
			}

#endif

	// Get the current domain.
#if ECMA_COMPAT
	internal
#else
	public
#endif
	static AppDomain CurrentDomain
			{
				get
				{
					if(currentDomain != null)
					{
						return currentDomain;
					}
					lock(typeof(AppDomain))
					{
						if(currentDomain == null)
						{
							Object domain = null;
							CurrentAppDomain(ref domain);
							currentDomain = new AppDomain(domain);
						}
						return currentDomain;
					}
				}
			}

#if !ECMA_COMPAT

	// Base directory used to resolve assemblies.
	public String BaseDirectory
			{
				get
				{
					return GetBaseDirectoryInternal(appDomain);
				}
			}

	// Base directory used to resolve dynamically-created assemblies.
	public String DynamicDirectory
			{
				get
				{
					return setup.DynamicBase;
				}
			}

	// Get the security evidence for this application domain.
	public Evidence Evidence
			{
				get
				{
					return evidence;
				}
			}

	// Search path, relative to "BaseDirectory", for private assemblies.
	public String RelativeSearchPath
			{
				get
				{
					return GetRelativeSearchPathInternal(appDomain);
				}
			}

	// Determine if the assemblies in the application domain are shadow copies.
	public bool ShadowCopyFiles
			{
				get
				{
					return GetShadowCopyFilesInternal(appDomain);
				}
			}

	// Get the setup information for this application domain.
	public AppDomainSetup SetupInformation
			{
				get
				{
					return setup;
				}
			}

	// Append a directory to the private path.
#if CONFIG_FRAMEWORK_2_0
	[Obsolete()]
#endif
	public void AppendPrivatePath(String path)
			{
				if (path == null || path.Length == 0)
				{
					// Nothing to do
					return;
				}
				String[] splitPaths = path.Split(Path.PathSeparator);

				AppendPrivatePathsInternal(appDomain, splitPaths);
			}

	// Clear the private path.
#if CONFIG_FRAMEWORK_2_0
	[Obsolete()]
#endif
	public void ClearPrivatePath()
			{
				ClearPrivatePathInternal(appDomain);
			}

	// Clear the shadow copy path.
	public void ClearShadowCopyPath()
			{
				ClearShadowCopyPathInternal(appDomain);
			}

#endif // !ECMA_COMPAT

#if CONFIG_REMOTING

	// Create a COM object instance using the local activator logic.
	public ObjectHandle CreateComInstanceFrom
				(String assemblyName, String typeName)
			{
				return Activator.CreateComInstanceFrom
					(assemblyName, typeName);
			}
	public ObjectHandle CreateComInstanceFrom
				(String assemblyName, String typeName,
				 byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
			{
				return Activator.CreateComInstanceFrom
					(assemblyName, typeName, hashValue, hashAlgorithm);
			}

	// Create an instance of an assembly and unwrap its handle.
	public Object CreateInstanceAndUnwrap(String assemblyName, String typeName)
			{
				return CreateInstance(assemblyName, typeName).Unwrap();
			}
	public Object CreateInstanceAndUnwrap(String assemblyName,
						       			  String typeName,
						       			  Object[] activationAttributes)
			{
				return CreateInstance(assemblyName, typeName,
									  activationAttributes).Unwrap();
			}
	public Object CreateInstanceAndUnwrap(String assemblyName,
						       			  String typeName,
						       			  bool ignoreCase,
						       			  BindingFlags bindingAttr,
						       			  Binder binder,
						       			  Object[] args,
						       			  CultureInfo culture,
						       			  Object[] activationAttributes,
						       			  Evidence securityAttributes)
			{
				return CreateInstance(assemblyName, typeName, ignoreCase,
									  bindingAttr, binder, args, culture,
									  activationAttributes,
									  securityAttributes).Unwrap();
			}

	// Create an instance of a type within this application domain.
	public ObjectHandle CreateInstance(String assemblyName, String typeName)
			{
				return CreateInstance(assemblyName, typeName, false,
									  BindingFlags.CreateInstance | 
									  BindingFlags.Public |
									  BindingFlags.Instance, null,
									  null, null, null, null);
			}
	public ObjectHandle CreateInstance(String assemblyName, String typeName,
								       Object[] activationAttributes)
			{
				return CreateInstance(assemblyName, typeName, false,
									  BindingFlags.CreateInstance | 
									  BindingFlags.Public |
									  BindingFlags.Instance, null,
									  null, null, activationAttributes, null);
			}
	[TODO]
	public ObjectHandle CreateInstance(String assemblyName, String typeName,
								       bool ignoreCase,
									   BindingFlags bindingAttr,
								       Binder binder, Object[] args,
								       CultureInfo culture,
								       Object[] activationAttributes,
								       Evidence securityAttributes)
			{
				// TODO ?
				return Activator.CreateInstance(assemblyName,
												typeName,
												ignoreCase,
												bindingAttr,
												binder,
												args,
												culture,
												activationAttributes,
												securityAttributes);
			}

	// Create a remote instance of a type within this application domain.
	public ObjectHandle CreateInstanceFrom(String assemblyName,
										   String typeName)
			{
				return CreateInstance(assemblyName, typeName);
			}
	public ObjectHandle CreateInstanceFrom(String assemblyName,
										   String typeName,
								    	   Object[] activationAttributes)
			{
				return CreateInstance(assemblyName, typeName,
									  activationAttributes);
			}
	public ObjectHandle CreateInstanceFrom(String assemblyName,
										   String typeName,
								    	   bool ignoreCase,
										   BindingFlags bindingAttr,
								    	   Binder binder, Object[] args,
								    	   CultureInfo culture,
								    	   Object[] activationAttributes,
								    	   Evidence securityAttributes)
			{
				return CreateInstance(assemblyName, typeName, ignoreCase,
									  bindingAttr, binder, args, culture,
									  activationAttributes,
									  securityAttributes);
			}

	// Create a remote instance of a type and unwrap it.
	public Object CreateInstanceFromAndUnwrap(String assemblyName,
											  String typeName)
			{
				return CreateInstanceFrom(assemblyName, typeName).Unwrap();
			}
	public Object CreateInstanceFromAndUnwrap(String assemblyName,
								     		  String typeName,
						    	     		  Object[] activationAttributes)
			{
				return CreateInstanceFrom(assemblyName, typeName,
									      activationAttributes).Unwrap();
			}
	public Object CreateInstanceFromAndUnwrap(String assemblyName,
										      String typeName,
								    	      bool ignoreCase,
										      BindingFlags bindingAttr,
								    	      Binder binder, Object[] args,
								    	      CultureInfo culture,
								    	      Object[] activationAttributes,
								    	      Evidence securityAttributes)
			{
				return CreateInstanceFrom(assemblyName, typeName, ignoreCase,
									      bindingAttr, binder, args, culture,
									      activationAttributes,
									      securityAttributes).Unwrap();
			}

	// Execute a delegate in a foreign application domain.
	[TODO]
	public void DoCallBack(CrossAppDomainDelegate theDelegate)
			{
				// TODO
				// for now just call it directly
				theDelegate();
			}

	// Give the application domain an infinite lifetime service.
	public override Object InitializeLifetimeService()
			{
				// Always returns null.
				return null;
			}

#endif // CONFIG_REMOTING

#if CONFIG_REFLECTION_EMIT

	// Define a dynamic assembly builder within this domain.
	public AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access)
			{
				return DefineDynamicAssembly(name, access, null, null,
											 null, null, null, false);
			}
	public AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access,
				 Evidence evidence)
			{
				return DefineDynamicAssembly(name, access, null, evidence,
											 null, null, null, false);
			}
	public AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access,
				 String dir)
			{
				return DefineDynamicAssembly(name, access, dir, null,
										     null, null, null, false);
			}
	public AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access,
				 String dir, Evidence evidence)
			{
				return DefineDynamicAssembly(name, access, dir, evidence,
											 null, null, null, false);
			}
	public AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access,
				 PermissionSet requiredPermissions,
				 PermissionSet optionalPermissions,
				 PermissionSet refusedPersmissions)
			{
				return DefineDynamicAssembly(name, access, null, null,
									  		 requiredPermissions,
											 optionalPermissions,
											 refusedPersmissions, false);
			}
	public AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access,
				 Evidence evidence, PermissionSet requiredPermissions,
				 PermissionSet optionalPermissions,
				 PermissionSet refusedPersmissions)
			{
				return DefineDynamicAssembly(name, access, null, evidence,
									  		 requiredPermissions,
											 optionalPermissions,
											 refusedPersmissions, false);
			}
	public AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access,
				 String dir, PermissionSet requiredPermissions,
				 PermissionSet optionalPermissions,
				 PermissionSet refusedPersmissions)
			{
				return DefineDynamicAssembly(name, access, dir, null,
											 requiredPermissions,
											 optionalPermissions,
											 refusedPersmissions, false);
			}
	public AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access,
				 String dir, Evidence evidence,
				 PermissionSet requiredPermissions,
				 PermissionSet optionalPermissions,
				 PermissionSet refusedPersmissions)
			{
				return DefineDynamicAssembly(name, access, dir, evidence,
											 requiredPermissions,
											 optionalPermissions,
											 refusedPersmissions, false);
			}
	public AssemblyBuilder DefineDynamicAssembly
				(AssemblyName name, AssemblyBuilderAccess access,
				 String dir, Evidence evidence,
				 PermissionSet requiredPermissions,
				 PermissionSet optionalPermissions,
				 PermissionSet refusedPersmissions,
				 bool isSynchronized)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				String aname = name.Name;
				if(aname == null || aname == String.Empty)
				{
					throw new ArgumentException(_("Emit_AssemblyNameInvalid"));
				}
				if(Char.IsWhiteSpace(aname[0]) ||
				   aname.IndexOf('/') != -1 ||
				   aname.IndexOf('\\') != -1)
				{
					throw new ArgumentException(_("Emit_AssemblyNameInvalid"));
				}
				return new AssemblyBuilder(name, access, dir, isSynchronized);
			}

#endif // CONFIG_REFLECTION_EMIT

#if !ECMA_COMPAT

	// Execute a particular assembly within this application domain.
	public int ExecuteAssembly(String assemblyFile)
			{
				return ExecuteAssembly(assemblyFile, null, null,
									   null, AssemblyHashAlgorithm.None);
			}
	public int ExecuteAssembly(String assemblyFile, Evidence assemblySecurity)
			{
				return ExecuteAssembly(assemblyFile, assemblySecurity, null,
									   null, AssemblyHashAlgorithm.None);
			}
	public int ExecuteAssembly(String assemblyFile, Evidence assemblySecurity,
							   String[] args)
			{
				return ExecuteAssembly(assemblyFile, assemblySecurity,
									   args, null, AssemblyHashAlgorithm.None);
			}
	[TODO]
	public int ExecuteAssembly(String assemblyFile, Evidence assemblySecurity,
							   String[] args, byte[] hashValue,
							   AssemblyHashAlgorithm hashAlgorithm)
			{
				if(assemblyFile == null)
				{
					throw new ArgumentNullException("assemblyFile");
				}
				// TODO
				return 0;
			}

	// Get a list of all assemblies in this application domain.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static Assembly[] GetAssembliesInternal(Object appDomain);

	// Get a list of all assemblies in this application domain.
	public Assembly[] GetAssemblies()
	{
		return GetAssembliesInternal(appDomain);
	}

	// Get the current thread identifier.
	public static int GetCurrentThreadId()
			{
				return Thread.InternalGetThreadId();
			}

	// Fetch the object associated with a particular data name.
	public Object GetData(String name)
			{
				return items[name];
			}

	// Get the type of this instance.
	public new Type GetType()
			{
				return base.GetType();
			}

#if CONFIG_FRAMEWORK_2_0
	public bool IsDefaultAppDomain()
			{
				return IsDefaultAppDomainInternal(appDomain);
			}
#endif

	// Determine if this domain is running finalizers prior to unload.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool IsFinalizingForUnloadInternal(Object appDomain);

	// Determine if this domain is running finalizers prior to unload.
	public bool IsFinalizingForUnload()
			{
				return IsFinalizingForUnloadInternal(appDomain);
			}

	// Load an assembly into this application domain by name.
	public Assembly Load(AssemblyName assemblyRef)
			{
				return Load(assemblyRef, null);
			}
	public Assembly Load(AssemblyName assemblyRef, Evidence assemblySecurity)
			{
				if(assemblyRef == null)
				{
					throw new ArgumentNullException("assemblyRef");
				}
				if(assemblyRef.CodeBase != null &&
				   assemblyRef.CodeBase.Length > 0)
				{
					return Assembly.LoadFrom(assemblyRef.CodeBase);
				}
				else
				{
					return Assembly.Load(assemblyRef.Name);
				}
			}

	// Load an assembly into this application domain by string name.
	public Assembly Load(String assemblyString)
			{
				return Load(assemblyString, null);
			}
	public Assembly Load(String assemblyString, Evidence assemblySecurity)
			{
				return Assembly.Load(assemblyString);
			}

	// Load an assembly into this application domain by explicit definition.
	public Assembly Load(byte[] rawAssembly)
			{
				return Load(rawAssembly, null, null,
							Assembly.GetCallingAssembly());
			}
	public Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore)
			{
				return Load(rawAssembly, rawSymbolStore, null,
							Assembly.GetCallingAssembly());
			}
	public Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore,
				  		 Evidence assemblySecurity)
			{
				return Load(rawAssembly, rawSymbolStore, assemblySecurity,
							Assembly.GetCallingAssembly());
			}
	internal Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore,
				  		   Evidence assemblySecurity, Assembly caller)
			{
				if(rawAssembly == null)
				{
					throw new ArgumentNullException("rawAssembly");
				}
				int error;
				Assembly assembly = Assembly.LoadFromBytes
					(rawAssembly, out error, caller);
				if(error == Assembly.LoadError_OK)
				{
					return assembly;
				}
				else
				{
					Assembly.ThrowLoadError("raw bytes", error);
					return null;
				}
			}

#if CONFIG_POLICY_OBJECTS

	// Set policy information for this application domain.
	public void SetAppDomainPolicy(PolicyLevel domainPolicy)
			{
				// Nothing to do here: we don't use such policies.
			}

	// Set the policy for principals.
	public void SetPrincipalPolicy(PrincipalPolicy policy)
			{
				// Nothing to do here: we don't use such policies.
			}

	// Set the default principal object for a thread.
	public void SetThreadPrincipal(IPrincipal principal)
			{
				// Nothing to do here: we don't use such principals.
			}

#endif

	// Set the cache location for shadow copied assemblies.
	public void SetCachePath(String s)
			{
				setup.CachePath = s;
			}


	// Set a data item on this application domain.
	public void SetData(String name, Object data)
			{
				items[name] = data;
			}

	// Set the dynamic base path.
	public void SetDynamicBase(String path)
			{
				setup.DynamicBase = path;
			}

	// Turn on shadow copying.
	public void SetShadowCopyFiles()
			{
				SetShadowCopyFilesInternal(appDomain, true);
			}

	// Set the location of the shadow copy directory.
	public void SetShadowCopyPath(String s)
			{
				if (s == null || s.Length == 0)
				{
					// Clear the directories to be shadow copied (now all private assemblies are shadow copied)
					ClearShadowCopyPathInternal(appDomain);;
				}
				String[] splitPaths = s.Split(Path.PathSeparator);

				SetShadowCopyPathInternal(appDomain, splitPaths);
			}

	// Event that is emitted to resolve assemblies.
	public event ResolveEventHandler AssemblyResolve;

	// Event that is emitted on process exit.
	public event EventHandler ProcessExit;

	// Event that is emitted to resolve resources.
	public event ResolveEventHandler ResourceResolve;

	// Event that is emitted to resolve types.
	public event ResolveEventHandler TypeResolve;

#endif // !ECMA_COMPAT

}; // class AppDomain

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System
