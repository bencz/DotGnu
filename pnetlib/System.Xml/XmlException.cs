/*
 * XmlException.cs - Implementation of the "System.Xml.XmlException" class.
 *
 * Copyright (C) 2002 Southern Storm Software, Pty Ltd.
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
 
namespace System.Xml
{

using System.Runtime.Serialization;

#if !ECMA_COMPAT
[Serializable]
#endif
public class XmlException : SystemException
{
	// Internal state.
	private int lineNumber, linePosition;

	// Constructors.
	public XmlException()
		: base(S._("Xml_Default"))
		{
		#if !ECMA_COMPAT
			HResult = unchecked((int)0x80131940);
		#endif
		}
	public XmlException(String message)
		: base(message)
		{
		#if !ECMA_COMPAT
			HResult = unchecked((int)0x80131940);
		#endif
		}
	public XmlException(String message, Exception innerException)
		: base(message, innerException)
		{
		#if !ECMA_COMPAT
			HResult = unchecked((int)0x80131940);
		#endif
		}
	public XmlException(String message, Exception innerException,
						int lineNumber, int linePosition)
		: base(message, innerException)
		{
		#if !ECMA_COMPAT
			HResult = unchecked((int)0x80131940);
		#endif
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
		}
#if CONFIG_SERIALIZATION
	protected XmlException(SerializationInfo info, StreamingContext context)
		: base(info, context)
		{
			HResult = unchecked((int)0x80131940);
			lineNumber = info.GetInt32("lineNumber");
			linePosition = info.GetInt32("linePosition");
		}
#endif
	
	// Get the default message to use for this exception type.
	public override String Message
			{
				get
				{
					String parentMsg = base.Message;
					if(parentMsg != null)
					{
						return parentMsg;
					}
					else
					{
						return S._("Xml_Default");
					}
				}
			}

	// Get the line number.
	public int LineNumber
			{
				get
				{
					return lineNumber;
				}
			}

	// Get the line position.
	public int LinePosition
			{
				get
				{
					return linePosition;
				}
			}

#if CONFIG_SERIALIZATION
	// Get the serialization data for this object.
	public override void GetObjectData(SerializationInfo info,
									   StreamingContext context)
			{
				base.GetObjectData(info, context);
				info.AddValue("lineNumber", lineNumber);
				info.AddValue("linePosition", linePosition);
				info.AddValue("res", String.Empty);		// For compatibility.
				info.AddValue("args", String.Empty);	// For compatibility.
			}
#endif

}; // class XmlException

}; // namespace System.Xml
