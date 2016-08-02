/*
 * ITypeLibImporterNotifySink.cs - Implementation of the
 *			"System.Runtime.InteropServices.ITypeLibImporterNotifySink" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

using System.Reflection;

[Guid("F1C3BF76-C3E4-11d3-88E7-00902754C43A")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ITypeLibImporterNotifySink
{

	// Report an event that occurred while converting an assembly.
	void ReportEvent(ImporterEventKind eventKind,
					 int eventCode, String eventMsg);

	// Resolve a type library reference to an assembly.
	Assembly ResolveRef(Object typeLib);

}; // interface ITypeLibImporterNotifySink

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
