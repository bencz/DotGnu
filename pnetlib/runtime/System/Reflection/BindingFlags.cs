/*
 * BindingFlags.cs - Implementation of the
 *			"System.Reflection.BindingFlags" class.
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

namespace System.Reflection
{

#if CONFIG_REFLECTION

[Flags]
public enum BindingFlags
{
	Default              = 0x00000000,
	IgnoreCase           = 0x00000001,
	DeclaredOnly         = 0x00000002,
	Instance             = 0x00000004,
	Static               = 0x00000008,
	Public               = 0x00000010,
	NonPublic            = 0x00000020,
	FlattenHierarchy     = 0x00000040,
	InvokeMethod         = 0x00000100,
	CreateInstance       = 0x00000200,
	GetField             = 0x00000400,
	SetField             = 0x00000800,
	GetProperty          = 0x00001000,
	SetProperty          = 0x00002000,
	PutDispProperty      = 0x00004000,
	PutRefDispProperty   = 0x00008000,
	ExactBinding         = 0x00010000,
	SuppressChangeType   = 0x00020000,
	OptionalParamBinding = 0x00040000,
	IgnoreReturn         = 0x01000000

}; // enum BindingFlags

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
