/*
 * HashCodeProviderAdapter.cs - Adapt a generic hash code provider
 *			into a non-generic one.
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

namespace Generics
{

using System;

public sealed class HashCodeProviderAdapter<T>
	: System.Collections.IHashCodeProvider
{

	// Internal state.
	private IHashCodeProvider<T> provider;

	// Constructor.
	public HashCodeProviderAdapter(IHashCodeProvider<T> provider)
			{
				if(provider == null)
				{
					throw new ArgumentNullException("provider");
				}
				this.provider = provider;
			}

	// Implement the non-generic IHashCodeProvider interface.
	public int GetHashCode(Object obj)
			{
				return provider.GetHashCode((T)obj);
			}

}; // class HashCodeProviderAdapter<T>

}; // namespace Generics
