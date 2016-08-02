/*
 * Application.cs - Implementation of the
 *			"System.Windows.Forms.Application" class.
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

namespace System.Windows.Forms
{

using Microsoft.Win32;
using System.Globalization;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Drawing.Toolkit;
#if CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FORMS
using System.Windows.Forms.VisualStyles;
#endif // CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FORMS

public sealed class Application
{
	// Internal state.
	private static Request requests;
	private static Request lastRequest;

	// Cannot instantiate this class.
	private Application() {}

#if !CONFIG_COMPACT_FORMS

	// Internal state.
	[ThreadStatic] private static InputLanguage inputLanguage;
	private static String safeTopLevelCaptionFormat = "{0}";

#if CONFIG_FRAMEWORK_2_0

	// The application wide default for the UseCompatibleTextRendering.
	internal static bool useCompatibleTextRendering = true;

	// The last value to which the UseWaitCursor of this application was set.
	private static bool useWaitCursor = false;

	// The application wide unhandled exception mode.
	internal static UnhandledExceptionMode unhandledExceptionMode = UnhandledExceptionMode.Automatic;

	// The application wide visual style state.
	internal static VisualStyleState visualStyleState = VisualStyleState.ClientAndNonClientAreasEnabled;

#endif // CONFIG_FRAMEWORK_2_0

	// Determine if it is possible to quit this application.
	public static bool AllowQuit
			{
				get
				{
					// Returns false for applet usage, which we don't have yet.
					return true;
				}
			}

#if CONFIG_WIN32_SPECIFICS

	// Get a registry key for a data path.
	private static RegistryKey GetAppDataRegistry(RegistryKey hive)
			{
				String key = "Software\\" + CompanyName + "\\" +
							 ProductName + "\\" + ProductVersion;
				return hive.CreateSubKey(key);
			}

	// Get the common registry key for data shared between all users.
	public static RegistryKey CommonAppDataRegistry
			{
				get
				{
					return GetAppDataRegistry(Registry.LocalMachine);
				}
			}

#endif

#if !ECMA_COMPAT

	// Get a data path based on a special folder name.
	private static String GetAppDataPath(Environment.SpecialFolder folder)
			{
				String path = Environment.GetFolderPath(folder);
				path += Path.DirectorySeparatorChar.ToString() +
						CompanyName +
						Path.DirectorySeparatorChar.ToString() +
						ProductName +
						Path.DirectorySeparatorChar.ToString() +
						ProductVersion;
				if(!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return path;
			}

	// Get the common data path for data shared between all users.
	public static String CommonAppDataPath
			{
				get
				{
					return GetAppDataPath
						(Environment.SpecialFolder.CommonApplicationData);
				}
			}

	// Get the local user application data path.
	public static String LocalUserAppDataPath
			{
				get
				{
					return GetAppDataPath
						(Environment.SpecialFolder.LocalApplicationData);
				}
			}

	// Get the user application data path.
	public static String UserAppDataPath
			{
				get
				{
					return GetAppDataPath
						(Environment.SpecialFolder.ApplicationData);
				}
			}

	// Get the company name for this application
	public static String CompanyName
			{
				get
				{
					Assembly assembly = Assembly.GetEntryAssembly();
					Object[] attrs = assembly.GetCustomAttributes
						(typeof(AssemblyCompanyAttribute), false);
					if(attrs != null && attrs.Length > 0)
					{
						return ((AssemblyCompanyAttribute)(attrs[0])).Company;
					}
					return assembly.GetName().Name;
				}
			}

	// Get the product name associated with this application.
	public static String ProductName
			{
				get
				{
					Assembly assembly = Assembly.GetEntryAssembly();
					Object[] attrs = assembly.GetCustomAttributes
						(typeof(AssemblyProductAttribute), false);
					if(attrs != null && attrs.Length > 0)
					{
						return ((AssemblyProductAttribute)(attrs[0])).Product;
					}
					return assembly.GetName().Name;
				}
			}

	// Get the product version associated with this application.
	public static String ProductVersion
			{
				get
				{
					Assembly assembly = Assembly.GetEntryAssembly();
					Object[] attrs = assembly.GetCustomAttributes
						(typeof(AssemblyInformationalVersionAttribute), false);
					if(attrs != null && attrs.Length > 0)
					{
						return ((AssemblyInformationalVersionAttribute)
							(attrs[0])).InformationalVersion;
					}
					return assembly.GetName().Version.ToString();
				}
			}

#endif // !ECMA_COMPAT

	// Get or set the culture for the current thread.
	public static CultureInfo CurrentCulture
			{
				get
				{
					return CultureInfo.CurrentCulture;
				}
				set
				{
				#if !ECMA_COMPAT
					Thread.CurrentThread.CurrentCulture = value;
				#endif
				}
			}

	// Get or set the input language for the current thread.
	public static InputLanguage CurrentInputLanguage
			{
				get
				{
					return inputLanguage;
				}
				set
				{
					inputLanguage = value;
				}
			}

	// Get the executable path for this application.
	public static String ExecutablePath
			{
				get
				{
					return (Environment.GetCommandLineArgs())[0];
				}
			}

	// Determine if a message loop exists on this thread.
	public static bool MessageLoop
			{
				get
				{
					return true;
				}
			}

	// Get or set the top-level warning caption format.
	public static String SafeTopLevelCaptionFormat
			{
				get
				{
					return safeTopLevelCaptionFormat;
				}
				set
				{
					safeTopLevelCaptionFormat = value;
				}
			}

	// Get the startup path for the executable.
	public static String StartupPath
			{
				get
				{
					return Path.GetDirectoryName(ExecutablePath);
				}
			}

#if CONFIG_WIN32_SPECIFICS

	// Get the registry key for user-specific data.
	public static RegistryKey UserAppDataRegistry
			{
				get
				{
					return GetAppDataRegistry(Registry.CurrentUser);
				}
			}

#endif

	// Add a message filter.
	public static void AddMessageFilter(IMessageFilter value)
			{
				// We don't use message filters in this implementation.
			}

	// Enable Windows XP visual styles.
	public static void EnableVisualStyles()
			{
				// Not used in this implementation.
			}

	// Exit the message loop on the current thread and close all windows.
	public static void ExitThread()
			{
				// We only allow one message loop in this implementation,
				// so "ExitThread" is the same as "Exit".
				Exit();
			}

#if !ECMA_COMPAT

	// Initialize OLE on the current thread.
	public static ApartmentState OleRequired()
			{
				// Not used in this implementation.
				return ApartmentState.Unknown;
			}

#endif

	// Raise the thread exception event.
	public static void OnThreadException(Exception t)
			{
				if(ThreadException != null)
				{
					ThreadException(null, new ThreadExceptionEventArgs(t));
				}
			}

	// Remove a message filter.
	public static void RemoveMessageFilter(IMessageFilter value)
			{
				// We don't use message filters in this implementation.
			}

#if CONFIG_FRAMEWORK_2_0

	// Run any filters for the message.
	// Returns true if filters processed the message and false otherwise
	[TODO]
	public static bool FilterMessage(Message message)
			{
				return false;
			}

	// Get a read only collection of all currently open forms in this application.
	[TODO]
	public static FormCollection OpenForms
			{
				get
				{
					return null;
				}
			}

	// Raise the Idle event
	public static void RaiseIdle(EventArgs e)
			{
				if(Idle != null)
				{
					Idle(null, e);
				}
			}

	// Register a callback to for checking if messages are still processed.
	// The MessageLoop property will return false if SWF is not processing messages.
	[TODO]
	public static void RegisterMessageLoop(MessageLoopCallback callback)
			{
			}

	public static bool RenderWithVisualStyles
			{
				get
				{
					// No visual styles are used.
					return false;
				}
			}

	// Shut down and restart the application.
	// Throws a NotSupportedException if it's no SWF application.
	[TODO]
	public static void Restart()
			{
				throw new NotSupportedException();
			}

	// Suspend or hibernate the system or requests the system to do so.
	// if force is true the system will be suspended immediately otherwise
	// a suspend request is sent to every app.
	// If disableWakeEvent is true the power state will not be restored on a
	// wake event.
	// Returns true if the system is being suspended, otherwise false.
	[TODO]
	public static bool SetSuspendState(PowerState state,
									   bool force,
									   bool disableWakeEvent)
			{
				return false;
			}

	// Instruct the application how to handle unhandled exceptions.
	// This function must be called before the first window is created
	// otherwise an InvalidOperationException will be thrown.
	[TODO]
	public static void SetUnhandledExceptionMode(UnhandledExceptionMode mode)
			{
				unhandledExceptionMode = mode;
			}
 
	// Instruct the application how to handle unhandled exceptions.
	// This version allows the mode to be set thread specific.
	// This function must be called before the first window is created
	// otherwise an InvalidOperationException will be thrown.
	[TODO]
	public static void SetUnhandledExceptionMode(UnhandledExceptionMode mode,
												 bool threadScope)
			{
				if(!threadScope)
				{
					unhandledExceptionMode = mode;
				}
			}

	// Set the application wide default for the UseCompatibleTextRendering.
	// This function must be called before the first window is created
	// otherwise an InvalidOperationException will be thrown.
	[TODO]
	public static void SetCompatibleTextRenderingDefault(bool defaultValue)
			{
				useCompatibleTextRendering = defaultValue;
			}

	//  Set or reset the UseWaitCursor for all windows in this application.
	[TODO]
	public static bool UseWaitCursor
			{
				get
				{
					return useWaitCursor;
				}
				set
				{
					if(useWaitCursor != value)
					{
						// TODO
						// Apply the property change to all open windows
						useWaitCursor = value;
					}
				}
			}

	// Get or set the visual style state used.
	public static VisualStyleState VisualStyleState
			{
				get
				{
					return visualStyleState;
				}
				set
				{
					visualStyleState = value;
				}
			}

	// Event that occurs when the application is about to enter a modal state.
	public static event EventHandler EnterThreadModal;

	// Event that occurs when the application is about to leave a modal state.
	public static event EventHandler LeaveThreadModal;

#endif // CONFIG_FRAMEWORK_2_0

	// Event that is raised when the application is about to exit.
	public static event EventHandler ApplicationExit;

	// Event that is raised when the message loop is entering the idle state.
	public static event EventHandler Idle;

	// Event that is raised for an untrapped thread exception.
	public static event ThreadExceptionEventHandler ThreadException;

	// Event that is raised when the current thread is about to exit.
	public static event EventHandler ThreadExit;

#endif // !CONFIG_COMPACT_FORMS

	// The thread that is running the main message loop.
	private static Thread mainThread;

	// Process all events that are currently in the message queue.
	public static void DoEvents()
			{
				bool isMainThread;
				Thread thread = Thread.CurrentThread;
				Request request;

				// Determine if we are the main thread.
				lock(typeof(Application))
				{
					isMainThread = (mainThread == thread);
				}

				// Process pending events.
				if(isMainThread)
				{
					ToolkitManager.Toolkit.ProcessEvents(false);
				}

				// Process requests that were sent via "SendRequest".
				while((request = NextRequest(thread, false)) != null)
				{
					request.Execute();
				}
			}

	// Tell the application to exit.
	public static void Exit()
			{
				lock(typeof(Application))
				{
					if(mainThread != null ||
					   Thread.CurrentThread == null)
					{
						ToolkitManager.Toolkit.Quit();
					}
				}
			}

	// Exit from the current thread when the main form closes.
	private static void ContextExit(Object sender, EventArgs e)
			{
			#if !CONFIG_COMPACT_FORMS
				ExitThread();
			#else
				Exit();
			#endif
			}

	// Inner version of "Run".  In this implementation we only allow a
	// message loop to be running on one of the threads.
	private static void RunMessageLoop(ApplicationContext context)
			{
				Form mainForm = context.MainForm;
				EventHandler handler;
				Request request;
				Thread thread = Thread.CurrentThread;
				bool isMainThread;

				// Connect the context's ThreadExit event to our "ExitThread".
				handler = new EventHandler(ContextExit);
				context.ThreadExit += handler;

				// Show the main form on-screen.
				if(mainForm != null)
				{
					mainForm.Show();
					mainForm.SelectNextControl (null, true, true, true, false);
					Form.activeForm = mainForm;
				}

				// Make sure that we are the only message loop.
				lock(typeof(Application))
				{
					if(mainThread != null)
					{
						isMainThread = false;
					}
					else
					{
						mainThread = thread;
						isMainThread = true;
					}
				}

				// Run the main message processing loop.
				if(isMainThread)
				{
					IToolkit toolkit = ToolkitManager.Toolkit;
					try
					{
						for(;;)
						{
							try {
								// Process events in the queue.
								if(!toolkit.ProcessEvents(false))
								{
								#if !CONFIG_COMPACT_FORMS
									// There were no events, so raise "Idle".
									if(Idle != null)
									{
										Idle(null, EventArgs.Empty);
									}
								#endif
		
									// Block until an event, or quit, arrives.
									if(!toolkit.ProcessEvents(true))
									{
										break;
									}
								}
		
								// Process requests sent via "SendRequest".
								while((request = NextRequest(thread, false))
											!= null)
								{
									request.Execute();
								}
							}
							catch( Exception e ) {
								Application.OnThreadException( e );
							}
						}
					}
					finally
					{
						// Reset the "mainThread" variable because there
						// is no message loop any more.
						lock(typeof(Application))
						{
							mainThread = null;
						}
					}
				}
				else
				{
					// This is not the main thread, so only process
					// requests that were sent via "SendRequest".
					while((request = NextRequest(thread, true)) != null)
					{
						request.Execute();
					}
				}

				// Disconnect from the context's "ThreadExit" event.
				context.ThreadExit -= handler;
				Form.activeForm = null;

			#if !CONFIG_COMPACT_FORMS

				// Raise the "ThreadExit" event.
				if(ThreadExit != null)
				{
					ThreadExit(null, EventArgs.Empty);
				}

				// Raise the "ApplicationExit" event.
				if(ApplicationExit != null)
				{
					ApplicationExit(null, EventArgs.Empty);
				}

			#endif
			}

	// Run an inner message loop until the dialog result is set on a form.
	internal static void InnerMessageLoop(Form form)
			{
				Request request;
				Thread thread = Thread.CurrentThread;
				bool isMainThread;
				bool resetMainThread;

				// Determine if we are running on the main thread or not.
				lock(typeof(Application))
				{
					if(mainThread == null)
					{
						// The main message loop hasn't started yet.
						// This might happen with MessageBox dialogs.
						mainThread = thread;
						isMainThread = true;
						resetMainThread = true;
					}
					else
					{
						isMainThread = (mainThread == thread);
						resetMainThread = false;
					}
				}

				// Run the main message processing loop.
				if(isMainThread)
				{
					IToolkit toolkit = ToolkitManager.Toolkit;
					try
					{
						while(!(form.dialogResultIsSet) && form.Visible)
						{
							// Process events in the queue.
							if(!toolkit.ProcessEvents(false))
							{
							#if !CONFIG_COMPACT_FORMS
								// There were no events, so raise "Idle".
								if(Idle != null)
								{
									Idle(null, EventArgs.Empty);
								}
							#endif
	
								// Block until an event, or quit, arrives.
								if(!toolkit.ProcessEvents(true))
								{
									break;
								}
							}
	
							// Process requests sent via "SendRequest".
							while((request = NextRequest(thread, false))
										!= null)
							{
								request.Execute();
							}
						}
					}
					finally
					{
						// Reset the "mainThread" variable because there
						// is no message loop any more.
						lock(typeof(Application))
						{
							if(resetMainThread)
							{
								mainThread = null;
							}
						}
					}
				}
				else
				{
					// This is not the main thread, so only process
					// requests that were sent via "SendRequest".
					while(!(form.dialogResultIsSet) && form.Visible)
					{
						if((request = NextRequest(thread, true)) != null)
						{
							request.Execute();
						}
						else
						{
							break;
						}
					}
				}
			}

	// Make the specified form visible and run the main loop.
	// The loop will exit when "Exit" is called.
	public static void Run(Form mainForm)
			{
				RunMessageLoop(new ApplicationContext(mainForm));
			}

#if !CONFIG_COMPACT_FORMS

	// Run the main message loop on this thread.
	public static void Run()
			{
				RunMessageLoop(new ApplicationContext());
			}

	// Run the main message loop for an application context.
	public static void Run(ApplicationContext context)
			{
				if(context == null)
				{
					context = new ApplicationContext();
				}
				RunMessageLoop(context);
			}

#endif // !CONFIG_COMPACT_FORMS

	// Information about a request to be executed in a specific thread.
	// This is used to help implement the "Control.Invoke" method.
	internal abstract class Request
	{
		public Request next;
		public Thread thread;

		// Execute the request.
		public abstract void Execute();

	}; // class Request

	// Send a request to a particular thread's message queue.
	internal static void SendRequest(Request request, Thread thread)
			{
				Object obj = typeof(Application);
				request.thread = thread;
				lock(obj)
				{
					// Add the request to the queue.
					request.next = null;
					if(requests != null)
					{
						lastRequest.next = request;
					}
					else
					{
						requests = request;
					}
					lastRequest = request;

					// Wake up all threads that are blocking in "NextRequest".
					Monitor.PulseAll(obj);

					// Wake up the thread that will receive the request,
					// as it may be blocking inside "ProcessEvents".
					ToolkitManager.Toolkit.Wakeup(thread);
				}
			}

	// Get the next pending request for a particular thread.
	private static Request NextRequest(Thread thread, bool block)
			{
				Object obj = typeof(Application);
				Request request, prev;
				lock(obj)
				{
					for(;;)
					{
						// See if there is a request on the queue for us.
						prev = null;
						request = requests;
						while(request != null)
						{
							if(request.thread == thread)
							{
								if(prev != null)
								{
									prev.next = request.next;
								}
								else
								{
									requests = request.next;
								}
								if(request.next == null)
								{
									lastRequest = prev;
								}
								else
								{
									request.next = null;
								}
								break;
							}
							prev = request;
							request = request.next;
						}

						// Bail out if we got something or we aren't blocking.
						if(request != null || !block)
						{
							break;
						}

						// Wait to be signalled by "SendRequest".
						Monitor.Wait(obj);
					}
				}
				return request;
			}

}; // class Application

}; // namespace System.Windows.Forms
