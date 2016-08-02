/*
 * WebException.cs - Implementation of the "System.Net.WebException" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Net
{

using System;
using System.Runtime.Serialization;

#if !ECMA_COMPAT
[Serializable]
#endif
public class WebException : InvalidOperationException
#if CONFIG_SERIALIZATION
	, ISerializable
#endif
{
	//Variables
	private WebResponse myresponse;
	private WebExceptionStatus mystatus; 	
	
	// Constructors.
	public WebException()
		: base(S._("Exception_Web"))
		{
		#if !ECMA_COMPAT
			HResult = unchecked((int)0x80131509);
		#endif
		}
	public WebException(String msg)
		: base(msg)
		{
		#if !ECMA_COMPAT
			HResult = unchecked((int)0x80131509);
		#endif
		}
	public WebException(String msg, Exception inner)
		: base(msg, inner)
		{
		#if !ECMA_COMPAT
			HResult = unchecked((int)0x80131509);
		#endif
		}
	public WebException(String msg, WebExceptionStatus status)
		: base(msg)
			{
				myresponse = null;
				mystatus = status;
			#if !ECMA_COMPAT
				HResult = unchecked((int)0x80131509);
			#endif
			}
	public WebException(String msg, Exception inner, 
		WebExceptionStatus status, WebResponse response) 
		: base(msg, inner) 
			{
				myresponse = response;
				mystatus = status;
			}
#if CONFIG_SERIALIZATION
	protected WebException(SerializationInfo info, StreamingContext context)
		: base(info, context) {}
#endif
	
	
	//Properties
	public WebResponse Response 
			{
				get
				{	
					return myresponse;
				}
			}
	
	public WebExceptionStatus Status
			{
				get
				{
					return mystatus;
				}
			} 
		

#if CONFIG_SERIALIZATION
	// Get the serialization data for this object.
	void ISerializable.GetObjectData(SerializationInfo info,
									 StreamingContext context)
			{
				base.GetObjectData(info, context);
			}
#endif

}; // class WebException

}; // namespace System.Net
