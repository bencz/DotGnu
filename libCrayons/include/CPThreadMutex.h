/*
 * CPThreadMutex.h - Posix thread mutex header.
 *
 * Copyright (C) 2005  Free Software Foundation, Inc.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

#ifndef _C_PTHREADMUTEX_H_
#define _C_PTHREADMUTEX_H_

#include "CrayonsInternal.h"
#include <pthread.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct _tagCMutex CMutex;
struct _tagCMutex
{
	pthread_mutex_t mutex;
};

#define CMutex_StaticInitializer { PTHREAD_MUTEX_INITIALIZER }

CINTERNAL void
CMutex_Lock(CMutex *_this);
CINTERNAL void
CMutex_Unlock(CMutex *_this);
CINTERNAL CStatus
CMutex_Create(CMutex **_this);
CINTERNAL void
CMutex_Destroy(CMutex **_this);

#ifdef __cplusplus
};
#endif

#endif /* _C_PTHREADMUTEX_H_ */
