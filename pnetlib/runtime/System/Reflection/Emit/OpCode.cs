/*
 * OpCode.cs - Implementation of the
 *			"System.Reflection.Emit.OpCode" class.
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

public struct OpCode
{
	// Internal state.  Declared "internal" for faster
	// access by other "Emit" classes.
	internal byte   flowControl;
	internal byte   opcodeType;
	internal byte   operandType;
	internal byte   size;
	internal byte   stackPop;
	internal byte   stackPush;
	internal short  value;
	internal String name;

	// Construct a new opcode.
	internal OpCode(String name, int value, FlowControl flowControl,
					OpCodeType opcodeType, OperandType operandType,
					StackBehaviour stackPop, StackBehaviour stackPush)
			{
				this.name = name;
				this.value = (short)value;
				this.flowControl = (byte)flowControl;
				this.opcodeType = (byte)opcodeType;
				this.operandType = (byte)operandType;
				if(value < 0x0100)
				{
					this.size = (byte)1;
				}
				else
				{
					this.size = (byte)2;
				}
				this.stackPop = (byte)stackPop;
				this.stackPush = (byte)stackPush;
			}

	// Determine if this opcode is identical to another.
	public override bool Equals(Object obj)
			{
				if(obj is OpCode)
				{
					OpCode other = (OpCode)obj;
					return (flowControl == other.flowControl &&
							opcodeType  == other.opcodeType &&
							operandType == other.operandType &&
							size        == other.size &&
							stackPop    == other.stackPop &&
							stackPush   == other.stackPush &&
							value       == other.value &&
							name        == other.name);
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this opcode.
	public override int GetHashCode()
			{
				return value;
			}

	// Get the flow control for this opcode.
	public FlowControl FlowControl
			{
				get
				{
					return (FlowControl)flowControl;
				}
			}

	// Get the name for this opcode.
	public String Name
			{
				get
				{
					return name;
				}
			}

	// Get the opcode type for this opcode.
	public OpCodeType OpCodeType
			{
				get
				{
					return (OpCodeType)opcodeType;
				}
			}

	// Get the operand type for this opcode.
	public OperandType OperandType
			{
				get
				{
					return (OperandType)operandType;
				}
			}

	// Get the size of the opcode.
	public int Size
			{
				get
				{
					return (int)size;
				}
			}

	// Get the stack pop behaviour for this opcode.
	public StackBehaviour StackBehaviourPop
			{
				get
				{
					return (StackBehaviour)stackPop;
				}
			}

	// Get the stack push behaviour for this opcode.
	public StackBehaviour StackBehaviourPush
			{
				get
				{
					return (StackBehaviour)stackPush;
				}
			}

	// Get the value for this opcode.
	public short Value
			{
				get
				{
					return value;
				}
			}

	// Convert the opcode into a string.
	public override String ToString()
			{
				return Name;
			}

}; // struct OpCode

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
