/*
 * signal-glue.cs - Glue between signal and the C# system library.
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

namespace OpenSystem.C
{

using System;
using System.Threading;

[GlobalScope]
public class LibCSignal
{

	// Control object that keeps track of signals within the system.
	private class SignalController
	{
		// Internal state.
		private static SignalController controller;
		private SignalInfo info;
		private uint process_signals;
	
		// Information about the pending signals for a specific thread.
		private class SignalInfo
		{
			public long thread;
			public uint pending;
			public int nextToTest;
			public SignalInfo next;
	
		}; // class SignalInfo
	
		// Constructor.
		private SignalController()
				{
					info = null;
					process_signals = 0;
				}

		// Get the global signal controller.
		public static SignalController Controller
				{
					get
					{
						lock(typeof(SignalController))
						{
							if(controller == null)
							{
								controller = new SignalController();
							}
							return controller;
						}
					}
				}
	
		// Initialize signal handling for a particular thread.
		public void Initialize(long thread)
				{
					lock(this)
					{
						SignalInfo current = info;
						while(current != null)
						{
							if(current.thread == thread)
							{
								return;
							}
							current = current.next;
						}
						if(current == null)
						{
							current = new SignalInfo();
							current.thread = thread;
							current.pending = 0;
							current.nextToTest = 0;
							current.next = info;
							info = current;
						}
					}
				}
	
		// Deliver a signal to a particular thread.
		public void Deliver(long thread, int signal)
				{
					lock(this)
					{
						if(thread == -1)
						{
							/* This signal was delivered process-wide */
							process_signals |= (((uint)1) << signal);
							return;
						}
						SignalInfo current = info;
						while(current != null)
						{
							if(current.thread == thread)
							{
								current.pending |= (((uint)1) << signal);
								return;
							}
							current = current.next;
						}
						current = new SignalInfo();
						current.thread = thread;
						current.pending = (((uint)1) << signal);
						current.nextToTest = 0;
						current.next = info;
						info = current;
						Monitor.PulseAll(this);
					}
				}
	
		// Suspend the current thread until a signal arrives.
		public int Suspend(long thread, uint blocked)
				{
					lock(this)
					{
						// Get the signal information block for this thread.
						SignalInfo current = info;
						while(current != null)
						{
							if(current.thread == thread)
							{
								break;
							}
							current = current.next;
						}
						if(current == null)
						{
							current = new SignalInfo();
							current.thread = thread;
							current.pending = 0;
							current.nextToTest = 0;
							current.next = info;
							info = current;
						}
	
						// Wait until an unblocked signal is delivered to us.
						int signal = 0;
						for(;;)
						{
							uint available =
								(current.pending | process_signals);
							if((available & ~blocked) != 0)
							{
								// We use "nextToTest" to create fairness
								// between multiple signals that arrive at
								// the thread.
								uint test = (((uint)1) << current.nextToTest);
								while((available & ~blocked & test) == 0)
								{
									current.nextToTest =
										(current.nextToTest + 1) & 31;
									test = (((uint)1) << current.nextToTest);
								}
								current.pending &= ~test;
								process_signals &= ~test;
								signal = current.nextToTest;
								current.nextToTest =
									(current.nextToTest + 1) & 31;
								break;
							}
							Monitor.Wait(this);
						}
						return signal;
					}
				}
	
		// Get the next pending signal, or -1 if none is available.
		public int Next(long thread, uint blocked)
				{
					lock(this)
					{
						// Get the signal information block for this thread.
						SignalInfo current = info;
						while(current != null)
						{
							if(current.thread == thread)
							{
								break;
							}
							current = current.next;
						}
						if(current == null)
						{
							current = new SignalInfo();
							current.thread = thread;
							current.pending = 0;
							current.nextToTest = 0;
							current.next = info;
							info = current;
						}
	
						// Find an unblocked signal that was delivered to us.
						uint available = (current.pending | process_signals);
						if((available & ~blocked) != 0)
						{
							// We use "nextToTest" to create fairness between
							// multiple signals that arrive at the thread.
							uint test = (((uint)1) << current.nextToTest);
							while((available & ~blocked & test) == 0)
							{
								current.nextToTest =
									(current.nextToTest + 1) & 31;
								test = (((uint)1) << current.nextToTest);
							}
							current.pending &= ~test;
							process_signals &= ~test;
							int signal = current.nextToTest;
							current.nextToTest = (current.nextToTest + 1) & 31;
							return signal;
						}
						return -1;
					}
				}
	
		// Return the set of pending signals for this thread.
		public uint Pending(long thread)
				{
					lock(this)
					{
						SignalInfo current = info;
						while(current != null)
						{
							if(current.thread == thread)
							{
								return current.pending;
							}
							current = current.next;
						}
						return 0;
					}
				}
	
	} // class SignalController

	// Initialize signal handling for a particular thread.
	public static void __syscall_siginit(long thread)
			{
				SignalController.Controller.Initialize(thread);
			}

	// Deliver a signal to a particular thread.
	public static void __syscall_sigdeliver(long thread, int signal)
			{
				SignalController.Controller.Deliver(thread, signal);
			}

	// Suspend this thread and wait for a signal to be delivered to it.
	// Returns the signal that was delivered.
	public static int __syscall_sigsuspend(long thread, uint blocked)
			{
				return SignalController.Controller.Suspend(thread, blocked);
			}

	// Get the next signal that is currently pending on this thread.
	// Returns -1 if there are no more signals currently pending.
	public static int __syscall_signext(long thread, uint blocked)
			{
				return SignalController.Controller.Next(thread, blocked);
			}

	// Get the set of signals that are pending for this thread.
	public static uint __syscall_sigpending(long thread)
			{
				return SignalController.Controller.Pending(thread);
			}

} // class LibCSignal

} // namespace OpenSystem.C
