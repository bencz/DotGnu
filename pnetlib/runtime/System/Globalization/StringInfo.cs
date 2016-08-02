/*
 * StringInfo.cs - Implementation of the
 *        "System.Globalization.StringInfo" class.
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
public class StringInfo
{
	// Constructor.
	public StringInfo() {}

	// Get the text element at a specific location within a string.
	public static String GetNextTextElement(String str)
			{
				return GetNextTextElement(str, 0);
			}
	public static String GetNextTextElement(String str, int index)
			{
				if(str == null)
				{
					throw new ArgumentNullException("str");
				}
				if(index < 0 || index >= str.Length)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_StringIndex"));
				}
				char ch = str[index];
				UnicodeCategory category = Char.GetUnicodeCategory(ch);
				if(category == UnicodeCategory.Surrogate &&
				   ch >= 0xD800 && ch <= 0xDBFF)
				{
					if((index + 1) < str.Length &&
					   str[index + 1] >= 0xDC00 && str[index + 1] <= 0xDFFF)
					{
						// Surrogate character pair.
						return str.Substring(index, 2);
					}
					else
					{
						// High surrogate on its own.
						return new String(ch, 1);
					}
				}
				else if(category == UnicodeCategory.NonSpacingMark ||
						category == UnicodeCategory.SpacingCombiningMark ||
						category == UnicodeCategory.EnclosingMark)
				{
					// A sequence of combining marks.
					int start = index;
					++index;
					while(index < str.Length)
					{
						ch = str[index];
						if(category != UnicodeCategory.NonSpacingMark &&
						   category != UnicodeCategory.SpacingCombiningMark &&
						   category != UnicodeCategory.EnclosingMark)
						{
							break;
						}
						++index;
					}
					return str.Substring(start, index - start);
				}
				else
				{
					// Ordinary base character.
					return new String(ch, 1);
				}
			}

	// Enumerate through the text elements in a string.
	public static TextElementEnumerator GetTextElementEnumerator(String str)
			{
				return GetTextElementEnumerator(str);
			}
	public static TextElementEnumerator GetTextElementEnumerator
				(String str, int index)
			{
				if(str == null)
				{
					throw new ArgumentNullException("str");
				}
				if(index < 0 || index >= str.Length)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_StringIndex"));
				}
				return new TextElementEnumerator(str, index);
			}

	// Parse the locations of combining characters in a string.
	public static int[] ParseCombiningCharacters(String str)
			{
				if(str == null)
				{
					throw new ArgumentNullException("str");
				}
				else if(str == String.Empty)
				{
					return null;
				}
				ArrayList list = new ArrayList(str.Length);
				int index = 0;
				char ch;
				UnicodeCategory category;
				while(index < str.Length)
				{
					list.Add(index);
					ch = str[index];
					category = Char.GetUnicodeCategory(ch);
					if(category == UnicodeCategory.Surrogate &&
					   ch >= 0xD800 && ch <= 0xDBFF)
					{
						if((index + 1) < str.Length &&
						   str[index + 1] >= 0xDC00 &&
						   str[index + 1] <= 0xDFFF)
						{
							// Surrogate character pair.
							index += 2;
						}
						else
						{
							// High surrogate on its own.
							++index;
						}
					}
					else if(category == UnicodeCategory.NonSpacingMark ||
							category == UnicodeCategory.SpacingCombiningMark ||
							category == UnicodeCategory.EnclosingMark)
					{
						// A sequence of combining marks.
						++index;
						while(index < str.Length)
						{
							ch = str[index];
							if(category != UnicodeCategory.NonSpacingMark &&
							   category !=
							   		UnicodeCategory.SpacingCombiningMark &&
							   category != UnicodeCategory.EnclosingMark)
							{
								break;
							}
							++index;
						}
					}
					else
					{
						// Ordinary base character.
						++index;
					}
				}
				return (int[])(list.ToArray(typeof(int)));
			}

}; // class StringInfo

#endif // !ECMA_COMPAT

}; // namespace System.Globalization
