/*
 * RealProxy.cs - Implementation of the
 *			"System.Runtime.Remoting.Proxies.RealProxy" class.
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

namespace System.Runtime.Remoting.Proxies
{

#if CONFIG_REMOTING

using System.Runtime.Serialization;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.CompilerServices;
using System.Threading;

public abstract class RealProxy
{
	// Internal state.
	private Type type;
	private Object stubData;
	private MarshalByRefObject serverObject;
	private Object proxy;
	private RemotingServices.Identity identity;
	private static readonly Object defaultStub = (Object)(-1);

	// Constructor.
	protected RealProxy() {}
	protected RealProxy(Type classToProxy)
			: this(classToProxy, IntPtr.Zero, null) {}
	protected RealProxy(Type classToProxy, IntPtr stub, Object stubData)
			{
				// The class must be either an interface or MarshalByRef.
				if(!(classToProxy.IsInterface) &&
				   !(classToProxy.IsMarshalByRef))
				{
					throw new ArgumentException
						(_("Remoting_NotMarshalOrInterface"),
						 "classToProxy");
				}

				// Get the default stub data if necessary.
				if(stub == IntPtr.Zero)
				{
					stubData = defaultStub;
				}

				// Validate the stub data.
				if(stubData == null)
				{
					throw new ArgumentNullException("stubData");
				}

				// Initialize this object's state.
				this.type = classToProxy;
				this.stubData = stubData;
				this.serverObject = null;
				this.proxy = CreateTransparentProxy(classToProxy);
			}

	// Create an object reference of the specified type.
	public virtual ObjRef CreateObjRef(Type requestedType)
			{
				return RemotingServices.Marshal
					((MarshalByRefObject)GetTransparentProxy(),
					 null, requestedType);
			}

	// Get the unmanaged IUnknown proxy instance.
	public virtual IntPtr GetCOMIUnknown(bool fIsMarshalled)
			{
				// COM is not supported in this implementation.
				return IntPtr.Zero;
			}

	// Get the serialization data for this instance.
	public virtual void GetObjectData(SerializationInfo info,
									  StreamingContext context)
			{
				RemotingServices.GetObjectData
					(GetTransparentProxy(), info, context);
			}

	// Get the type being proxied by this instance.
	public Type GetProxiedType()
			{
				if(!(type.IsInterface))
				{
					return type;
				}
				else
				{
					return typeof(MarshalByRefObject);
				}
			}

	// Get the stub data within a proxy.
	public static Object GetStubData(RealProxy rp)
			{
				if(rp != null)
				{
					return rp.stubData;
				}
				else
				{
					return null;
				}
			}

	// Get a transparent proxy for the current instance.
	public virtual Object GetTransparentProxy()
			{
				return proxy;
			}

	// Initialize a server object.
	[TODO]
	public IConstructionReturnMessage InitializeServerObject
				(IConstructionCallMessage ctorMsg)
			{
				// Nothing to do if we already have a server object.
				if(serverObject != null)
				{
					return null;
				}

				// Create the server object.
				Type serverType = GetProxiedType();
				if(ctorMsg != null && ctorMsg.ActivationType != serverType)
				{
					throw new RemotingException(_("Remoting_CtorMsg"));
				}
				serverObject = (MarshalByRefObject)
					FormatterServices.GetUninitializedObject(serverType);
				if(stubData == defaultStub)
				{
					stubData = Thread.CurrentContext.ContextID;
				}
				Object proxy = GetTransparentProxy();
				if(ctorMsg == null)
				{
					// TODO: create a default constructor call message.
				}
				IMethodReturnMessage returnMessage;
				returnMessage = RemotingServices.ExecuteMessage
					((MarshalByRefObject)proxy, ctorMsg);
				return new ConstructionResponse(returnMessage);
			}

	// Invoke a message on the object underlying this proxy.
	public abstract IMessage Invoke(IMessage msg);

	// Set the unmanaged IUnknown information for this object.
	public virtual void SetCOMIUnknown(IntPtr i)
			{
				// COM is not supported in this implementation.
			}

	// Set the stub data for a particular proxy.
	public static void SetStubData(RealProxy rp, Object stubData)
			{
				if(rp != null)
				{
					rp.stubData = stubData;
				}
			}

	// Get a COM interface for a particular Guid on this object.
	public virtual IntPtr SupportsInterface(ref Guid iid)
			{
				// COM is not supported in this implementation.
				return IntPtr.Zero;
			}

	// Attach this proxy to a remote server.
	protected void AttachServer(MarshalByRefObject s)
			{
				serverObject = s;
			}

	// Detach this proxy from a remote server.
	protected MarshalByRefObject DetachServer()
			{
				MarshalByRefObject saved = serverObject;
				serverObject = null;
				return saved;
			}

	// Get the unwrapped server instance for this proxy.
	protected MarshalByRefObject GetUnwrappedServer()
			{
				return serverObject;
			}

	// Create a transparency proxy using the runtime engine.
	//[MethodImpl(MethodImplOptions.InternalCall)]
	[TODO]
	internal static Object CreateTransparentProxy(Type serverType)
			{
				// TODO: make into an internalcall
				return null;
			}

	// Determine if an object is a transparent proxy.
	//[MethodImpl(MethodImplOptions.InternalCall)]
	[TODO]
	internal static bool IsTransparentProxy(Object proxy)
			{
				// TODO: make into an internalcall.
				return false;
			}

	// Get this proxy's identity.
	internal RemotingServices.Identity Identity
			{
				get
				{
					return identity;
				}
				set
				{
					identity = value;
				}
			}

}; // class RealProxy

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Proxies
