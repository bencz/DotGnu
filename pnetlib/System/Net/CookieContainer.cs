/*
 * CookieContainer.cs - Implementation of the
 *			"System.Net.CookieContainer" class.
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

#if !ECMA_COMPAT

using System.Collections;
using System.Globalization;

[Serializable]
public class CookieContainer
{
	// Internal state.
	private int capacity;
	private int perDomainCapacity;
	private int maxCookieSize;
	private int count;

	// Default values.
	public const int DefaultCookieLengthLimit		= 4096;
	public const int DefaultCookieLimit				= 300;
	public const int DefaultPerDomainCookieLimit	= 20;

	// Constructors.
	public CookieContainer()
			: this(DefaultCookieLimit, DefaultPerDomainCookieLimit,
				   DefaultCookieLengthLimit) {}
	public CookieContainer(int capacity)
			: this(capacity, DefaultPerDomainCookieLimit,
				   DefaultCookieLengthLimit) {}
	public CookieContainer(int capacity, int perDomainCapacity,
						   int maxCookieSize)
			{
				if(capacity <= 0 ||
				   (perDomainCapacity != Int32.MaxValue &&
				    capacity < perDomainCapacity))
				{
					throw new ArgumentException
						(S._("Arg_CookieCapacity"), "capacity");
				}
				if(perDomainCapacity <= 0)
				{
					throw new ArgumentException
						(S._("ArgRange_PositiveNonZero"),
						 "perDomainCapacity");
				}
				if(maxCookieSize <= 0)
				{
					throw new ArgumentException
						(S._("ArgRange_PositiveNonZero"),
						 "maxCookieSize");
				}
				this.capacity = capacity;
				this.perDomainCapacity = perDomainCapacity;
				this.maxCookieSize = maxCookieSize;
				this.count = 0;
			}

	// Get or set this object's properties.
	public int Capacity
			{
				get
				{
					return capacity;
				}
				set
				{
					if(value <= 0 ||
					   (perDomainCapacity != Int32.MaxValue &&
					    value < perDomainCapacity))
					{
						throw new ArgumentOutOfRangeException
							(S._("Arg_CookieCapacity"));
					}
					capacity = value;
				}
			}
	public int Count
			{
				get
				{
					return count;
				}
			}
	public int MaxCookieSize
			{
				get
				{
					return maxCookieSize;
				}
				set
				{
					if(value <= 0)
					{
						throw new ArgumentException
						 	("value", S._("ArgRange_PositiveNonZero"));
					}
					maxCookieSize = value;
				}
			}
	public int PerDomainCapacity
			{
				get
				{
					return perDomainCapacity;
				}
				set
				{
					if(value <= 0 ||
					   (value != Int32.MaxValue && capacity < value))
					{
						throw new ArgumentOutOfRangeException
							(S._("Arg_CookieCapacity"));
					}
					perDomainCapacity = value;
				}
			}

	// Add a cookie to this container.
	[TODO]
	public void Add(Cookie cookie)
			{
				if(cookie == null)
				{
					throw new ArgumentNullException("cookie");
				}
				// TODO
			}
	[TODO]
	public void Add(Uri uri, Cookie cookie)
			{
				if(uri == null)
				{
					throw new ArgumentNullException("uri");
				}
				if(cookie == null)
				{
					throw new ArgumentNullException("cookie");
				}
				// TODO
			}

	// Add a collection of cookies to this container.
	public void Add(CookieCollection cookies)
			{
				if(cookies == null)
				{
					throw new ArgumentNullException("cookies");
				}
				foreach(Cookie cookie in cookies)
				{
					Add(cookie);
				}
			}
	public void Add(Uri uri, CookieCollection cookies)
			{
				if(uri == null)
				{
					throw new ArgumentNullException("uri");
				}
				if(cookies == null)
				{
					throw new ArgumentNullException("cookies");
				}
				foreach(Cookie cookie in cookies)
				{
					Add(uri, cookie);
				}
			}

	// Get a HTTP cookie header for a particular URI.
	[TODO]
	public String GetCookieHeader(Uri uri)
			{
				if(uri == null)
				{
					throw new ArgumentNullException("uri");
				}
				// TODO
				return null;
			}

	// Get the cookies for a specific URI.
	[TODO]
	public CookieCollection GetCookies(Uri uri)
			{
				if(uri == null)
				{
					throw new ArgumentNullException("uri");
				}
				// TODO
				return null;
			}

	// Set the cookies for a specific URI.
	[TODO]
	public void SetCookies(Uri uri, String cookieHeader)
			{
				if(uri == null)
				{
					throw new ArgumentNullException("uri");
				}
				if(cookieHeader == null)
				{
					throw new ArgumentNullException("cookieHeader");
				}
				// TODO
			}

}; // class CookieContainer

#endif // !ECMA_COMPAT

}; // namespace System.Net
