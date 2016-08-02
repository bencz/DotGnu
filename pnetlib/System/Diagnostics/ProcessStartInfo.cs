/*
 * ProcessStartInfo.cs - Implementation of the
 *			"System.Diagnostics.ProcessStartInfo" class.
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
using System.Collections;
using System.Collections.Specialized;
using System.Text;

#if CONFIG_COMPONENT_MODEL
[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
public sealed class ProcessStartInfo
{
	// Special flags for starting processes.
	[Flags]
	internal enum ProcessStartFlags
	{
		CreateNoWindow  = 0x0001,
		ErrorDialog     = 0x0002,
		RedirectStdin   = 0x0004,
		RedirectStdout  = 0x0008,
		RedirectStderr  = 0x0010,
		UseShellExecute = 0x0020,
		ExecOverTop		= 0x0040

	}; // enum ProcessStartFlags

	// Internal state.
	private String arguments;
	private String fileName;
	internal ProcessStartFlags flags;
	internal StringDictionary envVars;
	private IntPtr errorDialogParent;
	private String verb;
	private ProcessWindowStyle style;
	private String workingDirectory;

	// Constructors.
	public ProcessStartInfo() : this(null, null) {}
	public ProcessStartInfo(String fileName) : this(fileName, null) {}
	public ProcessStartInfo(String fileName, String arguments)
			{
				this.fileName = fileName;
				this.arguments = arguments;
				this.flags = ProcessStartFlags.UseShellExecute;
			}

	// Get a process start flag.
	private bool GetFlag(ProcessStartFlags flag)
			{
				return ((flags & flag) != 0);
			}

	// Set a process start flag.
	private void SetFlag(ProcessStartFlags flag, bool value)
			{
				if(value)
				{
					flags |= flag;
				}
				else
				{
					flags &= ~flag;
				}
			}

	// Get or set object properties.
	[RecommendedAsConfigurable(true)]
	[DefaultValue("")]
	[MonitoringDescription("ProcessArguments")]
	[TypeConverter
		("System.Diagnostics.Design.StringValueConverter, System.Design")]
	public String Arguments
			{
				get
				{
					if(arguments == null)
					{
						return String.Empty;
					}
					else
					{
						return arguments;
					}
				}
				set
				{
					arguments = value;
				}
			}
	[MonitoringDescription("ProcessCreateNoWindow")]
	[DefaultValue(false)]
	public bool CreateNoWindow
			{
				get
				{
					return GetFlag(ProcessStartFlags.CreateNoWindow);
				}
				set
				{
					SetFlag(ProcessStartFlags.CreateNoWindow, value);
				}
			}
	[MonitoringDescription("ProcessEnvironmentVariables")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Editor
		("System.Diagnostics.Design.StringDictionaryEditor, System.Design",
		 "System.Drawing.Design.UITypeEditor, System.Drawing")]
	public StringDictionary EnvironmentVariables
			{
				get
				{
					if(envVars == null)
					{
						envVars = new StringDictionary();
						IDictionary env = Environment.GetEnvironmentVariables();
						if(env != null)
						{
							IDictionaryEnumerator e = env.GetEnumerator();
							while(e.MoveNext())
							{
								envVars.Add((String)(e.Key), (String)(e.Value));
							}
						}
					}
					return envVars;
				}
			}
	[MonitoringDescription("ProcessErrorDialog")]
	[DefaultValue(false)]
	public bool ErrorDialog
			{
				get
				{
					return GetFlag(ProcessStartFlags.ErrorDialog);
				}
				set
				{
					SetFlag(ProcessStartFlags.ErrorDialog, value);
				}
			}
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public IntPtr ErrorDialogParentHandle
			{
				get
				{
					return errorDialogParent;
				}
				set
				{
					errorDialogParent = value;
				}
			}
	[Editor
		("System.Diagnostics.Design.StartFileNameEditor, System.Design",
		 "System.Drawing.Design.UITypeEditor, System.Drawing")]
	[TypeConverter
		("System.Diagnostics.Design.StringValueConverter, System.Design")]
	[DefaultValue("")]
	[MonitoringDescription("ProcessFileName")]
	[RecommendedAsConfigurable(true)]
	public String FileName
			{
				get
				{
					if(fileName == null)
					{
						return String.Empty;
					}
					else
					{
						return fileName;
					}
				}
				set
				{
					fileName = value;
				}
			}
	[MonitoringDescription("ProcessRedirectStandardError")]
	[DefaultValue(false)]
	public bool RedirectStandardError
			{
				get
				{
					return GetFlag(ProcessStartFlags.RedirectStderr);
				}
				set
				{
					SetFlag(ProcessStartFlags.RedirectStderr, value);
				}
			}
	[MonitoringDescription("ProcessRedirectStandardInput")]
	[DefaultValue(false)]
	public bool RedirectStandardInput
			{
				get
				{
					return GetFlag(ProcessStartFlags.RedirectStdin);
				}
				set
				{
					SetFlag(ProcessStartFlags.RedirectStdin, value);
				}
			}
	[MonitoringDescription("ProcessRedirectStandardOutput")]
	[DefaultValue(false)]
	public bool RedirectStandardOutput
			{
				get
				{
					return GetFlag(ProcessStartFlags.RedirectStdout);
				}
				set
				{
					SetFlag(ProcessStartFlags.RedirectStdout, value);
				}
			}
	[MonitoringDescription("ProcessUseShellExecute")]
	[DefaultValue(true)]
	public bool UseShellExecute
			{
				get
				{
					return GetFlag(ProcessStartFlags.UseShellExecute);
				}
				set
				{
					SetFlag(ProcessStartFlags.UseShellExecute, value);
				}
			}
	[MonitoringDescription("ProcessVerb")]
	[DefaultValue("")]
	[TypeConverter("System.Diagnostics.Design.VerbConverter, System.Design")]
	public String Verb
			{
				get
				{
					if(verb == null)
					{
						return String.Empty;
					}
					else
					{
						return verb;
					}
				}
				set
				{
					verb = value;
				}
			}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public String[] Verbs
			{
				get
				{
					// We don't use verb lists in this implementation.
					return new String [0];
				}
			}
	[MonitoringDescription("ProcessWindowStyle")]
	public ProcessWindowStyle WindowStyle
			{
				get
				{
					return style;
				}
				set
				{
					if(((int)value) < ((int)(ProcessWindowStyle.Normal)) ||
					   ((int)value) > ((int)(ProcessWindowStyle.Maximized)))
					{
						throw new InvalidEnumArgumentException
							("value", (int)value, typeof(ProcessWindowStyle));
					}
					style = value;
				}
			}
	[MonitoringDescription("ProcessWorkingDirectory")]
	[DefaultValue("")]
	[RecommendedAsConfigurable(true)]
	[Editor
		("System.Diagnostics.Design.WorkingDirectoryEditor, System.Design",
		 "System.Drawing.Design.UITypeEditor, System.Drawing")]
	[TypeConverter
		("System.Diagnostics.Design.StringValueConverter, System.Design")]
	public String WorkingDirectory
			{
				get
				{
					if(workingDirectory == null)
					{
						return String.Empty;
					}
					else
					{
						return workingDirectory;
					}
				}
				set
				{
					workingDirectory = value;
				}
			}

	// Quote a string if necessary.
	private static String Quote(String str)
			{
				// Handle the empty string case first.
				if(str == null || str == String.Empty)
				{
					return "\"\"";
				}

				// Determine if there is a character that needs quoting.
				bool quote = false;
				foreach(char ch in str)
				{
					if(Char.IsWhiteSpace(ch) || ch == '"' || ch == '\'')
					{
						quote = true;
						break;
					}
				}
				if(!quote)
				{
					return str;
				}

				// Quote the string and return it.
				StringBuilder builder = new StringBuilder();
				builder.Append('"');
				foreach(char ch2 in str)
				{
					if(ch2 == '"')
					{
						builder.Append('"');
						builder.Append('"');
					}
					else
					{
						builder.Append(ch2);
					}
				}
				builder.Append('"');
				return builder.ToString();
			}

	// Convert an argv array into an argument string, with appropriate quoting.
	internal static String ArgVToArguments
				(String[] argv, int startIndex, String separator)
			{
				if(argv == null)
				{
					return String.Empty;
				}
				StringBuilder builder = new StringBuilder();
				int index = startIndex;
				while(index < argv.Length)
				{
					if(index > startIndex)
					{
						builder.Append(separator);
					}
					builder.Append(Quote(argv[index]));
					++index;
				}
				return builder.ToString();
			}

	// Convert an argument string into an argv array, undoing quoting.
	internal static String[] ArgumentsToArgV(String arguments)
			{
				// Handle the null case first.
				if(arguments == null)
				{
					return new String [0];
				}

				// Count the number of arguments in the string.
				int count = 0;
				int posn = 0;
				char ch, quotech;
				while(posn < arguments.Length)
				{
					ch = arguments[posn];
					if(Char.IsWhiteSpace(ch))
					{
						++posn;
						continue;
					}
					++count;
					if(ch == '"' || ch == '\'')
					{
						// Start of a quoted argument.
						++posn;
						quotech = ch;
						while(posn < arguments.Length)
						{
							ch = arguments[posn];
							if(ch == quotech)
							{
								if((posn + 1) < arguments.Length &&
								   arguments[posn + 1] == quotech)
								{
									// Escaped quote character.
									++posn;
								}
								else
								{
									// End of quoted sequence.
									++posn;
									break;
								}
							}
							++posn;
						}
					}
					else
					{
						// Start of an unquoted argument.
						while(posn < arguments.Length)
						{
							ch = arguments[posn];
							if(Char.IsWhiteSpace(ch) ||
							   ch == '"' || ch == '\'')
							{
								break;
							}
							++posn;
						}
					}
				}

				// Create the argument array and populate it.
				String[] argv = new String [count];
				StringBuilder builder;
				int start;
				count = 0;
				posn = 0;
				while(posn < arguments.Length)
				{
					ch = arguments[posn];
					if(Char.IsWhiteSpace(ch))
					{
						++posn;
						continue;
					}
					if(ch == '"' || ch == '\'')
					{
						// Start of a quoted argument.
						++posn;
						quotech = ch;
						builder = new StringBuilder();
						while(posn < arguments.Length)
						{
							ch = arguments[posn];
							if(ch == quotech)
							{
								if((posn + 1) < arguments.Length &&
								   arguments[posn + 1] == quotech)
								{
									// Escaped quote character.
									builder.Append(ch);
									++posn;
								}
								else
								{
									// End of quoted sequence.
									++posn;
									break;
								}
							}
							else
							{
								builder.Append(ch);
							}
							++posn;
						}
						argv[count++] = builder.ToString();
					}
					else
					{
						// Start of an unquoted argument.
						start = posn;
						while(posn < arguments.Length)
						{
							ch = arguments[posn];
							if(Char.IsWhiteSpace(ch) ||
							   ch == '"' || ch == '\'')
							{
								break;
							}
							++posn;
						}
						argv[count++] = arguments.Substring
							(start, posn - start);
					}
				}

				// Return the argument array to the caller.
				return argv;
			}

}; // class ProcessStartInfo

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
