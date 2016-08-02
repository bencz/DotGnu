/*
 * ClrEvent.cs - Implementation of the
 *		"System.Reflection.ClrEvent" class.
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

namespace System.Reflection
{

#if CONFIG_REFLECTION

using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

internal sealed class ClrEvent : EventInfo, IClrProgramItem
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{

	// Private data used by the runtime engine.
	private IntPtr privateData;

	// Implement the IClrProgramItem interface.
	public IntPtr ClrHandle
			{
				get
				{
					return privateData;
				}
			}

	// Get the custom attributes attached to this event.
	public override Object[] GetCustomAttributes(bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, inherit);
			}
	public override Object[] GetCustomAttributes(Type type, bool inherit)
			{
				return ClrHelpers.GetCustomAttributes(this, type, inherit);
			}

	// Determine if custom attributes are defined for this event.
	public override bool IsDefined(Type type, bool inherit)
			{
				return ClrHelpers.IsDefined(this, type, inherit);
			}

	// Override inherited properties.
	public override Type DeclaringType
			{
				get
				{
					return ClrHelpers.GetDeclaringType(this);
				}
			}
	public override Type ReflectedType
			{
				get
				{
					return ClrHelpers.GetDeclaringType(this);
				}
			}
	public override String Name
			{
				get
				{
					return ClrHelpers.GetName(this);
				}
			}
	public override EventAttributes Attributes
			{
				get
				{
					return (EventAttributes)
						ClrHelpers.GetMemberAttrs(privateData);
				}
			}

	// Get the add method for this event.
	public override MethodInfo GetAddMethod(bool nonPublic)
			{
				return ClrHelpers.GetSemantics
							(privateData,
						 	 MethodSemanticsAttributes.AddOn,
							 nonPublic);
			}

	// Get the raise method for this event.
	public override MethodInfo GetRaiseMethod(bool nonPublic)
			{
				return ClrHelpers.GetSemantics
							(privateData,
						 	 MethodSemanticsAttributes.Fire,
							 nonPublic);
			}

	// Get the remove method for this event.
	public override MethodInfo GetRemoveMethod(bool nonPublic)
			{
				return ClrHelpers.GetSemantics
							(privateData,
						 	 MethodSemanticsAttributes.RemoveOn,
							 nonPublic);
			}

#if CONFIG_SERIALIZATION

	// Get the serialization data for this event.
	public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				MemberInfoSerializationHolder.Serialize
					(info, MemberTypes.Event, Name, ToString(), ReflectedType);
			}

#endif

}; // class ClrEvent

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
