/*
 * JSScanner.cs - Main interface to the JScript scanning routines.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.JScript
{

using System;
using System.Text;
using System.Collections;
using System.Globalization;

public sealed class JSScanner
{
	// Internal state.
	private Context tokenInfo;
	private bool authoringMode;
	private bool sawEOL;
	private int posn, end;
	private int line, lineStart;
	private String source;
	private String stringLiteral;
	private String identifierName;
	private Hashtable keywords;
	private static Hashtable keywordsGlobal;

	// Constructors.
	public JSScanner()
			{
				tokenInfo = new Context(String.Empty);
				authoringMode = false;
				sawEOL = false;
				posn = 0;
				end = 0;
				line = 1;
				lineStart = 0;
				source = String.Empty;
				stringLiteral = null;
				identifierName = null;
				keywords = GetKeywordTable();
			}
	public JSScanner(Context context)
			: this()
			{
				SetSource(context);
			}
	internal JSScanner(String quickString)
			{
				// This version is used to quickly parse an identifier
				// to validate it using "VsaEngine.IsValidIdentifier"
				// and "VsaEngine.IsValidNamespaceName".  To scan a
				// string for real, use "JSScanner(Context)" instead.
				source = quickString;
				posn = 0;
				end = quickString.Length;
				keywords = GetKeywordTable();
			}

	// Get the current line within the source string.
	public int GetCurrentLine()
			{
				return line;
			}

	// Get the current position within the source string.
	public int GetCurrentPosition(bool absolute)
			{
				return posn;
			}

	// Fetch the next character in the input stream.
	internal int Fetch()
			{
				if(posn < end)
				{
					return (int)(source[posn++]);
				}
				else
				{
					return -1;
				}
			}

	// Peek at the next character in the input stream.
	internal int Peek()
			{
				if(posn < end)
				{
					return (int)(source[posn]);
				}
				else
				{
					return -1;
				}
			}

	// Peek two characters ahead in the input stream.
	private int Peek2()
			{
				if(posn < (end - 1))
				{
					return (int)(source[posn + 1]);
				}
				else
				{
					return -1;
				}
			}

	// Peek N characters ahead in the input stream.
	private int PeekN(int n)
			{
				if(posn <= (end - n))
				{
					return (int)(source[posn + n - 1]);
				}
				else
				{
					return -1;
				}
			}

	// Unget the last character.
	private void Unget(int ch)
			{
				if(ch != -1)
				{
					--posn;
				}
			}

	// Determine if a character is an identifier start.
	private static bool IsIdentifierStart(int ch)
			{
				// Handle the easy cases first.
				if(ch >= 'A' && ch < 'Z')
				{
					return true;
				}
				else if(ch >= 'a' && ch <= 'z')
				{
					return true;
				}
				else if(ch == '$' || ch == '_')
				{
					return true;
				}

				// Query the character's Unicode category and check it.
				UnicodeCategory category = Char.GetUnicodeCategory((char)ch);
				return (category == UnicodeCategory.UppercaseLetter ||
						category == UnicodeCategory.LowercaseLetter ||
						category == UnicodeCategory.TitlecaseLetter ||
						category == UnicodeCategory.ModifierLetter ||
						category == UnicodeCategory.OtherLetter ||
						category == UnicodeCategory.LetterNumber);
			}

	// Fetch an identifier start character, or -1 if not a valid start.
	private int FetchIdentifierStart()
			{
				int start = posn;
				int ch = Fetch();

				// Handle the easy cases first.
				if(ch >= 'A' && ch < 'Z')
				{
					return ch;
				}
				else if(ch >= 'a' && ch <= 'z')
				{
					return ch;
				}
				else if(ch == '$' || ch == '_')
				{
					return ch;
				}
				else if(ch == -1)
				{
					return -1;
				}

				// Deal with escape sequences.
				if(ch == '\\' && Peek() == 'u' && IsHexDigit(Peek2()) &&
				   IsHexDigit(PeekN(3)) && IsHexDigit(PeekN(4)) &&
				   IsHexDigit(PeekN(5)))
				{
					++posn;
					ch = FromHex(Fetch()) << 12;
					ch |= FromHex(Fetch()) << 8;
					ch |= FromHex(Fetch()) << 4;
					ch |= FromHex(Fetch());
				}

				// Query the character's Unicode category and check it.
				UnicodeCategory category = Char.GetUnicodeCategory((char)ch);
				if(category == UnicodeCategory.UppercaseLetter ||
				   category == UnicodeCategory.LowercaseLetter ||
				   category == UnicodeCategory.TitlecaseLetter ||
				   category == UnicodeCategory.ModifierLetter ||
				   category == UnicodeCategory.OtherLetter ||
				   category == UnicodeCategory.LetterNumber)
				{
					return ch;
				}
				else
				{
					posn = start;
					return -1;
				}
			}

	// Fetch an identifier part character, or -1 if not a valid part.
	private int FetchIdentifierPart()
			{
				int start = posn;
				int ch = Fetch();

				// Handle the easy cases first.
				if(ch >= 'A' && ch < 'Z')
				{
					return ch;
				}
				else if(ch >= 'a' && ch <= 'z')
				{
					return ch;
				}
				else if(ch >= '0' && ch <= '9')
				{
					return ch;
				}
				else if(ch == '$' || ch == '_')
				{
					return ch;
				}
				else if(ch == -1)
				{
					return -1;
				}

				// Deal with escape sequences.
				if(ch == '\\' && Peek() == 'u' && IsHexDigit(Peek2()) &&
				   IsHexDigit(PeekN(3)) && IsHexDigit(PeekN(4)) &&
				   IsHexDigit(PeekN(5)))
				{
					++posn;
					ch = FromHex(Fetch()) << 12;
					ch |= FromHex(Fetch()) << 8;
					ch |= FromHex(Fetch()) << 4;
					ch |= FromHex(Fetch());
				}

				// Query the character's Unicode category and check it.
				UnicodeCategory category = Char.GetUnicodeCategory((char)ch);
				if(category == UnicodeCategory.UppercaseLetter ||
				   category == UnicodeCategory.LowercaseLetter ||
				   category == UnicodeCategory.TitlecaseLetter ||
				   category == UnicodeCategory.ModifierLetter ||
				   category == UnicodeCategory.OtherLetter ||
				   category == UnicodeCategory.LetterNumber ||
				   category == UnicodeCategory.DecimalDigitNumber ||
				   category == UnicodeCategory.ConnectorPunctuation ||
				   category == UnicodeCategory.NonSpacingMark ||
				   category == UnicodeCategory.SpacingCombiningMark)
				{
					return ch;
				}
				else
				{
					posn = start;
					return -1;
				}
			}

	// Fetch an identifier, returning null if one was not found
	// or it was a keyword.  This is used to assist with identifier
	// validation in "VsaEngine".
	internal String FetchIdentifier()
			{
				StringBuilder builder = new StringBuilder();
				String name;
				Object keyword;
				bool quoted = false;
				int ch;
				if(Peek() == '\\')
				{
					// May be the start of an identifier or a quoted keyword.
					ch = FetchIdentifierStart();
					if(ch == -1)
					{
						if(IsIdentifierStart(Peek2()))
						{
							Fetch();
							ch = FetchIdentifierStart();
							quoted = true;
						}
						else
						{
							return null;
						}
					}
				}
				else
				{
					ch = FetchIdentifierStart();
					if(ch == -1)
					{
						return null;
					}
				}
				builder.Append((char)ch);
				while((ch = FetchIdentifierPart()) != -1)
				{
					builder.Append((char)ch);
				}
				name = builder.ToString();
				if(!quoted)
				{
					keyword = keywords[name];
					if(keyword != null)
					{
						// Cannot use keywords as identifiers.
						return null;
					}
				}
				return name;
			}

	// Skip white space, except for line terminators.
	private void SkipWhite()
			{
				int ch;
				for(;;)
				{
					ch = Fetch();
					switch(ch)
					{
						case (int)'\t': case (int)' ': case (int)'\f':
						case (int)'\v': case (int)'\u00A0':
							break;

						case (int)'\u2028': case (int)'\u2029':
						{
							Unget(ch);
							return;
						}
						// Not reached.

						default:
						{
							if(ch < 0x0080)
							{
								Unget(ch);
								return;
							}
							else if(!Char.IsWhiteSpace((char)ch))
							{
								Unget(ch);
								return;
							}
						}
						break;
					}
				}
			}

	// Bail out with an error.
	private void BailOut(JSError error)
			{
				// Save the current state into the token.
				tokenInfo.token = JSToken.None;
				tokenInfo.endLine = line;
				tokenInfo.endLinePosition = lineStart;
				tokenInfo.endPosition = posn;

				// Throw an exception to let the parser know about the error.
				throw new ScannerFailure(error);
			}

	// Get the next token from the input stream.  This code is based
	// on the description in ECMA-262 under "Lexical Conventions",
	// with modifications for JScript compatibility.
	public void GetNextToken()
			{
				int ch, startLine, save;
				JSToken token = JSToken.None;
				String name;
				StringBuilder builder;
				Object keyword;

				// Initialize the scanning state.
				stringLiteral = null;
				identifierName = null;
				startLine = line;

				// Keep looping until we have a useful token.
				do
				{
					// Skip white space prior to the next token.
					SkipWhite();

					// Update the token start information.
					tokenInfo.startPosition = posn;
					tokenInfo.startLine = line;
					tokenInfo.startLinePosition = lineStart;

					// Determine what to do based on the next character.
					ch = Fetch();
					switch(ch)
					{
						case -1:
						{
							// We have reached the end of the input stream.
							token = JSToken.EndOfFile;
						}
						break;

						case (int)'/':
						{
							// May be a comment start, division, or regex.
							ch = Peek();
							if(ch == (int)'/')
							{
								// Single-line comment.
								SkipSingleLineComment();
								if(authoringMode)
								{
									token = JSToken.Comment;
								}
							}
							else if(ch == (int)'*')
							{
								// Multi-line comment.
								SkipMultiLineComment();
								if(authoringMode)
								{
									return;
								}
							}
							else if(ch == (int)'=')
							{
								// Division assignment operator.
								++posn;
								token = JSToken.DivideAssign;
							}
							else
							{
								// Ordinary division operator.
								token = JSToken.Divide;
							}
						}
						break;

						case '#':
						{
							// Special case: ignore lines that start with "#!".
							// These may be present in stand-alone scripts in a
							// Unix shell environment.  Strictly speaking, this
							// isn't part of the spec, but it can be useful.
							if(Peek() == '!')
							{
								SkipSingleLineComment();
								if(authoringMode)
								{
									token = JSToken.Comment;
								}
							}
							else
							{
								BailOut(JSError.IllegalChar);
							}
						}
						break;

						case (int)'\r':
						case (int)'\n':
						case (int)'\u2028':
						case (int)'\u2029':
						{
							// Line terminator.
							if(ch == (int)'\r' && Peek() == (int)'\n')
							{
								// Eat LF's following CR's.
								++posn;
							}
							++line;
							lineStart = posn;
						}
						break;

						case '{':	token = JSToken.LeftCurly; break;
						case '}':	token = JSToken.RightCurly; break;
						case '[':	token = JSToken.LeftBracket; break;
						case ']':	token = JSToken.RightBracket; break;
						case '(':	token = JSToken.LeftParen; break;
						case ')':	token = JSToken.RightParen; break;
						case ';':	token = JSToken.Semicolon; break;
						case ',':	token = JSToken.Comma; break;
						case '?':	token = JSToken.ConditionalIf; break;
						case '~':	token = JSToken.BitwiseNot; break;

						case ':':
						{
							// "::" or ":".
							if(Peek() == ':')
							{
								token = JSToken.DoubleColon;
								++posn;
							}
							else
							{
								token = JSToken.Colon;
							}
						}
						break;

						case '+':
						{
							// "++", "+=", or "+".
							ch = Peek();
							if(ch == '+')
							{
								token = JSToken.Increment;
								++posn;
							}
							else if(ch == '=')
							{
								token = JSToken.PlusAssign;
								++posn;
							}
							else
							{
								token = JSToken.Plus;
							}
						}
						break;

						case '-':
						{
							// "--", "-=", or "-".
							ch = Peek();
							if(ch == '-')
							{
								token = JSToken.Decrement;
								++posn;
							}
							else if(ch == '=')
							{
								token = JSToken.MinusAssign;
								++posn;
							}
							else
							{
								token = JSToken.Minus;
							}
						}
						break;

						case '*':
						{
							// "*=", or "*".
							if(Peek() == '=')
							{
								token = JSToken.MultiplyAssign;
								++posn;
							}
							else
							{
								token = JSToken.Multiply;
							}
						}
						break;

						case '%':
						{
							// "%=", or "%".
							if(Peek() == '=')
							{
								token = JSToken.ModuloAssign;
								++posn;
							}
							else
							{
								token = JSToken.Modulo;
							}
						}
						break;

						case '&':
						{
							// "&&", "&=", or "&".
							ch = Peek();
							if(ch == '&')
							{
								token = JSToken.LogicalAnd;
								++posn;
							}
							else if(ch == '=')
							{
								token = JSToken.BitwiseAndAssign;
								++posn;
							}
							else
							{
								token = JSToken.BitwiseAnd;
							}
						}
						break;

						case '|':
						{
							// "||", "|=", or "|".
							ch = Peek();
							if(ch == '|')
							{
								token = JSToken.LogicalOr;
								++posn;
							}
							else if(ch == '=')
							{
								token = JSToken.BitwiseOrAssign;
								++posn;
							}
							else
							{
								token = JSToken.BitwiseOr;
							}
						}
						break;

						case '^':
						{
							// "^=", or "^".
							if(Peek() == '=')
							{
								token = JSToken.BitwiseXorAssign;
								++posn;
							}
							else
							{
								token = JSToken.BitwiseXor;
							}
						}
						break;

						case '<':
						{
							// "<<", "<<=", "<=", or "<".
							ch = Peek();
							if(ch == '<')
							{
								Fetch();
								ch = Peek();
								if(ch == '=')
								{
									token = JSToken.LeftShiftAssign;
									++posn;
								}
								else
								{
									token = JSToken.LeftShift;
								}
							}
							else if(ch == '=')
							{
								token = JSToken.LessThanEqual;
								++posn;
							}
							else
							{
								token = JSToken.LessThan;
							}
						}
						break;

						case '>':
						{
							// ">>", ">>=", ">>>", ">>>=", ">=", or ">".
							ch = Peek();
							if(ch == '>')
							{
								Fetch();
								ch = Peek();
								if(ch == '=')
								{
									token = JSToken.RightShiftAssign;
									++posn;
								}
								else if(ch == '>')
								{
									Fetch();
									if(Peek() == '=')
									{
										token =
											JSToken.UnsignedRightShiftAssign;
										++posn;
									}
									else
									{
										token = JSToken.UnsignedRightShift;
									}
								}
								else
								{
									token = JSToken.RightShift;
								}
							}
							else if(ch == '=')
							{
								token = JSToken.GreaterThanEqual;
								++posn;
							}
							else
							{
								token = JSToken.GreaterThan;
							}
						}
						break;

						case '=':
						{
							// "==", "====", or "=".
							ch = Peek();
							if(ch == '=')
							{
								Fetch();
								ch = Peek();
								if(ch == '=')
								{
									token = JSToken.StrictEqual;
									++posn;
								}
								else
								{
									token = JSToken.Equal;
								}
							}
							else
							{
								token = JSToken.Assign;
							}
						}
						break;

						case '!':
						{
							// "!=", "!==", or "!".
							if(Peek() == '=')
							{
								Fetch();
								if(Peek() == '=')
								{
									token = JSToken.StrictNotEqual;
									++posn;
								}
								else
								{
									token = JSToken.NotEqual;
								}
							}
							else
							{
								token = JSToken.LogicalNot;
							}
						}
						break;

						case '.':
						{
							// "." or the start of a floating-point value.
							ch = Peek();
							if(ch >= '0' && ch <= '9')
							{
								ParseDecimal(true);
								token = JSToken.NumericLiteral;
							}
							else
							{
								token = JSToken.AccessField;
							}
						}
						break;

						case '0': case '1': case '2': case '3':
						case '4': case '5': case '6': case '7':
						case '8': case '9':
						{
							if(ch == '0' && (Peek() == 'x' || Peek() == 'X'))
							{
								// Hexadecimal numeric literal.
								Fetch();
								if(!ParseHex())
								{
									BailOut(JSError.BadHexDigit);
								}
								token = JSToken.IntegerLiteral;
							}
							else
							{
								// Decimal numeric literal.
								if(ParseDecimal(false))
								{
									token = JSToken.NumericLiteral;
								}
								else
								{
									token = JSToken.IntegerLiteral;
								}
							}
						}
						break;

						case '"': case '\'':
						{
							// Parse a string literal.
							ParseStringLiteral(ch);
							token = JSToken.StringLiteral;
						}
						break;

						case '\\':
						{
							// May be the start of an identifier, or it
							// may be a quoted keyword.
							save = posn;
							--posn;
							if((ch = FetchIdentifierStart()) != -1)
							{
								// Definitely an identifier.
								builder = new StringBuilder();
								builder.Append((char)ch);
								while((ch = FetchIdentifierPart()) != -1)
								{
									builder.Append((char)ch);
								}
								identifierName = builder.ToString();
								token = JSToken.Identifier;
							}
							else
							{
								// Check to see if this is a quoted keyword.
								posn = save;
								builder = new StringBuilder();
								while((ch = FetchIdentifierPart()) != -1)
								{
									builder.Append((char)ch);
								}
								name = builder.ToString();
								keyword = keywords[name];
								if(keyword != null)
								{
									identifierName = name;
									token = JSToken.Identifier;
								}
								else
								{
									// Illegal '\' character.
									posn = save;
									BailOut(JSError.IllegalChar);
								}
							}
						}
						break;

						default:
						{
							if(IsIdentifierStart(ch))
							{
								// Parse an identifier from the input stream.
								builder = new StringBuilder();
								builder.Append((char)ch);
								while((ch = FetchIdentifierPart()) != -1)
								{
									builder.Append((char)ch);
								}

								// Resolve keyword identifiers.
								name = builder.ToString();
								keyword = keywords[name];
								if(keyword != null)
								{
									token = (JSToken)keyword;
								}
								else
								{
									identifierName = name;
									token = JSToken.Identifier;
								}
							}
							else
							{
								// Don't know what to do with this character.
								BailOut(JSError.IllegalChar);
							}
						}
						break;
					}
				}
				while(token == JSToken.None);

				// Update the token information.
				tokenInfo.token = token;
				tokenInfo.endLine = line;
				tokenInfo.endLinePosition = lineStart;
				tokenInfo.endPosition = posn;

				// Set the "sawEOL" flag appropriately.
				if(line > startLine)
				{
					if(token == JSToken.StringLiteral &&
					   tokenInfo.startLine == startLine)
					{
						// String literal that crosses a line boundary.
						sawEOL = false;
					}
					else
					{
						// We processed an end of line while scanning.
						sawEOL = true;
					}
				}
				else if(token == JSToken.EndOfFile)
				{
					// The end of the file is always a line ending.
					sawEOL = true;
				}
				else
				{
					sawEOL = false;
				}
			}

	// Get the position of the start of the current ling.
	public int GetStartLinePosition()
			{
				return lineStart;
			}

	// Get the string literal that was just parsed.
	public String GetStringLiteral()
			{
				return stringLiteral;
			}

	// Get the source code that is being scanned from the context.
	public String GetSourceCode()
			{
				return source;
			}

	// Determine if we got the end of line sequence before the current token.
	// This is used to determine where to do automatic semi-colon insertion.
	public bool GotEndOfLine()
			{
				return sawEOL;
			}

	// Set the authoring mode.
	public void SetAuthoringMode(bool mode)
			{
				authoringMode = mode;
			}

	// Set the source context to be scanned.
	public void SetSource(Context sourceContext)
			{
				tokenInfo = sourceContext;
				authoringMode = false;
				sawEOL = false;
				posn = tokenInfo.startPosition;
				end = tokenInfo.endPosition;
				line = tokenInfo.startLine;
				lineStart = tokenInfo.startLinePosition;
				source = tokenInfo.source;
				stringLiteral = null;
				identifierName = null;
			}

	// Skip a single-line comment.
	private void SkipSingleLineComment()
			{
				int ch;
				for(;;)
				{
					ch = Fetch();
					if(ch == -1 || ch == '\r' || ch == '\n' ||
					   ch == '\u2028' || ch == '\u2029')
					{
						if(ch == '\r' && Peek() == '\n')
						{
							++posn;
						}
						if(authoringMode)
						{
							// Bail out with a comment token if authoring.
							tokenInfo.token = JSToken.Comment;
							tokenInfo.endLine = line;
							tokenInfo.endLinePosition = lineStart;
							tokenInfo.endPosition = posn;
							sawEOL = true;
						}
						++line;
						lineStart = posn;
						break;
					}
				}
			}

	// Skip a multi-line comment.  Returns the new source position.
	public int SkipMultiLineComment()
			{
				int ch;
				for(;;)
				{
					ch = Fetch();
					if(ch == -1)
					{
						// We got EOF in the middle of a multi-line comment.
						tokenInfo.token = JSToken.UnterminatedComment;
						tokenInfo.endLine = line;
						tokenInfo.endLinePosition = lineStart;
						tokenInfo.endPosition = posn;
						if(authoringMode)
						{
							sawEOL = true;
							return posn;
						}
						else
						{
							throw new ScannerFailure(JSError.NoCommentEnd);
						}
					}
					else if(ch == '*' && Peek() == '/')
					{
						// This is the end of the multi-line comment.
						++posn;
						if(authoringMode)
						{
							tokenInfo.token = JSToken.Comment;
							tokenInfo.endLine = line;
							tokenInfo.endLinePosition = lineStart;
							tokenInfo.endPosition = posn;
							sawEOL = true;
						}
						return posn;
					}
					else if(ch == '\r' || ch == '\n' ||
					   	    ch == '\u2028' || ch == '\u2029')
					{
						if(ch == '\r' && Peek() == '\n')
						{
							++posn;
						}
						++line;
						lineStart = posn;
					}
				}
			}

	// Parse a regular expression from the input stream.  The entire
	// expression, including the leading and trailing '/'s and the flags
	// is returned in the "regex" value.  Returns false if not a regex.
	internal bool ParseRegex(out String regex)
			{
				int ch;
				int start = posn;
				int regexStart;

				// Initialize the return value, in case we bail out early.
				regex = null;

				// Back up in the stream to recover the "/" or "/=" characters.
				if(tokenInfo.token == JSToken.Divide)
				{
					regexStart = posn - 1;
				}
				else if(tokenInfo.token == JSToken.DivideAssign)
				{
					regexStart = posn - 2;
				}
				else
				{
					return false;
				}

				// Parse the main part of the regex.
				for(;;)
				{
					ch = Fetch();
					if(ch == '/')
					{
						break;
					}
					else if(ch == -1 || ch == '\r' || ch == '\n' ||
							ch == '\u2028' || ch == '\u2029')
					{
						// Could not find the terminator, so not a regex.
						posn = start;
						return false;
					}
					else if(ch == '\\')
					{
						ch = Peek();
						if(ch == -1 || ch == '\r' || ch == '\n' ||
						   ch == '\u2028' || ch == '\u2029')
						{
							// Invalid escape sequence, so not a regex.
							posn = start;
							return false;
						}
						++posn;
					}
				}

				// Parse the flags.
				while((ch = FetchIdentifierPart()) != -1)
				{
					// Keep fetching flags until we get something else.
				}

				// Finished parsing the regex.
				regex = source.Substring(regexStart, posn - regexStart);
				return true;
			}

	// Determine if a character is a hexadecimal digit.
	internal static bool IsHexDigit(int ch)
			{
				if(ch >= '0' && ch <= '9')
				{
					return true;
				}
				else if(ch >= 'A' && ch <= 'F')
				{
					return true;
				}
				else if(ch >= 'a' && ch <= 'f')
				{
					return true;
				}
				else
				{
					return false;
				}
			}

	// Convert a hexadecimal digit into a value from 0 to 15.
	internal static int FromHex(int ch)
			{
				if(ch >= '0' && ch <= '9')
				{
					return ch - '0';
				}
				else if(ch >= 'A' && ch <= 'F')
				{
					return ch - 'A' + 10;
				}
				else
				{
					return ch - 'a' + 10;
				}
			}

	// Parse a hexadecimal integer value.  Returns false if invalid.
	private bool ParseHex()
			{
				int ch = Fetch();
				if(!IsHexDigit(ch))
				{
					Unget(ch);
					return false;
				}
				do
				{
					ch = Fetch();
				}
				while(IsHexDigit(ch));
				Unget(ch);
				return true;
			}

	// Parse a decimal numeric value.  Returns true if floating point.
	private bool ParseDecimal(bool startsWithDot)
			{
				int ch;

				// Process the part before the decimal point.
				if(!startsWithDot)
				{
					ch = Fetch();
					while(ch >= '0' && ch <= '9')
					{
						ch = Fetch();
					}
					if(ch == '.')
					{
						// Is this part of a floating point value,
						// or an integer followed by a field accessor?
						ch = Peek();
						if(ch < '0' || ch > '9')
						{
							Unget('.');
							return false;
						}
					}
					else if(ch != 'e' && ch != 'E')
					{
						// Simple integer literal.
						Unget(ch);
						return false;
					}
					else
					{
						// Exponent value follows.
						Unget(ch);
					}
				}

				// Process the part after the decimal point.
				ch = Fetch();
				while(ch >= '0' && ch <= '9')
				{
					ch = Fetch();
				}
				if(ch == 'e' || ch == 'E')
				{
					// Process the exponent.
					ch = Fetch();
					if(ch == '-' || ch == '+')
					{
						ch = Fetch();
					}
					while(ch >= '0' && ch <= '9')
					{
						ch = Fetch();
					}
				}
				Unget(ch);

				// This is definitely a floating-point value.
				return true;
			}

	// Parse a string literal.
	private void ParseStringLiteral(int quote)
			{
				StringBuilder builder = new StringBuilder();
				int ch, num;
				for(;;)
				{
					ch = Fetch();
					if(ch == quote)
					{
						// This is the end of the string literal.
						break;
					}
					else if(ch == -1 || ch == '\r' || ch == '\n' ||
					        ch == '\u2028' || ch == '\u2029')
					{
						// Unexpected end of line in a string literal.
						Unget(ch);
						BailOut(JSError.UnterminatedString);
					}
					else if(ch == '\\')
					{
						// Process an escape sequence.
						ch = Fetch();
						switch(ch)
						{
							case -1:
							{
								BailOut(JSError.UnterminatedString);
							}
							break;

							case '\r': case '\n': case '\u2028': case '\u2029':
							{
								// This is a line continuation.
								if(ch == '\r' && Peek() == '\n')
								{
									++posn;
								}
								++line;
								lineStart = posn;
							}
							break;

							case '0': case '1': case '2': case '3':
							case '4': case '5': case '6': case '7':
							{
								// May be either "\0", octal-char, or "\c".
								if(ch == '0' &&
								   (Peek() < '0' || Peek() > '9'))
								{
									builder.Append('\u0000');
								}
								else if(Peek() >= '0' && Peek() <= '7' &&
										Peek2() >= '0' && Peek2() <= '7')
								{
									ch = (ch - '0') * 64 +
									     (Peek() - '0') * 8 +
										 (Peek2() - '0');
									builder.Append((char)ch);
									posn += 2;
								}
								else
								{
									builder.Append((char)ch);
								}
							}
							break;

							case 'x':
							{
								// 8-bit hexadecimal value.
								ch = Fetch();
								if(!IsHexDigit(ch))
								{
									BailOut(JSError.BadHexDigit);
								}
								num = FromHex(ch) << 4;
								ch = Fetch();
								if(!IsHexDigit(ch))
								{
									BailOut(JSError.BadHexDigit);
								}
								num |= FromHex(ch);
								builder.Append((char)num);
							}
							break;

							case 'u':
							{
								// 16-bit hexadecimal value.
								ch = Fetch();
								if(!IsHexDigit(ch))
								{
									BailOut(JSError.BadHexDigit);
								}
								num = FromHex(ch) << 12;
								ch = Fetch();
								if(!IsHexDigit(ch))
								{
									BailOut(JSError.BadHexDigit);
								}
								num |= FromHex(ch) << 8;
								ch = Fetch();
								if(!IsHexDigit(ch))
								{
									BailOut(JSError.BadHexDigit);
								}
								num |= FromHex(ch) << 4;
								ch = Fetch();
								if(!IsHexDigit(ch))
								{
									BailOut(JSError.BadHexDigit);
								}
								num |= FromHex(ch);
								builder.Append((char)num);
							}
							break;

							case 'b':	builder.Append('\u0008'); break;
							case 't':	builder.Append('\u0009'); break;
							case 'n':	builder.Append('\u000A'); break;
							case 'v':	builder.Append('\u000B'); break;
							case 'f':	builder.Append('\u000C'); break;
							case 'r':	builder.Append('\u000D'); break;

							default:
							{
								// Ordinary escaped character.
								builder.Append((char)ch);
							}
							break;
						}
					}
					else
					{
						// Ordinary character.
						builder.Append((char)ch);
					}
				}
				stringLiteral = builder.ToString();
			}

	// Determine if a token is an operator.
	public static bool IsOperator(JSToken token)
			{
				return (token >= JSToken.FirstOp &&
						token <= JSToken.LastOp);
			}

	// Determine if a token is a keyword.
	public static bool IsKeyword(JSToken token)
			{
				switch(token)
				{
					case JSToken.Break: case JSToken.Case:
					case JSToken.Catch: case JSToken.Continue:
					case JSToken.Default: case JSToken.Do:
					case JSToken.Else: case JSToken.Finally:
					case JSToken.For: case JSToken.Function:
					case JSToken.If: case JSToken.In:
					case JSToken.Instanceof: case JSToken.New:
					case JSToken.Return: case JSToken.Switch:
					case JSToken.This: case JSToken.Throw:
					case JSToken.Try: case JSToken.Typeof:
					case JSToken.Var: case JSToken.Void:
					case JSToken.While: case JSToken.With:
					case JSToken.Null: case JSToken.True:
					case JSToken.False: case JSToken.Abstract:
					case JSToken.Boolean: case JSToken.Byte:
					case JSToken.Char: case JSToken.Class:
					case JSToken.Const: case JSToken.Debugger:
					case JSToken.Double: case JSToken.Enum:
					case JSToken.Export: case JSToken.Extends:
					case JSToken.Final: case JSToken.Float:
					case JSToken.Goto: case JSToken.Implements:
					case JSToken.Import: case JSToken.Int:
					case JSToken.Interface: case JSToken.Long:
					case JSToken.Native: case JSToken.Package:
					case JSToken.Private: case JSToken.Protected:
					case JSToken.Public: case JSToken.Short:
					case JSToken.Static: case JSToken.Super:
					case JSToken.Synchronized: case JSToken.Throws:
					case JSToken.Transient: case JSToken.Volatile:
					case JSToken.Assert: case JSToken.Delete:
					case JSToken.Ensure: case JSToken.Event:
					case JSToken.Get: case JSToken.Invariant:
					case JSToken.Internal: case JSToken.Require:
					case JSToken.Set: case JSToken.Use:
					case JSToken.Sbyte: case JSToken.Ushort:
					case JSToken.Uint: case JSToken.Ulong:
						return true;

					default:
						return false;
				}
			}

	// Get the name of the just-parsed identifier.
	internal String GetIdentifierName()
			{
				return identifierName;
			}

	// Get the token context for the previous token that was scanned.
	internal Context GetTokenContext()
			{
				return tokenInfo;
			}

	// Keyword table.
	private static readonly Object[] keywordTable = {
				// Standard ECMAScript keywords.
				"break",		JSToken.Break,
				"case",			JSToken.Case,
				"catch",		JSToken.Catch,
				"continue",		JSToken.Continue,
				"default",		JSToken.Default,
				"do",			JSToken.Do,
				"else",			JSToken.Else,
				"false",		JSToken.False,
				"finally",		JSToken.Finally,
				"for",			JSToken.For,
				"function",		JSToken.Function,
				"if",			JSToken.If,
				"in",			JSToken.In,
				"instanceof",	JSToken.Instanceof,
				"new",			JSToken.New,
				"null",			JSToken.Null,
				"return",		JSToken.Return,
				"switch",		JSToken.Switch,
				"this",			JSToken.This,
				"throw",		JSToken.Throw,
				"true",			JSToken.True,
				"try",			JSToken.Try,
				"typeof",		JSToken.Typeof,
				"var",			JSToken.Var,
				"void",			JSToken.Void,
				"while",		JSToken.While,
				"with",			JSToken.With,

				// ECMAScript "future reserved words".
				"abstract",		JSToken.Abstract,
				"boolean",		JSToken.Boolean,
				"byte",			JSToken.Byte,
				"char",			JSToken.Char,
				"class",		JSToken.Class,
				"const",		JSToken.Const,
				"debugger",		JSToken.Debugger,
				"double",		JSToken.Double,
				"enum",			JSToken.Enum,
				"export",		JSToken.Export,
				"extends",		JSToken.Extends,
				"final",		JSToken.Final,
				"float",		JSToken.Float,
				"goto",			JSToken.Goto,
				"implements",	JSToken.Implements,
				"import",		JSToken.Import,
				"int",			JSToken.Int,
				"interface",	JSToken.Interface,
				"long",			JSToken.Long,
				"native",		JSToken.Native,
				"package",		JSToken.Package,
				"private",		JSToken.Private,
				"protected",	JSToken.Protected,
				"public",		JSToken.Public,
				"short",		JSToken.Short,
				"static",		JSToken.Static,
				"super",		JSToken.Super,
				"synchronized",	JSToken.Synchronized,
				"throws",		JSToken.Throws,
				"transient",	JSToken.Transient,
				"volatile",		JSToken.Volatile,

				// JScript-specific reserved words.
				"assert",		JSToken.Assert,
				"delete",		JSToken.Delete,
				"ensure",		JSToken.Ensure,
				"event",		JSToken.Event,
				"get",			JSToken.Get,
				"invariant",	JSToken.Invariant,
				"internal",		JSToken.Internal,
				"require",		JSToken.Require,
				"sbyte",		JSToken.Sbyte,
				"set",			JSToken.Set,
				"uint",			JSToken.Uint,
				"ulong",		JSToken.Ulong,
				"use",			JSToken.Use,
				"ushort",		JSToken.Ushort,
			};

	// Get the global keyword table.
	private static Hashtable GetKeywordTable()
			{
				lock(typeof(JSScanner))
				{
					if(keywordsGlobal != null)
					{
						return keywordsGlobal;
					}
					Hashtable table = keywordsGlobal = new Hashtable();
					int posn;
					for(posn = 0; posn < keywordTable.Length; posn += 2)
					{
						table[keywordTable[posn]] = keywordTable[posn + 1];
					}
					return keywordsGlobal;
				}
			}

	// Exception that is thrown from inside the scanner in exceptional
	// conditions (e.g. EOF inside a comment, invalid character).
	internal class ScannerFailure : Exception
	{
		// Accessible internal state.
		public JSError error;

		// Constructor.
		public ScannerFailure(JSError error)
				{
					this.error = error;
				}

	}; // class ScannerFailure

}; // class JSScanner

}; // namespace Microsoft.JScript
