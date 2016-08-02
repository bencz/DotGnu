/*
 * BaseVsaEngine.cs - front-end interface to the JScript engine.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.Vsa
{

using System;
using System.Collections;
using System.Reflection;
using System.Globalization;
using System.Security.Policy;

public abstract class BaseVsaEngine : IVsaEngine
{
	// Accessible internal state.
	protected String applicationPath;
	protected String assemblyVersion;
	protected String compiledRootNamespace;
	protected String engineMoniker;
	protected String engineName;
	protected IVsaSite engineSite;
	protected int errorLocale;
	protected Evidence executionEvidence;
	protected bool failedCompilation;
	protected bool genDebugInfo;
	protected bool haveCompiledState;
	protected bool isClosed;
	protected bool isDebugInfoSupported;
	protected bool isEngineCompiled;
	protected bool isEngineDirty;
	protected bool isEngineInitialized;
	protected bool isEngineRunning;
	protected Assembly loadedAssembly;
	protected String rootNamespace;
	protected String scriptLanguage;
	protected Type startupClass;
	protected BaseVsaStartup startupInstance;
	protected IVsaItems vsaItems;
	protected static Hashtable nameTable = new Hashtable();

	// Constructor.
	internal BaseVsaEngine(String language, String version, bool supportDebug)
			{
				applicationPath = String.Empty;
				assemblyVersion = version;
				compiledRootNamespace = null;
				engineMoniker = String.Empty;
				engineName = String.Empty;
				engineSite = null;
				errorLocale = CultureInfo.CurrentCulture.LCID;
				executionEvidence = null;
				failedCompilation = false;
				genDebugInfo = false;
				haveCompiledState = false;
				isClosed = false;
				isDebugInfoSupported = supportDebug;
				isEngineCompiled = false;
				isEngineDirty = false;
				isEngineInitialized = false;
				isEngineRunning = false;
				loadedAssembly = null;
				rootNamespace = String.Empty;
				scriptLanguage = language;
				startupClass = null;
				startupInstance = null;
				vsaItems = null;
			}

	// Implement the IVsaEngine interface.
	public Assembly Assembly
			{
				get
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed | Pre.EngineRunning);
						return loadedAssembly;
					}
				}
			}
	public Evidence Evidence
			{
				get
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.EngineInitialized);
						return executionEvidence;
					}
				}
				set
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.EngineNotRunning |
									  Pre.EngineInitialized);
						executionEvidence = value;
					}
				}
			}
	public bool GenerateDebugInfo
			{
				get
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.EngineInitialized);
						return genDebugInfo;
					}
				}
				set
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.EngineNotRunning |
									  Pre.EngineInitialized |
									  Pre.SupportForDebug);
						if(genDebugInfo != value)
						{
							isEngineCompiled = false;
							isEngineDirty = true;
							genDebugInfo = value;
						}
					}
				}
			}
	public bool IsCompiled
			{
				get
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.EngineInitialized);
						return isEngineCompiled;
					}
				}
			}
	public bool IsDirty
			{
				get
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.EngineInitialized);
						return isEngineDirty;
					}
				}
				set
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed);
						isEngineDirty = value;
						if(value)
						{
							isEngineCompiled = false;
						}
					}
				}
			}
	public bool IsRunning
			{
				get
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.EngineInitialized);
						return isEngineRunning;
					}
				}
			}
	public IVsaItems Items
			{
				get
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.EngineInitialized);
						return vsaItems;
					}
				}
			}
	public String Language
			{
				get
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.EngineInitialized);
						return scriptLanguage;
					}
				}
			}
	public int LCID
			{
				get
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.EngineInitialized);
						return errorLocale;
					}
				}
				set
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.EngineNotRunning |
									  Pre.EngineInitialized);
						try
						{
							// Validate the culture using "CultureInfo".
							CultureInfo culture =
								new CultureInfo(value);
						}
						catch(ArgumentException)
						{
							throw new VsaException(VsaError.LCIDNotSupported);
						}
						isEngineDirty = true;
						isEngineCompiled = false;
						errorLocale = value;
					}
				}
			}
	public String Name
			{
				get
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.EngineInitialized);
						return engineName;
					}
				}
				set
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.EngineNotRunning |
									  Pre.EngineInitialized);
						if(engineName != value)
						{
							// We should check to see if the name is in
							// use by some other engine, but since we don't
							// use the name for anything, and there can only
							// be one instance in use at a time, it isn't
							// worth doing the check.
							isEngineDirty = true;
							isEngineCompiled = false;
							engineName = value;
						}
					}
				}
			}
	public String RootMoniker
			{
				get
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed);
						return engineMoniker;
					}
				}
				set
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.RootMonikerNotSet);
						ValidateRootMoniker(value);
						engineMoniker = value;
					}
				}
			}
	public String RootNamespace
			{
				get
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.EngineInitialized);
						return rootNamespace;
					}
				}
				set
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.EngineNotRunning |
									  Pre.EngineInitialized);
						if(IsValidNamespaceName(value))
						{
							isEngineDirty = true;
							isEngineCompiled = false;
							rootNamespace = value;
						}
						else
						{
							throw new VsaException
								(VsaError.RootNamespaceInvalid);
						}
					}
				}
			}
	public IVsaSite Site
			{
				get
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.RootMonikerSet);
						return engineSite;
					}
				}
				set
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.SiteNotSet |
									  Pre.RootMonikerSet);
						if(value != null)
						{
							engineSite = value;
						}
						else
						{
							throw new VsaException(VsaError.SiteInvalid);
						}
					}
				}
			}
	public String Version
			{
				get
				{
					lock(this)
					{
						Preconditions(Pre.EngineNotClosed |
									  Pre.EngineInitialized);
						return assemblyVersion;
					}
				}
			}
	public virtual void Close()
			{
				lock(this)
				{
					Preconditions(Pre.EngineNotClosed);
					if(isEngineRunning)
					{
						Reset();
					}
					DoClose();
					isClosed = true;
				}
			}
	public virtual bool Compile()
			{
				lock(this)
				{
					// Make sure that we are in the right state.
					Preconditions(Pre.EngineNotClosed |
								  Pre.EngineNotRunning |
								  Pre.EngineInitialized |
								  Pre.RootNamespaceSet);

					// We need to have at least one code item.
					int posn;
					for(posn = 0; posn < vsaItems.Count; ++posn)
					{
						if(vsaItems[posn].ItemType == VsaItemType.Code)
						{
							break;
						}
					}
					if(posn >= vsaItems.Count)
					{
						throw new VsaException(VsaError.EngineEmpty);
					}

					// Reset the previous compiled state and then compile.
					try
					{
						ResetCompiledState();
						isEngineCompiled = DoCompile();
					}
					catch(VsaException)
					{
						throw;
					}
					catch(Exception e)
					{
						// Wrap the exception and re-throw it.
						throw new VsaException(VsaError.InternalCompilerError,
											   e.ToString(), e);
					}

					// Update the engine state.
					if(isEngineCompiled)
					{
						haveCompiledState = true;
						failedCompilation = false;
						compiledRootNamespace = rootNamespace;
					}

					// Finished compilation.
					return isEngineCompiled;
				}
			}
	public virtual Object GetOption(String name)
			{
				lock(this)
				{
					Preconditions(Pre.EngineNotClosed |
								  Pre.EngineInitialized);
					return GetCustomOption(name);
				}
			}
	public virtual void InitNew()
			{
				lock(this)
				{
					Preconditions(Pre.EngineNotClosed |
								  Pre.EngineNotInitialized |
								  Pre.RootMonikerSet |
								  Pre.SiteSet);
					isEngineInitialized = true;
				}
			}
	public abstract bool IsValidIdentifier(String identifier);
	public virtual void LoadSourceState(IVsaPersistSite site)
			{
				lock(this)
				{
					Preconditions(Pre.EngineNotClosed |
								  Pre.EngineNotInitialized |
								  Pre.RootMonikerSet |
								  Pre.SiteSet);
					isEngineInitialized = true;
					try
					{
						DoLoadSourceState(site);
					}
					catch
					{
						// Reset the "isEngineInitialized" flag.
						isEngineInitialized = true;
						throw;
					}
					isEngineDirty = true;
				}
			}
	public virtual void Reset()
			{
				lock(this)
				{
					Preconditions(Pre.EngineNotClosed |
								  Pre.EngineRunning);
					ResetCompiledState();
					isEngineRunning = false;
					loadedAssembly = null;
				}
			}
	public virtual void RevokeCache()
			{
				lock(this)
				{
					// We don't have a cache, so just check the preconditions.
					Preconditions(Pre.EngineNotClosed |
								  Pre.RootMonikerSet |
								  Pre.EngineNotRunning);
				}
			}
	public virtual void Run()
			{
				lock(this)
				{
					// Check that the engine is in the right state.
					Preconditions(Pre.EngineNotClosed |
								  Pre.EngineNotRunning |
								  Pre.RootMonikerSet |
								  Pre.SiteSet |
								  Pre.RootNamespaceSet);
					if(haveCompiledState)
					{
						if(rootNamespace != compiledRootNamespace)
						{
							throw new VsaException
								(VsaError.RootNamespaceInvalid);
						}
						loadedAssembly = LoadCompiledState();
					}
					else if(failedCompilation)
					{
						throw new VsaException(VsaError.EngineNotCompiled);
					}

					// Run the compiled script.
					try
					{
						isEngineRunning = true;
						DoRun();
					}
					catch(VsaException)
					{
						throw;
					}
					catch(Exception e)
					{
						// Wrap the exception and re-throw.
						throw new VsaException(VsaError.UnknownError,
											   e.ToString(), e);
					}
				}
			}
	public virtual void SaveCompiledState(out byte[] pe, out byte[] pdb)
			{
				lock(this)
				{
					Preconditions(Pre.EngineNotClosed |
								  Pre.EngineNotRunning |
								  Pre.EngineCompiled |
								  Pre.EngineInitialized);
					DoSaveCompiledState(out pe, out pdb);
				}
			}
	public virtual void SaveSourceState(IVsaPersistSite site)
			{
				lock(this)
				{
					Preconditions(Pre.EngineNotClosed |
								  Pre.EngineNotRunning |
								  Pre.EngineInitialized);
					if(site != null)
					{
						try
						{
							DoSaveSourceState(site);
						}
						catch(VsaException)
						{
							throw;
						}
						catch(Exception e)
						{
							// Wrap the exception and re-throw.
							throw new VsaException(VsaError.SaveElementFailed,
												   e.ToString(), e);
						}
					}
					else
					{
						throw new VsaException(VsaError.SiteInvalid);
					}
				}
			}
	public virtual void SetOption(String name, Object value)
			{
				lock(this)
				{
					Preconditions(Pre.EngineNotClosed |
								  Pre.EngineNotRunning |
								  Pre.EngineInitialized);
					SetCustomOption(name, value);
				}
			}

	// Helper method that constructs an error exception.
	protected VsaException Error(VsaError vsaErrorNumber)
			{
				return new VsaException(vsaErrorNumber);
			}

	// Raise an error condition.
	private void Raise(VsaError vsaErrorNumber)
			{
				throw Error(vsaErrorNumber);
			}

	// Pre-conditions that may be tested by the "Preconditions" method.
	protected enum Pre
	{
		None					= 0,
		EngineNotClosed			= (1<<0),
		SupportForDebug			= (1<<1),
		EngineCompiled			= (1<<2),
		EngineRunning			= (1<<3),
		EngineNotRunning		= (1<<4),
		RootMonikerSet			= (1<<5),
		RootMonikerNotSet		= (1<<6),
		RootNamespaceSet		= (1<<7),
		SiteSet					= (1<<8),
		SiteNotSet				= (1<<9),
		EngineInitialized		= (1<<10),
		EngineInitialised		= (1<<10),
		EngineNotInitialized	= (1<<11),
		EngineNotInitialised	= (1<<11)
	}

	// Test pre-conditions and raise an error if incorrect.
	protected void Preconditions(Pre flags)
			{
				// The object must never be closed.
				if(isClosed)
				{
					Raise(VsaError.EngineClosed);
				}
				if(flags == Pre.EngineNotClosed)
				{
					return;
				}

				// Test the various conditions.
				if((flags & Pre.SupportForDebug) != Pre.None)
				{
					if(!isDebugInfoSupported)
					{
						Raise(VsaError.DebugInfoNotSupported);
					}
				}
				if((flags & Pre.EngineCompiled) != Pre.None)
				{
					if(!haveCompiledState)
					{
						Raise(VsaError.EngineNotCompiled);
					}
				}
				if((flags & Pre.EngineRunning) != Pre.None)
				{
					if(!isEngineRunning)
					{
						Raise(VsaError.EngineNotRunning);
					}
				}
				if((flags & Pre.EngineNotRunning) != Pre.None)
				{
					if(isEngineRunning)
					{
						Raise(VsaError.EngineRunning);
					}
				}
				if((flags & Pre.RootMonikerSet) != Pre.None)
				{
					if(engineMoniker == String.Empty)
					{
						Raise(VsaError.RootMonikerNotSet);
					}
				}
				if((flags & Pre.RootMonikerNotSet) != Pre.None)
				{
					if(engineMoniker != String.Empty)
					{
						Raise(VsaError.RootMonikerAlreadySet);
					}
				}
				if((flags & Pre.RootNamespaceSet) != Pre.None)
				{
					if(rootNamespace == String.Empty)
					{
						Raise(VsaError.RootNamespaceNotSet);
					}
				}
				if((flags & Pre.SiteSet) != Pre.None)
				{
					if(engineSite == null)
					{
						Raise(VsaError.SiteNotSet);
					}
				}
				if((flags & Pre.SiteNotSet) != Pre.None)
				{
					if(engineSite != null)
					{
						Raise(VsaError.SiteAlreadySet);
					}
				}
				if((flags & Pre.EngineInitialized) != Pre.None)
				{
					if(!isEngineInitialized)
					{
						Raise(VsaError.EngineNotInitialized);
					}
				}
				if((flags & Pre.EngineNotInitialized) != Pre.None)
				{
					if(isEngineInitialized)
					{
						Raise(VsaError.EngineInitialized);
					}
				}
			}

#if !ECMA_COMPAT
	// Other properties (not used by JScript).
	public _AppDomain AppDomain
			{
				get
				{
					Preconditions(Pre.EngineNotClosed);
					throw new NotSupportedException();
				}
				set
				{
					Preconditions(Pre.EngineNotClosed);
					throw new VsaException(VsaError.AppDomainCannotBeSet);
				}
			}
#endif
	public String ApplicationBase
			{
				get
				{
					Preconditions(Pre.EngineNotClosed);
					throw new NotSupportedException();
				}
				set
				{
					Preconditions(Pre.EngineNotClosed);
					throw new VsaException(VsaError.ApplicationBaseCannotBeSet);
				}
			}

	// Load the compiled state into the application domain.
	protected virtual Assembly LoadCompiledState()
			{
				// We don't support loading JScript programs as
				// compiled assemblies just yet.
				return null;
			}

	// Validate a root moniker.
	protected virtual void ValidateRootMoniker(String rootMoniker)
			{
				// We don't care about root monikers in this implementation,
				// so just verify that it isn't null.
				if(rootMoniker == null)
				{
					throw new VsaException(VsaError.RootMonikerInvalid);
				}
			}

	// Internal implementation of "Close"
	protected abstract void DoClose();

	// Internal implementation of "Compile".
	protected abstract bool DoCompile();

	// Internal implementation of "LoadSourceState".
	protected abstract void DoLoadSourceState(IVsaPersistSite site);

	// Internal implementation of "SaveCompiledState".
	protected abstract void DoSaveCompiledState
			(out byte[] pe, out byte[] debugInfo);

	// Internal implementation of "SaveSourceState".
	protected abstract void DoSaveSourceState(IVsaPersistSite site);

	// Run the compiled script.
	internal virtual void DoRun() {}

	// Get a custom option value.
	protected abstract Object GetCustomOption(String name);

	// Determine if a namespace name is valid.
	protected abstract bool IsValidNamespaceName(String name);

	// Internal implementation of "Reset".
	protected abstract void ResetCompiledState();

	// Set a custom option value.
	protected abstract void SetCustomOption(String name, Object value);

}; // class BaseVsaEngine

}; // namespace Microsoft.Vsa
