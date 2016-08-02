
/*
 * thr_defs.h - Common thread definitions.
 *
 * Copyright (C) 2002, 2009  Southern Storm Software, Pty Ltd.
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

#ifndef	_THR_DEFS_H
#define	_THR_DEFS_H

/*
 * Define IL_THREAD_ASSERTIONS for enabling threading assertions.
 */
/* #define IL_THREAD_ASSERTIONS */

/*
 * Define IL_THREAD_DEBUG for enabling threading debug features.
 */
/* #define IL_THREAD_DEBUG */

/*
 * Choose which thread package we will be using.
 */
#include "thr_choose.h"
#include "il_utils.h"
#include "interrupt.h"
#if defined(IL_THREAD_ASSERTIONS) || defined(IL_THREAD_DEBUG)
#include <stdio.h>
#include <stdlib.h>
#endif

#ifdef IL_THREAD_ASSERTIONS
#define IL_THREAD_ASSERT(expr) \
	if(!(expr)) \
	{ \
		fprintf(stderr, "Threading assertion failure at %s:%ld\n", \
				__FILE__, (unsigned long)__LINE__); \
		abort(); \
	}
#else
#define IL_THREAD_ASSERT(expr)
#endif
/*
 * Include the package-specific definitions.
 */
#ifdef IL_USE_PTHREADS
#include "pt_defs.h"
#endif
#ifdef IL_USE_WIN32_THREADS
#include "w32_defs.h"
#endif
#ifdef IL_NO_THREADS
#include "no_defs.h"
#endif

#ifdef	__cplusplus
extern	"C" {
#endif

/*
 * Monitor support with the whole .NET monitor semantics.
 */
typedef struct _tagILMonitor ILMonitor;


typedef struct __tagILWaitMutex ILWaitMutex;

/*
 * Internal structure of a wakeup object.
 */
typedef struct _tagILWakeup
{
	_ILCondMutex			lock;
	_ILCondVar				condition;	
	ILUInt32	volatile	count;
	ILUInt32	volatile	limit;
	int			volatile	interrupted;
	void *		volatile    object;
	int						ownedMutexesCount;
	int						ownedMutexesCapacity;
	ILWaitMutex				**ownedMutexes;
} _ILWakeup;

/*
 * Internal structure of a wakeup queue item.
 */
typedef struct _tagILWakeupItem _ILWakeupItem;
struct _tagILWakeupItem
{
	_ILWakeupItem * volatile next;
	_ILWakeup	  * volatile wakeup;
	void		  * volatile object;

};

/*
 * Internal structure of a wakeup queue.
 */
typedef struct _tagILWakeupQueue
{
	_ILWakeupItem * volatile first;
	_ILWakeupItem * volatile last;
	_ILWakeupItem			 space;
	int				volatile spaceUsed;

} _ILWakeupQueue;

/*
 *	Supports linked lists of cleanup functions.
 */
typedef struct _tagILThreadCleanupEntry ILThreadCleanupEntry;

struct _tagILThreadCleanupEntry
{
	ILThreadCleanupFunc cleanup;
	ILThreadCleanupEntry *next;
};

/*
 * NOTE: For modifying the thread's state public modifiable state:
 * 1. lock the thread
 * 2. load the thread's state to a local variable
 * 3. do what you want to do with the thread's state with the local variable
 * 4. store the local variable to the thread's state
 * 5. unlock the thread
 *
 * This is done this way to be able to query the current thread's state
 * without having to lock the thread.
 * The reader will read the consistent state before the thread was locked
 * until all changes are done.
 * Additionally the compiler is able to hold the thread's state in a
 * register instead of having to read and write the volatile state multiple
 * times.
 */

/*
 * Helper structure for the thread's state.
 * The idea behind this is to divide the state into two parts where
 * the priv part is modified only by the thread the ILThread is belonging to.
 * The pub part may be modified by any other thread.
 * So the priv part can be modified without having to lock the thread.
 * The pub part must be changed only with the thread's lock held (see NOTE).
 *
 * Flags in the priv member:
 * IL_TS_RUNNING
 * IL_TS_STOPPED
 * IL_TS_WAIT_SLEEP_JOIN
 * IL_TS_ABORTED
 *
 * Flags in the pub member:
 * IL_TS_STOP_REQUESTED
 * IL_TS_SUSPEND_REQUESTED
 * IL_TS_BACKGROUND
 * IL_TS_UNSTARTED
 * IL_TS_SUSPENDED
 * IL_TS_ABORT_REQUESTED
 * internal flags
 * IL_TS_SUSPENDED_SELF
 *
 * Special:
 * The IL_TS_INTERRUPTED flag might be present in both parts
 * for now until the waithandle stuff is redone.
 */
struct _tagILThreadSplitState
{
	ILUInt16	priv;
	ILUInt16	pub;
};

typedef union _tagILThreadState _ILThreadState;
union _tagILThreadState
{
	ILUInt32						comb;
	struct _tagILThreadSplitState	split;
};

/*
 * Internal structure of a thread descriptor.
 */
struct _tagILThread
{
	_ILCriticalSection				lock;
	_ILThreadHandle		volatile	handle;
#ifdef IL_THREAD_NEED_IDENTIFIER
	/*
	 * Implementation specific native identification of the thread.
	 */
	_ILThreadIdentifier	volatile	identifier;
#endif
	_ILThreadState					state;
	ILInt32							useCount;
	unsigned char		volatile	resumeRequested;
	unsigned char		volatile	suspendRequested;
	unsigned int		volatile	numLocksHeld;
	_ILSemaphore					resumeAck;
	_ILSemaphore					suspendAck;
	ILThreadStartFunc 	volatile	startFunc;
	void *				volatile	startArg;
	void *				volatile	userObject;
	_ILWakeup						wakeup;
	_ILWakeupQueue					joinQueue;
	ILThreadCleanupEntry			*firstCleanupEntry;
	ILThreadCleanupEntry			*lastCleanupEntry;
	ILWaitHandle					*monitor;
	ILMonitor						*monitorFreeList;
	ILUInt32						monitorFreeCount;
	/* 1 if the gc knows the thread and is allowed to execute managed code */
#if defined(IL_INTERRUPT_SUPPORTS)
	ILInterruptHandler				interruptHandler;
#endif
	/* Add the implementation specific thread members */
#ifdef _IL_THREAD_EXT
	_IL_THREAD_EXT
#endif
};

/*
 * Extra thread state flags and masks that are used internally.
 */
#define	IL_TS_PUBLIC_FLAGS		0x01FF
#define	IL_TS_SUSPENDED_SELF	0x0200
#define	IL_TS_INTERRUPTED		0x0400
#define IL_TS_INTERRUPTED_OR_ABORT_REQUESTED \
		(IL_TS_INTERRUPTED | IL_TS_ABORT_REQUESTED)

/*
 * Wait handle kinds.
 */
#define	IL_WAIT_EVENT			0x0800
#define	IL_WAIT_MUTEX			0x1000
#define	IL_WAIT_NAMED_MUTEX		0x1001
#define	IL_WAIT_MONITOR			0x1002

/*
 * Close function for a wait handle.
 */
typedef int (*ILWaitCloseFunc)(ILWaitHandle *handle);

/*
 * Return values for "ILWaitCloseFunc".
 */
#define	IL_WAITCLOSE_OWNED		0
#define	IL_WAITCLOSE_FREE		1
#define	IL_WAITCLOSE_DONT_FREE	2

/*
 * Register a wakeup object with a wait handle, so it can be
 * woken at some future point.
 */
typedef int (*ILWaitRegisterFunc)(ILWaitHandle *handle, _ILWakeup *wakeup);

/*
 * Return values for "ILWaitRegisterFunc".
 */
#define	IL_WAITREG_FAILED		0
#define	IL_WAITREG_OK			1
#define	IL_WAITREG_ACQUIRED		2

/*
 * Unregister a wakeup object from a wait handle.  If "release"
 * is non-zero, then release the handle if the thread acquired it.
 * This may be called even if the wakeup object is not registered.
 * In that case, do a release only.
 */
typedef void (*ILWaitUnregisterFunc)(ILWaitHandle *handle, _ILWakeup *wakeup,
									 int release);

typedef int (*ILWaitSignalFunc)(ILWaitHandle *handle);

typedef struct _tagILWaitHandleVtable _ILWaitHandleVtable;
struct _tagILWaitHandleVtable
{
	const int					kind;
	const ILWaitCloseFunc		closeFunc;
	const ILWaitRegisterFunc	registerFunc;
	const ILWaitUnregisterFunc	unregisterFunc;
	const ILWaitSignalFunc		signalFunc;
};

/*
 * Internal structure of a wait handle.
 */
struct _tagILWaitHandle
{
	const _ILWaitHandleVtable  *vtable;
	_ILCriticalSection			lock;
};

/*
 * Some macros for accessing the waithandle members
 */
#define _ILWaitHandle_kind(handle)				((handle)->vtable->kind)
#define _ILWaitHandle_closeFunc(handle)			((handle)->vtable->closeFunc)
#define _ILWaitHandle_registerFunc(handle)		((handle)->vtable->registerFunc)
#define _ILWaitHandle_unregisterFunc(handle)	((handle)->vtable->unregisterFunc)
#define _ILWaitHandle_signalFunc(handle)		((handle)->vtable->signalFunc)

/*
 * Internal structure of a wait event handle.
 */
typedef struct
{
	ILWaitHandle			parent;	
	/* Bit 0 = Set Flag */
	/* Bit 1 = ManualReset Flag */
	int volatile			data;
	_ILWakeupQueue			queue;

} ILWaitEvent;

#define EVENT_SET_MASK (1)
#define EVENT_MANUALRESET_MASK (2)

/*
 * Internal structure of a wait mutex handle.
 */
struct __tagILWaitMutex
{
	ILWaitHandle			parent;
	_ILWakeup *   volatile	owner;
	unsigned long volatile	count;
	unsigned int volatile	options;
	_ILWakeupQueue			queue;

};

/*
 * Internal structure of a named wait mutex handle.
 */
typedef struct
{
	ILWaitMutex				parent;
	unsigned long volatile	numUsers;

} ILWaitMutexNamed;

/*
 * Internal structure of a wait monitor, which extends a wait
 * mutex with Wait/Pulse/PulseAll semantics.
 */

typedef struct
{
	ILWaitMutex			parent;
	_ILWakeupQueue  	signalQueue;
	int				waiters;
} ILWaitMonitor;

/*
 * Safe critical section enter and leave operations that will prevent the
 * thread from being suspended while it holds a lock.
 */
#define	_ILCriticalSectionEnter(critsect)	\
			do { \
				ILThread *__self = _ILThreadGetSelf(); \
				++(__self->numLocksHeld); \
				_ILCriticalSectionEnterUnsafe((critsect)); \
			} while (0)
#define	_ILCriticalSectionLeave(critsect)	\
			do { \
				ILThread *__self = _ILThreadGetSelf(); \
				_ILCriticalSectionLeaveUnsafe((critsect)); \
				if(--(__self->numLocksHeld) == 0) \
				{ \
					if(__self->suspendRequested) \
					{ \
						_ILThreadSuspendRequest(__self); \
					} \
				} \
			} while (0)

/*
 * Safe mutex lock and unlock operations that will prevent the
 * thread from being suspended while it holds a lock.
 */
#define	_ILMutexLock(mutex)	\
			do { \
				ILThread *__self = _ILThreadGetSelf(); \
				++(__self->numLocksHeld); \
				_ILMutexLockUnsafe((mutex)); \
			} while (0)
#define	_ILMutexUnlock(mutex)	\
			do { \
				ILThread *__self = _ILThreadGetSelf(); \
				_ILMutexUnlockUnsafe((mutex)); \
				if(--(__self->numLocksHeld) == 0) \
				{ \
					if(__self->suspendRequested) \
					{ \
						_ILThreadSuspendRequest(__self); \
					} \
				} \
			} while (0)

/*
 * Safe condition mutex lock and unlock operations that will prevent
 * the thread from being suspended while it holds a lock.
 */
#define	_ILCondMutexLock(mutex)	\
			({ \
				int __result; \
				ILThread *__self = _ILThreadGetSelf(); \
				++(__self->numLocksHeld); \
				if((__result = _ILCondMutexLockUnsafe((mutex)))) \
				{ \
					--(__self->numLocksHeld); \
				}\
				__result; \
			})
#define	_ILCondMutexTryLock(mutex, errval)	\
			do { \
				ILThread *__self = _ILThreadGetSelf(); \
				++(__self->numLocksHeld); \
				if((errval) = _ILCondMutexTryLockUnsafe((mutex))) \
				{ \
					if(--(__self->numLocksHeld) == 0) \
					{ \
						if(__self->suspendRequested) \
						{ \
							_ILThreadSuspendRequest(__self); \
						} \
					} \
				} \
			} while (0)
#define	_ILCondMutexUnlock(mutex)	\
			do { \
				ILThread *__self = _ILThreadGetSelf(); \
				_ILCondMutexUnlockUnsafe((mutex)); \
				if(--(__self->numLocksHeld) == 0) \
				{ \
					if(__self->suspendRequested) \
					{ \
						_ILThreadSuspendRequest(__self); \
					} \
				} \
			} while (0)

/*
 * Safe read/write lock and unlock operations that will prevent
 * the thread from being suspended while it holds a lock.
 */
#define	_ILRWLockReadLock(rwlock)	\
			do { \
				ILThread *__self = _ILThreadGetSelf(); \
				++(__self->numLocksHeld); \
				_ILRWLockReadLockUnsafe((rwlock)); \
			} while (0)
#define	_ILRWLockWriteLock(rwlock)	\
			do { \
				ILThread *__self = _ILThreadGetSelf(); \
				++(__self->numLocksHeld); \
				_ILRWLockWriteLockUnsafe((rwlock)); \
			} while (0)
#define	_ILRWLockUnlock(rwlock)	\
			do { \
				ILThread *__self = _ILThreadGetSelf(); \
				_ILRWLockUnlockUnsafe((rwlock)); \
				if(--(__self->numLocksHeld) == 0) \
				{ \
					if(__self->suspendRequested) \
					{ \
						_ILThreadSuspendRequest(__self); \
					} \
				} \
			} while (0)

/*
 * Some macros that can be used internally as shortcut instead of using
 * the public interface functions (named without the leading underscore)
 */
#define _ILHasThreads()	_ILThreadIsReal
#define _ILThreadSelf()	_ILThreadGetSelf()

/*
 * Initialize the system-specific thread routines and find the
 * thread handle and identifier for the main thread.
 */
void _ILThreadInitSystem(ILThread *mainThread);

/*
 * Initialize the interrupt subsystem.
 */
void _ILInterruptInit();

/*
 * Deinitialize the interrupt subsystem.
 */
void _ILInterruptDeinit();

/*
 * Function that is called to run a thread after system-specific
 * initialization has been performed.  If this function returns,
 * the thread must immediately exit.
 */
void _ILThreadRun(ILThread *thread);

/*
 * This function is only used for initializing an ILThread
 * structure for threads not created by pnet.
 */
void _ILThreadInitHandleSelf(ILThread *thread);

/*
 * Create a new system-specific thread.  Returns zero if
 * the creation process failed.
 */
int _ILThreadCreateSystem(ILThread *thread);

/*
 * Interrupt a thread in the wait/sleep/join state or when it enters the
 * wait/sleep/join state the next time.
 */
void _ThreadInterrupt(ILThread *thread);

/*
 * Process a request to suspend the current thread.
 */
void _ILThreadSuspendRequest(ILThread *thread);

/*
 *	Sets the priority of the thread.
 */
void _ILThreadSetPriority(ILThread *thread, int priority);

/*
 *	Gets the thread priority.
 */
int _ILThreadGetPriority(ILThread *thread);

/*
 * Get the number of running threads.
 * Running threads here is: started but not yet finished.
 * The function is used to check if we are still in single threaded mode.
 */
long _ILThreadGetNumThreads();

/*
 * Enter the "wait/sleep/join" state on the current thread.
 */
int _ILThreadEnterWaitState(ILThread *thread);

/*
 * Leave the "wait/sleep/join" state on the current thread.
 */
int _ILThreadLeaveWaitState(ILThread *thread, int result);

/*
 * Create a thread wakeup object.
 */
void _ILWakeupCreate(_ILWakeup *wakeup);

/*
 * Destroy a thread wakeup object.
 */
void _ILWakeupDestroy(_ILWakeup *wakeup);

/*
 * Set the limit for a "wait" operation.  The operation will
 * be performed when the thread calls "_ILWakeupWait".  The
 * "limit" value indicates how many signals are expected.
 * Returns zero if the thread is already interrupted.
 */
int _ILWakeupSetLimit(_ILWakeup *wakeup, ILUInt32 limit);

/*
 * Adjust the limit for a "wait" operation downwards.
 */
void _ILWakeupAdjustLimit(_ILWakeup *wakeup, ILUInt32 limit);

/*
 * Wait for a thread wakeup object to become available.  Returns 1
 * if the required number of signals arrived, 0 on timeout, or -1
 * if the wakeup was interrupted.
 */
int _ILWakeupWait(_ILWakeup *wakeup, ILUInt32 ms, void **object);

/*
 * Signal a thread wakeup object.  Returns zero if the wakeup
 * object is already signalled or interrupted.
 */
int _ILWakeupSignal(_ILWakeup *wakeup, void *object);

/*
 * Interrupt a thread wakeup object.
 */
void _ILWakeupInterrupt(_ILWakeup *wakeup);

void _ILWakeupCancelInterrupt(_ILWakeup *wakeup);

/*
 * Create a wakeup queue.
 */
void _ILWakeupQueueCreate(_ILWakeupQueue *queue);

/*
 * Destroy a wakeup queue.
 */
void _ILWakeupQueueDestroy(_ILWakeupQueue *queue);

/*
 * Add a wakeup object to the end of a wakeup queue.
 * Returns zero if out of memory.
 */
int _ILWakeupQueueAdd(_ILWakeupQueue *queue, _ILWakeup *wakeup, void *object);

/*
 * Remove a wakeup object from a wakeup queue.
 */
void _ILWakeupQueueRemove(_ILWakeupQueue *queue, _ILWakeup *wakeup);

/*
 * Determine if a wakeup queue is empty.
 */
#define	_ILWakeupQueueIsEmpty(queue)	((queue)->first == 0)

/*
 * Wake the first non-interrupted wakeup object on a queue.
 * Returns NULL if none of the objects can be woken, or the
 * specific wakeup object otherwise.
 */
_ILWakeup *_ILWakeupQueueWake(_ILWakeupQueue *queue);

/*
 * Wake all non-interrupted wakeup objects on a queue.
 */
void _ILWakeupQueueWakeAll(_ILWakeupQueue *queue);

/*
 * Make the thread enter a waiting state.
 */
int _ILEnterWait(ILThread *thread);

/*
 * Make the thread leave a waiting state.
 */
int _ILLeaveWait(ILThread *thread, int result);

/*
 * Wait on a handle, ignoring interrupt & abort.
 */
int _ILWaitOneBackupInterruptsAndAborts(ILWaitHandle *handle, int timeout);

/*
 * Returns 1 if the given thread owns the mutexd.
 */
#define ILWaitMutexThreadOwns(t, handle) \
	(((ILWaitMutex *)handle)->owner == &(t->wakeup))

/*
 * Initialize the monitor subsystem
 */
void _ILMonitorSystemInit();

/*
 * Destroy the monitor subsystem.
 * There must be no used monitors left when calling this function.
 */
void _ILMonitorSystemDeinit();

/*
 * Perform cleanup actions when a thread stops execution.
 */
void _ILMonitorDestroyThread(ILThread *thread);

#ifdef	__cplusplus
};
#endif

#endif	/* _THR_DEFS_H */
