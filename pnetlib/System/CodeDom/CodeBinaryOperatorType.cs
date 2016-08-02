/*
 * CodeBinaryOperatorType.cs - Implementation of the
 *		System.CodeDom.CodeBinaryOperatorType class.
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

namespace System.CodeDom
{

#if CONFIG_CODEDOM

using System.Runtime.InteropServices;

[Serializable]
#if CONFIG_COM_INTEROP
[ComVisible(true)]
#endif
public enum CodeBinaryOperatorType
{
	Add                = 0,
	Subtract           = 1,
	Multiply           = 2,
	Divide             = 3,
	Modulus            = 4,
	Assign             = 5,
	IdentityInequality = 6,
	IdentityEquality   = 7,
	ValueEquality      = 8,
	BitwiseOr          = 9,
	BitwiseAnd         = 10,
	BooleanOr          = 11,
	BooleanAnd         = 12,
	LessThan           = 13,
	LessThanOrEqual    = 14,
	GreaterThan        = 15,
	GreaterThanOrEqual = 16

}; // enum CodeBinaryOperatorType

#endif // CONFIG_CODEDOM

}; // namespace System.CodeDom
