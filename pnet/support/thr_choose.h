/*
 * thr_choose.h - Choose the thread package to use.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

#ifndef	_THR_CHOOSE_H
#define	_THR_CHOOSE_H

#include "il_gc.h"
#include "il_config.h"
#include "il_thread.h"

/*
 * An easy way to turn off threads completely for testing.
 */
/*#define IL_NO_THREADS*/

/*
 * Determine if we can use pthreads.  Right now, we only do this
 * for Linux, but we will extend it to other systems later.
 */
#if !defined(IL_NO_THREADS)
#if defined(linux) || defined(__linux) || defined(__linux__) || \
    defined(__FreeBSD__) || defined(__OpenBSD__) || defined(__sun) || \
	defined(__NetBSD__) || defined(__APPLE__)
#if defined(GC_LINUX_THREADS) || defined(GC_FREEBSD_THREADS) || \
    defined(GC_OPENBSD_THREADS) || defined(GC_SOLARIS_THREADS) || \
	defined(GC_NETBSD_THREADS) || defined(GC_DARWIN_THREADS)
#define	IL_USE_PTHREADS
#endif
#endif
#endif 

/*
 * Determine if we can use Win32 threads.
 */
#if !defined(IL_NO_THREADS)
#if defined(WIN32) || defined(_WIN32) || defined(__CYGWIN__)
#if defined(GC_WIN32_THREADS)
#define	IL_USE_WIN32_THREADS
#endif
#endif
#endif

/*
 * Determine if we can use BeOS threads.
 */
#if !defined(IL_NO_THREADS)
#if defined(BEOS) || defined(_BEOS)
#if defined(GC_BEOS_THREADS)
#define IL_USE_BEOS_THREADS
#endif
#endif
#endif

/*
 * If we don't know what thread package to use, then turn them all off.
 */
#if !defined(IL_USE_PTHREADS) && !defined(IL_USE_WIN32_THREADS)
#ifndef IL_NO_THREADS
#define	IL_NO_THREADS
#endif
#endif

#ifndef IL_NO_THREADS

#if defined(IL_USE_PTHREADS)
#include <pthread.h>
#include <signal.h>
#elif defined(IL_USE_WIN32_THREADS)
#include <windows.h>
#elif defined(IL_USE_BEOS_THREADS)
#include <OS.h>
#include <TLS.h>
#endif

#ifdef HAVE_LIBGC

#include <gc.h>

/* Make sure that CreateThread is redirected under all Win32 environments */
#if defined(IL_USE_WIN32_THREADS)
#define CreateThread GC_CreateThread
#endif

#endif	/* !HAVE_LIBGC */

#endif	/* !IL_NO_THREADS */

#endif	/* _THR_CHOOSE_H */

