/*
 * Accessibility.cs - Definitions for the "Accessibility" assembly.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

// Don't use this.  It is a really ugly COM interop assembly under Windows,
// and is provided for strict 100% compatibility only.

namespace Accessibility
{

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

#if CONFIG_COM_INTEROP

[Guid("618736E0-3C3D-11CF-810C-00AA00389B71")]
[TypeLibType(0x1050)]
public interface IAccessible
{
	[TypeLibFunc(0x0040)]
	[DispId(unchecked((int)0xFFFFEC66))]
	[MethodImpl(MethodImplOptions.InternalCall)]
	void accDoDefaultAction
			([In] [MarshalAs(UnmanagedType.Struct)] out Object varChild);

	[TypeLibFunc(0x0040)]
	[DispId(unchecked((int)0xFFFFEC67))]
	[MethodImpl(MethodImplOptions.InternalCall)]
	[return: MarshalAs(UnmanagedType.Struct)]
	Object accHitTest([In] int xLeft, [In] int yTop);

	[TypeLibFunc(0x0040)]
	[DispId(unchecked((int)0xFFFFEC69))]
	[MethodImpl(MethodImplOptions.InternalCall)]
	void accLocation
			(out int pxLeft, out int pyTop,
			 out int pcxWidth, out int pcyHeight,
			 [In] [Optional] [MarshalAs(UnmanagedType.Struct)]
			 	Object varChild);

	[TypeLibFunc(0x0040)]
	[DispId(unchecked((int)0xFFFFEC68))]
	[MethodImpl(MethodImplOptions.InternalCall)]
	[return: MarshalAs(UnmanagedType.Struct)]
	Object accNavigate
			([In] int navDir,
			 [In] [Optional] [MarshalAs(UnmanagedType.Struct)]
			 	Object varStart);

	[TypeLibFunc(0x0040)]
	[DispId(unchecked((int)0xFFFFEC6A))]
	[MethodImpl(MethodImplOptions.InternalCall)]
	void accSelect
			([In] int flagsSelect,
			 [In] [Optional] [MarshalAs(UnmanagedType.Struct)]
			 	Object varChild);

	[DispId(unchecked((int)0xFFFFEC76))]
	//[MethodImpl(MethodImplOptions.InternalCall)]
	//[return: MarshalAs(UnmanagedType.IDispatch)]
	[IndexerName("accChild")]
	Object this[[In] [MarshalAs(UnmanagedType.Struct)] Object varChild]
			{
				[TypeLibFunc(0x0040)]
				get;
			}
	
	[DispId(unchecked((int)0xFFFFEC77))]
	//[MethodImpl(MethodImplOptions.InternalCall)]
	int accChildCount { [TypeLibFunc(0x0040)] get; }

	[DispId(unchecked((int)0xFFFFEC6B))]
	//[MethodImpl(MethodImplOptions.InternalCall)]
	//[return: MarshalAs(UnmanagedType.BStr)]
	[IndexerName("accDefaultAction")]
	String this[[In] [Optional] [MarshalAs(UnmanagedType.Struct)]
					Object varChild]
			{
				[TypeLibFunc(0x0040)]
				get;
			}

	[DispId(unchecked((int)0xFFFFEC73))]
	//[MethodImpl(MethodImplOptions.InternalCall)]
	//[return: MarshalAs(UnmanagedType.BStr)]
	[IndexerName("accDescription")]
	String this[[In] [Optional] [MarshalAs(UnmanagedType.Struct)]
					Object varChild]
			{
				[TypeLibFunc(0x0040)]
				get;
			}

	[DispId(unchecked((int)0xFFFFEC77))]
	//[MethodImpl(MethodImplOptions.InternalCall)]
	//[return: MarshalAs(UnmanagedType.Struct)]
	Object accFocus { [TypeLibFunc(0x0040)] get; }

	[DispId(unchecked((int)0xFFFFEC70))]
	//[MethodImpl(MethodImplOptions.InternalCall)]
	//[return: MarshalAs(UnmanagedType.BStr)]
	[IndexerName("accHelp")]
	String this[[In] [Optional] [MarshalAs(UnmanagedType.Struct)]
					Object varChild]
			{
				[TypeLibFunc(0x0040)]
				get;
			}

	[TypeLibFunc(0x0040)]
	[DispId(unchecked((int)0xFFFFEC6F))]
	//[MethodImpl(MethodImplOptions.InternalCall)]
	int get_accHelpTopic([MarshalAs(UnmanagedType.BStr)] out String pszHelp,
			 			 [In] [Optional] [MarshalAs(UnmanagedType.Struct)]
			 			 Object varChild);

	[DispId(unchecked((int)0xFFFFEC6E))]
	//[MethodImpl(MethodImplOptions.InternalCall)]
	//[return: MarshalAs(UnmanagedType.BStr)]
	[IndexerName("accKeyboardShortcut")]
	String this[[In] [Optional] [MarshalAs(UnmanagedType.Struct)]
					Object varChild]
			{
				[TypeLibFunc(0x0040)]
				get;
			}

	[DispId(unchecked((int)0xFFFFEC75))]
	//[MethodImpl(MethodImplOptions.InternalCall)]
	//[return: MarshalAs(UnmanagedType.BStr)]
	[IndexerName("accName")]
	String this[[In] [Optional] [MarshalAs(UnmanagedType.Struct)]
					Object varChild]
			{
				[TypeLibFunc(0x0040)]
				get;
				[TypeLibFunc(0x0040)]
				set;
			}

	[DispId(unchecked((int)0xFFFFEC78))]
	//[MethodImpl(MethodImplOptions.InternalCall)]
	//[return: MarshalAs(UnmanagedType.IDispatch)]
	Object accParent { [TypeLibFunc(0x0040)] get; }

	[DispId(unchecked((int)0xFFFFEC72))]
	//[MethodImpl(MethodImplOptions.InternalCall)]
	//[return: MarshalAs(UnmanagedType.Struct)]
	[IndexerName("accRole")]
	Object this[[In] [Optional] [MarshalAs(UnmanagedType.Struct)]
					Object varChild]
			{
				[TypeLibFunc(0x0040)]
				get;
				[TypeLibFunc(0x0040)]
				set;
			}

	[DispId(unchecked((int)0xFFFFEC6C))]
	//[MethodImpl(MethodImplOptions.InternalCall)]
	//[return: MarshalAs(UnmanagedType.Struct)]
	Object accSelection { [TypeLibFunc(0x0040)] get; }

	[DispId(unchecked((int)0xFFFFEC71))]
	//[MethodImpl(MethodImplOptions.InternalCall)]
	//[return: MarshalAs(UnmanagedType.Struct)]
	[IndexerName("accState")]
	Object this[[In] [Optional] [MarshalAs(UnmanagedType.Struct)]
					Object varChild]
			{
				[TypeLibFunc(0x0040)]
				get;
				[TypeLibFunc(0x0040)]
				set;
			}

	[DispId(unchecked((int)0xFFFFEC74))]
	//[MethodImpl(MethodImplOptions.InternalCall)]
	//[return: MarshalAs(UnmanagedType.BStr)]
	[IndexerName("accValue")]
	String this[[In] [Optional] [MarshalAs(UnmanagedType.Struct)]
					Object varChild]
			{
				[TypeLibFunc(0x0040)]
				get;
				[TypeLibFunc(0x0040)]
				set;
			}

}; // interface IAccessible

[Guid("03022430-ABC4-11D0-BDE2-00AA001A1953")]
[InterfaceType(1)]
[TypeLibType(1)]
public interface IAccessibleHandler
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	void AccessibleObjectFromID
			([In] int hwnd, [In] int lObjectID,
			 [MarshalAs(UnmanagedType.Interface)]
			 	out IAccessible pIAccessible);

}; // interface IAccessibleHandler

#else // !CONFIG_COM_INTEROP

// The same as above, but without the horrible COM marker attributes.

using System;
using System.Runtime.CompilerServices;

public interface IAccessible
{
	void accDoDefaultAction(out Object varChild);
	Object accHitTest(int xLeft, int yTop);
	void accLocation
			(out int pxLeft, out int pyTop,
			 out int pcxWidth, out int pcyHeight,
		 	 Object varChild);
	Object accNavigate(int navDir, Object varStart);
	void accSelect(int flagsSelect, Object varChild);

	[IndexerName("accChild")]
	Object this[Object varChild] { get; }

	int accChildCount { get; }
	Object accFocus { get; }
	Object accParent { get; }
	Object accSelection { get; }

	[IndexerName("accDefaultAction")]
	String this[Object varChild] { get; }

	[IndexerName("accDescription")]
	String this[Object varChild] { get; }

	[IndexerName("accHelp")]
	String this[Object varChild] { get; }

	int get_accHelpTopic(out String pszHelp, Object varChild);

	[IndexerName("accKeyboardShortcut")]
	String this[Object varChild] { get; }

	[IndexerName("accName")]
	String this[Object varChild] { get; set; }

	[IndexerName("accRole")]
	Object this[Object varChild] { get; set; }

	[IndexerName("accState")]
	Object this[Object varChild] { get; set; }

	[IndexerName("accValue")]
	String this[Object varChild] { get; set; }

}; // interface IAccessible

public interface IAccessibleHandler
{
	void AccessibleObjectFromID(int hwnd, int lObjectID,
			 					out IAccessible pIAccessible);

}; // interface IAccessibleHandler

#endif // !CONFIG_COM_INTEROP

} // namespace Accessibility
