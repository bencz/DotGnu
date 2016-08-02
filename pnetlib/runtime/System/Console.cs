/*
 * Console.cs - Implementation of the "System.Console" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

namespace System
{

using System.IO;
using System.Text;
using System.Private;
using Platform;

public sealed class Console
{

#if !CONFIG_SMALL_CONSOLE

	/*
	 * Helper classes to initialize the streams only on first access.
	 */
	private class StdIn
	{
		public static TextReader stream;

		static StdIn()
				{
					stream = new StdReader(0);
				}
		
	}

	private class StdOut
	{
		public static TextWriter stream;

		static StdOut()
				{
					Encoding encoding = Encoding.Default;
					StreamWriter writer;

					if(encoding is UTF8Encoding)
					{
						// Disable the preamble if UTF-8.
						encoding = new UTF8Encoding();
					}
					writer = new StreamWriter(new StdStream(1), encoding);
					writer.AutoFlush = true;
					stream = TextWriter.Synchronized(writer);
				}
	}

	private class StdErr
	{
		public static TextWriter stream;

		static StdErr()
				{
					Encoding encoding = Encoding.Default;
					StreamWriter writer;

					if(encoding is UTF8Encoding)
					{
						// Disable the preamble if UTF-8.
						encoding = new UTF8Encoding();
					}
					writer = new StreamWriter(new StdStream(2), encoding);
					writer.AutoFlush = true;
					stream = TextWriter.Synchronized(writer);
				}
	}

	// This class cannot be instantiated.
	private Console() {}

	// Open the standard input stream.
	public static Stream OpenStandardInput()
			{
				return new StdStream(0);
			}
	public static Stream OpenStandardInput(int bufferSize)
			{
				return new StdStream(0);
			}

	// Open the standard output stream.
	public static Stream OpenStandardOutput()
			{
				return new StdStream(1);
			}
	public static Stream OpenStandardOutput(int bufferSize)
			{
				return new StdStream(1);
			}

	// Open the standard error stream.
	public static Stream OpenStandardError()
			{
				return new StdStream(2);
			}
	public static Stream OpenStandardError(int bufferSize)
			{
				return new StdStream(2);
			}

	// Get the standard input stream.
	public static TextReader In
			{
				get
				{
					return StdIn.stream;
				}
			}

	// Get the standard output stream.
	public static TextWriter Out
			{
				get
				{
					return StdOut.stream;
				}
			}

	// Get the standard error stream.
	public static TextWriter Error
			{
				get
				{
					return StdErr.stream;
				}
			}

#if CONFIG_FRAMEWORK_2_0

	// Get or set the input stream's encoding.
	[TODO]
	public static Encoding InputEncoding
			{
				get
				{
					// TODO
					return Encoding.Default;
				}
				set
				{
					// TODO
				}
			}

	// Get or set the output stream's encoding.
	[TODO]
	public static Encoding OutputEncoding
			{
				get
				{
					// TODO
					return Encoding.Default;
				}
				set
				{
					// TODO
				}
			}

#endif

	// Set the standard input stream.
	public static void SetIn(TextReader newIn)
			{
				if(newIn == null)
				{
					throw new ArgumentNullException("newIn");
				}
				StdIn.stream = TextReader.Synchronized(newIn);
			}

	// Set the standard output stream.
	public static void SetOut(TextWriter newOut)
			{
				if(newOut == null)
				{
					throw new ArgumentNullException("newOut");
				}
				StdOut.stream = TextWriter.Synchronized(newOut);
			}

	// Set the standard error stream.
	public static void SetError(TextWriter newError)
			{
				if(newError == null)
				{
					throw new ArgumentNullException("newError");
				}
				StdErr.stream = TextWriter.Synchronized(newError);
			}

	// Read a character from the standard input stream.
	public static int Read()
			{
				NormalMode();
				return In.Read();
			}

	// Read a line from the standard input stream.
	public static String ReadLine()
			{
				NormalMode();
				return In.ReadLine();
			}

	// Write a formatted string to standard output.
	public static void Write(String format, Object arg0)
			{
				Out.Write(format, arg0);
			}
	public static void Write(String format, Object arg0, Object arg1)
			{
				Out.Write(format, arg0, arg1);
			}
	public static void Write(String format, Object arg0, Object arg1,
							 Object arg2)
			{
				Out.Write(format, arg0, arg1, arg2);
			}
	public static void Write(String format, params Object[] args)
			{
				Out.Write(format, args);
			}
#if !ECMA_COMPAT
	[CLSCompliant(false)]
	public static void Write(String format, Object arg0, Object arg1,
							 Object arg2, Object arg3, __arglist)
			{
				ArgIterator iter = new ArgIterator(__arglist);
				Object[] list = new Object [4 + iter.GetRemainingCount()];
				list[0] = arg0;
				list[1] = arg1;
				list[2] = arg2;
				list[3] = arg3;
				int posn = 4;
				while(posn < list.Length)
				{
					list[posn] = TypedReference.ToObject(iter.GetNextArg());
					++posn;
				}
				Out.Write(format, list);
			}
#endif // !ECMA_COMPAT

	// Write primitive values to standard output.
	public static void Write(bool value)
			{
				Out.Write(value);
			}
	public static void Write(char value)
			{
				Out.Write(value);
			}
	public static void Write(char[] value)
			{
				Out.Write(value);
			}
	public static void Write(char[] value, int index, int count)
			{
				Out.Write(value, index, count);
			}
#if CONFIG_EXTENDED_NUMERICS
	public static void Write(double value)
			{
				Out.Write(value);
			}
	public static void Write(Decimal value)
			{
				Out.Write(value);
			}
	public static void Write(float value)
			{
				Out.Write(value);
			}
#endif
	public static void Write(int value)
			{
				Out.Write(value);
			}
	[CLSCompliant(false)]
	public static void Write(uint value)
			{
				Out.Write(value);
			}
	public static void Write(long value)
			{
				Out.Write(value);
			}
	[CLSCompliant(false)]
	public static void Write(ulong value)
			{
				Out.Write(value);
			}
	public static void Write(Object value)
			{
				Out.Write(value);
			}
	public static void Write(String value)
			{
				Out.Write(value);
			}

	// Write a newline to standard output.
	public static void WriteLine()
			{
				Out.WriteLine();
			}

	// Write a formatted string to standard output followed by a newline.
	public static void WriteLine(String format, Object arg0)
			{
				Out.WriteLine(format, arg0);
			}
	public static void WriteLine(String format, Object arg0, Object arg1)
			{
				Out.WriteLine(format, arg0, arg1);
			}
	public static void WriteLine(String format, Object arg0, Object arg1,
							     Object arg2)
			{
				Out.WriteLine(format, arg0, arg1, arg2);
			}
	public static void WriteLine(String format, params Object[] args)
			{
				Out.WriteLine(format, args);
			}
#if !ECMA_COMPAT
	[CLSCompliant(false)]
	public static void WriteLine(String format, Object arg0, Object arg1,
							     Object arg2, Object arg3, __arglist)
			{
				ArgIterator iter = new ArgIterator(__arglist);
				Object[] list = new Object [4 + iter.GetRemainingCount()];
				list[0] = arg0;
				list[1] = arg1;
				list[2] = arg2;
				list[3] = arg3;
				int posn = 4;
				while(posn < list.Length)
				{
					list[posn] = TypedReference.ToObject(iter.GetNextArg());
					++posn;
				}
				Out.WriteLine(format, list);
			}
#endif // !ECMA_COMPAT

	// Write primitive values to standard output followed by a newline.
	public static void WriteLine(bool value)
			{
				Out.WriteLine(value);
			}
	public static void WriteLine(char value)
			{
				Out.WriteLine(value);
			}
	public static void WriteLine(char[] value)
			{
				Out.WriteLine(value);
			}
	public static void WriteLine(char[] value, int index, int count)
			{
				Out.WriteLine(value, index, count);
			}
#if CONFIG_EXTENDED_NUMERICS
	public static void WriteLine(double value)
			{
				Out.WriteLine(value);
			}
	public static void WriteLine(Decimal value)
			{
				Out.WriteLine(value);
			}
	public static void WriteLine(float value)
			{
				Out.WriteLine(value);
			}
#endif
	public static void WriteLine(int value)
			{
				Out.WriteLine(value);
			}
	[CLSCompliant(false)]
	public static void WriteLine(uint value)
			{
				Out.WriteLine(value);
			}
	public static void WriteLine(long value)
			{
				Out.WriteLine(value);
			}
	[CLSCompliant(false)]
	public static void WriteLine(ulong value)
			{
				Out.WriteLine(value);
			}
	public static void WriteLine(Object value)
			{
				Out.WriteLine(value);
			}
	public static void WriteLine(String value)
			{
				Out.WriteLine(value);
			}

#else // CONFIG_SMALL_CONSOLE

	// This class cannot be instantiated.
	private Console() {}

	// Read a line from the standard input stream.
	public static String ReadLine()
			{
				StringBuilder builder = new StringBuilder();
				int ch;
				NormalMode();
				while((ch = Stdio.StdRead(0)) != -1 && ch != '\n')
				{
					if(ch != '\r')
					{
						builder.Append((char)ch);
					}
				}
				if(ch == -1 && builder.Length == 0)
				{
					return null;
				}
				else
				{
					return builder.ToString();
				}
			}

	// Write a formatted string to standard output.
	public static void Write(String format, Object arg0)
			{
				Stdio.StdWrite(1, String.Format(format, arg0));
			}
	public static void Write(String format, Object arg0, Object arg1)
			{
				Stdio.StdWrite(1, String.Format(format, arg0, arg1));
			}
	public static void Write(String format, Object arg0, Object arg1,
							 Object arg2)
			{
				Stdio.StdWrite(1, String.Format(format, arg0, arg1, arg2));
			}
	public static void Write(String format, params Object[] args)
			{
				Stdio.StdWrite(1, String.Format(format, args));
			}

	// Write primitive values to standard output.
	public static void Write(char value)
			{
				Stdio.StdWrite(1, value);
			}
	public static void Write(char[] value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("null");
				}
				foreach(char ch in value)
				{
					Stdio.StdWrite(1, ch);
				}
			}
	public static void Write(char[] value, int index, int count)
			{
				if(value == null)
				{
					throw new ArgumentNullException("null");
				}
				if(index < 0 || index > value.Length)
				{
					throw new ArgumentOutOfRangeException
						("index", _("ArgRange_StringIndex"));
				}
				if(count < 0 || count > (value.Length - index))
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_StringRange"));
				}
				while(count > 0)
				{
					Stdio.StdWrite(1, value[index]);
					++index;
					--count;
				}
			}
	public static void Write(int value)
			{
				Stdio.StdWrite(1, value.ToString());
			}
	public static void Write(Object value)
			{
				if(value != null)
				{
					Stdio.StdWrite(1, value.ToString());
				}
			}
	public static void Write(String value)
			{
				Stdio.StdWrite(1, value);
			}

	// Write a newline to standard output.
	public static void WriteLine()
			{
				Stdio.StdWrite(1, Environment.NewLine);
			}

	// Write a formatted string to standard output followed by a newline.
	public static void WriteLine(String format, Object arg0)
			{
				Write(format, arg0);
				WriteLine();
			}
	public static void WriteLine(String format, Object arg0, Object arg1)
			{
				Write(format, arg0, arg1);
				WriteLine();
			}
	public static void WriteLine(String format, Object arg0, Object arg1,
							     Object arg2)
			{
				Write(format, arg0, arg1, arg2);
				WriteLine();
			}
	public static void WriteLine(String format, params Object[] args)
			{
				Write(format, args);
				WriteLine();
			}

	// Write primitive values to standard output followed by a newline.
	public static void WriteLine(char value)
			{
				Stdio.StdWrite(1, value);
				WriteLine();
			}
	public static void WriteLine(char[] value)
			{
				Write(value);
				WriteLine();
			}
	public static void WriteLine(int value)
			{
				Write(value);
				WriteLine();
			}
	public static void WriteLine(Object value)
			{
				Write(value);
				WriteLine();
			}
	public static void WriteLine(String value)
			{
				Stdio.StdWrite(1, value);
				WriteLine();
			}

#endif // CONFIG_SMALL_CONSOLE

#if CONFIG_EXTENDED_CONSOLE

	// Global state for the extended console.
	private static String title = String.Empty;
	private static bool specialMode = false;
	private static bool treatControlCAsInput = false;
	private static Object readLock = new Object();
	private static int defaultAttrs = 0x07;
	private static int currentAttrs = 0x07;

	// Enable the "normal" input mode on the console.
	private static void NormalMode()
			{
				lock(typeof(Console))
				{
					if(specialMode)
					{
						specialMode = false;
						Stdio.SetConsoleMode(Stdio.MODE_NORMAL);
					}
				}
			}

	// Enable the "special" character-at-a-time input mode on the console.
	private static void SpecialMode()
			{
				lock(typeof(Console))
				{
					if(!specialMode)
					{
						specialMode = true;
						if(treatControlCAsInput)
						{
							Stdio.SetConsoleMode(Stdio.MODE_RAW);
						}
						else
						{
							Stdio.SetConsoleMode(Stdio.MODE_CBREAK);
						}
						defaultAttrs = Stdio.GetTextAttributes();
						currentAttrs = defaultAttrs;
					}
				}
			}

	// Output a beep on the console.
	public static void Beep()
			{
				Beep(800, 200);
			}
	public static void Beep(int frequency, int duration)
			{
				if(frequency < 37 || frequency > 32767)
				{
					throw new ArgumentOutOfRangeException
						("frequency", _("ArgRange_BeepFrequency"));
				}
				if(duration <= 0)
				{
					throw new ArgumentOutOfRangeException
						("duration", _("ArgRange_PositiveNonZero"));
				}
				lock(typeof(Console))
				{
					SpecialMode();
					Stdio.Beep(frequency, duration);
				}
			}

	// Clear the display to the current foreground and background color.
	// If "Clear" is the first extended console method called, then it
	// indicates that the terminal should enter the "alternative" mode
	// used for programs like "vi".  Returning to the normal mode will
	// restore what used to be displayed previously.
	public static void Clear()
			{
				lock(typeof(Console))
				{
					if(!specialMode)
					{
						specialMode = true;
						if(treatControlCAsInput)
						{
							Stdio.SetConsoleMode(Stdio.MODE_RAW_ALT);
						}
						else
						{
							Stdio.SetConsoleMode(Stdio.MODE_CBREAK_ALT);
						}
						defaultAttrs = Stdio.GetTextAttributes();
						currentAttrs = defaultAttrs;
					}
					Stdio.Clear();
				}
			}

	// Move an area of the screen buffer to a new location.
	public static void MoveBufferArea(int sourceLeft, int sourceTop,
									  int sourceWidth, int sourceHeight,
									  int targetLeft, int targetTop)
			{
				MoveBufferArea(sourceLeft, sourceTop,
							   sourceWidth, sourceHeight,
							   targetLeft, targetTop, ' ',
							   ForegroundColor, BackgroundColor);
			}
	public static void MoveBufferArea(int sourceLeft, int sourceTop,
									  int sourceWidth, int sourceHeight,
									  int targetLeft, int targetTop,
									  char sourceChar,
									  ConsoleColor sourceForeColor,
									  ConsoleColor sourceBackColor)
			{
				lock(typeof(Console))
				{
					SpecialMode();
					int width, height;
					Stdio.GetBufferSize(out width, out height);
					if(sourceLeft < 0 || sourceLeft >= width)
					{
						throw new ArgumentOutOfRangeException
							("sourceLeft", _("ArgRange_XCoordinate"));
					}
					if(sourceTop < 0 || sourceTop >= height)
					{
						throw new ArgumentOutOfRangeException
							("sourceTop", _("ArgRange_YCoordinate"));
					}
					if(sourceWidth < 0 || (sourceLeft + sourceWidth) > width)
					{
						throw new ArgumentOutOfRangeException
							("sourceWidth", _("ArgRange_Width"));
					}
					if(sourceHeight < 0 || (sourceTop + sourceHeight) > height)
					{
						throw new ArgumentOutOfRangeException
							("sourceHeight", _("ArgRange_Height"));
					}
					if(targetLeft < 0 || targetLeft >= width)
					{
						throw new ArgumentOutOfRangeException
							("targetLeft", _("ArgRange_XCoordinate"));
					}
					if(targetTop < 0 || targetTop >= height)
					{
						throw new ArgumentOutOfRangeException
							("targetTop", _("ArgRange_YCoordinate"));
					}
					if((((int)sourceForeColor) & ~0x0F) != 0)
					{
						throw new ArgumentException
							(_("Arg_InvalidColor"), "sourceForeColor");
					}
					if((((int)sourceBackColor) & ~0x0F) != 0)
					{
						throw new ArgumentException
							(_("Arg_InvalidColor"), "sourceBackColor");
					}
					Stdio.MoveBufferArea(sourceLeft, sourceTop,
										 sourceWidth, sourceHeight,
										 targetLeft, targetTop,
										 sourceChar,
										 ((int)(sourceForeColor)) |
										 (((int)(sourceBackColor)) << 4));
				}
			}

	// Read a key from the console.  If "intercept" is "false",
	// then the key is echoed to the console.
	public static ConsoleKeyInfo ReadKey()
			{
				return ReadKey(false);
			}
	public static ConsoleKeyInfo ReadKey(bool intercept)
			{
				lock(typeof(Console))
				{
					SpecialMode();
				}
				lock(readLock)
				{
					char ch;
					int key, modifiers;
					for(;;)
					{
						Stdio.ReadKey(out ch, out key, out modifiers);
						if(key == 0x1202)		// Interrupt
						{
							HandleCancel(ConsoleSpecialKey.ControlC);
							continue;
						}
						else if(key == 0x1203)	// CtrlBreak
						{
							HandleCancel(ConsoleSpecialKey.ControlBreak);
							continue;
						}
						if(!intercept && ch != '\0')
						{
							Stdio.StdWrite(1, ch);
						}
						return new ConsoleKeyInfo
							(ch, (ConsoleKey)key, (ConsoleModifiers)modifiers);
					}
				}
			}

	// Reset the foreground and background colors to the defaults.
	public static void ResetColor()
			{
				lock(typeof(Console))
				{
					SpecialMode();
					currentAttrs = defaultAttrs;
					Stdio.SetTextAttributes(defaultAttrs);
				}
			}

	// Set the buffer size.
	public static void SetBufferSize(int width, int height)
			{
				lock(typeof(Console))
				{
					SpecialMode();
					int wleft, wtop, wwidth, wheight;
					Stdio.GetWindowSize
						(out wleft, out wtop, out wwidth, out wheight);
					if(width <= 0 || width > 32767 || width < (wleft + wwidth))
					{
						throw new ArgumentOutOfRangeException
							("width", _("ArgRange_Width"));
					}
					if(height <= 0 || height > 32767 ||
					   height < (wtop + wheight))
					{
						throw new ArgumentOutOfRangeException
							("height", _("ArgRange_Height"));
					}
					Stdio.SetBufferSize(width, height);
				}
			}

	// Set the cursor position.
	public static void SetCursorPosition(int left, int top)
			{
				lock(typeof(Console))
				{
					SpecialMode();
					int width, height;
					Stdio.GetBufferSize(out width, out height);
					if(left < 0 || left >= width)
					{
						throw new ArgumentOutOfRangeException
							("left", _("ArgRange_XCoordinate"));
					}
					if(top < 0 || top >= height)
					{
						throw new ArgumentOutOfRangeException
							("top", _("ArgRange_YCoordinate"));
					}
					Stdio.SetCursorPosition(left, top);
				}
			}

	// Set the window position.
	public static void SetWindowPosition(int left, int top)
			{
				lock(typeof(Console))
				{
					SpecialMode();
					int width, height;
					Stdio.GetBufferSize(out width, out height);
					int wleft, wtop, wwidth, wheight;
					Stdio.GetWindowSize
						(out wleft, out wtop, out wwidth, out wheight);
					if(left < 0 || (left + wwidth) > width)
					{
						throw new ArgumentOutOfRangeException
							("left", _("ArgRange_XCoordinate"));
					}
					if(top < 0 || (left + wheight) > height)
					{
						throw new ArgumentOutOfRangeException
							("left", _("ArgRange_YCoordinate"));
					}
					Stdio.SetWindowSize(left, top, wwidth, wheight);
				}
			}

	// Set the window size.
	public static void SetWindowSize(int width, int height)
			{
				lock(typeof(Console))
				{
					SpecialMode();
					int bwidth, bheight;
					Stdio.GetBufferSize(out bwidth, out bheight);
					int wleft, wtop, wwidth, wheight;
					Stdio.GetWindowSize
						(out wleft, out wtop, out wwidth, out wheight);
					if(width <= 0 || (wleft + width) > bwidth)
					{
						throw new ArgumentOutOfRangeException
							("width", _("ArgRange_Width"));
					}
					if(height <= 0 || (wtop + height) > bheight)
					{
						throw new ArgumentOutOfRangeException
							("height", _("ArgRange_Height"));
					}
					Stdio.SetWindowSize(wleft, wtop, width, height);
				}
			}

	// Console properties.
	public static ConsoleColor BackgroundColor
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
						return (ConsoleColor)((currentAttrs >> 4) & 0x0F);
					}
				}
				set
				{
					if((((int)value) & ~0x0F) != 0)
					{
						throw new ArgumentException
							(_("Arg_InvalidColor"), "value");
					}
					lock(typeof(Console))
					{
						SpecialMode();
						currentAttrs = (currentAttrs & 0x0F) |
									   (((int)value) << 4);
						Stdio.SetTextAttributes(currentAttrs);
					}
				}
			}
	public static int BufferHeight
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
						int width, height;
						Stdio.GetBufferSize(out width, out height);
						return height;
					}
				}
			}
	public static int BufferWidth
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
						int width, height;
						Stdio.GetBufferSize(out width, out height);
						return width;
					}
				}
			}
	public static bool CapsLock
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
						return ((Stdio.GetLockState() & Stdio.CapsLock) != 0);
					}
				}
			}
	public static int CursorLeft
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
						int x, y;
						Stdio.GetCursorPosition(out x, out y);
						return x;
					}
				}
				set
				{
					SetCursorPosition(value, CursorTop);
				}
			}
	public static int CursorSize
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
						return Stdio.GetCursorSize();
					}
				}
				set
				{
					if(value < 1 || value > 100)
					{
						throw new ArgumentOutOfRangeException
							("value", _("ArgRange_CursorSize"));
					}
					lock(typeof(Console))
					{
						SpecialMode();
						Stdio.SetCursorSize(value);
					}
				}
			}
	public static int CursorTop
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
						int x, y;
						Stdio.GetCursorPosition(out x, out y);
						return y;
					}
				}
				set
				{
					SetCursorPosition(CursorLeft, value);
				}
			}
	public static bool CursorVisible
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
						return Stdio.GetCursorVisible();
					}
				}
				set
				{
					lock(typeof(Console))
					{
						SpecialMode();
						Stdio.SetCursorVisible(value);
					}
				}
			}
	public static ConsoleColor ForegroundColor
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
						return (ConsoleColor)(currentAttrs & 0x0F);
					}
				}
				set
				{
					if((((int)value) & ~0x0F) != 0)
					{
						throw new ArgumentException
							(_("Arg_InvalidColor"), "value");
					}
					lock(typeof(Console))
					{
						SpecialMode();
						currentAttrs = (currentAttrs & 0xF0) | ((int)value);
						Stdio.SetTextAttributes(currentAttrs);
					}
				}
			}
	public static bool KeyAvailable
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
					}
					lock(readLock)
					{
						return Stdio.KeyAvailable();
					}
				}
			}
	public static int LargestWindowHeight
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
						int width, height;
						Stdio.GetLargestWindowSize(out width, out height);
						return width;
					}
				}
			}
	public static int LargestWindowWidth
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
						int width, height;
						Stdio.GetLargestWindowSize(out width, out height);
						return height;
					}
				}
			}
	public static bool NumberLock
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
						return ((Stdio.GetLockState() & Stdio.NumLock) != 0);
					}
				}
			}
	public static String Title
			{
				get
				{
					// Note: we never query the initial console title
					// from the system because it may contain sensitive
					// data that we don't want the program to have access to.
					lock(typeof(Console))
					{
						return title;
					}
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					lock(typeof(Console))
					{
						SpecialMode();
						title = value;
						Stdio.SetConsoleTitle(title);
					}
				}
			}
	public static bool TreatControlCAsInput
			{
				get
				{
					lock(typeof(Console))
					{
						return treatControlCAsInput;
					}
				}
				set
				{
					lock(typeof(Console))
					{
						if(treatControlCAsInput != value)
						{
							specialMode = false;
							treatControlCAsInput = value;
							SpecialMode();
						}
					}
				}
			}
	public static int WindowHeight
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
						int left, top, width, height;
						Stdio.GetWindowSize
							(out left, out top, out width, out height);
						return height;
					}
				}
			}
	public static int WindowLeft
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
						int left, top, width, height;
						Stdio.GetWindowSize
							(out left, out top, out width, out height);
						return left;
					}
				}
			}
	public static int WindowTop
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
						int left, top, width, height;
						Stdio.GetWindowSize
							(out left, out top, out width, out height);
						return top;
					}
				}
			}
	public static int WindowWidth
			{
				get
				{
					lock(typeof(Console))
					{
						SpecialMode();
						int left, top, width, height;
						Stdio.GetWindowSize
							(out left, out top, out width, out height);
						return width;
					}
				}
			}

	// Event that is emitted for cancel keycodes like CTRL+C.
	public static event ConsoleCancelEventHandler CancelKeyPress;

	// Method that is called to handle "cancel" events.
	private static void HandleCancel(ConsoleSpecialKey specialKeys)
			{
				ConsoleCancelEventArgs args;
				args = new ConsoleCancelEventArgs(specialKeys);
				if(CancelKeyPress != null)
				{
					CancelKeyPress(null, args);
				}
				if(!(args.Cancel))
				{
					NormalMode();
					Environment.Exit(1);
				}
			}

#else // !CONFIG_EXTENDED_CONSOLE

	// Enable the "normal" input mode on the console.
	private static void NormalMode()
			{
				// Nothing to do if we don't have an extended console.
			}

#endif // !CONFIG_EXTENDED_CONSOLE

}; // class Console

}; // namespace System
