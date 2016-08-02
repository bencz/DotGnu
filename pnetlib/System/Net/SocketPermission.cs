/*
 * SocketPermission.cs - Implementation of the
 *		"System.Security.Permissions.SocketPermission" class.
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

namespace System.Net
{

#if CONFIG_PERMISSIONS

using System;
using System.Collections;
using System.Security;
using System.Security.Permissions;

public sealed class SocketPermission : CodeAccessPermission
#if !ECMA_COMPAT
	, IUnrestrictedPermission
#endif
{
	// Internal state.
	private PermissionState state;
	private ArrayList permissions;

	// Special port value that indicates "all ports".
	public const int AllPorts = -1;

	// Constructors.
	public SocketPermission(PermissionState state)
			{
				this.state = state;
				this.permissions = new ArrayList();
			}
	public SocketPermission(NetworkAccess access, TransportType transport,
							String hostName, int portNumber)
			{
				if(hostName == null)
				{
					throw new ArgumentNullException("hostName");
				}
				this.state = PermissionState.None;
				this.permissions = new ArrayList();
				this.permissions.Add
					(new EndpointPermission(access, transport,
											  hostName, portNumber));
			}
	internal SocketPermission(PermissionState state, ArrayList permissions)
			{
				this.state = state;
				this.permissions = (ArrayList)(permissions.Clone());
			}

	// Form the intersection of two EndpointPermission objects.
	// Returns null if no possible intersection between the two.
	private static EndpointPermission Intersect
				(EndpointPermission info1, EndpointPermission info2)
			{
				TransportType transport;
				int portNumber;
				String hostName;

				// Check the access values.
				if(info1.access != info2.access)
				{
					return null;
				}

				// Check the transport values.
				if(info1.transport == TransportType.All)
				{
					transport = info2.transport;
				}
				else if(info2.transport == TransportType.All)
				{
					transport = info1.transport;
				}
				else if(info1.transport == info2.transport)
				{
					transport = info1.transport;
				}
				else
				{
					return null;
				}

				// Check the port values.
				if(info1.portNumber == AllPorts)
				{
					portNumber = info2.portNumber;
				}
				else if(info2.portNumber == AllPorts)
				{
					portNumber = info1.portNumber;
				}
				else if(info1.portNumber == info2.portNumber)
				{
					portNumber = info1.portNumber;
				}
				else
				{
					return null;
				}

				// Check the hostnames.
				if(info1.hostName == "*.*.*.*")
				{
					hostName = info2.hostName;
				}
				else if(info2.hostName == "*.*.*.*")
				{
					hostName = info1.hostName;
				}
				else if(String.Compare(info1.hostName, info2.hostName, true)
							== 0)
				{
					hostName = info1.hostName;
				}
				else
				{
					return null;
				}

				// Build a new object for the intersection.
				return new EndpointPermission
					(info1.access, transport, hostName, portNumber);
			}

	// Determine if this object contains a specific permission as a subset.
	private bool Contains(EndpointPermission info1)
			{
				foreach(EndpointPermission info2 in permissions)
				{
					if(info1.access != info2.access)
					{
						continue;
					}
					if(info2.transport != TransportType.All &&
					   info1.transport != info2.transport)
					{
						continue;
					}
					if(info2.portNumber != AllPorts &&
					   info1.portNumber != info2.portNumber)
					{
						continue;
					}
					if(info2.hostName != "*.*.*.*" &&
					   info1.hostName != info2.hostName)
					{
						continue;
					}
					return true;
				}
				return false;
			}

	// Add end-point information that is parsed from an XML element list.
	private void AddFromXml(SecurityElement element, NetworkAccess access)
			{
				ArrayList children = element.Children;
				String value;
				TransportType transport;
				int portNumber;
				if(children == null)
				{
					return;
				}
				foreach(SecurityElement child in children)
				{
					if(child.Tag != "ENDPOINT")
					{
						continue;
					}
					value = child.Attribute("transport");
					if(value == null)
					{
						throw new ArgumentNullException("transport");
					}
					transport = (TransportType)Enum.Parse
						(typeof(TransportType), value, true);
					value = child.Attribute("port");
					if(value == null)
					{
						throw new ArgumentNullException("port");
					}
					if(String.Compare(value, "All", true) == 0)
					{
						portNumber = AllPorts;
					}
					else
					{
						portNumber = Int32.Parse(value);
					}
					value = child.Attribute("hostname");
					if(value == null)
					{
						throw new ArgumentNullException("hostname");
					}
					permissions.Add(new EndpointPermission
						(access, transport, value, portNumber));
				}
			}

	// Convert an XML value into a permissions value.
	public override void FromXml(SecurityElement esd)
			{
				String value;
				if(esd == null)
				{
					throw new ArgumentNullException("esd");
				}
				if(esd.Attribute("version") != "1")
				{
					throw new ArgumentException(S._("Arg_PermissionVersion"));
				}
				value = esd.Attribute("Unrestricted");
				if(value != null && Boolean.Parse(value))
				{
					state = PermissionState.Unrestricted;
				}
				else
				{
					state = PermissionState.None;
				}
				permissions.Clear();
				if(state != PermissionState.Unrestricted)
				{
					SecurityElement child =
						esd.SearchForChildByTag("ConnectAccess");
					if(child != null)
					{
						AddFromXml(child, NetworkAccess.Connect);
					}
					child = esd.SearchForChildByTag("AcceptAccess");
					if(child != null)
					{
						AddFromXml(child, NetworkAccess.Accept);
					}
				}
			}

	// Count the number of endpoints with a specific access value.
	private int CountWithAccess(NetworkAccess access)
			{
				int count = 0;
				foreach(EndpointPermission info in permissions)
				{
					if(info.access == access)
					{
						++count;
					}
				}
				return count;
			}

	// Convert a list of end-points into an XML element list.
	private void AddToXml(SecurityElement parent, NetworkAccess access)
			{
				SecurityElement child;
				foreach(EndpointPermission info in permissions)
				{
					if(info.access == access)
					{
						child = new SecurityElement("ENDPOINT");
						parent.AddChild(child);
						child.AddAttribute
							("host", SecurityElement.Escape(info.hostName));
						child.AddAttribute
							("transport", info.transport.ToString());
						if(info.portNumber == AllPorts)
						{
							child.AddAttribute("port", "All");
						}
						else
						{
							child.AddAttribute
								("port", info.portNumber.ToString());
						}
					}
				}
			}

	// Convert this permissions object into an XML value.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(SocketPermission).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				if(state == PermissionState.Unrestricted)
				{
					element.AddAttribute("Unrestricted", "true");
				}
				else
				{
					SecurityElement child;
					if(CountWithAccess(NetworkAccess.Connect) > 0)
					{
						child = new SecurityElement("ConnectAccess");
						element.AddChild(child);
						AddToXml(child, NetworkAccess.Connect);
					}
					if(CountWithAccess(NetworkAccess.Accept) > 0)
					{
						child = new SecurityElement("AcceptAccess");
						element.AddChild(child);
						AddToXml(child, NetworkAccess.Accept);
					}
				}
				return element;
			}

	// Implement the IPermission interface.
	public override IPermission Copy()
			{
				return new SocketPermission(state, permissions);
			}
	public override IPermission Intersect(IPermission target)
			{
				if(target == null)
				{
					return target;
				}
				else if(!(target is SocketPermission))
				{
					throw new ArgumentException(S._("Arg_PermissionMismatch"));
				}
				else if(((SocketPermission)target).IsUnrestricted())
				{
					return Copy();
				}
				else if(IsUnrestricted())
				{
					return target.Copy();
				}
				else
				{
					SocketPermission perm = new SocketPermission
						(PermissionState.None);
					EndpointPermission newInfo;
					foreach(EndpointPermission info in permissions)
					{
						foreach(EndpointPermission info2 in
									((SocketPermission)target).permissions)
						{
							newInfo = Intersect(info, info2);
							if(newInfo != null)
							{
								perm.permissions.Add(newInfo);
							}
						}
					}
					return perm;
				}
			}
	public override bool IsSubsetOf(IPermission target)
			{
				if(target == null)
				{
					return (state == PermissionState.None &&
							permissions.Count == 0);
				}
				else if(!(target is SocketPermission))
				{
					throw new ArgumentException(S._("Arg_PermissionMismatch"));
				}
				else if(((SocketPermission)target).IsUnrestricted())
				{
					return true;
				}
				else if(IsUnrestricted())
				{
					return false;
				}
				else
				{
					foreach(EndpointPermission info in permissions)
					{
						if(!((SocketPermission)target).Contains(info))
						{
							return false;
						}
					}
					return true;
				}
			}
	public override IPermission Union(IPermission target)
			{
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is SocketPermission))
				{
					throw new ArgumentException(S._("Arg_PermissionMismatch"));
				}
				else if(IsUnrestricted() ||
				        ((SocketPermission)target).IsUnrestricted())
				{
					return new SocketPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					SocketPermission perm = (SocketPermission)(Copy());
					foreach(EndpointPermission info in
								((SocketPermission)target).permissions)
					{
						perm.permissions.Add(info);
					}
					return perm;
				}
			}

	// Determine if this object has unrestricted permissions.
#if ECMA_COMPAT
	private bool IsUnrestricted()
#else
	public bool IsUnrestricted()
#endif
			{
				return (state == PermissionState.Unrestricted);
			}

#if !ECMA_COMPAT

	// Add permission information to this permissions object.
	public void AddPermission(NetworkAccess access, TransportType transport,
							  String hostName, int portNumber)
			{
				if(state == PermissionState.Unrestricted)
				{
					// No need to add permissions to an unrestricted set.
					return;
				}
				if(hostName == null)
				{
					throw new ArgumentNullException("hostName");
				}
				this.permissions.Add
					(new EndpointPermission(access, transport,
											hostName, portNumber));
			}

	// Iterate over the list of connection permissions.
	public IEnumerator ConnectList
			{
				get
				{
					return new SocketPermissionEnumerator
						(permissions, NetworkAccess.Connect);
				}
			}

	// Iterate over the list of accept permissions.
	public IEnumerator AcceptList
			{
				get
				{
					return new SocketPermissionEnumerator
						(permissions, NetworkAccess.Accept);
				}
			}

	// Enumerator class for the permission list.
	private sealed class SocketPermissionEnumerator : IEnumerator
	{
		// Internal state.
		private IEnumerator e;
		private NetworkAccess access;

		// Constructor.
		public SocketPermissionEnumerator(ArrayList list, NetworkAccess access)
				{
					e = list.GetEnumerator();
					this.access = access;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					while(e.MoveNext())
					{
						if(((EndpointPermission)(e.Current)).access == access)
						{
							return true;
						}
					}
					return false;
				}
		public void Reset()
				{
					e.Reset();
				}
		public Object Current
				{
					get
					{
						return e.Current;
					}
				}

	}; // class SocketPermissionEnumerator

#endif // !ECMA_COMPAT

}; // class SocketPermission

#endif // CONFIG_PERMISSIONS

}; // namespace System.Net
