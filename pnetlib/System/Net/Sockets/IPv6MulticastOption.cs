/*
 * IPv6MulticastOption.cs - Implementation of the
 *			"System.Net.Sockets.IPv6MulticastOption" class.
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

// Not ECMA-compatible, strictly-speaking, but necessary for IPv6 support.

public class IPv6MulticastOption
{
	// Internal state.
	private IPAddress group;
	private long ifindex;

	// Constructors.
	public IPv6MulticastOption(IPAddress group, long ifindex)
			{
				if(group == null)
				{
					throw new ArgumentNullException("group");
				}
				this.group = group;
				InterfaceIndex = ifindex;
			}
	public IPv6MulticastOption(IPAddress group)
			{
				if(group == null)
				{
					throw new ArgumentNullException("group");
				}
				this.group = group;
				this.ifindex = 0;
			}

	// Get or set the multicast properties.
	public IPAddress Group
			{
				get
				{
					return group;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					group = value;
				}
			}
	public long InterfaceIndex
			{
				get
				{
					return ifindex;
				}
				set
				{
					if(value < 0 || value > (long)(UInt32.MaxValue))
					{
						throw new ArgumentOutOfRangeException
							("value", S._("ArgRange_InterfaceIndex"));
					}
					ifindex = value;
				}
			}

}; // class IPv6MulticastOption

}; // namespace System.Net.Sockets
