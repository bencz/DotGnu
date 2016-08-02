/*
 * FirstMatchCodeGroup.cs - Implementation of the
 *		"System.Security.Policy.FirstMatchCodeGroup" class.
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
public sealed class FirstMatchCodeGroup : CodeGroup
{
	// Constructors.
	internal FirstMatchCodeGroup() {}
	public FirstMatchCodeGroup(IMembershipCondition membershipCondition,
					     	   PolicyStatement policy)
			: base(membershipCondition, policy)
			{
				// Nothing to do here.
			}

	// Properties.
	public override String MergeLogic
			{
				get
				{
					return "First Match";
				}
			}

	// Make a copy of this code group.
	public override CodeGroup Copy()
			{
				FirstMatchCodeGroup group;
				group = new FirstMatchCodeGroup
					(MembershipCondition, PolicyStatement);
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

	// Resolve the policy for this code group.
	public override PolicyStatement Resolve(Evidence evidence)
			{
				if(evidence == null)
				{
					throw new ArgumentNullException("evidence");
				}
				if(!MembershipCondition.Check(evidence))
				{
					return null;
				}
				PolicyStatement stmt = null;
				PolicyStatement childStmt;
				foreach(CodeGroup group in Children)
				{
					childStmt = group.Resolve(evidence);
					if(childStmt != null)
					{
						stmt = childStmt;
						break;
					}
				}
				childStmt = PolicyStatement;
				if(childStmt == null)
				{
					return stmt;
				}
				else if(stmt != null)
				{
					if((stmt.Attributes &
							PolicyStatementAttribute.Exclusive) != 0 &&
					   (childStmt.Attributes &
							PolicyStatementAttribute.Exclusive) != 0)
					{
						throw new PolicyException(_("Security_Exclusive"));
					}
					PolicyStatement newStmt = new PolicyStatement(null);
					newStmt.PermissionSetNoCopy =
						stmt.PermissionSetNoCopy.Union
							(childStmt.PermissionSetNoCopy);
					newStmt.Attributes =
						(stmt.Attributes | childStmt.Attributes);
					return newStmt;
				}
				else
				{
					return childStmt;
				}
			}

	// Resolve code groups that match specific evidence.
	public override CodeGroup ResolveMatchingCodeGroups(Evidence evidence)
			{
				if(evidence == null)
				{
					throw new ArgumentNullException("evidence");
				}
				if(!MembershipCondition.Check(evidence))
				{
					return null;
				}
				CodeGroup groups = Copy();
				CodeGroup childGroup;
				foreach(CodeGroup group in Children)
				{
					childGroup = group.ResolveMatchingCodeGroups(evidence);
					if(childGroup != null)
					{
						groups.AddChild(childGroup);
						break;
					}
				}
				return groups;
			}

}; // class FirstMatchCodeGroup

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
