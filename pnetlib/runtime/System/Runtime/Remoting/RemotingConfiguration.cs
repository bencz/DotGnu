/*
 * RemotingConfiguration.cs - Implementation of the
 *			"System.Runtime.Remoting.RemotingConfiguration" class.
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

using System.Collections;
using System.Reflection;
using System.Security.Permissions;

[SecurityPermission(SecurityAction.Demand,
					Flags=SecurityPermissionFlag.RemotingConfiguration)]
public class RemotingConfiguration
{
	// Internal state.
	private static String applicationId;
	private static String applicationName;
	private static String processId;
	private static bool customErrorsEnabled;
	private static bool customErrorsSet;
	private static Hashtable registeredClientTypes;
	private static Hashtable registeredServiceTypes;
	private static Hashtable activatedClientTypes;
	private static Hashtable activatedServiceTypes;

	// This class cannot be instantiated.
	private RemotingConfiguration() {}

	// Get the identifier for the currently running application
	public static String ApplicationId
			{
				get
				{
					lock(typeof(RemotingConfiguration))
					{
						EnsureLoaded();
						return applicationId;
					}
				}
			}

	// Get or set the application name.
	public static String ApplicationName
			{
				get
				{
					lock(typeof(RemotingConfiguration))
					{
						EnsureLoaded();
						return applicationName;
					}
				}
				set
				{
					lock(typeof(RemotingConfiguration))
					{
						applicationName = value;
					}
				}
			}

	// Get the process ID for the current process.
	public static String ProcessId
			{
				get
				{
					lock(typeof(RemotingConfiguration))
					{
						EnsureLoaded();
						return processId;
					}
				}
			}

	// Get a type from its name and assembly.
	internal static Type GetType(String typeName, String assemblyName)
			{
				if(assemblyName != null)
				{
					Assembly assembly = Assembly.Load(assemblyName);
					return assembly.GetType(typeName);
				}
				else
				{
					return Type.GetType(typeName);
				}
			}

	// Ensure that the configuration information has been loaded.
	[TODO]
	private static void EnsureLoaded()
			{
				// TODO
			}

	// Read remoting information from a configuration file.
	[TODO]
	public static void Configure(String filename)
			{
				// TODO
			}

	// Determine if custom errors are enabled.
	public static bool CustomErrorsEnabled(bool isLocalRequest)
			{
				lock(typeof(RemotingConfiguration))
				{
					EnsureLoaded();
					if(customErrorsSet)
					{
						return customErrorsEnabled;
					}
				}
				return !isLocalRequest;
			}

	// Convert a hash table into an array.
	private static Array ToArray(Hashtable table, Type elemType)
			{
				if(table == null)
				{
					return Array.CreateInstance(elemType, 0);
				}
				Array array = Array.CreateInstance(elemType, table.Count);
				IDictionaryEnumerator e = table.GetEnumerator();
				int index = 0;
				while(e.MoveNext())
				{
					array.SetValue(e.Value, index++);
				}
				return array;
			}

	// Get a list of client types that can be activated remotely.
	public static ActivatedClientTypeEntry[]
				GetRegisteredActivatedClientTypes()
			{
				lock(typeof(RemotingConfiguration))
				{
					EnsureLoaded();
					return (ActivatedClientTypeEntry[])
						ToArray(activatedClientTypes,
								typeof(ActivatedClientTypeEntry));
				}
			}

	// Get a list of service types that can be activated by remote clients.
	public static ActivatedServiceTypeEntry[]
				GetRegisteredActivatedServiceTypes()
			{
				lock(typeof(RemotingConfiguration))
				{
					EnsureLoaded();
					return (ActivatedServiceTypeEntry[])
						ToArray(activatedServiceTypes,
								typeof(ActivatedServiceTypeEntry));
				}
			}

	// Get a list of well known client types.
	public static WellKnownClientTypeEntry[]
				GetRegisteredWellKnownClientTypes()
			{
				lock(typeof(RemotingConfiguration))
				{
					EnsureLoaded();
					return (WellKnownClientTypeEntry[])
						ToArray(registeredClientTypes,
								typeof(WellKnownClientTypeEntry));
				}
			}

	// Get a list of well known service types.
	public static WellKnownServiceTypeEntry[]
				GetRegisteredWellKnownServiceTypes()
			{
				lock(typeof(RemotingConfiguration))
				{
					EnsureLoaded();
					return (WellKnownServiceTypeEntry[])
						ToArray(registeredServiceTypes,
								typeof(WellKnownServiceTypeEntry));
				}
			}

	// Determine if activation is allowed on a type.
	public static bool IsActivationAllowed(Type svrType)
			{
				lock(typeof(RemotingConfiguration))
				{
					EnsureLoaded();
					if(activatedServiceTypes != null)
					{
						return activatedServiceTypes.ContainsKey(svrType);
					}
				}
				return false;
			}

	// Determine if a type can be remotely activated.
	public static ActivatedClientTypeEntry
				IsRemotelyActivatedClientType(Type svrType)
			{
				lock(typeof(RemotingConfiguration))
				{
					EnsureLoaded();
					if(activatedClientTypes != null)
					{
						return (activatedClientTypes[svrType]
									as ActivatedClientTypeEntry);
					}
				}
				return null;
			}
	public static ActivatedClientTypeEntry
				IsRemotelyActivatedClientType
					(String typeName, String assemblyName)
			{
				return IsRemotelyActivatedClientType
					(GetType(typeName, assemblyName));
			}

	// Determine if a type is a well known client type.
	public static WellKnownClientTypeEntry
				IsWellKnownClientType(Type svrType)
			{
				lock(typeof(RemotingConfiguration))
				{
					EnsureLoaded();
					if(registeredClientTypes != null)
					{
						return (registeredClientTypes[svrType]
									as WellKnownClientTypeEntry);
					}
				}
				return null;
			}
	public static WellKnownClientTypeEntry
				IsWellKnownClientType
					(String typeName, String assemblyName)
			{
				return IsWellKnownClientType
					(GetType(typeName, assemblyName));
			}

	// Register a client type that can be activated remotely.
	public static void RegisterActivatedClientType
				(ActivatedClientTypeEntry entry)
			{
				if(entry == null)
				{
					throw new ArgumentNullException("entry");
				}
				lock(typeof(RemotingConfiguration))
				{
					EnsureLoaded();
					if(activatedClientTypes == null)
					{
						activatedClientTypes = new Hashtable();
					}
					activatedClientTypes[entry.ObjectType] = entry;
				}
			}
	public static void RegisterActivatedClientType(Type type, String appUrl)
			{
				RegisterActivatedClientType
					(new ActivatedClientTypeEntry(type, appUrl));
			}

	// Register a service type that can be activated by a remote client.
	public static void RegisterActivatedServiceType
				(ActivatedServiceTypeEntry entry)
			{
				if(entry == null)
				{
					throw new ArgumentNullException("entry");
				}
				lock(typeof(RemotingConfiguration))
				{
					EnsureLoaded();
					if(activatedServiceTypes == null)
					{
						activatedServiceTypes = new Hashtable();
					}
					activatedServiceTypes[entry.ObjectType] = entry;
				}
			}
	public static void RegisterActivatedServiceType(Type type)
			{
				RegisterActivatedServiceType
					(new ActivatedServiceTypeEntry(type));
			}

	// Register a well known client type.
	public static void RegisterWellKnownClientType
				(WellKnownClientTypeEntry entry)
			{
				if(entry == null)
				{
					throw new ArgumentNullException("entry");
				}
				lock(typeof(RemotingConfiguration))
				{
					EnsureLoaded();
					if(registeredClientTypes == null)
					{
						registeredClientTypes = new Hashtable();
					}
					registeredClientTypes[entry.ObjectType] = entry;
				}
			}
	public static void RegisterWellKnownClientType
				(Type type, String objectUrl)
			{
				RegisterWellKnownClientType
					(new WellKnownClientTypeEntry(type, objectUrl));
			}

	// Register a well known service type entry.
	public static void RegisterWellKnownServiceType
				(WellKnownServiceTypeEntry entry)
			{
				if(entry == null)
				{
					throw new ArgumentNullException("entry");
				}
				lock(typeof(RemotingConfiguration))
				{
					EnsureLoaded();
					if(registeredServiceTypes == null)
					{
						registeredServiceTypes = new Hashtable();
					}
					registeredServiceTypes[entry.ObjectType] = entry;
				}
			}
	public static void RegisterWellKnownServiceType
				(Type type, String objectUri, WellKnownObjectMode mode)
			{
				RegisterWellKnownServiceType
					(new WellKnownServiceTypeEntry(type, objectUri, mode));
			}

}; // class RemotingConfiguration

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting
