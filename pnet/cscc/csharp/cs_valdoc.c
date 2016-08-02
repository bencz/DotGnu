/*
 * cs_valdoc.c - Validate documentation comment blocks.
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
 */

#include "cs_internal.h"
#include "il_xml.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Control structure for validating XML from a documentation list.
 */
typedef struct
{
	ILNode_ListIter	iter;
	const char     *str;
	int			    len;
	const char	   *filename;
	long			linenum;

} CSXMLValidator;

/*
 * Read XML data from a documentation list.
 */
static int DocRead(CSXMLValidator *val, void *buffer, int len)
{
	int templen;
	ILNode *node;

again:
	if(val->len > 0)
	{
		/* Continue reading from the previous string */
		templen = val->len;
		if(len < templen)
		{
			templen = len;
		}
		ILMemCpy(buffer, val->str, templen);
		val->str += templen;
		val->len -= templen;
		return templen;
	}
	else
	{
		/* Find the next DocComment node */
		while((node = ILNode_ListIter_Next(&(val->iter))) != 0)
		{
			if(yyisa(node, ILNode_DocComment))
			{
				val->str = ((ILNode_DocComment *)node)->str;
				val->len = ((ILNode_DocComment *)node)->len;
				val->filename = yygetfilename(node);
				val->linenum = yygetlinenum(node);
				goto again;
			}
		}
		return 0;
	}
}

/*
 * Structure of a tag stack item.
 */
typedef struct _tagCSTag CSTag;
struct _tagCSTag
{
	CSTag *next;
	char   name[1];
};

/*
 * Push an item onto the tag stack.
 */
static void PushTag(CSTag **stack, const char *name)
{
	CSTag *tag = (CSTag *)ILMalloc(sizeof(CSTag) + strlen(name));
	if(!tag)
	{
		CCOutOfMemory();
	}
	tag->next = *stack;
	*stack = tag;
	strcpy(tag->name, name);
}

/*
 * Pop an item from the tag stack.
 */
static void PopTag(CSTag **stack)
{
	CSTag *tag = *stack;
	if(tag)
	{
		*stack = tag->next;
		ILFree(tag);
	}
}

void CSValidateDocs(ILNode *docList)
{
	CSXMLValidator val;
	ILXMLReader *reader;
	ILXMLItem item;
	CSTag *stack = 0;

	/* Initialize the reader control structure */
	ILNode_ListIter_Init(&(val.iter), docList);
	val.str = 0;
	val.len = 0;

	/* Create the XML reader object */
	reader = ILXMLCreate((ILXMLReadFunc)DocRead, &val, 0);
	if(!reader)
	{
		CCOutOfMemory();
	}

	/* Validate the XML text */
	while((item = ILXMLReadNext(reader)) != ILXMLItem_EOF)
	{
		if(item == ILXMLItem_StartTag)
		{
			/* Push a new item onto the tag stack */
			PushTag(&stack, ILXMLTagName(reader));
		}
		else if(item == ILXMLItem_EndTag)
		{
			/* Verify the top-most item on the tag stack */
			if(!stack || strcmp(stack->name, ILXMLTagName(reader)) != 0)
			{
				if(stack)
				{
					CCErrorOnLine(val.filename, val.linenum,
								  "unbalanced `<%s>' in documentation comment",
								  stack->name);
				}
				else
				{
					CCErrorOnLine(val.filename, val.linenum,
								  "unbalanced `</%s>' in documentation comment",
								  ILXMLTagName(reader));
				}
				goto bailout;
			}
			PopTag(&stack);
		}
	}

	/* Report an error if the tag stack is not empty */
	if(stack != 0)
	{
		CCErrorOnLine(val.filename, val.linenum,
					  "unbalanced `<%s>' in documentation comment",
					  stack->name);
	}

	/* Clean up and exit */
bailout:
	while(stack != 0)
	{
		PopTag(&stack);
	}
	ILXMLDestroy(reader);
}

#ifdef	__cplusplus
};
#endif
