/*
 * CancelEventArgs.cs - Implementation of the
 *			"System.ComponentModel.CancelEventArgs" class.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL

public class CancelEventArgs : EventArgs
{
	// Internal state.
	private bool cancel;

	// Constructors.
	public CancelEventArgs()
			{
				cancel = false;
			}
	public CancelEventArgs(bool cancel)
			{
				this.cancel = cancel;
			}
	
	// Get or set the cancel state.
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

}; // class CancelEventArgs

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
