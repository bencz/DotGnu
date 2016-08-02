/*
 * ilcheck.c Check runtime calls against the mscorlib.dll
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
 *
 * Hacked by Gopal.V from ilnative.c
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

#include <stdio.h>
#include "il_system.h"
#include "il_image.h"
#include "il_dumpasm.h"
#include "il_utils.h"
#include "il_engine.h"
#define OUT_OF_MEMORY() fprintf(stderr,"%s: out of memory \n",progname);
#ifdef	__cplusplus
extern	"C" {
#endif

static char* progname;

typedef struct _ILDict
{
	char *name;
	void *info;
	struct _ILDict *methods;
	struct _ILDict *prev;
}ILDict;

ILDict * ILDict_new(ILDict *prev,char *name)
{
	ILDict *retval=(ILDict*)calloc(1,sizeof(ILDict));
	if(!retval)OUT_OF_MEMORY();
	if(name!=NULL)retval->name=name;
	retval->prev=prev;
	return retval;
}

ILDict * ILDict_search(ILDict *end,char *name)
{
	ILDict *i;
	for(i=end;i!=NULL;i=i->prev)
	{
		if(!strcmp(i->name,name))return i;
	}
	return NULL;
}

static ILDict *klasses;

/*
 * Table of command-line options.
 */
static ILCmdLineOption const options[] = {
	{"-v", 'v', 0, 0, 0},
	{"-a",'a',0,0,0},
	{"--all",'a',0,"-a or --all"," output info for all methods"},
	{"--xml",'x',0,"--xml","output XML instead of human-readable text"},
	{"-L",'l',2,"-L","add library to search path"},
	{"--version", 'v', 0,
		"--version    or -v",
		"Print the version of the program."},
	{"--help", 'h', 0,
		"--help",
		"Print this help message."},
	{0, 0, 0, 0, 0}
};

static void usage(const char *progname);
static void version(void);
static int printNatives(const char *filename, ILContext *context,
						int xml,int all);

//snitched from some of rhys's code
static void AddString(char ***list, int *num, char *str)
{
	char **newlist = (char **)ILRealloc(*list, sizeof(char *) * (*num + 1));
	if(!newlist)
	{
		OUT_OF_MEMORY();
		return;
	}
	*list = newlist;
	(*list)[*num] = str;
	++(*num);
}


int main(int argc, char *argv[])
{
	int sawStdin;
	int state, opt;
	char *param;
	int errors,xml=0,all=0;
	ILContext *context;
	char **libraryDirs=NULL;
	int numLibraryDirs=0;
	progname = argv[0];

	/* Parse the command-line arguments */
	state = 0;
	while((opt = ILCmdLineNextOption(&argc, &argv, &state,
									 options, &param)) != 0)
	{
		switch(opt)
		{
			case 'v':
			{
				version();
				return 0;
			}
			/* Not reached */
			case 'x':
			{
				xml=1;
				break;
			}
			case 'a':
			{
				all=1;
				break;
			}
#ifdef DEBUG
			case -1:
			{
				fprintf(stderr,"Got <%c> instead of valid option\n",opt);
				return 0;
				break;
			}
#endif
			case 'l':
			{
				AddString(&(libraryDirs),&(numLibraryDirs),param);
				break;
			}
			default:
			{
				usage(progname);
				return 1;
			}
			/* Not reached */
		}
	}

	/* We need at least one input file argument */
	if(argc <= 1)
	{
		usage(progname);
		return 1;
	}

	context = ILContextCreate();
	/* Create a context to use for image loading */
	if(!context)
	{
		fprintf(stderr, "%s: out of memory\n", progname);
		return 1;
	}
	ILContextSetLibraryDirs(context,libraryDirs,numLibraryDirs);
	/* Load and print information about the input files */
	sawStdin = 0;
	errors = 0;
	while(argc > 1)
	{
		if(!strcmp(argv[1], "-"))
		{
			/* Dump the contents of stdin, but only once */
			if(!sawStdin)
			{
				errors |= printNatives("-", context, xml,all);
				sawStdin = 1;
			}
		}
		else
		{
			/* Dump the contents of a regular file */
			errors |= printNatives(argv[1], context, xml,all);
		}
		++argv;
		--argc;
	}

	/* Destroy the context */
	ILContextDestroy(context);
	
	/* Done */
	return errors;
}

static void usage(const char *progname)
{
	fprintf(stdout, "ILCHECK " VERSION " - IL PInvoke Check \n");
	fprintf(stdout, "Copyright (c) 2001 Southern Storm Software, Pty Ltd.\n");
	fprintf(stdout, "          (c) 2002 FSF India.\n");
	fprintf(stdout, "\n");
	fprintf(stdout, "Usage: %s [options] input ...\n", progname);
	fprintf(stdout, "\n");
	ILCmdLineHelp(options);
}

static void version(void)
{

	printf("ILCHECK " VERSION " - IL PInvoke Check \n");
	printf("Copyright (c) 2001 Southern Storm Software, Pty Ltd.\n");
	printf("          (c) 2002 FSF India.\n");
	printf("\n");
	printf("ILCHECK comes with ABSOLUTELY NO WARRANTY.  This is free software,\n");
	printf("and you are welcome to redistribute it under the terms of the\n");
	printf("GNU General Public License.  See the file COPYING for further details.\n");
	printf("\n");
	printf("Use the `--help' option to get help on the command-line options.\n");
}


/*
 * Dump information about a native method.
 */
static void dumpMethodInfo(ILImage *image, ILMethod *method,char *stat,int xml)
{
	/* Dump the method attributes */
	if(xml)
	{
		printf("\t <pinvoke name=\"%s\" ",ILMethod_Name(method));
		printf("class = \"%s.%s\" ",
				ILClass_Namespace(ILMethod_Owner(method)),
				ILClass_Name(ILMethod_Owner(method)));
		printf(" signature=\" ");
		/* Dump the method signature */
		ILDumpMethodType(stdout, image, ILMethod_Signature(method), 0,
					 ILMethod_Owner(method), ILMethod_Name(method),
					 method);
		/* Terminate the line */
		printf("\" status=\"%s\"/>\n",stat);
	}
	else
	{
		ILDumpFlags(stdout, ILMethod_Attrs(method),
				ILMethodDefinitionFlags, 0);

		/* Dump the method signature */
		ILDumpMethodType(stdout, image, ILMethod_Signature(method), 0,
					 ILMethod_Owner(method), ILMethod_Name(method),
					 method);
		putc(' ', stdout);

		/* Dump the implementation flags */
		ILDumpFlags(stdout, ILMethod_ImplAttrs(method),
				ILMethodImplementationFlags, 0);

		/* Terminate the line */
		putc('\n', stdout);
	}
}
static void addmethodinfo(ILImage *image, ILMethod *method,char *stat)
{
	/* Dump the method attributes */
	char *name=(char*)ILClass_Name(ILMethod_Owner(method));
	ILDict *this_class;	
	if(!ILDict_search(klasses,name))
	{	
		klasses=ILDict_new(klasses,name);
	}
	this_class=ILDict_search(klasses,name);
	this_class->methods=ILDict_new(this_class->methods,stat);
	this_class->methods->info=method;
}

static int FindPInvoke(ILMethod *method)
{
	ILPInvoke *pinvoke;
	void *handle;
	char *module_name=NULL;
	if(!method)return 0;
	pinvoke=ILPInvokeFind(method);
	if(!pinvoke)return 0;
	module_name=ILPInvokeResolveModule(pinvoke);
	if(!ILFileExists(module_name,NULL))
	{
		fprintf(stderr,"%s: File does not exist '%s' \n",progname,module_name);
		return -1;
	}
	handle=ILDynLibraryOpen(module_name);
	if(!handle)
	{
		fprintf(stderr,"%s: Could not dlopen '%s' \n",progname,module_name);
		return -1;
	}
	if(!ILDynLibraryGetSymbol(handle,ILMethod_Name(method)))
	{
		return 0;
		//the func will throw it's own error message :-(
	}
	ILDynLibraryClose(handle);
	return 1;
}
/*
 * Load an IL image an display the native methods.
 */
static int printNatives(const char *filename, ILContext *context,
						int xml,int all)
{
	ILImage *image;
	unsigned long numMethods;
	unsigned long token;
	ILMethod *method;
	
	ILDict *klass, *mthd;
	int error;
	
	klasses=ILDict_new(NULL,"<start>");
	/* Attempt to load the image into memory */
	if(ILImageLoadFromFile(filename, context, &image,
					  	   IL_LOADFLAG_FORCE_32BIT |
					  	   IL_LOADFLAG_NO_RESOLVE, 1) != 0)
	{
		return 1;
	}

	/* Walk the MethodDef table and print all internalcalls that are
	   missing from the internalcall table */
	numMethods = ILImageNumTokens(image, IL_META_TOKEN_METHOD_DEF);
	for(token = 1; token <= numMethods; ++token)
	{
		method = (ILMethod *)ILImageTokenInfo
									(image, IL_META_TOKEN_METHOD_DEF | token);
		if(method)
		{
			if(ILMethod_HasPInvokeImpl(method))
			{								
				error=FindPInvoke(method);
				if(error==-1)
				{
					break;
				}
				else if(!error)
				{
					addmethodinfo(image,method,"MISSING");
				}
				else if(all)
				{
					addmethodinfo(image,method,"OK");
				}
			}
		}
	}
	if(xml)printf("<PInvokeCallStatus>\n");
	for(klass=klasses;klass->prev!=NULL;klass=klass->prev)
	{
		if(xml)
		{
			printf("<class namespace=\"%s\" name=\"%s\">\n",ILClass_Namespace(ILMethod_Owner(klass->methods->info)),klass->name);
		}
		for(mthd=klass->methods;mthd!=NULL;mthd=mthd->prev)
		{
			dumpMethodInfo(image,mthd->info,mthd->name,xml);
		}
		if(xml)
		{
			printf("</class>\n");
		}
	}
	if(xml)printf("</PInvokeCallStatus>\n");
	/* Clean up and exit */
	ILImageDestroy(image);
	return 0;
}

#ifdef	__cplusplus
};
#endif
