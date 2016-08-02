/*
 * ilranlib.c - Stub for "ranlib", when dealing with IL binaries.
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
#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

int main(int argc, char *argv[])
{
	/* We don't use symbol tables in the IL toolchain, so this program is
	   just a stub to keep Unix-style make systems happy */
	if(argc > 1)
	{
		if(!strcmp(argv[1], "-V") || !strcmp(argv[1], "-v") ||
		   !strcmp(argv[1], "--version"))
		{
			printf("ILRANLIB " VERSION " - IL Archive Index Utility\n");
			printf("Copyright (c) 2003 Southern Storm Software, Pty Ltd.\n");
			printf("\n");
			printf("ILRANLIB comes with ABSOLUTELY NO WARRANTY.  This is free software,\n");
			printf("and you are welcome to redistribute it under the terms of the\n");
			printf("GNU General Public License.  See the file COPYING for further details.\n");
		}
	}
	return 0;
}

#ifdef	__cplusplus
};
#endif
