/*
 * Evidence.cs - Implementation of the "System.Security.Policy.Evidence" class.
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

namespace System.Security.Policy
{

#if CONFIG_PERMISSIONS && CONFIG_POLICY_OBJECTS

using System.Collections;
using System.Security.Permissions;

[Serializable]
public sealed class Evidence : ICollection, IEnumerable
{
	// Internal state.
	private Object[] hostEvidence;
	private Object[] assemblyEvidence;
	private bool locked;

	// Constructors.
	public Evidence()
			{
				this.hostEvidence = null;
				this.assemblyEvidence = null;
			}
	public Evidence(Evidence evidence)
			{
				if(evidence == null)
				{
					throw new ArgumentNullException("evidence");
				}
				this.hostEvidence = evidence.hostEvidence;
				this.assemblyEvidence = evidence.assemblyEvidence;
			}
	public Evidence(Object[] hostEvidence, Object[] assemblyEvidence)
			{
				this.hostEvidence = hostEvidence;
				this.assemblyEvidence = assemblyEvidence;
			}

	// Implement the ICollection interface.
	public void CopyTo(Array array, int index)
			{
				if(hostEvidence != null)
				{
					foreach(Object o1 in hostEvidence)
					{
						array.SetValue(o1, index++);
					}
				}
				if(assemblyEvidence != null)
				{
					foreach(Object o2 in assemblyEvidence)
					{
						array.SetValue(o2, index++);
					}
				}
			}
	public int Count
			{
				get
				{
					return ((hostEvidence != null) ? hostEvidence.Length : 0) +
					  	   ((assemblyEvidence != null)
						   		? assemblyEvidence.Length : 0);
				}
			}
	public bool IsSynchronized
			{
				get
				{
					return false;
				}
			}
	public Object SyncRoot
			{
				get
				{
					return this;
				}
			}

	// Determine if this evidence set is read-only.
	public bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

	// Get or set the lock flag on this evidence list.
	public bool Locked
			{
				get
				{
					return locked;
				}
				set
				{
					(new SecurityPermission
						(SecurityPermissionFlag.ControlEvidence)).Demand();
					locked = value;
				}
			}

	// Add to the assembly evidence list.
	public void AddAssembly(Object id)
			{
				if(locked)
				{
					(new SecurityPermission
						(SecurityPermissionFlag.ControlEvidence)).Demand();
				}
				if(id != null)
				{
					if(assemblyEvidence == null)
					{
						assemblyEvidence = new Object [] {id};
					}
					else
					{
						Object[] newList =
							new Object [assemblyEvidence.Length + 1];
						Array.Copy(assemblyEvidence, 0, newList, 0,
								   assemblyEvidence.Length);
						newList[assemblyEvidence.Length] = id;
						assemblyEvidence = newList;
					}
				}
			}

	// Add to the host evidence list.
	public void AddHost(Object id)
			{
				if(locked)
				{
					(new SecurityPermission
						(SecurityPermissionFlag.ControlEvidence)).Demand();
				}
				if(id != null)
				{
					if(hostEvidence == null)
					{
						hostEvidence = new Object [] {id};
					}
					else
					{
						Object[] newList =
							new Object [hostEvidence.Length + 1];
						Array.Copy(hostEvidence, 0, newList, 0,
								   hostEvidence.Length);
						newList[hostEvidence.Length] = id;
						hostEvidence = newList;
					}
				}
			}

	// Implement the IEnumerable interface.
	public IEnumerator GetEnumerator()
			{
				return new EvidenceEnumerator(this, true, true);
			}

	// Enumerate the assembly evidence objects.
	public IEnumerator GetAssemblyEnumerator()
			{
				return new EvidenceEnumerator(this, true, false);
			}

	// Enumerate the host evidence objects.
	public IEnumerator GetHostEnumerator()
			{
				return new EvidenceEnumerator(this, false, true);
			}

	// Merge two object arrays.
	private static Object[] Merge(Object[] list1, Object[] list2)
			{
				if(list1 == null)
				{
					return list2;
				}
				else if(list2 == null)
				{
					return list1;
				}
				Object[] newList = new Object [list1.Length + list2.Length];
				Array.Copy(list1, 0, newList, 0, list1.Length);
				Array.Copy(list2, 0, newList, list1.Length, list2.Length);
				return newList;
			}

	// Merge another evidence set into this one.
	public void Merge(Evidence evidence)
			{
				if(evidence == null)
				{
					throw new ArgumentNullException("evidence");
				}
				if(locked)
				{
					(new SecurityPermission
						(SecurityPermissionFlag.ControlEvidence)).Demand();
				}
				hostEvidence = Merge(hostEvidence, evidence.hostEvidence);
				assemblyEvidence =
					Merge(assemblyEvidence, evidence.assemblyEvidence);
			}

	// Get the number of hosts and assemblies.
	private int HostCount
			{
				get
				{
					if(hostEvidence != null)
					{
						return hostEvidence.Length;
					}
					else
					{
						return 0;
					}
				}
			}
	private int AssemblyCount
			{
				get
				{
					if(assemblyEvidence != null)
					{
						return assemblyEvidence.Length;
					}
					else
					{
						return 0;
					}
				}
			}

	// Evidence enumerator class.
	private sealed class EvidenceEnumerator : IEnumerator
	{
		// Internal state.
		private Evidence evidence;
		private bool enumHosts;
		private bool enumAssemblies;
		private int index;

		// Constructor.
		public EvidenceEnumerator(Evidence evidence, bool enumHosts,
								  bool enumAssemblies)
				{
					this.evidence = evidence;
					this.enumHosts = enumHosts;
					this.enumAssemblies = enumAssemblies;
					this.index = -1;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					++index;
					if(enumHosts && enumAssemblies)
					{
						return (index < evidence.Count);
					}
					else if(enumHosts)
					{
						return (index < evidence.HostCount);
					}
					else
					{
						return (index < evidence.AssemblyCount);
					}
				}
		public void Reset()
				{
					index = -1;
				}
		public Object Current
				{
					get
					{
						if(enumHosts && enumAssemblies)
						{
							if(index < 0)
							{
								throw new InvalidOperationException
									(_("Invalid_BadEnumeratorPosition"));
							}
							else if(index < evidence.HostCount)
							{
								return evidence.hostEvidence[index];
							}
							else if(index < evidence.Count)
							{
								return evidence.assemblyEvidence
									[index - evidence.HostCount];
							}
							else
							{
								throw new InvalidOperationException
									(_("Invalid_BadEnumeratorPosition"));
							}
						}
						else if(enumHosts)
						{
							if(index < 0 || index >= evidence.HostCount)
							{
								throw new InvalidOperationException
									(_("Invalid_BadEnumeratorPosition"));
							}
							else
							{
								return evidence.hostEvidence[index];
							}
						}
						else
						{
							if(index < 0 || index >= evidence.AssemblyCount)
							{
								throw new InvalidOperationException
									(_("Invalid_BadEnumeratorPosition"));
							}
							else
							{
								return evidence.assemblyEvidence[index];
							}
						}
					}
				}

	}; // class EvidenceEnumerator

}; // class Evidence

#else // !(CONFIG_PERMISSIONS && CONFIG_POLICY_OBJECTS)

// Define a dummy Evidence class if we aren't using policy objects.

public sealed class Evidence
{
	public Evidence() {}
	public Evidence(Evidence e) {}

}; // class Evidence

#endif // !(CONFIG_PERMISSIONS && CONFIG_POLICY_OBJECTS)

}; // namespace System.Security.Policy
