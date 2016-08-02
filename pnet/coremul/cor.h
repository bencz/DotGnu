/*
 * cor.h - Emulation of the "mscoree.dll" interface.
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

#ifndef	_COR_H_
#define	_COR_H_

/*
 * Include the OLE2 definitions that we need to do COM-like things.
 */
#include "corole2.h"

/*
 * Include the common header structures and definitions.
 */
#include "corhdr.h"

/*
 * Define the C API's.
 */
#ifdef __cplusplus
extern "C" {
#endif

/*
 * Define the name of the execution engine.
 */
#ifdef _COR_REAL_WIN32
	#define	MSCOREE_SHIM_W		L"mscoree.dll"
	#define	MSCOREE_SHIM_A		"mscoree.dll"
#else
	#define	MSCOREE_SHIM_W		L"libmscoree.so"
	#define	MSCOREE_SHIM_A		"libmscoree.so"
#endif

/*
 * Flags for "CoInitializeEE".
 */
typedef enum tagCOINITEE
{
	COINITEE_DEFAULT = 0,
	COINITEE_DLL     = 1,
	COINITEE_MAIN    = 2

} COINITIEE;

/*
 * Flags for "CoUninitializeEE".
 */
typedef enum tagCOUNINITEE
{
	COUNINITEE_DEFAULT = 0,
	COUNINITEE_DLL     = 1

} COUNINITIEE;

/*
 * Flags for "CoInitializeCor".
 */
typedef enum tagCOINITCOR
{
	COINITCOR_DEFAULT = 0

} COINITICOR;

/*
 * Initialize the execution engine.
 */
STDAPI CoInitializeEE(DWORD fFlags);

/*
 * Uninitialize the execution engine.
 */
STDAPI_(void) CoUninitializeEE(BOOL fFlags);

/*
 * Shutdown the COM routines that are used by the execution engine.
 */
STDAPI_(void) CoEEShutDownCOM(void);

/*
 * Entry point into the library that is called by a ".dll".
 */
BOOL STDMETHODCALLTYPE _CorDllMain
		(HINSTANCE hInst, DWORD dwReason, LPVOID lpReserved);

/*
 * Entry point into the library that is called by a ".exe".
 */
__int32 STDMETHODCALLTYPE _CorExeMain(void);

/*
 * Internal version of the ".exe" entry point.
 */
__int32 STDMETHODCALLTYPE _CorExeMain2(PBYTE  pUnmappedPE,
									   DWORD  cUnmappedPE,
									   LPWSTR pImageNameIn,
									   LPWSTR pLoadersFileName,
									   LPWSTR pCmdLine);

/*
 * Validate an image.  Returns a HRESULT.
 */
STDAPI _CorValidateImage(PVOID *ImageBase, LPCSTR FileName);

/*
 * Notify the library of an image unload.
 */
STDAPI_(void) _CorImageUnloading(PVOID ImageBase);

/*
 * Higher-level common language runtime initialization.
 */
STDAPI CoInitializeCor(DWORD fFlags);

/*
 * Higher-level common language runtime termination.
 */
STDAPI_(void) CoUninitializeCor(void);

/*
 * Add a destructor callback for a specific CPU exception code.
 */
typedef void (*TDestructorCallback)(EXCEPTION_RECORD *);
STDAPI_(void) AddDestructorCallback(int code, TDestructorCallback cb);

#ifdef __cplusplus
};
#endif

/*
 * Define the C++ API's.
 */
#ifdef __cplusplus

/*
 * Determine if an element type is a modifier.
 */
_cor_inline int CorIsModifierElementType(CorElementType type)
{
	if(type == ELEMENT_TYPE_BYREF || type == ELEMENT_TYPE_PTR)
	{
		return 1;
	}
	else
	{
		return (type & ELEMENT_TYPE_MODIFIER);
	}
}

/*
 * Determine if an element type is primitive.
 */
_cor_inline int CorIsPrimitiveType(CorElementType type)
{
	return (type < ELEMENT_TYPE_PTR);
}

/*
 * Get the uncompressed size of a metadata-encoded value.
 */
inline ULONG CorSigUncompressDataSize(PBYTE value)
{
	register int lead = value[0];
	if((lead & 0x80) == 0)
	{
		return 1;
	}
	else if((lead & 0xC0) == 0x80)
	{
		return 2;
	}
	else
	{
		return 4;
	}
}

#endif /* __cplusplus */

#endif /* _COR_H */
