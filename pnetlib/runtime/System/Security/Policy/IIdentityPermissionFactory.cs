/*
 * IIdentityPermissionFactory.cs - Implementation of the
 *		"System.Security.Policy.IIdentityPermissionFactory" class.
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

namespace System.Security.Policy
{

#if CONFIG_POLICY_OBJECTS && CONFIG_PERMISSIONS

public interface IIdentityPermissionFactory
{

	// Create a new identity permission for the specified evidence.
	IPermission CreateIdentityPermission(Evidence evidence);

}; // interface IIdentityPermissionFactory

#endif // CONFIG_POLICY_OBJECTS && CONFIG_PERMISSIONS

}; // namespace System.Security.Policy
