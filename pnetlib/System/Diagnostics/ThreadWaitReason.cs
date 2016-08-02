/*
 * ThreadWaitReason.cs - Implementation of the
 *			"System.Diagnostics.ThreadWaitReason" class.
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
public enum ThreadWaitReason
{
	Executive        = 0,
	FreePage         = 1,
	PageIn           = 2,
	SystemAllocation = 3,
	ExecutionDelay   = 4,
	Suspended        = 5,
	UserRequest      = 6,
	EventPairHigh    = 7,
	EventPairLow     = 8,
	LpcReceive       = 9,
	LpcReply         = 10,
	VirtualMemory    = 11,
	PageOut          = 12,
	Unknown          = 13,

}; // enum ThreadWaitReason

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
