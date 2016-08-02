/*
 * FlowControl.cs - Implementation of the
 *			"System.Reflection.Emit.FlowControl" class.
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

public enum FlowControl
{

	Branch      = 0,
	Break       = 1,
	Call        = 2,
	Cond_Branch = 3,
	Meta        = 4,
	Next        = 5,
	Phi         = 6,
	Return      = 7,
	Throw       = 8

}; // enum FlowControl

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
