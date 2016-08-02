/*
 * CFBEncrypt.cs - Implementation of the
 *		"System.Security.Cryptography.CFBEncrypt" class.
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

// Encryption process for "Cipher Feed Back" mode (CFB).

internal sealed class CFBEncrypt
{
	// Initialize a "CryptoAPITransform" object for CFB encryption.
	public static void Initialize(CryptoAPITransform transform)
			{
				// Initialize the CFB queue with the IV.
				transform.tempBuffer = new byte [transform.blockSize * 2];
				Array.Copy(transform.iv, 0, transform.tempBuffer,
						   transform.feedbackBlockSize, transform.blockSize);
				transform.tempSize = transform.feedbackBlockSize;
			}

	// Transform an input block into an output block.
	public static int TransformBlock(CryptoAPITransform transform,
									 byte[] inputBuffer, int inputOffset,
							         int inputCount, byte[] outputBuffer,
							         int outputOffset)
			{
				int blockSize = transform.blockSize;
				int feedbackSize = transform.feedbackBlockSize;
				byte[] iv = transform.iv;
				IntPtr state = transform.state;
				int offset = outputOffset;
				byte[] tempBuffer = transform.tempBuffer;
				int tempSize = transform.tempSize;

				// Process all of the bytes in the input.
				while(inputCount > 0)
				{
					// Encrypt the queue if we need more keystream data.
					if(tempSize >= feedbackSize)
					{
						CryptoMethods.Encrypt(state, tempBuffer,
											  feedbackSize, tempBuffer, 0);
						tempSize = 0;
					}

					// XOR the plaintext byte with the next keystream byte.
					outputBuffer[offset] =
						(byte)(tempBuffer[tempSize] ^
							   inputBuffer[inputOffset++]);
					--inputCount;

					// Feed the ciphertext byte back into the queue.
					tempBuffer[tempSize + blockSize] = outputBuffer[offset++];
					++tempSize;
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
				byte[] outputBuffer = new byte [inputCount];
				TransformBlock(transform, inputBuffer, inputOffset,
							   inputCount, outputBuffer, 0);
				return outputBuffer;
			}

}; // class CFBEncrypt

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
