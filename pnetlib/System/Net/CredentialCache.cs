/*
 * CredentialCache.cs - Implementation of the
 *			"System.Net.CredentialCache" class.
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

using System.Collections;
using System.Globalization;

public class CredentialCache : ICredentials, IEnumerable
{
	// Internal state.
	private static NetworkCredential defaultCredentials;
	private CredentialInfo list;

	// Information about a specific credential.
	private class CredentialInfo
	{
		public Uri uriPrefix;
		public String authType;
		public NetworkCredential cred;
		public CredentialInfo next;

	}; // class CredentialInfo

	// Constructor.
	public CredentialCache()
			{
				list = null;
			}

	// Get the default system credentials.
	public static ICredentials DefaultCredentials
			{
				get
				{
					lock(typeof(CredentialCache))
					{
						if(defaultCredentials == null)
						{
							defaultCredentials = new NetworkCredential
								(String.Empty, String.Empty, String.Empty);
						}
						return defaultCredentials;
					}
				}
			}

	// Add credentials to this cache.
	public void Add(Uri uriPrefix, String authType, NetworkCredential cred)
			{
				if(uriPrefix == null)
				{
					throw new ArgumentNullException("uriPrefix");
				}
				if(authType == null)
				{
					throw new ArgumentNullException("authType");
				}
				CredentialInfo info = list;
				CredentialInfo last = null;
				while(info != null)
				{
				#if !ECMA_COMPAT
					if(info.uriPrefix.Equals(uriPrefix) &&
					   String.Compare(info.authType, authType, true,
					   				  CultureInfo.InvariantCulture) == 0)
				#else
					if(info.uriPrefix.Equals(uriPrefix) &&
					   String.Compare(info.authType, authType, true) == 0)
				#endif
					{
						throw new ArgumentException
							(S._("Arg_DuplicateCredentials"));
					}
					last = info;
					info = info.next;
				}
				info = new CredentialInfo();
				info.uriPrefix = uriPrefix;
				info.authType = authType;
				info.cred = cred;
				info.next = null;
				if(last != null)
				{
					last.next = info;
				}
				else
				{
					list = info;
				}
			}

	// Determine if we have a credential match.
	private static bool Matches(CredentialInfo info, Uri uriPrefix,
								String authType)
			{
			#if !ECMA_COMPAT
				if(String.Compare(info.authType, authType, true,
				   				  CultureInfo.InvariantCulture) != 0)
			#else
				if(String.Compare(info.authType, authType, true) != 0)
			#endif
				{
					return false;
				}
				return info.uriPrefix.IsPrefix(uriPrefix);
			}

	// Get the credentials for a specific uri/auth combination.
	public NetworkCredential GetCredential(Uri uriPrefix, String authType)
			{
				if(uriPrefix == null)
				{
					throw new ArgumentNullException("uriPrefix");
				}
				if(authType == null)
				{
					throw new ArgumentNullException("authType");
				}
				CredentialInfo info = list;
				CredentialInfo longest = null;
				while(info != null)
				{
					if(Matches(info, uriPrefix, authType))
					{
						if(longest != null)
						{
							if(longest.uriPrefix.ToString().Length <
									info.uriPrefix.ToString().Length)
							{
								longest = info;
							}
						}
						else
						{
							longest = info;
						}
					}
					info = info.next;
				}
				if(longest != null)
				{
					return longest.cred;
				}
				else
				{
					return null;
				}
			}

	// Get an enumerator for this credential cache.
	public IEnumerator GetEnumerator()
			{
				return new CredentialEnumerator(this);
			}

	// Remove a specific uri/auth combination.
	public void Remove(Uri uriPrefix, String authType)
			{
				CredentialInfo info = list;
				CredentialInfo last = null;
				while(info != null)
				{
				#if !ECMA_COMPAT
					if(info.uriPrefix.Equals(uriPrefix) &&
					   String.Compare(info.authType, authType, true,
					   				  CultureInfo.InvariantCulture) == 0)
				#else
					if(info.uriPrefix.Equals(uriPrefix) &&
					   String.Compare(info.authType, authType, true) == 0)
				#endif
					{
						if(last != null)
						{
							last.next = info.next;
						}
						else
						{
							list = info.next;
						}
						return;
					}
					last = info;
					info = info.next;
				}
			}

	// Enumerator class for credential caches.
	private sealed class CredentialEnumerator : IEnumerator
	{
		// Internal state.
		private CredentialCache cache;
		private CredentialInfo current;
		private CredentialInfo next;

		// Constructor.
		public CredentialEnumerator(CredentialCache cache)
				{
					this.cache = cache;
					this.current = null;
					this.next = cache.list;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					if(next != null)
					{
						current = next;
						next = current.next;
						return true;
					}
					else
					{
						return false;
					}
				}
		public void Reset()
				{
					current = null;
					next = cache.list;
				}
		public Object Current
				{
					get
					{
						if(current == null)
						{
							throw new InvalidOperationException
								(S._("Invalid_BadEnumeratorPosition"));
						}
						return current.cred;
					}
				}

	}; // class CredentialEnumerator

}; // class CredentialCache

}; // namespace System.Net
