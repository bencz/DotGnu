/*
 * CspKeyContainerInfo.cs - Implementation of the
 *		"System.Security.Cryptography.CspKeyContainerInfo" class.
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

namespace System.Security.Cryptography
{

#if CONFIG_CRYPTO && CONFIG_FRAMEWORK_1_2

public sealed class CspKeyContainerInfo
{
	// Internal state.
	private CspParameters parameters;
	private bool randomlyGenerated;

	// Constructors.
	public CspKeyContainerInfo(CspParameters parameters)
			{
				this.parameters = parameters;
				this.randomlyGenerated = true;
			}
	internal CspKeyContainerInfo(CspParameters parameters, bool random)
			{
				this.parameters = parameters;
				this.randomlyGenerated = random;
			}

	// Get this object's properties.  Most of these have fake values
	// because we don't use CSP's, hardware devices, or machine key stores.
	public bool Accessible
			{
				get
				{
					return true;
				}
			}
	public bool Exportable
			{
				get
				{
					return true;
				}
			}
	public bool HardwareDevice
			{
				get
				{
					return true;
				}
			}
	public String KeyContainerName
			{
				get
				{
					return parameters.KeyContainerName;
				}
			}
	public KeyNumber KeyNumber
			{
				get
				{
					return (KeyNumber)(parameters.KeyNumber);
				}
			}
	public bool MachineKeyStore
			{
				get
				{
					return false;
				}
			}
	public bool Protected
			{
				get
				{
					return false;
				}
			}
	public String ProviderName
			{
				get
				{
					return parameters.ProviderName;
				}
			}
	public int ProviderType
			{
				get
				{
					return parameters.ProviderType;
				}
			}
	public bool RandomlyGenerated
			{
				get
				{
					return randomlyGenerated;
				}
			}
	public bool Removable
			{
				get
				{
					return false;
				}
			}
	public String UniqueKeyContainerName
			{
				get
				{
					return ProviderName + "\\" + KeyContainerName;
				}
			}

}; // class CspKeyContainerInfo

#endif // CONFIG_CRYPTO && CONFIG_FRAMEWORK_1_2

}; // namespace System.Security.Cryptography
