/*
 * PolicyStatement.cs - Implementation of the
 *		"System.Security.Policy.PolicyStatement" class.
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

#if !CONFIG_PERMISSIONS

public sealed class PolicyStatement {}

#else // CONFIG_PERMISSIONS

using System.Security.Permissions;

[Serializable]
public sealed class PolicyStatement
	: ISecurityEncodable, ISecurityPolicyEncodable
{

	// Internal state.
	private PermissionSet permSet;
	private PolicyStatementAttribute attributes;

	// Constructors.
	public PolicyStatement(PermissionSet permSet)
			: this(permSet, PolicyStatementAttribute.Nothing) {}
	public PolicyStatement(PermissionSet permSet,
						   PolicyStatementAttribute attributes)
			{
				this.permSet = permSet;
				this.attributes = attributes;
			}

	// Properties.
	public PolicyStatementAttribute Attributes
			{
				get
				{
					return attributes;
				}
				set
				{
					attributes = value;
				}
			}
	public String AttributeString
			{
				get
				{
					switch(attributes & PolicyStatementAttribute.All)
					{
						case PolicyStatementAttribute.Exclusive:
							return "Exclusive";
						case PolicyStatementAttribute.LevelFinal:
							return "LevelFinal";
						case PolicyStatementAttribute.All:
							return "Exclusive LevelFinal";
					}
					return String.Empty;
				}
			}
	public PermissionSet PermissionSet
			{
				get
				{
					if(permSet == null)
					{
						permSet = new PermissionSet
							(PermissionState.None);
					}
					return permSet.Copy();
				}
				set
				{
					if(value != null)
					{
						permSet = value.Copy();
					}
					else
					{
						permSet = new PermissionSet
							(PermissionState.None);
					}
				}
			}
	internal PermissionSet PermissionSetNoCopy
			{
				get
				{
					if(permSet == null)
					{
						permSet = new PermissionSet
							(PermissionState.None);
					}
					return permSet;
				}
				set
				{
					permSet = value;
				}
			}

	// Make a copy of this object.
	public PolicyStatement Copy()
			{
				return new PolicyStatement(permSet, attributes);
			}

	// Implement the ISecurityEncodable interface.
	public void FromXml(SecurityElement et)
			{
				FromXml(et, null);
			}
	public SecurityElement ToXml()
			{
				return ToXml(null);
			}

	// Implement the ISecurityPolicyEncodable interface.
	public void FromXml(SecurityElement et, PolicyLevel level)
			{
				if(et == null)
				{
					throw new ArgumentNullException("et");
				}
				if(et.Tag != "PolicyStatement")
				{
					throw new ArgumentException
						(_("Security_PolicyName"));
				}
				if(et.Attribute("version") != "1")
				{
					throw new ArgumentException
						(_("Security_PolicyVersion"));
				}
				String value = et.Attribute("Attributes");
				if(value != null)
				{
					attributes = (PolicyStatementAttribute)
						Enum.Parse(typeof(PolicyStatementAttribute), value);
				}
				else
				{
					attributes = PolicyStatementAttribute.Nothing;
				}
				permSet = null;
				if(level != null)
				{
					String name = et.Attribute("PermissionSetName");
					if(name != null)
					{
						permSet = level.GetNamedPermissionSet(value);
						if(permSet == null)
						{
							permSet = new PermissionSet(PermissionState.None);
						}
					}
				}
				if(permSet == null)
				{
					SecurityElement child;
					child = et.SearchForChildByTag("PermissionSet");
					if(child != null)
					{
						String permClass;
						permClass = child.Attribute("class");
						if(permClass != null &&
						   permClass.IndexOf("NamedPermissionSet") != -1)
						{
							permSet = new NamedPermissionSet
								("DefaultName", PermissionState.None);
						}
						else
						{
							permSet = new PermissionSet(PermissionState.None);
						}
						try
						{
							permSet.FromXml(child);
						}
						catch(Exception)
						{
							// Ignore errors during set loading.
						}
					}
				}
				if(permSet == null)
				{
					permSet = new PermissionSet(PermissionState.None);
				}
			}
	public SecurityElement ToXml(PolicyLevel level)
			{
				SecurityElement element;
				element = new SecurityElement("PolicyStatement");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(PolicyStatement).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				if(attributes != PolicyStatementAttribute.Nothing)
				{
					element.AddAttribute("Attributes", attributes.ToString());
				}
				if(permSet != null)
				{
					NamedPermissionSet namedSet;
					namedSet = (permSet as NamedPermissionSet);
					if(namedSet != null)
					{
						if(level != null &&
						   level.GetNamedPermissionSet(namedSet.Name) != null)
						{
							element.AddAttribute
								("PermissionSetName",
								 SecurityElement.Escape(namedSet.Name));
						}
						else
						{
							element.AddChild(permSet.ToXml());
						}
					}
					else
					{
						element.AddChild(permSet.ToXml());
					}
				}
				return element;
			}

}; // class PolicyStatement

#endif // CONFIG_PERMISSIONS

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
