/*
 * csant_cscc.c - Task dispatch for launching C# compilers.
 *
 * Copyright (C) 2001, 2002, 2003  Southern Storm Software, Pty Ltd.
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

#include "csant_defs.h"
#include "csant_fileset.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Find the pathname for a particular program.
 */
static char *FindProgramPath(const char *name, const char *env)
{
	const char *value;

	/* Check for the "name" property */
	value = CSAntGetProperty(name, -1);
	if(value)
	{
		return (char *)value;
	}

	/* Check for the "env" environment variable */
	value = CSAntGetProperty(env, -1);
	if(value)
	{
		return (char *)value;
	}

	/* Assume that it is "name" somewhere on the path */
	return (char *)name;
}

/*
 * Find the pathname for "cscc".
 */
static char *FindCsccPath(void)
{
	return FindProgramPath("cscc", "csant.env.CSCC");
}

/*
 * Add an argument to an argv-style list.
 */
static void AddArg(char ***argv, int *argc, char *value)
{
	if((*argc & 7) == 0)
	{
		*argv = (char **)ILRealloc(*argv, (*argc + 8) * sizeof(char *));
		if(!(*argv))
		{
			CSAntOutOfMemory();
		}
	}
	(*argv)[(*argc)++] = value;
}

/*
 * Add an argument to an argv-style list if it isn't already present.
 */
static void AddUnique(char ***argv, int *argc, const char *value, int len)
{
	int posn;
	char *copyValue;
	for(posn = 0; posn < *argc; ++posn)
	{
		if(!strncmp((*argv)[posn], value, len) &&
		   (*argv)[posn][len] == '\0')
		{
			return;
		}
	}
	copyValue = (char *)ILMalloc(len + 1);
	if(!copyValue)
	{
		CSAntOutOfMemory();
	}
	ILMemCpy(copyValue, value, len);
	copyValue[len] = '\0';
	AddArg(argv, argc, copyValue);
}

/*
 * Add an argument and value to an argv-style list.
 */
static void AddValueArg(char ***argv, int *argc, char *name, char *value)
{
	char *str = (char *)ILMalloc(strlen(name) + strlen(value) + 1);
	if(!str)
	{
		CSAntOutOfMemory();
	}
	strcpy(str, name);
	strcat(str, value);
	AddArg(argv, argc, str);
}

/*
 * Add an argument and length-specified value to an argv-style list.
 */
static void AddValueLenArg(char ***argv, int *argc, char *name,
						   char *value, int len)
{
	char *str = (char *)ILMalloc(strlen(name) + len + 1);
	if(!str)
	{
		CSAntOutOfMemory();
	}
	strcpy(str, name);
	ILMemCpy(str + strlen(name), value, len);
	str[strlen(name) + len] = '\0';
	AddArg(argv, argc, str);
}

/*
 * Free an argv-style list.
 */
static void FreeArgs(char **argv, int argc)
{
	int posn;
	for(posn = 0; posn < argc; ++posn)
	{
		if(argv[posn] != 0)
		{
			ILFree(argv[posn]);
		}
	}
	if(argv)
	{
		ILFree(argv);
	}
}

/*
 * Construct a semi-colon separated list of values.
 */
static char *SemiColonList(char *list, char *value)
{
	if(!list)
	{
		list = ILDupString(value);
		if(!list)
		{
			CSAntOutOfMemory();
		}
		return list;
	}
	else
	{
		list = (char *)ILRealloc(list, strlen(list) + strlen(value) + 2);
		if(!list)
		{
			CSAntOutOfMemory();
		}
		strcat(list, ";");
		strcat(list, value);
		return list;
	}
}

/*
 * Determine if a filanem ends in a particular extension.
 */
static int EndsIn(const char *filename, const char *extension)
{
	int flen = strlen(filename);
	int elen = strlen(extension);
	if(elen < flen && !ILStrICmp(filename + flen - elen, extension))
	{
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Flag values.
 */
#define	COMP_FLAG_NOT_SET		-1
#define	COMP_FLAG_TRUE			1
#define	COMP_FLAG_FALSE			0

/*
 * Arguments for executing a C# compiler.
 */
typedef struct
{
	char		   *output;
	const char	   *target;
	const char	   *version;
	int				debug;
	int				checked;
	int				unsafe;
	int				noStdLib;
	int				addConfig;
	int				optimize;
	int				warnAsError;
	int				saneWarnings;
	int				install;
	int				installAsDefault;
	CSAntFileSet   *sources;
	CSAntFileSet   *references;
	CSAntFileSet   *resources;
	CSAntFileSet   *modules;
	char		  **defines;
	int				numDefines;
	char		  **args;
	int				numArgs;

} CSAntCompileArgs;

/*
 * Free a set of compile arguments.
 */
static void FreeCompileArgs(CSAntCompileArgs *args)
{
	if(args->output)
	{
		ILFree(args->output);
	}
	CSAntFileSetDestroy(args->sources);
	CSAntFileSetDestroy(args->references);
	CSAntFileSetDestroy(args->resources);
	CSAntFileSetDestroy(args->modules);
	FreeArgs(args->defines, args->numDefines);
	FreeArgs(args->args, args->numArgs);
}

/*
 * Parse compile arguments from a task block.  Returns zero on error.
 */
static int ParseCompileArgs(CSAntTask *task, CSAntCompileArgs *args,
							int isCsc, const char *compilerName)
{
	const char *value;
	const char *value2;
	char *copyValue;
	CSAntTask *node;
	int index;

	/* Initialize the arguments */
	args->output = (char *)CSAntTaskParam(task, "output");
	args->target = CSAntTaskParam(task, "target");
	args->version = CSAntTaskParam(task, "version");
	args->debug = COMP_FLAG_NOT_SET;
	args->checked = COMP_FLAG_NOT_SET;
	args->unsafe = COMP_FLAG_NOT_SET;
	args->noStdLib = COMP_FLAG_NOT_SET;
	args->addConfig = COMP_FLAG_NOT_SET;
	args->optimize = COMP_FLAG_NOT_SET;
	args->warnAsError = COMP_FLAG_NOT_SET;
	args->saneWarnings = COMP_FLAG_NOT_SET;
	args->install = COMP_FLAG_NOT_SET;
	args->installAsDefault = COMP_FLAG_TRUE;
	args->sources = CSAntFileSetLoad(task, "sources", CSAntBaseSrcDir);
	args->references = CSAntFileSetLoad(task, "references", CSAntBaseBuildDir);
	args->resources = CSAntFileSetLoad(task, "resources", CSAntBaseBuildDir);
	args->modules = CSAntFileSetLoad(task, "modules", CSAntBaseBuildDir);
	args->defines = 0;
	args->numDefines = 0;
	args->args = 0;
	args->numArgs = 0;

	/* Validate "output" and "target" */
	if(!(args->output))
	{
		fprintf(stderr, "%s: no output specified\n", task->name);
		FreeCompileArgs(args);
		return 0;
	}
	args->output = CSAntDirCombine(CSAntBaseBuildDir, args->output);
	if(!(args->target))
	{
		if(isCsc)
		{
			fprintf(stderr, "%s: no target type specified\n", task->name);
			FreeCompileArgs(args);
			return 0;
		}
		if(EndsIn(args->output, ".dll"))
		{
			args->target = "library";
		}
		else
		{
			args->target = "exe";
		}
	}

	/* We need at least one source file */
	if(CSAntFileSetSize(args->sources) == 0)
	{
		fprintf(stderr, "%s: no source files specified\n", task->name);
		FreeCompileArgs(args);
		return 0;
	}

	/* Set the flag values */
	value = CSAntTaskParam(task, "debug");
	if(value)
	{
		args->debug = !ILStrICmp(value, "true");
	}
	value = CSAntTaskParam(task, "checked");
	if(value)
	{
		args->checked = !ILStrICmp(value, "true");
	}
	value = CSAntTaskParam(task, "unsafe");
	if(value)
	{
		args->unsafe = !ILStrICmp(value, "true");
	}
	value = CSAntTaskParam(task, "nostdlib");
	if(value)
	{
		args->noStdLib = !ILStrICmp(value, "true");
	}
	value = CSAntTaskParam(task, "optimize");
	if(value)
	{
		args->optimize = !ILStrICmp(value, "true");
	}
	value = CSAntTaskParam(task, "warnaserror");
	if(value)
	{
		args->warnAsError = !ILStrICmp(value, "true");
	}
	value = CSAntTaskParam(task, "sanewarnings");
	if(value)
	{
		args->saneWarnings = !ILStrICmp(value, "true");
	}
	value = CSAntTaskParam(task, "install");
	if(value)
	{
		args->install = !ILStrICmp(value, "true");
	}
	value = CSAntTaskParam(task, "installasdefault");
	if(value)
	{
		args->installAsDefault = !ILStrICmp(value, "true");
	}

	/* Get the list of symbol definitions */
	value = CSAntTaskParam(task, "define");
	if(value)
	{
		while(*value != '\0')
		{
			if(*value == ' ' || *value == ';' || *value == ',')
			{
				++value;
				continue;
			}
			value2 = value;
			while(*value != '\0' && *value != ' ' &&
			      *value != ';' && *value != ',')
			{
				++value;
			}
			AddUnique(&(args->defines), &(args->numDefines), value2,
					  (int)(value - value2));
		}
	}
	node = task->taskChildren;
	while(node != 0)
	{
		if(!strcmp(node->name, "define"))
		{
			value = CSAntTaskParam(node, "name");
			value2 = CSAntTaskParam(node, "value");
			if(value && value2 && !ILStrICmp(value2, "true"))
			{
				AddUnique(&(args->defines), &(args->numDefines), value,
						  strlen(value));
			}
		}
		node = node->next;
	}

	/* Add definitions from the profile */
	for(index = 0; index < CSAntNumProfileDefines; ++index)
	{
		if(!ILStrICmp(CSAntProfileValues[index], "true"))
		{
			AddUnique(&(args->defines), &(args->numDefines),
					  CSAntProfileDefines[index],
					  strlen(CSAntProfileDefines[index]));
		}
	}

	/* Define "DEBUG" and "TRACE" if debugging is enabled */
	if(args->debug == COMP_FLAG_TRUE)
	{
		AddUnique(&(args->defines), &(args->numDefines), "DEBUG", 5);
		AddUnique(&(args->defines), &(args->numDefines), "TRACE", 5);
	}

	/* Collect up additional arguments */
	if(!CSAntRedirectCsc)
	{
		node = task->taskChildren;
		while(node != 0)
		{
			if(!strcmp(node->name, "arg"))
			{
				/* Does this argument only apply to a specific compiler? */
				value = CSAntTaskParam(node, "compiler");
				if(!value || !ILStrICmp(value, compilerName))
				{
					value = CSAntTaskParam(node, "value");
					if(value)
					{
						copyValue = ILDupString(value);
						if(!copyValue)
						{
							CSAntOutOfMemory();
						}
						AddArg(&(args->args), &(args->numArgs), copyValue);
					}
				}
			}
			node = node->next;
		}
	}
	else
	{
		/* We are simulating csc, so we need to parse out some
		   of the csc-style options in the "arg" elements */
		args->addConfig = COMP_FLAG_TRUE;
		node = task->taskChildren;
		while(node != 0)
		{
			if(!strcmp(node->name, "arg"))
			{
				/* Does this argument only apply to a specific compiler? */
				value = CSAntTaskParam(node, "value");
				if(value)
				{
					if(!ILStrICmp(value, "/unsafe"))
					{
						args->unsafe = COMP_FLAG_TRUE;
					}
					else if(!ILStrICmp(value, "/nostdlib"))
					{
						args->noStdLib = COMP_FLAG_TRUE;
					}
					else if(!ILStrICmp(value, "/noconfig"))
					{
						args->addConfig = COMP_FLAG_FALSE;
					}
					else if(!strncmp(value, "/r:", 3) ||
					        !strncmp(value, "/R:", 3))
					{
						args->references = CSAntFileSetAdd
							(args->references, value + 3);
					}
				}
			}
			node = node->next;
		}
	}

	/* Done */
	return 1;
}

/*
 * Build a command-line for "cscc".
 */
static char **BuildCsccCommandLine(CSAntCompileArgs *args)
{
	char **argv = 0;
	int argc = 0;
	int posn;
	unsigned long numFiles;
	unsigned long file;
	char *temp;
	int len;

	/* Add the program name */
	AddArg(&argv, &argc, FindCsccPath());

	/* Add the explicitly-specified locations of "cscc-cs" and "cscc-c" */
	temp = (char *)CSAntGetProperty("cscc.plugins.cs", -1);
	if(temp)
	{
		AddValueArg(&argv, &argc, "-fplugin-cs-path=", temp);
	}
	temp = (char *)CSAntGetProperty("cscc.plugins.c", -1);
	if(temp)
	{
		AddValueArg(&argv, &argc, "-fplugin-c-path=", temp);
	}

	/* Set the output file */
	AddArg(&argv, &argc, "-o");
	AddArg(&argv, &argc, (char *)(args->output));

	/* Enable debugging if necessary */
	if(args->debug == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "-g");
	}

	/* Set the checked compilation state */
	if(args->checked == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "-fchecked");
	}
	else if(args->checked == COMP_FLAG_FALSE)
	{
		AddArg(&argv, &argc, "-funchecked");
	}

	/* Set the unsafe compilation state */
	if(args->unsafe == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "-funsafe");
	}

	/* Disable the standard library if necessary */
	if(args->noStdLib == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "-nostdlib");
	}

	/* Set the optimization level */
	if(args->optimize == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "-O2");
	}
	else if(args->optimize == COMP_FLAG_FALSE)
	{
		AddArg(&argv, &argc, "-O0");
	}

	/* Convert warnings into errors if requested */
	if(args->warnAsError == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "-Werror");
	}

	/* Turn on sane warning modes */
	if(args->saneWarnings == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "-Wno-empty-input");
	}

	/* Define pre-processor symbols */
	for(posn = 0; posn < args->numDefines; ++posn)
	{
		AddValueArg(&argv, &argc, "-D", args->defines[posn]);
	}

	/* We need to force Latin-1 if we are simulating csc */
	if(CSAntRedirectCsc)
	{
		AddArg(&argv, &argc, "-flatin1-charset");
	}

	/* Add "-fstdlib-name=corlib" if forced to do so */
	if(CSAntForceCorLib)
	{
		AddArg(&argv, &argc, "-fstdlib-name=corlib");
	}

	/* Add the assembly version information */
	if(args->version)
	{
		AddValueArg(&argv, &argc, "-fassembly-version",
					(char *)(args->version));
	}

	/* Add any extra arguments that were supplied */
	for(posn = 0; posn < args->numArgs; ++posn)
	{
		AddArg(&argv, &argc, args->args[posn]);
	}

	/* Add the resources to the command-line */
	numFiles = CSAntFileSetSize(args->resources);
	for(file = 0; file < numFiles; ++file)
	{
		AddValueArg(&argv, &argc, "-fresources=",
					CSAntFileSetFile(args->resources, file));
	}

	/* Add the modules to the command-line */
	numFiles = CSAntFileSetSize(args->modules);
	for(file = 0; file < numFiles; ++file)
	{
		AddValueArg(&argv, &argc, "-fmodule=",
					CSAntFileSetFile(args->modules, file));
	}

	/* Add the source files to the command-line */
	numFiles = CSAntFileSetSize(args->sources);
	for(file = 0; file < numFiles; ++file)
	{
		AddArg(&argv, &argc, CSAntFileSetFile(args->sources, file));
	}

	/* Add the library references to the command-line */
	numFiles = CSAntFileSetSize(args->references);
	for(file = 0; file < numFiles; ++file)
	{
		temp = CSAntFileSetFile(args->references, file);
		len = strlen(temp);
		while(len > 0 && temp[len - 1] != '/' && temp[len - 1] != '\\')
		{
			--len;
		}
		if(len > 0)
		{
			if(len == 1)
			{
				AddValueLenArg(&argv, &argc, "-L", temp, 1);
			}
			else
			{
				AddValueLenArg(&argv, &argc, "-L", temp, len - 1);
			}
		}
		if(EndsIn(temp + len, ".dll"))
		{
			AddValueLenArg(&argv, &argc, "-l", temp + len,
						   strlen(temp + len) - 4);
		}
		else
		{
			AddValueArg(&argv, &argc, "-l", temp + len);
		}
	}

	/* Add "-lSystem.Xml" and "-lSystem" if necessary to simulate csc */
	if(args->addConfig == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "-lSystem.Xml");
		AddArg(&argv, &argc, "-lSystem");
	}

	/* Terminate the command-line */
	AddArg(&argv, &argc, (char *)0);

	/* Done */
	return argv;
}

/*
 * Build a command-line for "csc".
 */
static char **BuildCscCommandLine(CSAntCompileArgs *args)
{
	char **argv = 0;
	int argc = 0;
	int posn, len, len2;
	unsigned long numFiles;
	unsigned long file;
	char *temp;
	char *temp2;

	/* Add the program name and fixed options */
	AddArg(&argv, &argc, FindProgramPath("csc", "csant.env.CSC"));
	AddArg(&argv, &argc, "/nologo");

	/* Enable debugging if necessary */
	if(args->debug == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "/debug");
	}

	/* Set the checked compilation state */
	if(args->checked == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "/checked+");
	}
	else if(args->checked == COMP_FLAG_FALSE)
	{
		AddArg(&argv, &argc, "/checked-");
	}

	/* Set the unsafe compilation state */
	if(args->unsafe == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "/unsafe");
	}

	/* Disable the standard library if necessary */
	if(args->noStdLib == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "/nostdlib");
		AddArg(&argv, &argc, "/noconfig");
	}

	/* Set the optimization level */
	if(args->optimize == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "/optimize+");
	}
	else if(args->optimize == COMP_FLAG_FALSE)
	{
		AddArg(&argv, &argc, "/optimize-");
	}

	/* Convert warnings into errors if requested */
	if(args->warnAsError == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "/warnaserror+");
	}

	/* Set the target and output file */
	AddValueArg(&argv, &argc, "/target:", (char *)(args->target));
	temp = CSAntDirCombineWin32(args->output, 0, 0);
	AddValueArg(&argv, &argc, "/out:", temp);
	ILFree(temp);

	/* Turn on sane warning modes */
	if(args->saneWarnings == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "/nowarn:626");
		AddArg(&argv, &argc, "/nowarn:649");
		AddArg(&argv, &argc, "/nowarn:168");
		AddArg(&argv, &argc, "/nowarn:67");
		AddArg(&argv, &argc, "/nowarn:169");
		AddArg(&argv, &argc, "/nowarn:679");
	}

	/* Define pre-processor symbols */
	temp = 0;
	for(posn = 0; posn < args->numDefines; ++posn)
	{
		temp = SemiColonList(temp, args->defines[posn]);
	}
	if(temp != 0)
	{
		AddValueArg(&argv, &argc, "/define:", temp);
	}

	/* Add the resources to the command-line */
	numFiles = CSAntFileSetSize(args->resources);
	for(file = 0; file < numFiles; ++file)
	{
		temp = CSAntFileSetFile(args->resources, file);
		len = len2 = strlen(temp);
		while(len > 0 && temp[len - 1] != '/' && temp[len - 1] != '\\')
		{
			--len;
		}
		if(len > 0)
		{
			temp2 = (char *)ILMalloc(len2 + (len2 - len) + 2);
			if(!temp2)
			{
				CSAntOutOfMemory();
			}
			strcpy(temp2, temp);
			strcat(temp2, ",");
			strcat(temp2, temp + len);
			AddValueArg(&argv, &argc, "/resource:", temp2);
			ILFree(temp2);
		}
		else
		{
			AddValueArg(&argv, &argc, "/resource:", temp);
		}
	}

	/* Add the references to the command-line */
	numFiles = CSAntFileSetSize(args->references);
	temp = 0;
	for(file = 0; file < numFiles; ++file)
	{
		temp = SemiColonList(temp,
					CSAntDirCombineWin32
						(CSAntFileSetFile(args->references, file), 0, 0));
	}
	if(temp != 0)
	{
		AddValueArg(&argv, &argc, "/reference:", temp);
	}

	/* Add the modules to the command-line */
	numFiles = CSAntFileSetSize(args->modules);
	temp = 0;
	for(file = 0; file < numFiles; ++file)
	{
		temp = SemiColonList(temp,
					CSAntDirCombineWin32
						(CSAntFileSetFile(args->modules, file), 0, 0));
	}
	if(temp != 0)
	{
		AddValueArg(&argv, &argc, "/addmodule:", temp);
	}

	/* Add any extra arguments that were supplied */
	for(posn = 0; posn < args->numArgs; ++posn)
	{
		AddArg(&argv, &argc, args->args[posn]);
	}

	/* Add the source files to the command-line */
	numFiles = CSAntFileSetSize(args->sources);
	for(file = 0; file < numFiles; ++file)
	{
		AddArg(&argv, &argc,
			   CSAntDirCombineWin32
			   		(CSAntFileSetFile(args->sources, file), 0, 0));
	}

	/* Terminate the command-line */
	AddArg(&argv, &argc, (char *)0);

	/* Done */
	return argv;
}

/*
 * Build a command-line for "mcs".
 */
static char **BuildMcsCommandLine(CSAntCompileArgs *args)
{
	char **argv = 0;
	int argc = 0;
	int posn;
	unsigned long numFiles;
	unsigned long file;
	char *temp, *temp2;
	int len, len2;

	/* Add the program name */
	AddArg(&argv, &argc, FindProgramPath("mcs", "csant.env.MCS"));

	/* Set the target and output file */
	AddValueArg(&argv, &argc, "-target:", (char *)(args->target));
	AddValueArg(&argv, &argc, "-out:", (char *)(args->output));

	/* Enable debugging if necessary */
	if(args->debug == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "-debug+");
	}

	/* Set the checked compilation state */
	if(args->checked == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "/checked+");
	}
	else if(args->checked == COMP_FLAG_FALSE)
	{
		AddArg(&argv, &argc, "/checked-");
	}

	/* Set the unsafe compilation state */
	if(args->unsafe == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "/unsafe");
	}

	/* Disable the standard library if necessary */
	if(args->noStdLib == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "-nostdlib");
	}

	/* Set the optimization level */
	if(args->optimize == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "-optimize+");
	}
	else if(args->optimize == COMP_FLAG_FALSE)
	{
		AddArg(&argv, &argc, "-optimize-");
	}

	/* Convert warnings into errors if requested */
	if(args->warnAsError == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "-warnaserror+");
	}

	/* Add any extra arguments that were supplied */
	for(posn = 0; posn < args->numArgs; ++posn)
	{
		AddArg(&argv, &argc, args->args[posn]);
	}

	/* Add the resources to the command-line */
	numFiles = CSAntFileSetSize(args->resources);
	for(file = 0; file < numFiles; ++file)
	{
		temp = CSAntFileSetFile(args->resources, file);
		len = len2 = strlen(temp);
		while(len > 0 && temp[len - 1] != '/' && temp[len - 1] != '\\')
		{
			--len;
		}
		if(len > 0)
		{
			temp2 = (char *)ILMalloc(len2 + (len2 - len) + 2);
			if(!temp2)
			{
				CSAntOutOfMemory();
			}
			strcpy(temp2, temp);
			strcat(temp2, ",");
			strcat(temp2, temp + len);
			AddValueArg(&argv, &argc, "-resource:", temp2);
			ILFree(temp2);
		}
		else
		{
			AddValueArg(&argv, &argc, "-resource:", temp);
		}
	}

	/* Add the source files to the command-line */
	numFiles = CSAntFileSetSize(args->sources);
	for(file = 0; file < numFiles; ++file)
	{
		AddArg(&argv, &argc, CSAntFileSetFile(args->sources, file));
	}

	/* Turn on sane warning modes */
	if(args->saneWarnings == COMP_FLAG_TRUE)
	{
		AddArg(&argv, &argc, "-nowarn:626,649,168,67,169,679");
	}

	/* Define pre-processor symbols */
	temp = 0;
	for(posn = 0; posn < args->numDefines; ++posn)
	{
		temp = SemiColonList(temp, args->defines[posn]);
	}
	if(temp != 0)
	{
		AddValueArg(&argv, &argc, "-define:", temp);
	}

	/* Add the references to the command-line */
	numFiles = CSAntFileSetSize(args->references);
	temp = 0;
	for(file = 0; file < numFiles; ++file)
	{
		temp = SemiColonList(temp,
					CSAntDirCombine	(CSAntFileSetFile(args->references, file), 0));
	}
	if(temp != 0)
	{
		AddValueArg(&argv, &argc, "-reference:", temp);
	}


	/* Terminate the command-line */
	AddArg(&argv, &argc, (char *)0);

	/* Done */
	return argv;
}

/*
 * Print and execute a command line.
 */
static int PrintAndExecute(char **argv)
{
	/* Print the command to be executed */
	if(!CSAntSilent)
	{
		int argc = 0;
		while(argv[argc] != 0)
		{
			fputs(argv[argc], stdout);
			++argc;
			if(argv[argc] != 0)
			{
				putc(' ', stdout);
			}
		}
		putc('\n', stdout);
	}

	/* Execute the command */
	if(!CSAntJustPrint)
	{
		return (ILSpawnProcess(argv) == 0);
	}
	else
	{
		return 1;
	}
}

/*
 * Build a command-line to install an assembly.
 */
static char **BuildInstallLine(const char *output, const char *subdir,
							   int installAsDefault)
{
	char **argv = 0;
	int argc = 0;

	/* Add the name of the "ilgac" program */
	AddArg(&argv, &argc, FindProgramPath("ilgac", "csant.env.ILGAC"));

	/* Add the command-line options */
	/*AddArg(&argv, &argc, "--silent");*/
	AddArg(&argv, &argc, "--install");
	AddArg(&argv, &argc, "--force");
	if(installAsDefault)
	{
		AddArg(&argv, &argc, "--default");
	}
	if(subdir)
	{
		AddArg(&argv, &argc, "--subdir");
		AddArg(&argv, &argc, (char *)subdir);
	}
	if(CSAntCacheDir)
	{
		AddArg(&argv, &argc, "--cache");
		AddArg(&argv, &argc, (char *)CSAntCacheDir);
	}

	/* Add the name of the assembly to install */
	AddArg(&argv, &argc, (char *)output);

	/* Terminate the command-line */
	AddArg(&argv, &argc, 0);
	return argv;
}

/*
 * Build a command-line to uninstall an assembly.
 */
static char **BuildUninstallLine(const char *output, const char *version,
								 const char *subdir)
{
	char **argv = 0;
	int argc = 0;

	/* Add the name of the "ilgac" program */
	AddArg(&argv, &argc, FindProgramPath("ilgac", "csant.env.ILGAC"));

	/* Add the command-line options */
	/*AddArg(&argv, &argc, "--silent");*/
	AddArg(&argv, &argc, "--uninstall");
	if(subdir)
	{
		AddArg(&argv, &argc, "--subdir");
		AddArg(&argv, &argc, (char *)subdir);
	}
	if(CSAntCacheDir)
	{
		AddArg(&argv, &argc, "--cache");
		AddArg(&argv, &argc, (char *)CSAntCacheDir);
	}

	/* Add the name of the assembly to uninstall */
	AddArg(&argv, &argc, (char *)output);

	/* Add the version number for the assembly */
	if(version)
	{
		AddArg(&argv, &argc, (char *)version);
	}

	/* Terminate the command-line */
	AddArg(&argv, &argc, 0);
	return argv;
}

/*
 * Build a command-line and execute it.
 */
static int BuildAndExecute(CSAntCompileArgs *args,
						   char **(*func)(CSAntCompileArgs *))
{
	char **argv;
	int result;

	/* Check the timestamps on the input and output files */
	if(!CSAntInstallMode && !CSAntUninstallMode)
	{
		if(!CSAntFileSetNewer(args->sources, args->output) &&
		   !CSAntFileSetNewer(args->references, args->output) &&
		   !CSAntFileSetNewer(args->resources, args->output) &&
		   !CSAntFileSetNewer(args->modules, args->output))
		{
			return 1;
		}
	}

	/* Build the command-line using the supplied function */
	if(CSAntInstallMode)
	{
		if(args->install != COMP_FLAG_TRUE)
		{
			/* Ignore targets that are not marked for installation */
			return 1;
		}
		argv = BuildInstallLine(args->output, 0, (args->installAsDefault > 0));
	}
	else if(CSAntUninstallMode)
	{
		if(args->install != COMP_FLAG_TRUE)
		{
			/* Ignore targets that are not marked for installation */
			return 1;
		}
		argv = BuildUninstallLine(args->output, args->version, 0);
	}
	else
	{
		argv = (*func)(args);
	}

	/* Print and execute the command */
	result = PrintAndExecute(argv);

	/* Clean up and exit */
	ILFree(argv);
	FreeCompileArgs(args);
	return result;
}

/*
 * Handle a "cscc" task, which invokes the Portable.NET C# compiler.
 */
int CSAntTask_Cscc(CSAntTask *task)
{
	CSAntCompileArgs args;

	/* Parse the arguments */
	if(!ParseCompileArgs(task, &args, 0, "cscc"))
	{
		return 0;
	}

	/* Execute the command */
	return BuildAndExecute(&args, BuildCsccCommandLine);
}

/*
 * Handle a "csc" task, which invokes the Microsoft C# compiler.
 */
int CSAntTask_Csc(CSAntTask *task)
{
	if(CSAntRedirectCsc)
	{
		/* Redirect NAnt-style <csc> tags to the <compile> functionality */
		return CSAntTask_Compile(task);
	}
	else
	{
		CSAntCompileArgs args;

		/* Parse the arguments */
		if(!ParseCompileArgs(task, &args, 1, "csc"))
		{
			return 0;
		}

		/* Execute the command */
		return BuildAndExecute(&args, BuildCscCommandLine);
	}
}

/*
 * Handle a "mcs" task, which invokes the Mono C# compiler.
 */
int CSAntTask_Mcs(CSAntTask *task)
{
	CSAntCompileArgs args;

	/* Parse the arguments */
	if(!ParseCompileArgs(task, &args, 0, "msc"))
	{
		return 0;
	}

	/* Execute the command */
	return BuildAndExecute(&args, BuildMcsCommandLine);
}

/*
 * Handle a "compile" task, which invokes the configured C# compiler.
 */
int CSAntTask_Compile(CSAntTask *task)
{
	CSAntCompileArgs args;
	const char *compiler = CSAntGetProperty("csant.compiler", -1);

	/* Parse the arguments */
	if(!ParseCompileArgs(task, &args, 0, compiler))
	{
		return 0;
	}

	/* Execute the command */
	if(!ILStrICmp(compiler, "cscc"))
	{
		return BuildAndExecute(&args, BuildCsccCommandLine);
	}
	else if(!ILStrICmp(compiler, "csc"))
	{
		return BuildAndExecute(&args, BuildCscCommandLine);
	}
	else if(!ILStrICmp(compiler, "mcs"))
	{
		return BuildAndExecute(&args, BuildMcsCommandLine);
	}
	else
	{
		fprintf(stderr, "%s: unknown compiler name\n", compiler);
		return 0;
	}
}

/*
 * Handle a "csdoc" task, which invokes the documentation generator.
 */
int CSAntTask_Csdoc(CSAntTask *task)
{
	const char *value;
	char *temp;
	char *output;
	char *library;
	CSAntFileSet *sources;
	CSAntFileSet *references;
	int dumpPrivate = 0;
	char **argv = 0;
	int argc = 0;
	unsigned long numFiles;
	unsigned long file;
	int len, result;
	FILE *outfile;

	/* Parse the parameters to the task */
	output = (char *)CSAntTaskParam(task, "output");
	library = (char *)CSAntTaskParam(task, "library");
	sources = CSAntFileSetLoad(task, "sources", CSAntBaseSrcDir);
	references = CSAntFileSetLoad(task, "references", CSAntBaseBuildDir);
	value = CSAntTaskParam(task, "private");
	if(value)
	{
		dumpPrivate = !ILStrICmp(value, "true");
	}

	/* Validate the parameters */
	if(!output)
	{
		CSAntFileSetDestroy(sources);
		CSAntFileSetDestroy(references);
		fprintf(stderr, "%s: no output specified\n", task->name);
		return 0;
	}
	if(!CSAntFileSetSize(sources))
	{
		CSAntFileSetDestroy(sources);
		CSAntFileSetDestroy(references);
		fprintf(stderr, "%s: no sources specified\n", task->name);
		return 0;
	}

	/* If the sources are not newer than the output, then bail out */
	if(!CSAntFileSetNewer(sources, output))
	{
		CSAntFileSetDestroy(sources);
		CSAntFileSetDestroy(references);
		return 1;
	}

	/* If the "--dummy-doc" flag was set on the csant command-line,
	   then create a dummy output file.  This can happen if the
	   system does not have "csdoc" installed */
	if(CSAntDummyDoc)
	{
		CSAntFileSetDestroy(sources);
		CSAntFileSetDestroy(references);
		if(!CSAntSilent)
		{
			printf("Creating dummy documentation file: %s\n", output);
		}
		if(!CSAntJustPrint)
		{
			outfile = fopen(output, "w");
			if(outfile)
			{
				/* Output the dummy XML */
				fputs("<Libraries>\n<Types Library=\"Dummy\">\n", outfile);
				fputs("</Types>\n</Libraries>\n", outfile);
				fclose(outfile);
				return 1;
			}
			else
			{
				/* The output file could not be created */
				perror(output);
				return 0;
			}
		}
		else
		{
			return 1;
		}
	}

	/* Add the program name */
	AddArg(&argv, &argc, FindProgramPath("csdoc", "csant.env.CSDOC"));

	/* Set the output file */
	AddArg(&argv, &argc, "-o");
	AddArg(&argv, &argc, (char *)output);

	/* Add the library name */
	if(library && *library != '\0')
	{
		AddValueArg(&argv, &argc, "-flibrary-name=", library);
	}

	/* Dump the private definitions as well if requested */
	if(dumpPrivate)
	{
		AddArg(&argv, &argc, "-fprivate");
	}

	/* Add the source files to the command-line */
	numFiles = CSAntFileSetSize(sources);
	for(file = 0; file < numFiles; ++file)
	{
		AddArg(&argv, &argc, CSAntFileSetFile(sources, file));
	}

	/* Add the library references to the command-line */
	numFiles = CSAntFileSetSize(references);
	for(file = 0; file < numFiles; ++file)
	{
		temp = CSAntFileSetFile(references, file);
		len = strlen(temp);
		while(len > 0 && temp[len - 1] != '/' && temp[len - 1] != '\\')
		{
			--len;
		}
		if(len > 0)
		{
			if(len == 1)
			{
				AddValueLenArg(&argv, &argc, "-L", temp, 1);
			}
			else
			{
				AddValueLenArg(&argv, &argc, "-L", temp, len - 1);
			}
		}
		if(EndsIn(temp + len, ".dll"))
		{
			AddValueLenArg(&argv, &argc, "-l", temp + len,
						   strlen(temp + len) - 4);
		}
		else
		{
			AddValueArg(&argv, &argc, "-l", temp + len);
		}
	}

	/* Terminate the command-line */
	AddArg(&argv, &argc, (char *)0);

	/* Print and execute the command */
	result = PrintAndExecute(argv);

	/* Clean up and exit */
	ILFree(argv);
	CSAntFileSetDestroy(sources);
	CSAntFileSetDestroy(references);
	return result;
}

/*
 * Handle a "resgen" task, which invokes the resource converter.
 */
int CSAntTask_ResGen(CSAntTask *task)
{
	char *output;
	char *input;
	char *temp;
	CSAntFileSet *inputs;
	int isLatin1;
	const char *compiler;
	char **argv = 0;
	int argc = 0;
	unsigned long numFiles;
	unsigned long file;
	int result;

	/* Get the output name */
	output = (char *)CSAntTaskParam(task, "output");
	if(!output)
	{
		fprintf(stderr, "%s: no output specified\n", task->name);
		return 0;
	}
	output = CSAntDirCombine(CSAntBaseBuildDir, output);

	/* Get the list of input files */
	inputs = CSAntFileSetLoad(task, "resources", CSAntBaseSrcDir);
	input = (char *)CSAntTaskParam(task, "input");
	if(input)
	{
		input = CSAntDirCombine(CSAntBaseSrcDir, input);
		inputs = CSAntFileSetAdd(inputs, input);
	}
	if(!CSAntFileSetSize(inputs))
	{
		CSAntFileSetDestroy(inputs);
		fprintf(stderr, "%s: no inputs specified\n", task->name);
		return 0;
	}

	/* Bail out if none of the input files are newer than the output */
	if(!CSAntFileSetNewer(inputs, output))
	{
		CSAntFileSetDestroy(inputs);
		return 1;
	}

	/* Determine if we should use Latin1 conversion */
	compiler = CSAntGetProperty("csant.compiler", -1);
	if(compiler && !ILStrICmp(compiler, "cscc"))
	{
		/* We are probably using our resgen */
		temp = (char *)CSAntTaskParam(task, "latin1");
		if(temp && !ILStrICmp(temp, "true"))
		{
			isLatin1 = 1;
		}
		else
		{
			isLatin1 = 0;
		}
	}
	else
	{
		/* Cannot rely upon other resgen's having Latin1 support */
		isLatin1 = 0;
	}

	/* Build the command-line to be executed */
	AddArg(&argv, &argc, FindProgramPath("resgen", "csant.env.RESGEN"));
	if(isLatin1)
	{
		AddArg(&argv, &argc, "--latin1");
	}
	else
	{
		AddArg(&argv, &argc, "/compile");
	}
	numFiles = CSAntFileSetSize(inputs);
	for(file = 0; file < numFiles; ++file)
	{
		temp = CSAntFileSetFile(inputs, file);
		AddArg(&argv, &argc, temp);
	}
	AddArg(&argv, &argc, output);
	AddArg(&argv, &argc, (char *)0);

	/* Print and execute the command */
	result = PrintAndExecute(argv);

	/* Clean up and exit */
	ILFree(argv);
	CSAntFileSetDestroy(inputs);
	return result;
}

/*
 * Handle a "reslink" task, which links resources into a standalone assembly.
 */
int CSAntTask_ResLink(CSAntTask *task)
{
	char *output;
	char *language;
	char *version;
	char *metadataVersion;
	char *temp;
	CSAntFileSet *resources;
	const char *compiler;
	char **argv = 0;
	int argc = 0;
	unsigned long numFiles;
	unsigned long file;
	int result;
	int install;
	int installAsDefault;

	/* Get the option values */
	output = (char *)CSAntTaskParam(task, "output");
	if(!output)
	{
		fprintf(stderr, "%s: no output specified\n", task->name);
		return 0;
	}
	output = CSAntDirCombine(CSAntBaseBuildDir, output);
	language = (char *)CSAntTaskParam(task, "language");
	if(!language)
	{
		language = "en";
	}
	version = (char *)CSAntTaskParam(task, "version");
	metadataVersion = (char *)CSAntTaskParam(task, "metadataVersion");
	resources = CSAntFileSetLoad(task, "resources", CSAntBaseBuildDir);
	temp = (char *)CSAntTaskParam(task, "install");
	install = (temp && !ILStrICmp(temp, "true"));
	temp = (char *)CSAntTaskParam(task, "installasdefault");
	if(temp)
	{
		installAsDefault = !ILStrICmp(temp, "true");
	}
	else
	{
		installAsDefault = 1;
	}

	/* Check that we have at least one input file */
	if(!CSAntFileSetSize(resources))
	{
		CSAntFileSetDestroy(resources);
		fprintf(stderr, "%s: no resource files specified\n", task->name);
		return 0;
	}

	/* Bail out if none of the input files are newer than the output */
	if(!CSAntInstallMode && !CSAntUninstallMode)
	{
		if(!CSAntFileSetNewer(resources, output))
		{
			CSAntFileSetDestroy(resources);
			return 1;
		}
	}

	/* Build the required command-line */
	if(CSAntInstallMode)
	{
		if(!install)
		{
			CSAntFileSetDestroy(resources);
			return 1;
		}
		argv = BuildInstallLine(output, language, installAsDefault);
	}
	else if(CSAntUninstallMode)
	{
		if(!install)
		{
			CSAntFileSetDestroy(resources);
			return 1;
		}
		argv = BuildUninstallLine(output, version, language);
	}
	else
	{
		/* Link the resources as specified */
		compiler = CSAntGetProperty("csant.compiler", -1);
		if(compiler && !ILStrICmp(compiler, "cscc"))
		{
			/* Use the cscc C# compiler to do the work */
			AddArg(&argv, &argc, FindCsccPath());
			AddArg(&argv, &argc, "-nostdlib");
			if(version && version[0] != '\0')
			{
				AddValueArg(&argv, &argc, "-fassembly-version=", version);
			}
			if(metadataVersion && metadataVersion[0] != '\0')
			{
				AddValueArg(&argv, &argc, "-fmetadata-version=",
							metadataVersion);
			}
			AddArg(&argv, &argc, "-o");
			AddArg(&argv, &argc, output);
			numFiles = CSAntFileSetSize(resources);
			for(file = 0; file < numFiles; ++file)
			{
				AddValueArg(&argv, &argc, "-fresources=",
							CSAntFileSetFile(resources, file));
			}
			AddValueArg(&argv, &argc, "-fculture=", language);
		}
		else
		{
			/* Use the Microsoft "al" linker to do the work */
			AddArg(&argv, &argc, FindProgramPath("al", "csant.env.AL"));
			AddArg(&argv, &argc, "/nologo");
			AddArg(&argv, &argc, "/target:library");
			if(version && version[0] != '\0')
			{
				AddValueArg(&argv, &argc, "/version:", version);
			}
			AddValueArg(&argv, &argc, "/out:", output);
			AddValueArg(&argv, &argc, "/culture:", language);
			numFiles = CSAntFileSetSize(resources);
			for(file = 0; file < numFiles; ++file)
			{
				AddValueArg(&argv, &argc, "/embed:",
							CSAntFileSetFile(resources, file));
			}
		}
		AddArg(&argv, &argc, (char *)0);
	}

	/* Print and execute the command */
	result = PrintAndExecute(argv);

	/* Clean up and exit */
	ILFree(argv);
	CSAntFileSetDestroy(resources);
	return 1;
}

#ifdef	__cplusplus
};
#endif
