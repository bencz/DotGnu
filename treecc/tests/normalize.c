/*
 * normalize.c - Normalize end of line markers.
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

#include <stdio.h>

#ifdef	__cplusplus
extern	"C" {
#endif

int main(int argc, char *argv[])
{
	int ch;
	while((ch = getc(stdin)) != EOF)
	{
		if(ch == 0x0A)
		{
			putc('\n', stdout);
		}
		else if(ch == 0x0D)
		{
			putc('\n', stdout);
			ch = getc(stdin);
			if(ch == EOF)
			{
				break;
			}
			else if(ch != 0x0A)
			{
				ungetc(ch, stdin);
			}
		}
		else
		{
			putc(ch, stdout);
		}
	}
	return 0;
}

#ifdef	__cplusplus
};
#endif
