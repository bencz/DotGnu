/*
 * cc_main.c - Main entry point for a "cscc" compiler plug-in.
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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

#include "cc_main.h"
#include "cc_preproc.h"
#ifdef HAVE_SYS_TYPES_H
	#include <sys/types.h>
#endif
#ifdef HAVE_UNISTD_H
	#include <unistd.h>
#endif
#ifdef HAVE_SYS_WAIT_H
	#include <sys/wait.h>
#endif
#ifndef WEXITSTATUS
	#define	WEXITSTATUS(status)		((unsigned)(status) >> 8)
#endif
#ifndef WIFEXITED
	#define	WIFEXITED(status)		(((status) & 255) == 0)
#endif
#ifndef WTERMSIG
	#define	WTERMSIG(status)		(((unsigned)(status)) & 0x7F)
#endif
#ifndef WIFSIGNALLED
	#define	WIFSIGNALLED(status)	(((status) & 255) != 0)
#endif
#ifndef WCOREDUMP
	#define	WCOREDUMP(status)		(((status) & 0x80) != 0)
#endif
#include <signal.h>

#ifdef _WIN32
#include <process.h>
#include <io.h>
#include <fcntl.h>
#ifndef P_NOWAIT
#define P_NOWAIT _P_NOWAIT
#endif
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Global variables that are exported to the plugin.
 */
ILGenInfo CCCodeGen;
ILNode *CCParseTree;
ILNode *CCParseTreeEnd;
ILNode *CCStandaloneAttrs;
ILNode *CCStandaloneAttrsEnd;

/*
 * Forward declarations.
 */
static void PreprocessClose(void);
static int InitCodeGen(void);
static void CloseCodeGen(void);
static void ParseFile(const char *filename, int is_stdin);

/*
 * State for the pre-processor.
 */
CCPreProc CCPreProcessorStream;
static FILE *preproc_outfile = 0;
static int preproc_is_stdout = 0;
static const char *preproc_filename = 0;

int CCMain(int argc, char *argv[])
{
	int saw_stdin;
	int posn;
	int status;

	/* Parse the command-line options */
	CCParseCommandLine(argc, argv, CCPluginOptionParseMode,
					   (char *)CCPluginName);

	/* Initialize the plugin */
	if(!CCPluginInit())
	{
		return 1;
	}

	/* Initialize the code generator if we aren't pre-processing */
	if(!preprocess_flag)
	{
		status = InitCodeGen();
		if(status != 0)
		{
			CCPluginShutdown(status);
			return status;
		}
	}
	else if(CCPluginUsesPreproc == CC_PREPROC_NONE)
	{
		/* We can't use a pre-processor for this language */
		fprintf(stderr, _("%s: pre-processing is not supported\n"), progname);
		CCPluginShutdown(1);
		return 1;
	}

	/* Generate modules and assemblies now if instructed to do so early */
	if(CCPluginGenModulesEarly)
	{
		ILGenModulesAndAssemblies(&CCCodeGen);
	}

	/* Parse all of the input files */
	saw_stdin = 0;
	CCParseTree = 0;
	for(posn = 0; posn < num_input_files; ++posn)
	{
		if(!strcmp(input_files[posn], "-"))
		{
			if(!saw_stdin)
			{
				ParseFile("stdin", 1);
				saw_stdin = 1;
			}
		}
		else
		{
			ParseFile(input_files[posn], 0);
		}
	}

	/* If we are only pre-processing, then we are finished */
	if(preprocess_flag)
	{
		PreprocessClose();
		CCPluginShutdown(CCHaveErrors != 0);
		return (CCHaveErrors != 0);
	}

	/* If we are only performing a syntax check, then bail out now */
	if(CCStringListContains(extension_flags, num_extension_flags,
							"syntax-check"))
	{
		CloseCodeGen();
		CCPluginShutdown(CCHaveErrors != 0);
		return (CCHaveErrors != 0);
	}

	/* Bail out if nothing was parsed, and we have errors */
	if(!CCParseTree && CCHaveErrors != 0)
	{
		CloseCodeGen();
		CCPluginShutdown(1);
		return 1;
	}

	CCCodeGen.semAnalysis = -1;
	CCCodeGen.optimizeFlag= (0x03) & optimize_flag;
	CCPluginSemAnalysis();
	CCCodeGen.semAnalysis = 0;

	/* If we are only performing a semantic check, then bail out now */
	if(CCStringListContains(extension_flags, num_extension_flags,
							"semantic-check"))
	{
		CloseCodeGen();
		CCPluginShutdown(CCHaveErrors != 0);
		return (CCHaveErrors != 0);
	}

	/* Generate the code if no errors occurred in the previous phases */
	if(CCHaveErrors == 0 && !CCPluginSkipCodeGen)
	{
		if(!CCPluginGenModulesEarly)
		{
			ILGenModulesAndAssemblies(&CCCodeGen);
		}
		if(CCCodeGen.outputIsJava)
		{
			JavaGenDiscard(CCParseTree, &CCCodeGen);
		}
		else
		{
			ILNode_GenDiscard(CCParseTree, &CCCodeGen);
		}
	}

	/* Perform post code generation tasks */
	CCPluginPostCodeGen();

	/* Close the code generator */
	CloseCodeGen();

	/* Done */
	CCPluginShutdown(CCHaveErrors != 0);
	return (CCHaveErrors != 0);
}

/*
 * Pre-process the input stream and write it directly to the output stream.
 */
static int Preprocess(void)
{
	FILE *outfile;
	int is_stdout;
	char buffer[BUFSIZ];
	int size;
	const char *filename;
	CCPreProcSymbol *symbol;
	const char *expectedFile;
	unsigned long expectedLine;

	/* Open the output stream */
	if(!preproc_outfile)
	{
		if(!output_filename || !strcmp(output_filename, "-"))
		{
			outfile = stdout;
			is_stdout = 1;
			filename = "stdout";
		}
		else
		{
			if((outfile = fopen(output_filename, "w")) == NULL)
			{
				perror(output_filename);
				return 1;
			}
			is_stdout = 0;
			filename = output_filename;
		}
		preproc_outfile = outfile;
		preproc_filename = filename;
		preproc_is_stdout = is_stdout;
	}
	else
	{
		outfile = preproc_outfile;
		filename = preproc_filename;
	}

	/* If the pre-processor is in C mode, then copy what it gave us */
	if(CCPluginUsesPreproc == CC_PREPROC_C)
	{
		while((size = fread(buffer, 1, sizeof(buffer),
							CCPreProcessorStream.stream)) > 0)
		{
			if(fwrite(buffer, 1, size, outfile) != (unsigned)size)
			{
				goto truncated;
			}
		}
		return 0;
	}

	/* Warn about the -C option, which we don't support yet */
	if(preproc_comments_flag)
	{
		fprintf(stderr, _("%s: warning: -C is not yet supported by "
						  "the C# pre-processor\n"), progname);
	}

	/* Copy the pre-processed data to the output stream */
	expectedFile = 0;
	expectedLine = 0;
	while((size = CCPreProcGetBuffer(&CCPreProcessorStream, buffer, BUFSIZ))
						> 0)
	{
		if(dump_output_format != DUMP_MACROS_ONLY)
		{
			if(!no_preproc_lines_flag)
			{
				/* Output line number information if it has changed */
				if(expectedFile != CCPreProcessorStream.lexerFileName ||
				   expectedLine != CCPreProcessorStream.lexerLineNumber)
				{
					expectedLine = CCPreProcessorStream.lexerLineNumber;
					if(expectedFile != CCPreProcessorStream.lexerFileName)
					{
						expectedFile = CCPreProcessorStream.lexerFileName;
						if(fprintf(outfile, "#line %lu \"%s\"\n",
								   expectedLine, expectedFile) < 0)
						{
							goto truncated;
						}
					}
					else
					{
						if(fprintf(outfile, "#line %lu\n", expectedLine) < 0)
						{
							goto truncated;
						}
					}
				}
				++expectedLine;
			}
			if(fwrite(buffer, 1, size, outfile) != (unsigned)size)
			{
			truncated:
				fprintf(stderr, _("%s: output truncated\n"), filename);
				return 1;
			}
		}
	}

	/* Dump the defined macros if necessary */
	if(dump_output_format != DUMP_OUTPUT_ONLY)
	{
		symbol = CCPreProcessorStream.symbols;
		while(symbol != 0)
		{
			fputs("#define ", outfile);
			fputs(symbol->name, outfile);
			putc('\n', outfile);
			symbol = symbol->next;
		}
	}

	/* Finished with this file */
	return CCPreProcessorStream.error;
}

/*
 * Close the pre-processor output stream.
 */
static void PreprocessClose(void)
{
	if(preproc_outfile != 0 && !preproc_is_stdout)
	{
		fclose(preproc_outfile);
	}
}

/*
 * Load a library from a specific path.  Returns zero on failure.
 * If "freePath" is non-zero, then free "path" afterwards.
 */
static ILImage *LoadLibraryFromPath(const char *path, int freePath)
{
	ILImage *image;
	int loadError;

	/* Attempt to load the image */
	loadError = ILImageLoadFromFile(path, CCCodeGen.context, &image,
									IL_LOADFLAG_FORCE_32BIT, 1);
	if(loadError == 0)
	{
		if(!ILAssemblyCreateImport(CCCodeGen.image, image))
		{
			CCOutOfMemory();
		}
	}
	else
	{
		image = 0;	
	}

	/* Clean up and exit */
	if(freePath)
	{
		ILFree((char *)path);
	}
	return image;
}

/*
 * Load the contents of a library into the code generator's context.
 * Returns zero if the library load failed.
 */
static ILImage *LoadLibrary(const char *name, int nostdlib_flag,
							int later_load)
{
	int len;
	int index;
	char *path;
	ILImage *image;
	ILAssembly *assem;
	const char *assemName;
	int ch1, ch2;

	/* If the name includes a path, then don't bother searching */
	if(strchr(name, '/') != 0 || strchr(name, '\\') != 0)
	{
		return LoadLibraryFromPath(name, 0);
	}

	/* Strip the ".dll" from the name, if present */
	len = strlen(name);
	if(len >= 4 && !ILStrICmp(name + len - 4, ".dll"))
	{
		len -= 4;
	}

	/* If we already have an assembly with this name,
	   then we assume that we already have the library.
	   It could have been loaded during dynamic linking,
	   or because the programmer specified the same
	   library twice on the compiler command-line */
	image = 0;
	while((image = ILContextNextImage(CCCodeGen.context, image)) != 0)
	{
		if(image == CCCodeGen.image)
		{
			/* Skip the image that we are building, as it may
			   have the same name as a library if we had to
			   build the library in two passes (e.g. System.dll) */
			continue;
		}
		assem = ILAssembly_FromToken(image, (IL_META_TOKEN_ASSEMBLY | 1));
		if(assem)
		{
			assemName = ILAssembly_Name(assem);
			index = 0;
			while(assemName[index] != '\0' && index < len)
			{
				/* Use case-insensitive comparisons on assembly names */
				ch1 = assemName[index];
				if(ch1 >= 'a' && ch1 <= 'z')
				{
					ch1 = (ch1 - 'a' + 'A');
				}
				ch2 = name[index];
				if(ch2 >= 'a' && ch2 <= 'z')
				{
					ch2 = (ch2 - 'a' + 'A');
				}
				if(ch1 != ch2)
				{
					break;
				}
				++index;
			}
			if(assemName[index] == '\0' && index == len)
			{
				/* The assembly is already loaded */
				return image;
			}
		}
	}

	/* Search the library link path for the name */
	path = ILImageSearchPath(name, 0, 0,
							 (const char **)link_dirs, num_link_dirs,
							 0, 0, 0, 0);
	if(path)
	{
		return LoadLibraryFromPath(path, 1);
	}

	return 0;
}

/*
 * Same as LoadLibrary except that the C compiler is handled and an
 * int is returned.
 */
static int LoadLib(const char *name, int nostdlib_flag, int later_load)
{
	if(!LoadLibrary(name, nostdlib_flag, later_load))
	{
		/* If this is the C compiler, then ignore the missing library,
		   since libraries are normally fixed up at link time */
		if(CCPluginUsesPreproc == CC_PREPROC_C)
		{
			if(later_load)
			{
				return 0;
			}
			else
			{
				return 1;
			}
		}
		else
		{
			/* Could not locate the library */
			fprintf(stderr, _("%s: No such library\n"), name);
		}
		return 0;
	}
	return 1;
}

/*
 * Initialize the code generator for tree building and assembly output.
 */
static int InitCodeGen(void)
{
	FILE *outfile;
	int library;
	int useBuiltinLibrary;

	/* Attempt to open the assembly output stream */
	if(!CCStringListContains(extension_flags, num_extension_flags,
							 "syntax-check") &&
	   !CCStringListContains(extension_flags, num_extension_flags,
							 "semantic-check") &&
	   !CCPluginSkipCodeGen)
	{
		if(!output_filename || !strcmp(output_filename, "-"))
		{
			outfile = stdout;
		}
		else
		{
			if((outfile = fopen(output_filename, "w")) == NULL)
			{
				perror(output_filename);
				return 1;
			}
		}
	}
	else
	{
		/* We are performing a syntax or semantic check only, so don't
		   bother generating the assembly code output */
		outfile = NULL;
	}

	/* Determine if we need the builtin mscorlib-replacement library.
	   This is mainly of use to the "cscctest" test suite */
	useBuiltinLibrary = CCStringListContains(extension_flags,
											 num_extension_flags,
											 "builtin-library");

	/* Initialize the code generator */
	ILGenInfoInit(&CCCodeGen, progname,
				  CCStringListGetValue(extension_flags, num_extension_flags,
				  					   "target-assembly-name"),
				  outfile, useBuiltinLibrary);
	CCCodeGen.debugFlag = debug_flag ? -1 : 0;
	CCCodeGen.errFunc = CCErrorOnLineV;
	CCCodeGen.warnFunc = CCWarningOnLineV;

	/* Set the default "checked" state */
	if(CCStringListContains(extension_flags, num_extension_flags, "checked"))
	{
		CCCodeGen.overflowInsns = -1;
	}
	else if(CCStringListContains(extension_flags, num_extension_flags,
								 "unchecked"))
	{
		CCCodeGen.overflowInsns = 0;
	}
	else
	{
		CCCodeGen.overflowInsns = 0;
	}
	CCCodeGen.overflowGlobal = CCCodeGen.overflowInsns;

	/* Switch to JVM mode if necessary */
	if(CCStringListContains(machine_flags, num_machine_flags, "jvm"))
	{
		if(!CCPluginJVMSupported)
		{
			fprintf(stderr, _("%s: compiling to the JVM is not supported\n"),
				    progname);
			return 1;
		}
		ILGenInfoToJava(&CCCodeGen);
	}

	/* Load the "mscorlib" library, to get the standard library */
	if(!nostdlib_flag || (CCPluginForceStdlib && !useBuiltinLibrary))
	{
		ILImage *corlibImage;

		char *name = CCStringListGetValue(extension_flags, num_extension_flags,
										  "stdlib-name");
		if(!name)
		{
			name = "mscorlib";
		}
		if((corlibImage = LoadLibrary(name, 0, 0)) != 0)
		{
			ILContextSetSystem(CCCodeGen.context, corlibImage);
		}
		else
		{
			return 1;
		}
	}

	/* Set the library path for the context so that assembly
	   references can be resolved properly */
	if(num_link_dirs)
	{
		char **libraryDirs;
		int numLibraryDirs = 0;
		int current;

		libraryDirs = (char **)ILMalloc(sizeof(char *) * num_link_dirs);
		if(!libraryDirs)
		{
			return 1;
		}
		for(current = 0; current < num_link_dirs; current++)
		{
			if(link_dirs[current])
			{
				char *dir = ILMalloc(strlen(link_dirs[current]) + 1);
				if(dir)
				{
					strcpy(dir, link_dirs[current]);
					libraryDirs[numLibraryDirs++] = dir;
				}
			}
		}
		if(numLibraryDirs)
		{
			ILContextSetLibraryDirs(CCCodeGen.context, libraryDirs, numLibraryDirs);
		}
		else
		{
			ILFree(libraryDirs);
		}
	}

	/* Load all of the other libraries, in reverse order */
	for(library = num_libraries - 1; library >= 0; --library)
	{
		if(!LoadLib(libraries[library], nostdlib_flag, 0))
		{
			return 1;
		}
	}

	/* Ready to go now */
	return 0;
}

/*
 * Close and destroy the code generator.
 */
static void CloseCodeGen(void)
{
	if(CCCodeGen.asmOutput != stdout && CCCodeGen.asmOutput != NULL)
	{
		fclose(CCCodeGen.asmOutput);
	}
	ILGenInfoDestroy(&CCCodeGen);
}

/*
 * Find the C pre-processor along a reasonable search path.
 */
static char *FindCpp(int *ourCpp)
{
	char *newPath = 0;
	char *value;
	int len;

	/* We are looking for our cpp implementation */
	*ourCpp = 1;

	/* Check the "-fcpp-path" command-line option first */
	value = CCStringListGetValue
		(extension_flags, num_extension_flags, "cpp-path");
	if(value)
	{
		return value;
	}

	/* Try the "CSCC_CPP" environment variable */
	value = getenv("CSCC_CPP");
	if(value)
	{
		return value;
	}

	/* Look in the same directory as the "cscc" binary */
	len = strlen(progname);
	while(len > 0 && progname[len - 1] != '/' && progname[len - 1] != '\\')
	{
		--len;
	}
	if(len > 0)
	{
		value = (char *)ILMalloc(len + 15);
		if(value)
		{
			strncpy(value, progname, len);
			strcpy(value + len, "cscc-cpp");
			if(ILFileExists(value, &newPath))
			{
				if(newPath)
				{
					ILFree(value);
					return newPath;
				}
				else
				{
					return value;
				}
			}
		}
	}

	/* Search the PATH for the "cscc-cpp" program */
	value = ILSearchPath(0, "cscc-cpp", 1);
	if(value)
	{
		return value;
	}

	/* We are looking for the system cpp implementation */
	*ourCpp = 0;

	/* Try the "CPP" environment variable for the system pre-processor */
	value = getenv("CPP");
	if(value)
	{
		return value;
	}

	/* Try looking for the system pre-processor if we couldn't find ours */
	if(ILFileExists("/usr/bin/cpp3", &newPath))
	{
		if(newPath)
			return newPath;
		else
			return "/usr/bin/cpp3";
	}
	else if(ILFileExists("/usr/bin/cpp", &newPath))
	{
		if(newPath)
			return newPath;
		else
			return "/usr/bin/cpp";
	}
	else if(ILFileExists("/bin/cpp", &newPath))
	{
		if(newPath)
			return newPath;
		else
			return "/bin/cpp";
	}
	else if(ILFileExists("/usr/local/bin/cpp", &newPath))
	{
		if(newPath)
			return newPath;
		else
			return "/usr/local/bin/cpp";
	}
	else if(ILFileExists("/usr/lib/cpp", &newPath))
	{
		if(newPath)
			return newPath;
		else
			return "/usr/lib/cpp";
	}
	else if(ILFileExists("/lib/cpp", &newPath))
	{
		if(newPath)
			return newPath;
		else
			return "/lib/cpp";
	}
	else if(ILFileExists("/usr/local/lib/cpp", &newPath))
	{
		if(newPath)
			return newPath;
		else
			return "/usr/local/lib/cpp";
	}
	else
	{
		return "cpp";
	}
}

/*
 * Spawn a child process and redirect its stdout to a readable pipe.
 */
static FILE *SpawnOnPipe(char **argv, int *pid)
{
#if defined(HAVE_FORK) && defined(HAVE_EXECV) && (defined(HAVE_WAITPID) || defined(HAVE_WAIT))
	int pipefds[2];
	FILE *file;

	/* Report the command line if verbose mode is enabled */
	if(verbose_mode == VERBOSE_CMDLINES)
	{
		int posn;
		for(posn = 0; argv[posn] != 0; ++posn)
		{
			if(posn != 0)
			{
				putc(' ', stderr);
			}
			fputs(argv[posn], stderr);
		}
		putc('\n', stderr);
	}

	/* Create a pipe to use to communicate with the child */
	if(pipe(pipefds) < 0)
	{
		perror("pipe");
		return 0;
	}

	/* Fork and create the child process */
	*pid = fork();
	if(*pid < 0)
	{
		/* We weren't able to fork the process */
		perror("fork");
		return 0;
	}
	else if(*pid == 0)
	{
		/* We are in the child */
		dup2(pipefds[1], 1);
		close(pipefds[0]);
		close(pipefds[1]);
		execvp(argv[0], argv);
		perror("execvp");
		exit(1);
	}
	else
	{
		/* We are in the parent */
		close(pipefds[1]);
		file = fdopen(pipefds[0], "r");
		if(!file)
		{
			perror("fdopen");
		}
		return file;
	}

#elif defined _WIN32
	int pipefds[2];
	FILE *file;
	int orig_stdout;
	
	/* Report the command line if verbose mode is enabled */
	if(verbose_mode == VERBOSE_CMDLINES)
	{
		int posn;
		for(posn = 0; argv[posn] != 0; ++posn)
		{
			if(posn != 0)
			{
				putc(' ', stderr);
			}
			fputs(argv[posn], stderr);
		}
		putc('\n', stderr);
	}
	
	/* Create a pipe to use to communicate with the child */
	if(_pipe(pipefds, 512, O_NOINHERIT) < 0)
	{
		perror("_pipe");
		return 0;
	}

	/* Duplicate the originale stdout descriptor */
	orig_stdout = _dup(_fileno(stdout));

	/* Duplicate write end of pipe to to stdout */
	if (_dup2(pipefds[1], _fileno(stdout)))
	{
		perror("_dup2");
		return 0;
	}

	/* Close original write end of pipe */
	_close(pipefds[1]);

	
	*pid = _spawnvp(P_NOWAIT, argv[0],
			(const char* const*)argv);

	/* Duplicate original stdout back */
	if (_dup2(orig_stdout, _fileno(stdout)))
	{
		perror("_dup2");
		return 0;
	}

	/* Close the duplicate stdout */
	_close(orig_stdout);

	file = _fdopen(pipefds[0], "r");
	if (!file)
	{
		perror("_fdopen");
		return 0;
	}

	return file;
#else
	
	fputs("Don't know how to spawn child processes on this system\n", stderr);
	return 0;
#endif
}

/*
 * Close the pipe that was created by "SpawnOnPipe".
 */
static int ClosePipe(FILE *file, int pid)
{
#if defined(HAVE_FORK) && defined(HAVE_EXECV) && (defined(HAVE_WAITPID) || defined(HAVE_WAIT))
	int status = 1;
	fclose(file);
#ifdef HAVE_WAITPID
	waitpid(pid, &status, 0);
#else
	{
		int result;
		while((result = wait(&status)) != pid && result != -1)
		{
			/* Some other child fell: not the one we are interested in */
		}
	}
#endif
	if(WIFEXITED(status))
	{
		return (WEXITSTATUS(status) == 0);
	}
	else
	{
		return 0;
	}
#elif defined _WIN32
	int status = 1;
	fclose(file);
	if (_cwait(&status, pid, _WAIT_CHILD) == -1)
	{
		perror("_cwait");
		return 0;
	}
	
	return (status == 0);
#else
	return 0;
#endif
}

/*
 * Import the list of macros to undefine from the system "cpp"
 * to return it to a pristine state ready to add our defines.
 */
extern char *ILCppUndefines[];
extern int ILCppNumUndefines;

/*
 * Parse a single input file.
 */
static void ParseFile(const char *filename, int is_stdin)
{
	char **argv = 0;
	int argc = 0;
	FILE *file;
	int posn;
	int fileIsPipe = 0;
	int pid = 0;
	int ourCpp;

	/* Open the input */
	if(CCPluginUsesPreproc == CC_PREPROC_C)
	{
		/* Try opening the file first, just to make sure that it is there */
		if(!is_stdin)
		{
			if((file = fopen(filename, "r")) == NULL)
			{
				perror(filename);
				CCHaveErrors |= 1;
				return;
			}
			fclose(file);
		}

		/* Build the command-line for the C pre-processor.  We assume that
		   it is the cscc-cpp pre-processor.  If it isn't, then tough luck */
		CCStringListAdd(&argv, &argc, FindCpp(&ourCpp));
		if(preprocess_flag)
		{
			if(no_preproc_lines_flag)
			{
				CCStringListAdd(&argv, &argc, "-P");
			}
			if(preproc_comments_flag)
			{
				CCStringListAdd(&argv, &argc, "-C");
			}
		}
		/* Note: we have to inhibit all cpp warnings or GNU cpp gets
		   touchy when we redefine some of its builtin symbols.  This
		   wouldn't be necessary if "-undef" actually did what it is
		   supposed to do, but it only undefines some symbols, not all */
#ifdef __APPLE__ 
#ifdef __MACH__
		/* Apple shipped a special "pre-compiled header" CPP with the
		   Darwin OS.  That obviously won't do us much good here, so
		   we need to set a flag to fall back on standard headers */
		if(!ourCpp)
		{
			CCStringListAdd(&argv, &argc, "-no-cpp-precomp");
		}
#endif
#endif
		CCStringListAdd(&argv, &argc, "-w");
		CCStringListAdd(&argv, &argc, "-nostdinc");
		CCStringListAdd(&argv, &argc, "-nostdinc++");
		if(!ourCpp)
		{
			CCStringListAdd(&argv, &argc, "-undef");
		}
		if(preproc_show_headers)
		{
			CCStringListAdd(&argv, &argc, "-H");
		}
		if(dependency_level != 0)
		{
			/* Generate dependencies while pre-processing */
			static char * const depLevels[] = {0, "-M", "-MD", "-MM", "-MMD"};
			CCStringListAdd(&argv, &argc, depLevels[dependency_level]);
			if(dependency_level == DEP_LEVEL_MD ||
		       dependency_level == DEP_LEVEL_MMD)
			{
				if(dependency_file != 0)
				{
					CCStringListAdd(&argv, &argc, dependency_file);
				}
			}
			if(dependency_gen_flag)
			{
				CCStringListAdd(&argv, &argc, "-MG");
			}
		}
		if(dump_output_format == DUMP_MACROS_ONLY)
		{
			CCStringListAdd(&argv, &argc, "-dM");
		}
		else if(dump_output_format == DUMP_MACROS_AND_OUTPUT)
		{
			CCStringListAdd(&argv, &argc, "-dN");
		}
		for(posn = 0; posn < num_include_dirs; ++posn)
		{
			CCStringListAddOption(&argv, &argc, "-I", include_dirs[posn]);
		}
		for(posn = 0; posn < num_sys_include_dirs; ++posn)
		{
			CCStringListAddOption(&argv, &argc, "-I", sys_include_dirs[posn]);
		}
		for(posn = 0; posn < num_imacros_files; ++posn)
		{
			CCStringListAdd(&argv, &argc, "-imacros");
			CCStringListAdd(&argv, &argc, imacros_files[posn]);
		}
		for(posn = 0; !ourCpp && posn < ILCppNumUndefines; ++posn)
		{
			/* Turn off a system cpp macro if we don't have it ourselves */
			if(!CCStringListContains(pre_defined_symbols,
									 num_pre_defined_symbols,
									 ILCppUndefines[posn]) &&
			   !CCStringListGetValue(pre_defined_symbols,
									 num_pre_defined_symbols,
									 ILCppUndefines[posn]) &&
			   !CCStringListContains(user_defined_symbols,
									 num_user_defined_symbols,
									 ILCppUndefines[posn]) &&
			   !CCStringListGetValue(user_defined_symbols,
									 num_user_defined_symbols,
									 ILCppUndefines[posn]))
			{
				CCStringListAddOption(&argv, &argc, "-U", ILCppUndefines[posn]);
			}
		}
		for(posn = 0; posn < num_pre_defined_symbols; ++posn)
		{
			CCStringListAddOption(&argv, &argc, "-D", pre_defined_symbols[posn]);
		}
		for(posn = 0; posn < num_user_defined_symbols; ++posn)
		{
			CCStringListAddOption(&argv, &argc, "-D", user_defined_symbols[posn]);
		}
		for(posn = 0; posn < num_undefined_symbols; ++posn)
		{
			CCStringListAddOption(&argv, &argc, "-U", undefined_symbols[posn]);
		}
		if(is_stdin)
		{
			CCStringListAdd(&argv, &argc, "-");
		}
		else
		{
			CCStringListAdd(&argv, &argc, (char *)filename);
		}
		CCStringListAdd(&argv, &argc, 0);

		/* Open a pipe to the command and run it */
		file = SpawnOnPipe(argv, &pid);
		if(!file)
		{
			CCHaveErrors |= 1;
			return;
		}
		fileIsPipe = 1;
	}
	else if(is_stdin)
	{
		file = stdin;
	}
	else if((file = fopen(filename, "r")) == NULL)
	{
		perror(filename);
		CCHaveErrors |= 1;
		return;
	}

	/* Set up the pre-processor or the simple file reader */
	if(CCPluginUsesPreproc == CC_PREPROC_CSHARP)
	{
		/* Use the C# pre-processor */
		CCPreProcInit(&CCPreProcessorStream, file, filename, !is_stdin);
		if(CCStringListContains(extension_flags, num_extension_flags,
								"latin1-charset"))
		{
			/* Disable UTF-8 pre-processing if the input charset is Latin-1 */
			CCPreProcessorStream.utf8 = 0;
		}
		for(posn = 0; posn < num_pre_defined_symbols; ++posn)
		{
			CCPreProcDefine(&CCPreProcessorStream, pre_defined_symbols[posn]);
		}
		for(posn = 0; posn < num_user_defined_symbols; ++posn)
		{
			CCPreProcDefine(&CCPreProcessorStream, user_defined_symbols[posn]);
		}

		/* If we are only pre-processing, then do that now */
		if(preprocess_flag)
		{
			CCHaveErrors |= Preprocess();
			CCPreProcDestroy(&CCPreProcessorStream);
			return;
		}
	}
	else if(CCPluginUsesPreproc == CC_PREPROC_C)
	{
		/* Use the C pre-processor */
		CCPreProcessorStream.stream = file;
		CCPreProcessorStream.lineNumber = 1;
		CCPreProcessorStream.filename =
			ILInternString((char *)filename, -1).string;
		CCPreProcessorStream.lexerLineNumber = 1;
		CCPreProcessorStream.lexerFileName = CCPreProcessorStream.filename;

		/* If we are only pre-processing, then do that now */
		if(preprocess_flag)
		{
			CCHaveErrors |= Preprocess();
			if(!ClosePipe(file, pid))
			{
				CCHaveErrors |= 1;
			}
			return;
		}
	}
	else
	{
		/* Don't perform any pre-processing */
		CCPreProcessorStream.stream = file;
		CCPreProcessorStream.lineNumber = 1;
		CCPreProcessorStream.filename =
			ILInternString((char *)filename, -1).string;
		CCPreProcessorStream.lexerLineNumber = 1;
		CCPreProcessorStream.lexerFileName = CCPreProcessorStream.filename;
	}

	/* Print the name of the file if verbose mode is enabled */
	if(verbose_mode == VERBOSE_FILENAMES)
	{
		fprintf(stderr, _("Parsing %s\n"), filename);
	}

	/* Initialize the lexical analyser */
	CCPluginRestart(NULL);

	/* Parse the input file */
	CCHaveErrors |= CCPluginParse();

	/* Destroy the pre-processor stream */
	if(CCPluginUsesPreproc == CC_PREPROC_CSHARP)
	{
		CCHaveErrors |= CCPreProcessorStream.error;
		CCPreProcDestroy(&CCPreProcessorStream);
	}
	else if(fileIsPipe)
	{
		if(!ClosePipe(file, pid))
		{
			CCHaveErrors |= 1;
		}
	}
	else if(!is_stdin)
	{
		fclose(file);
	}
}

int CCPluginInput(char *buf, int maxSize)
{
	if(CCPluginUsesPreproc == CC_PREPROC_CSHARP)
	{
		return CCPreProcGetBuffer(&CCPreProcessorStream, buf, maxSize);
	}
	else
	{
		FILE *file = CCPreProcessorStream.stream;
		int ch, len;
		len = 0;
		CCPreProcessorStream.lexerLineNumber = CCPreProcessorStream.lineNumber;
		CCPreProcessorStream.lexerFileName = CCPreProcessorStream.filename;
		while(maxSize > 0)
		{
			if((ch = getc(file)) == EOF)
			{
				break;
			}
			else if(ch == '\n')
			{
				/* Unix end of line sequence */
				*buf = '\n';
				++len;
				++(CCPreProcessorStream.lineNumber);
				break;
			}
			else if(ch == '\r')
			{
				/* MS-DOS or Mac end of line sequence */
				ch = getc(file);
				if(ch != '\n' && ch != EOF)
				{
					ungetc(ch, file);
				}
				*buf = '\n';
				++len;
				++(CCPreProcessorStream.lineNumber);
				break;
			}
			else
			{
				*buf++ = (char)ch;
				++len;
				--maxSize;
			}
		}
		return len;
	}
}

void CCPluginParseError(char *msg, char *text)
{
	char *newmsg;
	int posn, outposn;

	if(!strcmp(msg, "parse error") || !strcmp(msg, "syntax error"))
	{
		/* This error is pretty much useless at telling the user
		   what is happening.  Try to print a better message
		   based on the current input token */
	simpleError:
		if(text && *text != '\0')
		{
			CCError(_("parse error at or near `%s'"), text);
		}
		else
		{
			CCError(_("parse error"));
		}
	}
	else if(!strncmp(msg, "parse error, expecting `", 24))
	{
		/* We have to quote the token names in the "%token" declarations
		   within yacc grammars so that byacc doesn't mess up the output.
		   But the quoting causes Bison to output quote characters in
		   error messages which look awful.  This code attempts to fix
		   things up */
		newmsg = ILDupString(msg);
	expectingError:
		if(newmsg)
		{
			posn = 0;
			outposn = 0;
			while(newmsg[posn] != '\0')
			{
				if(newmsg[posn] == '`')
				{
					if(newmsg[posn + 1] == '"' && newmsg[posn + 2] == '`')
					{
						/* Convert <`"`> into <`> */
						posn += 2;
						newmsg[outposn++] = '`';
					}
					else if(newmsg[posn + 1] == '"')
					{
						/* Convert <`"> into <> */
						++posn;
					}
					else if(newmsg[posn + 1] == '`' ||
					        newmsg[posn + 1] == '\'')
					{
						/* Convert <``> or <`'> into <`> */
						++posn;
						newmsg[outposn++] = '`';
					}
					else
					{
						/* Ordinary <`> on its own */
						newmsg[outposn++] = '`';
					}
				}
				else if(newmsg[posn] == '\\')
				{
					/* Ignore backslashes in the input */
				}
				else if(newmsg[posn] == '"' && newmsg[posn + 1] == '\'')
				{
					/* Convert <"'> into <> */
					++posn;
				}
				else if(newmsg[posn] == '\'' && newmsg[posn + 1] == '"' &&
				        newmsg[posn + 2] == '\'')
				{
					/* Convert <'"'> into <'> */
					posn += 2;
					newmsg[outposn++] = '\'';
				}
				else if(newmsg[posn] == '\'' && newmsg[posn + 1] == '\'')
				{
					/* Convert <''> into <'> */
					++posn;
					newmsg[outposn++] = '\'';
				}
				else if(newmsg[posn] == ' ' && newmsg[posn + 1] == '\'')
				{
					/*  bison 1.75 - <'> following a space becomes <`> */
					++posn;
					newmsg[outposn++] = ' ';
					newmsg[outposn++] = '`';
				}
				else if(newmsg[posn] == '"')
				{
					/* Ignore quotes - bison 1.75 */
				}
				else
				{
					/* Ordinary character */
					newmsg[outposn++] = newmsg[posn];
				}
				++posn;
			}
			newmsg[outposn] = '\0';
			if(text && *text != '\0')
			{
				CCError(_("%s, at or near `%s'"), newmsg, text);
			}
			else
			{
				CCError("%s", newmsg);
			}
			ILFree(newmsg);
		}
		else
		{
			if(text && *text != '\0')
			{
				CCError(_("%s at or near `%s'"), msg, text);
			}
			else
			{
				CCError("%s", msg);
			}
		}
	}
	else if(!strncmp(msg, "parse error, unexpected ", 24))
	{
		/* The error probably has the form "parse error, unexpected ...,
		   expecting ..." - strip out the "unexpected" part */
		posn = 24;
		while(msg[posn] != '\0' &&
			  strncmp(msg + posn, ", expecting ", 12) != 0)
		{
			++posn;
		}
		if(msg[posn] == '\0')
		{
			goto simpleError;
		}
		newmsg = (char *)ILMalloc(strlen(msg) + 1);
		if(!newmsg)
		{
			goto defaultError;
		}
		strcpy(newmsg, "parse error, expecting ");
		strcat(newmsg, msg + posn + 12);
		goto expectingError;
	}
	else
	{
		/* The parser has probably included information as to what
		   is expected in this context, so report that */
	defaultError:
		if(text && *text != '\0')
		{
			CCError(_("%s at or near `%s'"), msg, text);
		}
		else
		{
			CCError("%s", msg);
		}
	}
}

void CCPluginAddTopLevel(ILNode *node)
{
	if(!node)
	{
		return;
	}
	if(!CCParseTree)
	{
		CCParseTree = ILNode_List_create();
		CCParseTreeEnd = 0;
	}
	if(!CCParseTreeEnd)
	{
		CCParseTreeEnd = CCParseTree;
	}
	ILNode_List_Add(CCParseTreeEnd, node);
	if(((ILNode_List *)CCParseTreeEnd)->rest)
	{
		CCParseTreeEnd = (ILNode *)(((ILNode_List *)CCParseTreeEnd)->rest);
	}
}

void CCPluginAddStandaloneAttrs(ILNode *node)
{
	if(!node)
	{
		return;
	}
	if(!CCStandaloneAttrs)
	{
		CCStandaloneAttrs = ILNode_List_create();
		CCStandaloneAttrsEnd = 0;
	}
	if(!CCStandaloneAttrsEnd)
	{
		CCStandaloneAttrsEnd = CCStandaloneAttrs;
	}
	ILNode_List_Add(CCStandaloneAttrsEnd, node);
	if(((ILNode_List *)CCStandaloneAttrsEnd)->rest)
	{
		CCStandaloneAttrsEnd =
			(ILNode *)(((ILNode_List *)CCStandaloneAttrsEnd)->rest);
	}
}

int CCLoadLibrary(const char *name)
{
	return LoadLib(name, nostdlib_flag, 1);
}

/*
 * The following functions are required by the treecc node routines.
 */

char *yycurrfilename(void)
{
	/* should be const char * but this has to be fixed in treecc. */
	return (char *)CCPreProcessorStream.lexerFileName;
}

long yycurrlinenum(void)
{
	return CCPreProcessorStream.lexerLineNumber;
}

void yynodefailed(void)
{
	CCOutOfMemory();
}

#ifdef	__cplusplus
};
#endif
