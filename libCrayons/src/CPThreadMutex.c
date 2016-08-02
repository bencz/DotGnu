/*
 * CPThreadMutex.c - Posix thread mutex implementation.
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

#include "CPThreadMutex.h"

#ifdef __cplusplus
extern "C" {
#endif

CINTERNAL void
CMutex_Lock(CMutex *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* lock the mutex */
	pthread_mutex_lock(&(_this->mutex));
}

CINTERNAL void
CMutex_Unlock(CMutex *_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* unlock the mutex */
	pthread_mutex_unlock(&(_this->mutex));
}

CINTERNAL CStatus
CMutex_Create(CMutex **_this)
{
	/* assertions */
	CASSERT((_this != 0));

	/* allocate the mutex */
	if(!(*_this = (CMutex *)CMalloc(sizeof(CMutex))))
	{
		return CStatus_OutOfMemory;
	}

	/* initialize the mutex */
	pthread_mutex_init(&((*_this)->mutex), 0);

	/* return successfully */
	return CStatus_OK;
}

CINTERNAL void
CMutex_Destroy(CMutex **_this)
{
	/* assertions */
	CASSERT((_this != 0));
	CASSERT((*_this != 0));

	/* finalize the mutex */
	pthread_mutex_destroy(&((*_this)->mutex));

	/* dispose of the mutex */
	CFree(*_this);

	/* null the this pointer */
	*_this = 0;
}


#ifdef __cplusplus
};
#endif
