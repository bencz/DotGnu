/*
 * X509Certificate.cs - Implementation of the
 *		"System.Security.Cryptography.X509Certificates.X509Certificate" class.
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

namespace System.Security.Cryptography.X509Certificates
{

#if CONFIG_X509_CERTIFICATES

using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
#if CONFIG_SERIALIZATION
using System.Runtime.Serialization;
#endif

[Serializable]
#if CONFIG_SERIALIZATION
public class X509Certificate: IDeserializationCallback, ISerializable
#else
public class X509Certificate
#endif
{
	// Internal state.
	private byte[] rawData;
	private byte[] hash;
	private String effectiveDate;
	private String expirationDate;
	private String issuer;
	private String keyAlgorithm;
	private byte[] keyAlgorithmParameters;
	private String name;
	private byte[] publicKey;
	private byte[] serialNumber;

	// Constructors.
	public X509Certificate(byte[] data)
			{
				if(data == null)
				{
					throw new ArgumentNullException("data");
				}
				Parse(data);
			}
#if !CONFIG_COMPACT_FRAMEWORK
	public X509Certificate(IntPtr handle)
			{
				// Handle-based certificate construction is not supported.
				throw new NotSupportedException
					(_("Crypto_CertNotSupp"));
			}
	public X509Certificate(X509Certificate cert)
			{
				if(cert == null)
				{
					throw new ArgumentNullException("cert");
				}
				Parse(cert.rawData);
			}
#if CONFIG_FRAMEWORK_2_0
	[TODO]
	public X509Certificate()
			{
				throw new NotImplementedException("X509Certificate()");
			}

	[TODO]
	public X509Certificate(byte[] rawData, String password)
			{
				throw new NotImplementedException("X509Certificate(byte[], String)");
			}
	[TODO]
	public X509Certificate(byte[] rawData, String password, X509KeyStorageFlags keyStorageFlags)
			{
				throw new NotImplementedException("X509Certificate(byte[], String, X509KeyStorageFlags)");
			}
#if CONFIG_SERIALIZATION
	[TODO]
	public X509Certificate(SerializationInfo info, StreamingContext context)
			{
				throw new NotImplementedException("X509Certificate(SerializationInfo, StreamingContext)");
			}
#endif // CONFIG_SERIALIZATION
	[TODO]
	public X509Certificate(String fileName)
			{
				throw new NotImplementedException("X509Certificate(String)");
			}
	[TODO]
	public X509Certificate(String fileName, String password)
			{
				throw new NotImplementedException("X509Certificate(String, String)");
			}
	[TODO]
	public X509Certificate(String fileName, String password, X509KeyStorageFlags keyStorageFlags)
			{
				throw new NotImplementedException("X509Certificate(String, String, X509KeyStorageFlags)");
			}
#endif // CONFIG_FRAMEWORK_2_0
#endif // !CONFIG_COMPACT_FRAMEWORK
	// Parse the contents of a certificate data block.
	private void Parse(byte[] data)
			{
				// Clone the data for internal storage.
				rawData = (byte[])(data.Clone());

				// Parse the ASN.1 data to get the field we are interested in.
				ASN1Parser parser = new ASN1Parser(rawData);
				ASN1Parser signed = parser.GetSequence();
				ASN1Parser certInfo = signed.GetSequence();
				if(certInfo.Type == ASN1Parser.ContextSpecific(0))
				{
					// Skip the version field.
					certInfo.Skip();
				}
				serialNumber = certInfo.GetContentsAsArray(ASN1Type.Integer);
				ASN1Parser algId = certInfo.GetSequence();
				issuer = ParseName(certInfo);
				ASN1Parser validity = certInfo.GetSequence();
				effectiveDate = validity.GetUTCTime();
				expirationDate = validity.GetUTCTime();
				name = ParseName(certInfo);
				ASN1Parser keyInfo = certInfo.GetSequence();
				algId = keyInfo.GetSequence();
				keyAlgorithm = ToHex(algId.GetObjectIdentifier());
				if(algId.IsAtEnd() || algId.IsNull())
				{
					keyAlgorithmParameters = null;
				}
				else
				{
					keyAlgorithmParameters = algId.GetWholeAsArray();
				}
				publicKey = keyInfo.GetBitString();

#if CONFIG_CRYPTO
				// Construct an MD5 hash of the certificate.  Is this correct?
				MD5 md5 = new MD5CryptoServiceProvider();
				md5.InternalHashCore(rawData, 0, rawData.Length);
				hash = md5.InternalHashFinal();
				md5.Initialize();
#endif
			}

	// Parse an X.509-format name and convert it into a string.
	private static String ParseName(ASN1Parser certInfo)
			{
				StringBuilder builder = new StringBuilder();
				ASN1Parser outer;
				ASN1Parser set;
				ASN1Parser pair;

				// Process the outer sequence.
				outer = certInfo.GetSequence();
				while(!outer.IsAtEnd())
				{
					// Process the next name attribute set.
					set = outer.GetSet();
					while(!set.IsAtEnd())
					{
						// Process the next attribute name/value pair.
						pair = set.GetSequence();
						pair.Skip(ASN1Type.ObjectIdentifier);
						if(pair.IsString())
						{
							// Add the value to the string we are building.
							if(builder.Length > 0)
							{
								builder.Append(", ");
							}
							builder.Append(pair.GetString());
						}
					}
				}

				// Convert the result into a name.
				return builder.ToString();
			}

	// Create a certificate from the contents of a certification file.
	public static X509Certificate CreateFromCertFile(String filename)
			{
				// Read the entire file into memory and create
				// a certificate from it.
				FileStream stream = new FileStream(filename, FileMode.Open);
				byte[] data = new byte [(int)(stream.Length)];
				stream.Read(data, 0, data.Length);
				stream.Close();
				return new X509Certificate(data);
			}

	// Create a certificate from the contents of a signed certificate file.
	public static X509Certificate CreateFromSignedFile(String filename)
			{
				// Not yet supported - we don't know what the "signed"
				// file format is supposed to be.
				throw new NotSupportedException(_("Crypto_CertNotSupp"));
			}

	// Compare two certificate objects for equality.  Certificates are
	// considered equal if the issuer and serial numbers are the same.
	public virtual bool Equals(X509Certificate other)
			{
				if(other == null)
				{
					return false;
				}
				if(issuer != other.issuer ||
				   serialNumber.Length != other.serialNumber.Length)
				{
					return false;
				}
				for(int index = 0; index < serialNumber.Length; ++index)
				{
					if(serialNumber[index] != other.serialNumber[index])
					{
						return false;
					}
				}
				return true;
			}

#if CONFIG_FRAMEWORK_2_0
	[TODO]
	public virtual byte[] Export(X509ContentType contentType)
			{
				throw new NotImplementedException("Export");
			}
	[TODO]
	public virtual byte[] Export(X509ContentType contentType, String password)
			{
				throw new NotImplementedException("Export");
			}
#endif
	// Convert a byte array into a hexadecimal string.
	private static String ToHex(byte[] array)
			{
				if(array == null)
				{
					return null;
				}
				StringBuilder builder = new StringBuilder();
				String hexChars = "01234567abcdef";
				for(int index = 0; index < array.Length; ++index)
				{
					builder.Append(hexChars[array[index] >> 4]);
					builder.Append(hexChars[array[index] & 0x0F]);
				}
				return builder.ToString();
			}

	// Get the hash of the certificate.
	public virtual byte[] GetCertHash()
			{
				return hash;
			}

	// Get the hash of the certificate as a hexadecimal string.
	public virtual String GetCertHashString()
			{
				return ToHex(GetCertHash());
			}

	// Get the effective date of this certificate as a string.
	public virtual String GetEffectiveDateString()
			{
				return effectiveDate;
			}

	// Get the expiration date of this certificate as a string.
	public virtual String GetExpirationDateString()
			{
				return expirationDate;
			}

	// Get the name of the certificate format.
	public virtual String GetFormat()
			{
				return "X509";
			}

	// Get the hash code for this certificate.
	public override int GetHashCode()
			{
				byte[] certHash = GetCertHash();
				int hash = 0;
				if(certHash != null)
				{
					for(int index = 0; index < certHash.Length; ++index)
					{
						hash = (hash << 5) + hash + certHash[index];
					}
				}
				return hash & 0x7FFFFFFF;
			}

	// Get the name of the certificate issuer.
	public virtual String GetIssuerName()
			{
				return issuer;
			}

	// Get the name of the key algorithm as a hexadecimal string.
	public virtual String GetKeyAlgorithm()
			{
				return keyAlgorithm;
			}

	// Get the key algorithm parameters for this certificate.
	public virtual byte[] GetKeyAlgorithmParameters()
			{
				return keyAlgorithmParameters;
			}

	// Get the key algorithm parameters for this certificate as a string.
	public virtual String GetKeyAlgorithmParametersString()
			{
				return ToHex(GetKeyAlgorithmParameters());
			}

	// Get the name of the certificate's principal.
	public virtual String GetName()
			{
				return name;
			}

	// Get the public key for this certificate.
	public virtual byte[] GetPublicKey()
			{
				return publicKey;
			}

	// Get the public key for this certificate as a hexadecimal string.
	public virtual String GetPublicKeyString()
			{
				return ToHex(GetPublicKey());
			}

	// Get the raw data for the certificate.
	public virtual byte[] GetRawCertData()
			{
				return rawData;
			}

	// Get the raw data for the certificate as a hexadecimal string.
	public virtual String GetRawCertDataString()
			{
				return ToHex(GetRawCertData());
			}

	// Get the serial number of the certificate.
	public virtual byte[] GetSerialNumber()
			{
				return serialNumber;
			}

	// Get the serial number of the certificate as a hexadecimal string.
	public virtual String GetSerialNumberString()
			{
				return ToHex(GetSerialNumber());
			}

#if CONFIG_FRAMEWORK_2_0
	[TODO]
	public IntPtr Handle
			{
				get
				{
					throw new NotImplementedException("Handle");
				}
			}
	[TODO]
	public virtual void Import(byte[] rawData)
			{
				throw new NotImplementedException("Import");
			}
	[TODO]
	public virtual void Import(byte[] rawData, String password, X509KeyStorageFlags keyStorageFlags)
			{
				throw new NotImplementedException("Import");
			}
	[TODO]
	public virtual void Import(String fileName)
			{
				throw new NotImplementedException("Import");
			}
	[TODO]
	public virtual void Import(String fileName, String password, X509KeyStorageFlags keyStorageFlags)
			{
				throw new NotImplementedException("Import");
			}
	[TODO]
	public virtual void Reset()
			{
				throw new NotImplementedException("Reset");
			}
#endif
#if CONFIG_SERIALIZATION
	[TODO]
	void IDeserializationCallback.OnDeserialization(Object sender)
			{
				throw new NotImplementedException("OnDeserialization");
			}
	[TODO]
	void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				throw new NotImplementedException("GetObjectData");
			}
#endif // CONFIG_SERIALIZATION

	// Get the string represenation of this certificate.
	public override String ToString()
			{
				return ToString(false);
			}
	public virtual String ToString(bool verbose)
			{
				StringBuilder builder = new StringBuilder();
				String newLine;
				if(verbose)
				{
					// Print all of the fields.
					newLine = Environment.NewLine;
					builder.Append("Issuer: ");
					builder.Append(issuer);
					builder.Append(newLine);
					builder.Append("Serial Number: ");
					builder.Append(ToHex(serialNumber));
					builder.Append(newLine);
					builder.Append("Subject: ");
					builder.Append(name);
					builder.Append(newLine);
					builder.Append("Effective Date: ");
					builder.Append(effectiveDate);
					builder.Append(newLine);
					builder.Append("Expiration Date: ");
					builder.Append(expirationDate);
					builder.Append(newLine);
					builder.Append("Key Algorithm: ");
					builder.Append(keyAlgorithm);
					builder.Append(newLine);
					builder.Append("MD5 Hash: ");
					builder.Append(hash);
					builder.Append(newLine);
				}
				else
				{
					// Print the issuer name and serial number only.
					builder.Append(issuer);
					builder.Append(", ");
					builder.Append(ToHex(serialNumber));
				}
				return builder.ToString();
			}

}; // class X509Certificate

#endif // CONFIG_X509_CERTIFICATES

}; // namespace System.Security.Cryptography.X509Certificates
