/*
 * TextElementEnumerator.cs - Implementation of the
 *        "System.Globalization.TextElementEnumerator" class.
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

namespace System.Globalization
{

#if !ECMA_COMPAT

using System;
using System.Collections;

[Serializable]
public class TextElementEnumerator : IEnumerator
{
	// Internal state.
	private String str;
	private int start;
	private int index;
	private int elementIndex;
	private String element;

	// Constructor.
	internal TextElementEnumerator(String str, int start)
			{
				this.str = str;
				this.start = start;
				this.index = start - 1;
				this.element = null;
			}

	// Implement the IEnumerator interface.
	public bool MoveNext()
			{
				if(++index < str.Length)
				{
					elementIndex = index;
					element = StringInfo.GetNextTextElement(str, index);
					index += element.Length - 1;
					return true;
				}
				else
				{
					element = null;
					return false;
				}
			}
	public void Reset()
			{
				index = start - 1;
				element = null;
			}
	public Object Current
			{
				get
				{
					if(element == null)
					{
						throw new InvalidOperationException
							(_("Invalid_BadEnumeratorPosition"));
					}
					return element;
				}
			}

	// Get the index of the current text element.
	public int ElementIndex
			{
				get
				{
					if(element == null)
					{
						throw new InvalidOperationException
							(_("Invalid_BadEnumeratorPosition"));
					}
					return elementIndex;
				}
			}

	// Get the current enumerator item as a string.
	public String GetTextElement()
			{
				if(element == null)
				{
					throw new InvalidOperationException
						(_("Invalid_BadEnumeratorPosition"));
				}
				return element;
			}

}; // class TextElementEnumerator

#endif // !ECMA_COMPAT

}; // namespace System.Globalization
