/*
 * funcptr5.c - Test function pointer usage.
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

typedef unsigned int size_t;
typedef int (*__compar_fn_t) (const void *, const void *);
extern void qsort(void *__base, size_t __nmemb, size_t __size,
				  __compar_fn_t __compar);

int compare_pos(const float *a, const float *b)
{
	return 1;
}

int main()
{
	int num = 0;
	static float *save_array = ((void *)0);
	qsort(save_array, num, sizeof(float),
		  (int (*) (const void *, const void *))compare_pos);
	qsort(save_array, num, sizeof(float), compare_pos);
	return 0;
}
