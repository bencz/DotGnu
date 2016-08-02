/*
 * PermissionSetAttribute.cs - Implementation of the
 *			"System.Security.Permissions.PermissionSetAttribute" class.
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
using System.Security;

[AttributeUsage(AttributeTargets.Assembly |
			 	AttributeTargets.Class |
			 	AttributeTargets.Struct |
			 	AttributeTargets.Constructor |
			 	AttributeTargets.Method,
			 	AllowMultiple=true, Inherited=false)]
public sealed class PermissionSetAttribute : CodeAccessSecurityAttribute
{
	// Internal state.
	private String file;
	private String name;
	private bool unicodeEncoded;
	private String xml;

	// Constructors.
	public PermissionSetAttribute(SecurityAction action)
			: base(action)
			{
				// Nothing to do here.
			}

	// Get or set the file for this permission set.
	public String File
			{
				get
				{
					return file;
				}
				set
				{
					file = value;
				}
			}

	// Get or set the name of this permission set.
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

	// Get or set the "unicode encoded" flag for this permission set.
	public bool UnicodeEncoded
			{
				get
				{
					return unicodeEncoded;
				}
				set
				{
					unicodeEncoded = value;
				}
			}

	// Get or set the XML data for this permission set.
	public String XML
			{
				get
				{
					return xml;
				}
				set
				{
					xml = value;
				}
			}

	// Create a permission object that corresponds to this attribute.
	public override IPermission CreatePermission()
			{
				// Use "CreatePermissionSet" instead.
				return null;
			}

	// Create a builtin permission set by name.
	private static PermissionSet CreateBuiltinPermissionSet(String name)
			{
				NamedPermissionSet set = null;
				switch(name)
				{
					case "Execution":
					{
						set = new NamedPermissionSet
							("Execution", PermissionState.None);
						set.Description = _("Arg_PermissionsExecution");
						set.AddPermission(new SecurityPermission
								(SecurityPermissionFlag.Execution));
					}
					break;

					case "FullTrust":
					{
						set = new NamedPermissionSet
							("FullTrust", PermissionState.Unrestricted);
						set.Description = _("Arg_PermissionsFullTrust");
					}
					break;

					case "Internet":
					{
						set = new NamedPermissionSet
							("Internet", PermissionState.None);
						set.Description = _("Arg_PermissionsInternet");
					}
					break;

					case "LocalIntranet":
					{
						set = new NamedPermissionSet
							("LocalIntranet", PermissionState.None);
						set.Description = _("Arg_PermissionsLocalIntranet");
					}
					break;

					case "Nothing":
					{
						set = new NamedPermissionSet
							("Nothing", PermissionState.None);
						set.Description = _("Arg_PermissionsNothing");
					}
					break;

					case "SkipVerification":
					{
						set = new NamedPermissionSet
							("SkipVerification", PermissionState.None);
						set.Description = _("Arg_PermissionsSkipVerification");
						set.AddPermission(new SecurityPermission
								(SecurityPermissionFlag.SkipVerification));
					}
					break;
				}
				return set;
			}

	// Create a permission set object.
	public PermissionSet CreatePermissionSet()
			{
				PermissionSet set;
				SecurityElement element;
				StreamReader reader;
				String buf;

				if(Unrestricted)
				{
					set = new PermissionSet(PermissionState.Unrestricted);
				}
				else if(name != null)
				{
					set = CreateBuiltinPermissionSet(name);
				}
				else if(file != null)
				{
					// Parse the contents of a file.
					reader = new StreamReader(file);
					buf = reader.ReadToEnd();
					reader.Close();
					set = new PermissionSet(PermissionState.None);
					element = SecurityElement.Parse(buf);
					if(element != null)
					{
						set.FromXml(element);
					}
				}
				else if(xml != null)
				{
					// Parse the contents of a string.
					set = new PermissionSet(PermissionState.None);
					element = SecurityElement.Parse(xml);
					if(element != null)
					{
						set.FromXml(element);
					}
				}
				else
				{
					set = new PermissionSet(PermissionState.None);
				}
				return set;
			}

}; // class PermissionSetAttribute

#endif // CONFIG_PERMISSIONS && !ECMA_COMPAT

}; // namespace System.Security.Permissions
