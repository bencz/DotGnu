/*
 * Support.cs - Random support routines.
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
using Microsoft.JScript.Vsa;

internal sealed class Support
{
	// Get the type code for an object value.
	public static TypeCode TypeCodeForObject(Object value)
			{
				if(value == null)
				{
					return TypeCode.Empty;
				}
			#if !ECMA_COMPAT
				if(value is IConvertible)
				{
					return ((IConvertible)value).GetTypeCode();
				}
				else
				{
					return TypeCode.Object;
				}
			#else
				if(value is DBNull)
				{
					return TypeCode.DBNull;
				}
				else if(value is Boolean)
				{
					return TypeCode.Boolean;
				}
				else if(value is Char)
				{
					return TypeCode.Char;
				}
				else if(value is SByte)
				{
					return TypeCode.SByte;
				}
				else if(value is Byte)
				{
					return TypeCode.Byte;
				}
				else if(value is Int16)
				{
					return TypeCode.Int16;
				}
				else if(value is UInt16)
				{
					return TypeCode.UInt16;
				}
				else if(value is Int32)
				{
					return TypeCode.Int32;
				}
				else if(value is UInt32)
				{
					return TypeCode.UInt32;
				}
				else if(value is Int64)
				{
					return TypeCode.Int64;
				}
				else if(value is UInt64)
				{
					return TypeCode.UInt64;
				}
				else if(value is Single)
				{
					return TypeCode.Single;
				}
				else if(value is Double)
				{
					return TypeCode.Double;
				}
				else if(value is Decimal)
				{
					return TypeCode.Decimal;
				}
				else if(value is DateTime)
				{
					return TypeCode.DateTime;
				}
				else if(value is String)
				{
					return TypeCode.String;
				}
				else
				{
					return TypeCode.Object;
				}
			#endif
			}

	// Get the type of an object, in JScript parlance.
	public static String Typeof(Object value)
			{
				switch(TypeCodeForObject(value))
				{
					case TypeCode.Empty:	break;

					case TypeCode.Object:
					{
						if(value is Microsoft.JScript.Missing
					#if ECMA_COMPAT
						  )
					#else
						   || value is System.Reflection.Missing)
					#endif
						{
							return "undefined";
						}
						else if(value is ScriptFunction)
						{
							return "function";
						}
						else if(value is JSObject)
						{
							return ((JSObject)value).Class.ToLower();
						}
						else
						{
							return "object";
						}
					}
					// Not reached.

					case TypeCode.DBNull:	return "object";
					case TypeCode.Boolean:	return "boolean";
					case TypeCode.SByte:
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.UInt16:
					case TypeCode.Int32:
					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
					case TypeCode.Single:
					case TypeCode.Double:
					case TypeCode.Decimal:	return "number";
					case TypeCode.DateTime:	return "date";
					case TypeCode.Char:
					case TypeCode.String:	return "string";
				}
				return "undefined";
			}

	// Determine if a statement label matches a label list.  The null
	// label always matches.
	public static bool LabelMatch(String label, String[] labels)
			{
				if(label == null)
				{
					return true;
				}
				else if(labels == null)
				{
					return false;
				}
				foreach(String lab in labels)
				{
					if(label == lab)
					{
						return true;
					}
				}
				return false;
			}

	// Create a compound statement node.  We do this carefully to
	// reduce the depth of the resulting node tree.
	public static JNode CreateCompound(JNode left, JNode right)
			{
				JCompound node;
				JCompound prev;
				if(left == null || left is JEmpty)
				{
					return right;
				}
				else if(right == null || right is JEmpty)
				{
					return left;
				}
				if(!(left is JCompound))
				{
					node = new JCompound(Context.BuildRange
											(left.context, right.context));
					node.stmt1 = left;
					node.stmt2 = right;
					return node;
				}
				node = (JCompound)left;
				prev = null;
				do
				{
					if(node.stmt1 == null)
					{
						node.stmt1 = right;
						return left;
					}
					if(node.stmt2 == null)
					{
						node.stmt2 = right;
						return left;
					}
					if(node.stmt3 == null)
					{
						node.stmt3 = right;
						return left;
					}
					if(node.stmt4 == null)
					{
						node.stmt4 = right;
						return left;
					}
					prev = node;
					node = node.next;
				}
				while(node != null);
				prev.next = new JCompound(right.context.MakeCopy());
				prev.next.stmt1 = right;
				left.context.endLine = right.context.endLine;
				left.context.endLinePosition = right.context.endLinePosition;
				return left;
			}

	// Add a label to a statement.
	public static void AddLabel(String label, JNode node)
			{
				JStatement stmt = (node as JStatement);
				if(stmt != null)
				{
					if(stmt.labels == null)
					{
						stmt.labels = new String [] {label};
					}
					else
					{
						String[] newLabels =
							new String [stmt.labels.Length + 1];
						Array.Copy(stmt.labels, newLabels,
								   stmt.labels.Length);
						newLabels[stmt.labels.Length] = label;
						stmt.labels = newLabels;
					}
				}
			}

	// Add an expression to a list.
	public static void AddExprToList(JExprList list, Object name, JNode expr)
			{
				JExprListElem elem = new JExprListElem
					(null, name, expr, null);
				if(list.last != null)
				{
					list.last.next = elem;
				}
				else
				{
					list.first = elem;
				}
				list.last = elem;
			}

	// Get the length of an expression list.
	public static int ExprListLength(JExprList list)
			{
				JExprListElem elem = list.first;
				int len = 0;
				while(elem != null)
				{
					++len;
					elem = elem.next;
				}
				return len;
			}

	// Evaluate an argument list.
	private static int EvalArgs(Object[] args, int posn, JNode node,
								VsaEngine engine)
			{
				if(node == null)
				{
					return posn;
				}
				else if(!(node is JArgList))
				{
					args[posn] = node.Eval(engine);
					return posn + 1;
				}
				else
				{
					posn = EvalArgs(args, posn, ((JArgList)node).expr1, engine);
					args[posn] = (((JArgList)node).expr2).Eval(engine);
					return posn + 1;
				}
			}
	public static Object[] EvalArgList(JNode node, VsaEngine engine)
			{
				int len;
				if(node == null)
				{
					len = 0;
				}
				else
				{
					len = 1;
					JNode temp = node;
					/* Extract the length */
					while(temp is JArgList)
					{
						++len;
						temp = ((JArgList)temp).expr1;
					}
				}
				Object[] args = new Object [len];
				EvalArgs(args, 0, node, engine);
				return args;
			}

	// Evaluate and print an expression tree.
	public static void Print(VsaEngine engine, JNode expr)
			{
				if(expr is JArgList)
				{
					Print(engine, ((JArgList)expr).expr1);
					Print(engine, ((JArgList)expr).expr2);
				}
				else
				{
					Object value = expr.Eval(engine);
					String pvalue;
					if(value is ArrayObject)
					{
						pvalue = ArrayPrototype.join(value, String.Empty);
					}
					else
					{
						pvalue = Convert.ToString(value);
					}
					if(pvalue != null)
					{
						ScriptStream.WriteLine(pvalue);
					}
				}
			}

}; // class Support

}; // namespace Microsoft.JScript
