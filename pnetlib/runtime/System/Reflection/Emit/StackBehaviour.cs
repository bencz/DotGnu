/*
 * StackBehaviour.cs - Implementation of the
 *			"System.Reflection.Emit.StackBehaviour" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Reflection.Emit
{

#if CONFIG_REFLECTION_EMIT

public enum StackBehaviour
{

	Pop0               = 0,
	Pop1               = 1,
	Pop1_pop1          = 2,
	Popi               = 3,
	Popi_pop1          = 4,
	Popi_popi          = 5,
	Popi_popi8         = 6,
	Popi_popi_popi     = 7,
	Popi_popr4         = 8,
	Popi_popr8         = 9,
	Popref             = 10,
	Popref_pop1        = 11,
	Popref_popi        = 12,
	Popref_popi_popi   = 13,
	Popref_popi_popi8  = 14,
	Popref_popi_popr4  = 15,
	Popref_popi_popr8  = 16,
	Popref_popi_popref = 17,
	Push0              = 18,
	Push1              = 19,
	Push1_push1        = 20,
	Pushi              = 21,
	Pushi8             = 22,
	Pushr4             = 23,
	Pushr8             = 24,
	Pushref            = 25,
	Varpop             = 26,
	Varpush            = 27

}; // enum StackBehaviour

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
