/*
 * SemaphoreAuditRule.cs - Implementation of the
 *			"System.Security.AccessControl.SemaphoreAuditRule" class.
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
using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
public sealed class SemaphoreAuditRule : AuditRule
{
	// Constructor.
	public SemaphoreAuditRule
				(IdentityReference identity, SemaphoreRights semaphoreRights,
				 AuditFlags auditFlags)
			: base(identity, (int)semaphoreRights, false,
				   InheritanceFlags.None, PropagationFlags.None, auditFlags) {}

	// Get this object's properties.
	public SemaphoreRights SemaphoreRights
			{
				get
				{
					return (SemaphoreRights)AccessMask;
				}
			}

}; // class SemaphoreAuditRule

#endif // CONFIG_ACCESS_CONTROL

}; // namespace System.Security.AccessControl
