/*
 * DeploymentProgressChangedEventArgs.cs - Implementation of the
 *		"System.Deployment.DeploymentProgressChangedEventArgs" class.
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

public class DeploymentProgressChangedEventArgs : ProgressChangedEventArgs
{
	// Internal state.
	private long bytesCompleted;
	private long bytesTotal;
	private String group;
	private DeploymentProgressState state;

	// Constructor.
	internal DeploymentProgressChangedEventArgs
				(int progressPercentage, Object userState,
				 long bytesCompleted, long bytesTotal,
				 String group, DeploymentProgressState state)
			: base(progressPercentage, userState)
			{
				this.bytesCompleted = bytesCompleted;
				this.bytesTotal = bytesTotal;
				this.group = group;
				this.state = state;
			}

	// Get this object's properties.
	public long BytesCompleted
			{
				get
				{
					return bytesCompleted;
				}
			}
	public long BytesTotal
			{
				get
				{
					return bytesTotal;
				}
			}
	public String Group
			{
				get
				{
					return group;
				}
			}
	public DeploymentProgressState State
			{
				get
				{
					return state;
				}
			}

}; // class DeploymentProgressChangedEventArgs

}; // namespace System.Deployment
