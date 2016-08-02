/*
 * IPermission.cs - Implementation of the
 *		"System.Security.IPermission" interface.
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

namespace System.Security
{

#if CONFIG_PERMISSIONS

using System;

public interface IPermission : ISecurityEncodable
{

	// Copy this permission object.
	IPermission Copy();

	// Throw an exception if the caller does not have
	// the specified permissions.
	void Demand();

	// Return the intersection of two permission objects.
	IPermission Intersect(IPermission target);

	// Determine if this object has a subset of another object's permissions.
	bool IsSubsetOf(IPermission target);

	// Return the union of two permission objects.
	IPermission Union(IPermission target);

}; // interface IPermission

#endif // CONFIG_PERMISSIONS

}; // namespace System.Security
