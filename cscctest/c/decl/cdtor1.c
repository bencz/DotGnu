/*
 * cdtor1.c - Test constructor and destructor definitions.
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

void ctor1(void) __attribute__((constructor))
{
}

void ctor2(void) __attribute__((constructor, __corder__(5)))
{
}

void ctor3(void) __attribute__((constructor, corder(0)))
{
}

void dtor1(void) __attribute__((__destructor__))
{
}

void dtor2(void) __attribute__((destructor, dorder(5)))
{
}

void dtor3(void) __attribute__((destructor, dorder(0)))
{
}

void cdtor(void) __attribute__((constructor, destructor, dorder(3)))
{
}
