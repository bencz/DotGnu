/*
 * AssemblyHash.cs - Implementation of the
 *		"System.Configuration.Assemblies.AssemblyHash" class.
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

namespace System.Configuration.Assemblies
{

#if !ECMA_COMPAT

public struct AssemblyHash : ICloneable
{
	// Internal state.
	private AssemblyHashAlgorithm hashAlg;
	private byte[] hash;

	// An empty assembly hash object.
	public static readonly AssemblyHash Empty;

	// Construct a new assembly hash.
	public AssemblyHash(byte[] value)
			: this(AssemblyHashAlgorithm.SHA1, value)
			{
				// Nothing to do here
			}
	public AssemblyHash(AssemblyHashAlgorithm algorithmId, byte[] value)
			{
				hashAlg = algorithmId;
				if(value != null)
				{
					hash = new byte [value.Length];
					Array.Copy(value, hash, value.Length);
				}
				else
				{
					hash = null;
				}
			}

	// Get or set the hash algorithm.
	public AssemblyHashAlgorithm Algorithm
			{
				get
				{
					return hashAlg;
				}
				set
				{
					hashAlg = value;
				}
			}

	// Clone this object.
	public Object Clone()
			{
				return new AssemblyHash(hashAlg, hash);
			}

	// Get the hash value that is stored in this object.
	public byte[] GetValue()
			{
				return hash;
			}

	// Set the hash value that is stored in this object.
	public void SetValue(byte[] value)
			{
				hash = value;
			}

}; // struct AssemblyHash

#endif // !ECMA_COMPAT

}; // namespace System.Configuration.Assemblies
