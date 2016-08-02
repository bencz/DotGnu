/*
 * funcptr4.c - Test function pointer usage, when addressed with "&".
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

/* Thanks to James McParlane for the test case */

typedef unsigned int size_t;

static int Compare( const void* p_Node1, const void* p_Node2 ) 
{ 
	return 0;
} 

void qsort( void*, size_t, size_t, int( * )( const void*, const void* ) ); 


main() 
{ 
	int ** children = 0;
	unsigned int count = 0;
	qsort( children, count, sizeof(void*), &Compare ); 
	return 0;
} 
