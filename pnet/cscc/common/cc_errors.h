/*
 * cc_errors.h - Error handling routines for "cscc" plugins.
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

#ifndef	_CSCC_CC_ERRORS_H
#define	_CSCC_CC_ERRORS_H

#include "il_varargs.h"
#include <codegen/cg_nodes.h>
#include <cscc/common/cc_intl.h>

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Error and warning indicators.
 */
extern int CCHaveErrors;
extern int CCHaveWarnings;

/*
 * Report an error on the current line.
 */
void CCError(const char *format, ...) IL_PRINTF(1, 2);

/*
 * Report an error on a specific line.
 */
void CCErrorOnLine(const char *filename, unsigned long linenum,
				   const char *format, ...) IL_PRINTF(3, 4);

/*
 * Same as CCErrorOnLine except it takes an IL_VA_LIST as variable argument.
 */
void CCErrorOnLineV(const char *filename, unsigned long linenum,
					const char *format, IL_VA_LIST va);

/*
 * Report a warning on the current line.
 */
void CCWarning(const char *format, ...) IL_PRINTF(1, 2);

/*
 * Report a typed warning on the current line.  The warning
 * will only be reported if the "type" is enabled.
 */
void CCTypedWarning(const char *type, const char *format, ...) IL_PRINTF(2, 3);

/*
 * Report a warning on a specific line.
 */
void CCWarningOnLine(const char *filename, unsigned long linenum,
				     const char *format, ...) IL_PRINTF(3, 4);

/*
 * Same as CCWarningOnLine except it takes an IL_VAL_LIST as variable argument.
 */
void CCWarningOnLineV(const char *filename, unsigned long linenum,
					  const char *format, IL_VA_LIST va);

/*
 * Report a typed warning on a specific line.  The warning
 * will only be reported if the "type" is enabled.
 */
void CCTypedWarningOnLine(const char *filename, unsigned long linenum,
				     	  const char *type, const char *format, ...)
						  IL_PRINTF(4, 5);

/*
 * Report either a warning or an error about unsafe constructs.
 */
void CCUnsafeMessage(ILGenInfo *info, ILNode *node, const char *construct);

/*
 * Report either a warning or an error about unsafe types.
 */
void CCUnsafeTypeMessage(ILGenInfo *info, ILNode *node);

/*
 * Enter an unsafe context, reporting a warning or error
 * about the context as necessary.
 */
void CCUnsafeEnter(ILGenInfo *info, ILNode *node, const char *construct);

/*
 * Leave an unsafe context.
 */
void CCUnsafeLeave(ILGenInfo *info);

#ifdef	__cplusplus
};
#endif

#endif	/* _CSCC_CC_ERRORS_H */
