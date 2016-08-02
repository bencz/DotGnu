/*
 * cg_errors.c - Error reporting functions for codegen.
 *
 * Copyright (C) 2009  Free Software Foundation, Inc.
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

#include "cg_nodes.h"
#include "cg_intl.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Backup function to print an error or warning message to stderr if the
 * error/warning functions in the gen info block are not set.
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

/*
 * Print an error relative to the given node.
 * Node must not be 0.
 */
void CGErrorForNode(ILGenInfo *info, ILNode *node, const char *format, ...)
{
	if(info->errFunc)
	{
		IL_VA_START(format);
		info->errFunc(yygetfilename(node), yygetlinenum(node),
					  format, IL_VA_GET_LIST);
		IL_VA_END;
	}
	else
	{
		IL_VA_START(format);
		PrintMessage(yygetfilename(node), yygetlinenum(node), 0,
					 format, IL_VA_GET_LIST);
		IL_VA_END;
	}
}

/*
 * Print an error relative to the given location.
 */
void CGErrorOnLine(ILGenInfo *info, const char *filename,
				   unsigned long linenum, const char *format, ...)
{
	if(info->errFunc)
	{
		IL_VA_START(format);
		info->errFunc(filename, linenum, format, IL_VA_GET_LIST);
		IL_VA_END;
	}
	else
	{
		IL_VA_START(format);
		PrintMessage(filename, linenum, 0,format, IL_VA_GET_LIST);
		IL_VA_END;
	}
}

/*
 * Print a warning relative to the given node.
 * Node must not be 0.
 */
void CGWarningForNode(ILGenInfo *info, ILNode *node, const char *format, ...)
{
	if(info->warnFunc)
	{
		IL_VA_START(format);
		info->warnFunc(yygetfilename(node), yygetlinenum(node),
					   format, IL_VA_GET_LIST);
		IL_VA_END;
	}
	else
	{
		IL_VA_START(format);
		PrintMessage(yygetfilename(node), yygetlinenum(node), 1,
					 format, IL_VA_GET_LIST);
		IL_VA_END;
	}
}

/*
 * Print a warning relative to the given location.
 */
void CGWarningOnLine(ILGenInfo *info, const char *filename,
					 unsigned long linenum, const char *format, ...)
{
	if(info->warnFunc)
	{
		IL_VA_START(format);
		info->warnFunc(filename, linenum, format, IL_VA_GET_LIST);
		IL_VA_END;
	}
	else
	{
		IL_VA_START(format);
		PrintMessage(filename, linenum, 1,format, IL_VA_GET_LIST);
		IL_VA_END;
	}
}

#ifdef	__cplusplus
};
#endif
