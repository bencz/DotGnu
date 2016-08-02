/*
 * Char.cs - Implementation of the "System.Char" class.
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

using System.Globalization;

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0
using System.Runtime.InteropServices;

[ComVisible(true)]
[Serializable]
#endif
public struct Char : IComparable
#if !ECMA_COMPAT
	, IConvertible
#endif
#if CONFIG_FRAMEWORK_2_0
	, IComparable<char>, IEquatable<char>
#endif
{
	private char value_;

	public const char MaxValue = '\uFFFF';
	public const char MinValue = '\u0000';

	// Override inherited methods.
	public override int GetHashCode()
			{ return unchecked(((int)value_) | (((int)value_) << 16)); }
	public override bool Equals(Object value)
			{
				if(value is Char)
				{
					return (value_ == ((Char)value).value_);
				}
				else
				{
					return false;
				}
			}

	// String conversion.
	public override String ToString()
			{
				return new String (value_, 1);
			}
	public String ToString(IFormatProvider provider)
			{
				return new String (value_, 1);
			}
#if !ECMA_COMPAT
	public static String ToString(char value)
			{
				return new String(value, 1);
			}
#endif

	// Parsing methods.
	public static char Parse(String s)
			{
				if(s != null)
				{
					if(s.Length == 1)
					{
						return s[0];
					}
					else
					{
						throw new FormatException(_("Format_NeedSingleChar"));
					}
				}
				else
				{
					throw new ArgumentNullException("s");
				}
			}

	// Implementation of the IComparable interface.
	public int CompareTo(Object value)
			{
				if(value != null)
				{
					if(!(value is Char))
					{
						throw new ArgumentException(_("Arg_MustBeChar"));
					}
					return ((int)value_) - ((int)(((Char)value).value_));
				}
				else
				{
					return 1;
				}
			}

#if CONFIG_FRAMEWORK_2_0

	// Implementation of the IComparable<char> interface.
	public int CompareTo(char value)
			{
				return ((int)value_) - ((int)(value.value_));
			}

	// Implementation of the IEquatable<char> interface.
	public bool Equals(char obj)
			{
				return (value_ == obj.value_);
			}

#endif // CONFIG_FRAMEWORK_2_0

#if !ECMA_COMPAT

	// Implementation of IConvertible interface.
	public TypeCode GetTypeCode()
			{
				return TypeCode.Char;
			}
	bool IConvertible.ToBoolean(IFormatProvider provider)
			{
				throw new InvalidCastException
					(String.Format
						(_("InvalidCast_FromTo"), "Char", "Boolean"));
			}
	byte IConvertible.ToByte(IFormatProvider provider)
			{
				return Convert.ToByte(value_);
			}
	sbyte IConvertible.ToSByte(IFormatProvider provider)
			{
				return Convert.ToSByte(value_);
			}
	short IConvertible.ToInt16(IFormatProvider provider)
			{
				return Convert.ToInt16(value_);
			}
	ushort IConvertible.ToUInt16(IFormatProvider provider)
			{
				return Convert.ToUInt16(value_);
			}
	char IConvertible.ToChar(IFormatProvider provider)
			{
				return value_;
			}
	int IConvertible.ToInt32(IFormatProvider provider)
			{
				return Convert.ToInt32(value_);
			}
	uint IConvertible.ToUInt32(IFormatProvider provider)
			{
				return Convert.ToUInt32(value_);
			}
	long IConvertible.ToInt64(IFormatProvider provider)
			{
				return Convert.ToInt64(value_);
			}
	ulong IConvertible.ToUInt64(IFormatProvider provider)
			{
				return Convert.ToUInt64(value_);
			}
	float IConvertible.ToSingle(IFormatProvider provider)
			{
				throw new InvalidCastException
					(String.Format
						(_("InvalidCast_FromTo"), "Char", "Single"));
			}
	double IConvertible.ToDouble(IFormatProvider provider)
			{
				throw new InvalidCastException
					(String.Format
						(_("InvalidCast_FromTo"), "Char", "Double"));
			}
	Decimal IConvertible.ToDecimal(IFormatProvider provider)
			{
				throw new InvalidCastException
					(String.Format
						(_("InvalidCast_FromTo"), "Char", "Decimal"));
			}
	DateTime IConvertible.ToDateTime(IFormatProvider provider)
			{
				throw new InvalidCastException
					(String.Format
						(_("InvalidCast_FromTo"), "Char", "DateTime"));
			}
	Object IConvertible.ToType(Type conversionType, IFormatProvider provider)
			{
				return Convert.DefaultToType(this, conversionType,
											 provider, true);
			}

#endif // !ECMA_COMPAT

#if CONFIG_EXTENDED_NUMERICS
	// Get the numeric value associated with a character.
	public static double GetNumericValue(char c)
			{
				if(c >= '0' && c <= '9')
				{
					return (double)(int)(c - '0');
				}
				else
				{
					return Platform.SysCharInfo.GetNumericValue(c);
				}
			}
	public static double GetNumericValue(String s, int index)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				return GetNumericValue(s[index]);
			}
#endif // CONFIG_EXTENDED_NUMERICS

	// Get the Unicode category for a character.
	public static UnicodeCategory GetUnicodeCategory(char c)
			{
				return Platform.SysCharInfo.GetUnicodeCategory(c);
			}
	public static UnicodeCategory GetUnicodeCategory(String s, int index)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				return Platform.SysCharInfo.GetUnicodeCategory(s[index]);
			}

	// Category testing.
	public static bool IsControl(char c)
			{
				return (GetUnicodeCategory(c) == UnicodeCategory.Control);
			}
	public static bool IsControl(String s, int index)
			{
				return (GetUnicodeCategory(s, index) ==
							UnicodeCategory.Control);
			}
	public static bool IsDigit(char c)
			{
				return (GetUnicodeCategory(c) ==
							UnicodeCategory.DecimalDigitNumber);
			}
	public static bool IsDigit(String s, int index)
			{
				return (GetUnicodeCategory(s, index) ==
							UnicodeCategory.DecimalDigitNumber);
			}
	public static bool IsLetter(char c)
			{
				UnicodeCategory category = GetUnicodeCategory(c);

				switch(category)
				{
					case UnicodeCategory.UppercaseLetter:
					case UnicodeCategory.LowercaseLetter:
					case UnicodeCategory.TitlecaseLetter:
					case UnicodeCategory.ModifierLetter:
					case UnicodeCategory.OtherLetter:
					{
						return true;
					}

					default:
					{
						return false;
					}
				}
			}
	public static bool IsLetter(String s, int index)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				return IsLetter(s[index]);
			}
	public static bool IsLetterOrDigit(char c)
			{
				UnicodeCategory category = GetUnicodeCategory(c);

				switch(category)
				{
					case UnicodeCategory.UppercaseLetter:
					case UnicodeCategory.LowercaseLetter:
					case UnicodeCategory.TitlecaseLetter:
					case UnicodeCategory.ModifierLetter:
					case UnicodeCategory.OtherLetter:
					case UnicodeCategory.DecimalDigitNumber:
					{
						return true;
					}

					default:
					{
						return false;
					}
				}
			}
	public static bool IsLetterOrDigit(String s, int index)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				return IsLetterOrDigit(s[index]);
			}
	public static bool IsLower(char c)
			{
				return (GetUnicodeCategory(c) ==
							UnicodeCategory.LowercaseLetter);
			}
	public static bool IsLower(String s, int index)
			{
				return (GetUnicodeCategory(s, index) ==
							UnicodeCategory.LowercaseLetter);
			}
	public static bool IsNumber(char c)
			{
				UnicodeCategory category = GetUnicodeCategory(c);

				switch(category)
				{
					case UnicodeCategory.DecimalDigitNumber:
					case UnicodeCategory.LetterNumber:
					case UnicodeCategory.OtherNumber:
					{
						return true;
					}

					default:
					{
						return false;
					}
				}
			}
	public static bool IsNumber(String s, int index)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				return IsNumber(s[index]);
			}
	public static bool IsPunctuation(char c)
			{
				UnicodeCategory category = GetUnicodeCategory(c);

				switch(category)
				{
					case UnicodeCategory.ConnectorPunctuation:
					case UnicodeCategory.DashPunctuation:
					case UnicodeCategory.OpenPunctuation:
					case UnicodeCategory.ClosePunctuation:
					case UnicodeCategory.InitialQuotePunctuation:
					case UnicodeCategory.FinalQuotePunctuation:
					case UnicodeCategory.OtherPunctuation:
					{
						return true;
					}

					default:
					{
						return false;
					}
				}
			}
	public static bool IsPunctuation(String s, int index)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				return IsPunctuation(s[index]);
			}
	public static bool IsSeparator(char c)
			{
				UnicodeCategory category = GetUnicodeCategory(c);

				switch(category)
				{
					case UnicodeCategory.SpaceSeparator:
					case UnicodeCategory.LineSeparator:
					case UnicodeCategory.ParagraphSeparator:
					{
						return true;
					}

					default:
					{
						return false;
					}
				}
			}
	public static bool IsSeparator(String s, int index)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				return IsSeparator(s[index]);
			}
	public static bool IsSurrogate(char c)
			{
				return (GetUnicodeCategory(c) ==
							UnicodeCategory.Surrogate);
			}
	public static bool IsSurrogate(String s, int index)
			{
				return (GetUnicodeCategory(s, index) ==
							UnicodeCategory.Surrogate);
			}
	public static bool IsSymbol(char c)
			{
				UnicodeCategory category = GetUnicodeCategory(c);

				switch(category)
				{
					case UnicodeCategory.MathSymbol:
					case UnicodeCategory.CurrencySymbol:
					case UnicodeCategory.ModifierSymbol:
					case UnicodeCategory.OtherSymbol:
					{
						return true;
					}

					default:
					{
						return false;
					}
				}
			}
	public static bool IsSymbol(String s, int index)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				return IsSymbol(s[index]);
			}
	public static bool IsUpper(char c)
			{
				return (GetUnicodeCategory(c) ==
							UnicodeCategory.UppercaseLetter);
			}
	public static bool IsUpper(String s, int index)
			{
				return (GetUnicodeCategory(s, index) ==
							UnicodeCategory.UppercaseLetter);
			}
	public static bool IsWhiteSpace(char c)
			{
				if(c == '\u0009' || c == '\u000a' || c == '\u000b' ||
				   c == '\u000c' || c == '\u000d' || c == '\u0085' ||
				   c == '\u2028' || c == '\u2029')
				{
					return true;
				}
				return (GetUnicodeCategory(c) ==
							UnicodeCategory.SpaceSeparator);
			}
	public static bool IsWhiteSpace(String s, int index)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				return IsWhiteSpace(s[index]);
			}

	// Case conversion.
	public static char ToLower(char c)
			{
				return CultureInfo.CurrentCulture.TextInfo.ToLower(c);
			}
	public static char ToUpper(char c)
			{
				return CultureInfo.CurrentCulture.TextInfo.ToUpper(c);
			}
#if !ECMA_COMPAT
	public static char ToLower(char c, CultureInfo culture)
			{
				if(culture != null)
				{
					return culture.TextInfo.ToLower(c);
				}
				else
				{
					throw new ArgumentNullException("culture");
				}
			}
	public static char ToUpper(char c, CultureInfo culture)
			{
				if(culture != null)
				{
					return culture.TextInfo.ToUpper(c);
				}
				else
				{
					throw new ArgumentNullException("culture");
				}
			}

#if CONFIG_FRAMEWORK_2_0
	public static bool IsHighSurrogate(char c)
			{
				return ((c >= 0xd800) && (c <= 0xdbff));
			}

	public static bool IsHighSurrogate(string s, int index)
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
				return IsHighSurrogate(s[index]);
			}

	public static bool IsLowSurrogate(char c)
			{
				return ((c >= 0xdc00) && (c <= 0xdfff));
			}

	public static bool IsLowSurrogate(string s, int index)
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
				return IsLowSurrogate(s[index]);
			}

	public static bool IsSurrogatePair(char highSurrogate, char lowSurrogate)
			{
				return (IsHighSurrogate(highSurrogate) &&
						IsLowSurrogate(lowSurrogate));
			}

	public static bool IsSurrogatePair(string s, int index)
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
				if(IsHighSurrogate(s[index]))
				{
					if(index + 1 < s.Length)
					{
						return IsLowSurrogate(s[index + 1]);
					}
				}
				return false;
			}

	public static char ToLowerInvariant(char c)
			{
				return ToLower(c, CultureInfo.InvariantCulture);
			}

	public static char ToUpperInvariant(char c)
			{
				return ToUpper(c, CultureInfo.InvariantCulture);
			}

	public static bool TryParse(string s, out char result)
			{
				if(s != null)
				{
					if(s.Length == 1)
					{
						result = s[0];
						return true;
					}
				}
				return false;
			}

#if !CONFIG_COMPACT_FRAMEWORK
	public static int ConvertToUtf32(char highSurrogate, char lowSurrogate)
			{
				if(!IsHighSurrogate(highSurrogate))
				{
					throw new ArgumentOutOfRangeException
						("highSurrogate", _("ArgRange_HighSurrogate"));
				}
				if(!IsLowSurrogate(lowSurrogate))
				{
					throw new ArgumentOutOfRangeException
						("lowSurrogate", _("ArgRange_LowSurrogate"));
				}
				return ((((int)highSurrogate & 0x3ff) << 10) |
						((int)lowSurrogate & 0x3ff)) + 0x10000;
			}

	public static int ConvertToUtf32(string s, int index)
			{
				int char0;
				int char1;

				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				if(index < 0 || index >= s.Length)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_StringIndex"));
				}
				char0 = s[index];
				// Check if the character is no surrogate
				if(char0 < 0xd800 || char0 >= 0xe000)
				{
					return char0;
				}
				if(char0 > 0xdbff)
				{
					throw new ArgumentException(_("Arg_LowSurrogate"));
				}
				if(index >= s.Length - 1)
				{
					throw new ArgumentException(_("Arg_HighSurrogate"));
				}
				char1 = s[index + 1];
				// Check if the character is a high surrogate
				if(char1 < 0xdc00 || char1 >= 0xe000)
				{
					throw new ArgumentException(_("Arg_HighSurrogate"));
				}
				return (((char1 & 0x3ff) << 10) |
						(char0 & 0x3ff)) + 0x10000;
			}

#endif // !CONFIG_COMPACT_FRAMEWORK
#endif // CONFIG_FRAMEWORK_2_0
#endif // !ECMA_COMPAT

}; // class Char

}; // namespace System
