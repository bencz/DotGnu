/*
 * link_init.c - Handle initializers and finalizers for C applications.
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

#include "linker.h"
#if HAVE_SYS_TYPES_H
	#include <sys/types.h>
#endif
#if HAVE_SYS_STAT_H
	#include <sys/stat.h>
#endif
#if HAVE_UNISTD_H
	#include <unistd.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/* From ilasm/ilasm_main.c */
int ILAsmMain(int argc, char *argv[], FILE *newStdin);

/*
 * Data structures for keeping track of initializers and finalizers.
 */
typedef struct _tagILInitFini ILInitFini;
struct _tagILInitFini
{
	ILMethod   *method;
	ILInitFini *next;
};
typedef struct _tagILOrder ILOrder;
struct _tagILOrder
{
	int			order;
	ILInitFini *first;
	ILInitFini *last;
	ILOrder	   *next;
};
typedef struct
{
	ILOrder	   *inits;
	ILOrder	   *finis;

} ILInitFiniList;

/*
 * Get an order value from a method attribute.
 */
static int GetInitFiniOrder(ILMethod *method, const char *name)
{
	ILAttribute *attr;
	ILSerializeReader *reader;
	int order;

	/* Get the attribute block */
	attr = ILLinkerFindAttribute(ILToProgramItem(method),
								 name, "OpenSystem.C",
								 ILType_Int32, ILType_Invalid);
	if(!attr)
	{
		/* The default order value is zero */
		return 0;
	}

	/* Read the attribute's value */
	reader = ILLinkerReadAttribute(attr);
	if(!reader)
	{
		return 0;
	}
	order = 0;
	if(ILSerializeReaderGetParamType(reader) == IL_META_ELEMTYPE_I4)
	{
		order = (int)ILSerializeReaderGetInt32(reader, IL_META_ELEMTYPE_I4);
	}
	ILSerializeReaderDestroy(reader);
	return order;
}

/*
 * Insert an initializer into an init/fini list.
 */
static void InsertInitializer(ILLinker *linker, ILInitFiniList *list,
							  ILMethod *method, int order)
{
	ILOrder *current, *prev;
	ILInitFini *info;

	/* Find the list head for this order value */
	current = list->inits;
	prev = 0;
	while(current != 0 && current->order < order)
	{
		prev = current;
		current = current->next;
	}
	if(!current || current->order != order)
	{
		current = (ILOrder *)ILMalloc(sizeof(ILOrder));
		if(!current)
		{
			_ILLinkerOutOfMemory(linker);
			return;
		}
		current->order = order;
		current->first = 0;
		current->last = 0;
		if(prev)
		{
			current->next = prev->next;
			prev->next = current;
		}
		else
		{
			current->next = list->inits;
			list->inits = current;
		}
	}

	/* Add the initializer to the end of the list for this order */
	info = (ILInitFini *)ILMalloc(sizeof(ILInitFini));
	if(!info)
	{
		_ILLinkerOutOfMemory(linker);
		return;
	}
	info->method = method;
	info->next = 0;
	if(current->last)
	{
		current->last->next = info;
		current->last = info;
	}
	else
	{
		current->first = info;
		current->last = info;
	}
}

/*
 * Insert a finalizer into an init/fini list.
 */
static void InsertFinalizer(ILLinker *linker, ILInitFiniList *list,
							ILMethod *method, int order)
{
	ILOrder *current, *prev;
	ILInitFini *info;

	/* Find the list head for this order value */
	current = list->inits;
	prev = 0;
	while(current != 0 && current->order > order)
	{
		prev = current;
		current = current->next;
	}
	if(!current || current->order != order)
	{
		current = (ILOrder *)ILMalloc(sizeof(ILOrder));
		if(!current)
		{
			_ILLinkerOutOfMemory(linker);
			return;
		}
		current->order = order;
		current->first = 0;
		current->last = 0;
		if(prev)
		{
			current->next = prev->next;
			prev->next = current;
		}
		else
		{
			current->next = list->inits;
			list->inits = current;
		}
	}

	/* Add the finalizer to the start of the list for this order */
	info = (ILInitFini *)ILMalloc(sizeof(ILInitFini));
	if(!info)
	{
		_ILLinkerOutOfMemory(linker);
		return;
	}
	info->method = method;
	info->next = current->first;
	if(!(current->first))
	{
		current->last = info;
	}
	current->first = info;
}

/*
 * Call a list of initializer or finalizer methods.
 */
static void CallInitFini(FILE *stream, ILOrder *list)
{
	ILInitFini *current;
	ILMethod *method;
	while(list != 0)
	{
		current = list->first;
		while(current != 0)
		{
			fputs("\tcall ", stream);
			method = current->method;
			ILDumpMethodType(stream, ILProgramItem_Image(method),
							 ILMethod_Signature(method),
							 IL_DUMP_QUOTE_NAMES, 0,
							 ILMethod_Name(method), 0);
			putc('\n', stream);
			current = current->next;
		}
		list = list->next;
	}
}

/*
 * Free a list of initializer or finalizer methods.
 */
static void FreeInitFini(ILOrder *list)
{
	ILOrder *next;
	ILInitFini *init, *nextInit;
	while(list != 0)
	{
		next = list->next;
		init = list->first;
		while(init != 0)
		{
			nextInit = init->next;
			ILFree(init);
			init = nextInit;
		}
		ILFree(list);
		list = next;
	}
}

/*
 * Call library initializers.
 */
static void CallLibraryInit(FILE *stream, ILLibrary *library)
{
	while(library != 0)
	{
		if(library->isCImage)
		{
			fprintf(stream, "\tcall void [%s]'%s'::'.init'()\n",
					library->name, library->moduleName);
		}
		library = library->next;
	}
}

/*
 * Call library finalizers, in reverse order.
 */
static void CallLibraryFini(FILE *stream, ILLibrary *library)
{
	if(!library)
	{
		return;
	}
	CallLibraryFini(stream, library->next);
	if(library->isCImage)
	{
		fprintf(stream, "\tcall void [%s]'%s'::'.fini'()\n",
				library->name, library->moduleName);
	}
}

ILImage *_ILLinkerCreateInitFini(ILLinker *linker)
{
	ILMethod *method;
	ILAttribute *attr;
	int order;
	ILInitFiniList list;
	FILE *stream;
	char iltemp[64];
	char otemp[64];
	char *argv[10];
	ILImage *image;

	/* Only generate initializers and finalizers for C applications.
	   C# applications will need to explicitly call the initializers
	   using helper methods within the "OpenSystem.C" assembly */
	if(!(linker->isCLink))
	{
		return 0;
	}

	/* Collect up all initializers and finalizers in the image */
	method = 0;
	list.inits = 0;
	list.finis = 0;
	while((method = (ILMethod *)ILImageNextToken
				(linker->image, IL_META_TOKEN_METHOD_DEF, method)) != 0)
	{
		attr = ILLinkerFindAttribute(ILToProgramItem(method),
									 "InitializerAttribute",
									 "OpenSystem.C",
									 ILType_Invalid, ILType_Invalid);
		if(attr)
		{
			/* This is an initializer */
			order = GetInitFiniOrder(method, "InitializerOrderAttribute");
			InsertInitializer(linker, &list, method, order);
		}
		attr = ILLinkerFindAttribute(ILToProgramItem(method),
									 "FinalizerAttribute",
									 "OpenSystem.C",
									 ILType_Invalid, ILType_Invalid);
		if(attr)
		{
			/* This is a finalizer */
			order = GetInitFiniOrder(method, "FinalizerOrderAttribute");
			InsertFinalizer(linker, &list, method, order);
		}
	}

	/* Open a temporary stream to write the init/fini code to */
#if !defined(IL_WIN32_NATIVE) && defined(HAVE_GETPID)
	sprintf(iltemp, "init-link-%d.iftmp", (int)(getpid()));
	sprintf(otemp, "init-link-%d.oftmp", (int)(getpid()));
#else
	strcpy(iltemp, "init-link.iftmp");
	strcpy(otemp, "init-link.oftmp");
#endif
	stream = fopen(iltemp, "w");
	if(!stream)
	{
		perror(iltemp);
		linker->error = 1;
		return 0;
	}

	/* Generate assembly, module, and memory model information */
	fputs(".assembly '<Assembly>' {}\n", stream);
	fputs(".module '<Module>'\n", stream);
	fprintf(stream, ".custom instance void "
			"[OpenSystem.C]OpenSystem.C.ModuleAttribute::.ctor() = "
			"(01 00 00 00)\n");
	fputs("\n", stream);

	/* Dump the "init-count" helper class, which keeps track
	   of how many times the library has been initialized */
	fputs(".class private sealed 'init-count' extends "
			"[.library]System.Object\n", stream);
	fputs("{\n", stream);
	fputs("\t.field public static int32 'count'\n", stream);
	fputs("}\n\n", stream);

	/* Generate the code for the global initializer */
	fputs(".method public specialname static void '.init'() cil managed\n",
		  stream);
	fputs("{\n", stream);
	fputs("\t.maxstack 3\n", stream);
	fputs("\t.locals init (class [.library]System.Type)\n", stream);
	fputs("\tldtoken 'init-count'\n", stream);
	fputs("\tcall class [.library]System.Type [.library]System.Type::"
		  "GetTypeFromHandle(valuetype [.library]System.RuntimeTypeHandle)\n",
		  stream);
	fputs("\tdup\n", stream);
	fputs("\tstloc.0\n", stream);
	fputs("\tcall void [.library]System.Threading.Monitor::Enter"
		  "(class [.library]System.Object)\n", stream);
	fputs("\t.try {\n", stream);
	fputs("\t\tldsfld int32 'init-count'::'count'\n", stream);
	fputs("\t\tdup\n", stream);
	fputs("\t\tldc.i4.1\n", stream);
	fputs("\t\tadd\n", stream);
	fputs("\t\tstsfld int32 'init-count'::'count'\n", stream);
	fputs("\t\tbrtrue L1\n", stream);
	fputs("\t\tleave L2\n", stream);
	fputs("\tL1:\n", stream);
	fputs("\t\tleave L3\n", stream);
	fputs("\t} finally {\n", stream);
	fputs("\t\tldloc.0\n", stream);
	fputs("\t\tcall void [.library]System.Threading.Monitor::Exit"
		  "(class [.library]System.Object)\n", stream);
	fputs("\t\tendfinally\n", stream);
	fputs("\t}\n", stream);
	fputs("L2:\n", stream);
	CallLibraryInit(stream, linker->libraries);
	CallInitFini(stream, list.inits);
	fputs("L3:\n", stream);
	fputs("\tret\n", stream);
	fputs("}\n\n", stream);

	/* Generate the code for the global finalizer */
	fputs(".method public specialname static void '.fini'() cil managed\n",
		  stream);
	fputs("{\n", stream);
	fputs("\t.maxstack 3\n", stream);
	fputs("\t.locals init (class [.library]System.Type)\n", stream);
	fputs("\tldtoken 'init-count'\n", stream);
	fputs("\tcall class [.library]System.Type [.library]System.Type::"
		  "GetTypeFromHandle(valuetype [.library]System.RuntimeTypeHandle)\n",
		  stream);
	fputs("\tdup\n", stream);
	fputs("\tstloc.0\n", stream);
	fputs("\tcall void [.library]System.Threading.Monitor::Enter"
		  "(class [.library]System.Object)\n", stream);
	fputs("\t.try {\n", stream);
	fputs("\t\tldsfld int32 'init-count'::'count'\n", stream);
	fputs("\t\tldc.i4.1\n", stream);
	fputs("\t\tsub\n", stream);
	fputs("\t\tdup\n", stream);
	fputs("\t\tstsfld int32 'init-count'::'count'\n", stream);
	fputs("\t\tbrtrue L4\n", stream);
	fputs("\t\tleave L5\n", stream);
	fputs("\tL4:\n", stream);
	fputs("\t\tleave L6\n", stream);
	fputs("\t} finally {\n", stream);
	fputs("\t\tldloc.0\n", stream);
	fputs("\t\tcall void [.library]System.Threading.Monitor::Exit"
		  "(class [.library]System.Object)\n", stream);
	fputs("\t\tendfinally\n", stream);
	fputs("\t}\n", stream);
	fputs("L5:\n", stream);
	CallInitFini(stream, list.finis);
	CallLibraryFini(stream, linker->libraries);
	fputs("L6:\n", stream);
	fputs("\tret\n", stream);
	fputs("}\n\n", stream);

	/* Generate the "init-on-demand" class, which is used to force
	   initialization of global variables when a C library is used
	   from C# code and the global initializer wasn't called by crt0 */
	fputs(".class private sealed beforefieldinit 'init-on-demand' extends "
			"[.library]System.Object\n", stream);
	fputs("{\n", stream);
	fputs("\t.method private static hidebysig specialname rtspecialname "
				"void .cctor() cil managed\n", stream);
	fputs("\t{\n", stream);
	fputs("\t\t.maxstack 2\n", stream);
	fputs("\t\t.locals init (class [.library]System.Type)\n", stream);
	fputs("\t\tldtoken 'init-count'\n", stream);
	fputs("\t\tcall class [.library]System.Type [.library]System.Type::"
		  "GetTypeFromHandle(valuetype [.library]System.RuntimeTypeHandle)\n",
		  stream);
	fputs("\t\tdup\n", stream);
	fputs("\t\tstloc.0\n", stream);
	fputs("\t\tcall void [.library]System.Threading.Monitor::Enter"
		  "(class [.library]System.Object)\n", stream);
	fputs("\t\t.try {\n", stream);
	fputs("\t\t\tldsfld int32 'init-count'::'count'\n", stream);
	fputs("\t\t\tbrtrue L7\n", stream);
	fputs("\t\t\tleave L8\n", stream);
	fputs("\t\tL7:\n", stream);
	fputs("\t\t\tleave L9\n", stream);
	fputs("\t\t} finally {\n", stream);
	fputs("\t\t\tldloc.0\n", stream);
	fputs("\t\t\tcall void [.library]System.Threading.Monitor::Exit"
		  "(class [.library]System.Object)\n", stream);
	fputs("\t\t\tendfinally\n", stream);
	fputs("\t\t}\n", stream);
	fputs("\tL8:\n", stream);
	fputs("\t\tcall void '.init'()\n", stream);
	fputs("\tL9:\n", stream);
	fputs("\t\tret\n", stream);
	fputs("\t}\n", stream);
	fputs("}\n", stream);

	/* Close the temporary stream and assemble it */
	fclose(stream);
	argv[0] = "ilasm";
	argv[1] = "-j";
	argv[2] = "-o";
	argv[3] = otemp;
	argv[4] = "--";
	argv[5] = iltemp;
	argv[6] = 0;
	if(ILAsmMain(6, argv, 0) != 0)
	{
		ILDeleteFile(iltemp);
		ILDeleteFile(otemp);
		linker->error = 1;
		return 0;
	}
	ILDeleteFile(iltemp);

	/* Load the extra object file that we just created */
	image = 0;
	if(ILImageLoadFromFile(otemp, linker->context, &image,
						   IL_LOADFLAG_FORCE_32BIT |
						   IL_LOADFLAG_NO_RESOLVE, 1) != 0)
	{
		ILDeleteFile(otemp);
		return 0;
	}
	if((linker->initTempFile = ILDupString(otemp)) == 0)
	{
		_ILLinkerOutOfMemory(linker);
	}

	/* Clean up and exit */
	FreeInitFini(list.inits);
	FreeInitFini(list.finis);
	return image;
}

#ifdef	__cplusplus
};
#endif
