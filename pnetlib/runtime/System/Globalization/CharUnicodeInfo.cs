/*
 * CharUnicodeInfo.cs - Implementation of the
 *        "System.Globalization.CharUnicodeInfo" class.
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

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0

using Platform;

public sealed class CharUnicodeInfo
{
	// Cannot instantiate this class.
	private CharUnicodeInfo() {}

	// Get the decimal digit value of a character.
	public static int GetDecimalDigitValue(char ch)
			{
				if(ch >= '0' && ch <= '9')
				{
					return (int)(ch - '0');
				}
				else
				{
					double value = SysCharInfo.GetNumericValue(ch);
					int ivalue = (int)value;
					if(((double)ivalue) == value)
					{
						if(ivalue >= 0 && ivalue <= 9)
						{
							return ivalue;
						}
					}
					return -1;
				}
			}
	public static int GetDecimalDigitValue(String s, int index)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				if(index < 0 || index >= s.Length)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_StringIndex"));
				}
				return GetDecimalDigitValue(s[index]);
			}

	// Get the digit value of a character.
	public static int GetDigitValue(char ch)
			{
				if(ch >= '0' && ch <= '9')
				{
					return (int)(ch - '0');
				}
				else
				{
					double value = SysCharInfo.GetNumericValue(ch);
					int ivalue = (int)value;
					if(((double)ivalue) == value)
					{
						return ivalue;
					}
					return -1;
				}
			}
	public static int GetDigitValue(String s, int index)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				if(index < 0 || index >= s.Length)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_StringIndex"));
				}
				return GetDigitValue(s[index]);
			}

	// Get the numeric value of a character.
	public static double GetNumericValue(char ch)
			{
				if(ch >= '0' && ch <= '9')
				{
					return (double)(int)(ch - '0');
				}
				else
				{
					return SysCharInfo.GetNumericValue(ch);
				}
			}
	public static double GetNumericValue(String s, int index)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				if(index < 0 || index >= s.Length)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_StringIndex"));
				}
				return GetNumericValue(s[index]);
			}

	// Get the unicode category of a character.
	public static UnicodeCategory GetUnicodeCategory(char ch)
			{
				return SysCharInfo.GetUnicodeCategory(ch);
			}
	public static UnicodeCategory GetUnicodeCategory(String s, int index)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				if(index < 0 || index >= s.Length)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_StringIndex"));
				}
				return GetUnicodeCategory(s[index]);
			}

}; // class CharUnicodeInfo

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0

}; // namespace System.Globalization
