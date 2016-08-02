/*
 * ProcessThread.cs - Implementation of the
 *			"System.Diagnostics.ProcessThread" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Diagnostics
{

#if CONFIG_EXTENDED_DIAGNOSTICS

using System.ComponentModel;

// We don't really support thread information in this implementation,
// because it isn't portable.  Instead, we mirror the process information,
// making it look like there is a single-thread process running.  This
// may be a bit weird if the current process is inspected and the program
// has created threads using "System.Threading.Thread".  Therefore, use
// of this API is severely discouraged.

[Designer("System.Diagnostics.Design.ProcessThreadDesigner, System.Design")]
public class ProcessThread
#if CONFIG_COMPONENT_MODEL
	: Component
#endif
{
	// Internal state.
	private Process process;

	// Constructor.
	internal ProcessThread(Process process)
			{
				this.process = process;
			}

	// Thread properties.
	[MonitoringDescription("ThreadBasePriority")]
	public int BasePriority
			{
				get
				{
					return process.BasePriority;
				}
			}
	[MonitoringDescription("ThreadCurrentPriority")]
	public int CurrentPriority
			{
				get
				{
					// Current is always the same as base in this version.
					return process.BasePriority;
				}
			}
	[MonitoringDescription("ThreadId")]
	public int Id
			{
				get
				{
					return process.Id;
				}
			}
	[Browsable(false)]
	public int IdealProcessor
			{
				set
				{
					process.ProcessorAffinity = new IntPtr(1 << value);
				}
			}
	[MonitoringDescription("ThreadPriorityBoostEnabled")]
	public bool PriorityBoostEnabled
			{
				get
				{
					return process.PriorityBoostEnabled;
				}
				set
				{
					process.PriorityBoostEnabled = value;
				}
			}
	[MonitoringDescription("ThreadPriorityLevel")]
	public ThreadPriorityLevel PriorityLevel
			{
				get
				{
					return ThreadPriorityLevel.Normal;
				}
				set
				{
					// Cannot change the priority level for threads.
				}
			}
	[MonitoringDescription("ThreadPrivilegedProcessorTime")]
	public TimeSpan PrivilegedProcessorTime
			{
				get
				{
					return process.PrivilegedProcessorTime;
				}
			}
	[Browsable(false)]
	public IntPtr ProcessorAffinity
			{
				set
				{
					process.ProcessorAffinity = value;
				}
			}
	[MonitoringDescription("ThreadStartAddress")]
	public IntPtr StartAddress
			{
				get
				{
					return IntPtr.Zero;
				}
			}
	[MonitoringDescription("ThreadStartTime")]
	public DateTime StartTime
			{
				get
				{
					return process.StartTime;
				}
			}
	[MonitoringDescription("ThreadThreadState")]
	public ThreadState ThreadState
			{
				get
				{
					// Assume that the thread is running, because there
					// is no portable way to find out what it is doing.
					return ThreadState.Running;
				}
			}
	[MonitoringDescription("ThreadTotalProcessorTime")]
	public TimeSpan TotalProcessorTime
			{
				get
				{
					return process.TotalProcessorTime;
				}
			}
	[MonitoringDescription("ThreadUserProcessorTime")]
	public TimeSpan UserProcessorTime
			{
				get
				{
					return process.UserProcessorTime;
				}
			}
	[MonitoringDescription("ThreadWaitReason")]
	public ThreadWaitReason WaitReason
			{
				get
				{
					// Since the thread is supposedly running (see above),
					// there is no way that it can be in the wait state.
					throw new InvalidOperationException
						(S._("Invalid_NotWaiting"));
				}
			}

	// Reset the ideal processor setting.
	public void ResetIdealProcessor()
			{
				process.ProcessorAffinity = new IntPtr(~0);
			}

}; // class ProcessThread

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
