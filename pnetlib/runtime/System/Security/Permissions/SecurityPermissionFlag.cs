/*
 * SecurityPermissionFlag.cs - Implementation of the
 *			"System.Security.Permissions.SecurityPermissionFlag" class.
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

using System;

[Flags]
public enum SecurityPermissionFlag
{

	NoFlags                = 0x0000,
	Assertion              = 0x0001,
	UnmanagedCode          = 0x0002,
	SkipVerification       = 0x0004,
	Execution              = 0x0008,
	ControlThread          = 0x0010,
#if !ECMA_COMPAT
	ControlEvidence        = 0x0020,
	ControlPolicy          = 0x0040,
	SerializationFormatter = 0x0080,
	ControlDomainPolicy    = 0x0100,
	ControlPrincipal       = 0x0200,
	ControlAppDomain       = 0x0400,
	RemotingConfiguration  = 0x0800,
	Infrastructure         = 0x1000,
	BindingRedirects	   = 0x2000,
	AllFlags               = 0x3FFF
#endif // !ECMA_COMPAT

}; // enum SecurityPermissionFlag

}; // namespace System.Security.Permissions
