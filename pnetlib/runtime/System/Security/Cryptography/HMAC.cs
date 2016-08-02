/*
 * HMAC.cs - Implementation of the
 *		"System.Security.Cryptography.HMAC" class.
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

namespace System.Security.Cryptography
{

#if CONFIG_CRYPTO && CONFIG_FRAMEWORK_1_2

using System;

public abstract class HMAC : KeyedHashAlgorithm
{
	// Internal state.
	protected int BlockSizeValue;
	private HashAlgorithm alg;
	private String algName;

	// Constructor.
	protected HMAC()
			{
				BlockSizeValue = 1;
			}

	// Create an instance of the default HMAC algorithm.
	public new static HMAC Create()
			{
				return (HMAC)(CryptoConfig.CreateFromName
					(CryptoConfig.HMACDefault, null));
			}

	// Create an instance of a specific HMAC algorithm.
	public new static HMAC Create(String algName)
			{
				return (HMAC)(CryptoConfig.CreateFromName(algName, null));
			}

	// Dispose this object.
	protected override void Dispose(bool disposing)
			{
				if(alg != null)
				{
					((IDisposable)alg).Dispose();
				}
				base.Dispose(disposing);
			}

	// Initialize the key.  If it is 64 bytes or less, then use
	// it as-is.  Otherwise hash it down.
	private void SetKey(byte[] key)
			{
				if(key.Length <= 64)
				{
					KeyValue = key;
				}
				else
				{
					KeyValue = alg.ComputeHash(key);
				}
			}

	// Get or set the hash key.
	public override byte[] Key
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
					else if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					SetKey(value);
				}
			}

	// Get or set the name of the hash algorithm implementation.
	public String HashName
			{
				get
				{
					return algName;
				}
				set
				{
					algName = value;
					alg = HashAlgorithm.Create(algName);
				}
			}

	// Initialize the hash algorithm.
	public override void Initialize()
			{
				if(alg != null)
				{
					alg.Initialize();
				}
				alg = null;
			}

	// Prepare the hash for the initial call to "HashCore" or "HashFinal".
	private void Prepare()
			{
				if(alg == null)
				{
					alg = SHA1.Create(algName);
					if(KeyValue != null)
					{
						alg.InternalHashCore(KeyValue, 0, KeyValue.Length);
					}
				}
			}

	// Write data to the underlying hash algorithm.
	protected override void HashCore(byte[] array, int ibStart, int cbSize)
			{
				Prepare();
				alg.InternalHashCore(array, ibStart, cbSize);
			}

	// Finalize the hash and return the final hash value.
	protected override byte[] HashFinal()
			{
				// Compute the final hash, which is "H(K, H(K, Data))".
				Prepare();
				byte[] inner = alg.InternalHashFinal();
				alg.Initialize();
				if(KeyValue != null)
				{
					alg.InternalHashCore(KeyValue, 0, KeyValue.Length);
				}
				alg.InternalHashCore(inner, 0, inner.Length);
				Array.Clear(inner, 0, inner.Length);
				return alg.InternalHashFinal();
			}

}; // class HMAC

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
