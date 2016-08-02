/*
 * ProtectionLevel.cs - Implementation of the
 *							"System.Net.Security.ProtectionLevel" class.
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

namespace System.Net.Security
{

#if CONFIG_FRAMEWORK_2_0 && !ECMA_COMPAT

public enum ProtectionLevel
{
	None			= 0,
	Sign			= 1,
	EncryptAndSign	= 2
}; // enum ProtectionLevel

#endif // CONFIG_FRAMEWORK_2_0 && !ECMA_COMPAT

}; // namespace System.Net.Security
