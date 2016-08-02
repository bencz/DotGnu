/*
 * ProtocolType.cs - Implementation of the
 *			"System.Net.Sockets.ProtocolType" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace System.Net.Sockets
{

public enum ProtocolType
{
	Unknown			= -1,
	Unspecified		= 0,
	IP				= 0,
	Icmp			= 1,
	Igmp			= 2,
	Ggp				= 3,
	Tcp				= 6,
	Pup				= 12,
	Udp				= 17,
	Idp				= 22,
	IPv6			= 41,
	ND				= 77,
	Raw				= 255,
	Ipx				= 1000,
	Spx				= 1256,
	SpxII			= 1257

}; // enum ProtocolType

}; // namespace System.Net.Sockets
