/*
 * ApplicationDeployment.cs - Implementation of the
 *		"System.Deployment.ApplicationDeployment" class.
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
using System.ComponentModel;

public class ApplicationDeployment
{
	// Internal state.
	private bool cancellationPending;
	private DateTime lastCheckForUpdateTime;
	private Version runningVersion;
	private String updatedApplicationFullName;
	private Version updatedVersion;
	private String updateLocation;

	// Cannot instantiate this class directly.
	private ApplicationDeployment()
			{
				cancellationPending = false;
				lastCheckForUpdateTime = DateTime.MinValue;
				runningVersion = new Version();
				updatedApplicationFullName = null;
				updatedVersion = new Version();
				updateLocation = null;
			}

	// Determine if there is a cancellation pending.
	public bool CancellationPending
			{
				get
				{
					return cancellationPending;
				}
			}

	// Get the current application deployment.
	public static ApplicationDeployment CurrentDeployment
			{
				get
				{
					// We only support standalone ClickOnce applications.
					// See the README for further details.
					throw new InvalidDeploymentException();
				}
			}

	// Determine if the current application was "deployed".
	public static bool IsNetworkDeployed
			{
				get
				{
					// We only support standalone ClickOnce applications.
					// See the README for further details.
					return false;
				}
			}

	// Get the last time that we checked for an update.
	public DateTime LastCheckForUpdateTime
			{
				get
				{
					return lastCheckForUpdateTime;
				}
			}

	// Get the version of the deployed application that is currently running.
	public Version RunningVersion
			{
				get
				{
					return runningVersion;
				}
			}

	// Get the full updated application name.
	public String UpdatedApplicationFullName
			{
				get
				{
					return updatedApplicationFullName;
				}
			}

	// Get the version of the most recently downloaded update.
	public Version UpdatedVersion
			{
				get
				{
					return updatedVersion;
				}
			}

	// Get the location from which the application updates itself.
	public String UpdateLocation
			{
				get
				{
					return updateLocation;
				}
			}

	// Stub out the update API's.  Because "CurrentDeployment" always throws
	// an exception, there is no way for the user to call these methods.

	// Check to see if there is a new update available.
	public bool CheckForUpdate()
			{
				throw new NotImplementedException();
			}

	// Check asynnchronously for a new update.
	public void CheckForUpdateAsync()
			{
				throw new NotImplementedException();
			}

	// Cancel a pending update check.
	public void CheckForUpdateAsyncCancel()
			{
				throw new NotImplementedException();
			}

	// Download the files in a particular group.
	public void DownloadFiles(String groupName)
			{
				throw new NotImplementedException();
			}

	// Download the files in a particular group asynchronously.
	public void DownloadFilesAsync(String groupName)
			{
				DownloadFilesAsync(groupName, null);
			}
	public void DownloadFilesAsync(String groupName, Object userState)
			{
				throw new NotImplementedException();
			}

	// Cancel an asynchronous file download.
	public void DownloadFilesAsyncCancel(String groupName)
			{
				throw new NotImplementedException();
			}

	// Get the update check information.
	public UpdateCheckInfo GetUpdateCheckInfo()
			{
				throw new NotImplementedException();
			}

	// Update this application.
	public bool Update()
			{
				throw new NotImplementedException();
			}

	// Update this application asynchronously.
	public void UpdateAsync()
			{
				throw new NotImplementedException();
			}

	// Cancel an asynchronous update.
	public void UpdateAsyncCancel()
			{
				throw new NotImplementedException();
			}

	// Events that may be emitted by this class.
	public event CheckForUpdateCompletedEventHandler CheckForUpdateCompleted;
	public event DownloadFilesCompletedEventHandler DownloadFilesCompleted;
	public event DeploymentProgressChangedEventHandler ProgressChanged;
#if CONFIG_FRAMEWORK_2_0
	public event AsyncCompletedEventHandler UpdateCompleted;
#endif

}; // class ApplicationDeployment

}; // namespace System.Deployment
