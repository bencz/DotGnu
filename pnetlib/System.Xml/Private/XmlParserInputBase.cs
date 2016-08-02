/*
 * XmlParserInputBase.cs - Implementation of the
 *			"System.Xml.Private.XmlParserInputBase" class.
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
using System.Text;

internal abstract class XmlParserInputBase : XmlErrorProcessor
{
	// Internal state.
	private EOFHandler eofHandler;
	private XmlNameTable nameTable;
	protected LogManager logger;


	// Fields.
	public char currChar;
	public char peekChar;


	// Constructor.
	protected XmlParserInputBase
				(XmlNameTable nameTable,
				 EOFHandler eof,
				 ErrorHandler error)
			: base(error)
			{
				this.nameTable = nameTable;
				this.eofHandler = eof;
				this.currChar = '\0';
				this.peekChar = '\0';
				this.logger = new LogManager();
			}


	// Get or set the end of file handler.
	public virtual EOFHandler EOFHandler
			{
				get { return eofHandler; }
				set { eofHandler = value; }
			}

	// Get the current line number.
	public abstract int LineNumber { get; }

	// Get the current line position.
	public abstract int LinePosition { get; }

	// Get the logger.
	public virtual LogManager Logger
			{
				get { return logger; }
			}


	// Handle the end of file state.
	protected virtual void EOF()
			{
				if(eofHandler != null)
				{
					eofHandler();
				}
			}

	// Expect the next character to match `expected'.
	public virtual void Expect(char expected)
			{
				if(!NextChar()) { Error("Xml_UnexpectedEOF"); }
				if(currChar != expected) { Error(/* TODO */); }
			}
	// Expect the stream characters to match `expected'.
	public virtual void Expect(String expected)
			{
				int len = expected.Length;
				for(int i = 0; i < len; ++i)
				{
					if(!NextChar()) { Error("Xml_UnexpectedEOF"); }
					if(currChar != expected[i]) { Error(/* TODO */); }
				}
			}

	// Move to the next character, returning false on EOF.
	public abstract bool NextChar();

	// Peek at the next character, returning false on EOF.
	public abstract bool PeekChar();

	// Read a name, calling Error if there are no valid characters.
	public virtual String ReadName()
			{
				// push a new log
				Logger.Push(new StringBuilder());

				// require an initial name character
				if(!NextChar()) { Error("Xml_UnexpectedEOF"); }
				if(!XmlCharInfo.IsNameInit(currChar)) { Error(/* TODO */); }

				// read until we consume all the name characters
				while(PeekChar() && XmlCharInfo.IsNameChar(peekChar))
				{
					NextChar();
				}

				// get the result from the log and pop it from the logger
				String result = Logger.Pop().ToString();

				// return our result
				return nameTable.Add(result);
			}

	// Read a name token, calling Error if there are no valid characters.
	public virtual String ReadNameToken()
			{
				// push a new log
				Logger.Push(new StringBuilder());

				// require at least one name character
				if(!NextChar()) { Error("Xml_UnexpectedEOF"); }
				if(!XmlCharInfo.IsNameChar(currChar)) { Error(/* TODO */); }

				// read until we consume all the name characters
				while(PeekChar() && XmlCharInfo.IsNameChar(peekChar))
				{
					NextChar();
				}

				// get the result from the log and pop it from the logger
				String result = Logger.Pop().ToString();

				// return our result
				return nameTable.Add(result);
			}

	// Read characters until target, calling Error on EOF.
	public virtual String ReadTo(char target)
			{
				return ReadTo(target, false);
			}
	public virtual String ReadTo(char target, bool includeTarget)
			{
				// push a new log
				Logger.Push(new StringBuilder());

				// read until we hit the target
				while(PeekChar() && peekChar != target) { NextChar(); }

				// if we didn't hit the target we hit eof, so give an error
				if(peekChar != target)
				{
					Logger.Pop();
					Error("Xml_UnexpectedEOF");
				}

				// check if we should include the target, and do so if need be
				if(includeTarget) { NextChar(); }

				// get the result from the log and pop it from the logger
				String result = Logger.Pop().ToString();

				// return our result
				return result;
			}

	// Skip whitespace characters, returning false if nothing is skipped.
	public virtual bool SkipWhitespace()
			{
				bool skipped = false;
				while(PeekChar() && XmlCharInfo.IsWhitespace(peekChar))
				{
					NextChar();
					skipped = true;
				}
				return skipped;
			}


	public class LogManager
	{
		// Internal state.
		private int top;
		private StringBuilder[] logs;

		// Constructor.
		public LogManager()
				{
					top = -1;
					logs = new StringBuilder[2];
				}

		// Append the input to the logs.
		public void Append(char c)
				{
					for(int i = 0; i <= top; ++i)
					{
						logs[i].Append(c);
					}
				}
		public void Append(String s)
				{
					for(int i = 0; i <= top; ++i)
					{
						logs[i].Append(s);
					}
				}

		// Clear the log stack.
		public void Clear()
				{
					while(top >= 0)
					{
						logs[top--] = null;
					}
				}

		// Push a log onto the log stack.
		public void Push(StringBuilder sb)
				{
					if(sb == null) { return; }
					++top;
					if(top == logs.Length)
					{
						StringBuilder[] tmp = new StringBuilder[top*2];
						Array.Copy(logs, 0, tmp, 0, top);
						logs = tmp;
					}
					logs[top] = sb;
				}

		// Pop a log off the log stack.
		public StringBuilder Pop()
				{
					StringBuilder tmp = logs[top];
					logs[top--] = null;
					return tmp;
				}

		// Peek at the log on top of the log stack.
		public StringBuilder Peek()
				{
					return logs[top];
				}

	}; // class LogManager

}; // class XmlParserInputBase

}; // namespace System.Xml.Private
