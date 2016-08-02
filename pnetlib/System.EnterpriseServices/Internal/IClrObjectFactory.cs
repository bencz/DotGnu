/*
 * IClrObjectFactory.cs - Implementation of the
 *		"System.EnterpriseServices.Internal.IClrObjectFactory" class.
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

#if !ECMA_COMPAT
[Guid("ecabafd2-7f19-11d2-978e-0000f8757e2a")]
#endif
public interface IClrObjectFactory
{
	// Create an object in a remote assembly.
#if !ECMA_COMPAT
	[DispId(1)]
#endif
	Object CreateFromAssembly(String assembly, String type, String mode);

	// Create an object in a remote mailbox.
#if !ECMA_COMPAT
	[DispId(4)]
#endif
	Object CreateFromMailbox(String Mailbox, String Mode);

	// Create an object using a remote virtual root.
#if !ECMA_COMPAT
	[DispId(2)]
#endif
	Object CreateFromVroot(String VrootUrl, String Mode);

	// Create an object using a WSDL service description.
#if !ECMA_COMPAT
	[DispId(3)]
#endif
	Object CreateFromWsdl(String WsdlUrl, String Mode);

}; // interface IClrObjectFactory

}; // namespace System.EnterpriseServices.Internal
