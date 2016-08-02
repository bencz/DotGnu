/*
 * AssemblyName.cs - Implementation of the
 *		"System.Reflection.AssemblyName" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Reflection
{

#if !ECMA_COMPAT

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Configuration.Assemblies;
using System.Security.Cryptography;

public sealed class AssemblyName
	: ICloneable
#if CONFIG_SERIALIZATION
	, ISerializable, IDeserializationCallback
#endif
{
	// Internal state.
	private String codeBase;
	private CultureInfo culture;
	private AssemblyNameFlags flags;
	private String name;
	private AssemblyHashAlgorithm hashAlg;
	private StrongNameKeyPair keyPair;
	private Version version;
	private AssemblyVersionCompatibility versionCompat;
	private byte[] publicKey;
	private byte[] publicKeyToken;
#if CONFIG_SERIALIZATION
	private SerializationInfo info;
#endif

	// Constructor.
	public AssemblyName()
			{
				versionCompat = AssemblyVersionCompatibility.SameMachine;
			}
#if CONFIG_SERIALIZATION
	internal AssemblyName(SerializationInfo info, StreamingContext context)
			{
				this.info = info;
			}
#endif
	private AssemblyName(AssemblyName other)
			{
				codeBase = other.codeBase;
				culture = other.culture;
				flags = other.flags;
				name = other.name;
				hashAlg = other.hashAlg;
				keyPair = other.keyPair;
				version = (version == null
					? null : (Version)(other.version.Clone()));
				versionCompat = other.versionCompat;
				publicKey = (other.publicKey == null
					? null : (byte[])(other.publicKey.Clone()));
				publicKeyToken = (other.publicKeyToken == null
					? null : (byte[])(other.publicKeyToken.Clone()));
			}

	// Fill assembly name information from a file.  Returns a load error code.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static int FillAssemblyNameFromFile
				(AssemblyName nameInfo, String assemblyFile, Assembly caller);

	// Get the assembly name for a specific file.
	public static AssemblyName GetAssemblyName(String assemblyFile)
			{
				if(assemblyFile == null)
				{
					throw new ArgumentNullException("assemblyFile");
				}
				assemblyFile = Path.GetFullPath(assemblyFile);
				AssemblyName nameInfo = new AssemblyName();
				if(assemblyFile[0] == '/' || assemblyFile[0] == '\\')
				{
					nameInfo.CodeBase = "file://" +
						assemblyFile.Replace('\\', '/');
				}
				else
				{
					nameInfo.CodeBase = "file:///" +
						assemblyFile.Replace('\\', '/');
				}
				int error = FillAssemblyNameFromFile
					(nameInfo, assemblyFile, Assembly.GetCallingAssembly());
				if(error != Assembly.LoadError_OK)
				{
					Assembly.ThrowLoadError(assemblyFile, error);
				}
				return nameInfo;
			}

	// Get or set the code base for the assembly name.
	public String CodeBase
			{
				get
				{
					return codeBase;
				}
				set
				{
					codeBase = value;
				}
			}

	// Get or set the culture associated with the assembly name.
	public CultureInfo CultureInfo
			{
				get
				{
					return culture;
				}
				set
				{
					culture = value;
				}
			}

	// Get the escaped code base for the assembly name.
	public String EscapedCodeBase
			{
				get
				{
					if(codeBase == null)
					{
						return null;
					}
					StringBuilder builder = new StringBuilder();
					foreach(char ch in codeBase)
					{
						if(ch == ' ' || ch == '%')
						{
							builder.Append(String.Format("%{0:x2}", (int)ch));
						}
						else
						{
							builder.Append(ch);
						}
					}
					return builder.ToString();
				}
			}

	// Get or set the assembly name flags.
	public AssemblyNameFlags Flags
			{
				get
				{
					return flags;
				}
				set
				{
					flags = value;
				}
			}

	// Get the full name of the assembly.
	public String FullName
			{
				get
				{
					if(name == null)
					{
						return null;
					}
					StringBuilder builder = new StringBuilder();
					builder.Append(name);
					builder.Append(", Version=");
					if(version != null)
					{
						builder.Append(version.ToString());
					}
					else
					{
						builder.Append("0.0.0.0");
					}
					builder.Append(", Culture=");
					if(culture != null && culture.LCID != 0x007F)
					{
						builder.Append(culture.Name);
					}
					else
					{
						builder.Append("neutral");
					}
					byte[] token = GetPublicKeyToken();
					builder.Append(", PublicKeyToken=");
					if(token != null)
					{
						foreach(byte b in token)
						{
							builder.Append(String.Format("{0:x2}", (int)b));
						}
					}
					else
					{
						builder.Append("null");
					}
					return builder.ToString();
				}
			}

	// Get or set the hash algorithm for this assembly name.
	public AssemblyHashAlgorithm HashAlgorithm
			{
				get
				{
					return hashAlg;
				}
				set
				{
					hashAlg = value;
				}
			}

	// Get or set the key pair for this assembly name.
	public StrongNameKeyPair KeyPair
			{
				get
				{
					return keyPair;
				}
				set
				{
					keyPair = value;
				}
			}

	// Get or set the simple name of the assembly name.
	public String Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}

	// Get or set the version information of the assembly name.
	public Version Version
			{
				get
				{
					return version;
				}
				set
				{
					version = value;
				}
			}

	// Get or set the version compatibility value for the assembly name.
	public AssemblyVersionCompatibility VersionCompatibility
			{
				get
				{
					return versionCompat;
				}
				set
				{
					versionCompat = value;
				}
			}

	// Clone this object.
	public Object Clone()
			{
				return new AssemblyName(this);
			}

	// Get the public key for the assembly's originator.
	public byte[] GetPublicKey()
			{
				return publicKey;
			}

	// Set the public key for the assembly's originator.
	public void SetPublicKey(byte[] publicKey)
			{
				this.publicKey = publicKey;
				this.flags |= AssemblyNameFlags.PublicKey;
			}

	// Get the public key token for the assembly's originator.
	public byte[] GetPublicKeyToken()
			{
			#if CONFIG_CRYPTO
				if(publicKeyToken == null && publicKey != null)
				{
					// The public key token is the reverse of the last
					// eight bytes of the SHA1 hash of the public key.
					SHA1CryptoServiceProvider sha1;
					sha1 = new SHA1CryptoServiceProvider();
					byte[] hash = sha1.ComputeHash(publicKey);
					((IDisposable)sha1).Dispose();
					publicKeyToken = new byte [8];
					int posn;
					for(posn = 0; posn < 8; ++posn)
					{
						publicKeyToken[posn] = hash[hash.Length - 1 - posn];
					}
				}
			#endif
				return publicKeyToken;
			}

	// Set the public key token for the assembly's originator.
	public void SetPublicKeyToken(byte[] publicKeyToken)
			{
				this.publicKeyToken = publicKeyToken;
			}

	// Convert this assembly name into a string.
	public override String ToString()
			{
				String name = FullName;
				if(name != null)
				{
					return name;
				}
				return base.ToString();
			}

	// Set the culture by name.
	internal void SetCultureByName(String name)
			{
				if(name == null || name.Length == 0 || name == "iv")
				{
					culture = null;
				}
				else
				{
					try
					{
						culture = new CultureInfo(name);
					}
					catch(Exception)
					{
						// The culture name was probably not understood.
						culture = null;
					}
				}
			}

	// Set the version information by number.
	internal void SetVersion(int major, int minor, int build, int revision)
			{
				version = new Version(major, minor, build, revision);
			}

	static internal AssemblyName Parse(String assemblyName)
			{
				AssemblyName retval = new AssemblyName();
				if(assemblyName.IndexOf(",") == -1)
				{
					retval.Name = assemblyName;
				}
				else
				{
					// TODO : Parse the rest of the information
					// as well. Version maybe important .
					retval.Name = assemblyName.Substring(0,
										assemblyName.IndexOf(","));
				}
				return retval;
			}

#if CONFIG_SERIALIZATION

	// Get the serialization data for this object.
	public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("_Name", name);
				info.AddValue("_PublicKey", publicKey, typeof(byte[]));
				info.AddValue("_PublicKeyToken", publicKeyToken,
							  typeof(byte[]));
				if(culture == null)
				{
					info.AddValue("_CultureInfo", -1);
				}
				else
				{
					info.AddValue("_CultureInfo", culture.LCID);
				}
				info.AddValue("_CodeBase", codeBase);
				info.AddValue("_Version", version, typeof(Version));
				info.AddValue("_HashAlgorithm", hashAlg,
							  typeof(AssemblyHashAlgorithm));
				info.AddValue("_StrongNameKeyPair", keyPair,
							  typeof(StrongNameKeyPair));
				info.AddValue("_VersionCompatibility", versionCompat,
							  typeof(AssemblyVersionCompatibility));
				info.AddValue("_Flags", flags, typeof(AssemblyNameFlags));
			}

	// Handle a deserialization callback on this object.
	public void OnDeserialization(Object sender)
			{
				if(info == null)
				{
					return;
				}
				name = info.GetString("_Name");
				publicKey = (byte[])(info.GetValue
					("_PublicKey", typeof(byte[])));
				publicKeyToken = (byte[])(info.GetValue
					("_PublicKeyToken", typeof(byte[])));
				int cultureID = info.GetInt32("_CultureInfo");
				if(cultureID != -1)
				{
					culture = new CultureInfo(cultureID);
				}
				else
				{
					culture = null;
				}
				codeBase = info.GetString("_CodeBase");
				version = (Version)(info.GetValue("_Version", typeof(Version)));
				hashAlg = (AssemblyHashAlgorithm)(info.GetValue
					("_HashAlgorithm", typeof(AssemblyHashAlgorithm)));
				keyPair = (StrongNameKeyPair)(info.GetValue
					("_StrongNameKeyPair", typeof(StrongNameKeyPair)));
				versionCompat = (AssemblyVersionCompatibility)(info.GetValue
					("_VersionCompatibility",
					 typeof(AssemblyVersionCompatibility)));
				flags = (AssemblyNameFlags)(info.GetValue
					("_Flags", typeof(AssemblyNameFlags)));
			}

#endif // CONFIG_SERIALIZATION

}; // class AssemblyName

#endif // !ECMA_COMPAT

}; // namespace System.Reflection
