/*
 * CodeGroup.cs - Implementation of the
 *		"System.Security.Policy.CodeGroup" class.
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
public abstract class CodeGroup
{
	// Internal state.
	private IMembershipCondition membershipCondition;
	private PolicyStatement policy;
	private IList children;
	private String description;
	private String name;

	// Constructors.
	public CodeGroup(IMembershipCondition membershipCondition,
					 PolicyStatement policy)
			{
				if(membershipCondition == null)
				{
					throw new ArgumentNullException("membershipCondition");
				}
				this.membershipCondition = membershipCondition;
				this.policy = policy;
				this.children = new ArrayList();
			}
	internal CodeGroup()
			{
				this.children = new ArrayList();
			}

	// Properties.
	public virtual String AttributeString
			{
				get
				{
					if(policy != null)
					{
						return policy.AttributeString;
					}
					else
					{
						return null;
					}
				}
			}
	public IList Children
			{
				get
				{
					return children;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					children = value;
				}
			}
	public String Description
			{
				get
				{
					return description;
				}
				set
				{
					description = value;
				}
			}
	public IMembershipCondition MembershipCondition
			{
				get
				{
					return membershipCondition;
				}
				set
				{
					if(value == null)
					{
						throw new ArgumentNullException("value");
					}
					membershipCondition = value;
				}
			}
	public abstract String MergeLogic { get; }
	public String Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}
	public virtual String PermissionSetName
			{
				get
				{
					if(policy != null)
					{
						NamedPermissionSet permSet;
						permSet = (policy.PermissionSetNoCopy
										as NamedPermissionSet);
						if(permSet != null)
						{
							return permSet.Name;
						}
					}
					return null;
				}
			}
	public PolicyStatement PolicyStatement
			{
				get
				{
					if(policy != null)
					{
						return policy.Copy();
					}
					else
					{
						return null;
					}
				}
				set
				{
					if(value != null)
					{
						policy = value.Copy();
					}
					else
					{
						policy = null;
					}
				}
			}

	// Add a child to this code group.
	public void AddChild(CodeGroup group)
			{
				if(group == null)
				{
					throw new ArgumentNullException("group");
				}
				Children.Add(group);
			}

	// Make a copy of this code group.
	public abstract CodeGroup Copy();

	// Create the XML form of this code group.
	protected virtual void CreateXml
				(SecurityElement element, PolicyLevel level)
			{
				// Nothing to do in the base class.
			}

	// Compare two code groups for equality.
	public override bool Equals(Object obj)
			{
				CodeGroup cg = (obj as CodeGroup);
				if(cg != null)
				{
					return Equals(cg, false);
				}
				else
				{
					return false;
				}
			}
	public bool Equals(CodeGroup cg, bool compareChildren)
			{
				if(cg == null)
				{
					return false;
				}
				if(Name != cg.Name || Description != cg.Description ||
				   !MembershipCondition.Equals(cg.MembershipCondition))
				{
					return false;
				}
				if(compareChildren)
				{
					IList list1 = Children;
					IList list2 = cg.Children;
					if(list1.Count != list2.Count)
					{
						return false;
					}
					int posn;
					for(posn = 0; posn < list1.Count; ++posn)
					{
						if(!((CodeGroup)(list1[posn])).Equals
								(((CodeGroup)(list2[posn])), true))
						{
							return false;
						}
					}
				}
				return true;
			}

	// Convert an XML security element into a code group.
	public void FromXml(SecurityElement et)
			{
				FromXml(et, null);
			}
	public void FromXml(SecurityElement et, PolicyLevel level)
			{
				SecurityElement child;
				String className;
				Type type;
				ArrayList list;
				CodeGroup group;

				if(et == null)
				{
					throw new ArgumentNullException("et");
				}
				if(et.Tag != "CodeGroup")
				{
					throw new ArgumentException
						(_("Security_CodeGroupName"));
				}
				if(et.Attribute("version") != "1")
				{
					throw new ArgumentException
						(_("Security_PolicyVersion"));
				}
				name = et.Attribute("Name");
				description = et.Attribute("Description");

				// Load the membership condition information for the group.
				child = et.SearchForChildByTag("IMembershipCondition");
				if(child != null)
				{
					className = child.Attribute("class");
					if(className == null)
					{
						throw new ArgumentException
							(_("Invalid_PermissionXml"));
					}
					type = Type.GetType(className);
					if(type == null && className.IndexOf('.') == -1)
					{
						// May not have been fully-qualified.
						type = Type.GetType
							("System.Security.Policy." + className);
					}
					if(!typeof(IMembershipCondition).IsAssignableFrom(type))
					{
						throw new ArgumentException
							(_("Invalid_PermissionXml"));
					}
					membershipCondition =
						(Activator.CreateInstance(type)
								as IMembershipCondition);
					if(membershipCondition != null)
					{
						membershipCondition.FromXml(child, level);
					}
				}
				else
				{
					throw new ArgumentException
						(_("Arg_InvalidMembershipCondition"));
				}

				// Load the children within this code group.
				list = new ArrayList();
				foreach(SecurityElement elem in et.Children)
				{
					if(elem.Tag != "CodeGroup")
					{
						continue;
					}
					className = child.Attribute("class");
					if(className == null)
					{
						throw new ArgumentException
							(_("Invalid_PermissionXml"));
					}
					type = Type.GetType(className);
					if(type == null && className.IndexOf('.') == -1)
					{
						// May not have been fully-qualified.
						type = Type.GetType
							("System.Security.Policy." + className);
					}
					if(!typeof(CodeGroup).IsAssignableFrom(type))
					{
						throw new ArgumentException
							(_("Invalid_PermissionXml"));
					}
					group = (Activator.CreateInstance(type) as CodeGroup);
					if(group != null)
					{
						group.FromXml(elem, level);
						list.Add(group);
					}
				}
				children = list;

				// Parse subclass-specific data from the element.
				ParseXml(et, level);
			}

	// Get the hash code for this instance.
	public override int GetHashCode()
			{
				return membershipCondition.GetHashCode();
			}

	// Remove a child from this code group.
	public void RemoveChild(CodeGroup group)
			{
				if(group == null)
				{
					throw new ArgumentNullException("group");
				}
				if(!(Children.Contains(group)))
				{
					throw new ArgumentException
						(_("Security_NotCodeGroupChild"));
				}
			}

	// Resolve the policy for this code group.
	public abstract PolicyStatement Resolve(Evidence evidence);

	// Resolve code groups that match specific evidence.
	public abstract CodeGroup ResolveMatchingCodeGroups(Evidence evidence);

	// Convert a code group into an XML security element
	public SecurityElement ToXml()
			{
				return ToXml(null);
			}
	public SecurityElement ToXml(PolicyLevel level)
			{
				SecurityElement element;
				element = new SecurityElement("CodeGroup");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(GetType().AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				element.AddChild(membershipCondition.ToXml(level));
				if(policy != null)
				{
					PermissionSet permSet = policy.PermissionSetNoCopy;
					if(permSet is NamedPermissionSet && level != null &&
					   level.GetNamedPermissionSet
					   		(((NamedPermissionSet)permSet).Name) != null)
					{
						element.AddAttribute
							("PermissionSetName",
							 ((NamedPermissionSet)permSet).Name);
					}
					else if(!permSet.IsEmpty())
					{
						element.AddChild(permSet.ToXml());
					}
					if(policy.Attributes != PolicyStatementAttribute.Nothing)
					{
						element.AddAttribute
							("Attributes", policy.Attributes.ToString());
					}
					foreach(CodeGroup group in Children)
					{
						element.AddChild(group.ToXml(level));
					}
				}
				if(name != null)
				{
					element.AddAttribute("Name", SecurityElement.Escape(name));
				}
				if(description != null)
				{
					element.AddAttribute
						("Description", SecurityElement.Escape(description));
				}
				CreateXml(element, level);
				return element;
			}

	// Parse the XML form of this code group.
	protected virtual void ParseXml
				(SecurityElement element, PolicyLevel level)
			{
				// Nothing to do in the base class.
			}

}; // class CodeGroup

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
