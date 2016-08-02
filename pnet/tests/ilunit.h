/*
 * ilunit.h - XUnit-style testing framework for C-based API's.
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

#ifndef	_ILUNIT_H
#define	_ILUNIT_H

#include <stdio.h>
#include "il_system.h"
#include "il_utils.h"
#include "il_program.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Prototype for a test function.  If this function returns
 * without calling a report function, the test is assumed
 * to have succeeded.
 */
typedef void (*ILUnitTestFunc)(void *arg);

/*
 * Register the beginning of a test suite.  All calls to
 * ILUnitRegister after this will be registered under the
 * suite name.
 */
void ILUnitRegisterSuite(const char *name);

/*
 * Register a test.
 */
void ILUnitRegister(const char *name, ILUnitTestFunc func, void *arg);

/*
 * User-supplied function that registers all tests.
 */
void ILUnitRegisterTests(void);

/*
 * User-supplied function that performs some cleanup after running the
 * requested tests.
 */
void ILUnitCleanupTests(void);

/*
 * Report a test failure with the simple "failed" message
 * and then abort the test.
 */
void ILUnitFail(void);

/*
 * Report a test failure and then abort the test.
 * This function automatically appends a '\n'
 * to the failure message.
 */
void ILUnitFailed(const char *msg, ...);

/*
 * Report a test failure, but continue with the test.
 * This allows extra information to be provided on the
 * next line of output.  This function can be used to
 * output the extra information.  It automatically
 * appends a '\n' to the output.
 */
void ILUnitFailMessage(const char *msg, ...);

/*
 * Report that the test has finished outputting extra
 * messages and the test should now abort.
 */
void ILUnitFailEndMessages(void);

/*
 * Assert a condition and abort the test if it is false.
 */
void _ILUnitAssert(const char *condition, const char *filename, int linenum);
#define	ILUnitAssert(x)	\
			do { \
				if(!(x)) \
				{ \
					_ILUnitAssert(#x, __FILE__, __LINE__); \
				} \
			} while (0)

/*
 * Report out of memory and abort the program.
 */
void ILUnitOutOfMemory(void);

/*
 * Start constructing an assembly stream, to be fed into "ilasm".
 */
void ILUnitAsmCreate(void);

/*
 * Close the assembly stream, assembly it, and then load the
 * result as a binary image.
 */
void ILUnitAsmClose(void);

/*
 * Output a method header to the assembly stream.
 */
void ILUnitAsmMethod(const char *signature, int maxStack);

/*
 * End the output of the current method in the assembly stream.
 */
void ILUnitAsmEndMethod(void);

/*
 * Write data to the assembly stream.
 */
void ILUnitAsmWrite(const char *str);
void ILUnitAsmPrintF(const char *format, ...);

/*
 * Get the file descriptor for the assembly stream.
 */
FILE *ILUnitAsmStream(void);

/*
 * Get the image that was loaded after processing the assembly stream.
 */
ILImage *ILUnitAsmImage(void);

#ifdef	__cplusplus
};
#endif

#endif	/* _ILUNIT_H */
