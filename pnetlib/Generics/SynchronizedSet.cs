/*
 * SynchronizedSet.cs - Wrap a set to make it synchronized.
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

namespace Generics
{

using System;

public class SynchronizedSet<T> : SynchronizedCollection<T>, ISet<T>
{
	// Internal state.
	protected ISet<T> set;

	// Constructor.
	public SynchronizedSet(ISet<T> set) : base(set)
			{
				this.set = set;
			}

	// Implement the ISet<T> interface.
	public void Add(T value)
			{
				lock(SyncRoot)
				{
					set.Add(value);
				}
			}
	public void Clear()
			{
				lock(SyncRoot)
				{
					set.Clear();
				}
			}
	public bool Contains(T value)
			{
				lock(SyncRoot)
				{
					return set.Contains(value);
				}
			}
	public void Remove(T value)
			{
				lock(SyncRoot)
				{
					set.Remove(value);
				}
			}

	// Implement the ICloneable interface.
	public override Object Clone()
			{
				lock(SyncRoot)
				{
					if(set is ICloneable)
					{
						return new SynchronizedSet<T>
							((ISet<T>)(((ICloneable)set).Clone()));
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_NotCloneable"));
					}
				}
			}

}; // class SynchronizedSet<T>

}; // namespace Generics
