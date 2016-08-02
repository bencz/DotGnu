/*
 * Environment.cs - Implementation of the "System.Environment" class.
 *
 * Copyright (C) 2001, 2002, 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2009  Free Software Foundation Inc.
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

namespace System
{

using System.Security;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Platform;

public sealed class Environment
{
	// Internal state.
	private static String newLine;
	private static int exitCode = 0;

	// This class cannot be instantiated.
	private Environment() {}

	// Initialize the environment state.
	static Environment()
			{
				// Get the newline string.
				try
				{
					newLine = SysCharInfo.GetNewLine();
				}
				catch(NotImplementedException)
				{
					// The runtime engine does not have "SysCharInfo".
					newLine = "\n";
				}
			}

	// Get the platform-specific newline string.
	public static String NewLine
			{
				get
				{
					return newLine;
				}
			}

	// Exit from the current process.
	public static void Exit(int exitCode)
			{
				TaskMethods.Exit(exitCode);
			}

	// Get or set the process exit code.
	public static int ExitCode
			{
				get
				{
					return exitCode;
				}
				set
				{
					if(exitCode != value)
					{
						exitCode = value;
						TaskMethods.SetExitCode(value);
					}
				}
			}

	// Determine if application shutdown has started.
	public static bool HasShutdownStarted
			{
				get
				{
					return false;
				}
			}

	// Get the stack trace for the current context.
	public static String StackTrace
			{
				get
				{
					return (new StackTrace(1)).ToString();
				}
			}

	// Get the version of the runtime engine.
	public static Version Version
			{
				get
				{
					return new Version(InfoMethods.GetRuntimeVersion());
				}
			}

	// Get the command line arguments.
	public static String[] GetCommandLineArgs()
			{
				String[] args = TaskMethods.GetCommandLineArgs();
				if(args != null)
				{
					return args;
				}
				else
				{
					throw new NotSupportedException
						(_("Exception_NoCmdLine"));
				}
			}

	// Get the command line as a single string.
	public static String CommandLine
			{
				get
				{
					String[] args = GetCommandLineArgs();
					return String.Join(" ", args);
				}
			}

	// Get the number of milliseconds since the last reboot.
	public static int TickCount
			{
				get
				{
					return TimeMethods.GetUpTime();
				}
			}

	// Get a particular environment variable.
	public static String GetEnvironmentVariable(String variable)
			{
				if(variable == null)
				{
					throw new ArgumentNullException("variable");
				}
				return TaskMethods.GetEnvironmentVariable(variable);
			}

	// Get a dictionary that allows access to the set of
	// environment variables for the current task.
	public static IDictionary GetEnvironmentVariables()
			{
				return new EnvironmentDictionary();
			}

#if !ECMA_COMPAT

	// Get the "System" directory.
	public static String SystemDirectory
			{
				get
				{
					String dir = DirMethods.GetSystemDirectory();
					if(dir != null)
					{
						return dir;
					}
					else
					{
						throw new NotSupportedException
							(_("Exception_NoSystemDir"));
					}
				}
			}

	// Get or set the current working directory.
	public static String CurrentDirectory
			{
				get
				{
					return Directory.GetCurrentDirectory();
				}
				set
				{
					Directory.SetCurrentDirectory(value);
				}
			}

	// Get the NetBIOS machine name.
	public static String MachineName
			{
				get
				{
					return InfoMethods.GetNetBIOSMachineName();
				}
			}

	// Get the operating system version.
	public static OperatingSystem OSVersion
			{
				get
				{
					return new OperatingSystem
						(InfoMethods.GetPlatformID(),
						 new Version(5, 1, 2600, 0));
				}
			}

	// Get the domain name for this machine.
	public static String UserDomainName
			{
				get
				{
					return InfoMethods.GetUserDomainName();
				}
			}

	// Determine if we are in interactive mode.
	public static bool UserInteractive
			{
				get
				{
					return InfoMethods.IsUserInteractive();
				}
			}

	// Get the name of the current user.
	public static String UserName
			{
				get
				{
					return InfoMethods.GetUserName();
				}
			}

	// Get the size of the working set.
	public static long WorkingSet
			{
				get
				{
					return InfoMethods.GetWorkingSet();
				}
			}

#if CONFIG_FRAMEWORK_2_0

	// Get the number of processors in this machine.
	public static int ProcessorCount
			{
				get
				{
					return InfoMethods.GetProcessorCount();
				}
			}

#endif // CONFIG_FRAMEWORK_2_0

	// Expand environment variable references in a string.
	public static String ExpandEnvironmentVariables(String name)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(name.IndexOf('%') != -1)
				{
					return name;
				}
				StringBuilder builder = new StringBuilder();
				int posn = 0;
				int index;
				String tag, value;
				while(posn < name.Length)
				{
					index = name.IndexOf('%', posn);
					if(index == -1)
					{
						builder.Append(name, posn, name.Length - posn);
						break;
					}
					if(index > posn)
					{
						builder.Append(name, posn, index - posn);
						posn = index;
						index = name.IndexOf('%', posn + 1);
						if(index == -1)
						{
							builder.Append(name, posn, name.Length - posn);
							break;
						}
						tag = name.Substring(posn + 1, index - posn - 1);
						value = GetEnvironmentVariable(tag);
						if(value != null)
						{
							builder.Append(value);
						}
						else
						{
							builder.Append(name, posn, index + 1 - posn);
						}
						posn = index + 1;
					}
				}
				return builder.ToString();
			}

	// Special folder names.
	public enum SpecialFolder
	{
		Desktop               = 0x00,
		Programs              = 0x02,
		Personal              = 0x05,
#if CONFIG_FRAMEWORK_2_0
		MyDocuments           = 0x05,
#endif // !CONFIG_FRAMEWORK_2_0
		Favorites             = 0x06,
		Startup               = 0x07,
		Recent                = 0x08,
		SendTo                = 0x09,
		StartMenu             = 0x0b,
		MyMusic               = 0x0d,
		DesktopDirectory      = 0x10,
		MyComputer            = 0x11,
		Templates             = 0x15,
		ApplicationData	      = 0x1a,
		LocalApplicationData  = 0x1c,
		InternetCache         = 0x20,
		Cookies               = 0x21,
		History               = 0x22,
		CommonApplicationData = 0x23,
		System                = 0x25,
		ProgramFiles          = 0x26,
		MyPictures            = 0x27,
		CommonProgramFiles    = 0x2b

	}; // enum SpecialFolder

	// Import the Win32 SHGetFolderPathA function from "shell32.dll"
	[DllImport("shell32.dll", CallingConvention=CallingConvention.Winapi)]
	[MethodImpl(MethodImplOptions.PreserveSig)]
	extern private static Int32 SHGetFolderPathA
				(IntPtr hwndOwner, int nFolder, IntPtr hToken,
				 uint dwFlags, IntPtr path);

	// Get a path to a specific system folder.
	public static String GetFolderPath(SpecialFolder folder)
			{
				// We can use the operating system under Win32.
				if(InfoMethods.GetPlatformID() != PlatformID.Unix)
				{
					// Allocate a buffer to hold the result path.
					IntPtr buffer = Marshal.AllocHGlobal(260 /*MAX_PATH*/ + 1);

					// Call "SHGetFolderPath" to retrieve the path.
					try
					{
						SHGetFolderPathA(IntPtr.Zero, (int)folder,
									     IntPtr.Zero, 0, buffer);
						String value = Marshal.PtrToStringAnsi(buffer);
						if(value != null && value.Length != 0)
						{
							Marshal.FreeHGlobal(buffer);
							return value;
						}
					}
					catch(Exception)
					{
						// We weren't able to find the function in the DLL.
					}
					Marshal.FreeHGlobal(buffer);
				}

				// Special handling for some of the cases.
				String dir = null;
				switch(folder)
				{
					case SpecialFolder.System:
					{
						dir = DirMethods.GetSystemDirectory();
					}
					break;

					case SpecialFolder.ApplicationData:
					{
						dir = InfoMethods.GetUserStorageDir() +
							  Path.DirectorySeparatorChar +
							  "ApplicationData";
					}
					break;

					case SpecialFolder.LocalApplicationData:
					{
						dir = InfoMethods.GetUserStorageDir() +
							  Path.DirectorySeparatorChar +
							  "LocalApplicationData";
					}
					break;

					case SpecialFolder.CommonApplicationData:
					{
						dir = InfoMethods.GetUserStorageDir() +
							  Path.DirectorySeparatorChar +
							  "CommonApplicationData";
					}
					break;
				}
				if(dir != null && dir.Length > 0)
				{
					return dir;
				}

				// The empty string indicates that the value is not present.
				return String.Empty;
			}

	// Get a list of logical drives on the system.
	public static String[] GetLogicalDrives()
			{
				return DirMethods.GetLogicalDrives();
			}

#endif // !ECMA_COMPAT

	// Private class that implements a dictionary for environment variables.
	private sealed class EnvironmentDictionary : IDictionary
	{
		public EnvironmentDictionary()
				{
					// Nothing to do here.
				}

		// Add an object to this dictionary.
		public void Add(Object key, Object value)
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}

		// Clear this dictionary.
		public void Clear()
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}

		// Determine if this dictionary contains a specific key.
		public bool Contains(Object key)
				{
					String keyName = key.ToString();
					if(keyName != null)
					{
						return (TaskMethods.GetEnvironmentVariable(keyName)
									!= null);
					}
					else
					{
						throw new ArgumentNullException("key");
					}
				}

		// Copy the contents of this dictionary to an array.
		public void CopyTo(Array array, int index)
				{
					int count;
					if(array == null)
					{
						throw new ArgumentNullException("array");
					}
					if(index < 0)
					{
						throw new ArgumentOutOfRangeException
							("index", _("ArgRange_Array"));
					}
					if(array.Rank != 1)
					{
						throw new ArgumentException(_("Arg_RankMustBe1"));
					}
					count = TaskMethods.GetEnvironmentCount();
					if(index >= array.Length ||
					   count > (array.Length - index))
					{
						throw new ArgumentException
							(_("Arg_InvalidArrayIndex"));
					}
					int posn;
					for(posn = 0; posn < count; ++posn)
					{
						array.SetValue(new DictionaryEntry
								(TaskMethods.GetEnvironmentKey(posn),
								 TaskMethods.GetEnvironmentValue(posn)),
								index + posn);
					}
				}

		// Enumerate all values in this dictionary.
		IEnumerator IEnumerable.GetEnumerator()
				{
					return new EnvironmentEnumerator();
				}
		public IDictionaryEnumerator GetEnumerator()
				{
					return new EnvironmentEnumerator();
				}

		// Remove a value from this dictionary.
		public void Remove(Object key)
				{
					throw new NotSupportedException(_("NotSupp_ReadOnly"));
				}

		// Count the number of items in this dictionary.
		public int Count
				{
					get
					{
						return TaskMethods.GetEnvironmentCount();
					}
				}

		// Determine if this dictionary has a fixed size.
		public bool IsFixedSize
				{
					get
					{
						return true;
					}
				}

		// Determine if this dictionary is read-only.
		public bool IsReadOnly
				{
					get
					{
						return true;
					}
				}

		// Determine if this dictionary is synchronized.
		public bool IsSynchronized
				{
					get
					{
						return false;
					}
				}

		// Get the synchronization root for this dictionary.
		public Object SyncRoot
				{
					get
					{
						return this;
					}
				}

		// Get a particular object from this dictionary.
		public Object this[Object key]
				{
					get
					{
						String keyName = key.ToString();
						if(keyName != null)
						{
							return TaskMethods.GetEnvironmentVariable(keyName);
						}
						else
						{
							throw new ArgumentNullException("key");
						}
					}
					set
					{
						throw new NotSupportedException(_("NotSupp_ReadOnly"));
					}
				}

		// Get a list of all keys in this dictionary.
		public ICollection Keys
				{
					get
					{
						int count = TaskMethods.GetEnvironmentCount();
						String[] keys = new String [count];
						int posn;
						for(posn = 0; posn < count; ++posn)
						{
							keys[posn] = TaskMethods.GetEnvironmentKey(posn);
						}
						return keys;
					}
				}

		// Get a list of all values in this dictionary.
		public ICollection Values
				{
					get
					{
						int count = TaskMethods.GetEnvironmentCount();
						String[] values = new String [count];
						int posn;
						for(posn = 0; posn < count; ++posn)
						{
							values[posn] =
								TaskMethods.GetEnvironmentValue(posn);
						}
						return values;
					}
				}

	};

	// Private class for enumerating over the contents of the environment.
	private sealed class EnvironmentEnumerator : IDictionaryEnumerator
	{
		private int posn;
		private int count;

		// Constructor.
		public EnvironmentEnumerator()
				{
					posn = -1;
					count = TaskMethods.GetEnvironmentCount();
				}

		// Move to the next item in sequence.
		public bool MoveNext()
				{
					++posn;
					return (posn < count);
				}

		// Reset the enumerator.
		public void Reset()
				{
					posn = -1;
				}

		// Get the current enumerator value.
		public Object Current
				{
					get
					{
						return Entry;
					}
				}

		// Get the current dictionary entry value.
		public DictionaryEntry Entry
				{
					get
					{
						if(posn >= 0 && posn < count)
						{
							return new DictionaryEntry
								(TaskMethods.GetEnvironmentKey(posn),
								 TaskMethods.GetEnvironmentValue(posn));
						}
						else
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
					}
				}

		// Get the key associated with the current enumerator value.
		public Object Key
				{
					get
					{
						if(posn >= 0 && posn < count)
						{
							return TaskMethods.GetEnvironmentKey(posn);
						}
						else
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
					}
				}

		// Get the value associated with the current enumerator value.
		public Object Value
				{
					get
					{
						if(posn >= 0 && posn < count)
						{
							return TaskMethods.GetEnvironmentValue(posn);
						}
						else
						{
							throw new InvalidOperationException
								(_("Invalid_BadEnumeratorPosition"));
						}
					}
				}
	};

}; // class Environment

}; // namespace System
