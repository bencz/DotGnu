/*
 * internal.c - Lookup routines for "internalcall" methods.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#include "engine.h"
#include "lib_defs.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Suppress internal classes based on the configuration profile.
 */
#if !defined(IL_CONFIG_FP_SUPPORTED) || !defined(IL_CONFIG_EXTENDED_NUMERICS)
	#define	_IL_Math_suppressed
	#define	_IL_Single_suppressed
	#define	_IL_Double_suppressed
	#define	_IL_Decimal_suppressed
 	#define _IL_NumberFormatter_suppressed
#endif
#if !defined(IL_CONFIG_VARARGS)
	#define _IL_ArgIterator_suppressed
	#define _IL_TypedReference_suppressed
#endif
#if !defined(IL_CONFIG_NETWORKING)
	#define _IL_IPAddress_suppressed
	#define _IL_SocketMethods_suppressed
	#define _IL_DnsMethods_suppressed
#endif
#if !defined(IL_CONFIG_REFLECTION)
	#define _IL_AssemblyBuilder_suppressed
	#define _IL_ClrConstructor_suppressed
	#define _IL_ClrField_suppressed
	#define _IL_ClrHelpers_suppressed
	#define _IL_ClrMethod_suppressed
	#define _IL_ClrParameter_suppressed
	#define _IL_ClrProperty_suppressed
	#define _IL_FieldInfo_suppressed
	#define _IL_MethodBase_suppressed
	#define _IL_Module_suppressed
	#define _IL_ModuleBuilder_suppressed
	#define _IL_RuntimeMethodHandle_suppressed
	#define _IL_AssemblyBuilder_suppressed
	#define _IL_EventBuilder_suppressed
	#define _IL_FieldBuilder_suppressed
	#define _IL_FieldBuilder_suppressed
	#define _IL_PropertyBuilder_suppressed
	#define _IL_TypeBuilder_suppressed
	#define _IL_MethodBuilder_suppressed
	#define _IL_SignatureHelper_suppressed
	#define _IL_ParameterBuilder_suppressed
	#define _IL_FormatterServices_suppressed
#endif
#if !defined(IL_CONFIG_RUNTIME_INFRA) && !defined(IL_CONFIG_REFLECTION)
	#define	_IL_Type_suppressed
	#define	_IL_ClrType_suppressed
	#define	_IL_Assembly_suppressed
	#define	_IL_ClrResourceStream_suppressed
	#define	_IL_CodeTable_suppressed
#endif
#if !defined(IL_CONFIG_RUNTIME_INFRA)
	#define	_IL_GCHandle_suppressed
#endif
#if !defined(IL_CONFIG_DEBUG_LINES)
	#define _IL_Debugger_suppressed
	#define _IL_StackFrame_suppressed
#endif
#if !defined(IL_CONFIG_FILESYSTEM)
	#define _IL_FileMethods_suppressed
	#define _IL_DirMethods_suppressed
#endif
#if !defined(IL_CONFIG_PINVOKE)
	#define _IL_Marshal_suppressed
#endif

/*
 * Import the method tables of all internal classes.
 */
#include "int_table.c"

int _ILFindInternalCall(ILExecProcess *process,ILMethod *method, 
						int ctorAlloc, ILInternalInfo *info)
{
	ILImage *image;
	ILClass *owner;
	const char *name;
	const char *namespace;
	int left, right, middle;
	const ILMethodTableEntry *entry;
	ILEngineInternalClassList* internalClassList;
	ILType *signature;
	int isCtor;
	int cmp;

	/* If the method is not in a secure image, then this is
	   probably an attempt to circumvent system security, which
	   we don't allow */
	image = ILProgramItem_Image(method);
	if(!ILImageIsSecure(image))
	{
		goto runtimeOnly;
	}

	/* Find the method's owner and bail out if no namespace */
	owner = ILMethod_Owner(method);
	namespace = ILClass_Namespace(owner);
	if(!namespace)
	{
		goto runtimeOnly;
	}
	name = ILClass_Name(owner);

	/* Search for the class's internalcall table */
	left = 0;
	right = numInternalClasses - 1;
	while(left <= right)
	{
		middle = (left + right) / 2;
		cmp = strcmp(name, internalClassTable[middle].name);
		if(!cmp)
		{
			if(!strcmp(namespace, internalClassTable[middle].namespace))
			{
				/* Search for the method within the class's table */
				entry = internalClassTable[middle].entry;
				name = ILMethod_Name(method);
				signature = ILMethod_Signature(method);
				while(entry->methodName != 0)
				{
					if(!strcmp(entry->methodName, name) &&
					   entry->signature != 0 &&
					   _ILLookupTypeMatch(signature, entry->signature))
					{
						if(ctorAlloc && entry[1].methodName &&
						   !(entry[1].signature))
						{
							info->un.func = entry[1].func;
						#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
							info->marshal = entry[1].marshal;
						#endif
						}
						else
						{
							info->un.func = entry->func;
						#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
							info->marshal = entry->marshal;
						#endif
						}
						return 1;
					}
					++entry;
				}
			}
			return 0;
		}
		else if(cmp < 0)
		{
			right = middle - 1;
		}
		else
		{
			left = middle + 1;
		}
	}
	
	for(internalClassList=process->internalClassTable;internalClassList!=NULL;
					internalClassList=internalClassList->next)
	{
		/* Search for the local internalcall table */
		left = 0;
		right = internalClassList->size - 1;
		while(left <= right)
		{
			middle = (left + right) / 2;
			cmp = strcmp(name, internalClassList->list[middle].name);
			if(!cmp)
			{
				if(!strcmp(namespace, 
								internalClassList->list[middle].nspace))
				{
					/* Search for the method within the class's table */
					entry = internalClassList->list[middle].entry;
					name = ILMethod_Name(method);
					signature = ILMethod_Signature(method);
					while(entry->methodName != 0)
					{
						if(!strcmp(entry->methodName, name) &&
						   entry->signature != 0 &&
						   _ILLookupTypeMatch(signature, entry->signature))
						{
							if(ctorAlloc && entry[1].methodName &&
							   !(entry[1].signature))
							{
								info->un.func = entry[1].func;
							#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
								info->marshal = entry[1].marshal;
							#endif
							}
							else
							{
								info->un.func = entry->func;
							#if defined(IL_USE_CVM) && !defined(HAVE_LIBFFI)
								info->marshal = entry->marshal;
							#endif
							}
							return 1;
						}
						++entry;
					}
				}
				return 0;
			}
			else if(cmp < 0)
			{
				right = middle - 1;
			}
			else
			{
				left = middle + 1;
			}
		}
	}


	/* Perhaps this is a "runtime" method for an array or delegate? */
runtimeOnly:
	if(_ILGetInternalArray(method, &isCtor, info))
	{
		if(isCtor)
		{
			/* Arrays only have allocation constructors */
			if(ctorAlloc)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}
		else
		{
			/* This is a regular method */
			return 1;
		}
	}
	else if(_ILGetInternalDelegate(method, &isCtor, info))
	{
		if(isCtor)
		{
			/* Delegates only have allocation constructors */
			if(ctorAlloc)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}
		else
		{
			/* This is a regular method */
			return 1;
		}
	}

	/* The class does not have internalcall methods */
	return 0;
}

/*
 * Find an "internalcall" method from its address, which is
 * useful when debugging the virtual machine.
 */
const ILMethodTableEntry *_ILFindInternalByAddr(void *addr,
												const char **className)
{
	int index;
	const ILMethodTableEntry *entry;
	for(index = 0; index < numInternalClasses; ++index)
	{
		entry = internalClassTable[index].entry;
		while(entry->methodName != 0)
		{
			if(entry->func == addr)
			{
				*className = internalClassTable[index].name;
				return entry;
			}
			++entry;
		}
	}
	return 0;
}

#ifdef	__cplusplus
};
#endif
