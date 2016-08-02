/*
 * binary1.c - Test binary arithmetic operators.
 *
 * Copyright (C) 2001, 2002  Southern Storm Software, Pty Ltd.
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

void m1(void)
{
	char sb1 = 34;
	unsigned char b1 = 42;
	short s1 = -126;
	unsigned short us1 = 67;
	int i1 = -1234;
	unsigned int ui1 = 54321;
	long long l1 = -9876543210;
	unsigned long long ul1 = 9876543210;
	__wchar__ c1 = 'A';
	float f1 = 1.5f;
	double d1 = 6.7;
	char sb2 = 34;
	unsigned char b2 = 42;
	short s2 = -126;
	unsigned short us2 = 67;
	int i2 = -1234;
	unsigned int ui2 = 54321;
	long long l2 = -9876543210;
	unsigned long long ul2 = 9876543210;
	__wchar__ c2 = 'A';
	float f2 = 1.5f;
	double d2 = 6.7;
	char sb3;
	unsigned char b3;
	short s3;
	unsigned short us3;
	int i3;
	unsigned int ui3;
	long long l3;
	unsigned long long ul3;
	__wchar__ c3;
	float f3;
	double d3;
	
	// sbyte op ???
	i3 = sb1 + sb2;
	i3 = sb1 + b2;
	i3 = sb1 + s2;
	i3 = sb1 + us2;
	i3 = sb1 + i2;
	l3 = sb1 + ui2;
	l3 = sb1 + l2;
	i3 = sb1 + c2;
	f3 = sb1 + f2;
	d3 = sb1 + d2;

	// byte op ???
	i3 = b1 - sb2;
	i3 = b1 - b2;
	i3 = b1 - s2;
	i3 = b1 - us2;
	i3 = b1 - i2;
	ui3 = b1 - ui2;
	l3 = b1 - l2;
	ul3 = b1 - ul2;
	i3 = b1 - c2;
	f3 = b1 - f2;
	d3 = b1 - d2;

	// short op ???
	i3 = s1 * sb2;
	i3 = s1 * b2;
	i3 = s1 * s2;
	i3 = s1 * us2;
	i3 = s1 * i2;
	l3 = s1 * ui2;
	l3 = s1 * l2;
	i3 = s1 * c2;
	f3 = s1 * f2;
	d3 = s1 * d2;

	// ushort op ???
	i3 = us1 / sb2;
	i3 = us1 / b2;
	i3 = us1 / s2;
	i3 = us1 / us2;
	i3 = us1 / i2;
	ui3 = us1 / ui2;
	l3 = us1 / l2;
	ul3 = us1 / ul2;
	i3 = us1 / c2;
	f3 = us1 / f2;
	d3 = us1 / d2;

	// int op ???
	i3 = i1 % sb2;
	i3 = i1 % b2;
	i3 = i1 % s2;
	i3 = i1 % us2;
	i3 = i1 % i2;
	l3 = i1 % ui2;
	l3 = i1 % l2;
	i3 = i1 % c2;
	f3 = i1 * f2;
	d3 = i1 * d2;

	// uint op ???
	l3 = ui1 + sb2;
	ui3 = ui1 + b2;
	l3 = ui1 + s2;
	ui3 = ui1 + us2;
	l3 = ui1 + i2;
	ui3 = ui1 + ui2;
	l3 = ui1 + l2;
	ul3 = ui1 + ul2;
	ui3 = ui1 + c2;
	f3 = ui1 + f2;
	d3 = ui1 + d2;

	// long op ???
	l3 = l1 - sb2;
	l3 = l1 - b2;
	l3 = l1 - s2;
	l3 = l1 - us2;
	l3 = l1 - i2;
	l3 = l1 - ui2;
	l3 = l1 - l2;
	l3 = l1 - c2;
	f3 = l1 - f2;
	d3 = l1 - d2;

	// ulong op ???
	ul3 = ul1 * b2;
	ul3 = ul1 * us2;
	ul3 = ul1 * ui2;
	ul3 = ul1 * ul2;
	ul3 = ul1 * c2;
	f3 = ul1 * f2;
	d3 = ul1 * d2;

	// float op ???
	f3 = f1 / sb2;
	f3 = f1 / b2;
	f3 = f1 / s2;
	f3 = f1 / us2;
	f3 = f1 / i2;
	f3 = f1 / ui2;
	f3 = f1 / l2;
	f3 = f1 / ul2;
	f3 = f1 / c2;
	f3 = f1 / f2;
	d3 = f1 / d2;

	// double op ???
	d3 = d1 + sb2;
	d3 = d1 + b2;
	d3 = d1 + s2;
	d3 = d1 + us2;
	d3 = d1 + i2;
	d3 = d1 + ui2;
	d3 = d1 + l2;
	d3 = d1 + ul2;
	d3 = d1 + c2;
	d3 = d1 + f2;
	d3 = d1 + d2;
}
