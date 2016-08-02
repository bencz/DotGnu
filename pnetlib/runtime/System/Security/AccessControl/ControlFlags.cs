/*
 * ControlFlags.cs - Implementation of the
 *			"System.Security.AccessControl.ControlFlags" class.
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
public enum ControlFlags
{
	None									= 0x0000,
	OwnerDefaulted							= 0x0001,
	GroupDefaulted							= 0x0002,
	DiscretionaryAclPresent					= 0x0004,
	DiscretionaryAclDefaulted				= 0x0008,
	SystemAclPresent						= 0x0010,
	SystemAclDefaulted						= 0x0020,
	DiscretionaryAclUntrusted				= 0x0040,
	ServerSecurity							= 0x0080,
	DiscretionaryAclAutoInheritRequired		= 0x0100,
	SystemAclAutoInheritRequired			= 0x0200,
	DiscretionaryAclAutoInherited			= 0x0400,
	SystemAclAutoInherited					= 0x0800,
	DiscretionaryAclProtected				= 0x1000,
	SystemAclProtected						= 0x2000,
	RMControlValid							= 0x4000,
	SelfRelative							= 0x8000

}; // enum ControlFlags

#endif // CONFIG_ACCESS_CONTROL

}; // namespace System.Security.AccessControl
