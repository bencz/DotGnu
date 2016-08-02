/*
 * IsolatedStorage.cs - Implementation of the
 *		"System.IO.IsolatedStorage.IsolatedStorage" class.
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

namespace System.IO.IsolatedStorage
{

#if CONFIG_ISOLATED_STORAGE

using System.Security;
using System.Security.Permissions;

// Isolated storage is used in the .NET Framework SDK to create "compartments"
// that are physically separated from the rest of the file system and from
// "less trusted" applications, on a per user or per domain basis.
//
// Physical separation isn't really possible on most platforms, and arguably
// it is a bad idea as it may prevent the user from accessing data created by
// applications running on their system.
//
// In this implementation, we use the regular filesystem operations with no
// physical separation, and let the underlying runtime engine and operating
// system worry about the security issues.  Per user and per domain profiles
// are handled by basing all operations in a store on a "base" directory.

public abstract class IsolatedStorage : MarshalByRefObject
{
	// Internal state.
	private IsolatedStorageScope scope;
	private Object assemblyIdentity;
	private Object domainIdentity;

	// Constructor.
	protected IsolatedStorage() {}

	// Get the current amount of space that has been used.
	[CLSCompliant(false)]
	public virtual ulong CurrentSize
			{
				get
				{
					throw new InvalidOperationException
						(_("Invalid_IsolatedCurrentSize"));
				}
			}

	// Get the assembly identity for this isolated storage object.
	public Object AssemblyIdentity
			{
				get
				{
					return assemblyIdentity;
				}
			}

	// Get the domain identity for this isolated storage object.
	public Object DomainIdentity
			{
				get
				{
					return domainIdentity;
				}
			}

	// Get the maximum amount of space that can be used.
	[CLSCompliant(false)]
	public virtual ulong MaximumSize
			{
				get
				{
					throw new InvalidOperationException
						(_("Invalid_IsolatedQuota"));
				}
			}

	// Get the internal path separator.
	protected virtual char SeparatorInternal
			{
				get
				{
					return '.';
				}
			}

	// Get the external path separator.
	protected virtual char SeparatorExternal
			{
				get
				{
					return '\\';
				}
			}

	// Get the scope of this storage object.
	public IsolatedStorageScope Scope
			{
				get
				{
					return scope;
				}
			}

#if CONFIG_PERMISSIONS

	// Get isolated storage permission information from a permission set.
	protected abstract IsolatedStoragePermission
			GetPermission(PermissionSet ps);

#endif

	// Initialise this storage object.  We don't use the evidence information,
	// because we let the underlying filesystem enforce security constraints.
	protected void InitStore(IsolatedStorageScope scope,
							 Type domainEvidenceType,
							 Type assemblyEvidenceType)
			{
				this.scope = scope;
				this.assemblyIdentity = null;
				this.domainIdentity = null;
			}

	// Remove this isolated storage object.
	public abstract void Remove();

}; // class IsolatedStorage

#endif // CONFIG_ISOLATED_STORAGE

}; // namespace System.IO.IsolatedStorage
