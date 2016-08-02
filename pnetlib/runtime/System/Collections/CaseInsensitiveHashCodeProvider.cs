/*
 * CaseInsensitiveHashCodeProvider.cs - Implementation of the
 *			"System.Collections.CaseInsensitiveHashCodeProvider" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Collections
{

#if !ECMA_COMPAT

using System;
using System.Globalization;

public class CaseInsensitiveHashCodeProvider : IHashCodeProvider
{
	// The default case insensitive comparer instances.
	private static readonly CaseInsensitiveHashCodeProvider
		defaultProvider =
			new CaseInsensitiveHashCodeProvider();
	private static readonly CaseInsensitiveHashCodeProvider
		defaultInvariantProvider =
			new CaseInsensitiveHashCodeProvider(CultureInfo.InvariantCulture);

	// Internal state.
	private TextInfo info;

	// Get the default comparer instances.
	public static CaseInsensitiveHashCodeProvider Default
			{
				get
				{
					return defaultProvider;
				}
			}
	public static CaseInsensitiveHashCodeProvider DefaultInvariant
			{
				get
				{
					return defaultInvariantProvider;
				}
			}

	// Constructors.
	public CaseInsensitiveHashCodeProvider()
			{
				info = CultureInfo.CurrentCulture.TextInfo;
			}
	public CaseInsensitiveHashCodeProvider(CultureInfo culture)
			{
				if(culture == null)
				{
					throw new ArgumentNullException("culture");
				}
				info = culture.TextInfo;
			}

	// Implement the IHashCodeProvider interface.
	public int GetHashCode(Object obj)
			{
				String str = (obj as String);
				if(str != null)
				{
					return info.ToLower(str).GetHashCode();
				}
				else if(obj != null)
				{
					return obj.GetHashCode();
				}
				else
				{
					throw new ArgumentNullException("obj");
				}
			}

}; // class CaseInsensitiveHashCodeProvider

#endif // !ECMA_COMPAT

}; // namespace System.Collections
