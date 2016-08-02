/*
 * lib_info.c - Internalcall methods for the "Platform.InfoMethods" class.
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
#include "il_sysio.h"
#if HAVE_SYS_TYPES_H
#include <sys/types.h>
#endif
#if HAVE_UNISTD_H
#include <unistd.h>
#endif
#if HAVE_PWD_H
#include <pwd.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * public static String GetRuntimeVersion();
 */
ILString *_IL_InfoMethods_GetRuntimeVersion(ILExecThread *thread)
{
	return ILStringCreate(thread, VERSION);
}

/*
 * public static String GetNetBIOSMachineName();
 */
ILString *_IL_InfoMethods_GetNetBIOSMachineName(ILExecThread *thread)
{
	char *env;
	env = getenv("COMPUTERNAME");
	if(env && *env != '\0')
	{
		return ILStringCreate(thread, env);
	}
#ifdef IL_CONFIG_NETWORKING
	return _IL_DnsMethods_InternalGetHostName(thread);
#else
	ILExecThreadThrowSystem(thread, "System.NotSupportedException", 
					"Exception_ThreadsNotSupported");
	return 0;
#endif /* IL_CONFIG_NETWORKING */
}

/*
 * public static PlatformID GetPlatformID();
 */
ILInt32 _IL_InfoMethods_GetPlatformID(ILExecThread *thread)
{
#ifdef IL_WIN32_PLATFORM
	return 2;			/* PlatformID.Win32NT */
#else
	return 4;			/* PlatformID.Unix */
#endif
}

/*
 * public static String GetPlatformName();
 */
ILString *_IL_InfoMethods_GetPlatformName(ILExecThread *thread)
{
	return ILStringCreate(thread, ILGetPlatformName());
}

/*
 * public static String GetUserDomainName();
 */
ILString *_IL_InfoMethods_GetUserDomainName(ILExecThread *thread)
{
	/* TODO */
	return _IL_InfoMethods_GetNetBIOSMachineName(thread);
}

/*
 * public static bool IsUserInteractive();
 */
ILBool _IL_InfoMethods_IsUserInteractive(ILExecThread * _thread)
{
#if HAVE_ISATTY
	return (isatty(0) && isatty(1));
#else
	return 0;
#endif
}

/*
 * public static String GetUserName();
 */
ILString *_IL_InfoMethods_GetUserName(ILExecThread *thread)
{
#if !defined(__palmos__)
	if(!ILImageIsSecure(_ILClrCallerImage(thread)))
	{
		/* We don't trust the caller, so don't tell them who the user is */
		return ILStringCreate(thread, "nobody");
	}
	else
	{
		char *env;
#if HAVE_GETPWUID && HAVE_GETEUID
		struct passwd *pwd = getpwuid(geteuid());
		if(pwd != NULL)
		{
			return ILStringCreate(thread, pwd->pw_name);
		}
#endif
		env = getenv("USER");
		if(env && *env != '\0')
		{
			return ILStringCreate(thread, env);
		}
		return ILStringCreate(thread, "nobody");
	}
#else
	return ILStringCreate(thread, "nobody");
#endif
}

/*
 * public static long GetWorkingSet();
 */
ILInt64 _IL_InfoMethods_GetWorkingSet(ILExecThread *thread)
{
	/* There is no reliable way to determine the working set */
	return 0;
}

/*
 * public static int GetProcessorCount();
 */
ILInt32 _IL_InfoMethods_GetProcessorCount(ILExecThread *thread)
{
	/* Simulate a uniprocessor machine */
	return 1;
}

/*
 * public static String GetGlobalConfigDir();
 */
extern ILString *_IL_InfoMethods_GetGlobalConfigDir(ILExecThread *_thread)
{
#if !defined(__palmos__)
	char *env;
	ILString *str;

	/* Try the "CLI_MACHINE_CONFIG_DIR" environment variable first */
	if((env = getenv("CLI_MACHINE_CONFIG_DIR")) != 0 && *env != '\0')
	{
		return ILStringCreate(_thread, env);
	}

	/* Return a standard path such as "/usr/local/share/cscc/config" */
	env = ILGetStandardDataPath("cscc/config");
	if(!env)
	{
		ILExecThreadThrowOutOfMemory(_thread);
		return 0;
	}
	str = ILStringCreate(_thread, env);
	ILFree(env);
	return str;
#else
	return 0;
#endif
}

/*
 * public static String GetUserStorageDir();
 */
extern ILString *_IL_InfoMethods_GetUserStorageDir(ILExecThread *_thread)
{
#if !defined(__palmos__)
	char *env;
	char *full;
	ILString *str;

	/* Try the "CLI_STORAGE_ROOT" environment variable first */
	if((env = getenv("CLI_STORAGE_ROOT")) != 0 && *env != '\0')
	{
		return ILStringCreate(_thread, env);
	}

	/* Use "$HOME/.cli" instead */
	env = getenv("HOME");
	if(env && *env != '\0' && ILGetFileType(env) == ILFileType_DIR)
	{
		full = (char *)ILMalloc(strlen(env) + 6);
		if(!full)
		{
			ILExecThreadThrowOutOfMemory(_thread);
			return 0;
		}
		strcpy(full, env);
	#ifdef IL_WIN32_NATIVE
		strcat(full, "\\.cli");
	#else
		strcat(full, "/.cli");
	#endif
		str = ILStringCreate(_thread, full);
		ILFree(full);
		return str;
	}

	/* We don't know how to get the user storage directory */
	return 0;
#else
	return 0;
#endif
}

#ifdef	__cplusplus
};
#endif
