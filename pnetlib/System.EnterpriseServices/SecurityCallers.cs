/*
 * SecurityCallers.cs - Implementation of the
 *			"System.EnterpriseServices.SecurityCallers" class.
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

public sealed class SecurityCallers : IEnumerable
{
	// Internal state.
	private ArrayList list;

	// Constructor.
	internal SecurityCallers()
			{
				list = new ArrayList();
			}

	// Get the number of callers in the chain.
	public int Count
			{
				get
				{
					return list.Count;
				}
			}

	// Get a particular item within the call chain.
	public SecurityIdentity this[int idx]
			{
				get
				{
					return (SecurityIdentity)(list[idx]);
				}
			}

	// Enumerate over all callers in the chain.
	public IEnumerator GetEnumerator()
			{
				return list.GetEnumerator();
			}

	// Push a caller onto the chain.
	internal void PushCaller(SecurityIdentity caller)
			{
				list.Insert(0, caller);
			}

	// Pop the top-most caller from the chain.  Cannot pop the original value.
	internal void PopCaller()
			{
				if(list.Count > 1)
				{
					list.RemoveAt(0);
				}
			}

}; // class SecurityCallers

}; // namespace System.EnterpriseServices
