/*
 * ClrObjectFactory.cs - Implementation of the
 *		"System.EnterpriseServices.Internal.ClrObjectFactory" class.
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

namespace System.EnterpriseServices.Internal
{

using System.Runtime.InteropServices;
using System.Security;

// This class is used by unmanaged COM+ code, which we don't support.

#if !ECMA_COMPAT
[Guid("ecabafd1-7f19-11d2-978e-0000f8757e2a")]
#endif
public class ClrObjectFactory : IClrObjectFactory
{
	// Constructor.
	public ClrObjectFactory() {}

	// Create an object in a remote assembly.
	public Object CreateFromAssembly(String assembly, String type, String mode)
			{
				throw new SecurityException();
			}

	// Create an object in a remote mailbox.
	public Object CreateFromMailbox(String Mailbox, String Mode)
			{
			#if CONFIG_COM_INTEROP
				throw new COMException();
			#elif !ECMA_COMPAT
				throw new ExternalException();
			#else
				throw new SystemException();
			#endif
			}

	// Create an object using a remote virtual root.
	public Object CreateFromVroot(String VrootUrl, String Mode)
			{
				throw new SecurityException();
			}

	// Create an object using a WSDL service description.
	public Object CreateFromWsdl(String WsdlUrl, String Mode)
			{
				throw new SecurityException();
			}

}; // class ClrObjectFactory

}; // namespace System.EnterpriseServices.Internal
