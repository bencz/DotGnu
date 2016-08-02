/*
 * ObjectAccessRule.cs - Implementation of the
 *			"System.Security.AccessControl.ObjectAccessRule" class.
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

#if CONFIG_ACCESS_CONTROL && !ECMA_COMPAT

using System.Security.Principal;

public abstract class ObjectAccessRule : AccessRule
{
	// Internal state.
	private Guid objectType;
	private Guid inheritedObjectType;

	// Constructor.
	protected ObjectAccessRule
				(IdentityReference identity, int accessMask,
				 bool isInherited, InheritanceFlags inheritanceFlags,
				 PropagationFlags propagationFlags, Guid objectType,
				 Guid inheritedObjectType, AccessControlType type)
			: base(identity, accessMask, isInherited,
				   inheritanceFlags, propagationFlags, type)
			{
				this.objectType = objectType;
				this.inheritedObjectType = inheritedObjectType;
			}

	// Get this object's properties.
	public Guid InheritedObjectType
			{
				get
				{
					return inheritedObjectType;
				}
			}
	public ObjectAceFlags ObjectFlags
			{
				get
				{
					return (ObjectAceFlags)AccessMask;
				}
			}
	public Guid ObjectType
			{
				get
				{
					return objectType;
				}
			}

}; // class ObjectAccessRule

#endif // CONFIG_ACCESS_CONTROL && !ECMA_COMPAT

}; // namespace System.Security.AccessControl
