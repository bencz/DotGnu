/*
 * interlocked_slist.c - Implementation for a lock free single linked list.
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

#include "thr_defs.h"
#include "interlocked_slist.h"

void ILInterlockedSListAppend(ILInterlockedSListHead *head,
							  ILInterlockedSListElement *elem)
{
	ILInterlockedSListElement *tail;
	ILInterlockedSListElement *prev_tail;

	IL_THREAD_ASSERT(head != 0);
	IL_THREAD_ASSERT(elem != 0);

	tail = &(head->tail);
	elem->next = &(head->tail);
	/* Exchange the current tail with the element to append. */
	/*
	 * We have to make sure that the next pointer is visible by other cpus
	 * at the time the tail is exchanged so we have to use release semantics
	 * here.
	 */
	prev_tail = (ILInterlockedSListElement *)ILInterlockedExchangeP_Release((void **)&(tail->next), elem);
	/* set the next pointer of the previous tail to the new element */
	prev_tail->next = elem;
	/* and make sure this is seen by all other threads */
	ILInterlockedMemoryBarrier();
}

void *ILInterlockedSListGet(ILInterlockedSListHead *head)
{
	ILInterlockedSListElement *tail;

	IL_THREAD_ASSERT(head != 0);

	tail = &(head->tail);
	do
	{
		ILInterlockedSListElement *prev_head;

		prev_head = (ILInterlockedSListElement *)ILInterlockedLoadP((void **)&(head->head.next));
		if(prev_head != tail)
		{
			ILInterlockedSListElement *next_head;

			next_head = (ILInterlockedSListElement *)ILInterlockedLoadP((void **)&(prev_head->next));
			if(ILInterlockedCompareAndExchangeP_Acquire((void **)&(head->head.next), next_head, prev_head) == prev_head)
			{
				/*
				 * We removed the head element successfully.
				 */
				if(next_head == tail)
				{
					/*
					 * The list is empty from the head's point of view.
					 * So try to reset the tail to the list's head.
					 */
					if(ILInterlockedCompareAndExchangeP_Acquire((void **)&(tail->next), &(head->head), prev_head) != prev_head)
					{
						/*
						 * We have to wait until the next pointer is set accordingly.
						 */
						do
						{
							_ILThreadYield();
							next_head = (ILInterlockedSListElement *)ILInterlockedLoadP((void **)&(prev_head->next));
						} while(next_head == tail);
						ILInterlockedStoreP_Acquire((void **)&(head->head.next), next_head);
					}
				}
				return prev_head;
			}
		}
		else
		{
			/* The list is empty */
			return 0;
		}
		
	} while(1);
	return 0;
}
