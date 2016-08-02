/*
 * src2xml.c - Document generation helper tool.  This program
 *             converts source code into XML that describes
 *             the complete set of documentable items
 *             in the source files.
 *
 * Copyright (C) 2000, 2002  Southern Storm Software, Pty Ltd.
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

#include <stdio.h>
#include <string.h>
#include <ctype.h>

static void Convert(const char *filename);

int main(int argc, char *argv[])
{
	printf("<?xml version=\"1.0\"?>\n");
	printf("<srcdoc>\n");
	while(argc > 1)
	{
		Convert(argv[1]);
		++argv;
		--argc;
	}
	printf("</srcdoc>\n");
	return 0;
}

static void Convert(const char *filename)
{
	char buffer[BUFSIZ];
	FILE *file;
	int temp;
	int inLiteral = 0;

	file = fopen(filename, "r");
	if(!file)
		return;

	while(fgets(buffer, BUFSIZ, file))
	{
		/* Strip spaces from the start and end of the line */
		temp = 0;
		while(buffer[temp] != '\0' && isspace(buffer[temp]))
			++temp;
		if(temp > 0)
			memmove(buffer, buffer + temp, strlen(buffer + temp) + 1);
		temp = strlen(buffer);
		while(temp > 0 && isspace(buffer[temp - 1]))
			--temp;
		buffer[temp] = '\0';

		/* Is this a literal XML text line? */
		if(inLiteral)
		{
			if(!strncmp(buffer, "*/", 2))
			{
				inLiteral = 0;
				continue;
			}
			temp = 0;
			while(buffer[temp] != '\0' &&
				  (isspace(buffer[temp]) || buffer[temp] == '*'))
				++temp;
			if(temp > 0)
				memmove(buffer, buffer + temp, strlen(buffer + temp) + 1);
			puts(buffer);
			continue;
		}
		else if(!strncmp(buffer, "/**", 3))
		{
			inLiteral = 1;
			continue;
		}
	}

	fclose(file);
}
