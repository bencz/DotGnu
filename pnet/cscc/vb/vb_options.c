/*
 * vb_options.c - Option processing for the VB compiler.
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
 */

#include <cscc/vb/vb_internal.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Option block list element.
 */
typedef struct _OptionBlock
{
	ILNode_Namespace	   *node;
	int						options;
	struct _OptionBlock	   *next;

} OptionBlock;
static OptionBlock *optionBlocks = 0;

/*
 * Find or create an option block for a specific namespace.
 */
static OptionBlock *GetOptionBlock(ILNode_Namespace *namespaceNode)
{
	OptionBlock *current = optionBlocks;
	OptionBlock *prev = 0;
	while(current != 0)
	{
		if(current->node == namespaceNode)
		{
			/* Move the block to the front of the list so that
			   it is quicker to find the next time we are called */
			if(prev)
			{
				prev->next = current->next;
				current->next = optionBlocks;
				optionBlocks = current;
			}
			return current;
		}
		prev = current;
		current = current->next;
	}
	current = (OptionBlock *)ILMalloc(sizeof(OptionBlock));
	if(!current)
	{
		CCOutOfMemory();
	}
	current->node = namespaceNode;
	current->options = 0;
	current->next = optionBlocks;
	optionBlocks = current;
	return current;
}

char *VBGetRootNamespace(void)
{
	return CCStringListGetValue(extension_flags, num_extension_flags, "root");
}

void VBAddGlobalImports(ILNode_Namespace *namespaceNode, ILScope *scope)
{
	/* TODO */
}

void VBOptionInit(ILNode_Namespace *namespaceNode)
{
	int options = 0;
	char *compare;

	/* Collect the options from the user-supplied command-line */
	if(!CCStringListContainsInv(extension_flags, num_extension_flags,
								"explicit"))
	{
		options |= VB_OPTION_EXPLICIT;
	}
	if(CCStringListContains(extension_flags, num_extension_flags, "strict"))
	{
		options |= VB_OPTION_STRICT;
	}
	compare = CCStringListGetValue(extension_flags, num_extension_flags,
								   "compare");
	if(compare)
	{
		if(!ILStrICmp(compare, "binary"))
		{
			options |= VB_OPTION_BINARY_COMPARE;
		}
	}
	else
	{
		options |= VB_OPTION_BINARY_COMPARE;
	}

	/* Set the options */
	GetOptionBlock(namespaceNode)->options = options;
}

void VBOptionSet(ILNode_Namespace *namespaceNode, int option, int value)
{
	if(value)
	{
		GetOptionBlock(namespaceNode)->options |= option;
	}
	else
	{
		GetOptionBlock(namespaceNode)->options &= ~option;
	}
}

int VBOptionIsSet(ILNode_Namespace *namespaceNode, int option)
{
	/* Find the global namespace node */
	while(namespaceNode != 0 && namespaceNode->enclosing != 0)
	{
		namespaceNode = namespaceNode->enclosing;
	}

	/* Retrieve the option block and then test the option */
	return ((GetOptionBlock(namespaceNode)->options & option) != 0);
}

#ifdef	__cplusplus
};
#endif
