/*
 * RawAcl.cs - Implementation of the
 *			"System.Security.AccessControl.RawAcl" class.
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

using System.Collections;

public sealed class RawAcl : GenericAcl
{
	// Internal state.
	private byte revision;
	private ArrayList elements;

	// Constructors.
	public RawAcl(byte[] binaryForm, int offset)
			: this(0, 16)
			{
				// TODO - read from the binary form.
			}
	public RawAcl(byte revision, int capacity)
			{
				this.revision = revision;
				this.elements = new ArrayList();
			}

	// Implement the ICollection interface.
	public override int Count
			{
				get
				{
					return elements.Count;
				}
			}

	// Get the binary form of this ACL.
	[TODO]
	public override void GetBinaryForm(byte[] binaryForm, int offset)
			{
				// TODO
			}

	// Insert an access control element into this ACL.
	public void InsertAce(int index, GenericAce ace)
			{
				if(ace == null)
				{
					throw new ArgumentNullException("ace");
				}
				elements.Insert(index, ace);
			}

	// Remove an access control element from this ACL.
	public void RemoveAce(int index)
			{
				elements.RemoveAt(index);
			}

	// Get the length of the binary form.
	[TODO]
	public override int BinaryLength
			{
				get
				{
					// TODO
					return 0;
				}
			}

	// Get or set items in this ACL.
	public override GenericAce this[int index]
			{
				get
				{
					return (GenericAce)(elements[index]);
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					elements[index] = value;
				}
			}

	// Get this ACL's revision.
	public byte Revision
			{
				get
				{
					return revision;
				}
			}

}; // class RawAcl

#endif // CONFIG_ACCESS_CONTROL

}; // namespace System.Security.AccessControl
