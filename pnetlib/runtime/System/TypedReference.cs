/*
 * TypedReference.cs - Implementation of the "System.TypedReference" class.
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

namespace System
{

#if CONFIG_REFLECTION

using System.Reflection;
using System.Runtime.CompilerServices;

[CLSCompliant(false)]
#if ECMA_COMPAT
internal
#else
public
#endif
struct TypedReference
{

	// Internal state.  These fields must be declared in this
	// order to match the runtime engine's requirements.
	private RuntimeTypeHandle type;
	private IntPtr			  value;

	// Check typed references for equality.
	public override bool Equals(Object obj)
			{
				throw new NotSupportedException(_("NotSupp_TypedRefEquals"));
			}

	// Get a hash code for a typed reference.
	public override int GetHashCode()
			{
				if(type.Value != IntPtr.Zero)
				{
					return Type.GetTypeFromHandle(type).GetHashCode();
				}
				else
				{
					return 0;
				}
			}

	// Get the target type that underlies a typed reference.
	public static Type GetTargetType(TypedReference value)
			{
				return Type.GetTypeFromHandle(value.type);
			}

	// Make a typed reference.
	[CLSCompliant(false)]
	public static TypedReference MakeTypedReference(Object target,
													FieldInfo[] flds)
			{
				if(target == null)
				{
					throw new ArgumentNullException("target");
				}
				if(flds == null)
				{
					throw new ArgumentNullException("flds");
				}
				if(flds.Length == 0)
				{
					throw new ArgumentException(_("Arg_MakeTypedRef"));
				}
				int posn;
				for(posn = 0; posn < flds.Length; ++posn)
				{
					if(!(flds[posn] is ClrField))
					{
						throw new ArgumentException
							(_("Arg_MakeTypedRefFields"));
					}
				}
				return ClrMakeTypedReference(target, flds);
			}

	// Internal version of "MakeTypedReference".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static TypedReference ClrMakeTypedReference
				(Object target, FieldInfo[] flds);

	// Set the value within a typed reference.
	[CLSCompliant(false)]
	public static void SetTypedReference(TypedReference target, Object value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
			#if !ECMA_COMPAT
				value = Convert.ChangeType(value, GetTargetType(target));
			#endif
				if(!ClrSetTypedReference(target, value))
				{
					throw new InvalidCastException
						(String.Format
							(_("InvalidCast_FromTo"),
 						     value.GetType().FullName,
							 GetTargetType(target).FullName));
				}
			}

	// Internal version of "SetTypedReference".
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern private static bool ClrSetTypedReference
				(TypedReference target, Object value);

	// Get the type handle within a typed reference.
	public static RuntimeTypeHandle TargetTypeToken(TypedReference value)
			{
				return value.type;
			}

	// Convert a typed reference into an object.
	[MethodImpl(MethodImplOptions.InternalCall)]
	extern public static Object ToObject(TypedReference value);

}; // struct TypedReference

#endif // CONFIG_REFLECTION

}; // namespace System
