/*
 * WaitForChangedResult.cs - Implementation of the
 *		"System.IO.WaitForChangedResult" class.
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

namespace System.IO
{

#if CONFIG_WIN32_SPECIFICS

public struct WaitForChangedResult
{
	// Internal state.
	private WatcherChangeTypes changeType;
	private String name;
	private String oldName;
	private bool timedOut;

	// Constructor.
	internal WaitForChangedResult
				(WatcherChangeTypes changeType, bool timedOut)
			{
				this.changeType = changeType;
				this.name = null;
				this.oldName = null;
				this.timedOut = timedOut;
			}

	// Get or set this structure's properties.
	public WatcherChangeTypes ChangeType
			{
				get
				{
					return changeType;
				}
				set
				{
					changeType = value;
				}
			}
	public String Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}
	public String OldName
			{
				get
				{
					return oldName;
				}
				set
				{
					oldName = value;
				}
			}
	public bool TimedOut
			{
				get
				{
					return timedOut;
				}
				set
				{
					timedOut = value;
				}
			}

}; // struct WaitForChangedResult

#endif // CONFIG_WIN32_SPECIFICS

}; // namespace System.IO
