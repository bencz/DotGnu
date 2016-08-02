/*
 * XsltCompileException.cs - Implementation of 
 *					"System.Xml.Xsl.XsltCompileException" class.
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

namespace System.Xml.Xsl
{
#if CONFIG_SERIALIZATION
	[Serializable]
#endif
	public class XsltCompileException: XsltException
	{
#if CONFIG_FRAMEWORK_2_0
		public XsltCompileException() : base(String.Empty, null)
		{
		}

		public XsltCompileException(string message) : base(message, null)
		{
		}

		public XsltCompileException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
#endif

		protected XsltCompileException(SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
		}

		public XsltCompileException(Exception inner, String sourceUri, 
									int lineNumber, int linePosition) 
									: base(sourceUri, lineNumber, linePosition,
											inner)
		{
		}

#if CONFIG_SERIALIZATION

		public override void GetObjectData(SerializationInfo info, 
											StreamingContext context)
		{
			base.GetObjectData (info, context);
		}

#endif

#if !CONFIG_FRAMEWORK_2_0
		public override String Message 
		{
 			get
			{
				return base.Message;
			}

 		}
#endif
	}
}//namespace
#endif // CONFIG_XSL
#endif // !ECMA_COMPAT
