/*
 * XmlCharInfo.cs - Implementation of the
 *		"System.Xml.XmlCharInfo.Private" class.
 *
 * Copyright (C) 2004  Free Software Foundation, Inc.
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

namespace System.Xml.Private
{

using System;
using System.Globalization;

internal sealed class XmlCharInfo
{
	private XmlCharInfo() {}


	// Returns true iff the given character is a valid XML character.
	public static bool IsChar(char c)
			{
				return c == '\t' ||
			    	   c == '\n' ||
				       c == '\r' ||
				       (c >= ' ' &&
				        (Char.GetUnicodeCategory(c) != UnicodeCategory.Surrogate));
			}

	// Returns true iff the given character is a valid name character.
	public static bool IsNameChar(char c)
			{
				if(c == '.' || c == '-' || c == '_' || c == ':')
				{
					return true;
				}
				switch(Char.GetUnicodeCategory(c))
				{
					case UnicodeCategory.UppercaseLetter:
					case UnicodeCategory.LowercaseLetter:
					case UnicodeCategory.TitlecaseLetter:
					case UnicodeCategory.OtherLetter:
					case UnicodeCategory.LetterNumber:
					case UnicodeCategory.SpacingCombiningMark:
					case UnicodeCategory.EnclosingMark:
					case UnicodeCategory.NonSpacingMark:
					case UnicodeCategory.ModifierLetter:
					case UnicodeCategory.DecimalDigitNumber:
						return true;
					default:
						return false;
				}
			}

	// Returns true iff the given character is a valid name start character.
	public static bool IsNameInit(char c)
			{
				if(c == '_' || c == ':')
				{
					return true;
				}
				switch(Char.GetUnicodeCategory(c))
				{
					case UnicodeCategory.UppercaseLetter:
					case UnicodeCategory.LowercaseLetter:
					case UnicodeCategory.TitlecaseLetter:
					case UnicodeCategory.OtherLetter:
					case UnicodeCategory.LetterNumber:
						return true;
					default:
						return false;
				}
			}

	// Returns true iff the given character is a valid public id character.
	public static bool IsPublicId(char c)
			{
				return (c != '"' && c != '&' && c != '<' && c != '>') &&
				       (c == '\n' || c == '\r' || c == '_' ||
				        (c >= ' ' && c <= 'Z') || (c >= 'a' && c <= 'z'));
			}

	// Returns true iff the given character is a valid whitespace character.
	public static bool IsWhitespace(char c)
			{
				return (c == '\t' || c == '\n' || c == '\r' || c == ' ');
			}

}; // class XmlCharInfo

}; // namespace System.Xml.Private
