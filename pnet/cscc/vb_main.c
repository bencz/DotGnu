/*
 * vb_main.c - Main entry point for the VB compiler plug-in.
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

#include "vb/vb_internal.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Entry points for the parser and lexer.
 */
extern int vb_debug;
extern int vb_parse(void);
extern void vb_restart(FILE *infile);

/*
 * Configuration variables that are used by "cc_main.c".
 */
char const CCPluginName[] = "cscc-vb";
int const CCPluginOptionParseMode = CMDLINE_PARSE_PLUGIN;
int const CCPluginUsesPreproc = CC_PREPROC_CSHARP;	/* TODO: VB preprocessor */
int const CCPluginJVMSupported = 1;
int const CCPluginSkipCodeGen = 0;
int const CCPluginGenModulesEarly = 0;
int const CCPluginForceStdlib = 0;

int CCPluginInit(void)
{
	/* Turn on VB-specific syntax and rules in "cscc/csharp" */
	prog_language = PROG_LANG_VB;
	return 1;
}

void CCPluginShutdown(int status)
{
	/* Nothing to do here */
}

int CCPluginParse(void)
{
	/*vb_debug = 1;*/
	return vb_parse();
}

void CCPluginRestart(FILE *infile)
{
	vb_restart(infile);
}

void CCPluginSemAnalysis(void)
{
	/* Perform type gathering */
	CCCodeGen.typeGather = -1;
	CCParseTree = CSTypeGather(&CCCodeGen, CCCodeGen.globalScope, CCParseTree);
	CCCodeGen.typeGather = 0;

	/* Perform semantic analysis */
	ILNode_SemAnalysis(CCParseTree, &CCCodeGen, &CCParseTree);
}

void CCPluginPostCodeGen(void)
{
	/* Nothing to do here */
}

int main(int argc, char *argv[])
{
	return CCMain(argc, argv);
}

#ifdef	__cplusplus
};
#endif
