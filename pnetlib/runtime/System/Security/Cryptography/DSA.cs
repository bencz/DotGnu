/*
 * DSA.cs - Implementation of the
 *		"System.Security.Cryptography.DSA" class.
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
using System.Text;

public abstract class DSA : AsymmetricAlgorithm
{
	// Constructor.
	internal DSA() {}

	// Create an instance of the default DSA implementation.
	public new static DSA Create()
			{
				return (DSA)(CryptoConfig.CreateFromName
						(CryptoConfig.DSADefault, null));
			}

	// Create an instance of a specific DSA implementation, by name.
	public new static DSA Create(String algName)
			{
				return (DSA)(CryptoConfig.CreateFromName(algName, null));
			}

	// Create a DSA signature for the specified data.
	public abstract byte[] CreateSignature(byte[] rgbHash);

	// Export the parameters for DSA signature generation.
	public abstract DSAParameters ExportParameters
				(bool includePrivateParameters);

	// Import the parameters for DSA signature generation.
	public abstract void ImportParameters(DSAParameters parameters);

	// Verify a DSA signature.
	public abstract bool VerifySignature(byte[] rgbHash, byte[] rgbSignature);

	// Reconstruct an asymmetric algorithm object from an XML string.
	public override void FromXmlString(String xmlString)
			{
				SecurityElement elem;
				DSAParameters dsaParams = new DSAParameters();
				String tag;
				if(xmlString == null)
				{
					throw new ArgumentNullException("xmlString");
				}
				try
				{
					elem = SecurityElement.Parse(xmlString);
					if(elem == null || elem.Tag != "DSAKeyValue")
					{
						throw new CryptographicException
							(_("Crypto_InvalidDSAParams"));
					}
					foreach(SecurityElement child in elem.Children)
					{
						tag = child.Tag;
						if(tag == "P")
						{
							dsaParams.P = Convert.FromBase64String
								(child.Text);
						}
						else if(tag == "Q")
						{
							dsaParams.Q = Convert.FromBase64String
								(child.Text);
						}
						else if(tag == "G")
						{
							dsaParams.G = Convert.FromBase64String
								(child.Text);
						}
						else if(tag == "Y")
						{
							dsaParams.Y = Convert.FromBase64String
								(child.Text);
						}
						else if(tag == "J")
						{
							dsaParams.J = Convert.FromBase64String
								(child.Text);
						}
						else if(tag == "Seed")
						{
							dsaParams.Seed = Convert.FromBase64String
								(child.Text);
						}
						else if(tag == "X")
						{
							dsaParams.X = Convert.FromBase64String
								(child.Text);
						}
						else if(tag == "PgenCounter")
						{
							byte[] count = Convert.FromBase64String
								(child.Text);
							dsaParams.Counter = 0;
							foreach(byte b in count)
							{
								dsaParams.Counter =
									(dsaParams.Counter << 8) + (int)b;
							}
						}
					}
				}
				catch(FormatException)
				{
					throw new CryptographicException
						(_("Crypto_InvalidDSAParams"));
				}
				catch(ArgumentNullException)
				{
					throw new CryptographicException
						(_("Crypto_InvalidDSAParams"));
				}
				ImportParameters(dsaParams);
			}

	// Get the XML string representation of an asymmetric algorithm object.
	public override String ToXmlString(bool includePrivateParameters)
			{
				DSAParameters dsaParams =
					ExportParameters(includePrivateParameters);
				byte[] countArray;
				StringBuilder builder = new StringBuilder();
				builder.Append("<DSAKeyValue>");
				BigIntToXml(builder, "P", dsaParams.P);
				BigIntToXml(builder, "Q", dsaParams.Q);
				BigIntToXml(builder, "G", dsaParams.G);
				BigIntToXml(builder, "Y", dsaParams.Y);
				BigIntToXml(builder, "J", dsaParams.J);
				BigIntToXml(builder, "Seed", dsaParams.Seed);
				if(dsaParams.Counter < 0x100)
				{
					countArray = new byte [1];
					countArray[0] = (byte)(dsaParams.Counter);
				}
				else if(dsaParams.Counter < 0x10000)
				{
					countArray = new byte [2];
					countArray[0] = (byte)(dsaParams.Counter >> 8);
					countArray[1] = (byte)(dsaParams.Counter);
				}
				else if(dsaParams.Counter < 0x1000000)
				{
					countArray = new byte [3];
					countArray[0] = (byte)(dsaParams.Counter >> 16);
					countArray[1] = (byte)(dsaParams.Counter >> 8);
					countArray[2] = (byte)(dsaParams.Counter);
				}
				else
				{
					countArray = new byte [4];
					countArray[0] = (byte)(dsaParams.Counter >> 24);
					countArray[1] = (byte)(dsaParams.Counter >> 16);
					countArray[2] = (byte)(dsaParams.Counter >> 8);
					countArray[3] = (byte)(dsaParams.Counter);
				}
				BigIntToXml(builder, "PgenCounter", dsaParams.X);
				if(includePrivateParameters)
				{
					BigIntToXml(builder, "X", dsaParams.X);
				}
				builder.Append("</DSAKeyValue>");
				return builder.ToString();
			}

}; // class DSA

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
