/*
 * ConsoleKeyInfo.cs - Implementation of the "System.ConsoleKeyInfo" class.
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

namespace System
{

#if CONFIG_EXTENDED_CONSOLE

public struct ConsoleKeyInfo
{
	// Internal state.
	private char keyChar;
	private ConsoleKey key;
	private ConsoleModifiers modifiers;

	// Constructors.
	public ConsoleKeyInfo(char keyChar, ConsoleKey key,
						  bool shift, bool alt, bool control)
			{
				this.keyChar = keyChar;
				this.key = key;
				this.modifiers =
					(shift ? ConsoleModifiers.Shift : 0) |
					(alt ? ConsoleModifiers.Alt : 0) |
					(control ? ConsoleModifiers.Control : 0);
			}
	internal ConsoleKeyInfo
				(char keyChar, ConsoleKey key, ConsoleModifiers modifiers)
			{
				this.keyChar = keyChar;
				this.key = key;
				this.modifiers = modifiers;
			}

	// Get this structure's properties.
	public char KeyChar
			{
				get
				{
					return keyChar;
				}
			}
	public ConsoleKey Key
			{
				get
				{
					return key;
				}
			}
	public ConsoleModifiers Modifiers
			{
				get
				{
					return modifiers;
				}
			}

}; // struct ConsoleKeyInfo

#endif // CONFIG_EXTENDED_CONSOLE

}; // namespace System
