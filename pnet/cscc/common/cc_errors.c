/*
 * cc_errors.c - Error reporting functions for "cscc" plugins.
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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

#include "cc_errors.h"
#include "cc_options.h"

#ifdef	__cplusplus
extern	"C" {
#endif

int CCHaveErrors = 0;
int CCHaveWarnings = 0;

/*
 * Print an error or warning message to stderr.
 */
static void PrintMessage(const char *filename, unsigned long linenum, int warning,
						 const char *format, IL_VA_LIST va)
{
	/* Print the filename and line number information */
	if(filename)
	{
		fputs(filename, stderr);
		putc(':', stderr);
	}
	fprintf(stderr, "%lu: ", linenum);

	/* Print the "warning: " prefix if this is a warning */
	if(warning)
	{
		fputs(_("warning: "), stderr);
	}

	/* Print the message */
#ifdef HAVE_VFPRINTF
	vfprintf(stderr, format, va);
#else
	/* Shouldn't happen, but at least print the format */
	fputs(format, stderr);
#endif

	/* Terminate the line */
	putc('\n', stderr);
}

void CCError(const char *format, ...)
{
	IL_VA_START(format);
	PrintMessage(yycurrfilename(), yycurrlinenum(), 0, format, IL_VA_GET_LIST);
	IL_VA_END;
	CCHaveErrors = 1;
}

void CCErrorOnLineV(const char *filename, unsigned long linenum,
					const char *format, IL_VA_LIST va)
{
	PrintMessage(filename, linenum, 0, format, va);
	CCHaveErrors = 1;
}

void CCErrorOnLine(const char *filename, unsigned long linenum,
				   const char *format, ...)
{
	IL_VA_START(format);
	PrintMessage(filename, linenum, 0, format, IL_VA_GET_LIST);
	IL_VA_END;
	CCHaveErrors = 1;
}

void CCWarning(const char *format, ...)
{
	if(!inhibit_warnings)
	{
		IL_VA_START(format);
		PrintMessage(yycurrfilename(), yycurrlinenum(), 1,
					 format, IL_VA_GET_LIST);
		IL_VA_END;
		CCHaveWarnings = 1;
		if(warnings_as_errors)
		{
			CCHaveErrors = 1;
		}
	}
}

/*
 * Determine if a warning has been enabled.
 */
static int WarningEnabled(const char *type)
{
	if(inhibit_warnings)
	{
		return 0;
	}
	if(*type != '-')
	{
		/* Warning is off by default unless explicitly enabled */
		if(all_warnings)
		{
			if(CCStringListContainsInv(warning_flags, num_warning_flags, type))
			{
				return 0;
			}
			return 1;
		}
		else
		{
			return CCStringListContains(warning_flags, num_warning_flags, type);
		}
	}
	else
	{
		/* Warning is on by default unless explicitly disabled */
		if(all_warnings)
		{
			if(CCStringListContainsInv(warning_flags, num_warning_flags,
									   type + 1))
			{
				return 0;
			}
			return 1;
		}
		else
		{
			return !CCStringListContainsInv(warning_flags, num_warning_flags,
									        type + 1);
		}
	}
}

void CCTypedWarning(const char *type, const char *format, ...)
{
	if(WarningEnabled(type))
	{
		IL_VA_START(format);
		PrintMessage(yycurrfilename(), yycurrlinenum(), 1,
					 format, IL_VA_GET_LIST);
		IL_VA_END;
		CCHaveWarnings = 1;
		if(warnings_as_errors)
		{
			CCHaveErrors = 1;
		}
	}
}

void CCWarningOnLineV(const char *filename, unsigned long linenum,
					  const char *format, IL_VA_LIST va)
{
	if(!inhibit_warnings)
	{
		PrintMessage(filename, linenum, 1, format, va);
		CCHaveWarnings = 1;
		if(warnings_as_errors)
		{
			CCHaveErrors = 1;
		}
	}
}

void CCWarningOnLine(const char *filename, unsigned long linenum,
				     const char *format, ...)
{
	if(!inhibit_warnings)
	{
		IL_VA_START(format);
		PrintMessage(filename, linenum, 1, format, IL_VA_GET_LIST);
		IL_VA_END;
		CCHaveWarnings = 1;
		if(warnings_as_errors)
		{
			CCHaveErrors = 1;
		}
	}
}

void CCTypedWarningOnLine(const char *filename, unsigned long linenum,
				     	  const char *type, const char *format, ...)
{
	if(WarningEnabled(type))
	{
		IL_VA_START(format);
		PrintMessage(filename, linenum, 1, format, IL_VA_GET_LIST);
		IL_VA_END;
		CCHaveWarnings = 1;
		if(warnings_as_errors)
		{
			CCHaveErrors = 1;
		}
	}
}

void CCUnsafeMessage(ILGenInfo *info, ILNode *node, const char *construct)
{
	if(!(info->outputIsJava) &&
	   CCStringListContains(extension_flags, num_extension_flags, "unsafe"))
	{
		if(info->unsafeLevel == 0)
		{
			/* Unsafe construct used outside an unsafe context */
			CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
						  _("%s used outside an unsafe context"), construct);
		}
		else
		{
			/* Unsafe construct used inside an unsafe context */
			CCTypedWarningOnLine(yygetfilename(node), yygetlinenum(node),
								 "unsafe", "%s", construct);
		}
	}
	else if(info->outputIsJava)
	{
		/* Unsafe constructs are never permitted in Java */
		CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
					  _("%s not permitted with Java output"), construct);
	}
	else
	{
		/* Unsafe constructs are not permitted */
		CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
					  _("%s not permitted unless -funsafe specified"),
					  construct);
	}
}

void CCUnsafeTypeMessage(ILGenInfo *info, ILNode *node)
{
	CCUnsafeMessage(info, node, _("unsafe pointer type"));
}

void CCUnsafeEnter(ILGenInfo *info, ILNode *node, const char *construct)
{
	if(!(info->outputIsJava) &&
	   CCStringListContains(extension_flags, num_extension_flags, "unsafe"))
	{
		/* Unsafe constructs are permitted, so just print a warning */
		CCTypedWarningOnLine(yygetfilename(node), yygetlinenum(node),
							 "unsafe", "%s", construct);
	}
	else if(info->outputIsJava)
	{
		/* Unsafe constructs are never permitted in Java */
		CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
					  _("%s not permitted with Java output"), construct);
	}
	else
	{
		/* Unsafe constructs are not permitted */
		CCErrorOnLine(yygetfilename(node), yygetlinenum(node),
					  _("%s not permitted unless -funsafe specified"),
					  construct);
	}
	++(info->unsafeLevel);
	info->hasUnsafe = -1;
}

void CCUnsafeLeave(ILGenInfo *info)
{
	--(info->unsafeLevel);
}

#ifdef	__cplusplus
};
#endif
