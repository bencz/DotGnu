/*
 * CTempFileList.c - Temporary file list implementation.
 *
 * Copyright (C) 2006  Free Software Foundation, Inc.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

#include "CTempFileList.h"
#include "CUtils.h"

#ifdef __cplusplus
extern "C" {
#endif

/* Structure of a temporary file list node. */
typedef struct _tagCTempFileNode CTempFileNode;
struct _tagCTempFileNode
{
	CTempFileNode *next;
	CChar8        *path;
};

/* Structure of a temporary file list. */
struct _tagCTempFileList
{
	CTempFileNode *head;
	CTempFileNode *tail;
};

/*\
|*| Create a temporary file list.
|*|
|*|   _this - new temporary file list
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CTempFileList_Create(CTempFileList **_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* allocate the temporary file list */
	if(!(*_this = (CTempFileList *)CMalloc(sizeof(CTempFileList))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the head and tail */
	(*_this)->head = 0;
	(*_this)->tail = 0;

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Destroy a temporary file list.
|*|
|*|   _this - temporary file list to be destroyed
|*|
|*|  NOTE: this method deletes all the temporary files
|*|        added to this list, and frees the path strings
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CTempFileList_Destroy(CTempFileList **_this)
{
	/* assertions */
	CASSERT((_this  != 0));
	CASSERT((*_this != 0));

	/* destroy any temporary files */
	if((*_this)->head != 0)
	{
		/* declarations */
		CTempFileNode *curr;
		CTempFileNode *next;

		/* destroy the temporary files */
		for(curr = (*_this)->head; curr != 0; curr = next)
		{
			/* get the next temporary file */
			next = curr->next;

			/* delete the file */
			CUtils_DeleteFile(curr->path);

			/* free the path string */
			CFree(curr->path);

			/* free the node */
			CFree(curr);
		}
	}

	/* free the temporary file list */
	CFree(*_this);

	/* null the temporary file list */
	*_this = 0;

	/* return successfully */
	return CStatus_OK;
}

/*\
|*| Add a temporary file to this temporary file list.
|*|
|*|   _this - this temporary file list
|*|    path - path to temporary file (list assumes ownership)
|*|
|*|  Returns status code.
\*/
CINTERNAL CStatus
CTempFileList_AddFile(CTempFileList *_this,
                      CChar8        *path)
{
	/* declarations */
	CTempFileNode *node;

	/* assertions */
	CASSERT((_this != 0));
	CASSERT((path  != 0));

	/* allocate the temporary file node */
	if(!(node = (CTempFileNode *)CMalloc(sizeof(CTempFileNode))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the node */
	node->path = path;
	node->next = 0;

	/* add the node to the list */
	if(_this->head == 0)
	{
		_this->head = node;
		_this->tail = node;
	}
	else
	{
		_this->tail->next = node;
		_this->tail       = node;
	}

	/* return successfully */
	return CStatus_OK;
}


#ifdef __cplusplus
};
#endif
