/*
 * MissingMemberException.cs - Implementation of the
 *		"System.MissingMemberException" class.
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

#if CONFIG_RUNTIME_INFRA

using System.Runtime.Serialization;

public class MissingMemberException : MemberAccessException
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{
#if !ECMA_COMPAT
	// Internal state.
	protected String ClassName;
	protected String MemberName;
	protected byte[] Signature;
#endif

	// Constructors.
	public MissingMemberException()
			: base(_("Exception_MemberMissing")) {}
	public MissingMemberException(String msg)
			: base(msg) {}
	public MissingMemberException(String msg, Exception inner)
			: base(msg, inner) {}
#if !ECMA_COMPAT
	public MissingMemberException(String className, String memberName)
			: this()
			{
				ClassName = className;
				MemberName = memberName;
			}
#endif
#if CONFIG_SERIALIZATION
	protected MissingMemberException(SerializationInfo info,
									 StreamingContext context)
			: base(info, context)
			{
				ClassName = info.GetString("MMClassName");
				MemberName = info.GetString("MMMemberName");
				Signature = (byte[])(info.GetValue
					("MMSignature", typeof(byte[])));
			}
#endif

#if !ECMA_COMPAT
	// Get the message string for this exception.
	public override String Message
			{
				get
				{
					return base.Message;
				}
			}
#endif

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_MemberMissing");
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80131512;
				}
			}

#if CONFIG_SERIALIZATION
	// Get the serialization data for this object.
	public override void GetObjectData(SerializationInfo info,
									   StreamingContext context)
			{
				base.GetObjectData(info, context);
				info.AddValue("MMClassName", ClassName);
				info.AddValue("MMMemberName", MemberName);
				info.AddValue("MMSignature", Signature, typeof(byte[]));
			}
#endif

}; // class MissingMemberException

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System
