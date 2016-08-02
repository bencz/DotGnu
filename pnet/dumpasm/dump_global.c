/*
 * dump_global.c - Disassemble global information.
 *
 * Copyright (C) 2001, 2009  Southern Storm Software, Pty Ltd.
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

#include "il_dumpasm.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Dump information about a module.
 */
static void Dump_Module(ILImage *image, FILE *outstream, int flags,
						unsigned long token, ILModule *module,
						unsigned long refToken)
{
	ILUInt32 generation;
	const unsigned char *guid;
	fputs(".module ", outstream);
	ILDumpIdentifier(outstream, ILModule_Name(module), 0, flags);
	if((flags & IL_DUMP_SHOW_TOKENS) != 0)
	{
		fprintf(outstream, " /*%08lX*/", token);
	}
	generation = ILModuleGetGeneration(module);
	if(generation != 0)
	{
		fprintf(outstream, "\n// Generation: %lu",
				(unsigned long)generation);
	}
	fputs("\n// MVID: ", outstream);
	ILDumpGUID(outstream, ILModuleGetMVID(module));
	guid = ILModuleGetEncId(module);
	if(guid)
	{
		fputs("\n// EncId: ", outstream);
		ILDumpGUID(outstream, guid);
	}
	guid = ILModuleGetEncBaseId(module);
	if(guid)
	{
		fputs("\n// EncBaseId: ", outstream);
		ILDumpGUID(outstream, guid);
	}
	putc('\n', outstream);
	if(ILProgramItem_HasAttrs(module))
	{
		ILDAsmDumpCustomAttrs(image, outstream, flags, 0,
							  ILToProgramItem(module));
	}
}

/*
 * Dump information about a module reference.
 */
static void Dump_ModuleRef(ILImage *image, FILE *outstream, int flags,
						   unsigned long token, ILModule *module,
						   unsigned long refToken)
{
	fputs(".module extern ", outstream);
	ILDumpIdentifier(outstream, ILModule_Name(module), 0, flags);
	if((flags & IL_DUMP_SHOW_TOKENS) != 0)
	{
		fprintf(outstream, " /*%08lX*/", token);
	}
	putc('\n', outstream);
	if(ILProgramItem_HasAttrs(module))
	{
		ILDAsmDumpCustomAttrs(image, outstream, flags, 0,
							  ILToProgramItem(module));
	}
}

/*
 * Dump information about an OS definition.
 */
static void Dump_OSDef(ILImage *image, FILE *outstream, int flags,
					   unsigned long token, ILOSInfo *osinfo,
					   unsigned long refToken)
{
	fprintf(outstream, "\t.os %lu .ver %lu:%lu\n",
			(unsigned long)(ILOSInfo_Identifier(osinfo)),
			(unsigned long)(ILOSInfo_Major(osinfo)),
			(unsigned long)(ILOSInfo_Minor(osinfo)));
}

/*
 * Dump information about a processor definition.
 */
static void Dump_ProcessorDef(ILImage *image, FILE *outstream, int flags,
							  unsigned long token, ILProcessorInfo *procinfo,
							  unsigned long refToken)
{
	fprintf(outstream, "\t.processor %lu\n",
			(unsigned long)ILProcessorInfo_Number(procinfo));
}

/*
 * Dump information about an assembly.
 */
static void Dump_Assembly(ILImage *image, FILE *outstream, int flags,
						  unsigned long token, ILAssembly *assem,
						  unsigned long refToken)
{
	const ILUInt16 *version;
	const void *orig;
	ILUInt32 origLen;
	fputs(".assembly ", outstream);
	if((flags & IL_DUMP_SHOW_TOKENS) != 0)
	{
		fprintf(outstream, "/*%08lX*/ ", token);
	}
	ILDumpFlags(outstream, ILAssembly_Attrs(assem), ILAssemblyFlags, 0);
	ILDumpIdentifier(outstream, ILAssembly_Name(assem), 0, flags);
	fputs("\n{\n", outstream);
	ILDAsmDumpSecurity(image, outstream, (ILProgramItem *)assem, flags);
	if(ILAssembly_HashAlg(assem) != 0)
	{
		fprintf(outstream, "\t.hash algorithm 0x%08lX\n",
				(unsigned long)(ILAssembly_HashAlg(assem)));
	}
	version = ILAssemblyGetVersion(assem);
	fprintf(outstream, "\t.ver %lu:%lu:%lu:%lu\n",
			(unsigned long)(version[0]), (unsigned long)(version[1]),
			(unsigned long)(version[2]), (unsigned long)(version[3]));
	if((orig = ILAssemblyGetOriginator(assem, &origLen)) != 0)
	{
		fputs("\t.publickey =", outstream);
		ILDAsmDumpBinaryBlob(outstream, image, orig, origLen);
		putc('\n', outstream);
	}
	if(ILAssembly_Locale(assem))
	{
		fputs("\t.locale ", outstream);
		ILDumpString(outstream, ILAssembly_Locale(assem));
		putc('\n', outstream);
	}
	ILDAsmWalkTokens(image, outstream, flags,
					 IL_META_TOKEN_OS_DEF,
					 (ILDAsmWalkFunc)Dump_OSDef, 0);
	ILDAsmWalkTokens(image, outstream, flags,
					 IL_META_TOKEN_PROCESSOR_DEF,
					 (ILDAsmWalkFunc)Dump_ProcessorDef, 0);
	if(ILProgramItem_HasAttrs(assem))
	{
		ILDAsmDumpCustomAttrs(image, outstream, flags, 1,
							  ILToProgramItem(assem));
	}
	fputs("}\n", outstream);
}

/*
 * Dump information about an OS reference.
 */
static void Dump_OSRef(ILImage *image, FILE *outstream, int flags,
					   unsigned long token, ILOSInfo *osinfo,
					   unsigned long refToken)
{
	if(ILProgramItem_Token(ILOSInfo_Assembly(osinfo)) == refToken)
	{
		fprintf(outstream, "\t.os %lu .ver %lu:%lu\n",
				(unsigned long)(ILOSInfo_Identifier(osinfo)),
				(unsigned long)(ILOSInfo_Major(osinfo)),
				(unsigned long)(ILOSInfo_Minor(osinfo)));
	}
}

/*
 * Dump information about a processor reference.
 */
static void Dump_ProcessorRef(ILImage *image, FILE *outstream, int flags,
					   		  unsigned long token, ILProcessorInfo *procinfo,
					   		  unsigned long refToken)
{
	if(ILProgramItem_Token(ILProcessorInfo_Assembly(procinfo)) == refToken)
	{
		fprintf(outstream, "\t.processor %lu\n",
				(unsigned long)(ILProcessorInfo_Number(procinfo)));
	}
}

/*
 * Dump information about an assembly reference.
 */
static void Dump_AssemblyRef(ILImage *image, FILE *outstream, int flags,
						     unsigned long token, ILAssembly *assem,
							 unsigned long refToken)
{
	const ILUInt16 *version;
	const void *orig;
	ILUInt32 origLen;
	fputs(".assembly extern ", outstream);
	if((flags & IL_DUMP_SHOW_TOKENS) != 0)
	{
		fprintf(outstream, "/*%08lX*/ ", token);
	}
	ILDumpFlags(outstream, ILAssembly_RefAttrs(assem),
				ILAssemblyRefFlags, 0);
	ILDumpIdentifier(outstream, ILAssembly_Name(assem), 0, flags);
	fputs("\n{\n", outstream);
	version = ILAssemblyGetVersion(assem);
	fprintf(outstream, "\t.ver %lu:%lu:%lu:%lu\n",
			(unsigned long)(version[0]), (unsigned long)(version[1]),
			(unsigned long)(version[2]), (unsigned long)(version[3]));
	if((orig = ILAssemblyGetOriginator(assem, &origLen)) != 0)
	{
		if(ILAssembly_HasFullOriginator(assem))
		{
			fputs("\t.publickey =", outstream);
		}
		else
		{
			fputs("\t.publickeytoken =", outstream);
		}
		ILDAsmDumpBinaryBlob(outstream, image, orig, origLen);
		putc('\n', outstream);
	}
	ILDAsmWalkTokens(image, outstream, flags,
					 IL_META_TOKEN_OS_REF,
					 (ILDAsmWalkFunc)Dump_OSRef, token);
	ILDAsmWalkTokens(image, outstream, flags,
					 IL_META_TOKEN_PROCESSOR_REF,
					 (ILDAsmWalkFunc)Dump_ProcessorRef, token);
	if(ILProgramItem_HasAttrs(assem))
	{
		ILDAsmDumpCustomAttrs(image, outstream, flags, 1,
							  ILToProgramItem(assem));
	}
	fputs("}\n", outstream);
}

/*
 * Dump information about a file declaration.
 */
static void Dump_File(ILImage *image, FILE *outstream, int flags,
					  unsigned long token, ILFileDecl *decl,
					  unsigned long refToken)
{
	const void *hash;
	ILUInt32 len;
	fputs(".file ", outstream);
	ILDumpFlags(outstream, ILFileDecl_Attrs(decl), ILFileFlags, 0);
	ILDumpIdentifier(outstream, ILFileDecl_Name(decl), 0, flags);
	if((hash = ILFileDeclGetHash(decl, &len)) != 0)
	{
		fputs(" .hash =", outstream);
		ILDAsmDumpBinaryBlob(outstream, image, hash, len);
	}
	putc('\n', outstream);
}

/*
 * Dump information about a manifest resource declaration.
 */
static void Dump_ManifestRes(ILImage *image, FILE *outstream, int flags,
					  		 unsigned long token, ILManifestRes *res,
					  		 unsigned long refToken)
{
	ILFileDecl *decl;
	ILAssembly *assem;
	fputs(".mresource ", outstream);
	ILDumpFlags(outstream, ILManifestRes_Attrs(res), ILManifestResFlags, 0);
	ILDumpIdentifier(outstream, ILManifestRes_Name(res), 0, flags);
	fputs("\n{\n", outstream);
	if((decl = ILManifestResGetOwnerFile(res)) != 0)
	{
		fputs("\t.file ", outstream);
		ILDumpIdentifier(outstream, ILFileDecl_Name(decl), 0, flags);
		fprintf(outstream, " at 0x%08lu\n",
				(unsigned long)(ILFileDecl_Attrs(decl)));
	}
	else if((assem = ILManifestResGetOwnerAssembly(res)) != 0)
	{
		fputs("\t.assembly extern ", outstream);
		ILDumpIdentifier(outstream, ILAssembly_Name(assem), 0, flags);
		putc('\n', outstream);
	}
	ILDAsmDumpCustomAttrs(image, outstream, flags, 1,
						  ILToProgramItem(res));
	fputs("}\n", outstream);
}

/*
 * Dump information about a exported type declaration.
 */
static void Dump_ExportedType(ILImage *image, FILE *outstream, int flags,
					  		  unsigned long token, ILExportedType *type,
					  		  unsigned long refToken)
{
	ILProgramItem *scope;
	ILFileDecl *decl;
	ILAssembly *assem;
	ILExportedType *expType;

	/* Dump the export heading */
	fputs(".class extern ", outstream);
	ILDumpFlags(outstream, ILExportedType_Attrs(type),
				ILExportedTypeDefinitionFlags, 0);
	ILDumpIdentifier(outstream, ILExportedType_Name(type),
					 ILExportedType_Namespace(type), flags);
	fputs("\n{\n", outstream);

	/* Dump the scope */
	scope = ILExportedType_Scope(type);
	if((decl = ILProgramItemToFileDecl(scope)) != 0)
	{
		fputs("\t.file ", outstream);
		ILDumpIdentifier(outstream, ILFileDecl_Name(decl), 0, flags);
		putc('\n', outstream);
	}
	else if((assem = ILProgramItemToAssembly(scope)) != 0)
	{
		fputs("\t.assembly extern ", outstream);
		ILDumpIdentifier(outstream, ILAssembly_Name(assem), 0, flags);
		putc('\n', outstream);
	}
	else if((expType = ILProgramItemToExportedType(scope)) != 0)
	{
		fputs("\t.comtype ", outstream);
		ILDumpIdentifier(outstream, ILExportedType_Name(expType),
						 ILExportedType_Namespace(expType), flags);
		putc('\n', outstream);
	}

	/* Dump the class identifier in the foreign scope */
	fprintf(outstream, "\t.class 0x%08lx\n",
			(unsigned long)(ILExportedType_Id(type)));

	/* Dump any custom attributes associated with the exported type */
	if(ILProgramItem_HasAttrs(type))
	{
		ILDAsmDumpCustomAttrs(image, outstream, flags, 0,
							  ILToProgramItem(type));
	}

	/* Dump the export footer */
	fputs("}\n", outstream);
}

void ILDAsmDumpGlobal(ILImage *image, FILE *outstream, int flags)
{
	/* Dump module references */
	ILDAsmWalkTokens(image, outstream, flags,
					 IL_META_TOKEN_MODULE_REF,
					 (ILDAsmWalkFunc)Dump_ModuleRef, 0);

	/* Dump assembly reference information */
	ILDAsmWalkTokens(image, outstream, flags,
					 IL_META_TOKEN_ASSEMBLY_REF,
					 (ILDAsmWalkFunc)Dump_AssemblyRef, 0);

	/* Dump file information */
	ILDAsmWalkTokens(image, outstream, flags,
					 IL_META_TOKEN_FILE,
					 (ILDAsmWalkFunc)Dump_File, 0);

	/* Dump assembly information */
	ILDAsmWalkTokens(image, outstream, flags,
					 IL_META_TOKEN_ASSEMBLY,
					 (ILDAsmWalkFunc)Dump_Assembly, 0);

	/* Dump manifest resource definitions */
	ILDAsmWalkTokens(image, outstream, flags,
					 IL_META_TOKEN_MANIFEST_RESOURCE,
					 (ILDAsmWalkFunc)Dump_ManifestRes, 0);

	/* Dump module definitions */
	ILDAsmWalkTokens(image, outstream, flags,
					 IL_META_TOKEN_MODULE,
					 (ILDAsmWalkFunc)Dump_Module, 0);

	/* Dump exported types */
	ILDAsmWalkTokens(image, outstream, flags,
					 IL_META_TOKEN_EXPORTED_TYPE,
					 (ILDAsmWalkFunc)Dump_ExportedType, 0);

	/* Dump the ".data" and ".tls" sections */
	ILDAsmDumpDataSections(outstream, image);
}

#ifdef	__cplusplus
};
#endif
