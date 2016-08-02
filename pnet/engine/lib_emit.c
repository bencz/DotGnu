/*
 * lib_emit.c - Internalcall methods for the "Reflection.Emit" classes.
 *
 * Copyright (C) 2002, 2003, 2008, 2009  Southern Storm Software, Pty Ltd.
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
#include "lib_defs.h"
#if HAVE_SYS_TYPES_H
	#include <sys/types.h>
#endif
#if HAVE_SYS_STAT_H
	#include <sys/stat.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

#ifdef IL_CONFIG_REFLECTION

/*
 * private static IntPtr ClrAssemblyCreate(String name, int v1, int v2,
 *                                         int v3, int v4,
 *                                         AssemblyBuilderAccess access,
 *                                         out IntPtr writer);
 */
ILNativeInt _IL_AssemblyBuilder_ClrAssemblyCreate(ILExecThread *_thread,
                                                  ILString *name, ILInt32 v1,
                                                  ILInt32 v2, ILInt32 v3,
                                                  ILInt32 v4, ILInt32 access,
                                                  ILNativeInt *writerReturn)
{
	const char *utf8Name;
	ILContext *context;
	ILImage *image;
	ILWriter *writer;
	int createdContext;
	ILAssembly *retval;

	/* Convert the name into a UTF8 string */
	utf8Name = ILStringToUTF8(_thread, name);
	if(!utf8Name)
	{
		return 0;
	}

	/* Lock the metadata system while we do this */
	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	/* Determine which context to use: internal or external */
	if((access & 1) != 0)
	{
		/* The assembly needs to be runnable in the current domain */
		context = _thread->process->context;
		createdContext = 0;
	}
	else
	{
		/* Create a new context outside the domain for a "Save" assembly */
		context = ILContextCreate();
		if(!context)
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			ILExecThreadThrowOutOfMemory(_thread);
			return 0;
		}
		createdContext = 1;
	}

	/* Create a new ILImage structure for the assembly */
	image = ILImageCreate(context);
	if(!image)
	{
		if(createdContext)
		{
			ILContextDestroy(context);
		}
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}

	/* Create a new ILWriter structure for the assembly.  We assume
	   that we are building a DLL, until we know otherwise later */
	writer = ILWriterCreate(0, 0, IL_IMAGETYPE_DLL, 0);
	if(!writer)
	{
		ILImageDestroy(image);
		if(createdContext)
		{
			ILContextDestroy(context);
		}
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}

	/* Create the initial ILAssembly structure in the image */
	if(!(retval=ILAssemblyCreate(image, 0, utf8Name, 0)))
	{
		ILWriterDestroy(writer);
		ILImageDestroy(image);
		if(createdContext)
		{
			ILContextDestroy(context);
		}
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}

	ILAssemblySetVersionSplit(retval, v1, v2, v3, v4);

	/* Unlock and return the information to the caller */
	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	*writerReturn = (ILNativeInt)writer;
	return (ILNativeInt)retval;
}

/*
 * private static bool ClrSave(IntPtr assembly, IntPtr writer, String path,
 *                             IntPtr entryMethod, PEFileKinds fileKind);
 */
ILBool _IL_AssemblyBuilder_ClrSave(ILExecThread *_thread, ILNativeInt _assembly,
                                   ILNativeInt _writer, ILString *_path,
                                   ILNativeInt _entryMethod, ILInt32 fileKind)
{
	/* TODO */
	ILProgramItem *item;
	ILImage *image;
	ILWriter *writer;
	ILMethod *entryMethod;
	const char *path;
	FILE *stream;
	int tmp;
	int needChmod;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	item = (ILProgramItem *)_assembly;
	image = ILProgramItem_Image(item);
	writer = (ILWriter *)_writer;
	entryMethod = (ILMethod *)_entryMethod;
	if (!(path = (const char *)ILStringToPathname(_thread, _path)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	if (!(stream = fopen(path, "wb")))
	{
		if (!(stream = fopen(path, "w")))
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			return 0;
		}
	}
	ILWriterSetStream(writer, stream, 1);
	/* this has to be kept in sync with PEFileKinds */
	needChmod = 0;
	switch (fileKind)
	{
		case 1: /* PEFileKinds.Dll */
		{
			ILWriterResetTypeAndFlags(writer,
			                          IL_IMAGETYPE_DLL,
			                          IL_WRITEFLAG_SUBSYS_CUI);
		}
		break;

		case 2: /* PEFileKinds.ConsoleApplication */
		{
			ILWriterResetTypeAndFlags(writer,
			                          IL_IMAGETYPE_EXE,
			                          IL_WRITEFLAG_SUBSYS_CUI);
			needChmod = 1;
		}
		break;

		case 3: /* PEFileKinds.WindowsApplication */
		{
			ILWriterResetTypeAndFlags(writer,
			                          IL_IMAGETYPE_EXE,
			                          IL_WRITEFLAG_SUBSYS_GUI);
			needChmod = 1;
		}
		break;
	}
	if (entryMethod)
	{
		ILWriterSetEntryPoint(writer, entryMethod);
	}
	ILWriterOutputMetadata(writer, image);
	if (!(tmp = ILWriterDestroy(writer)))
	{
		if ((fclose(stream)))
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			return 0;
		}
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		return 0;
	}
	else if (tmp == -1)
	{
		if ((fclose(stream)))
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			return 0;
		}
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	if ((fclose(stream)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		return 0;
	}
#if !(defined(WIN32) || defined(_WIN32) || defined(__CYGWIN__))
	if(needChmod)
	{
		int mask = umask(0);
		umask(mask);
		chmod(path, 0777 & ~mask);
	}
#endif

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILBool)1;
}
/*
 * private static int ClrWriteMethod(IntPtr assembly,
 *                                   IntPtr writer,
 *                                   byte[] header,
 *                                   byte[] code,
 *                                   IntPtr[] codeFixupPtrs,
 *                                   int[] codeFixupOffsets,
 *                                   byte[][] exceptionBlocks,
 *                                   IntPtr[] exceptionBlockFixupPtrs,
 *                                   int[] exceptionBlockFixupOffsets);
 */
ILInt32 _IL_AssemblyBuilder_ClrWriteMethod(ILExecThread *_thread,
                                           ILNativeInt _assembly,
                                           ILNativeInt _writer,
                                           System_Array *_header,
                                           System_Array *_code,
                                           System_Array *_codeFixupPtrs,
                                           System_Array *_codeFixupOffsets,
                                           System_Array *_exceptionBlocks,
                                           System_Array *_exceptionBlockFixupPtrs,
                                           System_Array *_exceptionBlockFixupOffsets)
{
	ILInt32 retval;
	ILWriter *writer;
	ILInt32 rva;
	ILNativeInt *ptrs;
	ILInt32 *offsets;
	System_Array **eBlocks;
	unsigned char *buf;
	unsigned long i, len, length;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	/* align to the next 4-byte boundary and get the starting rva */
	writer = (ILWriter *)_writer;
	ILWriterTextAlign(writer);
	retval = (ILInt32)ILWriterGetTextRVA(writer);

	/* write out the header */
	buf = (unsigned char *)ArrayToBuffer(_header);
	length = (unsigned long)ArrayLength(_header);
	ILWriterTextWrite(writer, buf, length);

	/* get the rva of the code section */
	rva = (ILInt32)ILWriterGetTextRVA(writer);

	/* write out the code section */
	buf = (unsigned char *)ArrayToBuffer(_code);
	length = (unsigned long)ArrayLength(_code);
	ILWriterTextWrite(writer, buf, length);

	/* register token fixups for the code section */
	if (_codeFixupPtrs && _codeFixupOffsets)
	{
		ptrs = (ILNativeInt *)ArrayToBuffer(_codeFixupPtrs);
		offsets = (ILInt32 *)ArrayToBuffer(_codeFixupOffsets);
		length = (unsigned long)ArrayLength(_codeFixupPtrs);
		for (i = 0; i < length; ++i)
		{
			ILWriterSetFixup(writer, rva+offsets[i], (ILProgramItem *)ptrs[i]);
		}
	}

	/* if there are no exception blocks, we're done */
	if (!_exceptionBlocks)
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		return retval;
	}

	/* align to the next 4-byte boundary */
	ILWriterTextAlign(writer);

	/* get the rva of the exception block section */
	rva = (ILInt32)ILWriterGetTextRVA(writer);

	/* write out the exception block section */
	eBlocks = (System_Array **)ArrayToBuffer(_exceptionBlocks);
	len = (unsigned long)ArrayLength(_exceptionBlocks);
	for (i = 0; i < len; ++i)
	{
		buf = (unsigned char *)ArrayToBuffer(eBlocks[i]);
		length = (unsigned long)ArrayLength(eBlocks[i]);
		ILWriterTextWrite(writer, buf, length);
	}

	/* register token fixups for the exception block section */
	if (_exceptionBlockFixupPtrs && _exceptionBlockFixupOffsets)
	{
		ptrs = (ILNativeInt *)ArrayToBuffer(_exceptionBlockFixupPtrs);
		offsets = (ILInt32 *)ArrayToBuffer(_exceptionBlockFixupOffsets);
		length = (unsigned long)ArrayLength(_exceptionBlockFixupPtrs);
		for (i = 0; i < length; ++i)
		{
			ILWriterSetFixup(writer, rva+offsets[i], (ILProgramItem *)ptrs[i]);
		}
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return retval;
}

/*
 * internal static int ClrGetItemToken(IntPtr item);
 */
ILInt32 _IL_AssemblyBuilder_ClrGetItemToken(ILExecThread *_thread,
                                            ILNativeInt item)
{
	if(item)
	{
		return ILProgramItem_Token((ILProgramItem *)item);
	}
	else
	{
		return 0;
	}
}

/*
 * internal static IntPtr GetItemFromToken(IntPtr assembly, int token);
 */
ILNativeInt _IL_AssemblyBuilder_ClrGetItemFromToken(ILExecThread *_thread,
                                                    ILNativeInt assembly,
                                                    ILInt32 token)
{
	if(assembly)
	{
		return (ILNativeInt)ILProgramItem_FromToken
			(ILProgramItem_Image((ILAssembly *)assembly), (ILToken)token);
	}
	else
	{
		return 0;
	}
}

/*
 * private static IntPtr ClrAttributeCreate(IntPtr assembly, IntPtr ctor,
 *											byte[] blob);
 */
ILNativeInt _IL_AssemblyBuilder_ClrAttributeCreate
		(ILExecThread * _thread, ILNativeInt assembly,
		 ILNativeInt ctor, System_Array * blob)
{
	if(assembly && blob)
	{
		ILMember * member = ILProgramItemToMember((ILProgramItem*)ctor);
		ILAttribute *attr = ILAttributeCreate
			(ILProgramItem_Image((ILAssembly *)assembly), 0);
		member = ILMemberImport(ILProgramItem_Image((ILAssembly *) assembly), member);
		if(!attr)
		{
			return 0;
		}
		ILAttributeSetType(attr, ILToProgramItem(member));
		if(!ILAttributeSetValue(attr, ArrayToBuffer(blob),
								(ILUInt32)(ArrayLength(blob))))
		{
			return 0;
		}
		return (ILNativeInt)attr;
	}
	else
	{
		return 0;
	}
}

/*
 * private static void ClrAttributeAddToItem(IntPtr item, IntPtr attribute);
 */
void _IL_AssemblyBuilder_ClrAttributeAddToItem
		(ILExecThread * _thread, ILNativeInt item, ILNativeInt attribute)
{
	if(item && attribute)
	{
		ILProgramItemAddAttribute
			((ILProgramItem *)item, (ILAttribute *)attribute);
		ILProgramItemConvertAttrs((ILProgramItem *)item);
	}
}

/*
 * private static IntPtr ClrEventCreate(IntPtr classInfo, String name,
 *                                      IntPtr type, EventAttributes attrs);
 */
ILNativeInt _IL_EventBuilder_ClrEventCreate(ILExecThread *_thread,
                                            ILNativeInt classInfo,
                                            ILString *name, ILNativeInt type,
                                            ILInt32 attrs)
{
	ILEvent *retval;
	ILClass *info;
	ILClass *typeInfo;
	const char *str;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	info = (ILClass *)classInfo;
	typeInfo = (ILClass *)type;
	if (!(str = (const char *)ILStringToAnsi(_thread, name)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	if (!(retval = ILEventCreate(info, 0, str, (ILUInt32)attrs, typeInfo)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILNativeInt)retval;
}

/*
 * private static ClrEventAddSemantics(IntPtr eventInfo,
 *                                     MethodSemanticsAttributes attr,
 *                                     MethodToken token);
 */
void _IL_EventBuilder_ClrEventAddSemantics(ILExecThread *_thread,
                                           ILNativeInt eventInfo,
                                           ILInt32 attr, void *token)
{
	ILProgramItem *item;
	ILImage *image;
	ILToken tok;
	ILMethod *method;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	item = (ILProgramItem *)eventInfo;
	image = ILProgramItem_Image(item);
	tok = *((ILToken *)token);
	if (!(method = ILMethod_FromToken(image,tok)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}
	if (!(ILMethodSemCreate(item, 0, (ILUInt32)attr, method)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
}

/*
 * private static IntPtr ClrFieldCreate(IntPtr classInfo, String name,
 *                                      IntPtr type, FieldAttributes attrs);
 */
ILNativeInt _IL_FieldBuilder_ClrFieldCreate(ILExecThread *_thread,
                                            ILNativeInt classInfo,
                                            ILString *name, ILNativeInt type,
                                            ILInt32 attrs)
{
	ILField *retval;
	ILClass *info;
	ILType *sig;
	const char *str;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	info = (ILClass *)classInfo;
	sig = (ILType *)type;
	if (!(str = (const char *)ILStringToAnsi(_thread, name)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	if (!(retval = ILFieldCreate(info, 0, str, (ILUInt32)attrs)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	ILMemberSetSignature((ILMember *)retval, sig);

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILNativeInt)retval;
}

/*
 * private static void ClrFieldSetConstant(IntPtr item, Object value);
 */
void _IL_FieldBuilder_ClrFieldSetConstant(ILExecThread *_thread,
                                          ILNativeInt item,
                                          ILObject *value)
{
	ILImage *image;
	ILProgramItem *owner;
	ILField *field;
	ILType *type;
	ILType *valueType;
	ILUInt32 elemType;
	ILConstant *constant;
	ILExecValue blob;
	ILUInt32 len;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	owner = (ILProgramItem *)item;
	image = ILProgramItem_Image(owner);
	field = (ILField *)item;
	type = ILTypeStripPrefixes(ILField_Type(field));
	valueType = ILClassToType(GetObjectClass(value));
	type = ILTypeGetEnumType(type);
	valueType = ILTypeGetEnumType(valueType);
	if (!(ILTypeAssignCompatible(image, valueType, type)))
	{
		return;
	}
	elemType = ILType_ToElement(type);

	/* handle null objects */
	if (!value)
	{
		if (!(constant = ILConstantCreate(image, 0, owner, IL_META_ELEMTYPE_CLASS)))
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			ILExecThreadThrowOutOfMemory(_thread);
			return;
		}
		blob.int32Value = (ILInt32)0;
		if (!(ILConstantSetValue(constant, &blob, 4)))
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			ILExecThreadThrowOutOfMemory(_thread);
			return;
		}
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		return;
	}

	switch(elemType)
	{
		case IL_META_ELEMTYPE_BOOLEAN:
		case IL_META_ELEMTYPE_I1:
		case IL_META_ELEMTYPE_U1:
		case IL_META_ELEMTYPE_CHAR:
		case IL_META_ELEMTYPE_I2:
		case IL_META_ELEMTYPE_U2:
		case IL_META_ELEMTYPE_I4:
		case IL_META_ELEMTYPE_U4:
		case IL_META_ELEMTYPE_R4:
		case IL_META_ELEMTYPE_I8:
		case IL_META_ELEMTYPE_U8:
		case IL_META_ELEMTYPE_R8:
		case IL_META_ELEMTYPE_I:
		case IL_META_ELEMTYPE_U:
		case IL_META_ELEMTYPE_R:
		{
			len = (ILUInt32)ILSizeOfType(_thread, type);
		}
		break;

		case IL_META_ELEMTYPE_STRING:
		{
			ILUInt16 *chars;
			ILUInt16 *ptr;
			ILUInt32 i;

			len = (((System_String *)value)->length);
			if (!(blob.ptrValue = (ILUInt16 *)ILMalloc(len*2)))
			{
				IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
				ILExecThreadThrowOutOfMemory(_thread);
				return;
			}
			chars = StringToBuffer(value);
			ptr = (ILUInt16 *)blob.ptrValue;
			for (i = 0; i < len; ++i, ++ptr)
			{
				IL_WRITE_UINT16(ptr, chars[i]);
			}
			if (!(constant = ILConstantCreate(image, 0, owner, elemType)))
			{
				ILFree(blob.ptrValue);
				IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
				ILExecThreadThrowOutOfMemory(_thread);
				return;
			}
			if (!(ILConstantSetValue(constant, blob.ptrValue, len*2)))
			{
				ILFree(blob.ptrValue);
				IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
				ILExecThreadThrowOutOfMemory(_thread);
				return;
			}
			ILFree(blob.ptrValue);
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		}
		return;

		default:
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		}
		return;
	}
	if (!(ILExecThreadUnbox(_thread, type, value, &blob)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		return;
	}
	if (!(constant = ILConstantCreate(image, 0, owner, elemType)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}
	if (!(ILConstantSetValue(constant, &blob, len)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
}

/*
 * private static void ClrFieldSetMarshal(IntPtr item, byte[] data);
 */
void _IL_FieldBuilder_ClrFieldSetMarshal(ILExecThread *_thread,
                                         ILNativeInt item,
                                         System_Array *data)
{
	ILFieldMarshal *marshal;
	ILProgramItem *owner;
	ILImage *image;
	ILUInt8 *blob;
	unsigned long length;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	owner = (ILProgramItem *)item;
	image = ILProgramItem_Image(owner);
	blob = (ILUInt8 *)ArrayToBuffer(data);
	length = (unsigned long)ArrayLength(data);
	if (!(marshal = ILFieldMarshalCreate(image, 0, owner)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}
	if (!(ILFieldMarshalSetType(marshal, blob, length)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
}

/*
 * private static void ClrFieldSetOffset(IntPtr item, int offset);
 */
void _IL_FieldBuilder_ClrFieldSetOffset(ILExecThread *_thread,
                                        ILNativeInt item, ILInt32 offset)
{
	ILFieldLayout *layout;
	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));
	if(item)
	{
		layout = ILFieldLayoutGetFromOwner((ILField *)item);
		if(layout)
		{
			ILFieldLayoutSetOffset(layout, (ILUInt32)offset);
		}
		else
		{
			if(!ILFieldLayoutCreate(ILProgramItem_Image(item), 0,
									(ILField *)item, (ILUInt32)offset))
			{
				IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
				ILExecThreadThrowOutOfMemory(_thread);
			}
		}
	}
	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
}

/*
 * private static void ClrFieldSetRVA(IntPtr item, int rva);
 */
void _IL_FieldBuilder_ClrFieldSetRVA(ILExecThread *_thread,
                                     ILNativeInt item, ILInt32 rva)
{
	ILFieldRVA *rvainfo;
	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));
	if(item)
	{
		rvainfo = ILFieldRVAGetFromOwner((ILField *)item);
		if(rvainfo)
		{
			ILFieldRVASetRVA(rvainfo, (ILUInt32)rva);
		}
		else
		{
			if(!ILFieldRVACreate(ILProgramItem_Image(item), 0,
								 (ILField *)item, (ILUInt32)rva))
			{
				IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
				ILExecThreadThrowOutOfMemory(_thread);
			}
		}
	}
	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
}

/*
 * private static IntPtr ClrModuleCreate(IntPtr assembly, String name);
 */
ILNativeInt _IL_ModuleBuilder_ClrModuleCreate(ILExecThread *_thread,
                                              ILNativeInt assembly,
                                              ILString *name)
{
	ILModule *retval;
	ILImage *image;
	const char *str;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	image = ILProgramItem_Image(assembly); 
	if (!(str = (const char *)ILStringToAnsi(_thread, name)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	if (!(retval = ILModuleCreate(image, 0, str, NULL)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILNativeInt)retval;
}

/*
 * private static int ClrModuleCreateString(IntPtr module, String str);
 */
ILInt32 _IL_ModuleBuilder_ClrModuleCreateString(ILExecThread *_thread,
                                                ILNativeInt module,
                                                ILString *str)
{
	ILUInt32 retval;
	ILProgramItem *item;
	ILImage *image;
	const char *string;
	int length;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	item = (ILProgramItem *)module;
	image = ILProgramItem_Image(item);
	length = (int)(((System_String *)str)->length);
	string = (const char *)ILStringToAnsi(_thread, str);
	if (!(retval = ILImageAddUserString(image, string, length)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILInt32)(retval | IL_META_TOKEN_STRING);
}

/*
 * internal static int ClrModuleWriteData(IntPtr module, byte[] data);
 */
ILInt32 _IL_ModuleBuilder_ClrModuleWriteData(ILExecThread *_thread,
                                             ILNativeInt module,
                                             System_Array *data)
{
	/* TODO */
	return 0;
}

/*
 * internal static int ClrModuleWriteGap(IntPtr module, int size);
 */
ILInt32 _IL_ModuleBuilder_ClrModuleWriteGap(ILExecThread *_thread,
                                            ILNativeInt module, ILInt32 size)
{
	/* TODO */
	return 0;
}

/*
 * private static IntPtr ClrPropertyCreate(IntPtr classInfo, String name,
 *                                         PropertyAttributes attrs,
 *                                         IntPtr signature);
 */
ILNativeInt _IL_PropertyBuilder_ClrPropertyCreate(ILExecThread *_thread,
                                                  ILNativeInt classInfo,
                                                  ILString *name,
                                                  ILInt32 attrs,
                                                  ILNativeInt signature)
{
	ILProperty *retval;
	ILClass *info;
	ILType *sig;
	const char *str;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	info = (ILClass *)classInfo;
	sig = (ILType *)signature;
	if (!(str = (const char *)ILStringToAnsi(_thread, name)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	if (!(retval = ILPropertyCreate(info, 0, str, (ILUInt32)attrs, sig)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILNativeInt)retval;
}

/*
 * private static void ClrPropertyAddSemantics(IntPtr item,
 *                                             MethodSemanticsAttributes attr,
 *                                             MethodToken token);
 */
void _IL_PropertyBuilder_ClrPropertyAddSemantics(ILExecThread *_thread,
                                                 ILNativeInt item,
                                                 ILInt32 attr,
                                                 void *token)
{
	ILProgramItem *itemInfo;
	ILImage *image;
	ILToken tok;
	ILMethod *method;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	itemInfo = (ILProgramItem *)item;
	image = ILProgramItem_Image(itemInfo);
	tok = *((ILToken *)token);
	if (!(method = ILMethod_FromToken(image,tok)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}
	if (!(ILMethodSemCreate(itemInfo, 0, (ILUInt32)attr, method)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
}

/*
 * private static void ClrPropertySetConstant(IntPtr item, Object value);
 */
void _IL_PropertyBuilder_ClrPropertySetConstant(ILExecThread *_thread,
                                                ILNativeInt item,
                                                ILObject *value)
{
	ILImage *image;
	ILProgramItem *owner;
	ILProperty *property;
	ILType *type;
	ILType *valueType;
	ILUInt32 elemType;
	ILConstant *constant;
	ILExecValue blob;
	ILUInt32 len;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	owner = (ILProgramItem *)item;
	image = ILProgramItem_Image(owner);
	property = (ILProperty *)item;
	type = ILTypeGetReturn(ILProperty_Signature(property));
	valueType = ILClassToType(GetObjectClass(value));
	type = ILTypeGetEnumType(type);
	valueType = ILTypeGetEnumType(valueType);
	if (!(ILTypeAssignCompatible(image, valueType, type)))
	{
		return;
	}
	elemType = ILType_ToElement(type);

	/* handle null objects */
	if (!value)
	{
		if (!(constant = ILConstantCreate(image, 0, owner, IL_META_ELEMTYPE_CLASS)))
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			ILExecThreadThrowOutOfMemory(_thread);
			return;
		}
		blob.int32Value = (ILInt32)0;
		if (!(ILConstantSetValue(constant, &blob, 4)))
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			ILExecThreadThrowOutOfMemory(_thread);
			return;
		}
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		return;
	}

	switch(elemType)
	{
		case IL_META_ELEMTYPE_BOOLEAN:
		case IL_META_ELEMTYPE_I1:
		case IL_META_ELEMTYPE_U1:
		case IL_META_ELEMTYPE_CHAR:
		case IL_META_ELEMTYPE_I2:
		case IL_META_ELEMTYPE_U2:
		case IL_META_ELEMTYPE_I4:
		case IL_META_ELEMTYPE_U4:
		case IL_META_ELEMTYPE_R4:
		case IL_META_ELEMTYPE_I8:
		case IL_META_ELEMTYPE_U8:
		case IL_META_ELEMTYPE_R8:
		case IL_META_ELEMTYPE_I:
		case IL_META_ELEMTYPE_U:
		case IL_META_ELEMTYPE_R:
		{
			len = (ILUInt32)ILSizeOfType(_thread, type);
		}
		break;

		case IL_META_ELEMTYPE_STRING:
		{
			ILUInt16 *chars;
			ILUInt16 *ptr;
			ILUInt32 i;

			len = (((System_String *)value)->length);
			if (!(blob.ptrValue = (ILUInt16 *)ILMalloc(len*2)))
			{
				IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
				ILExecThreadThrowOutOfMemory(_thread);
				return;
			}
			chars = StringToBuffer(value);
			ptr = (ILUInt16 *)blob.ptrValue;
			for (i = 0; i < len; ++i, ++ptr)
			{
				IL_WRITE_UINT16(ptr, chars[i]);
			}
			if (!(constant = ILConstantCreate(image, 0, owner, elemType)))
			{
				ILFree(blob.ptrValue);
				IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
				ILExecThreadThrowOutOfMemory(_thread);
				return;
			}
			if (!(ILConstantSetValue(constant, blob.ptrValue, len*2)))
			{
				ILFree(blob.ptrValue);
				IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
				ILExecThreadThrowOutOfMemory(_thread);
				return;
			}
			ILFree(blob.ptrValue);
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		}
		return;

		default:
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		}
		return;
	}
	if (!(ILExecThreadUnbox(_thread, type, value, &blob)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		return;
	}
	if (!(constant = ILConstantCreate(image, 0, owner, elemType)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}
	if (!(ILConstantSetValue(constant, &blob, len)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
}

/*
 * private static IntPtr ClrTypeCreate(IntPtr module, IntPtr scope,
 *                                     String name, String nspace,
 *                                     TypeAttributes attr, TypeToken parent);
 */
ILNativeInt _IL_TypeBuilder_ClrTypeCreate(ILExecThread *_thread,
                                          ILNativeInt module,
                                          ILNativeInt nestedParent,
                                          ILString *name,
                                          ILString *nspace,
                                          ILInt32 attr,
                                          void *parent)
{
	ILImage *image;
	ILToken token;
	ILProgramItem *scope;
	const char *typeName;
	const char *nameSpace;
	ILClass *baseClass;
	ILClass *retval;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	image = ((ILProgramItem *)module)->image;
	token = *((ILToken *)parent);
	if (!(scope = (ILProgramItem *)nestedParent) &&
	    !(scope = ILClassGlobalScope(image)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	if (!(typeName = (const char *)ILStringToAnsi(_thread, name)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	if (nspace)
	{
		if (!(nameSpace = (const char *)ILStringToAnsi(_thread, nspace)))
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			ILExecThreadThrowOutOfMemory(_thread);
			return 0;
		}
	}
	else
	{
		nameSpace = 0;
	}
	if (!token &&
	    ((!strcmp("<Module>", typeName) && !nameSpace) ||
	     (!strcmp("Object", typeName) && nameSpace && !strcmp("System", nameSpace)) ||
	     (attr & IL_META_TYPEDEF_INTERFACE)))
	{
		/* interfaces, <Module>, and System.Object */
		baseClass = NULL;
	}
	else if (!(baseClass = ILClass_FromToken(image, token)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}

	if ((retval = ILClassCreate(scope, 0, typeName, nameSpace,
								ILToProgramItem(baseClass))))
	{
		ILClassSetAttrs(retval, (ILUInt32)-1, (ILUInt32)attr);
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILNativeInt)retval;
}

/*
 * private static void ClrTypeSetPackingSize(IntPtr classInfo, int packingSize);
 */
void _IL_TypeBuilder_ClrTypeSetPackingSize(ILExecThread *_thread,
                                           ILNativeInt classInfo,
                                           ILInt32 packingSize)
{
	ILClassLayout *layout;
	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));
	if(classInfo)
	{
		layout = ILClassLayoutGetFromOwner((ILClass *)classInfo);
		if(layout)
		{
			ILClassLayoutSetPackingSize(layout, (ILUInt32)packingSize);
		}
		else
		{
			if(!ILClassLayoutCreate(ILProgramItem_Image(classInfo), 0,
									(ILClass *)classInfo,
									(ILUInt32)packingSize, 0))
			{
				IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
				ILExecThreadThrowOutOfMemory(_thread);
			}
		}
	}
	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
}

/*
 * private static void ClrTypeSetClassSize(IntPtr classInfo, int classSize);
 */
void _IL_TypeBuilder_ClrTypeSetClassSize(ILExecThread *_thread,
                                         ILNativeInt classInfo,
                                         ILInt32 classSize)
{
	ILClassLayout *layout;
	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));
	if(classInfo)
	{
		layout = ILClassLayoutGetFromOwner((ILClass *)classInfo);
		if(layout)
		{
			ILClassLayoutSetClassSize(layout, (ILUInt32)classSize);
		}
		else
		{
			if(!ILClassLayoutCreate(ILProgramItem_Image(classInfo), 0,
									(ILClass *)classInfo,
									0, (ILUInt32)classSize))
			{
				IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
				ILExecThreadThrowOutOfMemory(_thread);
			}
		}
	}
	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
}

/*
 * private static void ClrTypeAddInterface(IntPtr classInfo, TypeToken iface);
 */
void _IL_TypeBuilder_ClrTypeAddInterface(ILExecThread *_thread,
                                         ILNativeInt classInfo,
                                         void *iface)
{
	ILClass *class;
	ILToken token;
	ILImage *image;
	ILClass *interface;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	class = (ILClass *)classInfo;
	token = *((ILToken *)iface);
	image = ILClassToImage(class);
	if (!(interface = ILClass_FromToken(image, token)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}
	if (!(ILClassAddImplements(class, ILToProgramItem(interface), token)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
}

/*
 * private static int ClrTypeGetPackingSize(IntPtr classInfo);
 */
ILInt32 _IL_TypeBuilder_ClrTypeGetPackingSize(ILExecThread *_thread,
                                              ILNativeInt classInfo)
{
	ILInt32 size = 0;
	ILClassLayout *layout;
	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));
	if(classInfo)
	{
		layout = ILClassLayoutGetFromOwner((ILClass *)classInfo);
		if(layout)
		{
			size = (ILInt32)(ILClassLayoutGetPackingSize(layout));
		}
	}
	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return size;
}

/*
 * private static int ClrTypeGetClassSize(IntPtr classInfo);
 */
ILInt32 _IL_TypeBuilder_ClrTypeGetClassSize(ILExecThread *_thread,
                                            ILNativeInt classInfo)
{
	ILInt32 size = 0;
	ILClassLayout *layout;
	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));
	if(classInfo)
	{
		layout = ILClassLayoutGetFromOwner((ILClass *)classInfo);
		if(layout)
		{
			size = (ILInt32)(ILClassLayoutGetClassSize(layout));
		}
	}
	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return size;
}

/*
 * private static void ClrTypeSetParent(IntPtr classInfo, TypeToken parent);
 */
void _IL_TypeBuilder_ClrTypeSetParent(ILExecThread *_thread,
                                      ILNativeInt classInfo,
                                      void *parent)
{
	ILClass *class;
	ILToken token;
	ILImage *image;
	ILClass *parentClass;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	class = (ILClass *)classInfo;
	token = *((ILToken *)parent);
	image = ILClassToImage(class);
	if (!(parentClass = ILClass_FromToken(image, token)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}
	ILClassSetParent(class, ILToProgramItem(parentClass));

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
}

/*
 * internal static int ClrTypeImport(IntPtr module, IntPtr classInfo);
 */
ILInt32 _IL_TypeBuilder_ClrTypeImport(ILExecThread *_thread,
                                      ILNativeInt module,
                                      ILNativeInt classInfo)
{
	ILImage *image;
	ILClass *import;
	ILToken token;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	image = ((ILProgramItem *)module)->image;
	import = ILClassImport(image, (ILClass *)classInfo);
	token = 0;
	if (import)
	{
		token = ILClass_Token(import);
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILInt32)token;
}

/*
 * internal static int ClrTypeImportMember(IntPtr module, IntPtr memberInfo);
 */
ILInt32 _IL_TypeBuilder_ClrTypeImportMember(ILExecThread *_thread,
                                            ILNativeInt module,
                                            ILNativeInt memberInfo)
{
	ILImage *image;
	ILMember *import;
	ILToken token;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	image = ((ILProgramItem *)module)->image;
	import = ILMemberImport(image, (ILMember *)memberInfo);
	if (!import)
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	token = ILMember_Token(import);

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILInt32)token;
}

/*
 * private static void ClrTypeAddOverride(IntPtr module, int bodyToken,
 *                                        int declToken);
 */
void _IL_TypeBuilder_ClrTypeAddOverride(ILExecThread *_thread,
                                        ILNativeInt module,
                                        ILInt32 bodyToken,
                                        ILInt32 declToken)
{
	ILImage *image;
	ILMethod *body;
	ILMethod *decl;
	ILClass *class;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	image = ((ILProgramItem *)module)->image;
	if (!(body = ILMethod_FromToken(image, (ILToken)bodyToken)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}
	if (!(decl = ILMethod_FromToken(image, (ILToken)declToken)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}
	class = ILMethod_Owner(decl);
	if (!(ILOverrideCreate(class, 0, decl, body)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
}

/*
 * internal static IntPtr ClrMethodCreate(IntPtr classInfo, String name,
 *                                        MethodAttributes attributes,
 *                                        IntPtr signature);
 */
ILNativeInt _IL_MethodBuilder_ClrMethodCreate(ILExecThread *_thread,
                                              ILNativeInt classInfo,
                                              ILString *name,
                                              ILInt32 attributes,
                                              ILNativeInt signature)
{
	ILClass *class;
	const char *str;
	ILMethod *method;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	class = (ILClass *)classInfo;
	if (!(str = (const char *)ILStringToAnsi(_thread, name)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	if (!(method = ILMethodCreate(class, 0, str, (ILUInt32)attributes)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	ILMemberSetSignature((ILMember *)method, (ILType *)signature);

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILNativeInt)method;
}

/*
 * internal static void ClrMethodSetImplAttrs(IntPtr method,
 *                                            MethodImplAttributes attributes);
 */
void _IL_MethodBuilder_ClrMethodSetImplAttrs(ILExecThread *_thread,
                                             ILNativeInt item,
                                             ILInt32 attributes)
{
	if(item)
	{
		ILMethodSetImplAttrs((ILMethod *)item, ~((ILUInt32)0),
							 (ILUInt32)attributes);
	}
}

/*
 * internal static int ClrMethodCreateVarArgRef(IntPtr module,
 *                                              int methodToken,
 *                                              IntPtr signature);
 */
ILInt32 _IL_MethodBuilder_ClrMethodCreateVarArgRef(ILExecThread *_thread,
                                                   ILNativeInt module,
                                                   ILInt32 methodToken,
                                                   ILNativeInt signature)
{
	/* TODO */
	return 0;
}

/*
 * internal static void ClrMethodSetRVA(IntPtr method, int rva);
 */
void _IL_MethodBuilder_ClrMethodSetRVA(ILExecThread *_thread,
                                       ILNativeInt method, ILInt32 rva)
{
	if(method)
	{
		ILMethodSetRVA((ILMethod *)method, (ILUInt32)rva);
	}
}

/*
 * internal static void ClrMethodAddPInvoke(IntPtr method, int pinvAttrs,
 *                                          String dllName, String entryName);
 */
void _IL_MethodBuilder_ClrMethodAddPInvoke(ILExecThread *_thread,
                                           ILNativeInt method,
                                           ILInt32 pinvAttrs,
                                           ILString *dllName,
                                           ILString *entryName)
{
	ILMethod *methodInfo;
	ILModule *module;
	ILImage * image;
	const char *dll;
	const char *entry;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	methodInfo = (ILMethod *)method;
	image = ((ILProgramItem *)methodInfo)->image;
	if (!(dll = (const char *)ILStringToAnsi(_thread, dllName)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}
	if (!(entry = (const char *)ILStringToAnsi(_thread, entryName)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}
	if (!(module = ILModuleCreate(image, 0, dll, 0)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}
	if (!(ILPInvokeCreate(methodInfo, 0, (ILUInt32)pinvAttrs, module, entry)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return;
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
}

/*
 * private static IntPtr ClrSigCreateMethod(IntPtr context, int callConv,
 *                                          IntPtr returnType);
 */
ILNativeInt _IL_SignatureHelper_ClrSigCreateMethod(ILExecThread *_thread,
                                                   ILNativeInt context,
                                                   ILInt32 callConv,
                                                   ILNativeInt returnType)
{
	ILType *type = 0;
	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));
	if(context)
	{
		type = ILTypeCreateMethod((ILContext *)context, (ILType *)returnType);
		if(!type)
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			ILExecThreadThrowOutOfMemory(_thread);
			return 0;
		}
		ILTypeSetCallConv(type, (ILUInt32)callConv);
	}
	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILNativeInt)type;
}

/*
 * private static IntPtr ClrSigCreateProperty
 *            (IntPtr context, IntPtr returnType);
 */
ILNativeInt _IL_SignatureHelper_ClrSigCreateProperty(ILExecThread *_thread,
                                                     ILNativeInt context,
                                                     ILNativeInt returnType)
{
	ILType *type = 0;
	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));
	if(context)
	{
		type = ILTypeCreateProperty((ILContext *)context, (ILType *)returnType);
		if(!type)
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			ILExecThreadThrowOutOfMemory(_thread);
			return 0;
		}
	}
	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILNativeInt)type;
}

/*
 * private static IntPtr ClrSigModuleToContext(IntPtr module);
 */
ILNativeInt _IL_SignatureHelper_ClrSigModuleToContext(ILExecThread *_thread,
                                                      ILNativeInt module)
{
	if(module)
	{
		return (ILNativeInt)ILImageToContext
					(ILProgramItem_Image((ILModule *)module));
	}
	else
	{
		return 0;
	}
}

/*
 * private static IntPtr ClrSigCreatePrimitive(IntPtrContext, Type type);
 */
ILNativeInt _IL_SignatureHelper_ClrSigCreatePrimitive(ILExecThread *_thread,
                                                      ILNativeInt context,
                                                      ILObject *type)
{
	ILClass *classInfo = _ILGetClrClass(_thread, type);
	if(classInfo)
	{
		return (ILNativeInt)(ILClassToType(classInfo));
	}
	else
	{
		return (ILNativeInt)ILType_Invalid;
	}
}

/*
 * private static IntPtr ClrSigCreateArray(IntPtr context, int rank,
 *                                         IntPtr elemType);
 */
ILNativeInt _IL_SignatureHelper_ClrSigCreateArray(ILExecThread *_thread,
                                                  ILNativeInt context,
                                                  ILInt32 rank,
                                                  ILNativeInt elemType)
{
	ILType *type = 0;
	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));
	if(context)
	{
		type = ILTypeCreateArray
			((ILContext *)context, (unsigned long)(long)rank,
			 (ILType *)elemType);
		if(!type)
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			ILExecThreadThrowOutOfMemory(_thread);
			return 0;
		}
	}
	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILNativeInt)type;
}

/*
 * private static IntPtr ClrSigCreatePointer(IntPtr context, IntPtr elemType);
 */
ILNativeInt _IL_SignatureHelper_ClrSigCreatePointer(ILExecThread *_thread,
                                                    ILNativeInt context,
                                                    ILNativeInt elemType)
{
	ILType *type = 0;
	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));
	if(context)
	{
		type = ILTypeCreateRef
			((ILContext *)context, IL_TYPE_COMPLEX_PTR, (ILType *)elemType);
		if(!type)
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			ILExecThreadThrowOutOfMemory(_thread);
			return 0;
		}
	}
	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILNativeInt)type;
}

/*
 * private static IntPtr ClrSigCreateByRef(IntPtr context, IntPtr elemType);
 */
ILNativeInt _IL_SignatureHelper_ClrSigCreateByRef(ILExecThread *_thread,
                                                  ILNativeInt context,
                                                  ILNativeInt elemType)
{
	ILType *type = 0;
	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));
	if(context)
	{
		type = ILTypeCreateRef
			((ILContext *)context, IL_TYPE_COMPLEX_BYREF, (ILType *)elemType);
		if(!type)
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			ILExecThreadThrowOutOfMemory(_thread);
			return 0;
		}
	}
	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILNativeInt)type;
}

/*
 * private static IntPtr ClrSigCreateValueType(IntPtr module, int token);
 */
ILNativeInt _IL_SignatureHelper_ClrSigCreateValueType(ILExecThread *_thread,
                                                      ILNativeInt module,
                                                      ILInt32 token)
{
	if(module && token)
	{
		return (ILNativeInt)(ILType_FromValueType
					(ILClass_FromToken(ILProgramItem_Image(module),
									   (ILToken)(long)token)));
	}
	else
	{
		return (ILNativeInt)ILType_Invalid;
	}
}

/*
 * private static IntPtr ClrSigCreateClass(IntPtr module, int token);
 */
ILNativeInt _IL_SignatureHelper_ClrSigCreateClass(ILExecThread *_thread,
                                                  ILNativeInt module,
                                                  ILInt32 token)
{
	if(module && token)
	{
		return (ILNativeInt)(ILType_FromClass
					(ILClass_FromToken(ILProgramItem_Image(module),
									   (ILToken)(long)token)));
	}
	else
	{
		return (ILNativeInt)ILType_Invalid;
	}
}

/*
 * private static IntPtr ClrSigCreateLocal(IntPtr context);
 */
ILNativeInt _IL_SignatureHelper_ClrSigCreateLocal(ILExecThread *_thread,
                                                  ILNativeInt context)
{
	ILType *type = 0;
	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));
	if(context)
	{
		type = ILTypeCreateLocalList((ILContext *)context);
		if(!type)
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			ILExecThreadThrowOutOfMemory(_thread);
			return 0;
		}
	}
	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILNativeInt)type;
}

/*
 * private static bool ClrSigAddArgument(IntPtr context, IntPtr sig,
 *                                       IntPtr arg);
 */
ILBool _IL_SignatureHelper_ClrSigAddArgument(ILExecThread *_thread,
                                             ILNativeInt context,
                                             ILNativeInt sig,
                                             ILNativeInt arg)
{
	int result = 0;
	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));
	if(context && sig)
	{
		if(ILType_IsComplex((ILType *)sig) &&
		   ILType_Kind((ILType *)sig) == IL_TYPE_COMPLEX_LOCALS)
		{
			if(!ILTypeAddLocal((ILContext *)context,
							   (ILType *)sig, (ILType *)arg))
			{
				IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
				ILExecThreadThrowOutOfMemory(_thread);
				return 0;
			}
			result = 1;
		}
		else
		{
			if(!ILTypeAddParam((ILContext *)context,
							   (ILType *)sig, (ILType *)arg))
			{
				IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
				ILExecThreadThrowOutOfMemory(_thread);
				return 0;
			}
			result = 1;
		}
	}
	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILBool)result;
}

/*
 * private static bool ClrSigAddSentinel(IntPtr context, IntPtr sig);
 */
ILBool _IL_SignatureHelper_ClrSigAddSentinel(ILExecThread *_thread,
                                             ILNativeInt context,
                                             ILNativeInt sig)
{
	int result = 0;
	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));
	if(context && sig)
	{
		if(!ILTypeAddSentinel((ILContext *)context, (ILType *)sig))
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			ILExecThreadThrowOutOfMemory(_thread);
			return 0;
		}
		result = 1;
	}
	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILBool)result;
}

/*
 * private static IntPtr ClrSigCreateMethodCopy(IntPtr context, IntPtr module,
 *                                              int methodToken);
 */
ILNativeInt _IL_SignatureHelper_ClrSigCreateMethodCopy(ILExecThread *_thread,
                                                       ILNativeInt context,
                                                       ILNativeInt module,
                                                       ILInt32 methodToken)
{
	ILType *retval;
	ILContext *cntxt;
	ILProgramItem *item;
	ILImage *image;
	ILMethod *method;
	ILType *methodType;
	ILType *tmp;
	ILUInt32 callConv;
	unsigned long params;
	unsigned long i;

	if (!context || !module) { return 0; }

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	cntxt = (ILContext *)context;
	item = (ILProgramItem *)module;
	image = ILProgramItem_Image(item);
	method = ILMethod_FromToken(image, (ILToken)methodToken);
	methodType = ILMethod_Signature(method);
	tmp = ILTypeGetReturnWithPrefixes(methodType);
	callConv = ILType_CallConv(methodType);
	if (!(retval = ILTypeCreateMethod(cntxt, tmp)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	ILTypeSetCallConv(retval, callConv);
	params = ILTypeNumParams(methodType);
	for (i = 0; i < params; ++i)
	{
		tmp = ILTypeGetParamWithPrefixes(methodType, i);
		if (!(ILTypeAddParam(cntxt, retval, tmp)))
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			ILExecThreadThrowOutOfMemory(_thread);
			return 0;
		}
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILNativeInt)retval;
}

/*
 * private static bool ClrSigIdentical(IntPtr sig1, IntPtr sig2);
 */
ILBool _IL_SignatureHelper_ClrSigIdentical(ILExecThread *_thread,
                                           ILNativeInt sig1,
                                           ILNativeInt sig2)
{
	return (ILBool)(ILTypeIdentical((ILType *)sig1, (ILType *)sig2));
}

/*
 * private static int ClrSigGetHashCode(IntPtr sig);
 */
ILInt32 _IL_SignatureHelper_ClrSigGetHashCode(ILExecThread *_thread,
                                              ILNativeInt sig)
{
	if (sig)
	{
		return (ILInt32)ILTypeHash((ILType *)sig);
	}
	else
	{
		return 0;
	}
}

/*
 * private static long ClrSigFinalize(IntPtr module, IntPtr sig, bool field);
 */
ILInt64 _IL_SignatureHelper_ClrSigFinalize(ILExecThread *_thread,
                                           ILNativeInt module,
                                           ILNativeInt sig,
                                           ILBool field)
{
	ILProgramItem *item;
	ILImage *image;
	ILType *type;
	unsigned long offset;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	item = (ILProgramItem *)module;
	image = ILProgramItem_Image(item);
	type = (ILType *)sig;
	if (field)
	{
		offset = ILTypeToFieldSig(image, type);
	}
	else if (ILType_IsMethod(type))
	{
		offset = ILTypeToMethodSig(image, type);
	}
	else
	{
		offset = ILTypeToOtherSig(image, type);
	}
	if (!offset)
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return (ILInt64)-1;
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILInt64)offset;
}

/*
 * private static byte[] ClrSigGetBytes(IntPtr module, long offset);
 */
System_Array *_IL_SignatureHelper_ClrSigGetBytes(ILExecThread *_thread,
                                                 ILNativeInt module,
                                                 ILInt64 offset)
{
	ILProgramItem *item;
	ILImage *image;
	ILUInt8 *buf;
	System_Array *bytes;
	ILUInt32 blobOffset;
	ILUInt32 length;
	unsigned char *blob;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	item = (ILProgramItem *)module;
	image = ILProgramItem_Image(item);
	blobOffset = (ILUInt32)offset;
	blob = (unsigned char *)ILImageGetBlob(image, blobOffset, &length);
	bytes = (System_Array *)ILExecThreadNew(_thread, "[B", "(Ti)V", (ILVaInt)length);
	if (!bytes)
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	buf = (ILUInt8 *)ArrayToBuffer(bytes);
	ILMemCpy(buf, blob, length);

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return bytes;
}
/*
 * private static int ClrStandAloneToken(IntPtr module, IntPtr sig);
 */
ILInt32 _IL_SignatureHelper_ClrStandAloneToken(ILExecThread *_thread,
                                               ILNativeInt _module,
                                               ILNativeInt _sig)
{
	ILInt32 retval;
	ILProgramItem *module;
	ILImage *image;
	ILType *sig;
	ILStandAloneSig *saSig;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	module = (ILProgramItem *)_module;
	image = ILProgramItem_Image(module);
	sig = (ILType *)_sig;
	if (!(saSig = ILStandAloneSigCreate(image, 0, sig)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	retval = (ILInt32)ILProgramItem_Token(saSig);

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return retval;
}

/*
 * internal static IntPtr ClrParameterCreate(IntPtr method, int position,
 *                                           ParameterAttributes attributes,
 *                                           String name);
 */
ILNativeInt _IL_ParameterBuilder_ClrParameterCreate(ILExecThread *_thread,
                                                    ILNativeInt method,
                                                    ILInt32 position,
                                                    ILInt32 attributes,
                                                    ILString *name)
{
	ILParameter *retval;
	ILMethod *methodInfo;
	const char *str;

	IL_METADATA_WRLOCK(_ILExecThreadProcess(_thread));

	methodInfo = (ILMethod *)method;
	if(name != NULL)
	{
		if (!(str = (const char *)ILStringToAnsi(_thread, name)))
		{
			IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
			ILExecThreadThrowOutOfMemory(_thread);
			return 0;
		}
	}
	else
	{
		// Parameter names can be NULL
		str = NULL;
	}
	if (!(retval = ILParameterCreate(methodInfo, 0, str, (ILUInt32)attributes, (ILUInt32)position)))
	{
		IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}

	IL_METADATA_UNLOCK(_ILExecThreadProcess(_thread));
	return (ILNativeInt)retval;
}

/*
 * internal static int ClrParameterGetPosition(IntPtr parameter);
 */
ILInt32 _IL_ParameterBuilder_ClrParameterGetPosition(ILExecThread *_thread,
                                                     ILNativeInt parameter)
{
	if(parameter)
	{
		return ILParameter_Num((ILParameter *)parameter);
	}
	else
	{
		return 0;
	}
}

/*
 * internal static ParameterAttributes ClrParameterGetPosition
 *						(IntPtr parameter);
 */
ILInt32 _IL_ParameterBuilder_ClrParameterGetAttrs(ILExecThread *_thread,
                                                  ILNativeInt parameter)
{
	if(parameter)
	{
		return ILParameter_Attrs((ILParameter *)parameter);
	}
	else
	{
		return 0;
	}
}

/*
 * internal static String ClrParameterGetName(IntPtr parameter);
 */
ILString *_IL_ParameterBuilder_ClrParameterGetName(ILExecThread *_thread,
                                                   ILNativeInt parameter)
{
	if(parameter)
	{
		return ILStringCreate
			(_thread, ILParameter_Name((ILParameter *)parameter));
	}
	else
	{
		return 0;
	}
}

#endif /* IL_CONFIG_REFLECTION */

#ifdef	__cplusplus
};
#endif
