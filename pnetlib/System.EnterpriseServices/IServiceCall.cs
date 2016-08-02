/*
 * IServiceCall.cs - Implementation of the
 *			"System.EnterpriseServices.IServiceCall" class.
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
[ComImport]
#endif
#if !ECMA_COMPAT
[Guid("BD3E2E12-42DD-40f4-A09A-95A50C58304B")]
#endif
public interface IServiceCall
{
	// Start execution of the batch work for this call.
	void OnCall();

}; // interface IServiceCall

}; // namespace System.EnterpriseServices
