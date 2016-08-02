/*
 * ApplicationId.cs - Implementation of the "System.ApplicationId" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace System
{

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0

using System.Text;

public sealed class ApplicationId
{
	// Internal state.
	private byte[] publicKeyToken;
	private String strName;
	private Version version;
	private String strProcessorArchitecture;
	private String strCulture;

	// Constructor.
	public ApplicationId(byte[] publicKeyToken, String strName,
						 Version version, String strProcessorArchitecture,
						 String strCulture)
			{
				if(strName == null)
				{
					throw new ArgumentNullException("strName");
				}
				if(version == null)
				{
					throw new ArgumentNullException("version");
				}
				if(publicKeyToken == null)
				{
					throw new ArgumentNullException("publicKeyToken");
				}
				this.publicKeyToken = (Byte[])publicKeyToken.Clone();
				this.strName = strName;
				this.version = version;
				this.strProcessorArchitecture = strProcessorArchitecture;
				this.strCulture = strCulture;
			}

	// Get this object's properties.
	public String Culture
			{
				get
				{
					return strCulture;
				}
			}
	public String Name
			{
				get
				{
					return strName;
				}
			}
	public String ProcessorArchitecture
			{
				get
				{
					return strProcessorArchitecture;
				}
			}
	public byte[] PublicKeyToken
			{
				get
				{
					return (Byte[])publicKeyToken.Clone();
				}
			}
	public Version Version
			{
				get
				{
					return version;
				}
			}

	// Make a copy of this object.
	public ApplicationId Copy()
			{
				return (ApplicationId)(MemberwiseClone());
			}

	// Determine if two objects are equal.
	public override bool Equals(Object o)
			{
				ApplicationId other = (o as ApplicationId);
				if(other != null)
				{
					if(strName != other.strName ||
					   strProcessorArchitecture !=
					   		other.strProcessorArchitecture ||
					   strCulture != other.strCulture ||
					   version != other.version)
					{
						return false;
					}
					if(publicKeyToken == null)
					{
						return (other.publicKeyToken == null);
					}
					else if(other.publicKeyToken == null ||
							publicKeyToken.Length !=
								other.publicKeyToken.Length)
					{
						return false;
					}
					else
					{
						int posn;
						for(posn = 0; posn < publicKeyToken.Length; ++posn)
						{
							if(publicKeyToken[posn] !=
									other.publicKeyToken[posn])
							{
								return false;
							}
						}
						return true;
					}
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				if(strName != null)
				{
					return strName.GetHashCode();
				}
				else
				{
					return 0;
				}
			}

	// Convert this object into a string.
	public override String ToString()
			{
				StringBuilder builder = new StringBuilder();
				if(strName != null)
				{
					builder.Append(strName);
				}
				if(strCulture != null)
				{
					builder.Append(", culture=");
					builder.Append('"');
					builder.Append(strCulture);
					builder.Append('"');
				}
				if(version != null)
				{
					builder.Append(", version=");
					builder.Append('"');
					builder.Append(version.ToString());
					builder.Append('"');
				}
				if(publicKeyToken != null)
				{
					builder.Append(", publicKeyToken=");
					builder.Append('"');
					foreach(byte value in publicKeyToken)
					{
						BitConverter.AppendHex(builder, value);
					}
					builder.Append('"');
				}
				if(strProcessorArchitecture != null)
				{
					builder.Append(", processorArchitecture =");
					builder.Append('"');
					builder.Append(strProcessorArchitecture);
					builder.Append('"');
				}
				return builder.ToString();
			}

}; // class ApplicationId

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_2_0

}; // namespace System
