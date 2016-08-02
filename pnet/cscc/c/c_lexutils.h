/*
 * c_lexutils.h - Utility functions for assisting the C lexer.
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

#ifndef	__CSCC_C_LEXUTILS_H__
#define	__CSCC_C_LEXUTILS_H__

#include "il_values.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Semantic information for integer constants.
 */
typedef struct _tagCLexIntConst
{
	ILUInt64		value;
	int				isneg;
	ILMachineType	type;

} CLexIntConst;

/*
 * Semantic information for floating point constants.
 */
typedef struct _tagCLexFloatConst
{
	ILDouble		value;
	ILMachineType	type;

} CLexFloatConst;

/*
 * Process a "#line" or "#" directive in the input.
 */
void CLexLineDirective(const char *text);

/*
 * Process a "#using" directive in the input.
 */
void CLexUsingDirective(const char *text);

/*
 * Parse an integer constant and determine its most natural
 * value type.  This can handle decimal, octal, and hexadecimal
 * constants, with or without type suffixes.
 */
void CLexParseInt(const char *text, CLexIntConst *value);

/*
 * Parse a floating point value and determine its most natural
 * value type.
 */
void CLexParseFloat(const char *text, CLexFloatConst *value);

/*
 * Parse a character constant.  This will result
 * in signed 8-bit values.
 */
void CLexParseChar(const char *text, CLexIntConst *value);

/*
 * Parse a string constant.  If a "\uHHHH" sequence is
 * encountered, it will be converted into the equivalent
 * UTF-8 character encoding.  Note: "text" will be modified.
 */
void CLexParseString(char *text, ILIntString *value);

#ifdef	__cplusplus
};
#endif

#endif	/* __CSCC_C_LEXUTILS_H__ */
