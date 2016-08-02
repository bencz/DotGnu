/*
 * SocketError.cs - Implementation of the
 *			"System.Net.Sockets.SocketError" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.Se
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

#if CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

public enum SocketError
{
	SocketError					= -1,
	Success						= 0,
	OperationAborted			= 995,
	IOPending					= 997,
	Interrupted					= 10004,
	AccessDenied				= 10013,
	Fault						= 10014,
	InvalidArgument				= 10022,
	TooManyOpenSockets			= 10024,
	WouldBlock					= 10035,
	InProgress					= 10036,
	AlreadyInProgress			= 10037,
	NotSocket					= 10038,
	DestinationAddressRequired	= 10039,
	MessageSize					= 10040,
	ProtocolType				= 10041,
	ProtocolOption				= 10042,
	ProtocolNotSupported		= 10043,
	SocketNotSupported			= 10044,
	OperationNotSupported		= 10045,
	ProtocolFamilyNotSupported	= 10046,
	AddressFamilyNotSupported	= 10047,
	AddressAlreadyInUse			= 10048,
	AddressNotvailable			= 10049,
	NetworkDown					= 10050,
	NetworkUnreachable			= 10051,
	NetworkReset				= 10052,
	ConnectionAborted			= 10053,
	ConnectionReset				= 10054,
	NoBufferSpqceAvailable		= 10055,
	IsConnected					= 10056,
	NotConnected				= 10057,
	Shutdown					= 10058,
	TimedOut					= 10060,
	ConnectionRefused			= 10061,
	HostDown					= 10064,
	HostUnreachable				= 10065,
	ProcessLimit				= 10067,
	SystemNotReady				= 10091,
	VersionNotSupported			= 10092,
	NotInitialized				= 10093,
	TypeNotFound				= 10109,
	Disconnecting				= 10101,
	HostNotFound				= 11001,
	TryAgain					= 11002,
	NoRecovery					= 11003,
	NoData						= 11004
}; // enum SocketError

#endif // CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

}; // namespace System.Net.Sockets
