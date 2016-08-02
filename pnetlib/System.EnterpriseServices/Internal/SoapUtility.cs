/*
 * SoapUtility.cs - Implementation of the
 *		"System.EnterpriseServices.Internal.SoapUtility" class.
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
[Guid("5F9A955F-AA55-4127-A32B-33496AA8A44E")]
#endif
public sealed class SoapUtility : ISoapUtility
{
	// Constructor.
	public SoapUtility() {}

	// Get the path for the virtual root binary directory.
	public void GetServerBinPath(String rootWebServer, String inBaseUrl,
		     			  		 String inVirtualRoot, out String binPath)
			{
				throw new SecurityException();
			}

	// Get the path for the virtual root physical directory.
	public void GetServerPhysicalPath
				(String rootWebServer, String inBaseUrl,
		         String inVirtualRoot, out string physicalPath)
			{
				throw new SecurityException();
			}

	// Determine if secure services are available.
	public void Present()
			{
				throw new SecurityException();
			}

}; // class SoapUtility

}; // namespace System.EnterpriseServices.Internal
