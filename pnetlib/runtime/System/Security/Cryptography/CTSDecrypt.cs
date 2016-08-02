/*
 * CTSDecrypt.cs - Implementation of the
 *		"System.Security.Cryptography.CTSDecrypt" class.
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

// Decryption process for "Cipher Text Stealing" mode (CTS).

internal sealed class CTSDecrypt
{
	// Initialize a "CryptoAPITransform" object for CTS decryption.
	public static void Initialize(CryptoAPITransform transform)
			{
				transform.tempBuffer = new byte [transform.blockSize * 3];
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
				int offset = outputOffset;
				byte[] tempBuffer = transform.tempBuffer;
				int tempSize = transform.tempSize;
				int index;

				// Process all of the data in the input.  We need to keep
				// the last two blocks for the finalization process.
				while(inputCount >= blockSize)
				{
					// If the temporary buffer is full, then flush a block
					// through the cipher in CBC mode.
					if(tempSize > blockSize)
					{
						// Decrypt the ciphertext block and XOR with the IV.
						CryptoMethods.Decrypt(state, tempBuffer, blockSize,
											  tempBuffer, 0);
						for(index = blockSize - 1; index >= 0; --index)
						{
							tempBuffer[index] ^= iv[index];
						}

						// Copy the original ciphertext to the IV.
						Array.Copy(tempBuffer, blockSize, iv, 0, blockSize);

						// Copy the plaintext into place.
						Array.Copy(tempBuffer, 0, outputBuffer,
								   offset, blockSize);

						// Advance to the next output block.
						offset += blockSize;

						// Shift the second block down to the first position.
						Array.Copy(tempBuffer, blockSize * 2,
								   tempBuffer, blockSize, blockSize);
						tempSize -= blockSize;
					}

					// Copy the next block into the temporary buffer.
					Array.Copy(inputBuffer, inputOffset,
							   tempBuffer, tempSize + blockSize, blockSize);
					inputOffset += blockSize;
					inputCount -= blockSize;
					tempSize += blockSize;
				}
				transform.tempSize = tempSize;

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
				int offset;
				byte[] tempBuffer = transform.tempBuffer;
				byte[] outputBuffer;
				int tempSize;
				int index;

				// Allocate the output buffer.
				outputBuffer = new byte [inputCount + transform.tempSize];

				// Process as many full blocks as possible.
				index = inputCount - (inputCount % blockSize);
				offset = TransformBlock(transform, inputBuffer,
									    inputOffset, index,
										outputBuffer, 0);
				inputOffset += index;
				inputCount -= index;

				// Flush the first block if we need the extra space.
				tempSize = transform.tempSize;
				if(tempSize > blockSize && inputCount > 0)
				{
					// Decrypt the ciphertext block and XOR with the IV.
					CryptoMethods.Decrypt(state, tempBuffer, blockSize,
										  tempBuffer, 0);
					for(index = blockSize - 1; index >= 0; --index)
					{
						tempBuffer[index] ^= iv[index];
					}

					// Copy the original ciphertext to the IV.
					Array.Copy(tempBuffer, blockSize, iv, 0, blockSize);

					// Copy the plaintext into place.
					Array.Copy(tempBuffer, 0, outputBuffer,
							   offset, blockSize);

					// Advance to the next output block.
					offset += blockSize;

					// Shift the second block down to the first position.
					Array.Copy(tempBuffer, blockSize * 2,
							   tempBuffer, blockSize, blockSize);
					tempSize -= blockSize;
				}

				// Copy the remainder of the data into the temporary buffer.
				Array.Copy(inputBuffer, inputOffset,
						   tempBuffer, tempSize + blockSize, inputCount);
				tempSize += inputCount;

				// "Applied Cryptography" describes Cipher Text Stealing
				// as taking two blocks to generate the short end-point.
				// If we less than one block, then use CFB instead.
				if(tempSize < blockSize)
				{
					// Decrypt the single block in CFB mode.
					CryptoMethods.Encrypt(transform.state2, iv, 0, iv, 0);
					for(index = 0; index < tempSize; ++index)
					{
						outputBuffer[offset + index] =
							(byte)(iv[index] ^ tempBuffer[index + blockSize]);
					}
				}
				else
				{
					// Decrypt the second last ciphertext block.
					CryptoMethods.Decrypt(state, tempBuffer, blockSize,
										  tempBuffer, blockSize);

					// Rebuild the ciphertext for the last block.
					for(index = inputCount; index < blockSize; ++index)
					{
						tempBuffer[blockSize * 2 + index] =
								tempBuffer[blockSize + index];
					}

					// Get the last plaintext block from the second
					// last ciphertext block.
					for(index = inputCount - 1; index >= 0; --index)
					{
						outputBuffer[offset + blockSize + index] =
							(byte)(tempBuffer[blockSize + index] ^
								   tempBuffer[blockSize * 2 + index]);
					}

					// Decrypt the last ciphertext block that we rebuilt.
					CryptoMethods.Decrypt(state, tempBuffer, blockSize * 2,
										  tempBuffer, 0);

					// XOR the block with the IV to get the second
					// last plaintext block.
					for(index = blockSize - 1; index >= 0; --index)
					{
						outputBuffer[offset + index] =
							(byte)(iv[index] ^ tempBuffer[index]);
					}
				}

				// Finished.
				return outputBuffer;
			}

}; // class CTSDecrypt

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
