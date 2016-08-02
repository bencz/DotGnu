/*
 * c_main.c - Main entry point for the C compiler plug-in.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

#include <cscc/c/c_internal.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Entry points for the parser and lexer.
 */
extern int c_debug;
extern int c_parse(void);
extern void c_restart(FILE *infile);

/*
 * Configuration variables that are used by "cc_main.c".
 */
char const CCPluginName[] = "cscc-c-s";
int const CCPluginOptionParseMode = CMDLINE_PARSE_PLUGIN;
int const CCPluginUsesPreproc = CC_PREPROC_C;
int const CCPluginJVMSupported = 0;
int const CCPluginSkipCodeGen = 0;
int const CCPluginGenModulesEarly = 1;
int const CCPluginForceStdlib = 1;

int CCPluginInit(void)
{
	/* Initialize the C pre-processor with the standard macro definitions */
	CCStringListAdd(&pre_defined_symbols, &num_pre_defined_symbols,
					"__PTRDIFF_TYPE__=long");
	CCStringListAdd(&pre_defined_symbols, &num_pre_defined_symbols,
					"__SIZE_TYPE__=__unsigned_int__");
	CCStringListAdd(&pre_defined_symbols, &num_pre_defined_symbols,
					"__WINT_TYPE__=__unsigned_int__");
#if defined(__APPLE__) && defined(__MACH__)
	CCStringListAdd(&pre_defined_symbols, &num_pre_defined_symbols,
					"__VERSION__=\"" VERSION "-cscc\"");
#else
	CCStringListAdd(&pre_defined_symbols, &num_pre_defined_symbols,
					"__VERSION__=\"" VERSION " (cscc)\"");
#endif
	CCStringListAdd(&pre_defined_symbols, &num_pre_defined_symbols,
					"__WCHAR_TYPE__=__wchar__");
	CCStringListAdd(&pre_defined_symbols, &num_pre_defined_symbols,
					"__STDC__=1");
	CCStringListAdd(&pre_defined_symbols, &num_pre_defined_symbols,
					"__cli");
	CCStringListAdd(&pre_defined_symbols, &num_pre_defined_symbols,
					"__cli__");

	/* The plugin has been initialized */
	return 1;
}

void CCPluginShutdown(int status)
{
	/* Nothing to do here */
}

int CCPluginParse(void)
{
	/*c_debug = 1;*/
	return c_parse();
}

void CCPluginRestart(FILE *infile)
{
	/* Setup up the lexer with the specified input file */
	c_restart(infile);

	/* Begin code generation */
	CGenBeginCode(&CCCodeGen);
}

void CCPluginSemAnalysis(void)
{
	/* Nothing to do here: we do analysis on the fly during parsing */
}

void CCPluginPostCodeGen(void)
{
	CGenEndCode(&CCCodeGen);
}

int main(int argc, char *argv[])
{
	return CCMain(argc, argv);
}

#ifdef	__cplusplus
};
#endif
