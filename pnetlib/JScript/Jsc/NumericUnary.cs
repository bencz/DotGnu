/*
 * NumericUnary.cs - Numeric unary operators.
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

public sealed class NumericUnary : UnaryOp
{
	// Constructor.
	public NumericUnary(int operatorTok)
			: base(operatorTok)
			{
				// Nothing to do here.
			}

	// Evaluate a numeric unary operator on a value.
	public Object EvaluateUnary(Object v)
			{
				switch(operatorTok)
				{
					case JSToken.BitwiseNot:
					{
						return ~(Convert.ToInt32(v));
					}
					// Not reached.

					case JSToken.LogicalNot:
					{
						return !(Convert.ToBoolean(v));
					}
					// Not reached.

					case JSToken.Minus:
					{
						return -(Convert.ToNumber(v));
					}
					// Not reached.

					case JSToken.Plus:
					{
						return Convert.ToNumber(v);
					}
					// Not reached.
				}
				throw new JScriptException(JSError.InternalError);
			}

}; // class NumericUnary

}; // namespace Microsoft.JScript
