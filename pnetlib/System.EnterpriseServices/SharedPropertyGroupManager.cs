/*
 * SharedPropertyGroupManager.cs - Implementation of the
 *			"System.EnterpriseServices.SharedPropertyGroupManager" class.
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

namespace System.EnterpriseServices
{

using System.Collections;
using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
public sealed class SharedPropertyGroupManager : IEnumerable
{
	// Internal state.
	private Hashtable table;

	// Constructor.
	public SharedPropertyGroupManager()
			{
				table = new Hashtable();
			}

	// Find or create a shared property group.
	public SharedPropertyGroup CreatePropertyGroup
				(String name, ref PropertyLockMode dwIsoMode,
		     	 ref PropertyReleaseMode dwRelMode, out bool fExist)
			{
				SharedPropertyGroup group;
				group = (SharedPropertyGroup)(table[name]);
				if(group != null)
				{
					fExist = true;
					return group;
				}
				group = new SharedPropertyGroup();
				table[name] = group;
				fExist = false;
				return group;
			}

	// Get an enumerator for the group manager.
	public IEnumerator GetEnumerator()
			{
				return new GroupEnumerator(table.GetEnumerator());
			}

	// Find the group with a specific name.
	public SharedPropertyGroup Group(String name)
			{
				return (table[name] as SharedPropertyGroup);
			}

	// Enumerator for this class.
	private sealed class GroupEnumerator : IEnumerator
	{
		// Internal state.
		private IDictionaryEnumerator e;

		// Constructor.
		public GroupEnumerator(IDictionaryEnumerator e)
				{
					this.e = e;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					return e.MoveNext();
				}
		public void Reset()
				{
					e.Reset();
				}
		public Object Current
				{
					get
					{
						return e.Value;
					}
				}

	}; // class GroupEnumerator

}; // class SharedPropertyGroupManager

}; // namespace System.EnterpriseServices
