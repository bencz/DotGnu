/*
 * console.c - Console I/O routines.
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

#include "il_console.h"
#include "il_system.h"
#include <stdio.h>
#ifdef IL_WIN32_PLATFORM
#include <windows.h>
#ifdef IL_WIN32_NATIVE
	#include <malloc.h>
#else
	#include <alloca.h>
#endif
#else
#if TIME_WITH_SYS_TIME
	#include <sys/time.h>
    #include <time.h>
#else
    #if HAVE_SYS_TIME_H
		#include <sys/time.h>
    #else
        #include <time.h>
    #endif
#endif
#ifdef HAVE_SYS_TYPES_H
	#include <sys/types.h>
#endif
#ifdef HAVE_SYS_SELECT_H
	#include <sys/select.h>
#endif
#ifdef HAVE_UNISTD_H
	#include <unistd.h>
#endif
#include <errno.h>
#include <signal.h>
#if defined(IL_USE_TERMCAP)
	#include <termcap.h>
	#define	USE_TERMCAP	1
#elif defined(IL_USE_TERMINFO)
	#ifdef HAVE_CURSES_H
		#include <curses.h>
	#endif
	#ifdef HAVE_TERM_H
		#include <term.h>
	#endif
#endif
#ifdef HAVE_SYS_IOCTL_H
	#include <sys/ioctl.h>
#endif
#ifdef HAVE_TERMIOS_H
	#include <termios.h>
	#ifdef HAVE_TCGETATTR
		#define	USE_TTY_OPS	1
		#define	USE_TERMIOS	1
	#endif
#elif defined(HAVE_SYS_TERMIOS_H)
	#include <sys/termios.h>
	#ifdef HAVE_TCGETATTR
		#define	USE_TTY_OPS	1
		#define	USE_TERMIOS	1
	#endif
#endif
/* BeOS needs socket.h */
#ifdef HAVE_SYS_SOCKET_H
	#include <sys/socket.h>
#endif
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Determine which version of the console routines to use.
 */
#ifdef IL_WIN32_PLATFORM
	#define	IL_CONSOLE_WIN32	1
#else
#if defined(USE_TTY_OPS) && \
		(defined(IL_USE_TERMCAP) || defined(IL_USE_TERMINFO))
	#define	IL_CONSOLE_TERMCAP	1
#endif
#endif

/*
 * Key codes - keep in sync with "System.ConsoleKey".
 */
typedef enum
{
	Key_BackSpace			= 0x08,
	Key_Tab					= 0x09,
	Key_Clear				= 0x0C,
	Key_Enter				= 0x0D,
	Key_Pause				= 0x13,
	Key_Escape				= 0x1B,
	Key_SpaceBar			= 0x20,
	Key_PageUp				= 0x21,
	Key_PageDown			= 0x22,
	Key_End					= 0x23,
	Key_Home				= 0x24,
	Key_LeftArrow			= 0x25,
	Key_UpArrow				= 0x26,
	Key_RightArrow			= 0x27,
	Key_DownArrow			= 0x28,
	Key_Select				= 0x29,
	Key_Print				= 0x2A,
	Key_Execute				= 0x2B,
	Key_PrintScreen			= 0x2C,
	Key_Insert				= 0x2D,
	Key_Delete				= 0x2E,
	Key_Help				= 0x2F,
	Key_D0					= 0x30,
	Key_D1					= 0x31,
	Key_D2					= 0x32,
	Key_D3					= 0x33,
	Key_D4					= 0x34,
	Key_D5					= 0x35,
	Key_D6					= 0x36,
	Key_D7					= 0x37,
	Key_D8					= 0x38,
	Key_D9					= 0x39,
	Key_A					= 0x41,
	Key_B					= 0x42,
	Key_C					= 0x43,
	Key_D					= 0x44,
	Key_E					= 0x45,
	Key_F					= 0x46,
	Key_G					= 0x47,
	Key_H					= 0x48,
	Key_I					= 0x49,
	Key_J					= 0x4A,
	Key_K					= 0x4B,
	Key_L					= 0x4C,
	Key_M					= 0x4D,
	Key_N					= 0x4E,
	Key_O					= 0x4F,
	Key_P					= 0x50,
	Key_Q					= 0x51,
	Key_R					= 0x52,
	Key_S					= 0x53,
	Key_T					= 0x54,
	Key_U					= 0x55,
	Key_V					= 0x56,
	Key_W					= 0x57,
	Key_X					= 0x58,
	Key_Y					= 0x59,
	Key_Z					= 0x5A,
	Key_LeftWindows			= 0x5B,
	Key_RightWindows		= 0x5C,
	Key_Applications		= 0x5D,
	Key_Sleep				= 0x5F,
	Key_NumPad0				= 0x60,
	Key_NumPad1				= 0x61,
	Key_NumPad2				= 0x62,
	Key_NumPad3				= 0x63,
	Key_NumPad4				= 0x64,
	Key_NumPad5				= 0x65,
	Key_NumPad6				= 0x66,
	Key_NumPad7				= 0x67,
	Key_NumPad8				= 0x68,
	Key_NumPad9				= 0x69,
	Key_Multiply			= 0x6A,
	Key_Add					= 0x6B,
	Key_Separator			= 0x6C,
	Key_Subtract			= 0x6D,
	Key_Decimal				= 0x6E,
	Key_Divide				= 0x6F,
	Key_F1					= 0x70,
	Key_F2					= 0x71,
	Key_F3					= 0x72,
	Key_F4					= 0x73,
	Key_F5					= 0x74,
	Key_F6					= 0x75,
	Key_F7					= 0x76,
	Key_F8					= 0x77,
	Key_F9					= 0x78,
	Key_F10					= 0x79,
	Key_F11					= 0x7A,
	Key_F12					= 0x7B,
	Key_F13					= 0x7C,
	Key_F14					= 0x7D,
	Key_F15					= 0x7E,
	Key_F16					= 0x7F,
	Key_F17					= 0x80,
	Key_F18					= 0x81,
	Key_F19					= 0x82,
	Key_F20					= 0x83,
	Key_F21					= 0x84,
	Key_F22					= 0x85,
	Key_F23					= 0x86,
	Key_F24					= 0x87,
	Key_BrowserBack			= 0xA6,
	Key_BrowserForward		= 0xA7,
	Key_BrowserRefresh		= 0xA8,
	Key_BrowserStop			= 0xA9,
	Key_BrowserSearch		= 0xAA,
	Key_BrowserFavorites	= 0xAB,
	Key_BrowserHome			= 0xAC,
	Key_VolumeMute			= 0xAD,
	Key_VolumeDown			= 0xAE,
	Key_VolumeUp			= 0xAF,
	Key_MediaNext			= 0xB0,
	Key_MediaPrevious		= 0xB1,
	Key_MediaStop			= 0xB2,
	Key_MediaPlay			= 0xB3,
	Key_LaunchMail			= 0xB4,
	Key_LaunchMediaSelect	= 0xB5,
	Key_LaunchApp1			= 0xB6,
	Key_LaunchApp2			= 0xB7,
	Key_Oem1				= 0xBA,
	Key_OemPlus				= 0xBB,
	Key_OemComma			= 0xBC,
	Key_OemMinus			= 0xBD,
	Key_OemPeriod			= 0xBE,
	Key_Oem2				= 0xBF,
	Key_Oem3				= 0xC0,
	Key_Oem4				= 0xDB,
	Key_Oem5				= 0xDC,
	Key_Oem6				= 0xDD,
	Key_Oem7				= 0xDE,
	Key_Oem8				= 0xDF,
	Key_Oem102				= 0xE2,
	Key_Process				= 0xE5,
	Key_Packet				= 0xE7,
	Key_Attention			= 0xF6,
	Key_CrSel				= 0xF7,
	Key_ExSel				= 0xF8,
	Key_EraseEndOfFile		= 0xF9,
	Key_Play				= 0xFA,
	Key_Zoom				= 0xFB,
	Key_NoName				= 0xFC,
	Key_Pa1					= 0xFD,
	Key_OemClear			= 0xFE,

	/* Special key codes that are specific to our console implementation
	   to propagate certain signals up to the application program */
	Key_SizeChanged			= 0x1200,	/* Window size changed */
	Key_Resumed				= 0x1201,	/* Resumed after CTRL-Z suspend */
	Key_Interrupt			= 0x1202,	/* CTRL-C interrupt detected */
	Key_CtrlBreak			= 0x1203,	/* CTRL-BREAK interrupt detected */

} ConsoleKey;

/*
 * Modifier masks - keep in sync with "System.ConsoleModifiers".
 */
typedef enum
{
	Mod_Alt		= 0x0001,
	Mod_Shift	= 0x0002,
	Mod_Control	= 0x0004

} ConsoleModifiers;

#if defined(IL_CONSOLE_TERMCAP)

/*
 * Forward declaration.
 */
static void MapCharToKey(int ch, ILInt32 *key, ILInt32 *modifiers);
#define	IL_NEED_MAPCHAR		1
#define	IL_CHECK_ERASE		1

/*
 * Capability names for various special keys.
 */
typedef struct
{
	const char		   *name;
	const char		   *terminfoName;
	ConsoleKey			key;
	ConsoleModifiers	modifiers;

} SpecialKeyCap;
static SpecialKeyCap const SpecialKeys[] = {
	{"#1",			"kHLP",		Key_Help,			Mod_Shift},
	{"#2",			"hHOM",		Key_Home,			Mod_Shift},
	{"#4",			"kLFT",		Key_LeftArrow,		Mod_Shift},
	{"%1",			"khlp",		Key_Help,			0},
	{"%9",			"kprt",		Key_Print,			0},
	{"&9",			"kBEG",		Key_Home,			Mod_Shift},
	{"*7",			"kEND",		Key_End,			Mod_Shift},
	{"@1",			"kbeg",		Key_Home,			0},
	{"@7",			"kend",		Key_End,			0},
	{"k0",			"kf0",		Key_F10,			0},
	{"k1",			"kf1",		Key_F1,				0},
	{"k2",			"kf2",		Key_F2,				0},
	{"k3",			"kf3",		Key_F3,				0},
	{"k4",			"kf4",		Key_F4,				0},
	{"k5",			"kf5",		Key_F5,				0},
	{"k6",			"kf6",		Key_F6,				0},
	{"k7",			"kf7",		Key_F7,				0},
	{"k8",			"kf8",		Key_F8,				0},
	{"k9",			"kf9",		Key_F9,				0},
	{"k;",			"kf10",		Key_F10,			0},
	{"F1",			"kf11",		Key_F11,			0},
	{"F2",			"kf12",		Key_F12,			0},
	{"F3",			"kf13",		Key_F13,			0},
	{"F4",			"kf14",		Key_F14,			0},
	{"F5",			"kf15",		Key_F15,			0},
	{"F6",			"kf16",		Key_F16,			0},
	{"F7",			"kf17",		Key_F17,			0},
	{"F8",			"kf18",		Key_F18,			0},
	{"F9",			"kf19",		Key_F19,			0},
	{"FA",			"kf20",		Key_F20,			0},
	{"FB",			"kf21",		Key_F21,			0},
	{"FC",			"kf22",		Key_F22,			0},
	{"FD",			"kf23",		Key_F23,			0},
	{"FE",			"kf24",		Key_F24,			0},
	{"K1",			"ka1",		Key_Home,			0},
	{"K3",			"ka3",		Key_PageUp,			0},
	{"K4",			"kc1",		Key_End,			0},
	{"K5",			"kc3",		Key_PageDown,		0},
	{"kb",			"kbs",		Key_BackSpace,		0},
	{"kB",			"kcbt",		Key_Tab,			Mod_Shift},
	{"kd",			"kcud1",	Key_DownArrow,		0},
	{"kD",			"kdch1",	Key_Delete,			0},
	{"kF",			"kind",		Key_PageDown,		0},
	{"kh",			"khome",	Key_Home,			0},
	{"kH",			"kll",		Key_PageDown,		Mod_Control},
	{"kI",			"kich1",	Key_Insert,			0},
	{"kl",			"kcub1",	Key_LeftArrow,		0},
	{"kN",			"knp",		Key_PageDown,		0},
	{"kP",			"kpp",		Key_PageUp,			0},
	{"kr",			"kcuf1",	Key_RightArrow,		0},
	{"kR",			"kri",		Key_PageUp,			0},
	{"ku",			"kcuu1",	Key_UpArrow,		0},

	/* Known VT* special key codes that are sometimes not listed in termcap but
	   can be generated if the terminal is in an odd ANSI/VT compat mode */
	{"\033[A",		0,			Key_UpArrow,		0},
	{"\033[B",		0,			Key_DownArrow,		0},
	{"\033[C",		0,			Key_RightArrow,		0},
	{"\033[D",		0,			Key_LeftArrow,		0},
	{"\033[H",		0,			Key_Home,			0},
	{"\033[F",		0,			Key_End,			0},
	{"\033[P",		0,			Key_F1,				0},
	{"\033[Q",		0,			Key_F2,				0},
	{"\033[R",		0,			Key_F3,				0},
	{"\033[S",		0,			Key_F4,				0},
	{"\033[Z",		0,			Key_Tab,			Mod_Shift},
	{"\033OA",		0,			Key_UpArrow,		0},
	{"\033OB",		0,			Key_DownArrow,		0},
	{"\033OC",		0,			Key_RightArrow,		0},
	{"\033OD",		0,			Key_LeftArrow,		0},
	{"\033OH",		0,			Key_Home,			0},
	{"\033OF",		0,			Key_End,			0},
	{"\033OP",		0,			Key_F1,				0},
	{"\033OQ",		0,			Key_F2,				0},
	{"\033OR",		0,			Key_F3,				0},
	{"\033OS",		0,			Key_F4,				0},
	{"\033OZ",		0,			Key_Tab,			Mod_Shift},

	/* Other common keycodes, typical to the "linux" and "xterm" entries */
	{"\033[[A",		0,			Key_F1,				0},
	{"\033[[B",		0,			Key_F2,				0},
	{"\033[[C",		0,			Key_F3,				0},
	{"\033[[D",		0,			Key_F4,				0},
	{"\033[[E",		0,			Key_F5,				0},
	{"\033[15~",	0,			Key_F5,				0},
	{"\033[17~",	0,			Key_F6,				0},
	{"\033[18~",	0,			Key_F7,				0},
	{"\033[19~",	0,			Key_F8,				0},
	{"\033[20~",	0,			Key_F9,				0},
	{"\033[21~",	0,			Key_F10,			0},
	{"\033[23~",	0,			Key_F11,			0},
	{"\033[24~",	0,			Key_F12,			0},
	{"\033[25~",	0,			Key_F13,			0},
	{"\033[26~",	0,			Key_F14,			0},
	{"\033[28~",	0,			Key_F15,			0},
	{"\033[29~",	0,			Key_F16,			0},
	{"\033[31~",	0,			Key_F17,			0},
	{"\033[32~",	0,			Key_F18,			0},
	{"\033[33~",	0,			Key_F19,			0},
	{"\033[34~",	0,			Key_F20,			0},
	{"\033[[2A",	0,			Key_F1,				0},
	{"\033[[2B",	0,			Key_F2,				0},
	{"\033[[2C",	0,			Key_F3,				0},
	{"\033[[2D",	0,			Key_F4,				0},
	{"\033[[2E",	0,			Key_F5,				0},
	{"\033[2P",		0,			Key_F1,				Mod_Shift},
	{"\033[2Q",		0,			Key_F2,				Mod_Shift},
	{"\033[2R",		0,			Key_F3,				Mod_Shift},
	{"\033[2S",		0,			Key_F4,				Mod_Shift},
	{"\033[15;2~",	0,			Key_F5,				Mod_Shift},
	{"\033[17;2~",	0,			Key_F6,				Mod_Shift},
	{"\033[18;2~",	0,			Key_F7,				Mod_Shift},
	{"\033[19;2~",	0,			Key_F8,				Mod_Shift},
	{"\033[20;2~",	0,			Key_F9,				Mod_Shift},
	{"\033[21;2~",	0,			Key_F10,			Mod_Shift},
	{"\033[23;2~",	0,			Key_F11,			Mod_Shift},
	{"\033[24;2~",	0,			Key_F12,			Mod_Shift},
	{"\033[25;2~",	0,			Key_F13,			0},
	{"\033[26;2~",	0,			Key_F14,			0},
	{"\033[28;2~",	0,			Key_F15,			0},
	{"\033[29;2~",	0,			Key_F16,			0},
	{"\033[31;2~",	0,			Key_F17,			0},
	{"\033[32;2~",	0,			Key_F18,			0},
	{"\033[33;2~",	0,			Key_F19,			0},
	{"\033[34;2~",	0,			Key_F20,			0},
	{"\033O2P",		0,			Key_F1,				Mod_Shift},
	{"\033O2Q",		0,			Key_F2,				Mod_Shift},
	{"\033O2R",		0,			Key_F3,				Mod_Shift},
	{"\033O2S",		0,			Key_F4,				Mod_Shift},
	{"\033[1~",		0,			Key_Home,			0},
	{"\033[2~",		0,			Key_Insert,			0},
	{"\033[3~",		0,			Key_Delete,			0},
	{"\033[4~",		0,			Key_End,			0},
	{"\033[5~",		0,			Key_PageUp,			0},
	{"\033[6~",		0,			Key_PageDown,		0},
	{"\033[7~",		0,			Key_Home,			0},
	{"\033[8~",		0,			Key_End,			0},
	{"\033[2H",		0,			Key_Home,			Mod_Shift},
	{"\033[2F",		0,			Key_End,			Mod_Shift},
	{"\033O2H",		0,			Key_Home,			Mod_Shift},
	{"\033O2F",		0,			Key_End,			Mod_Shift},
	{"\033[1;2~",	0,			Key_Home,			Mod_Shift},
	{"\033[2;2~",	0,			Key_Insert,			Mod_Shift},
	{"\033[3;2~",	0,			Key_Delete,			Mod_Shift},
	{"\033[4;2~",	0,			Key_End,			Mod_Shift},
	{"\033[5;2~",	0,			Key_PageUp,			Mod_Shift},
	{"\033[6;2~",	0,			Key_PageDown,		Mod_Shift},
	{"\033[7;2~",	0,			Key_Home,			Mod_Shift},
	{"\033[8;2~",	0,			Key_End,			Mod_Shift},
};
#define	NumSpecialKeys	(sizeof(SpecialKeys) / sizeof(SpecialKeyCap))
static char *SpecialKeyStrings[NumSpecialKeys];

/*
 * Global state for the termcap-based console driver.
 */
static int consoleMode = IL_CONSOLE_NORMAL;
static int termcapInitialized = 0;
#ifdef IL_USE_TERMCAP
static char *termcapBuffer = 0;
static char *termcapBuffer2 = 0;
#endif
static int consoleIsTty = 0;
static int screenX = 0;
static int screenY = 0;
static int screenWidth = 80;
static int screenHeight = 24;
static int terminalHasColor = 0;
static int terminalHasTitle = 0;
static int eraseIsDel = 0;
static int volatile windowSizeChanged = 0;
static int volatile processResumed = 0;
static int volatile interruptSeen = 0;
#define	KEY_BUFFER_SIZE		32
static char keyBuffer[KEY_BUFFER_SIZE];
static int keyBufferLen = 0;
static char *enterAltMode = 0;
static char *leaveAltMode = 0;
static char *makeCursorVisible = 0;
#ifdef USE_TERMIOS
static struct termios savedNormal;
static struct termios currentAttrs;
#endif
static int consoleAttrs = 0x07;
static int cursorVisible = 1;
static int cursorSize = 25;

/*
 * Output a character to the console.
 */
static int ConsolePutchar(int ch)
{
	putc(ch, stdout);
	return 0;
}

/*
 * Get a particular string capability.
 */
static char *GetStringCap(const char *termcapName, const char *terminfoName)
{
#ifdef IL_USE_TERMCAP
	char *area = termcapBuffer2;
	return tgetstr((char *)termcapName, &area);
#else
	char *str;
	if(!terminfoName)
	{
		return 0;
	}
	str = tigetstr((char *)terminfoName);
	if(!str || str == (char *)(ILNativeInt)(-1))
	{
		return 0;
	}
	return str;
#endif
}

/*
 * Get a particular flag capability.
 */
static int GetFlagCap(const char *termcapName, const char *terminfoName)
{
#ifdef IL_USE_TERMCAP
	return tgetflag((char *)termcapName);
#else
	return (tigetflag((char *)terminfoName) > 0);
#endif
}

/*
 * Output a particular string capability.  Returns zero if no such capability.
 */
static int OutputStringCap(const char *termcapName, const char *terminfoName)
{
	char *str = GetStringCap(termcapName, terminfoName);
	if(str)
	{
		tputs(str, 1, ConsolePutchar);
		fflush(stdout);
		return 1;
	}
	else
	{
		return 0;
	}
}

/*
 * Initialize the termcap-based console driver.
 * Returns zero if stdin/stdout are not tty's.
 */
static int InitTermcapDriver(void)
{
	char *term;
	char *str;
	int posn;
#if defined(IL_USE_TERMINFO)
	int errret = ERR;
#endif
	if(termcapInitialized)
	{
		return consoleIsTty;
	}
	term = getenv("TERM");
	if(!isatty(0) || !isatty(1) || !term || *term == '\0')
	{
		/* Either stdin or stdout is not a tty, so cannot use console mode */
		consoleIsTty = 0;
	}
#ifdef IL_USE_TERMCAP
	else if((termcapBuffer = (char *)ILMalloc(8192)) == 0 ||
	        (termcapBuffer2 = (char *)ILMalloc(8192)) == 0)
	{
		/* Insufficient memory to do termcap processing */
		consoleIsTty = 0;
	}
	else if(tgetent(termcapBuffer, term) <= 0)
	{
		/* Could not load the termcap entry for the terminal */
		consoleIsTty = 0;
	}
#else
	if(setupterm(term, 1, &errret) != OK)
	{
		/* Could not load the terminfo entry for the terminal */
		consoleIsTty = 0;
	}
#endif
	else
	{
		/* Prime the screen size information from the termcap details */
	#ifdef IL_USE_TERMCAP
		screenWidth = tgetnum("co");
		if(screenWidth <= 0)
		{
			screenWidth = 80;
		}
		screenHeight = tgetnum("li");
		if(screenHeight <= 0)
		{
			screenHeight = 24;
		}
	#else
		screenWidth = tigetnum("cols");
		if(screenWidth <= 0)
		{
			screenWidth = 80;
		}
		screenHeight = tigetnum("lines");
		if(screenHeight <= 0)
		{
			screenHeight = 24;
		}
	#endif

		/* Query the terminal itself as to what the size is */
	#ifdef TIOCGWINSZ
		{
			struct winsize size;
			size.ws_row = 0;
			size.ws_col = 0;
			if(ioctl(0, TIOCGWINSZ, &size) >= 0)
			{
				if(size.ws_row > 0)
				{
					screenHeight = size.ws_row;
				}
				if(size.ws_col > 0)
				{
					screenWidth = size.ws_col;
				}
			}
		}
	#endif

		/* There's no way to get the current cursor position, so we just
		   assume that the position is the start of the bottom-most line */
		screenX = 0;
		screenY = screenHeight - 1;

		/* Determine if the terminal supports the xterm title sequence */
		if(!strcmp(term, "xterm") ||
		   !strncmp(term, "xterm-", 6) ||
		   !strcmp(term, "color-xterm") ||
		   !strcmp(term, "color_xterm") ||
		   !strcmp(term, "rxvt"))
		{
			terminalHasTitle = 1;
		}

		/* Determine if the terminal has ANSI color sequences */
		if(GetStringCap("AB", "setab"))
		{
			terminalHasColor = 1;
		}
		else if(terminalHasTitle)
		{
			/* All of the xterm variants support color */
			terminalHasColor = 1;
		}
		else if(!strcmp(term, "linux"))
		{
			terminalHasColor = 1;
		}

		/* Populate the list of special keys */
		for(posn = 0; posn < NumSpecialKeys; ++posn)
		{
			if(SpecialKeys[posn].name[0] == '\033')
			{
				/* This is an explicit key code for things that are
				   usually keycodes but not the ones listed in termcap */
				SpecialKeyStrings[posn] = ILDupString(SpecialKeys[posn].name);
			}
			else
			{
				str = GetStringCap(SpecialKeys[posn].name,
								   SpecialKeys[posn].terminfoName);
				if(str && *str != '\0')
				{
					SpecialKeyStrings[posn] = ILDupString(str);
				}
				else
				{
					SpecialKeyStrings[posn] = 0;
				}
			}
		}

		/* Get the strings to enter or leave "alternative" mode */
		enterAltMode = 0;
		leaveAltMode = 0;
		if(consoleMode == IL_CONSOLE_CBREAK_ALT ||
		   consoleMode == IL_CONSOLE_RAW_ALT)
		{
			str = GetStringCap("ti", "smcup");
			if(str && *str != '\0')
			{
				enterAltMode = ILDupString(str);
			}
			str = GetStringCap("te", "rmcup");
			if(str && *str != '\0')
			{
				leaveAltMode = ILDupString(str);
			}
			if(enterAltMode)
			{
				/* Write out the "enter alternative mode" string */
				tputs(enterAltMode, 1, ConsolePutchar);
				fflush(stdout);
			}
		}

		/* Get the string to use to make the cursor normal on exit */
		str = GetStringCap("ve", "cnorm");
		if(str && *str != '\0')
		{
			makeCursorVisible = ILDupString(str);
		}

		/* Assume that the attributes are currently set to the default */
		consoleAttrs = 0x07;

		/* Make the cursor initially visible and normal */
		cursorVisible = 1;
		cursorSize = 25;
		if(makeCursorVisible)
		{
			tputs(makeCursorVisible, 1, ConsolePutchar);
		}

		/* The console is a known tty */
		consoleIsTty = 1;
	}
	termcapInitialized = 1;
	return consoleIsTty;
}

/*
 * Refresh the window size from the kernel after a size change.
 */
static void RefreshWindowSize(void)
{
#ifdef TIOCGWINSZ
	struct winsize size;
	size.ws_row = 0;
	size.ws_col = 0;
	if(ioctl(0, TIOCGWINSZ, &size) >= 0)
	{
		if(size.ws_row > 0)
		{
			screenHeight = size.ws_row;
		}
		if(size.ws_col > 0)
		{
			screenWidth = size.ws_col;
		}
	}
#endif
}

#ifdef SIGINT

/*
 * Notice the signal that indicates a CTRL-C interrupt so that it
 * can be inserted into the character-at-a-time input buffer.
 * This is only used in cbreak mode, never normal or raw.
 */
static void InterruptProcess(int sig)
{
	interruptSeen = 1;
	signal(SIGINT, InterruptProcess);
}

#endif

#ifdef SIGWINCH

/*
 * Notice the signal that indicates a change in window size.
 */
static void WindowSizeChanged(int sig)
{
	windowSizeChanged = 1;
	signal(SIGWINCH, WindowSizeChanged);
}

#endif

#if defined(SIGTSTP) && defined(SIGCONT)

/*
 * Notice the signal that indicates that the process should suspend.
 */
static void StopProcess(int sig)
{
	/* Restore the cursor */
	if(makeCursorVisible)
	{
		write(1, makeCursorVisible, strlen(makeCursorVisible));
	}

	/* Leave "alternative" mode */
	if(leaveAltMode)
	{
		write(1, leaveAltMode, strlen(leaveAltMode));
	}

#ifdef USE_TERMIOS
	/* Restore the default tty attributes */
	tcsetattr(0, TCSANOW, &savedNormal);
#endif

	/* Re-install the signal handler for next time */
	signal(SIGTSTP, StopProcess);

	/* Suspend the process for real */
	kill(getpid(), SIGSTOP);
}

/*
 * Notice the signal that indicates that the process has resumed.
 */
static void ContinueProcess(int sig)
{
#ifdef USE_TERMIOS
	/* Restore the tty settings that we require */
	tcsetattr(0, TCSANOW, &currentAttrs);
#endif

	/* Re-enter "alternative" mode */
	if(enterAltMode)
	{
		write(1, enterAltMode, strlen(enterAltMode));
	}

	/* Record that the process has resumed */
	processResumed = 1;

	/* Re-install the signal handler for next time */
	signal(SIGCONT, ContinueProcess);
}

#endif

/*
 * Work around missing symbols on some platforms.
 */
#ifndef	IUCLC
#define	IUCLC	0
#endif
#ifndef	OLCUC
#define	OLCUC	0
#endif
#ifndef	OFILL
#define	OFILL	0
#endif
#ifndef	OFDEL
#define	OFDEL	0
#endif
#ifndef	OCRNL
#define	OCRNL	0
#endif
#ifndef	ONOCR
#define	ONOCR	0
#endif
#ifndef	ONLRET
#define	ONLRET	0
#endif

void ILConsoleSetMode(ILInt32 mode)
{
	char *term;

	/* Bail out if no mode change required */
	if(mode == consoleMode)
	{
		return;
	}

	/* Bail out if the console is not a tty */
	term = getenv("TERM");
	if(!isatty(0) || !isatty(1) || !term || *term == '\0')
	{
		return;
	}

	/* Save the original tty attributes if changing from "normal" */
	if(consoleMode == IL_CONSOLE_NORMAL)
	{
#ifdef USE_TERMIOS
		tcgetattr(0, &savedNormal);
		eraseIsDel = (savedNormal.c_cc[VERASE] == 0x7F);
#endif
	}

	/* Create the desired attributes */
	if(mode == IL_CONSOLE_NORMAL)
	{
#ifdef USE_TERMIOS
		currentAttrs = savedNormal;
#endif
	}
	else if(mode == IL_CONSOLE_CBREAK || mode == IL_CONSOLE_CBREAK_ALT)
	{
#ifdef USE_TERMIOS
		currentAttrs = savedNormal;
		currentAttrs.c_iflag &= ~(INLCR | IGNCR | ICRNL);
		currentAttrs.c_oflag &= ~(ONLCR | OCRNL | ONOCR | ONLRET);
		currentAttrs.c_lflag &= ~(ICANON | ECHO);
		currentAttrs.c_lflag |= ISIG;
#endif
	}
	else
	{
#ifdef USE_TERMIOS
		currentAttrs = savedNormal;
		currentAttrs.c_iflag &= ~(INLCR | IGNCR | ICRNL);
		currentAttrs.c_oflag &= ~(ONLCR | OCRNL | ONOCR | ONLRET);
		currentAttrs.c_lflag &= ~(ICANON | ECHO | ISIG);
#endif
	}

	/* Set the attributes */
#ifdef USE_TERMIOS
	tcsetattr(0, TCSANOW, &currentAttrs);
#endif

	/* Make the cursor visible again */
	if(makeCursorVisible)
	{
		write(1, makeCursorVisible, strlen(makeCursorVisible));
	}
	makeCursorVisible = 0;

	/* Leave "alternative" mode if we were previously in it */
	if(leaveAltMode)
	{
		write(1, leaveAltMode, strlen(leaveAltMode));
	}
	enterAltMode = 0;
	leaveAltMode = 0;

	/* Trap interesting signals */
#ifdef SIGINT
	if(mode == IL_CONSOLE_CBREAK || mode == IL_CONSOLE_CBREAK_ALT)
	{
		signal(SIGINT, InterruptProcess);
	}
	else
	{
		signal(SIGINT, SIG_DFL);
	}
#endif
#ifdef SIGWINCH
	signal(SIGWINCH, WindowSizeChanged);
#endif
#if defined(SIGTSTP) && defined(SIGCONT)
	signal(SIGTSTP, StopProcess);
	signal(SIGCONT, ContinueProcess);
#endif

	/* We are now in the new console mode */
	consoleMode = mode;
}

ILInt32 ILConsoleGetMode(void)
{
	return consoleMode;
}

void ILConsoleBeep(ILInt32 frequency, ILInt32 duration)
{
	if(InitTermcapDriver())
	{
		OutputStringCap("bl", "bel");
	}
}

void ILConsoleClear(void)
{
	if(InitTermcapDriver())
	{
		if(!OutputStringCap("cl", "clear"))
		{
			OutputStringCap("ho", "home");
			OutputStringCap("cd", "ed");
		}
		screenX = 0;
		screenY = 0;
	}
}

int ILConsoleKeyAvailable(void)
{
	fd_set readSet;
	struct timeval tv;
	int result;
	for(;;)
	{
		if(windowSizeChanged || processResumed || interruptSeen)
		{
			/* There is a pending signal to turn into a key indication */
			return 1;
		}
		if(keyBufferLen > 0)
		{
			/* There are left-over lookahead keys in the buffer */
			return 1;
		}
		FD_ZERO(&readSet);
		FD_SET(0, &readSet);
		tv.tv_sec = 0;
		tv.tv_usec = 0;
		result = select(1, &readSet, (fd_set *)0, (fd_set *)0, &tv);
		if(result > 0)
		{
			/* There is something waiting in the stdin input buffer */
			return 1;
		}
		else if(result < 0 && errno == EINTR)
		{
			/* The system call was interrupted, so retry it */
			continue;
		}
		else
		{
			/* Timeout occurred, so no key available */
			break;
		}
	}
	return 0;
}

void ILConsoleReadKey(ILUInt16 *ch, ILInt32 *key, ILInt32 *modifiers)
{
	fd_set readSet;
	struct timeval tv;
	struct timeval *tvp;
	int result, c, posn;
	int len, prefixMatch;
	for(;;)
	{
		/* Turn pending signals into special key codes */
		if(processResumed)
		{
			processResumed = 0;
			*ch = 0;
			*key = Key_Resumed;
			*modifiers = 0;
			return;
		}
		if(windowSizeChanged)
		{
			RefreshWindowSize();
			windowSizeChanged = 0;
			*ch = 0;
			*key = Key_SizeChanged;
			*modifiers = 0;
			return;
		}
		if(interruptSeen)
		{
			interruptSeen = 0;
			*ch = 0;
			*key = Key_Interrupt;
			*modifiers = 0;
			return;
		}

		/* Wait for an incoming character */
		FD_ZERO(&readSet);
		FD_SET(0, &readSet);
		if(keyBufferLen > 0)
		{
			tv.tv_sec = 0;
			tv.tv_usec = 300000;
			tvp = &tv;
		}
		else
		{
			tvp = (struct timeval *)0;
		}
		result = select(1, &readSet, (fd_set *)0, (fd_set *)0, tvp);
		if(result > 0)
		{
			/* There is something waiting in the stdin input buffer */
			if(read(0, keyBuffer + keyBufferLen, 1) == 1)
			{
				++keyBufferLen;
				prefixMatch = 0;
				for(posn = 0; posn < NumSpecialKeys; ++posn)
				{
					if(SpecialKeyStrings[posn] &&
					   SpecialKeyStrings[posn][0] == keyBuffer[0])
					{
						/* We have a potential prefix match */
						len = strlen(SpecialKeyStrings[posn]);
						if(len <= keyBufferLen &&
						   !ILMemCmp(keyBuffer, SpecialKeyStrings[posn], len))
						{
							/* Found a special key */
							if(len == 1)
							{
								/* Return the underlying character if the
								   key only has one associated with it */
								*ch = (ILUInt16)(keyBuffer[0] & 0xFF);
							}
							else
							{
								*ch = 0;
							}
							if(keyBufferLen > len)
							{
								ILMemMove(keyBuffer, keyBuffer + len,
										  keyBufferLen - len);
							}
							keyBufferLen -= len;
							*key = SpecialKeys[posn].key;
							*modifiers = SpecialKeys[posn].modifiers;
							return;
						}
						else if(len > keyBufferLen &&
						        !ILMemCmp(keyBuffer,
										  SpecialKeyStrings[posn],
										  keyBufferLen))
						{
							/* The buffer contains an entire key prefix */
							prefixMatch = 1;
						}
					}
				}
				if(!prefixMatch && keyBufferLen >= 2 && keyBuffer[0] == '\033')
				{
					/* This is probably a meta-character indication */
					goto meta;
				}
				if(!prefixMatch || keyBufferLen >= KEY_BUFFER_SIZE)
				{
					/* No prefix match, so we have a normal character */
					goto normalChar;
				}
			}
		}
		else if(result < 0 && errno == EINTR)
		{
			/* The system call was interrupted by a signal, so retry */
			continue;
		}
		else if(result < 0)
		{
			/* Some kind of error occurred */
			break;
		}
		else if(keyBufferLen >= 2 && keyBuffer[0] == '\033')
		{
			/* We got a timeout with at least two characters in the
			   lookahead buffer where the first is an Escape character.
			   This is probably a metacharacter indication.  e.g. ESC-A
			   is the same as ALT-A */
		meta:
			c = (((int)(keyBuffer[1])) & 0xFF);
			keyBufferLen -= 2;
			if(keyBufferLen > 0)
			{
				ILMemMove(keyBuffer, keyBuffer + 2, keyBufferLen);
			}
			*ch = 0;
			MapCharToKey(c, key, modifiers);
			*modifiers |= Mod_Alt;
			return;
		}
		else if(keyBufferLen > 0)
		{
			/* We got a timeout while processing lookahead characters.
			   Extract the first character from the buffer and return it.
			   It will normally be an Escape character, indicating the
			   actual Escape key rather than the start of a special key */
		normalChar:
			c = (((int)(keyBuffer[0])) & 0xFF);
			--keyBufferLen;
			if(keyBufferLen > 0)
			{
				ILMemMove(keyBuffer, keyBuffer + 1, keyBufferLen);
			}
			*ch = (ILUInt16)c;
			MapCharToKey(c, key, modifiers);
			return;
		}
	}
	*ch = 0;
	*key = 0;
	*modifiers = 0;
}

void ILConsoleSetPosition(ILInt32 x, ILInt32 y)
{
	if(InitTermcapDriver())
	{
		char *str = GetStringCap("cm", "cup");
		if(!str)
		{
			return;
		}
		if(x < 0)
		{
			x = 0;
		}
		else if(x >= screenWidth)
		{
			x = screenWidth - 1;
		}
		if(y < 0)
		{
			y = 0;
		}
		else if(y >= screenHeight)
		{
			y = screenHeight - 1;
		}
	#ifdef IL_USE_TERMCAP
		str = tgoto(str, x, y);
	#else
		str = tparm(str, y, x, 0, 0, 0, 0, 0, 0, 0);
	#endif
		if(str)
		{
			tputs(str, 1, ConsolePutchar);
			fflush(stdout);
		}
		screenX = x;
		screenY = y;
	}
}

void ILConsoleGetPosition(ILInt32 *x, ILInt32 *y)
{
	*x = screenX;
	*y = screenY;
}

void ILConsoleGetBufferSize(ILInt32 *width, ILInt32 *height)
{
	if(windowSizeChanged)
	{
		RefreshWindowSize();
	}
	*width = screenWidth;
	*height = screenHeight;
}

void ILConsoleSetBufferSize(ILInt32 width, ILInt32 height)
{
	/* We cannot change the buffer size with termcap */
}

void ILConsoleGetWindowSize(ILInt32 *left, ILInt32 *top,
							ILInt32 *width, ILInt32 *height)
{
	if(windowSizeChanged)
	{
		RefreshWindowSize();
	}
	*left = 0;
	*top = 0;
	*width = screenWidth;
	*height = screenHeight;
}

void ILConsoleSetWindowSize(ILInt32 left, ILInt32 top,
							ILInt32 width, ILInt32 height)
{
	/* We cannot change the window size with termcap */
}

void ILConsoleGetLargestWindowSize(ILInt32 *width, ILInt32 *height)
{
	ILInt32 left, top;
	ILConsoleGetWindowSize(&left, &top, width, height);
}

void ILConsoleSetTitle(const char *title)
{
	if(InitTermcapDriver() && terminalHasTitle)
	{
		fputs("\033]0;", stdout);
		if(title)
		{
			int ch;
			while((ch = (*title++ & 0xFF)) != '\0')
			{
				if(ch >= 0x20)
				{
					putc(ch, stdout);
				}
				else if(ch == '\t')
				{
					putc(' ', stdout);
				}
			}
		}
		putc(0x07, stdout);
		fflush(stdout);
	}
}

ILInt32 ILConsoleGetAttributes(void)
{
	if(InitTermcapDriver())
	{
		return consoleAttrs;
	}
	else
	{
		return 0x07;
	}
}

void ILConsoleSetAttributes(ILInt32 attrs)
{
	if(InitTermcapDriver())
	{
		consoleAttrs = attrs;
		if(terminalHasColor)
		{
			/* Use ANSI escapes to set the color.  If the foreground is gray,
			   then we use the default foreground color.  If the background
			   is black, then we use the default background color.  This
			   avoids problems where the user's preferred color scheme is
			   overridden by poorly written console applications */
			static char const MapAnsiColor[8] =
				{'0', '4', '2', '6', '1', '5', '3', '7'};
			fputs("\033[0", stdout);
			if((attrs & 0x0F) != 0x07)
			{
				if((attrs & 0x08) != 0)
				{
					fputs(";1", stdout);
				}
				fputs(";3", stdout);
				putc(MapAnsiColor[attrs & 0x07], stdout);
			}
			if((attrs & 0xF0) != 0x00)
			{
				fputs(";4", stdout);
				putc(MapAnsiColor[(attrs >> 4) & 0x07], stdout);
			}
			putc('m', stdout);
			fflush(stdout);
		}
		else
		{
			/* Non-color terminal: use the "bold" or "standout" capability */
			if((attrs & 0x08) != 0)
			{
				/* Enable a high intensity mode */
				if(!OutputStringCap("md", "bold"))
				{
					OutputStringCap("so", "smso");
				}
			}
			else
			{
				/* Return to the normal mode */
				if(!OutputStringCap("me", "sgr0"))
				{
					OutputStringCap("se", "rmso");
				}
			}
		}
	}
}

void ILConsoleWriteChar(ILInt32 ch)
{
	ch &= 0xFF;
	if(InitTermcapDriver())
	{
		if(ch == 0x0A)
		{
			/* Translate newline into CRLF */
			putc(0x0D, stdout);
			putc(0x0A, stdout);
			screenX = 0;
			if(screenY < (screenHeight - 1))
			{
				++screenY;
			}
		}
		else if(ch == 0x0D)
		{
			/* Return to the start of the current line */
			putc(0x0D, stdout);
			screenX = 0;
		}
		else if(ch == 0x08)
		{
			/* Backspace and erase one character */
			if(screenX > 0)
			{
				if(!OutputStringCap("bc", 0))
				{
					putc(0x08, stdout);
				}
				--screenX;
			}
			else if(screenY > 0)
			{
				screenX = screenWidth - 1;
				--screenY;
				if(GetFlagCap("bw", "bw"))
				{
					/* The terminal will wrap to the previous line for us */
					if(!OutputStringCap("bc", 0))
					{
						putc(0x08, stdout);
					}
				}
				else
				{
					/* Simulate a wrap back to the previous line */
					ILConsoleSetPosition(screenX, screenY);
				}
			}
		}
		else if(ch == 0x09)
		{
			/* Move to the next TAB stop (uniformly spaced at 8 characters) */
			do
			{
				ILConsoleWriteChar(0x20);
			}
			while((screenX % 8) != 0);
		}
		else if(ch != 0x00 && ch != 0x1B)
		{
			/* Output a regular character */
			putc(ch, stdout);
			++screenX;
			if(screenX >= screenWidth)
			{
				if(!GetFlagCap("am", "am"))
				{
					/* The cursor doesn't automatically wrap, so simulate */
					putc(0x0D, stdout);
					putc(0x0A, stdout);
				}
				screenX = 0;
				if(screenY < (screenHeight - 1))
				{
					++screenY;
				}
			}
		}
	}
	else
	{
		putc(ch, stdout);
	}
}

void ILConsoleMoveBufferArea(ILInt32 sourceLeft, ILInt32 sourceTop,
							 ILInt32 sourceWidth, ILInt32 sourceHeight,
							 ILInt32 targetLeft, ILInt32 targetTop,
							 ILUInt16 sourceChar, ILInt32 attributes)
{
	/* TODO */
}

ILInt32 ILConsoleGetLockState(void)
{
	/* We cannot detect the lock state on this platform, so we just
	   assume that neither CAPS nor NUM has been depressed */
	return 0;
}

ILInt32 ILConsoleGetCursorSize(void)
{
	InitTermcapDriver();
	return cursorSize;
}

void ILConsoleSetCursorSize(ILInt32 size)
{
	if(InitTermcapDriver())
	{
		cursorSize = size;
		if(cursorVisible && size >= 50)
			OutputStringCap("vs", "cvvis");
		else if(cursorVisible)
			OutputStringCap("ve", "cnorm");
		else
			OutputStringCap("vi", "civis");
	}
}

ILBool ILConsoleGetCursorVisible(void)
{
	InitTermcapDriver();
	return cursorVisible;
}

void ILConsoleSetCursorVisible(ILBool flag)
{
	if(InitTermcapDriver())
	{
		cursorVisible = (flag != 0);
		if(flag && cursorSize >= 50)
			OutputStringCap("vs", "cvvis");
		else if(flag)
			OutputStringCap("ve", "cnorm");
		else
			OutputStringCap("vi", "civis");
	}
}

#elif defined(IL_WIN32_PLATFORM)

/*
 * State variables for the Win32 console routines.
 */
static int consoleMode = IL_CONSOLE_NORMAL;
static HANDLE consoleInput = INVALID_HANDLE_VALUE;
static HANDLE consoleOutput = INVALID_HANDLE_VALUE;
static DWORD inputState = 0;
static DWORD outputState = 0;
static WORD attributes = 0;

/*
 * Determine if stdin and stdout are connected to the console.
 */
static int IsUsingConsole(void)
{
	HANDLE stdinHandle = GetStdHandle(STD_INPUT_HANDLE);
	HANDLE stdoutHandle = GetStdHandle(STD_OUTPUT_HANDLE);
	DWORD mode;
	if(stdinHandle == INVALID_HANDLE_VALUE ||
	   !GetConsoleMode(stdinHandle, &mode))
	{
		return 0;
	}
	if(stdoutHandle == INVALID_HANDLE_VALUE ||
	   !GetConsoleMode(stdoutHandle, &mode))
	{
		return 0;
	}
	return 1;
}

void ILConsoleSetMode(ILInt32 mode)
{
	if(mode == IL_CONSOLE_NORMAL || !IsUsingConsole())
	{
		/* Close the console input and output handles */
		if(consoleInput != INVALID_HANDLE_VALUE)
		{
			SetConsoleMode(consoleInput, inputState);
			consoleInput = INVALID_HANDLE_VALUE;
		}
		if(consoleOutput != INVALID_HANDLE_VALUE)
		{
			SetConsoleMode(consoleOutput, outputState);
			consoleOutput = INVALID_HANDLE_VALUE;
		}
	}
	else
	{
		/* Open the console input and output sides */
		if(consoleInput == INVALID_HANDLE_VALUE)
		{
			consoleInput = GetStdHandle(STD_INPUT_HANDLE);
			inputState = 0;
			GetConsoleMode(consoleInput, &inputState);
			SetConsoleMode(consoleInput, ENABLE_WINDOW_INPUT);
		}
		if(consoleOutput == INVALID_HANDLE_VALUE)
		{
			consoleOutput = GetStdHandle(STD_OUTPUT_HANDLE);
			outputState = 0;
			GetConsoleMode(consoleOutput, &outputState);
			SetConsoleMode(consoleOutput, ENABLE_PROCESSED_OUTPUT |
										  ENABLE_WRAP_AT_EOL_OUTPUT);
			attributes = 0x07;
			SetConsoleTextAttribute(consoleOutput, attributes);
		}
	}
	consoleMode = mode;
}

ILInt32 ILConsoleGetMode(void)
{
	return consoleMode;
}

void ILConsoleBeep(ILInt32 frequency, ILInt32 duration)
{
	if(consoleOutput != INVALID_HANDLE_VALUE)
	{
		unsigned char buf[1] = {(unsigned char)0x07};
		DWORD numWritten;
		WriteConsole(consoleOutput, (LPVOID)buf, 1, &numWritten, NULL);
	}
}

void ILConsoleClear(void)
{
	if(consoleOutput != INVALID_HANDLE_VALUE)
	{
		CONSOLE_SCREEN_BUFFER_INFO info;
		SMALL_RECT bounds;
		COORD scrollTo;
		CHAR_INFO charInfo;
		if(GetConsoleScreenBufferInfo(consoleOutput, &info))
		{
			bounds = info.srWindow;
			scrollTo.X = 0;
			scrollTo.Y = (SHORT)(-(bounds.Bottom - bounds.Top + 1));
			charInfo.Char.AsciiChar = ' ';
			charInfo.Attributes = attributes;
			ScrollConsoleScreenBuffer
				(consoleOutput, &bounds, NULL, scrollTo, &charInfo);
			scrollTo.X = info.srWindow.Left;
			scrollTo.Y = info.srWindow.Top;
			SetConsoleCursorPosition(consoleOutput, scrollTo);
		}
	}
}

int ILConsoleKeyAvailable(void)
{
	if(consoleInput != INVALID_HANDLE_VALUE)
	{
		DWORD numPending = 0;
		DWORD numRead = 0;
		PINPUT_RECORD records;
		if(GetNumberOfConsoleInputEvents(consoleInput, &numPending) &&
		   numPending > 0)
		{
			/* Look for key down and window size events in the input queue */
			records = (PINPUT_RECORD)
				alloca(sizeof(INPUT_RECORD) * numPending);
			if(PeekConsoleInput(consoleInput, records,
								numPending, &numRead) &&
			   numRead <= numPending)
			{
				while(numRead > 0)
				{
					if(records->EventType == KEY_EVENT &&
					   records->Event.KeyEvent.bKeyDown)
					{
						return 1;
					}
					if(records->EventType == WINDOW_BUFFER_SIZE_EVENT)
					{
						return 1;
					}
					++records;
					--numRead;
				}
			}
		}
	}
	return 0;
}

void ILConsoleReadKey(ILUInt16 *ch, ILInt32 *key, ILInt32 *modifiers)
{
	if(consoleInput != INVALID_HANDLE_VALUE)
	{
		INPUT_RECORD record;
		DWORD numRead;
		DWORD mods;
		while(ReadConsoleInput(consoleInput, &record, 1, &numRead) &&
			  numRead == 1)
		{
			if(record.EventType == KEY_EVENT &&
			   record.Event.KeyEvent.bKeyDown)
			{
				/* Convert the keydown event into what the application needs */
				*ch = (ILUInt16)(record.Event.KeyEvent.uChar.AsciiChar);
				*key = (ILInt32)(record.Event.KeyEvent.wVirtualKeyCode);
				mods = record.Event.KeyEvent.dwControlKeyState;
				*modifiers = 0;
				if((mods & SHIFT_PRESSED) != 0)
				{
					*modifiers |= Mod_Shift;
				}
				if((mods & (LEFT_CTRL_PRESSED | RIGHT_CTRL_PRESSED)) != 0)
				{
					*modifiers |= Mod_Control;
				}
				if((mods & (LEFT_ALT_PRESSED | RIGHT_ALT_PRESSED)) != 0)
				{
					*modifiers |= Mod_Alt;
				}

				/* Check for CTRL-C and CTRL-BREAK in the input queue */
				if(*key == Key_C && *modifiers == Mod_Control)
				{
					if(consoleMode == IL_CONSOLE_CBREAK ||
					   consoleMode == IL_CONSOLE_CBREAK_ALT)
					{
						*ch = 0;
						*key = Key_Interrupt;
						*modifiers = 0;
					}
				}
				else if(*key == Key_Pause && *modifiers == Mod_Control)
				{
					if(consoleMode == IL_CONSOLE_CBREAK ||
					   consoleMode == IL_CONSOLE_CBREAK_ALT)
					{
						*ch = 0;
						*key = Key_CtrlBreak;
						*modifiers = 0;
					}
				}
				return;
			}
			else if(record.EventType == WINDOW_BUFFER_SIZE_EVENT)
			{
				/* Tell the application that the window size has changed */
				*ch = 0;
				*key = Key_SizeChanged;
				*modifiers = 0;
				return;
			}
		}
		*ch = 0;
		*key = 0;
		*modifiers = 0;
	}
	else
	{
		int c = (getc(stdin) & 0xFF);
		*ch = (ILUInt16)c;
		*key = 0;
		*modifiers = 0;
	}
}

void ILConsoleSetPosition(ILInt32 x, ILInt32 y)
{
	if(consoleOutput != INVALID_HANDLE_VALUE)
	{
		COORD posn;
		posn.X = (SHORT)x;
		posn.Y = (SHORT)y;
		SetConsoleCursorPosition(consoleOutput, posn);
	}
}

void ILConsoleGetPosition(ILInt32 *x, ILInt32 *y)
{
	if(consoleOutput != INVALID_HANDLE_VALUE)
	{
		CONSOLE_SCREEN_BUFFER_INFO info;
		if(GetConsoleScreenBufferInfo(consoleOutput, &info))
		{
			*x = (ILInt32)(info.dwCursorPosition.X);
			*y = (ILInt32)(info.dwCursorPosition.Y);
			return;
		}
	}
	*x = 0;
	*y = 0;
}

void ILConsoleGetBufferSize(ILInt32 *width, ILInt32 *height)
{
	if(consoleOutput != INVALID_HANDLE_VALUE)
	{
		CONSOLE_SCREEN_BUFFER_INFO info;
		if(GetConsoleScreenBufferInfo(consoleOutput, &info))
		{
			*width = (ILInt32)(info.dwSize.X);
			*height = (ILInt32)(info.dwSize.Y);
			return;
		}
	}
	*width = 80;
	*height = 25;
}

void ILConsoleSetBufferSize(ILInt32 width, ILInt32 height)
{
	/* TODO */
}

void ILConsoleGetWindowSize(ILInt32 *left, ILInt32 *top,
							ILInt32 *width, ILInt32 *height)
{
	if(consoleOutput != INVALID_HANDLE_VALUE)
	{
		CONSOLE_SCREEN_BUFFER_INFO info;
		if(GetConsoleScreenBufferInfo(consoleOutput, &info))
		{
			*left = (ILInt32)(info.srWindow.Left);
			*top = (ILInt32)(info.srWindow.Top);
			*width = (ILInt32)(info.srWindow.Right - info.srWindow.Left + 1);
			*height = (ILInt32)(info.srWindow.Bottom - info.srWindow.Top + 1);
			return;
		}
	}
	*left = 0;
	*top = 0;
	*width = 80;
	*height = 25;
}

void ILConsoleSetWindowSize(ILInt32 left, ILInt32 top,
							ILInt32 width, ILInt32 height)
{
	/* TODO */
}

void ILConsoleGetLargestWindowSize(ILInt32 *width, ILInt32 *height)
{
	/* TODO */
	ILInt32 left, top;
	ILConsoleGetWindowSize(&left, &top, width, height);
}

void ILConsoleSetTitle(const char *title)
{
	if(title)
	{
		SetConsoleTitle(title);
	}
	else
	{
		SetConsoleTitle("");
	}
}

ILInt32 ILConsoleGetAttributes(void)
{
	/* TODO */
	return 0x07;
}

void ILConsoleSetAttributes(ILInt32 attrs)
{
	if(consoleOutput != INVALID_HANDLE_VALUE)
	{
		SetConsoleTextAttribute(consoleOutput, (attrs & 0xFF));
		attributes = (WORD)(attrs & 0xFF);
	}
}

void ILConsoleWriteChar(ILInt32 ch)
{
	if(consoleOutput != INVALID_HANDLE_VALUE)
	{
		unsigned char buf[1] = {(unsigned char)ch};
		DWORD numWritten;
		WriteConsole(consoleOutput, (LPVOID)buf, 1, &numWritten, NULL);
	}
	else
	{
		putc(ch & 0xFF, stdout);
	}
}

void ILConsoleMoveBufferArea(ILInt32 sourceLeft, ILInt32 sourceTop,
							 ILInt32 sourceWidth, ILInt32 sourceHeight,
							 ILInt32 targetLeft, ILInt32 targetTop,
							 ILUInt16 sourceChar, ILInt32 attributes)
{
	/* TODO */
}

ILInt32 ILConsoleGetLockState(void)
{
	/* TODO */
	return 0;
}

ILInt32 ILConsoleGetCursorSize(void)
{
	/* TODO */
	return 25;
}

void ILConsoleSetCursorSize(ILInt32 size)
{
	/* TODO */
}

ILBool ILConsoleGetCursorVisible(void)
{
	/* TODO */
	return 1;
}

void ILConsoleSetCursorVisible(ILBool flag)
{
	/* TODO */
}

#else /* No console */

/*
 * Forward declaration.
 */
static void MapCharToKey(int ch, ILInt32 *key, ILInt32 *modifiers);
#define	IL_NEED_MAPCHAR		1

void ILConsoleSetMode(ILInt32 mode)
{
	/* Nothing to do here */
}

ILInt32 ILConsoleGetMode(void)
{
	return IL_CONSOLE_NORMAL;
}

void ILConsoleBeep(ILInt32 frequency, ILInt32 duration)
{
	/* Nothing to do here */
}

void ILConsoleClear(void)
{
	/* Nothing to do here */
}

int ILConsoleKeyAvailable(void)
{
	/* Nothing to do here */
	return 0;
}

void ILConsoleReadKey(ILUInt16 *ch, ILInt32 *key, ILInt32 *modifiers)
{
	int c = (getc(stdin) & 0xFF);
	*ch = (ILUInt16)c;
	MapCharToKey(c, key, modifiers);
}

void ILConsoleSetPosition(ILInt32 x, ILInt32 y)
{
	/* Nothing to do here */
}

void ILConsoleGetPosition(ILInt32 *x, ILInt32 *y)
{
	*x = 0;
	*y = 0;
}

void ILConsoleGetBufferSize(ILInt32 *width, ILInt32 *height)
{
	*width = 80;
	*height = 24;
}

void ILConsoleSetBufferSize(ILInt32 width, ILInt32 height)
{
	/* Ignored in this version */
}

void ILConsoleGetWindowSize(ILInt32 *left, ILInt32 *top,
							ILInt32 *width, ILInt32 *height)
{
	*left = 0;
	*top = 0;
	*width = 80;
	*height = 24;
}

void ILConsoleSetWindowSize(ILInt32 left, ILInt32 top,
							ILInt32 width, ILInt32 height)
{
	/* Ignored in this version */
}

void ILConsoleGetLargestWindowSize(ILInt32 *width, ILInt32 *height)
{
	ILInt32 left, top;
	ILConsoleGetWindowSize(&left, &top, width, height);
}

void ILConsoleSetTitle(const char *title)
{
	/* Nothing to do here */
}

ILInt32 ILConsoleGetAttributes(void)
{
	return 0x07;
}

void ILConsoleSetAttributes(ILInt32 attrs)
{
	/* Nothing to do here */
}

void ILConsoleWriteChar(ILInt32 ch)
{
	putc(ch & 0xFF, stdout);
}

void ILConsoleMoveBufferArea(ILInt32 sourceLeft, ILInt32 sourceTop,
							 ILInt32 sourceWidth, ILInt32 sourceHeight,
							 ILInt32 targetLeft, ILInt32 targetTop,
							 ILUInt16 sourceChar, ILInt32 attributes)
{
	/* Nothing to do here */
}

ILInt32 ILConsoleGetLockState(void)
{
	/* We cannot detect the lock state on this platform, so we just
	   assume that neither CAPS nor NUM has been depressed */
	return 0;
}

ILInt32 ILConsoleGetCursorSize(void)
{
	return 25;
}

void ILConsoleSetCursorSize(ILInt32 size)
{
	/* Nothing to do here */
}

ILBool ILConsoleGetCursorVisible(void)
{
	return 1;
}

void ILConsoleSetCursorVisible(ILBool flag)
{
	/* Nothing to do here */
}

#endif /* No console */

#ifdef IL_NEED_MAPCHAR

/*
 * Table that maps the 128 ASCII characters to equivalent key specifications.
 * This is based on a standard US-101 keyboard layout mapping.
 */
typedef struct
{
	ConsoleKey			key;
	ConsoleModifiers	modifiers;

} MapKeyInfo;
static MapKeyInfo const MapKeys[128] = {
	{Key_D2,		Mod_Shift | Mod_Control},	/* ^@ */
	{Key_A,			Mod_Control},
	{Key_B,			Mod_Control},
	{Key_C,			Mod_Control},
	{Key_D,			Mod_Control},
	{Key_E,			Mod_Control},
	{Key_F,			Mod_Control},
	{Key_G,			Mod_Control},
	{Key_BackSpace,	0},
	{Key_Tab,		0},
	{Key_J,			Mod_Control},
	{Key_K,			Mod_Control},
	{Key_L,			Mod_Control},
	{Key_Enter,		0},
	{Key_N,			Mod_Control},
	{Key_O,			Mod_Control},
	{Key_P,			Mod_Control},
	{Key_Q,			Mod_Control},
	{Key_R,			Mod_Control},
	{Key_S,			Mod_Control},
	{Key_T,			Mod_Control},
	{Key_U,			Mod_Control},
	{Key_V,			Mod_Control},
	{Key_W,			Mod_Control},
	{Key_X,			Mod_Control},
	{Key_Y,			Mod_Control},
	{Key_Z,			Mod_Control},
	{Key_Escape,	0},
	{Key_Oem5,		Mod_Control},				/* ^\ */
	{Key_Oem6,		Mod_Control},				/* ^] */
	{Key_D6,		Mod_Shift | Mod_Control},	/* ^^ */
	{Key_OemMinus,	Mod_Shift | Mod_Control},	/* ^_ */
	{Key_SpaceBar,	0},
	{Key_D1,		Mod_Shift},					/* ! */
	{Key_Oem7,		Mod_Shift},					/* " */
	{Key_D3,		Mod_Shift},					/* # */
	{Key_D4,		Mod_Shift},					/* $ */
	{Key_D5,		Mod_Shift},					/* % */
	{Key_D7,		Mod_Shift},					/* & */
	{Key_Oem7,		0},							/* ' */
	{Key_D9,		Mod_Shift},					/* ( */
	{Key_D0,		Mod_Shift},					/* ) */
	{Key_D8,		Mod_Shift},					/* * */
	{Key_OemPlus,	Mod_Shift},					/* + */
	{Key_OemComma,	0},							/* , */
	{Key_OemMinus,	0},							/* - */
	{Key_OemPeriod,	0},							/* . */
	{Key_Oem2,		0},							/* / */
	{Key_D0,		0},
	{Key_D1,		0},
	{Key_D2,		0},
	{Key_D3,		0},
	{Key_D4,		0},
	{Key_D5,		0},
	{Key_D6,		0},
	{Key_D7,		0},
	{Key_D8,		0},
	{Key_D9,		0},
	{Key_Oem1,		Mod_Shift},					/* : */
	{Key_Oem1,		0},							/* ; */
	{Key_OemComma,	Mod_Shift},					/* < */
	{Key_OemPlus,	0},							/* = */
	{Key_OemPeriod,	Mod_Shift},					/* > */
	{Key_Oem2,		Mod_Shift},					/* ? */
	{Key_D2,		Mod_Shift},					/* @ */
	{Key_A,			Mod_Shift},
	{Key_B,			Mod_Shift},
	{Key_C,			Mod_Shift},
	{Key_D,			Mod_Shift},
	{Key_E,			Mod_Shift},
	{Key_F,			Mod_Shift},
	{Key_G,			Mod_Shift},
	{Key_H,			Mod_Shift},
	{Key_I,			Mod_Shift},
	{Key_J,			Mod_Shift},
	{Key_K,			Mod_Shift},
	{Key_L,			Mod_Shift},
	{Key_M,			Mod_Shift},
	{Key_N,			Mod_Shift},
	{Key_O,			Mod_Shift},
	{Key_P,			Mod_Shift},
	{Key_Q,			Mod_Shift},
	{Key_R,			Mod_Shift},
	{Key_S,			Mod_Shift},
	{Key_T,			Mod_Shift},
	{Key_U,			Mod_Shift},
	{Key_V,			Mod_Shift},
	{Key_W,			Mod_Shift},
	{Key_X,			Mod_Shift},
	{Key_Y,			Mod_Shift},
	{Key_Z,			Mod_Shift},
	{Key_Oem4,		0},							/* [ */
	{Key_Oem5,		0},							/* \ */
	{Key_Oem6,		0},							/* ] */
	{Key_D6,		Mod_Shift},					/* ^ */
	{Key_OemMinus,	Mod_Shift},					/* _ */
	{Key_Oem3,		0},							/* ` */
	{Key_A,			0},
	{Key_B,			0},
	{Key_C,			0},
	{Key_D,			0},
	{Key_E,			0},
	{Key_F,			0},
	{Key_G,			0},
	{Key_H,			0},
	{Key_I,			0},
	{Key_J,			0},
	{Key_K,			0},
	{Key_L,			0},
	{Key_M,			0},
	{Key_N,			0},
	{Key_O,			0},
	{Key_P,			0},
	{Key_Q,			0},
	{Key_R,			0},
	{Key_S,			0},
	{Key_T,			0},
	{Key_U,			0},
	{Key_V,			0},
	{Key_W,			0},
	{Key_X,			0},
	{Key_Y,			0},
	{Key_Z,			0},
	{Key_Oem4,		Mod_Shift},					/* { */
	{Key_Oem5,		Mod_Shift},					/* | */
	{Key_Oem6,		Mod_Shift},					/* } */
	{Key_Oem3,		Mod_Shift},					/* ~ */
	{Key_Delete,	0},
};

/*
 * Map an ASCII character code to a Windows-like key specification.
 */
static void MapCharToKey(int ch, ILInt32 *key, ILInt32 *modifiers)
{
#ifdef IL_CHECK_ERASE
	/* If erase is set to ^?, then map that to the backspace key
	   instead of the delete key */
	if(ch == 0x7F && eraseIsDel)
	{
		*key = Key_BackSpace;
		*modifiers = 0;
	}
	else
#endif
	if(ch >= 0 && ch < 128)
	{
		*key = (ILInt32)(MapKeys[ch].key);
		*modifiers = (ILInt32)(MapKeys[ch].modifiers);
	}
	else
	{
		*key = 0;
		*modifiers = 0;
	}
}

#endif /* IL_NEED_MAPCHAR */

#ifdef	__cplusplus
};
#endif
