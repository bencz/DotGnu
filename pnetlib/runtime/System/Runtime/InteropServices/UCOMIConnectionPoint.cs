/*
 * UCOMIConnectionPoint.cs - Implementation of the
 *			"System.Runtime.InteropServices.UCOMIConnectionPoint" class.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_COM_INTEROP

[Guid("B196B286-BAB4-101A-B69C-00AA00341D07")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[ComImport]
#if CONFIG_FRAMEWORK_1_2
[Obsolete("Use the class in System.Runtime.InteropServices.ComTypes instead")]
#endif
public interface UCOMIConnectionPoint
{
	void Advise(Object pUnkSink, out int pdwCookie);
	void EnumConnections(out UCOMIEnumConnections ppEnum);
	void GetConnectionInterface(out Guid pIID);
	void GetConnectionPointContainer(out UCOMIConnectionPointContainer ppCPC);
	void Unadvise(int dwCookie);

}; // class UCOMIConnectionPoint

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
