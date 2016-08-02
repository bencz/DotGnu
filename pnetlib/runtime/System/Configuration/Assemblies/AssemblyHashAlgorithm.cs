/*
 * AssemblyHashAlgorithm.cs - Implementation of the
 *		"System.Configuration.Assemblies.AssemblyHashAlgorithm" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Configuration.Assemblies
{

#if !ECMA_COMPAT

public enum AssemblyHashAlgorithm
{
	None = 0x0000,
	MD5  = 0x8003,
	SHA1 = 0x8004

}; // enum AssemblyHashAlgorithm

#endif // !ECMA_COMPAT

}; // namespace System.Configuration.Assemblies
