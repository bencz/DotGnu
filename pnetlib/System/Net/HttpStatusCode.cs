/*
 * HttpStatusCode.cs - Implementation of the
 *			"System.Net.HttpStatusCode" class.
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

namespace System.Net
{

public enum HttpStatusCode
{
	Continue						= 100,
	SwitchingProtocols				= 101,
	OK								= 200,
	Created							= 201,
	Accepted						= 202,
	NonAuthoritativeInformation		= 203,
	NoContent						= 204,
	ResetContent					= 205,
	PartialContent					= 206,
	Ambiguous						= 300,
	MultipleChoices					= 300,
	Moved							= 301,
	MovedPermanently				= 301,
	Found							= 302,
	Redirect						= 302,
	RedirectMethod					= 303,
	SeeOther						= 303,
	NotModified						= 304,
	UseProxy						= 305,
	Unused							= 306,
	RedirectKeepVerb				= 307,
	TemporaryRedirect				= 307,
	BadRequest						= 400,
	Unauthorized					= 401,
	PaymentRequired					= 402,
	Forbidden						= 403,
	NotFound						= 404,
	MethodNotAllowed				= 405,
	NotAcceptable					= 406,
	ProxyAuthenticationRequired		= 407,
	RequestTimeout					= 408,
	Conflict						= 409,
	Gone							= 410,
	LengthRequired					= 411,
	PreconditionFailed				= 412,
	RequestEntityTooLarge			= 413,
	RequestUriTooLong				= 414,
	UnsupportedMediaType			= 415,
	RequestedRangeNotSatisfiable	= 416,
	ExpectationFailed				= 417,
	InternalServerError				= 500,
	NotImplemented					= 501,
	BadGateway						= 502,
	ServiceUnavailable				= 503,
	GatewayTimeout					= 504,
	HttpVersionNotSupported			= 505

}; // enum HttpStatusCode

}; // namespace System.Net
