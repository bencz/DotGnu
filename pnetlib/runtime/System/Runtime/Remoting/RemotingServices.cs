/*
 * RemotingServices.cs - Implementation of the
 *			"System.Runtime.Remoting.RemotingServices" class.
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

namespace System.Runtime.Remoting
{

#if CONFIG_REMOTING

using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Contexts;
using System.Diagnostics;
using System.Threading;

public sealed class RemotingServices
{
	// This class cannot be instantiated.
	private RemotingServices() {}

	// Connect to a service and create a proxy object.
	public static Object Connect(Type classToProxy, String url)
			{
				return Connect(classToProxy, url, null);
			}
	[TODO]
	public static Object Connect(Type classToProxy, String url, Object data)
			{
				// TODO
				return null;
			}

	// Disconnect an object from all remote accesses.
	[TODO]
	public static bool Disconnect(MarshalByRefObject o)
			{
				// TODO
				return false;
			}

	// Execute a specific message.
	[TODO]
	public static IMethodReturnMessage ExecuteMessage
				(MarshalByRefObject target, IMethodCallMessage reqMsg)
			{
				// TODO
				return null;
			}

	// Get the envoy chain for a specific proxy object.
	public static IMessageSink GetEnvoyChainForProxy
				(MarshalByRefObject obj)
			{
				if(IsObjectOutOfContext(obj))
				{
					RealProxy proxy = GetRealProxy(obj);
					Identity id = proxy.Identity;
					if(id != null)
					{
						return id.envoyChain;
					}
				}
				return null;
			}

	// Get a lifetime service object.
	public static Object GetLifetimeService(MarshalByRefObject obj)
			{
				if(obj == null)
				{
					return null;
				}
				return obj.GetLifetimeService();
			}

	// Get the method associated with a specific message.
	[TODO]
	public static MethodBase GetMethodBaseFromMethodMessage(IMethodMessage msg)
			{
				// TODO
				return null;
			}

	// Serialize an object.
	public static void GetObjectData(Object obj, SerializationInfo info,
									 StreamingContext context)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				ObjRef objRef = Marshal((MarshalByRefObject)obj, null, null);
				objRef.GetObjectData(info, context);
			}

	// Get the URI for a specific object.
	public static String GetObjectUri(MarshalByRefObject obj)
			{
				if(obj != null)
				{
					Identity identity = obj.GetIdentity();
					if(identity != null)
					{
						return identity.uri;
					}
				}
				return null;
			}

	// Get an object reference that represents an object proxy.
	public static ObjRef GetObjRefForProxy(MarshalByRefObject obj)
			{
				if(IsTransparentProxy(obj))
				{
					RealProxy proxy = GetRealProxy(obj);
					Identity id = proxy.Identity;
					if(id != null)
					{
						return id.objRef;
					}
					else
					{
						return null;
					}
				}
				else
				{
					throw new RemotingException(_("Remoting_NoProxy"));
				}
			}

	// Get the real proxy underlying a transparent one.
	[TODO]
	public static RealProxy GetRealProxy(Object obj)
			{
				// TODO
				return null;
			}

	// Get the server type associated with a particular URI.
	[TODO]
	public static Type GetServerTypeForUri(String URI)
			{
				// TODO
				return null;
			}

	// Get the session ID from a particular message.
	public static String GetSessionIdForMethodMessage(IMethodMessage msg)
			{
				return msg.Uri;
			}

	// Determine if a method is overloaded.
	public static bool IsMethodOverloaded(IMethodMessage msg)
			{
				MethodBase method = msg.MethodBase;
				return (method.DeclaringType.GetMember
							(method.Name, MemberTypes.Method,
							 BindingFlags.Public |
							 BindingFlags.NonPublic |
							 BindingFlags.Instance).Length > 1);
			}

	// Determine if an object is outside the current application domain.
	public static bool IsObjectOutOfAppDomain(Object tp)
			{
				if(!(tp is MarshalByRefObject))
				{
					return false;
				}
				Identity id = ((MarshalByRefObject)tp).GetIdentity();
				if(id != null)
				{
					return id.otherAppDomain;
				}
				else
				{
					return false;
				}
			}

	// Determine if an object is outside the current context.
	public static bool IsObjectOutOfContext(Object tp)
			{
				if(IsTransparentProxy(tp))
				{
					RealProxy proxy = GetRealProxy(tp);
					Identity id = proxy.Identity;
					if(id != null && id.context != Thread.CurrentContext)
					{
						return true;
					}
				}
				return false;
			}

	// Determine if a method is one-way.
	public static bool IsOneWay(MethodBase method)
			{
				if(method != null)
				{
					return (method.GetCustomAttributes
						(typeof(OneWayAttribute), false).Length > 0);
				}
				else
				{
					return false;
				}
			}

	// Determine if an object is a transparent proxy.
	public static bool IsTransparentProxy(Object proxy)
			{
				return RealProxy.IsTransparentProxy(proxy);
			}

	// Set the log remoting stage.
	[Conditional("REMOTING_PERF")]
	public static void LogRemotingStage(int stage)
			{
				// Not used in this implementation.
			}

	// Marshal an object.
	public static ObjRef Marshal(MarshalByRefObject Obj)
			{
				return Marshal(Obj, null, null);
			}
	public static ObjRef Marshal(MarshalByRefObject Obj, String URI)
			{
				return Marshal(Obj, URI, null);
			}
	[TODO]
	public static ObjRef Marshal(MarshalByRefObject Obj, String ObjURI,
								 Type RequestedType)
			{
				// TODO
				return null;
			}

	// Set the URI to use to marshal an object.
	public static void SetObjectUriForMarshal
				(MarshalByRefObject obj, String uri)
			{
				if(obj == null)
				{
					return;
				}
				Identity id = obj.GetIdentity();
				if(id != null)
				{
					// Update the object's current identity.
					if(id.otherAppDomain)
					{
						throw new RemotingException(_("Remoting_NotLocal"));
					}
					if(id.uri != null)
					{
						throw new RemotingException(_("Remoting_HasIdentity"));
					}
					id.uri = uri;
				}
				else
				{
					// Create a new identity for the object and set it.
					id = new Identity();
					id.uri = uri;
					id.context = Thread.CurrentContext;
					id.otherAppDomain = false;
					obj.SetIdentity(id);
				}
			}

	// Create a proxy for an object reference.
	public static Object Unmarshal(ObjRef objectRef)
			{
				return Unmarshal(objectRef, false);
			}
	[TODO]
	public static Object Unmarshal(ObjRef objectRef, bool fRefine)
			{
				// TODO
				return null;
			}

	// Identity values, that are attached to marshalled objects.
	internal class Identity
	{
		// Accessible state.
		public String uri;
		public Context context;
		public bool otherAppDomain;
		public ObjRef objRef;
		public IMessageSink envoyChain;

	}; // class Identity

}; // class RemotingServices

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting
