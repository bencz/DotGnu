/*
 * Thread.cs - Implementation of the "System.Threading.Thread" class.
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

namespace System.Threading
{

using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Security.Principal;
using System.Diagnostics;
using System.Globalization;

public sealed class Thread
{
	// Note: the private fields must be in the order below,
	// because the runtime engine assumes they are this way.

	// Internal runtime handle for the thread.
	private IntPtr privateData;
	
	// Flagged true if the thread has been created from managed code.
	private bool createdFromManagedCode;

	// State information for abort exceptions.
	private Object stateInfo;

	// Entry point for the thread.
	private ThreadStart start;

	// Name of this thread.
	private String name;

#if CONFIG_REMOTING

	// The context for this thread.
	private Context context;

#endif // CONFIG_REMOTING

#if CONFIG_POLICY_OBJECTS

	// The security principal for this thread.
	private IPrincipal principal;

#endif // CONFIG_POLICY_OBJECTS

#if !ECMA_COMPAT
	// Flag that is set for threads in the thread pool.
	internal bool inThreadPool;
#endif

	// Private constructor only called by the engine
	private Thread(IntPtr privateData)
	{
		InitializeManaged(null, privateData);
	}
	
	// Constructor.
	public Thread(ThreadStart start)
			{
				if(start == null)
				{
					throw new ArgumentNullException("start");
				}

				this.createdFromManagedCode = true;
				
				InitializeManaged(start, IntPtr.Zero);

				InitializeThread();
			}

	// Destructor.
	~Thread()
			{
				FinalizeThread();
			}

	private void InitializeManaged(ThreadStart start, IntPtr privateData)
			{
				this.start = start;
				this.privateData = privateData;

#if CONFIG_POLICY_OBJECTS
				principal = new GenericPrincipal(new GenericIdentity(""), null);
#endif								
			}
				
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private void InitializeThread();

	// Internal finalization for threads.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private void FinalizeThread();

	// Abort this thread.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public void Abort();

	// Abort this thread with a particular state object.
	public void Abort(Object stateInfo)
			{
				if(this.stateInfo == null)
				{
					this.stateInfo = stateInfo;
				}
				Abort();
			}

#if CONFIG_RUNTIME_INFRA
	// Get the application domain of the currently executing thread.
	public static AppDomain GetDomain()
			{
				return AppDomain.CurrentDomain;
			}
#endif

	// Join with this thread.
	public void Join()
			{
				InternalJoin(-1);
			}

	// Join with this thread, stopping after a specified timeout.
	public bool Join(int millisecondsTimeout)
			{
				if(millisecondsTimeout < -1)
				{
					throw new ArgumentOutOfRangeException
						("millisecondsTimeout",
						 _("ArgRange_NonNegOrNegOne"));
				}
				return InternalJoin(millisecondsTimeout);
			}
	public bool Join(TimeSpan timeout)
			{
				return InternalJoin(Monitor.TimeSpanToMS(timeout));
			}

	// Internal version of "Join".  A timeout of -1 indicates
	// infinite, and zero indicates "test and return immediately".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private bool InternalJoin(int timeout);

	// Insert a memory barrier, which guarantees that all
	// pending memory loads and stores will be flushed.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void MemoryBarrier();

	// Reset the pending thread abort on the current thread.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void ResetAbort();

	// Sleep for a specific period of time.
	public static void Sleep(int millisecondsTimeout)
			{
				if(millisecondsTimeout < -1)
				{
					throw new ArgumentOutOfRangeException
						("millisecondsTimeout",
						 _("ArgRange_NonNegOrNegOne"));
				}
				InternalSleep(millisecondsTimeout);
			}
	public static void Sleep(TimeSpan timeout)
			{
				InternalSleep(Monitor.TimeSpanToMS(timeout));
			}

	// Internal version of "Sleep".  A timeout of -1 indicates
	// infinite, and zero indicates return immediately.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void InternalSleep(int timeout);

	// Start the thread executing.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public void Start();

	// Perform volatile read operations.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static byte VolatileRead(ref byte address);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[CLSCompliant(false)]
	extern public static sbyte VolatileRead(ref sbyte address);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static short VolatileRead(ref short address);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[CLSCompliant(false)]
	extern public static ushort VolatileRead(ref ushort address);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static int VolatileRead(ref int address);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[CLSCompliant(false)]
	extern public static uint VolatileRead(ref uint address);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static long VolatileRead(ref long address);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[CLSCompliant(false)]
	extern public static ulong VolatileRead(ref ulong address);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static IntPtr VolatileRead(ref IntPtr address);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[CLSCompliant(false)]
	extern public static UIntPtr VolatileRead(ref UIntPtr address);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static float VolatileRead(ref float address);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static double VolatileRead(ref double address);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Object VolatileRead(ref Object address);

	// Perform volatile write operations.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void VolatileWrite(ref byte address, byte value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[CLSCompliant(false)]
	extern public static void VolatileWrite(ref sbyte address, sbyte value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void VolatileWrite(ref short address, short value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[CLSCompliant(false)]
	extern public static void VolatileWrite(ref ushort address, ushort value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void VolatileWrite(ref int address, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[CLSCompliant(false)]
	extern public static void VolatileWrite(ref uint address, uint value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void VolatileWrite(ref long address, long value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[CLSCompliant(false)]
	extern public static void VolatileWrite(ref ulong address, ulong value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void VolatileWrite(ref IntPtr address, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[CLSCompliant(false)]
	extern public static void VolatileWrite(ref UIntPtr address, UIntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void VolatileWrite(ref float address, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void VolatileWrite(ref double address, double value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void VolatileWrite(ref Object address, Object value);

	// Properties.
	public static Thread CurrentThread
			{
				get
				{
					return InternalCurrentThread();
				}
			}
	public bool IsAlive
			{
				get
				{
					return ((InternalGetState() &
								(ThreadState.Unstarted |
								 ThreadState.Stopped)) == 0);
				}
			}
	public bool IsBackground
			{
				get
				{
					return ((InternalGetState() & ThreadState.Background) != 0);
				}
				set
				{
					InternalSetBackground(value);
				}
			}
	public String Name
			{
				get
				{
					return name;
				}
				set
				{
					lock(this)
					{
						if(name != null)
						{
							throw new InvalidOperationException
								(_("Invalid_WriteOnce"));
						}
						name = value;
					}
				}
			}
	public ThreadPriority Priority
			{
				get
				{
					return InternalGetPriority();
				}
				set
				{
					InternalSetPriority(value);
				}
			}
	public System.Threading.ThreadState ThreadState
			{
				get
				{
					return InternalGetState();
				}
			}

	// Internal version of "CurrentThread".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static Thread InternalCurrentThread();

	// Internal version of "IsBackground".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private void InternalSetBackground(bool value);

	// Internal version of "Priority".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private ThreadPriority InternalGetPriority();

	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private void InternalSetPriority(ThreadPriority value);

	// Internal version of "ThreadState".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private System.Threading.ThreadState InternalGetState();

	// Get the packed stack trace information for this thread.
	internal PackedStackFrame[] GetPackedStackTrace()
			{
				// We don't currently support getting the stack trace
				// for a foreign thread.  It is too risky security-wise.
				return null;
			}

	// Determine if the runtime engine can start threads.
	// Returns false on a single-threaded system.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static bool CanStartThreads();

	// Get the identifier for the current thread.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern internal static int InternalGetThreadId();

#if !ECMA_COMPAT

	// Allocate a local data store slot.
	public static LocalDataStoreSlot AllocateDataSlot()
			{
				return new LocalDataStoreSlot(false, null);
			}

	// Allocate a named data store slot.
	public static LocalDataStoreSlot AllocateNamedDataSlot(String name)
			{
				return LocalDataStoreSlot.GetNamed(name);
			}

	// Free a named data store slot.
	public static void FreeNamedDataSlot(String name)
			{
				LocalDataStoreSlot.FreeNamed(name);
			}

	// Get the compressed stack for a thread.
	public CompressedStack GetCompressedStack()
			{
				return CompressedStack.GetCompressedStack();
			}

	// Get the data in a particular data store slot.
	public static Object GetData(LocalDataStoreSlot slot)
			{
				if(slot == null)
				{
					return null;
				}
				else
				{
					return slot.Data;
				}
			}

	// Get the current domain identifier.
	public static int GetDomainID()
			{
				return GetDomain().domainID;
			}

	// Get a previously allocated named data store slot.
	public static LocalDataStoreSlot GetNamedDataSlot(String name)
			{
				return LocalDataStoreSlot.GetNamed(name);
			}

	// Interrupt this thread.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public void Interrupt();

	// Resume execution of this thread.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public void Resume();

	// Set the compressed stack for a thread.
	public void SetCompressedStack(CompressedStack stack)
			{
				// Ignored - not used in this implementation.
			}

	// Set the data in a particular local data store slot.
	public static void SetData(LocalDataStoreSlot slot, Object data)
			{
				if(slot != null)
				{
					slot.Data = data;
				}
			}

	// Perform a spin wait for a given number of iterations.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static void SpinWait(int iterations);

	// Suspend execution of this thread.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public void Suspend();

	// Get or set this thread's apartment state.
	public ApartmentState ApartmentState
			{
				get
				{
					return ApartmentState.Unknown;
				}
				set
				{
					// Ignored - we don't use apartment states.
				}
			}

#if CONFIG_REMOTING

	// Get the remoting context for the current thread.
	public static Context CurrentContext
			{
				get
				{
					Thread thread = CurrentThread;
					if(thread.context == null)
					{
						thread.context = Context.DefaultContext;
					}
					return thread.context;
				}
			}

#endif // CONFIG_REMOTING

	// Get or set the current culture for the thread.
	public CultureInfo CurrentCulture
			{
				get
				{
					return CultureInfo.CurrentCulture;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					CultureInfo.SetCurrentCulture(value);
				}
			}

#if CONFIG_POLICY_OBJECTS

	// Get or set the principal representing the thread's security context.
	public static IPrincipal CurrentPrincipal
			{
				get
				{
					return CurrentThread.principal;
				}
				set
				{
					CurrentThread.principal = value;
				}
			}

#endif

	// Get or set the current UI culture for the thread.
	public CultureInfo CurrentUICulture
			{
				get
				{
					return CultureInfo.CurrentUICulture;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					CultureInfo.SetCurrentUICulture(value);
				}
			}

	// Determine if this is a thread pool thread.
	public bool IsThreadPoolThread
			{
				get
				{
					return inThreadPool;
				}
			}

#endif // !ECMA_COMPAT

}; // class Thread

}; // namespace System.Threading
