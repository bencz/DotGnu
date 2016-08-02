/*
 * cg_genattrs.h - handle custom attributes.
 *
 * Copyright (C) 2009  Free Software Foundation, Inc.
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

#ifndef	__CODEGEN_CG_ERROR_H__
#define	__CODEGEN_CG_ERROR_H__

#include "il_varargs.h"
#include "cg_nodes.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Print an error relative to the given node.
 * Node must not be 0.
 */
void CGErrorForNode(ILGenInfo *info, ILNode *node, const char *format, ...)
					IL_PRINTF(3, 4);

/*
 * Print an error relative to the given location.
 */
void CGErrorOnLine(ILGenInfo *info, const char *filename,
				   unsigned long linenum, const char *format, ...)
				   IL_PRINTF(4, 5);

/*
 * Print a warning relative to the given node.
 * Node must not be 0.
 */
void CGWarningForNode(ILGenInfo *info, ILNode *node, const char *format, ...)
					  IL_PRINTF(3, 4);

/*
 * Print a warning relative to the given location.
 */
void CGWarningOnLine(ILGenInfo *info, const char *filename,
					 unsigned long linenum, const char *format, ...)
 					 IL_PRINTF(4, 5);

#ifdef	__cplusplus
};
#endif

#endif	/* __CODEGEN_CG_ERROR_H__ */
