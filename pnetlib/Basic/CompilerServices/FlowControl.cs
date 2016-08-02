/*
 * FlowControl.cs - Implementation of the
 *			"Microsoft.VisualBasic.FlowControl" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace Microsoft.VisualBasic.CompilerServices
{

using System;
using System.ComponentModel;
using System.Collections;

[StandardModule]
#if CONFIG_COMPONENT_MODEL
[EditorBrowsable(EditorBrowsableState.Never)]
#endif
public sealed class FlowControl
{
	// Cannot instantiate this class.
	private FlowControl() {}

	// Throw an exception if the argument is a value type.
	public static void CheckForSyncLockOnValueType(Object obj)
			{
				if(obj != null && obj.GetType().IsValueType)
				{
					throw new ArgumentException(S._("VB_IsValueType"));
				}
			}

	// Get the enumerator for an array.
	public static IEnumerator ForEachInArr(Array ary)
			{
				return ary.GetEnumerator();
			}

	// Get the enumerator for an object.
	public static IEnumerator ForEachInObj(Object obj)
			{
				if(obj == null)
				{
					Utils.ThrowException(91);	// NullReferenceException.
				}
				else if(obj is IEnumerable)
				{
					IEnumerator e = ((IEnumerable)obj).GetEnumerator();
					if(e != null)
					{
						return e;
					}
				}
				Utils.ThrowException(100);	// InvalidOperationException.
				return null;	// Not reached - keep the compiler happy.
			}

	// Get the next object in an enumeration sequence.
	public static bool ForEachNextObj(ref Object obj, IEnumerator enumerator)
			{
				if(enumerator.MoveNext())
				{
					obj = enumerator.Current;
					return true;
				}
				else
				{
					obj = null;
					return false;
				}
			}

	// Cast an object to a new type.
	private static Object CastToType(Object obj, TypeCode type)
			{
			#if !ECMA_COMPAT
				switch(type)
				{
					case TypeCode.Boolean:
						return ((IConvertible)obj).ToBoolean(null);

					case TypeCode.Byte:
						return ((IConvertible)obj).ToByte(null);

					case TypeCode.Int16:
						return ((IConvertible)obj).ToInt16(null);

					case TypeCode.Int32:
						return ((IConvertible)obj).ToInt32(null);

					case TypeCode.Int64:
						return ((IConvertible)obj).ToInt64(null);

					case TypeCode.Single:
						return ((IConvertible)obj).ToSingle(null);

					case TypeCode.Double:
						return ((IConvertible)obj).ToDouble(null);

					case TypeCode.Decimal:
						return ((IConvertible)obj).ToDecimal(null);
				}
				return obj;
			#else
				return obj;
			#endif
			}

	// Determine if a value is negative.
	private static bool ValueIsNegative(Object value, TypeCode type)
			{
				switch(type)
				{
					case TypeCode.Boolean:
						return ((bool)value);	// True == -1 in VB.

					case TypeCode.Int16:
						return (((short)value) < 0);

					case TypeCode.Int32:
						return (((int)value) < 0);

					case TypeCode.Int64:
						return (((long)value) < 0);

					case TypeCode.Single:
						return (((float)value) < 0.0f);

					case TypeCode.Double:
						return (((double)value) < 0.0);

					case TypeCode.Decimal:
						return (((decimal)value) < 0.0m);
				}
				return false;
			}

	// Initialize a "for" loop.
	public static bool ForLoopInitObj
				(Object Counter, Object Start, Object Limit,
				 Object StepValue, ref Object LoopForResult,
				 ref Object CounterResult)
			{
				// Validate the parameters.
				if(Start == null)
				{
					throw new ArgumentException
						(S._("VB_ValueIsNull"), "Start");
				}
				if(Limit == null)
				{
					throw new ArgumentException
						(S._("VB_ValueIsNull"), "Limit");
				}
				if(StepValue == null)
				{
					throw new ArgumentException
						(S._("VB_ValueIsNull"), "StepValue");
				}

				// Find a common numeric type between the three arguments.
				Type enumType;
				TypeCode type =
					ObjectType.CommonType(Start, Limit, out enumType);
				type = ObjectType.CommonType(StepValue, null, type, false);
				if(type == TypeCode.Empty)
				{
					throw new ArgumentException(S._("VB_CommonForType"));
				}

				// Create the "for" control object.
				ForInfo info = new ForInfo();
				LoopForResult = info;
				info.counter = CastToType(Start, type);
				info.limit = CastToType(Limit, type);
				info.stepValue = CastToType(StepValue, type);
				info.type = type;
				info.enumType = enumType;
				info.stepIsNegative = ValueIsNegative(info.stepValue, type);

				// Return the initial counter value.
				if(enumType == null)
				{
					CounterResult = info.counter;
				}
				else
				{
					CounterResult = Enum.ToObject(enumType, info.counter);
				}

				// Determine if we've already exceeded the limit.
				return CheckObjForLimit(info);
			}

	// Check for the end of a "decimal" iteration.
	public static bool ForNextCheckDec
				(Decimal count, Decimal limit, Decimal StepValue)
			{
				if(StepValue >= 0.0m)
				{
					return (count <= limit);
				}
				else
				{
					return (count >= limit);
				}
			}

	// Check the limit of an object-based "for" loop.
	private static bool CheckObjForLimit(ForInfo info)
			{
				if(!(info.stepIsNegative))
				{
					return (ObjectType.ObjTst(info.counter, info.limit, false)
								<= 0);
				}
				else
				{
					return (ObjectType.ObjTst(info.counter, info.limit, false)
								>= 0);
				}
			}

	// Check for the end of an object iteration.
	public static bool ForNextCheckObj
				(Object Counter, Object LoopObj, ref Object CounterResult)
			{
				if(!(LoopObj is ForInfo))
				{
					Utils.ThrowException(100);	// InvalidOperationException.
				}
				else if(Counter == null)
				{
					throw new ArgumentException
						(S._("VB_LoopCounterIsNull"), "Counter");
				}
				ForInfo info = (ForInfo)LoopObj;
				info.counter =
					CastToType(ObjectType.AddObj(Counter, info.stepValue),
							   info.type);
				if(info.enumType == null)
				{
					CounterResult = info.counter;
				}
				else
				{
					CounterResult = Enum.ToObject(info.enumType, info.counter);
				}
				return CheckObjForLimit(info);
			}

	// Check for the end of a "float" iteration.
	public static bool ForNextCheckR4
				(float count, float limit, float StepValue)
			{
				if(StepValue >= 0.0f)
				{
					return (count <= limit);
				}
				else
				{
					return (count >= limit);
				}
			}

	// Check for the end of a "double" iteration.
	public static bool ForNextCheckR8
				(double count, double limit, double StepValue)
			{
				if(StepValue >= 0.0)
				{
					return (count <= limit);
				}
				else
				{
					return (count >= limit);
				}
			}

	// Storage for an object-based "for" loop.
	private sealed class ForInfo
	{
		public Object counter;
		public Object limit;
		public Object stepValue;
		public TypeCode type;
		public Type enumType;
		public bool stepIsNegative;

	}; // class ForInfo

}; // class FlowControl

}; // namespace Microsoft.VisualBasic.CompilerServices
