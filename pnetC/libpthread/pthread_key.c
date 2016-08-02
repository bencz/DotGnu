/*
 * pthread_key.c - Thread-specific keys.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

#include <pthread.h>
#include <stdlib.h>
#include <errno.h>

/*
 * Information that is stored about a key.
 */
typedef struct pthread_key_info_t
  {
    pthread_key_t key;
    void (*destr_function) (void *);
    struct pthread_key_info_t *next;
  } pthread_key_info_t;

/*
 * Information about the data is that stored with a key on a thread.
 */
typedef struct pthread_key_data_t
  {
    pthread_key_t key;
    void *value;
    struct pthread_key_data_t *next;
  } pthread_key_data_t;

/*
 * Key information and storage.
 */
static pthread_mutex_t key_lock = PTHREAD_MUTEX_INITIALIZER;
static pthread_key_t key_last;
static pthread_key_info_t *key_info;
static __declspec(thread) pthread_key_data_t *key_data;

int
pthread_key_create (pthread_key_t *key, void (*destr_function) (void *))
{
  pthread_key_info_t *info;

  /* Allocate space for the key information block */
  info = (pthread_key_info_t *)malloc (sizeof (pthread_key_info_t));
  if (!info)
    {
      errno = ENOMEM;
      return -1;
    }
  info->destr_function = destr_function;

  /* Add the key information block to the active list */
  pthread_mutex_lock (&key_lock);
  info->key = *key = ++key_last;
  info->next = key_info;
  key_info = info;
  pthread_mutex_unlock (&key_lock);
  return 0;
}

int
pthread_key_delete (pthread_key_t key)
{
  /* Not used in this implementation */
  return 0;
}

int
pthread_setspecific (pthread_key_t key, __const void *pointer)
{
  pthread_key_data_t *data = key_data;
  while (data != 0)
    {
      if (data->key == key)
        {
	  data->value = (void *)pointer;
	  return 0;
	}
      data = data->next;
    }
  data = (pthread_key_data_t *)malloc (sizeof (pthread_key_data_t));
  if (!data)
    {
      errno = ENOMEM;
      return -1;
    }
  data->key = key;
  data->value = (void *)pointer;
  data->next = key_data;
  key_data = data;
  return 0;
}

void *
pthread_getspecific (pthread_key_t key)
{
  pthread_key_data_t *data = key_data;
  while (data != 0)
    {
      if (data->key == key)
        {
	  return data->value;
	}
      data = data->next;
    }
  return 0;
}

void
__pt_destroy_keys (void)
{
  pthread_key_data_t *data;
  pthread_key_info_t *info;
  void (*destr_function) (void *);
  while (key_data != 0)
    {
      data = key_data;
      key_data = key_data->next;
      if (data->value)
        {
	  destr_function = 0;
          pthread_mutex_lock (&key_lock);
	  info = key_info;
	  while (info != 0)
	    {
	      if (info->key == data->key)
	        {
		  destr_function = info->destr_function;
		  break;
		}
	      info = info->next;
	    }
          pthread_mutex_unlock (&key_lock);
	  if (destr_function)
	    {
	      (*destr_function) (data->value);
	    }
	}
      free (data);
    }
}
