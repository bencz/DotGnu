/*
 * cs_main.c - Main entry point for the C# compiler plug-in.
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

#include "csharp/cs_internal.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Entry points for the parser and lexer.
 */
extern int cs_debug;
extern int cs_parse(void);
extern void cs_restart(FILE *infile);

/*
 * Configuration variables that are used by "cc_main.c".
 */
char const CCPluginName[] = "cscc-cs";
int const CCPluginOptionParseMode = CMDLINE_PARSE_PLUGIN;
int const CCPluginUsesPreproc = CC_PREPROC_CSHARP;
int const CCPluginJVMSupported = 1;
int const CCPluginSkipCodeGen = 0;
int const CCPluginGenModulesEarly = 0;
int const CCPluginForceStdlib = 0;

int CCPluginInit(void)
{
	if(CCStringListContains(extension_flags, num_extension_flags,
							"metadata-only"))
	{
		CSMetadataOnly = 1;
	}
	if(CCStringListContains(extension_flags, num_extension_flags,
							"no-generics"))
	{
		CSNoGenerics = 1;
	}
	else if(CCStringListContains(extension_flags, num_extension_flags,
							     "generics"))
	{
		CSNoGenerics = 0;
	}
	else
	{
		CSNoGenerics = 0;
	}
	if(CCStringListContains(extension_flags, num_extension_flags,
							"latin1-charset"))
	{
		CSLatin1Charset = 1;
	}
	if(CCStringListContains(extension_flags, num_extension_flags,
							"no-hidebysig") ||
	   CCStringListContains(extension_flags, num_extension_flags,
							"hidebyname"))
	{
		CSNoHideBySig = 1;
	}
	else if(CCStringListContains(extension_flags, num_extension_flags,
							     "hidebysig") ||
	        CCStringListContains(extension_flags, num_extension_flags,
							     "no-hidebyname"))
	{
		CSNoHideBySig = 0;
	}
	else
	{
		CSNoHideBySig = 0;
	}
	CSMemberInfoInit();
	return 1;
}

void CCPluginShutdown(int status)
{
	CSMemberInfoDestroy();
}

int CCPluginParse(void)
{
	/*cs_debug = 1;*/
	return cs_parse();
}

void CCPluginRestart(FILE *infile)
{
	cs_restart(infile);
}

void CCPluginSemAnalysis(void)
{
	/* Perform type gathering */
	CCCodeGen.typeGather = -1;
	CCParseTree = CSTypeGather(&CCCodeGen, CCCodeGen.globalScope, CCParseTree);
	CCCodeGen.typeGather = 0;

	/* Perform semantic analysis */
	ILNode_SemAnalysis(CCParseTree, &CCCodeGen, &CCParseTree);
	if(CCStandaloneAttrs)
	{
		ILNode_SemAnalysis(CCStandaloneAttrs, &CCCodeGen, &CCStandaloneAttrs);
	}
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
