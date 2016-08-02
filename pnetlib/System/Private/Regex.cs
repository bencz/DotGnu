/*
 * Regex.cs - Implementation of the "System.Private.Regex" class.
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

namespace System.Private
{

using System;
using System.Text;
using Platform;

internal sealed class Regex : IDisposable
{
	// Internal state.
	private IntPtr handle;
	private RegexSyntax syntax;

	// Constructors.
	public Regex(String pattern) : this(pattern, RegexSyntax.PosixBasic) {}
	public Regex(String pattern, RegexSyntax syntax)
			{
				if(pattern == null)
				{
					throw new ArgumentNullException("pattern");
				}
				this.syntax = syntax;
				if((syntax & RegexSyntax.IgnoreCase) != 0)
				{
					pattern = pattern.ToLower();
				}
				if((syntax & RegexSyntax.Wildcard) != 0)
				{
					pattern = FileSystemToPosix(pattern);
					syntax = RegexSyntax.PosixExtended;
				}
				syntax &= (RegexSyntax.All &
						   ~(RegexSyntax.Debug | RegexSyntax.IgnoreCase));
				lock(typeof(Regex))
				{
					// We lock down "Regex" while we do this because the
					// syntax setting in the GNU regex routines is not
					// thread-safe without it.
					handle = RegexpMethods.CompileWithSyntaxInternal
								(pattern, (int)syntax);
				}
				if(handle == IntPtr.Zero)
				{
					throw new ArgumentException(S._("Arg_InvalidRegex"));
				}
			}

	// Destructor.
	~Regex()
			{
				Dispose();
			}

	// Implement the IDisposable interface.
	public void Dispose()
			{
				lock(this)
				{
					if(handle != IntPtr.Zero)
					{
						RegexpMethods.FreeInternal(handle);
						handle = IntPtr.Zero;
					}
				}
			}

	// Determine if a string matches this regular expression.
	public bool Match(String str)
			{
				if(str == null)
				{
					throw new ArgumentNullException("str");
				}
				if((syntax & RegexSyntax.IgnoreCase) != 0)
				{
					str = str.ToLower();
				}
				lock(this)
				{
					if(handle != IntPtr.Zero)
					{
						return (RegexpMethods.ExecInternal
									(handle, str, 0) == 0);
					}
					else
					{
						throw new ObjectDisposedException
							(S._("Exception_Disposed"));
					}
				}
			}
	public bool Match(String str, RegexMatchOptions options)
			{
				if(str == null)
				{
					throw new ArgumentNullException("str");
				}
				if((syntax & RegexSyntax.IgnoreCase) != 0)
				{
					str = str.ToLower();
				}
				lock(this)
				{
					if(handle != IntPtr.Zero)
					{
						options &= RegexMatchOptions.All;
						return (RegexpMethods.ExecInternal
									(handle, str, (int)options) == 0);
					}
					else
					{
						throw new ObjectDisposedException
							(S._("Exception_Disposed"));
					}
				}
			}

	// Determine if a string matches this regular expression and return
	// all of the matches in an array.  Returns null if no match.
	public RegexMatch[] Match(String str, RegexMatchOptions options,
						      int maxMatches)
			{
				if(str == null)
				{
					throw new ArgumentNullException("str");
				}
				else if(maxMatches < 0)
				{
					throw new ArgumentOutOfRangeException
						("maxMatches", "Must be non-negative");
				}
				if((syntax & RegexSyntax.IgnoreCase) != 0)
				{
					str = str.ToLower();
				}
				lock(this)
				{
					if(handle != IntPtr.Zero)
					{
						options &= RegexMatchOptions.All;
						return (RegexMatch[])RegexpMethods.MatchInternal
							(handle, str, maxMatches, (int)options,
							 typeof(RegexMatch));
					}
					else
					{
						throw new ObjectDisposedException
							(S._("Exception_Disposed"));
					}
				}
			}

	// Get the syntax options used by this regex object.
	public RegexSyntax Syntax
			{
				get
				{
					return syntax;
				}
			}

	// Determine if this regex object has been disposed.
	public bool IsDisposed
			{
				get
				{
					lock(this)
					{
						return (handle == IntPtr.Zero);
					}
				}
			}

	// Convert a filesystem wildcard into a Posix regex pattern.
	private static String FileSystemToPosix(String wildcard)
			{
				// Special case: match the empty string.
				if(wildcard == String.Empty)
				{
					return "^()$";
				}

				// Build the new regular expression.
				StringBuilder builder = new StringBuilder(wildcard.Length * 2);
				char ch;
				int posn = 0;
				int index, index2;
				builder.Append('^');
				while(posn < wildcard.Length)
				{
					ch = wildcard[posn++];
					switch(ch)
					{
						case '*':
						{
							if((posn + 1) < wildcard.Length &&
							   wildcard[posn] == '*' &&
							   (wildcard[posn + 1] == '/' ||
							    wildcard[posn + 1] == '\\'))
							{
								// "**/": match arbitrary directory prefixes.
								builder.Append("(.*[/\\]|())");
								posn += 2;
							}
							else
							{
								// Match a sequence of arbitrary characters.
								builder.Append('.');
								builder.Append('*');
							}
						}
						break;

						case '?':
						{
							// Match an arbitrary character.
							builder.Append('.');
						}
						break;

						case '[':
						{
							// Match a set of characters.
							index = wildcard.IndexOf(']', posn);
							index2 = wildcard.IndexOf('[', posn);
							if(index != -1 &&
							   (index2 == -1 || index2 > index))
							{
								// Output the set to the regex.
								builder.Append
									(wildcard.Substring
										(posn - 1, index - (posn - 1) + 1));
								posn = index + 1;
							}
							else
							{
								// Unmatched '[': treat as a literal character.
								builder.Append('\\');
								builder.Append(ch);
							}
						}
						break;

						case '.': case '^': case '$': case ']':
						case '(': case ')':
						{
							// Escape a special regex character.
							builder.Append('\\');
							builder.Append(ch);
						}
						break;

						case '/': case '\\':
						{
							// Match a directory separator, irrespective
							// of the type of operating system we are using.
							builder.Append('[');
							builder.Append('/');
							builder.Append('\\');
							builder.Append(']');
						}
						break;
						default:
						{
							builder.Append(ch);
						}
						break;
					}
				}
				builder.Append('$');
				return builder.ToString();
			}

}; // class Regex

}; // namespace System.Private
