/*
 * convert.c - Convert methods using a coder.
 *
 * Copyright (C) 2001, 2011  Southern Storm Software, Pty Ltd.
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

#include "engine_private.h"
#include "cvm_config.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Error codes for "_ILConvertMethod".
 */
#define	IL_CONVERT_OK				0
#define	IL_CONVERT_VERIFY_FAILED	1
#define	IL_CONVERT_ENTRY_POINT		2
#define	IL_CONVERT_NOT_IMPLEMENTED	3
#define	IL_CONVERT_OUT_OF_MEMORY	4
#define	IL_CONVERT_TYPE_INIT		5
#define	IL_CONVERT_DLL_NOT_FOUND	6

/*
 * Gererate code for functions where no il code is available.
 */
static unsigned char *GenMethod(ILCoder *coder, ILMethod *method,
								ILGenCodeFunc func,
							    int *errorCode, const char **errorInfo)
{
	int result;
	unsigned char *start;

	do
	{
		ILCoderExceptions coderExceptions;

		ILMemZero(&coderExceptions, sizeof(ILCoderExceptions));
		result = (*func)(coder, method, &start, &coderExceptions);
	} while (result == IL_CODER_END_RESTART);
	
	if(result != IL_CODER_END_OK)
	{
		*errorCode = IL_CONVERT_OUT_OF_MEMORY;
		return 0;
	}
	
	/* The method is converted now */
	*errorCode = IL_CONVERT_OK;
	return start;
}

#ifdef IL_USE_JIT

/*
 * Inner version of "_ILConvertMethod", which detects the type of
 * exception to throw, but does not throw it.
 * This method is invoked only during on demand compilation of a jitted IL method
 * and is secured through libjit.
 */
static unsigned char *ConvertMethod(ILExecThread *thread, ILMethod *method,
								    int *errorCode, const char **errorInfo)
{
	ILMethodCode code;
	ILCoder *coder = thread->process->coder;
	unsigned char *start;

#ifndef IL_CONFIG_VARARGS
	/* Vararg methods are not supported */
	if((ILMethod_CallConv(method) & IL_META_CALLCONV_MASK) ==
			IL_META_CALLCONV_VARARG)
	{
		METADATA_UNLOCK(_ILExecThreadProcess(thread));
		*errorCode = IL_CONVERT_NOT_IMPLEMENTED;
		return 0;
	}
#endif

	/* Get the method code */
	if(!ILMethodGetCode(method, &code))
	{
		code.code = 0;
	}

	/* The conversion is different depending upon whether the
	   method is written in IL or not */
	if(code.code)
	{
		/* Use the bytecode verifier and coder to convert the method */
		if(!_ILVerify(coder, &start, method, &code,
					  ILImageIsSecure(ILProgramItem_Image(method)), thread))
		{
			METADATA_UNLOCK(_ILExecThreadProcess(thread));
			*errorCode = IL_CONVERT_VERIFY_FAILED;
			return 0;
		}
	}
	else
	{
		ILJitMethodInfo *jitMethodInfo;

		jitMethodInfo = (ILJitMethodInfo *)method->userData;
		if(jitMethodInfo->fnInfo.flags == _IL_INTERNAL_GENCODE)
		{
			if(GenMethod(coder, method, jitMethodInfo->fnInfo.un.gen,
						 errorCode, errorInfo) == 0)
			{
				METADATA_UNLOCK(_ILExecThreadProcess(thread));
				*errorCode = IL_CONVERT_OUT_OF_MEMORY;
				return 0;
			}
		}
		else
		{
			/* All other cases should be handled in the jit coder. */

			METADATA_UNLOCK(_ILExecThreadProcess(thread));
			*errorCode = IL_CONVERT_OUT_OF_MEMORY;
			return 0;
		}
	}

	/* The method is converted now */
	*errorCode = IL_CONVERT_OK;
	return (unsigned char *)1;
}
#else
#ifdef IL_CONFIG_PINVOKE

/*
 * Locate or load an external module that is being referenced via "PInvoke".
 * Returns the system module pointer, or NULL if it could not be loaded.
 */
static void *LocateExternalModule(ILExecProcess *process, const char *name,
								  ILPInvoke *pinvoke)
{
	ILLoadedModule *loaded;
	char *pathname;

	/* Search for an already-loaded module with the same name */
	loaded = process->loadedModules;
	while(loaded != 0)
	{
		if(!ILStrICmp(loaded->name, name))
		{
			return loaded->handle;
		}
		loaded = loaded->next;
	}

	/* Create a new module structure.  We keep this structure even
	   if we cannot load the actual module.  This ensures that
	   future requests for the same module will be rejected without
	   re-trying the open */
	loaded = (ILLoadedModule *)ILMalloc(sizeof(ILLoadedModule) + strlen(name));
	if(!loaded)
	{
		return 0;
	}
	loaded->next = process->loadedModules;
	loaded->handle = 0;
	strcpy(loaded->name, name);
	process->loadedModules = loaded;

	/* Resolve the module name to a library name */
	pathname = ILPInvokeResolveModule(pinvoke);
	if(!pathname)
	{
		return 0;
	}

	/* Attempt to open the module */
	loaded->handle = ILDynLibraryOpen(pathname);
	ILFree(pathname);
	return loaded->handle;
}

#endif /* IL_CONFIG_PINVOKE */

/*
 * Inner version of "_ILConvertMethod", which detects the type of
 * exception to throw, but does not throw it.
 */
static unsigned char *ConvertMethod(ILExecThread *thread, ILMethod *method,
								    int *errorCode, const char **errorInfo)
{
	ILMethodCode code;
	ILPInvoke *pinv;
	ILCoder *coder = thread->process->coder;
	unsigned char *start;
	void *cif;
	void *ctorcif;
	int isConstructor;
#ifdef IL_CONFIG_PINVOKE
	ILModule *module;
	const char *name;
	void *moduleHandle;
#endif
	int result;
	ILInternalInfo fnInfo;
	ILInternalInfo ctorfnInfo;

	/* We need the metadata write lock */
	METADATA_WRLOCK(_ILExecThreadProcess(thread));

	/* Handle locked methods while cctors are executed. */
	if((start = (unsigned char *)ILCoderHandleLockedMethod(coder, method)))
	{
		METADATA_UNLOCK(_ILExecThreadProcess(thread));
		*errorCode = IL_CONVERT_OK;
		return start;
	}

	/* Is the method already converted? */
	if((start = (unsigned char *)(method->userData)) != 0)
	{
		METADATA_UNLOCK(_ILExecThreadProcess(thread));
		*errorCode = IL_CONVERT_OK;
		return start;
	}

#ifndef IL_CONFIG_VARARGS
	/* Vararg methods are not supported */
	if((ILMethod_CallConv(method) & IL_META_CALLCONV_MASK) ==
			IL_META_CALLCONV_VARARG)
	{
		METADATA_UNLOCK(_ILExecThreadProcess(thread));
		*errorCode = IL_CONVERT_NOT_IMPLEMENTED;
		return 0;
	}
#endif

	/* Make sure that we can lay out the method's class */
	if(!_ILLayoutClass(_ILExecThreadProcess(thread), ILMethod_Owner(method)))
	{
		METADATA_UNLOCK(_ILExecThreadProcess(thread));
		*errorCode = IL_CONVERT_TYPE_INIT;
		return 0;
	}

	/* Get the method code */
	if(!ILMethodGetCode(method, &code))
	{
		code.code = 0;
	}

	/* The conversion is different depending upon whether the
	   method is written in IL or not */
	if(code.code)
	{
		/* Use the bytecode verifier and coder to convert the method */
		if(!_ILVerify(coder, &start, method, &code,
					  ILImageIsSecure(ILProgramItem_Image(method)), thread))
		{
			METADATA_UNLOCK(_ILExecThreadProcess(thread));
			*errorCode = IL_CONVERT_VERIFY_FAILED;
			return 0;
		}
	}
	else
	{
		/* This is a "PInvoke", "internalcall", or "runtime" method */
		ILMemZero(&fnInfo, sizeof(ILInternalInfo));
		ILMemZero(&ctorfnInfo, sizeof(ILInternalInfo));
		pinv = ILPInvokeFind(method);
		isConstructor = ILMethod_IsConstructor(method);
		switch(method->implementAttrs &
					(IL_META_METHODIMPL_CODE_TYPE_MASK |
					 IL_META_METHODIMPL_INTERNAL_CALL |
					 IL_META_METHODIMPL_JAVA))
		{
			case IL_META_METHODIMPL_IL:
			case IL_META_METHODIMPL_OPTIL:
			{
				/* If we don't have a PInvoke record, then we don't
				   know what to map this method call to */
				if(!pinv)
				{
					METADATA_UNLOCK(_ILExecThreadProcess(thread));
					*errorCode = IL_CONVERT_ENTRY_POINT;
					return 0;
				}

			#ifdef IL_CONFIG_PINVOKE
				/* Find the module for the PInvoke record */
				module = ILPInvoke_Module(pinv);
				if(!module)
				{
					METADATA_UNLOCK(_ILExecThreadProcess(thread));
					*errorCode = IL_CONVERT_ENTRY_POINT;
					return 0;
				}
				name = ILModule_Name(module);
				if(!name || *name == '\0')
				{
					METADATA_UNLOCK(_ILExecThreadProcess(thread));
					*errorCode = IL_CONVERT_ENTRY_POINT;
					return 0;
				}
				moduleHandle = LocateExternalModule
									(thread->process, name, pinv);
				if(!moduleHandle)
				{
					METADATA_UNLOCK(_ILExecThreadProcess(thread));
					*errorCode = IL_CONVERT_DLL_NOT_FOUND;
					*errorInfo = name;
					return 0;
				}

				/* Get the name of the function within the module */
				name = ILPInvoke_Alias(pinv);
				if(!name || *name == '\0')
				{
					name = ILMethod_Name(method);
				}

			#ifdef IL_WIN32_PLATFORM

				if(!(pinv->member.attributes & IL_META_PINVOKE_NO_MANGLE))
				{
					/* We have to append an A or W to the function */
					/* name depending on the characterset used. */
					/* On Windows we have only either Ansi or Utf16 */
					int nameLength = strlen(name);
					ILUInt32 charSetUsed = ILPInvokeGetCharSet(pinv, method);
					char newName[nameLength + 2];

					strcpy(newName, name);
					if(charSetUsed == IL_META_MARSHAL_UTF16_STRING)
					{
						newName[nameLength] = 'W';
					}
					else
					{
						newName[nameLength] = 'A';
					}
					newName[nameLength + 1] = '\0';

					/* Look up the method within the module */
					fnInfo.un.func = ILDynLibraryGetSymbol(moduleHandle, newName);
				}
				if(!fnInfo.un.func)
				{
					/* Look up the method within the module */
					fnInfo.un.func = ILDynLibraryGetSymbol(moduleHandle, name);
				}
			#else	/* !IL_WIN32_PLATFORM */
				/* Look up the method within the module */
				fnInfo.un.func = ILDynLibraryGetSymbol(moduleHandle, name);
			#endif	/* !IL_WIN32_PLATFORM */
			#else /* !IL_CONFIG_PINVOKE */
				METADATA_UNLOCK(_ILExecThreadProcess(thread));
				*errorCode = IL_CONVERT_NOT_IMPLEMENTED;
				return 0;
			#endif /* IL_CONFIG_PINVOKE */
			}
			break;

			case IL_META_METHODIMPL_RUNTIME:
			case IL_META_METHODIMPL_IL | IL_META_METHODIMPL_INTERNAL_CALL:
			{
				/* "internalcall" and "runtime" methods must not
				   have PInvoke records associated with them */
				if(pinv)
				{
					METADATA_UNLOCK(_ILExecThreadProcess(thread));
					*errorCode = IL_CONVERT_VERIFY_FAILED;
					return 0;
				}

				/* Look up the internalcall function details */
				if(!_ILFindInternalCall(ILExecThreadGetProcess(thread),
										method, 0, &fnInfo))
				{
					if(isConstructor)
					{
						if(!_ILFindInternalCall(ILExecThreadGetProcess(thread),
												method, 1, &ctorfnInfo))
						{
							METADATA_UNLOCK(_ILExecThreadProcess(thread));
							*errorCode = IL_CONVERT_NOT_IMPLEMENTED;
							return 0;
						}
					}
					else
					{
						METADATA_UNLOCK(_ILExecThreadProcess(thread));
						*errorCode = IL_CONVERT_NOT_IMPLEMENTED;
						return 0;
					}
				}
				else if(isConstructor)
				{
					_ILFindInternalCall(ILExecThreadGetProcess(thread),
										method, 1, &ctorfnInfo);
				}
			}
			break;

			default:
			{
				/* No idea how to invoke this method */
				METADATA_UNLOCK(_ILExecThreadProcess(thread));
				*errorCode = IL_CONVERT_VERIFY_FAILED;
				return 0;
			}
			/* Not reached */
		}

		/* Bail out if we did not find the underlying native method */
		if(!(fnInfo.un.func) && !(ctorfnInfo.un.func))
		{
			METADATA_UNLOCK(_ILExecThreadProcess(thread));
			if(pinv)
				*errorCode = IL_CONVERT_ENTRY_POINT;
			else
				*errorCode = IL_CONVERT_NOT_IMPLEMENTED;
			return 0;
		}

		if(fnInfo.un.gen && fnInfo.flags == _IL_INTERNAL_GENCODE)
		{
			if((start = GenMethod(coder, method, fnInfo.un.gen,
								  errorCode, errorInfo)) == 0)
			{
				METADATA_UNLOCK(_ILExecThreadProcess(thread));
				*errorCode = IL_CONVERT_OUT_OF_MEMORY;
				return 0;
			}
		}
		else if(ctorfnInfo.un.gen && (ctorfnInfo.flags == _IL_INTERNAL_GENCODE))
		{
			if((start = GenMethod(coder, method, ctorfnInfo.un.gen,
								  errorCode, errorInfo)) == 0)
			{
				METADATA_UNLOCK(_ILExecThreadProcess(thread));
				*errorCode = IL_CONVERT_OUT_OF_MEMORY;
				return 0;
			}
		}
		else
		{
		#if defined(HAVE_LIBFFI)
			/* Generate a "cif" structure to handle the native call details */
			if(fnInfo.un.func)
			{
				/* Make the "cif" structure for the normal method entry */
				cif = _ILMakeCifForMethod(_ILExecThreadProcess(thread),
										  method, (pinv == 0));
				if(!cif)
				{
					METADATA_UNLOCK(_ILExecThreadProcess(thread));
					*errorCode = IL_CONVERT_OUT_OF_MEMORY;
					return 0;
				}
			}
			else
			{
				cif = 0;
			}
			if(ctorfnInfo.un.func)
			{
				/* Make the "cif" structure for the allocating constructor */
				ctorcif = _ILMakeCifForConstructor(_ILExecThreadProcess(thread),
													method, (pinv == 0));
				if(!ctorcif)
				{
					METADATA_UNLOCK(_ILExecThreadProcess(thread));
					*errorCode = IL_CONVERT_OUT_OF_MEMORY;
					return 0;
				}
			}
			else
			{
				ctorcif = 0;
			}
		#else
			/* Use the marshalling function pointer as the cif if no libffi */
			cif = fnInfo.marshal;
			ctorcif = ctorfnInfo.marshal;
		#endif

			/* Generate the coder stub for marshalling the call */
			if(!isConstructor)
			{
				/* We only need the method entry point */
				if(!ILCoderSetupExtern(coder, &start, method,
									   fnInfo.un.func, cif, (pinv == 0)))
				{
					METADATA_UNLOCK(_ILExecThreadProcess(thread));
					*errorCode = IL_CONVERT_OUT_OF_MEMORY;
					return 0;
				}
				while((result = ILCoderFinish(coder)) != IL_CODER_END_OK)
				{
					/* Do we need a coder restart due to cache overflow? */
					if(result != IL_CODER_END_RESTART)
					{
						METADATA_UNLOCK(_ILExecThreadProcess(thread));
						*errorCode = IL_CONVERT_OUT_OF_MEMORY;
						return 0;
					}
					if(!ILCoderSetupExtern(coder, &start, method,
										   fnInfo.un.func, cif, (pinv == 0)))
					{
						METADATA_UNLOCK(_ILExecThreadProcess(thread));
						*errorCode = IL_CONVERT_OUT_OF_MEMORY;
						return 0;
					}
				}
			}
			else
			{
				/* We need both the method and constructor entry points */
				if(!ILCoderSetupExternCtor(coder, &start, method,
									       fnInfo.un.func, cif,
										   ctorfnInfo.un.func, ctorcif,
										   (pinv == 0)))
				{
					METADATA_UNLOCK(_ILExecThreadProcess(thread));
					*errorCode = IL_CONVERT_OUT_OF_MEMORY;
					return 0;
				}
				while((result = ILCoderFinish(coder)) != IL_CODER_END_OK)
				{
					/* Do we need a coder restart due to cache overflow? */
					if(result != IL_CODER_END_RESTART)
					{
						METADATA_UNLOCK(_ILExecThreadProcess(thread));
						*errorCode = IL_CONVERT_OUT_OF_MEMORY;
						return 0;
					}
					if(!ILCoderSetupExternCtor(coder, &start, method,
										       fnInfo.un.func, cif,
											   ctorfnInfo.un.func, ctorcif,
											   (pinv == 0)))
					{
						METADATA_UNLOCK(_ILExecThreadProcess(thread));
						*errorCode = IL_CONVERT_OUT_OF_MEMORY;
						return 0;
					}
				}
			}
		}
	}

	/* The method is converted now */
	/* Run the needed cctors and unlock the metadata too */
	ILCoderRunCCtors(coder, start);
	*errorCode = IL_CONVERT_OK;
	return start;
}
#endif

unsigned char *_ILConvertMethod(ILExecThread *thread, ILMethod *method)
{
	ILObject *obj;
	const char *errorInfo = 0;
	int errorCode = IL_CONVERT_VERIFY_FAILED;
	unsigned char *start = ConvertMethod(thread, method, &errorCode, &errorInfo);
	if(start)
	{
		return start;
	}
	else
	{
		switch(errorCode)
		{
			case IL_CONVERT_VERIFY_FAILED:
			{
				_ILExecThreadSetException
					(thread, _ILSystemException(thread, 
						"System.Security.VerificationException"));
			}
			break;

			case IL_CONVERT_ENTRY_POINT:
			{
				_ILExecThreadSetException
					(thread, _ILSystemException(thread, 
						"System.EntryPointNotFoundException"));
			}
			break;

			case IL_CONVERT_NOT_IMPLEMENTED:
			{
				_ILExecThreadSetException
					(thread, _ILSystemException(thread, 
						"System.NotImplementedException"));
			}
			break;

			case IL_CONVERT_OUT_OF_MEMORY:
			{
				ILExecThreadThrowOutOfMemory(thread);
			}
			break;

			case IL_CONVERT_TYPE_INIT:
			{
				_ILExecThreadSetException
					(thread, _ILSystemException(thread, 
						"System.TypeInitializationException"));
			}
			break;

			case IL_CONVERT_DLL_NOT_FOUND:
			{
				obj = _ILSystemException(thread, "System.DllNotFoundException");

				if (errorInfo)
				{
					_ILSystemObjectSetField(thread, obj, "dllName",
						"oSystem.String;",
						(ILObject *)ILStringCreate(thread, errorInfo));
				}

				_ILExecThreadSetException(thread, obj);
			}
			break;
		}
		return start;
	}
}

#ifdef IL_CVM_DIRECT_UNROLLED

int _ILUnrollMethod(ILExecThread *thread, ILCoder *coder,
					unsigned char *pc, ILMethod *method)
{
	int result;
	METADATA_WRLOCK(_ILExecThreadProcess(thread));
	result = _ILCVMUnrollMethod(coder, pc, method);
	METADATA_UNLOCK(_ILExecThreadProcess(thread));
	return result;
}

#endif /* IL_CVM_DIRECT_UNROLLED */

#ifdef	__cplusplus
};
#endif
