/*
 * FileIOPermission.cs - Implementation of the
 *		"System.Security.Permissions.FileIOPermission" class.
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

#if CONFIG_PERMISSIONS

using System;
using System.IO;
using System.Collections;
using System.Security;

public sealed class FileIOPermission
	: CodeAccessPermission, IUnrestrictedPermission
{
	// Internal state.
	private PermissionState state;
	private String[] readList;
	private String[] writeList;
	private String[] appendList;
	private String[] discoveryList;
	private FileIOPermissionAccess allLocalFiles;
	private FileIOPermissionAccess allFiles;

	// Constructors.
	public FileIOPermission(PermissionState state)
			{
				if(state != PermissionState.Unrestricted &&
				   state != PermissionState.None)
				{
					throw new ArgumentException(_("Arg_PermissionState"));
				}
				this.state = state;
				if(state == PermissionState.Unrestricted)
				{
					allLocalFiles = FileIOPermissionAccess.AllAccess;
					allFiles = FileIOPermissionAccess.AllAccess;
				}
				else
				{
					allLocalFiles = FileIOPermissionAccess.NoAccess;
					allFiles = FileIOPermissionAccess.NoAccess;
				}
			}
	public FileIOPermission(FileIOPermissionAccess flag, String pathList)
			{
				if(pathList == null)
				{
					throw new ArgumentNullException("pathList");
				}
				if((flag & ~(FileIOPermissionAccess.AllAccess)) != 0)
				{
					throw new ArgumentException(_("Arg_FileIOAccess"));
				}
				this.state = PermissionState.None;
				String[] split = EnvironmentPermission.SplitPath(pathList);
				if((flag & FileIOPermissionAccess.Read) != 0)
				{
					readList = split;
				}
				if((flag & FileIOPermissionAccess.Write) != 0)
				{
					writeList = split;
				}
				if((flag & FileIOPermissionAccess.Append) != 0)
				{
					appendList = split;
				}
				if((flag & FileIOPermissionAccess.PathDiscovery) != 0)
				{
					discoveryList = split;
				}
				allLocalFiles = FileIOPermissionAccess.NoAccess;
				allFiles = FileIOPermissionAccess.NoAccess;
			}
	internal FileIOPermission(PermissionState state, String[] readList,
							  String[] writeList, String[] appendList,
							  String[] discoveryList,
							  FileIOPermissionAccess allLocalFiles,
							  FileIOPermissionAccess allFiles)
			{
				this.state = state;
				this.readList = readList;
				this.writeList = writeList;
				this.appendList = appendList;
				this.discoveryList = discoveryList;
				this.allLocalFiles = allLocalFiles;
				this.allFiles = allFiles;
			}
#if !ECMA_COMPAT
	public FileIOPermission(FileIOPermissionAccess flag, String[] pathList)
			{
				if(pathList == null)
				{
					throw new ArgumentNullException("pathList");
				}
				if((flag & ~(FileIOPermissionAccess.AllAccess)) != 0)
				{
					throw new ArgumentException(_("Arg_FileIOAccess"));
				}
				this.state = PermissionState.None;
				if((flag & FileIOPermissionAccess.Read) != 0)
				{
					readList = pathList;
				}
				if((flag & FileIOPermissionAccess.Write) != 0)
				{
					writeList = pathList;
				}
				if((flag & FileIOPermissionAccess.Append) != 0)
				{
					appendList = pathList;
				}
				if((flag & FileIOPermissionAccess.PathDiscovery) != 0)
				{
					discoveryList = pathList;
				}
				allLocalFiles = FileIOPermissionAccess.NoAccess;
				allFiles = FileIOPermissionAccess.NoAccess;
			}
#endif

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
					appendList = EnvironmentPermission.SplitPath
						(esd.Attribute("Append"), ';');
					discoveryList = EnvironmentPermission.SplitPath
						(esd.Attribute("PathDiscovery"), ';');
				}
			}

	// Convert this permissions object into an XML value.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(FileIOPermission).
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
					if(appendList != null)
					{
						element.AddAttribute
							("Append", SecurityElement.Escape
								(String.Join(";", appendList)));
					}
					if(discoveryList != null)
					{
						element.AddAttribute
							("PathDiscovery", SecurityElement.Escape
								(String.Join(";", discoveryList)));
					}
				}
				return element;
			}

	// Implement the IPermission interface.
	public override IPermission Copy()
			{
				return new FileIOPermission
					(state, readList, writeList, appendList, discoveryList,
					 allLocalFiles, allFiles);
			}
	public override IPermission Intersect(IPermission target)
			{
				// Handle the easy cases first.
				if(target == null)
				{
					return target;
				}
				else if(!(target is FileIOPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(((FileIOPermission)target).IsUnrestricted())
				{
					return Copy();
				}
				else if(IsUnrestricted())
				{
					return target.Copy();
				}

				// Create a new object and intersect the lists.
				return new FileIOPermission
					(PermissionState.None,
					 EnvironmentPermission.Intersect(readList,
					 		   ((FileIOPermission)target).readList),
					 EnvironmentPermission.Intersect(writeList,
					 		   ((FileIOPermission)target).writeList),
					 EnvironmentPermission.Intersect(appendList,
					 		   ((FileIOPermission)target).appendList),
					 EnvironmentPermission.Intersect(discoveryList,
					 		   ((FileIOPermission)target).discoveryList),
					 allLocalFiles & ((FileIOPermission)target).allLocalFiles,
					 allFiles & ((FileIOPermission)target).allFiles);
			}
	public override bool IsSubsetOf(IPermission target)
			{
				if(target == null)
				{
					return (state == PermissionState.None &&
							readList == null && writeList == null &&
							appendList == null && discoveryList == null);
				}
				else if(!(target is FileIOPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(((FileIOPermission)target).IsUnrestricted())
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
						(readList, ((FileIOPermission)target).readList) &&
							EnvironmentPermission.IsSubsetOf
						(writeList, ((FileIOPermission)target).writeList) &&
							EnvironmentPermission.IsSubsetOf
						(appendList, ((FileIOPermission)target).appendList) &&
							EnvironmentPermission.IsSubsetOf
						(discoveryList,
						 ((FileIOPermission)target).discoveryList) &&
						((allLocalFiles &
							((FileIOPermission)target).allLocalFiles) ==
								allLocalFiles) &&
						((allFiles &
							((FileIOPermission)target).allFiles) ==
								allFiles);
				}
			}
	public override IPermission Union(IPermission target)
			{
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is FileIOPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(IsUnrestricted() ||
				        ((FileIOPermission)target).IsUnrestricted())
				{
					return new FileIOPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					return new FileIOPermission
						(PermissionState.None,
						 EnvironmentPermission.Union(readList,
						 	   ((FileIOPermission)target).readList, false),
						 EnvironmentPermission.Union(writeList,
						 	   ((FileIOPermission)target).writeList, false),
						 EnvironmentPermission.Union(appendList,
						 	   ((FileIOPermission)target).appendList, false),
						 EnvironmentPermission.Union(discoveryList,
						 	   ((FileIOPermission)target).discoveryList, false),
						 allLocalFiles |
						 	((FileIOPermission)target).allLocalFiles,
						 allFiles | ((FileIOPermission)target).allFiles);
				}
			}

	// Determine if this object has unrestricted permissions.
#if ECMA_COMPAT
	private bool IsUnrestricted()
			{
				return (state == PermissionState.Unrestricted);
			}
	bool IUnrestrictedPermission.IsUnrestricted()
#else
	public bool IsUnrestricted()
#endif
			{
				return (state == PermissionState.Unrestricted);
			}

#if !ECMA_COMPAT

	// Clear specific path lists.
	private void ClearPathList(FileIOPermissionAccess access)
			{
				if((access & ~(FileIOPermissionAccess.AllAccess)) != 0)
				{
					throw new ArgumentException(_("Arg_FileIOAccess"));
				}
				if((access & FileIOPermissionAccess.Read) != 0)
				{
					readList = null;
				}
				if((access & FileIOPermissionAccess.Write) != 0)
				{
					writeList = null;
				}
				if((access & FileIOPermissionAccess.Append) != 0)
				{
					appendList = null;
				}
				if((access & FileIOPermissionAccess.PathDiscovery) != 0)
				{
					discoveryList = null;
				}
			}

	// Set the path list information.
	public void SetPathList(FileIOPermissionAccess access, String path)
			{
				ClearPathList(access);
				AddPathList(access, new String[] { path });
			}
	public void SetPathList(FileIOPermissionAccess access, String[] pathList)
			{
				ClearPathList(access);
				AddPathList(access, pathList);
			}

	// Add to the path list information.
	public void AddPathList(FileIOPermissionAccess access, String path)
			{
				AddPathList(access, new String[] { path });
			}
	public void AddPathList(FileIOPermissionAccess access, String[] pathList)
			{
				if(pathList == null)
				{
					throw new ArgumentNullException("pathList");
				}
				if((access & ~(FileIOPermissionAccess.AllAccess)) != 0)
				{
					throw new ArgumentException(_("Arg_FileIOAccess"));
				}
				foreach(String s in pathList)
				{
					if(s == null)
					{
						throw new ArgumentNullException("pathList element");
					}
				}
				if((access & FileIOPermissionAccess.Read) != 0)
				{
					readList = EnvironmentPermission.Union
						(readList, pathList, true);
				}
				if((access & FileIOPermissionAccess.Write) != 0)
				{
					writeList = EnvironmentPermission.Union
						(writeList, pathList, true);
				}
				if((access & FileIOPermissionAccess.Append) != 0)
				{
					appendList = EnvironmentPermission.Union
						(appendList, pathList, true);
				}
				if((access & FileIOPermissionAccess.PathDiscovery) != 0)
				{
					discoveryList = EnvironmentPermission.Union
						(discoveryList, pathList, true);
				}
			}

	// Get a specific path list.
	public String[] GetPathList(FileIOPermissionAccess access)
			{
				switch(access)
				{
					case FileIOPermissionAccess.Read:
					{
						if(readList != null)
						{
							return (String[])(readList.Clone());
						}
						else
						{
							return null;
						}
					}
					// Not reached.

					case FileIOPermissionAccess.Write:
					{
						if(writeList != null)
						{
							return (String[])(writeList.Clone());
						}
						else
						{
							return null;
						}
					}
					// Not reached.

					case FileIOPermissionAccess.Append:
					{
						if(appendList != null)
						{
							return (String[])(appendList.Clone());
						}
						else
						{
							return null;
						}
					}
					// Not reached.

					case FileIOPermissionAccess.PathDiscovery:
					{
						if(discoveryList != null)
						{
							return (String[])(discoveryList.Clone());
						}
						else
						{
							return null;
						}
					}
					// Not reached.

					default:
					{
						throw new ArgumentException(_("Arg_FileIOAccess"));
					}
					// Not reached.
				}
			}

	// Get or set the "all local files" flags.
	public FileIOPermissionAccess AllLocalFiles
			{
				get
				{
					return allLocalFiles;
				}
				set
				{
					if(state != PermissionState.Unrestricted)
					{
						allLocalFiles =
							value & FileIOPermissionAccess.AllAccess;
					}
				}
			}

	// Get or set the "all files" flags.
	public FileIOPermissionAccess AllFiles
			{
				get
				{
					return allFiles;
				}
				set
				{
					if(state != PermissionState.Unrestricted)
					{
						allFiles = value & FileIOPermissionAccess.AllAccess;
					}
				}
			}

#endif // !ECMA_COMPAT

}; // class FileIOPermission

#endif // CONFIG_PERMISSIONS

}; // namespace System.Security.Permissions
