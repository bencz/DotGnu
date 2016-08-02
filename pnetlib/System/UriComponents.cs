/*
 * UriComponents.cs - Implementation of the
 *								"System.UriComponents" enumeration.
 *
 * Copyright (C) 2009  Free Software Foundation Inc.
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

namespace System
{

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0

[Flags]
public enum UriComponents
{

	Scheme					= 0x01,
	UserInfo				= 0x02,
	Host					= 0x04,
	Port					= 0x08,
	SchemeAndServer			= Scheme | Host | Port,
	Path					= 0x10,
	Query					= 0x20,
	PathAndQuery			= Path | Query,
	HttpRequestUrl			= Scheme | Host | Port | Path | Query,
	Fragment				= 0x40,
	AbsoluteUri				= Scheme | UserInfo | Host | Port | Path | Query |
							  Fragment,
	StrongPort				= 0x80,
	HostAndPort				= StrongPort | Host,
	StrongAuthority			= StrongPort | Host | UserInfo,
	KeepDelimiter			= 0x40000000,
	SerializationInfoString	= 0x80000000

}; // enum UriComponents

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0

}; // namespace System
