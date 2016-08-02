/*
 * pthread.h - Threading routines.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2002, 2004  Southern Storm Software, Pty Ltd.
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
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#ifndef _PTHREAD_H
#define _PTHREAD_H	1

#include <features.h>
#include <sys/types.h>
#include <signal.h>
#include <time.h>

__BEGIN_DECLS

/*
 * Useful constants.
 */
enum
{
  PTHREAD_MUTEX_TIMED_NP,
  PTHREAD_MUTEX_RECURSIVE_NP,
  PTHREAD_MUTEX_ERRORCHECK_NP,
  PTHREAD_MUTEX_ADAPTIVE_NP
#ifdef __USE_UNIX98
  ,
  PTHREAD_MUTEX_NORMAL = PTHREAD_MUTEX_TIMED_NP,
  PTHREAD_MUTEX_RECURSIVE = PTHREAD_MUTEX_RECURSIVE_NP,
  PTHREAD_MUTEX_ERRORCHECK = PTHREAD_MUTEX_ERRORCHECK_NP,
  PTHREAD_MUTEX_DEFAULT = PTHREAD_MUTEX_NORMAL
#endif
#ifdef __USE_GNU
  /* For compatibility.  */
  , PTHREAD_MUTEX_FAST_NP = PTHREAD_MUTEX_ADAPTIVE_NP
#endif
};
enum
{
  PTHREAD_CREATE_JOINABLE,
  PTHREAD_CREATE_DETACHED
#define PTHREAD_CREATE_JOINABLE	PTHREAD_CREATE_JOINABLE
#define PTHREAD_CREATE_DETACHED	PTHREAD_CREATE_DETACHED
};
enum
{
  PTHREAD_INHERIT_SCHED,
  PTHREAD_EXPLICIT_SCHED
#define PTHREAD_INHERIT_SCHED	PTHREAD_INHERIT_SCHED
#define PTHREAD_EXPLICIT_SCHED	PTHREAD_EXPLICIT_SCHED
};
enum
{
  PTHREAD_SCOPE_SYSTEM,
  PTHREAD_SCOPE_PROCESS
#define PTHREAD_SCOPE_SYSTEM	PTHREAD_SCOPE_SYSTEM
#define PTHREAD_SCOPE_PROCESS	PTHREAD_SCOPE_PROCESS
};
enum
{
  PTHREAD_PROCESS_PRIVATE,
  PTHREAD_PROCESS_SHARED
#define PTHREAD_PROCESS_PRIVATE	PTHREAD_PROCESS_PRIVATE
#define PTHREAD_PROCESS_SHARED	PTHREAD_PROCESS_SHARED
};
enum
{
  PTHREAD_RWLOCK_PREFER_READER_NP,
  PTHREAD_RWLOCK_PREFER_WRITER_NP,
  PTHREAD_RWLOCK_PREFER_WRITER_NONRECURSIVE_NP,
  PTHREAD_RWLOCK_DEFAULT_NP = PTHREAD_RWLOCK_PREFER_WRITER_NP
};
#define PTHREAD_ONCE_INIT 0
enum
{
  PTHREAD_CANCEL_ENABLE,
  PTHREAD_CANCEL_DISABLE
#define PTHREAD_CANCEL_ENABLE       PTHREAD_CANCEL_ENABLE
#define PTHREAD_CANCEL_DISABLE      PTHREAD_CANCEL_DISABLE
};
enum
{
  PTHREAD_CANCEL_DEFERRED,
  PTHREAD_CANCEL_ASYNCHRONOUS
#define PTHREAD_CANCEL_DEFERRED     PTHREAD_CANCEL_DEFERRED
#define PTHREAD_CANCEL_ASYNCHRONOUS PTHREAD_CANCEL_ASYNCHRONOUS
};
#define PTHREAD_CANCELED ((void *)(-1))
#define SCHED_OTHER   0
#define SCHED_FIFO    1
#define SCHED_RR      2

/*
 * Thread identifier.
 */
typedef long long pthread_t;

/*
 * Scheduling parameters.
 */
struct sched_param
  {
    int __sched_priority;
  };

/*
 * Thread attributes.
 */
typedef struct __pthread_attr_s
  {
    int __detachstate;
    int __schedpolicy;
    struct sched_param __schedparam;
    int __inheritsched;
    int __scope;
    size_t __guardsize;
    int __stackaddr_set;
    void *__stackaddr;
    size_t __stacksize;
  } pthread_attr_t;

/*
 * Structure of a mutex, which is wrapped around a C# monitor.
 */
typedef struct __pthread_mutex_s
  {
    void *__m_monitor;
  } pthread_mutex_t;
#define PTHREAD_MUTEX_INITIALIZER	{0}
#define PTHREAD_RECURSIVE_MUTEX_INITIALIZER_NP	\
  PTHREAD_MUTEX_INITIALIZER
#define PTHREAD_ERRORCHECK_MUTEX_INITIALIZER_NP \
  PTHREAD_MUTEX_INITIALIZER
#define PTHREAD_ADAPTIVE_MUTEX_INITIALIZER_NP \
  PTHREAD_MUTEX_INITIALIZER

/*
 * Mutex attributes.
 */
typedef struct __pthread_mutexattr_s
  {
    int __mutex_kind;
    int __mutex_pshared;
  } pthread_mutexattr_t;

/*
 * Spin locks.
 */
typedef int pthread_spinlock_t;

/*
 * Once handling.
 */
typedef pthread_spinlock_t pthread_once_t;

/*
 * Condition variables.
 */
typedef struct __pthread_cond_s
  {
    pthread_spinlock_t __lock;
    pthread_mutex_t *__wait_mutex;
  } pthread_cond_t;
#define PTHREAD_COND_INITIALIZER {0, 0}

/*
 * Condition variable attributes.
 */
typedef struct __pthread_condattr_s
  {
    int __pshared;
    clockid_t __clock_id;
  } pthread_condattr_t;

/*
 * Read-write locks.
 */
typedef struct __pthread_rwlock_s
  {
    pthread_mutex_t __lock;
  } pthread_rwlock_t;
#define PTHREAD_RWLOCK_INITIALIZER \
  {PTHREAD_MUTEX_INITIALIZER}
#define PTHREAD_RWLOCK_WRITER_NONRECURSIVE_INITIALIZER_NP \
  {PTHREAD_MUTEX_INITIALIZER}

/*
 * Read-write lock attributes.
 */
typedef struct __pthread_rwlockattr_s
  {
    pthread_mutexattr_t __attr;
  } pthread_rwlockattr_t;

/*
 * Cleanup buffers.
 */
struct _pthread_cleanup_buffer
  {
    void (*__routine) (void *);
    void *__arg;
    int __canceltype;
    struct _pthread_cleanup_buffer *__prev;
  };

/*
 * Thread-specific storage key.
 */
typedef long long pthread_key_t;

/*
 * Barrier type.
 */
typedef struct __pthread_barrier_s
  {
    pthread_mutex_t __lock;
    pthread_cond_t __condition;
    unsigned int __desired_count;
    unsigned int __current_count;
  } pthread_barrier_t;

/*
 * Barrer attribute type.
 */
typedef struct __pthread_barrierattr_s
  {
    int __pshared;
  } pthread_barrierattr_t;

/*
 * Thread functions.
 */
extern int pthread_create (pthread_t *__restrict __thread,
			   __const pthread_attr_t *__restrict __attr,
			   void *(*__start_routine) (void *),
			   void *__restrict __arg);
extern pthread_t pthread_self (void);
extern int pthread_equal (pthread_t __thread1, pthread_t __thread2);
extern void pthread_exit (void *__retval);
extern int pthread_join (pthread_t __thread, void **__thread_return);
extern int pthread_detach (pthread_t __thread);
extern int pthread_yield (void);
extern int pthread_cancel (pthread_t __thread);
extern void pthread_testcancel (void);
extern int pthread_setcancelstate (int __state, int *__oldstate);
extern int pthread_setcanceltype (int __type, int *__oldtype);
extern int pthread_setschedparam (pthread_t __target_thread, int __policy,
				  __const struct sched_param *__param);
extern int pthread_getschedparam (pthread_t __target_thread,
				  int *__restrict __policy,
				  struct sched_param *__restrict __param);
extern int pthread_setschedprio (pthread_t __thread, int __prio);

/*
 * Thread attribute functions.
 */
extern int pthread_attr_init (pthread_attr_t *__attr);
extern int pthread_attr_destroy (pthread_attr_t *__attr);
extern int pthread_attr_setdetachstate (pthread_attr_t *__attr,
					int __detachstate);
extern int pthread_attr_getdetachstate (__const pthread_attr_t *__attr,
					int *__detachstate);
extern int pthread_attr_setschedparam (pthread_attr_t *__restrict __attr,
				       __const struct sched_param *__restrict
				       __param);
extern int pthread_attr_getschedparam (__const pthread_attr_t *__restrict
				       __attr,
				       struct sched_param *__restrict __param);
extern int pthread_attr_setschedpolicy (pthread_attr_t *__attr, int __policy);
extern int pthread_attr_getschedpolicy (__const pthread_attr_t *__restrict
					__attr, int *__restrict __policy);
extern int pthread_attr_setinheritsched (pthread_attr_t *__attr,
					 int __inherit);
extern int pthread_attr_getinheritsched (__const pthread_attr_t *__restrict
					 __attr, int *__restrict __inherit);
extern int pthread_attr_setscope (pthread_attr_t *__attr, int __scope);
extern int pthread_attr_getscope (__const pthread_attr_t *__restrict __attr,
				  int *__restrict __scope);
extern int pthread_attr_setguardsize (pthread_attr_t *__attr,
				      size_t __guardsize);
extern int pthread_attr_getguardsize (__const pthread_attr_t *__restrict
				      __attr, size_t *__restrict __guardsize);
extern int pthread_attr_setstackaddr (pthread_attr_t *__attr,
				      void *__stackaddr);
extern int pthread_attr_getstackaddr (__const pthread_attr_t *__restrict
				      __attr, void **__restrict __stackaddr);
extern int pthread_attr_setstack (pthread_attr_t *__attr, void *__stackaddr,
				  size_t __stacksize);
extern int pthread_attr_getstack (__const pthread_attr_t *__restrict __attr,
				  void **__restrict __stackaddr,
				  size_t *__restrict __stacksize);
extern int pthread_attr_setstacksize (pthread_attr_t *__attr,
				      size_t __stacksize);
extern int pthread_attr_getstacksize (__const pthread_attr_t *__restrict
				      __attr, size_t *__restrict __stacksize);
extern int pthread_getattr_np (pthread_t __th, pthread_attr_t *__attr);

/*
 * Mutex functions.
 */
extern int pthread_mutex_init (pthread_mutex_t *__restrict __mutex,
			       __const pthread_mutexattr_t *__restrict
			       __mutex_attr);
extern int pthread_mutex_destroy (pthread_mutex_t *__mutex);
extern int pthread_mutex_trylock (pthread_mutex_t *__mutex);
extern int pthread_mutex_lock (pthread_mutex_t *__mutex);
extern int pthread_mutex_timedlock (pthread_mutex_t *__restrict __mutex,
				    __const struct timespec *__restrict
				    __abstime);
extern int pthread_mutex_unlock (pthread_mutex_t *__mutex);

/*
 * Mutex attribute functions.
 */
extern int pthread_mutexattr_init (pthread_mutexattr_t *__attr);
extern int pthread_mutexattr_destroy (pthread_mutexattr_t *__attr);
extern int pthread_mutexattr_getpshared (__const pthread_mutexattr_t *
					 __restrict __attr,
					 int *__restrict __pshared);
extern int pthread_mutexattr_setpshared (pthread_mutexattr_t *__attr,
					 int __pshared);
extern int pthread_mutexattr_settype (pthread_mutexattr_t *__attr, int __kind);
extern int pthread_mutexattr_gettype (__const pthread_mutexattr_t *__restrict
				      __attr, int *__restrict __kind);

/*
 * Condition variable functions.
 */
extern int pthread_cond_init (pthread_cond_t *__restrict __cond,
			      __const pthread_condattr_t *__restrict __cond_attr);
extern int pthread_cond_destroy (pthread_cond_t *__cond);
extern int pthread_cond_signal (pthread_cond_t *__cond);
extern int pthread_cond_broadcast (pthread_cond_t *__cond);
extern int pthread_cond_wait (pthread_cond_t *__restrict __cond,
			      pthread_mutex_t *__restrict __mutex);
extern int pthread_cond_timedwait (pthread_cond_t *__restrict __cond,
				   pthread_mutex_t *__restrict __mutex,
				   __const struct timespec *__restrict
				   __abstime);

/*
 * Condition variable attribute functions.
 */
extern int pthread_condattr_init (pthread_condattr_t *__attr);
extern int pthread_condattr_destroy (pthread_condattr_t *__attr);
extern int pthread_condattr_getpshared (__const pthread_condattr_t *
					__restrict __attr,
					int *__restrict __pshared);
extern int pthread_condattr_setpshared (pthread_condattr_t *__attr,
					int __pshared);
extern int pthread_condattr_getclock (__const pthread_condattr_t *
				      __restrict __attr,
				      clockid_t *__restrict __clock_id);
extern int pthread_condattr_setclock (pthread_condattr_t *__attr,
				      clockid_t __clock_id);

/*
 * Read-write lock functions.
 */
extern int pthread_rwlock_init (pthread_rwlock_t *__restrict __rwlock,
			        __const pthread_rwlockattr_t *
				__restrict __attr);
extern int pthread_rwlock_destroy (pthread_rwlock_t *__rwlock);
extern int pthread_rwlock_rdlock (pthread_rwlock_t *__rwlock);
extern int pthread_rwlock_tryrdlock (pthread_rwlock_t *__rwlock);
extern int pthread_rwlock_timedrdlock (pthread_rwlock_t *__restrict __rwlock,
				       __const struct timespec *__restrict
				       __abstime);
extern int pthread_rwlock_wrlock (pthread_rwlock_t *__rwlock);
extern int pthread_rwlock_trywrlock (pthread_rwlock_t *__rwlock);
extern int pthread_rwlock_timedwrlock (pthread_rwlock_t *__restrict __rwlock,
				       __const struct timespec *__restrict
				       __abstime);
extern int pthread_rwlock_unlock (pthread_rwlock_t *__rwlock);

/*
 * Read-write lock attribute functions.
 */
extern int pthread_rwlockattr_init (pthread_rwlockattr_t *__attr);
extern int pthread_rwlockattr_destroy (pthread_rwlockattr_t *__attr);
extern int pthread_rwlockattr_getpshared (__const pthread_rwlockattr_t *
					  __restrict __attr,
					  int *__restrict __pshared);
extern int pthread_rwlockattr_setpshared (pthread_rwlockattr_t *__attr,
					  int __pshared);
extern int pthread_rwlockattr_getkind_np (__const pthread_rwlockattr_t *__attr,
					  int *__pref);
extern int pthread_rwlockattr_setkind_np (pthread_rwlockattr_t *__attr,
					  int __pref);

/*
 * Spin lock functions.
 */
extern int pthread_spin_init (pthread_spinlock_t *__lock, int __pshared);
extern int pthread_spin_destroy (pthread_spinlock_t *__lock);
extern int pthread_spin_lock (pthread_spinlock_t *__lock);
extern int pthread_spin_trylock (pthread_spinlock_t *__lock);
extern int pthread_spin_unlock (pthread_spinlock_t *__lock);

/*
 * Once functions.
 */
extern int pthread_once (pthread_once_t *__once_control,
                         void (*__init_routine) (void));

/*
 * Cleanup handlers.
 */
extern void _pthread_cleanup_push (struct _pthread_cleanup_buffer *__buffer,
				   void (*__routine) (void *), void *__arg);
extern void _pthread_cleanup_push_defer (struct _pthread_cleanup_buffer *
                                         __buffer, void (*__routine) (void *),
					 void *__arg);
extern void _pthread_cleanup_pop (struct _pthread_cleanup_buffer *__buffer,
                  		  int __execute);
extern void _pthread_cleanup_pop_restore (struct _pthread_cleanup_buffer *
                                          __buffer, int __execute);
#define pthread_cleanup_push(routine,arg)	\
		{ struct _pthread_cleanup_buffer _buffer;	\
		  _pthread_cleanup_push (&_buffer, (routine), (arg));
#define pthread_cleanup_pop(execute)	\
		_pthread_cleanup_pop (&_buffer, (execute)); }
#define pthread_cleanup_push_defer_np(routine,arg)	\
		{ struct _pthread_cleanup_buffer _buffer;	\
		  _pthread_cleanup_push_defer (&_buffer, (routine), (arg));
#define pthread_cleanup_pop_restore_np(execute)	\
		_pthread_cleanup_pop_restore (&_buffer, (execute)); }

/*
 * Thread-specific storage functions.
 */
int pthread_key_create (pthread_key_t *__key,
                        void (*__destr_function) (void *));
int pthread_key_delete (pthread_key_t __key);
int pthread_setspecific (pthread_key_t __key, __const void *__pointer);
void *pthread_getspecific (pthread_key_t __key);

/*
 * Barrier functions.
 */
extern int pthread_barrier_init (pthread_barrier_t *__restrict __barrier,
				 __const pthread_barrierattr_t *__restrict
				 __attr, unsigned int __count);
extern int pthread_barrier_destroy (pthread_barrier_t *__barrier);
extern int pthread_barrier_wait (pthread_barrier_t *__barrier);

/*
 * Barrier attribute functions.
 */
extern int pthread_barrierattr_init (pthread_barrierattr_t *__attr);
extern int pthread_barrierattr_destroy (pthread_barrierattr_t *__attr);
extern int pthread_barrierattr_getpshared (__const pthread_barrierattr_t *
					   __restrict __attr,
					   int *__restrict __pshared);
extern int pthread_barrierattr_setpshared (pthread_barrierattr_t *__attr,
					   int __pshared);

/*
 * Other functions.
 */
extern int pthread_getconcurrency (void);
extern int pthread_setconcurrency (int __new_level);
extern int pthread_getcpuclockid (pthread_t __thread, clockid_t *__clock_id);
extern int pthread_atfork (void (*__prepare) (void), void (*__parent) (void),
			   void (*__child) (void));
extern void pthread_kill_other_threads_np (void);

__END_DECLS

#endif  /* !_PTHREAD_H */
