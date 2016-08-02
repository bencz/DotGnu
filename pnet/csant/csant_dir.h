/*
 * csant_dir.h - Directory walking and name manipulation tools.
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

#ifndef	_CSANT_DIR_H
#define	_CSANT_DIR_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Opaque directory walking structure.
 */
typedef struct _tagCSAntDir CSAntDir;

/*
 * Open a directory walker.  Returns NULL on error.
 */
CSAntDir *CSAntDirOpen(const char *pathname, const char *regex);

/*
 * Close a directory walker.
 */
void CSAntDirClose(CSAntDir *dir);

/*
 * Get the next filename from within a directory walker.
 * Returns NULL at the end of the directory.
 */
const char *CSAntDirNext(CSAntDir *dir);

/*
 * Combine a pathname and filename into an ILMalloc'ed full name.
 * This will also normalize '/' and '\\' characters appropriately.
 */
char *CSAntDirCombine(const char *pathname, const char *filename);

/*
 * Combine a pathname, filename, and extension into an ILMalloc'ed
 * full name that uses Win32 pathname conventions.
 */
char *CSAntDirCombineWin32(const char *pathname, const char *filename,
						   const char *extension);

#ifdef	__cplusplus
};
#endif

#endif	/* _CSANT_DIR_H */
