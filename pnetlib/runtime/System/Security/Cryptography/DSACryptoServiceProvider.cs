/*
 * DSACryptoServiceProvider.cs - Implementation of the
 *		"System.Security.Cryptography.DSACryptoServiceProvider" class.
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
using System.IO;
using Platform;

// Note: this class assumes that the DSA parameters can be accessed
// in main memory.  This may not necessarily be the case if the
// system is using smart cards or some other kind of crypto dongle.
// We will modify this class when and if that becomes an issue.
//
// This implementation is based on the DSA description in the second
// edition of "Applied Cryptography: Protocols, Algorithms, and
// Source Code in C", Bruce Schneier, John Wiley & Sons, Inc, 1996.

public sealed class DSACryptoServiceProvider : DSA
{
	// Internal state.
	private bool persistKey;
	private static bool useMachineKeyStore;
	private DSAParameters dsaParams;

	// Constructors.
	public DSACryptoServiceProvider()
			: this(0, null) {}
	public DSACryptoServiceProvider(CspParameters parameters)
			: this(0, parameters) {}
	public DSACryptoServiceProvider(int dwKeySize)
			: this(dwKeySize, null) {}
	public DSACryptoServiceProvider(int dwKeySize, CspParameters parameters)
			{
				byte[] key;
				int result;

				// The Microsoft documentation is a little fuzzy as to
				// when this class retrieves a key, generates a new key,
				// or simply waits for the program to supply parameters
				// using "ImportParameters".
				//
				// If we are given a key container name, we ask the runtime
				// engine for the corresponding key.  The engine can either
				// return the key, say that there is no key, reject access
				// because the user doesn't want the program to use the key,
				// or tell the code to generate a new key for the user.
				//
				// If we are not given a key container name, we assume that
				// the program will be supplying the parameters later.
				//
				if(parameters != null && parameters.KeyContainerName != null)
				{
					// Attempt to get a DSA key from the user's keychain.
					key = CryptoMethods.GetKey(CryptoMethods.DSASign,
											   parameters.KeyContainerName,
											   parameters.Flags,
											   out result);
					if(key != null)
					{
						// The "ASN1ToPublic" method will determine if
						// the key is X.509, bare public, or private.
						dsaParams.ASN1ToPublic(key, 0, key.Length);
						Array.Clear(key, 0, key.Length);
						persistKey = true;
					}
					else if(result == CryptoMethods.UnknownKey)
					{
						throw new CryptographicException
							(_("Crypto_UnknownKey"),
							 parameters.KeyContainerName);
					}
					else if(result == CryptoMethods.NotPermitted)
					{
						throw new CryptographicException
							(_("Crypto_NoKeyAccess"),
							 parameters.KeyContainerName);
					}
					else if(result == CryptoMethods.GenerateKey)
					{
						// Generate a new key for the user.
						// TODO
					}
				}
			}

	// Destructor.
	~DSACryptoServiceProvider()
			{
				Dispose(false);
			}

	// Dispose this algorithm instance.
	protected override void Dispose(bool disposing)
			{
				dsaParams.Clear();
			}

	// Get the name of the key exchange algorithm.
	public override String KeyExchangeAlgorithm
			{
				get
				{
					// DSA cannot do key exchange.
					return null;
				}
			}

	// Get or set the size of the key modulus, in bits.
	public override int KeySize
			{
				get
				{
					return KeySizeValue;
				}
				set
				{
					base.KeySize = value;
				}
			}

	// Get the legal key sizes for this asymmetric algorithm.
	public override KeySizes[] LegalKeySizes
			{
				get
				{
					return LegalKeySizesValue;
				}
			}

	// Determine if the key should be persisted in the CSP.
	public bool PersistKeyInCsp
			{
				get
				{
					return persistKey;
				}
				set
				{
					persistKey = value;
				}
			}

	// Get the name of the signature algorithm.
	public override String SignatureAlgorithm
			{
				get
				{
					// W3C identifier for the DSA-SHA1 algorithm.
					return "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
				}
			}

	// Determine if we should use the machine key store.
	public static bool UseMachineKeyStore
			{
				get
				{
					return useMachineKeyStore;
				}
				set
				{
					useMachineKeyStore = value;
				}
			}

	// Create a DSA signature for the specified data.
	public override byte[] CreateSignature(byte[] rgbHash)
			{
				// Validate the parameter.
				if(rgbHash == null)
				{
					throw new ArgumentNullException("rgbHash");
				}

				// Check that we have sufficient DSA parameters to sign.
				if(dsaParams.G == null)
				{
					throw new CryptographicException
						(_("Crypto_DSAParamsNotSet"));
				}
				else if(dsaParams.X == null)
				{
					throw new CryptographicException
						(_("Crypto_CannotSignWithPublic"));
				}

				// Generate a random K less than Q to use in
				// signature generation.  We guarantee less than
				// by setting the high byte of K to at least one
				// less than the high byte of Q.
				int len = dsaParams.Q.Length;
				byte[] K = new byte [len];
				CryptoMethods.GenerateRandom(K, 1, K.Length - 1);
				int index = 0;
				while(index < len && K[index] >= dsaParams.Q[index])
				{
					if(dsaParams.Q[index] == 0)
					{
						K[index] = (byte)0;
						++index;
					}
					else
					{
						K[index] = (byte)(dsaParams.Q[index] - 1);
						break;
					}
				}

				// Compute R = ((G^K mod P) mod Q)
				byte[] temp1 = CryptoMethods.NumPow
					(dsaParams.G, K, dsaParams.P);
				byte[] R = CryptoMethods.NumMod(temp1, dsaParams.Q);
				Array.Clear(temp1, 0, temp1.Length);

				// Compute S = ((K^-1 * (hash + X * R)) mod Q)
				temp1 = CryptoMethods.NumInv(K, dsaParams.Q);
				byte[] temp2 = CryptoMethods.NumMul
					(dsaParams.X, R, dsaParams.Q);
				byte[] temp3 = CryptoMethods.NumAdd
					(rgbHash, temp2, dsaParams.Q);
				byte[] S = CryptoMethods.NumMul(temp1, temp3, dsaParams.Q);
				Array.Clear(temp1, 0, temp1.Length);
				Array.Clear(temp2, 0, temp2.Length);
				Array.Clear(temp3, 0, temp3.Length);
				Array.Clear(K, 0, K.Length);

				// Pack R and S into a signature blob and return it.
				ASN1Builder builder = new ASN1Builder();
				builder.AddBigInt(R);
				builder.AddBigInt(S);
				byte[] sig = builder.ToByteArray();
				Array.Clear(R, 0, R.Length);
				Array.Clear(S, 0, S.Length);
				return sig;
			}

	// Export the parameters for DSA signature generation.
	public override DSAParameters ExportParameters
				(bool includePrivateParameters)
			{
				if(dsaParams.G == null)
				{
					throw new CryptographicException
						(_("Crypto_DSAParamsNotSet"));
				}
				if(includePrivateParameters)
				{
					return dsaParams;
				}
				else
				{
					return dsaParams.ClonePublic();
				}
			}

	// Import the parameters for DSA signature generation.
	public override void ImportParameters(DSAParameters parameters)
			{
				// We need at least P, Q, G, and Y for public key operations.
				if(parameters.P == null || parameters.Q == null ||
				   parameters.G == null || parameters.Y == null ||
				   CryptoMethods.NumZero(parameters.P) ||
				   CryptoMethods.NumZero(parameters.Q))
				{
					throw new CryptographicException
						(_("Crypto_InvalidDSAParams"));
				}
				dsaParams = parameters;
			}

	// Verify a DSA signature.
	public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature)
			{
				// Validate the parameters.
				if(rgbHash == null)
				{
					throw new ArgumentNullException("rgbHash");
				}
				if(rgbSignature == null)
				{
					throw new ArgumentNullException("rgbSignature");
				}

				// Make sure that we have sufficient parameters to verify.
				if(dsaParams.G == null)
				{
					throw new CryptographicException
						(_("Crypto_DSAParamsNotSet"));
				}

				// Unpack the signature blob to get R and S.
				ASN1Parser parser;
				parser = (new ASN1Parser(rgbSignature)).GetSequence();
				byte[] R = parser.GetBigInt();
				byte[] S = parser.GetBigInt();
				parser.AtEnd();

				// Compute W = (S^-1 mod Q)
				byte[] W = CryptoMethods.NumInv(S, dsaParams.Q);

				// Compute U1 = ((hash * W) mod Q)
				byte[] U1 = CryptoMethods.NumMul(rgbHash, W, dsaParams.Q);

				// Compute U2 = ((R * W) mod Q)
				byte[] U2 = CryptoMethods.NumMul(R, W, dsaParams.Q);

				// Compute V = (((G^U1 * Y^U2) mod P) mod Q)
				byte[] temp1 = CryptoMethods.NumPow
					(dsaParams.G, U1, dsaParams.P);
				byte[] temp2 = CryptoMethods.NumPow
					(dsaParams.Y, U2, dsaParams.P);
				byte[] temp3 = CryptoMethods.NumMul(temp1, temp2, dsaParams.P);
				byte[] V = CryptoMethods.NumMod(temp3, dsaParams.Q);

				// Determine if we have a signature match.
				bool result = CryptoMethods.NumEq(V, R);

				// Clear sensitive values.
				Array.Clear(R, 0, R.Length);
				Array.Clear(S, 0, S.Length);
				Array.Clear(W, 0, W.Length);
				Array.Clear(U1, 0, U1.Length);
				Array.Clear(U2, 0, U2.Length);
				Array.Clear(temp1, 0, temp1.Length);
				Array.Clear(temp2, 0, temp2.Length);
				Array.Clear(temp3, 0, temp3.Length);
				Array.Clear(V, 0, V.Length);

				// Done.
				return result;
			}

	// Sign the contents of a byte array.
	public byte[] SignData(byte[] buffer)
			{
				byte[] hash = (new SHA1CryptoServiceProvider())
					.ComputeHash(buffer);
				byte[] signature = CreateSignature(hash);
				Array.Clear(hash, 0, hash.Length);
				return signature;
			}

	// Sign the contents of a stream.
	public byte[] SignData(Stream inputStream)
			{
				byte[] hash = (new SHA1CryptoServiceProvider())
					.ComputeHash(inputStream);
				byte[] signature = CreateSignature(hash);
				Array.Clear(hash, 0, hash.Length);
				return signature;
			}

	// Sign the contents of a sub-range within a byte array.
	public byte[] SignData(byte[] buffer, int offset, int count)
			{
				byte[] hash = (new SHA1CryptoServiceProvider())
					.ComputeHash(buffer, offset, count);
				byte[] signature = CreateSignature(hash);
				Array.Clear(hash, 0, hash.Length);
				return signature;
			}

	// Sign a hash value that was computed with a specific algorithm.
	public byte[] SignHash(byte[] rgbHash, String str)
			{
				if(rgbHash == null)
				{
					throw new ArgumentNullException("rgbHash");
				}
				if(str != null && str != "1.3.14.3.2.26")
				{
					throw new CryptographicException
						(_("Crypto_DSANeedsSHA1"));
				}
				return CreateSignature(rgbHash);
			}

	// Verify the signature on a buffer.
	public bool VerifyData(byte[] rgbData, byte[] rgbSignature)
			{
				byte[] hash = (new SHA1CryptoServiceProvider())
					.ComputeHash(rgbData);
				bool result = VerifySignature(hash, rgbSignature);
				Array.Clear(hash, 0, hash.Length);
				return result;
			}

	// Verify a hash value that was computed with a specific algorithm.
	public bool VerifyHash(byte[] rgbHash, String str, byte[] rgbSignature)
			{
				if(rgbHash == null)
				{
					throw new ArgumentNullException("rgbHash");
				}
				if(str != null && str != "1.3.14.3.2.26")
				{
					throw new CryptographicException
						(_("Crypto_DSANeedsSHA1"));
				}
				return VerifySignature(rgbHash, rgbSignature);
			}

}; // class DSACryptoServiceProvider

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
