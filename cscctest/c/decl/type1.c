/*
 * type1.c - Test simple type specifiers on basic types.
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

/* "char" and "unsigned char" */
char c;
unsigned char uc;
char unsigned uc2;
signed char sc;
char signed sc2;

/* "short" and "unsigned short" */
short s;
unsigned short us;
short unsigned us2;
signed short ss;
short signed ss2;
short int s2;
unsigned short int us3;
short unsigned int us4;
signed short int ss3;
short signed int ss4;
int short s3;
unsigned int short us5;
short int unsigned us6;
signed int short ss5;
short int signed ss6;

/* "int" and "unsigned int" */
int i;
unsigned int ui;
int unsigned ui2;
unsigned ui3;
signed int si;
int signed si2;
signed si3;

/* "__native__ int" and "unsigned __native__ int" */
__native__ int ni;
int __native__ ni2;
__native__ ni3;
unsigned __native__ int uni;
unsigned int __native__ uni2;
__native__ unsigned int uni3;
__native__ int unsigned uni4;
int __native__ unsigned uni5;
int unsigned __native__ uni6;
unsigned __native__ uni7;
__native__ unsigned uni8;
signed __native__ int sni;
signed int __native__ sni2;
__native__ signed int sni3;
__native__ int signed sni4;
int __native__ signed sni5;
int signed __native__ sni6;
signed __native__ sni7;
__native__ signed sni8;

/* "long" and "unsigned long" */
long int l;
int long l2;
long l3;
unsigned long int ul;
unsigned int long ul2;
long unsigned int ul3;
long int unsigned ul4;
int long unsigned ul5;
int unsigned long ul6;
unsigned long ul7;
long unsigned ul8;
signed long int sl;
signed int long sl2;
long signed int sl3;
long int signed sl4;
int long signed sl5;
int signed long sl6;
signed long sl7;
long signed sl8;

/* "long long" and "unsigned long long" */
long long int ll;
long int long ll2;
int long long ll3;
long long ll4;
long unsigned long int ull;
long unsigned int long ull2;
long long unsigned int ull3;
long long int unsigned ull4;
long int long unsigned ull5;
long int unsigned long ull6;
int unsigned long long ull7;
int long unsigned long ull8;
int long long unsigned ull9;
unsigned int long long ull10;
unsigned long int long ull11;
unsigned long long int ull12;
unsigned long long ull13;
long unsigned long ull14;
long long unsigned ull15;
long signed long int sll;
long signed int long sll2;
long long signed int sll3;
long long int signed sll4;
long int long signed sll5;
long int signed long sll6;
int signed long long sll7;
int long signed long sll8;
int long long signed sll9;
signed int long long sll10;
signed long int long sll11;
signed long long int sll12;
signed long long sll13;
long signed long sll14;
long long signed sll15;

/* "float", "double", and "long double" */
float f;
double d;
long float lf;
float long lf2;
long double ld;
double long ld2;
long long float llf;
long float long llf2;
float long long llf3;
long long double lld;
long double long lld2;
double long long lld3;

/* special types */
_Bool b;
__wchar__ wc;
__builtin_va_list va;
