/*
 * MarshalAsAttribute.cs - Implementation of the
 *			"System.Runtime.InteropServices.MarshalAsAttribute" class.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_RUNTIME_INFRA

[AttributeUsage(AttributeTargets.Field |
				AttributeTargets.Parameter |
				AttributeTargets.ReturnValue,
#if CONFIG_FRAMEWORK_2_0
				AllowMultiple=false,
#endif
				Inherited=false)]
public sealed class MarshalAsAttribute : Attribute
{
	// Internal state.
	private UnmanagedType type;

	// Constructors.
	public MarshalAsAttribute(UnmanagedType unmanagedType)
			{
				type = unmanagedType;
			}
	public MarshalAsAttribute(short unmanagedType)
			{
				type = (UnmanagedType)unmanagedType;
			}

	// Public fields.
	public UnmanagedType ArraySubType;
	public String MarshalCookie;
	public String MarshalType;
	public Type MarshalTypeRef;
	public int SizeConst;
	public short SizeParamIndex;
#if CONFIG_COM_INTEROP
	public VarEnum SafeArraySubType;
	public Type SafeArrayUserDefinedSubType;
#endif

	// Properties.
	public UnmanagedType Value
			{
				get
				{
					return type;
				}
			}

}; // class MarshalAsAttribute

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System.Runtime.InteropServices
