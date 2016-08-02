/*
 * ASN1Type.cs - Implementation of the
 *		"System.Security.Cryptography.ASN1Type" class.
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

#if CONFIG_CRYPTO || CONFIG_X509_CERTIFICATES

internal enum ASN1Type
{
	Integer				= 2,
	BitString			= 3,
	OctetString			= 4,
	Null				= 5,
	ObjectIdentifier	= 6,
	Sequence			= 16,
	Set					= 17,
	PrintableString		= 19,
	IA5String			= 22,
	UTCTime				= 23

}; // enum ASN1Type

#endif // CONFIG_CRYPTO || CONFIG_X509_CERTIFICATES

}; // namespace System.Security.Cryptography
