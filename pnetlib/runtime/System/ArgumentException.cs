/*
 * ArgumentException.cs - Implementation of the
 *		"System.ArgumentException" class.
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

using System.Runtime.Serialization;

public class ArgumentException : SystemException
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{

	// Internal state.
	private String paramName;

	// Constructors.
	public ArgumentException()
			: base(_("Exception_Argument")) {}
	public ArgumentException(String msg)
			: base(msg) {}
	public ArgumentException(String msg, Exception inner)
			: base(msg, inner) {}
	public ArgumentException(String msg, String param, Exception inner)
			: base(msg, inner) { paramName = param; }
	public ArgumentException(String msg, String param)
			: base(msg) { paramName = param; }
#if CONFIG_SERIALIZATION
	protected ArgumentException(SerializationInfo info,
								StreamingContext context)
			: base(info, context)
			{
				paramName = info.GetString("ParamName");
			}
#endif

	// Properties.
	public virtual String ParamName
			{
				get
				{
					return paramName;
				}
			}
#if !ECMA_COMPAT
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
					return _("Exception_Argument");
				}
			}

	// Get the extra data to insert into "Exception::ToString()"'s result.
	internal override String MessageExtra
			{
				get
				{
					if(paramName != null)
					{
						return String.Format
							   		(_("Exception_ArgParamName"), paramName);
					}
					else
					{
						return null;
					}
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80070057;
				}
			}

#if CONFIG_SERIALIZATION
	// Get the serialization data for this object.
	public override void GetObjectData(SerializationInfo info,
									   StreamingContext context)
			{
				base.GetObjectData(info, context);
				info.AddValue("ParamName", paramName, typeof(String));
			}
#endif

}; // class ArgumentException

}; // namespace System
