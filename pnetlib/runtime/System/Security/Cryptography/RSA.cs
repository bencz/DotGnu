/*
 * RSA.cs - Implementation of the
 *		"System.Security.Cryptography.RSA" class.
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
using System.Security;

public abstract class RSA : AsymmetricAlgorithm
{
	// Constructor.
	public RSA() {}

	// Create an instance of the default RSA implementation.
	public new static RSA Create()
			{
				return (RSA)(CryptoConfig.CreateFromName
						(CryptoConfig.RSADefault, null));
			}

	// Create an instance of a specific DSA implementation, by name.
	public new static RSA Create(String algName)
			{
				return (RSA)(CryptoConfig.CreateFromName(algName, null));
			}

	// Decrypt a value using the RSA private key.
	public abstract byte[] DecryptValue(byte[] rgb);

	// Encrypt a value using the RSA public key.
	public abstract byte[] EncryptValue(byte[] rgb);

	// Export the parameters for RSA signature generation.
	public abstract RSAParameters ExportParameters
				(bool includePrivateParameters);

	// Import the parameters for RSA signature generation.
	public abstract void ImportParameters(RSAParameters parameters);

	// Reconstruct an asymmetric algorithm object from an XML string.
	public override void FromXmlString(String xmlString)
			{
				SecurityElement elem;
				RSAParameters rsaParams = new RSAParameters();
				String tag;
				if(xmlString == null)
				{
					throw new ArgumentNullException("xmlString");
				}
				try
				{
					elem = SecurityElement.Parse(xmlString);
					if(elem == null || elem.Tag != "RSAKeyValue")
					{
						throw new CryptographicException
							(_("Crypto_InvalidRSAParams"));
					}
					foreach(SecurityElement child in elem.Children)
					{
						tag = child.Tag;
						if(tag == "Modulus")
						{
							rsaParams.Modulus = Convert.FromBase64String
								(child.Text);
						}
						else if(tag == "Exponent")
						{
							rsaParams.Exponent = Convert.FromBase64String
								(child.Text);
						}
						else if(tag == "D")
						{
							rsaParams.D = Convert.FromBase64String
								(child.Text);
						}
						else if(tag == "DP")
						{
							rsaParams.DP = Convert.FromBase64String
								(child.Text);
						}
						else if(tag == "DQ")
						{
							rsaParams.DQ = Convert.FromBase64String
								(child.Text);
						}
						else if(tag == "InverseQ")
						{
							rsaParams.InverseQ = Convert.FromBase64String
								(child.Text);
						}
						else if(tag == "P")
						{
							rsaParams.P = Convert.FromBase64String
								(child.Text);
						}
						else if(tag == "Q")
						{
							rsaParams.Q = Convert.FromBase64String
								(child.Text);
						}
					}
				}
				catch(FormatException)
				{
					throw new CryptographicException
						(_("Crypto_InvalidRSAParams"));
				}
				catch(ArgumentNullException)
				{
					throw new CryptographicException
						(_("Crypto_InvalidRSAParams"));
				}
				ImportParameters(rsaParams);
			}

	// Get the XML string representation of an asymmetric algorithm object.
	public override String ToXmlString(bool includePrivateParameters)
			{
				RSAParameters rsaParams =
					ExportParameters(includePrivateParameters);
				StringBuilder builder = new StringBuilder();
				builder.Append("<RSAKeyValue>");
				BigIntToXml(builder, "Modulus", rsaParams.Modulus);
				BigIntToXml(builder, "Exponent", rsaParams.Exponent);
				if(includePrivateParameters)
				{
					BigIntToXml(builder, "D", rsaParams.D);
					BigIntToXml(builder, "DP", rsaParams.DP);
					BigIntToXml(builder, "DQ", rsaParams.DQ);
					BigIntToXml(builder, "DP", rsaParams.DP);
					BigIntToXml(builder, "InverseQ", rsaParams.InverseQ);
					BigIntToXml(builder, "P", rsaParams.P);
					BigIntToXml(builder, "Q", rsaParams.Q);
				}
				builder.Append("</RSAKeyValue>");
				return builder.ToString();
			}

}; // class RSA

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
