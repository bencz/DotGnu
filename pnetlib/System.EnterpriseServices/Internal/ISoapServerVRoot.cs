/*
 * ISoapServerVRoot.cs - Implementation of the
 *		"System.EnterpriseServices.Internal.ISoapServerVRoot" class.
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

#if !ECMA_COMPAT
[Guid("A31B6577-71D2-4344-AEDF-ADC1B0DC5347")]
#endif
public interface ISoapServerVRoot
{
	// Create a virtual root with security options.
#if !ECMA_COMPAT
	[DispId(1)]
#endif
	void CreateVirtualRootEx(String rootWebServer, String inBaseUrl,
		     				 String inVirtualRoot, String homePage,
				   			 String discoFile, String secureSockets,
					     	 String authentication, String operation,
							 out String baseUrl, out String virtualRoot,
							 out String physicalPath);

	// Delete a virtual root.
#if !ECMA_COMPAT
	[DispId(2)]
#endif
	void DeleteVirtualRootEx(String rootWebServer, String baseUrl,
		     				 String virtualRoot);

	// Get the security status of an existing virtual root.
#if !ECMA_COMPAT
	[DispId(3)]
#endif
	void GetVirtualRootStatus(String rootWebServer, String inBaseUrl,
		     				  String inVirtualRoot, out String exists,
				   			  out String secureSockets, out String windowsAuth,
					     	  out String anonymous, out String homePage,
							  out String discoFile, out String physicalPath,
							  out String baseUrl, out String virtualRoot);

}; // interface ISoapServerVRoot

}; // namespace System.EnterpriseServices.Internal
