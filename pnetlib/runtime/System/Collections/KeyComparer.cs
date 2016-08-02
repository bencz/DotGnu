/*
 * KeyComparer.cs - Implementation of the
 *			"System.Collections.KeyComparer" class.
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

namespace System.Collections
{

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2

using System.Globalization;

public sealed class KeyComparer : IKeyComparer
{
	// Internal state.
	private IComparer comparer;
	private IHashCodeProvider hashCodeProvider;
	private static IHashCodeProvider defaultHashCodeProvider =
			new DefaultHashCodeProvider();

	// Constructors.
	private KeyComparer(IComparer comparer, IHashCodeProvider hashCodeProvider)
			{
				this.comparer = comparer;
				this.hashCodeProvider = hashCodeProvider;
			}

	// Determine if two objects are equal.
	bool IKeyComparer.Equals(Object x, Object y)
			{
				if(hashCodeProvider.GetHashCode(x) !=
				   hashCodeProvider.GetHashCode(y))
				{
					return false;
				}
				return (comparer.Compare(x, y) == 0);
			}

	// Create a new key comparer.
	public static IKeyComparer CreateKeyComparer(ComparisonType comparisonType)
			{
				switch(comparisonType)
				{
					case ComparisonType.CurrentCulture:
					{
						return new KeyComparer
							(new Comparer(CultureInfo.CurrentCulture),
							 defaultHashCodeProvider);
					}

					case ComparisonType.CurrentCultureIgnoreCase:
					{
						return new KeyComparer
							(new CaseInsensitiveComparer
								(CultureInfo.CurrentCulture),
							 new CaseInsensitiveHashCodeProvider
							 	(CultureInfo.CurrentCulture));
					}

					case ComparisonType.InvariantCulture:
					{
						return new KeyComparer
							(Comparer.DefaultInvariant,
							 defaultHashCodeProvider);
					}

					case ComparisonType.InvariantCultureIgnoreCase:
					{
						return new KeyComparer
							(CaseInsensitiveComparer.DefaultInvariant,
							 CaseInsensitiveHashCodeProvider.DefaultInvariant);
					}

					default:
					{
						return new KeyComparer
							(new OrdinalComparer(), defaultHashCodeProvider);
					}
				}
			}
	public static IKeyComparer CreateKeyComparer
				(CultureInfo culture, bool ignoreCase)
			{
				if(culture == null)
				{
					throw new ArgumentNullException("culture");
				}
				if(!ignoreCase)
				{
					return new KeyComparer
						(new Comparer(culture), defaultHashCodeProvider);
				}
				else
				{
					return new KeyComparer
						(new CaseInsensitiveComparer(culture),
						 new CaseInsensitiveHashCodeProvider(culture));
				}
			}
	public static IKeyComparer CreateKeyComparer
				(IComparer comparer, IHashCodeProvider hashCodeProvider)
			{
				if(comparer == null)
				{
					throw new ArgumentNullException("comparer");
				}
				if(hashCodeProvider == null)
				{
					throw new ArgumentNullException("hashCodeProvider");
				}
				return new KeyComparer(comparer, hashCodeProvider);
			}

	// Default hash code provider class.
	private sealed class DefaultHashCodeProvider : IHashCodeProvider
	{
		// Get the hash code for an object.
		public int GetHashCode(Object obj)
				{
					if(obj != null)
					{
						return obj.GetHashCode();
					}
					else
					{
						return 0;
					}
				}

	}; // class DefaultHashCodeProvider

	// Comparison class that uses ordinal values for strings.
	private sealed class OrdinalComparer : IComparer
	{

		// Implement the IComparer interface.
		public int Compare(Object a, Object b)
				{
					IComparable cmp;
					if(a != null && b != null)
					{
						if(a is String && b is String)
						{
							return String.CompareOrdinal((String)a, (String)b);
						}
						cmp = (a as IComparable);
						if(cmp != null)
						{
							return cmp.CompareTo(b);
						}
						cmp = (b as IComparable);
						if(cmp != null)
						{
							return -(cmp.CompareTo(a));
						}
						throw new ArgumentException
							(_("Arg_ABMustBeComparable"));
					}
					else if(a != null)
					{
						return 1;
					}
					else if(b != null)
					{
						return -1;
					}
					else
					{
						return 0;
					}
				}

	}; // class OrdinalComparer

}; // class KeyComparer

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2

}; // namespace System.Collections
