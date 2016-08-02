/*
 * StrongName.cs - Implementation of the
 *		"System.Security.Policy.StrongName" class.
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

using System.Security.Permissions;

[Serializable]
public sealed class StrongName
#if CONFIG_PERMISSIONS
	: IIdentityPermissionFactory
#endif
{
	// Internal state.
	private StrongNamePublicKeyBlob blob;
	private String name;
	private Version version;

	// Constructor.
	public StrongName(StrongNamePublicKeyBlob blob,
					  String name, Version version)
			{
				if(blob == null)
				{
					throw new ArgumentNullException("blob");
				}
				if(name == null)
				{
					throw new ArgumentNullException("name");
				}
				if(version == null)
				{
					throw new ArgumentNullException("version");
				}
				this.blob = blob;
				this.name = name;
				this.version = version;
			}

	// Get this object's properties.
	public String Name
			{
				get
				{
					return name;
				}
			}
	public StrongNamePublicKeyBlob PublicKey
			{
				get
				{
					return blob;
				}
			}
	public Version Version
			{
				get
				{
					return version;
				}
			}

	// Make a copy of this object.
	public Object Copy()
			{
				return new StrongName(blob, name, version);
			}

#if CONFIG_PERMISSIONS

	// Implement the IIdentityPermissionFactory interface
	public IPermission CreateIdentityPermission(Evidence evidence)
			{
				return new StrongNameIdentityPermission(blob, name, version);
			}

#endif

	// Determine if two objects are equal.
	public override bool Equals(Object obj)
			{
				StrongName other = (obj as StrongName);
				if(other != null)
				{
					return (other.blob.Equals(blob) &&
					        other.name == name &&
							other.version.Equals(version));
				}
				else
				{
					return false;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return blob.GetHashCode();
			}

	// Convert this object into a string.
	public override String ToString()
			{
				SecurityElement element = new SecurityElement("StrongName");
				element.AddAttribute("version", "1");
				element.AddAttribute("Key", blob.ToString());
				element.AddAttribute
					("Name", SecurityElement.Escape(name));
				element.AddAttribute("Version", version.ToString());
				return element.ToString();
			}

}; // class StrongName

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
