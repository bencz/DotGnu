/*
 * SynchronizedEnumerator.cs - Implementation of the
 *			"System.Private.SynchronizedEnumerator" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Private
{

using System;
using System.Collections;

// This is a helper class for wrapping up enumerators that
// have been obtained from syncrhonized collections so that
// the enumerator operations are also synchronized.  This
// ensures correct behaviour in symmetric multi-processing
// environments.

internal class SynchronizedEnumerator : IEnumerator
{
	// Internal state.
	protected Object      syncRoot;
	protected IEnumerator enumerator;

	// Constructor.
	public SynchronizedEnumerator(Object syncRoot, IEnumerator enumerator)
			{
				this.syncRoot = syncRoot;
				this.enumerator = enumerator;
			}

	// Implement the IEnumerator interface.
	public bool MoveNext()
			{
				lock(syncRoot)
				{
					return enumerator.MoveNext();
				}
			}
	public void Reset()
			{
				lock(syncRoot)
				{
					enumerator.Reset();
				}
			}
	public Object Current
			{
				get
				{
					lock(syncRoot)
					{
						return enumerator.Current;
					}
				}
			}

}; // class SynchronizedEnumerator

}; // namespace System.Private
