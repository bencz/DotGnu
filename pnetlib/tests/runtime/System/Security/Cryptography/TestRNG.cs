/*
 * TestRNG.cs - Test the random number generator.
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
using System.Security.Cryptography;

#if CONFIG_CRYPTO

public class TestRNG : TestCase
{

	// Internal state.
	private static byte[] randomBits;

	// Constructor.
	public TestRNG(String name)
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
	
	// Test that the random number generator actually works by seeing
	// if it generates non-zero byte streams.  There is a small non-zero
	// probability that a working random number generator will in fact
	// generate an all-zero stream, but since that is about 1^-128, the
	// chances of it happening by accident are quite rare.
	public void TestRNGWorks()
			{
				if(!CryptoTestCase.RandomWorks())
				{
					Fail("random number generator does not work");
				}
			}

	// Check that "GetNonZeroBytes" does indeed do that.  We generate
	// a large amount of random material to increase the chance that
	// the underlying generator will generate a zero which needs to
	// be corrected by "GetNonZeroBytes".  If the random number
	// generator doesn't work at all, we avoid running this test so
	// that we don't get an infinite loop inside the library.
	public void TestRNGNonZeroWorks()
			{
				if(CryptoTestCase.RandomWorks())
				{
					int round;
					int index;
					byte[] rand = new byte [16384];
					RandomNumberGenerator rng = RandomNumberGenerator.Create();
					for(round = 0; round < 32; ++round)
					{
						rng.GetNonZeroBytes(rand);
						for(index = 0; index < 16; ++index)
						{
							if(rand[index] == 0x00)
							{
								Fail("zero byte found in generated stream");
							}
						}
					}
				}
			}

	// The following tests are based on the statistical testing requirements
	// for cryptographic random number generators from NIST:
	//
	// NIST FIPS PUB 140-2: Security requirements for Cryptographic Modules
	// http://csrc.nist.gov/publications/fips/fips140-2/fips1402.pdf
	//
	// There are lots of other statistical tests in the following NIST
	// standard, which we haven't implement yet due to their complexity:
	//
	// NIST SP 800-22: A Statistical Test Suite for Random and Pseudorandom
	// Number Generators for Cryptographic Applications
	// http://csrc.nist.gov/publications/nistpubs/800-22/sp-800-22-051501.pdf

	// Perform the NIST "monobit" test.
	public void TestRNGBit()
			{
				// Generate 20,000 bits of random material, which we
				// will also be using for the later NIST tests.
				randomBits = new byte [20000 / 8];
				RandomNumberGenerator rng = RandomNumberGenerator.Create();
				rng.GetBytes(randomBits);

				// Count the number of 1 bits in the vector.
				int count = 0;
				int bit;
				for(bit = 0; bit < 20000; ++bit)
				{
					if((randomBits[bit / 8] & (1 << (bit & 7))) != 0)
					{
						++count;
					}
				}

				// Determine if there is a sufficient number of 0's and 1's.
				if(count <= 9725)
				{
					Fail("insufficient number of 1's in the stream");
				}
				else if(count >= 10275)
				{
					Fail("insufficient number of 0's in the stream");
				}
			}

	// Perform the NIST "poker" test.
	public void TestRNGPoker()
			{
				// Count the occurrences of each 4-bit value.
				int[] counts = new int [16];
				int posn;
				for(posn = 0; posn < (20000 / 8); ++posn)
				{
					++(counts[randomBits[posn] & 0x0F]);
					++(counts[(randomBits[posn] >> 4) & 0x0F]);
				}

				// Compute the co-efficient and check it.
				double sum = 0.0;
				for(posn = 0; posn < 16; ++posn)
				{
					sum += ((double)(counts[posn])) *
						   ((double)(counts[posn]));
				}
				double coeff = (16.0 / 5000.0) * sum - 5000.0;
				if(coeff <= 2.16 || coeff >= 46.17)
				{
					Fail("incorrect poker value");
				}
			}

	// Perform the NIST "runs" and "longruns" tests in one pass.
	public void TestRNGRuns()
			{
				// Count the length of the runs within the stream.
				int[] counts0 = new int [6];
				int[] counts1 = new int [6];
				int bit;
				int prev = -1;
				int value = 0;
				int count;
				count = 0;
				for(bit = 0; bit < 20000; ++bit)
				{
					if((randomBits[bit / 8] & (1 << (bit & 7))) != 0)
					{
						value = 1;
					}
					else
					{
						value = 0;
					}
					if(prev != value)
					{
						if(prev != -1)
						{
							if(count < 6)
							{
								if(value != 0)
								{
									++(counts1[count - 1]);
								}
								else
								{
									++(counts0[count - 1]);
								}
							}
							else
							{
								if(value != 0)
								{
									++(counts1[5]);
								}
								else
								{
									++(counts0[5]);
								}
								if(count >= 26)
								{
									Fail("long run detected");
								}
							}
							count = 1;
							prev = value;
						}
						else
						{
							prev = value;
							++count;
						}
					}
					else
					{
						++count;
					}
				}
				if(count < 6)
				{
					if(value != 0)
					{
						++(counts1[count - 1]);
					}
					else
					{
						++(counts0[count - 1]);
					}
				}
				else
				{
					if(value != 0)
					{
						++(counts1[5]);
					}
					else
					{
						++(counts0[5]);
					}
					if(count >= 26)
					{
						Fail("long run detected");
					}
				}

				// Determine if the run lengths are within
				// the required ranges.
				if(counts0[0] < 2315 || counts0[0] > 2685)
				{
					Fail("too many or too few 0 runs of length 1");
				}
				if(counts1[0] < 2315 || counts1[0] > 2685)
				{
					Fail("too many or too few 1 runs of length 1");
				}
				if(counts0[1] < 1114 || counts0[1] > 1386)
				{
					Fail("too many or too few 0 runs of length 2");
				}
				if(counts1[1] < 1114 || counts1[1] > 1386)
				{
					Fail("too many or too few 1 runs of length 2");
				}
				if(counts0[2] < 527 || counts0[2] > 723)
				{
					Fail("too many or too few 0 runs of length 3");
				}
				if(counts1[2] < 527 || counts1[2] > 723)
				{
					Fail("too many or too few 1 runs of length 3");
				}
				if(counts0[3] < 240 || counts0[3] > 384)
				{
					Fail("too many or too few 0 runs of length 4");
				}
				if(counts1[3] < 240 || counts1[3] > 384)
				{
					Fail("too many or too few 1 runs of length 4");
				}
				if(counts0[4] < 103 || counts0[4] > 209)
				{
					Fail("too many or too few 0 runs of length 5");
				}
				if(counts1[4] < 103 || counts1[4] > 209)
				{
					Fail("too many or too few 1 runs of length 5");
				}
				if(counts0[5] < 103 || counts0[5] > 209)
				{
					Fail("too many or too few 0 runs of length 6+");
				}
				if(counts1[5] < 103 || counts1[5] > 209)
				{
					Fail("too many or too few 1 runs of length 6+");
				}
			}

}; // TestRNG

#endif // CONFIG_CRYPTO
