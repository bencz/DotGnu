/*
 * KeyPressEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.KeyPressEventArgs" class.
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

namespace System.Windows.Forms
{

using System.Runtime.InteropServices;

#if !ECMA_COMPAT && !CONFIG_COMPACT_FORMS
[ComVisible(true)]
#endif
public class KeyPressEventArgs : EventArgs
{
	// Internal state.
	private char keyChar;
	private bool handled;

	// Constructor.
	public KeyPressEventArgs(char keyChar)
			{
				this.keyChar = keyChar;
			}

	// Determine whether this event has been handled yet or not.
	public bool Handled
			{
				get
				{
					return handled;
				}
				set
				{
					handled = value;
				}
			}

	// Get the key character.
	public char KeyChar
			{
				get
				{
					return keyChar;
				}
			}

}; // class KeyPressEventArgs

}; // namespace System.Windows.Forms
