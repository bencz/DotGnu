/*
 * treecc node allocation routines for C.
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
 *
 * As a special exception, when this file is copied by treecc into
 * a treecc output file, you may use that output file without restriction.
 */

#include <stdlib.h>

#ifndef YYNODESTATE_BLKSIZ
#define	YYNODESTATE_BLKSIZ	2048
#endif

/*
 * Types used by the allocation routines.
 */
struct YYNODESTATE_block
{
	char data__[YYNODESTATE_BLKSIZ];
	struct YYNODESTATE_block *next__;

};
struct YYNODESTATE_push
{
	struct YYNODESTATE_push *next__;
	struct YYNODESTATE_block *saved_block__;
	int saved_used__;
};

/*
 * The fixed global state to use for non-reentrant allocation.
 */
#ifndef YYNODESTATE_REENTRANT
static YYNODESTATE fixed_state__;
#endif

/*
 * Some macro magic to determine the default alignment
 * on this machine.  This will compile down to a constant.
 */
#define	YYNODESTATE_ALIGN_CHECK_TYPE(type,name)	\
	struct _YYNODESTATE_align_##name { \
		char pad; \
		type field; \
	}
#define	YYNODESTATE_ALIGN_FOR_TYPE(type)	\
	((unsigned)(unsigned long)(&(((struct _YYNODESTATE_align_##type *)0)->field)))
#define	YYNODESTATE_ALIGN_MAX(a,b)	\
	((a) > (b) ? (a) : (b))
#define	YYNODESTATE_ALIGN_MAX3(a,b,c) \
	(YYNODESTATE_ALIGN_MAX((a), YYNODESTATE_ALIGN_MAX((b), (c))))
YYNODESTATE_ALIGN_CHECK_TYPE(int, int);
YYNODESTATE_ALIGN_CHECK_TYPE(long, long);
#if defined(WIN32) && !defined(__CYGWIN__)
YYNODESTATE_ALIGN_CHECK_TYPE(__int64, long_long);
#else
YYNODESTATE_ALIGN_CHECK_TYPE(long long, long_long);
#endif
YYNODESTATE_ALIGN_CHECK_TYPE(void *, void_p);
YYNODESTATE_ALIGN_CHECK_TYPE(float, float);
YYNODESTATE_ALIGN_CHECK_TYPE(double, double);
#define	YYNODESTATE_ALIGNMENT	\
	YYNODESTATE_ALIGN_MAX( \
			YYNODESTATE_ALIGN_MAX3	\
			(YYNODESTATE_ALIGN_FOR_TYPE(int), \
		     YYNODESTATE_ALIGN_FOR_TYPE(long), \
			 YYNODESTATE_ALIGN_FOR_TYPE(long_long)), \
  	     YYNODESTATE_ALIGN_MAX3 \
		 	(YYNODESTATE_ALIGN_FOR_TYPE(void_p), \
			 YYNODESTATE_ALIGN_FOR_TYPE(float), \
			 YYNODESTATE_ALIGN_FOR_TYPE(double)))

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
	YYNODESTATE *state__ = &fixed_state__;
#endif
	state__->blocks__ = 0;
	state__->push_stack__ = 0;
	state__->used__ = 0;
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
	YYNODESTATE *state__ = &fixed_state__;
#endif
	struct YYNODESTATE_block *block__;
	void *result__;

	/* Round the size to the next alignment boundary */
	size__ = (size__ + YYNODESTATE_ALIGNMENT - 1) &
				~(YYNODESTATE_ALIGNMENT - 1);

	/* Do we need to allocate a new block? */
	block__ = state__->blocks__;
	if(!block__ || (state__->used__ + size__) > YYNODESTATE_BLKSIZ)
	{
		if(size__ > YYNODESTATE_BLKSIZ)
		{
			/* The allocation is too big for the node pool */
			return (void *)0;
		}
		block__ = (struct YYNODESTATE_block *)
						malloc(sizeof(struct YYNODESTATE_block));
		if(!block__)
		{
			/* The system is out of memory.  The programmer can
			   supply the "yynodefailed" function to report the
			   out of memory state and/or abort the program */
#ifdef YYNODESTATE_REENTRANT
			yynodefailed(state__);
#else
			yynodefailed();
#endif
			return (void *)0;
		}
		block__->next__ = state__->blocks__;
		state__->blocks__ = block__;
		state__->used__ = 0;
	}

	/* Allocate the memory and return it */
	result__ = (void *)(block__->data__ + state__->used__);
	state__->used__ += size__;
	return result__;
}

/*
 * Push the node allocation state.
 */
#ifdef YYNODESTATE_REENTRANT
int yynodepush(state__)
YYNODESTATE *state__;
{
#else
int yynodepush()
{
	YYNODESTATE *state__ = &fixed_state__;
#endif
	struct YYNODESTATE_block *saved_block__;
	int saved_used__;
	struct YYNODESTATE_push *push_item__;

	/* Save the current state of the node allocation pool */
	saved_block__ = state__->blocks__;
	saved_used__ = state__->used__;

	/* Allocate space for a push item */
#ifdef YYNODESTATE_REENTRANT
	push_item__ = (struct YYNODESTATE_push *)
			yynodealloc(state__, sizeof(struct YYNODESTATE_push));
#else
	push_item__ = (struct YYNODESTATE_push *)
			yynodealloc(sizeof(struct YYNODESTATE_push));
#endif
	if(!push_item__)
	{
		return 0;
	}

	/* Copy the saved information to the push item */
	push_item__->saved_block__ = saved_block__;
	push_item__->saved_used__ = saved_used__;

	/* Add the push item to the push stack */
	push_item__->next__ = state__->push_stack__;
	state__->push_stack__ = push_item__;
	return 1;
}

/*
 * Pop the node allocation state.
 */
#ifdef YYNODESTATE_REENTRANT
void yynodepop(state__)
YYNODESTATE *state__;
{
#else
void yynodepop()
{
	YYNODESTATE *state__ = &fixed_state__;
#endif
	struct YYNODESTATE_push *push_item__;
	struct YYNODESTATE_block *saved_block__;
	struct YYNODESTATE_block *temp_block__;

	/* Pop the top of the push stack */
	push_item__ = state__->push_stack__;
	if(push_item__ == 0)
	{
		saved_block__ = 0;
		state__->used__ = 0;
	}
	else
	{
		saved_block__ = push_item__->saved_block__;
		state__->used__ = push_item__->saved_used__;
		state__->push_stack__ = push_item__->next__;
	}

	/* Free unnecessary blocks */
	while(state__->blocks__ != saved_block__)
	{
		temp_block__ = state__->blocks__;
		state__->blocks__ = temp_block__->next__;
		free(temp_block__);
	}
}

/*
 * Clear the node allocation pool completely.
 */
#ifdef YYNODESTATE_REENTRANT
void yynodeclear(state__)
YYNODESTATE *state__;
{
#else
void yynodeclear()
{
	YYNODESTATE *state__ = &fixed_state__;
#endif
	struct YYNODESTATE_block *temp_block__;
	while(state__->blocks__ != 0)
	{
		temp_block__ = state__->blocks__;
		state__->blocks__ = temp_block__->next__;
		free(temp_block__);
	}
	state__->push_stack__ = 0;
	state__->used__ = 0;
}
