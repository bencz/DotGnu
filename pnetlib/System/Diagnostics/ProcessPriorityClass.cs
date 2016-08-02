/*
 * ProcessPriorityClass.cs - Implementation of the
 *			"System.Diagnostics.ProcessPriorityClass" class.
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
public enum ProcessPriorityClass
{

	Normal      = 0x0020,
	Idle        = 0x0040,
	High        = 0x0080,
	RealTime    = 0x0100,
	BelowNormal = 0x4000,
	AboveNormal = 0x8000

}; // enum ProcessPriorityClass

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics
