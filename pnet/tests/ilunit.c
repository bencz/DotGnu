/*
 * ilunit.c - XUnit-style testing framework for C-based API's.
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

#include "ilunit.h"
#include <setjmp.h>
#ifdef HAVE_STDARG_H
#include <stdarg.h>
#define	VA_LIST				va_list
#define	VA_START(arg)		va_list va; va_start(va, arg)
#define	VA_END				va_end(va)
#define	VA_ARG(va,type)		va_arg(va, type)
#define	VA_GET_LIST			va
#else
#ifdef HAVE_VARARGS_H
#include <varargs.h>
#define	VA_LIST				va_list
#define	VA_START(arg)		va_list va; va_start(va)
#define	VA_END				va_end(va)
#define	VA_ARG(va,type)		va_arg(va, type)
#define	VA_GET_LIST			va
#else
#define	VA_LIST				int
#define	VA_START
#define	VA_END
#define	VA_ARG(va,type)		((type)0)
#define	VA_GET_LIST			0
#endif
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Table of command-line options.
 */
static ILCmdLineOption const options[] = {
	{"-s", 's', 0, 0, 0},
	{"--stop-at-fail", 's', 0,
		"--stop-at-fail  or -s",
		"Stop at the first failed test."},
	{"-f", 'f', 0, 0, 0},
	{"--show-failed", 'f', 0,
		"--show-failed   or -f",
		"Only show tests that have failed."},
	{"-l", 'l', 0, 0 ,0},
	{"--list", 'l', 0,
		"--list          or -l",
		"List all test suites and tests that are registered."},
	{"--version", 'v', 0,
		"--version       or -v",
		"Print the version of the program."},
	{"--help", 'h', 0,
		"--help",
		"Print this help message."},
	{0, 0, 0, 0, 0}
};

static void usage(const char *progname);
static void version(void);
static void listAllTests(void);
static int runNamedTest(const char *name);
static void runAllTests(void);

/*
 * List of unit tests that have been registered.
 */
typedef struct _tagILUnitTest ILUnitTest;
struct _tagILUnitTest
{
	const char     *name;
	ILUnitTestFunc	func;
	void		   *arg;
	ILUnitTest	   *next;

};
static ILUnitTest *testList = 0;
static ILUnitTest *lastTest = 0;

/*
 * Control variables for running tests.
 */
static int stopAtFailed = 0;
static int showOnlyFailed = 0;
static int numFailed = 0;
static int numRun = 0;
static volatile ILUnitTest *testBeingRun;
static volatile int testFailed;
static jmp_buf jumpBack;

/*
 * Control variables for the assembly stream and image.
 */
static FILE *asmStream = NULL;
static ILContext *asmContext = 0;
static ILImage *asmImage = 0;

int main(int argc, char *argv[])
{
	char *progname = argv[0];
	int listTests = 0;
	int state, opt;
	char *param;

	/* Parse the command-line arguments */
	state = 0;
	while((opt = ILCmdLineNextOption(&argc, &argv, &state,
									 options, &param)) != 0)
	{
		switch(opt)
		{
			case 's':
			{
				stopAtFailed = 1;
			}
			break;

			case 'f':
			{
				showOnlyFailed = 1;
			}
			break;

			case 'l':
			{
				listTests = 1;
			}
			break;

			case 'v':
			{
				version();
				return 0;
			}
			/* Not reached */

			default:
			{
				usage(progname);
				return 1;
			}
			/* Not reached */
		}
	}

	/* Register the tests */
	ILUnitRegisterTests();

	/* If "-l" was supplied, then list the tests and bail out */
	if(listTests)
	{
		listAllTests();
		return 0;
	}

	/* Run the tests */
	if(argc > 1)
	{
		/* Run only the specified tests or suites */
		while(argc > 1)
		{
			if(!runNamedTest(argv[1]) && stopAtFailed)
			{
				break;
			}
			++argv;
			--argc;
		}
	}
	else
	{
		/* Run all tests in all suites */
		runAllTests();
	}

	/* Cleanup the tests */
	ILUnitCleanupTests();

	/* Print a summary of how many tests succeeded or failed */
	if(!showOnlyFailed || numFailed != 0)
	{
		printf("\n%d tests run, %d tests failed\n", numRun, numFailed);
	}

	/* Done */
	return (numFailed != 0);
}

static void usage(const char *progname)
{
	fprintf(stdout, "ILUNIT " VERSION " - IL Unit Tester\n");
	fprintf(stdout, "Copyright (c) 2001 Southern Storm Software, Pty Ltd.\n");
	fprintf(stdout, "\n");
	fprintf(stdout, "Usage: %s [options] [testname ...] \n", progname);
	fprintf(stdout, "\n");
	ILCmdLineHelp(options);
}

static void version(void)
{

	printf("ILUNIT " VERSION " - IL Unit Tester\n");
	printf("Copyright (c) 2001 Southern Storm Software, Pty Ltd.\n");
	printf("\n");
	printf("ILSIZE comes with ABSOLUTELY NO WARRANTY.  This is free software,\n");
	printf("and you are welcome to redistribute it under the terms of the\n");
	printf("GNU General Public License.  See the file COPYING for further details.\n");
	printf("\n");
	printf("Use the `--help' option to get help on the command-line options.\n");
}

/*
 * Print the name of a test suite with a line underneath it.
 */
static void printSuiteName(const char *name)
{
	fputs("Suite: ", stdout);
	fputs(name, stdout);
	fputs("\n-------", stdout);
	while(*name != '\0')
	{
		putc('-', stdout);
		++name;
	}
	putc('\n', stdout);
	putc('\n', stdout);
}

/*
 * List all tests within the system.
 */
static void listAllTests(void)
{
	ILUnitTest *test = testList;
	ILUnitTest *prevTest = 0;
	int numTests = 0;
	while(test != 0)
	{
		if(!(test->func))
		{
			/* This is the start of a test suite */
			if(prevTest)
			{
				putc('\n', stdout);
			}
			printSuiteName(test->name);
		}
		else
		{
			/* This is a particular test within the current suite */
			fputs(test->name, stdout);
			putc('\n', stdout);
			++numTests;
		}
		prevTest = test;
		test = test->next;
	}
	printf("\n%d tests total\n", numTests);
}

void ILUnitRegisterSuite(const char *name)
{
	ILUnitRegister(name, 0, 0);
}

void ILUnitRegister(const char *name, ILUnitTestFunc func, void *arg)
{
	ILUnitTest *test = (ILUnitTest *)ILMalloc(sizeof(ILUnitTest));
	if(!test)
	{
		ILUnitOutOfMemory();
	}
	test->name = name;
	test->func = func;
	test->arg = arg;
	test->next = 0;
	if(lastTest)
	{
		lastTest->next = test;
	}
	else
	{
		testList = test;
	}
	lastTest = test;
}

void ILUnitFail(void)
{
	if(showOnlyFailed)
	{
		fputs(testBeingRun->name, stdout);
		fputs(" ... ", stdout);
	}
	fputs("failed\n", stdout);
	testFailed = 1;
	longjmp(jumpBack, 1);
}

void ILUnitFailed(const char *msg, ...)
{
	VA_START(msg);
	if(showOnlyFailed)
	{
		fputs(testBeingRun->name, stdout);
		fputs(" ... ", stdout);
	}
#ifdef HAVE_VFPRINTF
	vfprintf(stdout, msg, va);
#else
	fputs(msg, stdout);
#endif
	VA_END;
	putc('\n', stdout);
	testFailed = 1;
	longjmp(jumpBack, 1);
}

void ILUnitFailMessage(const char *msg, ...)
{
	VA_START(msg);
	if(showOnlyFailed && !testFailed)
	{
		fputs(testBeingRun->name, stdout);
		fputs(" ... ", stdout);
	}
#ifdef HAVE_VFPRINTF
	vfprintf(stdout, msg, va);
#else
	fputs(msg, stdout);
#endif
	VA_END;
	putc('\n', stdout);
	testFailed = 1;
}

void ILUnitFailEndMessages(void)
{
	longjmp(jumpBack, 1);
}

void _ILUnitAssert(const char *condition, const char *filename, int linenum)
{
	ILUnitFailed("%s:%d: assertion failed: `%s'",
				 filename, linenum, condition);
}

void ILUnitOutOfMemory(void)
{
	fputs("virtual memory exhausted\n", stderr);
	exit(1);
}

/*
 * Run a particular test.  Returns zero if it failed.
 */
static int runTest(ILUnitTest *test)
{
	/* Print the name of the test we will be running */
	if(!showOnlyFailed)
	{
		fputs(test->name, stdout);
		fputs(" ... ", stdout);
		fflush(stdout);
	}

	/* Run the test */
	testBeingRun = test;
	testFailed = 0;
	if(!setjmp(jumpBack))
	{
		(*(test->func))(test->arg);
	}

	/* Report success if necessary */
	if(!testFailed && !showOnlyFailed)
	{
		fputs("ok\n", stdout);
	}

	/* Clean up any assembly stream data used by the test */
	if(asmStream)
	{
		fclose(asmStream);
		asmStream = NULL;
	}
	if(asmContext)
	{
		ILContextDestroy(asmContext);
		asmContext = 0;
		asmImage = 0;
	}
	ILDeleteFile("iltest.il");
	ILDeleteFile("iltest.obj");

	/* Update the number of successes or failures */
	++numRun;
	if(testFailed)
	{
		++numFailed;
		return 0;
	}
	else
	{
		return 1;
	}
}

/*
 * Run a particular named test.  Returns zero if it failed.
 */
static int runNamedTest(const char *name)
{
	ILUnitTest *test;
	int failed;

	/* Search for the test name */
	test = testList;
	while(test != 0)
	{
		if(!strcmp(test->name, name))
		{
			break;
		}
		test = test->next;
	}
	if(!test)
	{
		fputs(name, stdout);
		fputs(" ... no such test\n", stdout);
		++numRun;
		++numFailed;
		return 0;
	}

	/* We need to handle things differently for individual tests and suites */
	if(test->func)
	{
		/* This is an individual test */
		return runTest(test);
	}
	else
	{
		/* This is a suite of tests */
		if(!showOnlyFailed)
		{
			printSuiteName(test->name);
		}
		test = test->next;
		failed = 0;
		while(test != 0 && test->func != 0)
		{
			if(!runTest(test))
			{
				if(stopAtFailed)
				{
					return 0;
				}
				else
				{
					failed = 1;
				}
			}
			test = test->next;
		}
		if(!showOnlyFailed)
		{
			putc('\n', stdout);
		}
		return !failed;
	}
}

/*
 * Run all tests in all test suites.
 */
static void runAllTests(void)
{
	ILUnitTest *test = testList;
	ILUnitTest *prevTest = 0;
	while(test != 0)
	{
		if(test->func)
		{
			/* This is an individual test */
			if(!runTest(test))
			{
				if(stopAtFailed)
				{
					return;
				}
			}
		}
		else
		{
			/* This is the start of a test suite */
			if(prevTest && !showOnlyFailed)
			{
				putc('\n', stdout);
			}
			if(!showOnlyFailed)
			{
				printSuiteName(test->name);
			}
		}
		prevTest = test;
		test = test->next;
	}
}

void ILUnitAsmCreate(void)
{
	asmStream = fopen("iltest.il", "w");
	if(!asmStream)
	{
		ILUnitFailed("could not open `iltest.il' for writing");
	}
	fputs(".assembly extern mscorlib {}\n", asmStream);
	fputs(".assembly iltest {}\n", asmStream);
	fputs(".class public ILTest extends [mscorlib]System.Object\n", asmStream);
	fputs("{\n", asmStream);
}

/*
 * Library directories to inspect to find "mscorlib.dll".
 */
static char *libraryDirs[] = {"../samples"};
#define	numLibraryDirs	1

void ILUnitAsmClose(void)
{
	char *args[16];
	FILE *file;
	int loadError;

	/* Close the stream */
	if(!asmStream)
	{
		ILUnitFailed("assembly stream is not in use");
	}
	fputs("}\n", asmStream);
	fclose(asmStream);
	asmStream = NULL;

	/* Run the assembler to generate "iltest.obj" */
	args[0] = "../ilasm/ilasm";
	args[1] = "--format";
	args[2] = "obj";
	args[3] = "-o";
	args[4] = "iltest.obj";
	args[5] = "iltest.il";
	args[6] = 0;
	if(ILSpawnProcess(args) != 0)
	{
		ILUnitFailed("ilasm failed on `iltest.il'");
	}

	/* Attempt to open "iltest.obj" */
	if((file = fopen("iltest.obj", "rb")) == NULL)
	{
		if((file = fopen("iltest.obj", "r")) == NULL)
		{
			ILUnitFailed("could not open `iltest.obj'");
		}
	}

	/* Create a context and load "iltest.obj" into it */
	asmContext = ILContextCreate();
	if(!asmContext)
	{
		ILUnitOutOfMemory();
	}
	ILContextSetLibraryDirs(asmContext, libraryDirs, numLibraryDirs);
	loadError = ILImageLoad(file, "iltest.obj", asmContext, &asmImage,
							IL_LOADFLAG_FORCE_32BIT);
	if(loadError == IL_LOADERR_MEMORY)
	{
		ILUnitOutOfMemory();
	}
	fclose(file);
	if(loadError != 0)
	{
		ILUnitFailed("error loading `iltest.obj': %s\n",
					 ILImageLoadError(loadError));
	}
}

void ILUnitAsmMethod(const char *signature, int maxStack)
{
	if(!asmStream)
	{
		ILUnitFailed("assembly stream is not in use");
	}
	fputs(".method ", asmStream);
	fputs(signature, asmStream);
	fputs(" cil managed\n", asmStream);
	fputs("{\n", asmStream);
	fprintf(asmStream, "\t.maxstack %d\n", maxStack);
}

void ILUnitAsmEndMethod(void)
{
	if(!asmStream)
	{
		ILUnitFailed("assembly stream is not in use");
	}
	fputs("}\n", asmStream);
}

void ILUnitAsmWrite(const char *str)
{
	if(!asmStream)
	{
		ILUnitFailed("assembly stream is not in use");
	}
	fputs(str, asmStream);
}

void ILUnitAsmPrintF(const char *format, ...)
{
	VA_START(format);
	if(!asmStream)
	{
		ILUnitFailed("assembly stream is not in use");
	}
#ifdef HAVE_VFPRINTF
	vfprintf(asmStream, format, va);
#else
	fputs(format, asmStream);
#endif
	VA_END;
}

FILE *ILUnitAsmStream(void)
{
	if(!asmStream)
	{
		ILUnitFailed("assembly stream is not in use");
	}
	return asmStream;
}

ILImage *ILUnitAsmImage(void)
{
	if(!asmImage)
	{
		ILUnitFailed("assembly image is not loaded");
	}
	return asmImage;
}

#ifdef	__cplusplus
};
#endif
