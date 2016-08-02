/*
 * DiscretionaryAcl.cs - Implementation of the
 *			"System.Security.AccessControl.DiscretionaryAcl" class.
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

using System.Security.Principal;

public sealed class DiscretionaryAcl : CommonAcl
{
	// Constructors.
	public DiscretionaryAcl(bool isContainer, bool isDS,
							byte revision, int capacity)
			: base(new RawAcl(revision, capacity),
				   isContainer, isDS, false, revision) {}
	public DiscretionaryAcl(bool isContainer, bool isDS, RawAcl rawAcl)
			: base(rawAcl, isContainer, isDS, false, rawAcl.Revision) {}
	public DiscretionaryAcl(bool isContainer, bool isDS, int capacity)
			: base(new RawAcl
					((isDS ? GenericAcl.AclRevisionDS
					 	   : GenericAcl.AclRevision), capacity),
				     isContainer, isDS, false,
					 (isDS ? GenericAcl.AclRevisionDS
					 	   : GenericAcl.AclRevision)) {}

	// Add an access control element to this ACL.
#if !ECMA_COMPAT
	[TODO]
	public void AddAccess(AccessControlType accessType, SecurityIdentifier sid,
						  int accessMask, InheritanceFlags inheritanceFlags,
						  PropagationFlags propagationFlags,
						  ObjectAceFlags objectFlags, Guid objectType,
						  Guid inheritedObjectType)
			{
				// TODO
			}
#endif
	[TODO]
	public void AddAccess(AccessControlType accessType, SecurityIdentifier sid,
						  int accessMask, InheritanceFlags inheritanceFlags,
						  PropagationFlags propagationFlags)
			{
				// TODO
			}

	// Remove an access control element from this ACL.
#if !ECMA_COMPAT
	[TODO]
	public bool RemoveAccess
					(AccessControlType accessType, SecurityIdentifier sid,
				     int accessMask, InheritanceFlags inheritanceFlags,
				     PropagationFlags propagationFlags,
				     ObjectAceFlags objectFlags, Guid objectType,
				     Guid inheritedObjectType)
			{
				// TODO
				return false;
			}
	[TODO]
	public void RemoveAccessSpecific
					(AccessControlType accessType, SecurityIdentifier sid,
					 int accessMask, InheritanceFlags inheritanceFlags,
				     PropagationFlags propagationFlags,
				     ObjectAceFlags objectFlags, Guid objectType,
				     Guid inheritedObjectType)
			{
				// TODO
			}
#endif
	[TODO]
	public bool RemoveAccess
					(AccessControlType accessType, SecurityIdentifier sid,
					 int accessMask, InheritanceFlags inheritanceFlags,
					 PropagationFlags propagationFlags)
			{
				// TODO
				return false;
			}
	[TODO]
	public void RemoveAccessSpecific
					(AccessControlType accessType, SecurityIdentifier sid,
					 int accessMask, InheritanceFlags inheritanceFlags,
					 PropagationFlags propagationFlags)
			{
				// TODO
			}

	// Set an access control element in this ACL.
#if !ECMA_COMPAT
	[TODO]
	public void SetAccess(AccessControlType accessType, SecurityIdentifier sid,
						  int accessMask, InheritanceFlags inheritanceFlags,
						  PropagationFlags propagationFlags,
						  ObjectAceFlags objectFlags, Guid objectType,
						  Guid inheritedObjectType)
			{
				// TODO
			}
#endif
	[TODO]
	public void SetAccess(AccessControlType accessType, SecurityIdentifier sid,
						  int accessMask, InheritanceFlags inheritanceFlags,
						  PropagationFlags propagationFlags)
			{
				// TODO
			}

}; // class DiscretionaryAcl

#endif // CONFIG_ACCESS_CONTROL

}; // namespace System.Security.AccessControl
