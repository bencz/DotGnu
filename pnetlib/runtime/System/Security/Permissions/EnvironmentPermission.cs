/*
 * EnvironmentPermission.cs - Implementation of the
 *		"System.Security.Permissions.EnvironmentPermission" class.
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

public sealed class EnvironmentPermission
	: CodeAccessPermission, IUnrestrictedPermission
{
	// Internal state.
	private PermissionState state;
	private String[] readList;
	private String[] writeList;

	// Constructors.
	public EnvironmentPermission(PermissionState state)
			{
				if(state != PermissionState.Unrestricted &&
				   state != PermissionState.None)
				{
					throw new ArgumentException(_("Arg_PermissionState"));
				}
				this.state = state;
			}
	public EnvironmentPermission(EnvironmentPermissionAccess flag,
								 String pathList)
			{
				if(pathList == null)
				{
					throw new ArgumentNullException("pathList");
				}
				if((flag & ~(EnvironmentPermissionAccess.AllAccess)) != 0)
				{
					throw new ArgumentException(_("Arg_EnvironmentAccess"));
				}
				this.state = PermissionState.None;
				if((flag & EnvironmentPermissionAccess.Read) != 0)
				{
					readList = SplitPath(pathList, Path.PathSeparator);
				}
				if((flag & EnvironmentPermissionAccess.Write) != 0)
				{
					writeList = SplitPath(pathList, Path.PathSeparator);
				}
			}
	internal EnvironmentPermission(PermissionState state, String[] readList,
								   String[] writeList)
			{
				this.state = state;
				this.readList = readList;
				this.writeList = writeList;
			}

	// Split a path string into an array.
	internal static String[] SplitPath(String path, char separator)
			{
				if(path == null || path == String.Empty)
				{
					return null;
				}
				if(separator == ';')
				{
					return path.Split(';');
				}
				else
				{
					// On Unix platforms, the caller might have constructed a
					// string using PathSeparator, or supplied an explicit
					// path string using ';'.  This will handle both cases.
					return path.Split(separator, ';');
				}
			}
	internal static String[] SplitPath(String path)
			{
				return SplitPath(path, Path.PathSeparator);
			}

	// Create the intersection of two string lists.
	internal static String[] Intersect(String[] list1, String[] list2)
			{
				if(list1 == null || list2 == null)
				{
					return null;
				}
				int count = 0;
				foreach(String s1 in list1)
				{
					if(((IList)list2).Contains(s1))
					{
						++count;
					}
				}
				if(count == 0)
				{
					return null;
				}
				String[] list = new String [count];
				count = 0;
				foreach(String s2 in list1)
				{
					if(((IList)list2).Contains(s2))
					{
						list[count++] = s2;
					}
				}
				return list;
			}

	// Determine if one string list is a subset of another.
	internal static bool IsSubsetOf(String[] list1, String[] list2)
			{
				if(list1 == null)
				{
					return true;
				}
				else if(list2 == null)
				{
					return false;
				}
				foreach(String s in list1)
				{
					if(!((IList)list2).Contains(s))
					{
						return false;
					}
				}
				return true;
			}

	// Create the union of two string lists.
	internal static String[] Union(String[] list1, String[] list2, bool clone)
			{
				if(list1 == null)
				{
					if(list2 == null || !clone)
					{
						return list2;
					}
					else
					{
						return (String[])(list2.Clone());
					}
				}
				else if(list2 == null)
				{
					return list1;
				}
				int count = list1.Length;
				foreach(String s1 in list2)
				{
					if(!((IList)list1).Contains(s1))
					{
						++count;
					}
				}
				if(count == 0)
				{
					return null;
				}
				String[] list = new String [count];
				count = list1.Length;
				Array.Copy(list1, 0, list, 0, count);
				foreach(String s2 in list2)
				{
					if(!((IList)list1).Contains(s2))
					{
						list[count++] = s2;
					}
				}
				return list;
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
					readList = SplitPath(esd.Attribute("Read"), ';');
					writeList = SplitPath(esd.Attribute("Write"), ';');
				}
			}

	// Convert this permissions object into an XML value.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(EnvironmentPermission).
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
				}
				return element;
			}

	// Implement the IPermission interface.
	public override IPermission Copy()
			{
				return new EnvironmentPermission(state, readList, writeList);
			}
	public override IPermission Intersect(IPermission target)
			{
				// Handle the easy cases first.
				if(target == null)
				{
					return target;
				}
				else if(!(target is EnvironmentPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(((EnvironmentPermission)target).IsUnrestricted())
				{
					return Copy();
				}
				else if(IsUnrestricted())
				{
					return target.Copy();
				}

				// Create a new object and intersect the lists.
				return new EnvironmentPermission
					(PermissionState.None,
					 Intersect(readList,
					 		   ((EnvironmentPermission)target).readList),
					 Intersect(writeList,
					 		   ((EnvironmentPermission)target).writeList));
			}
	public override bool IsSubsetOf(IPermission target)
			{
				if(target == null)
				{
					return (state == PermissionState.None &&
							readList == null && writeList == null);
				}
				else if(!(target is EnvironmentPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(((EnvironmentPermission)target).IsUnrestricted())
				{
					return true;
				}
				else if(IsUnrestricted())
				{
					return false;
				}
				else
				{
					return IsSubsetOf
						(readList, ((EnvironmentPermission)target).readList) &&
							IsSubsetOf
						(writeList, ((EnvironmentPermission)target).writeList);
				}
			}
	public override IPermission Union(IPermission target)
			{
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is EnvironmentPermission))
				{
					throw new ArgumentException(_("Arg_PermissionMismatch"));
				}
				else if(IsUnrestricted() ||
				        ((EnvironmentPermission)target).IsUnrestricted())
				{
					return new EnvironmentPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					return new EnvironmentPermission
						(PermissionState.None,
						 Union(readList,
						 	   ((EnvironmentPermission)target).readList,
							   false),
						 Union(writeList,
						 	   ((EnvironmentPermission)target).writeList,
							   false));
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

	// Set the path list information.
	public void SetPathList(EnvironmentPermissionAccess flag, String pathList)
			{
				if(pathList == null)
				{
					throw new ArgumentNullException("pathList");
				}
				if((flag & ~(EnvironmentPermissionAccess.AllAccess)) != 0)
				{
					throw new ArgumentException(_("Arg_EnvironmentAccess"));
				}
				if((flag & EnvironmentPermissionAccess.Read) != 0)
				{
					readList = SplitPath(pathList, Path.PathSeparator);
				}
				if((flag & EnvironmentPermissionAccess.Write) != 0)
				{
					writeList = SplitPath(pathList, Path.PathSeparator);
				}
			}

	// Add to the path list information.
	public void AddPathList(EnvironmentPermissionAccess flag, String pathList)
			{
				if(pathList == null)
				{
					throw new ArgumentNullException("pathList");
				}
				if((flag & ~(EnvironmentPermissionAccess.AllAccess)) != 0)
				{
					throw new ArgumentException(_("Arg_EnvironmentAccess"));
				}
				if((flag & EnvironmentPermissionAccess.Read) != 0)
				{
					readList = Union(readList,
						SplitPath(pathList, Path.PathSeparator), false);
				}
				if((flag & EnvironmentPermissionAccess.Write) != 0)
				{
					writeList = Union(writeList,
						SplitPath(pathList, Path.PathSeparator), false);
				}
			}

	// Get a specific path list.
	public String GetPathList(EnvironmentPermissionAccess flag)
			{
				switch(flag)
				{
					case EnvironmentPermissionAccess.Read:
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

					case EnvironmentPermissionAccess.Write:
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

					default:
					{
						throw new ArgumentException(_("Arg_EnvironmentAccess"));
					}
					// Not reached.
				}
			}

#endif // !ECMA_COMPAT

}; // class EnvironmentPermission

#endif // CONFIG_PERMISSIONS

}; // namespace System.Security.Permissions
