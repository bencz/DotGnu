/*
 * cg_varusage.c - Variable tracking functions
 *
 * Copyright (C) 2002  Free Software Foundation, Inc.
 *
 * Contributed by Gopal. V
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

#include "cg_nodes.h"
#include "cg_varusage.h"

#ifdef	__cplusplus
extern	"C" {
#endif

ILVarUsageTable ILVarUsageTableDefault={0,{0,0,0,0,0,0,0,0},0};
		
ILVarUsageTable* ILVarUsageTableCreate(ILGenInfo *info,
										ILVarUsageTable *previous)
{
	ILVarUsageTable* retval=(ILVarUsageTable*)(ILCalloc(1,sizeof(
										ILVarUsageTable)));
	retval->modified__=0;
	if(previous)
	{
		ILMemCpy(retval->vars__, previous->vars__,sizeof(previous->vars__));
		if(previous->next__)
		{
			retval->next__=ILVarUsageTableCreate(info,previous->next__);
		}
		else
		{
			retval->next__=NULL;
		}
	}
	return retval;
}

void ILVarUsageTableDestroy(ILGenInfo *info, ILVarUsageTable* table)
{
	if(!table) return;
	if(table->next__) ILVarUsageTableDestroy(info,table->next__);
	ILFree(table);
}

/* merging two branches to the same location */
ILVarUsageTable* ILVarUsageTableMerge(ILGenInfo *info,
					ILVarUsageTable *table1,
					ILVarUsageTable *table2)
{
	int i;
	ILVarUsageTable* retval;
	if(table1==NULL)return ILVarUsageTableCreate(info,table2);
	if(table2==NULL)return ILVarUsageTableCreate(info,table1);
	retval=(ILVarUsageTable*)(ILCalloc(1,sizeof(
										ILVarUsageTable)));
	retval->modified__=0;
	for(i=0;i<IL_VAR_BLOCK_SIZE;i++)
	{
		retval->vars__[i]=(table1->vars__[i] & table2->vars__[i]);
	}
	if(table1->next__ && table2->next__)
	{
		retval->next__ = ILVarUsageTableMerge(info,table1->next__,
								table2->next__);
	}
	return retval;
}

void ILVarUsageSetVariable(ILGenInfo *info,
					ILVarUsageTable *table,
					int localVar,
					int status)
{
	int i;
	int extra_blocks= localVar / IL_VAR_BLOCK_SIZE;	
	for(i=0;i<extra_blocks;i++)
	{
		table->next__=ILVarUsageTableCreate(info,NULL);
		table=table->next__;
	}
	table->vars__[localVar % IL_VAR_BLOCK_SIZE] |= status;	
}

int ILVarUsageGetVariable(ILGenInfo *info,
					ILVarUsageTable *table,
					int localVar)
{
	int i;
	int extra_blocks= localVar / IL_VAR_BLOCK_SIZE;	
	for(i=0;i<extra_blocks;i++)
	{
		table=table->next__;
		if(table==NULL)
		{
			return 0;
		}
	}
	return table->vars__[localVar % IL_VAR_BLOCK_SIZE];	
}

void ILVarUsageClearVariable(ILGenInfo *info,
					ILVarUsageTable *table,
					int localVar)
{
	int i;
	int extra_blocks= localVar / IL_VAR_BLOCK_SIZE;	
	for(i=0;i<extra_blocks;i++)
	{
		table=table->next__;
		if(table==NULL)
		{
			return;
		}
	}
	table->vars__[localVar % IL_VAR_BLOCK_SIZE] = 0;	
}



#ifdef	__cplusplus
};
#endif
