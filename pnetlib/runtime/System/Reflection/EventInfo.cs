/*
 * EventInfo.cs - Implementation of the "System.Reflection.EventInfo" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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
using System.Runtime.InteropServices;
using System.Diagnostics;

#if CONFIG_COM_INTEROP
[ClassInterface(ClassInterfaceType.AutoDual)]
#if CONFIG_FRAMEWORK_1_2 && CONFIG_REFLECTION
[ComDefaultInterface(typeof(_EventInfo))]
#endif
#endif
public abstract class EventInfo : MemberInfo
#if CONFIG_COM_INTEROP && CONFIG_FRAMEWORK_1_2
	, _EventInfo
#endif
{

	// Constructor.
	protected EventInfo() : base() {}

	// Get the member type.
	public override MemberTypes MemberType
			{
				get
				{
					return MemberTypes.Event;
				}
			}

	// Add a new handler to this event.
#if !ECMA_COMPAT
	[DebuggerStepThrough]
	[DebuggerHidden]
#endif
	public void AddEventHandler(Object target, Delegate handler)
			{
				MethodInfo add = GetAddMethod(true);
				if(add == null)
				{
					throw new MemberAccessException
						(_("Reflection_NoEventAdd"));
				}
				if(handler == null ||
				   !EventHandlerType.IsAssignableFrom(handler.GetType()))
				{
					throw new ArgumentException
						(_("Reflection_InvalidEventHandler"));
				}
				Object[] parameters = new Object [1];
				parameters[0] = handler;
				add.Invoke(target, parameters);
			}

	// Get the add method for this event.
	public abstract MethodInfo GetAddMethod(bool nonPublic);
	public MethodInfo GetAddMethod()
			{
				return GetAddMethod(false);
			}

	// Get the raise method for this event.
	public abstract MethodInfo GetRaiseMethod(bool nonPublic);
	public MethodInfo GetRaiseMethod()
			{
				return GetRaiseMethod(false);
			}

	// Get the remove method for this event.
	public abstract MethodInfo GetRemoveMethod(bool nonPublic);
	public MethodInfo GetRemoveMethod()
			{
				return GetRemoveMethod(false);
			}

	// Remove an event handler from this event.
#if !ECMA_COMPAT
	[DebuggerStepThrough]
	[DebuggerHidden]
#endif
	public void RemoveEventHandler(Object target, Delegate handler)
			{
				MethodInfo remove = GetRemoveMethod(true);
				if(remove == null)
				{
					throw new MemberAccessException
						(_("Reflection_NoEventRemove"));
				}
				if(handler == null ||
				   !EventHandlerType.IsAssignableFrom(handler.GetType()))
				{
					throw new ArgumentException
						(_("Reflection_InvalidEventHandler"));
				}
				Object[] parameters = new Object [1];
				parameters[0] = handler;
				remove.Invoke(target, parameters);
			}

	// Get the attributes for this event.
	public abstract EventAttributes Attributes { get; }

	// Get the handler type for this event.
	public Type EventHandlerType
			{
				get
				{
					// Search for a parameter to the "Add" method
					// that inherits from "Delegate".
					ParameterInfo[] parameters;
					Type delegateType = typeof(Delegate);
					parameters = GetAddMethod(true).GetParameters();
					int posn;
					Type paramType;
					for(posn = 0; posn < parameters.Length; ++posn)
					{
						paramType = parameters[posn].ParameterType;
						if(paramType.IsSubclassOf(delegateType))
						{
							return paramType;
						}
					}
					return null;
				}
			}

#if !ECMA_COMPAT

	// Determine if this event is multicast.
	public bool IsMulticast
			{
				get
				{
					return typeof(MulticastDelegate).IsAssignableFrom
								(EventHandlerType);
				}
			}

	// Determine if this event has a special name.
	public bool IsSpecialName
			{
				get
				{
					return ((Attributes & EventAttributes.SpecialName) != 0);
				}
			}

#endif // !ECMA_COMPAT

}; // class EventInfo

#endif // CONFIG_REFLECTION

}; // namespace System.Reflection
