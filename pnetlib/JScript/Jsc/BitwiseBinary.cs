/*
 * BitwiseBinary.cs - Bitwise binary operators.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.JScript
{

using System;
using System.Reflection;

// Dummy class for backwards-compatibility.

public sealed class BitwiseBinary : BinaryOp
{
	// Constructor.
	public BitwiseBinary(int operatorTok)
			: base(operatorTok)
			{
				// Nothing to do here.
			}

	// Evaluate a bitwise binary operator on two values.
	public Object EvaluateBitwiseBinary(Object v1, Object v2)
			{
				int val1 = Convert.ToInt32(v1);
				int val2 = Convert.ToInt32(v2);
				Object result;
				switch(operatorTok)
				{
					case JSToken.BitwiseAnd:
						result = val1 & val2; break;
					case JSToken.BitwiseOr:
						result = val1 | val2; break;
					case JSToken.BitwiseXor:
						result = val1 ^ val2; break;
					case JSToken.LeftShift:
						result = val1 << val2; break;
					case JSToken.RightShift:
						result = val1 >> val2; break;
					case JSToken.UnsignedRightShift:
						result = ((uint)val1) >> val2; break;
					default:
						throw new JScriptException(JSError.InternalError);
				}
				return result;
			}

}; // class BitwiseBinary

}; // namespace Microsoft.JScript
