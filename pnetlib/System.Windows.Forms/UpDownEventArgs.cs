/*
 * UpDownEventArgs.cs - Implementation of the
 *			"System.Windows.Forms.UpDownEventArgs" class.
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

public class UpDownEventArgs : EventArgs
{
	// Internal state.
	private int buttonID;

	// Constructor.
	public UpDownEventArgs(int buttonPushed)
	{
		this.buttonID = buttonPushed;
	}

	// Get the id of the pushed button.
	public int ButtonID
	{
		get
		{
			return buttonID;
		}
	}

}; // class UpDownEventArgs

}; // namespace System.Windows.Forms
