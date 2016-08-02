/*
 * ActivationArguments.cs - Implementation of "System.ActivationArguments".
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

namespace System
{

#if CONFIG_FRAMEWORK_2_0

public sealed class ActivationArguments
{
	String fullName;
	String[] activationData;
	String[] applicationManifestPaths;

	public ActivationArguments(String fullName)
	{
		this.fullName = fullName;
		activationData = null;
		applicationManifestPaths = null;
	}

	public ActivationArguments(String fullName, String[] manifestPaths)
	{
		this.fullName = fullName;
		this.applicationManifestPaths = manifestPaths;
		this.activationData = null;
	}

	public ActivationArguments(String fullName, String[] manifestPaths,
												String[] activationData)
	{
		this.fullName = fullName;
		this.applicationManifestPaths = manifestPaths;
		this.activationData = activationData;
	}

	public String[] ActivationData
	{
		get
		{
			return this.activationData;
		}
		set
		{
			this.activationData = value;
		}
	}

	public String ApplicationFullName
	{
		get
		{
			return this.fullName;
		}
		set
		{
			if(value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.fullName = value;
		}
	}

	public String[] ApplicationManifestPaths
	{
		get
		{
			return this.applicationManifestPaths;
		}
		set
		{
			this.applicationManifestPaths = value;
		}
	}

}; // class ActivationArguments

#endif // CONFIG_FRAMEWORK_2_0

}; // namespace System
