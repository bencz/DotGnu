/*
 * PolicyLevel.cs - Implementation of the
 *		"System.Security.Policy.PolicyLevel" class.
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

#if CONFIG_POLICY_OBJECTS

using System.Collections;

[Serializable]
public sealed class PolicyLevel
{
	// Internal state.
	private ArrayList fullTrustAssemblies;
	private String label;
	private ArrayList namedPermissionSets;
	private CodeGroup rootCodeGroup;
	private String storeLocation;

	// Constructor.
	internal PolicyLevel(String label)
			{
				this.label = label;
				fullTrustAssemblies = new ArrayList();
				namedPermissionSets = new ArrayList();
				rootCodeGroup = DefaultRootCodeGroup();
			}

	// Properties.
	public IList FullTrustAssemblies
			{
				get
				{
					return new ArrayList(fullTrustAssemblies);
				}
			}
	public String Label
			{
				get
				{
					return label;
				}
			}
	public IList NamedPermissionSets
			{
				get
				{
					ArrayList list = new ArrayList();
					foreach(NamedPermissionSet pSet in namedPermissionSets)
					{
						list.Add(pSet.Copy());
					}
					return list;
				}
			}
	public CodeGroup RootCodeGroup
			{
				get
				{
					return rootCodeGroup;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					rootCodeGroup = value.Copy();
				}
			}
	public String StoreLocation
			{
				get
				{
					return storeLocation;
				}
			}

	// Add an entry to the "full trust assembly" list.
	public void AddFullTrustAssembly(StrongName sn)
			{
				if(sn == null)
				{
					throw new ArgumentNullException("sn");
				}
				AddFullTrustAssembly
					(new StrongNameMembershipCondition
						(sn.PublicKey, sn.Name, sn.Version));
			}
	public void AddFullTrustAssembly(StrongNameMembershipCondition snMC)
			{
				if(snMC == null)
				{
					throw new ArgumentNullException("snMC");
				}
				if(fullTrustAssemblies.Contains(snMC))
				{
					throw new ArgumentException
						(_("Security_FullTrustPresent"));
				}
				fullTrustAssemblies.Add(snMC);
			}

#if CONFIG_PERMISSIONS

	// Add an entry to the "named permission sets" list.
	public void AddNamedPermissionSet(NamedPermissionSet permSet)
			{
				if(permSet == null)
				{
					throw new ArgumentNullException("permSet");
				}
				namedPermissionSets.Add(permSet);
			}

	// Change a named permission set.
	public NamedPermissionSet ChangeNamedPermissionSet
				(String name, PermissionSet pSet)
			{
				// Validate the parameters.
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(pSet == null)
				{
					throw new ArgumentNullException("pSet");
				}

				// Find the existing permission set with this name.
				NamedPermissionSet current = GetNamedPermissionSet(name);
				if(current == null)
				{
					throw new ArgumentException
						(_("Security_PermissionSetNotFound"));
				}

				// Make a copy of the previous permission set.
				NamedPermissionSet prev =
					(NamedPermissionSet)(current.Copy());

				// Clear the permission set and recreate it from "pSet".
				current.CopyFrom(pSet);

				// Return the previsou permission set.
				return prev;
			}

	// Get a specific named permission set.
	public NamedPermissionSet GetNamedPermissionSet(String name)
			{
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				foreach(NamedPermissionSet set in namedPermissionSets)
				{
					if(set.Name == name)
					{
						return set;
					}
				}
				return null;
			}

	// Remove a named permission set.
	public NamedPermissionSet RemoveNamedPermissionSet
					(NamedPermissionSet permSet)
			{
				if(permSet == null)
				{
					throw new ArgumentNullException("permSet");
				}
				return RemoveNamedPermissionSet(permSet.Name);
			}
	public NamedPermissionSet RemoveNamedPermissionSet(String name)
			{
				// Validate the parameter.
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}

				// Find the existing permission set with this name.
				NamedPermissionSet current = GetNamedPermissionSet(name);
				if(current == null)
				{
					throw new ArgumentException
						(_("Security_PermissionSetNotFound"));
				}

				// Remove the permission set from the list.
				namedPermissionSets.Remove(current);

				// Return the permission set that was removed.
				return current;
			}

#endif // CONFIG_PERMISSIONS

	// Create a policy level object for the current application domain.
	public static PolicyLevel CreateAppDomainLevel()
			{
				return new PolicyLevel("AppDomain");
			}

	// Load policy information from an XML element.
	[TODO]
	public void FromXml(SecurityElement e)
			{
				// TODO
			}

	// Recover the last backed-up policy configuration.
	public void Recover()
			{
				// Nothing to do here: we don't support backups.
			}

	// Remove an entry from the "full trust assembly" list.
	public void RemoveFullTrustAssembly(StrongName sn)
			{
				if(sn == null)
				{
					throw new ArgumentNullException("sn");
				}
				RemoveFullTrustAssembly
					(new StrongNameMembershipCondition
						(sn.PublicKey, sn.Name, sn.Version));
			}
	public void RemoveFullTrustAssembly(StrongNameMembershipCondition snMC)
			{
				if(snMC == null)
				{
					throw new ArgumentNullException("snMC");
				}
				if(fullTrustAssemblies.Contains(snMC))
				{
					fullTrustAssemblies.Remove(snMC);
				}
				else
				{
					throw new ArgumentException
						(_("Security_FullTrustNotPresent"));
				}
			}

	// Create the default root code group.
	private CodeGroup DefaultRootCodeGroup()
			{
				UnionCodeGroup group = new UnionCodeGroup
					(new AllMembershipCondition(), null);
				group.Name = "All_Code";
				group.Description = _("Security_RootGroupDescription");
				return group;
			}

	// Reset to the default state.
	public void Reset()
			{
				fullTrustAssemblies.Clear();
				namedPermissionSets.Clear();
				rootCodeGroup = DefaultRootCodeGroup();
			}

	// Resolve policy information based on supplied evidence.
	[TODO]
	public PolicyStatement Resolve(Evidence evidence)
			{
				if(evidence == null)
				{
					throw new ArgumentNullException("evidence");
				}
				// TODO
				return null;
			}
	public CodeGroup ResolveMatchingCodeGroups(Evidence evidence)
			{
				if(evidence == null)
				{
					throw new ArgumentNullException("evidence");
				}
				return RootCodeGroup.ResolveMatchingCodeGroups(evidence);
			}

	// Convert this object into an XML element.
	[TODO]
	public SecurityElement ToXml()
			{
				// TODO
				return null;
			}

}; // class PolicyLevel

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
