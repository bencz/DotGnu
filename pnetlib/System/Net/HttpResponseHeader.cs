/*
 * HttpResponseHeader.cs - Implementation of the "System.Net.HttpResponseHeader" class.
 *
 * Copyright (C) 2007  Southern Storm Software, Pty Ltd.
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

#if CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

public enum HttpResponseHeader
{
	CacheControl		= 0,
	Connection			= 1,
	Date				= 2,
	KeepAlive			= 3,
	Pragma				= 4,
	Trailer				= 5,
	TransferEncoding	= 6,
	Upgrade				= 7,
	Via					= 8,
	Warning				= 9,
	Allow				= 10,
	ContentLength		= 11,
	ContentType			= 12,
	ContentEncoding		= 13,
	ContentLanguage		= 14,
	ContentLocation		= 15,
	ContentMd5			= 16,
	ContentRange		= 17,
	Expires				= 18,
	LastModified		= 19,
	AcceptRanges		= 20,
	Age					= 21,
	ETag				= 22,
	Location			= 23,
	ProxyAuthenticate	= 24,
	RetryAfter			= 25,
	Server				= 26,
	SetCookie			= 27,
	Vary				= 28,
	WwwAuthenticate		= 29
}; // enum HttpResponseHeader

#endif // CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

}; // namespace System.Net
