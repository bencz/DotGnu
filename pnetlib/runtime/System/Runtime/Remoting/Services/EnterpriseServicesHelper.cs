/*
 * EnterpriseServicesHelper.cs - Implementation of the
 *			"System.Runtime.Remoting.Services.EnterpriseServicesHelper" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.Remoting.Services
{

#if CONFIG_REMOTING

using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Proxies;

// This class is intended to be used by "System.EnterpriseServices.dll"
// to help with wrapping COM objects for remoting.  Since we don't
// actually support COM in this implementation, we just stub the class.

public sealed class EnterpriseServicesHelper
{
	// Create a return message for a constructor call.
	public static IConstructionReturnMessage CreateConstructionReturnMessage
				(IConstructionCallMessage ctorMsg, MarshalByRefObject retObj)
			{
				// Not used in this implementation.
				return null;
			}

	// Switch proxy wrappers.
	public static void SwitchWrappers(RealProxy oldcp, RealProxy newcp)
			{
				// Not used in this implementation.
			}

	// Wrap an unmanaged IUnknown instance with a COM object.
	public static Object WrapIUnknownWithComObject(IntPtr punk)
			{
				// Not used in this implementation.
				return null;
			}

}; // class EnterpriseServicesHelper

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Services
