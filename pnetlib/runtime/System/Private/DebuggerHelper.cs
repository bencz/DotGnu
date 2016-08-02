/*
 * DebuggerHelper.cs - Managed support for debugger.
 *
 * Copyright (C) 2007  Southern Storm Software, Pty Ltd.
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

namespace System.Private
{
using System.Text;
using System.Collections;
using System.Reflection;

// This class is used internally by debugger.
internal sealed class DebuggerHelper
{
	static LocalWatch locals;
	static Type methodOwner;
	static int maxItemsDumpCount = 16;
	static Object error;

	private DebuggerHelper()
			{
			}

	// Return string representation of given object.
	public static string ObjectToString(Object o)
			{
				if(o == null)
				{
					return "null";
				}
				if(o == error || o is String)
				{
					return o.ToString();
				}
				try
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(o.ToString());
					if(o is IEnumerable)
					{
						sb.Append(EnumerableToString((IEnumerable)(o)));
					}
					return sb.ToString();
				}
				catch(Exception ex)
				{
					return Error(ex.Message);
				}
			}

	// Return value of given expression.
	public static String ExpressionToString(String expression)
			{
				Object value = ExpressionNotFoundError();
				try
				{
					if(expression == null)
					{
						return Error("null expression");
					}

					ExpressionParser parser = new ExpressionParser(expression);
					String name;
					String[] args;

					// Parse start of the expression.
					if(!parser.Read(out name, out args))
					{
						return value.ToString();
					}

					// Check for constant
					if(Char.IsDigit(name, 0))
					{
						return name;
					}

					// Find matching local variable/function parameter
					value = GetLocal(name);

					// Try instance members
					if(value == error)
					{
						Object instance = GetLocal("this");
						if(instance != error)
						{
							value = GetNonStaticMemberValue(instance, name,
																		null);
						}
						else
						{
							error = value;		// restore last error
						}
					}

					// Call indexer if local variable found
					// and arguments were specified
					if(value != error)
					{
						if(args != null)
						{
							value = GetNonStaticMemberValue(value,
															"get_Item", args);
						}
					}
					else
					{
						// Check for static member
						Type matchingType = FindType(name);

						// Parse member name or try current method owner
						if(matchingType != null)
						{
							parser.Read(out name, out args);
						}
						else
						{
							matchingType = methodOwner;
						}

						// Find matching static field/method/property
						if(matchingType != null)
						{
							value = GetStaticMemberValue(matchingType,
																name, args);
						}
					}

					// Expression was not found
					if(value == error)
					{
						return value.ToString();
					}

					// Iterate until the leftmost member reference
					while(parser.Read(out name, out args) && value != error)
					{
						value = GetNonStaticMemberValue(value, name, args);
					}

					return ObjectToString(value);
				}
				catch(Exception ex)
				{
					return Error(ex.Message);
				}
			}

	// Clear list of locals.
	public static void ClearLocals()
			{
				locals = null;
			}

	// Add local variable or parameter to the list of locals.
	public static void AddLocal(String name, Type type, Object value)
			{
				// Assign name if it is null
				if(name == null)
				{
					int index = LocalWatch.CountWatches(locals);
					name = String.Format("var{0}", index);
				}

				// Create new watch and append it to the end of the watch list.
				LocalWatch watch = new LocalWatch(name, type, value);
				if(locals == null)
				{
					locals = watch;
				}
				else
				{
					locals.Append(watch);
				}
			}

	// Helper function for show_locals command.
	// If no error ocurred then result is xml string (first char is '<')
	// otherwise the error message is returned.
	public static String ShowLocals()
			{
				try
				{
					StringBuilder sb = new StringBuilder("<LocalVariables>\n");
					LocalWatch watch = locals;
					while(watch != null)
					{
						sb.AppendFormat(
@"  <LocalVariable Name=""{0}"" Value=""{1}"">
    <Type Name=""{2}"" />
  </LocalVariable>
",
							watch.Name,
							ObjectToString(watch.Value),
							watch.Type.Name);

						watch = watch.Next;
					}
					sb.Append("</LocalVariables>");
					return sb.ToString();
				}
				catch(Exception ex)
				{
					return Error("unable to show locals, " + ex.Message);
				}
			}

	// Return value of local variable with given name.
	private static Object GetLocal(String name)
			{
				LocalWatch current = locals;
				while(current != null)
				{
					if(current.Name == name)
					{
						return current.Value;
					}
					else
					{
						current = current.Next;
					}
				}
				return ExpressionNotFoundError();
			}

	// Set type that owns currently debugged method
	public static void SetMethodOwner(Type type)
			{
				methodOwner = type;
			}

	// Set current error and return it.
	private static String Error(String message)
			{
				error = String.Format("error: {0}", message);
				return (String) error;
			}

	// Set and returns "expression not found" error.
	private static String ExpressionNotFoundError()
			{
				return Error("expression not found");
			}

	// Return info about enumerable object.
	private static String EnumerableToString(IEnumerable o)
			{
				IEnumerator e = o.GetEnumerator();
				int count = 0;
				StringBuilder sb = new StringBuilder();

				// Dump first elements
				while(e.MoveNext())
				{
					if(count != 0)
					{
						sb.Append(", ");
					}
					count++;
					sb.Append(e.Current);
					
					// Display first 16 items at max.
					if(count >= maxItemsDumpCount)
					{
						count = Int32.MaxValue;
						break;
					}
				}

				// Try to determine correct count
				if(count == Int32.MaxValue && o is ICollection)
				{
					count = ((ICollection)(o)).Count;
				}

				// Dump information about count
				if(count != Int32.MaxValue)
				{
					sb.Insert(0, "count=" + count + " | ");
				}
				sb.Insert(0, " | elements: ");

				return sb.ToString();
			}

	// Create arguments from string values.
	// Return null if something fails.
	private static Object[] CreateArguments(ParameterInfo[] parameters,
											String[] values)
			{
				if(values.Length != parameters.Length)
				{
					return null;
				}
				try
				{
					Object[] result = new Object[parameters.Length];
					for(int i = 0; i < result.Length; i++)
					{
						String arg = ExpressionToString(values[i]);
						result[i] = Convert.ChangeType(arg,
												parameters[i].ParameterType);
					}
					return result;
				}
				catch
				{
					return null;
				}
			}

	// Find member with given name that can be called with given args.
	// Output args can be used to make actual method or property call.
	private static MemberInfo FindMember(Type type, Object obj,
										 String memberName, String[] args,
										 BindingFlags extraFlags,
										 out Object[] outArgs)
			{
				outArgs = null;
				string suffix = "." + memberName;

				foreach(MemberInfo mi in type.GetMembers(
					extraFlags |
					BindingFlags.Public |
					BindingFlags.NonPublic))
				{
					if(mi.Name == memberName || mi.Name.EndsWith(suffix))
					{
						switch(mi.MemberType)
						{
							case MemberTypes.Field:
							{
								return mi;
							}
							case MemberTypes.Method:
							case MemberTypes.Property:
							{
								if(args != null)
								{
									ParameterInfo[] paramInfo;
									if(mi is PropertyInfo)
									{
										paramInfo = ((PropertyInfo)(mi)).
														GetIndexParameters();
									}
									else
									{
										paramInfo = ((MethodInfo)(mi)).
															GetParameters();
									}
									outArgs = CreateArguments(paramInfo, args);
								}
								return mi;
							}
						}
					}
				}
				return null;
			}

	// Return member value as string.
	// Call with obj=null and extraFlags=BindingFlags.Static for static members
	// Call with BindingFlags.Instance for instance members
	private static Object GetMemberValue(Type type, Object obj,
										 String memberName, String[] args,
										 BindingFlags extraFlags)
			{
				Object[] outArgs;
				MemberInfo mi = FindMember(type, obj, memberName, args,
													extraFlags, out outArgs);

				if(mi is FieldInfo)
				{
					return ((FieldInfo)(mi)).GetValue(obj);
				}
				else if(mi is PropertyInfo)
				{
					return ((PropertyInfo)(mi)).GetValue(obj, outArgs);
				}
				else if(mi is MethodInfo)
				{
					return ((MethodInfo)(mi)).Invoke(obj, outArgs);
				}
				else
				{
					return ExpressionNotFoundError();
				}
			}

	// Return value of static member (field, property or method).
	private static Object GetStaticMemberValue(Type type, String memberName,
											   String[] args)
			{
				return GetMemberValue(type, null, memberName, args,
														BindingFlags.Static);
			}

	// Return value of non static member (field, property or method).
	private static Object GetNonStaticMemberValue(Object obj,
												  String memberName,
												  String[] args)
			{
				return GetMemberValue(obj.GetType(), obj, memberName, args,
					   									BindingFlags.Instance);
			}

	// Search assemblies in current app domain for type of given name.
	private static Type FindType(String typeName)
			{
				foreach(Assembly assembly in 
									AppDomain.CurrentDomain.GetAssemblies())
				{
					foreach(Type type in assembly.GetTypes())
					{
						if(type.Name == typeName)
						{
							return type;
						}
					}
				}
				return null;
			}

	// Simple parser for watched expressions.
	private class ExpressionParser
	{
		private String expression;
		private int index;

		public ExpressionParser(String expression)
				{
					this.expression = expression;
					this.index = 0;
				}

		// Read member name and arguments (if any)
		// Returns false when end of expression is reached.
		public bool Read(out String name, out String[] args)
				{
					name = null;
					args = null;

					// Bail out if whole expression is processed
					if(index >= expression.Length)
					{
						return false;
					}

					// Read name
					StringBuilder sb = new StringBuilder();
					for(; index < expression.Length; index++)
					{
						if(Char.IsLetterOrDigit(expression, index) ||
							expression[index] == '_')
						{
							sb.Append(expression[index]);
						}
						else
						{
							break;
						}
					}
					name = sb.ToString();

					// Move to next token
					while(true)
					{
						if(index >= expression.Length)
						{
							return true;
						}
						if(expression[index] == '.')
						{
							index++;
							return true;
						}
						if(expression[index] == '[' ||
							expression[index] == '(')
						{
							index++;
							break;
						}
						index++;
					}

			// Read arguments
			ArrayList list = new ArrayList();
			sb.Length = 0;
			for(; index < expression.Length; index++)
			{
				char ch = expression[index];
				if(ch == ')' || ch == ']')
				{
					list.Add(sb.ToString());
					index++;
					break;
				}
				if(ch == ',')
				{
					list.Add(sb.ToString());
					sb.Length = 0;
				}
				sb.Append(ch);
			}
			args = (String[]) list.ToArray(typeof(String));
			return true;
		}
	}; // class ExpressionParser

	// Information about function parameter or local variable.
	private class LocalWatch
	{
		private String name;
		private Type type;
		private Object value;
		private LocalWatch next;

		public LocalWatch(String name, Type type, Object value)
				{
					this.name = name;
					this.type = type;
					this.value = value;
				}

		public String Name
				{
					get
					{
						return name;
					}
				}

		public Type Type
				{
					get
					{
						return type;
					}
				}

		public Object Value
				{
					get
					{
						return value;
					}
				}

		// Return next watch in this list or null.
		public LocalWatch Next
				{
					get
					{
						return next;
					}
				}

		// Append watch to the end of the list.
		public void Append(LocalWatch watch)
				{
					LocalWatch tail = this;
					while(tail.Next != null)
					{
						tail = tail.Next;
					}
					tail.next = watch;
				}

		// Return watch count in the watch list.
		public static int CountWatches(LocalWatch watch)
				{
					int count = 0;
					while(watch != null)
					{
						watch = watch.Next;
						count++;
					}
					return count;
				}

	}; // class LocalWatch

}; // class DebuggerHelper

}; // namespace System.Private 

