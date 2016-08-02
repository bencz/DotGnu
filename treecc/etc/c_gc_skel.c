/*
 * treecc node allocation routines for C.
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
 *
 * As a special exception, when this file is copied by treecc into
 * a treecc output file, you may use that output file without restriction.
 */

#include <stdlib.h>
#include <gc.h>

/*
 * Initialize the node allocation pool.
 */
#ifdef YYNODESTATE_REENTRANT
void yynodeinit(state__)
YYNODESTATE *state__;
{
#else
void yynodeinit()
{
#endif
	GC_INIT();
	GC_init();
}

/*
 * Allocate a block of memory.
 */
#ifdef YYNODESTATE_REENTRANT
void *yynodealloc(state__, size__)
YYNODESTATE *state__;
unsigned int size__;
{
#else
void *yynodealloc(size__)
unsigned int size__;
{
#endif
	return (void *)GC_MALLOC((size_t)size__);
}

/*
 * Push the node allocation state.  Not used in the GC version.
 */
#ifdef YYNODESTATE_REENTRANT
int yynodepush(state__)
YYNODESTATE *state__;
{
#else
int yynodepush()
{
#endif
	return 1;
}

/*
 * Pop the node allocation state.  Not used in the GC version.
 */
#ifdef YYNODESTATE_REENTRANT
void yynodepop(state__)
YYNODESTATE *state__;
{
#else
void yynodepop()
{
#endif
}

/*
 * Clear the node allocation pool completely.  Not used in the GC version.
 */
#ifdef YYNODESTATE_REENTRANT
void yynodeclear(state__)
YYNODESTATE *state__;
{
#else
void yynodeclear()
{
#endif
}
