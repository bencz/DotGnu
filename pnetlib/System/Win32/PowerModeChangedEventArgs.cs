/*
 * PowerModeChangedEventArgs.cs - Implementation of the
 *		Microsoft.Win32.PowerModeChangedEventArgs class.
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

namespace Microsoft.Win32
{

using System;

#if CONFIG_WIN32_SPECIFICS

public class PowerModeChangedEventArgs : EventArgs
{
	// Internal state.
	private PowerModes mode;

	// Constructor.
	public PowerModeChangedEventArgs(PowerModes mode)
			{
				this.mode = mode;
			}

	// Get the power mode.
	public PowerModes Mode
			{
				get
				{
					return mode;
				}
			}

}; // class PowerModeChangedEventArgs

#endif // CONFIG_WIN32_SPECIFICS

}; // namespace Microsoft.Win32
