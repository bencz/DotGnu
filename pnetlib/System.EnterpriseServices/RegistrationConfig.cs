/*
 * RegistrationConfig.cs - Implementation of the
 *			"System.EnterpriseServices.RegistrationConfig" class.
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

#if !ECMA_COMPAT
[Serializable]
[Guid("36dcda30-dc3b-4d93-be42-90b2d74c64e7")]
#endif
public class RegistrationConfig
{
	// Internal state.
	private String application;
	private String applicationRootDirectory;
	private String assemblyFile;
	private InstallationFlags installationFlags;
	private String partition;
	private String typeLibrary;

	// Constructor.
	public RegistrationConfig() {}

	// Get or set this object's properties.
	public String Application
			{
				get
				{
					return application;
				}
				set
				{
					application = value;
				}
			}
	public String ApplicationRootDirectory
			{
				get
				{
					return applicationRootDirectory;
				}
				set
				{
					applicationRootDirectory = value;
				}
			}
	public String AssemblyFile
			{
				get
				{
					return assemblyFile;
				}
				set
				{
					assemblyFile = value;
				}
			}
	public InstallationFlags InstallationFlags
			{
				get
				{
					return installationFlags;
				}
				set
				{
					installationFlags = value;
				}
			}
	public String Partition
			{
				get
				{
					return partition;
				}
				set
				{
					partition = value;
				}
			}
	public String TypeLibrary
			{
				get
				{
					return typeLibrary;
				}
				set
				{
					typeLibrary = value;
				}
			}

}; // class RegistrationConfig

}; // namespace System.EnterpriseServices
