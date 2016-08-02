/*
 * UCOMITypeInfo.cs - Implementation of the
 *			"System.Runtime.InteropServices.UCOMITypeInfo" class.
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

[Guid("00020401-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[ComImport]
#if CONFIG_FRAMEWORK_1_2
[Obsolete("Use the class in System.Runtime.InteropServices.ComTypes instead")]
#endif
public interface UCOMITypeInfo
{
	void AddressOfMember(int memid, INVOKEKIND invKind, out IntPtr ppv);
	void CreateInstance(Object pUnkOuter, ref Guid riid, out Object ppvObj);
	void GetContainingTypeLib(out UCOMITypeLib ppTLB, out int pIndex);
	void GetDllEntry(int memid, INVOKEKIND invKind, out String pBstrDllName,
					 out String pBstrName, out short pwOrdinal);
	void GetDocumentation(int index, out String strName,
						  out String strDocString, out int dwHelpContext,
						  out String strHelpFile);
	void GetFuncDesc(int index, out IntPtr ppFuncDesc);
	void GetIDsOfNames(String[] rgszNames, int cNames, int[] pMemId);
	void GetImplTypeFlags(int index, out int pImplTypeFlags);
	void GetMops(int memid, out String pBstrMops);
	void GetNames(int memid, String[] rgBstrNames, int cMaxNames,
				  out int pcNames);
	void GetRefTypeInfo(int hRef, out UCOMITypeInfo ppTI);
	void GetRefTypeOfImplType(int index, out int href);
	void GetTypeAttr(out IntPtr ppTypeAttr);
	void GetTypeComp(out UCOMITypeComp ppTComp);
	void GetVarDesc(int index, out IntPtr ppVarDesc);
	void Invoke(Object pvInstance, int memid, short wFlags,
				ref DISPPARAMS pDispParams, out Object pVarResult,
				out EXCEPINFO pExcepInfo, out int puArgErr);
	void ReleaseFuncDesc(IntPtr pFuncDesc);
	void ReleaseTypeAttr(IntPtr pTypeAttr);
	void ReleaseVarDesc(IntPtr pVarDesc);

}; // class UCOMITypeInfo

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
