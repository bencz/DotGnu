/*
 * cdtor2.c - Test invalid constructor and destructor definitions.
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

/* Cannot have a return type */
int ctor1(void) __attribute__((constructor))
{
	return 0;
}
int dtor1(void) __attribute__((destructor))
{
	return 0;
}

/* Cannot have parameters */
void ctor2(int x) __attribute__((constructor))
{
}
void dtor2(int x) __attribute__((destructor))
{
}
