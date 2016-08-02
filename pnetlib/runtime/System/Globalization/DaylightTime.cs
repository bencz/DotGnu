/*
 * DaylightTime.cs - Implementation of the
 *        "System.Globalization.DaylightTime" class.
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

namespace System.Globalization
{

#if !ECMA_COMPAT

using System;

[Serializable]
public class DaylightTime
{
	// Internal state.
	private DateTime start;
	private DateTime end;
	private TimeSpan delta;

	// Constructor.
	public DaylightTime(DateTime start, DateTime end, TimeSpan delta)
			{
				this.start = start;
				this.end = end;
				this.delta = delta;
			}

	// Get this object's properties.
	public DateTime Start
			{
				get
				{
					return start;
				}
			}
	public DateTime End
			{
				get
				{
					return end;
				}
			}
	public TimeSpan Delta
			{
				get
				{
					return delta;
				}
			}

}; // class DaylightTime

#endif // !ECMA_COMPAT

}; // namespace System.Globalization
