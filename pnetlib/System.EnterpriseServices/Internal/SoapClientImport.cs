/*
 * SoapClientImport.cs - Implementation of the
 *		"System.EnterpriseServices.Internal.SoapClientImport" class.
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

namespace System.EnterpriseServices.Internal
{

using System.Runtime.InteropServices;
using System.Security;

// This class is used by unmanaged COM+ code, which we don't support.

#if !ECMA_COMPAT
[Guid("346D5B9F-45E1-45c0-AADF-1B7D221E9063")]
#endif
public sealed class SoapClientImport : ISoapClientImport
{
	// Constructor.
	public SoapClientImport() {}

	// Create a configuration file for a client TLB description.
	public void ProcessClientTlbEx(String progId, String virtualRoot,
		     					   String baseUrl, String authentication,
				   				   String assemblyName, String typeName)
			{
				throw new SecurityException();
			}

}; // interface SoapClientImport

}; // namespace System.EnterpriseServices.Internal
