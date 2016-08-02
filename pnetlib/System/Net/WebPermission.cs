/*
 * WebPermission.cs - Implementation of the
 *		"System.Security.Permissions.WebPermission" class.
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

namespace System.Net
{

#if CONFIG_PERMISSIONS

using System;
using System.Collections;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Text;

public sealed class WebPermission : CodeAccessPermission
#if !ECMA_COMPAT
	, IUnrestrictedPermission
#endif
{
	// Internal state.
	private PermissionState state;
	private ArrayList acceptList;
	private ArrayList connectList;

	// Constructors.
	public WebPermission()
			{
				this.state = PermissionState.None;
				this.acceptList = new ArrayList();
				this.connectList = new ArrayList();
			}
	public WebPermission(PermissionState state)
			{
				this.state = state;
				this.acceptList = new ArrayList();
				this.connectList = new ArrayList();
			}
#if !ECMA_COMPAT
	public WebPermission(NetworkAccess access, Regex uriRegex) : this()
			{
				AddPermission(access, uriRegex);
			}
#endif
	public WebPermission(NetworkAccess access, String uriString) : this()
			{
				AddPermission(access, uriString);
			}
	private WebPermission(WebPermission copyFrom)
			{
				this.state = copyFrom.state;
				this.acceptList = (ArrayList)(copyFrom.acceptList.Clone());
				this.connectList = (ArrayList)(copyFrom.connectList.Clone());
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
					throw new ArgumentException(S._("Arg_PermissionVersion"));
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
				acceptList.Clear();
				connectList.Clear();
				if(state != PermissionState.Unrestricted)
				{
					SecurityElement child;
					String str;
					child = esd.SearchForChildByTag("ConnectAccess");
					if(child != null && child.Children != null)
					{
						foreach(SecurityElement uri1 in child.Children)
						{
							if(uri1.Tag != "URI")
							{
								continue;
							}
							str = uri1.Attribute("uri");
							if(!IsRegex(str))
							{
								connectList.Add(RegexUnescape(str));
							}
						#if !ECMA_COMPAT
							else
							{
								connectList.Add(new Regex(str));
							}
						#endif
						}
					}
					child = esd.SearchForChildByTag("AcceptAccess");
					if(child != null && child.Children != null)
					{
						foreach(SecurityElement uri1 in child.Children)
						{
							if(uri1.Tag != "URI")
							{
								continue;
							}
							str = uri1.Attribute("uri");
							if(!IsRegex(str))
							{
								acceptList.Add(RegexUnescape(str));
							}
						#if !ECMA_COMPAT
							else
							{
								acceptList.Add(new Regex(str));
							}
						#endif
						}
					}
				}
			}

	// Convert this permissions object into an XML value.
	public override SecurityElement ToXml()
			{
				SecurityElement element;
				SecurityElement child;
				SecurityElement uri;
				element = new SecurityElement("IPermission");
				element.AddAttribute
					("class",
					 SecurityElement.Escape(typeof(WebPermission).
					 						AssemblyQualifiedName));
				element.AddAttribute("version", "1");
				if(state == PermissionState.Unrestricted)
				{
					element.AddAttribute("Unrestricted", "true");
				}
				else
				{
					if(connectList.Count > 0)
					{
						child = new SecurityElement("ConnectAccess");
						element.AddChild(child);
						foreach(Object p1 in connectList)
						{
							uri = new SecurityElement("URI");
							child.AddChild(uri);
						#if !ECMA_COMPAT
							if(p1 is Regex)
							{
								uri.AddAttribute
									("uri",
									 SecurityElement.Escape(p1.ToString()));
							}
							else
						#endif
							{
								uri.AddAttribute
									("uri",
									 SecurityElement.Escape
									 	(RegexEscape(p1.ToString())));
							}
						}
					}
					if(acceptList.Count > 0)
					{
						child = new SecurityElement("AcceptAccess");
						element.AddChild(child);
						foreach(Object p2 in acceptList)
						{
							uri = new SecurityElement("URI");
							child.AddChild(uri);
						#if !ECMA_COMPAT
							if(p2 is Regex)
							{
								uri.AddAttribute
									("uri",
									 SecurityElement.Escape(p2.ToString()));
							}
							else
						#endif
							{
								uri.AddAttribute
									("uri",
									 SecurityElement.Escape
									 	(RegexEscape(p2.ToString())));
							}
						}
					}
				}
				return element;
			}

	// Implement the IPermission interface.
	public override IPermission Copy()
			{
				return new WebPermission(this);
			}
	public override IPermission Intersect(IPermission target)
			{
				if(target == null)
				{
					return target;
				}
				else if(!(target is WebPermission))
				{
					throw new ArgumentException(S._("Arg_PermissionMismatch"));
				}
				else if(((WebPermission)target).IsUnrestricted())
				{
					return Copy();
				}
				else if(IsUnrestricted())
				{
					return target.Copy();
				}
				else
				{
					WebPermission perm = new WebPermission();
					WebPermission other = (WebPermission)target;
					foreach(Object p1 in acceptList)
					{
						if(!other.acceptList.Contains(p1))
						{
							perm.acceptList.Add(p1);
						}
					}
					foreach(Object p2 in connectList)
					{
						if(!other.connectList.Contains(p2))
						{
							perm.connectList.Add(p2);
						}
					}
					return perm;
				}
			}
	public override bool IsSubsetOf(IPermission target)
			{
				if(target == null)
				{
					return (state == PermissionState.None &&
							acceptList.Count == 0 &&
							connectList.Count == 0);
				}
				else if(!(target is WebPermission))
				{
					throw new ArgumentException(S._("Arg_PermissionMismatch"));
				}
				else if(((WebPermission)target).IsUnrestricted())
				{
					return true;
				}
				else if(IsUnrestricted())
				{
					return false;
				}
				else
				{
					WebPermission other = (WebPermission)target;
					foreach(Object p1 in acceptList)
					{
						if(!other.acceptList.Contains(p1))
						{
							return false;
						}
					}
					foreach(Object p2 in connectList)
					{
						if(!other.connectList.Contains(p2))
						{
							return false;
						}
					}
					return true;
				}
			}
	public override IPermission Union(IPermission target)
			{
				if(target == null)
				{
					return Copy();
				}
				else if(!(target is WebPermission))
				{
					throw new ArgumentException(S._("Arg_PermissionMismatch"));
				}
				else if(IsUnrestricted() ||
				        ((WebPermission)target).IsUnrestricted())
				{
					return new WebPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					WebPermission perm = new WebPermission
							((WebPermission)target);
					foreach(Object p1 in acceptList)
					{
						if(!perm.acceptList.Contains(p1))
						{
							perm.acceptList.Add(p1);
						}
					}
					foreach(Object p2 in connectList)
					{
						if(!perm.connectList.Contains(p2))
						{
							perm.connectList.Add(p2);
						}
					}
					return perm;
				}
			}

	// Determine if this object has unrestricted permissions.
#if ECMA_COMPAT
	private bool IsUnrestricted()
#else
	public bool IsUnrestricted()
#endif
			{
				return (state == PermissionState.Unrestricted);
			}

	// Add permission information to this permissions object.
#if !ECMA_COMPAT
	public void AddPermission(NetworkAccess access, Regex uriRegex)
			{
				if(state == PermissionState.Unrestricted)
				{
					// No need to add permissions to an unrestricted set.
					return;
				}
				if(uriRegex == null)
				{
					throw new ArgumentNullException("uriRegex");
				}
				if(access == NetworkAccess.Connect)
				{
					connectList.Add(uriRegex);
				}
				else if(access == NetworkAccess.Accept)
				{
					acceptList.Add(uriRegex);
				}
			}
#endif // !ECMA_COMPAT
	public void AddPermission(NetworkAccess access, String uriString)
			{
				if(state == PermissionState.Unrestricted)
				{
					// No need to add permissions to an unrestricted set.
					return;
				}
				if(uriString == null)
				{
					throw new ArgumentNullException("uriString");
				}
				if(access == NetworkAccess.Connect)
				{
					connectList.Add(uriString);
				}
				else if(access == NetworkAccess.Accept)
				{
					acceptList.Add(uriString);
				}
			}
	internal void AddPermission(NetworkAccess access, Object uri)
			{
				if(state == PermissionState.Unrestricted)
				{
					// No need to add permissions to an unrestricted set.
					return;
				}
				if(uri == null)
				{
					throw new ArgumentNullException("uri");
				}
				if(access == NetworkAccess.Connect)
				{
					connectList.Add(uri);
				}
				else if(access == NetworkAccess.Accept)
				{
					acceptList.Add(uri);
				}
			}

	// Iterate over the list of accept permissions.
	public IEnumerator AcceptList
			{
				get
				{
					return acceptList.GetEnumerator();
				}
			}

	// Iterate over the list of connection permissions.
	public IEnumerator ConnectList
			{
				get
				{
					return connectList.GetEnumerator();
				}
			}

	// Special characters for regular expressions.
	private const String regexChars = "\\+?.*()|{[^$#";

	// Escape a non-regex string.
	private static String RegexEscape(String str)
			{
				if(str == null)
				{
					return null;
				}
				StringBuilder builder = new StringBuilder();
				foreach(char ch in str)
				{
					if(regexChars.IndexOf(ch) != -1)
					{
						builder.Append('\\');
						builder.Append(ch);
					}
					else if(ch == '\t')
					{
						builder.Append("\\t");
					}
					else if(ch == '\r')
					{
						builder.Append("\\r");
					}
					else if(ch == '\n')
					{
						builder.Append("\\n");
					}
					else if(ch == '\f')
					{
						builder.Append("\\f");
					}
					else
					{
						builder.Append(ch);
					}
				}
				return builder.ToString();
			}

	// Determine if a string looks like a regex.
	private static bool IsRegex(String str)
			{
				if(str == null)
				{
					return false;
				}
				int index;
				char ch;
				for(index = 0; index < str.Length; ++index)
				{
					ch = str[index];
					if(ch == '\\')
					{
						++index;
						if(index >= str.Length)
						{
							return true;
						}
					}
					else if(regexChars.IndexOf(ch) != -1)
					{
						return true;
					}
				}
				return false;
			}

	// Unescape a regex string to turn it into a normal string.
	private static String RegexUnescape(String str)
			{
				if(str == null)
				{
					return null;
				}
				StringBuilder builder = new StringBuilder();
				int index;
				char ch;
				for(index = 0; index < str.Length; ++index)
				{
					ch = str[index];
					if(ch == '\\')
					{
						++index;
						if(index >= str.Length)
						{
							break;
						}
						ch = str[index];
						if(ch == 't')
						{
							builder.Append('\t');
						}
						else if(ch == 'r')
						{
							builder.Append('\r');
						}
						else if(ch == 'n')
						{
							builder.Append('\n');
						}
						else if(ch == 'f')
						{
							builder.Append('\f');
						}
						else
						{
							builder.Append(ch);
						}
					}
					else
					{
						builder.Append(ch);
					}
				}
				return builder.ToString();
			}

}; // class WebPermission

#endif // CONFIG_PERMISSIONS

}; // namespace System.Net
