/*
 * EventLogTraceListener.cs - Implementation of the
 *			"System.Diagnostics.EventLogTraceListener" class.
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

public sealed class EventLogTraceListener : TraceListener
{
	// Internal state.
	private EventLog eventLog;

	// Constructor.
	public EventLogTraceListener() {}
	public EventLogTraceListener(EventLog eventLog)
			{
				this.eventLog = eventLog;
			}
	public EventLogTraceListener(String source)
			{
				eventLog = new EventLog();
				eventLog.Source = source;
			}

	// Get or set the event log to output trace messages to.
	public EventLog EventLog
			{
				get
				{
					return eventLog;
				}
				set
				{
					eventLog = value;
				}
			}

	// Get or set the name of the event log.
	public override String Name
			{
				get
				{
					String name = base.Name;
					if(name == null || name == String.Empty)
					{
						if(eventLog != null)
						{
							name = base.Name = eventLog.Source;
						}
					}
					return name;
				}
				set
				{
					base.Name = value;
				}
			}

	// Close this trace listener.
	public override void Close()
			{
				if(eventLog != null)
				{
					eventLog.Close();
				}
			}

	// Dispose of this trace listener.
	protected override void Dispose(bool disposing)
			{
				if(disposing)
				{
					Close();
				}
			}

	// Write data to this trace listener's output stream.
	public override void Write(String message)
			{
				if(eventLog != null)
				{
					eventLog.WriteEntry(message);
				}
			}

	// Write data to this trace listener's output stream followed by newline.
	public override void WriteLine(String message)
			{
				Write(message);
			}

}; // class EventLogTraceListener

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
