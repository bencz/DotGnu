/*
 * MethodResponse.cs - Implementation of the
 *			"System.Runtime.Remoting.Messaging.MethodResponse" class.
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

namespace System.Runtime.Remoting.Messaging
{

#if CONFIG_REMOTING

using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;

[Serializable]
[CLSCompliant(false)]
public class MethodResponse : IMethodReturnMessage, ISerializable,
							  IMethodMessage, IMessage,
							  ISerializationRootObject,
							  IMessageDictionary
{
	// Internal state.
	protected IDictionary ExternalProperties;
	protected IDictionary InternalProperties;
	private Object[] outArgs;
	private String methodName;
	private String typeName;
	private String uri;
	private bool hasVarArgs;
	private bool isSoap;
	private LogicalCallContext context;
	private MethodBase method;
	private ParameterInfo[] parameters;
	private Type[] signature;
	private Exception exception;
	private Object returnValue;

	// Constructors.
	public MethodResponse(Header[] h1, IMethodCallMessage mcm)
			{
				isSoap = true;		// This form is used for SOAP requests.
				if(mcm == null)
				{
					throw new ArgumentNullException("mcm");
				}
				methodName = mcm.MethodName;
				typeName = mcm.TypeName;
				method = mcm.MethodBase;
				hasVarArgs = mcm.HasVarArgs;
				if(h1 != null)
				{
					foreach(Header header in h1)
					{
						ProcessHeader(header.Name, header.Value);
					}
				}
			}
	internal MethodResponse(IMethodReturnMessage mrm)
			{
				outArgs = mrm.OutArgs;
				methodName = mrm.MethodName;
				typeName = mrm.TypeName;
				uri = mrm.Uri;
				hasVarArgs = mrm.HasVarArgs;
				context = mrm.LogicalCallContext;
				method = mrm.MethodBase;
				exception = mrm.Exception;
				returnValue = mrm.ReturnValue;
			}

	// Implement the IMethodCallMessage interface.
	public virtual IDictionary Properties
			{
				get
				{
					if(InternalProperties == null)
					{
						InternalProperties = new Hashtable();
					}
					if(ExternalProperties == null)
					{
						ExternalProperties = new MessageProperties
							(this, InternalProperties);
					}
					return ExternalProperties;
				}
			}
	public int ArgCount
			{
				get
				{
					return OutArgCount;
				}
			}
	public Object[] Args
			{
				get
				{
					return OutArgs;
				}
			}
	public bool HasVarArgs
			{
				get
				{
					return hasVarArgs;
				}
			}
	public LogicalCallContext LogicalCallContext
			{
				get
				{
					if(context == null)
					{
						context = new LogicalCallContext();
					}
					return context;
				}
			}
	public MethodBase MethodBase
			{
				get
				{
					return method;
				}
			}
	public String MethodName
			{
				get
				{
					return methodName;
				}
			}
	public Object MethodSignature
			{
				get
				{
					if(signature == null)
					{
						FetchParameters();
						if(parameters != null)
						{
							signature = new Type [parameters.Length];
							int posn;
							for(posn = 0; posn < signature.Length; ++posn)
							{
								signature[posn] =
									parameters[posn].ParameterType;
							}
						}
					}
					return signature;
				}
			}
	public String TypeName
			{
				get
				{
					return typeName;
				}
			}
	public String Uri
			{
				get
				{
					return uri;
				}
				set
				{
					uri = value;
				}
			}
	public Object GetArg(int argNum)
			{
				return GetOutArg(argNum);
			}
	public String GetArgName(int index)
			{
				return GetOutArgName(index);
			}
	public Exception Exception
			{
				get
				{
					return exception;
				}
			}
	public int OutArgCount
			{
				get
				{
					if(outArgs != null)
					{
						return outArgs.Length;
					}
					else
					{
						return 0;
					}
				}
			}
	public Object[] OutArgs
			{
				get
				{
					return outArgs;
				}
			}
	public Object ReturnValue
			{
				get
				{
					return returnValue;
				}
			}
	public Object GetOutArg(int argNum)
			{
				return outArgs[argNum];
			}
	public String GetOutArgName(int index)
			{
				FetchParameters();
				if(parameters != null && outArgs != null)
				{
					int posn;
					for(posn = 0; posn < parameters.Length; ++posn)
					{
						if(parameters[posn].ParameterType.IsByRef)
						{
							if(index == 0)
							{
								return parameters[posn].Name;
							}
							--index;
						}
					}
				}
				throw new IndexOutOfRangeException(_("Arg_InvalidArrayIndex"));
			}

	// Implement the ISerializable interface.
	public virtual void GetObjectData(SerializationInfo info,
							  		  StreamingContext context)
			{
				// Not needed.
				throw new NotSupportedException();
			}

	// Handle incoming headers.
	public virtual Object HeaderHandler(Header[] h)
			{
				// Extract the method name from the headers, if present.
				if(h != null && h.Length != 0 && h[0].Name == "__methodName")
				{
					methodName = (String)(h[0].Value);
					if(h.Length != 1)
					{
						Header[] nh = new Header [h.Length - 1];
						Array.Copy(h, 1, nh, 0, h.Length - 1);
						nh = h;
					}
					else
					{
						h = null;
					}
				}

				// Process the headers to set the message properties.
				if(h != null)
				{
					foreach(Header header in h)
					{
						ProcessHeader(header.Name, header.Value);
					}
				}
				return null;
			}

	// Process a header.
	private void ProcessHeader(String name, Object value)
			{
				Properties[name] = value;
			}

	// Fetch the parameter information from the method block.
	private void FetchParameters()
			{
				if(parameters == null && method != null)
				{
					parameters = method.GetParameters();
				}
			}

	// Set the root object data for a SOAP method call.
	[TODO]
	private void RootSetSoapObjectData(SerializationInfo info)
			{
				// TODO
			}

	// Set the root object data for this method call.
	public void RootSetObjectData(SerializationInfo info,
								  StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}

				// Use a different algorithm for SOAP messages.
				if(isSoap)
				{
					RootSetSoapObjectData(info);
					return;
				}

				// De-serialize the supplied data.
				SerializationInfoEnumerator se = info.GetEnumerator();
				while(se.MoveNext())
				{
					if(se.Name == "__return")
					{
						exception = null;
					}
					else if(se.Name == "__fault")
					{
						exception = (Exception)(se.Value);
					}
					else
					{
						ProcessHeader(se.Name, se.Value);
					}
				}
			}

	// Implement the IMessageDictionary interface.
	String[] IMessageDictionary.SpecialProperties
			{
				get
				{
					return SpecialProperties;
				}
			}
	Object IMessageDictionary.GetSpecialProperty(String name)
			{
				return GetSpecialProperty(name);
			}
	void IMessageDictionary.SetSpecialProperty(String name, Object value)
			{
				SetSpecialProperty(name, value);
			}
	internal virtual String[] SpecialProperties
			{
				get
				{
					if(Exception == null)
					{
						return new String[] {
							"__Uri", "__MethodName", "__MethodSignature",
							"__TypeName", "__Return", "__OutArgs",
							"__CallContext"
						};
					}
					else
					{
						return new String[] {
							"__Uri", "__MethodName", "__MethodSignature",
							"__TypeName", "__CallContext"
						};
					}
				}
			}
	internal virtual Object GetSpecialProperty(String name)
			{
				switch(name)
				{
					case "__Uri":				return Uri;
					case "__MethodName":		return MethodName;
					case "__MethodSignature":	return MethodSignature;
					case "__TypeName":			return TypeName;
					case "__Return":
						if(Exception != null)
						{
							return Exception;
						}
						else
						{
							return ReturnValue;
						}
					case "__OutArgs":			return OutArgs;
					case "__CallContext":		return LogicalCallContext;
				}
				return null;
			}
	internal virtual void SetSpecialProperty(String name, Object value)
			{
				switch(name)
				{
					case "__Uri":		Uri = (String)value; break;
					case "__CallContext":
						context = (LogicalCallContext)value; break;
				}
			}

}; // class MethodResponse

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Messaging
