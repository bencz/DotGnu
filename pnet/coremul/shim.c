/*
 * shim.c - Implementation of the runtime switching shim.
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

#include "cor.h"

#ifdef __cplusplus
extern "C" {
#endif

/*
 * Get the direct CLR engine's module instance handle.
 */
static HINSTANCE GetDirectEngine(void)
{
#ifdef _COR_REAL_WIN32
	static HINSTANCE hInst = 0;

	/* TODO: lock the access to "hInst" */

	/* Return the cached module instance handle, if present */
	if(hInst)
	{
		return hInst;
	}

	/* TODO: find the direct engine and load it */
	return hInst;
#else
	/* The direct CLR engine API is not available on this platform */
	return 0;
#endif
}

/*
 * Stub out "GetProcAddress" on platforms that don't use the direct CLR.
 */
#ifndef _COR_REAL_WIN32
static void *GetProcAddress(HINSTANCE hInst, const char *name)
{
	return 0;
}
#endif

/*
 * Main entry point for IL applications.  This gets the name of the
 * calling program, determines which runtime engine should be used to
 * execute it, and then launches that engine.
 */
__int32 STDCALL _CorExeMain(void)
{
	HINSTANCE hInst;
	void *func;

	/* Get the full pathname of the application */
	/* TODO */

	/* Determine which engine to use for the application */
	/* TODO */

	/* TODO: overlay this process with the engine if it is non-direct */

	/* Find the direct engine and call its "_CorExeMain" function */
	hInst = GetDirectEngine();
	if(hInst)
	{
		func = GetProcAddress(hInst, "_CorExeMain");
		if(func)
		{
			return (*((__int32 (*)(void))func))();
		}
	}
	return 0;
}

/*
 * The rest of the "mscoree.dll" API's are stubbed out to pass
 * through to the underlying "direct" implementation, if any.
 * This will only happen for native applications that directly
 * host the direct CLR, or for DLL's that were previously loaded
 * by the direct CLR.  For non-direct CLR's such as Portable.Net,
 * Mono, or Rotor, we always bypass the following code.
 */

#define	COR_PASS_THROUGH(rettype,name,params,args,defreturn)	\
	rettype STDCALL name params \
	{ \
		HINSTANCE __hInst = GetDirectEngine(); \
		void *func; \
		if(__hInst) \
		{ \
			func = GetProcAddress(__hInst, #name); \
			if(func) \
			{ \
				return (*((rettype (*) params)func)) args; \
			} \
		} \
		return defreturn; \
	}
#define	COR_PASS_THROUGH_VOID(name,params,args)	\
	void STDCALL name params \
	{ \
		HINSTANCE __hInst = GetDirectEngine(); \
		void *func; \
		if(__hInst) \
		{ \
			func = GetProcAddress(__hInst, #name); \
			if(func) \
			{ \
				(*((void (*) params)func)) args; \
			} \
		} \
	}

COR_PASS_THROUGH(HRESULT, CoInitializeEE, (DWORD fFlags),
				 (fFlags), RESULT(E_NOTIMPL))
COR_PASS_THROUGH_VOID(CoUninitializeEE, (BOOL fFlags), (fFlags))
COR_PASS_THROUGH(BOOL, _CorDllMain,
				 (HINSTANCE hInst, DWORD dwReason, LPVOID lpReserved),
				 (hInst, dwReason, lpReserved), 0)
COR_PASS_THROUGH(__int32, _CorExeMain2,
				 (PBYTE pUnmappedPE, DWORD cUnmappedPE,
				  LPWSTR pImageNameIn, LPWSTR pLoadersFileName,
				  LPWSTR pCmdLine),
				 (pUnmappedPE, cUnmappedPE, pImageNameIn,
				  pLoadersFileName, pCmdLine), 0)
COR_PASS_THROUGH(HRESULT, _CorValidateImage,
				 (PVOID *ImageBase, LPCSTR FileName),
				 (ImageBase, FileName), RESULT(E_NOTIMPL))
COR_PASS_THROUGH_VOID(_CorImageUnloading, (PVOID ImageBase), (ImageBase))
COR_PASS_THROUGH(HRESULT, CoInitializeCor,
				 (DWORD fFlags), (fFlags), RESULT(E_NOTIMPL))
COR_PASS_THROUGH_VOID(CoUninitializeCor, (void), ())
COR_PASS_THROUGH_VOID(AddDestructorCallback,
					  (int code, TDestructorCallback cb), (code, cb))

#ifdef __cplusplus
};
#endif
