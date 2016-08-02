/*
 * LogRecord.cs - Implementation of the
 *			"System.EnterpriseServices.CompensatingResourceManager."
 *			"LogRecord" class.
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

namespace System.EnterpriseServices.CompensatingResourceManager
{

public sealed class LogRecord
{
	// Internal state.
	private LogRecordFlags flags;
	private Object record;
	private int sequence;

	// Constructor.
	internal LogRecord(LogRecordFlags flags, Object record, int sequence)
			{
				this.flags = flags;
				this.record = record;
				this.sequence = sequence;
			}

	// Get this object's properties.
	public LogRecordFlags Flags
			{
				get
				{
					return flags;
				}
			}
	public Object Record
			{
				get
				{
					return record;
				}
			}
	public int Sequence
			{
				get
				{
					return sequence;
				}
			}

}; // class LogRecord

}; // namespace System.EnterpriseServices.CompensatingResourceManager
