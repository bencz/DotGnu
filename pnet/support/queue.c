/*
 * queue.cs - Implementation of the ILQueue and related functions
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 *
 * Contribution from Charles Schuller <kyeran@hermes-solutions.biz>
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

/*  I need a singly linked list, so I'm starting this util lib  */

#include <stdio.h>
#include <stdlib.h>
#include "il_utils.h"
#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

struct _tagILQueueEntry
{
	void *data;
	struct _tagILQueueEntry *nextNode;
};

ILQueueEntry *ILQueueCreate(void)
{
	return NULL; /* could have saved a func -- Gopal */
}

void ILQueueDestroy(ILQueueEntry **listRoot)
{
	if(*listRoot == NULL)
	{
		/*  Queue is already gone  */
		/*  Just return, as this will be a 'cleanup' function anyway  */
		return;
	}
	if((*listRoot)->nextNode == NULL)
	{
		/*  Special case for the first node  */
		free((*listRoot)->data);
		free(*listRoot);
		*listRoot = NULL;
		return;
	}
	else
	{
		ILQueueDestroy(&((*listRoot)->nextNode));
		free((*listRoot)->data);
		free(*listRoot);
		*listRoot = NULL;
		return;
	}
}

int ILQueueAdd(ILQueueEntry **listRoot, void *newData)
{
	ILQueueEntry *tmpNode = NULL;

	if(listRoot == NULL)
	{
		*listRoot = (ILQueueEntry *)ILMalloc(sizeof(ILQueueEntry));
		if(!*listRoot)return 0; /* out of memory */
		(*listRoot)->data = newData;
		(*listRoot)->nextNode = NULL;
	}
	else
	{
		tmpNode = (ILQueueEntry *)malloc(sizeof(ILQueueEntry));
		if(!tmpNode)return 0; /* out of memory */
		tmpNode->data = newData;
		tmpNode->nextNode = *listRoot;
		(*listRoot) = tmpNode;
	}
	return 1;	
}

void *ILQueueRemove(ILQueueEntry **listRoot)
{
	void *thisData;

	if((*listRoot)->nextNode == NULL)
	{
		/*  This is the only node so just remove it  */
		thisData = (*listRoot)->data;
		/*  Don't free the data, we are returning it  
		 *  we should free it after we use it */
		ILFree(*listRoot);
		*listRoot = NULL;
		return thisData;
	}
	else if((*listRoot)->nextNode->nextNode == NULL)
	{
		thisData = (*listRoot)->nextNode->data;
		ILFree((*listRoot)->nextNode);
		(*listRoot)->nextNode = NULL;
		return thisData;
	}
	else
	{
		return ILQueueRemove(&(*listRoot)->nextNode);
	}
}

#ifdef	__cplusplus
};
#endif
