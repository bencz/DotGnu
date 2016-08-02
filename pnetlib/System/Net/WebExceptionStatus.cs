/*
 * WebExceptionStatus.cs - Implementation of the
 *			"System.Net.Sockets.WebExceptionStatus" class.
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

namespace System.Net
{

public enum WebExceptionStatus
{
	Success						= 0,
	NameResolutionFailure		= 1,
	ConnectFailure				= 2,
	ReceiveFailure				= 3,
	SendFailure					= 4,
	PipelineFailure				= 5,
	RequestCanceled				= 6,
	ProtocolError				= 7,
	ConnectionClosed			= 8,
	TrustFailure				= 9,
	SecureChannelFailure		= 10,
	ServerProtocolViolation		= 11,
	KeepAliveFailure			= 12,
	Pending						= 13,
	Timeout						= 14,
	ProxyNameResolutionFailure	= 15,
	UnknownError				= 16,
	MessageLengthLimitExceeded	= 17,

}; // enum WebExceptionStatus

}; // namespace System.Net
