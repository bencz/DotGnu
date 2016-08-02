/*
 * EntryWrittenEventArgs.cs - Implementation of the
 *			"System.Diagnostics.EntryWrittenEventArgs" class.
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

[Serializable]
public class EntryWrittenEventArgs : EventArgs
{
	// Internal state.
	private EventLogEntry entry;

	// Constructor.
	public EntryWrittenEventArgs()
			{
				this.entry = null;
			}
	public EntryWrittenEventArgs(EventLogEntry entry)
			{
				this.entry = entry;
			}

	// Get the entry that was written to the log.
	public EventLogEntry Entry
			{
				get
				{
					return entry;
				}
			}

}; // class EntryWrittenEventArgs

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
