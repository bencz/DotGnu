/*
 * IServicedComponentInfo.cs - Implementation of the
 *			"System.EnterpriseServices.IServicedComponentInfo" class.
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

namespace System.EnterpriseServices
{

using System.Runtime.InteropServices;

#if CONFIG_COM_INTEROP
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
#endif
#if !ECMA_COMPAT
[Guid("8165B19E-8D3A-4d0b-80C8-97DE310DB583")]
#endif
public interface IServicedComponentInfo
{
	// Get information about this component.
	void GetComponentInfo(ref int infoMask, out String[] infoArray);

}; // interface IServicedComponentInfo

}; // namespace System.EnterpriseServices
