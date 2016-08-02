/*
 * NetCodeGroup.cs - Implementation of the
 *		"System.Security.Policy.NetCodeGroup" class.
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
using System.Reflection;
using System.Security.Permissions;

[Serializable]
public sealed class NetCodeGroup : CodeGroup
{
	// Constructors.
	internal NetCodeGroup() {}
	public NetCodeGroup(IMembershipCondition membershipCondition)
			: base(membershipCondition, null)
			{
				// Nothing to do here.
			}

	// Properties.
	public override String AttributeString
			{
				get
				{
					return null;
				}
			}
	public override String MergeLogic
			{
				get
				{
					return "Union";
				}
			}
	public override String PermissionSetName
			{
				get
				{
					return _("Format_NetCodeGroupName");
				}
			}

	// Make a copy of this code group.
	public override CodeGroup Copy()
			{
				NetCodeGroup group;
				group = new NetCodeGroup(MembershipCondition);
				group.Name = Name;
				group.Description = Description;
				IList children = Children;
				if(children != null)
				{
					foreach(CodeGroup child in children)
					{
						group.AddChild(child);
					}
				}
				return group;
			}

	// Make a policy from host and scheme information.
	private static PolicyStatement MakePolicy(String scheme, String host)
			{
			#if CONFIG_REFLECTION
				// Create the uri corresponding to the parameters.
				if(host != null)
				{
					host = host.Replace(".", "\\.");
				}
				else
				{
					host = ".*";
				}
				String uri;
				if(scheme != null && String.Compare(scheme, "http", true) == 0)
				{
					uri = "(http|https)://" + host + "/.*";
				}
				else if(scheme != null)
				{
					uri = scheme + "://" + host + "/.*";
				}
				else
				{
					uri = ".*://" + host + "/.*";
				}

				// We need to create an instance of "System.Net.WebPermission",
				// but that class does not exist in this assembly.  So, we
				// have to create it in a somewhat round-about fashion.
				Assembly system = Assembly.Load("System");
				Type webPermType = system.GetType
					("System.Net.WebPermission", true, false);
				Object webPerm = Activator.CreateInstance(webPermType);
				Type networkAccessType = system.GetType
					("System.Net.NetworkAccess", true, false);
				Object networkAccess = Enum.ToObject
					(networkAccessType, 0x0040 /* Connect */);
				Type regexType = system.GetType
					("System.Text.RegularExpressions.Regex", true, false);
				Object regex = Activator.CreateInstance
					(regexType, new Object[] {uri});
				webPermType.InvokeMember("AddPermission",
										 BindingFlags.InvokeMethod |
										 BindingFlags.Public |
										 BindingFlags.Instance, null,
										 webPerm,
										 new Object[] {networkAccess, regex});

				// Create a permission set holding the web permission.
				PermissionSet permSet = new PermissionSet
					(PermissionState.None);
				permSet.AddPermission(webPerm as IPermission);

				// Return the final policy statement, from the permission set.
				return new PolicyStatement(permSet);
			#else
				return null;
			#endif
			}

	// Resolve the policy for this code group.
	public override PolicyStatement Resolve(Evidence evidence)
			{
				PolicyStatement stmt;
				PolicyStatement childStmt;
				IEnumerator e;
				Site site;
				UrlParser url;

				// Validate the parameter.
				if(evidence == null)
				{
					throw new ArgumentNullException("evidence");
				}

				// Check the membership condition.
				if(!MembershipCondition.Check(evidence))
				{
					return null;
				}

				// Scan the host evidence for a policy and site.
				stmt = null;
				site = null;
				e = evidence.GetHostEnumerator();
				while(e.MoveNext())
				{
					if(e.Current is Url)
					{
						url = ((Url)(e.Current)).parser;
						stmt = MakePolicy(url.Scheme, url.Host);
					}
					else if(e.Current is Site && site == null)
					{
						site = (Site)(e.Current);
					}
				}

				// Create a default policy statement if necessary.
				if(stmt == null && site != null)
				{
					stmt = MakePolicy(null, site.Name);
				}
				else if(stmt == null)
				{
					stmt = new PolicyStatement
						(new PermissionSet(PermissionState.None),
						 PolicyStatementAttribute.Nothing);
				}

				// Modify the policy statement from this code group.
				foreach(CodeGroup group in Children)
				{
					childStmt = group.Resolve(evidence);
					if(childStmt != null)
					{
						if((stmt.Attributes &
								PolicyStatementAttribute.Exclusive) != 0 &&
						   (childStmt.Attributes &
								PolicyStatementAttribute.Exclusive) != 0)
						{
							throw new PolicyException(_("Security_Exclusive"));
						}
					}
					stmt.PermissionSetNoCopy =
						stmt.PermissionSetNoCopy.Union
							(childStmt.PermissionSetNoCopy);
					stmt.Attributes |= childStmt.Attributes;
				}
				return stmt;
			}

	// Resolve code groups that match specific evidence.
	public override CodeGroup ResolveMatchingCodeGroups(Evidence evidence)
			{
				NetCodeGroup newGroup;
				CodeGroup child;

				// Validate the parameter.
				if(evidence == null)
				{
					throw new ArgumentNullException("evidence");
				}

				// Check the membership condition.
				if(!MembershipCondition.Check(evidence))
				{
					return null;
				}

				// Clone this group, except for the children.
				newGroup = new NetCodeGroup(MembershipCondition);
				newGroup.Name = Name;
				newGroup.Description = Description;

				// Resolve and add the children.
				foreach(CodeGroup group in Children)
				{
					child = group.ResolveMatchingCodeGroups(evidence);
					if(child != null)
					{
						newGroup.AddChild(child);
					}
				}

				// Return the result.
				return newGroup;
			}

}; // class NetCodeGroup

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
