/*
 * ThreadState.cs - Implementation of the
 *		"System.Threading.ThreadState" enumeration.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Threading
{

[Flags]
public enum ThreadState
{
	Running          = 0x0000,
	StopRequested    = 0x0001,
	SuspendRequested = 0x0002,
	Background       = 0x0004,
	Unstarted        = 0x0008,
	Stopped          = 0x0010,
	WaitSleepJoin    = 0x0020,
	Suspended        = 0x0040,
	AbortRequested   = 0x0080,
	Aborted          = 0x0100

}; // enum ThreadState

}; // namespace System.Threading
