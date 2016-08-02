/*
 * SortKey.cs - Implementation of the
 *		"System.Globalization.SortKey" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Globalization
{

#if !ECMA_COMPAT

public class SortKey
{
	// Internal state.
	private byte[] keyData;
	private String origString;

	// This class has virtual methods, but it cannot be constructed
	// or inherited according to the specification!  We construct the
	// SortKey instance from "CompareInfo".
	internal SortKey(byte[] keyData, String origString)
			{
				this.keyData = keyData;
				this.origString = origString;
			}

	// Compare two sort keys.
	public static int Compare(SortKey sortkey1, SortKey sortkey2)
			{
				if(sortkey1 == null)
				{
					throw new ArgumentNullException("sortkey1");
				}
				if(sortkey2 == null)
				{
					throw new ArgumentNullException("sortkey2");
				}
				byte[] data1 = sortkey1.keyData;
				byte[] data2 = sortkey2.keyData;
				int minlen = ((data1.Length < data2.Length) ?
									data1.Length : data2.Length);
				int posn;
				for(posn = 0; posn < minlen; ++posn)
				{
					if(data1[posn] < data2[posn])
					{
						return -1;
					}
					else if(data1[posn] > data2[posn])
					{
						return 1;
					}
				}
				if(data1.Length < data2.Length)
				{
					return -1;
				}
				else if(data1.Length > data2.Length)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}

	// Get the sort key data.
	public virtual byte[] KeyData
			{
				get
				{
					return keyData;
				}
			}

	// Get the original string that was used to create the sort key.
	public virtual String OriginalString
			{
				get
				{
					return origString;
				}
			}

	// Determine if two sort keys are equal.
	public override bool Equals(Object obj)
			{
				SortKey other = (obj as SortKey);
				if(other != null)
				{
					return (Compare(this, other) == 0);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return origString.GetHashCode();
			}

	// Convert this sort key into a string.
	public override String ToString()
			{
				return "[Sort key for:" + origString + "]";
			}

}; // class SortKey

#endif // !ECMA_COMPAT

}; // namespace System.Globalization
