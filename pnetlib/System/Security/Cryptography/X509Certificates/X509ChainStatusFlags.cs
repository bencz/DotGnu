/*
 * X509ChainStatusFlags.cs - Implementation of the
 *	"System.Security.Cryptography.X509Certificates.X509ChainStatusFlags" class.
 *
 * Copyright (C) 2010  Southern Storm Software, Pty Ltd.
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

#if CONFIG_FRAMEWORK_2_0 && !ECMA_COMPAT && CONFIG_X509_CERTIFICATES

[Flags]
public enum X509ChainStatusFlags
{
	NoError							= 0x00000000,
	NotTimeValid					= 0x00000001,
	NotTimeNested					= 0x00000002,
	Revoked							= 0x00000004,
	NotSignatureValid				= 0x00000008,
	NotValidForUsage				= 0x00000010,
	UntrustedRoot					= 0x00000020,
	RevocationStatusUnknown			= 0x00000040,
	Cyclic							= 0x00000080,
	InvalidExtension				= 0x00000100,
	InvalidPolicyConstraints		= 0x00000200,
	InvalidBasicConstraints			= 0x00000400,
	InvalidNameConstraints			= 0x00000800,
	HasNotSupportedNameConstraint	= 0x00001000,
	HasNotDefinedNameConstraint		= 0x00002000,
	HasNotPermittedNameConstraint	= 0x00004000,
	HasExcludeNameConstraint		= 0x00008000,
	PartialChain					= 0x00010000,
	CtlNotTimeValid					= 0x00020000,
	CtlNotSignatureValid			= 0x00040000,
	CtlNotValidForUsage				= 0x00080000,
	OfflineRevocation				= 0x01000000,
	NoIssuanceChainPolicy			= 0x02000000
}; // enum X509ChainStatusFlags

#endif // CONFIG_FRAMEWORK_2_0 && !ECMA_COMPAT && CONFIG_X509_CERTIFICATES

}; // namespace System.Security.Cryptography.X509Certificates
