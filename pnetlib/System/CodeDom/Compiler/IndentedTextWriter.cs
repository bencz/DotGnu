/*
 * IndentedTextWriter.cs - Implementation of the
 *		System.CodeDom.Compiler.IndentedTextWriter class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.CodeDom.Compiler
{

#if CONFIG_CODEDOM

using System.IO;
using System.Text;

public class IndentedTextWriter : TextWriter
{
	// Internal state.
	private TextWriter writer;
	private String tabString;
	private int indent;
	private bool atLineStart;

	// The default string to use for tabbing.
	public const String DefaultTabString = "    ";

	// Constructors.
	public IndentedTextWriter(TextWriter writer)
			: this(writer, DefaultTabString)
			{
				// Nothing to do here.
			}
	public IndentedTextWriter(TextWriter writer, String tabString)
			{
				this.writer = writer;
				this.tabString = tabString;
				this.indent = 0;
				this.atLineStart = true;
			}

	// Get the encoding in use by the underlying text writer.
	public override Encoding Encoding
			{
				get
				{
					return writer.Encoding;
				}
			}

	// Get or set the current indent level.
	public int Indent
			{
				get
				{
					return indent;
				}
				set
				{
					if(value >= 0)
					{
						indent = value;
					}
					else
					{
						indent = 0;
					}
				}
			}

	// Get the writer underlying this object.
	public TextWriter InnerWriter
			{
				get
				{
					return writer;
				}
			}

	// Get or set the newline string.
	public override String NewLine
			{
				get
				{
					return writer.NewLine;
				}
				set
				{
					writer.NewLine = value;
				}
			}

	// Close this writer.
	public override void Close()
			{
				writer.Close();
			}

	// Flush this writer.
	public override void Flush()
			{
				writer.Flush();
			}

	// Output tabs at the start of a line if necessary.
	protected virtual void OutputTabs()
			{
				if(atLineStart)
				{
					int level = indent;
					while(level > 0)
					{
						writer.Write(tabString);
						--level;
					}
					atLineStart = false;
				}
			}

	// Write values of various types.
	public override void Write(bool value)
			{
				OutputTabs();
				writer.Write(value);
			}
	public override void Write(char value)
			{
				OutputTabs();
				writer.Write(value);
			}
	public override void Write(char[] value)
			{
				OutputTabs();
				writer.Write(value);
			}
	public override void Write(double value)
			{
				OutputTabs();
				writer.Write(value);
			}
	public override void Write(int value)
			{
				OutputTabs();
				writer.Write(value);
			}
	public override void Write(long value)
			{
				OutputTabs();
				writer.Write(value);
			}
	public override void Write(Object value)
			{
				OutputTabs();
				writer.Write(value);
			}
	public override void Write(float value)
			{
				OutputTabs();
				writer.Write(value);
			}
	public override void Write(String value)
			{
				OutputTabs();
				writer.Write(value);
			}
	public override void Write(String value, Object arg0)
			{
				OutputTabs();
				writer.Write(value, arg0);
			}
	public override void Write(String value, Object arg0, Object arg1)
			{
				OutputTabs();
				writer.Write(value, arg0, arg1);
			}
	public override void Write(String value, params Object[] args)
			{
				OutputTabs();
				writer.Write(value, args);
			}
	public override void Write(char[] value, int index, int count)
			{
				OutputTabs();
				writer.Write(value, index, count);
			}

	// Write values of various types followed by a newline.
	public override void WriteLine()
			{
				OutputTabs();
				writer.WriteLine();
				atLineStart = true;
			}
	public override void WriteLine(bool value)
			{
				OutputTabs();
				writer.WriteLine(value);
				atLineStart = true;
			}
	public override void WriteLine(char value)
			{
				OutputTabs();
				writer.WriteLine(value);
				atLineStart = true;
			}
	public override void WriteLine(char[] value)
			{
				OutputTabs();
				writer.WriteLine(value);
				atLineStart = true;
			}
	public override void WriteLine(double value)
			{
				OutputTabs();
				writer.WriteLine(value);
				atLineStart = true;
			}
	public override void WriteLine(int value)
			{
				OutputTabs();
				writer.WriteLine(value);
				atLineStart = true;
			}
	public override void WriteLine(long value)
			{
				OutputTabs();
				writer.WriteLine(value);
				atLineStart = true;
			}
	public override void WriteLine(Object value)
			{
				OutputTabs();
				writer.WriteLine(value);
				atLineStart = true;
			}
	public override void WriteLine(float value)
			{
				OutputTabs();
				writer.WriteLine(value);
				atLineStart = true;
			}
	public override void WriteLine(String value)
			{
				OutputTabs();
				writer.WriteLine(value);
				atLineStart = true;
			}
	public override void WriteLine(String value, Object arg0)
			{
				OutputTabs();
				writer.WriteLine(value, arg0);
				atLineStart = true;
			}
	public override void WriteLine(String value, Object arg0, Object arg1)
			{
				OutputTabs();
				writer.WriteLine(value, arg0, arg1);
				atLineStart = true;
			}
	public override void WriteLine(String value, params Object[] args)
			{
				OutputTabs();
				writer.WriteLine(value, args);
				atLineStart = true;
			}
	public override void WriteLine(char[] value, int index, int count)
			{
				OutputTabs();
				writer.WriteLine(value, index, count);
				atLineStart = true;
			}
	[CLSCompliant(false)]
	public override void WriteLine(uint value)
			{
				OutputTabs();
				writer.WriteLine(value);
				atLineStart = true;
			}

	// Write a string with no tab processing.
	public void WriteLineNoTabs(String s)
			{
				writer.WriteLine(s);
			}

}; // class IndentedTextWriter

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom.Compiler
