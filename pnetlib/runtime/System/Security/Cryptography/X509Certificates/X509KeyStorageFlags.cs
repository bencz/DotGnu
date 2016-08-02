/*
 * X509KeyStorageFlags.cs - Implementation of the
 *		"System.Security.Cryptography.X509Certificates.X509KeyStorageFlags" class.
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

namespace System.Security.Cryptography.X509Certificates
{

#if CONFIG_FRAMEWORK_2_0 
#if CONFIG_X509_CERTIFICATES

[Serializable]
[Flags]
public enum X509KeyStorageFlags
{

	DefaultKeySet	= 0,
	UserKeySet		= 1,
	MachineKeySet	= 2,
	Exportable		= 4,
	UserProtected	= 8,
	PersistKeySet	= 16

}; // enum X509KeyStorageFlags

#endif // CONFIG_X509_CERTIFICATES
#endif // CONFIG_FRAMEWORK_2_0

}; // namespace System.Security.Cryptography.X509Certificates

