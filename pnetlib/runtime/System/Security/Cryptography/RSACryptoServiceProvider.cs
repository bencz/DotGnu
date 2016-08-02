/*
 * RSACryptoServiceProvider.cs - Implementation of the
 *		"System.Security.Cryptography.RSACryptoServiceProvider" class.
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
using System.IO;
using Platform;

// Note: this class assumes that the RSA parameters can be accessed
// in main memory.  This may not necessarily be the case if the
// system is using smart cards or some other kind of crypto dongle.
// We will modify this class when and if that becomes an issue.
//
// This implementation is based on the RSA description in PKCS #1 v2.1,
// available from "http://www.rsa.com/".

public sealed class RSACryptoServiceProvider : RSA
{
	// Internal state.
	private bool persistKey;
	private static bool useMachineKeyStore;
	private RSAParameters rsaParams;

	// Constructors.
	public RSACryptoServiceProvider()
			: this(0, null) {}
	public RSACryptoServiceProvider(CspParameters parameters)
			: this(0, parameters) {}
	public RSACryptoServiceProvider(int dwKeySize)
			: this(dwKeySize, null) {}
	public RSACryptoServiceProvider(int dwKeySize, CspParameters parameters)
			{
				byte[] key;
				int result;

				// See "DSACryptoServiceProvider" for explainatory comments.
				if(parameters != null && parameters.KeyContainerName != null)
				{
					// Attempt to get an RSA key from the user's keychain.
					key = CryptoMethods.GetKey(CryptoMethods.RSAEncrypt,
											   parameters.KeyContainerName,
											   parameters.Flags,
											   out result);
					if(key != null)
					{
						// The "ASN1ToPublic" method will determine if
						// the key is X.509, bare public, or private.
						rsaParams.ASN1ToPublic(key, 0, key.Length);
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
	~RSACryptoServiceProvider()
			{
				Dispose(false);
			}

	// Dispose this algorithm instance.
	protected override void Dispose(bool disposing)
			{
				rsaParams.Clear();
			}

	// Get the name of the key exchange algorithm.
	public override String KeyExchangeAlgorithm
			{
				get
				{
					return "RSA-PKCS1-KeyEx";
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
					// W3C identifier for the RSA-SHA1 algorithm.
					return "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
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

	// Apply the public key to a value.
	private byte[] ApplyPublic(byte[] value)
			{
				if(rsaParams.Modulus == null)
				{
					throw new CryptographicException
						(_("Crypto_RSAParamsNotSet"));
				}
				return CryptoMethods.NumPow(value, rsaParams.Exponent,
											rsaParams.Modulus);
			}

	// Apply the private key to a value.
	private byte[] ApplyPrivate(byte[] value)
			{
				if(rsaParams.P != null)
				{
					// Use the Chinese Remainder Theorem exponents.
					// Based on the description in PKCS #1.
					byte[] m1   = CryptoMethods.NumPow(value, rsaParams.DP,
													   rsaParams.P);
					byte[] m2   = CryptoMethods.NumPow(value, rsaParams.DQ,
													   rsaParams.Q);
					byte[] diff = CryptoMethods.NumSub(m1, m2, null);
					byte[] h    = CryptoMethods.NumMul(diff,
													   rsaParams.InverseQ,
													   rsaParams.P);
					byte[] prod = CryptoMethods.NumMul(rsaParams.Q, h, null);
					byte[] m    = CryptoMethods.NumAdd(m2, prod, null);

					// Clear all temporary values.
					Array.Clear(m1, 0, m1.Length);
					Array.Clear(m2, 0, m2.Length);
					Array.Clear(diff, 0, diff.Length);
					Array.Clear(h, 0, h.Length);
					Array.Clear(prod, 0, prod.Length);
					
					// Return the decrypted message.
					return m;
				}
				else if(rsaParams.D != null)
				{
					// Use the private exponent directly.
					return CryptoMethods.NumPow(value, rsaParams.D,
												rsaParams.Modulus);
				}
				else
				{
					// Insufficient parameters for private key operations.
					throw new CryptographicException
						(_("Crypto_RSAParamsNotSet"));
				}
			}

	// Decrypt a value using the RSA private key and OAEP padding.
	internal byte[] DecryptOAEP(byte[] rgb)
			{
				// Check the data against null.
				if(rgb == null)
				{
					throw new ArgumentNullException("rgb");
				}

				// Make sure that we have sufficient RSA parameters.
				if(rsaParams.Modulus == null)
				{
					throw new CryptographicException
						(_("Crypto_RSAParamsNotSet"));
				}

				// Check the length of the incoming value.
				int k = rsaParams.Modulus.Length;
				if(k < (2 * 20 + 2))
				{
					throw new CryptographicException
						(_("Crypto_RSAKeyTooShort"));
				}

				// Decrypt the incoming value, and expand to k octets,
				// as the big number routines in the engine may have
				// stripped leading zeroes from the value.
				byte[] decrypted = ApplyPrivate(rgb);
				if(decrypted.Length > k)
				{
					Array.Clear(decrypted, 0, decrypted.Length);
					throw new CryptographicException
						(_("Crypto_RSAInvalidCiphertext"));
				}
				byte[] msg = new byte [k];
				Array.Copy(decrypted, 0, msg, k - decrypted.Length,
						   decrypted.Length);

				// Acquire a mask generation method, based on SHA-1.
				MaskGenerationMethod maskGen = new PKCS1MaskGenerationMethod();

				// Extract the non-seed part of the decrypted message.
				byte[] maskedMsg = new byte [k - 20 - 1];
				Array.Copy(msg, 0, maskedMsg, 0, k - 20 - 1);

				// Decrypt the seed value.
				byte[] seedMask = maskGen.GenerateMask(maskedMsg, 20);
				byte[] seed = new byte [20];
				int index;
				for(index = 0; index < 20; ++index)
				{
					seed[index] = (byte)(msg[index + 1] ^ seedMask[index]);
				}

				// Decrypt the non-seed part of the decrypted message.
				byte[] msgMask = maskGen.GenerateMask(seed, maskedMsg.Length);
				for(index = 0; index < maskedMsg.Length; ++index)
				{
					maskedMsg[index] ^= msgMask[index];
				}

				// Validate the format of the unmasked message.  We do this
				// carefully, to prevent attackers from performing timing
				// attacks on the algorithm.  See PKCS #1 for details.
				int error = msg[0];
				for(index = 0; index < 20; ++index)
				{
					error |= (maskedMsg[index] ^ label[index]);
				}
				for(index = 20; index < (k - 20 - 2); ++index)
				{
					// Question: is this careful enough to prevent
					// timing attacks?  May need revisiting later.
					if(maskedMsg[index] != 0)
					{
						break;
					}
				}
				error |= (maskedMsg[index] ^ 0x01);
				if(error != 0)
				{
					// Something is wrong with the decrypted padding data.
					Array.Clear(decrypted, 0, decrypted.Length);
					Array.Clear(msg, 0, msg.Length);
					Array.Clear(maskedMsg, 0, maskedMsg.Length);
					Array.Clear(seedMask, 0, seedMask.Length);
					Array.Clear(seed, 0, seed.Length);
					Array.Clear(msgMask, 0, msgMask.Length);
					throw new CryptographicException
						(_("Crypto_RSAInvalidCiphertext"));
				}
				++index;

				// Extract the final decrypted message.
				byte[] finalMsg = new byte [maskedMsg.Length - index];
				Array.Copy(maskedMsg, index, finalMsg, 0,
						   maskedMsg.Length - index);

				// Destroy sensitive values.
				Array.Clear(decrypted, 0, decrypted.Length);
				Array.Clear(msg, 0, msg.Length);
				Array.Clear(maskedMsg, 0, maskedMsg.Length);
				Array.Clear(seedMask, 0, seedMask.Length);
				Array.Clear(seed, 0, seed.Length);
				Array.Clear(msgMask, 0, msgMask.Length);

				// Done.
				return finalMsg;
			}

	// Decrypt a value using the RSA private key and PKCS1 decoding.
	internal byte[] DecryptPKCS1(byte[] rgb)
			{
				// Check the data against null.
				if(rgb == null)
				{
					throw new ArgumentNullException("rgb");
				}

				// Make sure that we have sufficient RSA parameters.
				if(rsaParams.Modulus == null)
				{
					throw new CryptographicException
						(_("Crypto_RSAParamsNotSet"));
				}

				// Check the length of the incoming value.
				int k = rsaParams.Modulus.Length;
				if(k < 11)
				{
					throw new CryptographicException
						(_("Crypto_RSAKeyTooShort"));
				}

				// Decrypt the incoming value, and expand to k octets,
				// as the big number routines in the engine may have
				// stripped leading zeroes from the value.
				byte[] decrypted = ApplyPrivate(rgb);
				if(decrypted.Length > k)
				{
					Array.Clear(decrypted, 0, decrypted.Length);
					throw new CryptographicException
						(_("Crypto_RSAInvalidCiphertext"));
				}
				byte[] msg = new byte [k];
				Array.Copy(decrypted, 0, msg, k - decrypted.Length,
						   decrypted.Length);

				// Check the format of the padding data.  We need to
				// harden this against timing attacks.  It is hard to
				// do so since we don't know the expected length of
				// the final message.
				int error = msg[0] | (msg[1] ^ 0x02);
				int index = 2;
				while(index < (msg.Length - 1) && msg[index] != 0x00)
				{
					++index;
				}
				error |= msg[index];
				if(error != 0)
				{
					Array.Clear(decrypted, 0, decrypted.Length);
					Array.Clear(msg, 0, msg.Length);
					throw new CryptographicException
						(_("Crypto_RSAInvalidCiphertext"));
				}
				++index;

				// Extract the final message.
				byte[] finalMsg = new byte [msg.Length - index];
				Array.Copy(msg, index, finalMsg, 0, finalMsg.Length);

				// Destroy sensitive values.
				Array.Clear(decrypted, 0, decrypted.Length);
				Array.Clear(msg, 0, msg.Length);

				// Done.
				return finalMsg;
			}

	// Decrypt a value using the RSA private key and optional OAEP padding.
	public byte[] Decrypt(byte[] rgb, bool fOAEP)
			{
				if(fOAEP)
				{
					return DecryptOAEP(rgb);
				}
				else
				{
					return DecryptPKCS1(rgb);
				}
			}

	// Decrypt a value using the RSA private key.
	public override byte[] DecryptValue(byte[] rgb)
			{
				return DecryptPKCS1(rgb);
			}

	// Label value for OAEP encryption.  SHA-1 hash of the empty string.
	private static readonly byte[] label =
			{(byte)0xDA, (byte)0x39, (byte)0xA3, (byte)0xEE,
			 (byte)0x5E, (byte)0x6B, (byte)0x4B, (byte)0x0D,
			 (byte)0x32, (byte)0x55, (byte)0xBF, (byte)0xEF,
			 (byte)0x95, (byte)0x60, (byte)0x18, (byte)0x90,
			 (byte)0xAF, (byte)0xD8, (byte)0x07, (byte)0x09};

	// Encrypt a value using a specified OAEP padding array.
	// The array may be null to pad with zeroes.
	internal byte[] EncryptOAEP(byte[] rgb, byte[] padding,
								RandomNumberGenerator rng)
			{
				// Check the data against null.
				if(rgb == null)
				{
					throw new ArgumentNullException("rgb");
				}

				// Make sure that we have sufficient RSA parameters.
				if(rsaParams.Modulus == null)
				{
					throw new CryptographicException
						(_("Crypto_RSAParamsNotSet"));
				}

				// Format the label, padding string, and rgb into a message.
				int k = rsaParams.Modulus.Length;
				if(rgb.Length > (k - 2 * 20 - 2))
				{
					throw new CryptographicException
						(_("Crypto_RSAMessageTooLong"));
				}
				byte[] msg = new byte [k - 20 - 1];
				int posn = 0;
				Array.Copy(label, 0, msg, posn, 20);
				posn += 20;
				int padlen = k - rgb.Length - 2 * 20 - 2;
				if(padlen > 0 && padding != null)
				{
					if(padding.Length <= padlen)
					{
						Array.Copy(padding, 0, msg, posn, padding.Length);
					}
					else
					{
						Array.Copy(padding, 0, msg, posn, padlen);
					}
				}
				posn += padlen;
				msg[posn++] = (byte)0x01;
				Array.Copy(rgb, 0, msg, posn, rgb.Length);

				// Acquire a mask generation method, based on SHA-1.
				MaskGenerationMethod maskGen = new PKCS1MaskGenerationMethod();

				// Generate a random seed to use to generate masks.
				byte[] seed = new byte [20];
				rng.GetBytes(seed);

				// Generate a mask and encrypt the above message.
				byte[] mask = maskGen.GenerateMask(seed, msg.Length);
				int index;
				for(index = 0; index < msg.Length; ++index)
				{
					msg[index] ^= mask[index];
				}

				// Generate another mask and encrypt the seed.
				byte[] seedMask = maskGen.GenerateMask(msg, 20);
				for(index = 0; index < 20; ++index)
				{
					seed[index] ^= seedMask[index];
				}

				// Build the value to be encrypted using RSA.
				byte[] value = new byte [k];
				Array.Copy(seed, 0, value, 1, 20);
				Array.Copy(msg, 0, value, 21, msg.Length);

				// Encrypt the value.
				byte[] encrypted = ApplyPublic(value);

				// Destroy sensitive data.
				Array.Clear(msg, 0, msg.Length);
				Array.Clear(seed, 0, seed.Length);
				Array.Clear(mask, 0, mask.Length);
				Array.Clear(seedMask, 0, seedMask.Length);
				Array.Clear(value, 0, value.Length);

				// Done.
				return encrypted;
			}

	// Encrypt a value using the RSA public key and the PKCS1 encoding.
	internal byte[] EncryptPKCS1(byte[] rgb, RandomNumberGenerator rng)
			{
				// Check the data against null.
				if(rgb == null)
				{
					throw new ArgumentNullException("rgb");
				}

				// Make sure that we have sufficient RSA parameters.
				if(rsaParams.Modulus == null)
				{
					throw new CryptographicException
						(_("Crypto_RSAParamsNotSet"));
				}

				// Format the type codes, padding string, and data.
				int k = rsaParams.Modulus.Length;
				if(rgb.Length > (k - 11))
				{
					throw new CryptographicException
						(_("Crypto_RSAMessageTooLong"));
				}
				byte[] msg = new byte [k];
				msg[1] = (byte)0x02;
				byte[] padding = new byte [k - rgb.Length - 3];
				rng.GetNonZeroBytes(padding);
				Array.Copy(padding, 0, msg, 2, padding.Length);
				Array.Copy(rgb, 0, msg, k - rgb.Length, rgb.Length);

				// Encrypt the message.
				byte[] encrypted = ApplyPublic(msg);

				// Destroy sensitive data.
				Array.Clear(msg, 0, msg.Length);
				Array.Clear(padding, 0, padding.Length);

				// Done.
				return encrypted;
			}

	// Encrypt a value using the RSA private key and optional OAEP padding.
	public byte[] Encrypt(byte[] rgb, bool fOAEP)
			{
				if(fOAEP)
				{
					return EncryptOAEP
						(rgb, null, new RNGCryptoServiceProvider());
				}
				else
				{
					return EncryptPKCS1(rgb, new RNGCryptoServiceProvider());
				}
			}

	// Encrypt a value using the RSA public key.
	public override byte[] EncryptValue(byte[] rgb)
			{
				return EncryptPKCS1(rgb, new RNGCryptoServiceProvider());
			}

	// Export the parameters for RSA signature generation.
	public override RSAParameters ExportParameters
				(bool includePrivateParameters)
			{
				if(rsaParams.Modulus == null)
				{
					throw new CryptographicException
						(_("Crypto_RSAParamsNotSet"));
				}
				if(includePrivateParameters)
				{
					return rsaParams;
				}
				else
				{
					return rsaParams.ClonePublic();
				}
			}

	// Import the parameters for DSA signature generation.
	public override void ImportParameters(RSAParameters parameters)
			{
				// We need at least Modulus and Exponent for public key ops.
				if(parameters.Modulus == null || parameters.Exponent == null ||
				   CryptoMethods.NumZero(parameters.Modulus) ||
				   CryptoMethods.NumZero(parameters.Exponent))
				{
					throw new CryptographicException
						(_("Crypto_InvalidRSAParams"));
				}
				rsaParams = parameters;
			}

	// Convert a "halg" value into a HashAlgorithm instance.
	private static HashAlgorithm ObjectToHashAlg(Object halg)
			{
				HashAlgorithm alg;
				if(halg == null)
				{
					throw new ArgumentNullException("halg");
				}
				else if(halg is String)
				{
					alg = HashAlgorithm.Create((String)halg);
				}
				else if(halg is HashAlgorithm)
				{
					alg = (HashAlgorithm)halg;
				}
				else if(halg is Type &&
				        ((Type)halg).IsSubclassOf(typeof(HashAlgorithm)))
				{
					alg = (HashAlgorithm)(Activator.CreateInstance((Type)halg));
				}
				else
				{
					throw new ArgumentException
						(_("Crypto_NeedsHash"), "halg");
				}
				if(!(alg is SHA1) && !(alg is MD5) &&
				   !(alg is SHA256) && !(alg is SHA384) &&
				   !(alg is SHA512))
				{
					throw new CryptographicException
						(_("Crypto_PKCS1Hash"));
				}
				return alg;
			}

	// PKCS #1 headers for various hash algorithms.
	private static readonly byte[] md5Header =
			{(byte)0x30, (byte)0x20, (byte)0x30, (byte)0x0C,
			 (byte)0x06, (byte)0x08, (byte)0x2A, (byte)0x86,
			 (byte)0x48, (byte)0x86, (byte)0xF7, (byte)0x0D,
			 (byte)0x02, (byte)0x05, (byte)0x05, (byte)0x00,
			 (byte)0x04, (byte)0x10};
	private static readonly byte[] sha1Header =
			{(byte)0x30, (byte)0x21, (byte)0x30, (byte)0x09,
			 (byte)0x06, (byte)0x05, (byte)0x2B, (byte)0x0E,
			 (byte)0x03, (byte)0x02, (byte)0x1A, (byte)0x05,
			 (byte)0x00, (byte)0x04, (byte)0x14};
	private static readonly byte[] sha256Header =
			{(byte)0x30, (byte)0x31, (byte)0x30, (byte)0x0D,
			 (byte)0x06, (byte)0x09, (byte)0x60, (byte)0x86,
			 (byte)0x48, (byte)0x01, (byte)0x65, (byte)0x03,
			 (byte)0x04, (byte)0x02, (byte)0x01, (byte)0x05,
			 (byte)0x00, (byte)0x04, (byte)0x20};
	private static readonly byte[] sha384Header =
			{(byte)0x30, (byte)0x41, (byte)0x30, (byte)0x0D,
			 (byte)0x06, (byte)0x09, (byte)0x60, (byte)0x86,
			 (byte)0x48, (byte)0x01, (byte)0x65, (byte)0x03,
			 (byte)0x04, (byte)0x02, (byte)0x02, (byte)0x05,
			 (byte)0x00, (byte)0x04, (byte)0x30};
	private static readonly byte[] sha512Header =
			{(byte)0x30, (byte)0x41, (byte)0x30, (byte)0x0D,
			 (byte)0x06, (byte)0x09, (byte)0x60, (byte)0x86,
			 (byte)0x48, (byte)0x01, (byte)0x65, (byte)0x03,
			 (byte)0x04, (byte)0x02, (byte)0x03, (byte)0x05,
			 (byte)0x00, (byte)0x04, (byte)0x40};

	// Convert a hash algorithm instance into a PKCS #1 header.
	private byte[] HashAlgToHeader(HashAlgorithm alg)
			{
				if(alg is MD5)
				{
					return md5Header;
				}
				else if(alg is SHA1)
				{
					return sha1Header;
				}
				else if(alg is SHA256)
				{
					return sha256Header;
				}
				else if(alg is SHA384)
				{
					return sha384Header;
				}
				else
				{
					return sha512Header;
				}
			}

	// Convert a hash algorithm name into a PKCS #1 header.
	private byte[] HashAlgToHeader(String alg)
			{
				if(alg == "MD5")
				{
					return md5Header;
				}
				else if(alg == null || alg == "SHA1")
				{
					return sha1Header;
				}
				else if(alg == "SHA256")
				{
					return sha256Header;
				}
				else if(alg == "SHA384")
				{
					return sha384Header;
				}
				else if(alg == "SHA512")
				{
					return sha512Header;
				}
				else
				{
					throw new CryptographicException
						(_("Crypto_PKCS1Hash"));
				}
			}

	// Perform a sign operation.
	private byte[] Sign(byte[] rgbHash, byte[] header)
			{
				// Validate the parameters.
				if(rgbHash == null)
				{
					throw new ArgumentNullException("rgbHash");
				}
				if(rgbHash.Length != header[header.Length - 1])
				{
					throw new CryptographicException
						(_("Crypto_InvalidHashSize"));
				}

				// Make sure that we have sufficient RSA parameters.
				if(rsaParams.Modulus == null)
				{
					throw new CryptographicException
						(_("Crypto_RSAParamsNotSet"));
				}

				// Check the length of the incoming value.
				int k = rsaParams.Modulus.Length;
				if(k < (header.Length + rgbHash.Length + 11))
				{
					throw new CryptographicException
						(_("Crypto_RSAKeyTooShort"));
				}

				// Format the value to be signed.
				byte[] msg = new byte [k];
				msg[1] = (byte)0x01;
				int index, posn;
				posn = k - header.Length - rgbHash.Length;
				for(index = 2; index < (posn - 1); ++index)
				{
					msg[posn] = (byte)0xFF;
				}
				Array.Copy(header, 0, msg, posn, header.Length);
				Array.Copy(rgbHash, 0, msg, posn + header.Length,
						   rgbHash.Length);

				// Sign the value with the private key.
				byte[] signedValue = ApplyPrivate(msg);
				if(signedValue.Length < k)
				{
					// Zero-extend the value if necessary.
					byte[] zextend = new byte [k];
					Array.Copy(signedValue, 0, zextend, k - signedValue.Length,
							   signedValue.Length);
					Array.Clear(signedValue, 0, signedValue.Length);
					signedValue = zextend;
				}

				// Destroy sensitive values.
				Array.Clear(msg, 0, msg.Length);

				// Done.
				return signedValue;
			}

	// Compute a hash value over a buffer and sign it.
	public byte[] SignData(byte[] buffer, Object halg)
			{
				HashAlgorithm alg = ObjectToHashAlg(halg);
				return Sign(alg.ComputeHash(buffer),
							HashAlgToHeader(alg));
			}

	// Compute a hash value over a buffer fragment and sign it.
	public byte[] SignData(byte[] buffer, int offset, int count, Object halg)
			{
				HashAlgorithm alg = ObjectToHashAlg(halg);
				return Sign(alg.ComputeHash(buffer, offset, count),
							HashAlgToHeader(alg));
			}

	// Compute a hash value over a stream and sign it.
	public byte[] SignData(Stream inputStream, Object halg)
			{
				HashAlgorithm alg = ObjectToHashAlg(halg);
				return Sign(alg.ComputeHash(inputStream),
							HashAlgToHeader(alg));
			}

	// Sign an explicit hash value.
	public byte[] SignHash(byte[] rgbHash, String str)
			{
				return Sign(rgbHash, HashAlgToHeader(str));
			}

	// Perform a verify operation.
	private bool Verify(byte[] rgbHash, byte[] header, byte[] rgbSignature)
			{
				// Validate the parameters.
				if(rgbHash == null)
				{
					throw new ArgumentNullException("rgbHash");
				}
				if(rgbHash.Length != header[header.Length - 1])
				{
					throw new CryptographicException
						(_("Crypto_InvalidHashSize"));
				}
				if(rgbSignature == null)
				{
					throw new ArgumentNullException("rgbSignature");
				}

				// Make sure that we have sufficient RSA parameters.
				if(rsaParams.Modulus == null)
				{
					throw new CryptographicException
						(_("Crypto_RSAParamsNotSet"));
				}

				// Check the length of the incoming value.
				int k = rsaParams.Modulus.Length;
				if(k < (header.Length + rgbHash.Length + 11))
				{
					throw new CryptographicException
						(_("Crypto_RSAKeyTooShort"));
				}

				// Decode the signed value.
				byte[] msg = ApplyPublic(rgbSignature);
				if(msg.Length < k)
				{
					// Zero-extend the value if necessary.
					byte[] zextend = new byte [k];
					Array.Copy(msg, 0, zextend, k - msg.Length,
							   msg.Length);
					Array.Clear(msg, 0, msg.Length);
					msg = zextend;
				}

				// Check the decoded value against the expected data.
				bool ok = (msg[0] == 0x00 && msg[1] == 0x01);
				int index, posn;
				posn = k - header.Length - rgbHash.Length;
				for(index = 2; index < (posn - 1); ++index)
				{
					if(msg[posn] != 0xFF)
					{
						ok = false;
					}
				}
				if(msg[posn - 1] != 0x00)
				{
					ok = false;
				}
				for(index = 0; index < rgbHash.Length; ++index)
				{
					if(msg[posn + index] != rgbHash[index])
					{
						ok = false;
					}
				}

				// Destroy sensitive values.
				Array.Clear(msg, 0, msg.Length);

				// Done.
				return ok;
			}

	// Verify the signature on a buffer of data.
	public bool VerifyData(byte[] buffer, Object halg, byte[] rgbSignature)
			{
				HashAlgorithm alg = ObjectToHashAlg(halg);
				return Verify(alg.ComputeHash(buffer),
							  HashAlgToHeader(alg), rgbSignature);
			}

	// Verify a signature from an explicit hash value.
	public bool VerifyHash(byte[] rgbHash, String str, byte[] rgbSignature)
			{
				return Verify(rgbHash, HashAlgToHeader(str), rgbSignature);
			}

}; // class RSACryptoServiceProvider

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
