/*
 * NumericBinary.cs - Numeric binary operators.
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

// Dummy class for backwards-compatibility.

public sealed class NumericBinary : BinaryOp
{
	// Constructor.
	public NumericBinary(int operatorTok)
			: base(operatorTok)
			{
				// Nothing to do here.
			}

	// Evaluate a numeric binary operator on two values.
	public Object EvaluateNumericBinary(Object v1, Object v2)
			{
				return DoOp(v1, v2, operatorTok);
			}

	// Evaluate a numeric binary operator on two values.
	public static Object DoOp(Object v1, Object v2, JSToken operatorTok)
			{
				double n1 = Convert.ToNumber(v1);
				double n2 = Convert.ToNumber(v2);
				switch(operatorTok)
				{
					case JSToken.Minus:
					{
						return (n1 - n2);
					}
					// Not reached.
			
					case JSToken.Multiply:
					{
						return (n1 * n2);
					}
					// Not reached.
			
					case JSToken.Divide:
					{
						return (n1 / n2);
					}
					// Not reached.
			
					case JSToken.Modulo:
					{
						return (n1 % n2);
					}
					// Not reached.
				}
				throw new JScriptException(JSError.InternalError);
			}

}; // class NumericBinary

}; // namespace Microsoft.JScript
