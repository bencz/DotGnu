/*
 * RSAParameters.cs - Implementation of the
 *		"System.Security.Cryptography.RSAParameters" structure.
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

public struct RSAParameters
{
	public byte[] Exponent;
	public byte[] Modulus;
	[NonSerialized] public byte[] D;
	[NonSerialized] public byte[] DP;
	[NonSerialized] public byte[] DQ;
	[NonSerialized] public byte[] InverseQ;
	[NonSerialized] public byte[] P;
	[NonSerialized] public byte[] Q;

	// Clear the contents of this structure.
	internal void Clear()
			{
				if(Exponent != null)
				{
					Array.Clear(Exponent, 0, Exponent.Length);
					Exponent = null;
				}
				if(Modulus != null)
				{
					Array.Clear(Modulus, 0, Modulus.Length);
					Modulus = null;
				}
				if(D != null)
				{
					Array.Clear(D, 0, D.Length);
					D = null;
				}
				if(DP != null)
				{
					Array.Clear(DP, 0, DP.Length);
					DP = null;
				}
				if(DQ != null)
				{
					Array.Clear(DQ, 0, DQ.Length);
					DQ = null;
				}
				if(InverseQ != null)
				{
					Array.Clear(InverseQ, 0, InverseQ.Length);
					InverseQ = null;
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
			}

	// Clone the public parameters in this structure.
	internal RSAParameters ClonePublic()
			{
				RSAParameters p;
				p.Exponent = Exponent;
				p.Modulus = Modulus;
				p.D = null;
				p.DP = null;
				p.DQ = null;
				p.InverseQ = null;
				p.P = null;
				p.Q = null;
				return p;
			}

	// Object identifier for RSA: "1.2.840.113549.1.1.1".
	private static readonly byte[] rsaID =
			{(byte)0x2A, (byte)0x86, (byte)0x48, (byte)0x86, (byte)0xF7,
			 (byte)0x0D, (byte)0x01, (byte)0x01, (byte)0x01};

	// Convert an ASN.1 buffer into RSA public parameters.
	internal void ASN1ToPublic(ASN1Parser parser)
			{
				parser = parser.GetSequence();
				if(parser.Type == ASN1Type.Sequence)
				{
					// This looks like it may be a "SubjectPublicKeyInfo"
					// from an X.509 certificate.  Validate the algorithm ID.
					ASN1Parser alg = parser.GetSequence();
					byte[] objid = alg.GetObjectIdentifier();
					if(!ASN1Parser.IsObjectID(objid, rsaID))
					{
						throw new CryptographicException
							(_("Crypto_InvalidASN1"));
					}
					alg.GetNull();
					alg.AtEnd();

					// Get the public key information.
					ASN1Parser bitString = parser.GetBitStringContents();
					ASN1Parser inner = bitString.GetSequence();
					Modulus = inner.GetBigInt();
					Exponent = inner.GetBigInt();
					inner.AtEnd();
					bitString.AtEnd();
					parser.AtEnd();
				}
				else if(parser.Type == ASN1Type.Integer &&
				        parser.Length == 1)
				{
					// This looks like a list of private RSA parameters.
					ASN1ToPrivate(parser);
				}
				else
				{
					// This looks like a bare list of RSA parameters.
					Modulus = parser.GetBigInt();
					Exponent = parser.GetBigInt();
					parser.AtEnd();
				}
			}
	internal void ASN1ToPublic(byte[] buffer, int offset, int count)
			{
				ASN1ToPublic(new ASN1Parser(buffer, offset, count));
			}

	// Convert an ASN.1 buffer into RSA private parameters.
	internal void ASN1ToPrivate(ASN1Parser parser)
			{
				parser = parser.GetSequence();
				if(parser.GetInt32() != 0)
				{
					// Incorrect version for RSA private key parameters.
					throw new CryptographicException
						(_("Crypto_InvalidASN1"));
				}
				Modulus = parser.GetBigInt();
				Exponent = parser.GetBigInt();
				D = parser.GetBigInt();
				P = parser.GetBigInt();
				Q = parser.GetBigInt();
				DP = parser.GetBigInt();
				DQ = parser.GetBigInt();
				InverseQ = parser.GetBigInt();
				parser.AtEnd();
			}
	internal void ASN1ToPrivate(byte[] buffer, int offset, int count)
			{
				ASN1ToPrivate(new ASN1Parser(buffer, offset, count));
			}

	// Convert RSA public parameters into an ASN.1 buffer.
	internal void PublicToASN1(ASN1Builder builder, bool x509)
			{
				if(x509)
				{
					// Output an X.509 "SubjectPublicKeyInfo" block.
					ASN1Builder alg = builder.AddSequence();
					alg.AddObjectIdentifier(rsaID);
					alg.AddNull();
					ASN1Builder bitString = builder.AddBitStringContents();
					ASN1Builder inner = bitString.AddSequence();
					inner.AddBigInt(Modulus);
					inner.AddBigInt(Exponent);
				}
				else
				{
					// Output a bare list of RSA parameters.
					builder.AddBigInt(Modulus);
					builder.AddBigInt(Exponent);
				}
			}
	internal byte[] PublicToASN1(bool x509)
			{
				ASN1Builder builder = new ASN1Builder();
				PublicToASN1(builder, x509);
				return builder.ToByteArray();
			}

	// Convert RSA private parameters into an ASN.1 buffer.
	internal void PrivateToASN1(ASN1Builder builder)
			{
				builder.AddInt32(0);
				builder.AddBigInt(Modulus);
				builder.AddBigInt(Exponent);
				builder.AddBigInt(D);
				builder.AddBigInt(P);
				builder.AddBigInt(Q);
				builder.AddBigInt(DP);
				builder.AddBigInt(DQ);
				builder.AddBigInt(InverseQ);
			}
	internal byte[] PrivateToASN1()
			{
				ASN1Builder builder = new ASN1Builder();
				PrivateToASN1(builder);
				return builder.ToByteArray();
			}

}; // struct RSAParameters

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
