/*
 * ITypeInfo2.cs - Implementation of the
 *			"System.Runtime.InteropServices.ComTypes.ITypeInfo2" class.
 *
 * Copyright (C) 2003, 2004  Southern Storm Software, Pty Ltd.
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

[Guid("00020412-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[ComImport]
public interface ITypeInfo2
{
	void AddressOfMember(int memid, INVOKEKIND invKind, out IntPtr ppv);
	void CreateInstance(Object pUnkOuter, ref Guid riid, out Object ppvObj);
	void GetAllCustData(IntPtr pCustData);
	void GetAllFuncCustData(int index, IntPtr pCustData);
	void GetAllImplTypeCustData(int index, IntPtr pCustData);
	void GetAllParamCustData(int indexFunc, int indexParam, IntPtr pCustData);
	void GetAllVarCustData(int index, IntPtr pCustData);
	void GetContainingTypeLib(out ITypeLib ppTLB, out int pIndex);
	void GetCustData(ref Guid guid, out Object pVarVal);
	void GetDllEntry(int memid, INVOKEKIND invKind, out String pBstrDllName,
					 out String pBstrName, out short pwOrdinal);
	void GetDocumentation(int index, out String strName,
						  out String strDocString, out int dwHelpContext,
						  out String strHelpFile);
	[LCIDConversion(1)]
	void GetDocumentation2(int memid, out String pbstrHelpString,
						   out int pdwHelpStringContext,
						   out String pbstrHelpStringDll);
	void GetFuncCustData(int index, ref Guid guid, out Object pVarVal);
	void GetFuncDesc(int index, out IntPtr ppFuncDesc);
	void GetFuncIndexOfMemId(int memid, INVOKEKIND invKind, out int pFuncIndex);
	void GetIDsOfNames(String[] rgszNames, int cNames, int[] pMemId);
	void GetImplTypeCustData(int index, ref Guid guid, out Object pVarVal);
	void GetImplTypeFlags(int index, out int pImplTypeFlags);
	void GetMops(int memid, out String pBstrMops);
	void GetNames(int memid, String[] rgBstrNames, int cMaxNames,
				  out int pcNames);
	void GetParamCustData(int indexFunc, int indexParam, ref Guid guid,
						  out Object pVarVal);
	void GetRefTypeInfo(int hRef, out ITypeInfo ppTI);
	void GetRefTypeOfImplType(int index, out int href);
	void GetTypeAttr(out IntPtr ppTypeAttr);
	void GetTypeComp(out ITypeComp ppTComp);
	void GetTypeFlags(out int pTypeFlags);
	void GetTypeKind(out TYPEKIND pTypeKind);
	void GetVarCustData(int index, ref Guid guid, out Object pVarVal);
	void GetVarDesc(int index, out IntPtr ppVarDesc);
	void GetVarIndexOfMemId(int memid, out int pVarIndex);
	void Invoke(Object pvInstance, int memid, short wFlags,
				ref DISPPARAMS pDispParams, out Object pVarResult,
				out EXCEPINFO pExcepInfo, out int puArgErr);
	void ReleaseFuncDesc(IntPtr pFuncDesc);
	void ReleaseTypeAttr(IntPtr pTypeAttr);
	void ReleaseVarDesc(IntPtr pVarDesc);

}; // class ITypeInfo2

#endif // CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2

}; // namespace System.Runtime.InteropServices.ComTypes
