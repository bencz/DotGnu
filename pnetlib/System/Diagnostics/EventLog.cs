/*
 * EventLog.cs - Implementation of the
 *			"System.Diagnostics.EventLog" class.
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
using System.Security;

[DefaultEvent("EntryWritten")]
#if CONFIG_COMPONENT_MODEL
[DesignerAttribute("Microsoft.VisualStudio.Install.EventLogInstallableComponentDesigner, Microsoft.VisualStudio")]
[InstallerType("System.Diagnostics.EventLogInstaller, System.Configuration.Install")]
#endif
public class EventLog
#if CONFIG_COMPONENT_MODEL
	: Component, ISupportInitialize
#endif
{
	// Internal state.
	private String logName;
	private String machineName;
	private String source;
	private bool enableRaisingEvents;
	private EventLogEntryCollection entries;
#if CONFIG_COMPONENT_MODEL
	private ISynchronizeInvoke syncInvoke;
#endif
	private static ArrayList logs = new ArrayList();

	// Constructor.
	public EventLog() : this("", ".", "") {}
	public EventLog(String logName) : this(logName, ".", "") {}
	public EventLog(String logName, String machineName)
			: this(logName, machineName, "") {}
	public EventLog(String logName, String machineName, String source)
			{
				if(logName == null)
				{
					throw new ArgumentNullException("logName");
				}
				if(machineName == null)
				{
					throw new ArgumentNullException("machineName");
				}
				this.logName = logName;
				this.machineName = machineName;
				this.source = (source == null ? "" : source);
				this.entries = new EventLogEntryCollection();
			}

	// Get or set event log properties.
	[MonitoringDescription("LogMonitoring")]
	[DefaultValue(false)]
	[Browsable(false)]
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
	[MonitoringDescription("LogEntries")]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public EventLogEntryCollection Entries
			{
				get
				{
					return entries;
				}
			}
	[MonitoringDescription("LogLog")]
	[ReadOnly(true)]
	[DefaultValue("")]
	[RecommendedAsConfigurable(true)]
	[TypeConverter
		("System.Diagnostics.Design.LogConverter, System.Design")]
	public String Log
			{
				get
				{
					return logName;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					logName = value;
				}
			}
	[Browsable(false)]
	public String LogDisplayName
			{
				get
				{
					// We use the log name as the display name
					// in this implementation.
					return logName;
				}
			}
	[MonitoringDescription("LogMachineName")]
	[ReadOnly(true)]
	[DefaultValue(".")]
	[RecommendedAsConfigurable(true)]
	public String MachineName
			{
				get
				{
					return machineName;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					machineName = value;
				}
			}
	[MonitoringDescription("LogSource")]
	[DefaultValue("")]
	[ReadOnly(true)]
	[RecommendedAsConfigurable(true)]
	[TypeConverter
		("System.Diagnostics.Design.StringValueConverter, System.Design")]
	public String Source
			{
				get
				{
					return source;
				}
				set
				{
					source = (value == null ? "" : source);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[MonitoringDescription("LogSynchronizingObject")]
	[Browsable(false)]
	public ISynchronizeInvoke SynchronizingObject
			{
				get
				{
					return syncInvoke;
				}
				set
				{
					syncInvoke = value;
				}
			}
#endif

	// Implement the ISupportInitialize interface.
	public void BeginInit()
			{
				// Nothing to do here in this implementation.
			}
	public void EndInit()
			{
				// Nothing to do here in this implementation.
			}

	// Clear all entries from the event log.
	public void Clear()
			{
				entries.Clear();
			}

	// Close this event log.
	public void Close()
			{
				// Nothing to do here in this implementation.
			}

	// Create a new event source.
	public static void CreateEventSource(String source, String logName)
			{
				CreateEventSource(source, logName, ".");
			}
	public static void CreateEventSource
				(String source, String logName, String machineName)
			{
				// Validate the parameters.
				if(source == null || source == String.Empty)
				{
					throw new ArgumentNullException("source");
				}
				if(machineName == null)
				{
					throw new ArgumentNullException("machineName");
				}
				if(logName == null || logName == String.Empty)
				{
					logName = "Application";
				}

				// We can only create local sources in this implementation.
				if(machineName != ".")
				{
					throw new SecurityException
						(S._("Invalid_RemoteEventSource"));
				}

				// See if we already have a log with this name,
				// and bail out if we have.
				lock(typeof(EventLog))
				{
					foreach(EventLog log in logs)
					{
						if(log.Source == source &&
						   log.Log == logName &&
						   log.MachineName == machineName)
						{
							return;
						}
					}

					// Create a new event log on this machine.
					EventLog eventLog = new EventLog
						(logName, machineName, source);
					logs.Add(eventLog);
				}
			}

	// Delete an event log.
	public static void Delete(String logName)
			{
				Delete(logName, ".");
			}
	public static void Delete(String logName, String machineName)
			{
				// Validate the parameters.
				if(logName == null || logName == String.Empty)
				{
					throw new ArgumentNullException("logName");
				}
				if(machineName == null)
				{
					throw new ArgumentNullException("machineName");
				}
				if(machineName != ".")
				{
					throw new SecurityException
						(S._("Invalid_RemoteEventSource"));
				}

				// Look for the log and delete it.
				lock(typeof(EventLog))
				{
					foreach(EventLog log in logs)
					{
						if(log.Log == logName &&
						   log.MachineName == machineName)
						{
							logs.Remove(log);
							return;
						}
					}
				}
			}

	// Delete an event source.
	public static void DeleteEventSource(String source)
			{
				DeleteEventSource(source, ".");
			}
	public static void DeleteEventSource(String source, String machineName)
			{
				// Validate the parameters.
				if(source == null || source == String.Empty)
				{
					throw new ArgumentNullException("source");
				}
				if(machineName == null)
				{
					throw new ArgumentNullException("machineName");
				}
				if(machineName != ".")
				{
					throw new SecurityException
						(S._("Invalid_RemoteEventSource"));
				}

				// Look for the log and delete it.
				lock(typeof(EventLog))
				{
					foreach(EventLog log in logs)
					{
						if(log.Source == source &&
						   log.MachineName == machineName)
						{
							logs.Remove(log);
							return;
						}
					}
				}
			}

	// Dispose of this event log.
	protected override void Dispose(bool disposing)
			{
				// Nothing to do in this implementation.
			}

	// Determine if a particular log exists.
	public static bool Exists(String logName)
			{
				return Exists(logName, ".");
			}
	public static bool Exists(String logName, String machineName)
			{
				lock(typeof(EventLog))
				{
					foreach(EventLog log in logs)
					{
						if(log.Log == logName &&
						   log.MachineName == machineName)
						{
							return true;
						}
					}
					return false;
				}
			}

	// Get all active event logs.
	public static EventLog[] GetEventLogs()
			{
				return GetEventLogs(".");
			}
	public static EventLog[] GetEventLogs(String machineName)
			{
				lock(typeof(EventLog))
				{
					if(machineName == ".")
					{
						return (EventLog[])(logs.ToArray(typeof(EventLog)));
					}
					else
					{
						throw new SecurityException
							(S._("Invalid_RemoteEventSource"));
					}
				}
			}

	// Get the name of a log given its source name.
	public static String LogNameFromSourceName
				(String source, String machineName)
			{
				lock(typeof(EventLog))
				{
					foreach(EventLog log in logs)
					{
						if(log.Source == source &&
						   log.MachineName == machineName)
						{
							return log.Log;
						}
					}
					return null;
				}
			}

	// Determine if a particular event source exists.
	public static bool SourceExists(String source)
			{
				return SourceExists(source, ".");
			}
	public static bool SourceExists(String source, String machineName)
			{
				lock(typeof(EventLog))
				{
					foreach(EventLog log in logs)
					{
						if(log.Source == source &&
						   log.MachineName == machineName)
						{
							return true;
						}
					}
					return false;
				}
			}

	// Write an entry to this event log.
	private void WriteEntryToLog(String source, String message,
						   		 EventLogEntryType type, int eventID,
						   		 short category, byte[] rawData)
			{
				// Change to the new source.
				if(source != this.source)
				{
					Source = source;
				}

				// Create the entry.
				EventLogEntry entry = new EventLogEntry();
				entry.category = category.ToString();
				entry.categoryNumber = category;
				entry.data = rawData;
				entry.entryType = type;
				entry.eventID = eventID;
				entry.index = 0;
				entry.machineName = this.machineName;
				entry.message = message;
				entry.replacementStrings = null;
				entry.source = this.source;
				entry.timeWritten = entry.timeGenerated = DateTime.Now;
				entry.userName = Environment.UserName;

				// Write the entry to the log.
				entries.Add(entry);

				// Notify the listening event handlers for local events.
				if(enableRaisingEvents && machineName == "." &&
				   EntryWritten != null)
				{
					EntryWritten(null, new EntryWrittenEventArgs(entry));
				}
			}

	// Write an entry to an event log.
	public void WriteEntry(String message)
			{
				WriteEntryToLog(Source, message, EventLogEntryType.Information,
						  		0, (short)0, null);
			}
	public void WriteEntry(String message, EventLogEntryType type)
			{
				WriteEntryToLog(Source, message, type, 0, (short)0, null);
			}
	public void WriteEntry(String message, EventLogEntryType type, int eventID)
			{
				WriteEntryToLog(Source, message, type,
								eventID, (short)0, null);
			}
	public void WriteEntry(String message, EventLogEntryType type,
				   		   int eventID, short category)
			{
				WriteEntryToLog(Source, message, type, eventID,
								category, null);
			}
	public void WriteEntry(String message, EventLogEntryType type,
				   		   int eventID, short category, byte[] rawData)
			{
				WriteEntryToLog(Source, message, type, eventID,
						   		category, rawData);
			}
	public static void WriteEntry(String source, String message)
			{
				WriteEntry(source, message, EventLogEntryType.Information,
						   0, (short)0, null);
			}
	public static void WriteEntry(String source, String message,
						   		  EventLogEntryType type)
			{
				WriteEntry(source, message, type, 0, (short)0, null);
			}
	public static void WriteEntry(String source, String message,
						   		  EventLogEntryType type, int eventID)
			{
				WriteEntry(source, message, type, eventID, (short)0, null);
			}
	public static void WriteEntry(String source, String message,
						   		  EventLogEntryType type, int eventID,
						   		  short category)
			{
				WriteEntry(source, message, type, eventID,
						   category, null);
			}
	public static void WriteEntry(String source, String message,
						   		  EventLogEntryType type, int eventID,
						   		  short category, byte[] rawData)
			{
				lock(typeof(EventLog))
				{
					foreach(EventLog log in logs)
					{
						if(log.Source == source)
						{
							log.WriteEntryToLog(source, message, type,
												eventID, category, rawData);
						}
					}
				}
			}

	// Event that indicates when an entry is written to the log.
	[MonitoringDescription("LogEntryWritten")]
	public event EntryWrittenEventHandler EntryWritten;

}; // class EventLog

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
