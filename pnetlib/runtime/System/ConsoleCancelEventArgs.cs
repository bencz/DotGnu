/*
 * ConsoleCancelEventArgs.cs - Implementation of the
 *			"System.ConsoleCancelEventArgs" class.
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

public class ConsoleCancelEventArgs : EventArgs
{
	// Internal state.
	private bool cancel;
	private ConsoleSpecialKey specialKey;

	// Constructor.
	internal ConsoleCancelEventArgs(ConsoleSpecialKey specialKey)
			{
				this.cancel = false;
				this.specialKey = specialKey;
			}

	// Get or set this object's properties.
	public bool Cancel
			{
				get
				{
					return cancel;
				}
				set
				{
					cancel = value;
				}
			}
	public ConsoleSpecialKey SpecialKey
			{
				get
				{
					return specialKey;
				}
			}
	public ConsoleSpecialKeys SpecialKeys
			{
				get
				{
					// This property is obsolete - do not use.
					return (ConsoleSpecialKeys)specialKey;
				}
			}

}; // class ConsoleCancelEventArgs

#endif // CONFIG_EXTENDED_CONSOLE

}; // namespace System
