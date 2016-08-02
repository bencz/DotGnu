/*
 * TraceListener.cs - Implementation of the
 *			"System.Diagnostics.TraceListener" class.
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

namespace System.Diagnostics
{

#if !ECMA_COMPAT

public abstract class TraceListener : MarshalByRefObject, IDisposable
{
	// Internal state.
	private int indentLevel;
	private int indentSize;
	private String name;
	private bool needIndent;

	// Constructors.
	protected TraceListener()
			{
				this.indentLevel = 0;
				this.indentSize = 4;
				this.name = String.Empty;
				this.needIndent = false;
			}
	protected TraceListener(String name)
			{
				this.indentLevel = 0;
				this.indentSize = 4;
				this.name = name;
				this.needIndent = false;
			}

	// Listener properties.
	public int IndentLevel
			{
				get
				{
					return indentLevel;
				}
				set
				{
					if(value < 0)
					{
						value = 0;
					}
					indentLevel = value;
				}
			}
	public int IndentSize
			{
				get
				{
					return indentSize;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("ArgRange_NonNegative"));
					}
					indentSize = value;
				}
			}
	public virtual String Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}
	protected bool NeedIndent
			{
				get
				{
					return needIndent;
				}
				set
				{
					needIndent = value;
				}
			}

	// Close this trace listener.
	public virtual void Close()
			{
				// Nothing to do here.
			}

	// Dispose this trace listener.
	public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
	protected virtual void Dispose(bool disposing)
			{
				// Nothing to do here.
			}

	// Log a failure message to this trace listener.
	public virtual void Fail(String message)
			{
				// Nothing to do here.
			}
	public virtual void Fail(String message, String detailMessage)
			{
				// Nothing to do here.
			}

	// Flush this trace listener's output stream.
	public virtual void Flush()
			{
				// Nothing to do here.
			}

	// Write data to this trace listener's output stream.
	public virtual void Write(Object o)
			{
				if(o != null)
				{
					Write(o.ToString());
				}
			}
	public abstract void Write(String message);
	public virtual void Write(Object o, String category)
			{
				if(category == null)
				{
					if(o != null)
					{
						Write(o.ToString());
					}
				}
				else if(o != null)
				{
					Write(o.ToString(), category);
				}
				else
				{
					Write(String.Empty, category);
				}
			}
	public virtual void Write(String message, String category)
			{
				if(category == null)
				{
					Write(message);
				}
				else if(message != null)
				{
					Write(category + ": " + message);
				}
				else
				{
					Write(category + ": ");
				}
			}

	// Write indent data to this trace listener.
	protected virtual void WriteIndent()
			{
				NeedIndent = false;
				int level = indentLevel;
				String indent;
				if(indentSize == 4)
				{
					// Short-cut to improve common-case performance.
					indent = "    ";
				}
				else
				{
					indent = new String(' ', indentSize);
				}
				while(level > 0)
				{
					Write(indent);
					--level;
				}
			}

	// Write data to this trace listener's output stream followed by newline.
	public virtual void WriteLine(Object o)
			{
				if(o != null)
				{
					WriteLine(o.ToString());
				}
				else
				{
					WriteLine(String.Empty);
				}
			}
	public abstract void WriteLine(String message);
	public virtual void WriteLine(Object o, String category)
			{
				if(category == null)
				{
					if(o != null)
					{
						WriteLine(o.ToString());
					}
				}
				else if(o != null)
				{
					WriteLine(o.ToString(), category);
				}
				else
				{
					WriteLine(String.Empty, category);
				}
			}
	public virtual void WriteLine(String message, String category)
			{
				if(category == null)
				{
					WriteLine(message);
				}
				else if(message != null)
				{
					WriteLine(category + ": " + message);
				}
				else
				{
					WriteLine(category + ": ");
				}
			}

}; // class TraceListener

#endif // !ECMA_COMPAT

}; // namespace System.Diagnostics
