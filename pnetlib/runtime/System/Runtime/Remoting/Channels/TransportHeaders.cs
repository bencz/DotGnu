/*
 * TransportHeaders.cs - Implementation of the
 *			"System.Runtime.Remoting.Channels.TransportHeaders" class.
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

namespace System.Runtime.Remoting.Channels
{

#if CONFIG_REMOTING

using System.Collections;

[Serializable]
public class TransportHeaders : ITransportHeaders
{
	// Internal state.
	private ArrayList headers;

	// Constructor.
	public TransportHeaders()
			{
				// Use an array list to preserve header ordering.
				headers = new ArrayList();
			}

	// Get or set a header value.
	public Object this[Object key]
			{
				get
				{
					foreach(DictionaryEntry entry in headers)
					{
						if(entry.Key.Equals(key))
						{
							return entry.Value;
						}
					}
					return null;
				}
				set
				{
					if(key != null)
					{
						int posn;
						DictionaryEntry entry;
						for(posn = 0; posn < headers.Count; ++posn)
						{
							entry = (DictionaryEntry)(headers[posn]);
							if(entry.Key.Equals(key))
							{
								headers.RemoveAt(posn);
								break;
							}
						}
						headers.Add(new DictionaryEntry(key, value));
					}
				}
			}

	// Get an enumerator for the header list.
	public IEnumerator GetEnumerator()
			{
				return headers.GetEnumerator();
			}

}; // class TransportHeaders

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Channels
