/*
 * LingerOption.cs - Implementation of the
 *			"System.Net.Sockets.LingerOption" class.
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

namespace System.Net.Sockets
{

public class LingerOption
{
	// Internal state.
	private bool enabled;
	private int seconds;

	// Constructor.
	public LingerOption(bool enabled, int seconds)
			{
				this.enabled = enabled;
				this.seconds = seconds;
			}

	// Get or set the linger properties.
	public bool Enabled
			{
				get
				{
					return enabled;
				}
				set
				{
					enabled = value;
				}
			}
	public int LingerTime
			{
				get
				{
					return seconds;
				}
				set
				{
					seconds = value;
				}
			}

}; // class LingerOption

}; // namespace System.Net.Sockets
