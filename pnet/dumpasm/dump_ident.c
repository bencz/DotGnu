/*
 * dump_ident.c - Dump identifiers in assembly format.
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

#include "il_dumpasm.h"

#ifdef	__cplusplus
extern	"C" {
#endif

static void DumpIdentifier(FILE *stream, const char *name, int flags)
{
	int len;
	int quote;
	int hardQuote;
	len = 0;
	quote = ((flags & IL_DUMP_QUOTE_NAMES) != 0);
	hardQuote = 0;
	while(name[len] != '\0')
	{
		if(!((name[len] >= 'A' && name[len] <= 'Z') ||
		     (name[len] >= 'a' && name[len] <= 'z') ||
		     (name[len] >= '0' && name[len] <= '9') ||
		      name[len] == '_' || name[len] == '$' || name[len] == '.'))
		{
			hardQuote = 1;
		}
		++len;
	}
	quote |= hardQuote;
	if(!quote)
	{
		fwrite(name, 1, len, stream);
	}
	else if(!hardQuote)
	{
		putc('\'', stream);
		fwrite(name, 1, len, stream);
		putc('\'', stream);
	}
	else
	{
		putc('\'', stream);
		while(*name != '\0')
		{
			if(*name == '\\' || *name == '\'')
			{
				putc('\\', stream);
				putc(*name, stream);
			}
			else if(*name < 0x20 || *name > 0x7E)
			{
				fprintf(stream, "\\%03o", ((int)(*name)) & 0xFF);
			}
			else
			{
				putc(*name, stream);
			}
			++name;
		}
		putc('\'', stream);
	}
}

void ILDumpIdentifier(FILE *stream, const char *name,
					  const char *namespace, int flags)
{
	if(namespace && *namespace != '\0')
	{
		DumpIdentifier(stream, namespace, flags);
		putc('.', stream);
	}
	DumpIdentifier(stream, name, flags);
}

void ILDumpClassName(FILE *stream, ILImage *image, ILClass *info, int flags)
{
	ILType *synType;
	ILProgramItem *scope;
	ILAssembly *assem;
	ILModule *module;
	ILClass *importInfo;
	const char *assemName;

	/* If the class is synthesized, then dump the underlying type instead */
	synType = ILClass_SynType(info);
	if(synType)
	{
		ILDumpType(stream, image, synType, flags);
		return;
	}

	/* Do we need to add the "class" or "valuetype" prefix? */
	if((flags & IL_DUMP_CLASS_PREFIX) != 0)
	{
		if(ILClassIsValueType(info))
		{
			fputs("valuetype ", stream);
		}
		else
		{
			fputs("class ", stream);
		}
	}

	/* Determine how the type is imported */
	scope = ILClass_Scope(info);
	if((assem = ILProgramItemToAssembly(scope)) != 0)
	{
		/* Imported from an assembly */
		if(ILAssemblyIsRef(assem) || ILProgramItem_Image(assem) != image)
		{
			putc('[', stream);
			ILDumpIdentifier(stream, ILAssembly_Name(assem), 0, flags);
			putc(']', stream);
		}
		ILDumpIdentifier(stream, ILClassGetName(info),
						 ILClassGetNamespace(info), flags);
	}
	else if((module = ILProgramItemToModule(scope)) != 0)
	{
		/* Imported from a module */
		if(ILModuleIsRef(module))
		{
			fputs("[.module ", stream);
			ILDumpIdentifier(stream, ILModule_Name(module), 0, flags);
			putc(']', stream);
		}
		else if(ILProgramItem_Image(module) != image)
		{
			/* Use the module's assembly name if possible */
			assemName = ILImageGetAssemblyName(ILProgramItem_Image(module));
			if(assemName)
			{
				fputs("[", stream);
				ILDumpIdentifier(stream, assemName, 0, flags);
				putc(']', stream);
			}
			else
			{
				fputs("[.module ", stream);
				ILDumpIdentifier(stream, ILModule_Name(module), 0, flags);
				putc(']', stream);
			}
		}
		ILDumpIdentifier(stream, ILClassGetName(info),
						 ILClassGetNamespace(info), flags);
	}
	else if((importInfo = ILProgramItemToClass(scope)) != 0)
	{
		/* Nested within another class */
		ILDumpClassName(stream, image, importInfo,
						flags & ~IL_DUMP_CLASS_PREFIX);
		putc('/', stream);
		ILDumpIdentifier(stream, ILClassGetName(info),
						 ILClassGetNamespace(info), flags);
	}
	else
	{
		/* Don't know what the import scope is */
		ILDumpIdentifier(stream, ILClassGetName(info),
						 ILClassGetNamespace(info), flags);
	}

	/* Show the token at the end of the class name */
	if((flags & IL_DUMP_SHOW_TOKENS) != 0)
	{
		fprintf(stream, "/*%08lX*/",
				(unsigned long)(ILProgramItemGetToken(ILToProgramItem(info))));
	}
}

void ILDumpProgramItem(FILE *stream, ILImage *image,
					    ILProgramItem *item, int flags)
{
	ILClass *info;
	ILTypeSpec *spec;

	if((spec = ILProgramItemToTypeSpec(item)) != 0)
	{
		ILType *type = ILTypeSpec_Type(spec);

		ILDumpType(stream, image, type, flags);
	}
	else if((info = ILProgramItemToClass(item)) != 0)
	{
		info = ILClassResolve(info);
		ILDumpClassName(stream, image, info, flags);
	}
	else if(ILProgramItem_Token(item) == 0)
	{
		/* Might be an unresolved class reference */
		info = (ILClass *)item;
		ILDumpClassName(stream, image, info, flags);
	}
}

#ifdef	__cplusplus
};
#endif
