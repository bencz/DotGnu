
/*
 * scanner.h - C# source file to HTML translator
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

enum ErrorCodes {
	NoError,
	UnexpectedToken,
	ScannerStateError,
};

enum scanState {			// scanner states
	START, INID, INCOMMENT, INTEXT, INSTRING, DONE
};

extern char	tokenString[MAX_LENGTH];		// next token string from scanner
extern char	namespace[MAX_LENGTH];			// token string for namespace

extern int	getToken(void);

// end of file
