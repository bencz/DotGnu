/*
 * DeploymentManager.cs - Implementation of the
 *		"System.Deployment.DeploymentManager" class.
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
using System.Runtime.Remoting;
using System.ComponentModel;

public class DeploymentManager : IDisposable
{
	// Internal state.
	private bool cancellationPending;

	// Constructors.  Not supported: see README for futher details.
	public DeploymentManager(String appId)
			{
				throw new PlatformNotSupportedException();
			}
	public DeploymentManager(Uri deploymentSource)
			{
				throw new PlatformNotSupportedException();
			}
	public DeploymentManager
				(String deploymentLocalCachePath, Uri deploymentSource)
			{
				throw new PlatformNotSupportedException();
			}

	// Determine if a cancellation is pending.
	public bool CancellationPending
			{
				get
				{
					return cancellationPending;
				}
			}

	// Stub out the update API's.  Because the constructor always throws
	// an exception, there is no way for the user to call these methods.

#if CONFIG_FRAMEWORK_2_0

	// Bind to a deployment and returns its activation context.
	public ActivationContext Bind()
			{
				throw new NotImplementedException();
			}

#endif

	// Bind to a deployment asynchronously.
	public void BindAsync()
			{
				throw new NotImplementedException();
			}

	// Cancel an asynchronous operation.
	public void CancelAsync()
			{
				throw new NotImplementedException();
			}
	public void CancelAsync(String groupName)
			{
				throw new NotImplementedException();
			}

	// Determine the platform requirements for this deployment.
	public void DeterminePlatformRequirements()
			{
				throw new NotImplementedException();
			}

	// Determine the platform requirements for this deployment asynchronously.
	public void DeterminePlatformRequirementsAsync()
			{
				throw new NotImplementedException();
			}

	// Determine if the user trusts this deployment.
	public void DetermineTrust(TrustParams tp)
			{
				throw new NotImplementedException();
			}

	// Determine if the user trusts this deployment asynchronously.
	public void DetermineTrustAsync(TrustParams tp)
			{
				throw new NotImplementedException();
			}

	// Dispose of this object.
	public void Dispose()
			{
				// Nothing to do here in this implementation.
			}

#if CONFIG_REMOTING

	// Execute this deployment in a new domain.
	public ObjectHandle ExecuteNewDomain()
			{
				throw new NotImplementedException();
			}

	// Execute this deployment in a new process.
	public void ExecuteNewProcess()
			{
				throw new NotImplementedException();
			}

#endif // CONFIG_REMOTING

	// Synchronize against this deployment.
	public void Synchronize()
			{
				throw new NotImplementedException();
			}
	public void Synchronize(String groupName)
			{
				throw new NotImplementedException();
			}

	// Synchronize against this deployment asynchronously.
	public void SynchronizeAsync()
			{
				throw new NotImplementedException();
			}
	public void SynchronizeAsync(String groupName)
			{
				SynchronizeAsync(groupName, null);
			}
	public void SynchronizeAsync(String groupName, Object userState)
			{
				throw new NotImplementedException();
			}

	// Events that may be emitted by this manager.
#if CONFIG_FRAMEWORK_2_0
	public event BindCompletedEventHandler BindCompleted;
	public event AsyncCompletedEventHandler
					DeterminePlatformRequirementsCompleted;
	public event AsyncCompletedEventHandler DetermineTrustCompleted;
#endif
	public event DeploymentProgressChangedEventHandler ProgressChanged;
	public event SynchronizeCompletedEventHandler SynchronizeCompleted;

}; // class DeploymentManager

}; // namespace System.Deployment
