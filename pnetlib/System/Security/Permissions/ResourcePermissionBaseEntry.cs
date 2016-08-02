/*
 * ResourcePermissionBaseEntry.cs - Implementation of the
 *			"System.Diagnostics.ResourcePermissionBaseEntry" class.
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

namespace System.Security.Permissions
{

#if CONFIG_PERMISSIONS && !ECMA_COMPAT

[Serializable]
public class ResourcePermissionBaseEntry
{
	// Internal state.
	private int permissionAccess;
	private String[] permissionPath;

	// Constructors.
	public ResourcePermissionBaseEntry()
			{
				permissionAccess = 0;
				permissionPath = new String [0];
			}
	public ResourcePermissionBaseEntry(int permissionAccess,
									   String[] permissionPath)
			{
				if(permissionPath == null)
				{
					throw new ArgumentNullException("permissionPath");
				}
				this.permissionAccess = permissionAccess;
				this.permissionPath = permissionPath;
			}

	// Get this object's properties.
	public int PermissionAccess
			{
				get
				{
					return permissionAccess;
				}
			}
	public String[] PermissionAccessPath
			{
				get
				{
					return permissionPath;
				}
			}

	// Get the string form of the path.
	internal String StringPath
			{
				get
				{
					if(permissionPath.Length == 0)
					{
						return String.Empty;
					}
					else
					{
						return String.Join("\\", permissionPath);
					}
				}
			}

}; // class ResourcePermissionBaseEntry

#endif // CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
