/*
 * XsltException.cs - Implementation of "System.Xml.Xsl.XsltException" 
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * 
 * Contributed by Gopal.V 
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

#if !ECMA_COMPAT
#if CONFIG_XSL

using System;
using System.Runtime.Serialization;
using System.Xml.XPath;

namespace System.Xml.Xsl
{
#if CONFIG_SERIALIZATION
	[Serializable]
#endif
	public class XsltException: System.SystemException
	{
		private int lineNumber;
		private int linePosition;
		private String sourceUri;

		private static String CreateMessage(String message, XPathNavigator nav)
		{
			IXmlLineInfo li = nav as IXmlLineInfo;
			int lineNumber = li != null ? li.LineNumber : 0;
			int linePosition = li != null ? li.LinePosition : 0;
			String sourceUri = nav != null ? nav.BaseURI : String.Empty;
			return CreateMessage(lineNumber, linePosition, sourceUri, message);
		}

		private static String CreateMessage(int lineNumber, int linePosition,
											String sourceUri, String msg)
		{
			if(sourceUri != null)
			{
				msg = String.Concat(msg, " ", sourceUri);
			}
			if(lineNumber != 0)
			{
				msg = String.Concat(msg, " line ", lineNumber);
			}
			if(linePosition != 0)
			{
				msg = String.Concat(msg, ", position ", linePosition);
			}
			return msg;
		}

#if CONFIG_FRAMEWORK_2_0
		public XsltException() : base(String.Empty, null)
		{
		}

		public XsltException(String message) : base(message, null)
		{
		}
#endif

		public XsltException(String message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected internal XsltException(String sourceUri, int lineNumber,
								int linePosition, Exception innerException)
			: base(CreateMessage(lineNumber, linePosition, sourceUri, String.Empty),
					innerException)
		{
		}

#if CONFIG_SERIALIZATION

		protected XsltException(SerializationInfo info, 
								StreamingContext context)
		{
			lineNumber = info.GetInt32("lineNumber");
			linePosition = info.GetInt32("linePosition");
			sourceUri = info.GetString("sourceUri");
		}

		public override void GetObjectData(SerializationInfo info, 
											StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("lineNumber", lineNumber);
			info.AddValue("linePosition", linePosition);
			info.AddValue("sourceUri", sourceUri);
		}

#endif

		public int LineNumber 
		{
 			get
			{
				return lineNumber;
			}
 		}

		public int LinePosition 
		{
 			get
			{
				return linePosition;
			}
 		}

		public override String Message 
		{
 			get
			{
				String msg = base.Message;
				if(sourceUri != null)
				{
					msg = String.Concat(msg, " ", sourceUri);
				}
				if(lineNumber != 0)
				{
					msg = String.Concat(msg, " line ", lineNumber);
				}
				if(linePosition != 0)
				{
					msg = String.Concat(msg, ", position ", linePosition);
				}
				return msg;
			}
 		}

		public String SourceUri 
		{
 			get
			{
				return sourceUri;
			}
 		}

	}
}//namespace
#endif // CONFIG_XSL
#endif // !ECMA_COMPAT
