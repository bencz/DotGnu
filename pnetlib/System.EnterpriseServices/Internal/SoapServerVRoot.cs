/*
 * SoapServerVRoot.cs - Implementation of the
 *		"System.EnterpriseServices.Internal.SoapServerVRoot" class.
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
[Guid("CAA817CC-0C04-4d22-A05C-2B7E162F4E8F")]
#endif
public sealed class SoapServerVRoot : ISoapServerVRoot
{
	// Constructor.
	public SoapServerVRoot() {}

	// Create a virtual root with security options.
	public void CreateVirtualRootEx
				(String rootWebServer, String inBaseUrl,
		     	 String inVirtualRoot, String homePage,
				 String discoFile, String secureSockets,
				 String authentication, String operation,
				 out String baseUrl, out String virtualRoot,
				 out String physicalPath)
			{
				throw new SecurityException();
			}

	// Delete a virtual root.
	public void DeleteVirtualRootEx
				(String rootWebServer, String baseUrl, String virtualRoot)
			{
				throw new SecurityException();
			}

	// Get the security status of an existing virtual root.
	public void GetVirtualRootStatus
				(String rootWebServer, String inBaseUrl,
		     	 String inVirtualRoot, out String exists,
				 out String secureSockets, out String windowsAuth,
				 out String anonymous, out String homePage,
				 out String discoFile, out String physicalPath,
				 out String baseUrl, out String virtualRoot)
			{
				throw new SecurityException();
			}

}; // class SoapServerVRoot

}; // namespace System.EnterpriseServices.Internal
