/*
 * ISoapUtility.cs - Implementation of the
 *		"System.EnterpriseServices.Internal.ISoapUtility" class.
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
[Guid("5AC4CB7E-F89F-429b-926B-C7F940936BF4")]
#endif
public interface ISoapUtility
{
	// Get the path for the virtual root binary directory.
#if !ECMA_COMPAT
	[DispId(2)]
#endif
	void GetServerBinPath(String rootWebServer, String inBaseUrl,
		     			  String inVirtualRoot, out String binPath);

	// Get the path for the virtual root physical directory.
#if !ECMA_COMPAT
	[DispId(1)]
#endif
	void GetServerPhysicalPath(String rootWebServer, String inBaseUrl,
		     				   String inVirtualRoot, out string physicalPath);

	// Determine if secure services are available.
#if !ECMA_COMPAT
	[DispId(3)]
#endif
	void Present();

}; // interface ISoapUtility

}; // namespace System.EnterpriseServices.Internal
