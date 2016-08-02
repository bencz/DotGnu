/*
 * IComSoapPublisher.cs - Implementation of the
 *		"System.EnterpriseServices.Internal.IComSoapPublisher" class.
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
[Guid("d8013eee-730b-45e2-ba24-874b7242c425")]
#endif
public interface IComSoapPublisher
{
	// Create a mailbox at a specified URL.
#if !ECMA_COMPAT
	[DispId(6)]
#endif
	void CreateMailBox(String RootMailServer, String MailBox,
		     		   out String SmtpName, out String Domain,
				   	   out String PhysicalPath, out String Error);

	// Create a virtual root.
#if !ECMA_COMPAT
	[DispId(4)]
#endif
	void CreateVirtualRoot(String Operation, String FullUrl,
		     			   out String BaseUrl, out String VirtualRoot,
				   		   out String PhysicalPath, out String Error);

	// Delete a mailbox.
#if !ECMA_COMPAT
	[DispId(7)]
#endif
	void DeleteMailBox(String RootMailServer, String MailBox,
					   out string Error);

	// Delete a virtual root.
#if !ECMA_COMPAT
	[DispId(5)]
#endif
	void DeleteVirtualRoot(String RootWebServer, String FullUrl,
		     			   out String Error);

	// Install an assembly into the GAC.
#if !ECMA_COMPAT
	[DispId(13)]
#endif
	void GacInstall(String AssemblyPath);

	// Remove an assembly from the GAC.
#if !ECMA_COMPAT
	[DispId(14)]
#endif
	void GacRemove(String AssemblyPath);

	// Get the full name of an assembly in the cache.
#if !ECMA_COMPAT
	[DispId(15)]
#endif
	void GetAssemblyNameForCache(String TypeLibPath, out string CachePath);

	// Get the type that corresponds to a particular ProgId.
#if !ECMA_COMPAT
	[DispId(10)]
#endif
	String GetTypeNameFromProgId(String AssemblyPath, String ProgId);

	// Create a configuration file for a client TLB description.
#if !ECMA_COMPAT
	[DispId(9)]
#endif
	void ProcessClientTlb(String ProgId, String SrcTlbPath,
						  String PhysicalPath, String VRoot,
				   		  String BaseUrl, String Mode, String Transport,
						  out String AssemblyName, out String TypeName,
						  out String Error);

	// Create a configuration file for a server TLB description.
#if !ECMA_COMPAT
	[DispId(8)]
#endif
	void ProcessServerTlb(String ProgId, String SrcTlbPath,
						  String PhysicalPath, String Operation,
				   		  out String AssemblyName, out String TypeName,
					      out String Error);

	// Register an assembly.
#if !ECMA_COMPAT
	[DispId(11)]
#endif
	void RegisterAssembly(String AssemblyPath);

	// Unregister an assembly.
#if !ECMA_COMPAT
	[DispId(12)]
#endif
	void UnRegisterAssembly(String AssemblyPath);

}; // interface IComSoapPublisher

}; // namespace System.EnterpriseServices.Internal
