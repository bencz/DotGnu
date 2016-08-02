/*
 * doc_backend.h - API for documentation conversion back-ends.
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

#ifndef	_CSDOC_DOC_BACKEND_H
#define	_CSDOC_DOC_BACKEND_H

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Header to display in program usage messages.
 */
extern char const ILDocProgramHeader[];

/*
 * Name of the program to display in warranty messages.
 */
extern char const ILDocProgramName[];

/*
 * List of "-f" options for printing help.  These aren't
 * used by the parsing routines.
 */
extern ILCmdLineOption const ILDocProgramOptions[];

/*
 * Get the default output pathname, if one was not supplied
 * on the command-line.  Returns NULL if no default possible.
 * It is assumed that the backend will print an error message
 * describing why a default cannot be chosen.
 */
char *ILDocDefaultOutput(int numInputs, char **inputs, const char *progname);

/*
 * Validate the output pathname.  Returns non-zero if OK.
 * It is assumed that the backend will print an error message
 * describing why the pathname is invalid.
 */
int ILDocValidateOutput(char *outputPath, const char *progname);

/*
 * Convert a documentation tree into the backend's format.
 * Returns zero if an error occurred, which the backend
 * should report to stderr prior to returning.
 */
int ILDocConvert(ILDocTree *tree, int numInputs, char **inputs,
				 char *outputPath, const char *progname);

/*
 * Report out of memory and abort the program.  This is not
 * supplied by the backend, but may be useful for the backend.
 */
void ILDocOutOfMemory(const char *progname);

/*
 * Determine if a particular command-line flag is set.  This is
 * not supplied by the backend, but may be useful for the backend.
 */
int ILDocFlagSet(const char *flag);

/*
 * Get the value of a command-line flag if it is set.  This is
 * used for flags of the form "-fNAME=VALUE".  Returns NULL if
 * the flag is not set, or an empty string if the flag is present
 * but it does not have a value.
 */
const char *ILDocFlagValue(const char *flag);

/*
 * Get the n'th value of a command-line flag with multiple values.
 * Returns NULL if n is invalid.
 */
const char *ILDocFlagValueN(const char *flag, int n);

#ifdef	__cplusplus
};
#endif

#endif	/* _CSDOC_DOC_BACKEND_H */
