/*
 * XmlConvert.cs - Implementation of the "System.Xml.XmlConvert" class.
 *
 * Copyright (C) 2002 Southern Storm Software, Pty Ltd.
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
 
namespace System.Xml
{

using System;
using System.Text;
using System.Globalization;

public class XmlConvert
{
	// The list of all relevant DateTime formats for "ToDateTime".
	private static readonly String[] formatList = {
		"yyyy-MM-ddTHH:mm:ss",
		"yyyy-MM-ddTHH:mm:ss.f",
		"yyyy-MM-ddTHH:mm:ss.ff",
		"yyyy-MM-ddTHH:mm:ss.fff",
		"yyyy-MM-ddTHH:mm:ss.ffff",
		"yyyy-MM-ddTHH:mm:ss.fffff",
		"yyyy-MM-ddTHH:mm:ss.ffffff",
		"yyyy-MM-ddTHH:mm:ss.fffffff",
		"yyyy-MM-ddTHH:mm:ssZ",
		"yyyy-MM-ddTHH:mm:ss.fZ",
		"yyyy-MM-ddTHH:mm:ss.ffZ",
		"yyyy-MM-ddTHH:mm:ss.fffZ",
		"yyyy-MM-ddTHH:mm:ss.ffffZ",
		"yyyy-MM-ddTHH:mm:ss.fffffZ",
		"yyyy-MM-ddTHH:mm:ss.ffffffZ",
		"yyyy-MM-ddTHH:mm:ss.fffffffZ",
		"yyyy-MM-ddTHH:mm:sszzzzzz",
		"yyyy-MM-ddTHH:mm:ss.fzzzzzz",
		"yyyy-MM-ddTHH:mm:ss.ffzzzzzz",
		"yyyy-MM-ddTHH:mm:ss.fffzzzzzz",
		"yyyy-MM-ddTHH:mm:ss.ffffzzzzzz",
		"yyyy-MM-ddTHH:mm:ss.fffffzzzzzz",
		"yyyy-MM-ddTHH:mm:ss.ffffffzzzzzz",
		"yyyy-MM-ddTHH:mm:ss.fffffffzzzzzz",
		"HH:mm:ss",
		"HH:mm:ss.f",
		"HH:mm:ss.ff",
		"HH:mm:ss.fff",
		"HH:mm:ss.ffff",
		"HH:mm:ss.fffff",
		"HH:mm:ss.ffffff",
		"HH:mm:ss.fffffff",
		"HH:mm:ssZ",
		"HH:mm:ss.fZ",
		"HH:mm:ss.ffZ",
		"HH:mm:ss.fffZ",
		"HH:mm:ss.ffffZ",
		"HH:mm:ss.fffffZ",
		"HH:mm:ss.ffffffZ",
		"HH:mm:ss.fffffffZ",
		"HH:mm:sszzzzzz",
		"HH:mm:ss.fzzzzzz",
		"HH:mm:ss.ffzzzzzz",
		"HH:mm:ss.fffzzzzzz",
		"HH:mm:ss.ffffzzzzzz",
		"HH:mm:ss.fffffzzzzzz",
		"HH:mm:ss.ffffffzzzzzz",
		"HH:mm:ss.fffffffzzzzzz",
		"yyyy-MM-dd",
		"yyyy-MM-ddZ",
		"yyyy-MM-ddzzzzzz",
		"yyyy-MM",
		"yyyy-MMZ",
		"yyyy-MMzzzzzz",
		"yyyy",
		"yyyyZ",
		"yyyyzzzzzz",
		"--MM-dd",
		"--MM-ddZ",
		"--MM-ddzzzzzz",
		"---dd",
		"---ddZ",
		"---ddzzzzzz",
		"--MM--",
		"--MM--Z",
		"--MM--zzzzzz"
	};

	private static readonly	char[] hexCode = {'0','1','2','3','4','5','6','7'
				,'8','9','A','B','C','D','E','F'};
	
	// Constructor.
	public XmlConvert() {}

	// Determine if a character is hexadecimal.
	private static bool IsHex(char ch)
			{
				if(ch >= '0' && ch <= '9')
				{
					return true;
				}
				else if(ch >= 'a' && ch <= 'f')
				{
					return true;
				}
				else if(ch >= 'A' && ch <= 'F')
				{
					return true;
				}
				else
				{
					return false;
				}
			}

	// Convert a character from hex to integer.
	private static int FromHex(char ch)
			{
				if(ch >= '0' && ch <= '9')
				{
					return (int)(ch - '0');
				}
				else if(ch >= 'a' && ch <= 'f')
				{
					return (int)(ch - 'a' + 10);
				}
				else
				{
					return (int)(ch - 'A' + 10);
				}
			}

	// Determine if a character is a name start.
	internal static bool IsNameStart(char ch, bool allowColon)
			{
				UnicodeCategory category = Char.GetUnicodeCategory(ch);
				if(category == UnicodeCategory.LowercaseLetter ||
				   category == UnicodeCategory.UppercaseLetter ||
				   category == UnicodeCategory.OtherLetter ||
				   category == UnicodeCategory.TitlecaseLetter ||
				   category == UnicodeCategory.LetterNumber)
				{
					return true;
				}
				else
				{
					return ((allowColon && ch == ':') || ch == '_');
				}
			}

	// Determine if a character is a non-start name character.
	internal static bool IsNameNonStart(char ch, bool allowColon)
			{
				UnicodeCategory category = Char.GetUnicodeCategory(ch);
				if(category == UnicodeCategory.LowercaseLetter ||
				   category == UnicodeCategory.UppercaseLetter ||
				   category == UnicodeCategory.DecimalDigitNumber ||
				   category == UnicodeCategory.OtherLetter ||
				   category == UnicodeCategory.TitlecaseLetter ||
				   category == UnicodeCategory.LetterNumber ||
				   category == UnicodeCategory.SpacingCombiningMark ||
				   category == UnicodeCategory.EnclosingMark ||
				   category == UnicodeCategory.NonSpacingMark ||
				   category == UnicodeCategory.ModifierLetter)
				{
					return true;
				}
				else
				{
					return ((allowColon && ch == ':') || ch == '_' ||
							ch == '-' || ch == '.');
				}
			}

	// Decode a name that has escaped hexadecimal characters.
	public static String DecodeName(String name)
			{
				int posn, posn2;
				StringBuilder result;

				// Bail out if the name does not contain encoded characters.
				if(name == null || (posn = name.IndexOf("_x")) == -1)
				{
					return name;
				}

				// Decode the string and build a new one.
				result = new StringBuilder();
				if(posn > 0)
				{
					result.Append(name.Substring(0, posn));
				}
				while(posn <= (name.Length - 7))
				{
					if(name[posn] == '_' && name[posn + 1] == 'x' &&
					   IsHex(name[posn + 2]) && IsHex(name[posn + 3]) &&
					   IsHex(name[posn + 4]) && IsHex(name[posn + 5]) &&
					   name[posn + 6] == '_')
					{
						// This is a hexadecimal character encoding.
						result.Append((char)((FromHex(name[posn + 2]) << 12) |
											 (FromHex(name[posn + 3]) << 8) |
											 (FromHex(name[posn + 4]) << 4) |
											  FromHex(name[posn + 5])));
						posn += 7;
					}
					else
					{
						// Search for the next candidate.
						posn2 = name.IndexOf('_', posn + 1);
						if(posn2 != -1)
						{
							result.Append(name.Substring(posn, posn2 - posn));
							posn = posn2;
						}
						else
						{
							result.Append('_');
							break;
						}
					}
				}
				if(posn < name.Length)
				{
					result.Append(name.Substring(posn));
				}

				// Return the final string to the caller.
				return result.ToString();
			}

	// Hex characters to use for encoding purposes.
	private static readonly String hexchars = "0123456789ABCDEF";

	// Append the hexadecimal version of a character to a string builder.
	private static void AppendHex(StringBuilder result, char ch)
			{
				result.Append('_');
				result.Append('x');
				result.Append(hexchars[(ch >> 12) & 0x0F]);
				result.Append(hexchars[(ch >> 8) & 0x0F]);
				result.Append(hexchars[(ch >> 4) & 0x0F]);
				result.Append(hexchars[ch & 0x0F]);
				result.Append('_');
			}

	// Inner version of "EncodeName", "EncodeLocalName", and "EncodeNmToken".
	private static String InnerEncodeName(String name, bool allowColon,
										  bool isNmToken)
			{
				int posn;
				char ch;

				// Bail out if the name is null or empty.
				if(name == null || name.Length == 0)
				{
					return name;
				}

				// Bail out if the name is already OK.
				ch = name[0];
				if((isNmToken && IsNameNonStart(ch, allowColon)) ||
				   (!isNmToken && IsNameStart(ch, allowColon)))
				{
					if(ch != '_' || name.Length == 1 || name[1] != 'x')
					{
						for(posn = 1; posn < name.Length; ++posn)
						{
							ch = name[posn];
							if(!IsNameNonStart(ch, allowColon))
							{
								break;
							}
							if(ch == '_' && (posn + 1) < name.Length &&
							   name[posn + 1] == 'x')
							{
								break;
							}
						}
						if(posn >= name.Length)
						{
							return name;
						}
					}
				}

				// Build a new string with the encoded form.
				StringBuilder result = new StringBuilder();
				ch = name[0];
				if((isNmToken && IsNameNonStart(ch, allowColon)) ||
				   (!isNmToken && IsNameStart(ch, allowColon)))
				{
					if(ch == '_' && name.Length > 1 && name[1] == 'x')
					{
						AppendHex(result, '_');
					}
					else
					{
						result.Append(ch);
					}
				}
				else
				{
					AppendHex(result, ch);
				}
				for(posn = 1; posn < name.Length; ++posn)
				{
					ch = name[posn];
					if(IsNameNonStart(ch, allowColon))
					{
						if(ch == '_' && (posn + 1) < name.Length &&
						   name[posn + 1] == 'x')
						{
							AppendHex(result, '_');
						}
						else
						{
							result.Append(ch);
						}
					}
					else
					{
						AppendHex(result, ch);
					}
				}

				// Return the encoded string to the caller.
				return result.ToString();
			}

	// Encode a name to escape special characters using hexadecimal.
	public static String EncodeName(String name)
			{
				return InnerEncodeName(name, true, false);
			}

	// Encode a local name to escape special characters using hexadecimal.
	public static String EncodeLocalName(String name)
			{
				return InnerEncodeName(name, false, false);
			}

	// Encode a name token to escape special characters using hexadecimal.
	public static String EncodeNmToken(String name)
			{
				return InnerEncodeName(name, true, true);
			}

	// Convert a string to boolean.
	public static bool ToBoolean(String s)
			{
				if(s == null)
				{
					throw new ArgumentNullException("s");
				}
				s = s.Trim();
				if(s == "true" || s == "1")
				{
					return true;
				}
				else if(s == "false" || s == "0")
				{
					return false;
				}
				else
				{
					throw new FormatException(S._("Xml_InvalidBoolean"));
				}
			}

	// Convert a string to a byte value.
	public static byte ToByte(String s)
			{
				return Byte.Parse(s, NumberStyles.AllowLeadingWhite |
									 NumberStyles.AllowTrailingWhite,
								  NumberFormatInfo.InvariantInfo);
			}

	// Convert a string to a character value.
	public static char ToChar(String s)
			{
				return Char.Parse(s);
			}

	// Convert a string into a DateTime value.
	public static DateTime ToDateTime(String s)
			{
				return DateTime.ParseExact(s, formatList,
										   DateTimeFormatInfo.InvariantInfo,
										   DateTimeStyles.AllowLeadingWhite |
										   DateTimeStyles.AllowTrailingWhite);
			}

	// Convert a string into a DateTime value using a specific format.
	public static DateTime ToDateTime(String s, String format)
			{
				return DateTime.ParseExact(s, format,
										   DateTimeFormatInfo.InvariantInfo,
										   DateTimeStyles.AllowLeadingWhite |
										   DateTimeStyles.AllowTrailingWhite);
			}

	// Convert a string into a DateTime value using a list of formats.
	public static DateTime ToDateTime(String s, String[] formats)
			{
				return DateTime.ParseExact(s, formats,
										   DateTimeFormatInfo.InvariantInfo,
										   DateTimeStyles.AllowLeadingWhite |
										   DateTimeStyles.AllowTrailingWhite);
			}

#if CONFIG_EXTENDED_NUMERICS
	// Convert a string to a decimal value.
	public static Decimal ToDecimal(String s)
			{
				return Decimal.Parse(s, NumberStyles.AllowLeadingSign |
										NumberStyles.AllowDecimalPoint |
										NumberStyles.AllowLeadingWhite |
									 	NumberStyles.AllowTrailingWhite,
								     NumberFormatInfo.InvariantInfo);
			}

	// Convert a string to a double-precision value.
	public static double ToDouble(String s)
			{
				s = s.Trim();
				if(s == "-INF")
				{
					return Double.NegativeInfinity;
				}
				else if(s == "INF")
				{
					return Double.PositiveInfinity;
				}
				return Double.Parse(s, NumberStyles.AllowLeadingSign |
									   NumberStyles.AllowDecimalPoint |
									   NumberStyles.AllowExponent |
									   NumberStyles.AllowLeadingWhite |
									   NumberStyles.AllowTrailingWhite,
								    NumberFormatInfo.InvariantInfo);
			}
#endif

#if !ECMA_COMPAT

	// Convert a string into a GUID value.
	public static Guid ToGuid(String value) 
			{
				return new Guid(value);
			}

#endif

	// Convert a string to an Int16 value.
	public static short ToInt16(String s)
			{
				return Int16.Parse(s, NumberStyles.AllowLeadingSign |
									  NumberStyles.AllowLeadingWhite |
									  NumberStyles.AllowTrailingWhite,
								   NumberFormatInfo.InvariantInfo);
			}

	// Convert a string to a UInt16 value.
	[CLSCompliant(false)]
	public static ushort ToUInt16(String s)
			{
				return UInt16.Parse(s, NumberStyles.AllowLeadingWhite |
									   NumberStyles.AllowTrailingWhite,
								    NumberFormatInfo.InvariantInfo);
			}

	// Convert a string to an Int32 value.
	public static int ToInt32(String s)
			{
				return Int32.Parse(s, NumberStyles.AllowLeadingSign |
									  NumberStyles.AllowLeadingWhite |
									  NumberStyles.AllowTrailingWhite,
								   NumberFormatInfo.InvariantInfo);
			}

	// Convert a string to a UInt32 value.
	[CLSCompliant(false)]
	public static uint ToUInt32(String s)
			{
				return UInt32.Parse(s, NumberStyles.AllowLeadingWhite |
									   NumberStyles.AllowTrailingWhite,
								    NumberFormatInfo.InvariantInfo);
			}

	// Convert a string to an Int64 value.
	public static long ToInt64(String s)
			{
				return Int64.Parse(s, NumberStyles.AllowLeadingSign |
									  NumberStyles.AllowLeadingWhite |
									  NumberStyles.AllowTrailingWhite,
								   NumberFormatInfo.InvariantInfo);
			}

	// Convert a string to a UInt64 value.
	[CLSCompliant(false)]
	public static ulong ToUInt64(String s)
			{
				return UInt64.Parse(s, NumberStyles.AllowLeadingWhite |
									   NumberStyles.AllowTrailingWhite,
								    NumberFormatInfo.InvariantInfo);
			}

	// Convert a string to a signed byte value.
	[CLSCompliant(false)]
	public static sbyte ToSByte(String s)
			{
				return SByte.Parse(s, NumberStyles.AllowLeadingSign |
									  NumberStyles.AllowLeadingWhite |
									  NumberStyles.AllowTrailingWhite,
								   NumberFormatInfo.InvariantInfo);
			}

#if CONFIG_EXTENDED_NUMERICS
	// Convert a string to a single-precision value.
	public static float ToSingle(String s)
			{
				s = s.Trim();
				if(s == "-INF")
				{
					return Single.NegativeInfinity;
				}
				else if(s == "INF")
				{
					return Single.PositiveInfinity;
				}
				return Single.Parse(s, NumberStyles.AllowLeadingSign |
									   NumberStyles.AllowDecimalPoint |
									   NumberStyles.AllowExponent |
									   NumberStyles.AllowLeadingWhite |
									   NumberStyles.AllowTrailingWhite,
								    NumberFormatInfo.InvariantInfo);
			}
#endif

	// Convert a string to a TimeSpan value.
	public static TimeSpan ToTimeSpan(String s)
			{
				return TimeSpan.Parse(s);
			}

	// Convert a boolean value into a string.
	public static String ToString(bool value)
			{
				return (value ? "true" : "false");
			}

	// Convert a character value into a string.
	public static String ToString(char value)
			{
				return value.ToString(null);
			}

#if CONFIG_EXTENDED_NUMERICS
	// Convert a decimal value into a string.
	public static String ToString(Decimal value)
			{
				return value.ToString(null, NumberFormatInfo.InvariantInfo);
			}
#endif

	// Convert a byte value into a string.
	public static String ToString(byte value)
			{
				return value.ToString(null, NumberFormatInfo.InvariantInfo);
			}

	// Convert a signed byte value into a string.
	[CLSCompliant(false)]
	public static String ToString(sbyte value)
			{
				return value.ToString(null, NumberFormatInfo.InvariantInfo);
			}

	// Convert an Int16 value into a string.
	public static String ToString(short value)
			{
				return value.ToString(null, NumberFormatInfo.InvariantInfo);
			}

	// Convert a UInt16 value into a string.
	[CLSCompliant(false)]
	public static String ToString(ushort value)
			{
				return value.ToString(null, NumberFormatInfo.InvariantInfo);
			}

	// Convert an Int32 value into a string.
	public static String ToString(int value)
			{
				return value.ToString(null, NumberFormatInfo.InvariantInfo);
			}

	// Convert a UInt32 value into a string.
	[CLSCompliant(false)]
	public static String ToString(uint value)
			{
				return value.ToString(null, NumberFormatInfo.InvariantInfo);
			}

	// Convert an Int64 value into a string.
	public static String ToString(long value)
			{
				return value.ToString(null, NumberFormatInfo.InvariantInfo);
			}

	// Convert a UInt64 value into a string.
	[CLSCompliant(false)]
	public static String ToString(ulong value)
			{
				return value.ToString(null, NumberFormatInfo.InvariantInfo);
			}

#if CONFIG_EXTENDED_NUMERICS
	// Convert a single-precision value into a string.
	public static String ToString(float value)
			{
				if(Single.IsNegativeInfinity(value))
				{
					return "-INF";
				}
				else if(Single.IsPositiveInfinity(value))
				{
					return "INF";
				}
				return value.ToString("R", NumberFormatInfo.InvariantInfo);
			}

	// Convert a double-precision value into a string.
	public static String ToString(double value)
			{
				if(Double.IsNegativeInfinity(value))
				{
					return "-INF";
				}
				else if(Double.IsPositiveInfinity(value))
				{
					return "INF";
				}
				return value.ToString("R", NumberFormatInfo.InvariantInfo);
			}
#endif

	// Convert a TimeSpan value into a string.
	public static String ToString(TimeSpan value)
			{
				return value.ToString();
			}

	// Convert a DateTime value into a string using the default format.
	public static String ToString(DateTime value)
			{
				return ToString(value, "yyyy-MM-ddTHH:mm:ss.fffffffzzzzzz");
			}

	// Convert a DateTime value into a string using a specified format.
	public static String ToString(DateTime value, String format)
			{
				return value.ToString
					(format, DateTimeFormatInfo.InvariantInfo);
			}
	
#if !ECMA_COMPAT	

	// Convert a GUID into a string.
	public static String ToString(Guid value) 
			{
				return value.ToString();
			}

#endif

	// Inner version of "VerifyName" and "VerifyNCName".
	private static String InnerVerify(String name, bool allowColon)
			{
				int posn;
				if(name == null || name.Length == 0)
				{
					throw new ArgumentNullException("name");
				}
				if(!IsNameStart(name[0], allowColon))
				{
					throw new XmlException(S._("Xml_InvalidName"));
				}
				for(posn = 1; posn < name.Length; ++posn)
				{
					if(!IsNameNonStart(name[posn], allowColon))
					{
						throw new XmlException(S._("Xml_InvalidName"));
					}
				}
				return name;
			}

	// Verify that a string is a valid XML name.
	public static String VerifyName(String name)
			{
				return InnerVerify(name, true);
			}

	// Verify that a string is a valid XML qualified name.
	public static String VerifyNCName(String name)
			{
				return InnerVerify(name, false);
			}

	// Characters to use to encode 6-bit values in base64.
	internal const String base64Chars =
		"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

	// Convert a byte buffer into a base64 string.
	internal static String ToBase64String
				(byte[] inArray, int offset, int length)
			{
				// Validate the parameters.
				if(inArray == null)
				{
					throw new ArgumentNullException("inArray");
				}
				if(offset < 0 || offset > inArray.Length)
				{
					throw new ArgumentOutOfRangeException
						("offset", S._("ArgRange_Array"));
				}
				if(length < 0 || length > (inArray.Length - offset))
				{
					throw new ArgumentOutOfRangeException
						("length", S._("ArgRange_Array"));
				}

				// Convert the bytes.
				StringBuilder builder =
					new StringBuilder
						((int)(((((long)length) + 2L) * 4L) / 3L));
				int bits = 0;
				int numBits = 0;
				String base64 = base64Chars;
				int size = length;
				while(size > 0)
				{
					bits = (bits << 8) + inArray[offset++];
					numBits += 8;
					--size;
					while(numBits >= 6)
					{
						numBits -= 6;
						builder.Append(base64[bits >> numBits]);
						bits &= ((1 << numBits) - 1);
					}
				}
				length %= 3;
				if(length == 1)
				{
					builder.Append(base64[bits << (6 - numBits)]);
					builder.Append('=');
					builder.Append('=');
				}
				else if(length == 2)
				{
					builder.Append(base64[bits << (6 - numBits)]);
					builder.Append('=');
				}

				// Finished.
				return builder.ToString();
			}

	// Convert a byte buffer into a hex String.
	internal static String ToHexString
				(byte[] inArray, int offset, int length)
			{
				// Validate the parameters.
				if(inArray == null)
				{
					throw new ArgumentNullException("inArray");
				}
				if(offset < 0 || offset > inArray.Length)
				{
					throw new ArgumentOutOfRangeException
						("offset", S._("ArgRange_Array"));
				}
				if(length < 0 || length > (inArray.Length - offset))
				{
					throw new ArgumentOutOfRangeException
						("length", S._("ArgRange_Array"));
				}

				byte currentByte;
				// Convert the bytes.
				StringBuilder builder =
					new StringBuilder
						((int)(((long)length) * 2L));
				for(int a = 0; a < length; a++)
				{
					currentByte = inArray[offset+a];
					builder.Append(hexCode[currentByte >> 4]);
					builder.Append(hexCode[currentByte & 0xF]);
				}

				// Finished.
				return builder.ToString();
			}
}; // class XmlConvert

}; // namespace System.Xml
