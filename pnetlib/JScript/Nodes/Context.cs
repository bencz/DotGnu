/*
 * Context.cs - Context information for an AST node.
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

public class Context
{
	// Private state.
	internal CodeBase codebase;
	internal int startPosition;
	internal int endPosition;
	internal int startLine;
	internal int startLinePosition;
	internal int endLine;
	internal int endLinePosition;
	internal JSToken token;
	internal String source;

	// Constructors.
	internal Context(String source)
			{
				startPosition = 0;
				endPosition = source.Length;
				startLine = 1;
				startLinePosition = 0;
				endLine = 1;
				endLinePosition = 0;
				token = JSToken.None;
				this.source = source;
			}

	// Get the first column occupied by the construct.
	public int StartColumn
			{
				get
				{
					return startPosition - startLinePosition;
				}
			}

	// Get the first line occupied by the construct.
	public int StartLine
			{
				get
				{
					return startLine;
				}
			}

	// Get the construct's first position within the source string.
	public int StartPosition
			{
				get
				{
					return startPosition;
				}
			}

	// Get the last column occupied by the construct.
	public int EndColumn
			{
				get
				{
					return endPosition - endLinePosition;
				}
			}

	// Get the last line occupied by the construct.
	public int EndLine
			{
				get
				{
					return endLine;
				}
			}

	// Get the construct's last position within the source string.
	public int EndPosition
			{
				get
				{
					return endPosition;
				}
			}

	// Get the source code for the construct.
	public String GetCode()
			{
				if(startPosition < endPosition && endPosition <= source.Length)
				{
					return source.Substring
						(startPosition, endPosition - startPosition);
				}
				else
				{
					return null;
				}
			}

	// Get the token code for the construct.
	public JSToken GetToken()
			{
				return token;
			}

	// Make a copy of this context.
	internal Context MakeCopy()
			{
				return (Context)(MemberwiseClone());
			}

	// Build a new context object that covers a range of nodes.
	internal static Context BuildRange(Context start, Context end)
			{
				Context context = new Context(start.source);
				context.codebase = start.codebase;
				context.token = start.token;
				context.startPosition = start.startPosition;
				context.startLine = start.startLine;
				context.startLinePosition = start.startLinePosition;
				context.endLine = end.endLine;
				context.endLinePosition = end.endLinePosition;
				return context;
			}

}; // class Context

}; // namespace Microsoft.JScript
