/*
 * SerialPinChangedEventArgs.cs - Implementation of the
 *		"System.IO.Ports.SerialPinChangedEventArgs" class.
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

namespace System.IO.Ports
{

#if CONFIG_SERIAL_PORTS

public class SerialPinChangedEventArgs : EventArgs
{
	// Internal state.
	private SerialPinChanges eventType;
	private bool newValue;

	// Constructor.
	internal SerialPinChangedEventArgs(SerialPinChanges eventType,
									   bool newValue)
			{
				this.eventType = eventType;
				this.newValue = newValue;
			}

	// Properties.
	public SerialPinChanges EventType
			{
				get
				{
					return eventType;
				}
				set
				{
					eventType = value;
				}
			}
	public bool NewValue
			{
				get
				{
					return newValue;
				}
				set
				{
					newValue = value;
				}
			}

}; // class SerialPinChangedEventArgs

#endif // CONFIG_SERIAL_PORTS

}; // namespace System.IO.Ports
