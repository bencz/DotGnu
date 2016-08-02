/*
 * JScriptException.cs - Exception that may be thrown by the JScript engine.
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
using System.Runtime.Serialization;
using Microsoft.Vsa;

#if !ECMA_COMPAT
[Serializable]
#endif
public class JScriptException : ApplicationException, IVsaError
{
	// Internal state.
	internal JSError errorNumber;
	internal Object wrappedException;
	internal Context context;
	internal String message;

	// Constructors.
	internal JScriptException(JSError errorNumber)
			{
				this.errorNumber = errorNumber;
				this.wrappedException = null;
				this.context = null;
				this.message = null;
			}
	internal JScriptException(JSError errorNumber, Context context)
			{
				this.errorNumber = errorNumber;
				this.wrappedException = null;
				this.context = context;
				this.message = null;
			}
	internal JScriptException(Object value)
			{
				this.wrappedException = value;
				if(value is StackOverflowException)
				{
					this.errorNumber = JSError.OutOfStack;
				}
				else if(value is OutOfMemoryException)
				{
					this.errorNumber = JSError.OutOfMemory;
				}
				else
				{
					this.errorNumber = JSError.UncaughtException;
				}
				this.context = null;
				this.message = null;
			}
#if CONFIG_SERIALIZATION
	protected JScriptException(SerializationInfo info,
							   StreamingContext context)
			{
				// TODO
			}
	public override void GetObjectData(SerializationInfo info,
									   StreamingContext context)
			{
				// TODO
			}
#endif

	// Properties.
	public int Column
			{
				get
				{
					if(context != null)
					{
						return context.StartColumn + 1;
					}
					else
					{
						return 0;
					}
				}
			}
	public String Description
			{
				get
				{
					return Message;
				}
			}
	public int EndColumn
			{
				get
				{
					if(context != null)
					{
						return context.EndColumn + 1;
					}
					else
					{
						return 0;
					}
				}
			}
	public int EndLine
			{
				get
				{
					if(context != null)
					{
						return context.endLine;
					}
					else
					{
						return 0;
					}
				}
			}
	public int ErrorNumber
			{
				get
				{
					return unchecked(((int)errorNumber) + (int)0x800A0000);
				}
			}
	public int Line
			{
				get
				{
					if(context != null)
					{
						return context.StartLine;
					}
					else
					{
						return 0;
					}
				}
			}
	public String LineText
			{
				get
				{
					if(context != null)
					{
						return context.GetCode();
					}
					else
					{
						return String.Empty;
					}
				}
			}
	public override String Message
			{
				get
				{
					if(message != null)
					{
						if(context != null)
						{
							if(context.codebase != null &&
							   context.codebase.name != null)
							{
								return String.Format
									("{0}: line {1}, column {2}: {3}",
								     context.codebase.name,
									 Line, Column, message);
							}
							else
							{
								return String.Format
									("line {0}, column {1}: {2}",
								     Line, Column, message);
							}
						}
						else
						{
							return message;
						}
					}
					else
					{
						return errorNumber.ToString();
					}
				}
			}
	public int Number
			{
				get
				{
					return ErrorNumber;
				}
			}
	public int Severity
			{
				get
				{
					switch(errorNumber)
					{
						case JSError.ArrayMayBeCopied:
						case JSError.AssignmentToReadOnly:
						case JSError.BadOctalLiteral:
						case JSError.BaseClassIsExpandoAlready:
						case JSError.DifferentReturnTypeFromBase:
						case JSError.DuplicateName:
						case JSError.DupVisibility:
						case JSError.GetAndSetAreInconsistent:
						case JSError.HidesParentMember:
						case JSError.IncompatibleVisibility:
						case JSError.NewNotSpecifiedInMethodDeclaration:
						case JSError.NotDeletable:
						case JSError.NotMeantToBeCalledDirectly:
						case JSError.PossibleBadConversion:
						case JSError.ShouldBeAbstract:
						case JSError.SuspectAssignment:
						case JSError.SuspectLoopCondition:
						case JSError.SuspectSemicolon:
						case JSError.TooFewParameters:
						case JSError.TooManyParameters:
						case JSError.UselessAssignment:
						case JSError.UselessExpression:
							return 1;

						case JSError.Deprecated:
						case JSError.KeywordUsedAsIdentifier:
						case JSError.OctalLiteralsAreDeprecated:
							return 2;

						case JSError.BadWayToLeaveFinally:
						case JSError.StringConcatIsSlow:
						case JSError.UndeclaredVariable:
						case JSError.VariableLeftUninitialized:
						case JSError.VariableMightBeUnitialized:
							return 3;

						case JSError.AmbiguousBindingBecauseOfWith:
						case JSError.AmbiguousBindingBecauseOfEval:
						case JSError.PossibleBadConversionFromString:
							return 4;

						default: break;
					}
					return 0;
				}
			}
	public IVsaItem SourceItem
			{
				get
				{
					if(context != null && context.codebase != null)
					{
						return context.codebase.item;
					}
					else
					{
						throw new NoContextException();
					}
				}
			}
	public String SourceMoniker
			{
				get
				{
					if(context != null && context.codebase != null)
					{
						return context.codebase.name;
					}
					else
					{
						return "no source";
					}
				}
			}
	public override String StackTrace
			{
				get
				{
					return Message + Environment.NewLine + base.StackTrace;
				}
			}
	public int StartColumn
			{
				get
				{
					return Column;
				}
			}

}; // class JScriptException

}; // namespace Microsoft.JScript
