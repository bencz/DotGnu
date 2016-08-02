/*
 * SoapServerTlb.cs - Implementation of the
 *		"System.EnterpriseServices.Internal.SoapServerTlb" class.
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
[Guid("F6B6768F-F99E-4152-8ED2-0412F78517FB")]
#endif
public sealed class SoapServerTlb : ISoapServerTlb
{
	// Add configuration information for a server TLB description.
	public void AddServerTlb
				(String progId, String classId, String interfaceId,
			     String srcTlbPath, String rootWebServer, String baseUrl,
				 String virtualRoot, String clientActivated,
				 String wellKnown, String discoFile, String operation,
				 out String assemblyName, out String typeName)
			{
				throw new SecurityException();
			}

	// Delete configuration information for a server TLB description.
	public void DeleteServerTlb
				(String progId, String classId, String interfaceId,
			     String srcTlbPath, String rootWebServer,
			     String baseUrl, String virtualRoot, String operation,
			     String assemblyName, String typeName)
			{
				throw new SecurityException();
			}

}; // class SoapServerTlb

}; // namespace System.EnterpriseServices.Internal
