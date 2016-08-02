/*
 * Vsa.cs - Scripting engine interfaces.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

// [assembly:System.CLSCompliant(true)]
namespace Microsoft.Vsa
{

using System;
using System.Collections;
using System.CodeDom;
using System.Reflection;
using System.Security.Policy;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

// Script item flags.
public enum VsaItemFlag
{
	None   = 0,
	Module = 1,
	Class  = 2

}; // enum VsaItemFlag

// Script item types.
public enum VsaItemType
{
	Reference = 0,
	AppGlobal = 1,
	Code      = 2

}; // enum VsaItemType

// Interface for querying error information.
#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("E0C0FFE4-7eea-4ee2-b7e4-0080c7eb0b74")]
#endif
public interface IVsaError
{
	String Description { get; }
	int EndColumn { get; }
	int Line { get; }
	String LineText { get; }
	int Number { get; }
	int Severity { get; }
	IVsaItem SourceItem { get; }
	String SourceMoniker { get; }
	int StartColumn { get; }

}; // interface IVsaError

// Interface to a script item.
#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("E0C0FFE5-7eea-4ee5-b7e4-0080c7eb0b74")]
#endif
public interface IVsaItem
{
	bool IsDirty { get; }
	VsaItemType ItemType { get; }
	String Name { get; set; }
	Object GetOption(String name);
	void SetOption(String name, Object value);

}; // interface IVsaItem

// Collection of script items.
#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("0AB1EB6A-12BD-44d0-B941-0580ADFC73DE")]
#endif
public interface IVsaItems : IEnumerable
{
	int Count { get; }
	IVsaItem this[int index] { get; }
	IVsaItem this[String name] { get; }
	IVsaItem CreateItem(String name, VsaItemType type, VsaItemFlag itemFlag);
	void Remove(int index);
	void Remove(String name);

}; // interface IVsaItems

// Interface to a script code item.
#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("E0C0FFE7-7eea-4ee5-b7e4-0080c7eb0b74")]
#endif
public interface IVsaCodeItem : IVsaItem
{
#if CONFIG_CODEDOM
	CodeObject CodeDOM { get; }
#endif
	String SourceText { get; set; }
	void AddEventSource(String eventSourceName, String eventSourceType);
	void AppendSourceText(String text);
	void RemoveEventSource(String eventSourceName);

}; // interface IVsaCodeItem

// Interface to a global script item.
#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("4E76D92E-E29D-46f3-AE22-0333158109F1")]
#endif
public interface IVsaGlobalItem : IVsaItem
{
	bool ExposeMembers { get; set; }
	String TypeString { set; }

}; // interface IVsaGlobalItem

// Interface to a reference script item.
#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("E0C0FFE6-7eea-4ee5-b7e4-0080c7eb0b74")]
#endif
public interface IVsaReferenceItem : IVsaItem
{
	String AssemblyName { get; set; }

}; // interface IVsaReferenceItem

// Interface to a site that can be used to persist script source.
#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("E0C0FFE3-7eea-4ee2-b7e4-0080c7eb0b74")]
#endif
public interface IVsaPersistSite
{
	String LoadElement(String name);
	void SaveElement(String name, String source);

}; // interface IVsaPersistSite

// Interface to a site that is used to communicate with an engine.
#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("E0C0FFE2-7eea-4ee2-b7e4-0080c7eb0b74")]
#endif
public interface IVsaSite
{
	void GetCompiledState(out byte[] pe, out byte[] debugInfo);
	Object GetEventSourceInstance(String itemName, String eventSourceName);
	Object GetGlobalInstance(String name);
	void Notify(String notify, Object info);
	bool OnCompilerError(IVsaError error);

}; // interface IVsaSite

// Interface to a scripting engine.
#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("E0C0FFE1-7eea-4ee2-b7e4-0080c7eb0b74")]
#endif
public interface IVsaEngine
{
	// Properties.
	Assembly Assembly { get; }
	Evidence Evidence { get; set; }
	bool GenerateDebugInfo { get; set; }
	bool IsCompiled { get; }
	bool IsDirty { get; }
	bool IsRunning { get; }
	IVsaItems Items { get; }
	String Language { get; }
	int LCID { get; set; }
	String Name { get; }
	String RootMoniker { get; set; }
	String RootNamespace { get; set; }
	IVsaSite Site { get; set; }
	String Version { get; }

	// Methods.
	void Close();
	bool Compile();
	Object GetOption(String name);
	void InitNew();
	bool IsValidIdentifier(String identifier);
	void LoadSourceState(IVsaPersistSite site);
	void Reset();
	void RevokeCache();
	void Run();
	void SaveCompiledState(out byte[] pe, out byte[] pdb);
	void SaveSourceState(IVsaPersistSite site);
	void SetOption(String name, Object value);

}; // interface IVsaEngine

// DT code items.
#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("E0C0FFED-7eea-4ee5-b7e4-0080c7eb0b74")]
#endif
public interface IVsaDTCodeItem
{
	bool CanDelete { get; set; }
	bool CanMove { get; set; }
	bool CanRename { get; set; }
	bool Hidden { get; set; }
	bool ReadOnly { get; set; }

}; // interface IVsaDTCodeItem

// DT engine interface.
#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("E0C0FFEE-7eea-4ee5-b7e4-0080c7eb0b74")]
#endif
public interface IVsaDTEngine
{
	void AttachDebugger(bool isAttach);
	IVsaIDE GetIDE();
	void InitCompleted();
	String TargetURL { get; set; }

}; // interface IVsaDTEngine

// IDE interface for VSA.
#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("78470A10-8153-407d-AB1B-05067C54C36B")]
#endif
public interface IVsaIDE
{
	void EnableMainWindow(bool isEnable);
	void ShowIDE(bool showOrHide);
	String DefaultSearchPath { get; set; }
	Object ExtensibilityObject { get; }
	VsaIDEMode IDEMode { get; }
	IVsaIDESite Site { get; set; }

}; // interface IVsaIDE

// IDE site interface for VSA.
#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("7BD84086-1FB5-4b5d-8E05-EAA2F17218E0")]
#endif
public interface IVsaIDESite
{
	void Notify(String notify, Object optional);

}; // interface IVsaIDESite

// VSA IDE mode values.
public enum VsaIDEMode
{
	Break		= 0,
	Design		= 1,
	Run			= 2

}; // enum VsaIDEMode

// Scripting error codes.
public enum VsaError
{
	AppDomainCannotBeSet        = unchecked((int)0x80133000),
	AppDomainInvalid            = unchecked((int)0x80133001),
	ApplicationBaseCannotBeSet  = unchecked((int)0x80133002),
	ApplicationBaseInvalid      = unchecked((int)0x80133003),
	AssemblyExpected            = unchecked((int)0x80133004),
	AssemblyNameInvalid         = unchecked((int)0x80133005),
	BadAssembly                 = unchecked((int)0x80133006),
	CachedAssemblyInvalid       = unchecked((int)0x80133007),
	CallbackUnexpected          = unchecked((int)0x80133008),
	CodeDOMNotAvailable         = unchecked((int)0x80133009),
	CompiledStateNotFound       = unchecked((int)0x8013300A),
	DebugInfoNotSupported       = unchecked((int)0x8013300B),
	ElementNameInvalid          = unchecked((int)0x8013300C),
	ElementNotFound             = unchecked((int)0x8013300D),
	EngineBusy                  = unchecked((int)0x8013300E),
	EngineCannotClose           = unchecked((int)0x8013300F),
	EngineCannotReset           = unchecked((int)0x80133010),
	EngineClosed                = unchecked((int)0x80133011),
	EngineEmpty                 = unchecked((int)0x80133012),
	EngineInitialized           = unchecked((int)0x80133013),
	EngineNameInUse             = unchecked((int)0x80133014),
	EngineNotCompiled           = unchecked((int)0x80133015),
	EngineNotInitialized        = unchecked((int)0x80133016),
	EngineNotRunning            = unchecked((int)0x80133017),
	EngineRunning               = unchecked((int)0x80133018),
	EventSourceInvalid          = unchecked((int)0x80133019),
	EventSourceNameInUse        = unchecked((int)0x8013301A),
	EventSourceNameInvalid      = unchecked((int)0x8013301B),
	EventSourceNotFound         = unchecked((int)0x8013301C),
	EventSourceTypeInvalid      = unchecked((int)0x8013301D),
	GetCompiledStateFailed      = unchecked((int)0x8013301E),
	GlobalInstanceInvalid       = unchecked((int)0x8013301F),
	GlobalInstanceTypeInvalid   = unchecked((int)0x80133020),
	InternalCompilerError       = unchecked((int)0x80133021),
	ItemCannotBeRemoved         = unchecked((int)0x80133022),
	ItemFlagNotSupported        = unchecked((int)0x80133023),
	ItemNameInUse               = unchecked((int)0x80133024),
	ItemNameInvalid             = unchecked((int)0x80133025),
	ItemNotFound                = unchecked((int)0x80133026),
	ItemTypeNotSupported        = unchecked((int)0x80133027),
	LCIDNotSupported            = unchecked((int)0x80133028),
	LoadElementFailed           = unchecked((int)0x80133029),
	NotificationInvalid         = unchecked((int)0x8013302A),
	OptionInvalid               = unchecked((int)0x8013302B),
	OptionNotSupported          = unchecked((int)0x8013302C),
	RevokeFailed                = unchecked((int)0x8013302D),
	RootMonikerAlreadySet       = unchecked((int)0x8013302E),
	RootMonikerInUse            = unchecked((int)0x8013302F),
	RootMonikerInvalid          = unchecked((int)0x80133030),
	RootMonikerNotSet           = unchecked((int)0x80133031),
	RootMonikerProtocolInvalid  = unchecked((int)0x80133032),
	RootNamespaceInvalid        = unchecked((int)0x80133033),
	RootNamespaceNotSet         = unchecked((int)0x80133034),
	SaveCompiledStateFailed     = unchecked((int)0x80133035),
	SaveElementFailed           = unchecked((int)0x80133036),
	SiteAlreadySet              = unchecked((int)0x80133037),
	SiteInvalid                 = unchecked((int)0x80133038),
	SiteNotSet                  = unchecked((int)0x80133039),
	SourceItemNotAvailable      = unchecked((int)0x8013303A),
	SourceMonikerNotAvailable   = unchecked((int)0x8013303B),
	URLInvalid                  = unchecked((int)0x8013303C),
	BrowserNotExist             = unchecked((int)0x8013303D),
	DebuggeeNotStarted          = unchecked((int)0x8013303E),
	EngineNameInvalid           = unchecked((int)0x8013303F),
	EngineNotExist              = unchecked((int)0x80133040),
	FileFormatUnsupported       = unchecked((int)0x80133041),
	FileTypeUnknown             = unchecked((int)0x80133042),
	ItemCannotBeRenamed         = unchecked((int)0x80133043),
	MissingSource               = unchecked((int)0x80133044),
	NotInitCompleted            = unchecked((int)0x80133045),
	NameTooLong                 = unchecked((int)0x80133046),
	ProcNameInUse               = unchecked((int)0x80133047),
	ProcNameInvalid             = unchecked((int)0x80133048),
	VsaServerDown               = unchecked((int)0x80133049),
	MissingPdb                  = unchecked((int)0x8013304A),
	NotClientSideAndNoUrl       = unchecked((int)0x8013304B),
	CannotAttachToWebServer     = unchecked((int)0x8013304C),
	EngineNameNotSet            = unchecked((int)0x8013304D),
	UnknownError                = unchecked((int)0x801330FF)

}; // enum VsaError

// Exceptions that are thrown by Vsa classes.
#if !ECMA_COMPAT
[Serializable]
public class VsaException : ExternalException
#else
public class VsaException : SystemException
#endif
{
	// Internal state.
	private VsaError error;

	// Constructor.
	public VsaException(VsaError error)
			: this(error, null, null) {}
	public VsaException(VsaError error, String message)
			: this(error, message, null) {}
	public VsaException(VsaError error, String message,
						Exception innerException)
			: base(message, innerException)
			{
				this.error = error;
			#if !ECMA_COMPAT
				this.HResult = (int)error;
			#endif
			}

	// Serialization support.
#if CONFIG_SERIALIZATION
	public VsaException(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				error = (VsaError)(info.GetInt32("VsaException_HResult"));
			#if !ECMA_COMPAT
				HResult = (int)error;
				HelpLink = info.GetString("VsaException_HelpLink");
				Source = info.GetString("VsaException_Source");
			#endif
			}
	public override void GetObjectData(SerializationInfo info,
									   StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("VsaException_HResult", (int)error);
			#if !ECMA_COMPAT
				info.AddValue("VsaException_HelpLink", HelpLink);
				info.AddValue("VsaException_Source", Source);
			#endif
			}
#endif

	// Get the error code.
#if !ECMA_COMPAT
	public new VsaError ErrorCode
#else
	public VsaError ErrorCode
#endif
			{
				get
				{
					return error;
				}
			}

	// Convert this exception into a string.
	public override String ToString()
			{
				return base.ToString() + Environment.NewLine +
					   "ErrorCode: " + error;
			}

}; // class VsaException

// Attribute that indicates that an assembly was generated by Vsa routines.
[AttributeUsage(AttributeTargets.All)]
public class VsaModule : System.Attribute
{
	// Internal state.
	private bool isVsaModule;

	// Constructor.
	public VsaModule(bool bIsVsaModule)
			{
				isVsaModule = bIsVsaModule;
			}

	// Get or set the flag.
	public bool IsVsaModule
			{
				get
				{
					return isVsaModule;
				}
				set
				{
					isVsaModule = value;
				}
			}

}; // class VsaModule

// Vsa loader.  Not used in this implementation.
public sealed class VsaLoader : IVsaEngine
{
	// Internal state.
	private Assembly assembly;
	private Evidence evidence;
	private bool generateDebugInfo;
	private bool isCompiled;
	private bool isDirty;
	private bool isRunning;
	private IVsaItems items;
	private String language;
	private int lcid;
	private String name;
	private String rootMoniker;
	private String rootNamespace;
	private IVsaSite site;
	private String version;

	// Constructor.
	public VsaLoader()
			{
				rootNamespace = String.Empty;
			}

	// Implement the IVsaEngine interface.
	public Assembly Assembly
			{
				get
				{
					return assembly;
				}
			}
	public Evidence Evidence
			{
				get
				{
					return evidence;
				}
				set
				{
					evidence = value;
				}
			}
	public bool GenerateDebugInfo
			{
				get
				{
					return generateDebugInfo;
				}
				set
				{
					generateDebugInfo = value;
				}
			}
	public bool IsCompiled
			{
				get
				{
					return isCompiled;
				}
			}
	public bool IsDirty
			{
				get
				{
					return isDirty;
				}
			}
	public bool IsRunning
			{
				get
				{
					return isRunning;
				}
			}
	public IVsaItems Items
			{
				get
				{
					return items;
				}
			}
	public String Language
			{
				get
				{
					return language;
				}
			}
	public int LCID
			{
				get
				{
					return lcid;
				}
				set
				{
					lcid = value;
				}
			}
	public String Name
			{
				get
				{
					return name;
				}
			}
	public String RootMoniker
			{
				get
				{
					return rootMoniker;
				}
				set
				{
					rootMoniker = value;
				}
			}
	public String RootNamespace
			{
				get
				{
					return rootNamespace;
				}
				set
				{
					rootNamespace = value;
				}
			}
	public IVsaSite Site
			{
				get
				{
					return site;
				}
				set
				{
					site = value;
				}
			}
	public String Version
			{
				get
				{
					return version;
				}
			}
	public void Close()
			{
				site = null;
				Reset();
			}
	public bool Compile()
			{
				throw new NotSupportedException();
			}
	public Object GetOption(String name)
			{
				// Nothing to do here.
				return null;
			}
	public void InitNew()
			{
				// Nothing to do here.
			}
	public bool IsValidIdentifier(String identifier)
			{
				throw new NotSupportedException();
			}
	public void LoadSourceState(IVsaPersistSite site)
			{
				throw new NotSupportedException();
			}
	public void Reset()
			{
				// Nothing to do here.
			}
	public void RevokeCache()
			{
				// Nothing to do here.
			}
	public void Run()
			{
				// Nothing to do here.
			}
	public void SaveCompiledState(out byte[] pe, out byte[] pdb)
			{
				throw new NotSupportedException();
			}
	public void SaveSourceState(IVsaPersistSite site)
			{
				throw new NotSupportedException();
			}
	public void SetOption(String name, Object value)
			{
				// Nothing to do here.
			}

}; // class VsaLoader

} // namespace Microsoft.Vsa
