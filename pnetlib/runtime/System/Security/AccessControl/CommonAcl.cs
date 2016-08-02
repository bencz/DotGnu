/*
 * CommonAcl.cs - Implementation of the
 *			"System.Security.AccessControl.CommonAcl" class.
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

public abstract class CommonAcl : GenericAcl
{
	// Internal state.
	private RawAcl acl;
	private bool isContainer;
	private bool isDS;
	private bool wasCanonicalInitially;
	private byte revision;

	// Constructor.
	internal CommonAcl(RawAcl acl, bool isContainer, bool isDS,
					   bool wasCanonicalInitially, byte revision)
			{
				this.acl = acl;
				this.isContainer = isContainer;
				this.isDS = isDS;
				this.wasCanonicalInitially = wasCanonicalInitially;
				this.revision = revision;
			}

	// Get the binary form of this ACL.
	public override void GetBinaryForm(byte[] binaryForm, int offset)
			{
				acl.GetBinaryForm(binaryForm, offset);
			}

	// Purge access control elements for a particular principal.
	[TODO]
	public void Purge(SecurityIdentifier sid)
			{
				if(sid == null)
				{
					throw new ArgumentNullException("sid");
				}
				// TODO
			}

	// Remove access control elements that are inherited.
	[TODO]
	public void RemoveInheritedAces()
			{
				// TODO
			}

	// Get the length of the binary form.
	public override int BinaryLength
			{
				get
				{
					return acl.BinaryLength;
				}
			}

	// Get the number of items in this ACL.
	public override int Count
			{
				get
				{
					return acl.Count;
				}
			}

	// Get or set items in this ACL.
	public override GenericAce this[int index]
			{
				get
				{
					return acl[index].Copy();
				}
				set
				{
					throw new NotSupportedException(_("NotSupp_SetAce"));
				}
			}

	// Get this object's other properties.
	public bool IsContainer
			{
				get
				{
					return isContainer;
				}
			}
	public bool IsDS
			{
				get
				{
					return isDS;
				}
			}
	public byte Revision
			{
				get
				{
					return revision;
				}
			}
	public bool WasCanonicalInitially
			{
				get
				{
					return wasCanonicalInitially;
				}
			}

}; // class CommonAcl

#endif // CONFIG_ACCESS_CONTROL

}; // namespace System.Security.AccessControl
