/*
 * cc_main.h - Interface to the common "main" routine for "cscc" plugins.
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

#ifndef	_CSCC_CC_MAIN_H
#define	_CSCC_CC_MAIN_H

#include <cscc/common/cc_errors.h>
#include <cscc/common/cc_options.h>
#include <cscc/common/cc_preproc.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Definitions that are provided by the plugin.
 */

/*
 * Canonical name for the plugin.  e.g. "cscc-cs".
 */
extern char const CCPluginName[];

/*
 * Command-line option parse mode.
 */
extern int const CCPluginOptionParseMode;

/*
 * Values for "CCPluginUsesPreproc".
 */
#define	CC_PREPROC_NONE		0
#define	CC_PREPROC_CSHARP	1
#define	CC_PREPROC_C		2

/*
 * The pre-processor to use.  One of the values above.
 */
extern int const CCPluginUsesPreproc;

/*
 * Set to zero if compiling to the JVM is not supported.
 */
extern int const CCPluginJVMSupported;

/*
 * Set to non-zero to skip code generation (used by "csdoc").
 */
extern int const CCPluginSkipCodeGen;

/*
 * Set to non-zero to generate modules and assemblies earlier.
 */
extern int const CCPluginGenModulesEarly;

/*
 * Set to non-zero if "mscorlib" should be loaded even if "-nostdlib".
 */
extern int const CCPluginForceStdlib;

/*
 * Initialize the plugin after the command-line options have
 * been processed, but before the input files are parsed.
 * Returns zero if the program should abort.
 */
int CCPluginInit(void);

/*
 * Terminate the plugin just before shutdown.  If "status"
 * is non-zero, then compilation failed for some reason.
 */
void CCPluginShutdown(int status);

/*
 * Call the parser.  Returns non-zero on error, like "yyparse".
 */
int CCPluginParse(void);

/*
 * Restart the parser on a new file.
 */
void CCPluginRestart(FILE *infile);

/*
 * Perform semantic analysis.
 */
void CCPluginSemAnalysis(void);

/*
 * Perform post code generation tasks.
 */
void CCPluginPostCodeGen(void);

/*
 * Definitions that are provided to the plugin.
 */

/*
 * Code generator control object.
 */
extern ILGenInfo CCCodeGen;

/*
 * Top-level parse tree.
 */
extern ILNode *CCParseTree;
extern ILNode *CCParseTreeEnd;

/*
 * Standalone attribute blocks that aren't attached to a class.
 */
extern ILNode *CCStandaloneAttrs;
extern ILNode *CCStandaloneAttrsEnd;

/*
 * Pre-processor stream.
 */
CCPreProc CCPreProcessorStream;

/*
 * Main entry point.  Call this from the plugin's "main".
 */
int CCMain(int argc, char *argv[]);

/*
 * Input routine to be called from the lexer's "YY_INPUT" macro
 * to retrieve the next line of input.
 */
int CCPluginInput(char *buf, int maxSize);

/*
 * Report a parse error that came from "yyerror".
 * "text" is usually "yytext", but can be NULL or
 * the empty string if the caller does not know
 * which token is near the site of the error.
 */
void CCPluginParseError(char *msg, char *text);

/*
 * Add a node to the top-level parse tree.
 */
void CCPluginAddTopLevel(ILNode *node);

/*
 * Add a node to the standalone attribute list.
 */
void CCPluginAddStandaloneAttrs(ILNode *node);

/*
 * Load a library into the compiler.
 */
int CCLoadLibrary(const char *name);

#ifdef	__cplusplus
};
#endif

#endif	/* _CSCC_CC_MAIN_H */
