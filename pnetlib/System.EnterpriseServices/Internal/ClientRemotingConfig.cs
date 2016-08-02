/*
 * ClientRemotingConfig.cs - Implementation of the
 *		"System.EnterpriseServices.Internal.ClientRemotingConfig" class.
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

using System.Security;

public class ClientRemotingConfig
{
	// Constructor.
	public ClientRemotingConfig() {}

	// Write a configuration value.
	public static bool Write(String DestinationDirectory,
							 String VRoot, String BaseUrl,
							 String AssemblyName, String TypeName,
							 String ProgId, String Mode, String Transport)
			{
				// Not used in this implementation.
				throw new SecurityException();
			}

}; // class ClientRemotingConfig

}; // namespace System.EnterpriseServices.Internal
