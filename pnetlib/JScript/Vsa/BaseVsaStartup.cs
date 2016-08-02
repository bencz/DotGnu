/*
 * BaseVsaStartup.cs - Startup/shutdown handling for vsa engines.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.Vsa
{

using System;

public abstract class BaseVsaStartup
{
	// Accessible internal state.
	protected IVsaSite site;

	// Set the site.
	public void SetSite(IVsaSite site)
			{
				this.site = site;
			}

	// Startup/shutdown this object.
	public abstract void Startup();
	public abstract void Shutdown();

}; // class BaseVsaStartup

}; // namespace Microsoft.Vsa
