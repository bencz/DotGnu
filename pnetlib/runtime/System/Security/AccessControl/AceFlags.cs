/*
 * AceFlags.cs - Implementation of the
 *			"System.Security.AccessControl.AceFlags" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

namespace System.Security.AccessControl
{

#if CONFIG_ACCESS_CONTROL

[Flags]
public enum AceFlags : byte
{
	None				= 0x0000,
	ObjectInherit		= 0x0001,
	ContainerInherit	= 0x0002,
	NoPropagateInherit	= 0x0004,
	InheritOnly			= 0x0008,
	InheritanceFlags	= 0x000F,
	Inherited			= 0x0010,
	SuccessfulAccess	= 0x0040,
	FailedAccess		= 0x0080,
	AuditFlags			= 0x00C0,

}; // enum AceFlags

#endif // CONFIG_ACCESS_CONTROL

}; // namespace System.Security.AccessControl
