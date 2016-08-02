/*
 * AceEnumerator.cs - Implementation of the
 *			"System.Security.AccessControl.AceEnumerator" class.
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

public sealed class AceEnumerator : IEnumerator
{
	// Internal state.
	private GenericAcl acl;
	private int index;

	// Constructor.
	internal AceEnumerator(GenericAcl acl)
			{
				this.acl = acl;
				this.index = -1;
			}

	// Implement the IEnumerator interface.
	public bool MoveNext()
			{
				++index;
				return (index < acl.Count);
			}
	public void Reset()
			{
				index = -1;
			}
	Object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

	// Get the current item.
	public GenericAce Current
			{
				get
				{
					if(index < 0 || index >= acl.Count)
					{
						throw new InvalidOperationException
							(_("Invalid_BadEnumeratorPosition"));
					}
					return acl[index];
				}
			}

}; // class AceEnumerator

#endif // CONFIG_ACCESS_CONTROL

}; // namespace System.Security.AccessControl
