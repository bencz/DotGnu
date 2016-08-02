/*
 * cg_defs.h - Common definitions for the code generator.
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

#ifndef	_CODEGEN_CG_DEFS_H
#define	_CODEGEN_CG_DEFS_H

#include "il_values.h"
#include "il_utils.h"
#include "il_decimal.h"
#include "il_program.h"
#include "il_opcodes.h"
#include "il_system.h"
#include "il_dumpasm.h"
#include <stdio.h>

#ifdef	__cplusplus
extern	"C" {
#endif

typedef struct _tagILGenInfo  		ILGenInfo;
typedef struct _tagILScope  		ILScope;
typedef struct _tagILScopeData		ILScopeData;
typedef struct _tagILSwitchValue  	ILSwitchValue;
typedef unsigned long		  		ILLabel;
#define	ILLabel_Undefined			((ILLabel)0)

#ifdef	__cplusplus
};
#endif

#endif	/* _CODEGEN_CG_DEFS_H */
