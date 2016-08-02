/*
 * ICalls.cs - Substitute for Original ICalls set
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

/*
This file exists simply to provide an alternate implementation for
Mono's Internal Calls in System.Web
*/

namespace System.Web.Util 
{
	internal class ICalls 
	{
		private ICalls() 
		{
			// nothing here
		}

		// TODO :replace with internal calls to InfoMethods here
		static public String GetMachineConfigPath()
		{
			return "";
		}

		// TODO :replace with internal calls to InfoMethods here
		static public String GetMachineInstallDirectory()
		{
			return "";
		}
	}
}
