/*
 * SignatureDescription.cs - Implementation of the
 *		"System.Security.Cryptography.SignatureDescription" class.
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

public class SignatureDescription
{
	// Internal state.
	private String deformatter;
	private String digest;
	private String formatter;
	private String key;

	// Constructor.
	public SignatureDescription()
			{
				// Nothing to do here.
			}
	public SignatureDescription(SecurityElement el)
			{
				if(el == null)
				{
					throw new ArgumentNullException("el");
				}
				foreach(SecurityElement e in el.Children)
				{
					if(e.Tag == "Deformatter")
					{
						deformatter = e.Text;
					}
					else if(e.Tag == "Digest")
					{
						digest = e.Text;
					}
					else if(e.Tag == "Formatter")
					{
						formatter = e.Text;
					}
					else if(e.Tag == "Key")
					{
						key = e.Text;
					}
				}
			}

	// Get or set the signature deformatter algorithm.
	public String DeformatterAlgorithm
			{
				get
				{
					return deformatter;
				}
				set
				{
					deformatter = value;
				}
			}

	// Get or set the digest algorithm.
	public String DigestAlgorithm
			{
				get
				{
					return digest;
				}
				set
				{
					digest = value;
				}
			}

	// Get or set the signature formatter algorithm.
	public String FormatterAlgorithm
			{
				get
				{
					return formatter;
				}
				set
				{
					formatter = value;
				}
			}

	// Get or set the key exchange algorithm.
	public String KeyAlgorithm
			{
				get
				{
					return key;
				}
				set
				{
					key = value;
				}
			}

	// Create a signature deformatter from a key instance.
	public virtual AsymmetricSignatureDeformatter CreateDeformatter
				(AsymmetricAlgorithm key)
			{
				AsymmetricSignatureDeformatter obj;
				obj = (AsymmetricSignatureDeformatter)
					(CryptoConfig.CreateFromName(deformatter));
				obj.SetKey(key);
				return obj;
			}

	// Create a digest algorithm instance.
	public virtual HashAlgorithm CreateDigest()
			{
				return HashAlgorithm.Create(digest);
			}

	// Create a signature formatter from a key instance.
	public virtual AsymmetricSignatureFormatter CreateFormatter
				(AsymmetricAlgorithm key)
			{
				AsymmetricSignatureFormatter obj;
				obj = (AsymmetricSignatureFormatter)
					(CryptoConfig.CreateFromName(formatter));
				obj.SetKey(key);
				return obj;
			}

}; // class SignatureDescription

#endif // CONFIG_CRYPTO

}; // namespace System.Security.Cryptography
