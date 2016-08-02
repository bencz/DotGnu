/*
 * cg_varusage.h - Variable tracking functions.
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

#ifndef	_CODEGEN_CG_VARUSAGE_H
#define	_CODEGEN_CG_VARUSAGE_H

#ifdef	__cplusplus
extern	"C" {
#endif

#define IL_VAR_BLOCK_SIZE 32
		
struct _tagILVarUsageTable
{
	int modified__;
	char vars__[IL_VAR_BLOCK_SIZE]; /* 8 ints */
	struct _tagILVarUsageTable *next__;
};

#define IL_VAR_STATUS_UNDEF 		0
#define IL_VAR_STATUS_DECLARED 		1 
#define IL_VAR_STATUS_USED			(1 << 1)
#define IL_VAR_STATUS_MODIFIED		(1 << 2)

typedef struct _tagILVarUsageTable ILVarUsageTable;

extern ILVarUsageTable ILVarUsageTableDefault;

/* create a table cloned from the previous table , a NULL parent
 * creates a clear table */
ILVarUsageTable* ILVarUsageTableCreate(ILGenInfo *info,
										ILVarUsageTable *previous);

/* recursively free all entries in the table */
void ILVarUsageTableDestroy(ILGenInfo *info, ILVarUsageTable* table);

/* merging two branches to the same location */
ILVarUsageTable* ILVarUsageTableMerge(ILGenInfo *info,
					ILVarUsageTable *table1,
					ILVarUsageTable *table2);

/* update variable status to the value */
void ILVarUsageSetVariable(ILGenInfo *info,
					ILVarUsageTable *table,
					int localVar,
					int status);

int ILVarUsageGetVariable(ILGenInfo *info,
					ILVarUsageTable *table,
					int localVar);

/* clear variable when it goes out of scope */
void ILVarUsageClearVariable(ILGenInfo *info,
					ILVarUsageTable *table,
					int localVar);
#ifdef	__cplusplus
};
#endif

#endif	/* _CODEGEN_CG_VARUSAGE_H */
