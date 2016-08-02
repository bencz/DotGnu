/*
 * ITransaction.cs - Implementation of the
 *			"System.EnterpriseServices.ITransaction" class.
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
[Guid("0FB15084-AF41-11CE-BD2B-204C4F4F5020")]
#endif
public interface ITransaction
{
	// Abort the transaction.
	void Abort(ref BOID pboidReason, int fRetaining, int fAsync);

	// Commit the transaction.
	void Commit(int fRetaining, int grfTC, int grfRM);

	// Get information about this transaction.
	void GetTransactionInfo(out XACTTRANSINFO pinfo);

}; // interface ITransaction

}; // namespace System.EnterpriseServices
