/*
 * guid.c - Generate GUID values.
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

#include "il_system.h"
#include "il_utils.h"
#include "il_crypt.h"
#if defined(__palmos__)
#include <PalmTypes.h>
#include <TimeMgr.h>
#elif TIME_WITH_SYS_TIME
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
#ifdef HAVE_SYS_STAT_H
#include <sys/stat.h>
#endif
#ifdef HAVE_UNISTD_H
#include <unistd.h>
#endif
#ifdef HAVE_FCNTL_H
#include <fcntl.h>
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

void ILGUIDGenerate(unsigned char *guid)
{
#ifdef HAVE_OPEN
	int fd;
#endif
#ifdef HAVE_GETTIMEOFDAY
	struct timeval tv;
#elif defined(__palmos__)
	UInt32 timestamp;
#else
	time_t timestamp;
#endif
	int entropySize;
	int randomSize;
#ifdef HAVE_PID
	int pid;
#endif
	ILSHAContext sha;
	unsigned char hash[IL_SHA_HASH_SIZE];

	/* Clear the buffer before we start */
	ILMemZero(guid, 16);
	randomSize = 0;

#ifdef HAVE_OPEN
	/* Attempt to open the "/dev/urandom" device, which exists
	   on GNU/Linux systems and provides good random number values.
	   Note: /dev/random produces much better values, but it
	   can block sometimes while it collects super-random
	   entropy values.  We don't quite need that level of
	   randomness for GUID's, so we use /dev/urandom instead.
	   We further randomise the data using the re-distribution
	   code below so that it is difficult for an attacker to
	   get back from a GUID to the entropy values in the kernel */
	fd = open("/dev/urandom", O_RDONLY, 0);
	if(fd >= 0)
	{
		read(fd, guid, 16);
		close(fd);
		randomSize = 16;
	}
#endif

	/* Add the current system time and process ID to the GUID buffer */
#ifdef HAVE_GETTIMEOFDAY
	gettimeofday(&tv, 0);
	guid[0] = (unsigned char)(tv.tv_sec >> 24);
	guid[1] = (unsigned char)(tv.tv_sec >> 16);
	guid[2] = (unsigned char)(tv.tv_sec >> 8);
	guid[3] = (unsigned char)(tv.tv_sec);
	guid[4] = (unsigned char)(tv.tv_usec >> 24);
	guid[5] = (unsigned char)(tv.tv_usec >> 16);
	guid[6] = (unsigned char)(tv.tv_usec >> 8);
	guid[7] = (unsigned char)(tv.tv_usec);
	entropySize = 8;
#else
#if defined(__palmos__)
	timestamp = TimGetSeconds();
#else
	timestamp = time(0);
#endif
	guid[0] = (unsigned char)(timestamp >> 24);
	guid[1] = (unsigned char)(timestamp >> 16);
	guid[2] = (unsigned char)(timestamp >> 8);
	guid[3] = (unsigned char)(timestamp);
	entropySize = 4;
#endif
#ifdef HAVE_PID
	pid = getpid();
	guid[entropySize++] = (unsigned char)(pid >> 8);
	guid[entropySize++] = (unsigned char)(pid);
#endif
	if(entropySize < randomSize)
	{
		entropySize = randomSize;
	}

	/* Try to re-distribute the entropy a little bit by hashing
	   the contents of the seed with SHA1 four times.  This isn't
	   intended to be secure cryptographically.  It is used solely
	   to spread the entropy in the bits across the entire buffer */
	ILSHAInit(&sha);
	ILSHAData(&sha, guid, entropySize);
	ILSHAFinalize(&sha, hash);
	ILSHAInit(&sha);
	ILSHAData(&sha, guid, entropySize);
	ILSHAData(&sha, hash, IL_SHA_HASH_SIZE);
	ILSHAFinalize(&sha, hash);
	ILSHAInit(&sha);
	ILSHAData(&sha, guid, entropySize);
	ILSHAData(&sha, hash, IL_SHA_HASH_SIZE);
	ILSHAFinalize(&sha, hash);
	ILSHAInit(&sha);
	ILSHAData(&sha, guid, entropySize);
	ILSHAData(&sha, hash, IL_SHA_HASH_SIZE);
	ILSHAFinalize(&sha, hash);
	ILMemCpy(guid, hash, 16);
}

#ifdef	__cplusplus
};
#endif
