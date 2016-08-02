#ifdef	__cplusplus
extern	"C" {
#endif
#include "bf/bf_internal.h"

/*
 * Entry points for the parser and lexer.
 */
extern int bf_debug;
extern int bf_parse(void);
extern void bf_restart(FILE *infile);

/*
 * Configuration variables that are used by "cc_main.c".
 */
char const CCPluginName[] = "cscc-bf";
int const CCPluginOptionParseMode = CMDLINE_PARSE_PLUGIN;
int const CCPluginUsesPreproc = CC_PREPROC_NONE;
int const CCPluginJVMSupported = 1;
int const CCPluginSkipCodeGen = 0;
int const CCPluginGenModulesEarly = 0;
int const CCPluginForceStdlib = 1;

int CCPluginInit(void)
{
	/* Nothing to do here */
	return 1;
}

void CCPluginShutdown(int status)
{
	/* Nothing to do here */
}

int CCPluginParse(void)
{
	/* bf_debug = 1; */
	return bf_parse();
}

void CCPluginRestart(FILE *infile)
{
	bf_restart(infile);
}

void CCPluginSemAnalysis(void)
{
	/* Nothing to do here */
	if(optimize_flag)
	{
		CCParseTree = BFOptimize(&CCCodeGen, CCParseTree);
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


