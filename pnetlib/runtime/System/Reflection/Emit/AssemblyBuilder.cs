/*
 * AssemblyBuilder.cs - Implementation of the
 *		"System.Reflection.Emit.AssemblyBuilder" class.
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Reflection.Emit
{

#if CONFIG_REFLECTION_EMIT

using System;
using System.IO;
using System.Collections;
using System.Resources;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using System.Security.Permissions;
using System.Runtime.CompilerServices;
using System.Threading;

public sealed class AssemblyBuilder : Assembly
{
	// Internal state.
	private AssemblyBuilderAccess access;
	private String directory;
	private bool isSynchronized;
	private bool saved;
	private MethodInfo entryPoint;
	private IntPtr writer;
	private ModuleBuilder module;
	private ArrayList detachList;
	private PEFileKinds fileKind;

	// Constructor.  Called from AppDomain.DefineDynamicAssembly.
	internal AssemblyBuilder(AssemblyName name, AssemblyBuilderAccess access,
							 String directory, bool isSynchronized)
			{
				this.access = access;
				this.directory = (directory == null) ? "." : directory;
				this.isSynchronized = isSynchronized;
				this.saved = false;
				this.entryPoint = null;
				this.detachList = new ArrayList();
				fileKind = PEFileKinds.Dll;
				Version version = name.Version;
				lock(typeof(AssemblyBuilder))
				{
					if(version != null)
					{
						this.privateData = ClrAssemblyCreate
							(name.Name, version.Major, version.Minor,
							 version.Build, version.Revision,
							 access, out writer);
					}
					else
					{
						this.privateData = ClrAssemblyCreate
							(name.Name, 0, 0, 0, 0, access, out writer);
					}
				}
				if(this.privateData == IntPtr.Zero)
				{
					// The runtime engine disallowed dynamic assemblies.
					throw new SecurityException
						(_("Emit_NoDynamicAssemblies"));
				}
			}

	// Start a synchronized operation on this assembly builder.
	internal void StartSync()
			{
				if(isSynchronized)
				{
					Monitor.Enter(this);
				}
			}

	// End a synchronized operation on this assembly builder.
	internal void EndSync()
			{
				if(isSynchronized)
				{
					Monitor.Exit(this);
				}
			}

	// Get the code base for this assembly builder.
	public override String CodeBase
			{
				get
				{
					throw new NotSupportedException(_("NotSupp_Builder"));
				}
			}

	// Get the location of the assembly manifest file.
	public override String Location
			{
				get
				{
					throw new NotSupportedException(_("NotSupp_Builder"));
				}
			}

	// Get the entry point for this assembly builder.
	public override MethodInfo EntryPoint
			{
				get
				{
					return entryPoint;
				}
			}

	// Get the runtime image version that will be embedded in the assembly.
	public override String ImageRuntimeVersion
			{
				get
				{
					// Our runtime engine versions are different from
					// Microsoft's, but we want to match things up.
					// Use a version number from one of Microsoft's
					// public release versions.
					return "v1.1.4322";
				}
			}

	[TODO]
	public void AddResourceFile(String name, String fileName)
			{
		 		throw new NotImplementedException("AddResourceFile");
			}

	[TODO]
	public void AddResourceFile(String name, String fileName, ResourceAttributes attribute)
			{
		 		throw new NotImplementedException("AddResourceFile");
			}

	// Define a dynamic module that is attached to this assembly.
	public ModuleBuilder DefineDynamicModule(String name)
			{
				return DefineDynamicModule(name, false);
			}
	public ModuleBuilder DefineDynamicModule(String name, bool emitSymbolInfo)
			{
				try
				{
					StartSync();
					if(saved)
					{
						throw new InvalidOperationException
							(_("Emit_AlreadySaved"));
					}
					if(name == null)
					{
						throw new ArgumentNullException("name");
					}
					if(name == String.Empty)
					{
						throw new ArgumentException
							(_("Emit_ModuleNameInvalid"));
					}
					if(Char.IsWhiteSpace(name[0]) ||
				   	   name.IndexOf('/') != -1 ||
				   	   name.IndexOf('\\') != -1)
					{
						throw new ArgumentException
							(_("Emit_ModuleNameInvalid"));
					}
					if(module != null)
					{
						// We don't support assemblies with multiple modules.
		 				throw new NotSupportedException
							(_("Emit_SingleModuleOnly"));
					}
					module = new ModuleBuilder
						(this, name, true, emitSymbolInfo);
					return module;
				}
				finally
				{
					EndSync();
				}
			}
	public ModuleBuilder DefineDynamicModule(String name, String fileName)
			{
		 		return DefineDynamicModule(name, fileName, false);
			}
	public ModuleBuilder DefineDynamicModule(String name, String fileName,
											 bool emitSymbolInfo)
			{
				// We don't support modules in external files.
				return DefineDynamicModule(name, emitSymbolInfo);
		 		throw new NotSupportedException(_("Emit_ExternalModule"));
			}

	[TODO]
	public IResourceWriter DefineResource(String name, String description,
										  String fileName)
			{
		 		throw new NotImplementedException("DefineResource");
			}

	[TODO]
	public IResourceWriter DefineResource(String name, String description,
										  String fileName,
										  ResourceAttributes attribute)
			{
		 		throw new NotImplementedException("DefineResource");
			}

	[TODO]
	public void DefineUnmanagedResource(byte[] resource)
			{
		 		throw new NotImplementedException("DefineUnmanagedResource");
			}

	[TODO]
	public void DefineUnmanagedResource(String resourceFileName)
			{
		 		throw new NotImplementedException("DefineUnmanagedResource");
			}

	[TODO]
	public void DefineVersionInfoResource()
			{
				// We already added a 0.0.0.0 version info resource
		 		//throw new NotImplementedException("DefineVersionInfoResource");
			}

	[TODO]
	public void DefineVersionInfoResource(String product, String productVersion
										 ,String company, String copyright,
										  String trademark)
			{
		 		throw new NotImplementedException("DefineVersionInfoResource");
			}

	// Get a particular module within this assembly.
	public ModuleBuilder GetDynamicModule(String name)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				else if(name == String.Empty)
				{
					throw new ArgumentException
						(_("Emit_ModuleNameInvalid"));
				}
				if(module != null && module.Name == name)
				{
					return module;
				}
				else
				{
					return null;
				}
			}

	[TODO]
	public override Type[] GetExportedTypes()
			{
		 		throw new NotImplementedException("GetExportedTypes");
			}

	[TODO]
	public override FileStream GetFile(String name)
			{
		 		throw new NotImplementedException("GetFile");
			}

	[TODO]
	public override FileStream[] GetFiles(bool getResourceModules)
			{
		 		throw new NotImplementedException("GetFiles");
			}

	// Get information about a particular manifest resource.
	[TODO]
	public override ManifestResourceInfo
				GetManifestResourceInfo(String resourceName)
			{
		 		throw new NotImplementedException("GetManifestResourceInfo");
			}

	// Get the names of all manifest resources in this assembly.
	public override String[] GetManifestResourceNames()
			{
		 		throw new NotImplementedException("GetManifestResourceNames");
			}

	// Get a stream for a particular manifest resource.
	[TODO]
	public override Stream GetManifestResourceStream(String name)
			{
		 		throw new NotImplementedException("GetManifestResourceNames");
			}

	// Get a stream for a particular manifest resource, scoped by a type.
	[TODO]
	public override Stream GetManifestResourceStream(Type type, String name)
			{
		 		throw new NotImplementedException("GetManifestResourceStream");
			}

	[TODO]
	public void Save(String assemblyFileName)
			{
				if (assemblyFileName == null)
				{
					throw new ArgumentNullException(/* TODO */);
				}
				if (assemblyFileName.Length == 0)
				{
					throw new ArgumentException(/* TODO */);
				}
				if (saved || (access & AssemblyBuilderAccess.Save) == 0)
				{
					throw new InvalidOperationException(/* TODO */);
				}
				directory = Path.GetFullPath(directory);
				if (!Directory.Exists(directory))
				{
					throw new ArgumentException(/* TODO */);
				}
				String path = Path.Combine(directory, assemblyFileName);
				/* TODO: the rest of the exception throwing checks */
				IntPtr entry = IntPtr.Zero;
				if (entryPoint != null)
				{
					entry = ((IClrProgramItem)entryPoint).ClrHandle;
				}
				if (!(ClrSave(base.privateData, writer, path, entry, fileKind)))
				{
					throw new IOException(/* TODO */);
				}
				saved = true;
			}

	// Set a custom attribute on this assembly.
	public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
			{
				try
				{
					StartSync();
					if(saved)
					{
						throw new InvalidOperationException
							(_("Emit_AlreadySaved"));
					}
					SetCustomAttribute(this, customBuilder);
				}
				finally
				{
					EndSync();
				}
			}
	public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
			{
				try
				{
					StartSync();
					if(saved)
					{
						throw new InvalidOperationException
							(_("Emit_AlreadySaved"));
					}
					SetCustomAttribute(this, con, binaryAttribute);
				}
				finally
				{
					EndSync();
				}
			}

	// Set the entry point for this assembly builder.
	public void SetEntryPoint(MethodInfo entryMethod)
			{
				SetEntryPoint(entryMethod, PEFileKinds.ConsoleApplication);
			}
	public void SetEntryPoint(MethodInfo entryMethod, PEFileKinds fileKind)
			{
				if(entryMethod == null)
				{
					throw new ArgumentNullException("entryMethod");
				}
				if(entryMethod.DeclaringType.Assembly != this)
				{
					throw new InvalidOperationException
						(_("Invalid_EntryNotInAssembly"));
				}
				entryPoint = entryMethod;
				this.fileKind = fileKind;
			}

	// SetCustomAttribute():
	// All other XXXBuilder classes invoke call the public method
	// AssemblyBuilder.SetCustomAttribute(), which invokes one of the
	// internal SetCustomAttribute() methods that follow.

	// Set custom attributes on a program item in this assembly.
	internal void SetCustomAttribute
				(IClrProgramItem item, CustomAttributeBuilder customBuilder)
			{
				byte[] blob = customBuilder.ToBytes();
				IntPtr attribute = ClrAttributeCreate
					(base.privateData,
					 ((IClrProgramItem)(customBuilder.con)).ClrHandle, blob);
				ClrAttributeAddToItem(item.ClrHandle, attribute);
			}

	[TODO]
	internal void SetCustomAttribute
				(IClrProgramItem item, ConstructorInfo con,
				 byte[] binaryAttribute)
			{
		 		throw new NotImplementedException("SetCustomAttribute");
			}

	// Add declarative security to a program item in this assembly.
	[TODO]
	internal void AddDeclarativeSecurity(IClrProgramItem item,
										 SecurityAction action,
									     PermissionSet pset)
			{
		 		throw new NotImplementedException("AddDeclarativeSecurity");
			}

	// Add an item to this assembly's detach list.
	internal void AddDetach(IDetachItem item)
			{
				lock(typeof(AssemblyBuilder))
				{
					detachList.Add(item);
				}
			}

	// Detach everything used by this assembly, which ensures that
	// all pointers to native structures are invalidated.
	private void Detach()
			{
				lock(typeof(AssemblyBuilder))
				{
					foreach(IDetachItem item in detachList)
					{
						item.Detach();
					}
					privateData = IntPtr.Zero;
				}
			}

	// Write the method IL to the output.
	internal int WriteMethod(byte[] header,
	                         byte[] code,
	                         IntPtr[] codeFixupPtrs,
	                         int[] codeFixupOffsets,
	                         byte[][] exceptionBlocks,
	                         IntPtr[] exceptionBlockFixupPtrs,
	                         int[] exceptionBlockFixupOffsets)
	{
		return ClrWriteMethod(privateData,
		                      writer,
		                      header,
		                      code,
		                      codeFixupPtrs,
		                      codeFixupOffsets,
		                      exceptionBlocks,
		                      exceptionBlockFixupPtrs,
		                      exceptionBlockFixupOffsets);
	}

	// Create a new assembly.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrAssemblyCreate
			(String name, int v1, int v2, int v3, int v4,
			 AssemblyBuilderAccess access, out IntPtr writer);

	// Save the assembly to a file.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool ClrSave(IntPtr assembly, IntPtr writer,
	                                   String path, IntPtr entryMethod,
	                                   PEFileKinds fileKind /*, TODO */);

	// Get the token associated with a particular program item.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static int ClrGetItemToken(IntPtr item);

	// Get the program item associated with a particular token.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static IntPtr ClrGetItemFromToken
			(IntPtr assembly, int token);

	// Write the body of a method to the code section and return the RVA
	// that corresponds to it.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int ClrWriteMethod(IntPtr assembly,
	                                         IntPtr writer,
	                                         byte[] header,
	                                         byte[] code,
	                                         IntPtr[] codeFixupPtrs,
	                                         int[] codeFixupOffsets,
	                                         byte[][] exceptionBlocks,
	                                         IntPtr[] exceptionBlockFixupPtrs,
	                                         int[] exceptionBlockFixupOffsets);

	// Add a new attribute to an assembly image.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrAttributeCreate
			(IntPtr assembly, IntPtr ctor, byte[] blob);

	// Add an attribute to a program item and convert special attributes.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void ClrAttributeAddToItem
			(IntPtr item, IntPtr attribute);

}; // class AssemblyBuilder

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
