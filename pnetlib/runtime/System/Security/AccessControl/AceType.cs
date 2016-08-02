/*
 * AceType.cs - Implementation of the
 *			"System.Security.AccessControl.AceType" class.
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

public enum AceType : byte
{
	AccessAllowed				= 0,
	AccessDenied				= 1,
	SystemAudit					= 2,
	SystemAlarm					= 3,
	AccessAllowedCompound		= 4,
	AccessAllowedObject			= 5,
	AccessDeniedObject			= 6,
	SystemAuditObject			= 7,
	SystemAlarmObject			= 8,
	AccessAllowedCallback		= 9,
	AccessDeniedCallback		= 10,
	AccessAllowedCallbackObject	= 11,
	AccessDeniedCallbackObject	= 12,
	SystemAuditCallback			= 13,
	SystemAlarmCallback			= 14,
	SystemAuditCallbackObject	= 15,
	SystemAlarmCallbackObject	= 16,
	MaxDefinedAceType			= 16

}; // enum AceType

#endif // CONFIG_ACCESS_CONTROL

}; // namespace System.Security.AccessControl
