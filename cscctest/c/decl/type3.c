/*
 * type3.c - Test type definitions that involve declarators.
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

/* array types */
int a[20];
int a2[1];
int a3[0];
int a4[3][5];
int *a5[20];
int *a6[3][5];
int (* const volatile a7)[3];
char a8(int x)[] { return 0; }

/* pointer types */
char *p;
const char *p2;
char const *p3;
char * const p4;
__wchar__ * volatile p5;

/* function pointer types */
int (*f)();
int (**f2)(int, char *);
void f3(int (*)(char *)) {}
void f4(int (**x)(char *)) {}
