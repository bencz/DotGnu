/*
 * StringType.cs - Implementation of the
 *			"Microsoft.VisualBasic.StringType" class.
 *
 * Copyright (C) 2003, 2004  Southern Storm Software, Pty Ltd.
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

namespace Microsoft.VisualBasic.CompilerServices
{

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

[StandardModule]
#if CONFIG_COMPONENT_MODEL
[EditorBrowsable(EditorBrowsableState.Never)]
#endif
public sealed class StringType
{
	// This class cannot be instantiated.
	private StringType() {}

	// Convert a boolean value into a string.
	public static String FromBoolean(bool Value)
			{
				return (Value ? "True" : "False");
			}

	// Convert a byte value into a string.
	public static String FromByte(byte Value)
			{
				return Value.ToString();
			}

	// Convert char values into a string.
	public static String FromChar(char Value)
			{
				return Value.ToString();
			}

	// Convert a date value into a string.
	public static String FromDate(DateTime Value)
			{
				long ticks = Value.Ticks;
				if(ticks < TimeSpan.TicksPerDay)
				{
					// Format as a time value.
					return Value.ToString("T", null);
				}
				else if((ticks / TimeSpan.TicksPerDay) == 0)
				{
					// Format as a date value.
					return Value.ToString("d", null);
				}
				else
				{
					// Format as a date and time value.
					return Value.ToString("G", null);
				}
			}

	// Convert a decimal value into a string.
	public static String FromDecimal(Decimal Value)
			{
				return Value.ToString("G", null);
			}
	public static String FromDecimal
				(Decimal Value, NumberFormatInfo NumberFormat)
			{
				return Value.ToString("G", NumberFormat);
			}

	// Convert a double value into a string.
	public static String FromDouble(double Value)
			{
				return Value.ToString("G", null);
			}
	public static String FromDouble
				(double Value, NumberFormatInfo NumberFormat)
			{
				return Value.ToString("G", NumberFormat);
			}

	// Convert an integer value into a string.
	public static String FromInteger(int Value)
			{
				return Value.ToString();
			}

	// Convert a long value into a string.
	public static String FromLong(long Value)
			{
				return Value.ToString();
			}

	// Convert an object value into a string.
	public static String FromObject(Object Value)
			{
				if(Value != null)
				{
					if(Value is String)
					{
						return (String)Value;
					}
				#if !ECMA_COMPAT
					else if(Value is IConvertible)
					{
						return ((IConvertible)Value).ToString(null);
					}
					else if(Value is char[])
					{
						return new String(CharArrayType.FromObject(Value));
					}
					else
					{
						throw new InvalidCastException
							(String.Format(S._("VB_InvalidCast"),
										   Value.GetType(), "String"));
					}
				#else
					else
					{
						return Value.ToString();
					}
				#endif
				}
				else
				{
					return null;
				}
			}

	// Convert a short value into a string.
	public static String FromShort(short Value)
			{
				return Value.ToString();
			}

	// Convert a float value into a string.
	public static String FromSingle(float Value)
			{
				return Value.ToString("G", null);
			}
	public static String FromSingle
				(float Value, NumberFormatInfo NumberFormat)
			{
				return Value.ToString("G", NumberFormat);
			}

	// Insert a string into the middle of another.
	public static void MidStmtStr(ref String sDest, int StartPosition,
								  int MaxInsertLength, String sInsert)
			{
				// Get the lengths of the two strings.
				int len1 = (sDest != null ? sDest.Length : 0);
				int len2 = (sInsert != null ? sInsert.Length : 0);

				// Validate the starting index (base is one, not zero).
				--StartPosition;
				if(StartPosition < 0 || StartPosition >= len1)
				{
					throw new ArgumentException
						(S._("VB_InvalidStringIndex"), "StartPosition");
				}

				// Validate the replacement length.
				if(MaxInsertLength < 0)
				{
					throw new ArgumentException
						(S._("VB_InvalidStringLength"), "MaxInsertLength");
				}

				// Bounds-check the string that is to be inserted.
				if(len2 > MaxInsertLength)
				{
					len2 = MaxInsertLength;
				}
				int rest = (len1 - StartPosition);
				if(len2 > rest)
				{
					len2 = rest;
				}
				if(len2 == 0)
				{
					// No change to the incoming string.
					return;
				}

				// Build the final string.
				StringBuilder builder = new StringBuilder(len1);
				if(StartPosition > 0)
				{
					builder.Append(sDest, 0, StartPosition);
				}
				builder.Append(sInsert, 0, len2);
				rest = StartPosition + len2;
				if(rest < len1)
				{
					builder.Append(sDest, rest, len1 - rest);
				}
				sDest = builder.ToString();
			}

	// Compare two strings in various ways.
	public static int StrCmp(String sLeft, String sRight, bool TextCompare)
			{
				if(sLeft == null)
				{
					sLeft = String.Empty;
				}
				if(sRight == null)
				{
					sRight = String.Empty;
				}
				if(TextCompare)
				{
					return CultureInfo.CurrentCulture.CompareInfo
								.Compare(sLeft, sRight,
										 CompareOptions.IgnoreCase |
										 CompareOptions.IgnoreKanaType |
										 CompareOptions.IgnoreWidth);
				}
				else
				{
					return String.CompareOrdinal(sLeft, sRight);
				}
			}
	public static bool StrLike(String Source, String Pattern,
							   CompareMethod CompareOption)
			{
				// Handle the empty string cases first.
				if(Source == null || Source.Length == 0)
				{
					if(Pattern == null || Pattern.Length == 0)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else if(Pattern == null || Pattern.Length == 0)
				{
					return false;
				}

				// Compile the pattern into a set of matching rules.
				Rule[] rules = CompilePattern(Pattern, CompareOption);

				// Get the comparison object for the current culture.
				CompareInfo compare;
				if(CompareOption == CompareMethod.Binary)
				{
					compare = null;
				}
				else
				{
					compare = CultureInfo.CurrentCulture.CompareInfo;
				}

				// Execute the rules to match the string.
				int posn = 0;
				int len = Source.Length;
				int rule = 0;
				int backtrack = -1;
				bool doBacktrack = false;
				for(;;)
				{
					switch(rules[rule].command)
					{
						case RuleCommand.MatchEnd:
						{
							// Match the end of the string.
							if(posn >= len)
							{
								return true;
							}
							else
							{
								doBacktrack = true;
							}
						}
						break;

						case RuleCommand.MatchOne:
						{
							// Match a single character of any value.
							if(posn < len)
							{
								++posn;
								++rule;
							}
							else
							{
								doBacktrack = true;
							}
						}
						break;

						case RuleCommand.MatchDigit:
						{
							// Match a digit.
							if(posn < len &&
							   Source[posn] >= '0' && Source[posn] <= '9')
							{
								++posn;
								++rule;
							}
							else
							{
								doBacktrack = true;
							}
						}
						break;

						case RuleCommand.MatchAny:
						{
							// Match zero or more characters.
							rules[rule].posn = posn;
							rules[rule].len = backtrack;
							backtrack = rule;
							++rule;
						}
						break;

						case RuleCommand.MatchBinaryChar:
						{
							// Match a specific binary character.
							if(posn < len && Source[posn] == rules[rule].posn)
							{
								++posn;
								++rule;
							}
							else
							{
								doBacktrack = true;
							}
						}
						break;

						case RuleCommand.MatchTextChar:
						{
							// Match a specific text character.
							if(posn < len &&
							   EqualTextChar(Source, posn,
							   				 Pattern, rules[rule].posn,
											 compare))
							{
								++posn;
								++rule;
							}
							else
							{
								doBacktrack = true;
							}
						}
						break;

						case RuleCommand.MatchBinarySet:
						{
							// Match any character in a given binary set.
							if(posn < len &&
							   CharInBinarySet(Pattern, rules[rule].posn,
							   			       rules[rule].len, Source[posn]))
							{
								++posn;
								++rule;
							}
							else
							{
								doBacktrack = true;
							}
						}
						break;

						case RuleCommand.MatchTextSet:
						{
							// Match any character in a given text set.
							if(posn < len &&
							   CharInTextSet(Pattern, rules[rule].posn,
							   			     rules[rule].len, Source, posn,
										     compare))
							{
								++posn;
								++rule;
							}
							else
							{
								doBacktrack = true;
							}
						}
						break;

						case RuleCommand.MatchBinaryInvSet:
						{
							// Match any character not in a given binary set.
							if(posn < len &&
							   !CharInBinarySet(Pattern, rules[rule].posn,
							   			        rules[rule].len, Source[posn]))
							{
								++posn;
								++rule;
							}
							else
							{
								doBacktrack = true;
							}
						}
						break;

						case RuleCommand.MatchTextInvSet:
						{
							// Match any character not in a given text set.
							if(posn < len &&
							   !CharInTextSet(Pattern, rules[rule].posn,
							   			      rules[rule].len, Source, posn,
											  compare))
							{
								++posn;
								++rule;
							}
							else
							{
								doBacktrack = true;
							}
						}
						break;
					}
					if(doBacktrack)
					{
						if(backtrack != -1)
						{
							posn = ++(rules[backtrack].posn);
							rule = backtrack + 1;
							while(posn >= len)
							{
								backtrack = rules[backtrack].len;
								if(backtrack != -1)
								{
									posn = ++(rules[backtrack].posn);
									rule = backtrack + 1;
								}
								else
								{
									return false;
								}
							}
						}
						else
						{
							return false;
						}
						doBacktrack = false;
					}
				}
			}
	public static bool StrLikeBinary(String Source, String Pattern)
			{
				return StrLike(Source, Pattern, CompareMethod.Binary);
			}
	public static bool StrLikeText(String Source, String Pattern)
			{
				return StrLike(Source, Pattern, CompareMethod.Text);
			}

	// Available rule commands.
	private enum RuleCommand
	{
		MatchEnd,
		MatchOne,
		MatchDigit,
		MatchAny,
		MatchBinaryChar,
		MatchTextChar,
		MatchBinarySet,
		MatchTextSet,
		MatchBinaryInvSet,
		MatchTextInvSet

	}; // enum RuleCommand

	// Information about a "like" matching rule.
	private struct Rule
	{
		public RuleCommand command;
		public int posn;
		public int len;

	}; // struct Rule

	// Bail out for an invalid pattern.
	private static void InvalidPattern()
			{
				throw new ArgumentException(S._("VB_InvalidPattern"));
			}

	// Compile a "like" pattern into a set of matching rules.
	private static Rule[] CompilePattern(String pattern,
										 CompareMethod method)
			{
				Rule[] rules = new Rule [pattern.Length + 1];
				int posn, len, start;
				int rule = 0;
				char ch;
				len = pattern.Length;
				posn = 0;
				while(posn < len)
				{
					ch = pattern[posn++];
					if(ch == '?')
					{
						rules[rule].command = RuleCommand.MatchOne;
					}
					else if(ch == '#')
					{
						rules[rule].command = RuleCommand.MatchDigit;
					}
					else if(ch == '*')
					{
						rules[rule].command = RuleCommand.MatchAny;
					}
					else if(ch == '[')
					{
						if(posn >= len)
						{
							InvalidPattern();
						}
						ch = pattern[posn++];
						if(ch == '!')
						{
							if(method == CompareMethod.Binary)
							{
								rules[rule].command =
									RuleCommand.MatchBinaryInvSet;
							}
							else
							{
								rules[rule].command =
									RuleCommand.MatchTextInvSet;
							}
							if(posn >= len)
							{
								InvalidPattern();
							}
							ch = pattern[posn++];
						}
						else if(method == CompareMethod.Binary)
						{
							rules[rule].command = RuleCommand.MatchBinarySet;
						}
						else
						{
							rules[rule].command = RuleCommand.MatchTextSet;
						}
						start = posn - 1;
						if(ch == ']')
						{
							// Ignore empty "[]" sequences.
							continue;
						}
						while(posn < len && pattern[posn] != ']')
						{
							++posn;
						}
						if(posn >= len)
						{
							InvalidPattern();
						}
						rules[rule].posn = start;
						rules[rule].len = posn - start;
						++posn;
					}
					else if(method == CompareMethod.Binary)
					{
						rules[rule].command = RuleCommand.MatchBinaryChar;
						rules[rule].posn = ch;
					}
					else
					{
						rules[rule].command = RuleCommand.MatchTextChar;
						rules[rule].posn = posn - 1;
					}
					++rule;
				}
				rules[rule].command = RuleCommand.MatchEnd;
				return rules;
			}

	// Determine if two characters are equal, using a text compare.
	private static bool EqualTextChar(String str1, int posn1,
									  String str2, int posn2,
									  CompareInfo compare)
			{
				if(str1[posn1] == str2[posn2])
				{
					// Short cut for the common case.
					return true;
				}
				return (compare.Compare(str1, posn1, 1, str2, posn2, 1,
										CompareOptions.IgnoreCase |
										CompareOptions.IgnoreNonSpace |
										CompareOptions.IgnoreKanaType |
										CompareOptions.IgnoreWidth) == 0);
			}

	// Determine if a character appears in a binary set.
	private static bool CharInBinarySet(String pattern, int offset,
								        int length, char ch)
			{
				char match;
				while(length > 0)
				{
					match = pattern[offset++];
					--length;
					if(length > 1 && pattern[offset] == '-')
					{
						// Match against a character range.
						if(ch >= match && ch <= pattern[offset + 1])
						{
							return true;
						}
						offset += 2;
						length -= 2;
					}
					else
					{
						// Match against a single character.
						if(match == ch)
						{
							return true;
						}
					}
				}
				return false;
			}

	// Determine if a character appears in a text set.
	private static bool CharInTextSet(String pattern, int offset,
								      int length, String source, int posn,
								  	  CompareInfo compare)
			{
				char match;
				while(length > 0)
				{
					match = pattern[offset];
					if(length > 2 && pattern[offset + 1] == '-')
					{
						// Match against a character range.
						if(compare.Compare(source, posn, 1,
										   pattern, offset, 1,
										   CompareOptions.IgnoreCase |
										   CompareOptions.IgnoreNonSpace |
										   CompareOptions.IgnoreKanaType |
										   CompareOptions.IgnoreWidth) >= 0 &&
						   compare.Compare(source, posn, 1,
										   pattern, offset + 2, 1,
										   CompareOptions.IgnoreCase |
										   CompareOptions.IgnoreNonSpace |
										   CompareOptions.IgnoreKanaType |
										   CompareOptions.IgnoreWidth) <= 0)
						{
							return true;
						}
						offset += 2;
						length -= 2;
					}
					else
					{
						// Match against a single character.
						if(compare.Compare(source, posn, 1,
										   pattern, offset, 1,
										   CompareOptions.IgnoreCase |
										   CompareOptions.IgnoreNonSpace |
										   CompareOptions.IgnoreKanaType |
										   CompareOptions.IgnoreWidth) == 0)
						{
							return true;
						}
					}
					++offset;
					--length;
				}
				return false;
			}

}; // class StringType

}; // namespace Microsoft.VisualBasic.CompilerServices
