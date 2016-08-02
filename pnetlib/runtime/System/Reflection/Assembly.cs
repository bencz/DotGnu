/*
 * Assembly.cs - Implementation of the "System.Reflection.Assembly" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Reflection
{

#if CONFIG_RUNTIME_INFRA

using System;
using System.IO;
using System.Globalization;
using System.Security;
using System.Security.Policy;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Configuration.Assemblies;

#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDual)]
#endif
public class Assembly : IClrProgramItem
#if CONFIG_REFLECTION
	, ICustomAttributeProvider
#endif
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
#if !ECMA_COMPAT
	, IEvidenceFactory
#endif
{

	// Built-in handle for the assembly.  This must be the first field.
	internal IntPtr privateData;

	// Private constructor.  Normally called by the runtime engine only.
	internal Assembly() {}

	// Implement the IClrProgramItem interface.
	IntPtr IClrProgramItem.ClrHandle
			{
				get
				{
					return privateData;
				}
			}

#if CONFIG_REFLECTION

	// Create an instance of a specific type within this assembly.
	public Object CreateInstance(String typeName)
			{
				return CreateInstance(typeName, false,
									  BindingFlags.Public |
									  BindingFlags.Instance,
									  null, null, null, null);
			}
#if !ECMA_COMPAT
	public Object CreateInstance(String typeName, bool ignoreCase)
			{
				return CreateInstance(typeName, ignoreCase,
									  BindingFlags.Public |
									  BindingFlags.Instance,
									  null, null, null, null);
			}
	public
#else  // ECMA_COMPAT
	private
#endif // ECMA_COMPAT
	Object CreateInstance(String typeName, bool ignoreCase,
						  BindingFlags bindingAttr, Binder binder,
						  Object[] args, CultureInfo culture,
						  Object[] activationAttributes)
			{
				Type type = GetType(typeName, false, ignoreCase);
				if(type == null)
				{
					return null;
				}
				return Activator.CreateInstance(type, bindingAttr,
												binder, args, culture,
												activationAttributes);
			}

#endif // CONFIG_REFLECTION

#if !ECMA_COMPAT

	// Create a qualified type name.
	public static String CreateQualifiedName(String assemblyName,
											 String typeName)
			{
				return typeName + ", " + assemblyName;
			}

#endif // !ECMA_COMPAT

#if !ECMA_COMPAT

	// Get the custom attributes associated with this assembly.
	public virtual Object[] GetCustomAttributes(bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, inherit);
			}
	public virtual Object[] GetCustomAttributes(Type type, bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, type, inherit);
			}

	// Determine if custom attributes are associated with this assembly.
	public virtual bool IsDefined(Type type, bool inherit)
			{
				return ClrHelpers.IsDefined(this, type, inherit);
			}

#elif CONFIG_REFLECTION

	// Get the custom attributes associated with this assembly.
	Object[] ICustomAttributeProvider.GetCustomAttributes(bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, inherit);
			}
	Object[] ICustomAttributeProvider.GetCustomAttributes
					(Type type, bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, type, inherit);
			}

	// Determine if custom attributes are associated with this assembly.
	bool ICustomAttributeProvider.IsDefined(Type type, bool inherit)
			{
				return ClrHelpers.IsDefined(this, type, inherit);
			}

#endif // CONFIG_REFLECTION


	// The following three methods are not strictly speaking
	// ECMA-compatible, but they are useful in ECMA systems.

	// Get the assembly that called the method that called this method.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Assembly GetCallingAssembly();

	// Get the assembly that called this method.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Assembly GetExecutingAssembly();

	// Get the assembly that contained the program entry point.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Assembly GetEntryAssembly();

#if !ECMA_COMPAT

	// Get an array of all exported types in an assembly.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public virtual Type[] GetExportedTypes();

	// Get a file stream for a particular public manifest file.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public virtual FileStream GetFile(String name);

	// Get file streams for all public manifest files.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public virtual FileStream[] GetFiles(bool getResourceModules);
	public virtual FileStream[] GetFiles()
			{
				return GetFiles(false);
			}

	// Get information about a particular manifest resource.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public virtual ManifestResourceInfo
			GetManifestResourceInfo(String resourceName);

	// Get the names of all manifest resources in this assembly.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public virtual String[] GetManifestResourceNames();

	// Get a stream for a particular manifest resource.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public virtual Stream GetManifestResourceStream(String name);

	// Get a stream for a particular manifest resource, scoped by a type.
	public virtual Stream GetManifestResourceStream(Type type, String name)
			{
				if(name == null)
				{
					return null;
				}
				else if(type != null)
				{
					String nspace = type.Namespace;
					if(nspace != null && nspace != String.Empty)
					{
						return GetManifestResourceStream(nspace + "." + name);
					}
				}
				return GetManifestResourceStream(name);
			}

#else // ECMA_COMPAT

	// Get a file stream for a particular public manifest file.
	// Not strictly speaking ECMA-compatible, but needed for I18N.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public virtual FileStream GetFile(String name);

	// Get a stream for a particular manifest resource.
	// Not strictly speaking ECMA-compatible, but needed for I18N.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public virtual Stream GetManifestResourceStream(String name);

#endif // ECMA_COMPAT

	// Get a particular type from this assembly.
	public virtual Type GetType(String typeName)
			{
				return GetType(typeName, false, false);
			}
#if !ECMA_COMPAT
	public virtual Type GetType(String typeName, bool throwOnError)
			{
				return GetType(typeName, throwOnError, false);
			}
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public Type GetType(String typeName, bool throwOnError,
							   bool ignoreCase);
#else
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal Type GetType(String typeName, bool throwOnError,
							     bool ignoreCase);
#endif

	// Get an array of all types in an assembly.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public virtual Type[] GetTypes();

	// Error codes for "LoadFromName" and "LoadFromFile".
	internal const int LoadError_OK			   = 0;
	internal const int LoadError_InvalidName   = 1;
	internal const int LoadError_FileNotFound  = 2;
	internal const int LoadError_BadImage      = 3;
	internal const int LoadError_Security      = 4;

	// Internal version of "Load".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static Assembly LoadFromName(String name, out int error,
												Assembly parent);

	// Internal version of "LoadFrom".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static Assembly LoadFromFile(String name, out int error,
												 Assembly parent);

	// Internal version of "AppDomain.Load" for a byte array.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static Assembly LoadFromBytes(byte[] bytes, out int error,
												  Assembly parent);

	// Throw an exception based on a load error.
	internal static void ThrowLoadError(String name, int error)
			{
				if(error == LoadError_InvalidName)
				{
					throw new ArgumentException(_("Reflection_AssemblyName"));
				}
				else if(error == LoadError_FileNotFound)
				{
					throw new FileNotFoundException
						(String.Format(_("Reflection_AssemblyFile"), name));
				}
				else if(error == LoadError_BadImage)
				{
					throw new BadImageFormatException
						(String.Format(_("Reflection_BadImage"), name));
				}
				else
				{
					throw new SecurityException
						(String.Format(_("Reflection_AssemblySecurity"), name));
				}
			}

	// Load a particular assembly.
	public static Assembly Load(String assemblyString)
			{
				return Load(assemblyString, GetCallingAssembly());
			}
	internal static Assembly Load(String assemblyString, Assembly caller)
			{
				Assembly assembly;
				int error;
				if(assemblyString == null)
				{
					throw new ArgumentNullException("assemblyString");
				}
				if(assemblyString.Length >= 7 &&
				   String.Compare(assemblyString, 0, "file://", 0, 7, true)
				   		== 0)
				{
					if(assemblyString.Length >= 10 &&
					   assemblyString[7] == '/' &&
					   assemblyString[9] == ':')
					{
						// Specification of the form "file:///X:...".
						assemblyString = assemblyString.Substring(8);
					}
					else
					{
						// Some other type of file specification.
						assemblyString = assemblyString.Substring(7);
					}
					assembly = LoadFromFile(assemblyString, out error, caller);
				}
				else
				{
#if !ECMA_COMPAT
					AssemblyName name = AssemblyName.Parse(assemblyString);
					assembly = LoadFromName(name.Name, out error, 
											caller);
#else
					if(assemblyString.IndexOf(",") == -1)
					{
						assembly = LoadFromName(assemblyString, out error,
												 caller);
					}
					else
					{
						assembly = LoadFromName(assemblyString.Substring(0,
													assemblyString.IndexOf(",")),
													out error, caller);
					}
#endif // !ECMA_COMPAT
				}
				if(error == LoadError_OK)
				{
					return assembly;
				}
				else
				{
					ThrowLoadError(assemblyString, error);
					return null;
				}
			}

	// Load a particular assembly from a file.
#if !ECMA_COMPAT
	public
#else
	internal
#endif
	static Assembly LoadFrom(String assemblyFile)
			{
				return LoadFrom(assemblyFile, GetCallingAssembly());
			}
	internal static Assembly LoadFrom(String assemblyFile, Assembly caller)
			{
				char [] pathChars = new char[] {
										Path.DirectorySeparatorChar,
										Path.VolumeSeparatorChar,
										Path.AltDirectorySeparatorChar };
						
				if(assemblyFile == null)
				{
					throw new ArgumentNullException("assemblyFile");
				}
				int error;
				Assembly assembly;

				if(assemblyFile.Length >= 7 &&
				   String.Compare(assemblyFile, 0, "file://", 0, 7, true)
				   		== 0)
				{
					if(assemblyFile.Length >= 10 &&
					   assemblyFile[7] == '/' &&
					   assemblyFile[9] == ':')
					{
						// Specification of the form "file:///X:...".
						assemblyFile = assemblyFile.Substring(8);
					}
					else
					{
						// Some other type of file specification.
						assemblyFile = assemblyFile.Substring(7);
					}
				}
				
				if(assemblyFile.EndsWith(".dll") || 
					assemblyFile.EndsWith(".DLL") ||
					(assemblyFile.IndexOfAny(pathChars) != -1))
				{
					assembly = LoadFromFile(assemblyFile, out error,
											     caller);
				}
				else
				{
#if !ECMA_COMPAT
					AssemblyName name = AssemblyName.Parse(assemblyFile);
					assembly = LoadFromName(name.Name, out error,
												 caller);
#else
					if(assemblyFile.IndexOf(",") == -1)
					{
						assembly = LoadFromName(assemblyFile, out error,
												 caller);
					}
					else
					{
						assembly = LoadFromName(assemblyFile.Substring(0,
													assemblyFile.IndexOf(",")),
													out error, caller);
					}
#endif // !ECMA_COMPAT
					}
				if(error == LoadError_OK)
				{
					return assembly;
				}
				else
				{
					ThrowLoadError(assemblyFile, error);
					return null;
				}
			}

#if !ECMA_COMPAT

	// Load an assembly using evidence (which we dont' use in
	// this implemantation).
	public static Assembly Load(String assemblyString,
								Evidence assemblySecurity)
			{
				return Load(assemblyString, GetCallingAssembly());
			}

	// Load an assembly given an assembly name.
	public static Assembly Load(AssemblyName assemblyRef)
			{
				if(assemblyRef == null)
				{
					throw new ArgumentNullException("assemblyRef");
				}
				return Load(assemblyRef.FullName, GetCallingAssembly());
			}
	public static Assembly Load(AssemblyName assemblyRef,
								Evidence assemblySecurity)
			{
				if(assemblyRef == null)
				{
					throw new ArgumentNullException("assemblyRef");
				}
				return Load(assemblyRef.FullName, GetCallingAssembly());
			}

	// Load an assembly from a raw byte image.
	public static Assembly Load(byte[] rawAssembly)
			{
				return AppDomain.CurrentDomain.Load
					(rawAssembly, null, null, GetCallingAssembly());
			}
	public static Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore)
			{
				return AppDomain.CurrentDomain.Load
					(rawAssembly, rawSymbolStore, null,
					 GetCallingAssembly());
			}
	public static Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore,
								Evidence securityEvidence)
			{
				return AppDomain.CurrentDomain.Load
					(rawAssembly, rawSymbolStore, securityEvidence,
					 GetCallingAssembly());
			}

	// Load an assembly from a file.
	public static Assembly LoadFile(String path)
			{
				return LoadFrom(path, GetCallingAssembly());
			}
	public static Assembly LoadFile(String path, Evidence securityEvidence)
			{
				return LoadFrom(path, GetCallingAssembly());
			}
	public static Assembly LoadFrom(String assemblyFile,
									Evidence securityEvidence)
			{
				return LoadFrom(assemblyFile, GetCallingAssembly());
			}
	public static Assembly LoadFrom(String assemblyFile,
									Evidence securityEvidence,
									byte[] hashValue,
									AssemblyHashAlgorithm hashAlgorithm)
			{
				return LoadFrom(assemblyFile, GetCallingAssembly());
			}

	// Load an assembly using a partial name.
	public static Assembly LoadWithPartialName(String partialName)
			{
				return LoadWithPartialName(partialName, GetCallingAssembly());
			}
	public static Assembly LoadWithPartialName(String partialName, 
											   Evidence securityEvidence)
			{
				return LoadWithPartialName(partialName, GetCallingAssembly());
			}
	private static Assembly LoadWithPartialName(String partialName,
												Assembly caller)
			{
				Assembly assembly;
				AssemblyName name;
				int error;
				if(partialName == null)
				{
					throw new ArgumentNullException("partialName");
				}
				name = AssemblyName.Parse(partialName);
				assembly = LoadFromName(name.Name, out error, caller);
				if(error == LoadError_OK)
				{
					return assembly;
				}
				else
				{
					return null;
				}
			}

#endif // !ECMA_COMPAT

	// Convert this assembly into a string.
	public override String ToString()
			{
				String name = FullName;
				if(name != null)
				{
					return name;
				}
				else
				{
					return base.ToString();
				}
			}

	// Get the full name associated with this assembly.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private String GetFullName();

	// Get the full name associated with this assembly.
	public virtual String FullName
			{
				get
				{
					return GetFullName();
				}
			}

#if !ECMA_COMPAT

	// Get the code base associated with this assembly.
	public virtual String CodeBase
			{
				get
				{
					return GetName().CodeBase;
				}
			}
	
	// Get the escaped code base associated with this assembly.
	public virtual String EscapedCodeBase
			{
				get
				{
					return GetName().EscapedCodeBase;
				}
			}
	
	// Get the entry point for this assembly.
	public virtual MethodInfo EntryPoint
			{
				get
				{
					return (MethodInfo)(MethodBase.GetMethodFromHandle
								(GetEntryPoint()));
				}
			}

	// Get the security evidence for this assembly.
	public virtual Evidence Evidence
			{
				get
				{
					// We don't use evidence objects in this implementation
					// at the moment, so return a dummy evidence object.
					return new Evidence();
				}
			}

	// Determine if this assembly was loaded from the global assembly cache.
	public bool GlobalAssemblyCache
			{
				get
				{
					// We don't use a GAC in this implementation, or if
					// we do then we leave it up to the engine to decide.
					return false;
				}
			}

	// Get the runtime version that the assembly was compiled against.
	[ComVisible(false)]
	public virtual String ImageRuntimeVersion
			{
				get
				{
					return GetImageRuntimeVersion();
				}
			}
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private String GetImageRuntimeVersion();

	// Get the location where this assembly was loaded from.
	public virtual String Location
			{
				get
				{
					String retval;
					retval = GetLocation();
					return (retval == null) ? "" : retval;
				}
			}

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private String GetLocation();

	// Get the assembly that a particular type resides in.
	public static Assembly GetAssembly(Type type)
			{
				if(type == null)
				{
					throw new ArgumentNullException("type");
				}
				return type.Assembly;
			}

	// Get the entry point method for this assembly.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private RuntimeMethodHandle GetEntryPoint();

#endif // !ECMA_COMPAT

	// Get the full pathname of a satellite file underneath
	// the directory containing this assembly.  Returns null
	// if it isn't possible to retrieve the path or the file
	// doesn't exist.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal String GetSatellitePath(String filename);

#if CONFIG_REFLECTION && !ECMA_COMPAT

	// Module resolution event.
	public event ModuleResolveEventHandler ModuleResolve;

#if CONFIG_SERIALIZATION

	// Serialize this object.
	public virtual void GetObjectData(SerializationInfo info,
									  StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				UnitySerializationHolder.Serialize
					(info, UnitySerializationHolder.UnityType.Assembly,
					 FullName, this);
			}

#endif // CONFIG_SERIALIZATION

	// Get the loaded modules within this assembly.  We make no
	// distinction between loaded and unloaded in this implementation,
	// because the runtime engine hides the loading of modules.
	public Module[] GetLoadedModules()
			{
				return GetModules(false);
			}
	public Module[] GetLoadedModules(bool getResourceModules)
			{
				return GetModules(getResourceModules);
			}

	// Get a module by name from this assembly.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private Module GetModuleInternal(String name);

	// Get a particular module from within this assembly.
	public Module GetModule(String name)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				else if(name.Length == 0)
				{
					throw new ArgumentException
						(_("ArgRange_StringNonEmpty"), "name");
				}
				return GetModuleInternal(name);
			}

	// Get the modules within this assembly.
	public Module[] GetModules()
			{
				return GetModules(false);
			}
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public Module[] GetModules(bool getResourceModules);

	// Fill in an assembly name block with a loaded assembly's information.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private void FillAssemblyName(AssemblyName nameInfo);

	// Get the name of this assembly.
	public virtual AssemblyName GetName()
			{
				AssemblyName nameInfo = new AssemblyName();
				String filename = GetLocation();
				if(filename != null && filename.Length > 0)
				{
					if(filename[0] == '/' || filename[0] == '\\')
					{
						nameInfo.CodeBase = "file://" +
							filename.Replace('\\', '/');
					}
					else
					{
						nameInfo.CodeBase = "file:///" +
							filename.Replace('\\', '/');
					}
				}
				FillAssemblyName(nameInfo);
				return nameInfo;
			}
	public virtual AssemblyName GetName(bool copiedName)
			{
				// We don't support shadow copies in this implementation.
				return GetName();
			}

	// Get the assemblies referenced by this one.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private Assembly[] GetReferencedAssembliesInternal();

	// Get a list of the assemblies that are referenced by this one.
	public AssemblyName[] GetReferencedAssemblies()
			{
				Assembly[] list = GetReferencedAssembliesInternal();
				if(list == null)
				{
					return null;
				}
				AssemblyName[] names = new AssemblyName [list.Length];
				int posn;
				for(posn = 0; posn < list.Length; ++posn)
				{
					names[posn] = list[posn].GetName();
				}
				return names;
			}

	// Get a satellite resource assembly.
	public Assembly GetSatelliteAssembly(CultureInfo culture)
			{
				return GetSatelliteAssembly
					(culture, null, GetCallingAssembly());
			}
	public Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
			{
				return GetSatelliteAssembly
					(culture, version, GetCallingAssembly());
			}
	private Assembly GetSatelliteAssembly(CultureInfo culture,
										  Version version,
										  Assembly caller)
			{
				if(culture == null)
				{
					throw new ArgumentNullException("culture");
				}
				String shortName = FullName;
				if(shortName.IndexOf(',') != -1)
				{
					shortName = shortName.Substring(0, shortName.IndexOf(','));
				}
				String baseName = culture.Name + Path.DirectorySeparatorChar +
					 			  shortName + ".resources.dll";
				String path = GetSatellitePath(baseName);
				if(path == null)
				{
					throw new FileNotFoundException
						(String.Format
							(_("Reflection_AssemblyFile"), baseName));
				}
				else
				{
					return LoadFrom(path, caller);
				}
			}

	// Load a raw module and attach it to this assembly.
	public Module LoadModule(String moduleName, byte[] rawModule)
			{
				return LoadModule(moduleName, rawModule, null);
			}
	public Module LoadModule(String moduleName, byte[] rawModule,
							 byte[] rawSymbolStore)
			{
				// Raw module loading is not supported in this implementation.
				// It is too dangerous security-wise.
				throw new SecurityException
						(String.Format
							(_("Reflection_AssemblySecurity"), moduleName));
			}

#endif // CONFIG_REFLECTION && !ECMA_COMPAT

}; // class Assembly

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System.Reflection
