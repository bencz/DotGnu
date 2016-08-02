/*
 * Publish.cs - Implementation of the
 *		"System.EnterpriseServices.Internal.Publish" class.
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
[Guid("d8013eef-730b-45e2-ba24-874b7242c425")]
#endif
public class Publish : IComSoapPublisher
{
	// Constructor.
	public Publish() {}

	// Create a mailbox at a specified URL.
	public void CreateMailBox(String RootMailServer, String MailBox,
		     		   		  out String SmtpName, out String Domain,
				   	   		  out String PhysicalPath, out String Error)
			{
				throw new SecurityException();
			}

	// Create a virtual root.
	public void CreateVirtualRoot(String Operation, String FullUrl,
		     			   		  out String BaseUrl, out String VirtualRoot,
				   		   		  out String PhysicalPath, out String Error)
			{
				throw new SecurityException();
			}

	// Delete a mailbox.
	public void DeleteMailBox(String RootMailServer, String MailBox,
					   		  out string Error)
			{
				throw new SecurityException();
			}

	// Delete a virtual root.
	public void DeleteVirtualRoot(String RootWebServer, String FullUrl,
		     			   		  out String Error)
			{
				throw new SecurityException();
			}

	// Install an assembly into the GAC.
	public void GacInstall(String AssemblyPath)
			{
				throw new SecurityException();
			}

	// Remove an assembly from the GAC.
	public void GacRemove(String AssemblyPath)
			{
				throw new SecurityException();
			}

	// Get the full name of an assembly in the cache.
	public void GetAssemblyNameForCache
				(String TypeLibPath, out string CachePath)
			{
				throw new SecurityException();
			}

	// Get the path for storing client configuration files.
	public static String GetClientPhysicalPath(bool CreateDir)
			{
				throw new UnauthorizedAccessException();
			}

	// Get the type that corresponds to a particular ProgId.
	public String GetTypeNameFromProgId(String AssemblyPath, String ProgId)
			{
				throw new SecurityException();
			}

	// Parse a URL into base and virtual root portions.
	public static void ParseUrl(String FullUrl, out String BaseUrl,
		     					out String VirtualRoot)
			{
				throw new SecurityException();
			}

	// Create a configuration file for a client TLB description.
	public void ProcessClientTlb(String ProgId, String SrcTlbPath,
						  		 String PhysicalPath, String VRoot,
				   		  		 String BaseUrl, String Mode, String Transport,
						  		 out String AssemblyName, out String TypeName,
						  		 out String Error)
			{
				throw new SecurityException();
			}

	// Create a configuration file for a server TLB description.
	public void ProcessServerTlb(String ProgId, String SrcTlbPath,
						  		 String PhysicalPath, String Operation,
				   		  		 out String AssemblyName, out String TypeName,
					      		 out String Error)
			{
				throw new SecurityException();
			}

	// Register an assembly.
	public void RegisterAssembly(String AssemblyPath)
			{
				throw new SecurityException();
			}

	// Unregister an assembly.
	public void UnRegisterAssembly(String AssemblyPath)
			{
				throw new SecurityException();
			}

}; // class Publish

}; // namespace System.EnterpriseServices.Internal
