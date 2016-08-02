/*
 * ObjRef.cs - Implementation of the
 *			"System.Runtime.Remoting.ObjRef" class.
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

using System.Runtime.Serialization;

[Serializable]
public class ObjRef : IObjectReference, ISerializable
{
	// Internal state.
	private IChannelInfo channelInfo;
	private IEnvoyInfo envoyInfo;
	private IRemotingTypeInfo typeInfo;
	private String uri;

	// Constructors.
	[TODO]
	public ObjRef()
			{
				// TODO
			}
	[TODO]
	public ObjRef(MarshalByRefObject o, Type requestedType)
			{
				// TODO
			}
	[TODO]
	protected ObjRef(SerializationInfo info, StreamingContext context)
			{
				// TODO
			}

	// Implement the IObjectReference interface.
	[TODO]
	public virtual Object GetRealObject(StreamingContext context)
			{
				// TODO
				return null;
			}

	// Implement the ISerializable interface.
	[TODO]
	public virtual void GetObjectData(SerializationInfo info,
									  StreamingContext context)
			{
				// TODO
			}

	// Determine if the object reference is from this process.
	[TODO]
	public bool IsFromThisProcess()
			{
				// TODO
				return false;
			}

	// Determine if the object reference is from this application domain.
	[TODO]
	public bool IsFromThisAppDomain()
			{
				// TODO
				return false;
			}

	// Object reference properties.
	public virtual IChannelInfo ChannelInfo
			{
				get
				{
					return channelInfo;
				}
				set
				{
					channelInfo = value;
				}
			}
	public virtual IEnvoyInfo EnvoyInfo
			{
				get
				{
					return envoyInfo;
				}
				set
				{
					envoyInfo = value;
				}
			}
	public virtual IRemotingTypeInfo TypeInfo
			{
				get
				{
					return typeInfo;
				}
				set
				{
					typeInfo = value;
				}
			}
	public virtual String URI
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

}; // class ObjRef

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting
