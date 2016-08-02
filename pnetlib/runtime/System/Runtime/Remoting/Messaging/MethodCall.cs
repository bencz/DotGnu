/*
 * MethodCall.cs - Implementation of the
 *			"System.Runtime.Remoting.Messaging.MethodCall" class.
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
public class MethodCall : IMethodCallMessage, ISerializable,
						  IMethodMessage, IMessage, ISerializationRootObject,
						  IMessageDictionary
{
	// Internal state.
	protected IDictionary ExternalProperties;
	protected IDictionary InternalProperties;
	private Object[] args;
	private String methodName;
	private String typeName;
	private String uri;
	private bool hasVarArgs;
	private bool isSoap;
	private LogicalCallContext context;
	private MethodBase method;
	private ParameterInfo[] parameters;
	private Object srvID;
	private Type[] signature;

	// Constructors.
	public MethodCall(Header[] h1)
			{
				isSoap = true;		// This form is used for SOAP requests.
				Init();
				if(h1 != null)
				{
					foreach(Header h in h1)
					{
						ProcessHeader(h.Name, h.Value);
					}
				}
				ResolveMethod();
				AccessCheck();
			}
	public MethodCall(IMessage msg)
			{
				if(msg == null)
				{
					throw new ArgumentNullException("msg");
				}
				Init();
				IDictionaryEnumerator e = msg.Properties.GetEnumerator();
				while(e.MoveNext())
				{
					ProcessHeader(e.Key.ToString(), e.Value);
				}
				ResolveMethod();
				AccessCheck();
			}
	internal MethodCall(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				Init();
				RootSetObjectData(info, context);
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
					if(args != null)
					{
						return args.Length;
					}
					else
					{
						return 0;
					}
				}
			}
	public Object[] Args
			{
				get
				{
					return args;
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
	public int InArgCount
			{
				get
				{
					FetchParameters();
					int count = 0;
					if(parameters != null)
					{
						foreach(ParameterInfo p in parameters)
						{
							if(!(p.ParameterType.IsByRef))
							{
								++count;
							}
						}
					}
					return count;
				}
			}
	public Object[] InArgs
			{
				get
				{
					int count = InArgCount;
					Object[] inArgs = new Object [count];
					if(parameters != null && args != null)
					{
						int posn;
						count = 0;
						for(posn = 0; posn < args.Length; ++posn)
						{
							if(!(parameters[posn].ParameterType.IsByRef))
							{
								inArgs[count++] = args[posn];
							}
						}
					}
					return inArgs;
				}
			}
	public Object GetArg(int argNum)
			{
				return args[argNum];
			}
	public String GetArgName(int index)
			{
				FetchParameters();
				return parameters[index].Name;
			}
	public Object GetInArg(int argNum)
			{
				FetchParameters();
				if(parameters != null && args != null)
				{
					int posn;
					for(posn = 0; posn < args.Length; ++posn)
					{
						if(!(parameters[posn].ParameterType.IsByRef))
						{
							if(argNum == 0)
							{
								return args[posn];
							}
							--argNum;
						}
					}
				}
				throw new IndexOutOfRangeException(_("Arg_InvalidArrayIndex"));
			}
	public String GetInArgName(int index)
			{
				FetchParameters();
				if(parameters != null)
				{
					int posn;
					for(posn = 0; posn < args.Length; ++posn)
					{
						if(!(parameters[posn].ParameterType.IsByRef))
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
	public void GetObjectData(SerializationInfo info,
							  StreamingContext context)
			{
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

				// Resolve the method.
				ResolveMethod(false);
				return null;
			}

	// Initialization helper.
	public virtual void Init()
			{
				// Nothing to do here.
			}

	// Resolve the method.
	public void ResolveMethod()
			{
				ResolveMethod(true);
			}
	[TODO]
	private void ResolveMethod(bool throwOnError)
			{
				// TODO
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

				// De-serialize the main values.
				SerializationInfoEnumerator se = info.GetEnumerator();
				while(se.MoveNext())
				{
					ProcessHeader(se.Name, se.Value);
				}

				// Process the headers in the streaming context.
				if(context.State == StreamingContextStates.Remoting)
				{
					Header[] headers = (context.Context as Header[]);
					if(headers != null)
					{
						foreach(Header header in headers)
						{
							ProcessHeader(header.Name, header.Value);
						}
					}
				}
			}

	// Set the server identity within this object.
	internal void SetServerIdentity(Object srvID)
			{
				this.srvID = srvID;
			}

	// Process a header.
	private void ProcessHeader(String name, Object value)
			{
				Properties[name] = value;
			}

	// Perform an access check on the resolved method.
	[TODO]
	private void AccessCheck()
			{
				// TODO
			}

	// Fetch the parameter information from the method block.
	private void FetchParameters()
			{
				if(parameters == null && method != null)
				{
					parameters = method.GetParameters();
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
					return new String[] {
						"__Uri", "__MethodName", "__MethodSignature",
						"__TypeName", "__Args", "__CallContext"
					};
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
					case "__Args":				return Args;
					case "__CallContext":		return LogicalCallContext;
				}
				return null;
			}
	internal virtual void SetSpecialProperty(String name, Object value)
			{
				switch(name)
				{
					case "__Uri":				Uri = (String)value; break;
					case "__CallContext":
						context = (LogicalCallContext)value; break;
				}
			}

}; // class MethodCall

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Messaging
