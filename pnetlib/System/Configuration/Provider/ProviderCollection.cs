/*
 * ProviderCollection.cs - Implementation of the
 *		"System.Configuration.Provider.ProviderCollection" class.
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

namespace System.Configuration.Provider
{

#if CONFIG_FRAMEWORK_1_2

using System.Collections;
using System.Globalization;

public class ProviderCollection : ICollection, IEnumerable
{
	// Internal state.
	private ArrayList list;
	private bool readOnly;

	// Constructor.
	public ProviderCollection()
			{
				list = new ArrayList();
				readOnly = false;
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

	// Get a provider by name from this collection.
	public IProvider this[String name]
			{
				get
				{
					if(name == null)
					{
						throw new ArgumentNullException("name");
					}
					foreach(IProvider provider in list)
					{
						if(String.Compare(provider.Name, name, true,
										  CultureInfo.InvariantCulture) == 0)
						{
							return provider;
						}
					}
					return null;
				}
			}

	// Add a provider to this collection.
	public void Add(IProvider provider)
			{
				if(provider == null || provider.Name == null)
				{
					throw new ArgumentNullException("provider");
				}
				if(readOnly)
				{
					throw new NotSupportedException(S._("NotSupp_ReadOnly"));
				}
				list.Add(provider);
			}

	// Clear this collection.
	public void Clear()
			{
				if(readOnly)
				{
					throw new NotSupportedException(S._("NotSupp_ReadOnly"));
				}
				list.Clear();
			}

	// Remove a named provider from this collection.
	public void Remove(String name)
			{
				IProvider provider = this[name];
				if(readOnly)
				{
					throw new NotSupportedException(S._("NotSupp_ReadOnly"));
				}
				if(provider != null)
				{
					list.Remove(provider);
				}
			}

	// Set this collection to be read-only.
	public void SetReadOnly()
			{
				readOnly = true;
			}

}; // class ProviderCollection

#endif // CONFIG_FRAMEWORK_1_2

}; // namespace System.Configuration.Provider
