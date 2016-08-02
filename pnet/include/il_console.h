/*
 * il_console.h - Console I/O routines.
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

#ifndef	_IL_CONSOLE_H
#define	_IL_CONSOLE_H

#include "il_values.h"

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Mode values for "ILConsoleSetMode".
 */
#define	IL_CONSOLE_NORMAL		0
#define	IL_CONSOLE_CBREAK		1
#define	IL_CONSOLE_RAW			2
#define	IL_CONSOLE_CBREAK_ALT	3
#define	IL_CONSOLE_RAW_ALT		4

/*
 * Flags for "ILConsoleGetLockState".
 */
#define	IL_CONSOLE_CAPS_LOCK	0x01
#define	IL_CONSOLE_NUM_LOCK		0x02

/*
 * Set the current console mode.
 */
void ILConsoleSetMode(ILInt32 mode);

/*
 * Get the current console mode.
 */
ILInt32 ILConsoleGetMode(void);

/*
 * Emit a beep on the console.
 */
void ILConsoleBeep(ILInt32 frequency, ILInt32 duration);

/*
 * Clear the console to the current foreground and background colors.
 */
void ILConsoleClear(void);

/*
 * Determine if there is a key available to be read.
 */
int ILConsoleKeyAvailable(void);

/*
 * Read the next key from the console.
 */
void ILConsoleReadKey(ILUInt16 *ch, ILInt32 *key, ILInt32 *modifiers);

/*
 * Set the position of the console cursor.
 */
void ILConsoleSetPosition(ILInt32 x, ILInt32 y);

/*
 * Get the position of the console cursor.
 */
void ILConsoleGetPosition(ILInt32 *x, ILInt32 *y);

/*
 * Get the size of the scrollback buffer.
 */
void ILConsoleGetBufferSize(ILInt32 *width, ILInt32 *height);

/*
 * Set the size of the scrollback buffer.
 */
void ILConsoleSetBufferSize(ILInt32 width, ILInt32 height);

/*
 * Get the size and position of the visible window area.
 */
void ILConsoleGetWindowSize(ILInt32 *left, ILInt32 *top,
							ILInt32 *width, ILInt32 *height);

/*
 * Set the size and position of the visible window area.
 */
void ILConsoleSetWindowSize(ILInt32 left, ILInt32 top,
							ILInt32 width, ILInt32 height);

/*
 * Get the largest possible size for the visible window area.
 */
void ILConsoleGetLargestWindowSize(ILInt32 *width, ILInt32 *height);

/*
 * Set the title on the console window.
 */
void ILConsoleSetTitle(const char *title);

/*
 * Get the foreground and background attributes.
 */
ILInt32 ILConsoleGetAttributes(void);

/*
 * Set the foreground and background attributes.
 */
void ILConsoleSetAttributes(ILInt32 attrs);

/*
 * Write a character to the console.
 */
void ILConsoleWriteChar(ILInt32 ch);

/*
 * Move an area of the screen buffer elsewhere, effecting a scroll.
 */
void ILConsoleMoveBufferArea(ILInt32 sourceLeft, ILInt32 sourceTop,
							 ILInt32 sourceWidth, ILInt32 sourceHeight,
							 ILInt32 targetLeft, ILInt32 targetTop,
							 ILUInt16 sourceChar, ILInt32 attributes);

/*
 * Get the current caps/num lock state.
 */
ILInt32 ILConsoleGetLockState(void);

/*
 * Get the current size of the blinking cursor.
 */
ILInt32 ILConsoleGetCursorSize(void);

/*
 * Set the current size of the blinking cursor.
 */
void ILConsoleSetCursorSize(ILInt32 size);

/*
 * Get the current cursor visibility.
 */
ILBool ILConsoleGetCursorVisible(void);

/*
 * Set the current cursor visibility.
 */
void ILConsoleSetCursorVisible(ILBool flag);

#ifdef	__cplusplus 
};
#endif

#endif	/* _IL_CONSOLE_H */
