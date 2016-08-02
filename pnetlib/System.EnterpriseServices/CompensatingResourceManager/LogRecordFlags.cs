/*
 * LogRecordFlags.cs - Implementation of the
 *			"System.EnterpriseServices.CompensatingResourceManager."
 *			"LogRecordFlags" class.
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

[Flags]
public enum LogRecordFlags
{
	ForgetTarget			= 0x0001,
	WrittenDuringPrepare	= 0x0002,
	WrittenDuringCommit		= 0x0004,
	WrittenDuringAbort		= 0x0008,
	WrittenDuringReplay		= 0x0020,
	WrittenDurringRecovery	= 0x0010,	// Mis-spelt on purpose!
	ReplayInProgress		= 0x0040,

}; // enum LogRecordFlags

}; // namespace System.EnterpriseServices.CompensatingResourceManager
