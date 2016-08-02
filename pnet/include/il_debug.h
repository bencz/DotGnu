/*
 * il_debug.h - Routines for manipulating debug symbol information.
 *
 * Copyright (C) 2001, 2009  Southern Storm Software, Pty Ltd.
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

#ifndef	_IL_DEBUG_H
#define	_IL_DEBUG_H

#include "il_image.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Debug information types.
 */
#define	IL_DEBUGTYPE_LINE_COL			1	/* Line and column numbers */
#define	IL_DEBUGTYPE_LINE_OFFSETS		2	/* Line numbers and offsets */
#define	IL_DEBUGTYPE_LINE_COL_OFFSETS	3	/* Line, columns, and offsets */
#define	IL_DEBUGTYPE_VARS				4	/* Local variables */
#define	IL_DEBUGTYPE_VARS_OFFSETS		5	/* Local variables and offsets */

/*
 * Opaque type for a debug context.
 */
typedef struct _tagILDebugContext ILDebugContext;

/*
 * Iterator for debug information blocks.
 */
typedef struct
{
	ILDebugContext *dbg;			/* Debug context for the iterator */
	unsigned long	type;			/* Type of debug symbol information */
	const void     *data;			/* Start of debug symbol data */
	unsigned long	length;			/* Length of debug symbol data */
	unsigned long	reserved1;		/* Used internally */
	unsigned long	reserved2;		/* Used internally */

} ILDebugIter;

/*
 * Determine if an image has debug information.
 */
int ILDebugPresent(ILImage *image);

/*
 * Create a debug context for an image.  Returns NULL if out of memory.
 */
ILDebugContext *ILDebugCreate(ILImage *image);

/*
 * Destroy a debug context.
 */
void ILDebugDestroy(ILDebugContext *dbg);

/*
 * Get the image that is associated with a debug context.
 */
ILImage *ILDebugToImage(ILDebugContext *dbg);

/*
 * Convert a name into a pseudo-token for debug information.
 */
ILToken ILDebugGetPseudo(const char *name);

/*
 * Get a string from the debug string table.  Returns NULL
 * if the string table offset is invalid.
 */
const char *ILDebugGetString(ILDebugContext *dbg, ILUInt32 offset);

/*
 * Initialize a debug information block iterator for a token.
 */
void ILDebugIterInit(ILDebugIter *iter, ILDebugContext *dbg, ILToken token);

/*
 * Iterate over the debug information blocks for a token.
 * Returns zero if no more debug blocks.
 */
int ILDebugIterNext(ILDebugIter *iter);

/*
 * Get debug line information for a particular token
 * and offset.  The offset should be zero if the token
 * does not normally have offsets.  Returns the filename,
 * or NULL if no debug information was found.
 */
const char *ILDebugGetLineInfo(ILDebugContext *dbg, ILToken token,
						       ILUInt32 offset, ILUInt32 *line,
						       ILUInt32 *column);

/*
 * Get the name of a local variable at a particular offset
 * within a method.  Returns the name, or NULL if none.
 */
const char *ILDebugGetVarName(ILDebugContext *dbg, ILToken token,
							  ILUInt32 offset, ILUInt32 varNum);

#ifdef	__cplusplus
};
#endif

#endif	/* _IL_DEBUG_H */
