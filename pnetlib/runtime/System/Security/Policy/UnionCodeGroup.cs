/*
 * UnionCodeGroup.cs - Implementation of the
 *		"System.Security.Policy.UnionCodeGroup" class.
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
public sealed class UnionCodeGroup : CodeGroup
{
	// Constructors.
	internal UnionCodeGroup() {}
	public UnionCodeGroup(IMembershipCondition membershipCondition,
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
					return "Union";
				}
			}

	// Make a copy of this code group.
	public override CodeGroup Copy()
			{
				UnionCodeGroup group;
				group = new UnionCodeGroup
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
				PolicyStatement stmt = PolicyStatement;
				PolicyStatement childStmt;
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
						stmt.PermissionSetNoCopy =
							stmt.PermissionSetNoCopy.Union
								(childStmt.PermissionSetNoCopy);
						stmt.Attributes |= childStmt.Attributes;
					}
				}
				return stmt;
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
					}
				}
				return groups;
			}

}; // class UnionCodeGroup

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
