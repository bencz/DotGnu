/*
 * KeyEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.KeyEventArgs" class.
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
public class KeyEventArgs : EventArgs
{
	// Internal state.
	private Keys keyData;
	private bool handled;

	// Constructor.
	public KeyEventArgs(Keys keyData)
			{
				this.keyData = keyData;
			}

	// Determine if certain special keys were pressed.
	public virtual bool Alt
			{
				get
				{
					return ((keyData & Keys.Alt) != 0);
				}
			}
	public bool Control
			{
				get
				{
					return ((keyData & Keys.Control) != 0);
				}
			}
	public virtual bool Shift
			{
				get
				{
					return ((keyData & Keys.Shift) != 0);
				}
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

	// Get the key code.
	public Keys KeyCode
			{
				get
				{
					return (keyData & Keys.KeyCode);
				}
			}

	// Get the full key data.
	public Keys KeyData
			{
				get
				{
					return keyData;
				}
			}

	// Get the key code as an integer.
	public int KeyValue
			{
				get
				{
					return (int)(keyData & Keys.KeyCode);
				}
			}

	// Get the modifier flags.
	public Keys Modifiers
			{
				get
				{
					return (keyData & Keys.Modifiers);
				}
			}

}; // class KeyEventArgs

}; // namespace System.Windows.Forms
