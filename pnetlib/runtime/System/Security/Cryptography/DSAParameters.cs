/*
 * DSAParameters.cs - Implementation of the
 *		"System.Security.Cryptography.DSAParameters" structure.
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

public struct DSAParameters
{
	public int Counter;
	public byte[] G;
	public byte[] J;
	public byte[] P;
	public byte[] Q;
	public byte[] Seed;
	[NonSerialized] public byte[] X;
	public byte[] Y;

	// Clear the contents of this structure.
	internal void Clear()
			{
				Counter = 0;
				if(G != null)
				{
					Array.Clear(G, 0, G.Length);
					G = null;
				}
				if(J != null)
				{
					Array.Clear(J, 0, J.Length);
					J = null;
				}
				if(P != null)
				{
					Array.Clear(P, 0, P.Length);
					P = null;
				}
				if(Q != null)
				{
					Array.Clear(Q, 0, Q.Length);
					Q = null;
				}
				if(Seed != null)
				{
					Array.Clear(Seed, 0, Seed.Length);
					Seed = null;
				}
				if(X != null)
				{
					Array.Clear(X, 0, X.Length);
					X = null;
				}
				if(Y != null)
				{
					Array.Clear(Y, 0, Y.Length);
					Y = null;
				}
			}

	// Clone the public parameters in this structure.
	internal DSAParameters ClonePublic()
			{
				DSAParameters p;
				p.Counter = Counter;
				p.G = G;
				p.J = J;
				p.P = P;
				p.Q = Q;
				p.Seed = Seed;
				p.X = null;
				p.Y = Y;
				return p;
			}

	// Object identifier for DSA: "1.2.840.10040.4.1".
	private static readonly byte[] dsaID =
			{(byte)0x2A, (byte)0x86, (byte)0x48, (byte)0xCE,
			 (byte)0x38, (byte)0x04, (byte)0x01};

	// Convert an ASN.1 buffer into DSA public parameters.
	internal void ASN1ToPublic(ASN1Parser parser)
			{
				parser = parser.GetSequence();
				if(parser.Type == ASN1Type.Sequence)
				{
					// This looks like it may be a "SubjectPublicKeyInfo"
					// from an X.509 certificate.  Validate the algorithm ID.
					ASN1Parser alg = parser.GetSequence();
					byte[] objid = alg.GetObjectIdentifier();
					if(!ASN1Parser.IsObjectID(objid, dsaID))
					{
						throw new CryptographicException
							(_("Crypto_InvalidASN1"));
					}

					// Get the common P, Q, and G parameters.
					ASN1Parser algParams = alg.GetSequence();
					P = algParams.GetBigInt();
					Q = algParams.GetBigInt();
					G = algParams.GetBigInt();
					algParams.AtEnd();
					alg.AtEnd();

					// Get the public key information (Y).
					ASN1Parser bitString = parser.GetBitStringContents();
					Y = bitString.GetBigInt();
					bitString.AtEnd();
					parser.AtEnd();
				}
				else
				{
					// This looks like a bare list of DSA parameters.
					P = parser.GetBigInt();
					Q = parser.GetBigInt();
					G = parser.GetBigInt();
					Y = parser.GetBigInt();
					if(!parser.IsAtEnd())
					{
						// It looks like we have private DSA parameters also.
						J = parser.GetBigInt();
						X = parser.GetBigInt();
						Seed = parser.GetBigInt();
						Counter = parser.GetInt32();
					}
					parser.AtEnd();
				}
			}
	internal void ASN1ToPublic(byte[] buffer, int offset, int count)
			{
				ASN1ToPublic(new ASN1Parser(buffer, offset, count));
			}

	// Convert an ASN.1 buffer into DSA private parameters.
	internal void ASN1ToPrivate(ASN1Parser parser)
			{
				parser = parser.GetSequence();
				P = parser.GetBigInt();
				Q = parser.GetBigInt();
				G = parser.GetBigInt();
				Y = parser.GetBigInt();
				J = parser.GetBigInt();
				X = parser.GetBigInt();
				Seed = parser.GetBigInt();
				Counter = parser.GetInt32();
				parser.AtEnd();
			}
	internal void ASN1ToPrivate(byte[] buffer, int offset, int count)
			{
				ASN1ToPrivate(new ASN1Parser(buffer, offset, count));
			}

	// Convert DSA public parameters into an ASN.1 buffer.
	internal void PublicToASN1(ASN1Builder builder, bool x509)
			{
				if(x509)
				{
					// Output an X.509 "SubjectPublicKeyInfo" block.
					ASN1Builder alg = builder.AddSequence();
					alg.AddObjectIdentifier(dsaID);
					ASN1Builder inner = alg.AddSequence();
					inner.AddBigInt(P);
					inner.AddBigInt(Q);
					inner.AddBigInt(G);
					ASN1Builder bitString = builder.AddBitStringContents();
					bitString.AddBigInt(Y);
				}
				else
				{
					// Output the raw public parameters.
					builder.AddBigInt(P);
					builder.AddBigInt(Q);
					builder.AddBigInt(G);
					builder.AddBigInt(Y);
				}
			}
	internal byte[] PublicToASN1(bool x509)
			{
				ASN1Builder builder = new ASN1Builder();
				PublicToASN1(builder, x509);
				return builder.ToByteArray();
			}

	// Convert DSA private parameters into an ASN.1 buffer.
	internal void PrivateToASN1(ASN1Builder builder)
			{
				builder.AddBigInt(P);
				builder.AddBigInt(Q);
				builder.AddBigInt(G);
				builder.AddBigInt(Y);
				builder.AddBigInt(J);
				builder.AddBigInt(X);
				builder.AddBigInt(Seed);
				builder.AddInt32(Counter);
			}
	internal byte[] PrivateToASN1()
			{
				ASN1Builder builder = new ASN1Builder();
				PrivateToASN1(builder);
				return builder.ToByteArray();
			}

}; // struct DSAParameters

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
