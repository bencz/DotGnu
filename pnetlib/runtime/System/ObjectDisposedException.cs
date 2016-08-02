/*
 * ObjectDisposedException.cs - Implementation of the
 *			"System.ObjectDisposedException" class.
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

public class ObjectDisposedException : InvalidOperationException
{
	// Internal state.
	private String objectName;

	// Constructors.
	public ObjectDisposedException(String objectName)
		: base(_("Exception_Disposed"))
		{ this.objectName = objectName; }
	public ObjectDisposedException(String objectName, String msg)
		: base(msg) { this.objectName = objectName; }
#if CONFIG_SERIALIZATION
	protected ObjectDisposedException(SerializationInfo info,
									  StreamingContext context)
		: base(info, context)
		{
			objectName = info.GetString("ObjectName");
		}
#endif

	// Properties.
	public String ObjectName
			{
				get
				{
					return objectName;
				}
			}
	public override String Message
			{
				get
				{
					if(objectName != null)
					{
						return objectName + ": " + base.Message;
					}
					else
					{
						return base.Message;
					}
				}
			}

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_Disposed");
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80131509;
				}
			}

#if CONFIG_SERIALIZATION
	// Get the serialization data for this object.
	public override void GetObjectData(SerializationInfo info,
									   StreamingContext context)
			{
				base.GetObjectData(info, context);
				info.AddValue("ObjectName", objectName);
			}
#endif

}; // class ObjectDisposedException

}; // namespace System
