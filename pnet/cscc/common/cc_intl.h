/*
 * cc_intl.h - Internationalization support for "cscc" and its plugins.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

#ifndef	_CSCC_CC_INTL_H
#define	_CSCC_CC_INTL_H

#include "il_values.h"

#ifdef	__cplusplus
extern	"C" {
#endif

#if HAVE_LIBINTL_H && HAVE_GETTEXT

#include <libintl.h>
#define	_(str)	(gettext((str)))
#ifdef	gettext_noop
	#define	N_(str)	(gettext_noop((str)))
#else
	#define	N_(str)	(str)
#endif

#else

#define	_(str)	(str)
#define	N_(str)	(str)
#define	textdomain(domain)
#define	bindtextdomain(package,directory)

#endif

#ifdef	__cplusplus
};
#endif

#endif	/* _CSCC_CC_INTL_H */
