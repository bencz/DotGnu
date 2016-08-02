/*
 * BadImageFormatException.cs - Implementation of the
 *		"System.BadImageFormatException" class.
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

public class BadImageFormatException : SystemException
{

	// Internal state.
	private String fileName;
#if !ECMA_COMPAT
	private String fusionLog;
#endif

	// Constructors.
	public BadImageFormatException()
		: base(_("Exception_BadImage")) {}
	public BadImageFormatException(String msg)
		: base(msg) {}
	public BadImageFormatException(String msg, Exception inner)
		: base(msg, inner) {}
	public BadImageFormatException(String msg, String fileName, Exception inner)
		: base(msg, inner) { this.fileName = fileName; }
	public BadImageFormatException(String msg, String fileName)
		: base(msg) { this.fileName = fileName; }
#if CONFIG_SERIALIZATION
	protected BadImageFormatException(SerializationInfo info,
									  StreamingContext context)
		: base(info, context)
		{
			fileName = info.GetString("BadImageFormat_FileName");
			fusionLog = info.GetString("BadImageFormat_FusionLog");
		}
#endif

	// Get the message for this exception.  Because of "MessageDefault",
	// we don't actually need this.  However, we include it because
	// the ECMA standard expects this to be present.
	public override String Message
			{
				get
				{
					return base.Message;
				}
			}

	// Convert this exception into a string.  Because of "MessageExtra",
	// we don't actually need this.  However, we include it because the
	// ECMA standard expects this to be present.
	public override String ToString()
			{
				return base.ToString();
			}

	// Properties.
	public String FileName
			{
				get
				{
					return fileName;
				}
			}
#if !ECMA_COMPAT
	public String FusionLog
			{
				get
				{
					return fusionLog;
				}
			}
#endif

	// Get the extra data to insert into "Exception::ToString()"'s result.
	internal override String MessageExtra
			{
				get
				{
					if(fileName != null)
					{
						return String.Format
							   		(_("Exception_Filename"), fileName);
					}
					else
					{
						return null;
					}
				}
			}

	// Get the default message to use for this exception type.
	internal override String MessageDefault
			{
				get
				{
					return _("Exception_BadImage");
				}
			}

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x8007000b;
				}
			}

#if CONFIG_SERIALIZATION
	// Get the serialization data for this object.
	public override void GetObjectData(SerializationInfo info,
									   StreamingContext context)
			{
				base.GetObjectData(info, context);
				info.AddValue("BadImageFormat_FileName", fileName);
				info.AddValue("BadImageFormat_FusionLog", fusionLog);
			}
#endif

}; // class BadImageFormatException

#endif // CONFIG_RUNTIME_INFRA

}; // namespace System
