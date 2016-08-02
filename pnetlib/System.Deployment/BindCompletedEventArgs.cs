/*
 * BindCompletedEventArgs.cs - Implementation of the
 *		"System.Deployment.BindCompletedEventArgs" class.
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

#if CONFIG_FRAMEWORK_2_0

using System;
using System.ComponentModel;

public class BindCompletedEventArgs : AsyncCompletedEventArgs
{
	// Internal state.
	private ActivationContext activationContext;
	private String friendlyName;
	private bool isCached;

	// Constructor.
	internal BindCompletedEventArgs
				(Exception error, bool cancelled, Object userState,
				 ActivationContext activationContext, String friendlyName,
				 bool isCached)
			: base(error, cancelled, userState)
			{
				this.activationContext = activationContext;
				this.friendlyName = friendlyName;
				this.isCached = isCached;
			}

	// Get this object's properties.
	public ActivationContext ActivationContext
			{
				get
				{
					return activationContext;
				}
			}
	public String FriendlyName
			{
				get
				{
					return friendlyName;
				}
			}
	public bool IsCached
			{
				get
				{
					return isCached;
				}
			}

}; // class BindCompletedEventArgs

#endif // CONFIG_FRAMEWORK_2_0

}; // namespace System.Deployment
