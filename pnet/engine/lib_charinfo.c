/*
 * lib_charinfo.c - Internalcall methods for the "Platform.SysCharInfo" class.
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

#include "engine.h"
#include "lib_defs.h"
#include "il_utils.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * public static UnicodeCategory GetUnicodeCategory(char ch);
 */
ILInt32 _IL_SysCharInfo_GetUnicodeCategory(ILExecThread *thread, ILUInt16 ch)
{
	return (ILInt32)(ILGetUnicodeCategory((unsigned)ch));
}

/*
 * public static double GetNumericValue(char ch);
 */
ILDouble _IL_SysCharInfo_GetNumericValue(ILExecThread *thread, ILUInt16 ch)
{
	return (ILDouble)(ILGetUnicodeValue((unsigned)ch));
}

/*
 * public static String GetNewLine();
 */
ILString *_IL_SysCharInfo_GetNewLine(ILExecThread *thread)
{
#ifdef IL_WIN32_NATIVE
	return ILStringCreate(thread, "\r\n");
#else
	return ILStringCreate(thread, "\n");
#endif
}

#ifdef	__cplusplus
};
#endif
