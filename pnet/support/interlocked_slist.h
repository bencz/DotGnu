/*
 * interlocked_slist.h - Declarations for a lock free single linked list.
 *
 * Copyright (C) 2010  Southern Storm Software, Pty Ltd.
 *
 * Authors: Klaus Treichel (ktreichel@web.de)
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

#ifndef _INTERLOCKED_SLIST_H_
#define _INTERLOCKED_SLIST_H_

#include "interlocked.h"

/*
 * Semantics:
 */

typedef struct _tagILInterlockedSListHead ILInterlockedSListHead;
typedef struct _tagILInterlockedSListElement ILInterlockedSListElement;
struct _tagILInterlockedSListElement
{
	void   *next;	/* Really an ILInterlockedSListElement */
};

struct _tagILInterlockedSListHead
{
	ILInterlockedSListElement	head;
	ILInterlockedSListElement	tail;
};

static IL_INLINE void ILInterlockedSListHead_Init(ILInterlockedSListHead *head)
{
	head->head.next = &(head->tail);
	head->tail.next = &(head->head);
}

static IL_INLINE int ILInterlockedSList_IsEmpty(ILInterlockedSListHead *head)
{
	return (ILInterlockedLoadP(&(head->head.next)) == (void *)&(head->tail));
}

static IL_INLINE int ILInterlockedSList_IsClean(ILInterlockedSListHead *head)
{
	return ((ILInterlockedLoadP(&(head->head.next)) == (void *)&(head->tail)) &&
			(ILInterlockedLoadP(&(head->tail.next)) == (void *)&(head->head)));
}

/*
 * Append an element to the single linked list.
 */
void ILInterlockedSListAppend(ILInterlockedSListHead *head,
							  ILInterlockedSListElement *elem);

/*
 * Get the head element of the single linked list and remove it from the list.
 * The pointer returned is a pointer to the ILInterlockedSListElement removed.
 * Returns 0 if the list is empty.
 */
void *ILInterlockedSListGet(ILInterlockedSListHead *head);

#endif /* _INTERLOCKED_SLIST_H_ */
