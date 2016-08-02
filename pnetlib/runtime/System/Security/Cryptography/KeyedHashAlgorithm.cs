/*
 * KeyedHashAlgorithm.cs - Implementation of the
 *		"System.Security.Cryptography.KeyedHashAlgorithm" class.
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

public abstract class KeyedHashAlgorithm : HashAlgorithm
{
	// Internal state which is set by subclasses.
	protected byte[] KeyValue;

	// Constructor.
	protected KeyedHashAlgorithm() {}

	// Destructor.
	~KeyedHashAlgorithm()
			{
				Dispose(false);
			}

	// Dispose this object.
	protected override void Dispose(bool disposing)
			{
				if(KeyValue != null)
				{
					Array.Clear(KeyValue, 0, KeyValue.Length);
				}
				base.Dispose(disposing);
			}

	// Create an instance of the default keyed hash algorithm.
	public new static KeyedHashAlgorithm Create()
			{
				return (KeyedHashAlgorithm)
					(CryptoConfig.CreateFromName
						(CryptoConfig.KeyedHashDefault, null));
			}

	// Create an instance of a specific keyed hash algorithm.
	public new static KeyedHashAlgorithm Create(String hashName)
			{
				return (KeyedHashAlgorithm)
					(CryptoConfig.CreateFromName(hashName, null));
			}

	// Get or set the hash key.
	public virtual byte[] Key
			{
				get
				{
					return KeyValue;
				}
				set
				{
					if(State != 0)
					{
						throw new CryptographicException
							(_("Crypto_HashInProgress"));
					}
					KeyValue = value;
				}
			}

}; // class KeyedHashAlgorithm

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
