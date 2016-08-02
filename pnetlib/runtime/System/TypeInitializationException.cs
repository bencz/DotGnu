/*
 * TypeInitializationException.cs - Implementation of the
 *			"System.TypeInitializationException" class.
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

public sealed class TypeInitializationException : SystemException
{
	private String typeName;

	// Constructors.
	private TypeInitializationException()
		: base(_("Exception_TypeInit")) {}
	private TypeInitializationException(String msg)
		: base(msg) {}
#if ECMA_COMPAT
	private
#else
	public
#endif
	TypeInitializationException(String typeName, Exception inner)
		: base(null, inner)
		{ this.typeName = typeName; }
#if CONFIG_SERIALIZATION
	internal TypeInitializationException(SerializationInfo info,
										 StreamingContext context)
		: base(info, context)
		{
			typeName = info.GetString("TypeName");
		}
#endif

	// Properties.
	public String TypeName
		{
			get
			{
				if(typeName != null)
				{
					return typeName;
				}
				else
				{
					return String.Empty;
				}
			}
		}

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					if(typeName != null)
					{
						return String.Format
							(_("Exception_TypeInitName"), typeName);
					}
					else
					{
						return _("Exception_TypeInit");
					}
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80131534;
				}
			}

#if CONFIG_SERIALIZATION
	// Get the serialization data for this object.
	public override void GetObjectData(SerializationInfo info,
									   StreamingContext context)
			{
				base.GetObjectData(info, context);
				info.AddValue("TypeName", typeName);
			}
#endif

}; // class TypeInitializationException

}; // namespace System
