/*
 * Process.cs - Implementation of the "System.Diagnostics.Process" class.
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

using Platform;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

// This class deliberately supports only a subset of the full API,
// for reasons of security and portability.  The set of supported
// features roughly corresponds to "Windows 98 mode" in other systems.

// We need unrestricted permissions to start and manage processes.
#if CONFIG_PERMISSIONS
[PermissionSet(SecurityAction.LinkDemand, Unrestricted=true)]
[PermissionSet(SecurityAction.InheritanceDemand, Unrestricted=true)]
#endif
[DefaultProperty("StartInfo")]
[DefaultEvent("Exited")]
[Designer("System.Diagnostics.Design.ProcessDesigner, System.Design")]
public class Process
#if CONFIG_COMPONENT_MODEL
	: Component
#endif
{
	// Internal state.
	private bool enableRaisingEvents;
	private int exitCode;
	private IntPtr processHandle;
	private int processID;
	private bool hasExited;
	private String[] argv;
	private ProcessStartInfo startInfo;
#if CONFIG_COMPONENT_MODEL
	private ISynchronizeInvoke syncObject;
#endif
	private StreamWriter stdin; 
	private StreamReader stdout; 
	private StreamReader stderr; 
	private static Process currentProcess;
	private static ArrayList children;

	// Constructor.
	public Process()
			{
				this.processID = -1;
			}

	// Check to see if the process exists.
	private void Exists()
			{
				if(processID == -1)
				{
					throw new InvalidOperationException
						(S._("Invalid_ProcessNotStarted"));
				}
			}

	// Check to see if the process exists and has not exited.
	private void Running()
			{
				if(processID == -1)
				{
					throw new InvalidOperationException
						(S._("Invalid_ProcessNotStarted"));
				}
				if(hasExited)
				{
					throw new InvalidOperationException
						(S._("Invalid_ProcessExited"));
				}
			}

	// Check to see if the process has exited.
	private void Finished()
			{
				if(processID == -1)
				{
					throw new InvalidOperationException
						(S._("Invalid_ProcessNotStarted"));
				}
				if(!hasExited)
				{
					throw new InvalidOperationException
						(S._("Invalid_ProcessNotExited"));
				}
			}

	// Process properties.
	[MonitoringDescription("ProcessBasePriority")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int BasePriority
			{
				get
				{
					// We only use the normal priority in this implementation.
					Exists();
					return 8;
				}
			}
	[DefaultValue(false)]
	[Browsable(false)]
	[MonitoringDescription("ProcessEnableRaisingEvents")]
	public bool EnableRaisingEvents
			{
				get
				{
					return enableRaisingEvents;
				}
				set
				{
					enableRaisingEvents = value;
				}
			}
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessExitCode")]
	public int ExitCode
			{
				get
				{
					Finished();
					return exitCode;
				}
			}
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessExitTime")]
	public DateTime ExitTime
			{
				get
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessHandle")]
	public IntPtr Handle
			{
				get
				{
					Exists();
					return processHandle;
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessHandleCount")]
	public int HandleCount
			{
				get
				{
					Running();
					int count = GetHandleCount(processHandle);
					if(count < 0)
					{
						// Don't know how to get the value, so assume that
						// stdin, stdout, and stderr are the only handles.
						return 3;
					}
					else
					{
						return count;
					}
				}
			}
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessTerminated")]
	public bool HasExited
			{
				get
				{
					Exists();
					return hasExited;
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessId")]
	public int Id
			{
				get
				{
					Exists();
					return processID;
				}
			}
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessMachineName")]
	public String MachineName
			{
				get
				{
					Exists();
					return ".";		// Only local processes are supported.
				}
			}
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessMainModule")]
	public ProcessModule MainModule
			{
				get
				{
					Running();
					return new ProcessModule(argv[0], ProcessName);
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessMainWindowHandle")]
	public IntPtr MainWindowHandle
			{
				get
				{
					Running();
					return GetMainWindowHandle(processID);
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessMainWindowTitle")]
	public String MainWindowTitle
			{
				get
				{
					IntPtr handle = MainWindowHandle;
					if(handle != IntPtr.Zero)
					{
						return GetMainWindowTitle(handle);
					}
					else
					{
						return String.Empty;
					}
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessMaxWorkingSet")]
	public IntPtr MaxWorkingSet
			{
				get
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
				set
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessMinWorkingSet")]
	public IntPtr MinWorkingSet
			{
				get
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
				set
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessModules")]
	public ProcessModuleCollection Modules
			{
				get
				{
					// In this implementation, we only report the main module.
					return new ProcessModuleCollection(MainModule);
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessNonpagedSystemMemorySize")]
	public int NonpagedSystemMemorySize
			{
				get
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessPagedMemorySize")]
	public int PagedMemorySize
			{
				get
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessPagedSystemMemorySize")]
	public int PagedSystemMemorySize
			{
				get
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessPeakPagedMemorySize")]
	public int PeakPagedMemorySize
			{
				get
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessPeakVirtualMemorySize")]
	public int PeakVirtualMemorySize
			{
				get
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessPeakWorkingSet")]
	public int PeakWorkingSet
			{
				get
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessPriorityBoostEnabled")]
	public bool PriorityBoostEnabled
			{
				get
				{
					return false;
				}
				set
				{
					// Priority boosting is not supported.
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessPriorityClass")]
	public ProcessPriorityClass PriorityClass
			{
				get
				{
					// Everything is assumed to run at normal priority.
					return ProcessPriorityClass.Normal;
				}
				set
				{
					// Priority changes are not supported.
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessPrivateMemorySize")]
	public int PrivateMemorySize
			{
				get
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessPrivilegedProcessorTime")]
	public TimeSpan PrivilegedProcessorTime
			{
				get
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessProcessName")]
	public String ProcessName
			{
				get
				{
					// We always use "argv[0]" as the process name.
					Running();
					String name = argv[0];
					if(name != null && name.Length >= 4 &&
					   String.Compare(name, name.Length - 4,
					   				  ".exe", 0, 4, true) == 0)
					{
						name = name.Substring(0, name.Length - 4);
					}
					if(name != null)
					{
						int index1, index2;
						index1 = name.LastIndexOf('\\');
						index2 = name.LastIndexOf('/');
						if(index2 > index1)
						{
							index1 = index2;
						}
						if(index1 != -1)
						{
							name = name.Substring(index1 + 1);
						}
					}
					return name;
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessProcessorAffinity")]
	public IntPtr ProcessorAffinity
			{
				get
				{
					Running();
					return new IntPtr(GetProcessorAffinity(processHandle));
				}
				set
				{
					// Processor affinity cannot be changed.
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessResponding")]
	public bool Responding
			{
				get
				{
					IntPtr handle = MainWindowHandle;
					if(handle != IntPtr.Zero)
					{
						return MainWindowIsResponding(handle);
					}
					return true;
				}
			}
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessStandardError")]
	public StreamReader StandardError
			{
				get
				{
					if(stderr == null)
					{
						throw new InvalidOperationException
							(S._("Invalid_StandardStream"));
					}
					return stderr;
				}
			}
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessStandardInput")]
	public StreamWriter StandardInput
			{
				get
				{
					if(stdin == null)
					{
						throw new InvalidOperationException
							(S._("Invalid_StandardStream"));
					}
					return stdin;
				}
			}
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessStandardOutput")]
	public StreamReader StandardOutput
			{
				get
				{
					if(stdout == null)
					{
						throw new InvalidOperationException
							(S._("Invalid_StandardStream"));
					}
					return stdout;
				}
			}
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[MonitoringDescription("ProcessStartInfo")]
	public ProcessStartInfo StartInfo
			{
				get
				{
					if(startInfo == null)
					{
						startInfo = new ProcessStartInfo();
					}
					return startInfo;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					startInfo = value;
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessStartTime")]
	public DateTime StartTime
			{
				get
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}
#if CONFIG_COMPONENT_MODEL
	[Browsable(false)]
	[MonitoringDescription("ProcessSynchronizingObject")]
	public ISynchronizeInvoke SynchronizingObject
			{
				get
				{
					return syncObject;
				}
				set
				{
					syncObject = value;
				}
			}
#endif
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessThreads")]
	public ProcessThreadCollection Threads
			{
				get
				{
					// We report a single thread corresponding to each process.
					// See the comments in "ProcessThread.cs" for more info.
					return new ProcessThreadCollection
						(new ProcessThread(this));
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessTotalProcessorTime")]
	public TimeSpan TotalProcessorTime
			{
				get
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessUserProcessorTime")]
	public TimeSpan UserProcessorTime
			{
				get
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessVirtualMemorySize")]
	public int VirtualMemorySize
			{
				get
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[MonitoringDescription("ProcessWorkingSet")]
	public int WorkingSet
			{
				get
				{
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}

	// Close all resources associated with this object.
	public void Close()
			{
				if(IsCurrentProcess(this))
				{
					// We cannot close the current process.
					return;
				}
				if(processID != -1)
				{
					CloseProcess(processHandle, processID);
					processID = -1;
					processHandle = IntPtr.Zero;
				}
				enableRaisingEvents = false;
				hasExited = false;
				if(stdin != null)
				{
					stdin.Close();
					stdin = null;
				}
				if(stdout != null)
				{
					stdout.Close();
					stdout = null;
				}
				if(stderr != null)
				{
					stderr.Close();
					stderr = null;
				}
				argv = null;
#if CONFIG_COMPONENT_MODEL
				syncObject = null;
#endif
				lock(typeof(Process))
				{
					// Remove this process from the list of active children.
					if(children != null)
					{
						children.Remove(this);
					}
				}
			}

	// Close the main window for this process.
	public bool CloseMainWindow()
			{
				IntPtr handle = MainWindowHandle;
				if(handle != IntPtr.Zero)
				{
					return CloseMainWindow(handle);
				}
				else
				{
					return false;
				}
			}

	// Dispose this object.
	protected override void Dispose(bool disposing)
			{
				Close();
			}

	// Enter debug mode.
	public static void EnterDebugMode()
			{
				// Nothing to do here.
			}

	// Get the current process.
	public static Process GetCurrentProcess()
			{
				lock(typeof(Process))
				{
					if(currentProcess == null)
					{
						currentProcess = new Process();
						int processID;
						IntPtr processHandle;
						GetCurrentProcessInfo
							(out processID, out processHandle);
						currentProcess.processID = processID;
						currentProcess.processHandle = processHandle;
						currentProcess.argv = Environment.GetCommandLineArgs();
						if(currentProcess.argv != null &&
						   currentProcess.argv.Length > 0)
						{
							ProcessStartInfo info = new ProcessStartInfo();
							info.FileName = currentProcess.argv[0];
							info.Arguments =
								ProcessStartInfo.ArgVToArguments
									(currentProcess.argv, 1, " ");
							currentProcess.startInfo = info;
						}
					}
					return currentProcess;
				}
			}

	// Determine if a process is the current one.
	private static bool IsCurrentProcess(Process process)
			{
				lock(typeof(Process))
				{
					return (process == currentProcess);
				}
			}

	// Get a particular process by identifier.  Insecure, so not supported.
	public static Process GetProcessById(int processId)
			{
				return GetProcessById(processId, ".");
			}
	public static Process GetProcessById(int processId, String machineName)
			{
				// Get the full process list and then search it.
				Process[] list = GetProcesses(machineName);
				if(list != null)
				{
					foreach(Process process in list)
					{
						if(process.Id == processId)
						{
							return process;
						}
					}
				}
				throw new ArgumentException(S._("Arg_NoSuchProcess"));
			}

	// Get a list of all processes.
	public static Process[] GetProcesses()
			{
				// As far as the caller is concerned, the only processes
				// that exist are the current process and any children
				// that we have previously forked and have not yet exited.
				// We don't allow the application to access other processes.
				lock(typeof(Process))
				{
					int count = (children != null ? children.Count : 0);
					Process[] list = new Process [count + 1];
					list[0] = GetCurrentProcess();
					if(children != null)
					{
						children.CopyTo(list, 1);
					}
					return list;
				}
			}
	public static Process[] GetProcesses(String machineName)
			{
				if(machineName == null)
				{
					throw new ArgumentNullException("machineName");
				}
				else if(machineName == ".")
				{
					// Get information on the local machine.
					return GetProcesses();
				}
				else
				{
					// Cannot request information about remote computers.
					throw new PlatformNotSupportedException
						(S._("Invalid_Platform"));
				}
			}

	// Get a list of all processes with a specific name.
	public static Process[] GetProcessesByName(String processName)
			{
				return GetProcessesByName(processName, ".");
			}
	public static Process[] GetProcessesByName(String processName,
											   String machineName)
			{
				// Get the process list and then filter it by name.
				Process[] list = GetProcesses(machineName);
				int count = 0;
				foreach(Process p1 in list)
				{
					if(String.Compare(p1.ProcessName, processName, true) == 0)
					{
						++count;
					}
				}
				Process[] newList = new Process [count];
				count = 0;
				foreach(Process p2 in list)
				{
					if(String.Compare(p2.ProcessName, processName, true) == 0)
					{
						newList[count++] = p2;
					}
				}
				return newList;
			}

	// Kill the process.
	public void Kill()
			{
				if(IsCurrentProcess(this))
				{
					// We cannot kill the current process.
					return;
				}
				Running();
				KillProcess(processHandle, processID);
				processHandle = IntPtr.Zero;
				processID = -1;
				Close();
			}

	// Leave debug mode.
	public static void LeaveDebugMode()
			{
				// Nothing to do here.
			}

	// Raise the "Exited" event.
	protected void OnExited()
			{
				if(enableRaisingEvents && Exited != null)
				{
					Exited(null, new EventArgs());
				}
			}

	// Refresh information from the operating system.
	public void Refresh()
			{
				// Nothing to do here because nothing changeable is cached.
			}

	// Start a process.
	public bool Start()
			{
				ProcessStartInfo.ProcessStartFlags flags;

				// Validate the start information.
				if(startInfo == null || startInfo.FileName == String.Empty)
				{
					throw new InvalidOperationException
						(S._("Invalid_ProcessStartInfo"));
				}
				flags = startInfo.flags;
				if((flags & ProcessStartInfo.ProcessStartFlags.UseShellExecute)
							!= 0)
				{
					if((flags & (ProcessStartInfo.ProcessStartFlags
										.RedirectStdin |
					             ProcessStartInfo.ProcessStartFlags
										.RedirectStdout |
					             ProcessStartInfo.ProcessStartFlags
										.RedirectStderr)) != 0)
					{
						// Cannot redirect if using shell execution.
						throw new InvalidOperationException
							(S._("Invalid_ProcessStartInfo"));
					}
				}

				// Close the current process information, if any.
				Close();

				// If attempting to start using the current process,
				// then we want to do "execute over the top" instead,
				// replacing the current process with a new one.
				if(IsCurrentProcess(this))
				{
					flags |= ProcessStartInfo.ProcessStartFlags.ExecOverTop;
				}

				// Get the environment to use in the new process if it
				// was potentially modified by the programmer.
				String[] env = null;
				if(startInfo.envVars != null)
				{
					StringCollection coll = new StringCollection();
					IDictionaryEnumerator e =
						(IDictionaryEnumerator)
							(startInfo.envVars.GetEnumerator());
					while(e.MoveNext())
					{
						coll.Add(((String)(e.Key)).ToUpper(CultureInfo.InvariantCulture) +
						 "=" + ((String)(e.Value)));
					}
					env = new String [coll.Count];
					coll.CopyTo(env, 0);
				}

				// Get the pathname of the program to be executed.
				String program;
				if(startInfo.UseShellExecute && startInfo.WorkingDirectory != String.Empty && !Path.IsPathRooted(startInfo.FileName))
				{
					program = Path.Combine(startInfo.WorkingDirectory, startInfo.FileName);
				}
				else
				{
					program = startInfo.FileName;
				}

				// Parse the arguments into a local argv array.
				String[] args = ProcessStartInfo.ArgumentsToArgV
						(startInfo.Arguments);
				argv = new String [args.Length + 1];
				argv[0] = program;
				Array.Copy(args, 0, argv, 1, args.Length);

				// Start the process.
				IntPtr stdinHandle;
				IntPtr stdoutHandle;
				IntPtr stderrHandle;
				if(!StartProcess(program, startInfo.Arguments, startInfo.WorkingDirectory, argv,
								 (int)flags, (int)(startInfo.WindowStyle),
								 env, startInfo.Verb,
								 startInfo.ErrorDialogParentHandle,
								 out processHandle, out processID,
								 out stdinHandle, out stdoutHandle,
								 out stderrHandle))
				{
					// Checking errno for error
					Errno errno = Process.GetErrno();
					if( errno != Errno.Success ) {
						throw new Win32Exception(Process.GetErrnoMessage(errno));
					}
				}				

				// Wrap up the redirected I/O streams.
				if(stdinHandle != SocketMethods.GetInvalidHandle())
				{
					stdin = new StreamWriter
						(new FileStream(stdinHandle, FileAccess.Write, true));
					stdin.AutoFlush = true;
				}
				if(stdoutHandle != SocketMethods.GetInvalidHandle())
				{
					stdout = new StreamReader
						(new FileStream(stdoutHandle, FileAccess.Read, true));
				}
				if(stderrHandle != SocketMethods.GetInvalidHandle())
				{
					stderr = new StreamReader
						(new FileStream(stderrHandle, FileAccess.Read, true));
				}

				// Add the process to the list of active children.
				lock(typeof(Process))
				{
					if(children == null)
					{
						children = new ArrayList();
					}
					children.Add(this);
				}
				return true;
			}

	// Convenience wrappers for "Process.Start()".
	public static Process Start(ProcessStartInfo startInfo)
			{
				Process process = new Process();
				process.StartInfo = startInfo;
				if(process.Start())
				{
					return process;
				}
				else
				{
					return null;
				}
			}
	public static Process Start(String fileName)
			{
				Process process = new Process();
				process.StartInfo = new ProcessStartInfo(fileName);
				if(process.Start())
				{
					return process;
				}
				else
				{
					return null;
				}
			}
	public static Process Start(String fileName, String arguments)
			{
				Process process = new Process();
				process.StartInfo = new ProcessStartInfo(fileName, arguments);
				if(process.Start())
				{
					return process;
				}
				else
				{
					return null;
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return ProcessName;
			}

	// Wait for the process to exit.
	public void WaitForExit()
			{
				WaitForExit(Int32.MaxValue);
			}
	public bool WaitForExit(int milliseconds)
			{
				Exists();
				if(hasExited)
				{
					// The process has already exited.
					return true;
				}
				if(IsCurrentProcess(this))
				{
					// Cannot wait for the current process.
					return false;
				}
				if(!WaitForExit(processHandle, processID,
								milliseconds, out exitCode))
				{
					// Timeout occurred while waiting for the process.
					return false;
				}
				hasExited = true;
				OnExited();
				return true;
			}

	// Wait for the process to reach its idle state.
	public bool WaitForInputIdle()
			{
				return WaitForInputIdle(Int32.MaxValue);
			}
	public bool WaitForInputIdle(int milliseconds)
			{
				Exists();
				if(hasExited)
				{
					return false;
				}
				else if(IsCurrentProcess(this))
				{
					// Cannot wait for the current process.
					return false;
				}
				else
				{
					return WaitForInputIdle
						(processHandle, processID, milliseconds);
				}
			}

	// Event that is emitted when the process exits.
	[MonitoringDescription("ProcessExited")]
	[Category("Behavior")]
	public event EventHandler Exited;

	// Get the main window handle for a process.
	// Returns IntPtr.Zero if unknown.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr GetMainWindowHandle(int processID);

	// Get the title of a main window.  Returns null if unknown.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static String GetMainWindowTitle(IntPtr windowHandle);

	// Determine if the main window of a process is responding.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool MainWindowIsResponding(IntPtr windowHandle);

	// Get the process ID and handle of the current process.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void GetCurrentProcessInfo(out int processID,
											  		 out IntPtr handle);

	// Get the number of handles that are currently open by a process.
	// Returns -1 if the number cannot be determined on this platform.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int GetHandleCount(IntPtr processHandle);

	// Get the processor mask for which processors a process may run on.
	// Return 1 if it is impossible to determine the affinity information
	// (which essentially means "runs on processor 1").
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int GetProcessorAffinity(IntPtr processHandle);

	// Close a process, but leave the process running in the background.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void CloseProcess
			(IntPtr processHandle, int processID);

	// Kill a process.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void KillProcess
			(IntPtr processHandle, int processID);

	// Send a request to close a main window.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool CloseMainWindow(IntPtr windowHandle);

	// Wait for a particular process to exit.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool WaitForExit
			(IntPtr processHandle, int processID,
			 int milliseconds, out int exitCode);

	// Wait for a particular process to enter the idle state after startup.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool WaitForInputIdle
			(IntPtr processHandle, int processID, int milliseconds);

	// Start a new process.  Returns false if it could not be started.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool StartProcess
			(String filename, String arguments, String workingDir, String[] argv,
			 int flags, int windowStyle, String[] envVars,
			 String verb, IntPtr errorDialogParent,
			 out IntPtr processHandle, out int processID,
			 out IntPtr stdinHandle, out IntPtr stdoutHandle,
			 out IntPtr stderrHandle);
			 
	// Get the last-occurring system error code for the current thread.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static Errno GetErrno();
	
	// Get a descriptive message for an error from the underlying platform.
	// Returns null if the platform doesn't have an appropriate message.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static String GetErrnoMessage(Errno errno);


}; // class Process

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
