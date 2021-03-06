/*
 * AddressFamily.cs - Implementation of the
 *			"System.Net.Sockets.AddressFamily" class.
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

public enum AddressFamily
{
	Unknown				= -1,
	Unspecified			= 0,
	Unix				= 1,
	InterNetwork		= 2,
	ImpLink				= 3,
	Pup					= 4,
	Chaos				= 5,
	Ipx					= 6,
	NS					= Ipx,
	Iso					= 7,
	Osi					= Iso,
	Ecma				= 8,
	DataKit				= 9,
	Ccitt				= 10,
	Sna					= 11,
	DecNet				= 12,
	DataLink			= 13,
	Lat					= 14,
	HyperChannel		= 15,
	AppleTalk			= 16,
	NetBios				= 17,
	VoiceView			= 18,
	FireFox				= 19,
	Banyan				= 21,
	Atm					= 22,
	InterNetworkV6		= 23,
	Cluster				= 24,
	Ieee12844			= 25,
	Irda				= 26,
	NetworkDesigners	= 28,
	Max					= 29

}; // enum AddressFamily

}; // namespace System.Net.Sockets
