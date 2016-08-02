/*
 * list.c - General purpose lists.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
 *
 * Author: Thong Nguyen (tum@veridicus.com)
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

#include "il_utils.h"
#include "il_system.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Singlely linked list implementation.
 */
typedef struct _tagILSinglelyLinkedListEntry ILSinglelyLinkedListEntry;

struct _tagILSinglelyLinkedListEntry
{
	ILSinglelyLinkedListEntry *next;
	void					*data;
};

typedef struct _tagILSinglelyLinkedList
{
	ILList list;
	ILSinglelyLinkedListEntry *first;
	ILSinglelyLinkedListEntry *last;
}
ILSinglelyLinkedList;

static void ILSinglelyLinkedListDestroy(ILList *_list)
{
	ILSinglelyLinkedList *list;
	ILSinglelyLinkedListEntry *entry, *next;

	list = (ILSinglelyLinkedList *)_list;

	entry = list->first;

	while (entry)
	{
		next = entry->next;

		ILFree(entry);

		entry = next;
	}

	ILFree(list);
}

static int ILSinglelyLinkedListAppend(ILList *_list, void *data)
{
	ILSinglelyLinkedList *list;
	ILSinglelyLinkedListEntry *entry;

	entry = (ILSinglelyLinkedListEntry *)ILMalloc(sizeof(*entry));

	if (entry == 0)
	{
		return -1;
	}

	entry->data = data;
	entry->next = 0;

	list = (ILSinglelyLinkedList *)_list;

	if (list->last)
	{
		list->last->next = entry;
		list->last = entry;
	}
	else
	{
		list->first = list->last = entry;
	}

	list->list.count++;

	return list->list.count - 1;
}

static void *ILSinglelyLinkedListGetAt(ILList *_list, int index)
{
	ILSinglelyLinkedList *list;
	ILSinglelyLinkedListEntry *entry;
	
	list = (ILSinglelyLinkedList *)_list;

	entry = list->first;

	if (index < 0)
	{
		return 0;
	}

	if (index >= list->list.count)
	{
		return 0;
	}

	while (entry)
	{
		if (index == 0)
		{
			return entry->data;
		}

		index--;
	}

	return 0;
}

static void **ILSinglelyLinkedListFind(ILList *_list, void *data)
{
	int index;
	ILSinglelyLinkedList *list;
	ILSinglelyLinkedListEntry *entry;

	list = (ILSinglelyLinkedList *)_list;

	entry = list->first;

	index = 0;

	while (entry)
	{
		if (entry->data == data)
		{
			return &entry->data;
		}

		index++;
		entry = entry->next;
	}

	return 0;
}

static void **ILSinglelyLinkedListReverseFind(ILList *_list, void *data)
{
	int index;
	void **found;
	ILSinglelyLinkedList *list;
	ILSinglelyLinkedListEntry *entry;

	list = (ILSinglelyLinkedList *)_list;

	entry = list->first;

	index = 0;
	found = 0;

	while (entry)
	{
		if (entry->data == data)
		{
			found = &entry->data;
		}

		index++;
		entry = entry->next;
	}

	return found;
}

static int ILSinglelyLinkedListRemove(ILList *_list, void *data)
{
	int index;
	ILSinglelyLinkedList *list;
	ILSinglelyLinkedListEntry *entry, *prev;

	list = (ILSinglelyLinkedList *)_list;

	prev = 0;
	entry = list->first;

	index = 0;

	while (entry)
	{
		if (entry->data == data)
		{
			if (prev == 0)
			{
				list->first = entry->next;

				if (entry->next == 0)
				{
					list->last = 0;
				}				
			}
			else
			{
				prev->next = entry->next;

				if (entry->next == 0)
				{
					list->last = prev;
				}
			}

			ILFree(entry);

			list->list.count--;

			return index;
		}

		index++;
		prev = entry;
		entry = entry->next;
	}

	return -1;
}

static int ILSinglelyLinkedListRemoveAt(ILList *_list, int index)
{
	int count;
	ILSinglelyLinkedList *list;
	ILSinglelyLinkedListEntry *entry, *prev;

	list = (ILSinglelyLinkedList *)_list;

	prev = 0;
	entry = list->first;

	if (index < 0)
	{
		return -1;
	}

	if (index >= list->list.count)
	{
		return -1;
	}

	count = 0;

	while (entry)
	{
		if (count == index)
		{
			if (prev == 0)
			{
				list->first = entry->next;

				if (entry->next == 0)
				{
					list->last = 0;
				}				
			}
			else
			{
				prev->next = entry->next;

				if (entry->next == 0)
				{
					list->last = prev;
				}
			}

			ILFree(entry);

			list->list.count--;

			return index;
		}

		count++;
		prev = entry;
		entry = entry->next;
	}

	return -1;
}

static int ILSinglelyLinkedListWalk(ILList *_list, ILListWalkCallback WalkCallbackFunc, void *state)
{
	int count;
	ILSinglelyLinkedList *list;
	ILSinglelyLinkedListEntry *entry, *next;

	list = (ILSinglelyLinkedList *)_list;

	entry = list->first;

	count = 0;

	while (entry)
	{		
		count++;

		next = entry->next;

		if (!WalkCallbackFunc(_list, &entry->data, state))
		{
			return count;
		}

		entry = next;
	}

	return count;
}

ILList *ILSinglelyLinkedListCreate()
{
	ILSinglelyLinkedList *list;

	list = (ILSinglelyLinkedList *)ILMalloc(sizeof(ILSinglelyLinkedList));

	list->list.count = 0;

	list->list.appendFunc = ILSinglelyLinkedListAppend;
	list->list.findFunc = ILSinglelyLinkedListFind;
	list->list.reverseFindFunc = ILSinglelyLinkedListReverseFind;
	list->list.destroyFunc = ILSinglelyLinkedListDestroy;
	list->list.getAtFunc = ILSinglelyLinkedListGetAt;
	list->list.walkFunc = ILSinglelyLinkedListWalk;
	list->list.removeAtFunc = ILSinglelyLinkedListRemoveAt;
	list->list.removeFunc = ILSinglelyLinkedListRemove;

	list->first = 0;
	list->last = 0;

	return (ILList *)list;
}

/*
 * ArrayList implementation.
 */

typedef struct _tagILArraylist
{
	ILList list;
	int capacity;
	void **array;
}
ILArrayList;

static void ILArrayListDestroy(ILList *_list)
{
	ILArrayList *list = (ILArrayList *)_list;

	ILFree(list->array);
	ILFree(list);
}

static int ILArrayListAppend(ILList *_list, void *data)
{
	void **newArray;
	ILArrayList *list = (ILArrayList *)_list;

	if (list->list.count == list->capacity)
	{
		newArray = (void **)ILMalloc(list->capacity * 2);

		if (newArray == 0)
		{
			return -1;
		}

		ILMemCpy(newArray, list->array, list->capacity * sizeof(void *));

		ILFree(list->array);
		list->array = newArray;
		list->capacity = list->capacity * 2;
	}

	list->array[list->list.count++] = data;

	return list->list.count - 1;
}

static void *ILArrayListGetAt(ILList *_list, int index)
{
	ILArrayList *list = (ILArrayList *)_list;

	if (index < 0)
	{
		return 0;
	}

	if (index >= list->list.count)
	{
		return 0;
	}

	return list->array[index];
}

static void **ILArrayListFind(ILList *_list, void *data)
{
	int i;
	ILArrayList *list = (ILArrayList *)_list;

	for (i = 0; i < list->list.count; i++)
	{
		if (list->array[i] == data)
		{
			return &list->array[i];
		}
	}

	return 0;
}

static void **ILArrayListReverseFind(ILList *_list, void *data)
{
	int i;
	ILArrayList *list = (ILArrayList *)_list;

	for (i = list->list.count - 1; i >= 0; i--)
	{
		if (list->array[i] == data)
		{
			return &list->array[i];
		}
	}

	return 0;
}

static int ILArrayListRemove(ILList *_list, void *data)
{
	int i;
	ILArrayList *list = (ILArrayList *)_list;

	for (i = 0; i < list->list.count; i++)
	{
		if (list->array[i] == data)
		{
			return i;
		}
	}

	list->list.count--;

	return -1;
}

static int ILArrayListRemoveAt(ILList *_list, int index)
{
	ILArrayList *list = (ILArrayList *)_list;

	if (index < 0)
	{
		return -1;
	}

	if (index >= list->list.count)
	{
		return -1;
	}

	ILMemCpy(list->array + index, list->array + index + 1, list->list.count - index - 1);

	list->list.count--;
	list->array[list->list.count] = 0;

	return index;
}

static int ILArrayListWalk(ILList *_list, ILListWalkCallback WalkCallbackFunc, void *state)
{
	int i;
	ILArrayList *list = (ILArrayList *)_list;

	for (i = 0; i < list->list.count; i++)
	{
		if (!WalkCallbackFunc(_list, &list->array[i], state))
		{
			return i + 1;
		}
	}

	return i;
}

ILList *ILArrayListCreate(int capacity)
{
	ILArrayList *list;

	if ((list = (ILArrayList *)ILMalloc(sizeof(*list))) == 0)
	{
		return 0;
	}

	if (capacity < 16)
	{
		capacity = 16;
	}

	list->array = (void **)ILMalloc(capacity * sizeof(void *));

	if (list->array == 0)
	{
		ILFree(list);

		return 0;
	}

	list->list.count = 0;
	list->capacity = capacity;

	list->list.appendFunc = ILArrayListAppend;
	list->list.findFunc = ILArrayListFind;
	list->list.reverseFindFunc = ILArrayListReverseFind;
	list->list.destroyFunc = ILArrayListDestroy;
	list->list.getAtFunc = ILArrayListGetAt;
	list->list.walkFunc = ILArrayListWalk;
	list->list.removeAtFunc = ILArrayListRemoveAt;
	list->list.removeFunc = ILArrayListRemove;

	return (ILList *)list;
}

#ifdef	__cplusplus
};
#endif
