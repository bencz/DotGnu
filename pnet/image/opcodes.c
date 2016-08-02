/*
 * opcodes.c - Opcode information tables.
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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

#include "il_opcodes.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#define	OPDEF(name,popped,pushed,args,size)	\
			{name, popped, pushed, args, size},

ILOpcodeInfo const ILMainOpcodeTable[256] = {
#define IL_MAIN_OPDEF
#include "opdef.c"
#undef IL_MAIN_OPDEF
};

ILOpcodeInfo const ILPrefixOpcodeTable[256] = {
#define IL_PREFIX_OPDEF
#include "opdef.c"
#undef IL_PREFIX_OPDEF
};

#ifdef	__cplusplus
};
#endif
