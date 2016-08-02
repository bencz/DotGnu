/*
 * csant_fileset.h - File set management routines.
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

#ifndef	_CSANT_FILESET_H
#define	_CSANT_FILESET_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Opaque type definition for file sets.
 */
typedef struct _tagCSAntFileSet CSAntFileSet;

/*
 * Load a file set into memory, based on a task sub-node.
 * Returns NULL if there is no such sub-node.
 */
CSAntFileSet *CSAntFileSetLoad(CSAntTask *task, const char *name,
							   const char *configBaseDir);

/*
 * Destroy a file set that we no longer require.
 */
void CSAntFileSetDestroy(CSAntFileSet *fileset);

/*
 * Get the number of files in a file set.
 */
unsigned long CSAntFileSetSize(CSAntFileSet *fileset);

/*
 * Get a specific file in a file set.
 */
char *CSAntFileSetFile(CSAntFileSet *fileset, unsigned long num);

/*
 * Determine if any of the files in a file set are newer
 * than a specific file.
 */
int CSAntFileSetNewer(CSAntFileSet *fileset, const char *filename);

/*
 * Add a filanem to a file set.
 */
CSAntFileSet *CSAntFileSetAdd(CSAntFileSet *fileset, const char *filename);

#ifdef	__cplusplus
};
#endif

#endif	/* _CSANT_FILESET_H */
