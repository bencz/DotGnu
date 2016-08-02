/*
 * CryptoTestCase.cs - encapsulate a cryptographic algorithm test case.
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

using CSUnit;
using System;
using System.Reflection;
using System.IO;
using System.Text;
using System.Security.Cryptography;

#if CONFIG_CRYPTO

public class CryptoTestCase : TestCase
{

	// Constructor.
	public CryptoTestCase(String name)
			: base(name)
			{
				// Nothing to do here.	
			}

	// Set up for the tests.
	protected override void Setup()
			{
				// Nothing to do here.
			}

	// Clean up after the tests.
	protected override void Cleanup()
			{
				// Nothing to do here.
			}
	
	// Determine if two byte blocks are identical.
	public static bool IdenticalBlock(byte[] block1, int offset1,
									  byte[] block2, int offset2,
									  int length)
			{
				while(length > 0)
				{
					if(block1[offset1++] != block2[offset2++])
					{
						return false;
					}
					--length;
				}
				return true;
			}

	// Determine if the random number generator appears to be working.
	// We use this before calling "GenerateKey" for "DES" and "TripleDES",
	// to prevent infinite loops in the test suite.
	public static bool RandomWorks()
			{
				int index;
				byte[] rand = new byte [16];
				RandomNumberGenerator rng = RandomNumberGenerator.Create();
				rng.GetBytes(rand);
				for(index = 0; index < 16; ++index)
				{
					if(rand[index] != 0x00)
					{
						return true;
					}
				}
				return false;
			}

	// Run a symmetric algorithm test.
	protected void RunSymmetric(SymmetricAlgorithm alg, byte[] key,
								byte[] plaintext, byte[] expected)
			{
				// Set up the algorithm the way we want.
				alg.Mode = CipherMode.ECB;
				alg.Padding = PaddingMode.None;

				// Create an encryptor and run the test forwards.
				ICryptoTransform encryptor = alg.CreateEncryptor(key, null);
				byte[] output = new byte [plaintext.Length * 2];
				byte[] tail;
				int len = encryptor.TransformBlock
						(plaintext, 0, plaintext.Length,
						 output, 0);
				AssertEquals("ECB encrypt length mismatch",
							 len, expected.Length);
				tail = encryptor.TransformFinalBlock
						(plaintext, 0, 0);
				AssertNotNull("ECB encrypt tail should be non-null");
				AssertEquals("ECB encrypt tail should be zero length",
							 tail.Length, 0);
				if(!IdenticalBlock(expected, 0, output, 0,
								   expected.Length))
				{
					Fail("did not encrypt to the expected output");
				}
				encryptor.Dispose();

				// Create a decryptor and run the test backwards.
				ICryptoTransform decryptor = alg.CreateDecryptor(key, null);
				len = decryptor.TransformBlock
						(expected, 0, expected.Length, output, 0);
				AssertEquals("ECB decrypt length mismatch",
							 len, expected.Length);
				tail = decryptor.TransformFinalBlock
						(expected, 0, 0);
				AssertNotNull("ECB decrypt tail should be non-null");
				AssertEquals("ECB decrypt tail should be zero length",
							 tail.Length, 0);
				if(!IdenticalBlock(plaintext, 0, output, 0,
								   plaintext.Length))
				{
					Fail("did not decrypt to the original plaintext");
				}
				decryptor.Dispose();
			}
	protected void RunSymmetric(String name, byte[] key,
								byte[] plaintext, byte[] expected)
			{
				int index = name.IndexOf(':');
				if(index != -1)
				{
					// Use the default algorithm.
					Type type = Type.GetType("System.Security.Cryptography." +
											 name.Substring(0, index),
											 false, false);
					Object[] args;
					if((index + 1) < name.Length)
					{
						args = new Object [1];
						args[0] = name.Substring(index + 1);
					}
					else
					{
						args = new Object [0];
					}
					SymmetricAlgorithm alg = (SymmetricAlgorithm)
						(type.InvokeMember("Create",
										   BindingFlags.DeclaredOnly |
										   BindingFlags.Static |
										   BindingFlags.Public |
										   BindingFlags.InvokeMethod,
										   null, null, args));
					AssertEquals("default key size is wrong",
					             alg.KeySize, key.Length * 8);
					RunSymmetric(alg, key, plaintext, expected);
				}
				else
				{
					// Use the specified algorithm.
					RunSymmetric(SymmetricAlgorithm.Create(name), key,
								 plaintext, expected);
				}
			}

	// Run a hash algorithm test.
	protected void RunHash(HashAlgorithm alg, String value, byte[] expected)
			{
				// Make sure that the hash size is what we expect.
				AssertEquals("hash size is incorrect",
							 alg.HashSize, expected.Length * 8);

				// Convert the string form of the input into a byte array.
				byte[] input = Encoding.ASCII.GetBytes(value);

				// Get the hash value over the input.
				byte[] hash = alg.ComputeHash(input);

				// Compare the hash with the expected value.
				AssertNotNull("returned hash was null", hash);
				AssertEquals("hash length is wrong", hash.Length,
							 expected.Length);
				if(!IdenticalBlock(hash, 0, expected, 0, expected.Length))
				{
					Fail("incorrect hash value produced");
				}

				// Get the hash value over the input in a sub-buffer.
				byte[] input2 = new byte [input.Length + 20];
				Array.Copy(input, 0, input2, 10, input.Length);
				hash = alg.ComputeHash(input2, 10, input.Length);

				// Compare the hash with the expected value.
				AssertNotNull("returned hash was null", hash);
				AssertEquals("hash length is wrong", hash.Length,
							 expected.Length);
				if(!IdenticalBlock(hash, 0, expected, 0, expected.Length))
				{
					Fail("incorrect hash value produced");
				}

				// Get the hash value over the input via a stream.
				MemoryStream stream = new MemoryStream(input, false);
				hash = alg.ComputeHash(stream);

				// Compare the hash with the expected value.
				AssertNotNull("returned hash was null", hash);
				AssertEquals("hash length is wrong", hash.Length,
							 expected.Length);
				if(!IdenticalBlock(hash, 0, expected, 0, expected.Length))
				{
					Fail("incorrect hash value produced");
				}
			}
	protected void RunHash(String name, String value, byte[] expected)
			{
				int index = name.IndexOf(':');
				if(index != -1)
				{
					// Use the default algorithm.
					Type type = Type.GetType("System.Security.Cryptography." +
											 name.Substring(0, index),
											 false, false);
					Object[] args;
					if((index + 1) < name.Length)
					{
						args = new Object [1];
						args[0] = name.Substring(index + 1);
					}
					else
					{
						args = new Object [0];
					}
					HashAlgorithm alg = (HashAlgorithm)
						(type.InvokeMember("Create",
										   BindingFlags.DeclaredOnly |
										   BindingFlags.Static |
										   BindingFlags.Public |
										   BindingFlags.InvokeMethod,
										   null, null, args));
					RunHash(alg, value, expected);
				}
				else
				{
					// Use the specified algorithm.
					RunHash(HashAlgorithm.Create(name), value, expected);
				}
			}

	// Check that a size value is in a size list.
	private void CheckSize(String msg, KeySizes[] sizes, int value)
			{
				foreach(KeySizes size in sizes)
				{
					if(value >= size.MinSize && value <= size.MaxSize &&
					   ((value - size.MinSize) % size.SkipSize) == 0)
					{
						return;
					}
				}
				Fail(msg);
			}

	// Test the properties on a symmetric algorithm instance.
	protected void SymmetricPropertyTest(SymmetricAlgorithm alg,
										 int expectedKeySize,
										 int expectedBlockSize)
			{
				// Check the initial property values.
				AssertEquals("initial key size is incorrect",
							 expectedKeySize, alg.KeySize);
				AssertEquals("initial block size is incorrect",
							 expectedBlockSize, alg.BlockSize);
				AssertEquals("initial feedback block size is incorrect",
							 expectedBlockSize, alg.FeedbackSize);
				AssertEquals("initial cipher mode is incorrect",
							 CipherMode.CBC, alg.Mode);
				AssertEquals("initial padding mode is incorrect",
							 PaddingMode.PKCS7, alg.Padding);
				AssertNotNull("legal key size array is null",
							  alg.LegalKeySizes);
				AssertNotNull("legal block size array is null",
							  alg.LegalBlockSizes);

				// Check that the size values are initially valid.
				CheckSize("initial key size is not legal",
						  alg.LegalKeySizes, alg.KeySize);
				CheckSize("initial block size is not legal",
						  alg.LegalBlockSizes, alg.BlockSize);

				// TODO: Try setting the key size to all legal values.

				// Check automatic key and IV generation.  If the random
				// number generator doesn't work, then skip the test for
				// DES and TripleDES, to prevent an infinite loop within
				// those algorithm's weak key checking code.
				if((!(alg is DES) && !(alg is TripleDES)) || RandomWorks())
				{
					byte[] key = alg.Key;
					AssertNotNull("generated key should not be null", key);
					AssertEquals("generated key is the wrong size",
								 alg.KeySize, key.Length * 8);
					byte[] iv = alg.IV;
					AssertNotNull("generated IV should not be null", iv);
					AssertEquals("generated IV is the wrong size",
								 alg.BlockSize, iv.Length * 8);
				}
			}

	// Test the properties on a hash algorithm instance.
	protected void HashPropertyTest(HashAlgorithm alg, int expectedHashSize)
			{
				AssertEquals("hash size is incorrect",
							 alg.HashSize, expectedHashSize);
				AssertEquals("input block size is incorrect",
							 alg.InputBlockSize, 1);
				AssertEquals("output block size is incorrect",
							 alg.OutputBlockSize, 1);
				AssertEquals("multiple block transform flag is incorrect",
							 alg.CanTransformMultipleBlocks, true);
				try
				{
					byte[] hash = alg.Hash;
					Fail("should not be able to get the hash yet");
				}
				catch(CryptographicException)
				{
					// Success
				}
			}

	// Perform a primitive ECB encryption on a block.
	private void ECBBlock(byte[] buf, int index,
						  SymmetricAlgorithm alg, byte[] key)
			{
				ICryptoTransform encryptor;
				CipherMode mode = alg.Mode;
				PaddingMode padding = alg.Padding;
				alg.Mode = CipherMode.ECB;
				alg.Padding = PaddingMode.None;
				encryptor = alg.CreateEncryptor(key, null);
				alg.Mode = mode;
				alg.Padding = padding;
				encryptor.TransformBlock(buf, index, alg.BlockSize / 8,
										 buf, index);
				encryptor.Dispose();
			}

	// XOR two blocks.
	private void XorBlock(byte[] buf1, int index1, byte[] buf2,
					      int index2, SymmetricAlgorithm alg)
			{
				int length = alg.BlockSize / 8;
				while(length-- > 0)
				{
					buf1[index1++] ^= buf2[index2++];
				}
			}

	// XOR two blocks.
	private void XorBlock(byte[] buf1, int index1, byte[] buf2,
					      int index2, int length)
			{
				while(length-- > 0)
				{
					buf1[index1++] ^= buf2[index2++];
				}
			}

	// Copy one block to another.
	private void CopyBlock(byte[] src, int srcIndex,
						   byte[] dest, int destIndex,
						   SymmetricAlgorithm alg)
			{
				int length = alg.BlockSize / 8;
				while(length-- > 0)
				{
					dest[destIndex++] = src[srcIndex++];
				}
			}

	// Convert a string into a byte array, with padding applied.
	private byte[] StringToBytes(String str, SymmetricAlgorithm alg)
			{
				PaddingMode padding = alg.Padding;
				CipherMode cipher = alg.Mode;
				int size = alg.BlockSize / 8;
				int len, pad;
				byte[] input = Encoding.ASCII.GetBytes(str);
				byte[] padded;
				if(cipher == CipherMode.ECB || cipher == CipherMode.CBC)
				{
					// Block cipher mode - zero or PKCS7 padding only.
					if(padding == PaddingMode.None)
					{
						padding = PaddingMode.Zeros;
					}
				}
				else
				{
					// Stream cipher mode - padding is never required.
					padding = PaddingMode.None;
				}
				switch(padding)
				{
					case PaddingMode.None: break;

					case PaddingMode.PKCS7:
					{
						len = input.Length;
						len += size - (len % size);
						pad = len - input.Length;
						padded = new byte [len];
						Array.Copy(input, 0, padded, 0, input.Length);
						len = input.Length;
						while(len < padded.Length)
						{
							padded[len++] = (byte)pad;
						}
						input = padded;
					}
					break;

					case PaddingMode.Zeros:
					{
						len = input.Length;
						if((len % size) != 0)
						{
							len += size - (len % size);
						}
						padded = new byte [len];
						Array.Copy(input, 0, padded, 0, input.Length);
						input = padded;
					}
					break;
				}
				return input;
			}

	// Create a test key for a specific algorihtm.
	private byte[] CreateKey(SymmetricAlgorithm alg)
			{
				byte[] key = new byte [alg.KeySize / 8];
				int posn;
				for(posn = 0; posn < key.Length; ++posn)
				{
					key[posn] = (byte)posn;
				}
				return key;
			}

	// Create a test IV for a specific algorithm.
	private byte[] CreateIV(SymmetricAlgorithm alg)
			{
				if(alg.Mode == CipherMode.ECB)
				{
					// ECB modes don't need an IV.
					return null;
				}
				else
				{
					// All other modes do need an IV.
					byte[] iv = new byte [alg.BlockSize / 8];
					int posn;
					for(posn = 0; posn < iv.Length; ++posn)
					{
						iv[posn] = (byte)(iv.Length - posn);
					}
					return iv;
				}
			}

	// ECB-encrypt a buffer.
	private byte[] DoECB(byte[] input, SymmetricAlgorithm alg, byte[] key)
			{
				byte[] output = new byte [input.Length];
				int size = alg.BlockSize / 8;
				Array.Copy(input, 0, output, 0, input.Length);
				int index = 0;
				while(index < input.Length)
				{
					ECBBlock(output, index, alg, key);
					index += size;
				}
				return output;
			}

	// CBC-encrypt a buffer.
	private byte[] DoCBC(byte[] input, SymmetricAlgorithm alg,
						 byte[] key, byte[] _iv)
			{
				byte[] iv = new byte [_iv.Length];
				Array.Copy(_iv, 0, iv, 0, _iv.Length);
				byte[] output = new byte [input.Length];
				int size = alg.BlockSize / 8;
				Array.Copy(input, 0, output, 0, input.Length);
				int index = 0;
				while(index < input.Length)
				{
					XorBlock(output, index, iv, 0, alg);
					ECBBlock(output, index, alg, key);
					CopyBlock(output, index, iv, 0, alg);
					index += size;
				}
				return output;
			}

	// OFB-encrypt a buffer.
	private byte[] DoOFB(byte[] input, SymmetricAlgorithm alg,
						 byte[] key, byte[] _iv)
			{
				byte[] iv = new byte [_iv.Length];
				Array.Copy(_iv, 0, iv, 0, _iv.Length);
				byte[] output = new byte [input.Length];
				Array.Copy(input, 0, output, 0, input.Length);
				int size = alg.BlockSize / 8;
				int index = 0;
				while(index < input.Length)
				{
					ECBBlock(iv, 0, alg, key);
					if((input.Length - index) >= size)
					{
						XorBlock(output, index, iv, 0, alg);
					}
					else
					{
						XorBlock(output, index, iv, 0, input.Length - index);
					}
					index += size;
				}
				return output;
			}

	// CFB-encrypt a buffer.
	private byte[] DoCFB(byte[] input, SymmetricAlgorithm alg,
						 byte[] key, byte[] _iv)
			{
				byte[] iv = new byte [_iv.Length];
				Array.Copy(_iv, 0, iv, 0, _iv.Length);
				byte[] output = new byte [input.Length];
				Array.Copy(input, 0, output, 0, input.Length);
				int size = alg.BlockSize / 8;
				int index = 0;
				while(index < input.Length)
				{
					ECBBlock(iv, 0, alg, key);
					if((input.Length - index) >= size)
					{
						XorBlock(output, index, iv, 0, alg);
						CopyBlock(output, index, iv, 0, alg);
					}
					else
					{
						XorBlock(output, index, iv, 0, input.Length - index);
					}
					index += size;
				}
				return output;
			}

	// CTS-encrypt a buffer.
	private byte[] DoCTS(byte[] input, SymmetricAlgorithm alg,
						 byte[] key, byte[] _iv)
			{
				if(input.Length < (alg.BlockSize / 8))
				{
					// Streams shorter than one block are CFB-encrypted.
					return DoCFB(input, alg, key, _iv);
				}
				byte[] iv = new byte [_iv.Length];
				Array.Copy(_iv, 0, iv, 0, _iv.Length);
				byte[] output = new byte [input.Length];
				int size = alg.BlockSize / 8;
				Array.Copy(input, 0, output, 0, input.Length);
				int index = 0;
				int limit = input.Length;
				limit -= limit % size;
				limit -= size;

				// Encrypt the bulk of the input with CBC.
				while(index < limit)
				{
					XorBlock(output, index, iv, 0, alg);
					ECBBlock(output, index, alg, key);
					CopyBlock(output, index, iv, 0, alg);
					index += size;
				}

				// Encrypt the last two blocks using ciphertext stealing.
				byte[] last = new byte [size * 2];
				Array.Copy(output, index, last, 0, input.Length - limit);
				XorBlock(last, 0, iv, 0, alg);
				ECBBlock(last, 0, alg, key);
				XorBlock(last, size, last, 0, alg);
				ECBBlock(last, size, alg, key);
				Array.Copy(last, size, output, index, size);
				Array.Copy(last, 0, output, index + size,
						   input.Length % size);
				return output;
			}

	// Get a string that describes a particular cipher mode test,
	// for use in error messages.
	private static String GetError(String msg, SymmetricAlgorithm alg,
								   String input)
			{
				return msg + String.Format
					(" ({0}, {1}, \"{2}\")", alg.Mode, alg.Padding, input);
			}

	// Run a cipher mode test.
	private void RunModeTest(SymmetricAlgorithm alg, CipherMode mode,
							 PaddingMode padding, String input)
			{
				// Set the algorithm modes.
				alg.Mode = mode;
				alg.Padding = padding;

				// Get the raw and padded versions of the input.
				byte[] rawInput = Encoding.ASCII.GetBytes(input);
				byte[] paddedInput = StringToBytes(input, alg);

				// Generate key and IV values.
				byte[] key = CreateKey(alg);
				byte[] iv = CreateIV(alg);

				// Encrypt the raw input in the selected mode.
				int size = alg.BlockSize / 8;
				int cutoff = rawInput.Length - rawInput.Length % size;
				ICryptoTransform encryptor;
				encryptor = alg.CreateEncryptor(key, iv);
				Assert(GetError("encryptor cannot transform multiple blocks",
								alg, input),
					   encryptor.CanTransformMultipleBlocks);
				if(mode == CipherMode.ECB || mode == CipherMode.CBC)
				{
					AssertEquals(GetError("encryptor has wrong input size",
										  alg, input),
								 size, encryptor.InputBlockSize);
					AssertEquals(GetError("encryptor has wrong output size",
										  alg, input),
								 size, encryptor.OutputBlockSize);
				}
				else
				{
					AssertEquals(GetError("encryptor has wrong input size",
										  alg, input),
								 1, encryptor.InputBlockSize);
					AssertEquals(GetError("encryptor has wrong output size",
										  alg, input),
								 1, encryptor.OutputBlockSize);
				}
				byte[] rawOutput = new byte [rawInput.Length + 256];
				int len = encryptor.TransformBlock
					(rawInput, 0, cutoff, rawOutput, 0);
				byte[] rawTail = encryptor.TransformFinalBlock
					(rawInput, cutoff, rawInput.Length - cutoff);
				Array.Copy(rawTail, 0, rawOutput, len, rawTail.Length);
				len += rawTail.Length;
				((IDisposable)encryptor).Dispose();

				// Reverse the ciphertext back to the original.
				cutoff = len - len % size;
				ICryptoTransform decryptor;
				decryptor = alg.CreateDecryptor(key, iv);
				Assert(GetError("decryptor cannot transform multiple blocks",
								alg, input),
					   decryptor.CanTransformMultipleBlocks);
				if(mode == CipherMode.ECB || mode == CipherMode.CBC)
				{
					AssertEquals(GetError("decryptor has wrong input size",
										  alg, input),
								 size, decryptor.InputBlockSize);
					AssertEquals(GetError("decryptor has wrong output size",
										  alg, input),
								 size, decryptor.OutputBlockSize);
				}
				else
				{
					AssertEquals(GetError("decryptor has wrong input size",
										  alg, input),
								 1, decryptor.InputBlockSize);
					AssertEquals(GetError("decryptor has wrong output size",
										  alg, input),
								 1, decryptor.OutputBlockSize);
				}
				byte[] rawReverse = new byte [rawInput.Length + 256];
				int rlen = decryptor.TransformBlock
					(rawOutput, 0, cutoff, rawReverse, 0);
				rawTail = decryptor.TransformFinalBlock
					(rawOutput, cutoff, len - cutoff);
				Array.Copy(rawTail, 0, rawReverse, rlen, rawTail.Length);
				rlen += rawTail.Length;
				((IDisposable)decryptor).Dispose();

				// Compare the reversed plaintext with the original.
				if(padding != PaddingMode.None)
				{
					AssertEquals(GetError
							("reversed plaintext has incorrect length",
							 alg, input), rawInput.Length, rlen);
					if(!IdenticalBlock(rawInput, 0, rawReverse, 0, rlen))
					{
						Fail(GetError
							("reversed plaintext is not the same as original",
							 alg, input));
					}
				}
				else
				{
					if(rawInput.Length > rlen)
					{
						Fail(GetError
							("reversed plaintext has incorrect length",
							 alg, input));
					}
					if(!IdenticalBlock(rawInput, 0, rawReverse, 0,
									   rawInput.Length))
					{
						Fail(GetError
							("reversed plaintext is not the same as original",
							 alg, input));
					}
				}

				// Encrypt the padded plaintext using a primitive
				// algorithm simulation to verify the expected output.
				byte[] paddedOutput;
				switch(mode)
				{
					case CipherMode.ECB:
					{
						paddedOutput = DoECB(paddedInput, alg, key);
					}
					break;

					case CipherMode.CBC:
					{
						paddedOutput = DoCBC(paddedInput, alg, key, iv);
					}
					break;

					case CipherMode.OFB:
					{
						paddedOutput = DoOFB(paddedInput, alg, key, iv);
					}
					break;

					case CipherMode.CFB:
					{
						paddedOutput = DoCFB(paddedInput, alg, key, iv);
					}
					break;

					case CipherMode.CTS:
					default:
					{
						paddedOutput = DoCTS(paddedInput, alg, key, iv);
					}
					break;
				}

				// Compare the actual output with the expected output.
				AssertEquals(GetError("ciphertext has incorrect length",
									  alg, input),
							 paddedOutput.Length, len);
				if(!IdenticalBlock(paddedOutput, 0, rawOutput, 0, len))
				{
					Fail(GetError("ciphertext was not the expected value",
								  alg, input));
				}
			}

	// Run a mode test using a number of different inputs and padding modes.
	protected void RunModeTest(SymmetricAlgorithm alg, CipherMode mode,
							   PaddingMode padding)
			{
				RunModeTest(alg, mode, padding, "");
				RunModeTest(alg, mode, padding, "abc");
				RunModeTest(alg, mode, padding, "abcdefgh");
				RunModeTest(alg, mode, padding, "abcdefghijk");
				RunModeTest(alg, mode, padding, "abcdefghijklmno");
				RunModeTest(alg, mode, padding,
							"The time has come the walrus said.");
			}
	protected void RunModeTest(SymmetricAlgorithm alg, CipherMode mode)
			{
				RunModeTest(alg, mode, PaddingMode.None);
				RunModeTest(alg, mode, PaddingMode.PKCS7);
				RunModeTest(alg, mode, PaddingMode.Zeros);
			}

}; // CryptoTestCase

#endif // CONFIG_CRYPTO
