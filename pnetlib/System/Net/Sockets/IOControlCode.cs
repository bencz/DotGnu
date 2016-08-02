/*
 * IOControlCode.cs - Implementation of the
 *			"System.Net.Sockets.IOControlCode" class.
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

#if CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

public enum IOControlCode
{
	EnableCircularQueuing				= 671088642,
	Flush								= 671088644,
	AddressListChange					= 671088663,
	DataToRead							= 1074030207,
	OobDataRead							= 1074033415,
	GetBroadcastAddress					= 1207959557,
	AddressListQuery					= 1207959574,
	QueryTargetPnpHandle				= 1207959576,
	AsyncIO								= 2127772029,
	NonBlockingIO						= 2147772030,
	AssociateHandle						= 2281701377,
	MultipointLoopback					= 2281701385,
	MulticastScope						= 2281701386,
	SetQos								= 2281701387,
	SetGroupQos							= 2281701388,
	RoutingInterfaceChange				= 2281701397,
	NamespaceChange						= 2281701401,
	ReceiveAll							= 2550136833,
	ReceiveAllMulticast					= 2550136834,
	ReceiveAllIgmpMulticast				= 2550136835,
	KeepAliveValues						= 2550136836,
	AbsorbRouterAllert					= 2550136837,
	UnicastInterface					= 2550136838,
	LimitBroadcasts						= 2550136839,
	BindToInterface						= 2550136840,
	MulticastInterface					= 2550136841,
	AddMulticastGroupOnInterface		= 2550136842,
	DeleteMulticastGroupFromInterface	= 2550136843,
	GetExtensionFunctionPointer			= 3355443206,
	GetQos								= 3355443207,
	GetGroupQos							= 3355443208,
	TranslateHandle						= 3355443213,
	RoutingInterfaceQuery				= 3355443220,
	AddressListSort						= 3355443225
}; // enum IOControlCode

#endif // CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

}; // namespace System.Net.Sockets
