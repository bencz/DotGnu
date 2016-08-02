/*
 * PermissionSet.cs - Implementation of the
 *		"System.Security.PermissionSet" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Security
{

#if CONFIG_PERMISSIONS

using System;
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class PermissionSet : ICollection, IEnumerable, ISecurityEncodable,
							 IStackWalk, IDeserializationCallback
{

	// Internal state.
	private PermissionState state;
	private ArrayList permissions;

	// Constructors.
	public PermissionSet(PermissionState state)
			{
				if(state != PermissionState.Unrestricted &&
				   state != PermissionState.None)
				{
					throw new ArgumentException(_("Arg_PermissionState"));
				}
				this.state = state;
				permissions = new ArrayList();
			}
	public PermissionSet(PermissionSet permSet)
			{
				if(permSet != null)
				{
					state = permSet.state;
					permissions = new ArrayList(permSet.permissions.Count);
					int posn;
					for(posn = 0; posn < permSet.permissions.Count; ++posn)
					{
						permissions[posn] =
							((IPermission)(permSet.permissions[posn])).Copy();
					}
				}
				else
				{
					state = PermissionState.Unrestricted;
					permissions = new ArrayList();
				}
			}

	// Find the index of a permission by type.
	private int FindPermission(Type type)
			{
				int posn;
				for(posn = 0; posn < permissions.Count; ++posn)
				{
					if(permissions[posn].GetType() == type)
					{
						return posn;
					}
				}
				return -1;
			}

	// Add a permission value to this set.
	public virtual IPermission AddPermission(IPermission perm)
			{
				if(perm == null)
				{
					throw new ArgumentNullException("perm");
				}
				int index = FindPermission(perm.GetType());
				if(index == -1)
				{
					permissions.Add(perm);
					return perm;
				}
				else
				{
					return (IPermission)(permissions[index]);
				}
			}

	// Assert the permissions in this set.
	public virtual void Assert()
			{
				// We must have the "Assertion" security flag for this to work.
				SecurityPermission perm;
				perm = new SecurityPermission(SecurityPermissionFlag.Assertion);
				perm.Demand();

				// Assert all of the CodeAccessPermission objects in the set.
				int posn;
				CodeAccessPermission caperm;
				for(posn = 0; posn < permissions.Count; ++posn)
				{
					caperm = (permissions[posn] as CodeAccessPermission);
					if(caperm != null)
					{
						caperm.Assert(2);
					}
				}
			}

	// Return a copy of this permission set.
	public virtual PermissionSet Copy()
			{
				return new PermissionSet(this);
			}

	// Demand the permissions in this set.
	public virtual void Demand()
			{
				int posn;
				IPermission perm;
				for(posn = 0; posn < permissions.Count; ++posn)
				{
					perm = ((IPermission)(permissions[posn]));
					if(perm != null)
					{
						perm.Demand();
					}
				}
			}

	// Deny the permissions in this set.
	public virtual void Deny()
			{
				int posn;
				CodeAccessPermission perm;
				for(posn = 0; posn < permissions.Count; ++posn)
				{
					perm = (permissions[posn] as CodeAccessPermission);
					if(perm != null)
					{
						perm.Deny(2);
					}
				}
			}

	// Convert an XML security element into a permission set.
	public virtual void FromXml(SecurityElement et)
			{
				// Validate the parameter.
				if(et == null)
				{
					throw new ArgumentNullException("et");
				}
				if(et.Tag != "PermissionSet")
				{
					throw new ArgumentException(_("Invalid_PermissionXml"));
				}
				if(et.Attribute("version") != "1")
				{
					throw new ArgumentException(_("Arg_PermissionVersion"));
				}

				// Initialize the permission set from the tag.
				if(et.Attribute("Unrestricted") == "true")
				{
					state = PermissionState.Unrestricted;
				}
				else
				{
					state = PermissionState.None;
				}
				permissions.Clear();

#if CONFIG_REFLECTION
				// Process the children.
				ArrayList children = et.Children;
				String className;
				Type type;
				Object[] args;
				IPermission perm;
				args = new Object [1];
				args[0] = PermissionState.None;
				if(children != null)
				{
					foreach(SecurityElement child in children)
					{
						if(child.Tag != "IPermission" &&
						   child.Tag != "Permission")
						{
							// Skip tags that we don't understand.
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
								("System.Security.Permissions." + className);
						}
						if(!typeof(IPermission).IsAssignableFrom(type))
						{
							throw new ArgumentException
								(_("Invalid_PermissionXml"));
						}
						perm = (Activator.CreateInstance(type, args)
									as IPermission);
						if(perm != null)
						{
							perm.FromXml(child);
							AddPermission(perm);
						}
					}
				}
#endif // CONFIG_REFLECTION
			}

	// Determine if this permission set is a subset of another.
	public virtual bool IsSubsetOf(PermissionSet target)
			{
				// Handle the simple cases first.
				if(target == null)
				{
					return false;
				}
				else if(target.IsUnrestricted())
				{
					return true;
				}
				else if(IsUnrestricted())
				{
					return false;
				}

				// Scan the source permission set and check subset conditions.
				IPermission other;
				foreach(IPermission perm in permissions)
				{
					other = target.GetPermission(perm.GetType());
					if(other == null || !perm.IsSubsetOf(other))
					{
						return false;
					}
				}
				return true;
			}

	// Permit only the permissions in this set.
	public virtual void PermitOnly()
			{
				// Demand the permission first, because we cannot permit it
				// for exclusive access if we are not allowed have it at all.
				Demand();

				// Create a permission set and copy all CA objects into it.
				PermissionSet set = new PermissionSet(PermissionState.None);
				int posn;
				CodeAccessPermission perm;
				for(posn = 0; posn < permissions.Count; ++posn)
				{
					perm = (permissions[posn] as CodeAccessPermission);
					if(perm != null)
					{
						set.AddPermission(perm.Copy());
					}
				}

				// Set the current "PermitOnly" context on the call stack.
				CodeAccessPermission.PermitOnly(set, 2);
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return ToXml().ToString();
			}

	// Convert this permission set into an XML security element.
	public virtual SecurityElement ToXml()
			{
				SecurityElement elem = new SecurityElement("PermissionSet");
				elem.AddAttribute
					("class", typeof(PermissionSet).AssemblyQualifiedName);
				elem.AddAttribute("version", "1");
				if(IsUnrestricted())
				{
					elem.AddAttribute("Unrestricted", "true");
				}
				foreach(IPermission perm in permissions)
				{
					elem.AddChild(perm.ToXml());
				}
				return elem;
			}

	// Form the union of this security set and another.
	public virtual PermissionSet Union(PermissionSet other)
			{
				PermissionSet pset;
				if(other == null)
				{
					pset = new PermissionSet(state);
				}
				else if(IsUnrestricted() || other.IsUnrestricted())
				{
					pset = new PermissionSet(PermissionState.Unrestricted);
				}
				else
				{
					pset = new PermissionSet(PermissionState.None);
				}
				foreach(IPermission perm in permissions)
				{
					pset.AddPermission(perm);
				}
				if(other == null || other.IsEmpty())
				{
					return pset;
				}
				IEnumerator e = other.GetEnumerator();
				IPermission permOther, permThis;
				int index;
				while(e.MoveNext())
				{
					permOther = (e.Current as IPermission);
					if(permOther != null)
					{
						index = pset.FindPermission(permOther.GetType());
						if(index != -1)
						{
							permThis = (IPermission)(permissions[index]);
							permThis = permThis.Union(permOther);
							if(permThis != null)
							{
								pset.SetPermission(permThis);
							}
						}
						else
						{
							pset.AddPermission(permOther);
						}
					}
				}
				return pset;
			}

	// Implement the ICollection interface.
	public virtual void CopyTo(Array array, int index)
			{
				permissions.CopyTo(array, index);
			}
	public virtual int Count
			{
				get
				{
					return permissions.Count;
				}
			}
	public virtual bool IsSynchronized
			{
				get
				{
					return false;
				}
			}
	public virtual Object SyncRoot
			{
				get
				{
					return this;
				}
			}

	// Implement the IEnumerable interface.
	public virtual IEnumerator GetEnumerator()
			{
				return permissions.GetEnumerator();
			}

	// Determine if this permission set is unrestricted.
#if ECMA_COMPAT
	internal
#else
	public
#endif
	virtual bool IsUnrestricted()
			{
				return (state == PermissionState.Unrestricted);
			}

	// Get a permission object of a specific type from this set.
#if ECMA_COMPAT
	internal
#else
	public
#endif
	virtual IPermission GetPermission(Type permClass)
			{
				if(permClass != null)
				{
					int index = FindPermission(permClass);
					if(index != -1)
					{
						return (IPermission)(permissions[index]);
					}
				}
				return null;
			}

	// Determine if this permission set is empty.
#if ECMA_COMPAT
	internal
#else
	public
#endif
	virtual bool IsEmpty()
			{
				return (permissions.Count == 0);
			}

	// Set a permission into this permission set.
#if ECMA_COMPAT
	internal
#else
	public
#endif
	virtual IPermission SetPermission(IPermission perm)
			{
				if(perm != null)
				{
					int index = FindPermission(perm.GetType());
					if(index != -1)
					{
						permissions[index] = perm;
					}
					else
					{
						permissions.Add(perm);
					}
					return perm;
				}
				else
				{
					return null;
				}
			}

	// Implement the IDeserializationCallback interface.
	void IDeserializationCallback.OnDeserialization(Object sender)
			{
				// Not needed in this implementation.
			}

#if !ECMA_COMPAT

	// Determine if this permission set is read-only.
	public virtual bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

	// Determine if the set contains permissions that do not
	// derive from CodeAccessPermission.
	public bool ContainsNonCodeAccessPermissions()
			{
				int posn;
				for(posn = 0; posn < permissions.Count; ++posn)
				{
					if(!(permissions[posn] is CodeAccessPermission))
					{
						return true;
					}
				}
				return false;
			}

	// Form the intersection of this permission set and another.
	public virtual PermissionSet Intersect(PermissionSet other)
			{
				PermissionSet pset;
				if(other == null)
				{
					pset = new PermissionSet(PermissionState.None);
				}
				else if(!IsUnrestricted() || !other.IsUnrestricted())
				{
					pset = new PermissionSet(PermissionState.None);
				}
				else
				{
					pset = new PermissionSet(PermissionState.Unrestricted);
				}
				if(other == null || other.IsEmpty() || IsEmpty())
				{
					return pset;
				}
				IEnumerator e = other.GetEnumerator();
				IPermission permOther, permThis;
				int index;
				while(e.MoveNext())
				{
					permOther = (e.Current as IPermission);
					if(permOther != null)
					{
						index = FindPermission(permOther.GetType());
						if(index != -1)
						{
							permThis = (IPermission)(permissions[index]);
							permThis = permThis.Intersect(permOther);
							if(permThis != null)
							{
								pset.AddPermission(permThis);
							}
						}
					}
				}
				return pset;
			}

	// Remove a particular permission from this set.
	public virtual IPermission RemovePermission(Type permClass)
			{
				if(permClass == null)
				{
					int index = FindPermission(permClass);
					if(index != -1)
					{
						IPermission perm = (IPermission)(permissions[index]);
						permissions.RemoveAt(index);
						return perm;
					}
				}
				return null;
			}

	// Convert a permission set from one format to another.
	public static byte[] ConvertPermissionSet
				(String inFormat, byte[] inData, String outFormat)
			{
				// Validate the parameters.
				if(inFormat == null)
				{
					throw new ArgumentNullException("inFormat");
				}
				if(inData == null)
				{
					throw new ArgumentNullException("inData");
				}
				if(outFormat == null)
				{
					throw new ArgumentNullException("outFormat");
				}

				// Convert the input data into a permission set.
				PermissionSet permSet;
				SecurityElement e;
				switch(inFormat.ToLower(CultureInfo.InvariantCulture))
				{
					case "xml": case "xmlascii":
					{
						permSet = new PermissionSet(PermissionState.None);
						e = (new MiniXml(Encoding.UTF8.GetString(inData)))
								.Parse();
						permSet.FromXml(e);
					}
					break;

					case "xmlunicode":
					{
						permSet = new PermissionSet(PermissionState.None);
						e = (new MiniXml(Encoding.Unicode.GetString(inData)))
								.Parse();
						permSet.FromXml(e);
					}
					break;

				#if CONFIG_SERIALIZATION
					case "binary":
					{
						MemoryStream inStream = new MemoryStream(inData);
						permSet = (PermissionSet)
							((new BinaryFormatter()).Deserialize(inStream));
					}
					break;
				#endif

					default: return null;
				}

				// Convert the output data into a permission set.
				switch(outFormat.ToLower(CultureInfo.InvariantCulture))
				{
					case "xml": case "xmlascii":
					{
						e = permSet.ToXml();
						return Encoding.UTF8.GetBytes(e.ToString());
					}
					// Not reached.

					case "xmlunicode":
					{
						e = permSet.ToXml();
						return Encoding.Unicode.GetBytes(e.ToString());
					}
					// Not reached.

				#if CONFIG_SERIALIZATION
					case "binary":
					{
						MemoryStream outStream = new MemoryStream();
						(new BinaryFormatter()).Serialize(outStream, permSet);
						return outStream.ToArray();
					}
					// Not reached.
				#endif
				}
				return null;
			}

#endif // !ECMA_COMPAT

	// Copy the contents of another permission set into this one.
	internal virtual void CopyFrom(PermissionSet pSet)
			{
				if(pSet.IsUnrestricted())
				{
					state = PermissionState.Unrestricted;
				}
				else
				{
					state = PermissionState.None;
				}
				permissions.Clear();
				foreach(IPermission perm in pSet)
				{
					SetPermission(perm.Copy());
				}
			}

}; // class PermissionSet

#endif // CONFIG_PERMISSIONS

}; // namespace System.Security
