/*
 * CryptoAPITransform.cs - Implementation of the
 *		"System.Security.Cryptography.CryptoAPITransform" class.
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

// This is a helper class that is used by the DES, TripleDES, RC2,
// and Rijndael symmetric algorithm classes to create encryptor and
// decryptor objects that call through to the engine to do the work.
//
// This class in turn calls out to classes such as "CBCEncrypt",
// "ECBDecrypt", etc to handle each of the cipher modes.

public sealed class CryptoAPITransform : ICryptoTransform, IDisposable
{
	// Delegate types for the mode-specific processing routines.
	private delegate int ProcessBlock(CryptoAPITransform transform,
								      byte[] inputBuffer, int inputOffset,
									  int inputCount, byte[] outputBuffer,
									  int outputOffset);
	private delegate byte[] ProcessFinal(CryptoAPITransform transform,
								         byte[] inputBuffer, int inputOffset,
									     int inputCount);

	// Internal state.
	internal IntPtr state;
	internal IntPtr state2;
	internal byte[] iv;
	internal int blockSize;
	internal int feedbackBlockSize;
	internal PaddingMode padding;
	internal byte[] tempBuffer;
	internal int tempSize;
	private CipherMode mode;
	private ProcessBlock processBlock;
	private ProcessFinal processFinal;

	// Constructor.
	internal CryptoAPITransform(int algorithm, byte[] iv, byte[] key,
								int blockSize, int feedbackBlockSize,
								CipherMode mode, PaddingMode padding,
								bool encrypt)
			{
				// Initialize the common state.
				if(iv != null)
				{
					this.iv = (byte[])(iv.Clone());
				}
				else
				{
					this.iv = null;
				}
				this.blockSize = blockSize / 8;
				this.feedbackBlockSize = feedbackBlockSize / 8;
				this.padding = padding;
				this.mode = mode;

				// Determine which processing methods to use based on the
				// mode and the encrypt/decrypt flag.
				switch(mode)
				{
					case CipherMode.CBC:
					{
						// Cipher Block Chaining Mode.
						if(encrypt)
						{
							state = CryptoMethods.EncryptCreate(algorithm, key);
							processBlock = new ProcessBlock
								(CBCEncrypt.TransformBlock);
							processFinal = new ProcessFinal
								(CBCEncrypt.TransformFinalBlock);
						}
						else
						{
							CBCDecrypt.Initialize(this);
							state = CryptoMethods.DecryptCreate(algorithm, key);
							processBlock = new ProcessBlock
								(CBCDecrypt.TransformBlock);
							processFinal = new ProcessFinal
								(CBCDecrypt.TransformFinalBlock);
						}
					}
					break;

					case CipherMode.ECB:
					{
						// Electronic Code Book Mode.
						if(encrypt)
						{
							state = CryptoMethods.EncryptCreate(algorithm, key);
							processBlock = new ProcessBlock
								(ECBEncrypt.TransformBlock);
							processFinal = new ProcessFinal
								(ECBEncrypt.TransformFinalBlock);
						}
						else
						{
							ECBDecrypt.Initialize(this);
							state = CryptoMethods.DecryptCreate(algorithm, key);
							processBlock = new ProcessBlock
								(ECBDecrypt.TransformBlock);
							processFinal = new ProcessFinal
								(ECBDecrypt.TransformFinalBlock);
						}
					}
					break;

					case CipherMode.OFB:
					{
						// Output Feed Back Mode.
						OFBEncrypt.Initialize(this);
						state = CryptoMethods.EncryptCreate(algorithm, key);
						processBlock = new ProcessBlock
							(OFBEncrypt.TransformBlock);
						processFinal = new ProcessFinal
							(OFBEncrypt.TransformFinalBlock);
					}
					break;

					case CipherMode.CFB:
					{
						// Cipher Feed Back Mode.
						if(encrypt)
						{
							CFBEncrypt.Initialize(this);
							state = CryptoMethods.EncryptCreate(algorithm, key);
							processBlock = new ProcessBlock
								(CFBEncrypt.TransformBlock);
							processFinal = new ProcessFinal
								(CFBEncrypt.TransformFinalBlock);
						}
						else
						{
							CFBDecrypt.Initialize(this);
							state = CryptoMethods.EncryptCreate(algorithm, key);
							processBlock = new ProcessBlock
								(CFBDecrypt.TransformBlock);
							processFinal = new ProcessFinal
								(CFBDecrypt.TransformFinalBlock);
						}
					}
					break;

					case CipherMode.CTS:
					{
						// Cipher Text Stealing Mode.
						if(encrypt)
						{
							CTSEncrypt.Initialize(this);
							state = CryptoMethods.EncryptCreate(algorithm, key);
							processBlock = new ProcessBlock
								(CTSEncrypt.TransformBlock);
							processFinal = new ProcessFinal
								(CTSEncrypt.TransformFinalBlock);
						}
						else
						{
							// We need an encryptor as well to handle
							// streams with only a single block in them.
							CTSDecrypt.Initialize(this);
							state = CryptoMethods.DecryptCreate(algorithm, key);
							state2 = CryptoMethods.EncryptCreate
								(algorithm, key);
							processBlock = new ProcessBlock
								(CTSDecrypt.TransformBlock);
							processFinal = new ProcessFinal
								(CTSDecrypt.TransformFinalBlock);
						}
					}
					break;
				}
			}

	// Destructor.
	~CryptoAPITransform()
			{
				Dispose(false);
			}

	// Clear sensitive state values.
	public void Clear()
			{
				((IDisposable)this).Dispose();
			}

	// Dispose this object.
	void IDisposable.Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
	private void Dispose(bool disposing)
			{
				if(state != IntPtr.Zero)
				{
					CryptoMethods.SymmetricFree(state);
					state = IntPtr.Zero;
				}
				if(state2 != IntPtr.Zero)
				{
					CryptoMethods.SymmetricFree(state2);
					state2 = IntPtr.Zero;
				}
				if(tempBuffer != null)
				{
					Array.Clear(tempBuffer, 0, tempBuffer.Length);
				}
				tempSize = 0;
				if(iv != null)
				{
					// Usually not sensitive, but let's be paranoid anyway.
					Array.Clear(iv, 0, iv.Length);
				}
			}

	// Determine if we can reuse this transform object.
	public bool CanReuseTransform
			{
				get
				{
					return true;
				}
			}

	// Determine if multiple blocks can be transformed.
	public bool CanTransformMultipleBlocks
			{
				get
				{
					return true;
				}
			}

	// Get the input block size.
	public int InputBlockSize
			{
				get
				{
					if(mode == CipherMode.ECB || mode == CipherMode.CBC)
					{
						return blockSize;
					}
					else
					{
						return 1;
					}
				}
			}

	// Get the output block size.
	public int OutputBlockSize
			{
				get
				{
					if(mode == CipherMode.ECB || mode == CipherMode.CBC)
					{
						return blockSize;
					}
					else
					{
						return 1;
					}
				}
			}

	// Get the key handle.
	public IntPtr KeyHandle
			{
				get
				{
					// We don't support key handles in this implementation.
					return IntPtr.Zero;
				}
			}

	// Transform an input block into an output block.
	public int TransformBlock(byte[] inputBuffer, int inputOffset,
							  int inputCount, byte[] outputBuffer,
							  int outputOffset)
			{
				// Validate the parameters.
				if(inputBuffer == null)
				{
					throw new ArgumentNullException("inputBuffer");
				}
				else if(inputOffset < 0 || inputOffset > inputBuffer.Length)
				{
					throw new ArgumentOutOfRangeException
						("inputOffset", _("ArgRange_Array"));
				}
				else if(inputCount < 0 ||
				        inputCount > (inputBuffer.Length - inputOffset))
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}
				if(outputBuffer == null)
				{
					throw new ArgumentNullException("outputBuffer");
				}
				else if(outputOffset < 0 || outputOffset > outputBuffer.Length)
				{
					throw new ArgumentOutOfRangeException
						("outputOffset", _("ArgRange_Array"));
				}
				if(state == IntPtr.Zero)
				{
					throw new ObjectDisposedException
						(null, _("Crypto_MissingKey"));
				}

				// Process the input.
				return processBlock(this, inputBuffer, inputOffset,
									inputCount, outputBuffer, outputOffset);
			}

	// Transform the final input block.
	public byte[] TransformFinalBlock(byte[] inputBuffer,
									  int inputOffset,
									  int inputCount)
			{
				byte[] outputBuffer;

				// Validate the parameters.
				if(inputBuffer == null)
				{
					throw new ArgumentNullException("inputBuffer");
				}
				else if(inputOffset < 0 || inputOffset > inputBuffer.Length)
				{
					throw new ArgumentOutOfRangeException
						("inputOffset", _("ArgRange_Array"));
				}
				else if(inputCount < 0 ||
				        inputCount > (inputBuffer.Length - inputOffset))
				{
					throw new ArgumentException(_("Arg_InvalidArrayRange"));
				}
				if(state == IntPtr.Zero)
				{
					throw new ObjectDisposedException
						(null, _("Crypto_MissingKey"));
				}

				// Process the input.
				outputBuffer = processFinal(this, inputBuffer,
								    		inputOffset, inputCount);

				// Clear sensitive state values.
				Clear();

				// Return the final output buffer to the caller.
				return outputBuffer;
			}

}; // class CryptoAPITransform

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
