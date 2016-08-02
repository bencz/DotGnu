/*
 * HMACSHA1.cs - Implementation of the
 *		"System.Security.Cryptography.HMACSHA1" class.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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
using Platform;

#if !CONFIG_FRAMEWORK_1_2

public class HMACSHA1 : KeyedHashAlgorithm
{
	// Internal state.
	private SHA1 alg;
	private String algName;

	// Constructors.
	public HMACSHA1()
			{
				HashSizeValue = 160;
				KeyValue = new byte [64];
				CryptoMethods.GenerateRandom(KeyValue, 0, 64);
				alg = null;
				algName = CryptoConfig.SHA1Default;
			}
	public HMACSHA1(byte[] rgbKey)
			{
				HashSizeValue = 160;
				if(rgbKey == null)
				{
					throw new ArgumentNullException("rgbKey");
				}
				SetKey(rgbKey);
				alg = null;
				algName = CryptoConfig.SHA1Default;
			}

	// Destructor.
	~HMACSHA1()
			{
				Dispose(false);
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
					KeyValue = (new SHA1CryptoServiceProvider()).
						ComputeHash(key);
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

}; // class HMACSHA1

#else // CONFIG_FRAMEWORK_1_2

public class HMACSHA1 : HMAC
{
	// Constructors.
	public HMACSHA1()
			{
				HashName = "SHA1";
				HashSizeValue = 160;
				byte[] key = new byte [64];
				CryptoMethods.GenerateRandom(key, 0, 64);
			}
	public HMACSHA1(byte[] rgbKey)
			{
				HashName = "SHA1";
				HashSizeValue = 160;
				Key = rgbKey;
			}

	// Destructor.
	~HMACSHA1()
			{
				Dispose(false);
			}

}; // class HMACSHA1

#endif // CONFIG_FRAMEWORK_1_2

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
