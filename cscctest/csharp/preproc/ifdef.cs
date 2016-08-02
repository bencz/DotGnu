/*
 * ifdef.cs - Test the use of #if pre-processor directives.
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

#define	A
#undef	BB

/* Conditions that are always true */
#if true
	x1;
#else
	x2;
#endif
#if false || true
	y1;
#else
	y2;
#endif
#if !false
	z1;
#else
	z2;
#endif
#if BB || A
	w1;
#else
	w2;
#endif

/* Conditions that are always false */
#if false
	x1;
#else
	x2;
#endif
#if false && true
	y1;
#else
	y2;
#endif
#if !true
	z1;
#else
	z2;
#endif
#if BB && A
	w1;
#else
	w2;
#endif

/* Test "elif" */
#if false
	v1;
#elif true
	v2;
#elif true
	v3;
#endif
#if false
	v1;
#elif false
	v2;
#elif true
	v3;
#endif
#if false
	u1;
#elif true
	u2;
#else
	u3;
#endif
#if false
	u1;
#elif false
	u2;
#else
	u3;
#endif

/* Test nested "if"'s */
#if true
	#if true
		t1;
	#else
		t2;
	#endif
#else
	#if true
		t3;
	#else
		t4;
	#endif
#endif
#if true
	#if false
		t1;
	#else
		t2;
	#endif
#else
	#if false
		t3;
	#else
		t4;
	#endif
#endif
#if false
	#if true
		t1;
	#else
		t2;
	#endif
#else
	#if true
		t3;
	#else
		t4;
	#endif
#endif
#if false
	#if false
		t1;
	#else
		t2;
	#endif
#else
	#if false
		t3;
	#else
		t4;
	#endif
#endif

/* Test condition parsing on valid expressions */
#if !A
#endif
#if A || B && C
#endif
#if A && B && C
#endif
#if A && B || C
#endif
#if A == true
#endif
#if !A != true
	s1;
#else
	s2;
#endif
#if !A == true
	s1;
#else
	s2;
#endif
#if A && (B || C)
#endif

/* Test condition parsing on invalid expressions */
#if A true
#endif
#if A ||
#endif
#if (A
#endif
#if %
#endif

/* We should not be able to change symbols inside a suppressed case */
#undef M1
#if false
	#define M1
#endif
#if M1
	r1;
#else
	r2;		// We should get this case.
#endif
#define M1
#if false
	#undef M1
#endif
#if M1
	r1;		// We should get this case.
#else
	r2;
#endif
