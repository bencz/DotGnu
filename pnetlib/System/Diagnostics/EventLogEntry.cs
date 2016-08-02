/*
 * EventLogEntry.cs - Implementation of the
 *			"System.Diagnostics.EventLogEntry" class.
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
using System.Runtime.Serialization;

[Serializable]
#if CONFIG_COMPONENT_MODEL
[DesignTimeVisible(false)]
[ToolboxItem(false)]
#endif
public sealed class EventLogEntry
#if CONFIG_COMPONENT_MODEL
	: Component
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
#elif CONFIG_SERIALIZATION
	: ISerializable
#endif
{
	// Internal state.
	internal String category;
	internal short categoryNumber;
	internal byte[] data;
	internal EventLogEntryType entryType;
	internal int eventID;
	internal int index;
	internal String machineName;
	internal String message;
	internal String[] replacementStrings;
	internal String source;
	internal DateTime timeGenerated;
	internal DateTime timeWritten;
	internal String userName;

	// Constructor.
	internal EventLogEntry() {}
#if CONFIG_SERIALIZATION
	internal EventLogEntry(SerializationInfo info, StreamingContext context)
			{
				// The serialization uses a binary format which
				// we don't yet know the details of.
				throw new NotImplementedException();
			}
#endif

	// Event log properties.
	[MonitoringDescription("LogEntryCategory")]
	public String Category
			{
				get
				{
					return category;
				}
			}
	[MonitoringDescription("LogEntryCategoryNumber")]
	public short CategoryNumber
			{
				get
				{
					return categoryNumber;
				}
			}
	[MonitoringDescription("LogEntryData")]
	public byte[] Data
			{
				get
				{
					return data;
				}
			}
	[MonitoringDescription("LogEntryEntryType")]
	public EventLogEntryType EntryType
			{
				get
				{
					return entryType;
				}
			}
	[MonitoringDescription("LogEntryEventID")]
	public int EventID
			{
				get
				{
					return eventID;
				}
			}
	[MonitoringDescription("LogEntryIndex")]
	public int Index
			{
				get
				{
					return index;
				}
			}
	[MonitoringDescription("LogEntryMachineName")]
	public String MachineName
			{
				get
				{
					return machineName;
				}
			}
	[MonitoringDescription("LogEntryMessage")]
#if CONFIG_COMPONENT_MODEL
	[Editor("System.ComponentModel.Design.BinaryEditor, System.Design",
			"System.Drawing.Design.UITypeEditor, System.Drawing")]
#endif
	public String Message
			{
				get
				{
					return message;
				}
			}
	[MonitoringDescription("LogEntryReplacementStrings")]
	public String[] ReplacementStrings
			{
				get
				{
					return replacementStrings;
				}
			}
	[MonitoringDescription("LogEntrySource")]
	public String Source
			{
				get
				{
					return source;
				}
			}
	[MonitoringDescription("LogEntryTimeGenerated")]
	public DateTime TimeGenerated
			{
				get
				{
					return timeGenerated;
				}
			}
	[MonitoringDescription("LogEntryTimeWritten")]
	public DateTime TimeWritten
			{
				get
				{
					return timeWritten;
				}
			}
	[MonitoringDescription("LogEntryUserName")]
	public String UserName
			{
				get
				{
					return userName;
				}
			}

	// Determine if two event log entries are equal.
	public bool Equals(EventLogEntry otherEntry)
			{
				if(otherEntry == null)
				{
					return false;
				}
				if(category != otherEntry.category)
				{
					return false;
				}
				if(categoryNumber != otherEntry.categoryNumber)
				{
					return false;
				}
				if(data == null)
				{
					if(otherEntry.data != null)
					{
						return false;
					}
				}
				else if(otherEntry.data == null)
				{
					return false;
				}
				else if(otherEntry.data.Length != data.Length)
				{
					return false;
				}
				else
				{
					int dposn;
					for(dposn = 0; dposn < data.Length; ++dposn)
					{
						if(data[dposn] != otherEntry.data[dposn])
						{
							return false;
						}
					}
				}
				if(entryType != otherEntry.entryType)
				{
					return false;
				}
				if(eventID != otherEntry.eventID)
				{
					return false;
				}
				if(index != otherEntry.index)
				{
					return false;
				}
				if(machineName != otherEntry.machineName)
				{
					return false;
				}
				if(message != otherEntry.message)
				{
					return false;
				}
				if(replacementStrings == null)
				{
					if(otherEntry.replacementStrings != null)
					{
						return false;
					}
				}
				else if(otherEntry.replacementStrings == null)
				{
					return false;
				}
				else if(otherEntry.replacementStrings.Length
							!= replacementStrings.Length)
				{
					return false;
				}
				else
				{
					int rposn;
					for(rposn = 0; rposn < replacementStrings.Length; ++rposn)
					{
						if(replacementStrings[rposn] !=
						   otherEntry.replacementStrings[rposn])
						{
							return false;
						}
					}
				}
				if(source != otherEntry.source)
				{
					return false;
				}
				if(timeGenerated != otherEntry.timeGenerated)
				{
					return false;
				}
				if(timeWritten != otherEntry.timeWritten)
				{
					return false;
				}
				if(userName != otherEntry.userName)
				{
					return false;
				}
				return true;
			}

#if CONFIG_SERIALIZATION
	// Implement the ISerializable interface.
	void ISerializable.GetObjectData(SerializationInfo info,
									 StreamingContext context)
			{
				// The serialization uses a binary format which
				// we don't yet know the details of.
				throw new NotImplementedException();
			}
#endif

}; // class EventLogEntry

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
