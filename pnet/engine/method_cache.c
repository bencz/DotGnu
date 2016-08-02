/*
 * method_cache.c - Method cache implementation.
 *
 * Copyright (C) 2002, 2009  Southern Storm Software, Pty Ltd.
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

/*
See the bottom of this file for documentation on the cache system.
*/

#include "method_cache.h"
#include "il_system.h"
#include "il_align.h"
#include "il_meta.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Tune the default size of a cache page.  Memory is allocated from
 * the system in chunks of this size.  This will also determine
 * the maximum method size that can be translated.
 */
#ifndef	IL_CONFIG_CACHE_PAGE_SIZE
#define	IL_CONFIG_CACHE_PAGE_SIZE		(128 * 1024)
#endif

/*
 * Structure of a debug information header for a method.
 * This header is followed by the debug data, which is
 * stored as compressed metadata integers.
 */
typedef struct _tagILCacheDebug ILCacheDebug;
struct _tagILCacheDebug
{
	ILCacheDebug   *next;			/* Next block for the method */

};

/*
 * Method information block, organised as a red-black tree node.
 * There may be more than one such block associated with a method
 * if the method contains exception regions.
 */
typedef struct _tagILCacheMethod ILCacheMethod;
struct _tagILCacheMethod
{
	void		   *method;			/* Method containing the region */
	void		   *cookie;			/* Cookie value for the region */
	unsigned char  *start;			/* Start of the region */
	unsigned char  *end;			/* End of the region */
	ILCacheDebug   *debug;			/* Debug information for method */
	ILCacheMethod  *left;			/* Left sub-tree and red/black bit */
	ILCacheMethod  *right;			/* Right sub-tree */

};

/*
 * Structure of the method cache.
 */
#define	IL_CACHE_DEBUG_SIZE		64
struct _tagILCache
{
	void		    **pages;		/* List of pages currently in the cache */
	unsigned long	  numPages;		/* Number of pages currently in the cache */
	unsigned long	  pageSize;		/* Size of a page for allocation */
	unsigned char    *freeStart;	/* Start of the current free region */
	unsigned char    *freeEnd;		/* End of the current free region */
	int				  outOfMemory;	/* True when cache is out of memory */
	int				  needRestart;	/* True when page restart is required */
	long			  pagesLeft;	/* Number of pages left to allocate */
	ILCacheMethod    *method;		/* Information for the current method */
	ILCacheMethod	  head;			/* Head of the lookup tree */
	ILCacheMethod	  nil;			/* Nil pointer for the lookup tree */
	unsigned char    *start;		/* Start of the current method */
	unsigned char	  debugData[IL_CACHE_DEBUG_SIZE];
	int				  debugLen;		/* Length of temporary debug data */
	ILCacheDebug     *firstDebug;	/* First debug block for method */
	ILCacheDebug     *lastDebug;	/* Last debug block for method */

};

/*
 * Allocate a cache page and add it to the cache.
 */
static void AllocCachePage(ILCache *cache)
{
	void *ptr;
	void **list;

	/* If we are already out of memory, then bail out */
	if(cache->outOfMemory || !(cache->pagesLeft))
	{
		goto failAlloc;
	}

	/* Try to allocate a physical page */
	ptr = ILPageAlloc(cache->pageSize);
	if(!ptr)
	{
		goto failAlloc;
	}

	/* Add the page to the page list.  We keep this in an array
	   that is separate from the pages themselves so that we don't
	   have to "touch" the pages to free them.  Touching the pages
	   may cause them to be swapped in if they are currently out.
	   There's no point doing that if we are trying to free them */
	list = (void **)ILRealloc(cache->pages, sizeof(void *) *
											(cache->numPages + 1));
	if(!list)
	{
		ILPageFree(ptr, cache->pageSize);
	failAlloc:
		cache->outOfMemory = 1;
		cache->freeStart = 0;
		cache->freeEnd = 0;
		return;
	}
	cache->pages = list;
	list[(cache->numPages)++] = ptr;

	/* One less page before we hit the limit */
	if(cache->pagesLeft > 0)
	{
		--(cache->pagesLeft);
	}

	/* Set up the working region within the new page */
	cache->freeStart = ptr;
	cache->freeEnd = (void *)(((char *)ptr) + (int)(cache->pageSize));
}

/*
 * Get or set the sub-trees of a node.
 */
#define	GetLeft(node)	\
	((ILCacheMethod *)(((ILNativeUInt)((node)->left)) & ~((ILNativeUInt)1)))
#define	GetRight(node)	((node)->right)
#define	SetLeft(node,value)	\
	((node)->left = (ILCacheMethod *)(((ILNativeUInt)(value)) | \
						(((ILNativeUInt)((node)->left)) & ((ILNativeUInt)1))))
#define	SetRight(node,value)	\
			((node)->right = (value))

/*
 * Get or set the red/black state of a node.
 */
#define	GetRed(node)	\
	((((ILNativeUInt)((node)->left)) & ((ILNativeUInt)1)) != 0)
#define	SetRed(node)	\
	((node)->left = (ILCacheMethod *)(((ILNativeUInt)((node)->left)) | \
									  ((ILNativeUInt)1)))
#define	SetBlack(node)	\
	((node)->left = (ILCacheMethod *)(((ILNativeUInt)((node)->left)) & \
									  ~((ILNativeUInt)1)))

/*
 * Compare a key against a node, being careful of sentinel nodes.
 */
static int CacheCompare(ILCache *cache, unsigned char *key,
						ILCacheMethod *node)
{
	if(node == &(cache->nil) || node == &(cache->head))
	{
		/* Every key is greater than the sentinel nodes */
		return 1;
	}
	else
	{
		/* Compare a regular node */
		if(key < node->start)
		{
			return -1;
		}
		else if(key > node->start)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
}

/*
 * Rotate a sub-tree around a specific node.
 */
static ILCacheMethod *CacheRotate(ILCache *cache, unsigned char *key,
								  ILCacheMethod *around)
{
	ILCacheMethod *child, *grandChild;
	int setOnLeft;
	if(CacheCompare(cache, key, around) < 0)
	{
		child = GetLeft(around);
		setOnLeft = 1;
	}
	else
	{
		child = GetRight(around);
		setOnLeft = 0;
	}
	if(CacheCompare(cache, key, child) < 0)
	{
		grandChild = GetLeft(child);
		SetLeft(child, GetRight(grandChild));
		SetRight(grandChild, child);
	}
	else
	{
		grandChild = GetRight(child);
		SetRight(child, GetLeft(grandChild));
		SetLeft(grandChild, child);
	}
	if(setOnLeft)
	{
		SetLeft(around, grandChild);
	}
	else
	{
		SetRight(around, grandChild);
	}
	return grandChild;
}

/*
 * Split a red-black tree at the current position.
 */
#define	Split()		\
			do { \
				SetRed(temp); \
				SetBlack(GetLeft(temp)); \
				SetBlack(GetRight(temp)); \
				if(GetRed(parent)) \
				{ \
					SetRed(grandParent); \
					if((CacheCompare(cache, key, grandParent) < 0) != \
							(CacheCompare(cache, key, parent) < 0)) \
					{ \
						parent = CacheRotate(cache, key, grandParent); \
					} \
					temp = CacheRotate(cache, key, greatGrandParent); \
					SetBlack(temp); \
				} \
			} while (0)

/*
 * Add a method region block to the red-black lookup tree
 * that is associated with a method cache.
 */
static void AddToLookupTree(ILCache *cache, ILCacheMethod *method)
{
	unsigned char *key = method->start;
	ILCacheMethod *temp;
	ILCacheMethod *greatGrandParent;
	ILCacheMethod *grandParent;
	ILCacheMethod *parent;
	ILCacheMethod *nil = &(cache->nil);
	int cmp;

	/* Search for the insert position */
	temp = &(cache->head);
	greatGrandParent = temp;
	grandParent = temp;
	parent = temp;
	while(temp != nil)
	{
		/* Adjust our ancestor pointers */
		greatGrandParent = grandParent;
		grandParent = parent;
		parent = temp;

		/* Compare the key against the current node */
		cmp = CacheCompare(cache, key, temp);
		if(cmp == 0)
		{
			/* This is a duplicate, which normally shouldn't happen.
			   If it does happen, then ignore the node and bail out */
			return;
		}
		else if(cmp < 0)
		{
			temp = GetLeft(temp);
		}
		else
		{
			temp = GetRight(temp);
		}

		/* Do we need to split this node? */
		if(GetRed(GetLeft(temp)) && GetRed(GetRight(temp)))
		{
			Split();
		}
	}

	/* Insert the new node into the current position */
	method->left = (ILCacheMethod *)(((ILNativeUInt)nil) | ((ILNativeUInt)1));
	method->right = nil;
	if(CacheCompare(cache, key, parent) < 0)
	{
		SetLeft(parent, method);
	}
	else
	{
		SetRight(parent, method);
	}
	Split();
	SetBlack(cache->head.right);
}

/*
 * Flush the current debug buffer.
 */
static void FlushCacheDebug(ILCachePosn *posn)
{
	ILCache *cache = posn->cache;
	ILCacheDebug *debug;

	/* Allocate a new ILCacheDebug structure to hold the data */
	debug = _ILCacheAlloc(posn, (unsigned long)(sizeof(ILCacheDebug) +
												cache->debugLen));
	if(!debug)
	{
		cache->debugLen = 0;
		return;
	}

	/* Copy the temporary debug data into the new structure */
	ILMemCpy(debug + 1, cache->debugData, cache->debugLen);

	/* Link the structure into the debug list */
	debug->next = 0;
	if(cache->lastDebug)
	{
		cache->lastDebug->next = debug;
	}
	else
	{
		cache->firstDebug = debug;
	}
	cache->lastDebug = debug;

	/* Reset the temporary debug buffer */
	cache->debugLen = 0;
}

/*
 * Write a debug pair to the cache.  The pair (-1, -1)
 * terminates the debug information for a method.
 */
static void WriteCacheDebug(ILCachePosn *posn, ILInt32 offset, ILInt32 nativeOffset)
{
	ILCache *cache = posn->cache;

	/* Write the two values to the temporary debug buffer */
	cache->debugLen += ILMetaCompressInt(cache->debugData + cache->debugLen,
										 offset);
	cache->debugLen += ILMetaCompressInt(cache->debugData + cache->debugLen,
										 nativeOffset);
	if((cache->debugLen + IL_META_COMPRESS_MAX_SIZE * 2 + 1) >
			(int)(sizeof(cache->debugData)))
	{
		/* Overflow occurred: write -2 to mark the end of this buffer */
		cache->debugLen += ILMetaCompressInt
				(cache->debugData + cache->debugLen, -2);

		/* Flush the debug data that we have collected so far */
		FlushCacheDebug(posn);
	}
}

ILCache *_ILCacheCreate(long limit, unsigned long cachePageSize)
{
	ILCache *cache;
	unsigned long size;

	/* Allocate space for the cache control structure */
	if((cache = (ILCache *)ILMalloc(sizeof(ILCache))) == 0)
	{
		return 0;
	}

	/* Initialize the rest of the cache fields */
	cache->pages = 0;
	cache->numPages = 0;
	size = ILPageAllocSize();
	if (cachePageSize == 0)
	{
		cachePageSize= IL_CONFIG_CACHE_PAGE_SIZE;
	}
	size = (cachePageSize / size) * size;
	if(!size)
	{
		size = ILPageAllocSize();
	}
	cache->pageSize = size;
	cache->freeStart = 0;
	cache->freeEnd = 0;
	cache->outOfMemory = 0;
	cache->needRestart = 0;
	if(limit > 0)
	{
		cache->pagesLeft = limit / size;
		if(cache->pagesLeft < 1)
		{
			cache->pagesLeft = 1;
		}
	}
	else
	{
		cache->pagesLeft = -1;
	}
	cache->method = 0;
	cache->nil.method = 0;
	cache->nil.cookie = 0;
	cache->nil.start = 0;
	cache->nil.end = 0;
	cache->nil.debug = 0;
	cache->nil.left = &(cache->nil);
	cache->nil.right = &(cache->nil);
	cache->head.method = 0;
	cache->head.cookie = 0;
	cache->head.start = 0;
	cache->head.end = 0;
	cache->head.debug = 0;
	cache->head.left = 0;
	cache->head.right = &(cache->nil);
	cache->start = 0;
	cache->debugLen = 0;
	cache->firstDebug = 0;
	cache->lastDebug = 0;

	/* Allocate the initial cache page */
	AllocCachePage(cache);
	if(cache->outOfMemory)
	{
		_ILCacheDestroy(cache);
		return 0;
	}

	/* Ready to go */
	return cache;
}

void _ILCacheDestroy(ILCache *cache)
{
	unsigned long page;

	/* Free all of the cache pages */
	for(page = 0; page < cache->numPages; ++page)
	{
		ILPageFree(cache->pages[page], cache->pageSize);
	}
	if(cache->pages)
	{
		ILFree(cache->pages);
	}

	/* Free the cache object itself */
	ILFree(cache);
}

int _ILCacheIsFull(ILCache *cache, ILCachePosn *posn)
{
	return (cache->outOfMemory || (posn && posn->ptr >= posn->limit));
}

void *_ILCacheStartMethod(ILCache *cache, ILCachePosn *posn,
					      int align, void *method)
{
	ILNativeUInt temp;

	/* Do we need to allocate a new cache page? */
	if(cache->needRestart)
	{
		cache->needRestart = 0;
		AllocCachePage(cache);
	}

	/* Bail out if the cache is already full */
	if(cache->outOfMemory)
	{
		return 0;
	}

	/* Set up the initial cache position */
	posn->cache = cache;
	posn->ptr = cache->freeStart;
	posn->limit = cache->freeEnd;

	/* Align the method start */
	if(align <= 1)
	{
		align = 1;
	}
	temp = (((ILNativeUInt)(posn->ptr)) + ((ILNativeUInt)align) - 1) &
		   ~(((ILNativeUInt)align) - 1);
	if(((unsigned char *)temp) >= posn->limit)
	{
		/* There is insufficient space in this page, so create a new one */
		AllocCachePage(cache);
		if(cache->outOfMemory)
		{
			return 0;
		}

		/* Set up the cache position again and align it */
		posn->ptr = cache->freeStart;
		posn->limit = cache->freeEnd;
		temp = (((ILNativeUInt)(posn->ptr)) + ((ILNativeUInt)align) - 1) &
			   ~(((ILNativeUInt)align) - 1);
	}
	posn->ptr = (unsigned char *)temp;

	/* Allocate memory for the method information block */
	cache->method = (ILCacheMethod *)_ILCacheAlloc(posn, sizeof(ILCacheMethod));
	if(cache->method)
	{
		cache->method->method = method;
		cache->method->cookie = 0;
		cache->method->start = posn->ptr;
		cache->method->end = posn->ptr;
		cache->method->debug = 0;
		cache->method->left = 0;
		cache->method->right = 0;
	}
	cache->start = posn->ptr;

	/* Clear the debug data */
	cache->debugLen = 0;
	cache->firstDebug = 0;
	cache->lastDebug = 0;

	/* Return the method entry point to the caller */
	return (void *)(posn->ptr);
}

int _ILCacheEndMethod(ILCachePosn *posn)
{
	ILCache *cache = posn->cache;
	ILCacheMethod *method;
	ILCacheMethod *next;

	/* Determine if we ran out of space while writing the method */
	if(posn->ptr >= posn->limit)
	{
		/* Determine if the method was too big, or we need a restart.
		   The method is judged to be too big if we had a new page and
		   yet it was insufficent to hold the method */
		if(cache->freeStart ==
				((unsigned char *)(cache->pages[cache->numPages - 1])) &&
		   cache->freeEnd == (cache->freeStart + cache->pageSize))
		{
			return IL_CACHE_END_TOO_BIG;
		}
		else
		{
			cache->needRestart = 1;
			return IL_CACHE_END_RESTART;
		}
	}

	/* Terminate the debug information and flush it */
	if(cache->firstDebug || cache->debugLen)
	{
		WriteCacheDebug(posn, -1, -1);
		if(cache->debugLen)
		{
			FlushCacheDebug(posn);
		}
	}

	/* Flush the position information back to the cache */
	cache->freeStart = posn->ptr;
	cache->freeEnd = posn->limit;

	/* Update the last method region block and then
	   add all method regions to the lookup tree */
	method = cache->method;
	if(method)
	{
		method->end = posn->ptr;
		do
		{
			method->debug = cache->firstDebug;
			next = method->right;
			AddToLookupTree(cache, method);
			method = next;
		}
		while(method != 0);
		cache->method = 0;
	}

	/* The method is ready to go */
	return IL_CACHE_END_OK;
}

void *_ILCacheAlloc(ILCachePosn *posn, unsigned long size)
{
	unsigned char *ptr;

	/* Bail out if the request is too big to ever be satisfiable */
	if(size > (unsigned long)(posn->limit - posn->ptr))
	{
		posn->ptr = posn->limit;
		return 0;
	}

	/* Allocate memory from the top of the free region, so that it
	   does not overlap with the method code being written at the
	   bottom of the free region */
	ptr = (unsigned char *)(((ILNativeUInt)(posn->limit - size)) &
		                    ~(((ILNativeUInt)IL_BEST_ALIGNMENT) - 1));
	if(ptr < posn->ptr)
	{
		/* When we aligned the block, it caused an overflow */
		posn->ptr = posn->limit;
		return 0;
	}

	/* Allocate the block and return it */
	posn->limit = ptr;
	return (void *)ptr;
}

void *_ILCacheAllocNoMethod(ILCache *cache, unsigned long size)
{
	unsigned char *ptr;

	/* Bail out if the request is too big to ever be satisfiable */
	if(size > (unsigned long)(cache->freeEnd - cache->freeStart))
	{
		return 0;
	}

	/* Allocate memory from the top of the free region, so that it
	   does not overlap with the method code being written at the
	   bottom of the free region */
	ptr = (unsigned char *)(((ILNativeUInt)(cache->freeEnd - size)) &
		                    ~(((ILNativeUInt)IL_BEST_ALIGNMENT) - 1));
	if(ptr < cache->freeStart)
	{
		/* When we aligned the block, it caused an overflow */
		return 0;
	}

	/* Allocate the block and return it */
	cache->freeEnd = ptr;
	return (void *)ptr;
}

void _ILCacheAlignMethod(ILCachePosn *posn, int align, int diff, int nop)
{
	ILNativeUInt current;
	ILNativeUInt next;

	/* Determine the location of the next alignment boundary */
	if(align <= 1)
	{
		align = 1;
	}
	current = (ILNativeUInt)(posn->ptr);
	next = (current + ((ILNativeUInt)align) - 1) &
		   ~(((ILNativeUInt)align) - 1);
	if(current == next || (next - current) >= (ILNativeUInt)diff)
	{
		return;
	}

	/* Detect overflow of the free memory region */
	if(next > ((ILNativeUInt)(posn->limit)))
	{
		posn->ptr = posn->limit;
		return;
	}

	/* Fill from "current" to "next" with nop bytes */
	while(current < next)
	{
		*((posn->ptr)++) = (unsigned char)nop;
		++current;
	}
}

void _ILCacheMarkBytecode(ILCachePosn *posn, ILUInt32 offset)
{
	WriteCacheDebug(posn, (ILInt32)offset,
				    (ILInt32)(posn->ptr - posn->cache->start));
}

void _ILCacheNewRegion(ILCachePosn *posn, void *cookie)
{
	ILCacheMethod *method;
	ILCacheMethod *newMethod;

	/* Fetch the current method information block */
	method = posn->cache->method;
	if(!method)
	{
		return;
	}

	/* If the current region starts here, then simply update it */
	if(method->start == posn->ptr)
	{
		method->cookie = cookie;
		return;
	}

	/* Close off the current method region */
	method->end = posn->ptr;

	/* Allocate a new method region block and initialise it */
	newMethod = (ILCacheMethod *)_ILCacheAlloc(posn, sizeof(ILCacheMethod));
	if(!newMethod)
	{
		return;
	}
	newMethod->method = method->method;
	newMethod->cookie = cookie;
	newMethod->start = posn->ptr;
	newMethod->end = posn->ptr;

	/* Attach the new region to the cache */
	newMethod->left = 0;
	newMethod->right = method;
	posn->cache->method = newMethod;
}

void _ILCacheSetCookie(ILCachePosn *posn, void *cookie)
{
	if(posn->cache->method)
	{
		posn->cache->method->cookie = cookie;
	}
}

void *_ILCacheGetMethod(ILCache *cache, void *pc, void **cookie)
{
	ILCacheMethod *node = cache->head.right;
	while(node != &(cache->nil))
	{
		if(((unsigned char *)pc) < node->start)
		{
			node = GetLeft(node);
		}
		else if(((unsigned char *)pc) >= node->end)
		{
			node = GetRight(node);
		}
		else
		{
			if(cookie)
			{
				*cookie = node->cookie;
			}
			return node->method;
		}
	}
	return 0;
}

/*
 * Count the number of methods in a sub-tree.
 */
static unsigned long CountMethods(ILCacheMethod *node,
								  ILCacheMethod *nil,
								  void **prev)
{
	unsigned long num;

	/* Bail out if we've reached a leaf */
	if(node == nil)
	{
		return 0;
	}

	/* Count the number of methods in the left sub-tree */
	num = CountMethods(GetLeft(node), nil, prev);

	/* Process the current node */
	if(node->method != 0 && node->method != *prev)
	{
		++num;
		*prev = node->method;
	}

	/* Count the number of methods in the right sub-tree */
	return num + CountMethods(GetRight(node), nil, prev);
}

/*
 * Fill a list with methods.
 */
static unsigned long FillMethodList(void **list,
									ILCacheMethod *node,
								    ILCacheMethod *nil,
								    void **prev)
{
	unsigned long num;

	/* Bail out if we've reached a leaf */
	if(node == nil)
	{
		return 0;
	}

	/* Process the methods in the left sub-tree */
	num = FillMethodList(list, GetLeft(node), nil, prev);

	/* Process the current node */
	if(node->method != 0 && node->method != *prev)
	{
		list[num] = node->method;
		++num;
		*prev = node->method;
	}

	/* Process the methods in the right sub-tree */
	return num + FillMethodList(list + num, GetRight(node), nil, prev);
}

void **_ILCacheGetMethodList(ILCache *cache)
{
	void *prev;
	unsigned long num;
	void **list;

	/* Count the number of distinct methods in the tree */
	prev = 0;
	num = CountMethods(cache->head.right, &(cache->nil), &prev);

	/* Allocate a list to hold all of the method descriptors */
	list = (void **)ILMalloc((num + 1) * sizeof(void *));
	if(!list)
	{
		return 0;
	}

	/* Fill the list with methods and then return it */
	prev = 0;
	FillMethodList(list, cache->head.right, &(cache->nil), &prev);
	list[num] = 0;
	return list;
}

/*
 * Temporary structure for iterating over a method's debug list.
 */
typedef struct
{
	ILCacheDebug   *list;
	ILMetaDataRead	reader;

} ILCacheDebugIter;

/*
 * Initialize a debug information list iterator for a method.
 */
static void InitDebugIter(ILCacheDebugIter *iter, ILCache *cache, void *start)
{
	ILCacheMethod *node = cache->head.right;
	while(node != &(cache->nil))
	{
		if(((unsigned char *)start) < node->start)
		{
			node = GetLeft(node);
		}
		else if(((unsigned char *)start) >= node->end)
		{
			node = GetRight(node);
		}
		else
		{
			iter->list = node->debug;
			if(iter->list)
			{
				iter->reader.data = (unsigned char *)(iter->list + 1);
				iter->reader.len = IL_CACHE_DEBUG_SIZE;
				iter->reader.error = 0;
			}
			return;
		}
	}
	iter->list = 0;
}

/*
 * Get the next debug offset pair from a debug information list.
 * Returns non-zero if OK, or zero at the end of the list.
 */
static int GetNextDebug(ILCacheDebugIter *iter, ILUInt32 *offset,
						ILUInt32 *nativeOffset)
{
	while(iter->list)
	{
		ILInt32 value;

		value = ILMetaUncompressInt(&(iter->reader));
		if(value == -1)
		{
			return 0;
		}
		else if(value != -2)
		{
			*offset = (ILUInt32)value;
			*nativeOffset = (ILUInt32)(ILMetaUncompressInt(&(iter->reader)));
			return 1;
		}
		iter->list = iter->list->next;
		if(iter->list)
		{
			iter->reader.data = (unsigned char *)(iter->list + 1);
			iter->reader.len = IL_CACHE_DEBUG_SIZE;
			iter->reader.error = 0;
		}
	}
	return 0;
}

ILUInt32 _ILCacheGetNative(ILCache *cache, void *start,
						   ILUInt32 offset, int exact)
{
	ILCacheDebugIter iter;
	ILUInt32 ofs, nativeOfs;
	ILUInt32 prevNativeOfs = IL_MAX_UINT32;

	/* Search for the bytecode offset */
	InitDebugIter(&iter, cache, start);
	while(GetNextDebug(&iter, &ofs, &nativeOfs))
	{
		if(exact)
		{
			if(ofs == offset)
			{
				return nativeOfs;
			}
		}
		else if(ofs > offset)
		{
			return prevNativeOfs;
		}
		prevNativeOfs = nativeOfs;
	}
	return IL_MAX_UINT32;
}

ILUInt32 _ILCacheGetBytecode(ILCache *cache, void *start,
							 ILUInt32 offset, int exact)
{
	ILCacheDebugIter iter;
	ILUInt32 ofs, nativeOfs;
	ILUInt32 prevOfs = IL_MAX_UINT32;

	/* Search for the native offset */
	InitDebugIter(&iter, cache, start);
	while(GetNextDebug(&iter, &ofs, &nativeOfs))
	{
		if (nativeOfs == offset)
		{
			return ofs;
		}
		else if((!exact) && (nativeOfs > offset))
		{
			if(prevOfs == IL_MAX_UINT32)
			{
				/* this is the first IL offset */
				return ofs;
			}
			else
			{
				/* previous one matches better */
				return prevOfs;
			}
		}
		prevOfs = ofs;
	}
	return IL_MAX_UINT32;
}

unsigned long _ILCacheGetSize(ILCache *cache)
{
	return (cache->numPages * cache->pageSize) -
		   (cache->freeEnd - cache->freeStart);
}

/*

Using the cache
---------------

To output the code for a method, first call ILCacheStartMethod:

	ILCachePosn posn;
	void *start;

	start = ILCacheStartMethod(cache, &posn, METHOD_ALIGNMENT, method);

"METHOD_ALIGNMENT" is used to align the start of the method on an
appropriate boundary for the target CPU.  Use the value 1 if no
special alignment is required.  Note: this value is a hint to the
cache - it may alter the alignment value.

"method" is a value that uniquely identifies the method that is being
translated.  Usually this is the "ILMethod *" pointer.

The function initializes the "posn" structure, and returns the starting
address for the method.  If the function returns NULL, then it indicates
that the cache is full and further method translation is not possible.

To write code to the method, use the following:

	ILCacheByte(&posn, value);
	ILCacheWord16(&posn, value);
	ILCacheWord32(&posn, value);
	ILCacheWord64(&posn, value);

These macros write the value to cache and then update the current
position.  If the macros detect the end of the current cache page,
they will flag overflow, but otherwise do nothing (overflow is
flagged when posn->ptr == posn->limit).  The current position
in the method can be obtained using ILCacheGetPosn.

Some CPU optimization guides recommend that labels should be aligned.
This can be achieved using ILCacheAlignMethod.

Once the method code has been output, call ILCacheEndMethod to finalize
the process.  This function returns one of three result codes:

	IL_CACHE_END_OK       The translation process was successful.
	IL_CACHE_END_RESTART  The cache page overflowed.  It is necessary
	                      to restart the translation process from
	                      the beginning (ILCacheStartMethod).
	IL_CACHE_END_TOO_BIG  The cache page overflowed, but the method
	                      is too big to fit and a restart won't help.

The caller should repeatedly translate the method while ILCacheEndMethod
continues to return IL_CACHE_END_RESTART.  Normally there will be no
more than a single request to restart, but the caller should not rely
upon this.  The cache algorithm guarantees that the restart loop will
eventually terminate.

Cache data structure
--------------------

The cache consists of one or more "cache pages", which contain method
code and auxillary data.  The default size for a cache page is 128k
(IL_CONFIG_CACHE_PAGE_SIZE).  The size is adjusted to be a multiple
of the system page size (usually 4k), and then stored in "pageSize".

Method code is written into a cache page starting at the bottom of the
page, and growing upwards.  Auxillary data is written into a cache page
starting at the top of the page, and growing downwards.  When the two
regions meet, a new cache page is allocated and the process restarts.

No method, plus its auxillary data, can be greater in size than one
cache page.  The default should be sufficient for normal applications,
but is easy to increase should the need arise.

Each method has one or more ILCacheMethod auxillary data blocks associated
with it.  These blocks indicate the start and end of regions within the
method.  Normally these regions correspond to exception "try" blocks, or
regular code between "try" blocks.

The ILCacheMethod blocks are organised into a red-black tree, which
is used to perform fast lookups by address (ILCacheGetMethod).  These
lookups are used when walking the stack during exceptions or security
processing.

Each method can also have offset information associated with it, to map
between native code addresses and offsets within the original bytecode.
This is typically used to support debugging.  Offset information is stored
as auxillary data, attached to the ILCacheMethod block.

Threading issues
----------------

Writing a method to the cache, querying a method by address, or querying
offset information for a method, are not thread-safe.  The caller should
arrange for a cache lock to be acquired prior to performing these
operations.

Executing methods from the cache is thread-safe, as the method code is
fixed in place once it has been written.

Note: some CPU's require that a special cache flush instruction be
performed before executing method code that has just been written.
This is especially important in SMP environments.  It is the caller's
responsibility to perform this flush operation.

We do not provide locking or CPU flush capabilities in the cache
implementation itself, because the caller may need to perform other
duties before flushing the CPU cache or releasing the lock.

The following is the recommended way to map an "ILMethod *" pointer
to a starting address for execution:

	Look in "ILMethod" to see if we already have a starting address.
		If so, then bail out.
	Acquire the cache lock.
	Check again to see if we already have a starting address, just
		in case another thread got here first.  If so, then release
		the cache lock and bail out.
	Translate the method.
	Update the "ILMethod" structure to contain the starting address.
	Force a CPU cache line flush.
	Release the cache lock.

Why aren't methods flushed when the cache fills up?
---------------------------------------------------

In this cache implementation, methods are never "flushed" when the
cache becomes full.  Instead, all translation stops.  This is not a bug.
It is a feature.

In a multi-threaded environment, it is impossible to know if some
other thread is executing the code of a method that may be a candidate
for flushing.  Impossible that is unless one introduces a huge number
of read-write locks, one per method, to prevent a method from being
flushed.  The read locks must be acquired on entry to a method, and
released on exit.  The write locks are acquired prior to translation.

The overhead of introducing all of these locks and the associated cache
data structures is very high.  The only safe thing to do is to assume
that once a method has been translated, its code must be fixed in place
for all time.

We've looked at the code for other Free Software and Open Source JIT's,
and they all use a constantly-growing method cache.  No one has found
a solution to this problem, it seems.  Suggestions are welcome.

To prevent the cache from chewing up all of system memory, it is possible
to set a limit on how far it will grow.  Once the limit is reached, out
of memory will be reported and there is no way to recover.

*/

#ifdef	__cplusplus
};
#endif
