/*
 * corwin32.h - Emulate enough Win32 types to fool non-Win32 systems.
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

#ifndef	__CORWIN32_H__
#define	__CORWIN32_H__

#ifdef	__cplusplus
extern	"C"	{
#endif

/*
 * Determine if we are building on a real Win32 system or not.
 */
#if defined(_WIN32) || defined(WIN32) || defined(__CYGWIN__)
	#include <windows.h>
	#define	_COR_REAL_WIN32	1
#endif

#ifndef _COR_REAL_WIN32

/*
 * Get common definitions such as NULL and wchar_t.
 */
#include <stdio.h>
#include <stddef.h>
/*#include <wchar.h> -- not present on all platforms (TODO - fix later) */

/*
 * Include Windows error codes.
 */
#include "corwinerror.h"

/*
 * Defining API function prototypes.
 */
#ifndef	IN
#define	IN
#endif
#ifndef	OUT
#define	OUT
#endif
#ifndef	OPTIONAL
#define	OPTIONAL
#endif
#ifndef	PACKED
#define	PACKED
#endif
#ifndef	_stdcall
#define	_stdcall
#endif
#ifndef	__stdcall
#define	__stdcall
#endif
#ifndef	_cdecl
#define	_cdecl
#endif
#ifndef	__cdecl
#define	__cdecl
#endif
#ifndef	__declspec
#define	__declspec(x)
#endif
#ifndef	_declspec
#define	_declspec(x)
#endif
#ifndef	pascal
#define	pascal
#endif
#ifndef	_pascal
#define	_pascal
#endif
#ifndef	__pascal
#define	__pascal
#endif
#ifndef	PASCAL
#define	PASCAL	__pascal
#endif
#ifndef	CDECL
#define	CDECL	__cdecl
#endif
#ifndef	STDCALL
#define	STDCALL	__stdcall
#endif
#ifndef	WINAPI
#define	WINAPI	__stdcall
#endif
#ifndef	WINAPIV
#define	WINAPIV	__cdecl
#endif
#ifndef	APIENTRY
#define	APIENTRY __stdcall
#endif
#ifndef	CALLBACK
#define CALLBACK __stdcall
#endif
#ifndef	APIPRIVATE
#define APIPRIVATE __stdcall
#endif
#ifndef	DECLSPEC_IMPORT
#define	DECLSPEC_IMPORT
#endif
#ifndef	DECLSPEC_EXPORT
#define	DECLSPEC_EXPORT
#endif
#ifndef	DECLSPEC_NORETURN
#define	DECLSPEC_NORETURN
#endif
#ifndef	DECLARE_STDCALL_P
#define	DECLARE_STDCALL_P(type)	type __stdcall
#endif
#ifndef	_export
#define	_export
#endif
#ifndef	__export
#define	__export
#endif

/*
 * Define the basic Win32 types.
 */
#if !defined(NO_STRICT) && !defined(STRICT)
#define	STRICT	1
#endif
#ifndef	TRUE
#define	TRUE	1
#endif
#ifndef	FALSE
#define	FALSE	0
#endif
#ifndef	VOID
#define	VOID			void
#endif
#ifndef	CONST
#define	CONST			const
#endif
typedef int				WINBOOL, *PWINBOOL, *LPWINBOOL;
typedef int				BOOL, *PBOOL, *LPBOOL;
typedef char			CHAR, CCHAR, *PCHAR, *LPCHAR, *PCH, *LPCH;
typedef unsigned char	BYTE, *PBYTE, *LPBYTE;
typedef unsigned char	UCHAR, *PUCHAR, *LPUCHAR;
typedef unsigned short	WCHAR, *PWCHAR, *LPWCHAR, *PWCH, *LPWCH;
#ifdef UNICODE
typedef WCHAR			TCHAR, _TCHAR;
#else
typedef CHAR			TCHAR, _TCHAR;
#endif
typedef short			SHORT, *PSHORT, *LPSHORT;
typedef unsigned short	USHORT, *PUSHORT, *LPUSHORT;
typedef unsigned short	WORD, *PWORD, *LPWORD;
typedef unsigned long	DWORD, *PDWORD, *LPDWORD;
typedef long			LONG, *PLONG, *LPLONG;
typedef unsigned long	ULONG, *PULONG, *LPULONG;
typedef	int				INT, *PINT, *LPINT;
typedef unsigned int	UINT, *PUINT, *LPUINT;
typedef float			FLOAT, *PFLOAT, *LPFLOAT;
typedef void		   *PVOID, *LPVOID;
typedef CONST void	   *PCVOID, *LPCVOID;
typedef	UINT			WPARAM;
typedef LONG			LPARAM, LRESULT;
#ifndef	_HRESULT_DEFINED
#define	_HRESULT_DEFINED
typedef	LONG			HRESULT;
#endif
typedef WORD			ATOM;
typedef	void		   *HANDLE;
typedef	HANDLE		   *PHANDLE, *LPHANDLE;
#ifndef	STRICT
#define	DECLARE_HANDLE(name)	typedef struct name##__ { int i } *name
#else
#define	DECLARE_HANDLE(name)	typedef HANDLE name
#endif
typedef int (WINAPI *FARPROC)();
typedef int (WINAPI *NEARPROC)();
typedef int (WINAPI *PROC)(), *NPCSTR;
typedef char *PSZ;
typedef WCHAR *PWSTR, *LPWSTR, *NWPSTR;
typedef CONST WCHAR *PCWSTR, *LPCWSTR, *PCWCH, *LPCWCH;
typedef CHAR *PSTR, *LPSTR, *NPSTR;
typedef CONST CHAR *PCSTR, *LPCSTR;
typedef TCHAR TBYTE, *PTSTR, *LPTSTR, *PTCH, *LPTCH, *PTBYTE, *LP, *PTCHAR;
typedef CONST TCHAR *PCTSTR, *LPCTSTR;
typedef DWORD LCID, PLCID;
DECLARE_HANDLE(HINSTANCE);

/*
 * Types with specific sizes.
 */
typedef	int __int32;
typedef unsigned long ULONG32;

/*
 * Useful PE file section types.
 */
typedef struct _IMAGE_DATA_DIRECTORY
{
	DWORD	VirtualAddress;
	DWORD	Size;

} IMAGE_DATA_DIRECTORY;

/*
 * Manipulate words.
 */
#define	MAKEWORD(a,b)	((WORD)(((BYTE)(a)) |(((WORD)((BYTE)(b)))<<8)))
#define	MAKELONG(a,b)	((LONG)(((WORD)(a)) |(((DWORD)((WORD)(b)))<<16)))
#define	LOWORD(l)		((WORD)((DWORD)(l)))
#define	HIWORD(l)		((WORD)((((DWORD)(l)) >> 16) & 0xFFFF))
#define	LOBYTE(w)		((BYTE)((w)))
#define	HIBYTE(w)		((BYTE)((((WORD)(w)) >> 8) & 0xFF))

/*
 * Structured C exceptions.
 */
#define	__try
#define	__except(x)	if (0)
#define	__finally
#define	_try		__try
#define	_except(x)	__except((x))
#define	_finally	__finally
#define	EXCEPTION_MAXIMUM_PARAMETERS	15
typedef struct _EXCEPTION_RECORD
{
	DWORD ExceptionCode, ExceptionFlags;
	struct _EXCEPTION_RECORD *ExceptionRecord;
	PVOID ExceptionAddress;
	DWORD NumberParameters;
	DWORD ExceptionInformation[EXCEPTION_MAXIMUM_PARAMETERS];

} EXCEPTION_RECORD, *PEXCEPTION_RECORD;

/*
 * COM-style function and interface declarations.
 */
#ifndef	EXTERN_C
#ifdef	__cplusplus
#define	EXTERN_C	extern "C"
#else
#define	EXTERN_C	extern
#endif
#endif
#define	STDMETHODCALLTYPE		__stdcall
#define	STDMETHODVCALLTYPE		__cdecl
#define	STDAPICALLTYPE			__stdcall
#define	STDAPIVCALLTYPE			__cdecl
#define	STDAPI					EXTERN_C HRESULT STDAPICALLTYPE
#define	STDAPI_(type)			EXTERN_C type STDAPICALLTYPE
#define	STDAPIV					EXTERN_C HRESULT STDAPIVCALLTYPE
#define	STDAPIV_(type)			EXTERN_C type STDAPIVCALLTYPE
#define	STDMETHODIMP			HRESULT STDMETHODCALLTYPE
#define	STDMETHODIMP_(type)		type STDMETHODCALLTYPE
#define	STDMETHODIMPV			HRESULT STDMETHODVCALLTYPE
#define	STDMETHODIMPV_(type)	type STDMETHODVCALLTYPE
#define	interface				struct
#ifdef __cplusplus
#define	STDMETHOD(m)			virtual HRESULT STDMETHODCALLTYPE m
#define	STDMETHOD_(type,m)		virtual type STDMETHODCALLTYPE m
#define	PURE					=0
#define	THIS_
#define	THIS					void
#else
#define	STDMETHOD(m)			HRESULT (STDMETHODCALLTYPE *m)
#define	STDMETHOD_(type,m)		type (STDMETHODCALLTYPE *m)
#define	PURE
#define	THIS_					INTERFACE *,
#define	THIS					INTERFACE *
#ifndef	CONST_VTABLE
#define	CONST_VTABLE
#endif
#endif
#if defined(__cplusplus) && !defined(CINTERFACE)
#define	DECLARE_INTERFACE(i)	interface i
#define	DECLARE_INTERFACE_(i,b)	interface i : public b
#define	IENUM_THIS_(T)
#define	IENUM_THIS(T)
#else
#define	DECLARE_INTERFACE(i)	\
			typedef interface i { CONST_VTABLE struct i##Vtbl *lpVtbl; } i; \
			typedef CONST_VTABLE struct i##Vtbl i##Vtbl; \
			CONST_VTABLE struct i##Vtbl
#define	DECLARE_INTERFACE_(i,b)	DECLARE_INTERFACE(i)
#define	IENUM_THIS_(T)			T *,
#define	IENUM_THIS(T)			T *
#endif
#define	BEGIN_INTERFACE
#define	END_INTERFACE
#define	FWD_DECL(i)				typedef interface i i
#define	DECLARE_ENUMERATOR_(I,T)	\
DECLARE_INTERFACE_(I,IUnknown) \
{ \
	STDMETHOD(QueryInterface)(IENUM_THIS_(I) REFIID, PVOID*) PURE; \
	STDMETHOD_(ULONG,AddRef)(IENUM_THIS(I)) PURE; \
	STDMETHOD_(ULONG,Release)(IENUM_THIS(I)) PURE; \
	STDMETHOD(Next)(IENUM_THIS_(I) ULONG, T*, ULONG*) PURE; \
	STDMETHOD(Skip)(IENUM_THIS_(I) ULONG) PURE; \
	STDMETHOD(Reset)(IENUM_THIS(I)) PURE; \
	STDMETHOD(Clone)(IENUM_THIS_(I) I**) PURE; \
}
#define	DECLARE_ENUMERATOR(T)	DECLARE_ENUMERATOR_(IEnum##T,T)
#ifndef	_GUID_DEFINED
#define	_GUID_DEFINED
typedef struct _GUID
{
	unsigned long Data1;
	unsigned short Data2, Data3;
	unsigned char Data4[8];

} GUID, *REFGUID, *LPGUID;
#endif
#ifndef	UUID_DEFINED
#define	UUID_DEFINED
typedef	GUID UUID;
#endif
typedef GUID IID, CLSID, *LPCLSID, *LPIID, *REFIID, *REFCLSID;
typedef GUID FMTID, *REFFMTID;
#define	uuid_t	UUID
typedef unsigned long PROPID;
#ifndef _REFGUID_DEFINED
#if defined(__cplusplus) && !defined(CINTERFACE)
#define	REFGUID		const GUID &
#define	REFIID		const IID &
#define	REFCLSID	const CLSID &
#else
#define	REFGUID		const GUID * const
#define	REFIID		const IID * const
#define	REFCLSID	const CLSID * const
#endif
#define	_REFGUID_DEFINED
#define	_REFGIID_DEFINED
#define	_REFCLSID_DEFINED
#endif
#ifndef	GUID_SECTION
#define	GUID_SECTION		".text"
#endif
#ifdef __GNUC__
#define	GUID_SECT			__attribute__((section(GUID_SECTION)))
#else
#define	GUID_SECT
#endif
#if !defined(INITGUID) || defined(__cplusplus)
#define	GUID_EXT			EXTERN_C
#else
#define	GUID_EXT
#endif
#ifdef	INITGUID
#define	DEFINE_GUID(n,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8)	\
	GUID_EXT const GUID n GUID_SECT = {l,w1,w2,{b1,b2,b3,b4,b5,b6,b7,b8}}
#else
#define	DEFINE_GUID(n,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8)	\
	GUID_EXT const GUID n
#endif
#define	DEFINE_OLEGUID(n,l,w1,w2) DEFINE_GUID(n,l,w1,w2,0xC0,0,0,0,0,0,0,0x46)
#define	EXTERN_GUID(n,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8)	\
	GUID_EXT const GUID n
#define	RESULT(x)		((HRESULT)(x))

/*
 * Create an instance of a COM-like class.
 */
STDMETHODIMP CoCreateInstance(const CLSID *rclsid, const IID *riid, void **ppv);

#endif /* !_COR_REAL_WIN32 */

#ifdef	__cplusplus
};
#endif

#endif /* __CORWIN32_H__ */
