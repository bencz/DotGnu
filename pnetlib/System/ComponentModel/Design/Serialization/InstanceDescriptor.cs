/*
 * InstanceDescriptor.cs - Implementation of the
 *		"System.ComponentModel.Design.Serialization.InstanceDescriptor" class.
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

namespace System.ComponentModel.Design.Serialization
{

#if CONFIG_COMPONENT_MODEL_DESIGN

using System.Collections;
using System.Reflection;

public sealed class InstanceDescriptor
{
	// Internal state.
	private MemberInfo member;
	private ICollection arguments;
	private bool isComplete;

	// Constructors.
	public InstanceDescriptor(MemberInfo member, ICollection arguments)
			{
				this.member = member;
				this.arguments = arguments;
				this.isComplete = true;
			}
	public InstanceDescriptor
				(MemberInfo member, ICollection arguments, bool isComplete)
			{
				this.member = member;
				this.arguments = arguments;
				this.isComplete = isComplete;
			}

	// Get this object's properties.
	public ICollection Arguments
			{
				get
				{
					return arguments;
				}
			}
	public bool IsComplete
			{
				get
				{
					return isComplete;
				}
			}
	public MemberInfo MemberInfo
			{
				get
				{
					return member;
				}
			}

	// Invoke this instance descriptor.
	public Object Invoke()
			{
				int posn;

				// Convert the arguments.
				Object[] args = new Object [Arguments.Count];
				Arguments.CopyTo(args, 0);
				for(posn = 0; posn < args.Length; ++posn)
				{
					if(args[posn] is InstanceDescriptor)
					{
						args[posn] =
							((InstanceDescriptor)(args[posn])).Invoke();
					}
				}

				// Determine how to invoke the member.
				Object result = null;
				switch(member.MemberType)
				{
					case MemberTypes.Constructor:
					{
						result = ((ConstructorInfo)member).Invoke(args);
					}
					break;

					case MemberTypes.Method:
					{
						result = ((MethodInfo)member).Invoke(null, args);
					}
					break;

					case MemberTypes.Property:
					{
						result = ((PropertyInfo)member).GetValue(null, args);
					}
					break;

					case MemberTypes.Field:
					{
						result = ((FieldInfo)member).GetValue(null);
					}
					break;
				}
				return result;
			}

}; // class InstanceDescriptor

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design.Serialization
