/*
 * InstallContext.cs - Implementation of the
 *	    "System.Configuration.Install.InstallContext" class.
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

namespace System.Configuration.Install
{

#if !ECMA_COMPAT

using System.IO;
using System.Collections;
using System.Collections.Specialized;

public class InstallContext
{
    // Internal state.
    private String logFilePath;
    private StringDictionary parameters;

    // Constructors.
    public InstallContext()
			{
				this.logFilePath = null;
				this.parameters = new StringDictionary();
			}
    public InstallContext(String logFilePath, String[] commandLine)
			{
				this.logFilePath = logFilePath;
				this.parameters = ParseCommandLine(commandLine);
			}
    internal InstallContext(StringDictionary parameters)
			{
				this.logFilePath = parameters["logfile"];
				this.parameters = parameters;
			}

    // Get the command-line parameters.
    public StringDictionary Parameters
			{
				get
				{
					return parameters;
				}
			}

    // Determine if a particular parameter is true.
    public bool IsParameterTrue(String paramName)
			{
				String value = parameters[paramName.ToLower()];
				if(value == null)
				{
					// Special check for "--x" forms of option names.
					value = parameters["-" + paramName.ToLower()];
				}
				if(value == null)
				{
					return false;
				}
				else if(String.Compare(value, "true", true) == 0 ||
						String.Compare(value, "yes", true) == 0 ||
						value == "1" || value == String.Empty)
				{
				    return true;
				}
				else
				{
				    return false;
				}
		    }

    // Write a message to the log file.
    public void LogMessage(String message)
		    {
				if(logFilePath != null && message != null)
				{
				    if(IsParameterTrue("LogToConsole"))
				    {
						Console.Write(message);
				    }
				    StreamWriter writer = new StreamWriter(logFilePath, true);
				    writer.Write(message);
				    writer.Flush();
				    writer.Close();
				}
		    }
    internal void LogLine(String message)
			{
				if(logFilePath != null && message != null)
				{
				    if(IsParameterTrue("LogToConsole"))
				    {
						Console.WriteLine(message);
				    }
				    StreamWriter writer = new StreamWriter(logFilePath, true);
				    writer.WriteLine(message);
				    writer.Flush();
				    writer.Close();
				}
			}

	// Add to a string dictionary, overridding previous values.
	private static void Add(StringDictionary dict, String name, String value)
			{
				if(dict[name] != null)
				{
					dict.Remove(name);
				}
				dict[name] = value;
			}

    // Parse a command line into a string dictionary.
    protected static StringDictionary ParseCommandLine(String[] args)
		    {
				StringDictionary dict = new StringDictionary();
				if(args != null)
				{
				    int posn = 0;
				    String str;
				    while(posn < args.Length)
				    {
						str = args[posn];
						if(str.Length > 0 &&
						   (str[0] == '/' || str[0] == '-'))
						{
						    int index = str.IndexOf('=');
						    if(index < 0)
						    {
								if((posn + 1) < args.Length &&
								   args[posn + 1].StartsWith("="))
								{
								    if(args[posn + 1].Length == 1)
								    {
										// Option of the form "/name = value".
										if((posn + 2) < args.Length)
										{
										    Add(dict,
												str.Substring(1).ToLower(),
												 "");
										    ++posn;
										}
										else
										{
										    Add(dict,
												str.Substring(1).ToLower(),
												 args[posn + 2]);
										    posn += 2;
										}
								    }
								    else
								    {
										// Option of the form "/name =value".
										Add(dict, str.Substring(1).ToLower(),
												 args[posn + 1].Substring(1));
										++posn;
								    }
								}
								else
								{
								    // Option of the form "/name".
								    Add(dict, str.Substring(1).ToLower(), "");
								}
						    }
						    else if((index + 1) < str.Length &&
									(posn + 1) < args.Length)
						    {
								// Option of the form "/name= value".
								Add(dict, str.Substring(1, index - 1).ToLower(),
										 args[posn + 1]);
						    }
						    else
						    {
								// Option of the form "/name=value".
								Add(dict, str.Substring(1, index - 1).ToLower(),
										 str.Substring(index + 1));
						    }
						}
						++posn;
				    }
				}
				return dict;
		    }
    internal static StringDictionary ParseCommandLine
				(String[] args, int start, int length, out String[] newArgs)
			{
				newArgs = new String [length];
				Array.Copy(args, start, newArgs, 0, length);
				return ParseCommandLine(newArgs);
			}

}; // class InstallContext

#endif // !ECMA_COMPAT

}; // namespace System.Configuration.Install
