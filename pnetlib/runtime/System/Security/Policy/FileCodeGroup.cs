/*
 * FileCodeGroup.cs - Implementation of the
 *		"System.Security.Policy.FileCodeGroup" class.
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
using System.Security.Permissions;

[Serializable]
public sealed class FileCodeGroup : CodeGroup
{
	// Internal state.
	private FileIOPermissionAccess access;

	// Constructors.
	internal FileCodeGroup() {}
	public FileCodeGroup(IMembershipCondition membershipCondition,
					     FileIOPermissionAccess access)
			: base(membershipCondition, null)
			{
				this.access = access;
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
					return String.Format(_("Format_FileIOPermSetName"),
										 access.ToString());
				}
			}

	// Make a copy of this code group.
	public override CodeGroup Copy()
			{
				FileCodeGroup group;
				group = new FileCodeGroup(MembershipCondition, access);
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

	// Create the XML form of this code group.
	protected override void CreateXml
				(SecurityElement element, PolicyLevel level)
			{
				element.AddAttribute("Access", access.ToString());
			}

	// Compare two code groups for equality.
	public override bool Equals(Object obj)
			{
				FileCodeGroup cg = (obj as FileCodeGroup);
				if(cg != null)
				{
					if(!base.Equals(cg))
					{
						return false;
					}
					return (cg.access == access);
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this instance.
	public override int GetHashCode()
			{
				return base.GetHashCode();
			}

	// Make a policy from url information.
	private PolicyStatement MakePolicy(UrlParser url)
			{
				if(String.Compare(url.Scheme, "file", true) != 0)
				{
					return null;
				}
				PermissionSet permSet = new PermissionSet
					(PermissionState.None);
				permSet.AddPermission(new FileIOPermission(access, url.Rest));
				return new PolicyStatement
					(permSet, PolicyStatementAttribute.Nothing);
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
						stmt = MakePolicy(url);
					}
					else if(e.Current is Site && site == null)
					{
						site = (Site)(e.Current);
					}
				}

				// Create a default policy statement if necessary.
				if(stmt == null)
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
				FileCodeGroup newGroup;
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
				newGroup = new FileCodeGroup(MembershipCondition, access);
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

	// Parse the XML form of this code group.
	protected override void ParseXml
				(SecurityElement element, PolicyLevel level)
			{
				String value = element.Attribute("Access");
				if(value != null)
				{
					access = (FileIOPermissionAccess)
						Enum.Parse(typeof(FileIOPermissionAccess), value);
				}
				else
				{
					access = FileIOPermissionAccess.NoAccess;
				}
			}

}; // class FileCodeGroup

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
