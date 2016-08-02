/*
 * MulticastOption.cs - Implementation of the
 *			"System.Net.Sockets.MulticastOption" class.
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

public class MulticastOption
{
	// Internal state.
	private IPAddress group;
	private IPAddress mcint;

	// Constructors.
	public MulticastOption(IPAddress group, IPAddress mcint)
			{
				if(group == null)
				{
					throw new ArgumentNullException("group");
				}
				if(mcint == null)
				{
					throw new ArgumentNullException("mcint");
				}
				this.group = group;
				this.mcint = mcint;
			}
	public MulticastOption(IPAddress group)
			{
				if(group == null)
				{
					throw new ArgumentNullException("group");
				}
				this.group = group;
				this.mcint = IPAddress.Any;
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
					group = value;
				}
			}
	public IPAddress LocalAddress
			{
				get
				{
					return mcint;
				}
				set
				{
					mcint = value;
				}
			}

}; // class MulticastOption

}; // namespace System.Net.Sockets
