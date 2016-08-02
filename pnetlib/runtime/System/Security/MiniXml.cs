/*
 * MiniXml.cs - Implementation of the "System.Security.MiniXml" class.
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

namespace System.Security
{

#if CONFIG_PERMISSIONS || CONFIG_POLICY_OBJECTS || CONFIG_REMOTING

using System;
using System.IO;

// This class is used by "SecurityElement" to implement a very
// simple XML parser, suitable for processing code access security tags.
// It isn't for general-purpose XML parsing (use System.Xml instead).
// We cannot use System.Xml directly due to library circularities.

internal sealed class MiniXml
{

	// Token types.
	private enum Token
	{
		EOF,
		StartTag,
		EndTag,
		SingletonTag,
		Text

	}; // enum Token

	// Internal state.
	private String input;
	private int posn;
	private Token token;
	private String value;
	private String args;

	// Constructors.
	public MiniXml(String input)
			{
				this.input = (input == null ? String.Empty : input);
				this.posn = 0;
			}
	public MiniXml(TextReader input)
			{
				this.input = input.ReadToEnd();
				this.posn = 0;
			}
	public MiniXml(Stream input)
			{
				StreamReader reader = new StreamReader(input);
				this.input = reader.ReadToEnd();
				this.posn = 0;
				((IDisposable)reader).Dispose();
			}

	// Read the next token from the input.
	private void NextToken()
			{
				char ch;
				int start;
				int level;
				args = String.Empty;
				for(;;)
				{
					// Skip white space prior to the next token.
					while(posn < input.Length && Char.IsWhiteSpace(input[posn]))
					{
						++posn;
					}
					if(posn >= input.Length)
					{
						token = Token.EOF;
						return;
					}

					// What kind of token is this?
					ch = input[posn];
					if(ch == '<')
					{
						// Some form of tag.
						if((posn + 3) < input.Length &&
						   input[posn + 1] == '!' &&
						   input[posn + 2] == '-' &&
						   input[posn + 3] == '-')
						{
							// Comment tag.
							start = input.IndexOf("-->", posn + 4);
							if(start != -1)
							{
								posn = start + 3;
							}
							else
							{
								posn = input.Length;
							}
						}
						else if((posn + 1) < input.Length &&
								input[posn + 1] == '/')
						{
							// End tag.
							posn += 2;
							start = posn;
							while(posn < input.Length &&
								  ((ch = input[posn]) != '>' &&
								   !Char.IsWhiteSpace(ch)))
							{
								++posn;
							}
							value = input.Substring(start, posn - start);
							while(posn < input.Length && input[posn] != '>')
							{
								++posn;
							}
							if(posn < input.Length)
							{
								++posn;
							}
							token = Token.EndTag;
							return;
						}
						else if((posn + 8) < input.Length &&
								input[posn + 1] == '!' &&
								input[posn + 2] == '[' &&
								input[posn + 3] == 'C' &&
								input[posn + 4] == 'D' &&
								input[posn + 5] == 'A' &&
								input[posn + 6] == 'T' &&
								input[posn + 7] == 'A' &&
								input[posn + 8] == '[')
						{
							// CDATA text block.
							start = input.IndexOf("]]>", posn + 9);
							if(start != -1)
							{
								value = input.Substring
									(posn + 9, start - (posn + 9));
								posn = start + 3;
							}
							else
							{
								value = input.Substring(posn + 9);
								posn = input.Length;
							}
							token = Token.Text;
							return;
						}
						else if((posn + 1) < input.Length &&
								(input[posn + 1] == '!' ||
								 input[posn + 1] == '?'))
						{
							// DTD or XML declaration.
							level = 1;
							++posn;
							while(posn < input.Length)
							{
								ch = input[posn++];
								if(ch == '>')
								{
									if(--level == 0)
									{
										break;
									}
								}
								else if(ch == '<')
								{
									++level;
								}
							}
						}
						else
						{
							// Start or singleton tag.
							++posn;
							start = posn;
							while(posn < input.Length &&
								  ((ch = input[posn]) != '>' && ch != '/' &&
								   !Char.IsWhiteSpace(ch)))
							{
								++posn;
							}
							value = input.Substring(start, posn - start);
							start = posn;
							while(posn < input.Length && input[posn] != '>')
							{
								++posn;
							}
							if(input[posn - 1] == '/')
							{
								args = input.Substring(start, posn - start - 1);
								token = Token.SingletonTag;
							}
							else
							{
								args = input.Substring(start, posn - start);
								token = Token.StartTag;
							}
							if(posn < input.Length)
							{
								++posn;
							}
							return;
						}
					}
					else if(ch == '&')
					{
						// Ampersand-escaped character.
						start = posn;
						++posn;
						while(posn < input.Length && input[posn] != ';')
						{
							++posn;
						}
						if(posn < input.Length)
						{
							++posn;
						}
						value = input.Substring(start, posn - start);
						if(value == "&lt;")
						{
							value = "<";
						}
						else if(value == "&gt;")
						{
							value = ">";
						}
						else if(value == "&amp;")
						{
							value = "&";
						}
						else if(value == "&quot;")
						{
							value = "\"";
						}
						else if(value == "&apos;")
						{
							value = "'";
						}
						else
						{
							value = " ";
						}
						token = Token.Text;
						return;
					}
					else
					{
						// Start of an ordinary text run.
						start = posn;
						++posn;
						while(posn < input.Length &&
							  ((ch = input[posn]) != '<' && ch != '&'))
						{
							++posn;
						}
						while(posn > start &&
							  Char.IsWhiteSpace(input[posn - 1]))
						{
							--posn;
						}
						value = input.Substring(start, posn - start);
						token = Token.Text;
						return;
					}
				}
			}

	// Parse an element tag.
	private SecurityElement ParseElement()
			{
				// Create the new element.
				SecurityElement element;
				element = new SecurityElement(value);

				// Parse and add the attribute arguments.
				int temp = 0;
				int start;
				String name;
				String avalue;
				for(;;)
				{
					while(temp < args.Length && Char.IsWhiteSpace(args[temp]))
					{
						++temp;
					}
					if(temp >= args.Length)
					{
						break;
					}
					start = temp;
					while(temp < args.Length && args[temp] != '=')
					{
						++temp;
					}
					name = args.Substring(start, temp - start);
					if(temp < args.Length)
					{
						++temp;
					}
					if(temp < args.Length && args[temp] == '"')
					{
						++temp;
						start = temp;
						while(temp < args.Length && args[temp] != '"')
						{
							++temp;
						}
						avalue = args.Substring(start, temp - start);
						if(temp < args.Length)
						{
							++temp;
						}
					}
					else if(temp < args.Length && args[temp] == '\'')
					{
						++temp;
						start = temp;
						while(temp < args.Length && args[temp] != '\'')
						{
							++temp;
						}
						avalue = args.Substring(start, temp - start);
						if(temp < args.Length)
						{
							++temp;
						}
					}
					else
					{
						avalue = String.Empty;
					}
					element.AddAttribute(name, avalue);
				}

				// Parse the children of this element.
				if(token == Token.SingletonTag)
				{
					NextToken();
				}
				else
				{
					NextToken();
					while(token != Token.EOF && token != Token.EndTag)
					{
						if(token == Token.StartTag ||
						   token == Token.SingletonTag)
						{
							SecurityElement child;
							child = ParseElement();
							element.AddChild(child);
						}
						else if(token == Token.Text)
						{
							String prevText = element.Text;
							if(prevText != null)
							{
								element.Text = prevText + value;
							}
							else
							{
								element.Text = value;
							}
						}
						NextToken();
					}
				}

				// Return the final element to the caller.
				return element;
			}

	// Parse the input data.
	public SecurityElement Parse()
			{
				// Skip until we find a start or singleton token.
				do
				{
					NextToken();
				}
				while(token != Token.EOF &&
					  token != Token.StartTag &&
					  token != Token.SingletonTag);
				if(token == Token.EOF)
				{
					return null;
				}

				// Parse the element.
				return ParseElement();
			}

	// Load the contents of an XML file.
	public static SecurityElement Load(String filename)
			{
				try
				{
					StreamReader reader = new StreamReader(filename);
					SecurityElement e = (new MiniXml(reader)).Parse();
					reader.Close();
					return e;
				}
				catch(Exception)
				{
					return null;
				}
			}

}; // class MiniXml

#endif // CONFIG_PERMISSIONS || CONFIG_POLICY_OBJECTS || CONFIG_REMOTING

}; // namespace System.Security
