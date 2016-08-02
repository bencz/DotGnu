/*
 * dumpconfig.c - Dump build configuration options.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

#include "il_config.h"
#include <stdio.h>
#ifdef HAVE_STRING_H
	#include <string.h>
#endif
#ifdef HAVE_SYS_UTSNAME_H
	#include <sys/utsname.h>
#endif
#include "engine_private.h"
#include "lib_defs.h"
#include "cvm.h"
#include "cvm_config.h"

#ifndef IL_CONFIG_REDUCE_CODE

static void PrintFormatted(FILE *stream, const char* left, const char * right)
{
	const int columnWidth=30;
	int spaces=columnWidth-strlen(left);
	fprintf(stream,left);
	while((spaces-- >= 0)) fputc(' ',stream);
	if(right)
	{
		fputs(": ",stream);
		/* Note: Ugly hack to capitalize first word of const string */
		if((right[0] >='a') && (right[0]<='z'))
		{
			fputc(right[0]-'a'+'A',stream);
			right++;
		}
		fprintf(stream,"%s\n",right);
	}
	else
	{
		fprintf(stream,":");
	}
}

int _ILDumpConfig(FILE *stream,int level)
{
#ifdef HAVE_UNAME
	struct utsname buf;
#endif
	PrintFormatted(stream,"Engine Version",VERSION);
#ifdef IL_USE_CVM
	PrintFormatted(stream,"Engine Flavour",IL_CVM_FLAVOUR);
#endif
#ifdef IL_USE_JIT
	PrintFormatted(stream,"Engine Flavour", "just in time compiler");
#endif

#ifdef HAVE_UNAME
	if(uname(&buf)==0)
	{
		PrintFormatted(stream,"Platform Info", NULL);
		fprintf(stream, " %s %s %s\n", buf.sysname, buf.release, buf.machine);
	}
#endif

#ifdef __DATE__
	#ifdef __TIME__
		PrintFormatted(stream,"Compiled On",NULL);
		fprintf(stream," %s %s (local)\n",__DATE__,__TIME__);
	#else
		PrintFormatted(stream,"Compiled On",__DATE__);
	#endif
#endif

#ifdef IL_USE_CVM
#ifdef HAVE_COMPUTED_GOTO
	#ifdef HAVE_PIC_COMPUTED_GOTO
		PrintFormatted(stream,"Computed Goto","Yes (PIC)");
	#else
		PrintFormatted(stream,"Computed Goto","Yes");
	#endif
#else
	PrintFormatted(stream,"Computed Goto","No");
#endif

	if(level > 1)
	{
		PrintFormatted(stream,"Fast Moves",NULL);
	#if defined(CVM_LONGS_ALIGNED_WORD) || defined(CVM_REALS_ALIGNED_WORD) || \
		defined(CVM_DOUBLES_ALIGNED_WORD) 
				
		#ifdef CVM_LONGS_ALIGNED_WORD
			fprintf(stream," longs");
		#endif
		#ifdef CVM_REALS_ALIGNED_WORD
			fprintf(stream," floats");
		#endif
		#ifdef CVM_DOUBLES_ALIGNED_WORD
			fprintf(stream," doubles");
		#endif
		fprintf(stream,"\n");
	#else
		fprintf(stream," None\n");
	#endif

	/* There was no way I could put that in cvm_config.h */
	#if defined(CVM_X86) || defined(CVM_ARM) || defined(CVM_PPC) || \
		defined(CVM_X86_64)
		#if defined(CVM_X86) && defined(__GNUC__) && !defined(IL_NO_ASM)
			PrintFormatted(stream,"Manual Register Allocation",
										"Yes (esi,edi,ebx)");
		#endif

		#if defined(CVM_X86_64) && defined(__GNUC__) && !defined(IL_NO_ASM)
			PrintFormatted(stream,"Manual Register Allocation",
										"Yes (r12,r14,r15)");
		#endif
			
		#if defined(CVM_ARM) && defined(__GNUC__) && !defined(IL_NO_ASM)
			PrintFormatted(stream,"Manual Register Allocation",
										"Yes (r4,r5,r6)");
		#endif
		
		#if defined(CVM_PPC) && defined(__GNUC__) && !defined(IL_NO_ASM)
			PrintFormatted(stream,"Manual Register Allocation",
										"Yes (r18,r19,r20)");
		#endif
		#if !defined(__GNUC__) || defined(IL_NO_ASM)
			PrintFormatted(stream,"Manual Register Allocation","No");
		#endif
	#else 
		PrintFormatted(stream,"Manual Register Allocation","No");
	#endif
	}
#endif

#ifdef HAVE_LIBGC
	PrintFormatted(stream,"Garbage Collector","Boehm");
#else
	PrintFormatted(stream,"Garbage Collector","No");
#endif

#ifdef HAVE_LIBFFI
	PrintFormatted(stream, "Libffi", "Yes");
#else
	PrintFormatted(stream, "Libffi", "No");
#endif

#ifdef IL_DEBUGGER
	PrintFormatted(stream,"Debugger support", "Yes");
#else
	PrintFormatted(stream,"Debugger support", "No");
#endif

	if(ILHasThreads())
	{
		PrintFormatted(stream, "Threading",  "Enabled");
	}
	else
	{
		PrintFormatted(stream, "Threading",  "Disabled");
	}

	if(level>1)
	{
	#ifdef IL_CONFIG_USE_THIN_LOCKS
		PrintFormatted(stream,"Monitor Implementation","Thin-locks");
	#else
		PrintFormatted(stream,"Monitor Implementation","Standard");
	#endif
	}

#ifdef BUILD_PROFILE_NAME
	PrintFormatted(stream,"Build Profile", ""BUILD_PROFILE_NAME"");
#endif

	if(level>1)
	{
	/* Profile options */
	
	#ifdef IL_CONFIG_FP_SUPPORTED
		PrintFormatted(stream, "Floating Points", "Enabled");
	#else
		PrintFormatted(stream, "Floating Points", "Disabled");
	#endif
	
	#ifdef IL_CONFIG_EXTENDED_NUMERICS
		PrintFormatted(stream, "Extended Numerics", "Enabled");
	#else
		PrintFormatted(stream, "Extended Numerics", "Disabled");
	#endif
	
	#ifdef IL_CONFIG_NON_VECTOR_ARRAYS
		PrintFormatted(stream, "Multi-Dimensional Arrays", "Enabled");
	#else
		PrintFormatted(stream, "Multi-Dimensional Arrays", "Disabled");
	#endif
		
	#ifdef IL_CONFIG_FILESYSTEM
		PrintFormatted(stream, "Filesystem Access", "Enabled");
	#else
		PrintFormatted(stream, "Filesystem Access", "Disabled");
	#endif
	
	#ifdef IL_CONFIG_NETWORKING
		PrintFormatted(stream, "Networking", "Enabled");
	#else
		PrintFormatted(stream, "Networking", "Disabled");
	#endif
	
	#ifdef IL_CONFIG_LATIN1
		PrintFormatted(stream, "Internationlization", "Disabled");
	#else
		PrintFormatted(stream, "Internationlization", "Enabled");
	#endif
	
	#ifdef IL_CONFIG_DEBUG_LINES
		PrintFormatted(stream,"Debug Support","Enabled");
	#else
		PrintFormatted(stream,"Debug Support","Disabled");
	#endif
	
	#ifdef IL_CONFIG_PINVOKE
		PrintFormatted(stream, "PInvoke Support", "Enabled");
	#else
		PrintFormatted(stream, "PInvoke Support", "Disabled");
	#endif
		
	#ifdef IL_CONFIG_REFLECTION
		PrintFormatted(stream, "Reflection Support", "Enabled");
	#else
		PrintFormatted(stream, "Reflection Support", "Disabled");
	#endif
	
	#ifdef IL_CONFIG_APPDOMAINS
		PrintFormatted(stream, "Appdomain Support", "Enabled");
	#else
		PrintFormatted(stream, "Appdomain Support", "Disabled");
	#endif
		
	#ifdef IL_CONFIG_REMOTING
		PrintFormatted(stream, "Remoting Support", "Enabled");
	#else
		PrintFormatted(stream, "Remoting Support", "Disabled");
	#endif
	}
	return 0;
}
#endif
