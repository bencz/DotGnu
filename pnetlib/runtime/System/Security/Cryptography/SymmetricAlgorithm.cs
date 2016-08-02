/*
 * SymmetricAlgorithm.cs - Implementation of the
 *		"System.Security.Cryptography.SymmetricAlgorithm" class.
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

public abstract class SymmetricAlgorithm : IDisposable
{
	// State that is accessible to subclasses.
	protected int BlockSizeValue;
	protected int FeedbackSizeValue;
	protected byte[] IVValue;
	protected int KeySizeValue;
	protected byte[] KeyValue;
	protected KeySizes[] LegalBlockSizesValue;
	protected KeySizes[] LegalKeySizesValue;
	protected CipherMode ModeValue;
	protected PaddingMode PaddingValue;

	// Create a symmetric algorithm object.
	public static SymmetricAlgorithm Create()
			{
				return (SymmetricAlgorithm)
					(CryptoConfig.CreateFromName
						(CryptoConfig.SymmetricDefault, null));
			}
	public static SymmetricAlgorithm Create(String algName)
			{
				return (SymmetricAlgorithm)
					(CryptoConfig.CreateFromName(algName, null));
			}

	// Constructor.
	public SymmetricAlgorithm()
			{
				ModeValue = CipherMode.CBC;
				PaddingValue = PaddingMode.PKCS7;

				// Note: some implementations check that the subclass is
				// one of the builtin classes.  This is to ensure that
				// the user hasn't made their own cryptographic algorithm.
				//
				// Supposedly this is to obey US export rules that say
				// that plugging a new algorithm into an existing system
				// should not be allowed.
				//
				// However, releasing source code makes such rules meaningless
				// as the user could always modify the source and recompile.
				//
				// Even if this library was binary-only, it wouldn't stop
				// the user implementing their own encryption class that
				// didn't inherit from this class and then use that.
			}

	// Destructor.
	~SymmetricAlgorithm()
			{
				Dispose(false);
			}

	// Get or set the block size.
	public virtual int BlockSize
			{
				get
				{
					return BlockSizeValue;
				}
				set
				{
					KeySizes.Validate(LegalBlockSizesValue, value,
									  "Crypto_InvalidBlockSize");
					BlockSizeValue = value;
				}
			}

	// Get or set the feedback size.
	public virtual int FeedbackSize
			{
				get
				{
					return FeedbackSizeValue;
				}
				set
				{
					if(value > BlockSizeValue)
					{
						throw new CryptographicException
							(_("Crypto_InvalidFeedbackSize"),
							 value.ToString());
					}
					FeedbackSizeValue = value;
				}
			}

	// Get or set the IV.
	public virtual byte[] IV
			{
				get
				{
					if(IVValue == null)
					{
						GenerateIV();
					}
					return IVValue;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					else if(value.Length != BlockSizeValue)
					{
						throw new CryptographicException
							(_("Crypto_InvalidIVSize"), value.ToString());
					}
					IVValue = value;
				}
			}

	// Get or set the key value.
	public virtual byte[] Key
			{
				get
				{
					if(KeyValue == null)
					{
						GenerateKey();
					}
					return KeyValue;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					KeySizes.Validate(LegalKeySizesValue, value.Length * 8,
									  "Crypto_InvalidKeySize");
					KeyValue = value;
					KeySizeValue = value.Length * 8;
				}
			}

	// Get or set the key size value.
	public virtual int KeySize
			{
				get
				{
					return KeySizeValue;
				}
				set
				{
					KeySizes.Validate(LegalKeySizesValue, value,
									  "Crypto_InvalidKeySize");
					KeySizeValue = value;
				}
			}

	// Get the list of legal block sizes.
	public virtual KeySizes[] LegalBlockSizes
			{
				get
				{
					return LegalBlockSizesValue;
				}
			}

	// Get the list of legal key sizes.
	public virtual KeySizes[] LegalKeySizes
			{
				get
				{
					return LegalKeySizesValue;
				}
			}

	// Get or set the cipher mode.
	public virtual CipherMode Mode
			{
				get
				{
					return ModeValue;
				}
				set
				{
					if(value < CipherMode.CBC || value > CipherMode.CTS)
					{
						throw new CryptographicException
							(_("Crypto_InvalidCipherMode"));
					}
					ModeValue = value;
				}
			}

	// Get or set the padding mode.
	public virtual PaddingMode Padding
			{
				get
				{
					return PaddingValue;
				}
				set
				{
					if(value < PaddingMode.None || value > PaddingMode.Zeros)
					{
						throw new CryptographicException
							(_("Crypto_InvalidPaddingMode"));
					}
					PaddingValue = value;
				}
			}

	// Clear the state of this object.
	public void Clear()
			{
				((IDisposable)this).Dispose();
			}

	// Dispose the state of this object.
	void IDisposable.Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
	protected virtual void Dispose(bool disposing)
			{
				if(KeyValue != null)
				{
					Array.Clear(KeyValue, 0, KeyValue.Length);
				}
				if(IVValue != null)
				{
					Array.Clear(IVValue, 0, IVValue.Length);
				}
			}

	// Create a decryptor object with the current key and IV.
	public virtual ICryptoTransform CreateDecryptor()
			{
				return CreateDecryptor(Key, IV);
			}

	// Create a decryptor object with a specific key and IV.
	public abstract ICryptoTransform CreateDecryptor
				(byte[] rgbKey, byte[] rgbIV);

	// Create an encryptor object with the current key and IV.
	public virtual ICryptoTransform CreateEncryptor()
			{
				return CreateEncryptor(Key, IV);
			}
	public abstract ICryptoTransform CreateEncryptor
				(byte[] rgbKey, byte[] rgbIV);

	// Generate a random IV.
	public abstract void GenerateIV();

	// Generate a random key.
	public abstract void GenerateKey();

	// Determine if a key size is valid.
	public bool ValidKeySize(int bitLength)
			{
				return KeySizes.Validate(LegalKeySizesValue, bitLength);
			}

	// Validate "rgbKey" and "rgbIV" values for a call on
	// "CreateDecryptor" or "CreateEncryptor".
	internal void ValidateCreate(byte[] rgbKey, byte[] rgbIV)
			{
				if(rgbKey == null)
				{
					throw new ArgumentNullException("rgbKey");
				}
				KeySizes.Validate(LegalKeySizesValue, rgbKey.Length * 8,
								  "Crypto_InvalidKeySize");
				if(rgbIV != null)
				{
					// Verify the size of the IV against the block size.
					if((rgbIV.Length * 8) != BlockSizeValue)
					{
						throw new CryptographicException
							(_("Crypto_InvalidIVSize"),
							 rgbIV.Length.ToString());
					}
				}
				else if(ModeValue != CipherMode.ECB)
				{
					// We must have an IV in every mode except ECB.
					throw new ArgumentNullException("rgbIV");
				}
			}

}; // class SymmetricAlgorithm

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
