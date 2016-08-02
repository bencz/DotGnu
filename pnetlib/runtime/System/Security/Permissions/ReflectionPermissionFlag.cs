/*
 * ReflectionPermissionFlag.cs - Implementation of the
 *			"System.Security.Permissions.ReflectionPermissionFlag" class.
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

namespace System.Security.Permissions
{

#if CONFIG_PERMISSIONS && CONFIG_REFLECTION

[Flags]
public enum ReflectionPermissionFlag
{

	NoFlags         = 0x0000,
	TypeInformation = 0x0001,
	MemberAccess    = 0x0002,
#if !ECMA_COMPAT
	ReflectionEmit  = 0x0004,
	AllFlags        = 0x0007
#endif

}; // enum ReflectionPermissionFlag

#endif // CONFIG_PERMISSIONS && CONFIG_REFLECTION

}; // namespace System.Security.Permissions
