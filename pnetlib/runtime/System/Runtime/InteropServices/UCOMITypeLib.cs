/*
 * UCOMITypeLib.cs - Implementation of the
 *			"System.Runtime.InteropServices.UCOMITypeLib" class.
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

[Guid("00020402-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[ComImport]
#if CONFIG_FRAMEWORK_1_2
[Obsolete("Use the class in System.Runtime.InteropServices.ComTypes instead")]
#endif
public interface UCOMITypeLib
{
	void FindName(String szNameBuf, int lHashVal, UCOMITypeInfo[] ppTInfo,
				  int[] rgMemId, ref short pcFound);
	void GetDocumentation(int index, out String strName,
						  out String strDocString, out int dwHelpContext,
						  out String strHelpFile);
	void GetLibAttr(out IntPtr ppTLibAttr);
	void GetTypeComp(out UCOMITypeComp ppTComp);
	void GetTypeInfo(int index, out UCOMITypeInfo ppTI);
	int GetTypeInfoCount();
	void GetTypeInfoOfGuid(ref Guid guid, out UCOMITypeInfo ppTI);
	void GetTypeInfoType(int index, out TYPEKIND pTKind);
	bool IsName(String szNameBuf, int lHashVal);
	void ReleaseTLibAttr(IntPtr pTLibAttr);

}; // class UCOMITypeLib

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
