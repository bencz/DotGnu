/*
 * HttpRequestHeader.cs - Implementation of the "System.Net.HttpRequestHeader" class.
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

public enum HttpRequestHeader
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
	Accept				= 20,
	AcceptCharset		= 21,
	AcceptEncoding		= 22,
	AcceptLanguage		= 23,
	Authorization		= 24,
	Cookie				= 25,
	Expect				= 26,
	From				= 27,
	Host				= 28,
	IfMatch				= 29,
	IfModifiedSince		= 30,
	IfNoneMatch			= 31,
	IfRange				= 32,
	IfUnmodifiedSince	= 33,
	MaxForwards			= 34,
	ProxyAuthorization	= 35,
	Referer				= 36,
	Range				= 37,
	Te					= 38,
	Translate			= 39,
	UserAgent			= 40
}; // enum HttpRequestHeader

#endif // CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

}; // namespace System.Net
