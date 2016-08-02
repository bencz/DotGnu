/*
 * ConsoleExtensions.cs - Implementation of the
 *			"DotGNU.Terminal.ConsoleExtensions" class.
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

namespace DotGNU.Terminal
{

#if CONFIG_EXTENDED_CONSOLE

using System;

// This class contains some extensions for console input on Unix systems.

public sealed class ConsoleExtensions
{
	// Cannot instantiate this class.
	private ConsoleExtensions() {}

	// Read a key while processing window resizes and process resumption.
	public static ConsoleKeyInfo ReadKey()
			{
				return ReadKey(false);
			}
	public static ConsoleKeyInfo ReadKey(bool intercept)
			{
				ConsoleKeyInfo key = Console.ReadKey(intercept);
				if(key.Key == (ConsoleKey)0x1200)
				{
					// "SizeChanged" key indication.
					if(SizeChanged != null)
					{
						SizeChanged(null, EventArgs.Empty);
					}
				}
				else if(key.Key == (ConsoleKey)0x1201)
				{
					// "Resumed" key indication.
					if(Resumed != null)
					{
						Resumed(null, EventArgs.Empty);
					}
				}
				return key;
			}

	// Event that is emitted when the console window size changes.
	public static event EventHandler SizeChanged;

	// Event that is emitted when the program is resumed after a suspend.
	public static event EventHandler Resumed;

}; // class ConsoleExtensions

#endif // CONFIG_EXTENDED_CONSOLE

}; // namespace DotGNU.Terminal
