/*
 * PermissionRequestEvidence.cs - Implementation of the
 *			"System.Security.Policy.PermissionRequestEvidence" class.
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

[Serializable]
public sealed class PermissionRequestEvidence
{
	// Internal state.
	private PermissionSet request;
	private PermissionSet optional;
	private PermissionSet denied;

	// Constructor
	public PermissionRequestEvidence(PermissionSet request,
									 PermissionSet optional,
									 PermissionSet denied)
			{
				this.request = request;
				this.optional = optional;
				this.denied = denied;
			}

	// Get this object's properties.
	public PermissionSet DeniedPermissions
			{
				get
				{
					return denied;
				}
			}
	public PermissionSet OptionalPermissions
			{
				get
				{
					return optional;
				}
			}
	public PermissionSet RequestedPermissions
			{
				get
				{
					return request;
				}
			}

	// Create a copy of this object.
	public PermissionRequestEvidence Copy()
			{
				return new PermissionRequestEvidence
					(request, optional, denied);
			}

	// Convert this object into a string.
	public override String ToString()
			{
				SecurityElement element = new SecurityElement
					("System.Security.Policy.PermissionRequestEvidence");
				SecurityElement child;
				element.AddAttribute("version", "1");
				if(request != null)
				{
					child = new SecurityElement("Request");
					child.AddChild(request.ToXml());
					element.AddChild(child);
				}
				if(optional != null)
				{
					child = new SecurityElement("Optional");
					child.AddChild(optional.ToXml());
					element.AddChild(child);
				}
				if(denied != null)
				{
					child = new SecurityElement("Denied");
					child.AddChild(denied.ToXml());
					element.AddChild(child);
				}
				return element.ToString();
			}

}; // class PermissionRequestEvidence

#endif // CONFIG_PERMISSIONS && CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
