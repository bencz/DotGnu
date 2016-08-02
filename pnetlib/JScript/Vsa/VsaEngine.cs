/*
 * VsaEngine.cs - front-end interface to the JScript engine.
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
 
namespace Microsoft.JScript.Vsa
{

using System;
using System.IO;
using System.Text;
using System.Reflection;
using Microsoft.JScript;
using Microsoft.Vsa;

public class VsaEngine : BaseVsaEngine, IRedirectOutput
{
	// Internal state.
	private static VsaEngine primaryEngine;
	private bool detached;
	internal bool printSupported;
	internal EngineInstance engineInstance;
	private LenientGlobalObject lenientGlobalObject;
	private GlobalScope globalScope;
	private Object[] scopeStack;
	private int scopeStackSize;
	private Object currentScope;

	// Constructors.
	public VsaEngine() : this(true) {}
	public VsaEngine(bool fast)
			: base("JScript", "7.0.3300", true)
			{
				vsaItems = new VsaItems(this);
				detached = false;
				printSupported = false;
				engineInstance = new EngineInstance(this);
				engineInstance.Init();

				// Set the global context to this engine if we are
				// the only engine instance in the system.
				Globals.SetContextEngine(this);

				// Create the global object that contains all of
				// the definitions in the engine's global scope.
				lenientGlobalObject = new LenientGlobalObject(this);

				// Create the global scope object.
				globalScope = new GlobalScope
					(null, lenientGlobalObject.globalObject);

				// Initialize the scope stack.
				scopeStack = new Object [8];
				scopeStack[0] = globalScope;
				scopeStackSize = 1;
				currentScope = globalScope;
			}

	// Clone an application domain's engine.
	public virtual IVsaEngine Clone(AppDomain domain)
			{
				throw new NotImplementedException();
			}

	// Run this engine in a specified application domain.
	public virtual void Run(AppDomain domain)
			{
				throw new NotImplementedException();
			}

	// Compile an "empty" script context.
	public virtual bool CompileEmpty()
			{
				lock(this)
				{
					return DoCompile();
				}
			}

	// Run an "empty" script context.
	public virtual void RunEmpty()
			{
				// Let the base class do the work.
				base.Run();
			}

	// Connect up the events in use by this engine.
	public virtual void ConnectEvents()
			{
				// Noting to do here.
			}

	// Disconnect the events in use by this engine.
	public virtual void DisconnectEvents()
			{
				// Noting to do here.
			}

	// Register an event source with this engine.
	public virtual void RegisterEventSource(String name)
			{
				// Nothing to do here.
			}

	// Get the assembly or assembly builder in use.
	public virtual Assembly GetAssembly()
			{
				// We don't use assemblies in this implementation.
				return null;
			}

	// Get the module or module builder in use.
	public virtual Module GetModule()
			{
				// We don't use modules in this implementation.
				return null;
			}

	// Get the global script scope for this engine.
	public virtual IVsaScriptScope GetGlobalScope()
			{
				// As far as we can tell, this API is obsolete or it was
				// never actually used for anything.  Let us know if you
				// find something important that depends upon it.
				return null;
			}

	// Get the main scope.
	public virtual GlobalScope GetMainScope()
			{
				return globalScope;
			}

	// Interrupt the engine if it is running in a separate thread.
	public virtual void Interrupt()
			{
				// We don't support threading yet.
			}

	// Determine if an identifier is valid for this engine.
	public override bool IsValidIdentifier(String identifier)
			{
				if(identifier == null)
				{
					return false;
				}
				JSScanner scanner = new JSScanner(identifier);
				if(scanner.FetchIdentifier() == null)
				{
					return false;
				}
				return (scanner.Fetch() == -1);
			}

	// Internal implementation of "Close"
	protected override void DoClose()
			{
				// Close all code items in the engine.
				((VsaItems)vsaItems).Close();
				vsaItems = null;

				// Clear the engine site, which will no longer be required.
				engineSite = null;

				// Reset global variables that may refer to this instance.
				lock(typeof(VsaEngine))
				{
					if(primaryEngine == this)
					{
						primaryEngine = null;
					}
					if(Globals.contextEngine == this)
					{
						Globals.contextEngine = null;
#if !CONFIG_SMALL_CONSOLE
						ScriptStream.Out = Console.Out;
						ScriptStream.Error = Console.Error;
#endif
					}
				}

#if !ECMA_COMPAT
				// Force a garbage collection to clean everything up.
				GC.Collect();
#endif
			}

	// Internal implementation of "Compile".
	protected override bool DoCompile()
			{
				failedCompilation = false;
				if(vsaItems != null)
				{
					failedCompilation = !(((VsaItems)vsaItems).Compile());
				}
				return !failedCompilation;
			}

	// Load the compiled state into the application domain.
	protected override Assembly LoadCompiledState()
			{
				return base.LoadCompiledState();
			}

	// Internal implementation of "LoadSourceState".
	protected override void DoLoadSourceState(IVsaPersistSite site)
			{
				// Nothing to do here - source loading is not supported.
			}

	// Internal implementation of "SaveCompiledState".
	protected override void DoSaveCompiledState
				(out byte[] pe, out byte[] debugInfo)
			{
				// Saving scripts to assembly form is not supported.
				throw new VsaException(VsaError.SaveCompiledStateFailed);
			}

	// Internal implementation of "SaveSourceState".
	protected override void DoSaveSourceState(IVsaPersistSite site)
			{
				// Nothing to do here - source saving is not supported.
			}

	// Run the compiled script.
	internal override void DoRun()
			{
				if(vsaItems != null)
				{
					((VsaItems)vsaItems).Run();
				}
			}

	// Get a custom option value.
	protected override Object GetCustomOption(String name)
			{
				// Check options that we understand.
				if(String.Compare(name, "detach", true) == 0)
				{
					return detached;
				}
				else if(String.Compare(name, "print", true) == 0)
				{
					return printSupported;
				}

				// The option was not understood.
				throw new VsaException(VsaError.OptionNotSupported);
			}

	// Determine if a namespace name is valid.
	protected override bool IsValidNamespaceName(String name)
			{
				if(name == null)
				{
					return false;
				}
				JSScanner scanner = new JSScanner(name);
				if(scanner.FetchIdentifier() == null)
				{
					return false;
				}
				while(scanner.Peek() == '.')
				{
					scanner.Fetch();
					if(scanner.FetchIdentifier() == null)
					{
						return false;
					}
				}
				return (scanner.Fetch() == -1);
			}

	// Internal implementation of "Reset".
	protected override void ResetCompiledState()
			{
				compiledRootNamespace = null;
				failedCompilation = true;
				haveCompiledState = false;
				((VsaItems)vsaItems).Reset();
				isEngineCompiled = false;
				isEngineRunning = false;
			}

	// Set a custom option value.
	protected override void SetCustomOption(String name, Object value)
			{
				// Handle the "detach" option, which allows us to support
				// more than one engine instance per process.
				if(String.Compare(name, "detach", true) == 0)
				{
					if(value is Boolean && ((bool)value) && !detached)
					{
						DetachEngine();
					}
					return;
				}
				else if(String.Compare(name, "print", true) == 0)
				{
					printSupported = (value is Boolean && ((bool)value));
					return;
				}

				// The option is not understood.
				throw new VsaException(VsaError.OptionNotSupported);
			}

	// Validate a root moniker.
	protected override void ValidateRootMoniker(String rootMoniker)
			{
				base.ValidateRootMoniker(rootMoniker);
			}

	// Implement the IRedirectOutput interface.
	public virtual void SetOutputStream(IMessageReceiver output)
			{
				// Wrap up the receiver to turn it into a stream.
				COMCharStream stream = new COMCharStream(output);
				StreamWriter writer = new StreamWriter
						(stream, Encoding.Default);

				// Do a flush after every write operation.
				writer.AutoFlush = true;

				// Set the stdout and stderr streams for the script.
				lock(typeof(VsaEngine))
				{
					if(Globals.contextEngine != this)
					{
						// This instance has been detached, so set its
						// local output streams.
						engineInstance.SetOutputStreams(writer, writer);
					}
					else
					{
#if !CONFIG_SMALL_CONSOLE
						// Use the global "ScriptStream" class for the
						// default engine instance.
						ScriptStream.Out = writer;
						ScriptStream.Error = writer;
#endif
					}
				}
			}

	// Make a new engine instance and set it up for stand-alone operation.
	internal static VsaEngine MakeNewEngine()
			{
				VsaEngine engine = new VsaEngine(true);
				engine.InitVsaEngine
					("JScript.Vsa.VsaEngine://Microsoft.JScript.VsaEngine.Vsa",
					 new ThrowOnErrorVsaSite());
				return engine;
			}

	// Create the primary engine instance.
	public static VsaEngine CreateEngine()
			{
				lock(typeof(VsaEngine))
				{
					if(primaryEngine == null)
					{
						primaryEngine = MakeNewEngine();
					}
					return primaryEngine;
				}
			}

	// Detach this engine from the primary position.
	private void DetachEngine()
			{
				lock(typeof(VsaEngine))
				{
					if(this == primaryEngine)
					{
						primaryEngine = null;
					}
					if(this == Globals.contextEngine)
					{
						Globals.contextEngine = null;
					}
					engineInstance.DetachOutputStreams();
					detached = true;
				}
			}

	// Create the primary engine instance for a given DLL type.
	public static VsaEngine CreateEngineWithType
				(RuntimeTypeHandle callingTypeHandle)
			{
				// Use the primary engine in the current process.
				return CreateEngine();
			}

	// Create an engine and get its global scope.
	public static GlobalScope CreateEngineAndGetGlobalScope
				(bool fast, String[] AssemblyNames)
			{
				// We don't support assembly-based JScript executions.
				throw new NotSupportedException();
			}

	// Create an engine and get its global scope.
	public static GlobalScope CreateEngineAndGetGlobalScopeWithType
				(bool fast, String[] AssemblyNames,
				 RuntimeTypeHandle callingTypeHandle)
			{
				// We don't support assembly-based JScript executions.
				throw new NotSupportedException();
			}

	// Initialize a VSA engine in a non-VSA mode manually.
	// Use "CreateEngine" instead of this.
	public void InitVsaEngine(String rootMoniker, IVsaSite site)
			{
				rootNamespace = "JScript.DefaultNamespace";
				engineMoniker = rootMoniker;
				engineSite = site;
				isEngineDirty = true;
				isEngineCompiled = false;
				isEngineInitialized = true;
			}

	// Get the original constructors for various types.
	public ObjectConstructor GetOriginalObjectConstructor()
			{
				return engineInstance.GetObjectConstructor();
			}
	public ArrayConstructor GetOriginalArrayConstructor()
			{
				return engineInstance.GetArrayConstructor();
			}
#if false
	public RegExpConstructor GetOriginalRegExpConstructor()
			{
				// TODO
				return null;
			}
#endif

	// Get the lenient global object associated with this engine.
	public LenientGlobalObject LenientGlobalObject
			{
				get
				{
					return lenientGlobalObject;
				}
			}

	// Push an object onto the script object stack.
	public void PushScriptObject(ScriptObject obj)
			{
				if(scopeStackSize >= scopeStack.Length)
				{
					Object[] stack = new Object [scopeStackSize + 8];
					Array.Copy(scopeStack, 0, stack, 0, scopeStackSize);
					scopeStack = stack;
				}
				scopeStack[scopeStackSize++] = obj;
				currentScope = obj;
			}

	// Push an object onto the script object stack, and check for overflow.
	internal void PushScriptObjectChecked(ScriptObject obj)
			{
				if(scopeStackSize > 500)
				{
					throw new JScriptException(JSError.OutOfStack);
				}
				else
				{
					PushScriptObject(obj);
				}
			}

	// Pop an object from the script object stack.
	public ScriptObject PopScriptObject()
			{
				// Never pop the global object.
				if(scopeStackSize > 1)
				{
					--scopeStackSize;
					currentScope = scopeStack[scopeStackSize - 1];
				}
				return (ScriptObject)currentScope;
			}

	// Get the object at the top of the script object stack.
	public ScriptObject ScriptObjectStackTop()
			{
				return (ScriptObject)currentScope;
			}

	// Get the current size of the script object stack.
	internal int ScriptObjectStackSize()
			{
				return scopeStackSize;
			}

	// Reset the script object stack to a particular size.
	internal void ScriptObjectStackSetSize(int size)
			{
				if(size < scopeStackSize)
				{
					scopeStackSize = size;
				}
			}

	// Reset the engine.
	public override void Reset()
			{
				// Let the base class do the work.
				base.Reset();
			}

	// Debugger entry point: restart the engine for another expression.
	public virtual void Restart()
			{
				// Nothing to do here - debugging not yet supported.
			}

}; // class VsaEngine

}; // namespace Microsoft.JScript.Vsa
