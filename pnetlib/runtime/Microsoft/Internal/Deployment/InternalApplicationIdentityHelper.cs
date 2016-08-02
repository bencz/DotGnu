/*
 * InternalApplicationIdentityHelper.cs - Implementation of the
 *	"Microsoft.Internal.Deployment.InternalApplicationIdentityHelper" class.
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

namespace Microsoft.Internal.Deployment
{

#if CONFIG_WIN32_SPECIFICS && CONFIG_FRAMEWORK_2_0

using System;

public abstract sealed class InternalApplicationIdentityHelper
{
	// Constructor.
	private InternalApplicationIdentityHelper() {}

	// Get the internal application identity.
	public static Object GetInternapAppId(ApplicationIdentity id)
			{
				return id.id;
			}

}; // class InternalApplicationIdentityHelper

#endif // CONFIG_WIN32_SPECIFICS && CONFIG_FRAMEWORK_2_0

}; // namespace Microsoft.Internal.Deployment
