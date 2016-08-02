/*
 * TextWriter.cs - Implementation of the "System.IO.TextWriter" class.
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

namespace System.IO
{

using System;
using System.Globalization;
using System.Text;

public abstract class TextWriter : MarshalByRefObject, IDisposable
{

	// The "Null" text writer that eats everything sent to it.
	public static readonly TextWriter Null = new NullTextWriter();

	// Local state.
#if !ECMA_COMPAT
	protected char[] CoreNewLine;
#else
	private char[] CoreNewLine;
#endif
	private IFormatProvider provider; 

	// Constructors.
	protected TextWriter()
			{
				CoreNewLine = Environment.NewLine.ToCharArray();
				provider = null;
			}
	protected TextWriter(IFormatProvider provider)
			{
				CoreNewLine = Environment.NewLine.ToCharArray();
				this.provider = provider;
			}

	// Close this text writer.
	public virtual void Close()
			{
				Dispose(true);
			}

	// Dispose this text writer.  Normally overridden by subclasses.
	protected virtual void Dispose(bool disposing)
			{
				// Nothing to do in the base class.
			}

	// Implement IDisposable.
	void IDisposable.Dispose()
			{
				Dispose(true);
			}

	// Flush the contents of the text writer.  Normally overridden.
	public virtual void Flush()
			{
				// Nothing to do in the base class.
			}

	// Wrap a text writer to make it synchronized.
	public static TextWriter Synchronized(TextWriter writer)
			{
				if(writer == null)
				{
					throw new ArgumentNullException("writer");
				}
				if(writer is SyncTextWriter)
				{
					return writer;
				}
				else
				{
					return new SyncTextWriter(writer);
				}
			}

	// Write a formatted string to this text writer.
	public virtual void Write(String format, Object arg0)
			{
				Write(String.Format(format, arg0));
			}
	public virtual void Write(String format, Object arg0, Object arg1)
			{
				Write(String.Format(format, arg0, arg1));
			}
	public virtual void Write(String format, Object arg0, Object arg1,
							  Object arg2)
			{
				Write(String.Format(format, arg0, arg1, arg2));
			}
	public virtual void Write(String format, params Object[] args)
			{
				Write(String.Format(format, args));
			}

	// Write primitive values to this text writer.
	public virtual void Write(bool value)
			{
				Write(value ? Boolean.TrueString : Boolean.FalseString);
			}
	public virtual void Write(char value)
			{
				// Overridden by subclasses.
			}
	public virtual void Write(char[] value)
			{
				if(value != null)
				{
					Write(value, 0, value.Length);
				}
			}
	public virtual void Write(char[] value, int index, int count)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(index < 0 || index > value.Length)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_Array"));
				}
				else if(count < 0 || (value.Length - index) < count)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_Array"));
				}
				while(count > 0)
				{
					Write(value[index++]);
					--count;
				}
			}
#if CONFIG_EXTENDED_NUMERICS
	public virtual void Write(double value)
			{
				Write(value.ToString(null, FormatProvider));
			}
	public virtual void Write(Decimal value)
			{
				Write(value.ToString(null, FormatProvider));
			}
	public virtual void Write(float value)
			{
				Write(value.ToString(null, FormatProvider));
			}
#endif
	public virtual void Write(int value)
			{
				Write(value.ToString(null, FormatProvider));
			}
	[CLSCompliant(false)]
	public virtual void Write(uint value)
			{
				Write(value.ToString(null, FormatProvider));
			}
	public virtual void Write(long value)
			{
				Write(value.ToString(null, FormatProvider));
			}
	[CLSCompliant(false)]
	public virtual void Write(ulong value)
			{
				Write(value.ToString(null, FormatProvider));
			}
	public virtual void Write(Object value)
			{
				if(value != null)
				{
					Write(value.ToString());
				}
			}
	public virtual void Write(String value)
			{
				Write(value.ToCharArray());
			}

	// Write a newline to this text writer.
	public virtual void WriteLine()
			{
				Write(CoreNewLine);
			}

	// Write a formatted string to this text writer followed by a newline.
	public virtual void WriteLine(String format, Object arg0)
			{
				WriteLine(String.Format(format, arg0));
			}
	public virtual void WriteLine(String format, Object arg0, Object arg1)
			{
				WriteLine(String.Format(format, arg0, arg1));
			}
	public virtual void WriteLine(String format, Object arg0, Object arg1,
							     Object arg2)
			{
				WriteLine(String.Format(format, arg0, arg1, arg2));
			}
	public virtual void WriteLine(String format, params Object[] args)
			{
				WriteLine(String.Format(format, args));
			}

	// Write primitive values to this text writer followed by a newline.
	public virtual void WriteLine(bool value)
			{
				Write(value);
				WriteLine();
			}
	public virtual void WriteLine(char value)
			{
				Write(value);
				WriteLine();
			}
	public virtual void WriteLine(char[] value)
			{
				Write(value);
				WriteLine();
			}
	public virtual void WriteLine(char[] value, int index, int count)
			{
				Write(value, index, count);
				WriteLine();
			}
#if CONFIG_EXTENDED_NUMERICS
	public virtual void WriteLine(double value)
			{
				Write(value);
				WriteLine();
			}
	public virtual void WriteLine(Decimal value)
			{
				Write(value);
				WriteLine();
			}
	public virtual void WriteLine(float value)
			{
				Write(value);
				WriteLine();
			}
#endif
	public virtual void WriteLine(int value)
			{
				Write(value);
				WriteLine();
			}
	[CLSCompliant(false)]
	public virtual void WriteLine(uint value)
			{
				Write(value);
				WriteLine();
			}
	public virtual void WriteLine(long value)
			{
				Write(value);
				WriteLine();
			}
	[CLSCompliant(false)]
	public virtual void WriteLine(ulong value)
			{
				Write(value);
				WriteLine();
			}
	public virtual void WriteLine(Object value)
			{
				Write(value);
				WriteLine();
			}
	public virtual void WriteLine(String value)
			{
				Write(value);
				WriteLine();
			}

	// Get the encoding in use by this text writer.
	public abstract System.Text.Encoding Encoding { get; }

	// Get or set the newline string in use by this text writer.
	public virtual String NewLine
			{
				get
				{
					return new String(CoreNewLine);
				}
				set
				{
					if(value != null)
					{
						CoreNewLine = value.ToCharArray();
					}
					else
					{
						CoreNewLine = "\r\n".ToCharArray();
					}
				}
			}

	// Get the format provider in use by this text writer.
	public virtual IFormatProvider FormatProvider
			{
				get
				{
					if(provider != null)
					{
						return provider;
					}
					else
					{
					#if CONFIG_REFLECTION
						return CultureInfo.CurrentCulture;
					#else
						return null;
					#endif
					}
				}
			}

	// Private class that implements the null text writer.
	private sealed class NullTextWriter : TextWriter
	{
		// Constructor.
		public NullTextWriter() : base() {}

		// Override common writing methods to null them out.
		public override void Write(String value) {}
		public override void Write(char[] value, int index, int count) {}

		// Override the "Encoding" property to null it out.
		public override System.Text.Encoding Encoding
				{
					get
					{
						return System.Text.Encoding.Default;
					}
				}

	}; // class NullTextWriter

	// Private class that implements synchronized text writers.
	private sealed class SyncTextWriter : TextWriter
	{
		// Private storage for the underlying writer.
		private TextWriter writer;

		// Constructor.
		public SyncTextWriter(TextWriter writer)
				{
					this.writer = writer;
				}

		// Close this text writer.
		public override void Close()
				{
					lock(writer)
					{
						writer.Close();
					}
				}
	
		// Dispose this text writer.
		protected override void Dispose(bool disposing)
				{
					lock(writer)
					{
						writer.Dispose(disposing);
					}
				}
	
		// Flush the contents of the text writer.
		public override void Flush()
				{
					lock(writer)
					{
						writer.Flush();
					}
				}
	
		// Write a formatted string to this text writer.
		public override void Write(String format, Object arg0)
				{
					lock(writer)
					{
						writer.Write(format, arg0);
					}
				}
		public override void Write(String format, Object arg0, Object arg1)
				{
					lock(writer)
					{
						writer.Write(format, arg0, arg1);
					}
				}
		public override void Write(String format, Object arg0, Object arg1,
								  Object arg2)
				{
					lock(writer)
					{
						writer.Write(format, arg0, arg1, arg2);
					}
				}
		public override void Write(String format, params Object[] args)
				{
					lock(writer)
					{
						writer.Write(format, args);
					}
				}
	
		// Write primitive values to this text writer.
		public override void Write(bool value)
				{
					lock(writer)
					{
						writer.Write(value);
					}
				}
		public override void Write(char value)
				{
					lock(writer)
					{
						writer.Write(value);
					}
				}
		public override void Write(char[] value)
				{
					lock(writer)
					{
						writer.Write(value);
					}
				}
		public override void Write(char[] value, int index, int count)
				{
					lock(writer)
					{
						writer.Write(value, index, count);
					}
				}
#if CONFIG_EXTENDED_NUMERICS
		public override void Write(double value)
				{
					lock(writer)
					{
						writer.Write(value);
					}
				}
		public override void Write(Decimal value)
				{
					lock(writer)
					{
						writer.Write(value);
					}
				}
		public override void Write(float value)
				{
					lock(writer)
					{
						writer.Write(value);
					}
				}
#endif
		public override void Write(int value)
				{
					lock(writer)
					{
						writer.Write(value);
					}
				}
		public override void Write(uint value)
				{
					lock(writer)
					{
						writer.Write(value);
					}
				}
		public override void Write(long value)
				{
					lock(writer)
					{
						writer.Write(value);
					}
				}
		public override void Write(ulong value)
				{
					lock(writer)
					{
						writer.Write(value);
					}
				}
		public override void Write(Object value)
				{
					lock(writer)
					{
						writer.Write(value);
					}
				}
		public override void Write(String value)
				{
					lock(writer)
					{
						writer.Write(value);
					}
				}
	
		// Write a newline to this text writer.
		public override void WriteLine()
				{
					lock(writer)
					{
						writer.WriteLine();
					}
				}
	
		// Write a formatted string to this text writer followed by a newline.
		public override void WriteLine(String format, Object arg0)
				{
					lock(writer)
					{
						writer.WriteLine(format, arg0);
					}
				}
		public override void WriteLine(String format, Object arg0, Object arg1)
				{
					lock(writer)
					{
						writer.WriteLine(format, arg0, arg1);
					}
				}
		public override void WriteLine(String format, Object arg0, Object arg1,
								     Object arg2)
				{
					lock(writer)
					{
						writer.WriteLine(format, arg0, arg1, arg2);
					}
				}
		public override void WriteLine(String format, params Object[] args)
				{
					lock(writer)
					{
						writer.WriteLine(format, args);
					}
				}
	
		// Write primitive values to this text writer followed by a newline.
		public override void WriteLine(bool value)
				{
					lock(writer)
					{
						writer.WriteLine(value);
					}
				}
		public override void WriteLine(char value)
				{
					lock(writer)
					{
						writer.WriteLine(value);
					}
				}
		public override void WriteLine(char[] value)
				{
					lock(writer)
					{
						writer.WriteLine(value);
					}
				}
		public override void WriteLine(char[] value, int index, int count)
				{
					lock(writer)
					{
						writer.WriteLine(value, index, count);
					}
				}
#if CONFIG_EXTENDED_NUMERICS
		public override void WriteLine(double value)
				{
					lock(writer)
					{
						writer.WriteLine(value);
					}
				}
		public override void WriteLine(Decimal value)
				{
					lock(writer)
					{
						writer.WriteLine(value);
					}
				}
		public override void WriteLine(float value)
				{
					lock(writer)
					{
						writer.WriteLine(value);
					}
				}
#endif
		public override void WriteLine(int value)
				{
					lock(writer)
					{
						writer.WriteLine(value);
					}
				}
		public override void WriteLine(uint value)
				{
					lock(writer)
					{
						writer.WriteLine(value);
					}
				}
		public override void WriteLine(long value)
				{
					lock(writer)
					{
						writer.WriteLine(value);
					}
				}
		public override void WriteLine(ulong value)
				{
					lock(writer)
					{
						writer.WriteLine(value);
					}
				}
		public override void WriteLine(Object value)
				{
					lock(writer)
					{
						writer.WriteLine(value);
					}
				}
		public override void WriteLine(String value)
				{
					lock(writer)
					{
						writer.WriteLine(value);
					}
				}
	
		// Get the encoding in use by this text writer.
		public override System.Text.Encoding Encoding
				{
					get
					{
						System.Text.Encoding enc;
						lock(writer)
						{
							enc = writer.Encoding;
						}
						return enc;
					}
				}
	
		// Get or set the newline string in use by this text writer.
		public override String NewLine
				{
					get
					{
						String nl;
						lock(writer)
						{
							nl = writer.NewLine;
						}
						return nl;
					}
					set
					{
						lock(writer)
						{
							writer.NewLine = value;
						}
					}
				}
	
		// Get the format provider in use by this text writer.
		public override IFormatProvider FormatProvider
				{
					get
					{
						IFormatProvider prov;
						lock(writer)
						{
							prov = writer.FormatProvider;
						}
						return prov;
					}
				}
	
	}; // class SyncTextWriter
	
}; // class TextWriter

}; // namespace System.IO
