/*
 * bf_optimize.c - BF optimiser 
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
 * 
 * Contributed by Gopal.V
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
#include "bf_internal.h"

ILNode * BFOptimize(ILGenInfo *info, ILNode *tree)
{
	ILNode * newList;
	ILNode * list;
	ILNode * item=NULL;
	ILNode * lastItem = NULL;
	ILNode_ListIter iter;
	if(!yyisa(tree,ILNode_BFBody))
	{
		return tree;
	}
	list = ((ILNode_BFBody*)tree)->body;
	if(!yyisa(list,ILNode_List)) return tree;
	newList=ILNode_List_create();
	ILNode_ListIter_Init(&iter,list);
	while((item = ILNode_ListIter_Next(&iter)) !=NULL)
	{
		item = BFOptimize(info,item);

		if(lastItem == NULL)
		{
			lastItem = item;
		}
		else
		{
			if(yyisa(lastItem,ILNode_BFOpt) &&
				yykind(lastItem) == yykind(item))
			{
				((ILNode_BFOpt*)lastItem)->count+=
					((ILNode_BFOpt*)item)->count;
			}
			else
			{
				ILNode_List_Add(newList,lastItem);
				lastItem = item;
			}
		}
	}
	if (lastItem != NULL)
	{
		ILNode_List_Add(newList,lastItem);
	}
	((ILNode_BFBody*)tree)->body=newList;
	return tree;
}
