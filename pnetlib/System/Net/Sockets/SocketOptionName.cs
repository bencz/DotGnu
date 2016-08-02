/*
 * SocketOptionName.cs - Implementation of the
 *			"System.Net.Sockets.SocketOptionName" class.
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

public enum SocketOptionName
{
	// Options for SocketOptionLevel.IP.
	IPOptions				= 0x00000001,
	HeaderIncluded			= 0x00000002,
	TypeOfService			= 0x00000003,
	IpTimeToLive			= 0x00000004,
	MulticastInterface		= 0x00000009,
	MulticastTimeToLive		= 0x0000000A,
	MulticastLoopback		= 0x0000000B,
	AddMembership			= 0x0000000C,
	DropMembership			= 0x0000000D,
	DontFragment			= 0x0000000E,
	AddSourceMembership		= 0x0000000F,
	DropSourceMembership	= 0x00000010,
	BlockSource				= 0x00000011,
	UnblockSource			= 0x00000012,
	PacketInformation		= 0x00000013,

	// Options for SocketOptionLevel.Tcp.
	NoDelay					= 0x00000001,
	BsdUrgent				= 0x00000002,
	Expedited				= 0x00000002,

	// Options for SocketOptionLevel.Udp.
	NoChecksum				= 0x00000001,
	ChecksumCoverage		= 0x00000014,

	// Options for SocketOptionLevel.Socket.
	Debug					= 0x00000001,
	AcceptConnection		= 0x00000002,
	ReuseAddress			= 0x00000004,
	KeepAlive				= 0x00000008,
	DontRoute				= 0x00000010,
	Broadcast				= 0x00000020,
	UseLoopback				= 0x00000040,
	Linger					= 0x00000080,
	OutOfBandInline			= 0x00000100,
	SendBuffer				= 0x00001001,
	ReceiveBuffer			= 0x00001002,
	SendLowWater			= 0x00001003,
	ReceiveLowWater			= 0x00001004,
	SendTimeout				= 0x00001005,
	ReceiveTimeout			= 0x00001006,
	Error					= 0x00001007,
	Type					= 0x00001008,
	MaxConnections			= 0x7FFFFFFF,
	DontLinger				= unchecked((int)0xFFFFFF7F),
	ExclusiveAddressUse		= unchecked((int)0xFFFFFFFB)

}; // enum SocketOptionName

}; // namespace System.Net.Sockets
