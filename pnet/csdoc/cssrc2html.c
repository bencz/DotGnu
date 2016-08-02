
/*
 * cssrc2html.c - C# source file to HTML translator
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * Author: Jeff Post
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
 *
 */

#include	<stdio.h>
#include	<stdlib.h>
#include	<string.h>

#include	"cssrc2html.h"
#include	"fontcolors.h"
#include	"tokens.h"
#include	"scanner.h"

#define	MAX_FN_LEN	128

char		lineBuffer[MAX_LENGTH];		// input line from source file
bool		inComment = FALSE;
bool		firstFont = TRUE;
int		token;
int		tabspace = 4;
int		column = 0;						// for expanding tabs
int		lastcolor;						// last color tagged in HTML file

char		basename[MAX_FN_LEN];		// input file base name
char		srcname[MAX_FN_LEN];			// name of input source file
char		dstname[MAX_FN_LEN];			// name of html output file
char		optiontext[MAX_FN_LEN];

// Read next line from input file

bool ReadFromTextFile(FILE *sr)
{
	if (fgets(lineBuffer, MAX_LENGTH - 1, sr) != NULL)
		return TRUE;
	return FALSE;
}

// Write line to output file.
// Returns true if write ok, else returns false.

bool WriteToHtmlFile(FILE *sw, char *str)
{
	if (fputs(str, sw) == EOF)
	{
		printf("Error writing HTML file!\n");
		return FALSE;
	}
	return TRUE;
}

// Put italic tag in HTML file

bool italicize(FILE *sw)
{
	if (fputs("<i>", sw) == EOF)
	{
		printf("Error writing HTML file!\n");
		return FALSE;
	}
	return TRUE;
}

bool deItalicize(FILE *sw)
{
	if (fputs("</i>", sw) == EOF)
	{
		printf("Error writing HTML file!\n");
		return FALSE;
	}
	return TRUE;
}

// Set font color tag in HTML file

bool setColor(FILE *sw, int color)
{
	if (!firstFont && (color != lastcolor) && (fputs("</FONT>", sw) == EOF))
	{
		printf("Error writing HTML file!\n");
		return FALSE;
	}
	firstFont = FALSE;
	if (fputs("<FONT class=\"", sw) == EOF)
	{
		printf("Error writing HTML file!\n");
		return FALSE;
	}
	if (fputs(fontColors[color], sw) == EOF)
	{
		printf("Error writing HTML file!\n");
		return FALSE;
	}
	if (fputs("\">", sw) == EOF)
	{
		printf("Error writing HTML file!\n");
		return FALSE;
	}
	return TRUE;
}

void usage(void)
{
	printf("\nUsage: cssrc2html [-options] filename"
			 "\noptions:"
			 "\n  -tn  set tab spacing to n (default 4)"
			 "\n  -v   display version number and exit"
			 "\n  -h or -? display this help message and exit"
			 "\n\n");
}

//
// The conversion program.
//

int main(int argc, char *argv[])
{
	int	i, j;
	int	keycolor;		// current font color for key words
	char	option;
	FILE	*sr;				// the input source file
	FILE	*sw;				// the output HTML file

	if (argc < 2)
	{
		usage();
		return 1;
	}

	lastcolor = keycolor = 0;
	namespace[0] = '\0';

	for (i=1; i<argc; i++)
	{
		if (*argv[i] == '-')			// if command line option
		{
			option = argv[i][1];
			if (option == 'v')			// report version and exit
			{
				printf("\ncssrc2html version %d.%d.%d\n\n", VERSION, REV_MAJOR, REV_MINOR);
				return 0;
			}
			else if (option == 't' || option == 'T')	// set tab spacing
			{
				j = strlen(argv[i]) - 2;
				strncpy(optiontext, &argv[i][2], j);
				optiontext[j] = '\0';
				tabspace = atoi(optiontext);
				if (tabspace < 2)
				{
					printf("\ntab spacing must be 2 or greater\n");
					return 1;
				}
			}
			else if (option == 'h' || option == 'H' || option == '?')
			{
				usage();
				return 1;
			}
		}
		else								// get name of input file
		{
			j = strlen(argv[i]) - 1;	// find file name extension, if any
			while (j > 0)
			{
				if (argv[i][j] == '.')
					break;
				--j;
			}

			if (!j)							// if user did not specify file extention, default to '.cs'
			{
				strcpy(basename, argv[i]);
				strcpy(srcname, basename);
				strcat(srcname, ".cs");
			}
			else								// else use file name specified
			{
				strncpy(basename, argv[i], j);
				strcpy(srcname, argv[i]);
			}
			strcpy(dstname, basename);
			strcat(dstname, ".html");
		}
	}

	printf("\ncssrc2html\n   input file: %s, output file %s\n", srcname, dstname);
	printf("   tab spacing %d\n\n", tabspace);

	sr = fopen(srcname, "r");
	if (!sr)
	{
		printf("Can't open input file\n");
		return 1;
	}

	sw = fopen(dstname, "w");
	if (!sw)
	{
		printf("Can't create HTML file!\n");
		fclose(sr);
		return 1;
	}

// Create preamble in output file

	fputs("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0//EN\">\n", sw);
	fputs("<HTML><HEAD>\n<TITLE>", sw);
	fputs(srcname, sw);
	fputs("</TITLE>\n", sw);
	fputs("<STYLE type=\"text/css\">\n", sw);
	for (i=0; i<MAX_COLOR; i++)
		fputs(fontColorText[i], sw);
	fputs("</STYLE>\n", sw);
	fputs("</HEAD><BODY><PRE>\n", sw);

	while (ReadFromTextFile(sr))		// process the input file line by line
	{
		token = TokenNULL;
		do
		{										// til end of line...
			token = getToken();

			if (token == TokenEND)		// end of line found
			{
				column = 0;
				if (!WriteToHtmlFile(sw, tokenString))
				{
					fclose(sr);
					return 1;
				}
			}
			else if (token == TokenTAB)	// expand tabs
			{
				do
				{
					if (!WriteToHtmlFile(sw, " "))
					{
						fclose(sr);
						return 1;
					}
					column++;
				} while ((column % tabspace) != 0);
			}
			else									// process token
			{
				column += strlen(tokenString);
				if (token == TokenCOMMENT)			// start of comment
				{
					inComment = TRUE;
					if (lastcolor != CommentColor && !setColor(sw, CommentColor))
					{
						fclose(sr);
						return 1;
					}
					lastcolor = CommentColor;
					if (!italicize(sw))
					{
						fclose(sr);
						return 1;
					}
					if (!WriteToHtmlFile(sw, tokenString))
					{
						fclose(sr);
						return 1;
					}
				}
				else if (token == TokenCOMMENTEND && inComment)	// end of comment
				{
					inComment = FALSE;
					if (!WriteToHtmlFile(sw, tokenString))
					{
						fclose(sr);
						return 1;
					}
					if (!deItalicize(sw))
					{
						fclose(sr);
						return 1;
					}
				}
				else
				{
					if (!inComment)		// set colors for keywords, if not in comment
					{
						switch (token)
						{
							case TokenDeclKEYWORD:
								keycolor = DeclKeywordColor;
								break;

							case TokenTypeKEYWORD:
								keycolor = TypeKeywordColor;
								break;

							case TokenStmtKEYWORD:
								keycolor = StmtKeywordColor;
								break;

							case TokenExprKEYWORD:
								keycolor = ExprKeywordColor;
								break;

							case TokenSTRING:
								keycolor = StringColor;
								break;

							default:				// not a keyword, so revert to normal color
								keycolor = NormalColor;
								break;
						}
						if (lastcolor != keycolor && !setColor(sw, keycolor))
						{
							fclose(sr);
							return 1;
						}
						lastcolor = keycolor;
					}
					if (!WriteToHtmlFile(sw, tokenString))	// output the token text
					{
						fclose(sr);
						return 1;
					}
				}
			}
		} while (token != TokenEND && token != TokenERROR);
	}

// Create postamble in output file

	if (fputs("<br><br></FONT></PRE></BODY></HTML>\n", sw) == EOF)
	{
		printf("Error writing HTML file!\n");
		fclose(sr);
		return 1;
	}
	fclose(sr);			// all done
	fflush(sw);
	fclose(sw);
	return 0;
}

// end of file
