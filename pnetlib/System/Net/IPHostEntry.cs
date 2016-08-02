/*
 * IPHostEntry.cs - Implementation of the "System.Net.IPHostEntry" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Net
{

using System;

public class IPHostEntry
{
	// Internal state.  These fields must be here in precisely
	// this order so that the engine knows how to fill them in.
	private IPAddress[] addressList;
	private String[] aliases;
	private String hostName;

	// Constructor.
	public IPHostEntry() {}

	// Get or set the list of addresses associated with this entry.
	public IPAddress[] AddressList
			{
				get
				{
					return addressList;
				}
				set
				{
					addressList = value;
				}
			}

	// Get or set the list of aliases associated with this entry.
	public String[] Aliases
			{
				get
				{
					return aliases;
				}
				set
				{
					aliases = value;
				}
			}

	// Get or set the hostname associated with this entry.
	public String HostName
			{
				get
				{
					return hostName;
				}
				set
				{
					hostName = value;
				}
			}

}; // class IPHostEntry

}; // namespace System.Net
