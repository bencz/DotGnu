/*
 * RegistryPermission.cs - Implementation of the
 *		"System.Security.Permissions.RegistryPermission" class.
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

using System;
using System.IO;
using System.Collections;
using System.Security;

public sealed class RegistryPermission
	: CodeAccessPermission, IUnrestrictedPermission
{
	// Internal state.
	private PermissionState state;
	private String[] readList;
	private String[] writeList;
	private String[] createList;

	// Constructors.
	public RegistryPermission(PermissionState state)
			{
				if(state != PermissionState.Unrestricted &&
				   state != PermissionState.None)
				{
					throw new ArgumentException(_("Arg_PermissionState"));
				}
				this.state = state;
			}
	public RegistryPermission(RegistryPermissionAccess flag, String pathList)
			{
				if(pathList == null)
				{
					throw new ArgumentNullException("pathList");
				}
				if((flag & ~(RegistryPermissionAccess.AllAccess)) != 0)
				{
					throw new ArgumentException(_("Arg_RegistryAccess"));
				}
				this.state = PermissionState.None;
				String[] split = EnvironmentPermission.SplitPath(pathList);
				if((flag & RegistryPermissionAccess.Read) != 0)
				{
					readList = split;
				}
				if((flag & RegistryPermissionAccess.Write) != 0)
				{
					writeList = split;
				}
				if((flag & RegistryPermissionAccess.Create) != 0)
				{
					createList = split;
				}
			}
	internal RegistryPermission(PermissionState state, String[] readList,
							    String[] writeList, String[] createList)
			{
				this.state = state;
				this.readList = readList;
				this.writeList = writeList;
				this.createList = createList;
			}

	// Convert an XML value into a permissions value.
	public override void FromXml(SecurityElement esd)
			{
				String value;
				if(esd == null)
				{
					throw new ArgumentNullException("esd");
				}
				if(esd.Attribute("version") != "1")
				{
					throw new ArgumentException(_("Arg_PermissionVersion"));
				}
				value = esd.Attribute("Unrestricted");
				if(value != null && Boolean.Parse(value))
				{
					state = PermissionState.Unrestricted;
				}
				else
				{
					state = PermissionState.None;
				}
				if(state != PermissionState.Unrestricted)
				{
					readList = EnvironmentPermission.SplitPath
						(esd.Attribute("Read"), ';');
					writeList = EnvironmentPermission.SplitPath
						(esd.Attribute("Write"), ';');
					createList = EnvironmentPermission.SplitPath
						(esd.Attribute("Create"), ';');
				}
			}

	// Convert this permissions object into an XML value.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(RegistryPermission).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				if(state == PermissionState.Unrestricted)
				{
					element.AddAttribute("Unrestricted", "true");
				}
				else
				{
					// Always use ";" as the separator so that we can
					// guarantee a fixed external form, regardless of
					// whatever PathSeparator is set to.
					if(readList != null)
					{
						element.AddAttribute
							("Read", SecurityElement.Escape
								(String.Join(";", readList)));
					}
					if(writeList != null)
					{
						element.AddAttribute
							("Write", SecurityElement.Escape
								(String.Join(";", writeList)));
					}
					if(createList != null)
					{
						element.AddAttribute
							("Create", SecurityElement.Escape
								(String.Join(";", createList)));
					}
				}
				return element;
			}

	// Implement the IPermission interface.
	public override IPermission Copy()
			{
				return new RegistryPermission
					(state, readList, writeList, createList);
			}
	public override IPermission Intersect(IPermission target)
			{
				// Handle the easy cases first.
				if(target == null)
				{
					return target;
				}
				else if(!(target is RegistryPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(((RegistryPermission)target).IsUnrestricted())
				{
					return Copy();
				}
				else if(IsUnrestricted())
				{
					return target.Copy();
				}

				// Create a new object and intersect the lists.
				return new RegistryPermission
					(PermissionState.None,
					 EnvironmentPermission.Intersect(readList,
					 		   ((RegistryPermission)target).readList),
					 EnvironmentPermission.Intersect(writeList,
					 		   ((RegistryPermission)target).writeList),
					 EnvironmentPermission.Intersect(createList,
					 		   ((RegistryPermission)target).createList));
			}
	public override bool IsSubsetOf(IPermission target)
			{
				if(target == null)
				{
					return (state == PermissionState.None &&
							readList == null && writeList == null &&
							createList == null);
				}
				else if(!(target is RegistryPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(((RegistryPermission)target).IsUnrestricted())
				{
					return true;
				}
				else if(IsUnrestricted())
				{
					return false;
				}
				else
				{
					return EnvironmentPermission.IsSubsetOf
						(readList, ((RegistryPermission)target).readList) &&
							EnvironmentPermission.IsSubsetOf
						(writeList, ((RegistryPermission)target).writeList) &&
							EnvironmentPermission.IsSubsetOf
						(createList, ((RegistryPermission)target).createList);
				}
			}
	public override IPermission Union(IPermission target)
			{
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is RegistryPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(IsUnrestricted() ||
				        ((RegistryPermission)target).IsUnrestricted())
				{
					return new RegistryPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					return new RegistryPermission
						(PermissionState.None,
						 EnvironmentPermission.Union(readList,
						 	   ((RegistryPermission)target).readList, false),
						 EnvironmentPermission.Union(writeList,
						 	   ((RegistryPermission)target).writeList, false),
						 EnvironmentPermission.Union(createList,
						 	   ((RegistryPermission)target).createList, false));
				}
			}

	// Determine if this object has unrestricted permissions.
	public bool IsUnrestricted()
			{
				return (state == PermissionState.Unrestricted);
			}

	// Set the path list information.
	public void SetPathList(RegistryPermissionAccess flag, String pathList)
			{
				if(pathList == null)
				{
					throw new ArgumentNullException("pathList");
				}
				if((flag & ~(RegistryPermissionAccess.AllAccess)) != 0)
				{
					throw new ArgumentException(_("Arg_RegistryAccess"));
				}
				if((flag & RegistryPermissionAccess.Read) != 0)
				{
					readList = EnvironmentPermission.SplitPath
						(pathList, Path.PathSeparator);
				}
				if((flag & RegistryPermissionAccess.Write) != 0)
				{
					writeList = EnvironmentPermission.SplitPath
						(pathList, Path.PathSeparator);
				}
				if((flag & RegistryPermissionAccess.Create) != 0)
				{
					createList = EnvironmentPermission.SplitPath
						(pathList, Path.PathSeparator);
				}
			}

	// Add to the path list information.
	public void AddPathList(RegistryPermissionAccess flag, String pathList)
			{
				if(pathList == null)
				{
					throw new ArgumentNullException("pathList");
				}
				if((flag & ~(RegistryPermissionAccess.AllAccess)) != 0)
				{
					throw new ArgumentException(_("Arg_RegistryAccess"));
				}
				if((flag & RegistryPermissionAccess.Read) != 0)
				{
					readList = EnvironmentPermission.Union(readList,
						EnvironmentPermission.SplitPath
							(pathList, Path.PathSeparator), false);
				}
				if((flag & RegistryPermissionAccess.Write) != 0)
				{
					writeList = EnvironmentPermission.Union(writeList,
						EnvironmentPermission.SplitPath
							(pathList, Path.PathSeparator), false);
				}
				if((flag & RegistryPermissionAccess.Create) != 0)
				{
					createList = EnvironmentPermission.Union(createList,
						EnvironmentPermission.SplitPath
							(pathList, Path.PathSeparator), false);
				}
			}

	// Get a specific path list.
	public String GetPathList(RegistryPermissionAccess flag)
			{
				switch(flag)
				{
					case RegistryPermissionAccess.Read:
					{
						if(readList != null)
						{
							return String.Join
								(Path.PathSeparator.ToString(), readList);
						}
						else
						{
							return String.Empty;
						}
					}
					// Not reached.

					case RegistryPermissionAccess.Write:
					{
						if(writeList != null)
						{
							return String.Join
								(Path.PathSeparator.ToString(), writeList);
						}
						else
						{
							return String.Empty;
						}
					}
					// Not reached.

					case RegistryPermissionAccess.Create:
					{
						if(createList != null)
						{
							return String.Join
								(Path.PathSeparator.ToString(), createList);
						}
						else
						{
							return String.Empty;
						}
					}
					// Not reached.

					default:
					{
						throw new ArgumentException(_("Arg_RegistryAccess"));
					}
					// Not reached.
				}
			}

}; // class RegistryPermission

#endif // CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
