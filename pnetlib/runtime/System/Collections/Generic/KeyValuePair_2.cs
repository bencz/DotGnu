/*
 * KeyValuePair_2.cs - Implementation of the
 *		"System.Collections.Generic.KeyValuePair<K, V>" class.
 *
 * Copyright (C) 2003, 2008  Southern Storm Software, Pty Ltd.
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

namespace System.Collections.Generic
{

#if CONFIG_FRAMEWORK_2_0

#if ECMA_COMPAT
public struct KeyValuePair<K,V>
#else
[Serializable]
public struct KeyValuePair<TKey,TValue>
#endif
{
	// Internal state.
#if ECMA_COMPAT
	public K Key;
	public V Value;

	// Constructor.
	public KeyValuePair(K key, V value)
			{
				this.Key = key;
				this.Value = value;
			}
#else // !ECMA_COMPAT
	private TKey key;
	private TValue value;

	// Constructor.
	public KeyValuePair(TKey key, TValue value)
			{
				this.key = key;
				this.value = value;
			}

	// methods
	[TODO]
	public override String ToString()
	{
		throw new NotImplementedException();
	}

	// properties
	public TKey Key
			{
				get
				{
					return key;
				}
			}

	public TValue Value
			{
				get
				{
					return this.value;
				}
			}
#endif // !ECMA_COMPAT

}; // struct KeyValuePair<K,V>

#endif // CONFIG_FRAMEWORK_2_0

}; // namespace System.Collections.Generic
