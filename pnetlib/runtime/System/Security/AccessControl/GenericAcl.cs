/*
 * GenericAcl.cs - Implementation of the
 *			"System.Security.AccessControl.GenericAcl" class.
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

public abstract class GenericAcl : ICollection, IEnumerable
{
	// Public version and length values.
	public static readonly byte AclRevision = 2;
	public static readonly byte AclRevisionDS = 4;
	public static readonly int MaxBinaryLength = 65535;

	// Constructor.
	protected GenericAcl() {}

	// Implement the ICollection interface.
	void ICollection.CopyTo(Array array, int index)
			{
				if(array == null)
				{
					throw new ArgumentNullException("array");
				}
				int count = Count;
				int posn;
				for(posn = 0; posn < count; ++posn)
				{
					array.SetValue(this[posn], index + posn);
				}
			}
	public abstract int Count { get; }
	public bool IsSynchronized
			{
				get
				{
					return false;
				}
			}
	public Object SyncRoot
			{
				get
				{
					return null;
				}
			}

	// Implement the IEnumerable interface.
	IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

	// Copy the contents of this ACL to an array.
	public void CopyTo(GenericAce[] array, int index)
			{
				((ICollection)this).CopyTo(array, index);
			}

	// Get the binary form of this ACL.
	public abstract void GetBinaryForm(byte[] binaryForm, int offset);

	// Get an enumerator for this ACL.
	public AceEnumerator GetEnumerator()
			{
				return new AceEnumerator(this);
			}

	// Get the length of the binary form.
	public abstract int BinaryLength { get; }

	// Get or set items in this ACL.
	public abstract GenericAce this[int index] { get; set; }

}; // class GenericAcl

#endif // CONFIG_ACCESS_CONTROL

}; // namespace System.Security.AccessControl
