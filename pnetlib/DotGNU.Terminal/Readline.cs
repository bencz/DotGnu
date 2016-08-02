/*
 * Readline.cs - Implementation of the "DotGNU.Terminal.Readline" class.
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

/*

Control keys that are understood by this readline implementation (based
loosely on the GNU readline library's key bindings):

	^A		Move to the start of the line (home).
	^B		Move back one character (left).
	^C		Cancel the line and restart with a new prompt.
	^D		EOF or delete the character under the cursor (ControlDIsEOF flag).
	^E		Move to the end of the line (end).
	^F		Move forward one character (right).
	^G		Ring the terminal bell.
	^H		Backspace or delete character, depending upon tty modes.
	^J		Terminate the current line (LF).
	^K		Erase all characters until the end of the current line.
	^L		Clear the screen and repaint the current line.
	^M		Terminate the current line (CR).
	^N		Move down one line in the history.
	^P		Move up one line in the history.
	^U		Erase all characters until the start of the current line.
	^V		Prefix a control character.
	^W		Erase the previous word, delimited by whitespace.
	^Y		Paste in the last section erased by ^K, ^U, or ^W.
	^Z		EOF indication (ControlZIsEOF flag).
	ESC		Clear the current line.
	DEL		Backspace or delete character, depending upon tty modes.
	ALT-F	Move forward one word.
	ALT-B	Move backward one word.
	ALT-D	Erase all characters until the end of the current word.
	ALT-DEL	Erase all characters until the start of the current word.

*/

namespace DotGNU.Terminal
{

using System;
using System.IO;

public sealed class Readline
{
	// Internal state.
	private static bool enterIsDuplicate = false;
	private static bool controlDIsEOF = true;
	private static bool controlZIsEOF = IsWindows();

	// Cannot instantiate this class.
	private Readline() {}

	// Determine if this platform appears to be running Windows.
	// We use this to determine the default behaviour of CTRL-Z.
	private static bool IsWindows()
			{
			#if !ECMA_COMPAT
				return (Environment.OSVersion.Platform != PlatformID.Unix);
			#else
				return (Path.DirectorySeparatorChar == '\\');
			#endif
			}

#if CONFIG_EXTENDED_CONSOLE

	// Line input buffer.
	private static char[] buffer = new char [256];
	private static byte[] widths = new byte [256];
	private static int posn, length, column, lastColumn;
	private static bool overwrite = false;
	private static int historyPosn;
	private static String historySave;
	private static String yankedString = null;

	// Make room for one more character in the input buffer.
	private static void MakeRoom()
			{
				if(length >= buffer.Length)
				{
					char[] newBuffer = new char [buffer.Length * 2];
					byte[] newWidths = new byte [buffer.Length * 2];
					Array.Copy(buffer, 0, newBuffer, 0, buffer.Length);
					Array.Copy(widths, 0, newWidths, 0, buffer.Length);
				}
			}

	// Repaint the line starting at the current character.
	private static void Repaint(bool step, bool moveToEnd)
			{
				int posn = Readline.posn;
				int column = Readline.column;
				int width;

				// Paint the characters in the line.
				while(posn < length)
				{
					if(buffer[posn] == '\t')
					{
						width = 8 - (column % 8);
						widths[posn] = (byte)width;
						while(width > 0)
						{
							Console.Write(' ');
							--width;
							++column;
						}
					}
					else if(buffer[posn] < 0x20)
					{
						Console.Write('^');
						Console.Write((char)(buffer[posn] + 0x40));
						widths[posn] = 2;
						column += 2;
					}
					else if(buffer[posn] == '\u007F')
					{
						Console.Write('^');
						Console.Write('?');
						widths[posn] = 2;
						column += 2;
					}
					else
					{
						Console.Write(buffer[posn]);
						widths[posn] = 1;
						++column;
					}
					++posn;
				}

				// Adjust the position of the last column.
				if(column > lastColumn)
				{
					lastColumn = column;
				}
				else if(column < lastColumn)
				{
					// We need to clear some characters beyond this point.
					width = lastColumn - column;
					lastColumn = column;
					while(width > 0)
					{
						Console.Write(' ');
						--width;
						++column;
					}
				}

				// Backspace to the initial cursor position.
				if(moveToEnd)
				{
					width = column - lastColumn;
					Readline.posn = length;
				}
				else if(step)
				{
					width = column - (Readline.column + widths[Readline.posn]);
					Readline.column += widths[Readline.posn];
					++(Readline.posn);
				}
				else
				{
					width = column - Readline.column;
				}
				while(width > 0)
				{
					Console.Write('\u0008');
					--width;
				}
			}

	// Add a character to the input buffer.
	private static void AddChar(char ch)
			{
				if(overwrite && posn < length)
				{
					buffer[posn] = ch;
					Repaint(true, false);
				}
				else
				{
					MakeRoom();
					if(posn < length)
					{
						Array.Copy(buffer, posn, buffer, posn + 1,
								   length - posn);
					}
					buffer[posn] = ch;
					++length;
					Repaint(true, false);
				}
			}

	// Go back a specific number of characters.
	private static void GoBack(int num)
			{
				int width;
				while(num > 0)
				{
					--posn;
					width = widths[posn];
					column -= width;
					while(width > 0)
					{
						Console.Write('\u0008');
						--width;
					}
					--num;
				}
			}

	// Backspace one character.
	private static void Backspace()
			{
				if(posn > 0)
				{
					GoBack(1);
					Delete();
				}
			}

	// Delete the character under the cursor.
	private static void Delete()
			{
				if(posn < length)
				{
					Array.Copy(buffer, posn + 1, buffer, posn,
							   length - posn - 1);
					--length;
					Repaint(false, false);
				}
			}

	// Delete a number of characters under the cursor.
	private static void Delete(int num)
			{
				Array.Copy(buffer, posn + num, buffer, posn,
						   length - posn - num);
				length -= num;
				Repaint(false, false);
			}

	// Print a list of alternatives for tab completion.
	private static void PrintAlternatives(String[] list)
			{
				int width, maxWidth;
				int columns, column, posn;
				String str;

				// Determine the maximum string length, for formatting.
				maxWidth = 0;
				foreach(String a in list)
				{
					if(a != null)
					{
						width = a.Length;
						if(width > maxWidth)
						{
							maxWidth = width;
						}
					}
				}

				// Determine the number of columns.
				width = Console.WindowWidth;
				if(maxWidth > (width - 7))
				{
					columns = 1;
				}
				else
				{
					columns = width / (maxWidth + 7);
				}

				// Print the strings.
				column = 0;
				for(posn = 0; posn < list.Length; ++posn)
				{
					str = list[posn];
					if(str != null)
					{
						Console.Write(str);
						width = str.Length;
					}
					else
					{
						width = 0;
					}
					++column;
					if(column < columns)
					{
						while(width < maxWidth)
						{
							Console.Write(' ');
							++width;
						}
						Console.Write("       ");
					}
					else
					{
						Console.Write("\r\n");
						column = 0;
					}
				}
				if(column != 0)
				{
					Console.Write("\r\n");
				}
			}

	// Tab across to the next stop, or perform tab completion.
	private static void Tab(String prompt)
			{
				if(TabComplete == null)
				{
					// Add the TAB character and repaint the line.
					AddChar('\t');
				}
				else
				{
					// Perform tab completion and insert the results.
					TabCompleteEventArgs e;
					e = new TabCompleteEventArgs
						(new String(buffer, 0, posn),
						 new String(buffer, posn, length - posn));
					TabComplete(null, e);
					if(e.Insert != null)
					{
						// Insert the value that we found.
						bool saveOverwrite = overwrite;
						overwrite = false;
						foreach(char ch in e.Insert)
						{
							AddChar(ch);
						}
						overwrite = saveOverwrite;
					}
					else if(e.Alternatives != null && e.Alternatives.Length > 0)
					{
						// Print the alternatives for the user.
						int savePosn = posn;
						EndLine();
						PrintAlternatives(e.Alternatives);
						if(prompt != null)
						{
							Console.Write(prompt);
						}
						posn = savePosn;
						Redraw();
					}
					else
					{
						// No alternatives, or alternatives not supplied yet.
						Console.Beep();
					}
				}
			}

	// End the current line.
	private static void EndLine()
			{
				// Repaint the line and move to the end.
				Repaint(false, true);

				// Output the line terminator to the terminal.
				Console.Write("\r\n");
			}

	// Move left one character.
	private static void MoveLeft()
			{
				if(posn > 0)
				{
					GoBack(1);
				}
			}

	// Move right one character.
	private static void MoveRight()
			{
				if(posn < length)
				{
					Repaint(true, false);
				}
			}

	// Set the current buffer contents to a historical string.
	private static void SetCurrent(String line)
			{
				if(line == null)
				{
					line = String.Empty;
				}
				Clear();
				foreach(char ch in line)
				{
					AddChar(ch);
				}
			}

	// Move up one line in the history.
	private static void MoveUp()
			{
				if(historyPosn == -1)
				{
					if(History.Count > 0)
					{
						historySave = new String(buffer, 0, length);
						historyPosn = 0;
						SetCurrent(History.GetHistory(historyPosn));
					}
				}
				else if((historyPosn + 1) < History.Count)
				{
					++historyPosn;
					SetCurrent(History.GetHistory(historyPosn));
				}
				else
				{
					Console.Beep();
				}
			}

	// Move down one line in the history.
	private static void MoveDown()
			{
				if(historyPosn == 0)
				{
					historyPosn = -1;
					SetCurrent(historySave);
				}
				else if(historyPosn > 0)
				{
					--historyPosn;
					SetCurrent(History.GetHistory(historyPosn));
				}
				else
				{
					Console.Beep();
				}
			}

	// Move to the beginning of the current line.
	private static void MoveHome()
			{
				GoBack(posn);
			}

	// Move to the end of the current line.
	private static void MoveEnd()
			{
				Repaint(false, true);
			}

	// Clear the entire line.
	private static void Clear()
			{
				GoBack(posn);
				length = 0;
				Repaint(false, false);
			}

	// Cancel the current line and start afresh with a new prompt.
	private static void CancelLine(String prompt)
			{
				EndLine();
				if(prompt != null)
				{
					Console.Write(prompt);
				}
				posn = 0;
				length = 0;
				column = 0;
				lastColumn = 0;
				historyPosn = -1;
			}

	// Redraw the current line.
	private static void Redraw()
			{
				String str = new String(buffer, 0, length);
				int savePosn = posn;
				posn = 0;
				length = 0;
				column = 0;
				lastColumn = 0;
				foreach(char ch in str)
				{
					AddChar(ch);
				}
				GoBack(length - savePosn);
			}

	// Erase all characters until the start of the current line.
	private static void EraseToStart()
			{
				if(posn > 0)
				{
					int savePosn = posn;
					yankedString = new String(buffer, 0, posn);
					GoBack(savePosn);
					Delete(savePosn);
				}
			}

	// Erase all characters until the end of the current line.
	private static void EraseToEnd()
			{
				yankedString = new String(buffer, posn, length - posn);
				length = posn;
				Repaint(false, false);
			}

	// Erase the previous word on the current line (delimited by whitespace).
	private static void EraseWord()
			{
				int temp = posn;
				while(temp > 0 && Char.IsWhiteSpace(buffer[temp - 1]))
				{
					--temp;
				}
				while(temp > 0 && !Char.IsWhiteSpace(buffer[temp - 1]))
				{
					--temp;
				}
				if(temp < posn)
				{
					temp = posn - temp;
					GoBack(temp);
					yankedString = new String(buffer, posn, temp);
					Delete(temp);
				}
			}

	// Determine if a character is a "word character" (letter or digit).
	private static bool IsWordCharacter(char ch)
			{
				return Char.IsLetterOrDigit(ch);
			}

	// Erase to the end of the current word.
	private static void EraseToEndWord()
			{
				int temp = posn;
				while(temp < length && !IsWordCharacter(buffer[temp]))
				{
					++temp;
				}
				while(temp < length && IsWordCharacter(buffer[temp]))
				{
					++temp;
				}
				if(temp > posn)
				{
					temp -= posn;
					yankedString = new String(buffer, posn, temp);
					Delete(temp);
				}
			}

	// Erase to the start of the current word.
	private static void EraseToStartWord()
			{
				int temp = posn;
				while(temp > 0 && !IsWordCharacter(buffer[temp - 1]))
				{
					--temp;
				}
				while(temp > 0 && IsWordCharacter(buffer[temp - 1]))
				{
					--temp;
				}
				if(temp < posn)
				{
					temp = posn - temp;
					GoBack(temp);
					yankedString = new String(buffer, posn, temp);
					Delete(temp);
				}
			}

	// Move forward one word in the input line.
	private static void MoveForwardWord()
			{
				while(posn < length && !IsWordCharacter(buffer[posn]))
				{
					MoveRight();
				}
				while(posn < length && IsWordCharacter(buffer[posn]))
				{
					MoveRight();
				}
			}

	// Move backward one word in the input line.
	private static void MoveBackwardWord()
			{
				while(posn > 0 && !IsWordCharacter(buffer[posn - 1]))
				{
					MoveLeft();
				}
				while(posn > 0 && IsWordCharacter(buffer[posn - 1]))
				{
					MoveLeft();
				}
			}

	// Read the next line of input using line editing.  Returns "null"
	// if an EOF indication is encountered in the input.
	public static String ReadLine(String prompt)
			{
				ConsoleKeyInfo key;
				char ch;
				bool done;
				bool ctrlv;

				// Output the prompt.
				if(prompt != null)
				{
					Console.Write(prompt);
				}

				// Enter the main character input loop.
				posn = 0;
				length = 0;
				column = 0;
				lastColumn = 0;
				done = false;
				overwrite = false;
				historyPosn = -1;
				ctrlv = false;
				do
				{
					key = ConsoleExtensions.ReadKey(true);
					ch = key.KeyChar;
					if(ctrlv)
					{
						ctrlv = false;
						if((ch >= 0x0001 && ch <= 0x001F) || ch == 0x007F)
						{
							// Insert a control character into the buffer.
							AddChar(ch);
							continue;
						}
					}
					if(ch != '\0')
					{
						switch(ch)
						{
							case '\u0001':
							{
								// CTRL-A: move to the home position.
								MoveHome();
							}
							break;

							case '\u0002':
							{
								// CTRL-B: go back one character.
								MoveLeft();
							}
							break;

							case '\u0003':
							{
								// CTRL-C encountered in "raw" mode.
								CancelLine(prompt);
							}
							break;

							case '\u0004':
							{
								// CTRL-D: EOF or delete the current character.
								if(controlDIsEOF)
								{
									// Signal an EOF if the buffer is empty.
									if(length == 0)
									{
										EndLine();
										return null;
									}
								}
								else
								{
									Delete();
								}
							}
							break;

							case '\u0005':
							{
								// CTRL-E: move to the end position.
								MoveEnd();
							}
							break;

							case '\u0006':
							{
								// CTRL-F: go forward one character.
								MoveRight();
							}
							break;

							case '\u0007':
							{
								// CTRL-G: ring the terminal bell.
								Console.Beep();
							}
							break;

							case '\u0008': case '\u007F':
							{
								if(key.Key == ConsoleKey.Delete)
								{
									// Delete the character under the cursor.
									Delete();
								}
								else
								{
									// Delete the character before the cursor.
									Backspace();
								}
							}
							break;

							case '\u0009':
							{
								// Process a tab.
								Tab(prompt);
							}
							break;

							case '\u000A': case '\u000D':
							{
								// Line termination.
								EndLine();
								done = true;
							}
							break;

							case '\u000B':
							{
								// CTRL-K: erase until the end of the line.
								EraseToEnd();
							}
							break;

							case '\u000C':
							{
								// CTRL-L: clear screen and redraw.
								Console.Clear();
								Console.Write(prompt);
								Redraw();
							}
							break;

							case '\u000E':
							{
								// CTRL-N: move down in the history.
								MoveDown();
							}
							break;

							case '\u0010':
							{
								// CTRL-P: move up in the history.
								MoveUp();
							}
							break;

							case '\u0015':
							{
								// CTRL-U: erase to the start of the line.
								EraseToStart();
							}
							break;

							case '\u0016':
							{
								// CTRL-V: prefix a control character.
								ctrlv = true;
							}
							break;

							case '\u0017':
							{
								// CTRL-W: erase the previous word.
								EraseWord();
							}
							break;

							case '\u0019':
							{
								// CTRL-Y: yank the last erased string.
								if(yankedString != null)
								{
									foreach(char ch in yankedString)
									{
										AddChar(ch);
									}
								}
							}
							break;

							case '\u001A':
							{
								// CTRL-Z: Windows end of file indication.
								if(controlZIsEOF && length == 0)
								{
									EndLine();
									return null;
								}
							}
							break;

							case '\u001B':
							{
								// Escape is "clear line".
								Clear();
							}
							break;

							default:
							{
								if(ch >= ' ')
								{
									// Ordinary character.
									AddChar(ch);
								}
							}
							break;
						}
					}
					else if(key.Modifiers == (ConsoleModifiers)0)
					{
						switch(key.Key)
						{
							case ConsoleKey.BackSpace:
							{
								// Delete the character before the cursor.
								Backspace();
							}
							break;

							case ConsoleKey.Delete:
							{
								// Delete the character under the cursor.
								Delete();
							}
							break;

							case ConsoleKey.Enter:
							{
								// Line termination.
								EndLine();
								done = true;
							}
							break;

							case ConsoleKey.Escape:
							{
								// Clear the current line.
								Clear();
							}
							break;

							case ConsoleKey.Tab:
							{
								// Process a tab.
								Tab(prompt);
							}
							break;

							case ConsoleKey.LeftArrow:
							{
								// Move left one character.
								MoveLeft();
							}
							break;

							case ConsoleKey.RightArrow:
							{
								// Move right one character.
								MoveRight();
							}
							break;

							case ConsoleKey.UpArrow:
							{
								// Move up one line in the history.
								MoveUp();
							}
							break;

							case ConsoleKey.DownArrow:
							{
								// Move down one line in the history.
								MoveDown();
							}
							break;

							case ConsoleKey.Home:
							{
								// Move to the beginning of the line.
								MoveHome();
							}
							break;

							case ConsoleKey.End:
							{
								// Move to the end of the line.
								MoveEnd();
							}
							break;

							case ConsoleKey.Insert:
							{
								// Toggle insert/overwrite mode.
								overwrite = !overwrite;
							}
							break;
						}
					}
					else if((key.Modifiers & ConsoleModifiers.Alt) != 0)
					{
						switch(key.Key)
						{
							case ConsoleKey.F:
							{
								// ALT-F: move forward a word.
								MoveForwardWord();
							}
							break;

							case ConsoleKey.B:
							{
								// ALT-B: move backward a word.
								MoveBackwardWord();
							}
							break;

							case ConsoleKey.D:
							{
								// ALT-D: erase until the end of the word.
								EraseToEndWord();
							}
							break;

							case ConsoleKey.BackSpace:
							case ConsoleKey.Delete:
							{
								// ALT-DEL: erase until the start of the word.
								EraseToStartWord();
							}
							break;
						}
					}
				}
				while(!done);
				if(length == 0 && enterIsDuplicate)
				{
					if(History.Count > 0)
					{
						return History.GetHistory(0);
					}
				}
				return new String(buffer, 0, length);
			}

#else // !CONFIG_EXTENDED_CONSOLE

	// We don't have the extended console, so fall back
	// to the old-fashioned console input mechanisms.

	// Read the next line of input using line editing.  Returns "null"
	// if an EOF indication is encountered in the input.
	public static String ReadLine(String prompt)
			{
				if(prompt != null)
				{
					Console.Write(prompt);
				}
				String line = Console.ReadLine();
				if(line != null && line.Length == 0 && enterIsDuplicate)
				{
					if(History.Count > 0)
					{
						line = History.GetHistory(0);
					}
				}
				return line;
			}

#endif // !CONFIG_EXTENDED_CONSOLE

	// Get or set a flag that indicates if pressing the "Enter" key on an
	// empty line causes the most recent history line to be duplicated.
	public static bool EnterIsDuplicate
			{
				get
				{
					return enterIsDuplicate;
				}
				set
				{
					enterIsDuplicate = value;
				}
			}

	// Get or set a flag that indicates if CTRL-D is an EOF indication
	// or the "delete character" key.  The default is true (i.e. EOF).
	public static bool ControlDIsEOF
			{
				get
				{
					return controlDIsEOF;
				}
				set
				{
					controlDIsEOF = value;
				}
			}

	// Get or set a flag that indicates if CTRL-Z is an EOF indication.
	// The default is true on Windows system, false otherwise.
	public static bool ControlZIsEOF
			{
				get
				{
					return controlZIsEOF;
				}
				set
				{
					controlZIsEOF = value;
				}
			}

	// Event that is emitted to allow for tab completion.  If there are
	// no attached handlers, then the Tab key will do normal tabbing.
	public static event TabCompleteEventHandler TabComplete;

}; // class Readline

}; // namespace DotGNU.Terminal
