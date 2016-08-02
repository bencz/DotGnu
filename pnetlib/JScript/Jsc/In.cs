/*
 * In.cs - Implementation for "in" operators.
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
using System.Collections;

// Dummy class for backwards-compatibility.

public sealed class In : BinaryOp
{
	// Constructor.
	internal In() : base((int)(JSToken.In)) {}

	// Evaluate an "in" operator on two values.
	public static bool JScriptIn(Object v1, Object v2)
			{
				if(v2 is ScriptObject)
				{
					// This is an ordinary JScript object.
					return ((ScriptObject)v2).HasProperty
						(Convert.ToString(v1));
				}
				else if(v2 is Array)
				{
					// This is an underlying CLI array.
					Array array = (v2 as Array);
					double index = Convert.ToNumber(v1);
					if(index == Math.Round(index) &&
					   index >= (double)(Int32.MinValue) &&
					   index <= (double)(Int32.MaxValue))
					{
						int indexi = (int)index;
						return (indexi >= array.GetLowerBound(0) &&
						        indexi <= array.GetUpperBound(0));
					}
					else
					{
						return false;
					}
				}
				else if(v2 is IDictionary)
				{
					// This is an underlying CLI dictionary.
					if(v1 == null)
					{
						return false;
					}
					return ((IDictionary)v2).Contains(v1);
				}
				else if(v2 is IEnumerable)
				{
					// Get the enumerator to search a CLI collection.
					if(v1 == null)
					{
						return false;
					}
					foreach(Object obj in ((IEnumerable)v2))
					{
						if(v1.Equals(obj))
						{
							return true;
						}
					}
					return false;
				}
				else if(v2 is IEnumerator)
				{
					// Use the explicitly-supplied enumerator.
					if(v1 == null)
					{
						return false;
					}
					IEnumerator e = (IEnumerator)v2;
					while(e.MoveNext())
					{
						if(v1.Equals(e.Current))
						{
							return true;
						}
					}
					return false;
				}
				else
				{
					throw new JScriptException(JSError.ObjectExpected);
				}
			}

}; // class In

}; // namespace Microsoft.JScript
