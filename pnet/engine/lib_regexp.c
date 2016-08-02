/*
 * lib_regexp.c - Internalcall methods for "Platform.Regexp"
 *
 * Copyright (C) 2002  Free Software Foundation, Inc.
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
 * 
 * Contributions by Gopal V, Rhys Weatherley
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
#include "il_regex.h"

#ifdef	__cplusplus
extern	"C" {
#endif

ILNativeInt _IL_RegexpMethods_CompileInternal(ILExecThread * _thread, 
										ILString * pattern, ILInt32 flags)
{
	char *pat;
	int error;
	regex_t *result;
	pat=ILStringToAnsi(_thread,pattern);
	if(!pat)
	{
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	result=ILCalloc(1,sizeof(regex_t));
	if(!result)
	{
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	error=IL_regcomp(result,pat,flags);
	if(error)
	{
		ILFree(result);
		return 0;
	}
	return (ILNativeInt)result;
}

/*
 * public static IntPtr CompileWithSyntaxInternal(String pattern, int syntax);
 */
ILNativeInt _IL_RegexpMethods_CompileWithSyntaxInternal(ILExecThread *_thread,
														ILString *pattern,
														ILInt32 syntax)
{
	char *pat;
	int error;
	struct re_pattern_buffer *result;
	pat=ILStringToAnsi(_thread,pattern);
	if(!pat)
	{
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	result=(struct re_pattern_buffer *)ILCalloc
				(1,sizeof(struct re_pattern_buffer));
	if(!result)
	{
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	if(syntax == RE_SYNTAX_POSIX_BASIC)
	{
		error=IL_regcomp(result,pat,0);
	}
	else if(syntax == RE_SYNTAX_POSIX_EXTENDED)
	{
		error=IL_regcomp(result,pat,REG_EXTENDED);
	}
	else
	{
		re_set_syntax((reg_syntax_t)syntax);
		error=(IL_re_compile_pattern(pat,strlen(pat),result) != NULL);
	}
	if(error != 0)
	{
		ILFree(result);
		return 0;
	}
	else
	{
		return (ILNativeInt)result;
	}
}

ILInt32 _IL_RegexpMethods_ExecInternal(ILExecThread * _thread,
									   ILNativeInt compiled,
									   ILString * input, ILInt32 flags)
{
	char *pat;
	pat= ILStringToAnsi(_thread,input);
	if(!pat)
	{
		ILExecThreadThrowOutOfMemory(_thread);
		return -1;
	}
	return IL_regexec((regex_t*)compiled,pat,0,0,flags);
}

/*
 * Match information that may be returned by "MatchInternal".
 */
typedef struct
{
	ILInt32	start;
	ILInt32 end;

} RegexMatch;

/*
 * public static Array MatchInternal(IntPtr compiled,
 *									 String input, int maxMatches,
 *								     int flags, Type elemType);
 */
ILObject *_IL_RegexpMethods_MatchInternal(ILExecThread *_thread,
										  ILNativeInt compiled,
										  ILString *input,
										  ILInt32 maxMatches,
										  ILInt32 flags,
										  ILObject *elemType)
{
	char *pat;
	regmatch_t *matches;
	ILClass *elemClass;
	ILObject *array;
	ILInt32 numMatches, index;
	RegexMatch *matchList;

	pat= ILStringToAnsi(_thread,input);
	if(!pat)
	{
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	if(maxMatches > 0)
	{
		matches = (regmatch_t *)ILCalloc(maxMatches, sizeof(regmatch_t));
		if(!matches)
		{
			ILExecThreadThrowOutOfMemory(_thread);
			return 0;
		}
	}
	else
	{
		matches = 0;
		maxMatches = 0;
	}
	if(IL_regexec((regex_t*)compiled,pat,(size_t)maxMatches,matches,flags) != 0)
	{
		if(matches != 0)
		{
			ILFree(matches);
		}
		return 0;
	}
	elemClass = _ILGetClrClass(_thread, elemType);
	if(!elemClass)
	{
		if(matches != 0)
		{
			ILFree(matches);
		}
		return 0;
	}
	
	numMatches = ((regex_t*)compiled)->re_nsub + 1;

	numMatches = (maxMatches < numMatches) ? maxMatches : numMatches ;
							
	array = _IL_Array_CreateArray_jiiii
		(_thread, (ILNativeInt)elemClass, 1, numMatches, 0, 0);
	if(!array)
	{
		if(matches != 0)
		{
			ILFree(matches);
		}
		return 0;
	}
	matchList = ArrayToBuffer(array);

	index = 0;
	while(index < numMatches)
	{
		matchList[index].start = (ILInt32)(matches[index].rm_so);
		matchList[index].end = (ILInt32)(matches[index].rm_eo);
		++index;
	}
	if(matches != 0)
	{
		ILFree(matches);
	}
	return array;
}

void _IL_RegexpMethods_FreeInternal(ILExecThread * _thread, 
									ILNativeInt compiled)
{
	if(compiled)
	{
		IL_regfree((regex_t*)compiled);
		ILFree((void*)compiled);
	}
}
#ifdef	__cplusplus
};
#endif
