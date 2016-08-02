/*
 * type4.c - Test type definitions that involve "__typeof__".
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

int func(int x)
{
	return x;
}

__typeof__(func) func2
{
	return 0;
}

__typeof__(func) func3;

__typeof__(func) func4 __attribute__((weak, alias("func2")));
__typeof__(func) func5 __attribute__((__alias__("func2")));

__typeof__(func) func6 __attribute__((pinvoke("module.so"), ansi));
