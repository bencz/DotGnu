/*
 * corole2.h - Emulate enough OLE2 infrastructure to fool non-Win32 systems.
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

#ifndef	__COROLE2_H__
#define	__COROLE2_H__

#include "corwin32.h"

#ifdef	__cplusplus
extern	"C"	{
#endif

#ifdef _COR_REAL_WIN32

#include <ole2.h>

#else /* !_COR_REAL_WIN32 */

/*
 * Declare the IUnknown interface.
 */
EXTERN_C const IID IID_IUnknown;
#ifndef	__IUnknown_INTERFACE_DEFINED__
#define	__IUnknown_INTERFACE_DEFINED__
#undef INTERFACE
#define INTERFACE	IUnknown
DECLARE_INTERFACE(IUnknown)
{
	STDMETHOD(QueryInterface)(THIS_ REFIID, PVOID *) PURE;
	STDMETHOD_(ULONG,AddRef)(THIS) PURE;
	STDMETHOD_(ULONG,Release)(THIS) PURE;
};
typedef IUnknown *LPUNKNOWN;
#endif

/*
 * Declare the IClassFactory interface.
 */
EXTERN_C const IID IID_IClassFactory;
#ifndef	__IClassFactory_INTERFACE_DEFINED__
#define	__IClassFactory_INTERFACE_DEFINED__
#undef INTERFACE
#define INTERFACE	IClassFactory
DECLARE_INTERFACE_(IClassFactory,IUnknown)
{
	STDMETHOD(QueryInterface)(THIS_ REFIID, PVOID *) PURE;
	STDMETHOD_(ULONG,AddRef)(THIS) PURE;
	STDMETHOD_(ULONG,Release)(THIS) PURE;
	STDMETHOD(CreateInstance)(THIS_ LPUNKNOWN, REFIID, PVOID *) PURE;
	STDMETHOD(LockServer)(THIS_ BOOL) PURE;
};
typedef IClassFactory *LPCLASSFACTORY;
#endif

/*
 * C interface for calling methods on IUnknown and IClassFactory.
 */
#ifdef	COBJMACROS
#define	IUnknown_QueryInterface(T,r,O)	\
			(T)->lpVtbl->QueryInterface((T),(r),(O))
#define	IUnknown_AddRef(T)	(T)->lpVtbl->AddRef((T))
#define	IUnknown_Release(T)	(T)->lpVtbl->Release((T))
#define	IClassFactory_QueryInterface(T,r,O)	\
			(T)->lpVtbl->QueryInterface((T),(r),(O))
#define	IClassFactory_AddRef(T)		(T)->lpVtbl->AddRef((T))
#define	IClassFactory_Release(T)	(T)->lpVtbl->Release((T))
#define	IClassFactory_CreateInstance(T,p,r,O)	\
			(T)->lpVtbl->CreateInstance((T),(p),(r),(O))
#define	IClassFactory_LockServer(T,f)	\
			(T)->lpVtbl->LockServer((T),(f))
#endif

#endif /* !_COR_REAL_WIN32 */

/*
 * Determine how to force a definition inline in C++ code.
 */
#if defined(__GNUC__)
	#define	_cor_inline		inline
#else
	#ifndef FORCEINLINE
		#if _MSC_VER < 1200
			#define _cor_inline inline
		#else
			#define _cor_inline __forceinline
		#endif
	#else
		#define	_cor_inline FORCEINLINE
	#endif
#endif

#ifdef	__cplusplus
};
#endif

#endif /* __COROLE2_H__ */
