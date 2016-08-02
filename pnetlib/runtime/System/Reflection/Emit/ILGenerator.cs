/*
 * ILGenerator.cs - Implementation of "System.Reflection.Emit.ILGenerator" 
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
 * 
 * Contributions from Gopal.V <gopalv82@symonds.net> 
 *                    Rhys Weatherley <rweather@southern-storm.com.au>
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

#if CONFIG_REFLECTION_EMIT

namespace System.Reflection.Emit
{

using System;
using System.IO;
using System.Diagnostics.SymbolStore;
using System.Runtime.InteropServices;

public class ILGenerator : IDetachItem
{
	// Internal state.
	private ModuleBuilder module;
	private byte[] code;
	private int offset;
	private int height;
	private int maxHeight;
	private LabelInfo[] labels;
	private int numLabels;
	private SignatureHelper locals;
	private int numLocals;
	private ExceptionTry exceptionStack;
	private ExceptionTry exceptionList;
	private ExceptionTry exceptionListEnd;
	private TokenFixup tokenFixups;

	// Information about a label in the current method.
	private struct LabelInfo
	{
		public int       offset;
		public int       height;
		public LabelRef  refs;

	}; // struct LabelInfo

	// Reference information for back-patching a label.
	private class LabelRef
	{
		public LabelRef	next;
		public int		address;
		public int		switchEnd;

	}; // class LabelRef

	// Information about a token fixup.
	private class TokenFixup
	{
		public TokenFixup	next;
		public int			offset;
		public IntPtr		clrHandle;

	}; // class TokenFixup

	// Information about an exception try block.
	private class ExceptionTry
	{
		public ExceptionTry		next;
		public int				beginTry;
		public int				endTry;
		public int				endCatch;
		public ExceptionClause	clauses;
		public Label			endLabel;

	}; // class ExceptionTry

	// Clause types.
	private const int Except_Catch   = 0;
	private const int Except_Filter  = 1;
	private const int Except_Finally = 2;
	private const int Except_Fault   = 4;

	// Information about an exception clause.
	private class ExceptionClause
	{
		public ExceptionClause	prev;
		public int				clauseType;
		public int				beginClause;
		public int				endClause;
		public Type				classInfo;

	}; // class ExceptionClause

	// Constructor.
	internal ILGenerator(ModuleBuilder module, int size)
			{
				this.module = module;
				if(size < 16)
				{
					size = 16;
				}
				code = new byte [size];
				offset = 0;
				height = 0;
				maxHeight = 0;
				labels = null;
				numLabels = 0;
				locals = null;
				exceptionStack = null;
				exceptionList = null;
				exceptionListEnd = null;
				tokenFixups = null;
				module.assembly.AddDetach(this);
			}
	private ILGenerator(ModuleBuilder module, byte[] explicitBody)
			: this(module, explicitBody.Length)
			{
				Array.Copy(explicitBody, 0, code, 0, explicitBody.Length);
				maxHeight = 8;
			}

	// Terminate the previous exception clause.
	private void TerminateClause()
			{
				if(exceptionStack == null)
				{
					throw new NotSupportedException
						(_("Emit_NeedExceptionBlock"));
				}
				if(exceptionStack.clauses == null)
				{
					// No clauses yet, so terminate the "try" part.
					Emit(OpCodes.Leave, exceptionStack.endLabel);
					exceptionStack.endTry = offset;
					exceptionStack.endCatch = offset;
				}
				else
				{
					exceptionStack.clauses.endClause = offset;
					switch(exceptionStack.clauses.clauseType)
					{
						case Except_Catch:
						{
							Emit(OpCodes.Leave, exceptionStack.endLabel);
							exceptionStack.endCatch = offset;
						}
						break;

						case Except_Filter:
						{
							Emit(OpCodes.Endfilter);
						}
						break;

						case Except_Finally:
						case Except_Fault:
						{
							Emit(OpCodes.Endfinally);
						}
						break;
					}
				}
				height = 0;
			}

	// Begin a catch block on the current exception.
	public virtual void BeginCatchBlock(Type exceptionType)
			{
				// Terminate the current clause.
				TerminateClause();

				// The operation is invalid if current is finally or fault.
				if(exceptionStack.clauses != null)
				{
					if(exceptionStack.clauses.clauseType == Except_Finally ||
					   exceptionStack.clauses.clauseType == Except_Fault)
					{
						throw new InvalidOperationException
							(_("Emit_CatchAfterFinally"));
					}
				}

				// Create a new clause information block.
				ExceptionClause clause = new ExceptionClause();
				clause.prev = exceptionStack.clauses;
				clause.clauseType = Except_Catch;
				clause.beginClause = offset;
				clause.endClause = -1;
				clause.classInfo = exceptionType;
				exceptionStack.clauses = clause;
			}

	// Begin a filter block on the current exception.
	public virtual void BeginExceptFilterBlock()
			{
				// Terminate the current clause.
				TerminateClause();

				// Create a new clause information block.
				ExceptionClause clause = new ExceptionClause();
				clause.prev = exceptionStack.clauses;
				clause.clauseType = Except_Filter;
				clause.beginClause = offset;
				clause.endClause = -1;
				clause.classInfo = null;
				exceptionStack.clauses = clause;
			}

	// Begin the output of an exception block within the current method.
	public virtual Label BeginExceptionBlock()
			{
				ExceptionTry tryBlock = new ExceptionTry();
				tryBlock.next = exceptionStack;
				tryBlock.beginTry = offset;
				tryBlock.endTry = -1;
				tryBlock.endCatch = -1;
				tryBlock.clauses = null;
				tryBlock.endLabel = DefineLabel();
				exceptionStack = tryBlock;
				return tryBlock.endLabel;
			}

	// Begin a fault block on the current exception.
	public virtual void BeginFaultBlock()
			{
				// Terminate the current clause.
				TerminateClause();

				// The operation is invalid if current is finally or fault.
				if(exceptionStack.clauses != null)
				{
					if(exceptionStack.clauses.clauseType == Except_Finally ||
					   exceptionStack.clauses.clauseType == Except_Fault)
					{
						throw new InvalidOperationException
							(_("Emit_CatchAfterFinally"));
					}
				}

				// Create a new clause information block.
				ExceptionClause clause = new ExceptionClause();
				clause.prev = exceptionStack.clauses;
				clause.clauseType = Except_Fault;
				clause.beginClause = offset;
				clause.endClause = -1;
				clause.classInfo = null;
				exceptionStack.clauses = clause;
				height = 1;		// Top of stack is the exception object.
				if(height > maxHeight)
				{
					maxHeight = height;
				}
			}

	// Begin a finally block on the current exception.
	public virtual void BeginFinallyBlock()
			{
				// Terminate the current clause.
				TerminateClause();

				// The operation is invalid if current is finally or fault.
				if(exceptionStack.clauses != null)
				{
					if(exceptionStack.clauses.clauseType == Except_Finally ||
					   exceptionStack.clauses.clauseType == Except_Fault)
					{
						throw new InvalidOperationException
							(_("Emit_CatchAfterFinally"));
					}
				}

				// Create a new clause information block.
				ExceptionClause clause = new ExceptionClause();
				clause.prev = exceptionStack.clauses;
				clause.clauseType = Except_Finally;
				clause.beginClause = offset;
				clause.endClause = -1;
				clause.classInfo = null;
				exceptionStack.clauses = clause;
				height = 1;		// Top of stack is the exception object.
				if(height > maxHeight)
				{
					maxHeight = height;
				}
			}

	// End the output of an exception block.
	public virtual void EndExceptionBlock()
			{
				// Make sure that the request is legal.
				ExceptionTry tryBlock = exceptionStack;
				if(tryBlock == null)
				{
					throw new NotSupportedException
						(_("Emit_NeedExceptionBlock"));
				}
				if(tryBlock.clauses == null)
				{
					throw new InvalidOperationException
						(_("Emit_NoExceptionClauses"));
				}

				// Terminate the last clause in the list.
				TerminateClause();

				// Mark the label for the end of the exception block.
				MarkLabel(tryBlock.endLabel);

				// Add the exception to the end of the real block list.
				exceptionStack = tryBlock.next;
				tryBlock.next = null;
				if(exceptionListEnd != null)
				{
					exceptionListEnd.next = tryBlock;
				}
				else
				{
					exceptionList = tryBlock;
				}
				exceptionListEnd = tryBlock;
			}

	// Enter a lexical naming scope for debug information.
	public virtual void BeginScope()
			{
				// Scopes are not currently used in this implementation.
			}

	// Declare a local variable within the current method.
	public LocalBuilder DeclareLocal(Type localType)
			{
				if(localType == null)
				{
					throw new ArgumentNullException("localType");
				}
				if(locals == null)
				{
					locals = SignatureHelper.GetLocalVarSigHelper(module);
				}
				locals.AddArgument(localType);
				LocalBuilder builder = new LocalBuilder
						(module, localType, numLocals);
				++numLocals;
				return builder;
			}

	// Declare a label within the current method.
	public virtual Label DefineLabel()
			{
				if(labels == null)
				{
					labels = new LabelInfo [8];
				}
				else if(numLabels >= labels.Length)
				{
					LabelInfo[] newLabels = new LabelInfo [numLabels * 2];
					Array.Copy(labels, 0, newLabels, 0, numLabels);
					labels = newLabels;
				}
				return new Label(numLabels++);
			}

	// Emit a single byte to the current method's code.
	private void EmitByte(int value)
			{
				if(offset >= code.Length)
				{
					byte[] newCode = new byte [code.Length * 2];
					Array.Copy(code, 0, newCode, 0, code.Length);
					code = newCode;
				}
				code[offset++] = (byte)value;
			}

	// Emit a token value to the current method's code.
	private void EmitToken(int token)
			{
				EmitByte(token);
				EmitByte(token >> 8);
				EmitByte(token >> 16);
				EmitByte(token >> 24);
			}

	// Emit a token value to the current method's code and register it
	// to have a token fixup at the end of the assembly output process.
	private void EmitTokenWithFixup(int token)
			{
				TokenFixup fixup = new TokenFixup();
				fixup.next = tokenFixups;
				fixup.offset = offset;
				fixup.clrHandle = AssemblyBuilder.ClrGetItemFromToken
						(module.assembly.privateData, token);
				tokenFixups = fixup;
				EmitByte(token);
				EmitByte(token >> 8);
				EmitByte(token >> 16);
				EmitByte(token >> 24);
			}

	// Emit a raw opcode with no stack adjustments.
	private void EmitRawOpcode(int value)
			{
				value &= 0xFFFF;
				if(value < 0x0100)
				{
					EmitByte(value);
				}
				else
				{
					EmitByte(value >> 8);
					EmitByte(value);
				}
			}

	// Emit an opcode value to the current method's code and then
	// adjust the stack height information accordingly.  We use a
	// "ref" parameter to avoid unnecessary data copies in the
	// methods that call this one.
	private void EmitOpcode(ref OpCode opcode)
			{
				// Output the opcode to the instruction stream.
				int value = (opcode.value & 0xFFFF);
				if(value < 0x0100)
				{
					EmitByte(value);
				}
				else
				{
					EmitByte(value >> 8);
					EmitByte(value);
				}

				// Adjust the stack requirements.
				switch((StackBehaviour)(opcode.stackPop))
				{
					case StackBehaviour.Pop0:
					case StackBehaviour.Varpop:
						break;

					case StackBehaviour.Pop1:
					case StackBehaviour.Popi:
					case StackBehaviour.Popref:
						--height;
						break;

					case StackBehaviour.Pop1_pop1:
					case StackBehaviour.Popi_pop1:
					case StackBehaviour.Popi_popi:
					case StackBehaviour.Popi_popi8:
					case StackBehaviour.Popi_popr4:
					case StackBehaviour.Popi_popr8:
					case StackBehaviour.Popref_pop1:
					case StackBehaviour.Popref_popi:
						height -= 2;
						break;

					case StackBehaviour.Popi_popi_popi:
					case StackBehaviour.Popref_popi_popi:
					case StackBehaviour.Popref_popi_popi8:
					case StackBehaviour.Popref_popi_popr4:
					case StackBehaviour.Popref_popi_popr8:
					case StackBehaviour.Popref_popi_popref:
						height -= 3;
						break;

					default: break;
				}
				switch((StackBehaviour)(opcode.stackPush))
				{
					case StackBehaviour.Push0:
						break;

					case StackBehaviour.Push1:
					case StackBehaviour.Pushi:
					case StackBehaviour.Pushi8:
					case StackBehaviour.Pushr4:
					case StackBehaviour.Pushr8:
					case StackBehaviour.Pushref:
					case StackBehaviour.Varpush:
						++height;
						break;

					case StackBehaviour.Push1_push1:
						height += 2;
						break;

					default: break;
				}

				// Update the maximum stack height appropriately.
				if(height > maxHeight)
				{
					maxHeight = height;
				}
				else if(height < 0)
				{
					height = 0;
				}
			}

	// Emit simple opcodes.
	public virtual void Emit(OpCode opcode)
			{
				EmitOpcode(ref opcode);
			}
	public virtual void Emit(OpCode opcode, byte val)
			{
				EmitOpcode(ref opcode);
				EmitByte(val);
			}
	public virtual void Emit(OpCode opcode, short val)
			{
				EmitOpcode(ref opcode);
				EmitByte(val);
				EmitByte(val >> 8);
			}
	public virtual void Emit(OpCode opcode, int val)
			{
				EmitOpcode(ref opcode);
				EmitByte(val);
				EmitByte(val >> 8);
				EmitByte(val >> 16);
				EmitByte(val >> 24);
			}
	public virtual void Emit(OpCode opcode, long val)
			{
				EmitOpcode(ref opcode);
				EmitByte((int)val);
				EmitByte((int)(val >> 8));
				EmitByte((int)(val >> 16));
				EmitByte((int)(val >> 24));
				EmitByte((int)(val >> 32));
				EmitByte((int)(val >> 40));
				EmitByte((int)(val >> 48));
				EmitByte((int)(val >> 56));
			}
	[CLSCompliant(false)]
	public void Emit(OpCode opcode, sbyte val)
			{
				EmitOpcode(ref opcode);
				EmitByte(val);
			}
	public virtual void Emit(OpCode opcode, float val)
			{
				byte[] bytes;
				EmitOpcode(ref opcode);
				bytes = BitConverter.GetLittleEndianBytes(val);
				EmitByte(bytes[0]);
				EmitByte(bytes[1]);
				EmitByte(bytes[2]);
				EmitByte(bytes[3]);
			}
	public virtual void Emit(OpCode opcode, double val)
			{
				byte[] bytes;
				EmitOpcode(ref opcode);
				bytes = BitConverter.GetLittleEndianBytes(val);
				EmitByte(bytes[0]);
				EmitByte(bytes[1]);
				EmitByte(bytes[2]);
				EmitByte(bytes[3]);
				EmitByte(bytes[4]);
				EmitByte(bytes[5]);
				EmitByte(bytes[6]);
				EmitByte(bytes[7]);
			}
	public virtual void Emit(OpCode opcode, String val)
			{
				StringToken token = module.GetStringConstant(val);
				EmitOpcode(ref opcode);
				EmitToken(token.Token);
			}

	// Emit a call on a constructor.
	public virtual void Emit(OpCode opcode, ConstructorInfo constructor)
			{
				// Bail out if "constructor" is null.
				if(constructor == null)
				{
					throw new ArgumentNullException("constructor");
				}

				// Adjust the stack to account for the changes.
				switch((StackBehaviour)(opcode.stackPop))
				{
					case StackBehaviour.Pop0:
						break;

					case StackBehaviour.Varpop:
					{
						if(constructor is ConstructorBuilder)
						{
							height -= ((ConstructorBuilder)constructor).numParams;
						}
						else
						{
							ParameterInfo[] paramList = constructor.GetParameters();
							if(paramList != null)
							{
								height -= paramList.Length;
							}
						}
					}
					break;

					case StackBehaviour.Pop1:
					case StackBehaviour.Popi:
					case StackBehaviour.Popref:
						--height;
						break;

					case StackBehaviour.Pop1_pop1:
					case StackBehaviour.Popi_pop1:
					case StackBehaviour.Popi_popi:
					case StackBehaviour.Popi_popi8:
					case StackBehaviour.Popi_popr4:
					case StackBehaviour.Popi_popr8:
					case StackBehaviour.Popref_pop1:
					case StackBehaviour.Popref_popi:
						height -= 2;
						break;

					case StackBehaviour.Popi_popi_popi:
					case StackBehaviour.Popref_popi_popi:
					case StackBehaviour.Popref_popi_popi8:
					case StackBehaviour.Popref_popi_popr4:
					case StackBehaviour.Popref_popi_popr8:
					case StackBehaviour.Popref_popi_popref:
						height -= 3;
						break;

					default: break;
				}
				switch((StackBehaviour)(opcode.stackPush))
				{
					case StackBehaviour.Push0:
						break;

					case StackBehaviour.Push1:
					case StackBehaviour.Pushi:
					case StackBehaviour.Pushi8:
					case StackBehaviour.Pushr4:
					case StackBehaviour.Pushr8:
					case StackBehaviour.Pushref:
					case StackBehaviour.Varpush:
						++height;
						break;

					case StackBehaviour.Push1_push1:
						height += 2;
						break;

					default: break;
				}

				if(height > maxHeight)
				{
					maxHeight = height;
				}
				else if(height < 0)
				{
					height = 0;
				}

				// Output the instruction.
				MethodToken token = module.GetConstructorToken(constructor);
				EmitRawOpcode(opcode.value);
				EmitTokenWithFixup(token.Token);
			}

	// Emit a reference to a field
	public virtual void Emit(OpCode opcode, FieldInfo field)
			{
				FieldToken token = module.GetFieldToken(field);
				EmitOpcode(ref opcode);
				EmitTokenWithFixup(token.Token);
			}

	// Emit code for a branch instruction.  Note: unlike other implementations,
	// we always output the branch in such a way that an out of range error
	// can never occur.  This makes it easier to process forward references
	// in a single pass through the code.
	public virtual void Emit(OpCode opcode, Label label)
			{
				int index = label.index;
				int shortForm, longForm;
				if(index < 0 || index >= numLabels)
				{
					return;
				}
				if((OperandType)(opcode.operandType) ==
						OperandType.ShortInlineBrTarget)
				{
					// Convert a short opcode into its long form.
					shortForm = opcode.value;
					if(shortForm >= 0x2B && shortForm <= 0x37)
					{
						longForm = shortForm - 0x2B + 0x38;
					}
					else
					{
						longForm = 0xDD;
					}
				}
				else if((OperandType)(opcode.operandType) ==
							OperandType.InlineBrTarget)
				{
					// Convert a long opcode into its short form.
					longForm = opcode.value;
					if(longForm >= 0x38 && longForm <= 0x44)
					{
						shortForm = longForm + 0x2B - 0x38;
					}
					else
					{
						shortForm = 0xDE;
					}
				}
				else
				{
					// Ignore non-branch opcodes.
					return;
				}
				if(labels[index].offset != 0)
				{
					// The label is already defined.  Determine if the
					// branch is long or short.
					int dest = (labels[index].offset - 1) - (offset + 2);
					if(dest >= -128 && dest <= 127)
					{
						EmitByte(shortForm);
						EmitByte(dest);
					}
					else
					{
						dest = (labels[index].offset - 1) - (offset + 5);
						EmitByte(longForm);
						EmitByte(dest);
						EmitByte(dest >> 8);
						EmitByte(dest >> 16);
						EmitByte(dest >> 24);
					}
				}
				else
				{
					// Output the long form and add a reference to the label.
					EmitByte(longForm);
					EmitByte(0);
					EmitByte(0);
					EmitByte(0);
					EmitByte(0);
					LabelRef newRef = new LabelRef();
					newRef.next = labels[index].refs;
					newRef.address = offset - 4;
					newRef.switchEnd = -1;
					labels[index].refs = newRef;
				}
				switch((StackBehaviour)(opcode.stackPop))
				{
					case StackBehaviour.Popi:
						--height;
						break;

					case StackBehaviour.Pop1_pop1:
						height -= 2;
						break;

					default: break;
				}
			}

	// Emit a switch statement.
	public virtual void Emit(OpCode opcode, Label[] labels)
			{
				// Determine where the switch statement ends.
				int switchEnd = offset + opcode.size + labels.Length * 4;

				// Emit the opcode and the table length.
				EmitOpcode(ref opcode);
				EmitToken(labels.Length);

				// Output the table of switch labels.
				int posn;
				Label label;
				LabelInfo info;
				LabelRef newRef;
				for(posn = 0; posn < labels.Length; ++posn)
				{
					// Skip the label if it is invalid (shouldn't happen).
					label = labels[posn];
					if(label.index < 0 || label.index >= numLabels)
					{
						continue;
					}

					// Fetch the label information block.
					info = this.labels[label.index];

					// If it is already defined, output the offset now.
					// Otherwise add a reference and output a placeholder.
					if(info.offset != 0)
					{
						EmitToken(info.offset - switchEnd);
					}
					else
					{
						newRef = new LabelRef();
						newRef.next = this.labels[label.index].refs;
						newRef.address = offset;
						newRef.switchEnd = switchEnd;
						this.labels[label.index].refs = newRef;
						EmitToken(0);
					}
				}
			}

	// Emit a reference to a local variable.
	public virtual void Emit(OpCode opcode, LocalBuilder lbuilder)
			{
				// Validate the parameters.
				if(lbuilder == null)
				{
					throw new ArgumentNullException("lbuilder");
				}

				// Determine if we can squash the instruction a bit more.
				int index = lbuilder.index;
				if(opcode.value == unchecked((short)0xFE0C)) // "ldloc"
				{
					if(index == 0)
					{
						opcode = OpCodes.Ldloc_0;
					}
					else if(index == 1)
					{
						opcode = OpCodes.Ldloc_1;
					}
					else if(index == 2)
					{
						opcode = OpCodes.Ldloc_2;
					}
					else if(index == 3)
					{
						opcode = OpCodes.Ldloc_3;
					}
					else if(index < 0x0100)
					{
						opcode = OpCodes.Ldloc_S;
					}
				}
				else if(opcode.value == unchecked((short)0xFE0D)) // "ldloca"
				{
					if(index < 0x0100)
					{
						opcode = OpCodes.Ldloca_S;
					}
				}
				else if(opcode.value == unchecked((short)0xFE0E)) // "stloc"
				{
					if(index == 0)
					{
						opcode = OpCodes.Stloc_0;
					}
					else if(index == 1)
					{
						opcode = OpCodes.Stloc_1;
					}
					else if(index == 2)
					{
						opcode = OpCodes.Stloc_2;
					}
					else if(index == 3)
					{
						opcode = OpCodes.Stloc_3;
					}
					else if(index < 0x0100)
					{
						opcode = OpCodes.Stloc_S;
					}
				}

				// Output the instruction and its argument.
				EmitOpcode(ref opcode);
				if(opcode.operandType == (int)(OperandType.ShortInlineVar))
				{
					EmitByte(index);
				}
				else if(opcode.operandType == (int)(OperandType.InlineVar))
				{
					EmitByte(index);
					EmitByte(index >> 8);
				}
			}

	// Emit an instruction that refers to a method.
	public virtual void Emit(OpCode opcode, MethodInfo method)
			{
				// Bail out if "method" is null.
				if(method == null)
				{
					throw new ArgumentNullException("method");
				}

				// Adjust the stack to account for the changes.
				switch((StackBehaviour)(opcode.stackPop))
				{
					case StackBehaviour.Pop0:
						break;

					case StackBehaviour.Varpop:
					{
						if(method is MethodBuilder)
						{
							height -= ((MethodBuilder)method).numParams;
						}
						else
						{
							ParameterInfo[] paramList = method.GetParameters();
							if(paramList != null)
							{
								height -= paramList.Length;
							}
						}
						if(!method.IsStatic && opcode.value != 0x73) // "newobj"
						{
							--height;
						}
					}
					break;

					case StackBehaviour.Pop1:
					case StackBehaviour.Popi:
					case StackBehaviour.Popref:
						--height;
						break;

					case StackBehaviour.Pop1_pop1:
					case StackBehaviour.Popi_pop1:
					case StackBehaviour.Popi_popi:
					case StackBehaviour.Popi_popi8:
					case StackBehaviour.Popi_popr4:
					case StackBehaviour.Popi_popr8:
					case StackBehaviour.Popref_pop1:
					case StackBehaviour.Popref_popi:
						height -= 2;
						break;

					case StackBehaviour.Popi_popi_popi:
					case StackBehaviour.Popref_popi_popi:
					case StackBehaviour.Popref_popi_popi8:
					case StackBehaviour.Popref_popi_popr4:
					case StackBehaviour.Popref_popi_popr8:
					case StackBehaviour.Popref_popi_popref:
						height -= 3;
						break;

					default: break;
				}
				switch((StackBehaviour)(opcode.stackPush))
				{
					case StackBehaviour.Push0:
						break;

					case StackBehaviour.Push1:
					case StackBehaviour.Pushi:
					case StackBehaviour.Pushi8:
					case StackBehaviour.Pushr4:
					case StackBehaviour.Pushr8:
					case StackBehaviour.Pushref:
						++height;
						break;

					case StackBehaviour.Varpush:
					{
						if(method.ReturnType != typeof(void))
						{
							++height;
						}
					}
					break;

					case StackBehaviour.Push1_push1:
						height += 2;
						break;

					default: break;
				}

				if(height > maxHeight)
				{
					maxHeight = height;
				}
				else if(height < 0)
				{
					height = 0;
				}

				// Output the instruction.
				MethodToken token = module.GetMethodToken(method);
				EmitRawOpcode(opcode.value);
				EmitTokenWithFixup(token.Token);
			}

	// Emit an instruction that takes a signature token as an argument.
	public virtual void Emit(OpCode opcode, SignatureHelper shelper)
			{
				// Emit the instruction.
				SignatureToken token = module.GetSignatureToken(shelper);
				EmitOpcode(ref opcode);
				EmitToken(token.Token);

				// Adjust the stack height for the "calli" instruction.
				if(opcode.stackPop == (int)(StackBehaviour.Varpop))
				{
					height -= shelper.numArgs + 1;
					if(height < 0)
					{
						height = 0;
					}
				}
			}

	// Emit an instruction that refers to a type.
	public virtual void Emit(OpCode opcode, Type type)
			{
				TypeToken token = module.GetTypeToken(type);
				EmitOpcode(ref opcode);
				EmitTokenWithFixup(token.Token);
			}

	// Emit an instruction to call a method with vararg parameters.
	public void EmitCall(OpCode opcode, MethodInfo methodInfo,
						 Type[] optionalParamTypes)
			{
				// Call the method call directly if no optional parameters.
				if(optionalParamTypes == null ||
				   optionalParamTypes.Length == 0)
				{
					Emit(opcode, methodInfo);
					return;
				}

				// Get the method's token, which takes care of importing.
				MethodToken token = module.GetMethodToken(methodInfo);

				// Make a copy of the method's signature and adjust it.
				SignatureHelper helper =
					SignatureHelper.GetMethodSigHelper(module, token);
				helper.AddSentinel();
				foreach(Type type in optionalParamTypes)
				{
					helper.AddArgument(type);
				}

				// Create a new token for the vararg member reference.
				int refToken = MethodBuilder.ClrMethodCreateVarArgRef
						(module.privateData, token.Token, helper.sig);

				// Emit the raw instruction.
				EmitRawOpcode(opcode.value);
				EmitTokenWithFixup(refToken);

				// Adjust the stack to account for the changes.
				if(opcode.stackPush == (int)(StackBehaviour.Varpush))
				{
					if(methodInfo.ReturnType != typeof(void))
					{
						++height;
					}
				}
				if(opcode.stackPop == (int)(StackBehaviour.Varpop))
				{
					height -= optionalParamTypes.Length;
					if(methodInfo is MethodBuilder)
					{
						height -= ((MethodBuilder)methodInfo).numParams;
					}
					else
					{
						ParameterInfo[] paramList = methodInfo.GetParameters();
						if(paramList != null)
						{
							height -= paramList.Length;
						}
					}
					if(!methodInfo.IsStatic && opcode.value != 0x73) // "newobj"
					{
						--height;
					}
				}
				if(height > maxHeight)
				{
					maxHeight = height;
				}
				else if(height < 0)
				{
					height = 0;
				}
			}

	// Emit an indirect call instruction.
	public void EmitCalli(OpCode opcode, CallingConventions callConv,
						  Type returnType, Type[] paramTypes,
						  Type[] optionalParamTypes)
			{
				// Check the calling convention.
				if(optionalParamTypes != null)
				{
					if((callConv & CallingConventions.VarArgs) == 0)
					{
						throw new InvalidOperationException
							(_("Emit_VarArgsWithNonVarArgMethod"));
					}
				}

				// Build the full signature.
				SignatureHelper helper =
					SignatureHelper.GetMethodSigHelper
						(module, callConv, (CallingConvention)0,
						 returnType, paramTypes);
				if(optionalParamTypes != null)
				{
					helper.AddSentinel();
					foreach(Type type in optionalParamTypes)
					{
						helper.AddArgument(type);
					}
				}

				// Emit the instruction using the constructed signature.
				Emit(opcode, helper);
			}
	public void EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv,
						  Type returnType, Type[] paramTypes)
			{
				// Build the full signature.
				SignatureHelper helper =
					SignatureHelper.GetMethodSigHelper
						(module, CallingConventions.Standard,
						 unmanagedCallConv, returnType, paramTypes);

				// Emit the instruction using the constructed signature.
				Emit(opcode, helper);
			}

	// Exit a lexical naming scope for debug information.
	public virtual void EndScope()
			{
				// Scopes are not currently used in this implementation.
			}

	// Mark a label as existing at this point within the code.
	public virtual void MarkLabel(Label loc)
			{
				// Validate the label identifier.
				int index = loc.index;
				if(index < 0 || index >= numLabels)
				{
					throw new ArgumentException(_("Emit_InvalidLabel"));
				}
				else if(labels[index].offset != 0)
				{
					throw new ArgumentException(_("Emit_LabelAlreadyDefined"));
				}

				// Update the label information.
				if(labels[index].height > height)
				{
					height = labels[index].height;
				}
				labels[index].offset = offset + 1;

				// Perform fixups on all existing references to the label.
				LabelRef refs = labels[index].refs;
				labels[index].refs = null;
				int address, switchEnd, dest;
				while(refs != null)
				{
					address = refs.address;
					switchEnd = refs.switchEnd;
					if(switchEnd == -1)
					{
						// Back-patch an ordinary long jump.
						dest = offset - (address + 4);
					}
					else
					{
						// Back-patch an entry in a switch table.
						dest = offset - switchEnd;
					}
					code[address]     = (byte)dest;
					code[address + 1] = (byte)(dest >> 8);
					code[address + 2] = (byte)(dest >> 16);
					code[address + 3] = (byte)(dest >> 24);
					refs = refs.next;
				}
			}

	// Mark a sequence point within the debug information.
	public virtual void MarkSequencePoint
				(ISymbolDocumentWriter document, int startLine,
				 int startColumn, int endLine, int endColumn)
			{
				// Sequence points are not currently used.
			}

	// Short-cut helper methods.  Not yet implemented or used.
	public virtual void EmitWriteLine(FieldInfo field)
			{
				Type fieldType;
				MethodInfo method;
				Type[] paramList;

				// Validate the parameter.
				if(field == null)
				{
					throw new ArgumentNullException("field");
				}
				fieldType = field.FieldType;
				if(fieldType is TypeBuilder || fieldType is EnumBuilder)
				{
					throw new NotSupportedException(_("NotSupp_Builder"));
				}

				// Push the "Out" stream reference onto the stack.
				method = typeof(Console).GetMethod("get_Out");
				Emit(OpCodes.Call, method);

				// Load the field value onto the stack.
				if(field.IsStatic)
				{
					Emit(OpCodes.Ldsfld, field);
				}
				else
				{
					Emit(OpCodes.Ldarg_0);
					Emit(OpCodes.Ldfld, field);
				}

				// Find and call the "WriteLine" method.
				paramList = new Type [1];
				paramList[0] = fieldType;
				method = typeof(TextWriter).GetMethod("WriteLine", paramList);
				if(method == null)
				{
					throw new ArgumentException(_("Emit_MissingWriteLine"));
				}
				Emit(OpCodes.Callvirt, method);
			}
	public virtual void EmitWriteLine(LocalBuilder lbuilder)
			{
				Type localType;
				MethodInfo method;
				Type[] paramList;

				// Validate the parameter.
				if(lbuilder == null)
				{
					throw new ArgumentNullException("lbuilder");
				}
				localType = lbuilder.LocalType;
				if(localType is TypeBuilder || localType is EnumBuilder)
				{
					throw new NotSupportedException(_("NotSupp_Builder"));
				}

				// Push the "Out" stream reference onto the stack.
				method = typeof(Console).GetMethod("get_Out");
				Emit(OpCodes.Call, method);

				// Load the local variable's value onto the stack.
				Emit(OpCodes.Ldloc, lbuilder);

				// Find and call the "WriteLine" method.
				paramList = new Type [1];
				paramList[0] = localType;
				method = typeof(TextWriter).GetMethod("WriteLine", paramList);
				if(method == null)
				{
					throw new ArgumentException(_("Emit_MissingWriteLine"));
				}
				Emit(OpCodes.Callvirt, method);
			}
	public virtual void EmitWriteLine(String val)
			{
				Type[] paramList;
				MethodInfo method;

				// Locate the "Console.WriteLine(String)" method.
				paramList = new Type [1];
				paramList[0] = typeof(String);
				method = typeof(Console).GetMethod("WriteLine", paramList);

				// Output the code to call the method.
				Emit(OpCodes.Ldstr, val);
				Emit(OpCodes.Call, method);
			}
	public virtual void ThrowException(Type exceptionType)
			{
				// Validate the parameter.
				if(exceptionType == null)
				{
					throw new ArgumentNullException("exceptionType");
				}
				else if(!exceptionType.IsSubclassOf(typeof(Exception)) &&
						exceptionType != typeof(Exception))
				{
					throw new ArgumentException
						(_("Emit_NotAnExceptionType"));
				}

				// Locate the zero-argument constructor.
				ConstructorInfo constructor;
				constructor = exceptionType.GetConstructor(Type.EmptyTypes);
				if(constructor == null)
				{
					throw new ArgumentException
						(_("Emit_NeedDefaultConstructor"));
				}

				// Create and throw the exception.
				Emit(OpCodes.Newobj, constructor);
				Emit(OpCodes.Throw);
			}
	public void UsingNamespace(String usingNamespace)
			{
				// Namespace debug information not currently used,
				// so simply validate the parameter and exit.
				if(usingNamespace == null)
				{
					throw new ArgumentNullException("usingNamespace");
				}
				else if(usingNamespace.Length == 0)
				{
					throw new ArgumentException(_("Emit_NameEmpty"));
				}
			}

	// Detach this item.
	void IDetachItem.Detach()
			{
				tokenFixups = null;
			}

	// Finish processing the labels.
	// Returns true if we need fat exception blocks.
	private bool FinishLabels()
			{
				bool fatExceptions = false;

				// Do we need fat exception blocks?
				ExceptionTry exception = exceptionList;
				while (exception != null)
				{
					if (exception.beginTry > 0xFFFF ||
					    exception.endTry > 0xFFFF ||
					    exception.endTry - exception.beginTry > 0xFF ||
					    exception.endCatch - exception.endTry > 0xFF)
					{
						fatExceptions = true;
						break;
					}
					exception = exception.next;
				}

				for (int i = 0; i < numLabels; ++i)
				{
					// Write the final offsets into all branch instructions
					LabelRef label = labels[i].refs;
					while (label != null)
					{
						int delta;
						if (label.switchEnd != -1)
						{
							delta = label.address - label.switchEnd;
						}
						else
						{
							delta = label.address - labels[i].offset + 4;
						}
						code[labels[i].offset] = (byte)delta;
						code[labels[i].offset + 1] = (byte)(delta >> 8);
						code[labels[i].offset + 2] = (byte)(delta >> 16);
						code[labels[i].offset + 3] = (byte)(delta >> 24);
						label = label.next;
					}
				}

				return fatExceptions;
			}

	// Copy the fixup data to the given arrays.
	private static void FixupsToArrays
				(ref IntPtr[] ptrs, ref int[] offsets, TokenFixup fixups)
			{
				if (fixups == null) { return; }

				System.Collections.ArrayList p = new System.Collections.ArrayList();
				System.Collections.ArrayList o = new System.Collections.ArrayList();
				while (fixups != null)
				{
					p.Add(fixups.clrHandle);
					o.Add(fixups.offset);
					fixups = fixups.next;
				}
				ptrs = (IntPtr[])p.ToArray(typeof(IntPtr));
				offsets = (int[])o.ToArray(typeof(int));
			}

	// Build the method header.
	private byte[] BuildHeader(bool initLocals)
			{
				byte[] header;

				if (offset < 0x40 && locals == null &&
				    exceptionList == null && maxHeight <= 2)
				{
					// Use the tiny format
					header = new byte[1];
					header[0] = ((byte)((offset << 2) | 0x02));
				}
				else
				{
					// Use the fat format
					header = new byte[12];
					header[0] = (byte)(initLocals ? 0x13 : 0x03);
					if (exceptionList != null)
					{
						// There will be more sections following the method code
						header[0] |= (byte)0x08;
					}
					header[1] = (byte)0x30;
					header[2] = (byte)(maxHeight);
					header[3] = (byte)(maxHeight >> 8);
					header[4] = (byte)(offset);
					header[5] = (byte)(offset >> 8);
					header[6] = (byte)(offset >> 16);
					header[7] = (byte)(offset >> 24);
					if (locals != null)
					{
						int token = locals.StandAloneToken();
						header[8]  = (byte)(token);
						header[9]  = (byte)(token >> 8);
						header[10] = (byte)(token >> 16);
						header[11] = (byte)(token >> 24);
					}
					else
					{
						header[8]  = (byte)0x00;
						header[9]  = (byte)0x00;
						header[10] = (byte)0x00;
						header[11] = (byte)0x00;
					}
				}

				return header;
			}

	// Build the method's exception blocks.
	private void BuildExceptionBlocks
				(ref byte[][] e, ref TokenFixup fixups, bool fatExceptions)
			{
				if (exceptionList == null) { return; }

				// Count the number of exception blocks
				int numExceptions = 0;
				ExceptionTry exception = exceptionList;
				while (exception != null)
				{
					ExceptionClause clause = exception.clauses;
					while (clause != null)
					{
						++numExceptions;
						clause = clause.prev;
					}
					exception = exception.next;
				}

				// Switch formats if the section size is too big for tiny
				if (!fatExceptions && ((numExceptions * 12) + 4) > 0xFF)
				{
					fatExceptions = true;
				}

				e = new byte[numExceptions+1][];
				int index = 0;

				// What type of exception header should we use?
				if (fatExceptions)
				{
					// Use the fat format for the exception information
					int tmp = (numExceptions * 24) + 4;
					e[index] = new byte[4];
					e[index][0] = (byte)0x41;
					e[index][1] = (byte)(tmp);
					e[index][2] = (byte)(tmp >> 8);
					e[index][3] = (byte)(tmp >> 16);
					++index;
					exception = exceptionList;
					while (exception != null)
					{
						ExceptionClause clause = exception.clauses;
						while (clause != null)
						{
							int flags;
							int tryOffset;
							int tryLength;
							int handlerOffset;
							int handlerLength;

							flags = clause.clauseType;
							tryOffset = exception.beginTry;
							tryLength = exception.endTry - tryOffset;
							handlerOffset = clause.beginClause;
							handlerLength = clause.endClause - handlerOffset;

							e[index] = new byte[24];

							e[index][0]  = (byte)(flags);
							e[index][1]  = (byte)(flags >> 8);
							e[index][2]  = (byte)(flags >> 16);
							e[index][3]  = (byte)(flags >> 24);
							e[index][4]  = (byte)(tryOffset);
							e[index][5]  = (byte)(tryOffset >> 8);
							e[index][6]  = (byte)(tryOffset >> 16);
							e[index][7]  = (byte)(tryOffset >> 24);
							e[index][8]  = (byte)(tryLength);
							e[index][9]  = (byte)(tryLength >> 8);
							e[index][10] = (byte)(tryLength >> 16);
							e[index][11] = (byte)(tryLength >> 24);
							e[index][12] = (byte)(handlerOffset);
							e[index][13] = (byte)(handlerOffset >> 8);
							e[index][14] = (byte)(handlerOffset >> 16);
							e[index][15] = (byte)(handlerOffset >> 24);
							e[index][16] = (byte)(handlerLength);
							e[index][17] = (byte)(handlerLength >> 8);
							e[index][18] = (byte)(handlerLength >> 16);
							e[index][19] = (byte)(handlerLength >> 24);
							if (flags == Except_Filter)
							{
								// ?? TODO ??
								e[index][20] = (byte)(handlerOffset);
								e[index][21] = (byte)(handlerOffset >> 8);
								e[index][22] = (byte)(handlerOffset >> 16);
								e[index][23] = (byte)(handlerOffset >> 24);
							}
							else if (clause.classInfo != null)
							{
								int token;
								IntPtr handle;
								IClrProgramItem item;

								item = (IClrProgramItem)clause.classInfo;
								handle = item.ClrHandle;
								token = AssemblyBuilder.ClrGetItemToken(handle);

								e[index][20] = (byte)(token);
								e[index][21] = (byte)(token >> 8);
								e[index][22] = (byte)(token >> 16);
								e[index][23] = (byte)(token >> 24);

								TokenFixup fixup = new TokenFixup();
								fixup.next = fixups;
								fixup.offset = index*24;
								fixup.clrHandle = handle;
								fixups = fixup;
							}
							else
							{
								e[index][20] = (byte)0x00;
								e[index][21] = (byte)0x00;
								e[index][22] = (byte)0x00;
								e[index][23] = (byte)0x00;
							}
							clause = clause.prev;
							++index;
						}
						exception = exception.next;
					}
				}
				else
				{
					// Use the tiny format for the exception information
					int tmp = (numExceptions * 12) + 4;
					e[index] = new byte[4];
					e[index][0] = (byte)0x01;
					e[index][1] = (byte)tmp;
					e[index][2] = (byte)0x00;
					e[index][3] = (byte)0x00;
					++index;
					exception = exceptionList;
					while(exception != null)
					{
						ExceptionClause clause = exception.clauses;
						while (clause != null)
						{
							int flags;
							int tryOffset;
							int tryLength;
							int handlerOffset;
							int handlerLength;

							flags = clause.clauseType;
							tryOffset = exception.beginTry;
							tryLength = exception.endTry - tryOffset;
							handlerOffset = clause.beginClause;
							handlerLength = clause.endClause - handlerOffset;

							e[index] = new byte[12];

							e[index][0] = (byte)(flags);
							e[index][1] = (byte)(flags >> 8);
							e[index][2] = (byte)(tryOffset);
							e[index][3] = (byte)(tryOffset >> 8);
							e[index][4] = (byte)(tryLength);
							e[index][5] = (byte)(handlerOffset);
							e[index][6] = (byte)(handlerOffset >> 8);
							e[index][7] = (byte)(handlerLength);
							if (flags == Except_Filter)
							{
								// ?? TODO ??
								e[index][8]  = (byte)(handlerOffset);
								e[index][9]  = (byte)(handlerOffset >> 8);
								e[index][10] = (byte)(handlerOffset >> 16);
								e[index][11] = (byte)(handlerOffset >> 24);
							}
							else if (clause.classInfo != null)
							{
								int token;
								IntPtr handle;
								IClrProgramItem item;

								item = (IClrProgramItem)clause.classInfo;
								handle = item.ClrHandle;
								token = AssemblyBuilder.ClrGetItemToken(handle);

								e[index][8]  = (byte)(token);
								e[index][9]  = (byte)(token >> 8);
								e[index][10] = (byte)(token >> 16);
								e[index][11] = (byte)(token >> 24);

								TokenFixup fixup = new TokenFixup();
								fixup.next = fixups;
								fixup.offset = index*12;
								fixup.clrHandle = handle;
								fixups = fixup;
							}
							else
							{
								e[index][8]  = (byte)0x00;
								e[index][9]  = (byte)0x00;
								e[index][10] = (byte)0x00;
								e[index][11] = (byte)0x00;
							}
							clause = clause.prev;
							++index;
						}
						exception = exception.next;
					}
				}
			}

	// Write the contents of this generator to the code section
	// and return the RVA that corresponds to it.
	internal int WriteCode(bool initLocals)
			{
				// Finish the label processing for the method
				bool fatExceptions = FinishLabels();

				// Create the method header
				byte[] header = BuildHeader(initLocals);

				// Output the exception information if necessary
				byte[][] e = null;
				TokenFixup eFixups = null;
				BuildExceptionBlocks(ref e, ref eFixups, fatExceptions);

				// Get the fixup data for the code section
				IntPtr[] cFixupPtrs = null;
				int[] cFixupOffsets = null;
				FixupsToArrays(ref cFixupPtrs, ref cFixupOffsets, tokenFixups);

				// Get the fixup data for the exception blocks
				IntPtr[] eFixupPtrs = null;
				int[] eFixupOffsets = null;
				FixupsToArrays(ref eFixupPtrs, ref eFixupOffsets, eFixups);

				// Get the trimmed down code array
				byte[] c = new byte[offset];
				Array.Copy(code, c, offset);

				// Pass the rest of the work off to the assembly builder
				return module.assembly.WriteMethod(header,
				                                   c,
				                                   cFixupPtrs,
				                                   cFixupOffsets,
				                                   e,
				                                   eFixupPtrs,
				                                   eFixupOffsets);
			}

	// Write an explicit method body to the code section and
	// return the RVA that corresponds to it.
	internal static int WriteExplicitCode
				(ModuleBuilder module, byte[] explicitBody, bool initLocals)
			{
				ILGenerator ilgen = new ILGenerator(module, explicitBody);
				return ilgen.WriteCode(initLocals);
			}

}; // class ILGenerator.cs

}; // namespace System.Reflection.Emit

#endif // CONFIG_REFLECTION_EMIT
