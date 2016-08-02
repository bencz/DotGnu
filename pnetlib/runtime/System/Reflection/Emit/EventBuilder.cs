/*
 * EventBuilder.cs - Implementation of the
 *		"System.Reflection.Emit.EventBuilder" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

using System;
using System.Reflection;
using System.Globalization;
using System.Runtime.CompilerServices;

public sealed class EventBuilder : IClrProgramItem, IDetachItem
{
	// Internal state.
	private TypeBuilder type;
	private IntPtr privateData;

	// Constructor.
	internal EventBuilder(TypeBuilder type, String name,
						  EventAttributes attributes, Type eventType)
			{
				// Validate the parameters.
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				else if(eventType == null)
				{
					throw new ArgumentNullException("eventType");
				}

				// Register this item to be detached later.
				type.module.assembly.AddDetach(this);

				// Create the event.
				lock(typeof(AssemblyBuilder))
				{
					this.type = type;
					this.privateData = ClrEventCreate
						(((IClrProgramItem)type).ClrHandle, name,
					 	SignatureHelper.CSToILType(type.module, eventType),
					 	attributes);
				}
			}

	// Add method semantics to this event.
	private void AddSemantics(MethodSemanticsAttributes attr,
							  MethodBuilder mdBuilder)
			{
				try
				{
					type.StartSync();
					if(mdBuilder == null)
					{
						throw new ArgumentNullException("mdBuilder");
					}
					lock(typeof(AssemblyBuilder))
					{
						ClrEventAddSemantics
							(privateData, attr,
						 	type.module.GetMethodToken(mdBuilder));
					}
				}
				finally
				{
					type.EndSync();
				}
			}

	// Add an "other" method to this event.
	public void AddOtherMethod(MethodBuilder mdBuilder)
			{
				AddSemantics(MethodSemanticsAttributes.Other, mdBuilder);
			}

	// Get the token code for this event.
	public EventToken GetEventToken()
			{
				lock(typeof(AssemblyBuilder))
				{
					return new EventToken
						(AssemblyBuilder.ClrGetItemToken(privateData));
				}
			}

	// Set the "add on" method for this event.
	public void SetAddOnMethod(MethodBuilder mdBuilder)
			{
				AddSemantics(MethodSemanticsAttributes.AddOn, mdBuilder);
			}

	// Set custom attributes for this event.
	public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
			{
				try
				{
					type.StartSync();
					type.module.assembly.SetCustomAttribute
						(this, customBuilder);
				}
				finally
				{
					type.EndSync();
				}
			}
	public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
			{
				try
				{
					type.StartSync();
					type.module.assembly.SetCustomAttribute
						(this, con, binaryAttribute);
				}
				finally
				{
					type.EndSync();
				}
			}

	// Set the "raise" method for this event.
	public void SetRaiseMethod(MethodBuilder mdBuilder)
			{
				AddSemantics(MethodSemanticsAttributes.Fire, mdBuilder);
			}

	// Set the "remove on" method for this event.
	public void SetRemoveOnMethod(MethodBuilder mdBuilder)
			{
				AddSemantics(MethodSemanticsAttributes.RemoveOn, mdBuilder);
			}

	// Get the CLR handle for this object.
	IntPtr IClrProgramItem.ClrHandle
			{
				get
				{
					return privateData;
				}
			}

	// Detach this item.
	void IDetachItem.Detach()
			{
				privateData = IntPtr.Zero;
			}

	// Create a new event and attach it to a particular class.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static IntPtr ClrEventCreate
			(IntPtr classInfo, String name, IntPtr type,
			 EventAttributes attributes);

	// Add semantic information to this event.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static void ClrEventAddSemantics
			(IntPtr eventInfo, MethodSemanticsAttributes attr,
			 MethodToken token);

}; // class EventBuilder

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
