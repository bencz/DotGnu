/*
 * test_output.c - Test harness for the routines in "gen.h".
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

#include "parse.h"
#include "gen.h"

#ifdef	__cplusplus
extern	"C" {
#endif

extern FILE *TreeCCErrorFile;
extern int TreeCCErrorStripPath;

int main(int argc, char *argv[])
{
	FILE *infile;
	TreeCCInput input;
	TreeCCContext *context;
	TreeCCStream *stream;

	/* Validate the command-line arguments */
	if(argc != 2)
	{
		fprintf(stderr, "Usage: %s filename\n", argv[0]);
		return 1;
	}

	/* Attempt to open the input file */
	if((infile = fopen(argv[1], "r")) == NULL)
	{
		perror(argv[1]);
		return 1;
	}

	/* Make sure all error messages go to stdout, not stderr */
	TreeCCErrorFile = stdout;
	TreeCCErrorStripPath = 1;

	/* Open the token parser */
	TreeCCOpen(&input, argv[0], infile, argv[1]);

	/* Create the parsing context */
	context = TreeCCContextCreate(&input);
	context->debugMode = 1;
	context->strip_filenames = 1;

	/* Attempt to open the output streams */
	if((context->headerStream =
			TreeCCStreamCreate(context, "output.h", NULL, 1)) == 0 ||
	   (context->sourceStream =
	   		TreeCCStreamCreate(context, "output.c", NULL, 0)) == 0)
	{
		TreeCCContextDestroy(context);
		TreeCCClose(&input, 1);
		return 1;
	}
	context->headerStream->defaultFile = 1;
	context->sourceStream->defaultFile = 1;

	/* Parse the contents of the input stream */
	TreeCCParse(context);

	/* Validate the node hierarchy and the operations */
	TreeCCNodeValidate(context);
	TreeCCOperationValidate(context);

	/* If there were errors, then bail out now */
	if(input.errors)
	{
		TreeCCContextDestroy(context);
		TreeCCClose(&input, 1);
		return 1;
	}

	/* Generate the final source code */
	TreeCCGenerate(context);

	/* Flush the contents of the streams */
	stream = context->streamList;
	while(stream != 0)
	{
		if(stream->isHeader)
		{
			TreeCCStreamFlushStdio(stream, stdout);
		}
		stream = stream->nextStream;
	}
	stream = context->streamList;
	while(stream != 0)
	{
		if(!(stream->isHeader))
		{
			TreeCCStreamFlushStdio(stream, stdout);
		}
		stream = stream->nextStream;
	}

	/* Close the parser and the input stream */
	TreeCCContextDestroy(context);
	TreeCCClose(&input, 1);

	/* Done */
	return 0;
}

#ifdef	__cplusplus
};
#endif
