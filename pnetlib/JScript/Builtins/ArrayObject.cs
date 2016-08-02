/*
 * ArrayObject.cs - Implementation of array objects.
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

public class ArrayObject : JSObject
{
	// Internal state.
	internal Array array;
	internal uint arrayLen;

	// Constructor.
	internal ArrayObject(ScriptObject prototype)
			: base(prototype)
			{
				array = null;
				arrayLen = 0;
			}
	internal ArrayObject(ScriptObject prototype, uint len)
			: base(prototype)
			{
				array = null;
				arrayLen = len;
			}
			
	// Determines wether `name' is an array index.
	internal static bool IsArrayIndex(String name)
		{
			if(name != null && name.Length > 0)
			{
				for(int i=0; i < name.Length; i++)
				{
					if(!Char.IsNumber(name, i))
					{
						return false;
					}
				}
				// TODO : improve this operation ?
				return !Convert.ToUInt32(name).ToString().Equals(name);
			}
			else
			{
				return false;
			}
		}

	// Get or set the length of the array.
	public virtual Object length
			{
				get
				{
					if(arrayLen <= (uint)(Int32.MaxValue))
					{
						return (int)arrayLen;
					}
					else
					{
						return (double)arrayLen;
					}
				}
				set
				{
					double num = Convert.ToNumber(value);
					uint inum = Convert.ToUInt32(value);
					if(num != (double)inum)
					{
						throw new JScriptException
							(JSError.ArrayLengthAssignIncorrect);
					}
					if(array != null && inum < arrayLen &&
					   inum < (uint)(array.Length))
					{
						if(arrayLen < (uint)(array.Length))
						{
							Array.Clear(array, (int)inum,
										(int)(arrayLen - inum));
						}
						else
						{
							Array.Clear(array, (int)inum,
										array.Length - (int)inum);
						}
					}
					arrayLen = inum;
				}
			}

	// Splice the array slowly.
	protected void SpliceSlowly(uint start, uint deleteCount,
								Object[] args, ArrayObject outArray,
								uint oldLength, uint newLength)
			{
				// This exists for backwards-compatibility, but there
				// is actually no way to call it from user code.
			}

	// Get a property from this object.  Null if not present.
	internal override Object Get(String name)
			{
				if(name == null || name.Length == 0)
				{
					return null;
				}
				else if(name == "length")
				{
					return length;
				}
				else if(IsArrayIndex(name))
				{
					uint inum = Convert.ToUInt32(name);
					if(inum < arrayLen && array != null &&
							inum < (uint)(array.Length))
					{
						return array.GetValue((int)inum);
					}
					else
					{
						return null;
					}
				}
				else
				{
					return base.Get(name);
				}
			}

	// Get a property from this object by numeric index.
	internal override Object GetIndex(int index)
			{
				if(index >= 0 && ((uint)index) < arrayLen &&
				   array != null && index < array.Length)
				{
					return array.GetValue(index);
				}
				else
				{
					return base.Get(Convert.ToString(index));
				}
			}

	// Put a property to this object.
	internal override void Put(String name, Object value)
			{
				if(name == "length")
				{
					length = value;
				}
				else if(IsArrayIndex(name))
				{
					PutIndex(Convert.ToInt32(name), value);
				}
				else if(CanPut(name))
				{
					base.Put(name, value);
				}
			}

	// Put a property to this object by numeric index.
	internal override void PutIndex(int index, Object value)
			{
				if (index < 0)
				{
					throw new ArgumentException();
				}
				uint newlen = (uint)(index + 1);
				if (array == null)
				{
					array = new Object[newlen];
				}
				if (newlen > arrayLen)
				{
					arrayLen = newlen;
				}
				if (array.Length <= index)
				{
					Object[] a2 = new Object[newlen];
					array.CopyTo(a2, 0);
					array = a2;
					array.SetValue(value, index);
				}
				else
				{
					array.SetValue(value, index);
				}
			}

	// Determine if this object has a specific property.
	internal override bool HasOwnProperty(String name)
			{
				if(name != null && name == "length")
				{
					return true;
				}
				else if(IsArrayIndex(name))
				{
					int index = Convert.ToInt32(name);
					return (index >= 0 && (uint)index < arrayLen &&
						array != null && index < array.Length && array.GetValue(index) != null);
				}
				else
				{
					return base.HasOwnProperty(name);
				}
			}

	// Delete a property from this object.
	internal override bool Delete(String name)
			{
				if(name == null || name.Length == 0 || name == "length")
				{
					return false;
				}
				else if(IsArrayIndex(name))
				{
					int index = Convert.ToInt32(name);
					if(array != null && array.GetValue(index) != null)
					{
						array.SetValue(null, Convert.ToInt32(name));
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return base.Delete(name);
				}
			}

	// Get the default value for this object.
	internal override Object DefaultValue(DefaultValueHint hint)
			{
				// TODO
				return null;
			}

	// Get an enumerator for the properties in this object.
	internal override IEnumerator GetPropertyEnumerator()
			{
				if(array != null)
				{
					return new JoinedEnumerator
						(new ArrayKeyEnumerator(array), base.GetPropertyEnumerator());
				}
				else
				{
					return base.GetPropertyEnumerator();
				}
			}

	// Wrapper class for wrapping up a native array.
	internal sealed class Wrapper : ArrayObject
	{
		// Constructor.
		public Wrapper(ScriptObject prototype, Array array)
				: base(prototype, (uint)(array.Length))
				{
					this.array = array;
				}

		// Get or set the length of the array.
		public override Object length
				{
					get
					{
						return array.Length;
					}
					set
					{
						throw new JScriptException
							(JSError.AssignmentToReadOnly);
					}
				}

		// Get a property from this object by numeric index.
		internal override Object GetIndex(int index)
				{
					if(index >= 0 && index < array.Length)
					{
						return array.GetValue(index);
					}
					else
					{
						return null;
					}
				}

		// Put a property to this object.
		internal override void Put(String name, Object value)
				{
					if(name == "length")
					{
						length = value;
					}
					else if(CanPut(name))
					{
						double num = Convert.ToNumber(name);
						uint inum = Convert.ToUInt32(name);
						if(num != (double)inum)
						{
							// Force an array index exception.
							inum = (uint)(array.Length);
						}
						array.SetValue(value, (int)inum);
					}
				}

		// Put a property to this object by numeric index.
		internal override void PutIndex(int index, Object value)
				{
					array.SetValue(value, index);
				}

		// Delete a property from this object.
		internal override bool Delete(String name)
				{
					// Deletions are not allowed on native arrays.
					return false;
				}

	}; // class Wrapper
	
	// Enumerates the index positions of an array instead of it's values.
	internal sealed class ArrayKeyEnumerator : IEnumerator
	{
		internal Array array;
		internal int current;
		
		public ArrayKeyEnumerator(Array array)
		{
			if(array == null)
			{
				throw new ArgumentNullException("array");
			}
			this.array = array;
			this.current = -1;
		}
		
		// Advance to the next element
		public bool MoveNext()
		{
			if(current >= -1 && current < array.Length)
			{
				for(; current < array.Length - 1;)
				{
					if(array.GetValue(++current) != null)
					{
						return true;
					}
				}
			}
			return false;
		}
	
		// Reset the enumerator to it's inital state
		public void Reset()
		{
			current = -1;
		}
	
		// Get the current index position
		public Object Current
		{
			get
			{
				if(current >= 0 && current < array.Length)
				{
					return current;
				}
				else
				{	
					throw new InvalidOperationException();
				}
			}
		}
	}; // class ArrayKeyEnumerator
	
	// An enumerator that enumerates other enumerators... :-)
	internal sealed class JoinedEnumerator : IEnumerator
	{
		internal IEnumerator[] enumerators;
		internal int current;
		
		public JoinedEnumerator(params IEnumerator[] enumerators)
		{
			if(enumerators == null)
			{
				throw new ArgumentNullException("enumerators");
			}
			else if(enumerators.Length == 0)
			{
				throw new ArgumentException
					("No enumerators given. At least one required.", "enumerators");
			}
			
			this.enumerators = enumerators;
			this.current = -1;
		}
		
		// Advance to next element contained in the enumerators
		public bool MoveNext()
		{
			if(current == -1)
			{
				return enumerators[++current].MoveNext();
			}
			else if(current < enumerators.Length)
			{	
				// if we cant advance in the current enumerator,
				// advance amongst the other enumerators 'til we 
				// have the next element
				if(!enumerators[current].MoveNext())
				{
					for(;current < enumerators.Length; current++)
					{
						if(enumerators[current].MoveNext())
						{
							return true;
						}
					}
				}
				else
				{
					return true;
				}
			}
			return false;
		}
		
		// Resets the enumerator to it's initial state
		public void Reset()
		{
			for(;current >= 0;current--)
			{
				enumerators[current].Reset();
			}
		}
		
		// Get the current element from the current enumerator
		public Object Current
		{
			get
			{
				if(current >= 0 && current < enumerators.Length)
				{
					return enumerators[current].Current;
				}
				else
				{
					throw new InvalidOperationException();
				}
			}
		}
		
	}; // class JoinedEnumerator

}; // class ArrayObject

}; // namespace Microsoft.JScript
