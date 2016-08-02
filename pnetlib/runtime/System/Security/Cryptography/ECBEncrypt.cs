/*
 * ECBEncrypt.cs - Implementation of the
 *		"System.Security.Cryptography.ECBEncrypt" class.
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

// Encryption process for "Electronic Code Book" mode (ECB).

internal sealed class ECBEncrypt
{
	// Transform an input block into an output block.
	public static int TransformBlock(CryptoAPITransform transform,
									 byte[] inputBuffer, int inputOffset,
							         int inputCount, byte[] outputBuffer,
							         int outputOffset)
			{
				int blockSize = transform.blockSize;
				IntPtr state = transform.state;
				int offset = outputOffset;

				// Process all of the blocks in the input.
				while(inputCount >= blockSize)
				{
					CryptoMethods.Encrypt(state, inputBuffer, inputOffset,
										  outputBuffer, offset);
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
				IntPtr state = transform.state;
				int offset = 0;
				int size, index, pad;
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
					CryptoMethods.Encrypt(state, inputBuffer, inputOffset,
										  outputBuffer, offset);
					inputOffset += blockSize;
					inputCount -= blockSize;
					offset += blockSize;
				}

				// Format and encrypt the final partial block.
				if(transform.padding == PaddingMode.PKCS7)
				{
					// Pad the block according to PKCS #7.
					for(index = 0; index < inputCount; ++index)
					{
						outputBuffer[offset + index] =
							inputBuffer[inputOffset + index];
					}
					pad = blockSize - (inputCount % blockSize);
					while(index < blockSize)
					{
						outputBuffer[offset + index] = (byte)pad;
						++index;
					}

					// Encrypt the block.
					CryptoMethods.Encrypt(state, outputBuffer,
										  offset + index - blockSize,
										  outputBuffer,
										  offset + index - blockSize);
				}
				else if(inputCount > 0)
				{
					// Pad the block with zero bytes.
					for(index = 0; index < inputCount; ++index)
					{
						outputBuffer[offset + index] =
							inputBuffer[inputOffset + index];
					}
					while(index < blockSize)
					{
						outputBuffer[offset + index] = (byte)0x00;
						++index;
					}

					// Encrypt the block.
					CryptoMethods.Encrypt(state, outputBuffer,
										  offset + index - blockSize,
										  outputBuffer,
										  offset + index - blockSize);
				}

				// Finished.
				return outputBuffer;
			}

}; // class ECBEncrypt

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
