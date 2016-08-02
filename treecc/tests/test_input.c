/*
 * test_input.c - Test harness for the routines in "input.h".
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

#include "system.h"
#include "input.h"
#include "errors.h"

#ifdef	__cplusplus
extern	"C" {
#endif

extern FILE *TreeCCErrorFile;
extern int TreeCCErrorStripPath;

int main(int argc, char *argv[])
{
	FILE *infile;
	TreeCCInput input;
	int count;

	/* Validate the command-line argument */
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

	/* Read tokens until EOF */
	while(TreeCCNextToken(&input))
	{
		switch(input.token)
		{
			case TREECC_TOKEN_EOF:
			{
				TreeCCDebug(input.linenum,
					"eof marker returned non-zero (should not happen)");
			}
			break;

			case TREECC_TOKEN_IDENTIFIER:
			{
				TreeCCDebug(input.linenum, "identifier (len = %d): %s",
				       	    (int)strlen(input.text), input.text);
				if(!strcmp(input.text, "parse_literal"))
				{
					input.parseLiteral = 1;
				}
				else if(!strcmp(input.text, "no_parse_literal"))
				{
					input.parseLiteral = 0;
				}
			}
			break;

			case TREECC_TOKEN_LITERAL_DEFNS:
			{
				TreeCCDebug(input.linenum, "literal definitions:\n%s",
							input.text);
			}
			break;

			case TREECC_TOKEN_LITERAL_CODE:
			{
				TreeCCDebug(input.linenum, "literal code:\n%s", input.text);
			}
			break;

			case TREECC_TOKEN_LITERAL_END:
			{
				TreeCCDebug(input.linenum,
							"literal definitions at end:\n%s", input.text);
			}
			break;

			case TREECC_TOKEN_LPAREN:
			{
				TreeCCDebug(input.linenum, "(");
			}
			break;

			case TREECC_TOKEN_RPAREN:
			{
				TreeCCDebug(input.linenum, ")");
			}
			break;

			case TREECC_TOKEN_LBRACE:
			{
				TreeCCDebug(input.linenum, "{");
			}
			break;

			case TREECC_TOKEN_RBRACE:
			{
				TreeCCDebug(input.linenum, "}");
			}
			break;

			case TREECC_TOKEN_LSQUARE:
			{
				TreeCCDebug(input.linenum, "[");
			}
			break;

			case TREECC_TOKEN_RSQUARE:
			{
				TreeCCDebug(input.linenum, "]");
			}
			break;

			case TREECC_TOKEN_COMMA:
			{
				TreeCCDebug(input.linenum, ",");
			}
			break;

			case TREECC_TOKEN_EQUALS:
			{
				TreeCCDebug(input.linenum, "=");
			}
			break;

			case TREECC_TOKEN_STAR:
			{
				TreeCCDebug(input.linenum, "*");
			}
			break;

			case TREECC_TOKEN_REF:
			{
				TreeCCDebug(input.linenum, "&");
			}
			break;

			case TREECC_TOKEN_SEMI:
			{
				TreeCCDebug(input.linenum, ";");
			}
			break;

			case TREECC_TOKEN_COLON_COLON:
			{
				TreeCCDebug(input.linenum, "::");
			}
			break;

			case TREECC_TOKEN_STRING:
			{
				TreeCCDebug(input.linenum, "string: \"%s\"", input.text);
			}
			break;

			case TREECC_TOKEN_UNKNOWN:
			{
				TreeCCDebug(input.linenum, "unknown keyword");
			}
			break;

			case TREECC_TOKEN_NODE:
			{
				TreeCCDebug(input.linenum, "%%node");
			}
			break;

			case TREECC_TOKEN_ABSTRACT:
			{
				TreeCCDebug(input.linenum, "%%abstract");
			}
			break;

			case TREECC_TOKEN_TYPEDEF:
			{
				TreeCCDebug(input.linenum, "%%typedef");
			}
			break;

			case TREECC_TOKEN_OPERATION:
			{
				TreeCCDebug(input.linenum, "%%operation");
			}
			break;

			case TREECC_TOKEN_NOCREATE:
			{
				TreeCCDebug(input.linenum, "%%nocreate");
			}
			break;

			case TREECC_TOKEN_VIRTUAL:
			{
				TreeCCDebug(input.linenum, "%%virtual");
			}
			break;

			case TREECC_TOKEN_INLINE:
			{
				TreeCCDebug(input.linenum, "%%inline");
			}
			break;

			case TREECC_TOKEN_SPLIT:
			{
				TreeCCDebug(input.linenum, "%%split");
			}
			break;

			case TREECC_TOKEN_OPTION:
			{
				TreeCCDebug(input.linenum, "%%option");
			}
			break;

			case TREECC_TOKEN_HEADER:
			{
				TreeCCDebug(input.linenum, "%%header");
			}
			break;

			case TREECC_TOKEN_OUTPUT:
			{
				TreeCCDebug(input.linenum, "%%output");
			}
			break;

			case TREECC_TOKEN_OUTDIR:
			{
				TreeCCDebug(input.linenum, "%%outdir");
			}
			break;

			case TREECC_TOKEN_BOTH:
			{
				TreeCCDebug(input.linenum, "%%both");
			}
			break;

			case TREECC_TOKEN_DECLS:
			{
				TreeCCDebug(input.linenum, "%%decls");
			}
			break;

			case TREECC_TOKEN_END:
			{
				TreeCCDebug(input.linenum, "%%end");
			}
			break;

			case TREECC_TOKEN_ENUM:
			{
				TreeCCDebug(input.linenum, "%%enum");
			}
			break;

			case TREECC_TOKEN_COMMON:
			{
				TreeCCDebug(input.linenum, "%%common");
			}
			break;

			case TREECC_TOKEN_INCLUDE:
			{
				TreeCCDebug(input.linenum, "%%include");
			}
			break;

			case TREECC_TOKEN_READONLY:
			{
				TreeCCDebug(input.linenum, "%%readonly");
			}
			break;
		}
	}

	/* Check that extra reads at EOF will give EOF */
	for(count = 0; count < 50; ++count)
	{
		if(input.token != TREECC_TOKEN_EOF)
		{
			TreeCCError(&input, "read something other than EOF after EOF");
			break;
		}
		if(TreeCCNextToken(&input))
		{
			TreeCCError(&input, "did not get an EOF read after EOF");
			break;
		}
	}

	/* Close the token parser and the input file */
	TreeCCClose(&input, 1);

	/* Done */
	return 0;
}

#ifdef	__cplusplus
};
#endif
