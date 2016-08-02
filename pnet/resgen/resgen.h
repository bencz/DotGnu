/*
 * resgen.h - Internal API's used by "resgen".
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

#ifndef	_RESGEN_RESGEN_H
#define	_RESGEN_RESGEN_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Global hash table, that holds all of the input strings.
 */
typedef struct _tagILResHashEntry ILResHashEntry;
struct _tagILResHashEntry
{
	ILResHashEntry  *next;
	int				 nameLen;
	int				 valueLen;
	const char	    *filename;
	long			 linenum;
	long			 offset;
	long			 position;
	char			 data[1];

};
#define	IL_RES_HASH_TABLE_SIZE		4096
extern ILResHashEntry *ILResHashTable[IL_RES_HASH_TABLE_SIZE];
extern unsigned long ILResNumStrings;

/*
 * Add a resource to the global hash table.
 */
int ILResAddResource(const char *filename, long linenum,
					 const char *name, int nameLen,
					 const char *value, int valueLen);

/*
 * Create a sorted array of all strings in the global hash table.
 * Returns NULL if the hash table is empty.
 */
ILResHashEntry **ILResCreateSortedArray(void);

/*
 * Load the resources in various formats.  Returns non-zero on error.
 */
int ILResLoadText(const char *filename, FILE *stream, int latin1);
int ILResLoadPO(const char *filename, FILE *stream, int latin1);
int ILResLoadBinary(const char *filename, FILE *stream);
int ILResLoadBinaryIL(const char *filename, unsigned char *address,
					  unsigned long size);
int ILResLoadXML(const char *filename, FILE *stream);

/*
 * Write the resources in various formats.
 */
void ILResWriteText(FILE *stream, int latin1);
void ILResWriteSortedText(FILE *stream, int latin1);
void ILResWritePO(FILE *stream, int latin1);
void ILResWriteSortedPO(FILE *stream, int latin1);
void ILResWriteBinary(FILE *stream);
void ILResWriteXML(FILE *stream);

/*
 * Report out of memory and abort the program.
 */
void ILResOutOfMemory(void);

/*
 * Determine if a character is white space.
 */
#define	ILResIsWhiteSpace(ch)	\
			((ch) == '\n' || (ch) == '\r' || (ch) == '\t' || \
			 (ch) == '\f' || (ch) == '\v' || (ch) == ' ' || \
			 (ch) == 0x1A)

#ifdef	__cplusplus
};
#endif

#endif /* _RESGEN_RESGEN_H */
