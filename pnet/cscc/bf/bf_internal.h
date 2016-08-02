/*
 * bf_internal.h - common include files for BF operations 
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

#ifndef	_CSCC_BF_INTERNAL_H
#define	_CSCC_BF_INTERNAL_H
#include <cscc/common/cc_main.h>
#include <cscc/bf/bf_defs.h>

ILNode * BFOptimize(ILGenInfo *info, ILNode *tree);

#endif	/* _CSCC_BF_INTERNAL_H */
