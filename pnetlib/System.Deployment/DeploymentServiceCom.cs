/*
 * DeploymentServiceCom.cs - Implementation of the
 *		"System.Deployment.DeploymentServiceCom" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

namespace System.Deployment
{

using System;
using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[Guid("33246f92-d56f-4e34-837a-9a49bfc91df3")]
[ComVisible(true)]
#endif
#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDispatch)]
#endif
public class DeploymentServiceCom
{
	// Constructors.
	public DeploymentServiceCom() {}

	// Activate a particular deployment.
	public void ActivateDeployment
				(String deploymentUrl,
				 String deploymentLocalCachePath,
				 bool isShortcut)
			{
				// Nothing to do here in this implementation.
			}

	// Check for an update in a particular deployment.
	public void CheckForUpdate(String textualSubId)
			{
				// Nothing to do here in this implementation.
			}

	// End the service "right now".
	public void EndServiceRightNow()
			{
				// Nothing to do here in this implementation.
			}

	// Maintain a subscription to a particular deployment.
	public void MaintainSubscription(String textualSubId)
			{
				// Nothing to do here in this implementation.
			}

	// Migrate the deployments.
	public void MigrateDeployments()
			{
				// Nothing to do here in this implementation.
			}

}; // class DeploymentServiceCom

}; // namespace System.Deployment
