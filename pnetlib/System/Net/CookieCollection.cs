/*
 * CookieCollection.cs - Implementation of the
 *			"System.Net.CookieCollection" class.
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
public class CookieCollection : ICollection, IEnumerable
{
	// Internal state.
	private ArrayList list;

	// Constructor.
	public CookieCollection()
			{
				list = new ArrayList();
			}

	// Determine if this collection is read-only.
	public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

	// Get a specific item from this cookie collection.
	public Cookie this[int index]
			{
				get
				{
					return (Cookie)(list[index]);
				}
			}
	public Cookie this[String name]
			{
				get
				{
					if(name == null)
					{
						throw new ArgumentNullException("name");
					}
					foreach(Cookie cookie in list)
					{
						if(String.Compare(cookie.Name, name, true,
										  CultureInfo.InvariantCulture) == 0)
						{
							return cookie;
						}
					}
					return null;
				}
			}

	// Implement the ICollection interface.
	public void CopyTo(Array array, int index)
			{
				list.CopyTo(array, index);
			}
	public int Count
			{
				get
				{
					return list.Count;
				}
			}
	public bool IsSynchronized
			{
				get
				{
					return false;
				}
			}
	public Object SyncRoot
			{
				get
				{
					return this;
				}
			}

	// Implement the IEnumerable interface.
	public IEnumerator GetEnumerator()
			{
				return list.GetEnumerator();
			}

	// Add a cookie to this collection.
	public void Add(Cookie cookie)
			{
				if(cookie == null)
				{
					throw new ArgumentNullException("cookie");
				}
				int index = list.IndexOf(cookie);
				if(index != -1)
				{
					// Replace an the existing cookie with the same
					// name with a new cookie.
					list[index] = cookie;
				}
				else
				{
					list.Add(cookie);
				}
			}

	// Add the contents of another cookie collection to this one.
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

}; // class CookieCollection

#endif // !ECMA_COMPAT

}; // namespace System.Net
