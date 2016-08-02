/*
 * CharEnumerator.cs - Implementation of the "System.CharEnumerator" class.
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

namespace System
{

using System.Collections;
#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0
using System.Collections.Generic;
#endif

public sealed class CharEnumerator : IEnumerator, ICloneable
#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0
	, IEnumerator<char>
#endif
{
	// Internal state for the enumerator.
	String str;
	int    index;
	int    length;

	// Construct a new enumerator.  Called from the "String" class.
	internal CharEnumerator(String str)
			{
				this.str = str;
				index = -1;
				length = str.Length;
			}

	// Implement the ICloneable interface.
	public Object Clone()
			{
				return MemberwiseClone();
			}

	// Implement the IEnumerator interface.
	public bool MoveNext()
			{
				++index;
				return (index < length);
			}
	public void Reset()
			{
				index = -1;
			}
	Object IEnumerator.Current
			{
				get
				{
					if(index >= 0 && index < length)
					{
						return (Object)(str[index]);
					}
					else
					{
						throw new InvalidOperationException
							(_("Invalid_BadEnumeratorPosition"));
					}
				}
			}

	// Get the current element.
	public char Current
			{
				get
				{
					if(index >= 0 && index < length)
					{
						return str[index];
					}
					else
					{
						throw new InvalidOperationException
							(_("Invalid_BadEnumeratorPosition"));
					}
				}
			}

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0

	void IDisposable.Dispose()
			{
				// Nothing to do here
			}

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0

}; // class CharEnumerator

}; // namespace System
