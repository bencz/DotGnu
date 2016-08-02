/*
 * pthread_glue.cs - Glue routines for Pthreads.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

using System;
using System.Threading;
using System.Runtime.InteropServices;
using OpenSystem.C;

namespace OpenSystem.C
{

// Class that manages the startup process for a thread.
internal unsafe class ThreadStartup
{
	// Internal state.
	private long id;
	private void *start;
	private void *arg;
	private void *exitval;
	private bool detached;
	private bool exited;
	private bool cancelled;
	private int cancelState;
	private int cancelType;

	// Constructor.
	public ThreadStartup(void *start, void *arg)
			{
				this.id = id;
				this.start = start;
				this.arg = arg;
				this.exitval = null;
				this.detached = false;
				this.exited = false;
				this.cancelled = false;
				this.cancelState = 1;	/* disabled */
				this.cancelType = 0;	/* deferred */
			}

	// Set the thread identifier.
	public void SetThreadId(long id)
			{
				this.id = id;
			}

	// The method to invoke to start the thread.
	public void Start()
			{
				try
				{
					__module.__libc_thread_set_self(id);
					exitval = __module.__pt_thread_run(id, start, arg);
				}
				finally
				{
					__module.__pt_destroy_keys();
					exited = true;
					if(detached)
					{
						__module.__libc_thread_unregister(id);
					}
				}
			}

	// Get or set the exit value.
	public void *ExitValue
			{
				get
				{
					return exitval;
				}
				set
				{
					exitval = value;
				}
			}

	// Get or set the detached state.
	public bool Detached
			{
				get
				{
					return detached;
				}
				set
				{
					detached = value;
				}
			}

	// Get or set the exited state.
	public bool Exited
			{
				get
				{
					return exited;
				}
				set
				{
					exited = value;
				}
			}

	// Get or set the cancelled state.
	public bool Cancelled
			{
				get
				{
					return cancelled;
				}
				set
				{
					cancelled = value;
				}
			}

	// Get or set the cancellation state.
	public int CancelState
			{
				get
				{
					return cancelState;
				}
				set
				{
					cancelState = value;
				}
			}

	// Get or set the cancellation type.
	public int CancelType
			{
				get
				{
					return cancelType;
				}
				set
				{
					cancelType = value;
				}
			}

}; // class ThreadStartup

// Helper structure for getting around oddities in "ref" for spinlocks.
[StructLayout(LayoutKind.Sequential)]
internal struct SpinLock
{
	public int value;

}; // struct SpinLock

}; // namespace OpenSystem.C

__module
{

	// Import functions from "pthread_thread.c" and "pthread_key.c".
	extern public static unsafe void *__pt_thread_run(long id, void *start, void *arg);
	extern public static unsafe void pthread_exit(void *exitval);
	extern public static unsafe void __pt_destroy_keys();

	// Import the thread management functions from "libc".
	extern public static long __pthread_self();
	extern public static void __libc_thread_set_self(long id);
	extern public static long __libc_thread_register
				(Thread thread, Object state);
	extern public static void __libc_thread_unregister(long id);
	extern public static Thread __libc_thread_object(long id);
	extern public static Object __libc_thread_state(long id);

	// Get the location of the "errno" variable from "libc".
	extern public static unsafe int *__errno_location();

	// Create a new thread, and then call back to "__pt_thread_run".
	public static unsafe long __pt_thread_create(void *start, void *arg)
			{
				ThreadStartup startup = new ThreadStartup(start, arg);
				Thread thread = new Thread(new ThreadStart(startup.Start));
				thread.IsBackground = true;
				long id = __libc_thread_register(thread, startup);
				startup.SetThreadId(id);
				thread.Start();
				return id;
			}

	// Join to an exiting thread.
	public static unsafe int pthread_join(long id, void **thread_return)
			{
				Thread thread = __libc_thread_object(id);
				ThreadStartup startup = (ThreadStartup)
					__libc_thread_state(id);
				if(thread == null)
				{
					*(__errno_location()) = 3;	/* ESRCH */
					return -1;
				}
				pthread_testcancel();
				thread.Join();
				if(startup != null)
				{
					if(thread_return != null)
					{
						*thread_return = startup.ExitValue;
					}
				}
				else if(thread_return != null)
				{
					// Foreign thread with an unknown exit value.
					*thread_return = null;
				}
				__libc_thread_unregister(id);
				pthread_testcancel();
				return 0;
			}

	// Detach a thread so that we don't need to worry about a join.
	public static unsafe int pthread_detach(long id)
			{
				Thread thread = __libc_thread_object(id);
				ThreadStartup startup = (ThreadStartup)
					__libc_thread_state(id);
				if(thread == null)
				{
					*(__errno_location()) = 3;	/* ESRCH */
					return -1;
				}
				if(startup != null)
				{
					startup.Detached = true;
					if(startup.Exited)
					{
						__libc_thread_unregister(id);
					}
				}
				return 0;
			}

	// Cancel a thread.
	public static unsafe int pthread_cancel(long id)
			{
				Thread thread = __libc_thread_object(id);
				ThreadStartup startup = (ThreadStartup)
					__libc_thread_state(id);
				if(thread == null)
				{
					*(__errno_location()) = 3;	/* ESRCH */
					return -1;
				}
				if(startup != null && startup.CancelState == 0)
				{
					startup.Cancelled = true;
				}
				return 0;
			}

	// Test to see if the current thread has been cancelled.
	public static void pthread_testcancel()
			{
				ThreadStartup startup = (ThreadStartup)
					__libc_thread_state(__pthread_self());
				if(startup != null && startup.Cancelled)
				{
					pthread_exit((void *)(new IntPtr(-1)));
				}
			}

	// Set the cancellation state for a thread.
	public static unsafe int pthread_setcancelstate(int state, int *oldstate)
			{
				ThreadStartup startup = (ThreadStartup)
					__libc_thread_state(__pthread_self());
				if(startup != null)
				{
					if(oldstate != null)
					{
						*oldstate = startup.CancelState;
					}
					startup.CancelState = state;
				}
				else if(oldstate != null)
				{
					*oldstate = 1;	/* disabled */
				}
				return 0;
			}

	// Set the cancellation type for a thread.
	public static unsafe int pthread_setcanceltype(int type, int *oldtype)
			{
				ThreadStartup startup = (ThreadStartup)
					__libc_thread_state(__pthread_self());
				if(startup != null)
				{
					if(oldtype != null)
					{
						*oldtype = startup.CancelType;
					}
					startup.CancelState = type;
				}
				else if(oldtype != null)
				{
					*oldtype = 0;	/* deferred */
				}
				return 0;
			}

	// Determine if a thread is detached.
	public static unsafe int __pt_thread_is_detached(long id)
			{
				Thread thread = __libc_thread_object(id);
				ThreadStartup startup = (ThreadStartup)
					__libc_thread_state(id);
				if(thread == null)
				{
					return -1;
				}
				if(startup != null)
				{
					return (startup.Detached ? 1 : 0);
				}
				else
				{
					return 0;
				}
			}

	// Initialize a spin lock.
	public static unsafe int pthread_spin_init(int *_lock, int pshared)
			{
				Interlocked.Exchange(ref ((SpinLock *)_lock)->value, 0);
				return 0;
			}

	// Destroy a spin lock.
	public static unsafe int pthread_spin_destroy(int *_lock)
			{
				return 0;
			}

	// Lock a spin lock.
	public static unsafe int pthread_spin_lock(int *_lock)
			{
				while(Interlocked.CompareExchange
							(ref ((SpinLock *)_lock)->value, 1, 0) != 0)
				{
					// Spin until the value was zero before the exchange.
				}
				return 0;
			}

	// Try to lock a spin lock.
	public static unsafe int pthread_spin_trylock(int *_lock)
			{
				if(Interlocked.CompareExchange(ref ((SpinLock *)_lock)->value, 1, 0) == 0)
				{
					return 0;
				}
				else
				{
					*(__errno_location()) = 16;	/* EBUSY */
					return -1;
				}
			}

	// Try to lock a spin lock, without side-effecting errno.
	public static unsafe int __pt_spin_trylock(int *_lock)
			{
				if(Interlocked.CompareExchange(ref ((SpinLock *)_lock)->value, 1, 0) == 0)
				{
					return 0;
				}
				else
				{
					return -1;
				}
			}

	// Unlock a spin lock.
	public static unsafe int pthread_spin_unlock(int *_lock)
			{
				Interlocked.Exchange(ref ((SpinLock *)_lock)->value, 0);
				return 0;
			}

} // __module
