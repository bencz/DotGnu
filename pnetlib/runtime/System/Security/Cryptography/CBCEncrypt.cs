/*
 * CBCEncrypt.cs - Implementation of the
 *		"System.Security.Cryptography.CBCEncrypt" class.
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
using Platform;

// Encryption process for "Cipher Block Chaining" mode (CBC).

internal sealed class CBCEncrypt
{
	// Transform an input block into an output block.
	public static int TransformBlock(CryptoAPITransform transform,
									 byte[] inputBuffer, int inputOffset,
							         int inputCount, byte[] outputBuffer,
							         int outputOffset)
			{
				int blockSize = transform.blockSize;
				byte[] iv = transform.iv;
				IntPtr state = transform.state;
				int offset = outputOffset;
				int index;

				// Process all of the blocks in the input.
				while(inputCount >= blockSize)
				{
					// XOR the plaintext with the IV.
					for(index = blockSize - 1; index >= 0; --index)
					{
						iv[index] ^= inputBuffer[inputOffset + index];
					}

					// Encrypt the IV to get the ciphertext and the next IV.
					CryptoMethods.Encrypt(state, iv, 0, iv, 0);
					Array.Copy(iv, 0, outputBuffer, offset, blockSize);

					// Advance to the next block.
					inputOffset += blockSize;
					inputCount -= blockSize;
					offset += blockSize;
				}

				// Finished.
				return offset - outputOffset;
			}

	// Transform the final input block.
	public static byte[] TransformFinalBlock(CryptoAPITransform transform,
										     byte[] inputBuffer,
									  		 int inputOffset,
									  		 int inputCount)
			{
				int blockSize = transform.blockSize;
				byte[] iv = transform.iv;
				IntPtr state = transform.state;
				int offset = 0;
				int size, pad, index;
				byte[] outputBuffer;

				// Allocate space for the final block.
				if(transform.padding == PaddingMode.PKCS7)
				{
					size = inputCount + blockSize - (inputCount % blockSize);
				}
				else
				{
					size = inputCount;
					if((size % blockSize) != 0)
					{
						size += blockSize - (inputCount % blockSize);
					}
				}
				outputBuffer = new byte [size];

				// Process full blocks in the input.
				while(inputCount >= blockSize)
				{
					// XOR the plaintext with the IV.
					for(index = blockSize - 1; index >= 0; --index)
					{
						iv[index] ^= inputBuffer[inputOffset + index];
					}

					// Encrypt the IV to get the ciphertext and the next IV.
					CryptoMethods.Encrypt(state, iv, 0, iv, 0);
					Array.Copy(iv, 0, outputBuffer, offset, blockSize);

					// Advance to the next block.
					inputOffset += blockSize;
					inputCount -= blockSize;
					offset += blockSize;
				}

				// Format and encrypt the final partial block.
				if(transform.padding == PaddingMode.PKCS7)
				{
					// Pad the block according to PKCS #7 and XOR with the IV.
					for(index = 0; index < inputCount; ++index)
					{
						iv[index] ^= inputBuffer[inputOffset + index];
					}
					pad = blockSize - (inputCount % blockSize);
					while(index < blockSize)
					{
						iv[index] ^= (byte)pad;
						++index;
					}

					// Encrypt the IV to get the ciphertext and the next IV.
					CryptoMethods.Encrypt(state, iv, 0, iv, 0);
					Array.Copy(iv, 0, outputBuffer, offset, blockSize);
				}
				else if(inputCount > 0)
				{
					// Pad the block with zero bytes and XOR with the IV.
					// The zero padding is implicit.
					for(index = 0; index < inputCount; ++index)
					{
						iv[index] ^= inputBuffer[inputOffset + index];
					}

					// Encrypt the IV to get the ciphertext and the next IV.
					CryptoMethods.Encrypt(state, iv, 0, iv, 0);
					Array.Copy(iv, 0, outputBuffer, offset, blockSize);
				}

				// Finished.
				return outputBuffer;
			}

}; // class CBCEncrypt

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
