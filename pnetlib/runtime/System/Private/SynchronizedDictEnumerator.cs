/*
 * SynchronizedDictEnumerator.cs - Implementation of the
 *			"System.Private.SynchronizedDictEnumerator" class.
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
// have been obtained from syncrhonized dictionaries so that
// the enumerator operations are also synchronized.  This
// ensures correct behaviour in symmetric multi-processing
// environments.

internal class SynchronizedDictEnumerator : SynchronizedEnumerator,
										    IDictionaryEnumerator
{
	// Constructor.
	public SynchronizedDictEnumerator(Object syncRoot, IEnumerator enumerator)
			: base(syncRoot, enumerator)
			{
				// Nothing to do here
			}

	// Implement the IDictionaryEnumerator interface.
	public DictionaryEntry Entry
			{
				get
				{
					lock(syncRoot)
					{
						return ((IDictionaryEnumerator)enumerator).Entry;
					}
				}
			}
	public Object Key
			{
				get
				{
					lock(syncRoot)
					{
						return ((IDictionaryEnumerator)enumerator).Key;
					}
				}
			}
	public Object Value
			{
				get
				{
					lock(syncRoot)
					{
						return ((IDictionaryEnumerator)enumerator).Value;
					}
				}
			}

}; // class SynchronizedDictEnumerator

}; // namespace System.Private
