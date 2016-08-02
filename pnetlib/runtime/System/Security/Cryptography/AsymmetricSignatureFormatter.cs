/*
 * AsymmetricSignatureFormatter.cs - Implementation of the
 *		"System.Security.Cryptography.AsymmetricSignatureFormatter" class.
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

namespace System.Security.Cryptography
{

#if CONFIG_CRYPTO

using System;

public abstract class AsymmetricSignatureFormatter
{
	// Constructor.
	public AsymmetricSignatureFormatter() {}

	// Set the hash algorithm.
	public abstract void SetHashAlgorithm(string strName);

	// Set the key to use to compute the signature.
	public abstract void SetKey(AsymmetricAlgorithm key);

	// Create a signature for a specified hash value.
	public abstract byte[] CreateSignature(byte[] rgbHash);
	public virtual byte[] CreateSignature(HashAlgorithm hash)
			{
				if(hash == null)
				{
					throw new ArgumentNullException("hash");
				}
				return CreateSignature(hash.Hash);
			}

}; // class AsymmetricSignatureFormatter

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
