/*
 * il_align.h - Determine the best alignment on the target platform.
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

#ifndef	_IL_ALIGN_H
#define	_IL_ALIGN_H

#include "il_values.h"
#ifdef HAVE_STDDEF_H
#include <stddef.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * The following is some macro magic that attempts to detect
 * the best alignment to use on the target platform.  The final
 * value, "IL_BEST_ALIGNMENT", will be a compile-time constant if
 * the offsetof macro is available.
 */

#define	_IL_ALIGN_CHECK_TYPE(type,name)	\
	struct _IL_align_##name { \
		char pad; \
		type field; \
	}

#ifdef offsetof
#define _IL_ALIGN_FOR_TYPE(name)	\
	(offsetof(struct _IL_align_##name, field))
#else
#define	_IL_ALIGN_FOR_TYPE(name)	\
	((ILNativeUInt)(&(((struct _IL_align_##name *)0)->field)))
#endif

#define	_IL_ALIGN_MAX(a,b)	\
	((a) > (b) ? (a) : (b))

#define	_IL_ALIGN_MAX3(a,b,c) \
	(_IL_ALIGN_MAX((a), _IL_ALIGN_MAX((b), (c))))

_IL_ALIGN_CHECK_TYPE(ILInt32, int);
_IL_ALIGN_CHECK_TYPE(ILInt64, long);
_IL_ALIGN_CHECK_TYPE(void *, void_p);
_IL_ALIGN_CHECK_TYPE(ILFloat, float);
_IL_ALIGN_CHECK_TYPE(ILDouble, double);
_IL_ALIGN_CHECK_TYPE(ILNativeFloat, long_double);

#define	IL_BEST_ALIGNMENT	\
	_IL_ALIGN_MAX(_IL_ALIGN_MAX3(_IL_ALIGN_FOR_TYPE(int), \
						 		 _IL_ALIGN_FOR_TYPE(long), \
						 		 _IL_ALIGN_FOR_TYPE(void_p)), \
			  	  _IL_ALIGN_MAX3(_IL_ALIGN_FOR_TYPE(float), \
			  			 		 _IL_ALIGN_FOR_TYPE(double), \
			  			 		 _IL_ALIGN_FOR_TYPE(long_double)))

#ifdef __cris__
#undef IL_BEST_ALIGNMENT
#define IL_BEST_ALIGNMENT 4
#endif

#ifdef	__cplusplus
};
#endif

#endif	/* _IL_ALIGN_H */
