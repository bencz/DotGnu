/*
 * ECBDecrypt.cs - Implementation of the
 *		"System.Security.Cryptography.ECBDecrypt" class.
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

// Decryption process for "Electronic Code Book" mode (ECB).

internal sealed class ECBDecrypt
{
	// Initialize a "CryptoAPITransform" object for ECB decryption.
	public static void Initialize(CryptoAPITransform transform)
			{
				transform.tempBuffer = new byte [transform.blockSize * 2];
				transform.tempSize = 0;
			}

	// Transform an input block into an output block.
	public static int TransformBlock(CryptoAPITransform transform,
									 byte[] inputBuffer, int inputOffset,
							         int inputCount, byte[] outputBuffer,
							         int outputOffset)
			{
				int blockSize = transform.blockSize;
				byte[] iv = transform.iv;
				IntPtr state = transform.state;
				byte[] tempBuffer = transform.tempBuffer;
				int offset = outputOffset;
				int index;
				bool needPadding = (transform.padding != PaddingMode.None);

				// Process a left-over block from last time.
				if(transform.tempSize > 0 && inputCount > 0)
				{
					// Decrypt the ciphertext to get the plaintext.
					CryptoMethods.Decrypt(state, tempBuffer, 0,
										  outputBuffer, offset);

					// Advance to the next block and clear the temporary block.
					offset += blockSize;
					transform.tempSize = 0;
					for(index = blockSize - 1; index >= 0; --index)
					{
						tempBuffer[index] = (byte)0x00;
					}
				}

				// Process all of the blocks in the input, minus one.
				// If we don't need padding, then process all of the blocks.
				while((needPadding && inputCount > blockSize) ||
				      (!needPadding && inputCount >= blockSize))
				{
					// Decrypt the ciphertext to get the plaintext.
					CryptoMethods.Decrypt(state, inputBuffer, inputOffset,
										  outputBuffer, offset);

					// Advance to the next block.
					inputOffset += blockSize;
					inputCount -= blockSize;
					offset += blockSize;
				}

				// Save the last block for next time.
				if(needPadding && inputCount > 0)
				{
					Array.Copy(inputBuffer, inputOffset,
							   tempBuffer, 0, inputCount);
					transform.tempSize = inputCount;
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
				IntPtr state = transform.state;
				byte[] tempBuffer = transform.tempBuffer;
				byte[] outputBuffer;
				int offset, index, pad;

				// Allocate a temporary output buffer.
				outputBuffer = new byte [inputCount + blockSize];

				// Push the remaining bytes through the decryptor.  The
				// final block will end up in "transform.tempBuffer".
				offset = TransformBlock(transform, inputBuffer, inputOffset,
										inputCount, outputBuffer, 0);

				// Decrypt the final block in "tempBuffer".
				if(transform.tempSize > 0)
				{
					// Decrypt the ciphertext to get the plaintext.
					CryptoMethods.Decrypt(state, tempBuffer, 0,
										  tempBuffer, 0);

					// Remove padding.
					if(transform.padding == PaddingMode.PKCS7)
					{
						// Use PKCS #7 padding.
						pad = tempBuffer[blockSize - 1];
						if(pad == 0 || pad > blockSize)
						{
							pad = blockSize;
						}
						Array.Copy(tempBuffer, 0, outputBuffer,
								   offset, blockSize - pad);
						offset += blockSize - pad;
						pad = 0;
					}
					else if(transform.padding == PaddingMode.Zeros)
					{
						// Strip zeroes from the end of the block.
						index = blockSize;
						while(index > 0 && tempBuffer[index - 1] == 0)
						{
							--index;
						}
						Array.Copy(tempBuffer, 0, outputBuffer,
								   offset, index);
						offset += index;
					}
					else
					{
						// No padding, so return the whole block.
						Array.Copy(tempBuffer, 0, outputBuffer,
								   offset, blockSize);
						offset += blockSize;
					}
				}

				// Reduce the output buffer size to the final length.
				if(offset != outputBuffer.Length)
				{
					byte[] newout = new byte [offset];
					if(offset != 0)
					{
						Array.Copy(outputBuffer, 0, newout, 0, offset);
					}
					Array.Clear(outputBuffer, 0, outputBuffer.Length);
					outputBuffer = newout;
				}

				// Finished.
				return outputBuffer;
			}

}; // class ECBDecrypt

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
