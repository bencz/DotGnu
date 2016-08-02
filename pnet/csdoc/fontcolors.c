
/*
 * fontcolors.c - C# source file to HTML translator
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * Author: Jeff Post
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
 *
 */
 
#include	"fontcolors.h"

char *fontColors[MAX_COLOR] = {
	"normal",		// black - normal text
	"csdecl",		// purple - declaration keywords
	"cstype",		// red - type keywords
	"csstmt",		// teal (blue-green) - statement keywords
	"csexpr",		// maroon (brown) -  expression keywords
	"comment",		// navy (dark blue) - comments
	"string"			// green - quoted strings
};

// Choose colors for token types

char *fontColorText[MAX_COLOR] = {
	"  .normal {color: black}\n",		// black - normal text
	"  .csdecl {color: purple}\n",	// purple - declaration keywords
	"  .cstype {color: red}\n",		// red - type keywords
	"  .csstmt {color: teal}\n",		// teal (blue-green) - statement keywords
	"  .csexpr {color: maroon}\n",	// maroon (brown) -  expression keywords
	"  .comment {color: navy}\n",		// navy (dark blue) - comments
	"  .string {color: green}\n"		// green - quoted strings
};

// end of file
