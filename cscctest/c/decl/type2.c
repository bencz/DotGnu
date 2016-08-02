/*
 * type2.c - Test invalid cases of simple type specifiers on basic types.
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

/* duplicate specifiers */
const const int x;
const int const x2;
volatile long volatile long x3;

/* multiple base types */
int int y;
int _Bool y2;

/* signed and unsigned specified together */
signed unsigned i;
unsigned signed i2;

/* invalid "char" cases */
short char c;
char short c2;
long char c3;
char long c4;
__native__ char c5;
char __native__ c6;
signed unsigned char c7;
unsigned char signed c8;

/* invalid "short" cases */
short short s;
short long s2;
long short s3;
short __native__ s4;
__native__ short s5;
signed short __native__ s6;
signed __native__ short s7;
short signed __native__ s8;
short __native__ signed s9;
__native__ signed short s10;
__native__ short signed s11;
unsigned short __native__ s12;
unsigned __native__ short s13;
short unsigned __native__ s14;
short __native__ unsigned s15;
__native__ unsigned short s16;
__native__ short unsigned s17;

/* invalid "long" cases */
long __native__ l;
__native__ long l2;
signed long __native__ l3;
signed __native__ long l4;
long signed __native__ l5;
long __native__ signed l6;
__native__ signed long l7;
__native__ long signed l8;
unsigned long __native__ l9;
unsigned __native__ long l10;
long unsigned __native__ l11;
long __native__ unsigned l12;
__native__ unsigned long l13;
__native__ long unsigned l14;

/* invalid "float" and "double" cases */
signed float f;
float signed f2;
unsigned float f3;
float unsigned f4;
__native__ float f5;
float __native__ f6;
short float f7;
float short f8;
signed double d;
double signed d2;
unsigned double d3;
double unsigned d4;
__native__ double d5;
double __native__ d6;
short double d7;
double short d8;

/* default to int */
const ci;
