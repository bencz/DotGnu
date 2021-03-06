/*
 * CheckForUpdateCompletedEventArgs.cs - Implementation of the
 *		"System.Deployment.CheckForUpdateCompletedEventArgs" class.
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

public class CheckForUpdateCompletedEventArgs : AsyncCompletedEventArgs
{
	// Internal state.
	private Version availableVersion;
	private bool isUpdateRequired;
	private Version minimumRequiredVersion;
	private bool updateAvailable;
	private long updateSize;

	// Constructor.
	internal CheckForUpdateCompletedEventArgs
				(Exception error, bool cancelled, Object userState,
				 Version availableVersion, bool isUpdateRequired,
				 Version minimumRequiredVersion, bool updateAvailable,
				 long updateSize)
			: base(error, cancelled, userState)
			{
				this.availableVersion = availableVersion;
				this.isUpdateRequired = isUpdateRequired;
				this.minimumRequiredVersion = minimumRequiredVersion;
				this.updateAvailable = updateAvailable;
				this.updateSize = updateSize;
			}

	// Get this object's properties.
	public Version AvailableVersion
			{
				get
				{
					return availableVersion;
				}
			}
	public bool IsUpdateRequired
			{
				get
				{
					return isUpdateRequired;
				}
			}
	public Version MinimumRequiredVersion
			{
				get
				{
					return minimumRequiredVersion;
				}
			}
	public bool UpdateAvailable
			{
				get
				{
					return updateAvailable;
				}
			}
	public long UpdateSize
			{
				get
				{
					return updateSize;
				}
			}

}; // class CheckForUpdateCompletedEventArgs

}; // namespace System.Deployment
