/*
 * StringComparison.cs - Implementation of the "System.StringComparison" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace System
{

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2

public enum StringComparison
{
	CurrentCulture				= 0,
	CurrentCultureIgnoreCase	= 1,
	InvariantCulture			= 2,
	InvariantCultureIgnoreCase	= 3,
	Ordinal						= 4,
	OrdinalIgnoreCase			= 5

}; // enum StringComparison

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2

}; // namespace System
