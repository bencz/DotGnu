/*
 * X509KeyUsageFlags.cs - Implementation of the
 *	"System.Security.Cryptography.X509Certificates.X509KeyUsageFlags" class.
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
public enum X509KeyUsageFlags
{
	None				= 0x0000,
	EncipherOnly		= 0x0001,
	CrlSign				= 0x0002,
	KeyCertSign			= 0x0004,
	KeyAgreement		= 0x0008,
	DataEncipherment	= 0x0010,
	KeyEncipherment		= 0x0020,
	NonRepudiation		= 0x0040,
	DigitalSignature	= 0x0080,
	DecipherOnly		= 0x8000,
}; // enum X509KeyUsageFlags

#endif // CONFIG_FRAMEWORK_2_0 && !ECMA_COMPAT && CONFIG_X509_CERTIFICATES

}; // namespace System.Security.Cryptography.X509Certificates
