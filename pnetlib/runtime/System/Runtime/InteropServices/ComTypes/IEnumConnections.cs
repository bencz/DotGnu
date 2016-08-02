/*
 * IEnumConnections.cs - Implementation of the
 *		"System.Runtime.InteropServices.ComTypes.IEnumConnections" class.
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

namespace System.Runtime.InteropServices.ComTypes
{

#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2

[Guid("B196B287-BAB4-101A-B69C-00AA00341D07")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[ComImport]
public interface IEnumConnections
{
	void Clone(out IEnumConnections ppenum);
	int Next(int celt, CONNECTDATA[] rgelt, out int pceltFetched);
	void Reset();
	int Skip(int celt);

}; // class IEnumConnections

#endif // CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2

}; // namespace System.Runtime.InteropServices.ComTypes
