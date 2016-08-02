/*
 * SoapMessage.cs - Implementation of the
 *			"System.Runtime.Serialization.SoapMessage" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Runtime.Serialization.Formatters
{

#if CONFIG_REMOTING

using System.Runtime.Remoting.Messaging;

[Serializable]
public class SoapMessage : ISoapMessage
{
	// Internal state.
	private Header[] headers;
	private String methodName;
	private String[] paramNames;
	private Type[] paramTypes;
	private Object[] paramValues;
	private String xmlNameSpace;

	// Constructor.
	public SoapMessage() {}

	// Get or set the object properties.
	public Header[] Headers
			{
				get
				{
					return headers;
				}
				set
				{
					headers = value;
				}
			}
	public String MethodName
			{
				get
				{
					return methodName;
				}
				set
				{
					methodName = value;
				}
			}
	public String[] ParamNames
			{
				get
				{
					return paramNames;
				}
				set
				{
					paramNames = value;
				}
			}
	public Type[] ParamTypes
			{
				get
				{
					return paramTypes;
				}
				set
				{
					paramTypes = value;
				}
			}
	public Object[] ParamValues
			{
				get
				{
					return paramValues;
				}
				set
				{
					paramValues = value;
				}
			}
	public String XmlNameSpace
			{
				get
				{
					return xmlNameSpace;
				}
				set
				{
					xmlNameSpace = value;
				}
			}

}; // class SoapMessage

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Serialization.Formatters
