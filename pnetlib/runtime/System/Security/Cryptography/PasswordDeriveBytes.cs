/*
 * PasswordDeriveBytes.cs - Implementation of the
 *		"System.Security.Cryptography.PasswordDeriveBytes" class.
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
using System.Text;

// Note: the implementation of this class is based on PKCS #5 version 2.0.

public class PasswordDeriveBytes : DeriveBytes
{
	// Internal state.
	private String strPassword;
	internal byte[] rgbSalt;
	private String strHashName;
	internal int iterations;
	private HashAlgorithm hashAlgorithm;
	private int blockNum, posn, size;
	private byte[] block;

	// Constructors.
	public PasswordDeriveBytes(String strPassword, byte[] rgbSalt)
			: this(strPassword, rgbSalt, null, 0, null) {}
	public PasswordDeriveBytes(String strPassword, byte[] rgbSalt,
							   CspParameters cspParams)
			: this(strPassword, rgbSalt, null, 0, cspParams) {}
	public PasswordDeriveBytes(String strPassword, byte[] rgbSalt,
							   String strHashName, int iterations)
			: this(strPassword, rgbSalt, strHashName, iterations, null) {}
	public PasswordDeriveBytes(String strPassword, byte[] rgbSalt,
							   String strHashName, int iterations,
							   CspParameters cspParams)
			{
				this.strPassword = strPassword;
				this.rgbSalt = rgbSalt;
				this.strHashName = strHashName;
				this.iterations = iterations;
			}

	// Destructor.
	~PasswordDeriveBytes()
			{
				blockNum = 0;
				if(block != null)
				{
					Array.Clear(block, 0, block.Length);
				}
			}

	// Get or set the name of the hash algorithm.
	public String HashName
			{
				get
				{
					return strHashName;
				}
				set
				{
					if(strHashName == null)
					{
						strHashName = value;
					}
					else
					{
						throw new CryptographicException
							(_("Crypto_HashAlreadySet"));
					}
				}
			}

	// Get or set the iteration count.
	public int IterationCount
			{
				get
				{
					return iterations;
				}
				set
				{
					if(iterations == 0)
					{
						iterations = value;
					}
					else
					{
						throw new CryptographicException
							(_("Crypto_CountAlreadySet"));
					}
				}
			}

	// Get or set the salt.
	public byte[] Salt
			{
				get
				{
					return rgbSalt;
				}
				set
				{
					if(rgbSalt == null)
					{
						rgbSalt = value;
					}
					else
					{
						throw new CryptographicException
							(_("Crypto_SaltAlreadySet"));
					}
				}
			}

	// Derive a key for a specific cryptographic algorithm.
	public byte[] CryptDeriveKey(String algname, String alghashname,
								 int keySize, byte[] rgbIV)
			{
				if((algname == "DES" || algname == "RC2") &&
			   	   alghashname == "MD5" && keySize == 8)
				{
					// Use the older PKCS #5 password generation routine.
					MD5 md5 = new MD5CryptoServiceProvider();
					if(strPassword != null)
					{
						byte[] pwd = Encoding.UTF8.GetBytes(strPassword);
						md5.InternalHashCore(pwd, 0, pwd.Length);
						Array.Clear(pwd, 0, pwd.Length);
					}
					if(rgbSalt != null)
					{
						md5.InternalHashCore(rgbSalt, 0, rgbSalt.Length);
					}
					byte[] tempHash = md5.InternalHashFinal();
					md5.Initialize();
					int count = iterations;
					while(count > 1)
					{
						md5.InternalHashCore(tempHash, 0, tempHash.Length);
						Array.Clear(tempHash, 0, tempHash.Length);
						tempHash = md5.InternalHashFinal();
						md5.Initialize();
						--count;
					}
					byte[] key = new byte [8];
					Array.Copy(tempHash, 0, key, 0, 8);
					if(rgbIV != null)
					{
						Array.Copy(tempHash, 8, rgbIV, 0, 8);
					}
					Array.Clear(tempHash, 0, tempHash.Length);
					return key;
				}
				else
				{
					// Use the newer PKCS #5 password generation routine.
					Reset();
					if(alghashname != null)
					{
						strHashName = alghashname;
					}
					byte[] result = GetBytes(keySize);
					if(rgbIV != null)
					{
						byte[] iv = GetBytes(rgbIV.Length);
						Array.Copy(iv, 0, rgbIV, 0, rgbIV.Length);
						Array.Clear(iv, 0, iv.Length);
					}
					return result;
				}
			}

	// Get the pseudo-random key bytes.
	public override byte[] GetBytes(int cb)
			{
				// Initialize the pseudo-random generator.
				if(hashAlgorithm == null)
				{
					if(strHashName == null)
					{
						strHashName = "MD5";
					}
					hashAlgorithm = HashAlgorithm.Create(strHashName);
					blockNum = 1;
					size = hashAlgorithm.HashSize;
					posn = size;
				}

				// Allocate the result array and then fill it.
				byte[] result = new byte [cb];
				int index = 0;
				int templen;
				while(cb > 0)
				{
					// Copy existing data from the previous block.
					if(posn < size)
					{
						templen = (size - posn);
						if(cb < templen)
						{
							templen = cb;
						}
						Array.Copy(block, posn, result, index, templen);
						cb -= templen;
						index -= templen;
						posn = size;
						if(cb <= 0)
						{
							break;
						}
					}

					// Generate a new block using the hash algorithm.
					if(strPassword != null)
					{
						byte[] pwd = Encoding.UTF8.GetBytes(strPassword);
						hashAlgorithm.InternalHashCore(pwd, 0, pwd.Length);
						Array.Clear(pwd, 0, pwd.Length);
					}
					if(rgbSalt != null)
					{
						hashAlgorithm.InternalHashCore
							(rgbSalt, 0, rgbSalt.Length);
					}
					byte[] numbuf = new byte [4];
					numbuf[0] = (byte)(blockNum >> 24);
					numbuf[1] = (byte)(blockNum >> 16);
					numbuf[2] = (byte)(blockNum >> 8);
					numbuf[3] = (byte)blockNum;
					hashAlgorithm.InternalHashCore(numbuf, 0, 4);
					Array.Clear(numbuf, 0, numbuf.Length);
					byte[] lastHash = hashAlgorithm.InternalHashFinal();
					hashAlgorithm.Initialize();
					templen = iterations;
					byte[] temphash;
					while(templen > 1)
					{
						hashAlgorithm.InternalHashCore
							(lastHash, 0, lastHash.Length);
						temphash = hashAlgorithm.InternalHashFinal();
						hashAlgorithm.Initialize();
						for(int tempindex = 0; tempindex < lastHash.Length;
							++tempindex)
						{
							lastHash[tempindex] ^= temphash[tempindex];
						}
						Array.Clear(temphash, 0, temphash.Length);
						--templen;
					}
					if(block != null)
					{
						Array.Clear(block, 0, block.Length);
					}
					block = lastHash;
					++blockNum;
					posn = 0;
				}

				// Return the result array to the caller.
				return result;
			}

	// Reset the state.
	public override void Reset()
			{
				hashAlgorithm = null;
				blockNum = 0;
				if(block != null)
				{
					Array.Clear(block, 0, block.Length);
				}
			}

}; // class PasswordDeriveBytes

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
